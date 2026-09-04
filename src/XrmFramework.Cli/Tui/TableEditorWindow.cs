// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data;
using Terminal.Gui;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;
// XrmFramework.Core also declares a Key type (.table alternate keys) — alias Terminal.Gui's.
using GuiKey = Terminal.Gui.Key;

namespace XrmFramework.Cli.Tui;

/// <summary>
/// Full-screen editor over the tables passed to it: a list of tracked tables on the left, the
/// columns of whichever one is selected on the right. Space/Enter toggles a column's
/// <c>Select</c> flag, <c>R</c> renames its C# name — the same two mutations as
/// <c>tables columns add</c>/<c>set</c>, applied and saved immediately via
/// <see cref="TableFileStore.Save" /> instead of requiring a table/column name on the
/// command line.
/// </summary>
internal sealed class TableEditorWindow : Window
{
    private const string HelpText = "↑/↓ navigate    Space/Enter toggle    R rename    O option set    P pull    Esc/Q quit";

    private readonly IReadOnlyList<TrackedTable> _tables;
    private readonly IReadOnlyList<TrackedTable> _allTables;
    private readonly ListView _tableList;
    private readonly FrameView _columnsFrame;
    private readonly TableView _columnTable;
    private readonly Label _status;

    private List<Column> _sortedColumns = new();
    private TrackedTable? _current;

    /// <summary>
    /// Set by <see cref="RequestPull" /> and read back by <c>TableEditorApp.Run</c> once this
    /// window's <c>Application.Run</c> call returns — <see langword="null" /> means the user
    /// quit normally, no pull requested.
    /// </summary>
    public PullRequest? PendingPull { get; private set; }

    /// <param name="tables">Browsable, left-pane tables — those with columns to edit.</param>
    /// <param name="allTables">
    /// Every locally tracked file, the global option sets pseudo-table included: an option set
    /// rename (<see cref="EditOptionSet" />) has to reach every copy, the same way
    /// <c>tables optionsets set</c> does.
    /// </param>
    public TableEditorWindow(IReadOnlyList<TrackedTable> tables, IReadOnlyList<TrackedTable> allTables)
        : base("XrmFramework — tables edit")
    {
        _tables = tables;
        _allTables = allTables;

        var tablesFrame = new FrameView
        {
            Title = "Tables",
            X = 0,
            Y = 0,
            Width = Dim.Percent(34),
            Height = Dim.Fill(1)
        };

        _tableList = new ListView(_tables.Select(FormatTableEntry).ToList())
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        tablesFrame.Add(_tableList);

        _columnsFrame = new FrameView
        {
            Title = "Columns",
            X = Pos.Right(tablesFrame),
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(1)
        };

        _columnTable = new TableView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            FullRowSelect = true
        };
        _columnsFrame.Add(_columnTable);

        _status = new Label
        {
            X = 0,
            Y = Pos.Bottom(tablesFrame),
            Width = Dim.Fill(),
            Height = 1
        };

        Add(tablesFrame, _columnsFrame, _status);

        _tableList.SelectedItemChanged += args => LoadColumns(args.Item);
        _columnTable.KeyPress += ColumnTable_KeyPress;

        // Global shortcuts (quit, pull) via RootKeyEvent rather than an overridden ProcessKey:
        // when the Tables pane (a ListView) has focus, its own key handling — including the
        // incremental "jump to item" search on plain letters — consumes the event before it
        // would ever bubble up to this Window, so Q/P (and, from that pane, even Esc) silently
        // do nothing. RootKeyEvent fires before any view gets a look, so it works regardless of
        // which pane has focus. Application.Current is checked to leave it inert while a modal
        // (rename dialog, option set editor...) is on top — otherwise typing a name containing
        // 'q' would close whatever dialog is open on the first such letter.
        Application.RootKeyEvent = HandleGlobalKey;

        if (_tables.Count > 0)
        {
            SetStatus(null);
            LoadColumns(0);
        }
        else
        {
            SetStatus("No table tracked yet — press P to pull from the environment.");
        }
    }

    private bool HandleGlobalKey(KeyEvent keyEvent)
    {
        // Inert while any other Toplevel (a dialog, the option set editor...) is on top: those
        // handle their own Esc, and letters must reach a focused TextField untouched.
        if (Application.Current != this)
            return false;

        if (keyEvent.Key is GuiKey.Esc or GuiKey.q or GuiKey.Q)
        {
            Application.RequestStop();
            return true;
        }

        if (keyEvent.Key is GuiKey.p or GuiKey.P)
        {
            RequestPull();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Asks which kind of pull to run, then quits this window with <see cref="PendingPull" /> set
    /// so <c>TableEditorApp.Run</c> performs it — a pull is a network call with its own
    /// confirmation and progress output, which needs the plain console, not this screen. Nothing
    /// happens locally until control returns there.
    /// </summary>
    private void RequestPull()
    {
        var choice = MessageBox.Query(
            "Pull from environment",
            "Update every table already tracked, or browse the environment for new ones to import?",
            "Update tracked", "Import new...", "Cancel");

        switch (choice)
        {
            case 0:
                PendingPull = PullRequest.UpdateTracked;
                Application.RequestStop();
                break;

            case 1:
                PendingPull = PullRequest.ImportNew;
                Application.RequestStop();
                break;
        }
    }

    private static string FormatTableEntry(TrackedTable tracked)
        => $"{tracked.Table.LogicalName,-28} {tracked.Table.Name}";

    private void LoadColumns(int index)
    {
        _current = _tables[index];

        _sortedColumns = _current.Table.Columns
            .OrderBy(c => c.LogicalName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _columnTable.Table = BuildDataTable(_sortedColumns);
        RefreshTitle();
    }

    private static DataTable BuildDataTable(IReadOnlyList<Column> columns)
    {
        var data = new DataTable();
        data.Columns.Add("Sel");
        data.Columns.Add("Logical name");
        data.Columns.Add("C# name");
        data.Columns.Add("Type");
        data.Columns.Add("Option set");

        foreach (var column in columns)
        {
            data.Rows.Add(
                SelectedMark(column.Selected),
                column.LogicalName,
                column.Name,
                column.Type.ToString(),
                column.EnumName ?? string.Empty);
        }

        return data;
    }

    private static string SelectedMark(bool selected) => selected ? "✓" : string.Empty;

    private void RefreshTitle()
    {
        if (_current == null)
            return;

        var selected = _sortedColumns.Count(c => c.Selected);
        _columnsFrame.Title = $"Columns — {_current.Table.LogicalName} ({selected}/{_sortedColumns.Count} selected)";
    }

    private void ColumnTable_KeyPress(View.KeyEventEventArgs e)
    {
        if (_current == null)
            return;

        var row = _columnTable.SelectedRow;
        if (row < 0 || row >= _sortedColumns.Count)
            return;

        switch (e.KeyEvent.Key)
        {
            case GuiKey.Space:
            case GuiKey.Enter:
                ToggleSelected(row);
                e.Handled = true;
                break;

            case GuiKey.r:
            case GuiKey.R:
                RenameSelected(row);
                e.Handled = true;
                break;

            case GuiKey.o:
            case GuiKey.O:
                EditOptionSet(row);
                e.Handled = true;
                break;
        }
    }

    private void ToggleSelected(int row)
    {
        var column = _sortedColumns[row];
        var previous = column.Selected;
        column.Selected = !previous;

        if (!TrySave(out var error))
        {
            column.Selected = previous;
            ShowError(error!);
            return;
        }

        _columnTable.Table.Rows[row]["Sel"] = SelectedMark(column.Selected);
        _columnTable.Update();
        RefreshTitle();
        SetStatus($"{(column.Selected ? "Activated" : "Deactivated")} {column.LogicalName}.");
    }

    private void RenameSelected(int row)
    {
        var column = _sortedColumns[row];

        var newName = PromptForName(column);
        if (newName == null || string.Equals(newName, column.Name, StringComparison.Ordinal))
            return;

        if (string.IsNullOrWhiteSpace(newName))
        {
            ShowError("The C# name cannot be empty.");
            return;
        }

        // The C# name must stay unique within the table: two columns compiling to the same
        // identifier would only fail later, at the consuming project's build. Same rule as
        // ColumnHelper.Set (tables columns set --name).
        var conflict = _sortedColumns.FirstOrDefault(
            c => c != column && string.Equals(c.Name, newName, StringComparison.OrdinalIgnoreCase));

        if (conflict != null)
        {
            ShowError($"'{newName}' is already used by column {conflict.LogicalName}.");
            return;
        }

        var previousName = column.Name;
        column.Name = newName;

        if (!TrySave(out var error))
        {
            column.Name = previousName;
            ShowError(error!);
            return;
        }

        _columnTable.Table.Rows[row]["C# name"] = newName;
        _columnTable.Update();
        SetStatus($"Renamed {column.LogicalName}: {previousName} -> {newName}.");
    }

    private static string? PromptForName(Column column)
        => Prompts.AskText($"Rename {column.LogicalName}", "New C# name:", column.Name);

    /// <summary>
    /// Opens <see cref="OptionSetEditorWindow" /> on the option set the selected column
    /// references, if any. A column not tied to one (<see cref="Column.EnumName" /> empty) or
    /// one whose option set was never pulled locally (no matching entry in
    /// <see cref="XrmFramework.Core.Table.Enums" /> anywhere in <see cref="_allTables" />) just
    /// reports why instead of opening anything.
    /// </summary>
    private void EditOptionSet(int row)
    {
        var column = _sortedColumns[row];

        if (string.IsNullOrEmpty(column.EnumName))
        {
            SetStatus($"{column.LogicalName} is not tied to an option set.");
            return;
        }

        var known = _allTables.Any(t => t.Table.Enums.Any(
            e => string.Equals(e?.LogicalName, column.EnumName, StringComparison.OrdinalIgnoreCase)));

        if (!known)
        {
            ShowError(
                $"No option set definition tracked locally for '{column.EnumName}'. " +
                "Run 'tables optionsets list' or 'tables pull' first.");
            return;
        }

        Application.Run(new OptionSetEditorWindow(_allTables, column.EnumName));
    }

    private bool TrySave(out string? error)
    {
        try
        {
            TableFileStore.Save(_current!.Path, _current.Table);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private static void ShowError(string message) => Prompts.ShowError(message);

    private void SetStatus(string? message)
        => _status.Text = string.IsNullOrEmpty(message) ? HelpText : $"{message}   {HelpText}";
}
