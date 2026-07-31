// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.DeployUtils.TableSync;
using XrmFramework.DeployUtils.Tests.TableSync.Fixtures;

namespace XrmFramework.DeployUtils.Tests.TableSync;

[TestFixture]
public class DefinitionAnalyzerTests
{
    /// <summary>
    /// The assembly containing our local fixtures. It is used directly rather
    /// than loading an external .dll — the API surface is the same.
    /// </summary>
    private static Assembly TestAssembly => typeof(TableSyncTestContactDefinition).Assembly;

    /// <summary>
    /// Filter keeping only the definitions coming from our fixtures (prefixed
    /// with "TableSyncTest"). The test project references other assemblies
    /// that contain their own [EntityDefinition], which are excluded here.
    /// </summary>
    private static IReadOnlyList<DefinitionInfo> OurDefinitions()
        => DefinitionAnalyzer.ExtractDefinitions(TestAssembly)
            .Where(d => d.TableName.StartsWith("TableSyncTest"))
            .ToList();

    // ──────────────────────────────────────────────────────────────────────────
    // Nominal case
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
        // TableSyncTestNoSuffix does not end with "Definition" -> name kept as-is.
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
    // Column extraction
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_ExtractsAllColumnsFromNestedColumnsClass()
    {
        var contact = GetOurDefinition("TableSyncTestContact");
        // ContactDefinition.Columns contains 4 constants.
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
    // Option set names carried by [OptionSet(typeof(...))]
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_ExtractsOptionSetName_FromColumnAttribute()
    {
        var def = GetOurDefinition("TableSyncTestOptionSet");
        var status = def.Columns.Single(c => c.Name == "StatusCode");

        Assert.AreEqual("TableSyncTestRenamedStatus", status.OptionSetName,
            "The enum's C# name is what compiled code depends on: it has to come out of the assembly.");
    }

    [Test]
    public void ExtractDefinitions_OptionSetNameIsNull_WhenColumnHasNoOptionSet()
    {
        var def = GetOurDefinition("TableSyncTestOptionSet");

        Assert.IsNull(def.Columns.Single(c => c.Name == "Name").OptionSetName);
        Assert.IsNull(def.Columns.Single(c => c.Name == "Id").OptionSetName);
    }

    [Test]
    public void ExtractDefinitions_ExtractsOptionSetMembers_WithTheirValues()
    {
        var status = GetOurDefinition("TableSyncTestOptionSet").Columns.Single(c => c.Name == "StatusCode");

        var members = status.OptionSetValues.Select(v => $"{v.Value}:{v.Name}").ToList();

        CollectionAssert.AreEqual(new[] { "1:EnCours", "2:Termine", "3:Modele" }, members,
            "Member names are what MyEnum.EnCours compiles against; the value is the CRM key.");
    }

    [Test]
    public void ExtractDefinitions_KeepsMembersInDeclarationOrder()
    {
        var global = GetOurDefinition("TableSyncTestOptionSet").Columns.Single(c => c.Name == "GlobalPick");

        Assert.AreEqual("Null", global.OptionSetValues[0].Name,
            "The synthetic Null member comes first: order is what tells it apart from a real option 0.");
    }

    [Test]
    public void ExtractDefinitions_OptionSetValuesIsEmpty_WhenColumnHasNoOptionSet()
    {
        var def = GetOurDefinition("TableSyncTestOptionSet");

        CollectionAssert.IsEmpty(def.Columns.Single(c => c.Name == "Name").OptionSetValues);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Filters: ignoring classes that are not usable Definitions
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_IgnoresTypesWithoutEntityName()
    {
        // TableSyncTestIncompleteDefinition is decorated with [EntityDefinition] but has
        // no EntityName field -> the analyzer must filter it out.
        Assert.IsFalse(OurDefinitions().Any(d => d.TableName == "TableSyncTestIncomplete"));
    }

    [Test]
    public void ExtractDefinitions_IgnoresTypesWithoutEntityDefinitionAttribute()
    {
        // TableSyncTestNotADefinition has an EntityName field but not the attribute.
        Assert.IsFalse(OurDefinitions().Any(d => d.EntityName == "tabsync_ghost"));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Detecting the "generated by XrmFramework" marker
    // ──────────────────────────────────────────────────────────────────────────

    [Test]
    public void ExtractDefinitions_DetectsGeneratedCodeAttribute()
    {
        var account = GetOurDefinition("TableSyncTestAccount");
        Assert.IsTrue(account.IsFullyGenerated,
            "TableSyncTestAccountDefinition is decorated with [GeneratedCode(\"XrmFramework\", \"2.0\")].");
    }

    [Test]
    public void ExtractDefinitions_ManuallyWrittenDefinition_IsNotFlaggedGenerated()
    {
        var contact = GetOurDefinition("TableSyncTestContact");
        Assert.IsFalse(contact.IsFullyGenerated);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Errors
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
        Assert.IsNotNull(found, $"Definition '{tableName}' not found among the fixtures.");
        return found!;
    }
}
