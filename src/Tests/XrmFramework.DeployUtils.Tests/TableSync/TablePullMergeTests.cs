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
/// Règles de fusion appliquées lors d'une récupération depuis le CRM : ce qui devient un
/// identifiant C# appartient au fichier versionné, ce qui décrit la table appartient au CRM.
/// </summary>
[TestFixture]
public class TablePullMergeTests
{
    // ══════════════════════════════════════════════════════════════════════════
    // Ce qui appartient au fichier
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Merge_KeepsHandRenamedColumnName_OverCrmSchemaName()
    {
        // L'équipe a renommé « FtpNumeroContrat » en « NumeroContrat » ; le code compilé référence
        // ce nom. Le reprendre depuis le CRM casserait la compilation à chaque récupération.
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
            "Une colonne activée ne doit jamais être rétrogradée par une récupération.");
    }

    [Test]
    public void Merge_KeepsExistingDeselection_WhenColumnWasDeliberatelyDisabled()
    {
        // createdon est sélectionnée d'office à la création ; si l'équipe l'a désactivée,
        // la récupération ne doit pas la réactiver.
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

        Assert.AreEqual("ContratLocation", merged.Name, "Le nom de type C# appartient au fichier.");
        Assert.IsTrue(merged.IsLocked);
        Assert.AreEqual("ftp_contrats", merged.CollectionName, "Le nom de collection vient du CRM.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Ce qui appartient au CRM
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
        Assert.AreEqual(1, column.Labels.Count, "Les libellés doivent être rafraîchis depuis le CRM.");
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
    // Colonnes disparues du CRM
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Merge_KeepsColumnsMissingFromCrm()
    {
        // « pull » rafraîchit, il ne détruit pas : la désélection des orphelines relève
        // de « tables sync --clean ».
        var existing = Table("account", "Account",
            Column("name", "Name", AttributeTypeCode.String),
            Column("ftp_supprimee", "Supprimee", AttributeTypeCode.String, selected: true));

        var fresh = Table("account", "Account", Column("name", "Name", AttributeTypeCode.String));

        var merged = TableMerger.Merge(existing, fresh);

        var orphan = merged.Columns.SingleOrDefault(c => c.LogicalName == "ftp_supprimee");
        Assert.IsNotNull(orphan, "Une colonne absente du CRM doit être conservée.");
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
        Assert.AreEqual("StatutCompte", merges.Name, "Le nom du type énuméré appartient au fichier.");
        Assert.AreEqual("Actif", merges.Values.Single(v => v.Value == 1).Name,
            "Les membres sont rapprochés par valeur, seule donnée stable.");
    }

    [Test]
    public void MergeGlobalOptionSets_IsAdditive_AndKeepsUnrelatedEnums()
    {
        // Récupérer une seule table ne doit jamais retirer les option sets globaux
        // référencés par les autres tables du projet.
        var existing = new Table { LogicalName = "globalEnums", Name = "OptionSet" };
        existing.Enums.Add(Enumeration("ftp_devise", "Devise", (1, "Euro")));
        existing.Enums.Add(Enumeration("ftp_pays", "Pays", (1, "France")));

        var fresh = new[] { Enumeration("ftp_devise", "Currency", (1, "Euro"), (2, "Dollar")) };

        var merged = TableMerger.MergeGlobalOptionSets(existing, fresh);

        Assert.AreEqual(2, merged.Enums.Count, "L'option set non concerné doit survivre.");

        var devise = merged.Enums.Single(e => e.LogicalName == "ftp_devise");
        Assert.AreEqual("Devise", devise.Name, "Le nom retenu reste celui du fichier.");
        Assert.AreEqual(2, devise.Values.Count, "Les valeurs sont rafraîchies depuis le CRM.");
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
            "Un ordre instable produirait des diffs parasites à chaque exécution.");
    }

    [Test]
    public void Merge_ReturnsFreshTable_WhenNoExistingFile()
    {
        var fresh = Table("account", "Account", Column("name", "Name", AttributeTypeCode.String));

        Assert.AreSame(fresh, TableMerger.Merge(null, fresh));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Localisation du fichier cible
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void FindTableFile_MatchesOnLogicalName_NotOnFileName()
    {
        // Le fichier a été renommé à la main en même temps que le nom de type C#.
        // Se fier au nom de fichier créerait un doublon au lieu de mettre à jour l'existant.
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
            try { Directory.Delete(directory, recursive: true); } catch { /* meilleur effort */ }
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
