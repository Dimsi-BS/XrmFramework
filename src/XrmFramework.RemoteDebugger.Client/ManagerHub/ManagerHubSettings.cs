// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.RemoteDebugger.Client.ManagerHub;

/// <summary>
/// Paramètres de connexion au Manager (Plugin Monitor) pour le RemoteDebugger.
/// Calqué sur le modèle <c>HubOptions</c> + configuration MSAL de l'application Desktop.
/// </summary>
/// <remarks>
/// Si ces paramètres sont renseignés dans
/// <see cref="XrmFramework.RemoteDebugger.Common.RemoteDebugger{T}"/>,
/// les événements d'exécution sont transmis en temps réel à l'interface Plugin Monitor
/// via le DesktopHub du Manager, avec la même authentification Azure AD que le Desktop.
/// </remarks>
public class ManagerHubSettings
{
    /// <summary>
    /// URL de base de l'API Manager.
    /// Exemple : <c>https://manager.example.com</c> ou <c>https://localhost:5001</c>
    /// Identique à la propriété <c>ApiUrl</c> de <c>HubOptions</c> dans l'application Desktop.
    /// </summary>
    public string ApiUrl { get; set; }

    /// <summary>
    /// ClientId de l'application Azure AD (MSAL <c>PublicClientApplication</c>).
    /// Identique à la variable d'environnement utilisateur <c>ClientId</c> lue par le Desktop.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// TenantId (ou nom de domaine) Azure AD.
    /// Identique à la variable d'environnement utilisateur <c>Tenant</c> lue par le Desktop.
    /// </summary>
    public string Tenant { get; set; }

    /// <summary>
    /// Indique si les paramètres sont suffisants pour établir la connexion.
    /// </summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ApiUrl) &&
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(Tenant);

    /// <summary>
    /// Crée une instance pré-remplie depuis les variables d'environnement utilisateur
    /// <c>ClientId</c> et <c>Tenant</c> — les mêmes que celles utilisées par le Desktop.
    /// </summary>
    public static ManagerHubSettings FromEnvironment(string apiUrl) => new()
    {
        ApiUrl   = apiUrl,
        ClientId = System.Environment.GetEnvironmentVariable("ClientId"),
        Tenant   = System.Environment.GetEnvironmentVariable("Tenant"),
    };
}
