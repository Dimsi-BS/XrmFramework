// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Terminal.Gui;

namespace XrmFramework.Cli.Tui;

/// <summary>
/// Small modal dialogs shared by <see cref="TableEditorWindow" /> and
/// <see cref="OptionSetEditorWindow" />.
/// </summary>
internal static class Prompts
{
    /// <summary>
    /// A single text field in a modal dialog. Esc or the Cancel button return
    /// <see langword="null" />; OK returns whatever the field holds, blank included — the caller
    /// decides whether blank is valid.
    /// </summary>
    public static string? AskText(string dialogTitle, string label, string initialValue)
    {
        var input = new TextField(initialValue) { X = 0, Y = 1, Width = Dim.Fill() };
        var promptLabel = new Label(label) { X = 0, Y = 0 };

        var okButton = new Button("OK", true);
        var cancelButton = new Button("Cancel", false);

        var dialog = new Dialog(dialogTitle, 60, 7, okButton, cancelButton);
        dialog.Add(promptLabel, input);

        string? result = null;
        okButton.Clicked += () =>
        {
            result = input.Text.ToString();
            Application.RequestStop();
        };
        cancelButton.Clicked += () => Application.RequestStop();
        dialog.KeyPress += e =>
        {
            if (e.KeyEvent.Key == Key.Esc)
            {
                Application.RequestStop();
                e.Handled = true;
            }
        };

        input.SetFocus();
        Application.Run(dialog);

        return result;
    }

    public static void ShowError(string message) => MessageBox.ErrorQuery("Error", message, "OK");
}
