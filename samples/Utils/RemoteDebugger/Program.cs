// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Configuration;
using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Client.ManagerHub;
using XrmFramework.RemoteDebugger.Common;

var remoteDebugger = new RemoteDebugger<AzureRelayHybridConnectionMessageManager>();

// Connexion au Manager (Plugin Monitor) — identique à l'application Desktop.
// Lit ApiUrl depuis App.config (appSettings "ApiUrl").
// Lit ClientId et Tenant depuis les variables d'environnement utilisateur,
// comme le fait Program.cs du Desktop.
// Si ApiUrl est vide ou absent, la connexion est ignorée silencieusement.
var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
var hubSettings = ManagerHubSettings.FromEnvironment(apiUrl);
if (hubSettings.IsConfigured)
{
    remoteDebugger.ManagerHub = hubSettings;
}

remoteDebugger.StartWithConsoleUI();
