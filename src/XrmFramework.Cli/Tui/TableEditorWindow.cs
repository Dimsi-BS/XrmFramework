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
    private const string HelpText = "↑/↓ navigate    Space/Enter toggle    R rename    Esc/Q quit";

    private readonly IReadOnlyList<TrackedTable> _tables;
    private readonly ListView _tableList;
    private readonly FrameView _columnsFrame;
    private readonly TableView _columnTable;
    private readonly Label _status;

    private List<Column> _sortedColumns = new();
    private TrackedTable? _current;

    public TableEditorWindow(IReadOnlyList<TrackedTable> tables)
        : base("XrmFramework — tables edit")
    {
        _tables = tables;

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

        SetStatus(null);

        if (_tables.Count > 0)
            LoadColumns(0);
    }

    /// <remarks>
    /// Global quit shortcut. Overriding here (rather than a <c>KeyPress</c> handler) relies on
    /// the normal Terminal.Gui key routing: whichever pane has focus gets first chance to handle
    /// the key (the rename dialog runs as its own modal Toplevel, so it never reaches this
    /// override), and only an unhandled key bubbles up to the Window itself.
    /// </remarks>
    public override bool ProcessKey(KeyEvent keyEvent)
    {
        if (keyEvent.Key is GuiKey.Esc or GuiKey.q or GuiKey.Q)
        {
            Application.RequestStop();
            return true;
        }

        return base.ProcessKey(keyEvent);
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
    {
        var input = new TextField(column.Name) { X = 0, Y = 1, Width = Dim.Fill() };
        var label = new Label("New C# name:") { X = 0, Y = 0 };

        var okButton = new Button("OK", true);
        var cancelButton = new Button("Cancel", false);

        var dialog = new Dialog($"Rename {column.LogicalName}", 60, 7, okButton, cancelButton);
        dialog.Add(label, input);

        string? result = null;
        okButton.Clicked += () =>
        {
            result = input.Text.ToString();
            Application.RequestStop();
        };
        cancelButton.Clicked += () => Application.RequestStop();
        dialog.KeyPress += e =>
        {
            if (e.KeyEvent.Key == GuiKey.Esc)
            {
                Application.RequestStop();
                e.Handled = true;
            }
        };

        input.SetFocus();
        Application.Run(dialog);

        return result;
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

    private static void ShowError(string message) => MessageBox.ErrorQuery("Error", message, "OK");

    private void SetStatus(string? message)
        => _status.Text = string.IsNullOrEmpty(message) ? HelpText : $"{message}   {HelpText}";
}
