// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables sync</c> command: one-shot migration of a project's definitions from
/// XrmFramework 2.* to 3.1 or above.
/// </summary>
/// <remarks>
/// It updates the <c>.table</c> files from the <c>[EntityDefinition]</c> classes of the assembly last
/// compiled under 2.*, then removes from the <c>*Definition.cs</c> files sitting next to them
/// everything the 3.1 source generator now emits.
/// The actual logic lives in <see cref="TableSyncHelper.Sync(string, string, bool)"/>.
/// </remarks>
public sealed class TableSyncCommand : Command<TableSyncCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attributes: a global MSTest using (transitive via
        // DeployUtils) makes [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("--dll <PATH>")]
        [System.ComponentModel.Description("Path to the 2.* assembly to analyze (contains *Definition classes with [[EntityDefinition]]).")]
        public string? DllPath { get; init; }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory holding the .table and *Definition.cs files to migrate.")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--clean")]
        [System.ComponentModel.Description("Sets Select=false on orphaned columns and deletes .table files entirely generated with no CRM data.")]
        public bool Clean { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(DllPath))
                return ValidationResult.Error("The --dll option is required.");

            if (string.IsNullOrWhiteSpace(TablesDirectory))
                return ValidationResult.Error("The --tables-dir option is required.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => TableSyncHelper.Sync(settings.DllPath!, settings.TablesDirectory!, settings.Clean);
}
