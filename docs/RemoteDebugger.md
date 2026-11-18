# Remote Debugger

- [Remote Debugger](#remote-debugger)
  - [Overview](#overview)
  - [How It Works](#how-it-works)
  - [Prerequisites](#prerequisites)
  - [Setting Up a Debug Session](#setting-up-a-debug-session)
    - [Create the Debug Session Record](#create-the-debug-session-record)
    - [Start the Local Debugger](#start-the-local-debugger)
  - [Standard Mode](#standard-mode)
  - [Interactive Console (TUI) Mode](#interactive-console-tui-mode)
    - [TUI Keyboard Shortcuts](#tui-keyboard-shortcuts)
  - [Debugging from macOS or Linux](#debugging-from-macos-or-linux)
    - [Why the plugin project stays on .NET Framework](#why-the-plugin-project-stays-on-net-framework)
    - [Configuration without an App.config](#configuration-without-an-appconfig)
  - [Session Recording and Replay](#session-recording-and-replay)
    - [Automatic Session Save](#automatic-session-save)
    - [Manual Replay](#manual-replay)
  - [Lifecycle Events](#lifecycle-events)
  - [Disabling the Remote Debugger](#disabling-the-remote-debugger)
  - [How It Integrates with the Plugin Pipeline](#how-it-integrates-with-the-plugin-pipeline)
  - [Custom Workflow Activities](#custom-workflow-activities)
  - [Troubleshooting](#troubleshooting)

---

## Overview

The XrmFramework Remote Debugger lets you step through plugin and Custom API execution **in your local IDE** while the plugin is triggered by real user actions (or automated processes) on a live Dataverse environment — including dev, UAT, or even production sandboxes. Visual Studio on Windows, or Rider / VS Code on macOS and Linux — see [Debugging from macOS or Linux](#debugging-from-macos-or-linux).

The mechanism relies on **Azure Relay Hybrid Connections**: when a plugin fires in Dataverse, the framework checks whether the initiating (or root) user has an active `DebugSession` record. If so, the full execution context is serialised and forwarded over the relay to your local machine, which executes the plugin code locally. Any `IOrganizationService` calls made by the local plugin are transparently forwarded back to Dataverse through the same relay channel, so you work with real data.

---

## How It Works

```
Dataverse sandbox                    Azure Relay                  Developer machine
─────────────────                    ───────────                  ─────────────────
Plugin fires
  │
  ├─ DebugSession found for user?
  │     No -> execute normally
  │     Yes ──────────────────────► Hybrid Connection ──────────► RemoteDebugger receives context
  │                                                                │
  │                                                                ├─ Resolves plugin type locally
  │                                                                ├─ Executes plugin in local process
  │                                                                │     (breakpoints, watches, etc.)
  │                                                                │
  │◄──────────────────────────────── Hybrid Connection ◄───────── Returns modified context (or exception)
  │
  └─ Dataverse applies the updated context
```

Every `IOrganizationService` call emitted by your local plugin is intercepted, forwarded to Dataverse through the relay, and the response is returned to the local plugin — making the debugging experience fully transparent with respect to real data.

---

## Prerequisites

- An **Azure Relay namespace** with a Hybrid Connection endpoint.  
- The `DebugSession` entity deployed in your Dataverse environment (it is part of the XrmFramework managed solution).  
- The `XrmFramework.RemoteDebugger.Client` NuGet package referenced in your local runner project.
- An IDE that can debug the runner process: Visual Studio, Rider, or VS Code with the C# extension. Nothing needs to be installed on the Dataverse side beyond the solution.

---

## Setting Up a Debug Session

### Create the Debug Session Record

Create a `DebugSession` record in Dataverse (via the model-driven app or via code) with the following fields:

| Field | Description |
|-------|-------------|
| `DebugeeId` | The Dataverse user whose actions will be intercepted (lookup to `systemuser`). |
| `SessionEnd` | Expiry date/time of the session. The plugin checks `SessionEnd >= DateTime.Today`; expired sessions are ignored automatically. |
| `RelayUrl` | Base URL of your Azure Relay namespace (e.g. `https://mynamespace.servicebus.windows.net`). |
| `HybridConnectionName` | Name of the Hybrid Connection within the namespace. |
| `SasKeyName` | The Shared Access Signature key name (e.g. `RootManageSharedAccessKey`). |
| `SasConnectionKey` | The corresponding SAS key value. |

> **Tip:** Keep sessions short-lived. Set `SessionEnd` to a few hours ahead of your debugging session to avoid accidental interception after you finish.

### Start the Local Debugger

A scaffolded solution already has the runner, in `Utils/RemoteDebugger`. It targets **two frameworks**, and which one you run decides what you can debug:

| Run it on | Debugs | Runs on |
|-----------|--------|---------|
| .NET Framework (`net48`, or `net462`) | plugins, Custom APIs **and Custom Workflow Activities** | Windows |
| .NET (`net10.0`) | plugins and Custom APIs | Windows, **macOS, Linux** |

```bash
dotnet run -f net10.0        # or -f net48 on Windows
```

Your IDE's run configuration will also let you pick. Either way the **plugin project is not retargeted**: it stays on .NET Framework, which is what Dataverse requires, and the assembly under the debugger is the one that gets deployed.

Writing the runner by hand instead is fine — reference the `XrmFramework.RemoteDebugger.Client` package and your plugin assembly, then choose a mode (see below).

---

## Standard Mode

Simple console mode — blocks until the user presses **Enter**:

```csharp
using XrmFramework.RemoteDebugger.Common;

var debugger = new RemoteDebugger<AzureRelayHybridConnectionMessageManager>();
debugger.Start();
```

In standard mode:
- Each intercepted execution is printed to the console.
- You can set breakpoints in your plugin code in Visual Studio, attach to the runner process, and they will be hit when the plugin fires.

---

## Interactive Console (TUI) Mode

The TUI mode provides a rich, real-time terminal interface built with **Spectre.Console**:

```csharp
using XrmFramework.RemoteDebugger.Common;

var debugger = new RemoteDebugger<AzureRelayHybridConnectionMessageManager>();
debugger.SessionSavePath = @".\PluginTestSessions";   // optional — enables auto-save
debugger.StartWithConsoleUI();
```

The TUI displays a live table of all intercepted executions, each row showing:
- Execution status (pending / running / completed / failed)
- Plugin type (short name)
- Elapsed duration
- Number of `IOrganizationService` calls made

### TUI Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `↑` / `↓` | Navigate the execution list |
| `Enter` | Zoom in — show full detail of the selected execution |
| `Esc` | Zoom out — return to the list view |
| `R` | Replay the selected execution without the debugger |
| `D` | Replay the selected execution in debug mode — on Windows, prompts you to attach the debugger first |
| `S` | Save the selected execution as a `.pluginsession.json` file |
| `Q` | Quit |

> **On macOS and Linux**, `D` behaves like `R`: the attach prompt it relies on (`Debugger.Launch()`) only exists on Windows. Start the runner from your IDE's debugger instead — your breakpoints are then already live, and `R` is enough.

---

## Debugging from macOS or Linux

Run the runner on its `net10.0` target and everything above works as it does on Windows — same relay, same TUI, same session files. Plugins and Custom APIs are fully debuggable; **Custom Workflow Activities are not** (see [below](#custom-workflow-activities)).

```bash
cd Utils/RemoteDebugger
dotnet run -f net10.0
```

To stop on breakpoints, start it under your IDE's debugger rather than from the shell: in **Rider**, the run configuration generated for the project, with `net10.0` selected; in **VS Code**, the C# extension's generated launch configuration. Breakpoints go in your plugin source as usual.

### Why the plugin project stays on .NET Framework

It is not a compromise — it is the point. Dataverse only accepts .NET Framework plugin assemblies, so the `Plugins` project keeps its `net48` target and **the assembly you debug is byte-for-byte the one you deploy**. A .NET runtime loads and runs it as it is.

That works because the `Microsoft.Xrm.Sdk` shipped for .NET carries the same assembly identity as the .NET Framework one, so the plugin binds to the runner's copy and accepts the execution context it is handed. The runner's build copies the plugin's output into a `plugins/` folder and resolves it from there, since a .NET project cannot reference a .NET Framework one directly.

### Configuration without an App.config

A .NET runner has no App.config. The scaffolded project copies the solution's `Config/` folder next to the executable and reads it from there, so `connectionStrings.config` works unchanged.

If you would rather not keep an Azure Relay SAS key in a file, set it in the environment instead — this takes precedence over configuration on every target framework:

```bash
export XRMFRAMEWORK_DEBUG_RELAY='Endpoint=sb://my-namespace.servicebus.windows.net/;SharedAccessKeyName=…;SharedAccessKey=…;EntityPath=…'
```

---

## Session Recording and Replay

Every intercepted execution can be persisted to disk as a **PluginTestSession** (`.pluginsession.json`). This file captures the full execution context, all `IOrganizationService` responses, and the final output context. It can be replayed locally without requiring any live Dataverse connection.

### Automatic Session Save

Set `SessionSavePath` before calling `StartWithConsoleUI()` or `Start()`:

```csharp
debugger.SessionSavePath = @".\PluginTestSessions";
```

After each successful execution, the session is automatically saved to that directory.

### Manual Replay

Replay a saved session directly from code — useful for unit / integration tests:

```csharp
using XrmFramework.RemoteDebugger.Client;

var json   = File.ReadAllText(@".\PluginTestSessions\mySession.pluginsession.json");
var output = PluginTestRunner.RunFromJson(json);

Console.WriteLine($"OutputParameters: {output.OutputParameters?.Count ?? 0}");
Console.WriteLine($"SharedVariables:  {output.SharedVariables?.Count ?? 0}");
```

`PluginTestRunner.RunFromJson` resolves the plugin type from the saved assembly-qualified name, re-creates the full `IServiceProvider` from the recorded responses, and executes the plugin locally — making it ideal for **regression tests** after a code change. `PluginTestRunner.Run` takes an already-deserialized `PluginTestSession` if you have one.

---

## Lifecycle Events

`RemoteDebugger<T>` exposes events you can subscribe to in order to integrate with your own logging or monitoring infrastructure:

```csharp
var debugger = new RemoteDebugger<AzureRelayHybridConnectionMessageManager>();

debugger.ExecutionStarted          += record => Console.WriteLine($"[START] {record.PluginShortName}");
debugger.OrgServiceCallStarted     += (record, call) => Console.WriteLine($"  -> {call}");
debugger.OrgServiceCallCompleted   += (record, call) => Console.WriteLine($"  ← {call.Duration}");
debugger.ExecutionCompleted        += record => Console.WriteLine($"[OK]    {record.PluginShortName} ({record.Duration})");
debugger.ExecutionFailed           += (record, ex) => Console.WriteLine($"[FAIL]  {record.PluginShortName}: {ex.Message}");

debugger.StartWithConsoleUI();
```

| Event | Fired when |
|-------|-----------|
| `ExecutionStarted` | A new plugin execution context is received from the relay. |
| `OrgServiceCallStarted` | The local plugin issues an `IOrganizationService` request. |
| `OrgServiceCallCompleted` | The response from Dataverse is returned to the local plugin. |
| `ExecutionCompleted` | The local execution finishes successfully and the updated context is sent back. |
| `ExecutionFailed` | The local execution throws an unhandled exception. |

---

## Disabling the Remote Debugger

The redirect logic is compiled out when the `DISABLE_REMOTE_DEBUG` preprocessor symbol is defined. Add it to your plugin project's release configuration to ensure zero overhead in production:

```xml
<!-- In your plugin .csproj -->
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <DefineConstants>DISABLE_REMOTE_DEBUG</DefineConstants>
</PropertyGroup>
```

When the symbol is defined, `SendToRemoteDebugger` always returns `false` and no `DebugSession` lookup is performed.

---

## How It Integrates with the Plugin Pipeline

Inside `Plugin.Execute`, just before the matching step methods are dispatched, the framework calls `SendToRemoteDebugger`:

1. If the context is already a debug context (i.e. it came from the remote debugger itself), the redirect is skipped.
2. The framework queries for an active `DebugSession` whose `DebugeeId` matches the initiating or root user.
3. If a valid session is found, the full `RemoteDebugExecutionContext` is serialised and sent over the Hybrid Connection.
4. The relay blocks, forwarding every `IOrganizationService` request/response in both directions until the local plugin returns its updated context (or raises an exception).
5. The returned context is merged back into the Dataverse execution context, and the plugin returns normally.
6. If the relay endpoint is unreachable (e.g. the local runner is not started), an `HttpRequestException` is caught silently and the plugin executes normally in Dataverse.

---

## Custom Workflow Activities

A custom workflow activity goes through the same relay, with one difference that matters: it is a
`CodeActivity`, not an `IPlugin`. The runner therefore instantiates it and runs it through a
`WorkflowInvoker`, passing the workflow extensions (`IWorkflowContext`,
`IOrganizationServiceFactory`, `IServiceEndpointNotificationService`, `ITracingService`) rather
than calling `Execute` on a service provider.

Its `InArgument` / `InOutArgument` properties are read into the context before it is sent, and the
`OutArgument` / `InOutArgument` values the local run produces are written back into the activity
when the context returns — that is what `RemoteDebugExecutionContext.Arguments` carries.

Which of the two the runner builds is decided from the type it resolved, so an activity is run as
an activity whatever the context says. `IsWorkflowContext` on the context is a hint, set when the
captured context is an `IWorkflowContext`.

> **Only on .NET Framework.** A workflow activity is a `CodeActivity` run by a `WorkflowInvoker`,
> both from `System.Activities` — Windows Workflow Foundation, which Microsoft never ported past
> .NET Framework. `Microsoft.Xrm.Sdk.Workflow`, where `IWorkflowContext` lives, ships for `net462`
> only for the same reason. So a runner on `net10.0` refuses an activity with a
> `PlatformNotSupportedException` saying so, rather than failing obscurely. Run the runner on
> `net48` to debug one — on Windows, or in a Windows VM. Plugins and Custom APIs **in the same
> assembly** debug fine on .NET; it is the activity itself that cannot run.

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---------|-------------|-----|
| Plugin executes normally despite an active `DebugSession` | The runner is not started, or the Hybrid Connection URL / SAS key is wrong | Start the runner first; verify the relay settings in the `DebugSession` record |
| `Unable to cast object of type '<YourActivity>' to type 'Microsoft.Xrm.Sdk.IPlugin'` | A custom workflow activity reached the runner flagged as a plugin — fixed; the runner now decides from the resolved type | Update the XrmFramework packages on both sides |
| `'<YourType>' is neither an IPlugin nor a CodeActivity` | The resolved type is neither kind, so there is nothing to run | A workflow activity must derive from `CodeActivity`, a plugin must implement `IPlugin` |
| `InvalidPluginExecutionException: No type found` | The plugin type could not be resolved locally | Ensure the plugin assembly is loaded by the runner (add a project reference or load it explicitly) |
| `No type found`, on a `net10.0` runner only | The `plugins/` folder next to the runner is empty, so the plugin assembly cannot be resolved | Rebuild the runner — its build copies the Plugins project's output there. If you wrote the runner by hand, call `PluginAssemblyLoader.UsePluginDirectory()` and point it at that output |
| `PlatformNotSupportedException` naming your activity | A Custom Workflow Activity on a `net10.0` runner | Run the runner on `net48` — see [Custom Workflow Activities](#custom-workflow-activities) |
| `No Azure Relay connection string` | Neither `XRMFRAMEWORK_DEBUG_RELAY` nor a `DebugConnectionString` connection string is set | Set the environment variable, or check `Config/connectionStrings.config` is beside the runner |
| Session expired immediately | `SessionEnd` set to a past date | Update the `SessionEnd` field to a future date/time |
| TUI shows execution as failed with `HttpRequestException` | Runner stopped while execution was in progress | Restart the runner; the plugin will execute normally in Dataverse for that invocation |

---

*See also: [Plugins](Plugins.md) · [Custom APIs](CustomApis.md)*
