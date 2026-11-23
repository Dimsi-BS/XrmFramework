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
/// <c>Config/xrmFramework.config</c>, or to the one <c>--environment</c> names. Delegates to
/// <see cref="RegistrationHelper.RegisterPluginsAndWorkflows(string, string, DeployOptions)" />.
/// The inventory is produced by executing the registration code via the net462 tool
/// <c>XrmFramework.PluginInventory</c>, which runs wherever the CLI runs - Windows, macOS and
/// Linux alike, with no .NET Framework and no Mono.
/// </summary>
public sealed class DeployPluginsCommand : Command<DeployPluginsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("--dll <PATH>")]
        [System.ComponentModel.Description("net48 or net462 assembly of the plugin project to deploy (plugins, custom APIs, workflows).")]
        public string? DllPath { get; init; }

        [CommandOption("--project <NAME>")]
        [System.ComponentModel.Description("Project name as declared in xrmFramework.config (e.g. Plugins).")]
        public string? Project { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Project root containing the Config/ folder (default: current folder).")]
        public string ProjectRoot { get; init; } = ".";

        [CommandOption("--package <PATH>")]
        [System.ComponentModel.Description("Explicit .nupkg to push, overriding the one found next to the assembly. Only used when the project is configured with deployment=\"package\".")]
        public string? PackagePath { get; init; }

        [CommandOption("--on-premise")]
        [System.ComponentModel.Description("Targets an On-Premises CRM (default: Dataverse Online).")]
        public bool OnPremise { get; init; }

        // -NoPrompt is accepted too, for the scripts written against that spelling: Program.cs
        // rewrites it to --noprompt before Spectre parses, a short option name being limited to
        // one character.
        [CommandOption("-n|--noprompt")]
        [System.ComponentModel.Description("Silent mode: skips the connection confirmation (CI/CD). Also accepts -NoPrompt.")]
        public bool NoPrompt { get; init; }

        [CommandOption("--auth <MODE>")]
        [System.ComponentModel.Description("How to authenticate: auto (default), connectionstring, or wif for Azure DevOps workload identity federation. auto picks wif when no connection string is declared and the job offers an OIDC endpoint.")]
        public string? Auth { get; init; }

        [CommandOption("--environment <URL>")]
        [System.ComponentModel.Description("Dataverse organization URL to deploy to. Workload identity federation only - with a connection string, change its Url instead.")]
        public string? EnvironmentUrl { get; init; }

        [CommandOption("--tenant <ID>")]
        [System.ComponentModel.Description("Entra tenant to sign in to (default: $AZURESUBSCRIPTION_TENANT_ID).")]
        public string? TenantId { get; init; }

        [CommandOption("--application-id <ID>")]
        [System.ComponentModel.Description("Entra application whose federated credential trusts the job (default: $AZURESUBSCRIPTION_CLIENT_ID).")]
        public string? ApplicationId { get; init; }

        [CommandOption("--service-connection-id <ID>")]
        [System.ComponentModel.Description("Azure DevOps service connection to request the OIDC token for (default: $AZURESUBSCRIPTION_SERVICE_CONNECTION_ID).")]
        public string? ServiceConnectionId { get; init; }

        [CommandOption("--oidc-request-url <URL>")]
        [System.ComponentModel.Description("Complete OIDC token endpoint, query included, for a job whose endpoint cannot be derived from $SYSTEM_OIDCREQUESTURI.")]
        public string? OidcRequestUrl { get; init; }

        [CommandOption("--oidc-request-token <TOKEN>")]
        [System.ComponentModel.Description("Token authorizing the OIDC token request. Prefer the $SYSTEM_ACCESSTOKEN environment variable: a command line is visible to every process on the agent.")]
        public string? OidcRequestToken { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(DllPath))
                return ValidationResult.Error("The --dll option is required.");

            if (!File.Exists(DllPath))
                return ValidationResult.Error($"Assembly not found: {DllPath}");

            if (string.IsNullOrWhiteSpace(Project))
                return ValidationResult.Error("The --project option is required.");

            if (!string.IsNullOrWhiteSpace(PackagePath) && !File.Exists(PackagePath))
                return ValidationResult.Error($"Plugin package not found: {PackagePath}");

            if (!AuthenticationModeParser.TryParse(Auth, out _))
                return ValidationResult.Error(
                    $"Unknown --auth value \"{Auth}\". Use {AuthenticationModeParser.AcceptedValues}.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        // 1. Points the config to the project root's Config/.
        ConfigHelper.UseProjectConfig(Path.GetFullPath(settings.ProjectRoot));

        AuthenticationModeParser.TryParse(settings.Auth, out var authentication);

        var deployOptions = new DeployOptions
        {
            IsOnPremise = settings.OnPremise,
            NoPrompt = settings.NoPrompt,
            PackagePath = string.IsNullOrWhiteSpace(settings.PackagePath)
                ? null
                : Path.GetFullPath(settings.PackagePath),
            Authentication = authentication,
            EnvironmentUrl = settings.EnvironmentUrl,
            TenantId = settings.TenantId,
            ApplicationId = settings.ApplicationId,
            ServiceConnectionId = settings.ServiceConnectionId,
            OidcRequestUrl = settings.OidcRequestUrl,
            OidcRequestToken = settings.OidcRequestToken
        };

        // 2. Deploys: the assembly is NOT loaded in this process - that invariant still holds, and
        //    still matters, because this one has its own Microsoft.Xrm.Sdk. Its path is passed to
        //    the inventory host (and its metadata read without runtime loading).
        return RegistrationHelper.RegisterPluginsAndWorkflows(
            Path.GetFullPath(settings.DllPath!), settings.Project!, deployOptions);
    }
}
