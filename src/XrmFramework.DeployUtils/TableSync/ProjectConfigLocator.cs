// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XrmFramework.DeployUtils.TableSync;

/// <summary>
/// Emplacements résolus pour un projet consommateur du framework.
/// </summary>
public sealed class ProjectConfigLocation
{
    internal ProjectConfigLocation(string projectRoot, string tablesDirectory)
    {
        ProjectRoot = projectRoot;
        TablesDirectory = tablesDirectory;
    }

    /// <summary>
    /// Racine de la solution : le dossier contenant <c>Config/xrmFramework.config</c>.
    /// C'est la valeur à passer à <see cref="Configuration.ConfigHelper.UseProjectConfig" />.
    /// </summary>
    public string ProjectRoot { get; }

    /// <summary>
    /// Répertoire <c>Definitions</c> du projet Core, ou <see langword="null" /> s'il n'a pas pu
    /// être déduit — auquel cas l'appelant doit exiger une option explicite.
    /// </summary>
    public string TablesDirectory { get; }
}

/// <summary>
/// Localise la configuration XrmFramework d'un projet en remontant l'arborescence depuis le
/// répertoire courant, afin que le CLI puisse être lancé depuis n'importe quel sous-répertoire
/// de la solution consommatrice.
/// </summary>
/// <remarks>
/// Reproduit sans MSBuild la résolution utilisée par le DefinitionManager, qui reçoit
/// <c>$(RootFolder)/$(XrmFrameworkCoreProjectName)</c> via un attribut d'assembly injecté à la
/// compilation (cf. <c>XrmFramework.DefinitionManager.props</c>).
/// </remarks>
public static class ProjectConfigLocator
{
    private const string ConfigDirectoryName = "Config";
    private const string XrmFrameworkConfigFileName = "xrmFramework.config";
    private const string DirectoryBuildPropsFileName = "Directory.Build.props";
    private const string CoreProjectNamePropertyName = "XrmFrameworkCoreProjectName";
    private const string DefinitionsDirectoryName = "Definitions";

    /// <summary>
    /// Remonte depuis <paramref name="startDirectory" /> jusqu'à trouver la racine de solution.
    /// </summary>
    /// <returns>
    /// Les emplacements résolus, ou <see langword="null" /> si aucune configuration n'a été
    /// trouvée jusqu'à la racine du volume.
    /// </returns>
    public static ProjectConfigLocation Locate(string startDirectory)
    {
        if (string.IsNullOrWhiteSpace(startDirectory))
            throw new ArgumentException("Le répertoire de départ est obligatoire.", nameof(startDirectory));

        var directory = new DirectoryInfo(Path.GetFullPath(startDirectory));

        while (directory != null)
        {
            // La découverte ne teste que xrmFramework.config : connectionStrings.config contient
            // des secrets et se trouve gitignoré dans les solutions générées, donc absent d'un
            // clone frais. L'exiger ici ferait remonter « configuration introuvable » au lieu de
            // laisser ConfigHelper signaler précisément le fichier manquant.
            if (File.Exists(Path.Combine(directory.FullName, ConfigDirectoryName, XrmFrameworkConfigFileName)))
                return new ProjectConfigLocation(directory.FullName, ResolveTablesDirectory(directory.FullName));

            directory = directory.Parent;
        }

        return null;
    }

    /// <summary>
    /// Déduit le répertoire <c>Definitions</c> à partir de la propriété MSBuild
    /// <c>XrmFrameworkCoreProjectName</c> déclarée dans le <c>Directory.Build.props</c> racine.
    /// </summary>
    /// <returns>Le chemin, ou <see langword="null" /> si la déduction échoue.</returns>
    private static string ResolveTablesDirectory(string projectRoot)
    {
        var coreProjectName = ReadCoreProjectName(Path.Combine(projectRoot, DirectoryBuildPropsFileName));

        if (string.IsNullOrWhiteSpace(coreProjectName))
            return null;

        var coreProjectDirectory = Path.Combine(projectRoot, coreProjectName.Trim());

        // Le dossier Definitions peut ne pas exister encore (aucune table récupérée à ce jour) ;
        // en revanche, un projet Core absent signale une propriété obsolète qu'il vaut mieux ne
        // pas suivre silencieusement.
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

            // Les fichiers MSBuild du dépôt sont écrits sans espace de noms ; on compare donc
            // sur le nom local pour rester tolérant si un xmlns venait à être ajouté.
            return root?.Descendants()
                .Where(e => e.Name.LocalName == CoreProjectNamePropertyName)
                .Select(e => e.Value)
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
        }
        catch (System.Xml.XmlException)
        {
            // Un Directory.Build.props illisible ne doit pas faire échouer la commande :
            // l'appelant retombe sur l'option explicite.
            return null;
        }
    }
}
