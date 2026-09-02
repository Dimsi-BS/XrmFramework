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
/// <c>tables optionsets list / set</c>: entirely offline renames of an option set and/or its
/// members, across every local <c>.table</c> file that declares it.
/// </summary>
[TestFixture]
public class OptionSetHelperTests
{
    private string _root = string.Empty;
    private string _tablesDir = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "XrmFramework.OptionSetHelper_" + Guid.NewGuid().ToString("N"));
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
    // tables optionsets list
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void List_Overview_ReturnsNoMatch_WhenNothingIsTracked()
    {
        var exitCode = OptionSetHelper.List(_root, _tablesDir, null, null, globalOnly: false);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitNoMatch));
    }

    [Test]
    public void List_Overview_ReturnsSuccess_WhenAtLeastOneOptionSetExists()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.List(_root, _tablesDir, null, null, globalOnly: false);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess));
    }

    [Test]
    public void List_Members_ReturnsNoMatch_WhenOptionSetIsUnknown()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.List(_root, _tablesDir, "ftp_nope", null, globalOnly: false);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitNoMatch));
    }

    [Test]
    public void List_Members_ReturnsSuccess_WhenOptionSetExists()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.List(_root, _tablesDir, "ftp_contrat_statut", null, globalOnly: false);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // tables optionsets set — local option set
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Set_RenamesTheEnum_AndPersists()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.Set(_root, _tablesDir, "ftp_contrat_statut", "StatutContrat", null, null);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess));
        Assert.That(LoadEnum("Contrat", "ftp_contrat_statut").Name, Is.EqualTo("StatutContrat"));
    }

    [Test]
    public void Set_RenamesOneMember_LeavingTheOthersUntouched()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.Set(_root, _tablesDir, "ftp_contrat_statut", null, 1, "EnCours");

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess));
        var optionSet = LoadEnum("Contrat", "ftp_contrat_statut");
        Assert.That(optionSet.Values.Single(v => v.Value == 1).Name, Is.EqualTo("EnCours"));
        Assert.That(optionSet.Values.Single(v => v.Value == 2).Name, Is.EqualTo("Termine"),
            "A member not requested must not be touched.");
    }

    [Test]
    public void Set_RenamesTheEnumAndAMember_InOneCall()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.Set(_root, _tablesDir, "ftp_contrat_statut", "StatutContrat", 1, "EnCours");

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess));
        var optionSet = LoadEnum("Contrat", "ftp_contrat_statut");
        Assert.That(optionSet.Name, Is.EqualTo("StatutContrat"));
        Assert.That(optionSet.Values.Single(v => v.Value == 1).Name, Is.EqualTo("EnCours"));
    }

    [Test]
    public void Set_ReturnsNoMatch_WhenOptionSetIsUnknown()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.Set(_root, _tablesDir, "ftp_nope", "NewName", null, null);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitNoMatch));
    }

    [Test]
    public void Set_ReportsUnknownMember_ButStillAppliesTheEnumRename()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.Set(_root, _tablesDir, "ftp_contrat_statut", "StatutContrat", 999, "Whatever");

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess));
        Assert.That(LoadEnum("Contrat", "ftp_contrat_statut").Name, Is.EqualTo("StatutContrat"));
    }

    [Test]
    public void Set_DoesNotTouchTheFile_WhenTheLockedCopyIsTheOnlyMatch()
    {
        WriteContrat(locked: true);

        var exitCode = OptionSetHelper.Set(_root, _tablesDir, "ftp_contrat_statut", "ShouldNotApply", null, null);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess),
            "A frozen copy is reported, not an error.");
        Assert.That(LoadEnum("Contrat", "ftp_contrat_statut").Name, Is.EqualTo("StatutContratOld"),
            "Locked must never be renamed — it belongs to the framework package's own generated code.");
    }

    [Test]
    public void Set_ReturnsSuccess_WhenRequestedNameAlreadyMatches()
    {
        WriteContrat();

        var exitCode = OptionSetHelper.Set(_root, _tablesDir, "ftp_contrat_statut", "StatutContratOld", null, null);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // tables optionsets set — global option set, declared in several files
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Set_RenamesEveryCopy_OfAGlobalOptionSet()
    {
        WriteContrat();
        WriteGlobalOptionSets();

        var exitCode = OptionSetHelper.Set(_root, _tablesDir, "ftp_type_global", "TypeContrat", null, null);

        Assert.That(exitCode, Is.EqualTo(OptionSetHelper.ExitSuccess));
        Assert.That(LoadEnum("Contrat", "ftp_type_global").Name, Is.EqualTo("TypeContrat"),
            "The table's own copy must be renamed.");
        Assert.That(LoadEnum(TableFileStore.GlobalOptionSetFileName, "ftp_type_global").Name, Is.EqualTo("TypeContrat"),
            "OptionSets.table's copy must be renamed too — it is a separate JSON object.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    private void WriteContrat(bool locked = false)
    {
        var table = new Table { Name = "Contrat", LogicalName = "ftp_contrat" };

        table.Columns.Add(new Column
        {
            LogicalName = "ftp_contratid", Name = "Id", Selected = true, EnumName = "ftp_contrat_statut"
        });
        table.Columns.Add(new Column
        {
            LogicalName = "ftp_type", Name = "Type", Selected = true, EnumName = "ftp_type_global"
        });

        var localOptionSet = new OptionSetEnum
        {
            LogicalName = "ftp_contrat_statut", Name = "StatutContratOld", IsLocked = locked
        };
        localOptionSet.Values.Add(new OptionSetEnumValue { Value = 1, Name = "EnCoursOld" });
        localOptionSet.Values.Add(new OptionSetEnumValue { Value = 2, Name = "Termine" });
        table.Enums.Add(localOptionSet);

        var globalOptionSet = new OptionSetEnum
        {
            LogicalName = "ftp_type_global", Name = "TypeGlobalOld", IsGlobal = true
        };
        globalOptionSet.Values.Add(new OptionSetEnumValue { Value = 100000000, Name = "TypeA" });
        table.Enums.Add(globalOptionSet);

        TableFileStore.Save(TableFileStore.BuildTableFilePath(_tablesDir, "Contrat"), table);
    }

    private void WriteGlobalOptionSets()
    {
        var pseudoTable = new Table
        {
            Name = TableFileStore.GlobalOptionSetFileName, LogicalName = TableFileStore.GlobalOptionSetLogicalName
        };

        var globalOptionSet = new OptionSetEnum
        {
            LogicalName = "ftp_type_global", Name = "TypeGlobalOld", IsGlobal = true
        };
        globalOptionSet.Values.Add(new OptionSetEnumValue { Value = 100000000, Name = "TypeA" });
        pseudoTable.Enums.Add(globalOptionSet);

        TableFileStore.Save(
            TableFileStore.BuildTableFilePath(_tablesDir, TableFileStore.GlobalOptionSetFileName), pseudoTable);
    }

    private OptionSetEnum LoadEnum(string fileName, string optionSetLogicalName)
    {
        var path = TableFileStore.BuildTableFilePath(_tablesDir, fileName);
        var table = TableFileStore.Load(path);

        return table.Enums.Single(e => e.LogicalName == optionSetLogicalName);
    }
}
