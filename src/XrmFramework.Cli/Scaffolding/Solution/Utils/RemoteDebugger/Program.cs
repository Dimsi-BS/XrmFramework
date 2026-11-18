using System;
using System.IO;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Client.ManagerHub;
using XrmFramework.RemoteDebugger.Common;

// Both target frameworks run this same file. The two lines below are what make that possible: each
// one does its work on the framework that needs it and nothing on the other.

// A .NET runner has no App.config. Its build copies the solution's Config/ folder next to the
// executable, and configuration is read from there. A .NET Framework runner reads its own
// App.config and has no such folder, so this does nothing.
if (Directory.Exists(Path.Combine(AppContext.BaseDirectory, "Config")))
{
    ConfigHelper.UseProjectConfig(AppContext.BaseDirectory);
}

// A .NET runner cannot reference the .NET Framework Plugins project, so its build copies the plugin
// output into plugins/ and assemblies are resolved from there - which is what lets the debugger
// resolve the plugin type Dataverse asks for. On .NET Framework the project reference already put
// them in place and plugins/ does not exist, so this does nothing.
PluginAssemblyLoader.UsePluginDirectory("plugins");

var remoteDebugger = new RemoteDebugger<AzureRelayHybridConnectionMessageManager>();

// Connection to the Manager (Plugin Monitor) — identical to the Desktop application.
// ApiUrl comes from configuration (appSettings "ApiUrl"), or from an environment variable of the
// same name, which is how ClientId and Tenant are already read. If it ends up empty, the connection
// is silently skipped.
var apiUrl = ConfigHelper.GetAppSetting("ApiUrl")
             ?? Environment.GetEnvironmentVariable("ApiUrl");

var hubSettings = ManagerHubSettings.FromEnvironment(apiUrl);
if (hubSettings.IsConfigured)
{
    remoteDebugger.ManagerHub = hubSettings;
}

// The Azure Relay connection string comes from the "DebugConnectionString" connection string, or
// from the XRMFRAMEWORK_DEBUG_RELAY environment variable, which takes precedence.
remoteDebugger.StartWithConsoleUI();
