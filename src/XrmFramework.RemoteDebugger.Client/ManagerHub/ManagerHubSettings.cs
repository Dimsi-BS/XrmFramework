// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.RemoteDebugger.Client.ManagerHub;

/// <summary>
/// Manager (Plugin Monitor) connection settings for the RemoteDebugger.
/// Modeled after the <c>HubOptions</c> pattern + MSAL configuration of the Desktop application.
/// </summary>
/// <remarks>
/// When these settings are populated on
/// <see cref="XrmFramework.RemoteDebugger.Common.RemoteDebugger{T}"/>,
/// execution events are forwarded in real time to the Plugin Monitor interface
/// via the Manager's DesktopHub, using the same Azure AD authentication as the Desktop.
/// </remarks>
public class ManagerHubSettings
{
    /// <summary>
    /// Base URL of the Manager API.
    /// Example: <c>https://manager.example.com</c> or <c>https://localhost:5001</c>
    /// Identical to the <c>ApiUrl</c> property of <c>HubOptions</c> in the Desktop application.
    /// </summary>
    public string ApiUrl { get; set; }

    /// <summary>
    /// ClientId of the Azure AD application (MSAL <c>PublicClientApplication</c>).
    /// Identical to the <c>ClientId</c> user environment variable read by the Desktop.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Azure AD TenantId (or domain name).
    /// Identical to the <c>Tenant</c> user environment variable read by the Desktop.
    /// </summary>
    public string Tenant { get; set; }

    /// <summary>
    /// Indicates whether the settings are sufficient to establish the connection.
    /// </summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ApiUrl) &&
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(Tenant);

    /// <summary>
    /// Creates a pre-filled instance from the <c>ClientId</c> and <c>Tenant</c> user
    /// environment variables — the same ones used by the Desktop.
    /// </summary>
    public static ManagerHubSettings FromEnvironment(string apiUrl) => new()
    {
        ApiUrl   = apiUrl,
        ClientId = System.Environment.GetEnvironmentVariable("ClientId"),
        Tenant   = System.Environment.GetEnvironmentVariable("Tenant"),
    };
}
