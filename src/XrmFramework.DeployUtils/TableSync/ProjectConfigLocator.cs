// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XrmFramework.DeployUtils.TableSync;

/// <summary>
/// Locations resolved for a consumer project of the framework.
/// </summary>
public sealed class ProjectConfigLocation
{
    internal ProjectConfigLocation(string projectRoot, string tablesDirectory)
    {
        ProjectRoot = projectRoot;
        TablesDirectory = tablesDirectory;
    }

    /// <summary>
    /// Solution root: the folder containing <c>Config/xrmFramework.config</c>.
    /// This is the value to pass to <see cref="Configuration.ConfigHelper.UseProjectConfig" />.
    /// </summary>
    public string ProjectRoot { get; }

    /// <summary>
    /// <c>Definitions</c> directory of the Core project, or <see langword="null" /> if it could not
    /// be inferred — in which case the caller must require an explicit option.
    /// </summary>
    public string TablesDirectory { get; }
}

/// <summary>
/// Locates a project's XrmFramework configuration by walking up the directory tree from the
/// current directory, so that the CLI can be launched from any subdirectory
/// of the consumer solution.
/// </summary>
/// <remarks>
/// Reproduces without MSBuild the resolution used by the DefinitionManager, which receives
/// <c>$(RootFolder)/$(XrmFrameworkCoreProjectName)</c> via an assembly attribute injected at
/// compile time (see <c>XrmFramework.DefinitionManager.props</c>).
/// </remarks>
public static class ProjectConfigLocator
{
    private const string ConfigDirectoryName = "Config";
    private const string XrmFrameworkConfigFileName = "xrmFramework.config";
    private const string DirectoryBuildPropsFileName = "Directory.Build.props";
    private const string CoreProjectNamePropertyName = "XrmFrameworkCoreProjectName";
    private const string DefinitionsDirectoryName = "Definitions";

    /// <summary>
    /// Walks up from <paramref name="startDirectory" /> until it finds the solution root.
    /// </summary>
    /// <returns>
    /// The resolved locations, or <see langword="null" /> if no configuration was
    /// found up to the volume root.
    /// </returns>
    public static ProjectConfigLocation Locate(string startDirectory)
    {
        if (string.IsNullOrWhiteSpace(startDirectory))
            throw new ArgumentException("The starting directory is required.", nameof(startDirectory));

        var directory = new DirectoryInfo(Path.GetFullPath(startDirectory));

        while (directory != null)
        {
            // Discovery only tests for xrmFramework.config: connectionStrings.config contains
            // secrets and is gitignored in generated solutions, so it is absent from a
            // fresh clone. Requiring it here would surface "configuration not found" instead of
            // letting ConfigHelper precisely report the missing file.
            if (File.Exists(Path.Combine(directory.FullName, ConfigDirectoryName, XrmFrameworkConfigFileName)))
                return new ProjectConfigLocation(directory.FullName, ResolveTablesDirectory(directory.FullName));

            directory = directory.Parent;
        }

        return null;
    }

    /// <summary>
    /// Infers the <c>Definitions</c> directory from the MSBuild property
    /// <c>XrmFrameworkCoreProjectName</c> declared in the root <c>Directory.Build.props</c>.
    /// </summary>
    /// <returns>The path, or <see langword="null" /> if inference fails.</returns>
    private static string ResolveTablesDirectory(string projectRoot)
    {
        var coreProjectName = ReadCoreProjectName(Path.Combine(projectRoot, DirectoryBuildPropsFileName));

        if (string.IsNullOrWhiteSpace(coreProjectName))
            return null;

        var coreProjectDirectory = Path.Combine(projectRoot, coreProjectName.Trim());

        // The Definitions folder may not exist yet (no table retrieved so far);
        // however, a missing Core project signals a stale property that is better not
        // followed silently.
        if (!Directory.Exists(coreProjectDirectory))
            return null;

        return Path.Combine(coreProjectDirectory, DefinitionsDirectoryName);
    }

    private static string ReadCoreProjectName(string propsPath)
    {
        if (!File.Exists(propsPath))
            return null;

        try
        {
            var root = XDocument.Load(propsPath).Root;

            // The repository's MSBuild files are written without a namespace; comparison is
            // therefore done on the local name to remain tolerant if an xmlns were ever added.
            return root?.Descendants()
                .Where(e => e.Name.LocalName == CoreProjectNamePropertyName)
                .Select(e => e.Value)
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
        }
        catch (System.Xml.XmlException)
        {
            // An unreadable Directory.Build.props must not fail the command:
            // the caller falls back to the explicit option.
            return null;
        }
    }
}
