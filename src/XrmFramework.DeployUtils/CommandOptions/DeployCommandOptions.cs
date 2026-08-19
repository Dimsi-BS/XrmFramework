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
///   -n / --noprompt / -NoPrompt   Silent run: skips the CRM connection confirmation (e.g. CI/CD).
/// </code>
/// </example>
public class DeployCommandOptions
{
    // -NoPrompt is accepted too, for backward compatibility with the deployment scripts written
    // against that spelling: the alias is rewritten to --noprompt before parsing (see
    // CommandLineAliases), CommandLineParser reading a single dash as one-letter switches only.
    [Option('n', "noprompt", Required = false, Default = false,
        HelpText = "Silent mode: skips the CRM connection confirmation without an interactive prompt (also accepts -NoPrompt). Useful in CI/CD.")]
    public bool NoPrompt { get; set; }
}
