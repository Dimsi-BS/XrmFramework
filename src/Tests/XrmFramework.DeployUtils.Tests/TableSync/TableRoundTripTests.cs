// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.Tests.TableSync.Fixtures;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Filet de sécurité sur la fidélité du cycle charger → réécrire d'un fichier .table.
///
/// Toute commande qui met à jour un .table existant (tables sync, tables pull) le désérialise
/// puis le resérialise : une propriété JSON non modélisée dans <see cref="Table" /> ou
/// <see cref="Column" /> serait silencieusement détruite au passage. Ces tests s'exécutent sur
/// les .table réellement livrés par le framework, qui constituent le corpus le plus riche
/// dont on dispose (labels localisés, relations, clés alternatives, marqueurs Locked).
///
/// L'assertion est volontairement asymétrique : on exige qu'aucune information présente à
/// l'origine ne disparaisse, mais on tolère les ajouts. Réécrire une valeur par défaut restée
/// implicite (typiquement "Type": "Boolean", omis par les fichiers antérieurs au forçage de
/// <c>DefaultValueHandling.Include</c> sur <see cref="Column.Type" />) est inoffensif ;
/// perdre un "Locked": true ne l'est pas.
/// </summary>
[TestFixture]
public class TableRoundTripTests
{
    // Réglages identiques à ceux de TableFileSyncer : c'est ce couple qui est utilisé
    // en production pour réécrire les fichiers.
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };

    /// <summary>
    /// Fichiers .table livrés par le framework, découverts en remontant depuis le répertoire
    /// de sortie des tests jusqu'à la racine du dépôt.
    /// </summary>
    private static IEnumerable<TestCaseData> ShippedTableFiles()
    {
        var definitionsDir = RepositoryPaths.ShippedDefinitionsDirectory;

        return Directory.GetFiles(definitionsDir, "*.table")
                        .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                        .Select(path => new TestCaseData(path).SetName(
                            $"RoundTrip_{Path.GetFileNameWithoutExtension(path)}"));
    }

    [TestCaseSource(nameof(ShippedTableFiles))]
    public void RoundTrip_ShippedTableFile_LosesNoInformation(string tablePath)
    {
        var originalJson = File.ReadAllText(tablePath);

        var table = JsonConvert.DeserializeObject<Table>(originalJson);
        Assert.IsNotNull(table, $"Le fichier {Path.GetFileName(tablePath)} doit être désérialisable.");

        var producedJson = JsonConvert.SerializeObject(table, SerializerSettings);

        AssertIsSubsetOf(JToken.Parse(originalJson), JToken.Parse(producedJson), "$");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Assertion
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Vérifie que tout ce que contient <paramref name="original" /> se retrouve à l'identique
    /// dans <paramref name="produced" />. Les propriétés supplémentaires côté produit sont admises.
    /// </summary>
    private static void AssertIsSubsetOf(JToken original, JToken produced, string path)
    {
        switch (original)
        {
            case JObject originalObject:
                var producedObject = produced as JObject;
                Assert.IsNotNull(producedObject, $"{path} : un objet JSON est attendu après réécriture.");

                foreach (var property in originalObject.Properties())
                {
                    var producedProperty = producedObject!.Property(property.Name);

                    Assert.IsNotNull(producedProperty,
                        $"{path}.{property.Name} : propriété perdue lors de la réécriture. " +
                        "La classe du modèle ne la déclare probablement pas — Newtonsoft ignore " +
                        "silencieusement les propriétés JSON inconnues.");

                    AssertIsSubsetOf(property.Value, producedProperty!.Value, $"{path}.{property.Name}");
                }

                break;

            case JArray originalArray:
                var producedArray = produced as JArray;
                Assert.IsNotNull(producedArray, $"{path} : un tableau JSON est attendu après réécriture.");

                Assert.AreEqual(originalArray.Count, producedArray!.Count,
                    $"{path} : le nombre d'éléments a changé lors de la réécriture.");

                if (IsKeyedByLogicalName(originalArray))
                    AssertIsSubsetOfKeyedArray(originalArray, producedArray, path);
                else
                    for (var i = 0; i < originalArray.Count; i++)
                        AssertIsSubsetOf(originalArray[i], producedArray[i], $"{path}[{i}]");

                break;

            default:
                Assert.AreEqual(original.ToString(), produced.ToString(),
                    $"{path} : la valeur a changé lors de la réécriture.");
                break;
        }
    }

    /// <summary>
    /// Vrai si tous les éléments du tableau sont des objets porteurs d'un "LogName".
    /// Ces collections sont indexées par nom logique côté modèle — la colonne "Cols" est stockée
    /// dans un <see cref="ColumnCollection" /> adossé à un SortedList — donc leur ordre d'écriture
    /// n'est pas signifiant et ne doit pas faire échouer le test.
    /// </summary>
    private static bool IsKeyedByLogicalName(JArray array)
        => array.Count > 0 && array.All(item => item is JObject o && o.Property("LogName") != null);

    /// <summary>
    /// Rapproche les éléments par "LogName" plutôt que par position. Le contrôle du nombre
    /// d'éléments ayant déjà eu lieu, un doublon silencieusement absorbé par le modèle se traduit
    /// ici par un nom logique introuvable.
    /// </summary>
    private static void AssertIsSubsetOfKeyedArray(JArray originalArray, JArray producedArray, string path)
    {
        var producedByKey = producedArray
            .GroupBy(item => item["LogName"]!.ToString())
            .ToDictionary(group => group.Key, group => new Queue<JToken>(group));

        foreach (var originalItem in originalArray)
        {
            var key = originalItem["LogName"]!.ToString();

            Assert.IsTrue(producedByKey.TryGetValue(key, out var candidates) && candidates!.Count > 0,
                $"{path} : l'élément « {key} » a disparu lors de la réécriture.");

            AssertIsSubsetOf(originalItem, candidates!.Dequeue(), $"{path}[{key}]");
        }
    }

}
