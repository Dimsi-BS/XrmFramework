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
        // Répertoire isolé par test pour éviter toute contamination croisée.
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
        catch { /* meilleur effort */ }
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
    // Création d'un .table absent
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_CreatesNewTableFile_WhenMissing()
    {
        var def = ContactDef();
        var syncer = new TableFileSyncer(_tempDir);

        syncer.Sync(new[] { def });

        var path = Path.Combine(_tempDir, "Contact.table");
        Assert.IsTrue(File.Exists(path), "Le fichier .table doit être créé.");
    }

    [Test]
    public void Sync_NewTable_ContainsAllColumnsWithSelectTrue()
    {
        var def = ContactDef();
        new TableFileSyncer(_tempDir).Sync(new[] { def });

        var table = LoadTable("Contact");
        Assert.AreEqual(2, table.Columns.Count);
        Assert.IsTrue(table.Columns.All(c => c.Selected),
            "Toutes les colonnes d'une table nouvellement créée doivent être Selected.");
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
    // Mise à jour d'un .table existant
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_AddsMissingColumn_WhenAbsentFromExistingTable()
    {
        // Fichier existant ne contenant que Id ; la Definition ajoute Name.
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
            "Les colonnes présentes dans la Definition doivent passer à Selected=true.");
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
        Assert.AreEqual(1, idCol.Labels.Count, "Les Labels CRM ne doivent pas être perdus.");
    }

    [Test]
    public void Sync_WithoutClean_DoesNotTouchColumnsAbsentFromDefinition()
    {
        // Colonne "etrangere" présente dans le fichier mais pas dans la Definition.
        WriteTable("Contact", BuildTable("contact", "Contact", "contacts",
            new Column { LogicalName = "contactid", Name = "Id", Selected = true },
            new Column { LogicalName = "etrangere", Name = "Etrangere", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: false);

        var orphan = LoadTable("Contact").Columns.Single(c => c.LogicalName == "etrangere");
        Assert.IsTrue(orphan.Selected,
            "Sans --clean, les colonnes hors Definition doivent rester telles quelles.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Mode --clean : colonnes orphelines dans une table gérée
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
            "Avec --clean, les colonnes hors Definition doivent être de-sélectionnées.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Mode --clean : fichiers orphelins (sans Definition correspondante)
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Sync_Clean_DeletesOrphanedFile_WithoutLabels()
    {
        // Fichier sans aucune colonne avec Labels → considéré comme entièrement
        // produit par TableSyncHelper, donc supprimable.
        WriteTable("Ghost", BuildTable("ghost", "Ghost", "ghosts",
            new Column { LogicalName = "ghostid", Name = "Id", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: true);

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "Ghost.table")),
            "Le .table orphelin sans Labels doit être supprimé.");
    }

    [Test]
    public void Sync_Clean_KeepsOrphanedFile_WithLabels()
    {
        // Fichier ayant au moins une colonne avec Labels → données CRM réelles.
        // Doit être conservé même si plus aucune Definition ne le référence.
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
            "Un .table orphelin contenant des Labels CRM doit être conservé.");
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
            "Toutes les colonnes d'un .table orphelin conservé doivent être de-sélectionnées.");
    }

    [Test]
    public void Sync_WithoutClean_PreservesOrphanedFiles_Untouched()
    {
        WriteTable("Ghost", BuildTable("ghost", "Ghost", "ghosts",
            new Column { LogicalName = "ghostid", Name = "Id", Selected = true }));

        new TableFileSyncer(_tempDir).Sync(new[] { ContactDef() }, clean: false);

        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "Ghost.table")),
            "Sans --clean, les fichiers orphelins ne doivent pas être supprimés.");
        Assert.IsTrue(LoadTable("Ghost").Columns.Single().Selected,
            "Sans --clean, les colonnes orphelines ne doivent pas être de-sélectionnées.");
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
