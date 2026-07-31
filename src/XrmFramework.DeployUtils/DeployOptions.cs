// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

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
}
