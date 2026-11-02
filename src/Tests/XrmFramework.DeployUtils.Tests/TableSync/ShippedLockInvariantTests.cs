// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.Tests.TableSync.Fixtures;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// The framework's own code refers to constants generated from the <c>.table</c> files it ships —
/// <c>TeamDefinition.Columns.Type</c>, <c>SystemUserDefinition.Columns.AccessMode</c>. A consuming
/// project may track its own copy of one of those tables, pulled from the environment under the CRM
/// schema names, and the name it gave a column there used to win the fold: the project compiled,
/// the framework's own code did not.
/// </summary>
/// <remarks>
/// <c>"Locked": true</c> is what settles that — the generators keep the locked name, and
/// <c>FrameworkTableCatalog.SeedLocks</c> carries it onto the project's copy — so every column the
/// framework names in its code has to carry it. Nothing but this test notices when a new reference
/// is written against a column that does not, and the symptom surfaces in a consumer's build rather
/// than here.
/// </remarks>
[TestFixture]
public class ShippedLockInvariantTests
{
    /// <summary>
    /// <c>XDefinition.Columns.Y</c>, the shape of every reference to a generated column constant.
    /// </summary>
    private static readonly Regex ColumnReference =
        new Regex(@"(\w+)Definition\.Columns\.(\w+)\b", RegexOptions.Compiled);

    /// <summary>
    /// Directories whose code does not ship, and whose references therefore bind nothing at a
    /// consumer's site: they are renamed alongside the <c>.table</c> they read, in one commit.
    /// </summary>
    private static readonly string[] NotShipped =
    {
        Path.DirectorySeparatorChar + "Tests" + Path.DirectorySeparatorChar,
        Path.DirectorySeparatorChar + "DemoPlugin" + Path.DirectorySeparatorChar,
        Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar,
        Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar
    };

    /// <summary>One reference, and the file it was read in — so a failure can be gone to.</summary>
    private sealed class Reference
    {
        public Reference(string tableName, string columnName, string file)
        {
            TableName = tableName;
            ColumnName = columnName;
            File = file;
        }

        public string TableName { get; }

        public string ColumnName { get; }

        public string File { get; }

        public override string ToString() => $"{TableName}Definition.Columns.{ColumnName} ({File})";
    }

    private static IReadOnlyDictionary<string, Table> ShippedByTableName()
        => Directory.GetFiles(RepositoryPaths.ShippedDefinitionsDirectory, "*.table")
                    .Select(path => JsonConvert.DeserializeObject<Table>(File.ReadAllText(path))!)
                    .Where(table => !string.IsNullOrEmpty(table.Name))
                    .ToDictionary(table => table.Name!, StringComparer.Ordinal);

    /// <summary>
    /// Every reference the shipping code makes to a column of a table the framework ships.
    /// </summary>
    private static IReadOnlyList<Reference> ReferencesToShippedTables()
    {
        var shipped = ShippedByTableName();
        var source = RepositoryPaths.SourceDirectory;
        var references = new List<Reference>();

        foreach (var path in Directory.EnumerateFiles(source, "*.cs", SearchOption.AllDirectories))
        {
            if (NotShipped.Any(fragment => path.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0))
                continue;

            foreach (Match match in ColumnReference.Matches(File.ReadAllText(path)))
            {
                var tableName = match.Groups[1].Value;

                // A project table named in framework code is a sample or a test fixture, and the
                // framework ships no .table for it to lock anything in.
                if (shipped.ContainsKey(tableName))
                    references.Add(new Reference(tableName, match.Groups[2].Value,
                                                 Path.GetFileName(path)));
            }
        }

        return references;
    }

    /// <summary>
    /// Guards the two tests below: a scan that finds nothing would pass them both while proving
    /// nothing at all.
    /// </summary>
    [Test]
    public void TheScan_FindsReferencesToLockAtAll()
    {
        Assert.IsTrue(ReferencesToShippedTables().Count > 0,
                      "no reference to a shipped column was found — has the source layout moved?");
    }

    [Test]
    public void EveryColumnNamedByFrameworkCode_IsLockedByTheShippedTable()
    {
        var shipped = ShippedByTableName();

        var unlocked = ReferencesToShippedTables()
            .Where(reference =>
            {
                var column = shipped[reference.TableName].Columns.FirstOrDefault(
                    c => string.Equals(c.Name, reference.ColumnName, StringComparison.Ordinal));

                return column != null && !column.IsLocked;
            })
            .Select(reference => reference.ToString())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(text => text, StringComparer.Ordinal)
            .ToList();

        Assert.IsEmpty(
            unlocked,
            "The framework's code names these columns, so a project's copy of the table must not be "
            + "able to rename them: mark each one \"Locked\": true in the shipped .table."
            + Environment.NewLine + string.Join(Environment.NewLine, unlocked));
    }

    /// <summary>
    /// The mirror case: a constant the framework names but its own <c>.table</c> does not declare
    /// exists only by the grace of the consuming project selecting that column. There is nothing to
    /// lock, and nothing to stop the reference breaking.
    /// </summary>
    [Test]
    public void EveryColumnNamedByFrameworkCode_IsDeclaredByTheShippedTable()
    {
        var shipped = ShippedByTableName();

        var undeclared = ReferencesToShippedTables()
            .Where(reference => !shipped[reference.TableName].Columns.Any(
                       c => string.Equals(c.Name, reference.ColumnName, StringComparison.Ordinal)))
            .Select(reference => reference.ToString())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(text => text, StringComparer.Ordinal)
            .ToList();

        Assert.IsEmpty(
            undeclared,
            "The framework's code names these columns, but the .table it ships declares none of "
            + "them: the constant exists only if the consuming project happens to select the column."
            + Environment.NewLine + string.Join(Environment.NewLine, undeclared));
    }
}
