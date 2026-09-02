// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables columns add</c> command: activates (<c>Select: true</c>) columns
/// already present in one or more local <c>.table</c> files — entirely offline, no environment
/// connection. The logic lives in <see cref="ColumnHelper.Add" />.
/// </summary>
public sealed class TableColumnsAddCommand : Command<TableColumnsAddCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attributes: a global MSTest using (transitive via DeployUtils)
        // makes [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("-t|--table <NAME>")]
        [System.ComponentModel.Description("Logical name of a table to edit. Repeatable option, and accepts a comma-separated list.")]
        public string[]? Tables { get; init; }

        [CommandOption("--prefix <PREFIX>")]
        [System.ComponentModel.Description("Also edits every tracked table whose logical name starts with this prefix.")]
        public string? Prefix { get; init; }

        [CommandOption("-c|--column <NAME>")]
        [System.ComponentModel.Description("Logical name of a column to activate. Repeatable option, and accepts a comma-separated list.")]
        public string[]? Columns { get; init; }

        [CommandOption("--all")]
        [System.ComponentModel.Description("Activates every column not yet selected, instead of an explicit --column list.")]
        public bool All { get; init; }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory holding the .table files (default: the Core project's Definitions folder).")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Root containing the Config/ folder (default: search by walking up from the current folder).")]
        public string? ProjectRoot { get; init; }

        [CommandOption("-n|--noprompt")]
        [System.ComponentModel.Description("Silent mode: skips the confirmation (CI/CD).")]
        public bool NoPrompt { get; init; }

        public override ValidationResult Validate()
        {
            if ((Columns == null || Columns.Length == 0) && !All)
                return ValidationResult.Error("Specify --column, or --all to activate every remaining column.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => ColumnHelper.Add(
            settings.ProjectRoot,
            settings.TablesDirectory,
            settings.Tables,
            settings.Prefix,
            settings.Columns,
            settings.All,
            settings.NoPrompt);
}
