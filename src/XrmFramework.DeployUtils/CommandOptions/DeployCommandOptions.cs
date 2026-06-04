// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace XrmFramework.DeployUtils.CommandOptions;

/// <summary>
/// Options de ligne de commande pour le déploiement de plugins/workflows.
/// </summary>
/// <example>
/// Utilisation typique dans Program.cs :
/// <code>
/// RegistrationHelper.RegisterPluginsAndWorkflows&lt;XrmFramework.Plugin&gt;("MyProject", false, args);
/// </code>
/// Options disponibles :
/// <code>
///   -n / --noprompt   Exécution silencieuse : ignore la confirmation de connexion au CRM (ex: CI/CD).
/// </code>
/// </example>
public class DeployCommandOptions
{
    [Option('n', "noprompt", Required = false, Default = false,
        HelpText = "Mode silencieux : passe la confirmation de connexion au CRM sans prompt interactif. Utile en CI/CD.")]
    public bool NoPrompt { get; set; }
}
