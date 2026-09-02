// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;
using XrmFramework.Core;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// <c>xrmframework tables columns</c> commands: activate or adjust columns of already
    /// tracked <c>.table</c> files, entirely offline.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="CrmTableHelper" />, none of these entry points connects to the
    /// environment: they only read and write the <c>.table</c> files already produced by
    /// <c>tables pull</c> or <c>migrate sync-tables</c>, via <see cref="TableFileStore" />.
    /// In accordance with the CLI contract, they return an exit code and never call
    /// <c>Environment.Exit</c>.
    /// </remarks>
    public static class ColumnHelper
    {
        /// <summary>Success.</summary>
        public const int ExitSuccess = 0;

        /// <summary>No table or column matches the requested criteria.</summary>
        public const int ExitNoMatch = 1;

        /// <summary>Configuration or directory not found.</summary>
        public const int ExitNotFound = 2;

        /// <summary>Unexpected error, or a requested change conflicts with the file's content.</summary>
        public const int ExitError = 3;

        // ══════════════════════════════════════════════════════════════════════
        // tables columns list
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lists the columns already tracked in one or more local <c>.table</c> files.
        /// Without <c>--table</c> or <c>--prefix</c>, inspects every table already tracked
        /// by the project.
        /// </summary>
        public static int List(
            string projectRoot,
            string tablesDirectory,
            IEnumerable<string> tableNames,
            string prefix,
            string filter,
            bool unselectedOnly)
        {
            try
            {
                var directory = ResolveTablesDirectory(projectRoot, tablesDirectory, out var errorCode);
                if (directory == null)
                    return errorCode;

                var names = CrmTableHelper.SplitNames(tableNames).ToList();
                var (files, unknownTables) = ResolveTableFiles(directory, names, prefix, defaultToAllWhenNoCriteria: true);

                foreach (var name in unknownTables)
                    AnsiConsole.MarkupLine($"[yellow]No local .table for:[/] {Markup.Escape(name)}");

                if (files.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No table matches the criteria.[/]");
                    return ExitNoMatch;
                }

                var totalColumns = 0;

                foreach (var (path, table) in files)
                {
                    var columns = table.Columns
                        .Where(c => MatchesColumn(c, filter, unselectedOnly))
                        .OrderBy(c => c.LogicalName, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine(
                        $"[bold]{Markup.Escape(table.Name ?? Path.GetFileNameWithoutExtension(path))}[/] " +
                        $"([cyan]{Markup.Escape(table.LogicalName ?? string.Empty)}[/]) — " +
                        Markup.Escape(Path.GetFileName(path)));

                    if (columns.Count == 0)
                    {
                        AnsiConsole.MarkupLine("  [grey]No column matches the criteria.[/]");
                        continue;
                    }

                    RenderColumnTable(columns);
                    totalColumns += columns.Count;
                }

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[bold]{totalColumns}[/] column(s) across [bold]{files.Count}[/] table(s).");
                return ExitSuccess;
            }
            catch (Exception ex)
            {
                return CrmTableHelper.ReportUnexpected(ex);
            }
        }

        private static bool MatchesColumn(Column column, string filter, bool unselectedOnly)
        {
            if (unselectedOnly && column.Selected)
                return false;

            if (string.IsNullOrWhiteSpace(filter))
                return true;

            return (column.LogicalName ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                   || (column.Name ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void RenderColumnTable(IEnumerable<Column> columns)
        {
            var grid = new Spectre.Console.Table().Border(TableBorder.Rounded);
            grid.AddColumn("Logical name");
            grid.AddColumn("Name");
            grid.AddColumn("Type");
            grid.AddColumn("Selected");
            grid.AddColumn("Locked");

            foreach (var column in columns)
            {
                grid.AddRow(
                    Markup.Escape(column.LogicalName ?? string.Empty),
                    Markup.Escape(column.Name ?? string.Empty),
                    Markup.Escape(column.Type.ToString()),
                    column.Selected ? "[green]yes[/]" : string.Empty,
                    column.IsLocked ? "[yellow]yes[/]" : string.Empty);
            }

            AnsiConsole.Write(grid);
        }

        // ══════════════════════════════════════════════════════════════════════
        // tables columns add
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Activates (<c>Select: true</c>) columns already present in one or more local
        /// <c>.table</c> files. Requires an explicit <c>--table</c> or <c>--prefix</c>: unlike
        /// <see cref="List" />, this command mutates files, so it never defaults to the whole
        /// project.
        /// </summary>
        public static int Add(
            string projectRoot,
            string tablesDirectory,
            IEnumerable<string> tableNames,
            string prefix,
            IEnumerable<string> columnNames,
            bool all,
            bool noPrompt)
        {
            try
            {
                var names = CrmTableHelper.SplitNames(tableNames).ToList();

                if (names.Count == 0 && string.IsNullOrWhiteSpace(prefix))
                {
                    AnsiConsole.MarkupLine(
                        "[red]Specify the table(s) to edit via[/] [cyan]--table[/] [red]or[/] [cyan]--prefix[/].");
                    return ExitNoMatch;
                }

                var columns = CrmTableHelper.SplitNames(columnNames).ToList();

                if (columns.Count == 0 && !all)
                {
                    AnsiConsole.MarkupLine(
                        "[red]Specify the column(s) to activate via[/] [cyan]--column[/] [red]or[/] [cyan]--all[/].");
                    return ExitNoMatch;
                }

                var directory = ResolveTablesDirectory(projectRoot, tablesDirectory, out var errorCode);
                if (directory == null)
                    return errorCode;

                var (files, unknownTables) = ResolveTableFiles(directory, names, prefix, defaultToAllWhenNoCriteria: false);

                foreach (var name in unknownTables)
                    AnsiConsole.MarkupLine($"[yellow]No local .table for:[/] {Markup.Escape(name)}");

                if (files.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No table matches the criteria.[/]");
                    return ExitNoMatch;
                }

                var plan = BuildActivationPlan(files, columns, all);
                var totalToActivate = plan.Sum(p => p.ToActivate.Count);

                if (totalToActivate == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Nothing to activate — all requested columns are already selected.[/]");
                    return ExitSuccess;
                }

                AnsiConsole.WriteLine();
                foreach (var (path, table, toActivate) in plan.Where(p => p.ToActivate.Count > 0))
                    AnsiConsole.MarkupLine(
                        $"{Markup.Escape(table.Name ?? Path.GetFileNameWithoutExtension(path))}: " +
                        $"[cyan]{toActivate.Count}[/] column(s) — " +
                        Markup.Escape(string.Join(", ", toActivate.Select(c => c.LogicalName))));

                AnsiConsole.WriteLine();

                if (!noPrompt && !AnsiConsole.Confirm($"Activate {totalToActivate} column(s)?"))
                    return ExitSuccess;

                foreach (var (path, table, toActivate) in plan.Where(p => p.ToActivate.Count > 0))
                {
                    foreach (var column in toActivate)
                        column.Selected = true;

                    TableFileStore.Save(path, table);

                    AnsiConsole.MarkupLine(
                        $"[blue]Updated[/] {Markup.Escape(Path.GetFileName(path))} " +
                        $"([bold]{toActivate.Count}[/] column(s) activated)");
                }

                return ExitSuccess;
            }
            catch (Exception ex)
            {
                return CrmTableHelper.ReportUnexpected(ex);
            }
        }

        private static List<(string Path, CoreTable Table, List<Column> ToActivate)> BuildActivationPlan(
            IEnumerable<(string Path, CoreTable Table)> files, IReadOnlyList<string> columnNames, bool all)
        {
            var plan = new List<(string, CoreTable, List<Column>)>();

            foreach (var (path, table) in files)
            {
                List<Column> targets;

                if (all)
                {
                    targets = table.Columns.Where(c => !c.Selected).ToList();
                }
                else
                {
                    targets = new List<Column>();

                    foreach (var columnName in columnNames)
                    {
                        var column = table.Columns.FirstOrDefault(
                            c => string.Equals(c.LogicalName, columnName, StringComparison.OrdinalIgnoreCase));

                        if (column == null)
                        {
                            AnsiConsole.MarkupLine(
                                $"[yellow]{Markup.Escape(table.LogicalName)}:[/] column not found: " +
                                $"{Markup.Escape(columnName)}");
                            continue;
                        }

                        if (!column.Selected)
                            targets.Add(column);
                    }
                }

                plan.Add((path, table, targets));
            }

            return plan;
        }

        // ══════════════════════════════════════════════════════════════════════
        // tables columns set
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Renames a column's C# <see cref="Column.Name" /> and/or toggles its
        /// <see cref="Column.Selected" /> flag in a local <c>.table</c> file.
        /// </summary>
        /// <param name="select">
        /// <see langword="true" /> to activate the column, <see langword="false" /> to deactivate
        /// it, or <see langword="null" /> to leave the selection untouched.
        /// </param>
        public static int Set(
            string projectRoot,
            string tablesDirectory,
            string tableName,
            string columnName,
            string newName,
            bool? select)
        {
            try
            {
                var directory = ResolveTablesDirectory(projectRoot, tablesDirectory, out var errorCode);
                if (directory == null)
                    return errorCode;

                var path = FindTableFile(directory, tableName);
                if (path == null)
                {
                    AnsiConsole.MarkupLine($"[red]No local .table for:[/] {Markup.Escape(tableName)}");
                    return ExitNoMatch;
                }

                var table = TableFileStore.Load(path);
                var column = table.Columns.FirstOrDefault(
                    c => string.Equals(c.LogicalName, columnName, StringComparison.OrdinalIgnoreCase));

                if (column == null)
                {
                    AnsiConsole.MarkupLine(
                        $"[red]Column not found in[/] {Markup.Escape(Path.GetFileName(path))}: " +
                        Markup.Escape(columnName));
                    return ExitNoMatch;
                }

                var changes = new List<string>();

                if (!string.IsNullOrWhiteSpace(newName) && !string.Equals(column.Name, newName, StringComparison.Ordinal))
                {
                    // The C# name must stay unique within the table: two columns compiling to the
                    // same identifier would only fail later, at the consuming project's build.
                    var conflict = table.Columns.FirstOrDefault(
                        c => c != column && string.Equals(c.Name, newName, StringComparison.OrdinalIgnoreCase));

                    if (conflict != null)
                    {
                        AnsiConsole.MarkupLine(
                            $"[red]'{Markup.Escape(newName)}' is already used by column[/] " +
                            Markup.Escape(conflict.LogicalName) + ".");
                        return ExitError;
                    }

                    changes.Add($"Name: {column.Name} -> {newName}");
                    column.Name = newName;
                }

                if (select.HasValue && column.Selected != select.Value)
                {
                    changes.Add($"Select: {(column.Selected ? "true" : "false")} -> {(select.Value ? "true" : "false")}");
                    column.Selected = select.Value;
                }

                if (changes.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Nothing to change.[/]");
                    return ExitSuccess;
                }

                TableFileStore.Save(path, table);

                AnsiConsole.MarkupLine(
                    $"[blue]Updated[/] {Markup.Escape(Path.GetFileName(path))}: " +
                    Markup.Escape(string.Join(", ", changes)));

                return ExitSuccess;
            }
            catch (Exception ex)
            {
                return CrmTableHelper.ReportUnexpected(ex);
            }
        }

        /// <summary>
        /// Finds a local table by its logical name, falling back to its C# <see cref="Table.Name" />
        /// so that a user typing the identifier they see in code still finds the file.
        /// </summary>
        private static string FindTableFile(string directory, string tableName)
        {
            var byLogicalName = TableFileStore.FindTableFile(directory, tableName);
            if (byLogicalName != null)
                return byLogicalName;

            foreach (var (path, table) in LoadLocalTables(directory))
                if (string.Equals(table.Name, tableName, StringComparison.OrdinalIgnoreCase))
                    return path;

            return null;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Local file resolution
        // ══════════════════════════════════════════════════════════════════════

        private static string ResolveTablesDirectory(string projectRoot, string tablesDirectory, out int errorCode)
        {
            errorCode = ExitSuccess;

            var location = CrmTableHelper.ResolveLocation(projectRoot);
            if (location == null)
            {
                errorCode = ExitNotFound;
                return null;
            }

            var targetDirectory = tablesDirectory ?? location.TablesDirectory;
            if (string.IsNullOrWhiteSpace(targetDirectory))
            {
                AnsiConsole.MarkupLine(
                    "[red]Unable to infer the .table directory.[/] " +
                    "Declare [cyan]XrmFrameworkCoreProjectName[/] in the root's " +
                    "Directory.Build.props, or pass [cyan]--tables-dir[/].");
                errorCode = ExitNotFound;
                return null;
            }

            if (!Directory.Exists(targetDirectory))
            {
                AnsiConsole.MarkupLine($"[red]Directory not found:[/] {Markup.Escape(targetDirectory)}");
                errorCode = ExitNotFound;
                return null;
            }

            return targetDirectory;
        }

        /// <summary>
        /// Loads every local <c>.table</c> file, excluding the global option sets pseudo-table
        /// (it describes no entity — see <see cref="TableFileStore.GlobalOptionSetLogicalName" />).
        /// An unreadable file is skipped rather than fatal, as elsewhere in <see cref="TableFileStore" />.
        /// </summary>
        private static List<(string Path, CoreTable Table)> LoadLocalTables(string directory)
        {
            var result = new List<(string, CoreTable)>();

            foreach (var path in Directory.GetFiles(directory, "*" + TableFileStore.TableFileExtension)
                                          .OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                CoreTable table;
                try
                {
                    table = TableFileStore.Load(path);
                }
                catch (Exception)
                {
                    continue;
                }

                if (string.Equals(table.LogicalName, TableFileStore.GlobalOptionSetLogicalName,
                                  StringComparison.OrdinalIgnoreCase))
                    continue;

                result.Add((path, table));
            }

            return result;
        }

        /// <summary>
        /// Matches local tables against explicit logical names and/or a prefix, the same way
        /// <see cref="CrmTableHelper" /> matches entities against the environment.
        /// </summary>
        /// <param name="defaultToAllWhenNoCriteria">
        /// When neither <paramref name="names" /> nor <paramref name="prefix" /> is given: return
        /// every locally tracked table (read-only commands) if <see langword="true" />, or none
        /// (mutating commands, which must not silently target the whole project) if
        /// <see langword="false" />.
        /// </param>
        private static (List<(string Path, CoreTable Table)> Matched, List<string> Unknown) ResolveTableFiles(
            string directory, IReadOnlyCollection<string> names, string prefix, bool defaultToAllWhenNoCriteria)
        {
            var all = LoadLocalTables(directory);

            if (names.Count == 0 && string.IsNullOrWhiteSpace(prefix))
            {
                var defaults = defaultToAllWhenNoCriteria
                    ? all.OrderBy(e => e.Table.LogicalName, StringComparer.OrdinalIgnoreCase).ToList()
                    : new List<(string, CoreTable)>();

                return (defaults, new List<string>());
            }

            var matched = new List<(string Path, CoreTable Table)>();
            var matchedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in all)
            {
                var isNameMatch = names.Any(
                    n => string.Equals(n, entry.Table.LogicalName, StringComparison.OrdinalIgnoreCase));

                var isPrefixMatch = !string.IsNullOrWhiteSpace(prefix)
                    && (entry.Table.LogicalName ?? string.Empty).StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

                if (!isNameMatch && !isPrefixMatch)
                    continue;

                matched.Add(entry);
                if (isNameMatch)
                    matchedNames.Add(entry.Table.LogicalName);
            }

            var unknown = names.Where(n => !matchedNames.Contains(n)).ToList();

            return (matched.OrderBy(e => e.Table.LogicalName, StringComparer.OrdinalIgnoreCase).ToList(), unknown);
        }
    }
}
