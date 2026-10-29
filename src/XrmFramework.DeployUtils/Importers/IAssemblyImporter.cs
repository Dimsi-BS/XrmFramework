using System;
using System.Collections.Generic;
using System.Reflection;
using Deploy;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using CustomApi = XrmFramework.DeployUtils.Model.CustomApi;
using CustomApiRequestParameter = Deploy.CustomApiRequestParameter;
using CustomApiResponseProperty = Deploy.CustomApiResponseProperty;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Importers;

/// <summary>
///     Imports Components from <see cref="System" /> or <see cref="Deploy" /> to <see cref="ICrmComponent" />
/// </summary>
internal interface IAssemblyImporter
{
	/// <summary>
	///     Creates an <see cref="AssemblyInfo" /> from a local assembly file path (metadata only,
	///     without loading the assembly into the runtime).
	/// </summary>
	/// <param name="dllPath">Path to the local plugin assembly.</param>
	/// <returns>
	///     <see cref="AssemblyInfo" />
	/// </returns>
	AssemblyInfo CreateAssemblyFromLocal(string dllPath);

	/// <summary>
	///     Creates a <see cref="IAssemblyContext" /> with only the <see cref="AssemblyInfo" /> Property computed from a
	///     <see cref="Deploy.PluginAssembly" />
	/// </summary>
	/// <param name="assembly"></param>
	/// <returns>
	///     <see cref="IAssemblyContext" />
	/// </returns>
	IAssemblyContext CreateAssemblyFromRemote(PluginAssembly assembly);

	/// <summary>
	///     Creates a <see cref="IAssemblyContext" /> with only the <see cref="AssemblyInfo" /> Property computed from a
	///     remote <see cref="AssemblyInfo" />
	/// </summary>
	/// <param name="assemblyInfo"></param>
	/// <returns>
	///     <see cref="IAssemblyContext" />, or <see langword="null" /> when <paramref name="assemblyInfo" /> is
	///     <see langword="null" />, meaning the assembly is not registered yet
	/// </returns>
	IAssemblyContext CreateAssemblyFromRemote(AssemblyInfo assemblyInfo);

	/// <summary>
	///     Creates a <see cref="PluginPackage" /> wrapping <paramref name="assembly" /> from the
	///     <c>.nupkg</c> at <paramref name="packagePath" />.
	/// </summary>
	/// <param name="assembly">The local assembly the package was built from.</param>
	/// <param name="packagePath">
	///     Path of the <c>.nupkg</c> to read, as resolved by
	///     <see cref="AssemblyImporter.ResolvePackagePath" /> or given explicitly on the command line.
	/// </param>
	PluginPackage CreatePackageFromLocal(AssemblyInfo assembly, string packagePath);

	/// <summary>
	///     Creates a <see cref="PluginPackage" /> from the remote
	/// </summary>
	/// <param name="package"></param>
	/// <returns></returns>
	PluginPackage CreatePackageFromRemote(Deploy.PluginPackage package);

	/// <summary>
	///     Creates a <see cref="Step" /> from a remote Assembly <see cref="Deploy.SdkMessageProcessingStep" />
	///     and the <see cref="Deploy.SdkMessageProcessingStepImage" /> registered on the remote Assembly
	/// </summary>
	/// <param name="sdkStep"></param>
	/// <param name="sdkImages"></param>
	/// <param name="step"></param>
	/// <returns></returns>
	bool TryCreateStepFromRemote(SdkMessageProcessingStep sdkStep, IEnumerable<SdkMessageProcessingStepImage> sdkImages, out Step step);

	/// <summary>
	///     Creates a <see cref="Plugin" /> from a remote Assembly <see cref="Deploy.PluginType" />
	///     and the <see cref="Step" /> already parsed
	/// </summary>
	/// <param name="pluginType"></param>
	/// <param name="steps"></param>
	/// <returns></returns>
	Plugin CreatePluginFromRemote(PluginType pluginType, IEnumerable<Step> steps);

	/// <summary>
	///     Creates a <see cref="CustomApi" /> from a remote Assembly <see cref="Deploy.CustomApi" />,
	///     the <see cref="Deploy.CustomApiRequestParameter" />
	///     and the <see cref="Deploy.CustomApiResponseProperty" /> registered on the remote Assembly
	/// </summary>
	/// <param name="customApi"></param>
	/// <param name="requestParameters"></param>
	/// <param name="responseProperties"></param>
	/// <returns></returns>
	CustomApi CreateCustomApiFromRemote(Deploy.CustomApi customApi,
		IEnumerable<CustomApiRequestParameter> requestParameters,
		IEnumerable<CustomApiResponseProperty> responseProperties);
}
