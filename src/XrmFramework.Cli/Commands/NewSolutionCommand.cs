// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.Scaffolding;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework new solution</c> command: scaffolds a new XrmFramework solution in-process,
/// replacing the <c>xrmSolution</c> dotnet-new template (and its <c>pwsh</c> postAction) with the CLI
/// itself as the only tool a consumer needs. The actual logic lives in
/// <see cref="SolutionScaffolder.Create(string, string, string)"/>.
/// </summary>
public sealed class NewSolutionCommand : Command<NewSolutionCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [System.ComponentModel.Description("Name of the solution and its root projects (e.g. Contoso -> Contoso.Core, Contoso.Plugins).")]
        public string? Name { get; init; }

        [CommandOption("-o|--output <DIRECTORY>")]
        [System.ComponentModel.Description("Directory the solution's own folder is created into. Defaults to the current directory.")]
        public string? Output { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return ValidationResult.Error("A solution name is required.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var templateRoot = Path.Combine(AppContext.BaseDirectory, "Scaffolding", "Solution");
        var output = settings.Output ?? Directory.GetCurrentDirectory();

        return SolutionScaffolder.Create(templateRoot, settings.Name!, output);
    }
}
