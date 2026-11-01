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
using XrmFramework.DeployUtils.Tests.TableSync.Fixtures;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// <see cref="FrameworkTableCatalog" /> pins down the list of tables shipped by the framework
/// because it cannot be inferred from the assembly at runtime. These tests are the safety net
/// for that duplication: adding, removing, or renaming a <c>.table</c> in
/// <c>src/XrmFramework/Definitions</c> without updating the inventory breaks the build, rather
/// than letting <c>migrate sync-tables</c> silently redeposit a duplicate at consumers' sites.
/// </summary>
[TestFixture]
public class FrameworkTableCatalogTests
{
    private static IReadOnlyList<Table> ShippedTables()
        => Directory.GetFiles(RepositoryPaths.ShippedDefinitionsDirectory, "*.table")
                    .Select(path => JsonConvert.DeserializeObject<Table>(File.ReadAllText(path))!)
                    .ToList();

    /// <summary>Makes two sets of names comparable — and readable on failure.</summary>
    private static string Normalize(IEnumerable<string> names)
        => string.Join(", ", names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase));

    [Test]
    public void Catalog_ListsExactlyTheShippedTableNames()
    {
        Assert.AreEqual(
            Normalize(ShippedTables().Select(t => t.Name)),
            Normalize(FrameworkTableCatalog.TableNames),
            "The table name inventory must match the .table files shipped by the framework.");
    }

    [Test]
    public void Catalog_ListsExactlyTheShippedLogicalNames()
    {
        Assert.AreEqual(
            Normalize(ShippedTables().Select(t => t.LogicalName)),
            Normalize(FrameworkTableCatalog.LogicalNames),
            "The logical name inventory must match the .table files shipped by the framework.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Recognition
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void IsFrameworkTable_MatchesOnTableName_IgnoringCase()
    {
        // The shipped file is called Systemuser.table while the table is named SystemUser:
        // case must never make a difference.
        Assert.IsTrue(FrameworkTableCatalog.IsFrameworkTable("Systemuser", logicalName: null));
        Assert.IsTrue(FrameworkTableCatalog.IsFrameworkTable("SystemUser", logicalName: null));
    }

    [Test]
    public void IsFrameworkTable_MatchesOnLogicalName_WhenTableWasRenamed()
    {
        Assert.IsTrue(FrameworkTableCatalog.IsFrameworkTable("Utilisateur", "systemuser"));
    }

    [Test]
    public void IsFrameworkTable_IgnoresProjectTables()
    {
        Assert.IsFalse(FrameworkTableCatalog.IsFrameworkTable("Contact", "contact"));
        Assert.IsFalse(FrameworkTableCatalog.IsFrameworkTable(null, null));
    }

    [Test]
    public void IsFrameworkTable_IgnoresDeployUtilsOwnTables()
    {
        // These .table files live in XrmFramework.DeployUtils: they are not shipped to
        // consuming projects, which must therefore be able to track them on their own.
        Assert.IsFalse(FrameworkTableCatalog.IsFrameworkTable("WebResource", "webresource"));
        Assert.IsFalse(FrameworkTableCatalog.IsFrameworkTable("Publisher", "publisher"));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Seeding the locks
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// A project's copy of <c>team</c> as a pull leaves it: the columns it cares about, named after
    /// the CRM schema names, and no lock anywhere — the environment knows nothing of locks.
    /// </summary>
    private static Table ProjectCopyOfTeam()
    {
        var table = new Table { LogicalName = "team", Name = "Team", CollectionName = "teams" };

        table.Columns.Add(new Column
        {
            LogicalName = "teamtype", Name = "TeamType", Selected = true, EnumName = "team|teamtype"
        });
        table.Columns.Add(new Column { LogicalName = "name", Name = "Name", Selected = true });
        table.Columns.Add(new Column { LogicalName = "dpp_custom", Name = "Custom", Selected = true });

        table.Enums.Add(new OptionSetEnum { LogicalName = "team|teamtype", Name = "TypeDEquipe" });

        return table;
    }

    [Test]
    public void SeedLocks_NullTable_SeedsNothing()
    {
        Assert.AreEqual(0, FrameworkTableCatalog.SeedLocks(null));
    }

    [Test]
    public void SeedLocks_TableTheFrameworkDoesNotShip_SeedsNothing()
    {
        var table = new Table { LogicalName = "contact", Name = "Contact" };
        table.Columns.Add(new Column { LogicalName = "firstname", Name = "FirstName", Selected = true });

        Assert.AreEqual(0, FrameworkTableCatalog.SeedLocks(table));
        Assert.IsFalse(table.Columns.Single().IsLocked);
    }

    /// <summary>
    /// The case that started this: the framework ships <c>teamtype</c> locked under the name its own
    /// code refers to, while the project's copy called it something else and knew of no lock.
    /// </summary>
    [Test]
    public void SeedLocks_ProjectCopyOfAFrameworkTable_LocksTheColumnAndRestoresItsName()
    {
        var table = ProjectCopyOfTeam();

        Assert.IsTrue(FrameworkTableCatalog.SeedLocks(table) > 0);

        var teamType = table.Columns.Single(c => c.LogicalName == "teamtype");

        Assert.AreEqual("Type", teamType.Name);
        Assert.IsTrue(teamType.IsLocked);
        Assert.IsTrue(table.IsLocked, "the shipped table is locked, so the project's copy is too");
    }

    [Test]
    public void SeedLocks_ProjectCopyOfAFrameworkTable_LocksTheOptionSetAndRestoresItsName()
    {
        var table = ProjectCopyOfTeam();

        FrameworkTableCatalog.SeedLocks(table);

        var optionSet = table.Enums.Single(e => e.LogicalName == "team|teamtype");

        Assert.AreEqual("TeamType", optionSet.Name);
        Assert.IsTrue(optionSet.IsLocked);
    }

    /// <summary>
    /// The project's own columns are what its copy exists for: seeding must not touch them.
    /// </summary>
    [Test]
    public void SeedLocks_LeavesTheProjectsOwnColumnsAlone()
    {
        var table = ProjectCopyOfTeam();

        FrameworkTableCatalog.SeedLocks(table);

        var custom = table.Columns.Single(c => c.LogicalName == "dpp_custom");

        Assert.AreEqual("Custom", custom.Name);
        Assert.IsFalse(custom.IsLocked);
    }

    /// <summary>
    /// A locked column the project does not track stays out: the framework's own copy declares it,
    /// and adding it here would widen the project's generated surface on its behalf.
    /// </summary>
    [Test]
    public void SeedLocks_DoesNotAddALockedColumnTheCopyDoesNotTrack()
    {
        var table = ProjectCopyOfTeam();
        var before = table.Columns.Count;

        FrameworkTableCatalog.SeedLocks(table);

        Assert.AreEqual(before, table.Columns.Count);
        Assert.IsFalse(table.Columns.Any(c => c.LogicalName == "businessunitid"),
                       "businessunitid is locked by the framework but absent from this copy");
    }

    /// <summary>
    /// Every pull seeds, so a second one must find nothing left to do — otherwise each retrieval
    /// would report work it did not perform.
    /// </summary>
    [Test]
    public void SeedLocks_IsIdempotent()
    {
        var table = ProjectCopyOfTeam();

        var first = FrameworkTableCatalog.SeedLocks(table);

        Assert.IsTrue(first > 0);
        Assert.AreEqual(0, FrameworkTableCatalog.SeedLocks(table));
    }

    /// <summary>
    /// The shipped <c>.table</c> files must be readable from the installed tool, which has no
    /// repository to read them from: they travel as embedded resources, and this is what notices
    /// when one stops being embedded.
    /// </summary>
    [Test]
    public void SeedLocks_ReadsEveryShippedTableThatLocksAColumn()
    {
        var checked_ = 0;

        foreach (var shipped in ShippedTables().Where(t => t.Columns.Any(c => c.IsLocked)))
        {
            var lockedColumn = shipped.Columns.First(c => c.IsLocked);

            var table = new Table { LogicalName = shipped.LogicalName, Name = shipped.Name };
            table.Columns.Add(new Column { LogicalName = lockedColumn.LogicalName, Name = "Renamed" });

            Assert.IsTrue(FrameworkTableCatalog.SeedLocks(table) > 0,
                          $"{shipped.Name} locks columns but seeds nothing — is it still embedded?");
            Assert.AreEqual(lockedColumn.Name, table.Columns.Single().Name);
            checked_++;
        }

        Assert.IsTrue(checked_ > 0, "no shipped table locks a column — this test would prove nothing");
    }
}
