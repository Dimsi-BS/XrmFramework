// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.RemoteDebugger.Client;

/// <summary>
/// Shared settings for the remote debugger, notably the JSON serialization settings
/// used to persist and restore plugin test sessions.
/// </summary>
public static class RemoteDebuggerSettings
{
    /// <summary>
    /// JSON serialization settings common to the whole remote debugger.
    /// Uses <see cref="RemoteDebuggerContractResolver"/> to correctly handle
    /// CRM types (Entity, AttributeCollection, OrganizationRequest, etc.).
    /// </summary>
    public static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ContractResolver = new RemoteDebuggerContractResolver()
    };
}
