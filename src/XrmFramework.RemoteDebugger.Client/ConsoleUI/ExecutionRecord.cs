// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client.ConsoleUI;

/// <summary>
/// Status of a plugin execution in the remote debugger.
/// </summary>
public enum ExecutionStatus
{
    /// <summary>Execution in progress.</summary>
    Running,
    /// <summary>Execution completed successfully.</summary>
    Succeeded,
    /// <summary>Execution completed with an error.</summary>
    Failed
}

/// <summary>
/// Represents a plugin or workflow activity execution tracked by the remote debugger.
/// Contains all the information needed to display, analyze, and replay the execution.
/// </summary>
public class ExecutionRecord
{
    private static int _nextId;
    private readonly object _lock = new();

    public ExecutionRecord(RemoteDebugExecutionContext inputContext)
    {
        Id = System.Threading.Interlocked.Increment(ref _nextId);
        InputContext = inputContext;
        StartTime = DateTime.Now;
        Status = ExecutionStatus.Running;

        // Extract the plugin's short name
        PluginShortName = ExtractShortTypeName(inputContext.TypeAssemblyQualifiedName);
    }

    /// <summary>Sequential identifier of the execution (1, 2, 3...).</summary>
    public int Id { get; }

    /// <summary>Short name of the plugin type (without namespace or assembly info).</summary>
    public string PluginShortName { get; }

    /// <summary>Name of the CRM message (Create, Update, Delete...).</summary>
    public string MessageName => InputContext.MessageName ?? "";

    /// <summary>Logical name of the primary entity.</summary>
    public string PrimaryEntityName => InputContext.PrimaryEntityName ?? "";

    /// <summary>ID of the primary entity.</summary>
    public Guid PrimaryEntityId => InputContext.PrimaryEntityId;

    /// <summary>Current status of the execution.</summary>
    public ExecutionStatus Status { get; private set; }

    /// <summary>Start time of the execution.</summary>
    public DateTime StartTime { get; }

    /// <summary>Total duration (null if still running).</summary>
    public TimeSpan? Duration { get; private set; }

    /// <summary>Input context (snapshot before the plugin executes).</summary>
    public RemoteDebugExecutionContext InputContext { get; }

    /// <summary>Output context (after the plugin executes), null if still running.</summary>
    public RemoteDebugExecutionContext OutputContext { get; private set; }

    /// <summary>Exception raised during execution, null on success.</summary>
    public Exception Error { get; private set; }

    /// <summary>List of OrgService calls made during the execution.</summary>
    public IReadOnlyList<OrgServiceCallRecord> OrgServiceCalls => _orgServiceCalls;
    private readonly List<OrgServiceCallRecord> _orgServiceCalls = new();

    /// <summary>Trace logs emitted by the plugin via <c>ITracingService.Trace</c>.</summary>
    public IReadOnlyList<string> TraceLogs => _traceLogs;
    private readonly List<string> _traceLogs = new();

    /// <summary>
    /// Complete test session for replaying this execution.
    /// Available only after the execution has completed.
    /// </summary>
    public PluginTestSession TestSession { get; private set; }

    /// <summary>
    /// Displayed duration (running or completed).
    /// </summary>
    public TimeSpan ElapsedTime => Duration ?? (DateTime.Now - StartTime);

    /// <summary>
    /// Number of OrgService calls made (including ones still running).
    /// </summary>
    public int OrgServiceCallCount
    {
        get { lock (_lock) { return _orgServiceCalls.Count; } }
    }

    /// <summary>
    /// Adds a trace line emitted by the plugin via <c>ITracingService</c>.
    /// Called from the callback passed to <c>LocalServiceProvider</c>.
    /// </summary>
    internal void AddTraceLog(string message)
    {
        lock (_lock)
        {
            _traceLogs.Add(message);
        }
    }

    /// <summary>
    /// Creates a new OrgService call and adds it to the list.
    /// </summary>
    internal OrgServiceCallRecord BeginOrgServiceCall(string requestJson)
    {
        var call = new OrgServiceCallRecord(requestJson);
        lock (_lock)
        {
            call.Index = _orgServiceCalls.Count + 1;
            _orgServiceCalls.Add(call);
        }
        return call;
    }

    /// <summary>
    /// Marks the execution as completed successfully and builds the test session.
    /// </summary>
    internal void Complete(RemoteDebugExecutionContext outputContext)
    {
        OutputContext = outputContext;
        Duration = DateTime.Now - StartTime;
        Status = ExecutionStatus.Succeeded;

        BuildTestSession();
    }

    /// <summary>
    /// Marks the execution as failed.
    /// </summary>
    internal void Fail(Exception error)
    {
        Error = error;
        Duration = DateTime.Now - StartTime;
        Status = ExecutionStatus.Failed;
    }

    /// <summary>
    /// Builds the test session from the recorded data.
    /// </summary>
    private void BuildTestSession()
    {
        // Deep copy of the input context via JSON round-trip
        RemoteDebugExecutionContext inputCopy;
        try
        {
            var json = JsonConvert.SerializeObject(InputContext, RemoteDebuggerSettings.JsonSerializerSettings);
            inputCopy = JsonConvert.DeserializeObject<RemoteDebugExecutionContext>(
                json, RemoteDebuggerSettings.JsonSerializerSettings);
        }
        catch
        {
            inputCopy = InputContext;
        }

        var session = new PluginTestSession
        {
            PluginTypeAssemblyQualifiedName = InputContext.TypeAssemblyQualifiedName,
            Timestamp = StartTime,
            SessionId = Guid.NewGuid(),
            // UTC time of the start of the execution: used during replay to
            // inject FixedDateTimeProvider and make relative dates reproducible.
            ExecutionDate = StartTime.Kind == DateTimeKind.Utc ? StartTime : StartTime.ToUniversalTime(),
            InputContext = inputCopy,
            OutputContext = OutputContext
        };

        lock (_lock)
        {
            foreach (var call in _orgServiceCalls)
            {
                session.OrgServiceCalls.Add(new RecordedOrgServiceCall
                {
                    RequestJson = call.RequestJson,
                    ResponseJson = call.ResponseJson
                });
            }

            foreach (var log in _traceLogs)
                session.TraceLogs.Add(log);
        }

        TestSession = session;
    }

    /// <summary>Extracts the simple type name from the fully qualified name.</summary>
    private static string ExtractShortTypeName(string assemblyQualifiedName)
    {
        if (string.IsNullOrEmpty(assemblyQualifiedName)) return "UnknownPlugin";

        var typePart = assemblyQualifiedName.Split(new[] { ',' }, 2)[0].Trim();
        var lastDot = typePart.LastIndexOf('.');
        return lastDot >= 0 ? typePart.Substring(lastDot + 1) : typePart;
    }
}

public class RecordedOrgServiceCall
{
    public string RequestJson { get; set; }
    public string ResponseJson { get; set; }
}

public class PluginTestSession
{
    public string PluginTypeAssemblyQualifiedName { get; set; }

    /// <summary>Timestamp of the execution (local time), used to name the session file.</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>Unique identifier of the session, used to distinguish multiple sessions of the same plugin.</summary>
    public Guid SessionId { get; set; } = Guid.NewGuid();

    public DateTime ExecutionDate { get; set; }

    public RemoteDebugExecutionContext InputContext { get; set; }

    public RemoteDebugExecutionContext OutputContext { get; set; }

    public IList<RecordedOrgServiceCall> OrgServiceCalls { get; set; } = new List<RecordedOrgServiceCall>();

    /// <summary>
    /// Logs emitted by the plugin via <c>ITracingService.Trace</c> during the execution.
    /// Kept in the session for analysis and replay.
    /// </summary>
    public IList<string> TraceLogs { get; set; } = new List<string>();
}
