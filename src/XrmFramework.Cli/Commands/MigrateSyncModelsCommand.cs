// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework migrate sync-models</c> command: generates <c>.model</c> files from the
/// hand-written <c>IBindingModel</c> classes found in a compiled assembly.
/// </summary>
/// <remarks>
/// Reflects over every class decorated with <c>[CrmEntity]</c> that implements
/// <c>IBindingModel</c> and writes the <c>.model</c> file <c>ModelSourceFileGenerator</c> would
/// need to reproduce it. When <c>--source-dir</c> is given, it also strips the now-redundant
/// mapped properties from the hand-written classes, so the project compiles once the generator
/// takes over. The actual logic lives in <see cref="ModelSyncHelper.Sync(string, string, string?)"/>.
/// </remarks>
public sealed class MigrateSyncModelsCommand : Command<MigrateSyncModelsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Fully qualified attribute: a global MSTest using (transitive via DeployUtils) makes
        // [Description] ambiguous with UnitTesting.DescriptionAttribute.
        [CommandOption("--dll <PATH>")]
        [System.ComponentModel.Description("Path to the assembly to analyze (contains hand-written binding model classes with [[CrmEntity]]).")]
        public string? DllPath { get; init; }

        [CommandOption("--models-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory the .model files are written into — created if it does not exist yet.")]
        public string? ModelsDirectory { get; init; }

        [CommandOption("--source-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Directory holding the hand-written .cs files (searched recursively). When given, strips the now-redundant [[CrmMapping]]/[[ChildRelationship]]/[[ExtendBindingModel]] properties from each class a .model file was written for, keeping everything else the project added by hand.")]
        public string? SourceDirectory { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(DllPath))
                return ValidationResult.Error("The --dll option is required.");

            if (string.IsNullOrWhiteSpace(ModelsDirectory))
                return ValidationResult.Error("The --models-dir option is required.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => ModelSyncHelper.Sync(settings.DllPath!, settings.ModelsDirectory!, settings.SourceDirectory);
}
