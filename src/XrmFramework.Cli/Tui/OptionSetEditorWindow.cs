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
/// Full-screen editor for one option set's C# name and its members' names, opened from
/// <see cref="TableEditorWindow" /> (<c>O</c> on a column tied to one). The interactive
/// counterpart of <c>tables optionsets set</c>: same rule set, applied and saved immediately
/// through <see cref="TableFileStore.Save" /> instead of a <c>--option</c>/<c>--value</c> pair
/// on the command line.
/// </summary>
/// <remarks>
/// A global option set can be declared in several <c>.table</c> files at once — the historical
/// DefinitionManager wrote it into every table whose column referenced it, plus the shared
/// <c>OptionSets.table</c>. A rename here walks every copy in <see cref="_allTables" /> the same
/// way <c>OptionSetHelper.Set</c> does, skipping (and reporting) any copy marked <c>Locked</c>:
/// its name belongs to the framework package's own generated code.
/// </remarks>
internal sealed class OptionSetEditorWindow : Window
{
    private const string HelpText = "↑/↓ navigate    Enter/R rename value    N rename option set    Esc/Q close";

    private readonly IReadOnlyList<TrackedTable> _allTables;
    private readonly string _logicalName;
    private readonly TableView _valuesTable;
    private readonly Label _status;

    private List<OptionSetEnumValue> _sortedValues = new();
    private OptionSetEnum _reference = null!;

    public OptionSetEditorWindow(IReadOnlyList<TrackedTable> allTables, string logicalName)
        : base($"Option set — {logicalName}")
    {
        _allTables = allTables;
        _logicalName = logicalName;

        _valuesTable = new TableView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(1),
            FullRowSelect = true
        };

        _status = new Label
        {
            X = 0,
            Y = Pos.Bottom(_valuesTable),
            Width = Dim.Fill(),
            Height = 1
        };

        Add(_valuesTable, _status);

        _valuesTable.KeyPress += ValuesTable_KeyPress;

        SetStatus(null);
        LoadValues();
    }

    public override bool ProcessKey(KeyEvent keyEvent)
    {
        if (keyEvent.Key is GuiKey.Esc or GuiKey.q or GuiKey.Q)
        {
            Application.RequestStop();
            return true;
        }

        if (keyEvent.Key is GuiKey.n or GuiKey.N)
        {
            RenameEnum();
            return true;
        }

        return base.ProcessKey(keyEvent);
    }

    /// <remarks>
    /// Every tracked file that declares this logical name, whether or not it is the one the
    /// column being edited belongs to — same lookup as <c>OptionSetHelper.FindCopies</c>.
    /// </remarks>
    private List<(TrackedTable Tracked, OptionSetEnum Enum)> FindCopies()
        => _allTables
            .Select(t => (Tracked: t, Enum: t.Table.Enums.FirstOrDefault(
                e => string.Equals(e?.LogicalName, _logicalName, StringComparison.OrdinalIgnoreCase))))
            .Where(x => x.Enum != null)
            .Select(x => (x.Tracked, Enum: x.Enum!))
            .ToList();

    private void LoadValues()
    {
        // Members are metadata, not identifiers a project renames per copy: any copy carries the
        // same values, so the first one is representative — same choice as
        // OptionSetHelper.ListMembers. The C# name can legitimately disagree between copies
        // (a rename here corrects that), so re-reading it after every save keeps the title honest.
        _reference = FindCopies()[0].Enum;
        _sortedValues = _reference.Values.OrderBy(v => v.Value).ToList();
        _valuesTable.Table = BuildDataTable();
        RefreshTitle();
    }

    private DataTable BuildDataTable()
    {
        var data = new DataTable();
        data.Columns.Add("Value");
        data.Columns.Add("C# name");
        data.Columns.Add("External value");

        foreach (var value in _sortedValues)
            data.Rows.Add(value.Value.ToString(), value.Name ?? string.Empty, value.ExternalValue ?? string.Empty);

        return data;
    }

    private void RefreshTitle()
    {
        var tag = _reference.IsGlobal ? " (global)" : string.Empty;
        Title = $"Option set — {_logicalName}{tag} — {_reference.Name}";
    }

    private void ValuesTable_KeyPress(View.KeyEventEventArgs e)
    {
        var row = _valuesTable.SelectedRow;
        if (row < 0 || row >= _sortedValues.Count)
            return;

        if (e.KeyEvent.Key is GuiKey.Enter or GuiKey.r or GuiKey.R)
        {
            RenameValue(row);
            e.Handled = true;
        }
    }

    private void RenameEnum()
    {
        var newName = Prompts.AskText($"Rename option set {_logicalName}", "C# name:", _reference.Name ?? string.Empty);
        if (newName == null || string.Equals(newName, _reference.Name, StringComparison.Ordinal))
            return;

        if (string.IsNullOrWhiteSpace(newName))
        {
            Prompts.ShowError("The C# name cannot be empty.");
            return;
        }

        Apply(newEnumName: newName, valueNumber: null, newValueName: null);
    }

    private void RenameValue(int row)
    {
        var value = _sortedValues[row];

        var newName = Prompts.AskText($"Rename value {value.Value}", "C# name:", value.Name ?? string.Empty);
        if (newName == null || string.Equals(newName, value.Name, StringComparison.Ordinal))
            return;

        if (string.IsNullOrWhiteSpace(newName))
        {
            Prompts.ShowError("The C# name cannot be empty.");
            return;
        }

        Apply(newEnumName: null, valueNumber: value.Value, newValueName: newName);
    }

    /// <remarks>
    /// Mirrors <c>OptionSetHelper.Set</c>'s loop exactly (locked copies skipped and reported,
    /// per-copy revert if a save fails), just against the already-loaded <see cref="_allTables" />
    /// instead of re-reading every file from disk.
    /// </remarks>
    private void Apply(string? newEnumName, int? valueNumber, string? newValueName)
    {
        var touched = new List<string>();
        var frozen = new List<string>();
        var errors = new List<string>();
        var valueFound = false;

        foreach (var (tracked, enumEntry) in FindCopies())
        {
            if (enumEntry.IsLocked)
            {
                frozen.Add(Path.GetFileName(tracked.Path));
                continue;
            }

            var previousEnumName = enumEntry.Name;
            OptionSetEnumValue? member = null;
            string? previousValueName = null;
            var changed = false;

            if (newEnumName != null && !string.Equals(enumEntry.Name, newEnumName, StringComparison.Ordinal))
            {
                enumEntry.Name = newEnumName;
                changed = true;
            }

            if (valueNumber.HasValue)
            {
                member = enumEntry.Values.FirstOrDefault(v => v.Value == valueNumber.Value);

                if (member != null)
                {
                    valueFound = true;
                    previousValueName = member.Name;

                    if (!string.Equals(member.Name, newValueName, StringComparison.Ordinal))
                    {
                        member.Name = newValueName;
                        changed = true;
                    }
                }
            }

            if (!changed)
                continue;

            try
            {
                TableFileStore.Save(tracked.Path, tracked.Table);
                touched.Add(Path.GetFileName(tracked.Path));
            }
            catch (Exception ex)
            {
                enumEntry.Name = previousEnumName;
                if (member != null)
                    member.Name = previousValueName;

                errors.Add($"{Path.GetFileName(tracked.Path)}: {ex.Message}");
            }
        }

        LoadValues();

        if (errors.Count > 0)
        {
            Prompts.ShowError(string.Join("\n", errors));
            return;
        }

        if (valueNumber.HasValue && !valueFound)
        {
            SetStatus($"No member valued {valueNumber.Value} in {_logicalName}.");
            return;
        }

        if (touched.Count == 0)
        {
            SetStatus(frozen.Count > 0 ? $"Frozen in {string.Join(", ", frozen)}; nothing else to change." : "Nothing to change.");
            return;
        }

        var summary = $"Updated {touched.Count} file(s): {string.Join(", ", touched)}.";
        SetStatus(frozen.Count > 0 ? $"{summary} Kept frozen in {string.Join(", ", frozen)}." : summary);
    }

    private void SetStatus(string? message)
        => _status.Text = string.IsNullOrEmpty(message) ? HelpText : $"{message}   {HelpText}";
}
