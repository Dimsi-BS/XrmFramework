// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Spectre.Console;
using Terminal.Gui;
using XrmFramework.DeployUtils.TableSync;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.Cli.Tui;

/// <summary>
/// A <c>.table</c> file already tracked locally, paired with the file it was loaded from — the
/// same pairing <c>ColumnHelper</c>/<c>OptionSetHelper</c> use internally, republished here since
/// their own is <c>internal</c> to <c>XrmFramework.DeployUtils</c>.
/// </summary>
internal sealed record TrackedTable(string Path, CoreTable Table);

/// <summary>
/// <c>xrmframework tables edit</c> command: launches a full-screen console editor
/// (<see cref="TableEditorWindow" />) over the locally tracked <c>.table</c> files — the
/// interactive counterpart of <c>tables columns list/add/set</c>, entirely offline.
/// </summary>
/// <remarks>
/// Resolution of the tables directory and the initial load are deliberately kept outside
/// Terminal.Gui: both can fail with a message worth printing to a normal scrolling console,
/// which stops making sense once <see cref="Application.Init(ConsoleDriver, IMainLoopDriver)" />
/// has taken over the screen.
/// </remarks>
public static class TableEditorApp
{
    public const int ExitSuccess = 0;
    public const int ExitNotFound = 2;

    public static int Run(string? projectRoot, string? tablesDirectory)
    {
        var directory = ResolveTablesDirectory(projectRoot, tablesDirectory);
        if (directory == null)
            return ExitNotFound;

        // The full set, OptionSets.table (the global option sets pseudo-table) included: a
        // global option set can be declared in several files at once, and renaming it has to
        // walk all of them the same way "tables optionsets set" does — TableEditorWindow only
        // browses the subset with actual columns.
        var allTables = LoadLocalTables(directory);
        var browsableTables = allTables.Where(t => !IsGlobalOptionSetsPseudoTable(t.Table)).ToList();

        if (browsableTables.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No .table file found in[/] {Markup.Escape(directory)}.");
            return ExitSuccess;
        }

        Application.Init();
        try
        {
            Application.Run(new TableEditorWindow(browsableTables, allTables));
        }
        finally
        {
            Application.Shutdown();
        }

        return ExitSuccess;
    }

    /// <remarks>
    /// Mirrors <c>ColumnHelper.ResolveTablesDirectory</c> (kept <c>internal</c> to DeployUtils):
    /// same <c>--tables-dir</c> override, same <see cref="ProjectConfigLocator" /> fallback.
    /// </remarks>
    private static string? ResolveTablesDirectory(string? projectRoot, string? tablesDirectory)
    {
        var location = ProjectConfigLocator.Locate(projectRoot ?? Directory.GetCurrentDirectory());

        var directory = tablesDirectory ?? location?.TablesDirectory;

        if (string.IsNullOrWhiteSpace(directory))
        {
            AnsiConsole.MarkupLine(
                "[red]Unable to infer the .table directory.[/] " +
                "Declare [cyan]XrmFrameworkCoreProjectName[/] in the root's " +
                "Directory.Build.props, or pass [cyan]--tables-dir[/].");
            return null;
        }

        if (!Directory.Exists(directory))
        {
            AnsiConsole.MarkupLine($"[red]Directory not found:[/] {Markup.Escape(directory)}");
            return null;
        }

        return directory;
    }

    /// <remarks>
    /// Mirrors <c>ColumnHelper.LoadLocalTables(directory, includeGlobalOptionSets: true)</c>
    /// (kept <c>internal</c>): same skip of unreadable files. Unlike that default, the pseudo-table
    /// is kept here — option set edits need every file that might declare a copy, callers that
    /// only want browsable tables filter it out via <see cref="IsGlobalOptionSetsPseudoTable" />.
    /// </remarks>
    private static List<TrackedTable> LoadLocalTables(string directory)
    {
        var result = new List<TrackedTable>();

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

            result.Add(new TrackedTable(path, table));
        }

        return result.OrderBy(t => t.Table.LogicalName, StringComparer.OrdinalIgnoreCase).ToList();
    }

    internal static bool IsGlobalOptionSetsPseudoTable(CoreTable table)
        => string.Equals(table.LogicalName, TableFileStore.GlobalOptionSetLogicalName, StringComparison.OrdinalIgnoreCase);
}
