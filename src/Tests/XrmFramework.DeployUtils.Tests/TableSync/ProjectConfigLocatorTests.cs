// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Découverte de la configuration en remontant l'arborescence : c'est ce qui permet de lancer le
/// CLI depuis n'importe quel sous-répertoire d'une solution consommatrice.
/// </summary>
[TestFixture]
public class ProjectConfigLocatorTests
{
    private string _root = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(),
            "XrmFramework.LocatorTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);
        }
        catch { /* meilleur effort */ }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Remontée d'arborescence
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Locate_FindsProjectRoot_FromDeeplyNestedDirectory()
    {
        BuildSolution();
        var deep = CreateDirectory("Sample.Plugins", "bin", "Debug", "net462");

        var location = ProjectConfigLocator.Locate(deep);

        Assert.IsNotNull(location);
        AssertSamePath(_root, location!.ProjectRoot);
    }

    [Test]
    public void Locate_FindsProjectRoot_WhenStartingAtRootItself()
    {
        BuildSolution();

        var location = ProjectConfigLocator.Locate(_root);

        Assert.IsNotNull(location);
        AssertSamePath(_root, location!.ProjectRoot);
    }

    [Test]
    public void Locate_ReturnsNull_WhenNoConfigAnywhereUpTheTree()
    {
        var orphan = CreateDirectory("sans-config");

        Assert.IsNull(ProjectConfigLocator.Locate(orphan));
    }

    [Test]
    public void Locate_SucceedsWithoutConnectionStringsFile()
    {
        // connectionStrings.config porte des secrets et est gitignoré dans les solutions générées :
        // il est donc absent d'un clone frais. La découverte ne doit pas en dépendre, sans quoi
        // l'utilisateur verrait « configuration introuvable » au lieu du vrai fichier manquant.
        BuildSolution(withConnectionStrings: false);

        var location = ProjectConfigLocator.Locate(CreateDirectory("Sample.Plugins"));

        Assert.IsNotNull(location);
        AssertSamePath(_root, location!.ProjectRoot);
    }

    [Test]
    public void Locate_StopsAtNearestRoot_WhenSolutionsAreNested()
    {
        BuildSolution();
        var innerRoot = CreateDirectory("Imbriquee");
        Directory.CreateDirectory(Path.Combine(innerRoot, "Config"));
        File.WriteAllText(Path.Combine(innerRoot, "Config", "xrmFramework.config"), "<xrmFramework />");

        var innerNested = Path.Combine(innerRoot, "Sous-dossier");
        Directory.CreateDirectory(innerNested);

        var location = ProjectConfigLocator.Locate(innerNested);

        Assert.IsNotNull(location);
        AssertSamePath(innerRoot, location!.ProjectRoot);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Déduction du répertoire Definitions
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Locate_DerivesTablesDirectory_FromCoreProjectNameProperty()
    {
        BuildSolution();

        var location = ProjectConfigLocator.Locate(_root);

        AssertSamePath(Path.Combine(_root, "Sample.Core", "Definitions"), location!.TablesDirectory);
    }

    [Test]
    public void Locate_DerivesTablesDirectory_EvenWhenDefinitionsFolderIsMissing()
    {
        // Une solution qui n'a encore récupéré aucune table n'a pas de dossier Definitions ;
        // la commande doit pouvoir le créer plutôt que d'exiger --tables-dir.
        BuildSolution(withDefinitionsFolder: false);

        var location = ProjectConfigLocator.Locate(_root);

        AssertSamePath(Path.Combine(_root, "Sample.Core", "Definitions"), location!.TablesDirectory);
    }

    [Test]
    public void Locate_ReturnsNullTablesDirectory_WhenPropsFileIsAbsent()
    {
        BuildSolution(withDirectoryBuildProps: false);

        var location = ProjectConfigLocator.Locate(_root);

        Assert.IsNotNull(location);
        Assert.IsNull(location!.TablesDirectory,
            "Sans Directory.Build.props, le répertoire doit être demandé explicitement.");
    }

    [Test]
    public void Locate_ReturnsNullTablesDirectory_WhenCoreProjectDoesNotExist()
    {
        // Propriété obsolète pointant un projet renommé ou supprimé : mieux vaut exiger
        // --tables-dir que d'écrire des .table dans un répertoire inventé.
        BuildSolution(coreProjectName: "Projet.Disparu", withDefinitionsFolder: false);
        Directory.Delete(Path.Combine(_root, "Projet.Disparu"), recursive: true);

        var location = ProjectConfigLocator.Locate(_root);

        Assert.IsNull(location!.TablesDirectory);
    }

    [Test]
    public void Locate_ReturnsNullTablesDirectory_WhenPropsFileIsMalformed()
    {
        BuildSolution(withDirectoryBuildProps: false);
        File.WriteAllText(Path.Combine(_root, "Directory.Build.props"), "<Project><PropertyGroup>");

        var location = ProjectConfigLocator.Locate(_root);

        Assert.IsNotNull(location, "Un props illisible ne doit pas faire échouer la découverte.");
        Assert.IsNull(location!.TablesDirectory);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Reproduit la structure d'une solution générée par le template XrmFramework.
    /// </summary>
    private void BuildSolution(
        string coreProjectName = "Sample.Core",
        bool withConnectionStrings = true,
        bool withDirectoryBuildProps = true,
        bool withDefinitionsFolder = true)
    {
        var configDir = Path.Combine(_root, "Config");
        Directory.CreateDirectory(configDir);

        File.WriteAllText(Path.Combine(configDir, "xrmFramework.config"),
            "<xrmFramework selectedConnection=\"Xrm\"><entitySolution name=\"Sample\" /></xrmFramework>");

        if (withConnectionStrings)
            File.WriteAllText(Path.Combine(configDir, "connectionStrings.config"), "<connectionStrings />");

        if (withDirectoryBuildProps)
            File.WriteAllText(Path.Combine(_root, "Directory.Build.props"),
                "<Project><PropertyGroup>" +
                $"<XrmFrameworkCoreProjectName>{coreProjectName}</XrmFrameworkCoreProjectName>" +
                "</PropertyGroup></Project>");

        Directory.CreateDirectory(Path.Combine(_root, coreProjectName));

        if (withDefinitionsFolder)
            Directory.CreateDirectory(Path.Combine(_root, coreProjectName, "Definitions"));

        Directory.CreateDirectory(Path.Combine(_root, "Sample.Plugins"));
    }

    private string CreateDirectory(params string[] segments)
    {
        var path = Path.Combine(new[] { _root }.Concat(segments).ToArray());
        Directory.CreateDirectory(path);
        return path;
    }

    private static void AssertSamePath(string expected, string actual)
        => Assert.AreEqual(
            Path.GetFullPath(expected).TrimEnd(Path.DirectorySeparatorChar),
            Path.GetFullPath(actual).TrimEnd(Path.DirectorySeparatorChar));
}
