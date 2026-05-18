// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.RemoteDebugger.Client;

/// <summary>
/// Paramètres partagés du débogueur distant, notamment les réglages de sérialisation JSON
/// utilisés pour persister et restaurer les sessions de test de plugins.
/// </summary>
public static class RemoteDebuggerSettings
{
    /// <summary>
    /// Paramètres de sérialisation JSON communs à tout le débogueur distant.
    /// Utilise <see cref="RemoteDebuggerContractResolver"/> pour gérer correctement
    /// les types CRM (Entity, AttributeCollection, OrganizationRequest, etc.).
    /// </summary>
    public static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ContractResolver = new RemoteDebuggerContractResolver()
    };
}
