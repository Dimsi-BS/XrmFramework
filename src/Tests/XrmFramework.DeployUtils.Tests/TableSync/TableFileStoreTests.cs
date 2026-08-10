// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Reading the selection carried by the <c>.table</c> directory.
/// </summary>
/// <remarks>
/// This is the list that <c>tables pull</c> retrieves when no table is explicitly
/// requested: it must reflect the content of the files, not their names.
/// </remarks>
[TestFixture]
public class TableFileStoreTests
{
    private string _tablesDir = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _tablesDir = Path.Combine(Path.GetTempPath(),
            "XrmFramework.TableFileStore_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tablesDir);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Directory.Exists(_tablesDir))
                Directory.Delete(_tablesDir, recursive: true);
        }
        catch { /* best effort */ }
    }

    [Test]
    public void Save_DoesNotWriteTheNamesADeclaredOneStandsIn_For()
    {
        // Key.EffectiveLogicalName and Key.MemberName read the two declared properties: writing
        // them out would add a field to every versioned .table, and make each one disagree with
        // itself the day a key is renamed.
        var table = new Table { LogicalName = "ftp_contrat", Name = "Contrat" };
        table.Keys.Add(new Key { LogicalName = "ftp_reference_key", Name = "Reference" });

        var path = Path.Combine(_tablesDir, "Contrat.table");
        TableFileStore.Save(path, table);

        var content = File.ReadAllText(path);

        Assert.That(content, Does.Not.Contain(nameof(Key.EffectiveLogicalName)));
        Assert.That(content, Does.Not.Contain(nameof(Key.MemberName)));
    }

    [Test]
    public void ReadTrackedLogicalNames_ReadsFileContent_NotFileName()
    {
        // A .table renamed by hand still tracks its entity.
        WriteTable("ContratLocation", "ftp_contrat");
        WriteTable("Account", "account");

        var tracked = TableFileStore.ReadTrackedLogicalNames(_tablesDir);

        Assert.That(tracked, Is.EquivalentTo(new[] { "ftp_contrat", "account" }));
    }

    [Test]
    public void ReadTrackedLogicalNames_ExcludesGlobalOptionSets()
    {
        WriteTable("Account", "account");
        WriteTable(TableFileStore.GlobalOptionSetFileName, TableFileStore.GlobalOptionSetLogicalName);

        var tracked = TableFileStore.ReadTrackedLogicalNames(_tablesDir);

        // The pseudo-table for global option sets does not correspond to any entity: requesting
        // it from the environment would only produce a "table not found" warning.
        Assert.That(tracked, Is.EquivalentTo(new[] { "account" }));
    }

    [Test]
    public void ReadTrackedLogicalNames_IgnoresUnreadableFile_AndKeepsTheOthers()
    {
        WriteTable("Account", "account");
        File.WriteAllText(Path.Combine(_tablesDir, "Corrompu.table"), "{ pas du JSON");

        var tracked = TableFileStore.ReadTrackedLogicalNames(_tablesDir);

        Assert.That(tracked, Is.EquivalentTo(new[] { "account" }));
    }

    [Test]
    public void ReadTrackedLogicalNames_IgnoresOtherFiles()
    {
        WriteTable("Account", "account");
        File.WriteAllText(Path.Combine(_tablesDir, "Account.cs"), "// generated");

        var tracked = TableFileStore.ReadTrackedLogicalNames(_tablesDir);

        Assert.That(tracked, Is.EquivalentTo(new[] { "account" }));
    }

    [Test]
    public void ReadTrackedLogicalNames_IsCaseInsensitive()
    {
        WriteTable("Account", "account");

        var tracked = TableFileStore.ReadTrackedLogicalNames(_tablesDir);

        // Names typed on the command line do not respect the CRM's casing.
        Assert.That(tracked.Contains("Account"), Is.True);
    }

    [Test]
    public void ReadTrackedLogicalNames_ReturnsEmpty_WhenDirectoryIsMissingOrEmpty()
    {
        Assert.That(TableFileStore.ReadTrackedLogicalNames(_tablesDir), Is.Empty);
        Assert.That(TableFileStore.ReadTrackedLogicalNames(Path.Combine(_tablesDir, "absent")), Is.Empty);
        Assert.That(TableFileStore.ReadTrackedLogicalNames(null), Is.Empty);
    }

    private void WriteTable(string fileName, string logicalName)
        => TableFileStore.Save(
            TableFileStore.BuildTableFilePath(_tablesDir, fileName),
            new Table { Name = fileName, LogicalName = logicalName });
}
