// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Spectre.Console.Cli;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// Options shared by every <c>deploy</c> command: which environment to reach and how to prove who
/// we are.
/// </summary>
/// <remarks>
/// Factored out rather than repeated so the two commands cannot drift apart - a deployment that
/// authenticates one way for plugins and another for web resources would be its own kind of bug.
/// </remarks>
public abstract class DeployCommandSettings : CommandSettings
{
    [CommandOption("--project-root <DIR>")]
    [System.ComponentModel.Description("Project root containing the Config/ folder (default: current folder).")]
    public string ProjectRoot { get; init; } = ".";

    // -NoPrompt is accepted too, for the scripts written against that spelling: Program.cs
    // rewrites it to --noprompt before Spectre parses, a short option name being limited to
    // one character.
    [CommandOption("-n|--noprompt")]
    [System.ComponentModel.Description("Silent mode: skips the connection confirmation (CI/CD). Also accepts -NoPrompt.")]
    public bool NoPrompt { get; init; }

    [CommandOption("--auth <MODE>")]
    [System.ComponentModel.Description("How to authenticate: auto (default), connectionstring, or wif for Azure DevOps workload identity federation. auto picks wif when no connection string is declared and the job offers an OIDC endpoint or already holds a token.")]
    public string? Auth { get; init; }

    [CommandOption("--environment <URL>")]
    [System.ComponentModel.Description("Dataverse organization URL to deploy to. For --auth wif and --auth clientsecret - with a declared connection string, change its Url instead.")]
    public string? EnvironmentUrl { get; init; }

    [CommandOption("--tenant <ID>")]
    [System.ComponentModel.Description("Entra tenant to sign in to (default: $XRMFRAMEWORK_TENANT_ID, $AZURESUBSCRIPTION_TENANT_ID, then $tenantId).")]
    public string? TenantId { get; init; }

    [CommandOption("--application-id <ID>")]
    [System.ComponentModel.Description("Entra application to sign in as - the one whose federated credential trusts the job, or the one --secret belongs to (default: $XRMFRAMEWORK_CLIENT_ID, $AZURESUBSCRIPTION_CLIENT_ID, then $servicePrincipalId).")]
    public string? ApplicationId { get; init; }

    [CommandOption("--service-connection-id <ID>")]
    [System.ComponentModel.Description("Azure DevOps service connection to request the OIDC token for - Power Platform or Azure (default: $XRMFRAMEWORK_SERVICE_CONNECTION_ID, then $AZURESUBSCRIPTION_SERVICE_CONNECTION_ID).")]
    public string? ServiceConnectionId { get; init; }

    [CommandOption("--secret <SECRET>")]
    [System.ComponentModel.Description("Secret of the Entra application, for --auth clientsecret. Prefer the $XRMFRAMEWORK_CLIENT_SECRET environment variable: a command line is visible to every process on the agent.")]
    public string? ClientSecret { get; init; }

    [CommandOption("--oidc-request-url <URL>")]
    [System.ComponentModel.Description("Complete OIDC token endpoint, query included, for a job whose endpoint cannot be derived from $SYSTEM_OIDCREQUESTURI.")]
    public string? OidcRequestUrl { get; init; }

    [CommandOption("--oidc-request-token <TOKEN>")]
    [System.ComponentModel.Description("Token authorizing the OIDC token request. Prefer the $SYSTEM_ACCESSTOKEN environment variable: a command line is visible to every process on the agent.")]
    public string? OidcRequestToken { get; init; }

    /// <summary>
    ///     Checks what every <c>deploy</c> command checks. Override and call <c>base</c>.
    /// </summary>
    public override Spectre.Console.ValidationResult Validate()
    {
        if (!AuthenticationModeParser.TryParse(Auth, out _))
            return Spectre.Console.ValidationResult.Error(
                $"Unknown --auth value \"{Auth}\". Use {AuthenticationModeParser.AcceptedValues}.");

        return Spectre.Console.ValidationResult.Success();
    }

    /// <summary>Turns the command line into the options the deployment pipeline reads.</summary>
    public DeployOptions ToDeployOptions()
    {
        AuthenticationModeParser.TryParse(Auth, out var authentication);

        return new DeployOptions
        {
            NoPrompt = NoPrompt,
            Authentication = authentication,
            EnvironmentUrl = EnvironmentUrl,
            TenantId = TenantId,
            ApplicationId = ApplicationId,
            ClientSecret = ClientSecret,
            ServiceConnectionId = ServiceConnectionId,
            OidcRequestUrl = OidcRequestUrl,
            OidcRequestToken = OidcRequestToken
        };
    }
}
