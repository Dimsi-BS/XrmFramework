// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace XrmFramework.DeployUtils.CommandOptions;

/// <summary>
/// Command-line options for deploying plugins/workflows.
/// </summary>
/// <example>
/// Typical usage in Program.cs:
/// <code>
/// RegistrationHelper.RegisterPluginsAndWorkflows&lt;XrmFramework.Plugin&gt;("MyProject", false, args);
/// </code>
/// Available options:
/// <code>
///   -n / --noprompt   Silent run: skips the CRM connection confirmation (e.g. CI/CD).
/// </code>
/// </example>
public class DeployCommandOptions
{
    [Option('n', "noprompt", Required = false, Default = false,
        HelpText = "Silent mode: skips the CRM connection confirmation without an interactive prompt. Useful in CI/CD.")]
    public bool NoPrompt { get; set; }
}
