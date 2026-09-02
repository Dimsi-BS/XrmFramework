// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables optionsets list</c> command: without <c>--option</c>, lists every
/// option set declared across the local <c>.table</c> files; with it, lists that option set's
/// members. Entirely offline. The logic lives in <see cref="OptionSetHelper.List" />.
/// </summary>
public sealed class TableOptionSetsListCommand : Command<TableOptionSetsListCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attributes: a global MSTest using (transitive via DeployUtils)
        // makes [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("-o|--option <LOGICALNAME>")]
        [System.ComponentModel.Description("Logical name of an option set: lists its members instead of the project-wide overview.")]
        public string? Option { get; init; }

        [CommandOption("--filter <TEXT>")]
        [System.ComponentModel.Description("Overview only: only keeps option sets whose logical name or C# name contains this text.")]
        public string? Filter { get; init; }

        [CommandOption("--global-only")]
        [System.ComponentModel.Description("Overview only: only keeps global option sets.")]
        public bool GlobalOnly { get; init; }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory holding the .table files (default: the Core project's Definitions folder).")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Root containing the Config/ folder (default: search by walking up from the current folder).")]
        public string? ProjectRoot { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => OptionSetHelper.List(
            settings.ProjectRoot,
            settings.TablesDirectory,
            settings.Option,
            settings.Filter,
            settings.GlobalOnly);
}
