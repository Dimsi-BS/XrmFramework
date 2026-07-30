// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Lecture de la sélection portée par le répertoire des <c>.table</c>.
/// </summary>
/// <remarks>
/// C'est cette liste que <c>tables pull</c> récupère lorsqu'aucune table n'est demandée
/// explicitement : elle doit refléter le contenu des fichiers, et non leurs noms.
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
        catch { /* meilleur effort */ }
    }

    [Test]
    public void ReadTrackedLogicalNames_ReadsFileContent_NotFileName()
    {
        // Un .table renommé à la main reste le suivi de son entité.
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

        // Le pseudo-table des option sets globaux ne correspond à aucune entité : le demander
        // à l'environnement ne produirait qu'un avertissement « table introuvable ».
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
        File.WriteAllText(Path.Combine(_tablesDir, "Account.cs"), "// généré");

        var tracked = TableFileStore.ReadTrackedLogicalNames(_tablesDir);

        Assert.That(tracked, Is.EquivalentTo(new[] { "account" }));
    }

    [Test]
    public void ReadTrackedLogicalNames_IsCaseInsensitive()
    {
        WriteTable("Account", "account");

        var tracked = TableFileStore.ReadTrackedLogicalNames(_tablesDir);

        // Les noms saisis en ligne de commande ne respectent pas la casse du CRM.
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
