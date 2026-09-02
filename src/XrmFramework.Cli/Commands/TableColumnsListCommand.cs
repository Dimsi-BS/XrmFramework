// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables columns list</c> command: lists the columns already tracked in a
/// local <c>.table</c> file, selected or not — entirely offline, no environment connection.
/// The logic lives in <see cref="ColumnHelper.List" />.
/// </summary>
public sealed class TableColumnsListCommand : Command<TableColumnsListCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attributes: a global MSTest using (transitive via DeployUtils)
        // makes [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("-t|--table <NAME>")]
        [System.ComponentModel.Description("Logical name of a table to inspect. Repeatable option, and accepts a comma-separated list. Default: every table already tracked (having a .table file).")]
        public string[]? Tables { get; init; }

        [CommandOption("--prefix <PREFIX>")]
        [System.ComponentModel.Description("Also inspects every tracked table whose logical name starts with this prefix.")]
        public string? Prefix { get; init; }

        [CommandOption("--filter <TEXT>")]
        [System.ComponentModel.Description("Only keeps columns whose logical name or C# name contains this text.")]
        public string? Filter { get; init; }

        [CommandOption("--unselected-only")]
        [System.ComponentModel.Description("Only keeps columns not yet activated (Select: false) — the candidates for 'tables columns add'.")]
        public bool UnselectedOnly { get; init; }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory holding the .table files (default: the Core project's Definitions folder).")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Root containing the Config/ folder (default: search by walking up from the current folder).")]
        public string? ProjectRoot { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => ColumnHelper.List(
            settings.ProjectRoot,
            settings.TablesDirectory,
            settings.Tables,
            settings.Prefix,
            settings.Filter,
            settings.UnselectedOnly);
}
