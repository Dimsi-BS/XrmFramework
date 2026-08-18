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
}
