// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Reporting;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// <c>xrmframework deploy webresources</c> command: publishes the web resources found under
/// <c>--path</c> (or auto-discovered from a folder named after <c>--project</c>) to the
/// environment selected in <c>Config/xrmFramework.config</c>, or to the one <c>--environment</c>
/// names. Delegates to
/// <see cref="WebResourceHelper.SyncWebResources(string, string, DeployOptions)" />.
/// </summary>
public sealed class DeployWebResourcesCommand : Command<DeployWebResourcesCommand.Settings>
{
    public sealed class Settings : DeployCommandSettings
    {
        [CommandOption("--project <NAME>")]
        [System.ComponentModel.Description("Project name as declared in xrmFramework.config (e.g. Webresources).")]
        public string? Project { get; init; }

        [CommandOption("--path <DIR>")]
        [System.ComponentModel.Description("Webresources project folder (default: auto-discovered from a folder named after --project).")]
        public string? Path { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Project))
                return ValidationResult.Error("The --project option is required.");

            if (!string.IsNullOrWhiteSpace(Path) && !Directory.Exists(Path))
                return ValidationResult.Error($"Directory not found: {Path}");

            return base.Validate();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        ConfigHelper.UseProjectConfig(System.IO.Path.GetFullPath(settings.ProjectRoot));

        return DeployErrors.Report(() => WebResourceHelper.SyncWebResources(
            settings.Project!, settings.Path!, settings.ToDeployOptions()));
    }
}
