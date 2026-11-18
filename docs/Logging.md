# Logging and telemetry

XrmFramework writes logs to two channels at once:

| Channel | Backed by | Where it lands | Retention |
|---|---|---|---|
| Tracing | `Microsoft.Xrm.Sdk.ITracingService` | `PluginTraceLog` table (plugin trace logs) | 24 h, 10 kb per execution |
| Telemetry | `Microsoft.Xrm.Sdk.PluginTelemetry.ILogger` | the Application Insights resource configured on the environment | 90 days by default, queryable with KQL |

The tracing channel behaves exactly as before: same messages, same text, no level prefix. The
telemetry channel is additional, and writes nothing at all when the environment has no
Application Insights integration.

## Enabling Application Insights

Telemetry needs no code and no connection string in the plugin. An administrator enables it once
per environment:

> Power Platform admin center → Environments → *your environment* → Settings → Resources →
> Application Insights → Configure

Give each environment its own Application Insights resource. Until this is configured,
`ILogger` is not provided by the sandbox and the framework silently keeps tracing only.

## Writing logs from a plugin

`context.Log(...)` keeps working and now also reaches Application Insights at the `Information`
level. Level aware methods are available on plugin, Custom API, workflow and service contexts:

```csharp
public class AccountPostCreate : Plugin
{
    protected override void AddSteps()
        => AddStep(Stages.PostOperation, Messages.Create, Modes.Synchronous, AccountDefinition.EntityName, nameof(OnCreate));

    public void OnCreate(IPluginContext context, IAccountService accountService)
    {
        context.LogInformation("Account {0} created", context.PrimaryEntityId);

        using (context.BeginScope("Scoring"))
        {
            context.LogDebug("Computing score");

            try
            {
                accountService.ComputeScore(context.PrimaryEntityId);
            }
            catch (Exception e)
            {
                context.LogError(e, "Scoring failed for {0}", context.PrimaryEntityId);
                throw new InvalidPluginExecutionException(e.Message, e);
            }
        }
    }
}
```

| Method | Telemetry level | Notes |
|---|---|---|
| `Log(message, args)` | `Information` | unchanged behaviour on the tracing side |
| `Log(XrmLogLevel.X, message, args)` | explicit | |
| `LogTrace` / `LogDebug` / `LogInformation` / `LogWarning` | matching level | |
| `LogError(exception, message, args)` | `Error` | the exception is passed typed, so it shows up in the `exceptions` table |
| `LogCritical(exception, message, args)` | `Critical` | |
| `BeginScope(name)` | — | the scope shows in the `customDimensions` of every entry written inside it. No effect on the tracing channel |
| `IsTelemetryEnabled(level)` | — | guards expensive diagnostics. The tracing channel always writes, whatever it returns |

What the framework itself writes:

| Framework trace | Level |
|---|---|
| plugin entry, class name, step `Start` / `End, duration`, `Exiting` | `Information` |
| step skipped by filtering attributes, timestamps, user ids, input/shared variables dumps, workflow arguments dumps | `Debug` |
| per call traces of the generated `Logged<IService>` wrappers | `Debug` |
| exceptions caught by the plugin pipeline | `Error` |

Each step runs inside a scope named `{PluginClass}.{StepMethod}`.

## Choosing the minimum level

The default minimum level sent to Application Insights is `Information`, so the verbose dumps and
the per-service-call traces stay out of it. Change it for a whole plugin assembly with an assembly
level attribute, typically in `AssemblyInfo.cs`:

```csharp
[assembly: TelemetryOptions(XrmLogLevel.Debug)]   // send everything
[assembly: TelemetryOptions(XrmLogLevel.Warning)] // warnings and errors only
[assembly: TelemetryOptions(XrmLogLevel.None)]    // disable telemetry completely
```

Keep using the tracing channel: not every environment enables Application Insights, and plugin
trace logs remain the tool of choice while developing and debugging.

## Querying Application Insights

Entries written by your plugins run in the sandbox, so they carry
`cloud_RoleInstance == "SandboxRoleInstance"`:

```kusto
traces
| where cloud_RoleInstance == "SandboxRoleInstance"
| where timestamp > ago(30m)
| project timestamp, message, severityLevel, customDimensions
| order by timestamp desc
```

Entries written inside a given scope — the scope name is carried by `customDimensions`, inspect
one row to see under which key your resource stores it:

```kusto
traces
| where cloud_RoleInstance == "SandboxRoleInstance"
| where customDimensions contains "AccountPostCreate.OnCreate"
| project timestamp, message, severityLevel, customDimensions
```

Exceptions logged with `LogError` / `LogCritical`:

```kusto
exceptions
| where cloud_RoleInstance == "SandboxRoleInstance"
| project timestamp, type, outerMessage, customDimensions
```

Entries of one request are correlated by `operation_ParentId`, which matches the request id
returned in the response headers of the originating Web API call.

## Limitations

- Telemetry is unavailable in a Plugin Registration Tool profiling/debug session and in a
  XrmFramework [RemoteDebugger](RemoteDebugger.md) session: the replayed service provider does not
  offer `ILogger`. Tracing keeps working, so nothing is lost locally.
- Console applications and Azure Functions (contexts built on `ServiceContextBase`) have no
  sandbox telemetry logger; the level aware methods work there and fall back to the host log
  (`Console.WriteLine` by default). Azure Functions already report to Application Insights
  through their own host instrumentation.
- A custom logger declared with `[assembly: LoggerClass(typeof(MyLogger))]` keeps telemetry as
  long as it derives from `DefaultLogHelper`. A class implementing `ILogger` from scratch writes
  to its own channel only.
- Sampling is not implemented on the plugin side; configure it on the Application Insights
  resource if ingestion volume becomes a concern.
