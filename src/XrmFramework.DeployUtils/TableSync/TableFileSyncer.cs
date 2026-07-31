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

            SyncDefinition(def, tablePath);
        }

        if (skippedFrameworkTables.Count > 0)
            AnsiConsole.MarkupLine(
                $"[grey]Skipped[/] [bold]{skippedFrameworkTables.Count}[/] table(s) shipped by the " +
                "framework and not tracked by the project: " +
                string.Join(", ", skippedFrameworkTables.OrderBy(n => n, StringComparer.OrdinalIgnoreCase)));

        // 2. --clean mode: process .table files with no matching Definition
        if (clean)
        {
            CleanOrphanedTableFiles(selectedByTable);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Synchronizing a Definition to its .table
    // ──────────────────────────────────────────────────────────────────────────

    private void SyncDefinition(DefinitionInfo def, string tablePath)
    {
        var isNew = !File.Exists(tablePath);
        var table = isNew ? CreateMinimalTable(def) : LoadTable(tablePath);

        table.Name = def.TableName;

        var added = 0;
        var updated = 0;

        foreach (var colInfo in def.Columns)
        {
            var existing = table.Columns
                .FirstOrDefault(c => string.Equals(
                    c.LogicalName, colInfo.LogicalName, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                // Column absent from the .table → minimal creation
                var newCol = new Column
                {
                    LogicalName = colInfo.LogicalName,
                    Name = colInfo.Name,
                    Selected = true
                };
                table.Columns.Add(newCol);
                added++;
            }
            else if (!existing.Selected)
            {
                existing.Selected = true;
                existing.Name = colInfo.Name;
                updated++;
            }
            // If already Selected=true: nothing to do
        }

        SaveTable(tablePath, table);

        if (isNew)
            AnsiConsole.MarkupLine(
                $"[green]Created[/]    {def.TableName}.table " +
                $"([bold]{def.Columns.Count}[/] column(s))");
        else
            AnsiConsole.MarkupLine(
                $"[blue]Updated[/] {def.TableName}.table " +
                $"(+{added} created, {updated} activated)");
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
                // This file is managed → de-select the columns absent from the Definition
                DeSelectOrphanColumns(tableFile, tableName, selectedByTable[tableName]);
            }
            else
            {
                // No matching Definition in the DLL → delete if tool-generated
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
            // File with CRM data → keep it but de-select everything
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
