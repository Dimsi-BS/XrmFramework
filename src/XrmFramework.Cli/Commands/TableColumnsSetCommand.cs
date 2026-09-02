// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables columns set</c> command: renames a column's C# name and/or toggles
/// its selection in a local <c>.table</c> file — entirely offline, no environment connection.
/// The logic lives in <see cref="ColumnHelper.Set" />.
/// </summary>
public sealed class TableColumnsSetCommand : Command<TableColumnsSetCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attributes: a global MSTest using (transitive via DeployUtils)
        // makes [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("-t|--table <NAME>")]
        [System.ComponentModel.Description("Logical name (or C# name) of the table to edit.")]
        public string? Table { get; init; }

        [CommandOption("-c|--column <NAME>")]
        [System.ComponentModel.Description("Logical name of the column to edit.")]
        public string? Column { get; init; }

        [CommandOption("--name <NEWNAME>")]
        [System.ComponentModel.Description("Renames the column's C# name.")]
        public string? NewName { get; init; }

        [CommandOption("--select")]
        [System.ComponentModel.Description("Activates the column (Select: true).")]
        public bool Select { get; init; }

        [CommandOption("--deselect")]
        [System.ComponentModel.Description("Deactivates the column (Select: false).")]
        public bool Deselect { get; init; }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory holding the .table files (default: the Core project's Definitions folder).")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Root containing the Config/ folder (default: search by walking up from the current folder).")]
        public string? ProjectRoot { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Table))
                return ValidationResult.Error("The --table option is required.");

            if (string.IsNullOrWhiteSpace(Column))
                return ValidationResult.Error("The --column option is required.");

            if (Select && Deselect)
                return ValidationResult.Error("--select and --deselect are mutually exclusive.");

            if (string.IsNullOrWhiteSpace(NewName) && !Select && !Deselect)
                return ValidationResult.Error("Nothing to set. Use --name and/or --select/--deselect.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        bool? select = settings.Select ? true : settings.Deselect ? false : (bool?)null;

        return ColumnHelper.Set(
            settings.ProjectRoot,
            settings.TablesDirectory,
            settings.Table!,
            settings.Column!,
            settings.NewName,
            select);
    }
}
