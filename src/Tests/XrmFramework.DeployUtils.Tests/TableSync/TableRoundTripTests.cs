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
/// Safety net for the fidelity of a .table file's load -> rewrite cycle.
///
/// Any command that updates an existing .table (migrate sync-tables, tables pull) deserializes it
/// then reserializes it: a JSON property not modeled in <see cref="Table" /> or
/// <see cref="Column" /> would be silently destroyed in the process. These tests run against
/// the .table files actually shipped by the framework, which are the richest corpus
/// available (localized labels, relationships, alternate keys, Locked markers).
///
/// The assertion is deliberately asymmetric: no information present in the original may
/// disappear, but additions are tolerated. Rewriting a default value that was left
/// implicit (typically "Type": "Boolean", omitted by files predating the forcing of
/// <c>DefaultValueHandling.Include</c> on <see cref="Column.Type" />) is harmless;
/// losing a "Locked": true is not.
/// </summary>
[TestFixture]
public class TableRoundTripTests
{
    // Settings identical to TableFileSyncer's: this is the pair actually used
    // in production to rewrite the files.
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };

    /// <summary>
    /// .table files shipped by the framework, discovered by walking up from the test
    /// output directory to the repository root.
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
        Assert.IsNotNull(table, $"File {Path.GetFileName(tablePath)} must be deserializable.");

        var producedJson = JsonConvert.SerializeObject(table, SerializerSettings);

        AssertIsSubsetOf(JToken.Parse(originalJson), JToken.Parse(producedJson), "$");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Assertion
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Checks that everything contained in <paramref name="original" /> is found unchanged
    /// in <paramref name="produced" />. Extra properties on the produced side are allowed.
    /// </summary>
    private static void AssertIsSubsetOf(JToken original, JToken produced, string path)
    {
        switch (original)
        {
            case JObject originalObject:
                var producedObject = produced as JObject;
                Assert.IsNotNull(producedObject, $"{path}: a JSON object is expected after rewriting.");

                foreach (var property in originalObject.Properties())
                {
                    var producedProperty = producedObject!.Property(property.Name);

                    Assert.IsNotNull(producedProperty,
                        $"{path}.{property.Name}: property lost during rewriting. " +
                        "The model class probably doesn't declare it — Newtonsoft silently " +
                        "ignores unknown JSON properties.");

                    AssertIsSubsetOf(property.Value, producedProperty!.Value, $"{path}.{property.Name}");
                }

                break;

            case JArray originalArray:
                var producedArray = produced as JArray;
                Assert.IsNotNull(producedArray, $"{path}: a JSON array is expected after rewriting.");

                Assert.AreEqual(originalArray.Count, producedArray!.Count,
                    $"{path}: the number of elements changed during rewriting.");

                if (IsKeyedByLogicalName(originalArray))
                    AssertIsSubsetOfKeyedArray(originalArray, producedArray, path);
                else
                    for (var i = 0; i < originalArray.Count; i++)
                        AssertIsSubsetOf(originalArray[i], producedArray[i], $"{path}[{i}]");

                break;

            default:
                Assert.AreEqual(original.ToString(), produced.ToString(),
                    $"{path}: the value changed during rewriting.");
                break;
        }
    }

    /// <summary>
    /// True if every element of the array is an object carrying a "LogName".
    /// These collections are indexed by logical name on the model side — the "Cols" column is
    /// stored in a <see cref="ColumnCollection" /> backed by a SortedList — so their write order
    /// is not meaningful and must not cause the test to fail.
    /// </summary>
    private static bool IsKeyedByLogicalName(JArray array)
        => array.Count > 0 && array.All(item => item is JObject o && o.Property("LogName") != null);

    /// <summary>
    /// Matches elements by "LogName" rather than by position. Since the element-count check has
    /// already taken place, a duplicate silently absorbed by the model shows up here as a
    /// logical name that cannot be found.
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
                $"{path}: element \"{key}\" disappeared during rewriting.");

            AssertIsSubsetOf(originalItem, candidates!.Dequeue(), $"{path}[{key}]");
        }
    }

}
