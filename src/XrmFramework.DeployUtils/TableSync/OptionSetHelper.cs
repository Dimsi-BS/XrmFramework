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
    /// <c>xrmframework tables optionsets</c> commands: renames an option set and/or one of its
    /// members across every local <c>.table</c> file that records it — entirely offline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The same option set can be declared in <b>several</b> files: the historical
    /// DefinitionManager kept in a table's own <c>Enums</c> every option set one of its columns
    /// referenced — globals included — while also writing the globals to
    /// <c>OptionSets.table</c> (see <see cref="TableFileSyncer" />, which renames the same way
    /// when recovering names from a 2.* assembly). A rename here therefore walks every
    /// <c>.table</c> file, <c>OptionSets.table</c> included, and updates every copy it finds —
    /// never just the first one.
    /// </para>
    /// <para>
    /// In accordance with the CLI contract, these entry points return an exit code and never call
    /// <c>Environment.Exit</c>.
    /// </para>
    /// </remarks>
    public static class OptionSetHelper
    {
        /// <summary>Success.</summary>
        public const int ExitSuccess = 0;

        /// <summary>No option set (or member) matches the requested criteria.</summary>
        public const int ExitNoMatch = 1;

        /// <summary>Configuration or directory not found.</summary>
        public const int ExitNotFound = 2;

        /// <summary>Unexpected error.</summary>
        public const int ExitError = 3;

        // ══════════════════════════════════════════════════════════════════════
        // tables optionsets list
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Without <paramref name="optionSetLogicalName" />: lists every option set declared
        /// across the local <c>.table</c> files, one row per distinct logical name. With it:
        /// lists the members of that one option set.
        /// </summary>
        public static int List(
            string projectRoot,
            string tablesDirectory,
            string optionSetLogicalName,
            string filter,
            bool globalOnly)
        {
            try
            {
                var directory = ColumnHelper.ResolveTablesDirectory(projectRoot, tablesDirectory, out var errorCode);
                if (directory == null)
                    return errorCode;

                var files = ColumnHelper.LoadLocalTables(directory, includeGlobalOptionSets: true);

                if (!string.IsNullOrWhiteSpace(optionSetLogicalName))
                    return ListMembers(files, optionSetLogicalName);

                return ListOverview(files, filter, globalOnly);
            }
            catch (Exception ex)
            {
                return CrmTableHelper.ReportUnexpected(ex);
            }
        }

        private static int ListMembers(IEnumerable<(string Path, CoreTable Table)> files, string optionSetLogicalName)
        {
            var copies = FindCopies(files, optionSetLogicalName);

            if (copies.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No option set found for:[/] {Markup.Escape(optionSetLogicalName)}");
                return ExitNoMatch;
            }

            // Members are metadata, not identifiers a project renames per copy: any copy carries
            // the same values, so the first one is representative. The enum's own Name is the one
            // field that can legitimately disagree between copies (see ListOverview), so it is
            // reported per copy instead.
            var reference = copies[0].Enum;

            AnsiConsole.MarkupLine(
                $"[bold]{Markup.Escape(reference.LogicalName)}[/]" +
                (reference.IsGlobal ? " [cyan](global)[/]" : string.Empty));

            foreach (var (path, enumEntry) in copies)
                AnsiConsole.MarkupLine(
                    $"  {Markup.Escape(Path.GetFileName(path))}: [bold]{Markup.Escape(enumEntry.Name ?? string.Empty)}[/]" +
                    (enumEntry.IsLocked ? " [yellow](locked)[/]" : string.Empty));

            AnsiConsole.WriteLine();

            var grid = new Spectre.Console.Table().Border(TableBorder.Rounded);
            grid.AddColumn("Value");
            grid.AddColumn("Name");
            grid.AddColumn("External value");

            foreach (var value in reference.Values.OrderBy(v => v.Value))
                grid.AddRow(
                    value.Value.ToString(),
                    Markup.Escape(value.Name ?? string.Empty),
                    Markup.Escape(value.ExternalValue ?? string.Empty));

            AnsiConsole.Write(grid);

            return ExitSuccess;
        }

        private static int ListOverview(
            IEnumerable<(string Path, CoreTable Table)> files, string filter, bool globalOnly)
        {
            var groups = files
                .SelectMany(f => f.Table.Enums.Where(e => e?.LogicalName != null)
                                              .Select(e => (File: Path.GetFileNameWithoutExtension(f.Path), Enum: e)))
                .GroupBy(x => x.Enum.LogicalName, StringComparer.OrdinalIgnoreCase)
                .Where(g => MatchesOverview(g, filter, globalOnly))
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (groups.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No option set matches the criteria.[/]");
                return ExitNoMatch;
            }

            var grid = new Spectre.Console.Table().Border(TableBorder.Rounded);
            grid.AddColumn("Logical name");
            grid.AddColumn("Name");
            grid.AddColumn("Global");
            grid.AddColumn("Locked");
            grid.AddColumn("Values");
            grid.AddColumn("Declared in");

            foreach (var group in groups)
            {
                var entries = group.ToList();
                var names = entries.Select(e => e.Enum.Name).Where(n => !string.IsNullOrEmpty(n))
                                    .Distinct(StringComparer.Ordinal).ToList();

                // More than one distinct non-empty Name means the copies have drifted apart —
                // "set" updates every copy at once precisely to prevent this.
                var nameDisplay = names.Count <= 1
                    ? Markup.Escape(names.FirstOrDefault() ?? string.Empty)
                    : $"[red]{Markup.Escape(string.Join(" / ", names))} (mismatch)[/]";

                grid.AddRow(
                    Markup.Escape(group.Key),
                    nameDisplay,
                    entries.Any(e => e.Enum.IsGlobal) ? "yes" : string.Empty,
                    entries.Any(e => e.Enum.IsLocked) ? "[yellow]yes[/]" : string.Empty,
                    entries.Max(e => e.Enum.Values.Count).ToString(),
                    Markup.Escape(string.Join(", ", entries.Select(e => e.File)
                                                            .Distinct(StringComparer.OrdinalIgnoreCase))));
            }

            AnsiConsole.Write(grid);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold]{groups.Count}[/] option set(s).");

            return ExitSuccess;
        }

        private static bool MatchesOverview(
            IEnumerable<(string File, OptionSetEnum Enum)> group, string filter, bool globalOnly)
        {
            if (globalOnly && !group.Any(e => e.Enum.IsGlobal))
                return false;

            if (string.IsNullOrWhiteSpace(filter))
                return true;

            return group.Any(e =>
                (e.Enum.LogicalName ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                || (e.Enum.Name ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // ══════════════════════════════════════════════════════════════════════
        // tables optionsets set
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Renames an option set's C# <see cref="OptionSetEnum.Name" /> and/or one member's
        /// <see cref="OptionSetEnumValue.Name" />, in every local file that declares it. A copy
        /// marked <c>Locked</c> is left untouched and reported — its name belongs to the
        /// framework package's own generated code.
        /// </summary>
        /// <param name="valueNumber">Numeric value of the member to rename, or <see langword="null" />
        /// to leave every member as is.</param>
        public static int Set(
            string projectRoot,
            string tablesDirectory,
            string optionSetLogicalName,
            string newEnumName,
            int? valueNumber,
            string newValueName)
        {
            try
            {
                var directory = ColumnHelper.ResolveTablesDirectory(projectRoot, tablesDirectory, out var errorCode);
                if (directory == null)
                    return errorCode;

                var files = ColumnHelper.LoadLocalTables(directory, includeGlobalOptionSets: true);
                var copies = FindCopies(files, optionSetLogicalName);

                if (copies.Count == 0)
                {
                    AnsiConsole.MarkupLine($"[yellow]No option set found for:[/] {Markup.Escape(optionSetLogicalName)}");
                    return ExitNoMatch;
                }

                var touchedFiles = new List<string>();
                var frozenFiles = new List<string>();
                var valueFoundSomewhere = false;

                foreach (var (path, table) in files)
                {
                    var enumEntry = table.Enums.FirstOrDefault(
                        e => string.Equals(e?.LogicalName, optionSetLogicalName, StringComparison.OrdinalIgnoreCase));

                    if (enumEntry == null)
                        continue;

                    if (enumEntry.IsLocked)
                    {
                        frozenFiles.Add(Path.GetFileName(path));
                        continue;
                    }

                    var changed = false;

                    if (!string.IsNullOrWhiteSpace(newEnumName)
                        && !string.Equals(enumEntry.Name, newEnumName, StringComparison.Ordinal))
                    {
                        enumEntry.Name = newEnumName;
                        changed = true;
                    }

                    if (valueNumber.HasValue)
                    {
                        var member = enumEntry.Values.FirstOrDefault(v => v.Value == valueNumber.Value);

                        if (member != null)
                        {
                            valueFoundSomewhere = true;

                            if (!string.Equals(member.Name, newValueName, StringComparison.Ordinal))
                            {
                                member.Name = newValueName;
                                changed = true;
                            }
                        }
                    }

                    if (!changed)
                        continue;

                    TableFileStore.Save(path, table);
                    touchedFiles.Add(Path.GetFileName(path));
                }

                if (valueNumber.HasValue && !valueFoundSomewhere)
                    AnsiConsole.MarkupLine(
                        $"[yellow]No member valued[/] {valueNumber.Value} [yellow]in[/] " +
                        Markup.Escape(optionSetLogicalName));

                foreach (var file in frozenFiles)
                    AnsiConsole.MarkupLine(
                        $"[grey]Kept the frozen name in[/] {Markup.Escape(file)}");

                if (touchedFiles.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Nothing to change.[/]");
                    return ExitSuccess;
                }

                AnsiConsole.MarkupLine(
                    $"[blue]Updated[/] [bold]{touchedFiles.Count}[/] file(s): " +
                    Markup.Escape(string.Join(", ", touchedFiles)));

                return ExitSuccess;
            }
            catch (Exception ex)
            {
                return CrmTableHelper.ReportUnexpected(ex);
            }
        }

        private static List<(string Path, OptionSetEnum Enum)> FindCopies(
            IEnumerable<(string Path, CoreTable Table)> files, string optionSetLogicalName)
        {
            var result = new List<(string, OptionSetEnum)>();

            foreach (var (path, table) in files)
            {
                var enumEntry = table.Enums.FirstOrDefault(
                    e => string.Equals(e?.LogicalName, optionSetLogicalName, StringComparison.OrdinalIgnoreCase));

                if (enumEntry != null)
                    result.Add((path, enumEntry));
            }

            return result;
        }
    }
}
