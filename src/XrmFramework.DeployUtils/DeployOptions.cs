// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.DeployUtils;

/// <summary>
/// Options de déploiement transmises au pipeline par
/// <see cref="RegistrationHelper.RegisterPluginsAndWorkflows{TPlugin}" />.
/// </summary>
public sealed class DeployOptions
{
    /// <summary>
    /// Indique si la cible est un CRM On-Premises.
    /// </summary>
    public bool IsOnPremise { get; set; }

    /// <summary>
    /// Lorsque <see langword="true" />, la confirmation interactive de connexion au CRM
    /// est ignorée. À utiliser dans les pipelines CI/CD ou les scripts non-interactifs.
    /// Équivalent à l'option <c>--noprompt</c> / <c>-n</c> en ligne de commande.
    /// </summary>
    public bool NoPrompt { get; set; }
}
