// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework tables optionsets set</c> command: renames an option set's C# name and/or
/// one of its member's name, in every local <c>.table</c> file that declares it (a global
/// option set is typically declared by several). Entirely offline. The logic lives in
/// <see cref="OptionSetHelper.Set" />.
/// </summary>
public sealed class TableOptionSetsSetCommand : Command<TableOptionSetsSetCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attributes: a global MSTest using (transitive via DeployUtils)
        // makes [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("-o|--option <LOGICALNAME>")]
        [System.ComponentModel.Description("Logical name of the option set to edit.")]
        public string? Option { get; init; }

        [CommandOption("--name <NEWNAME>")]
        [System.ComponentModel.Description("Renames the option set's C# name.")]
        public string? NewName { get; init; }

        [CommandOption("--value <NUMBER>")]
        [System.ComponentModel.Description("Numeric value of the member to rename. Requires --value-name.")]
        public int? Value { get; init; }

        [CommandOption("--value-name <NEWNAME>")]
        [System.ComponentModel.Description("New C# name for the member designated by --value.")]
        public string? ValueName { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Option))
                return ValidationResult.Error("The --option option is required.");

            if (Value.HasValue != !string.IsNullOrWhiteSpace(ValueName))
                return ValidationResult.Error("--value and --value-name must be given together.");

            if (string.IsNullOrWhiteSpace(NewName) && !Value.HasValue)
                return ValidationResult.Error("Nothing to set. Use --name and/or --value with --value-name.");

            return ValidationResult.Success();
        }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory holding the .table files (default: the Core project's Definitions folder).")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Root containing the Config/ folder (default: search by walking up from the current folder).")]
        public string? ProjectRoot { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => OptionSetHelper.Set(
            settings.ProjectRoot,
            settings.TablesDirectory,
            settings.Option!,
            settings.NewName,
            settings.Value,
            settings.ValueName);
}
