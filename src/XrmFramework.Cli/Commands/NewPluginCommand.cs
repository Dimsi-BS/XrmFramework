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
/// <c>xrmframework new plugin</c> command: adds a plugin project (and its Deploy.* companion) to an
/// existing solution, replacing the <c>xrmPluginProject</c> dotnet-new template and its
/// <c>pwsh</c> postAction (merge, <c>dotnet sln add</c>, <c>dotnet add reference</c>, the
/// <c>xrmFramework.config</c> XML edit, and the interactive solution-name prompt). The actual logic
/// lives in <see cref="PluginScaffolder.Create(string, string, string, string?)"/>.
/// </summary>
public sealed class NewPluginCommand : Command<NewPluginCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [System.ComponentModel.Description("Name of the plugin project (e.g. Contoso.Plugins).")]
        public string? Name { get; init; }

        [CommandOption("-s|--solution-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Root of the XrmFramework solution to add the project to. Defaults to the current directory.")]
        public string? SolutionDirectory { get; init; }

        [CommandOption("--solution-unique-name <NAME>")]
        [System.ComponentModel.Description("Unique name of the Dataverse solution the project deploys to. Prompted for if omitted.")]
        public string? SolutionUniqueName { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return ValidationResult.Error("A project name is required.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var templateRoot = Path.Combine(AppContext.BaseDirectory, "Scaffolding", "Plugin");
        var solutionRoot = settings.SolutionDirectory ?? Directory.GetCurrentDirectory();

        return PluginScaffolder.Create(templateRoot, settings.Name!, solutionRoot, settings.SolutionUniqueName);
    }
}
