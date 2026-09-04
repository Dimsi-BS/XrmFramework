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
/// What <see cref="TableEditorWindow" /> asked for when it requested a pull, read back by
/// <see cref="TableEditorApp.Run" /> once <c>Application.Run</c> returns.
/// </summary>
internal enum PullRequest
{
    /// <summary>Re-pull every table already tracked locally — <c>tables pull</c> with no criteria.</summary>
    UpdateTracked,

    /// <summary>Browse the environment and pick new tables to pull by logical name.</summary>
    ImportNew
}

/// <summary>
/// <c>xrmframework tables edit</c> command: launches a full-screen console editor
/// (<see cref="TableEditorWindow" />) over the locally tracked <c>.table</c> files — the
/// interactive counterpart of <c>tables columns list/add/set</c>, entirely offline except for
/// <c>P</c> (pull), which briefly hands the terminal back to <see cref="CrmTableHelper" />.
/// </summary>
/// <remarks>
/// Resolution of the tables directory and the initial load are deliberately kept outside
/// Terminal.Gui: both can fail with a message worth printing to a normal scrolling console,
/// which stops making sense once <see cref="Application.Init(ConsoleDriver, IMainLoopDriver)" />
/// has taken over the screen. A pull is the same story once it is running: it is a network call
/// with its own confirmation prompt and progress lines, already built entirely on
/// <c>AnsiConsole</c> in <see cref="CrmTableHelper" /> — reusing it as is (rather than
/// re-implementing a progress UI inside Terminal.Gui) means <c>tables edit</c> exits its own
/// screen for the duration of the pull, then reopens over whatever
/// <see cref="CrmTableHelper.Pull" /> left on disk.
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

        while (true)
        {
            // The full set, OptionSets.table (the global option sets pseudo-table) included: a
            // global option set can be declared in several files at once, and renaming it has to
            // walk all of them the same way "tables optionsets set" does — TableEditorWindow only
            // browses the subset with actual columns.
            var allTables = LoadLocalTables(directory);
            var browsableTables = allTables.Where(t => !IsGlobalOptionSetsPseudoTable(t.Table)).ToList();

            TableEditorWindow window;

            Application.Init();
            try
            {
                window = new TableEditorWindow(browsableTables, allTables);
                Application.Run(window);
            }
            finally
            {
                // TableEditorWindow's constructor points this at itself; clearing it here avoids
                // a stale closure over a disposed window surviving into whatever runs next.
                Application.RootKeyEvent = null;
                Application.Shutdown();
            }

            if (window.PendingPull == null)
                return ExitSuccess;

            RunPull(window.PendingPull.Value, projectRoot, directory);
            // Loop back: re-Init a fresh Terminal.Gui session over whatever the pull left on disk.
        }
    }

    /// <summary>
    /// Runs entirely on the plain, non-alternate-screen console — Terminal.Gui has already been
    /// shut down by the caller. Reuses <see cref="CrmTableHelper" /> verbatim: same environment
    /// resolution, same confirmation prompt, same per-table progress and error reporting as
    /// <c>tables pull</c> on the command line.
    /// </summary>
    private static void RunPull(PullRequest request, string? projectRoot, string tablesDirectory)
    {
        AnsiConsole.WriteLine();

        if (request == PullRequest.ImportNew)
        {
            // Shows every table in the environment, flagging what's already tracked — the same
            // browse step "tables list" offers before typing a --table name by hand.
            CrmTableHelper.List(projectRoot, prefix: null, filter: null, customOnly: false);
            AnsiConsole.WriteLine();

            var input = AnsiConsole.Prompt(
                new TextPrompt<string>("Logical name(s) to import ([grey]comma-separated, empty to cancel[/]):")
                    .AllowEmpty());

            var names = SplitNames(input);
            if (names.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Nothing to import.[/]");
            }
            else
            {
                CrmTableHelper.Pull(projectRoot, tablesDirectory, names, prefix: null, noPrompt: false);
            }
        }
        else
        {
            CrmTableHelper.Pull(projectRoot, tablesDirectory, tableNames: null, prefix: null, noPrompt: false);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press Enter to return to the editor...[/]");
        Console.ReadLine();
    }

    private static List<string> SplitNames(string value)
        => value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

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
