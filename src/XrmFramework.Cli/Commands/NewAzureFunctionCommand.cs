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
/// <c>xrmframework new azurefunction</c> command: adds an isolated-worker Azure Function project to
/// an existing solution, replacing the <c>xrmAzureFunction</c> dotnet-new template. Logic in
/// <see cref="SimpleProjectScaffolder.Create(string, string, string, string)"/>.
/// </summary>
public sealed class NewAzureFunctionCommand : Command<NewAzureFunctionCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [System.ComponentModel.Description("Name of the Azure Function project.")]
        public string? Name { get; init; }

        [CommandOption("-s|--solution-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Root of the XrmFramework solution to add the project to. Defaults to the current directory.")]
        public string? SolutionDirectory { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return ValidationResult.Error("A project name is required.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var templateRoot = Path.Combine(AppContext.BaseDirectory, "Scaffolding", "AzureFunction");
        var solutionRoot = settings.SolutionDirectory ?? Directory.GetCurrentDirectory();

        return SimpleProjectScaffolder.Create(templateRoot, settings.Name!, solutionRoot, "Azure Function");
    }
}
