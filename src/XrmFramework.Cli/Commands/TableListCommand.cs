// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables list</c> command: lists the tables of the environment selected in
/// the current project's configuration. The logic lives in <see cref="CrmTableHelper.List" />.
/// </summary>
public sealed class TableListCommand : Command<TableListCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attributes: a global MSTest using (transitive via DeployUtils)
        // makes [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("--prefix <PREFIX>")]
        [System.ComponentModel.Description("Only keep tables whose logical name starts with this prefix (e.g. ftp_).")]
        public string? Prefix { get; init; }

        [CommandOption("--filter <TEXT>")]
        [System.ComponentModel.Description("Only keep tables whose logical name or label contains this text.")]
        public string? Filter { get; init; }

        [CommandOption("--custom-only")]
        [System.ComponentModel.Description("Only keep custom tables.")]
        public bool CustomOnly { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Root containing the Config/ folder (default: search by walking up from the current folder).")]
        public string? ProjectRoot { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => CrmTableHelper.List(settings.ProjectRoot, settings.Prefix, settings.Filter, settings.CustomOnly);
}
