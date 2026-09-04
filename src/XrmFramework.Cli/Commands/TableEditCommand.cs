// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console.Cli;
using XrmFramework.Cli.Tui;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables edit</c> command: launches a full-screen, interactive editor over
/// the locally tracked <c>.table</c> files — entirely offline, no environment connection.
/// The console UI lives in <see cref="TableEditorApp" /> / <see cref="TableEditorWindow" />.
/// </summary>
public sealed class TableEditCommand : Command<TableEditCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attributes: a global MSTest using (transitive via DeployUtils)
        // makes [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory holding the .table files (default: the Core project's Definitions folder).")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Root containing the Config/ folder (default: search by walking up from the current folder).")]
        public string? ProjectRoot { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => TableEditorApp.Run(settings.ProjectRoot, settings.TablesDirectory);
}
