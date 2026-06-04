// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using XrmFramework.DeployUtils.TableSync;
using XrmFramework.DeployUtils.Tests.TableSync.Fixtures;

namespace XrmFramework.DeployUtils.Tests.TableSync;

[TestFixture]
public class DefinitionAnalyzerTests
{
    /// <summary>
    /// L'assembly contenant nos fixtures locales. On l'utilise directement plutôt
    /// que de charger un .dll externe — l'API surface est la même.
    /// </summary>
    private static Assembly TestAssembly => typeof(TableSyncTestContactDefinition).Assembly;

    /// <summary>
    /// Filtre ne gardant que les définitions issues de nos fixtures (préfixées
    /// par "TableSyncTest"). Le projet de tests référence d'autres assemblies
    /// qui contiennent leurs propres [EntityDefinition], on les exclut ici.
    /// </summary>
    private static IReadOnlyList<DefinitionInfo> OurDefinitions()
        => DefinitionAnalyzer.ExtractDefinitions(TestAssembly)
            .Where(d => d.TableName.StartsWith("TableSyncTest"))
            .ToList();

    // ──────────────────────────────────────────────────────────────────────────
    // Cas nominal
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_FindsAllEntityDefinitionAttributedTypes()
    {
        var tableNames = OurDefinitions().Select(d => d.TableName).ToList();

        Assert.That(tableNames, Has.Member("TableSyncTestContact"));
        Assert.That(tableNames, Has.Member("TableSyncTestAccount"));
        Assert.That(tableNames, Has.Member("TableSyncTestNoSuffix"));
        Assert.That(tableNames, Has.Member("TableSyncTestEmpty"));
        Assert.That(tableNames, Has.Member("TableSyncTestNoCollection"));
    }

    [Test]
    public void ExtractDefinitions_StripsDefinitionSuffix_FromTableName()
    {
        var contact = GetOurDefinition("TableSyncTestContact");
        Assert.AreEqual("TableSyncTestContact", contact.TableName);
    }

    [Test]
    public void ExtractDefinitions_KeepsTypeName_WhenNoDefinitionSuffix()
    {
        var noSuffix = GetOurDefinition("TableSyncTestNoSuffix");
        // TableSyncTestNoSuffix ne se termine pas par "Definition" → nom conservé.
        Assert.AreEqual("TableSyncTestNoSuffix", noSuffix.TableName);
    }

    [Test]
    public void ExtractDefinitions_ExtractsEntityName()
    {
        var contact = GetOurDefinition("TableSyncTestContact");
        Assert.AreEqual("tabsync_contact", contact.EntityName);
    }

    [Test]
    public void ExtractDefinitions_ExtractsCollectionName_WhenPresent()
    {
        var contact = GetOurDefinition("TableSyncTestContact");
        Assert.AreEqual("tabsync_contacts", contact.EntityCollectionName);
    }

    [Test]
    public void ExtractDefinitions_CollectionNameNull_WhenFieldMissing()
    {
        var noColl = GetOurDefinition("TableSyncTestNoCollection");
        Assert.IsNull(noColl.EntityCollectionName);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Extraction des colonnes
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_ExtractsAllColumnsFromNestedColumnsClass()
    {
        var contact = GetOurDefinition("TableSyncTestContact");
        // ContactDefinition.Columns contient 4 constantes.
        Assert.AreEqual(4, contact.Columns.Count);
    }

    [Test]
    public void ExtractDefinitions_Column_LogicalNameAndNameAreSet()
    {
        var contact = GetOurDefinition("TableSyncTestContact");
        var idCol = contact.Columns.Single(c => c.Name == "Id");

        Assert.AreEqual("tabsync_contactid", idCol.LogicalName);
    }

    [Test]
    public void ExtractDefinitions_EmptyColumnsClass_YieldsZeroColumns()
    {
        var empty = GetOurDefinition("TableSyncTestEmpty");
        Assert.AreEqual(0, empty.Columns.Count);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Filtres : ignorer les classes qui ne sont pas des Definitions exploitables
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_IgnoresTypesWithoutEntityName()
    {
        // TableSyncTestIncompleteDefinition est décoré [EntityDefinition] mais n'a
        // pas de champ EntityName → l'analyzer doit le filtrer.
        Assert.IsFalse(OurDefinitions().Any(d => d.TableName == "TableSyncTestIncomplete"));
    }

    [Test]
    public void ExtractDefinitions_IgnoresTypesWithoutEntityDefinitionAttribute()
    {
        // TableSyncTestNotADefinition a un champ EntityName mais pas l'attribut.
        Assert.IsFalse(OurDefinitions().Any(d => d.EntityName == "tabsync_ghost"));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Détection du marqueur "généré par XrmFramework"
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_DetectsGeneratedCodeAttribute()
    {
        var account = GetOurDefinition("TableSyncTestAccount");
        Assert.IsTrue(account.IsFullyGenerated,
            "TableSyncTestAccountDefinition est décoré [GeneratedCode(\"XrmFramework\", \"2.0\")].");
    }

    [Test]
    public void ExtractDefinitions_ManuallyWrittenDefinition_IsNotFlaggedGenerated()
    {
        var contact = GetOurDefinition("TableSyncTestContact");
        Assert.IsFalse(contact.IsFullyGenerated);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Erreurs
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_FromMissingDllPath_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(
            () => DefinitionAnalyzer.ExtractDefinitions("/path/that/does/not/exist.dll"));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helper
    // ──────────────────────────────────────────────────────────────────────────

    private static DefinitionInfo GetOurDefinition(string tableName)
    {
        var found = OurDefinitions().SingleOrDefault(d => d.TableName == tableName);
        Assert.IsNotNull(found, $"Definition '{tableName}' introuvable dans les fixtures.");
        return found!;
    }
}
