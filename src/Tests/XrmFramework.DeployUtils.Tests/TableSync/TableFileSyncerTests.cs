// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

[TestFixture]
public class TableFileSyncerTests
{
    private string _tempDir = string.Empty;

    [SetUp]
    public void SetUp()
    {
        // Directory isolated per test to avoid any cross-contamination.
        _tempDir = Path.Combine(Path.GetTempPath(),
            "XrmFramework.TableSyncTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }
        catch { /* best effort */ }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Construction
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Constructor_ThrowsWhenDirectoryDoesNotExist()
    {
        var ghostPath = Path.Combine(Path.GetTempPath(),
            "XrmFramework.TableSyncTests_ghost_" + Guid.NewGuid().ToString("N"));

        Assert.Throws<DirectoryNotFoundException>(
            () => new TableFileSyncer(ghostPath));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Creating a missing .table
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_CreatesNewTableFile_WhenMissing()
    {
        var def = ContactDef();
        var syncer = new TableFileSyncer(_tempDir);

        syncer.Sync(new[] { def });

        var path = Path.Combine(_tempDir, "Contact.table");
        Assert.IsTrue(File.Exists(path), "The .table file must be created.");
    }

    [Test]
    public void Sync_NewTable_ContainsAllColumnsWithSelectTrue()
    {
        var def = ContactDef();
        new TableFileSyncer(_tempDir).Sync(new[] { def });

        var table = LoadTable("Contact");
        Assert.AreEqual(2, table.Columns.Count);
        Assert.IsTrue(table.Columns.All(c => c.Selected),
            "All columns of a newly created table must be Selected.");
    }

    [Test]
    public void Sync_NewTable_SetsLogicalAndCollectionNames()
    {
        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() });

        var table = LoadTable("Contact");
        Assert.AreEqual("contact", table.LogicalName);
        Assert.AreEqual("Contact", table.Name);
        Assert.AreEqual("contacts", table.CollectionName);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Updating an existing .table
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_AddsMissingColumn_WhenAbsentFromExistingTable()
    {
        // Existing file containing only Id; the Definition adds Name.
        WriteTable("Contact", BuildTable("contact", "Contact", "contacts",
            new Column { LogicalName = "contactid", Name = "Id", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() });

        var table = LoadTable("Contact");
        Assert.AreEqual(2, table.Columns.Count);
        Assert.IsTrue(table.Columns.Any(c => c.LogicalName == "fullname" && c.Selected));
    }

    [Test]
    public void Sync_SetsSelectTrue_WhenColumnExistsButIsDeselected()
    {
        WriteTable("Contact", BuildTable("contact", "Contact", "contacts",
            new Column { LogicalName = "contactid", Name = "Id", Selected = false },
            new Column { LogicalName = "fullname", Name = "FullName", Selected = false }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() });

        var table = LoadTable("Contact");
        Assert.IsTrue(table.Columns.All(c => c.Selected),
            "Columns present in the Definition must switch to Selected=true.");
    }

    [Test]
    public void Sync_PreservesExistingMetadata_OnAlreadySelectedColumn()
    {
        WriteTable("Contact", BuildTable("contact", "Contact", "contacts",
            new Column
            {
                LogicalName = "contactid",
                Name = "Id",
                Type = AttributeTypeCode.Uniqueidentifier,
                Selected = true,
                Labels = { new LocalizedLabel { Label = "Contact", LangId = 1036 } }
            }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() });

        var idCol = LoadTable("Contact").Columns.Single(c => c.LogicalName == "contactid");
        Assert.AreEqual(AttributeTypeCode.Uniqueidentifier, idCol.Type);
        Assert.AreEqual(1, idCol.Labels.Count, "CRM Labels must not be lost.");
    }

    [Test]
    public void Sync_WithoutClean_DoesNotTouchColumnsAbsentFromDefinition()
    {
        // Column "etrangere" present in the file but not in the Definition.
        WriteTable("Contact", BuildTable("contact", "Contact", "contacts",
            new Column { LogicalName = "contactid", Name = "Id", Selected = true },
            new Column { LogicalName = "etrangere", Name = "Etrangere", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: false);

        var orphan = LoadTable("Contact").Columns.Single(c => c.LogicalName == "etrangere");
        Assert.IsTrue(orphan.Selected,
            "Without --clean, columns outside the Definition must be left as-is.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // --clean mode: orphaned columns in a managed table
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_Clean_DeselectsColumns_AbsentFromDefinition_InManagedTable()
    {
        WriteTable("Contact", BuildTable("contact", "Contact", "contacts",
            new Column { LogicalName = "contactid", Name = "Id", Selected = true },
            new Column { LogicalName = "obsolete", Name = "Obsolete", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: true);

        var table = LoadTable("Contact");
        Assert.IsTrue(table.Columns.Single(c => c.LogicalName == "contactid").Selected);
        Assert.IsFalse(table.Columns.Single(c => c.LogicalName == "obsolete").Selected,
            "With --clean, columns outside the Definition must be deselected.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // --clean mode: orphaned files (without a matching Definition)
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_Clean_DeletesOrphanedFile_WithoutLabels()
    {
        // File without any column carrying Labels → considered entirely
        // produced by TableSyncHelper, hence removable.
        WriteTable("Ghost", BuildTable("ghost", "Ghost", "ghosts",
            new Column { LogicalName = "ghostid", Name = "Id", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: true);

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "Ghost.table")),
            "The orphaned .table without Labels must be deleted.");
    }

    [Test]
    public void Sync_Clean_KeepsOrphanedFile_WithLabels()
    {
        // File with at least one column carrying Labels → real CRM data.
        // Must be kept even if no Definition references it anymore.
        WriteTable("Legacy", BuildTable("legacy", "Legacy", "legacies",
            new Column
            {
                LogicalName = "legacyid",
                Name = "Id",
                Selected = true,
                Labels = { new LocalizedLabel { Label = "Legacy", LangId = 1036 } }
            }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: true);

        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "Legacy.table")),
            "An orphaned .table containing CRM Labels must be kept.");
    }

    [Test]
    public void Sync_Clean_DeselectsAllColumns_InKeptOrphanedFile()
    {
        WriteTable("Legacy", BuildTable("legacy", "Legacy", "legacies",
            new Column
            {
                LogicalName = "legacyid",
                Name = "Id",
                Selected = true,
                Labels = { new LocalizedLabel { Label = "Legacy", LangId = 1036 } }
            },
            new Column
            {
                LogicalName = "legacyname",
                Name = "LegacyName",
                Selected = true,
                Labels = { new LocalizedLabel { Label = "Nom", LangId = 1036 } }
            }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: true);

        var table = LoadTable("Legacy");
        Assert.IsTrue(table.Columns.All(c => !c.Selected),
            "All columns of a kept orphaned .table must be deselected.");
    }

    [Test]
    public void Sync_WithoutClean_PreservesOrphanedFiles_Untouched()
    {
        WriteTable("Ghost", BuildTable("ghost", "Ghost", "ghosts",
            new Column { LogicalName = "ghostid", Name = "Id", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: false);

        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "Ghost.table")),
            "Without --clean, orphaned files must not be deleted.");
        Assert.IsTrue(LoadTable("Ghost").Columns.Single().Selected,
            "Without --clean, orphaned columns must not be deselected.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Tables shipped by the framework (SystemUser, Role, ...)
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_DoesNotCreateFile_ForFrameworkTable()
    {
        new TableFileSyncer(_tempDir).Sync(new[] { SystemUserDef() });

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "SystemUser.table")),
            "A table shipped by the framework must not be recreated in the project: " +
            "its .table is already part of the XrmFramework package.");
    }

    [Test]
    public void Sync_UpdatesFrameworkTable_WhenProjectAlreadyTracksIt()
    {
        // The project tracks its own copy of SystemUser to add its own columns to it:
        // the table then becomes a table like any other.
        WriteTable("SystemUser", BuildTable("systemuser", "SystemUser", "systemusers",
            new Column { LogicalName = "systemuserid", Name = "Id", IsLocked = true, Selected = false }));

        new TableFileSyncer(_tempDir).Sync(new[] { SystemUserDef() });

        var table = LoadTable("SystemUser");
        Assert.IsTrue(table.Columns.Single(c => c.LogicalName == "systemuserid").Selected,
            "A column referenced by the code must be activated, even on a framework table.");
        Assert.IsTrue(table.Columns.Any(c => c.LogicalName == "fullname" && c.Selected),
            "Missing columns must be added just like on any other table.");
    }

    [Test]
    public void Sync_PreservesLockedMarker_OnFrameworkColumn()
    {
        // "Locked" identifies columns brought in by the framework on a table that the
        // project extends: synchronization must not erase it.
        WriteTable("SystemUser", BuildTable("systemuser", "SystemUser", "systemusers",
            new Column { LogicalName = "systemuserid", Name = "Id", IsLocked = true, Selected = false }));

        new TableFileSyncer(_tempDir).Sync(new[] { SystemUserDef() });

        Assert.IsTrue(LoadTable("SystemUser").Columns.Single(c => c.LogicalName == "systemuserid").IsLocked,
            "The framework's Locked marker must survive synchronization.");
    }

    [Test]
    public void Sync_SkipsFrameworkTable_IdentifiedByLogicalName()
    {
        // Definition bearing a different class name but targeting the framework's entity:
        // the logical name is what decides.
        var renamed = new DefinitionInfo
        {
            TableName = "Utilisateur",
            EntityName = "systemuser",
            EntityCollectionName = "systemusers",
            Columns = new List<DefinitionColumnInfo> { new("systemuserid", "Id") }
        };

        new TableFileSyncer(_tempDir).Sync(new[] { renamed });

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "Utilisateur.table")));
    }

    [Test]
    public void Sync_StillProcessesProjectTables_WhenFrameworkTablesArePresent()
    {
        new TableFileSyncer(_tempDir).Sync(new[] { SystemUserDef(), ContactDef() });

        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "Contact.table")),
            "Filtering out framework tables must not affect the project's own tables.");
    }

    [Test]
    public void Sync_Clean_TreatsTrackedFrameworkTableLikeAnyOther()
    {
        // Framework table tracked by the project: --clean deselects columns that no
        // Definition references anymore, just like everywhere else.
        WriteTable("SystemUser", BuildTable("systemuser", "SystemUser", "systemusers",
            new Column { LogicalName = "systemuserid", Name = "Id", Selected = true },
            new Column { LogicalName = "obsolete", Name = "Obsolete", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { SystemUserDef() }, clean: true);

        var table = LoadTable("SystemUser");
        Assert.IsTrue(table.Columns.Single(c => c.LogicalName == "systemuserid").Selected);
        Assert.IsFalse(table.Columns.Single(c => c.LogicalName == "obsolete").Selected);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    private static DefinitionInfo ContactDef() => new()
    {
        TableName = "Contact",
        EntityName = "contact",
        EntityCollectionName = "contacts",
        Columns = new List<DefinitionColumnInfo>
        {
            new("contactid", "Id"),
            new("fullname",  "FullName")
        }
    };

    /// <summary>
    /// Definition of a table shipped by the framework: it appears in the consuming project's
    /// DLL because the XrmFramework package's .table is compiled into it.
    /// </summary>
    private static DefinitionInfo SystemUserDef() => new()
    {
        TableName = "SystemUser",
        EntityName = "systemuser",
        EntityCollectionName = "systemusers",
        Columns = new List<DefinitionColumnInfo>
        {
            new("systemuserid", "Id"),
            new("fullname",     "FullName")
        }
    };

    private static Table BuildTable(string logicalName, string name, string collName,
                                    params Column[] columns)
    {
        var t = new Table
        {
            LogicalName = logicalName,
            Name = name,
            CollectionName = collName
        };
        foreach (var c in columns)
            t.Columns.Add(c);
        return t;
    }

    private void WriteTable(string tableName, Table table)
    {
        var json = JsonConvert.SerializeObject(table, Formatting.Indented,
            new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
        File.WriteAllText(Path.Combine(_tempDir, tableName + ".table"), json);
    }

    private Table LoadTable(string tableName)
    {
        var json = File.ReadAllText(Path.Combine(_tempDir, tableName + ".table"));
        return JsonConvert.DeserializeObject<Table>(json)!;
    }
}
