// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables pull</c> command: generates or updates the <c>.table</c> files from
/// the selected environment's metadata. Without <c>--table</c> or <c>--prefix</c>,
/// refreshes all tables already tracked by the project.
/// The logic lives in <see cref="CrmTableHelper.Pull" />.
/// </summary>
public sealed class TablePullCommand : Command<TablePullCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-t|--table <NAME>")]
        [System.ComponentModel.Description("Logical name of a table to retrieve. Repeatable option, and accepts a comma-separated list. Default: all tables that already have a .table file.")]
        public string[]? Tables { get; init; }

        [CommandOption("--prefix <PREFIX>")]
        [System.ComponentModel.Description("Also retrieves all tables whose logical name starts with this prefix.")]
        public string? Prefix { get; init; }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory for the .table files (default: the Core project's Definitions folder).")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Root containing the Config/ folder (default: search by walking up from the current folder).")]
        public string? ProjectRoot { get; init; }

        [CommandOption("-n|--noprompt")]
        [System.ComponentModel.Description("Silent mode: skips the confirmation (CI/CD).")]
        public bool NoPrompt { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => CrmTableHelper.Pull(
            settings.ProjectRoot,
            settings.TablesDirectory,
            settings.Tables,
            settings.Prefix,
            settings.NoPrompt);
}
