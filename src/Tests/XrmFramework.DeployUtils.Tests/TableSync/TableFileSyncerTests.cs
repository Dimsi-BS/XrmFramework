// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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

    [Test]
    public void Sync_TakesTwoTables_WhoseNamesDifferByCaseAlone()
    {
        // Two distinct CRM tables whose C# names collide once case is set aside. The index of
        // selected columns is keyed on that name and grouped case-insensitively, as the dictionary
        // it builds is: grouping case-sensitively instead produced two keys the dictionary then
        // rejected as one duplicate, aborting the sync on an ArgumentException.
        var upper = ContactDef();

        var lower = SystemUserDef();
        lower.TableName = "contact";

        Assert.DoesNotThrow(
            () => new TableFileSyncer(_tempDir).Sync(new[] { upper, lower }, clean: true));
    }

    [TestCase(false, TestName = "Sync_RefusesTwoNamesForOneTable_HandWrittenCopyFirst")]
    [TestCase(true, TestName = "Sync_RefusesTwoNamesForOneTable_GeneratedCopyFirst")]
    public void Sync_RefusesTwoNamesForOneTable_WhenNoTableFileSettlesIt(bool generatedFirst)
    {
        // Which of the two names the project meant cannot be told from here: both classes carry
        // [GeneratedCode("XrmFramework", "2.0")], the 3.1 generator emitting the very same attribute
        // as the 2.* tool, and the namespace that would separate them never reaches this far. With
        // no .table file yet describing the entity, nothing on disk can settle it either — electing
        // one would rename a definition out from under its call sites, so the sync stops, whatever
        // order reflection hands the types back in.
        var handWritten = SystemUserDef();

        var generated = SystemUserDef();
        generated.TableName = "Systemuser";

        var definitions = generatedFirst
            ? new[] { generated, handWritten }
            : new[] { handWritten, generated };

        var conflict = Assert.Throws<DefinitionNameConflictException>(
            () => new TableFileSyncer(_tempDir).Sync(definitions));

        Assert.AreEqual("systemuser", conflict!.Conflicts.Single().LogicalName);
        Assert.AreEqual(new[] { "SystemUser", "Systemuser" },
            conflict.Conflicts.Single().Names.ToArray(),
            "Both names are reported, so one run tells the reader everything to fix.");

        Assert.IsFalse(Directory.EnumerateFiles(_tempDir, "*.table").Any(),
            "Refusing means refusing before writing: nothing was created.");
    }

    [TestCase(false, TestName = "Sync_ResolvesTwoNamesForOneTable_UsingExistingTableFile_HandWrittenCopyFirst")]
    [TestCase(true, TestName = "Sync_ResolvesTwoNamesForOneTable_UsingExistingTableFile_GeneratedCopyFirst")]
    public void Sync_ResolvesTwoNamesForOneTable_UsingExistingTableFile(bool generatedFirst)
    {
        // A business process flow: its logical name carries the process' GUID, so a .table created
        // before anyone gave it a nicer name ends up called after that GUID — here, the entity
        // already has such a file, named "BpfXyz123". Its current Name is necessarily what the 3.1
        // generator used to emit the BpfXyz123Definition class below, so "BpfLeadToOpportunityProcess"
        // — the other candidate — is the versioned 2.* class the project's own code refers to, and
        // the one to keep. The file itself is renamed accordingly, not just its Name field.
        var handWritten = BpfDef("BpfLeadToOpportunityProcess");
        var generated = BpfDef("BpfXyz123");

        var definitions = generatedFirst
            ? new[] { generated, handWritten }
            : new[] { handWritten, generated };

        WriteTable("BpfXyz123", BuildTable("eco_bpf_xyz123", "BpfXyz123", "eco_bpf_xyz123s",
            new Column { LogicalName = "eco_bpf_xyz123id", Name = "Id", Selected = true }));

        Assert.DoesNotThrow(() => new TableFileSyncer(_tempDir).Sync(definitions));

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "BpfXyz123.table")),
            "The stale file name must be gone once renamed.");

        var table = LoadTable("BpfLeadToOpportunityProcess");
        Assert.AreEqual("BpfLeadToOpportunityProcess", table.Name,
            "The versioned class' name is kept, not the one the generator produced from the old .table.");
        Assert.IsTrue(table.Columns.Single(c => c.LogicalName == "eco_bpf_xyz123id").Selected);
    }

    /// <summary>A Bpf* Definition under the given C# name, all describing the same CRM entity.</summary>
    private static DefinitionInfo BpfDef(string tableName) => new()
    {
        TableName = tableName,
        EntityName = "eco_bpf_xyz123",
        EntityCollectionName = "eco_bpf_xyz123s",
        Columns = new List<DefinitionColumnInfo> { new("eco_bpf_xyz123id", "Id") }
    };

    [Test]
    public void Sync_ReportsEveryNameConflict_InOneRun()
    {
        // The fix is one edit per table. Failing on the first would turn a single pass into as many
        // round trips as there are tables to align.
        var contact = ContactDef();
        var otherContact = ContactDef();
        otherContact.TableName = "Kontakt";

        var systemUser = SystemUserDef();
        var otherSystemUser = SystemUserDef();
        otherSystemUser.TableName = "Systemuser";

        var conflict = Assert.Throws<DefinitionNameConflictException>(
            () => new TableFileSyncer(_tempDir).Sync(
                new[] { contact, otherContact, systemUser, otherSystemUser }));

        Assert.AreEqual(new[] { "contact", "systemuser" },
            conflict!.Conflicts.Select(c => c.LogicalName).OrderBy(n => n).ToArray());
        Assert.IsFalse(Directory.EnumerateFiles(_tempDir, "*.table").Any(),
            "Nothing is written when the run cannot be trusted to write the right thing.");
    }

    [Test]
    public void Sync_FoldsTheCopiesOfOneTable_WhenTheyAgreeOnItsName()
    {
        // Same table, same name, two classes: the versioned *Definition.cs and the generated one.
        // Nothing to settle, so the file is written once and neither copy's columns are lost.
        var first = SystemUserDef();

        var second = SystemUserDef();
        second.Columns = new List<DefinitionColumnInfo> { new("domainname", "DomainName") };

        WriteTable("SystemUser", BuildTable("systemuser", "SystemUser", "systemusers",
            new Column { LogicalName = "systemuserid", Name = "Id", Selected = true },
            new Column { LogicalName = "domainname", Name = "DomainName" },
            new Column { LogicalName = "obsolete", Name = "Obsolete", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { first, second }, clean: true);

        var table = LoadTable("SystemUser");
        Assert.AreEqual("SystemUser", table.Name);
        Assert.IsTrue(table.Columns.Single(c => c.LogicalName == "domainname").Selected,
            "A column only the second copy declares is referenced by the code all the same.");
        Assert.IsFalse(table.Columns.Single(c => c.LogicalName == "obsolete").Selected);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // --clean mode: orphaned files (without a matching Definition)
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_Clean_DeletesOrphanedFile_WithoutLabels()
    {
        // File without any column carrying Labels -> considered entirely
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
        // File with at least one column carrying Labels -> real CRM data.
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
    // Option set names read from [OptionSet(typeof(...))]
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_AppliesOptionSetName_ToLocalOptionSet()
    {
        WriteTable("Contact", TableWithLocalOptionSet(currentName: "contact_statuscode"));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus") });

        var optionSet = LoadTable("Contact").Enums.Single();
        Assert.AreEqual("ContactStatus", optionSet.Name,
            "The C# name found on the column must be carried over to the option set it points at.");
    }

    [Test]
    public void Sync_PreservesOptionSetValues_WhenRenaming()
    {
        WriteTable("Contact", TableWithLocalOptionSet(currentName: "contact_statuscode"));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus") });

        var optionSet = LoadTable("Contact").Enums.Single();
        Assert.AreEqual("contact|statuscode", optionSet.LogicalName, "The CRM key must not move.");
        Assert.AreEqual(1, optionSet.Values.Count, "Renaming must not drop the members.");
    }

    [Test]
    public void Sync_AppliesOptionSetName_ToGlobalOptionSetInItsOwnFile()
    {
        // A global option set is not in the table that uses it: it lives in OptionSets.table.
        WriteTable("Contact", TableWithColumnPointingAt("tabsync_yesno"));
        WriteTable(TableFileStore.GlobalOptionSetFileName, GlobalOptionSetTable(
            new OptionSetEnum { LogicalName = "tabsync_yesno", Name = "tabsync_yesno", IsGlobal = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("YesNo") });

        Assert.AreEqual("YesNo", LoadTable(TableFileStore.GlobalOptionSetFileName).Enums.Single().Name);
    }

    [Test]
    public void Sync_AppliesOptionSetName_ToBothCopies_WhenAGlobalIsMirroredInItsTable()
    {
        // The 2.* DefinitionManager kept in a table's own Enums every option set one of its columns
        // referenced — globals included — while also writing the globals to OptionSets.table. Both
        // copies reach the generator, so renaming only one leaves the other to contradict it.
        var contact = TableWithColumnPointingAt("tabsync_yesno");
        contact.Enums.Add(new OptionSetEnum
        {
            LogicalName = "tabsync_yesno",
            Name = "tabsync_yesno",
            IsGlobal = true
        });
        WriteTable("Contact", contact);

        WriteTable(TableFileStore.GlobalOptionSetFileName, GlobalOptionSetTable(
            new OptionSetEnum { LogicalName = "tabsync_yesno", Name = "tabsync_yesno", IsGlobal = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("YesNo") });

        Assert.AreEqual("YesNo", LoadTable("Contact").Enums.Single().Name,
            "The copy held by the table must be renamed.");
        Assert.AreEqual("YesNo", LoadTable(TableFileStore.GlobalOptionSetFileName).Enums.Single().Name,
            "The shared copy must be renamed too, and its file written back.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Option set member names — what MyEnum.EnCours compiles against
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_AppliesMemberNames_FromTheEnumDeclaredInTheAssembly()
    {
        WriteTable("Contact", TableWithLocalOptionSet(currentName: "contact_statuscode",
            values: new[] { Value(1, "Actif"), Value(2, "Termine") }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus",
            Member(1, "EnCours"), Member(2, "Termine")) });

        var values = LoadTable("Contact").Enums.Single().Values.ToList();
        Assert.AreEqual("EnCours", values.Single(v => v.Value == 1).Name,
            "Code written against ContactStatus.EnCours must keep compiling after the migration.");
        Assert.AreEqual("Termine", values.Single(v => v.Value == 2).Name);
    }

    [Test]
    public void Sync_MatchesMembersOnTheirValue_NotTheirPosition()
    {
        WriteTable("Contact", TableWithLocalOptionSet(currentName: "contact_statuscode",
            values: new[] { Value(2, "Deux"), Value(1, "Un") }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus",
            Member(1, "Premier"), Member(2, "Second")) });

        var values = LoadTable("Contact").Enums.Single().Values.ToList();
        Assert.AreEqual("Premier", values.Single(v => v.Value == 1).Name);
        Assert.AreEqual("Second", values.Single(v => v.Value == 2).Name);
    }

    [Test]
    public void Sync_IgnoresTheSyntheticNullMember_OnAnOptionSetAllowingAnEmptyValue()
    {
        // The generator prepends "Null = 0" when HasNullValue is set: it mirrors the flag, not a CRM
        // option, and must not claim the real option numbered 0.
        var table = TableWithLocalOptionSet(currentName: "contact_statuscode",
            values: new[] { Value(0, "Aucun"), Value(1, "Un") });
        table.Enums.Single().HasNullValue = true;
        WriteTable("Contact", table);

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus",
            Member(0, "Null"), Member(0, "Vide"), Member(1, "Premier")) });

        var values = LoadTable("Contact").Enums.Single().Values.ToList();
        Assert.AreEqual("Vide", values.Single(v => v.Value == 0).Name,
            "Value 0 belongs to the real option that follows the synthetic Null.");
        Assert.AreEqual("Premier", values.Single(v => v.Value == 1).Name);
    }

    [Test]
    public void Sync_KeepsTheNullMember_WhenTheOptionSetDoesNotAllowAnEmptyValue()
    {
        // No HasNullValue: a member named Null is a genuine CRM option and must be honoured.
        WriteTable("Contact", TableWithLocalOptionSet(currentName: "contact_statuscode",
            values: new[] { Value(0, "Zero") }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus",
            Member(0, "Null")) });

        Assert.AreEqual("Null", LoadTable("Contact").Enums.Single().Values.Single().Name);
    }

    [Test]
    public void Sync_LeavesAmbiguousValueAlone_WhenTwoMembersShareIt()
    {
        // C# allows aliases; there is no way to tell which name the .table should carry.
        WriteTable("Contact", TableWithLocalOptionSet(currentName: "contact_statuscode",
            values: new[] { Value(1, "Original"), Value(2, "Deux") }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus",
            Member(1, "First"), Member(1, "Alias"), Member(2, "Second")) });

        var values = LoadTable("Contact").Enums.Single().Values.ToList();
        Assert.AreEqual("Original", values.Single(v => v.Value == 1).Name,
            "An ambiguous value is left as it is rather than guessed at.");
        Assert.AreEqual("Second", values.Single(v => v.Value == 2).Name,
            "The ambiguity must not spill over onto the other members.");
    }

    [Test]
    public void Sync_LeavesValueAlone_WhenTheAssemblyDeclaresNoMemberForIt()
    {
        WriteTable("Contact", TableWithLocalOptionSet(currentName: "contact_statuscode",
            values: new[] { Value(1, "Un"), Value(9, "Neuf") }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus",
            Member(1, "Premier")) });

        Assert.AreEqual("Neuf", LoadTable("Contact").Enums.Single().Values.Single(v => v.Value == 9).Name,
            "A CRM option the code never referenced keeps the name the .table already had.");
    }

    [Test]
    public void Sync_AppliesMemberNames_ToAGlobalOptionSetInItsOwnFile()
    {
        WriteTable("Contact", TableWithColumnPointingAt("tabsync_yesno"));

        var global = new OptionSetEnum { LogicalName = "tabsync_yesno", Name = "tabsync_yesno", IsGlobal = true };
        global.Values.Add(Value(1, "Oui"));
        WriteTable(TableFileStore.GlobalOptionSetFileName, GlobalOptionSetTable(global));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("YesNo", Member(1, "Yes")) });

        Assert.AreEqual("Yes", LoadTable(TableFileStore.GlobalOptionSetFileName).Enums.Single().Values.Single().Name);
    }

    [Test]
    public void Sync_DoesNotRenameMembersOfALockedOptionSet()
    {
        WriteTable("Contact", TableWithColumnPointingAt("tabsync_yesno"));

        var global = new OptionSetEnum
        {
            LogicalName = "tabsync_yesno",
            Name = "FrameworkYesNo",
            IsGlobal = true,
            IsLocked = true
        };
        global.Values.Add(Value(1, "Oui"));
        WriteTable(TableFileStore.GlobalOptionSetFileName, GlobalOptionSetTable(global));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("YesNo", Member(1, "Yes")) });

        Assert.AreEqual("Oui", LoadTable(TableFileStore.GlobalOptionSetFileName).Enums.Single().Values.Single().Name,
            "A frozen option set is frozen down to its members.");
    }

    [Test]
    public void Sync_PreservesValueLabelsAndExternalValue_WhenRenamingMembers()
    {
        var table = TableWithLocalOptionSet(currentName: "contact_statuscode", values: new OptionSetEnumValue[0]);
        var value = new OptionSetEnumValue { Value = 1, Name = "Actif", ExternalValue = "ACT" };
        value.Labels.Add(new LocalizedLabel { Label = "Actif", LangId = 1036 });
        table.Enums.Single().Values.Add(value);
        WriteTable("Contact", table);

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus", Member(1, "EnCours")) });

        var renamed = LoadTable("Contact").Enums.Single().Values.Single();
        Assert.AreEqual("EnCours", renamed.Name);
        Assert.AreEqual("ACT", renamed.ExternalValue, "Only the C# name moves.");
        Assert.AreEqual(1, renamed.Labels.Count, "CRM labels must survive the rename.");
    }

    [Test]
    public void Sync_DoesNotRenameLockedOptionSet()
    {
        // "Locked" marks the option sets shipped by the framework: their name belongs to the
        // package's own generated code.
        WriteTable("Contact", TableWithColumnPointingAt("tabsync_yesno"));
        WriteTable(TableFileStore.GlobalOptionSetFileName, GlobalOptionSetTable(new OptionSetEnum
        {
            LogicalName = "tabsync_yesno",
            Name = "FrameworkYesNo",
            IsGlobal = true,
            IsLocked = true
        }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("YesNo") });

        Assert.AreEqual("FrameworkYesNo", LoadTable(TableFileStore.GlobalOptionSetFileName).Enums.Single().Name);
    }

    [Test]
    public void Sync_LeavesOptionSetAlone_WhenColumnCarriesNoAttribute()
    {
        WriteTable("Contact", TableWithLocalOptionSet(currentName: "contact_statuscode"));

        // Same column, but the code declares no [OptionSet].
        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet(null) });

        Assert.AreEqual("contact_statuscode", LoadTable("Contact").Enums.Single().Name);
    }

    [Test]
    public void Sync_LeavesOptionSetAlone_WhenColumnIsNewAndCarriesNoCrmMetadata()
    {
        // A column created by the migration has no EnumName yet: there is no option set to name.
        WriteTable("Contact", BuildTable("contact", "Contact", "contacts"));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDefWithOptionSet("ContactStatus") });

        CollectionAssert.IsEmpty(LoadTable("Contact").Enums,
            "Naming an option set that the .table does not describe would invent an empty enum.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // The global option sets pseudo-table
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_Clean_KeepsGlobalOptionSetTable()
    {
        // OptionSets.table describes no entity, so no Definition claims it, and it holds no column
        // to vouch for its content: both orphan heuristics would condemn it.
        WriteTable(TableFileStore.GlobalOptionSetFileName, GlobalOptionSetTable(
            new OptionSetEnum { LogicalName = "tabsync_yesno", Name = "YesNo", IsGlobal = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: true);

        Assert.IsTrue(File.Exists(GlobalOptionSetPath()),
            "Deleting the global option sets would take every shared enum of the project with it.");
        Assert.AreEqual(1, LoadTable(TableFileStore.GlobalOptionSetFileName).Enums.Count);
    }

    [Test]
    public void Sync_Clean_StillDeletesARealTableNamedOptionSets()
    {
        // Identification goes by logical name, not by file name: a genuine entity sitting in the
        // file name the globals use is treated like any other orphan.
        WriteTable(TableFileStore.GlobalOptionSetFileName,
            BuildTable("ftp_optionset", "OptionSets", "ftp_optionsets",
                new Column { LogicalName = "ftp_optionsetid", Name = "Id", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: true);

        Assert.IsFalse(File.Exists(GlobalOptionSetPath()));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Contact whose StatusCode column declares <c>[OptionSet(typeof(...))]</c>.</summary>
    private static DefinitionInfo ContactDefWithOptionSet(
        string optionSetName, params DefinitionOptionSetValue[] members) => new()
    {
        TableName = "Contact",
        EntityName = "contact",
        EntityCollectionName = "contacts",
        Columns = new List<DefinitionColumnInfo>
        {
            new("contactid", "Id"),
            new("statuscode", "StatusCode", optionSetName, members)
        }
    };

    /// <summary>A member of the enum as the analyzed assembly declares it.</summary>
    private static DefinitionOptionSetValue Member(int value, string name) => new(value, name);

    /// <summary>A value of the option set as the .table records it.</summary>
    private static OptionSetEnumValue Value(int value, string name)
        => new() { Value = value, Name = name };

    /// <summary>Contact.table whose statuscode column points at a local option set.</summary>
    private static Table TableWithLocalOptionSet(string currentName, OptionSetEnumValue[] values = null)
    {
        var table = BuildTable("contact", "Contact", "contacts",
            new Column
            {
                LogicalName = "statuscode",
                Name = "StatusCode",
                Selected = true,
                EnumName = "contact|statuscode"
            });

        var optionSet = new OptionSetEnum { LogicalName = "contact|statuscode", Name = currentName };

        foreach (var value in values ?? new[] { Value(0, "Active") })
            optionSet.Values.Add(value);

        table.Enums.Add(optionSet);

        return table;
    }

    /// <summary>Contact.table whose statuscode column points at an option set declared elsewhere.</summary>
    private static Table TableWithColumnPointingAt(string enumLogicalName)
        => BuildTable("contact", "Contact", "contacts",
            new Column
            {
                LogicalName = "statuscode",
                Name = "StatusCode",
                Selected = true,
                EnumName = enumLogicalName
            });

    /// <summary>The OptionSets.table pseudo-table: no entity, no column, only shared enums.</summary>
    private static Table GlobalOptionSetTable(params OptionSetEnum[] optionSets)
    {
        var table = new Table
        {
            LogicalName = TableFileStore.GlobalOptionSetLogicalName,
            Name = TableFileStore.GlobalOptionSetFileName
        };

        foreach (var optionSet in optionSets)
            table.Enums.Add(optionSet);

        return table;
    }

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

    /// <summary>Path of the global option sets pseudo-table in the temporary directory.</summary>
    private string GlobalOptionSetPath()
        => Path.Combine(_tempDir,
                        TableFileStore.GlobalOptionSetFileName + TableFileStore.TableFileExtension);

    private Table LoadTable(string tableName)
    {
        var json = File.ReadAllText(Path.Combine(_tempDir, tableName + ".table"));
        return JsonConvert.DeserializeObject<Table>(json)!;
    }
}
