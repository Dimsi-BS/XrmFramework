// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.Activities;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Client.ConsoleUI;
using XrmFramework.RemoteDebugger.Client.ManagerHub;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Common
{
    /// <summary>
    /// Remote debugger that receives plugin execution contexts via Azure Relay,
    /// executes the plugins locally, and returns the modified contexts.
    ///
    /// <para><b>Standard mode</b> (no TUI):</para>
    /// <code>
    /// var debugger = new RemoteDebugger&lt;AzureRelayHybridConnectionMessageManager&gt;();
    /// debugger.Start();
    /// </code>
    ///
    /// <para><b>Interactive TUI mode</b> — modern console interface:</para>
    /// <code>
    /// var debugger = new RemoteDebugger&lt;AzureRelayHybridConnectionMessageManager&gt;();
    /// debugger.SessionSavePath = @".\PluginTestSessions";
    /// debugger.StartWithConsoleUI();
    /// </code>
    /// </summary>
    public class RemoteDebugger<T> where T : IRemoteDebuggerMessageManager, new()
    {
        // ── Infrastructure ──────────────────────────────────────────────

        /// <summary>Message manager for Azure Relay communication.</summary>
        public T Manager { get; }

        // ── Session recording ────────────────────────────────────────────

        /// <summary>
        /// Directory where test sessions (.pluginsession.json) are saved.
        /// If <c>null</c> (default), no automatic saving is performed.
        /// In TUI mode, saving can also be triggered manually via [S].
        /// </summary>
        public string SessionSavePath { get; set; }

        /// <summary>
        /// Directory where exchanged messages (.json) are saved.
        /// If <c>null</c> (default), no automatic saving is performed.
        /// Each message (incoming/outgoing context, OrgService request/response, exception)
        /// is saved to a separate JSON file.
        /// </summary>
        public string MessageLogPath { get; set; }

        // ── Lifecycle events ─────────────────────────────────────────────

        /// <summary>Raised at the start of each plugin execution.</summary>
        public event Action<ExecutionRecord> ExecutionStarted;

        /// <summary>Raised at the start of each OrgService call.</summary>
        public event Action<ExecutionRecord, OrgServiceCallRecord> OrgServiceCallStarted;

        /// <summary>Raised at the end of each OrgService call.</summary>
        public event Action<ExecutionRecord, OrgServiceCallRecord> OrgServiceCallCompleted;

        /// <summary>Raised when an execution completes successfully.</summary>
        public event Action<ExecutionRecord> ExecutionCompleted;

        /// <summary>Raised when an execution fails with an exception.</summary>
        public event Action<ExecutionRecord, Exception> ExecutionFailed;

        // ── Message events ────────────────────────────────────────────────

        /// <summary>
        /// Raised when a message is received from the plugin (incoming context,
        /// OrgService response, etc.).
        /// </summary>
        public event Action<RemoteDebuggerMessage> MessageReceived;

        /// <summary>
        /// Raised when a message is sent to the plugin (outgoing context,
        /// OrgService request, exception).
        /// </summary>
        public event Action<RemoteDebuggerMessage> MessageSent;

        // ── Internal log ──────────────────────────────────────────────────

        // Standard mode: Console.WriteLine. TUI mode: ui.AddLog (without Spectre markup).
        private Action<string> _log = Console.WriteLine;

        // ── Manager connection (Plugin Monitor) ──────────────────────────

        /// <summary>
        /// SignalR connection settings to the Manager.
        /// When populated (non-empty URL + token), execution events
        /// are forwarded in real time to the Plugin Monitor interface.
        /// If the property is <c>null</c> or <see cref="ManagerHubSettings.IsConfigured"/>
        /// is <c>false</c>, the debugger's behavior remains unchanged.
        /// </summary>
        public ManagerHubSettings ManagerHub { get; set; }

        private ManagerHubForwarder _hubForwarder;

        // ── Constructor ───────────────────────────────────────────────────

        public RemoteDebugger()
        {
            Manager = new T();
        }

        // ════════════════════════════════════════════════════════════════
        // Standard mode — no TUI
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Starts the debugger in standard console mode.
        /// Blocks until the user presses Enter.
        /// </summary>
        public void Start()
        {
            InitHubForwarder(logMsg => _log(logMsg));

            Manager.ContextReceived += remoteContext =>
            {
                var record = new ExecutionRecord(remoteContext);
                ExecutionStarted?.Invoke(record);
                RunExecution(remoteContext, record);
            };

            Manager.RunAndBlock();
        }

        // ════════════════════════════════════════════════════════════════
        // TUI mode — interactive console interface
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Starts the debugger with the interactive console interface (Spectre.Console TUI).
        /// <para>
        /// Features:
        /// <list type="bullet">
        ///   <item>Live table of all executions (status, duration, CRM calls)</item>
        ///   <item>[Enter] Zoom in — detail of the selected execution</item>
        ///   <item>[ESC]   Zoom out — back to the list</item>
        ///   <item>[↑↓]   Navigate the list</item>
        ///   <item>[R]    Replay the execution (without debugger)</item>
        ///   <item>[D]    Replay in debug mode (attaches the debugger)</item>
        ///   <item>[S]    Save the session as a .pluginsession.json file</item>
        ///   <item>[Q]    Quit</item>
        /// </list>
        /// </para>
        /// Blocks until the user quits with [Q].
        /// </summary>
        public void StartWithConsoleUI()
        {
            var ui = new DebuggerConsoleUi(
                onSave: SaveSession,
                onReplay: ReplayExecution);

            // Redirect internal logs to the TUI (without Spectre markup)
            _log = msg => ui.AddLog($"[grey]{Markup.Escape(msg)}[/]");

            // ── Wire events to the TUI ───────────────────────────────────
            // Note: the render loop (120ms) reads OrgServiceCalls directly
            // from ExecutionRecord — no need to wire OrgServiceCallStarted/Completed.

            Manager.ContextReceived += remoteContext =>
            {
                // The TUI adds the record to its internal list and returns it
                var record = ui.NotifyExecutionStarted(remoteContext);
                ExecutionStarted?.Invoke(record);
                RunExecution(remoteContext, record);
            };

            ExecutionCompleted += rec => ui.NotifyExecutionCompleted(rec, rec.OutputContext);
            ExecutionFailed   += (rec, ex) => ui.NotifyExecutionFailed(rec, ex);

            // ── Manager connection (Plugin Monitor) ─────────────────────
            InitHubForwarder(msg => ui.AddLog($"[grey]{Markup.Escape(msg)}[/]"));

            // ── Azure Relay connection ────────────────────────────────────
            ui.AddLog("[grey]Opening Azure Relay connection…[/]");
            Manager.OpenAsync().GetAwaiter().GetResult();
            ui.AddLog($"[green]Connection established — PID {System.Diagnostics.Process.GetCurrentProcess().Id}[/]");
            ui.AddLog("[grey]Waiting for plugin executions…[/]");

            // ── Start the interface (blocks until [Q]) ────────────────────
            ui.Run();

            // ── Clean shutdown ────────────────────────────────────────────
            Manager.CloseAsync().GetAwaiter().GetResult();
        }

        // ════════════════════════════════════════════════════════════════
        // Execution core (shared between both modes)
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Executes a plugin locally, capturing all OrgService calls.
        /// Raises the lifecycle events at each step.
        /// </summary>
        private void RunExecution(RemoteDebugExecutionContext remoteContext, ExecutionRecord record)
        {
            // ── Incoming context ──────────────────────────────────────────
            var incomingContextMsg = new RemoteDebuggerMessage(
                RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
            OnMessageReceived(incomingContextMsg);

            // ── Service provider intercepting OrgService calls ───────────
            var serviceProvider = new LocalServiceProvider(remoteContext, record.AddTraceLog);

            serviceProvider.RequestSent += request =>
            {
                // Create and track the CRM call
                var call = record.BeginOrgServiceCall(request.Content);
                OrgServiceCallStarted?.Invoke(record, call);

                // The OrgService request message is emitted to the cloud
                OnMessageSent(request);

                // Forward to the CRM cloud
                var response = Manager.SendMessageWithResponse(request).GetAwaiter().GetResult();

                // The OrgService response is received from the cloud
                OnMessageReceived(response);

                // End of the call (idempotent — safe to call again)
                call.Complete(response.Content);
                OrgServiceCallCompleted?.Invoke(record, call);

                return response;
            };

            // ── Plugin execution on a dedicated thread ───────────────────
            var task = Task.Run(() => ExecutePluginType(remoteContext, serviceProvider));

            try
            {
                var pluginFound = task.GetAwaiter().GetResult();

                if (!pluginFound)
                {
                    // ── Type unknown locally -> return the context unchanged ──
                    // Mark the record as completed to stop the timer in the TUI.
                    record.Complete(remoteContext);
                    ExecutionCompleted?.Invoke(record);

                    var passthroughMsg = new RemoteDebuggerMessage(
                        RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
                    OnMessageSent(passthroughMsg);
                    Manager.SendMessage(passthroughMsg);
                    return;
                }

                // ── Success ────────────────────────────────────────────────
                // record.Complete() also builds the PluginTestSession for replay
                record.Complete(remoteContext);
                ExecutionCompleted?.Invoke(record);

                // Outgoing context (after local execution)
                var outgoingContextMsg = new RemoteDebuggerMessage(
                    RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
                OnMessageSent(outgoingContextMsg);

                // Automatic save to disk if SessionSavePath is configured
                if (SessionSavePath != null && record.TestSession != null)
                {
                    TrySaveSession(record.TestSession);
                }
            }
            catch (Exception e)
            {
                // ── Error ──────────────────────────────────────────────────
                record.Fail(e);
                ExecutionFailed?.Invoke(record, e);

                var exceptionMsg = new RemoteDebuggerMessage(
                    RemoteDebuggerMessageType.Exception, e, remoteContext.Id);
                OnMessageSent(exceptionMsg);
                Manager.SendMessage(exceptionMsg);
            }
        }

        /// <summary>
        /// Resolves the plugin type from its AssemblyQualifiedName and executes it.
        /// Returns <c>true</c> if the plugin was found and executed,
        /// <c>false</c> if the type is unknown locally (context to be returned unchanged).
        /// </summary>
        private bool ExecutePluginType(
            RemoteDebugExecutionContext remoteContext,
            LocalServiceProvider serviceProvider)
        {
            // Clean up the type name (remove Version= and PublicKeyToken= for portability)
            var parts = remoteContext.TypeAssemblyQualifiedName
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !p.StartsWith("Version=") && !p.StartsWith("PublicKeyToken=") && !p.StartsWith("Culture="))
                .ToList();

            var typeName = string.Join(", ", parts);
            var pluginType = Type.GetType(typeName);

            if (pluginType == null)
                return false;

            if (remoteContext.IsWorkflowContext)
            {
                var activity = (CodeActivity)Activator.CreateInstance(pluginType);
                var invoker = new WorkflowInvoker(activity);

                AddWorkflowExtension<IWorkflowContext>(serviceProvider, invoker);
                AddWorkflowExtension<IOrganizationServiceFactory>(serviceProvider, invoker);
                AddWorkflowExtension<IServiceEndpointNotificationService>(serviceProvider, invoker);
                AddWorkflowExtension<ITracingService>(serviceProvider, invoker);

                var inputs = remoteContext.Arguments.ToDictionary(k => k.Key, k => k.Value);
                var outputs = invoker.Invoke(inputs);

                remoteContext.Arguments.Clear();
                foreach (var kv in outputs)
                    remoteContext.Arguments[kv.Key] = kv.Value;
            }
            else
            {
                IPlugin plugin;
                if (pluginType.GetConstructor(new[] { typeof(string), typeof(string) }) != null)
                    plugin = (IPlugin)Activator.CreateInstance(
                        pluginType, remoteContext.UnsecureConfig, remoteContext.SecureConfig);
                else
                    plugin = (IPlugin)Activator.CreateInstance(pluginType);

                plugin.Execute(serviceProvider);
            }

            return true;
        }

        // ════════════════════════════════════════════════════════════════
        // Manager Hub connection
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Initializes and connects the <see cref="ManagerHubForwarder"/> if
        /// <see cref="ManagerHub"/> is configured. No effect otherwise.
        /// </summary>
        private void InitHubForwarder(Action<string> logAction)
        {
            if (ManagerHub?.IsConfigured != true) return;

            _hubForwarder = new ManagerHubForwarder(ManagerHub, logAction);
            _hubForwarder.ConnectAsync().GetAwaiter().GetResult();
            WireHubForwarder();
        }

        /// <summary>
        /// Subscribes the <see cref="ManagerHubForwarder"/> to all lifecycle
        /// events in fire-and-forget mode (does not disrupt the local flow).
        /// </summary>
        private void WireHubForwarder()
        {
            ExecutionStarted     += record        => _ = _hubForwarder.OnExecutionStartedAsync(record);
            OrgServiceCallStarted  += (record, call) => _ = _hubForwarder.OnOrgServiceCallStartedAsync(record, call);
            OrgServiceCallCompleted += (record, call) => _ = _hubForwarder.OnOrgServiceCallCompletedAsync(record, call);
            ExecutionCompleted   += record        => _ = _hubForwarder.OnExecutionCompletedAsync(record);
            ExecutionFailed      += (record, ex)  => _ = _hubForwarder.OnExecutionFailedAsync(record, ex);
        }

        // ════════════════════════════════════════════════════════════════
        // Message logging
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Raises the <see cref="MessageReceived"/> event and saves the message
        /// to disk if <see cref="MessageLogPath"/> is configured.
        /// </summary>
        private void OnMessageReceived(RemoteDebuggerMessage message)
        {
            MessageReceived?.Invoke(message);
            if (MessageLogPath != null)
                TryLogMessage(message, "IN");
        }

        /// <summary>
        /// Raises the <see cref="MessageSent"/> event and saves the message
        /// to disk if <see cref="MessageLogPath"/> is configured.
        /// </summary>
        private void OnMessageSent(RemoteDebuggerMessage message)
        {
            MessageSent?.Invoke(message);
            if (MessageLogPath != null)
                TryLogMessage(message, "OUT");
        }

        /// <summary>
        /// Saves a message to a timestamped JSON file.
        /// File name: <c>{PluginExecutionId}_{direction}_{MessageType}_{timestamp}.json</c>
        /// </summary>
        private void TryLogMessage(RemoteDebuggerMessage message, string direction)
        {
            try
            {
                Directory.CreateDirectory(MessageLogPath);
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                var fileName  = $"{message.PluginExecutionId}_{direction}_{message.MessageType}_{timestamp}.json";
                var json      = JsonConvert.SerializeObject(
                    message,
                    Formatting.Indented,
                    RemoteDebuggerSettings.JsonSerializerSettings);
                File.WriteAllText(
                    Path.Combine(MessageLogPath, fileName),
                    json,
                    Encoding.UTF8);
            }
            catch
            {
                // Non-blocking logging — silently ignore errors
            }
        }

        // ════════════════════════════════════════════════════════════════
        // Save and Replay
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Saves an ExecutionRecord's session to disk.
        /// Called from the TUI ([S]) or automatically if SessionSavePath is set.
        /// </summary>
        private void SaveSession(ExecutionRecord record)
        {
            if (record?.TestSession == null)
            {
                _log("Cannot save: no session available.");
                return;
            }

            var path = SessionSavePath ?? ".";
            TrySaveSession(record.TestSession, path);
        }

        private void TrySaveSession(PluginTestSession session, string path = null)
        {
            path ??= SessionSavePath ?? ".";
            try
            {
                var filePath = PluginTestSessionRecorder.Save(path, session);
                _log($"Session saved: {filePath}  ({session.OrgServiceCalls.Count} OrgService call(s))");
            }
            catch (Exception ex)
            {
                _log($"Save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Replays a recorded execution, optionally attaching the debugger.
        /// Called from the TUI ([R] or [D]).
        /// </summary>
        private void ReplayExecution(ExecutionRecord record, bool debugMode)
        {
            if (record?.TestSession == null)
            {
                _log("Cannot replay: session not available.");
                return;
            }

            if (debugMode)
            {
                _log($"Attach the debugger to PID {System.Diagnostics.Process.GetCurrentProcess().Id}, then the execution will start.");
                System.Diagnostics.Debugger.Launch();
            }

            try
            {
                _log($"Replaying #{record.Id} ({record.PluginShortName})...");
                var output = PluginTestRunner.Run(record.TestSession);
                _log($"Replay #{record.Id} completed — {output.OutputParameters?.Count ?? 0} OutputParam(s), {output.SharedVariables?.Count ?? 0} SharedVar(s).");
            }
            catch (Exception ex)
            {
                _log($"Replay #{record.Id} failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════════
        // Saved session browser
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Launches the interactive browser for test sessions saved on disk.
        /// <para>
        /// Displays a three-level console interface:
        /// <list type="bullet">
        ///   <item>Groups by <b>CorrelationId</b> — named after the first plugin triggered in the correlation.</item>
        ///   <item>List of <b>sessions</b> in the selected group.</item>
        ///   <item>Full <b>detail</b> of a session (input context / OrgService calls / output context).</item>
        /// </list>
        /// </para>
        /// Shortcuts: [↑↓] navigate · [Enter] zoom in · [Esc] go back ·
        ///              [R] replay · [D] replay in debug · [F5] reload · [Q] quit.
        /// </summary>
        /// <param name="sessionPath">
        ///   Directory containing the <c>*.pluginsession.json</c> files.
        ///   Uses <see cref="SessionSavePath"/> if not provided.
        /// </param>
        public void BrowseSessions(string sessionPath = null)
        {
            var path = sessionPath ?? SessionSavePath ?? ".";

            var ui = new SessionBrowserUi(
                sessionPath: path,
                onReplay: (session, debugMode) =>
                {
                    if (debugMode)
                    {
                        _log($"Attach the debugger to PID {System.Diagnostics.Process.GetCurrentProcess().Id}, then the execution will start.");
                        System.Diagnostics.Debugger.Launch();
                    }

                    try
                    {
                        var output = PluginTestRunner.Run(session);
                        _log($"Replay completed — {output.OutputParameters?.Count ?? 0} OutputParam(s), {output.SharedVariables?.Count ?? 0} SharedVar(s).");
                    }
                    catch (Exception ex)
                    {
                        _log($"Replay failed: {ex.GetType().Name}: {ex.Message}");
                    }
                });

            ui.Run();
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static void AddWorkflowExtension<TService>(
            IServiceProvider provider, WorkflowInvoker invoker)
            where TService : class
        {
            var service = provider.GetService(typeof(TService));
            if (service != null)
                invoker.Extensions.Add(() => (TService)service);
        }
    }
}
