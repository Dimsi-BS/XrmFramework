// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// <c>tables columns list / add / set</c>: entirely offline edits of already-versioned
/// <c>.table</c> files, exercised end to end (write to disk -> command -> re-read).
/// </summary>
[TestFixture]
public class ColumnHelperTests
{
    private string _root = string.Empty;
    private string _tablesDir = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "XrmFramework.ColumnHelper_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_root, "Config"));
        File.WriteAllText(Path.Combine(_root, "Config", "xrmFramework.config"),
            "<xrmFramework selectedConnection=\"Xrm\"><entitySolution name=\"Sample\" /></xrmFramework>");

        _tablesDir = Path.Combine(_root, "Sample.Core", "Definitions");
        Directory.CreateDirectory(_tablesDir);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);
        }
        catch { /* best effort */ }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // tables columns list
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void List_ReturnsNoMatch_WhenNoTableIsTracked()
    {
        var exitCode = ColumnHelper.List(_root, _tablesDir, null, null, null, unselectedOnly: false);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitNoMatch));
    }

    [Test]
    public void List_DefaultsToEveryTrackedTable_WhenNoCriteriaGiven()
    {
        WriteContrat();
        WriteTable("Account", "account", new Column { LogicalName = "name", Name = "Name", Selected = true });

        var exitCode = ColumnHelper.List(_root, _tablesDir, null, null, null, unselectedOnly: false);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));
    }

    [Test]
    public void List_UnselectedOnly_FiltersToInactiveColumns()
    {
        // Reading behavior back through the table object itself: the console rendering is not
        // asserted, but MatchesColumn's filtering must leave the file untouched (list is read-only).
        WriteContrat();

        var exitCode = ColumnHelper.List(_root, _tablesDir, new[] { "ftp_contrat" }, null, null, unselectedOnly: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));
        var table = TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!);
        Assert.That(table.Columns.Single(c => c.LogicalName == "ftp_datedebut").Selected, Is.False,
            "list must never mutate the file.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // tables columns add
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Add_ReturnsNoMatch_WhenNeitherTableNorPrefixIsGiven()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Add(_root, _tablesDir, null, null, new[] { "ftp_datedebut" }, all: false, noPrompt: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitNoMatch));
    }

    [Test]
    public void Add_ReturnsNoMatch_WhenNeitherColumnNorAllIsGiven()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Add(_root, _tablesDir, new[] { "ftp_contrat" }, null, null, all: false, noPrompt: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitNoMatch));
    }

    [Test]
    public void Add_ActivatesRequestedColumns_AndPersistsToDisk()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Add(
            _root, _tablesDir, new[] { "ftp_contrat" }, null, new[] { "ftp_datedebut", "ftp_datefin" },
            all: false, noPrompt: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));

        var table = TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!);
        Assert.That(table.Columns.Single(c => c.LogicalName == "ftp_datedebut").Selected, Is.True);
        Assert.That(table.Columns.Single(c => c.LogicalName == "ftp_datefin").Selected, Is.True);
        Assert.That(table.Columns.Single(c => c.LogicalName == "ftp_montant").Selected, Is.False,
            "A column not requested must not be touched.");
    }

    [Test]
    public void Add_AcceptsCommaSeparatedColumns()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Add(
            _root, _tablesDir, new[] { "ftp_contrat" }, null, new[] { "ftp_datedebut,ftp_datefin" },
            all: false, noPrompt: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));

        var table = TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!);
        Assert.That(table.Columns.Where(c => c.Selected).Select(c => c.LogicalName),
            Is.EquivalentTo(new[] { "ftp_contratid", "ftp_datedebut", "ftp_datefin" }));
    }

    [Test]
    public void Add_ReportsUnknownColumn_ButStillActivatesTheOthers()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Add(
            _root, _tablesDir, new[] { "ftp_contrat" }, null, new[] { "ftp_datedebut", "ftp_nope" },
            all: false, noPrompt: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));
        var table = TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!);
        Assert.That(table.Columns.Single(c => c.LogicalName == "ftp_datedebut").Selected, Is.True);
    }

    [Test]
    public void Add_All_ActivatesEveryRemainingColumn()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Add(_root, _tablesDir, new[] { "ftp_contrat" }, null, null, all: true, noPrompt: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));
        var table = TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!);
        Assert.That(table.Columns.All(c => c.Selected), Is.True);
    }

    [Test]
    public void Add_MatchesTablesByPrefix()
    {
        WriteContrat();
        WriteTable("ContratLocation", "ftp_contratlocation",
            new Column { LogicalName = "ftp_id", Name = "Id", Selected = true },
            new Column { LogicalName = "ftp_libelle", Name = "Libelle", Selected = false });

        var exitCode = ColumnHelper.Add(_root, _tablesDir, null, "ftp_", new[] { "ftp_libelle" }, all: false, noPrompt: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));

        // "ftp_contrat" has no "ftp_libelle" column: only the matching table is touched.
        var contratLocation = TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contratlocation")!);
        Assert.That(contratLocation.Columns.Single(c => c.LogicalName == "ftp_libelle").Selected, Is.True);
    }

    [Test]
    public void Add_ReturnsSuccess_WhenEverythingRequestedIsAlreadySelected()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Add(
            _root, _tablesDir, new[] { "ftp_contrat" }, null, new[] { "ftp_contratid" }, all: false, noPrompt: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess),
            "Already-selected columns are a no-op, not an error.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // tables columns set
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Set_RenamesColumn_AndPersists()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Set(_root, _tablesDir, "ftp_contrat", "ftp_datefin", "DateFinContrat", select: null);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));
        var table = TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!);
        Assert.That(table.Columns.Single(c => c.LogicalName == "ftp_datefin").Name, Is.EqualTo("DateFinContrat"));
    }

    [Test]
    public void Set_RejectsRename_WhenNameCollidesWithAnotherColumn()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Set(_root, _tablesDir, "ftp_contrat", "ftp_montant", "DateDebut", select: null);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitError));
        var table = TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!);
        Assert.That(table.Columns.Single(c => c.LogicalName == "ftp_montant").Name, Is.EqualTo("Montant"),
            "A rejected rename must not be written.");
    }

    [Test]
    public void Set_TogglesSelection_BothWays()
    {
        WriteContrat();

        Assert.That(ColumnHelper.Set(_root, _tablesDir, "ftp_contrat", "ftp_montant", null, select: true),
            Is.EqualTo(ColumnHelper.ExitSuccess));
        Assert.That(TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!)
            .Columns.Single(c => c.LogicalName == "ftp_montant").Selected, Is.True);

        Assert.That(ColumnHelper.Set(_root, _tablesDir, "ftp_contrat", "ftp_montant", null, select: false),
            Is.EqualTo(ColumnHelper.ExitSuccess));
        Assert.That(TableFileStore.Load(TableFileStore.FindTableFile(_tablesDir, "ftp_contrat")!)
            .Columns.Single(c => c.LogicalName == "ftp_montant").Selected, Is.False);
    }

    [Test]
    public void Set_FindsTableByItsCSharpName_NotOnlyItsLogicalName()
    {
        WriteContrat();

        // "Contrat" is the file's C# Name; the file itself is ftp_contrat's logical name.
        var exitCode = ColumnHelper.Set(_root, _tablesDir, "Contrat", "ftp_montant", null, select: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));
    }

    [Test]
    public void Set_ReturnsNoMatch_WhenTableIsNotTrackedLocally()
    {
        var exitCode = ColumnHelper.Set(_root, _tablesDir, "ftp_inconnu", "whatever", "NewName", select: null);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitNoMatch));
    }

    [Test]
    public void Set_ReturnsNoMatch_WhenColumnIsNotInTheTable()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Set(_root, _tablesDir, "ftp_contrat", "ftp_inconnue", "NewName", select: null);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitNoMatch));
    }

    [Test]
    public void Set_ReturnsSuccess_WhenRequestedValueAlreadyMatches()
    {
        WriteContrat();

        var exitCode = ColumnHelper.Set(_root, _tablesDir, "ftp_contrat", "ftp_contratid", "Id", select: true);

        Assert.That(exitCode, Is.EqualTo(ColumnHelper.ExitSuccess));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    private void WriteContrat()
        => WriteTable("Contrat", "ftp_contrat",
            new Column { LogicalName = "ftp_contratid", Name = "Id", Selected = true },
            new Column { LogicalName = "ftp_datedebut", Name = "DateDebut", Selected = false },
            new Column { LogicalName = "ftp_datefin", Name = "DateFin", Selected = false },
            new Column { LogicalName = "ftp_montant", Name = "Montant", Selected = false });

    private void WriteTable(string fileName, string logicalName, params Column[] columns)
    {
        var table = new Table { Name = fileName, LogicalName = logicalName };
        foreach (var column in columns)
            table.Columns.Add(column);

        TableFileStore.Save(TableFileStore.BuildTableFilePath(_tablesDir, fileName), table);
    }
}
