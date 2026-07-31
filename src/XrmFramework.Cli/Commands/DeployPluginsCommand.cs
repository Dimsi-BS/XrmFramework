// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework deploy plugins</c> command: deploys an XrmFramework assembly
/// (plugins, custom APIs, workflows) to the environment selected in
/// <c>Config/xrmFramework.config</c>. Delegates to
/// <see cref="RegistrationHelper.RegisterPluginsAndWorkflows(string, string, bool, bool)" />.
/// The inventory is produced by executing the registration code via the net462 tool
/// <c>XrmFramework.PluginInventory</c> (deployment requires .NET Framework / Windows).
/// </summary>
public sealed class DeployPluginsCommand : Command<DeployPluginsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("--dll <PATH>")]
        [System.ComponentModel.Description("net462 assembly of the plugin project to deploy (plugins, custom APIs, workflows).")]
        public string? DllPath { get; init; }

        [CommandOption("--project <NAME>")]
        [System.ComponentModel.Description("Project name as declared in xrmFramework.config (e.g. Plugins).")]
        public string? Project { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Project root containing the Config/ folder (default: current folder).")]
        public string ProjectRoot { get; init; } = ".";

        [CommandOption("--on-premise")]
        [System.ComponentModel.Description("Targets an On-Premises CRM (default: Dataverse Online).")]
        public bool OnPremise { get; init; }

        [CommandOption("-n|--noprompt")]
        [System.ComponentModel.Description("Silent mode: skips the connection confirmation (CI/CD).")]
        public bool NoPrompt { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(DllPath))
                return ValidationResult.Error("The --dll option is required.");

            if (!File.Exists(DllPath))
                return ValidationResult.Error($"Assembly not found: {DllPath}");

            if (string.IsNullOrWhiteSpace(Project))
                return ValidationResult.Error("The --project option is required.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        // 1. Points the config to the project root's Config/.
        ConfigHelper.UseProjectConfig(Path.GetFullPath(settings.ProjectRoot));

        // 2. Deploys: the assembly is NOT loaded in this net8 process; its path is passed
        //    to the net462 inventory tool (and its metadata read without runtime loading).
        return RegistrationHelper.RegisterPluginsAndWorkflows(
            Path.GetFullPath(settings.DllPath!), settings.Project!, settings.OnPremise, settings.NoPrompt);
    }
}
