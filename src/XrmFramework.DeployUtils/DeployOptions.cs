// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.DeployUtils;

/// <summary>
/// Deployment options passed to the pipeline by
/// <see cref="RegistrationHelper.RegisterPluginsAndWorkflows{TPlugin}" />.
/// </summary>
public sealed class DeployOptions
{
    /// <summary>
    /// Indicates whether the target is an On-Premises CRM.
    /// </summary>
    public bool IsOnPremise { get; set; }

    /// <summary>
    /// When <see langword="true" />, the interactive CRM connection confirmation
    /// is skipped. To be used in CI/CD pipelines or non-interactive scripts.
    /// Equivalent to the <c>--noprompt</c> / <c>-n</c> command-line option.
    /// </summary>
    public bool NoPrompt { get; set; }

    /// <summary>
    /// Explicit path to the <c>.nupkg</c> to push, overriding the default resolution next to the
    /// assembly. Only read when the project is configured for
    /// <see cref="Configuration.DeploymentMode.Package" />.
    /// Equivalent to the <c>--package</c> command-line option.
    /// </summary>
    public string PackagePath { get; set; }

    /// <summary>
    /// How the deployment proves who it is to Dataverse.
    /// Equivalent to the <c>--auth</c> command-line option.
    /// </summary>
    public Configuration.AuthenticationMode Authentication { get; set; } = Configuration.AuthenticationMode.Auto;

    /// <summary>
    /// Dataverse organization URL to deploy to, overriding the one the connection string declares.
    /// Lets one repository serve dev, UAT and production by changing a pipeline variable instead of
    /// a committed file.
    /// Equivalent to the <c>--environment</c> command-line option.
    /// </summary>
    public string EnvironmentUrl { get; set; }

    /// <summary>
    /// Entra tenant to sign in to, for workload identity federation.
    /// Equivalent to the <c>--tenant</c> command-line option.
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// Entra application whose federated credential trusts the running job.
    /// Equivalent to the <c>--application-id</c> command-line option.
    /// </summary>
    public string ApplicationId { get; set; }

    /// <summary>
    /// Azure DevOps service connection to request the OIDC token for.
    /// Equivalent to the <c>--service-connection-id</c> command-line option.
    /// </summary>
    public string ServiceConnectionId { get; set; }

    /// <summary>
    /// Complete OIDC token endpoint, query included, for the jobs whose endpoint cannot be derived
    /// from the usual variables.
    /// Equivalent to the <c>--oidc-request-url</c> command-line option.
    /// </summary>
    public string OidcRequestUrl { get; set; }

    /// <summary>
    /// Token authorizing the OIDC token request - the job's own access token.
    /// Equivalent to the <c>--oidc-request-token</c> command-line option.
    /// </summary>
    public string OidcRequestToken { get; set; }

    /// <summary>
    /// How long to wait for Dataverse to unpack a pushed plugin package and create the
    /// <c>pluginassembly</c> and <c>plugintype</c> rows the steps are registered against.
    /// Packages holding hundreds of plugin types take minutes, not seconds.
    /// </summary>
    public TimeSpan PackageProcessingTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
