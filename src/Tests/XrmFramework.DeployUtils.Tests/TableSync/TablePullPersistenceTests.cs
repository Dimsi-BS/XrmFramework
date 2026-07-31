// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;
using static XrmFramework.DeployUtils.Tests.TableSync.MetadataFixtureBuilder;
using DataverseMetadata = Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Survival of the selection across a pull, verified <b>end to end</b>: Dataverse metadata
/// → conversion → merge → write to disk → re-read.
/// </summary>
/// <remarks>
/// The tests in <see cref="TablePullMergeTests" /> cover the in-memory merge; these cover
/// what actually reaches the versioned file. The distinction is not theoretical:
/// serialization omits default values, so <c>Select</c> disappears from the JSON as soon as
/// it is <c>false</c>. They go through <see cref="TablePullWriter" />, i.e. exactly the
/// path the command takes.
/// </remarks>
[TestFixture]
public class TablePullPersistenceTests
{
    private static readonly string[] Prefixes = { "ftp" };

    private string _tablesDir = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _tablesDir = Path.Combine(Path.GetTempPath(),
            "XrmFramework.PullPersistence_" + Guid.NewGuid().ToString("N"));
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

    // ══════════════════════════════════════════════════════════════════════════
    // Survival of the selection
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Pull_KeepsColumnSelected_WhenTeamActivatedIt()
    {
        // First pull: ftp_commentaire is not activated by default.
        Pull(Contrat());
        Assert.IsFalse(LoadTable().Columns.Single(c => c.LogicalName == "ftp_commentaire").Selected);

        // The team activates it — either by hand, or via "tables sync" because the code references it.
        Activate("ftp_commentaire");

        // Second pull: the selection must survive.
        Pull(Contrat());

        Assert.IsTrue(LoadTable().Columns.Single(c => c.LogicalName == "ftp_commentaire").Selected,
            "An activated column must survive a pull.");
    }

    [Test]
    public void Pull_WritesSelectTrue_IntoJson()
    {
        // Check against the actual JSON rather than the object model: serialization omits
        // default values, so a mis-set Select would silently disappear from the file.
        Pull(Contrat());
        Activate("ftp_commentaire");
        Pull(Contrat());

        var colonne = JObject.Parse(File.ReadAllText(TableFilePath()))["Cols"]!
            .Single(c => (string?)c["LogName"] == "ftp_commentaire");

        Assert.AreEqual(true, (bool?)colonne["Select"],
            "The written file must carry \"Select\": true.");
    }

    [Test]
    public void Pull_KeepsSelection_OfColumnRemovedFromEnvironment()
    {
        Pull(Contrat());
        Activate("ftp_commentaire");

        // The column disappears from the environment: a pull refreshes, it does not destroy.
        Pull(ContratSansCommentaire());

        var orpheline = LoadTable().Columns.SingleOrDefault(c => c.LogicalName == "ftp_commentaire");

        Assert.IsNotNull(orpheline, "The column absent from the CRM must be kept.");
        Assert.IsTrue(orpheline!.Selected, "And keep its selection.");
    }

    [Test]
    public void Pull_DoesNotReactivate_DeliberatelyDeselectedSystemColumn()
    {
        // createdon is activated by default on creation; if the team deactivates it,
        // a pull must not reverse that decision.
        Pull(Contrat());
        Deactivate("createdon");

        Pull(Contrat());

        Assert.IsFalse(LoadTable().Columns.Single(c => c.LogicalName == "createdon").Selected);
    }

    [Test]
    public void Pull_KeepsSelection_WhenFileWasRenamedByHand()
    {
        Pull(Contrat());
        Activate("ftp_commentaire");

        // The team renames the C# type and the file; the match is made on LogName.
        RenameTableFile("ContratDeLocation");

        Pull(Contrat());

        Assert.AreEqual(1, Directory.GetFiles(_tablesDir, "*.table").Length,
            "The renamed file must be updated, not duplicated.");
        Assert.IsTrue(LoadTable().Columns.Single(c => c.LogicalName == "ftp_commentaire").Selected);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Idempotence
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Pull_IsIdempotent_WhenNothingChanged()
    {
        Pull(Contrat());
        Activate("ftp_commentaire");
        Pull(Contrat());

        var apresPremier = File.ReadAllText(TableFilePath());

        Pull(Contrat());

        Assert.AreEqual(apresPremier, File.ReadAllText(TableFilePath()),
            "A second, identical pull must produce an empty diff.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Executes the command's actual path: conversion, merge, write.</summary>
    private void Pull(DataverseMetadata.EntityMetadata entity)
        => TablePullWriter.Write(_tablesDir, MetadataTableFactory.Convert(entity, Prefixes).Table);

    private string TableFilePath()
        => Directory.GetFiles(_tablesDir, "*.table").Single();

    private Table LoadTable() => TableFileStore.Load(TableFilePath());

    private void Activate(string logicalName) => SetSelected(logicalName, true);

    private void Deactivate(string logicalName) => SetSelected(logicalName, false);

    private void SetSelected(string logicalName, bool selected)
    {
        var path = TableFilePath();
        var table = TableFileStore.Load(path);

        table.Columns.Single(c => c.LogicalName == logicalName).Selected = selected;

        TableFileStore.Save(path, table);
    }

    private void RenameTableFile(string newName)
    {
        var path = TableFilePath();
        var table = TableFileStore.Load(path);

        table.Name = newName;

        File.Delete(path);
        TableFileStore.Save(TableFileStore.BuildTableFilePath(_tablesDir, newName), table);
    }

    // ── Simulated metadata ────────────────────────────────────────────────────

    private static DataverseMetadata.EntityMetadata Contrat()
        => Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", "ftp_nom",
            Attribute("ftp_contratid", "ftp_ContratId", DataverseMetadata.AttributeTypeCode.Uniqueidentifier),
            StringAttribute("ftp_nom", "ftp_Nom", 100),
            Attribute("createdon", "CreatedOn", DataverseMetadata.AttributeTypeCode.DateTime),
            StringAttribute("ftp_commentaire", "ftp_Commentaire", 2000));

    private static DataverseMetadata.EntityMetadata ContratSansCommentaire()
        => Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", "ftp_nom",
            Attribute("ftp_contratid", "ftp_ContratId", DataverseMetadata.AttributeTypeCode.Uniqueidentifier),
            StringAttribute("ftp_nom", "ftp_Nom", 100),
            Attribute("createdon", "CreatedOn", DataverseMetadata.AttributeTypeCode.DateTime));
}
