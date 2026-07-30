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
/// <see cref="FrameworkTableCatalog" /> fige la liste des tables livrées par le framework parce
/// qu'elle ne peut pas être déduite de l'assembly à l'exécution. Ces tests sont le garde-fou de
/// cette duplication : ajouter, retirer ou renommer un <c>.table</c> dans
/// <c>src/XrmFramework/Definitions</c> sans mettre l'inventaire à jour casse le build, plutôt que
/// de laisser <c>tables sync</c> redéposer silencieusement un doublon chez les consommateurs.
/// </summary>
[TestFixture]
public class FrameworkTableCatalogTests
{
    private static IReadOnlyList<Table> ShippedTables()
        => Directory.GetFiles(RepositoryPaths.ShippedDefinitionsDirectory, "*.table")
                    .Select(path => JsonConvert.DeserializeObject<Table>(File.ReadAllText(path))!)
                    .ToList();

    /// <summary>Rend deux jeux de noms comparables — et lisibles en cas d'échec.</summary>
    private static string Normalize(IEnumerable<string> names)
        => string.Join(", ", names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase));

    [Test]
    public void Catalog_ListsExactlyTheShippedTableNames()
    {
        Assert.AreEqual(
            Normalize(ShippedTables().Select(t => t.Name)),
            Normalize(FrameworkTableCatalog.TableNames),
            "L'inventaire des noms de table doit correspondre aux .table livrés par le framework.");
    }

    [Test]
    public void Catalog_ListsExactlyTheShippedLogicalNames()
    {
        Assert.AreEqual(
            Normalize(ShippedTables().Select(t => t.LogicalName)),
            Normalize(FrameworkTableCatalog.LogicalNames),
            "L'inventaire des noms logiques doit correspondre aux .table livrés par le framework.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Reconnaissance
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void IsFrameworkTable_MatchesOnTableName_IgnoringCase()
    {
        // Le fichier livré s'appelle Systemuser.table alors que la table se nomme SystemUser :
        // la casse ne doit jamais faire la différence.
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
        // Ces .table vivent dans XrmFramework.DeployUtils : ils ne sont pas livrés aux projets
        // consommateurs, qui doivent donc pouvoir les suivre eux-mêmes.
        Assert.IsFalse(FrameworkTableCatalog.IsFrameworkTable("WebResource", "webresource"));
        Assert.IsFalse(FrameworkTableCatalog.IsFrameworkTable("Publisher", "publisher"));
    }
}
