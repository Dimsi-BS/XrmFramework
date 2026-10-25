// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Importers;

namespace XrmFramework.DeployUtils.Configuration;

/// <summary>
///     Outcome of resolving <see cref="DeploymentMode.Auto" /> against what the build actually
///     produced.
/// </summary>
public sealed class DeploymentModeResolution
{
	public DeploymentModeResolution(DeploymentMode mode, string reason, string packagePath = null)
	{
		Mode = mode;
		Reason = reason;
		PackagePath = packagePath;
	}

	/// <summary>The concrete mode to deploy with. Never <see cref="DeploymentMode.Auto" />.</summary>
	public DeploymentMode Mode { get; }

	/// <summary>Short explanation, shown in the deployment summary so the choice is never a guess.</summary>
	public string Reason { get; }

	/// <summary>Package to push, when <see cref="Mode" /> is <see cref="DeploymentMode.Package" />.</summary>
	public string PackagePath { get; }
}

/// <summary>
///     Decides whether an assembly is deployed on its own or wrapped in a plugin package.
///     <para>
///         The build already knows: <c>XrmFrameworkPluginPackage</c> makes it produce a
///         <c>.nupkg</c> and leave the assembly unsigned. <see cref="DeploymentMode.Auto" /> reads
///         that back instead of asking for the same thing twice, in the project and in
///         <c>xrmFramework.config</c>, where the two can drift apart.
///     </para>
///     <para>
///         The presence of a <c>.nupkg</c> alone would be too weak a signal for this decision.
///         Switching an already-deployed project between the two shapes is not a redeploy: the old
///         <c>pluginassembly</c> has to go, which takes its plugin types and steps with it, so the
///         step ids change. A stale file left in <c>bin</c> must not be able to trigger that, hence
///         the two corroborating signals below and the refusal to guess when they disagree.
///     </para>
/// </summary>
public static class DeploymentModeResolver
{
	public static DeploymentModeResolution Resolve(DeploymentMode declared, string dllPath,
		string explicitPackagePath = null)
	{
		if (string.IsNullOrWhiteSpace(dllPath)) throw new ArgumentNullException(nameof(dllPath));

		var packagePath = string.IsNullOrWhiteSpace(explicitPackagePath)
			? AssemblyImporter.ResolvePackagePath(dllPath)
			: explicitPackagePath;

		switch (declared)
		{
			// Declared explicitly: the configuration decides, and a missing package is reported by
			// the deployment itself with instructions on how to produce one.
			case DeploymentMode.Package:
				return new DeploymentModeResolution(DeploymentMode.Package,
					"declared in xrmFramework.config", packagePath);

			case DeploymentMode.Assembly:
				// Worth saying out loud: the project looks built for a package while the
				// configuration asks for a bare assembly, which Dataverse will reject for being
				// unsigned. Not an error - the configuration is allowed to be the authority.
				return new DeploymentModeResolution(DeploymentMode.Assembly,
					packagePath != null && !IsSigned(dllPath)
						? "declared in xrmFramework.config, although the build produced a package and left the assembly unsigned"
						: "declared in xrmFramework.config");

			case DeploymentMode.Auto:
				return ResolveAuto(dllPath, packagePath, explicitPackagePath != null);

			default:
				throw new ArgumentOutOfRangeException(nameof(declared), declared, "Unknown deployment mode.");
		}
	}

	private static DeploymentModeResolution ResolveAuto(string dllPath, string packagePath,
		bool packageWasGivenExplicitly)
	{
		// Asking for a specific package is asking for package mode.
		if (packageWasGivenExplicitly)
			return new DeploymentModeResolution(DeploymentMode.Package, "--package was given", packagePath);

		if (packagePath == null)
			return new DeploymentModeResolution(DeploymentMode.Assembly,
				"auto-detected: the build produced no plugin package");

		var signed = IsSigned(dllPath);

		// A signed assembly is what a bare registration needs and what package mode deliberately
		// avoids producing. Signed plus a package means the package is almost certainly left over
		// from an earlier build, so the safe reading is "assembly", not "silently change shape".
		if (signed)
			return new DeploymentModeResolution(DeploymentMode.Assembly,
				$"auto-detected: the assembly is signed, so '{Path.GetFileName(packagePath)}' is ignored as a leftover - set deployment=\"Package\" to use it anyway");

		// Unsigned and packaged, but the package predates the assembly: the last build did not
		// refresh it, so pushing it would deploy code that is not the code just built.
		if (File.GetLastWriteTimeUtc(packagePath) < File.GetLastWriteTimeUtc(dllPath))
			throw new InvalidOperationException(
				$"The plugin package '{Path.GetFileName(packagePath)}' is older than the assembly it should contain, " +
				"so deploying it would push code that is not what was just built. Rebuild the project - or delete the " +
				"stale package and set deployment=\"Assembly\" if this project is no longer deployed as a package.");

		return new DeploymentModeResolution(DeploymentMode.Package,
			"auto-detected: the build produced a plugin package and left the assembly unsigned", packagePath);
	}

	/// <summary>
	///     Reads the strong name from the assembly's metadata, without loading it into the runtime -
	///     the deployment runs on net8/net10 and the assembly is .NET Framework.
	/// </summary>
	private static bool IsSigned(string dllPath)
	{
		try
		{
			var token = AssemblyName.GetAssemblyName(Path.GetFullPath(dllPath)).GetPublicKeyToken();
			return token != null && token.Length > 0;
		}
		catch (Exception)
		{
			// Unreadable identity is not a reason to pick a deployment shape; treat it as unsigned
			// and let the rest of the pipeline report the real problem with the file.
			return false;
		}
	}
}
