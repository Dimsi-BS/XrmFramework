// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Configuration;
using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Client.ManagerHub;
using XrmFramework.RemoteDebugger.Common;

var remoteDebugger = new RemoteDebugger<AzureRelayHybridConnectionMessageManager>();

// Connection to the Manager (Plugin Monitor) — identical to the Desktop application.
// Reads ApiUrl from App.config (appSettings "ApiUrl").
// Reads ClientId and Tenant from the user environment variables,
// just like the Desktop's Program.cs does.
// If ApiUrl is empty or missing, the connection is silently skipped.
var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
var hubSettings = ManagerHubSettings.FromEnvironment(apiUrl);
if (hubSettings.IsConfigured)
{
    remoteDebugger.ManagerHub = hubSettings;
}

remoteDebugger.StartWithConsoleUI();
