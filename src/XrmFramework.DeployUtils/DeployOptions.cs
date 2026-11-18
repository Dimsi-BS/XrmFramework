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
    /// How long to wait for Dataverse to unpack a pushed plugin package and create the
    /// <c>pluginassembly</c> and <c>plugintype</c> rows the steps are registered against.
    /// Packages holding hundreds of plugin types take minutes, not seconds.
    /// </summary>
    public TimeSpan PackageProcessingTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
