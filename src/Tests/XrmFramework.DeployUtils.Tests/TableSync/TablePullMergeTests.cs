// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Merge rules applied during a pull from the CRM: whatever becomes a C# identifier
/// belongs to the versioned file, whatever describes the table belongs to the CRM.
/// </summary>
[TestFixture]
public class TablePullMergeTests
{
    // ══════════════════════════════════════════════════════════════════════════
    // What belongs to the file
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Merge_KeepsHandRenamedColumnName_OverCrmSchemaName()
    {
        // The team renamed "FtpNumeroContrat" to "NumeroContrat"; the compiled code references
        // this name. Picking it back up from the CRM would break the build on every pull.
        var existing = Table("ftp_contrat", "Contrat",
            Column("ftp_numerocontrat", "NumeroContrat", AttributeTypeCode.String));

        var fresh = Table("ftp_contrat", "Contrat",
            Column("ftp_numerocontrat", "FtpNumeroContrat", AttributeTypeCode.String));

        var merged = TableMerger.Merge(existing, fresh);

        Assert.AreEqual("NumeroContrat", merged.Columns.Single().Name);
    }

    [Test]
    public void Merge_KeepsExistingSelection_WhenColumnWasActivated()
    {
        var existing = Table("account", "Account",
            Column("telephone1", "Telephone1", AttributeTypeCode.String, selected: true));

        var fresh = Table("account", "Account",
            Column("telephone1", "Telephone1", AttributeTypeCode.String));

        var merged = TableMerger.Merge(existing, fresh);

        Assert.IsTrue(merged.Columns.Single().Selected,
            "An activated column must never be downgraded by a pull.");
    }

    [Test]
    public void Merge_KeepsExistingDeselection_WhenColumnWasDeliberatelyDisabled()
    {
        // createdon is selected by default on creation; if the team deactivated it,
        // the pull must not reactivate it.
        var existing = Table("account", "Account",
            Column("createdon", "CreatedOn", AttributeTypeCode.DateTime, selected: false));

        var fresh = Table("account", "Account",
            Column("createdon", "CreatedOn", AttributeTypeCode.DateTime, selected: true));

        var merged = TableMerger.Merge(existing, fresh);

        Assert.IsFalse(merged.Columns.Single().Selected);
    }

    [Test]
    public void Merge_PreservesColumnLockedMarker()
    {
        var existing = Table("role", "Role",
            Column("name", "Name", AttributeTypeCode.String, isLocked: true));

        var fresh = Table("role", "Role", Column("name", "Name", AttributeTypeCode.String));

        var merged = TableMerger.Merge(existing, fresh);

        Assert.IsTrue(merged.Columns.Single().IsLocked);
    }

    [Test]
    public void Merge_PreservesTableNameAndLockedMarker()
    {
        var existing = Table("ftp_contrat", "ContratLocation");
        existing.IsLocked = true;

        var fresh = Table("ftp_contrat", "Contrat");
        fresh.CollectionName = "ftp_contrats";

        var merged = TableMerger.Merge(existing, fresh);

        Assert.AreEqual("ContratLocation", merged.Name, "The C# type name belongs to the file.");
        Assert.IsTrue(merged.IsLocked);
        Assert.AreEqual("ftp_contrats", merged.CollectionName, "The collection name comes from the CRM.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // What belongs to the CRM
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Merge_RefreshesMetadataFromCrm()
    {
        var existing = Table("account", "Account",
            Column("name", "Name", AttributeTypeCode.String, stringLength: 100));

        var freshColumn = Column("name", "Name", AttributeTypeCode.Memo, stringLength: 500);
        freshColumn.Capabilities = AttributeCapabilities.Read | AttributeCapabilities.Update;
        freshColumn.Labels.Add(new LocalizedLabel { Label = "Nom du compte", LangId = 1036 });

        var merged = TableMerger.Merge(existing, Table("account", "Account", freshColumn));

        var column = merged.Columns.Single();
        Assert.AreEqual(AttributeTypeCode.Memo, column.Type);
        Assert.AreEqual(500, column.StringLength);
        Assert.AreEqual(AttributeCapabilities.Read | AttributeCapabilities.Update, column.Capabilities);
        Assert.AreEqual(1, column.Labels.Count, "Labels must be refreshed from the CRM.");
    }

    [Test]
    public void Merge_AddsColumnsDiscoveredInCrm()
    {
        var existing = Table("account", "Account", Column("accountid", "Id", AttributeTypeCode.Uniqueidentifier));

        var fresh = Table("account", "Account",
            Column("accountid", "Id", AttributeTypeCode.Uniqueidentifier),
            Column("ftp_nouveau", "Nouveau", AttributeTypeCode.String));

        var merged = TableMerger.Merge(existing, fresh);

        Assert.AreEqual(2, merged.Columns.Count);
        Assert.IsTrue(merged.Columns.Any(c => c.LogicalName == "ftp_nouveau"));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Columns that disappeared from the CRM
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Merge_KeepsColumnsMissingFromCrm()
    {
        // "pull" refreshes, it does not destroy: deselecting orphans is the job
        // of "migrate sync-tables --clean".
        var existing = Table("account", "Account",
            Column("name", "Name", AttributeTypeCode.String),
            Column("ftp_supprimee", "Supprimee", AttributeTypeCode.String, selected: true));

        var fresh = Table("account", "Account", Column("name", "Name", AttributeTypeCode.String));

        var merged = TableMerger.Merge(existing, fresh);

        var orphan = merged.Columns.SingleOrDefault(c => c.LogicalName == "ftp_supprimee");
        Assert.IsNotNull(orphan, "A column absent from the CRM must be kept.");
        Assert.IsTrue(orphan!.Selected);
    }

    [Test]
    public void GetColumnsMissingFromCrm_ListsOrphansForReporting()
    {
        var existing = Table("account", "Account",
            Column("name", "Name", AttributeTypeCode.String),
            Column("ftp_supprimee", "Supprimee", AttributeTypeCode.String));

        var fresh = Table("account", "Account", Column("name", "Name", AttributeTypeCode.String));

        var missing = TableMerger.GetColumnsMissingFromCrm(existing, fresh);

        Assert.AreEqual(1, missing.Count);
        Assert.AreEqual("ftp_supprimee", missing[0].LogicalName);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Option sets
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Merge_KeepsHandRenamedEnumAndMemberNames()
    {
        var existing = Table("account", "Account");
        existing.Enums.Add(Enumeration("account|statuscode", "StatutCompte",
            (1, "Actif"), (2, "Inactif")));

        var fresh = Table("account", "Account");
        fresh.Enums.Add(Enumeration("account|statuscode", "AccountStatus",
            (1, "Active"), (2, "Inactive")));

        var merged = TableMerger.Merge(existing, fresh);

        var merges = merged.Enums.Single();
        Assert.AreEqual("StatutCompte", merges.Name, "The enum type name belongs to the file.");
        Assert.AreEqual("Actif", merges.Values.Single(v => v.Value == 1).Name,
            "Members are matched by value, the only stable piece of data.");
    }

    [Test]
    public void MergeGlobalOptionSets_IsAdditive_AndKeepsUnrelatedEnums()
    {
        // Pulling a single table must never remove global option sets
        // referenced by the project's other tables.
        var existing = new Table { LogicalName = "globalEnums", Name = "OptionSet" };
        existing.Enums.Add(Enumeration("ftp_devise", "Devise", (1, "Euro")));
        existing.Enums.Add(Enumeration("ftp_pays", "Pays", (1, "France")));

        var fresh = new[] { Enumeration("ftp_devise", "Currency", (1, "Euro"), (2, "Dollar")) };

        var merged = TableMerger.MergeGlobalOptionSets(existing, fresh);

        Assert.AreEqual(2, merged.Enums.Count, "The unrelated option set must survive.");

        var devise = merged.Enums.Single(e => e.LogicalName == "ftp_devise");
        Assert.AreEqual("Devise", devise.Name, "The retained name remains the one from the file.");
        Assert.AreEqual(2, devise.Values.Count, "Values are refreshed from the CRM.");
    }

    [Test]
    public void MergeGlobalOptionSets_KeepsTheNullabilityAnyCopyEstablishes()
    {
        // The same global choice reached through two columns. Only one of the copies carries the
        // flag — a multi-select column reaches us as a Virtual attribute, which used to answer
        // "no null value" whatever the options say. Keeping the first copy alone made the
        // generated enum depend on the order the retrieval walked the columns in.
        var withoutFlag = Enumeration("ftp_canal", "Canal", (100, "Direct"));
        var withFlag = Enumeration("ftp_canal", "Canal", (100, "Direct"));
        withFlag.HasNullValue = true;

        var merged = TableMerger.MergeGlobalOptionSets(null, new[] { withoutFlag, withFlag });

        Assert.AreEqual(1, merged.Enums.Count, "The two copies describe one and the same choice.");
        Assert.IsTrue(merged.Enums.Single().HasNullValue,
            "The choice has no option valued 0: the generated enum needs its explicit Null member.");
    }

    [Test]
    public void MergeGlobalOptionSets_OrdersEnumsDeterministically()
    {
        var fresh = new[]
        {
            Enumeration("ftp_zebre", "Zebre", (1, "A")),
            Enumeration("ftp_alpha", "Alpha", (1, "A"))
        };

        var merged = TableMerger.MergeGlobalOptionSets(null, fresh);

        Assert.AreEqual(new[] { "ftp_alpha", "ftp_zebre" },
            merged.Enums.Select(e => e.LogicalName).ToArray(),
            "An unstable order would produce spurious diffs on every run.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Alternate keys
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Merge_KeepsHandRenamedKeyName_OverTheNameDerivedFromTheLabel()
    {
        var existing = Table("ftp_contrat", "Contrat");
        existing.Keys.Add(new Key { LogicalName = "ftp_reference_key", Name = "Reference" });

        var fresh = Table("ftp_contrat", "Contrat");
        fresh.Keys.Add(new Key { LogicalName = "ftp_reference_key", Name = "ReferenceLookupKey" });

        var merged = TableMerger.Merge(existing, fresh);

        Assert.AreEqual("Reference", merged.Keys.Single().Name);
    }

    [Test]
    public void Merge_RecognizesAKeyDeclaredTheOldWay()
    {
        // A file written before Key.LogicalName existed holds the logical name in Name. Matching on
        // LogicalName alone left it unrecognized, and the pull renamed the constant the project
        // compiles against.
        var existing = Table("ftp_contrat", "Contrat");
        existing.Keys.Add(new Key { Name = "ftp_reference_key" });

        var fresh = Table("ftp_contrat", "Contrat");
        fresh.Keys.Add(new Key { LogicalName = "ftp_reference_key", Name = "ReferenceLookupKey" });

        var merged = TableMerger.Merge(existing, fresh);

        Assert.AreEqual("ftp_reference_key", merged.Keys.Single().Name);
        Assert.AreEqual("ftp_reference_key", merged.Keys.Single().LogicalName,
            "The pull is what fills in the logical name the old format never wrote.");
    }

    [Test]
    public void Merge_ReturnsFreshTable_WhenNoExistingFile()
    {
        var fresh = Table("account", "Account", Column("name", "Name", AttributeTypeCode.String));

        Assert.AreSame(fresh, TableMerger.Merge(null, fresh));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Locating the target file
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void FindTableFile_MatchesOnLogicalName_NotOnFileName()
    {
        // The file was renamed by hand along with the C# type name.
        // Relying on the file name would create a duplicate instead of updating the existing one.
        var directory = Path.Combine(Path.GetTempPath(), "XrmFramework.StoreTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            var path = Path.Combine(directory, "ContratLocation.table");
            TableFileStore.Save(path, Table("ftp_contrat", "ContratLocation"));

            Assert.AreEqual(path, TableFileStore.FindTableFile(directory, "ftp_contrat"));
            Assert.IsNull(TableFileStore.FindTableFile(directory, "ftp_inconnue"));
        }
        finally
        {
            try { Directory.Delete(directory, recursive: true); } catch { /* best effort */ }
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    private static Table Table(string logicalName, string name, params Column[] columns)
    {
        var table = new Table { LogicalName = logicalName, Name = name, CollectionName = logicalName + "s" };

        foreach (var column in columns)
            table.Columns.Add(column);

        return table;
    }

    private static Column Column(
        string logicalName,
        string name,
        AttributeTypeCode type,
        bool selected = false,
        bool isLocked = false,
        int? stringLength = null)
        => new()
        {
            LogicalName = logicalName,
            Name = name,
            Type = type,
            Selected = selected,
            IsLocked = isLocked,
            StringLength = stringLength
        };

    private static OptionSetEnum Enumeration(
        string logicalName, string name, params (int Value, string Name)[] values)
    {
        var result = new OptionSetEnum { LogicalName = logicalName, Name = name };

        foreach (var value in values)
            result.Values.Add(new OptionSetEnumValue { Value = value.Value, Name = value.Name });

        return result;
    }
}
