// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Spectre.Console;
using CoreTable = XrmFramework.Core.Table;
using XrmFramework.Core;

namespace XrmFramework.DeployUtils.TableSync
{

/// <summary>
/// Synchronizes the .table files of a directory with the information
/// extracted from the *Definition classes of an assembly.
/// </summary>
/// <remarks>
/// Two kinds of information travel this way, and both are C# identifiers the CRM knows nothing
/// about: which columns the code actually references — they become <c>Select: true</c> — and the
/// name each option set is compiled under, read from <c>[OptionSet(typeof(...))]</c>. Under 2.*
/// both lived in the generated <c>.cs</c>; from 3.1 on the generator reads them from the
/// <c>.table</c>, so a migration has to move them across or the upgraded project no longer compiles.
/// </remarks>
public sealed class TableFileSyncer
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };

    private readonly string _tablesDirectory;

    public TableFileSyncer(string tablesDirectory)
    {
        if (!Directory.Exists(tablesDirectory))
            throw new DirectoryNotFoundException(
                $"Table directory not found: {tablesDirectory}");

        _tablesDirectory = tablesDirectory;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Main entry point
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Synchronizes the .table files according to the provided <paramref name="definitions"/>.
    /// </summary>
    /// <param name="definitions">Information extracted from the DLL by <see cref="DefinitionAnalyzer"/>.</param>
    /// <param name="clean">
    ///   If true, sets Select=false on columns absent from any Definition
    ///   and deletes .table files entirely created by the tool (no real CRM data).
    /// </param>
    /// <remarks>
    ///   Tables shipped by the framework (see <see cref="FrameworkTableCatalog"/>) are
    ///   never created here — their .table belongs to the XrmFramework package — but are still kept
    ///   up to date when the project already tracks a copy of them.
    /// </remarks>
    public void Sync(IReadOnlyList<DefinitionInfo> definitions, bool clean = false)
    {
        // Index of logical names selected per entity, used in --clean mode
        // to identify columns that no longer belong to any Definition.
        var selectedByTable = definitions.GroupBy(d => d.TableName).ToDictionary(
            g => g.Key,
            g => new HashSet<string>(
                     g.SelectMany(d => d.Columns.Select(c => c.LogicalName)),
                     StringComparer.OrdinalIgnoreCase),
            StringComparer.OrdinalIgnoreCase);

        AnsiConsole.MarkupLine($"[bold]{definitions.Count}[/] Definition class(es) found in the DLL.");

        var skippedFrameworkTables = new List<string>();

        // Global option sets live in their own pseudo-table, so renaming one found on a column
        // means reaching outside the table being synchronized.
        var globalOptionSets = LoadGlobalOptionSets();

        // 1. Update / create the .table files for each Definition
        foreach (var def in definitions)
        {
            var tablePath = TablePath(def.TableName);

            // The .table files for tables shipped by the framework (SystemUser, Role, ...) are part
            // of the XrmFramework package: they are compiled into the consumer project, so their
            // Definitions appear in the analyzed DLL. Dropping a copy here would produce a
            // duplicate. However, once the file exists, the project has deliberately chosen
            // to track the table in order to enrich it with its own columns — the framework's
            // columns being marked "Locked": true — and it is then synchronized like the others.
            if (!File.Exists(tablePath) && FrameworkTableCatalog.IsFrameworkTable(def))
            {
                skippedFrameworkTables.Add(def.TableName);
                continue;
            }

            SyncDefinition(def, tablePath, globalOptionSets);
        }

        if (skippedFrameworkTables.Count > 0)
            AnsiConsole.MarkupLine(
                $"[grey]Skipped[/] [bold]{skippedFrameworkTables.Count}[/] table(s) shipped by the " +
                "framework and not tracked by the project: " +
                string.Join(", ", skippedFrameworkTables.OrderBy(n => n, StringComparer.OrdinalIgnoreCase)));

        globalOptionSets.SaveIfRenamed();

        // 2. --clean mode: process .table files with no matching Definition
        if (clean)
        {
            CleanOrphanedTableFiles(selectedByTable);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Synchronizing a Definition to its .table
    // ──────────────────────────────────────────────────────────────────────────

    private void SyncDefinition(DefinitionInfo def, string tablePath, GlobalOptionSetTable globalOptionSets)
    {
        var isNew = !File.Exists(tablePath);
        var table = isNew ? CreateMinimalTable(def) : LoadTable(tablePath);

        table.Name = def.TableName;

        var added = 0;
        var updated = 0;
        var renamedOptionSets = 0;

        foreach (var colInfo in def.Columns)
        {
            var existing = table.Columns
                .FirstOrDefault(c => string.Equals(
                    c.LogicalName, colInfo.LogicalName, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                // Column absent from the .table -> minimal creation
                var newCol = new Column
                {
                    LogicalName = colInfo.LogicalName,
                    Name = colInfo.Name,
                    Selected = true
                };
                table.Columns.Add(newCol);
                added++;
                existing = newCol;
            }
            else if (!existing.Selected)
            {
                existing.Selected = true;
                existing.Name = colInfo.Name;
                updated++;
            }
            // If already Selected=true: nothing to do

            if (ApplyOptionSetName(table, existing, colInfo, globalOptionSets))
                renamedOptionSets++;
        }

        SaveTable(tablePath, table);

        if (isNew)
            AnsiConsole.MarkupLine(
                $"[green]Created[/]    {def.TableName}.table " +
                $"([bold]{def.Columns.Count}[/] column(s))");
        else
            AnsiConsole.MarkupLine(
                $"[blue]Updated[/] {def.TableName}.table " +
                $"(+{added} created, {updated} activated" +
                (renamedOptionSets > 0 ? $", {renamedOptionSets} option set(s) named" : string.Empty) + ")");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Option set names
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Carries the C# name read from <c>[OptionSet(typeof(...))]</c> over to the option set the column
    /// points at. Returns true if a name was actually applied.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The CRM only knows the option set's logical name; its C# identifier is a project decision that
    /// compiled code depends on. Under 2.* it lived in the generated <c>.cs</c>; from 3.1 on the
    /// generator reads it from the <c>.table</c>, so the migration has to move it across.
    /// The column links the two: its <c>EnumName</c> is the logical name to look up.
    /// </para>
    /// <para>
    /// The same option set can be recorded <b>twice</b>: the 2.* DefinitionManager kept in a table's
    /// own <c>Enums</c> every option set one of its columns referenced — globals included — while also
    /// writing the globals to <c>OptionSet.table</c>. Both copies reach the generator, which unions
    /// them, so both have to be renamed: stopping at the first one found would leave the other holding
    /// the old name.
    /// </para>
    /// </remarks>
    private static bool ApplyOptionSetName(CoreTable table, Column column, DefinitionColumnInfo colInfo,
                                           GlobalOptionSetTable globalOptionSets)
    {
        if (string.IsNullOrEmpty(colInfo.OptionSetName) || string.IsNullOrEmpty(column.EnumName))
            return false;

        var renamed = false;

        foreach (var local in table.Enums)
            if (string.Equals(local.LogicalName, column.EnumName, StringComparison.OrdinalIgnoreCase))
                renamed |= Rename(local, colInfo, table.Name);

        renamed |= globalOptionSets.Rename(column.EnumName, colInfo);

        return renamed;
    }

    /// <summary>
    /// Applies to <paramref name="optionSet"/> the enum name and the member names the code compiles
    /// against, unless it is frozen.
    /// </summary>
    private static bool Rename(OptionSetEnum optionSet, DefinitionColumnInfo colInfo, string owner)
    {
        // "Locked" marks the option sets shipped by the framework: their names are part of the
        // package's own generated code, and renaming them here would break it.
        if (optionSet.IsLocked)
        {
            if (!string.Equals(optionSet.Name, colInfo.OptionSetName, StringComparison.Ordinal))
                AnsiConsole.MarkupLine(
                    $"[grey]Kept[/] the frozen name [bold]{optionSet.Name}[/] for {optionSet.LogicalName} " +
                    $"(the code says {colInfo.OptionSetName})");

            return false;
        }

        var renamed = RenameOptionSet(optionSet, colInfo.OptionSetName, owner);
        renamed |= RenameValues(optionSet, colInfo, owner);

        return renamed;
    }

    private static bool RenameOptionSet(OptionSetEnum optionSet, string name, string owner)
    {
        if (string.Equals(optionSet.Name, name, StringComparison.Ordinal))
            return false;

        if (!string.IsNullOrEmpty(optionSet.Name))
            AnsiConsole.MarkupLine(
                $"[yellow]Renamed[/] option set {optionSet.LogicalName} in {owner}: " +
                $"{optionSet.Name} -> [bold]{name}[/]");

        optionSet.Name = name;
        return true;
    }

    /// <summary>
    /// Carries the enum member names over to the option set's values, matched on the numeric value.
    /// </summary>
    /// <remarks>
    /// The generator derives a member name from the CRM label and strips its diacritics
    /// (<c>Modèle</c> becomes <c>Modele</c>), but teams rename them, and every <c>MyEnum.EnCours</c>
    /// in the project's code compiles against the result. Recovering them is what keeps that code
    /// building after the migration.
    /// </remarks>
    private static bool RenameValues(OptionSetEnum optionSet, DefinitionColumnInfo colInfo, string owner)
    {
        var names = BuildValueLookup(colInfo.OptionSetValues, optionSet.HasNullValue);

        if (names.Count == 0)
            return false;

        var changes = new List<string>();

        foreach (var value in optionSet.Values)
        {
            string name;
            if (!names.TryGetValue(value.Value, out name)
                || string.Equals(value.Name, name, StringComparison.Ordinal))
                continue;

            changes.Add($"{value.Name} -> {name}");
            value.Name = name;
        }

        if (changes.Count == 0)
            return false;

        AnsiConsole.MarkupLine(
            $"[yellow]Renamed[/] [bold]{changes.Count}[/] member(s) of {optionSet.Name} in {owner}: " +
            string.Join(", ", changes.ToArray()));

        return true;
    }

    /// <summary>
    /// Indexes the assembly's enum members by value, dropping what cannot be matched with certainty.
    /// </summary>
    /// <remarks>
    /// Two members can share a value — the generator emits a synthetic <c>Null = 0</c> ahead of the
    /// real ones for an option set that allows an empty value, and C# permits aliases besides. The
    /// synthetic member mirrors <c>HasNullValue</c> rather than a CRM option, so it is skipped;
    /// any remaining collision is genuinely ambiguous and its value is left untouched.
    /// </remarks>
    private static Dictionary<int, string> BuildValueLookup(
        IReadOnlyList<DefinitionOptionSetValue> values, bool hasNullValue)
    {
        var lookup = new Dictionary<int, string>();
        var ambiguous = new List<int>();

        foreach (var value in values)
        {
            if (hasNullValue && value.Value == 0 && string.Equals(value.Name, "Null", StringComparison.Ordinal))
                continue;

            if (lookup.ContainsKey(value.Value))
            {
                ambiguous.Add(value.Value);
                continue;
            }

            lookup[value.Value] = value.Name;
        }

        foreach (var value in ambiguous)
            lookup.Remove(value);

        return lookup;
    }

    /// <summary>
    /// The <c>OptionSet.table</c> pseudo-table, loaded once and written back only if a name changed.
    /// </summary>
    private sealed class GlobalOptionSetTable
    {
        private readonly string _path;
        private readonly CoreTable _table;
        private bool _renamed;

        public GlobalOptionSetTable(string path, CoreTable table)
        {
            _path = path;
            _table = table;
        }

        public bool Rename(string logicalName, DefinitionColumnInfo colInfo)
        {
            if (_table == null)
                return false;

            var optionSet = _table.Enums.FirstOrDefault(
                e => string.Equals(e.LogicalName, logicalName, StringComparison.OrdinalIgnoreCase));

            if (optionSet == null
                || !TableFileSyncer.Rename(optionSet, colInfo, TableFileStore.GlobalOptionSetFileName))
                return false;

            _renamed = true;
            return true;
        }

        public void SaveIfRenamed()
        {
            if (_renamed)
                SaveTable(_path, _table);
        }
    }

    /// <summary>
    /// Loads the global option sets pseudo-table, located by its content rather than its file name.
    /// </summary>
    private GlobalOptionSetTable LoadGlobalOptionSets()
    {
        foreach (var path in Directory.GetFiles(_tablesDirectory, "*" + TableFileStore.TableFileExtension))
        {
            CoreTable table;
            try
            {
                table = LoadTable(path);
            }
            catch (Exception)
            {
                // An unreadable file is not this step's problem; it is reported where it is written.
                continue;
            }

            if (string.Equals(table.LogicalName, TableFileStore.GlobalOptionSetLogicalName,
                              StringComparison.OrdinalIgnoreCase))
                return new GlobalOptionSetTable(path, table);
        }

        return new GlobalOptionSetTable(null, null);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // --clean mode
    // ──────────────────────────────────────────────────────────────────────────

    private void CleanOrphanedTableFiles(
        Dictionary<string, HashSet<string>> selectedByTable)
    {
        var allTableFiles = Directory.GetFiles(_tablesDirectory, "*.table");

        // Set of names managed by the DLL's Definitions
        var managedNames = new HashSet<string>(selectedByTable.Keys, StringComparer.OrdinalIgnoreCase);

        foreach (var tableFile in allTableFiles)
        {
            var tableName = Path.GetFileNameWithoutExtension(tableFile);

            if (managedNames.Contains(tableName))
            {
                // This file is managed -> de-select the columns absent from the Definition
                DeSelectOrphanColumns(tableFile, tableName, selectedByTable[tableName]);
            }
            else
            {
                // No matching Definition in the DLL -> delete if tool-generated
                HandleFullyOrphanedFile(tableFile, tableName);
            }
        }
    }

    /// <summary>
    /// Sets Select=false on the columns of a managed .table that are no longer in its Definition.
    /// </summary>
    private void DeSelectOrphanColumns(string tableFile, string tableName,
        HashSet<string> definitionLogicalNames)
    {
        var table = LoadTable(tableFile);
        var deSelected = 0;

        foreach (var col in table.Columns)
        {
            if (col.Selected && !definitionLogicalNames.Contains(col.LogicalName))
            {
                col.Selected = false;
                deSelected++;
            }
        }

        if (deSelected > 0)
        {
            SaveTable(tableFile, table);
            AnsiConsole.MarkupLine(
                $"[yellow]Cleaned[/]  {tableName}.table " +
                $"([bold]{deSelected}[/] column(s) de-selected)");
        }
    }

    /// <summary>
    /// Processes a .table with no matching Definition:
    /// deletes it if entirely tool-generated (no column with CRM Labels),
    /// otherwise de-selects all columns and keeps the file.
    /// </summary>
    private void HandleFullyOrphanedFile(string tableFile, string tableName)
    {
        var table = LoadTable(tableFile);

        // The global option sets pseudo-table describes no entity, so no Definition will ever claim
        // it, and it holds no column to vouch for its content: both heuristics below would condemn
        // it. It is identified by its logical name rather than its file name, so that a real table
        // that happens to be called OptionSet is still processed normally.
        if (string.Equals(table.LogicalName, TableFileStore.GlobalOptionSetLogicalName,
                          StringComparison.OrdinalIgnoreCase))
            return;

        // "Tool-generated" heuristic: no column has Labels (real CRM data).
        // A .table produced by the DefinitionManager always has Labels on its columns.
        var hasRealCrmData = table.Columns.Any(c => c.Labels.Count > 0);

        if (!hasRealCrmData)
        {
            File.Delete(tableFile);
            AnsiConsole.MarkupLine(
                $"[red]Deleted[/] {tableName}.table " +
                "(no Definition, no real CRM data)");
        }
        else
        {
            // File with CRM data -> keep it but de-select everything
            var deSelected = 0;
            foreach (var col in table.Columns.Where(c => c.Selected))
            {
                col.Selected = false;
                deSelected++;
            }

            if (deSelected > 0)
            {
                SaveTable(tableFile, table);
                AnsiConsole.MarkupLine(
                    $"[yellow]Kept[/] {tableName}.table " +
                    $"(no Definition — {deSelected} column(s) de-selected)");
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers I/O
    // ──────────────────────────────────────────────────────────────────────────

    private string TablePath(string tableName)
        => Path.Combine(_tablesDirectory, $"{tableName}.table");

    private static CoreTable CreateMinimalTable(DefinitionInfo def) => new()
    {
        LogicalName = def.EntityName,
        Name = def.TableName,
        CollectionName = def.EntityCollectionName ?? string.Empty
    };

    private static CoreTable LoadTable(string path)
    {
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<CoreTable>(json)
               ?? throw new InvalidDataException($"Unable to deserialize {path}");
    }

    private static void SaveTable(string path, CoreTable table)
    {
        var json = JsonConvert.SerializeObject(table, SerializerSettings);
        File.WriteAllText(path, json);
    }
}
}
