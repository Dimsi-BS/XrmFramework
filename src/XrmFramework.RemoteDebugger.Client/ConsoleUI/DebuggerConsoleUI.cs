// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Rendering;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client.ConsoleUI;

/// <summary>
/// Modern console interface for the XrmFramework remote debugger.
/// Displays plugin executions in real time, allows zooming into
/// each execution to analyze OrgService calls,
/// and offers to replay an execution in debug mode.
/// </summary>
public class DebuggerConsoleUi(
    Action<ExecutionRecord> onSave = null,
    Action<ExecutionRecord, bool> onReplay = null)
{
    // ── View state ─────────────────────────────────────────────────────
    private enum View { List, Detail }

    private View _currentView = View.List;
    private readonly List<ExecutionRecord> _executions = new();
    private int _selectedIndex;
    private int _traceScrollOffset;
    private const int TracePageSize = 10;
    private readonly object _lock = new();

    // ── Message log ────────────────────────────────────────────────────
    private readonly List<string> _logs = new();
    private const int MaxLogs = 6;

    // ── Lifecycle control ─────────────────────────────────────────────
    private CancellationTokenSource _cts;

    // ── Application title ─────────────────────────────────────────────
    private const string AppTitle = "XrmFramework Remote Debugger";

    // ════════════════════════════════════════════════════════════════
    // Public API — called from RemoteDebugger<T>
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Records the start of a new plugin execution.
    /// </summary>
    public ExecutionRecord NotifyExecutionStarted(RemoteDebugExecutionContext context)
    {
        var record = new ExecutionRecord(context);
        lock (_lock)
        {
            _executions.Add(record);
            // Automatically select the latest execution
            _selectedIndex = _executions.Count - 1;
        }
        AddLog($"[grey]Execution #{record.Id} started:[/] [cyan]{record.PluginShortName}[/] · {record.MessageName} · {record.PrimaryEntityName}");
        return record;
    }

    /// <summary>
    /// Records an OrgService call on the current execution.
    /// </summary>
    public OrgServiceCallRecord NotifyOrgServiceCallStarted(ExecutionRecord record, string requestJson)
    {
        return record.BeginOrgServiceCall(requestJson);
    }

    /// <summary>
    /// Marks an OrgService call as completed successfully.
    /// </summary>
    public void NotifyOrgServiceCallCompleted(OrgServiceCallRecord call, string responseJson)
    {
        call.Complete(responseJson);
    }

    /// <summary>
    /// Marks an execution as completed successfully.
    /// </summary>
    public void NotifyExecutionCompleted(ExecutionRecord record, RemoteDebugExecutionContext outputContext)
    {
        record.Complete(outputContext);
        AddLog($"[grey]Execution #{record.Id} completed:[/] [green]{record.Duration?.TotalMilliseconds:F0}ms[/] ({record.OrgServiceCallCount} CRM calls)");
    }

    /// <summary>
    /// Marks an execution as failed.
    /// </summary>
    public void NotifyExecutionFailed(ExecutionRecord record, Exception error)
    {
        record.Fail(error);
        var shortError = error?.Message?.Split('\n')[0] ?? "Unknown error";
        if (shortError.Length > 60) shortError = shortError.Substring(0, 57) + "...";
        AddLog($"[grey]Execution #{record.Id}:[/] [red]FAILED {Markup.Escape(shortError)}[/]");
    }

    /// <summary>
    /// Adds a message to the log.
    /// </summary>
    public void AddLog(string markupMessage)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        lock (_lock)
        {
            _logs.Add($"[grey]{timestamp}[/]  {markupMessage}");
            if (_logs.Count > MaxLogs * 3)
                _logs.RemoveRange(0, _logs.Count - MaxLogs * 3);
        }
    }

    // ════════════════════════════════════════════════════════════════
    // Main interface loop
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Starts the console interface. Blocks until the user quits.
    /// </summary>
    public void Run()
    {
        _cts = new CancellationTokenSource();

        try
        {
            AnsiConsole.Cursor.Hide();

            // Start the render loop in a separate task
            var renderTask = Task.Run(RunRenderLoopAsync);

            // Read the keyboard on the main thread
            RunKeyboardLoop();

            // Wait for the render loop to finish
            try { renderTask.GetAwaiter().GetResult(); }
            catch (OperationCanceledException) { }
        }
        finally
        {
            AnsiConsole.Cursor.Show();
            AnsiConsole.Clear();
        }
    }

    private async Task RunRenderLoopAsync()
    {
        await AnsiConsole.Live(new Text("Initializing..."))
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Bottom)
            .StartAsync(async ctx =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        IRenderable view;
                        lock (_lock)
                        {
                            view = _currentView == View.Detail && TryGetSelected(out var sel)
                                ? BuildDetailView(sel)
                                : BuildMainView();
                        }
                        ctx.UpdateTarget(view);
                        ctx.Refresh();
                    }
                    catch (Exception ex)
                    {
                        AddLog($"[red]Render error: {Markup.Escape(ex.Message)}[/]");
                    }

                    try
                    {
                        await Task.Delay(150, _cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            });
    }

    private void RunKeyboardLoop()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);
                HandleKey(key);
            }
            Thread.Sleep(50);
        }
    }

    private void HandleKey(ConsoleKeyInfo key)
    {
        lock (_lock)
        {
            switch (_currentView)
            {
                case View.List:
                    HandleListKey(key);
                    break;
                case View.Detail:
                    HandleDetailKey(key);
                    break;
            }
        }
    }

    private void HandleListKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                if (_selectedIndex > 0) _selectedIndex--;
                break;

            case ConsoleKey.DownArrow:
                if (_selectedIndex < _executions.Count - 1) _selectedIndex++;
                break;

            case ConsoleKey.Enter:
                if (_executions.Count > 0)
                {
                    _traceScrollOffset = 0;
                    _currentView = View.Detail;
                }
                break;

            case ConsoleKey.R:
                if (TryGetSelected(out var toReplay) && toReplay.TestSession != null)
                    LaunchReplay(toReplay, debugMode: false);
                break;

            case ConsoleKey.D:
                if (TryGetSelected(out var toDebug) && toDebug.TestSession != null)
                    LaunchReplay(toDebug, debugMode: true);
                break;

            case ConsoleKey.S:
                if (TryGetSelected(out var toSave) && toSave.TestSession != null)
                    onSave?.Invoke(toSave);
                break;

            case ConsoleKey.Q:
            case ConsoleKey.Escape:
                _cts.Cancel();
                break;
        }
    }

    private void HandleDetailKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.Escape:
                _traceScrollOffset = 0;
                _currentView = View.List;
                break;

            case ConsoleKey.UpArrow:
                if (_traceScrollOffset > 0) _traceScrollOffset--;
                break;

            case ConsoleKey.DownArrow:
                if (TryGetSelected(out var recForScroll) && recForScroll.TraceLogs.Count > 0)
                {
                    var maxOffset = Math.Max(0, recForScroll.TraceLogs.Count - TracePageSize);
                    if (_traceScrollOffset < maxOffset) _traceScrollOffset++;
                }
                break;

            case ConsoleKey.R:
                if (TryGetSelected(out var toReplay) && toReplay.TestSession != null)
                    LaunchReplay(toReplay, debugMode: false);
                break;

            case ConsoleKey.D:
                if (TryGetSelected(out var toDebug) && toDebug.TestSession != null)
                    LaunchReplay(toDebug, debugMode: true);
                break;

            case ConsoleKey.S:
                if (TryGetSelected(out var toSave) && toSave.TestSession != null)
                    onSave?.Invoke(toSave);
                break;

            case ConsoleKey.Q:
                _cts.Cancel();
                break;
        }
    }

    private void LaunchReplay(ExecutionRecord record, bool debugMode)
    {
        if (onReplay != null)
        {
            Task.Run(() => onReplay(record, debugMode));
            AddLog($"[yellow]Replay #{record.Id} {(debugMode ? "in debug mode" : "")} launched...[/]");
        }
        else if (record.TestSession != null)
        {
            Task.Run(() =>
            {
                if (debugMode)
                {
                    AddLog($"[yellow]Attach the debugger to PID [bold]{Process.GetCurrentProcess().Id}[/] then press a key...[/]");
                    Debugger.Launch();
                }

                try
                {
                    var result = PluginTestRunner.Run(record.TestSession);
                    AddLog($"[green]OK Replay #{record.Id} completed ({result.OutputParameters?.Count ?? 0} OutputParams)[/]");
                }
                catch (Exception ex)
                {
                    AddLog($"[red]FAILED Replay #{record.Id}: {Markup.Escape(ex.Message)}[/]");
                }
            });
            AddLog($"[yellow]Replay #{record.Id} launched...[/]");
        }
    }

    private bool TryGetSelected(out ExecutionRecord record)
    {
        if (_executions.Count > 0 && _selectedIndex >= 0 && _selectedIndex < _executions.Count)
        {
            record = _executions[_selectedIndex];
            return true;
        }
        record = null;
        return false;
    }

    // ════════════════════════════════════════════════════════════════
    // Rendering — Main view (list)
    // ════════════════════════════════════════════════════════════════

    private IRenderable BuildMainView()
    {
        var rows = new List<IRenderable>();

        // ── Header ─────────────────────────────────────────────────
        rows.Add(new Panel(
                new Markup($"[bold deepskyblue1]{AppTitle}[/]  [grey]|[/]  PID: [white]{Process.GetCurrentProcess().Id}[/]  [grey]|[/]  {DateTime.Now:HH:mm:ss}"))
            .Border(BoxBorder.None)
            .Padding(0, 0));

        // ── Executions table ──────────────────────────────────────────
        rows.Add(BuildExecutionTable());

        // ── Log panel ─────────────────────────────────────────────────
        rows.Add(BuildLogPanel());

        // ── Shortcut bar ───────────────────────────────────────────────
        rows.Add(new Rule("[grey][[up/down]] Navigate   [[Enter]] Detail   [[R]] Replay   [[D]] Debug   [[S]] Save   [[Q]] Quit[/]")
            .Border(BoxBorder.None)
            .RuleStyle(Style.Parse("grey")));

        return new Rows(rows);
    }

    private IRenderable BuildExecutionTable()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey37)
            .AddColumn(new TableColumn("[grey]#[/]").RightAligned().Width(4))
            .AddColumn(new TableColumn("[white]Plugin[/]").Width(22))
            .AddColumn(new TableColumn("[white]Message[/]").Width(12))
            .AddColumn(new TableColumn("[white]Entity[/]").Width(22))
            .AddColumn(new TableColumn("[grey]Calls[/]").Centered().Width(7))
            .AddColumn(new TableColumn("[white]Status[/]").Width(14));

        if (_executions.Count == 0)
        {
            table.AddRow(
                new Markup("[grey]-[/]"),
                new Markup("[grey]Waiting for executions...[/]"),
                new Text(""), new Text(""), new Text(""), new Text(""));
            return table;
        }

        // Show the last 20 executions
        var startIdx = Math.Max(0, _executions.Count - 20);
        for (int i = startIdx; i < _executions.Count; i++)
        {
            var rec = _executions[i];
            var isSelected = i == _selectedIndex;

            var idStr = isSelected
                ? $"[bold yellow]> {rec.Id}[/]"
                : $"[grey]{rec.Id}[/]";

            var pluginStr = isSelected
                ? $"[bold white]{Markup.Escape(rec.PluginShortName)}[/]"
                : $"[white]{Markup.Escape(rec.PluginShortName)}[/]";

            var entityStr = FormatEntityColumn(rec);
            var callsStr = $"[grey]{rec.OrgServiceCallCount}[/]";
            var statusStr = FormatStatus(rec);

            table.AddRow(
                new Markup(idStr),
                new Markup(pluginStr),
                new Markup($"[cyan]{Markup.Escape(rec.MessageName)}[/]"),
                new Markup(entityStr),
                new Markup(callsStr),
                new Markup(statusStr));
        }

        return table;
    }

    private static string FormatEntityColumn(ExecutionRecord rec)
    {
        var entity = Markup.Escape(rec.PrimaryEntityName);
        if (rec.PrimaryEntityId != Guid.Empty)
        {
            var shortId = rec.PrimaryEntityId.ToString("D").Substring(0, 8);
            return $"[white]{entity}[/] [grey]({shortId}...)[/]";
        }
        return $"[white]{entity}[/]";
    }

    private static string FormatStatus(ExecutionRecord rec)
    {
        switch (rec.Status)
        {
            case ExecutionStatus.Running:
                return $"[yellow]{rec.ElapsedTime.TotalMilliseconds:F0}ms...[/]";
            case ExecutionStatus.Succeeded:
                return $"[green]OK {rec.Duration?.TotalMilliseconds:F0}ms[/]";
            case ExecutionStatus.Failed:
                return $"[red]FAILED {rec.Duration?.TotalMilliseconds:F0}ms[/]";
            default:
                return "";
        }
    }

    private IRenderable BuildLogPanel()
    {
        var recentLogs = _logs.Skip(Math.Max(0, _logs.Count - MaxLogs)).ToList();
        var logContent = recentLogs.Count > 0
            ? string.Join("\n", recentLogs)
            : "[grey](no log)[/]";

        return new Panel(new Markup(logContent))
            .Header("[grey] Logs [/]")
            .Border(BoxBorder.Ascii)
            .BorderColor(Color.Grey23)
            .Padding(1, 0);
    }

    // ════════════════════════════════════════════════════════════════
    // Rendering — Detail view (zoom in)
    // ════════════════════════════════════════════════════════════════

    private IRenderable BuildDetailView(ExecutionRecord rec)
    {
        var rows = new List<IRenderable>();

        // ── Header ─────────────────────────────────────────────────
        var headerStatus = FormatStatus(rec);
        rows.Add(new Panel(
                new Markup(
                    $"[bold]#{rec.Id}[/]  [deepskyblue1]{Markup.Escape(rec.PluginShortName)}[/]  " +
                    $"[grey]·[/]  [cyan]{Markup.Escape(rec.MessageName)}[/]  " +
                    $"[grey]·[/]  [white]{Markup.Escape(rec.PrimaryEntityName)}[/]" +
                    $"    {headerStatus}"))
            .Border(BoxBorder.None)
            .Padding(0, 0));

        // ── Input context ─────────────────────────────────────────────
        rows.Add(BuildInputContextPanel(rec));

        // ── OrgService calls ───────────────────────────────────────────
        rows.Add(BuildOrgCallsPanel(rec));

        // ── Plugin traces ──────────────────────────────────────────────
        if (rec.TraceLogs.Count > 0)
            rows.Add(BuildTraceLogsPanel(rec));

        // ── Output context or error ───────────────────────────────────
        if (rec.Status == ExecutionStatus.Succeeded)
            rows.Add(BuildOutputContextPanel(rec));
        else if (rec.Status == ExecutionStatus.Failed)
            rows.Add(BuildErrorPanel(rec));
        else
            rows.Add(new Panel(new Markup("[yellow]Execution in progress...[/]"))
                .Header("[yellow] In progress [/]").Padding(1, 0));

        // ── Shortcut bar ───────────────────────────────────────────────
        rows.Add(new Rule("[grey][[ESC]] Back   [[up/down]] Traces   [[R]] Replay   [[D]] Debug   [[S]] Save   [[Q]] Quit[/]")
            .Border(BoxBorder.None)
            .RuleStyle(Style.Parse("grey")));

        return new Rows(rows);
    }

    private IRenderable BuildInputContextPanel(ExecutionRecord rec)
    {
        var ctx = rec.InputContext;
        var sb = new StringBuilder();

        AppendField(sb, "Stage", FormatStage(ctx.Stage, ctx.IsWorkflowContext));
        AppendField(sb, "UserId", ctx.UserId.ToString("D").Substring(0, 8) + "...");
        AppendField(sb, "Entity", $"{ctx.PrimaryEntityName} ({ctx.PrimaryEntityId:D})");
        AppendField(sb, "Depth", ctx.Depth.ToString());

        if (ctx.InputParameters?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]InputParameters[/]");
            foreach (var param in ctx.InputParameters)
            {
                var value = FormatParameterValue(param.Value);
                sb.AppendLine($"    [grey].[/] [cyan]{Markup.Escape(param.Key)}[/] = {value}");
            }
        }

        if (ctx.PreEntityImages?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]PreEntityImages[/]");
            foreach (var img in ctx.PreEntityImages)
                sb.AppendLine($"    [grey].[/] [cyan]{Markup.Escape(img.Key)}[/] ({img.Value?.Attributes?.Count ?? 0} attributes)");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header("[deepskyblue1] Input Context [/]")
            .BorderColor(Color.DeepSkyBlue1)
            .Padding(1, 0);
    }

    private IRenderable BuildOrgCallsPanel(ExecutionRecord rec)
    {
        var calls = rec.OrgServiceCalls;

        if (calls.Count == 0)
        {
            return new Panel(new Markup("[grey](no OrgService call)[/]"))
                .Header("[blue] OrgService Calls (0) [/]")
                .BorderColor(Color.Blue)
                .Padding(1, 0);
        }

        var sb = new StringBuilder();
        foreach (var call in calls)
        {
            string statusIcon;
            string durationStr = "";

            if (call.IsRunning)
            {
                statusIcon = "[yellow]...[/]";
            }
            else if (call.Success == true)
            {
                statusIcon = "[green]OK[/]";
                durationStr = $"  [grey]{call.Duration?.TotalMilliseconds:F0}ms[/]";
            }
            else
            {
                statusIcon = "[red]ERR[/]";
                durationStr = call.ErrorMessage != null
                    ? $"  [red]{Markup.Escape(TruncateStr(call.ErrorMessage, 50))}[/]"
                    : "";
            }

            sb.AppendLine(
                $"  {statusIcon}  [grey]{call.Index}.[/]  " +
                $"[cyan]{Markup.Escape(call.RequestType)}[/]  " +
                $"[white]{Markup.Escape(call.EntityLogicalName)}[/]" +
                (call.EntityId != Guid.Empty ? $" [grey]({call.EntityId.ToString("D").Substring(0, 8)}...)[/]" : "") +
                durationStr);
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header($"[blue] OrgService Calls ({calls.Count}) [/]")
            .BorderColor(Color.Blue)
            .Padding(1, 0);
    }

    private IRenderable BuildTraceLogsPanel(ExecutionRecord rec)
    {
        var logs  = rec.TraceLogs;
        var total = logs.Count;

        // Clamp the offset in case the logs changed during rendering
        var offset = Math.Min(_traceScrollOffset, Math.Max(0, total - TracePageSize));
        var end    = Math.Min(offset + TracePageSize, total);

        var sb = new StringBuilder();
        for (int i = offset; i < end; i++)
            sb.AppendLine($"  [grey]{Markup.Escape(logs[i])}[/]");

        // Position indicator + scroll hint
        var scrollHint = total > TracePageSize
            ? $"  [grey]line {offset + 1}-{end} / {total}   [[up/down]] to scroll[/]"
            : $"  [grey]{total} line(s)[/]";
        sb.Append(scrollHint);

        var header = total > TracePageSize
            ? $"[yellow] Traces ({total})  {offset + 1}-{end} [/]"
            : $"[yellow] Traces ({total}) [/]";

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header(header)
            .BorderColor(Color.Yellow)
            .Padding(1, 0);
    }

    private IRenderable BuildOutputContextPanel(ExecutionRecord rec)
    {
        var ctx = rec.OutputContext;
        var sb = new StringBuilder();

        if (ctx?.OutputParameters?.Count > 0)
        {
            sb.AppendLine("  [underline]OutputParameters[/]");
            foreach (var param in ctx.OutputParameters)
            {
                var value = FormatParameterValue(param.Value);
                sb.AppendLine($"    [grey].[/] [cyan]{Markup.Escape(param.Key)}[/] = {value}");
            }
        }
        else
        {
            sb.AppendLine("  [grey]OutputParameters: (none)[/]");
        }

        if (ctx?.SharedVariables?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]SharedVariables[/]");
            foreach (var v in ctx.SharedVariables)
                sb.AppendLine($"    [grey].[/] [cyan]{Markup.Escape(v.Key)}[/] = {FormatParameterValue(v.Value)}");
        }

        if (ctx?.PostEntityImages?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]PostEntityImages[/]");
            foreach (var img in ctx.PostEntityImages)
                sb.AppendLine($"    [grey].[/] [cyan]{Markup.Escape(img.Key)}[/] ({img.Value?.Attributes?.Count ?? 0} attributes)");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header("[green] Output Context [/]")
            .BorderColor(Color.Green)
            .Padding(1, 0);
    }

    private IRenderable BuildErrorPanel(ExecutionRecord rec)
    {
        var ex = rec.Error;
        if (ex == null)
            return new Panel(new Markup("[red](unknown error)[/]"))
                .Header("[red] Error [/]").Padding(1, 0);

        var sb = new StringBuilder();
        sb.AppendLine($"  [bold red]{Markup.Escape(ex.GetType().Name)}[/]");
        sb.AppendLine($"  [white]{Markup.Escape(TruncateStr(ex.Message, 200))}[/]");

        if (ex.StackTrace != null)
        {
            sb.AppendLine();
            sb.AppendLine("  [grey]Stack Trace:[/]");
            foreach (var line in ex.StackTrace.Split('\n').Take(8))
                sb.AppendLine($"  [grey]{Markup.Escape(line.TrimEnd())}[/]");
        }

        if (ex.InnerException != null)
        {
            sb.AppendLine();
            sb.AppendLine($"  [grey]Caused by:[/] [red]{Markup.Escape(ex.InnerException.GetType().Name)}[/]");
            sb.AppendLine($"  [grey]{Markup.Escape(TruncateStr(ex.InnerException.Message, 100))}[/]");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header("[red] Error [/]")
            .BorderColor(Color.Red)
            .Padding(1, 0);
    }

    // ════════════════════════════════════════════════════════════════
    // Formatting helpers
    // ════════════════════════════════════════════════════════════════

    private static void AppendField(StringBuilder sb, string label, string value)
    {
        sb.AppendLine($"  [grey]{label,-14}[/] [white]{Markup.Escape(value)}[/]");
    }

    private static string FormatStage(int stage, bool isWorkflow)
    {
        if (isWorkflow) return $"{stage} (Workflow)";
        switch (stage)
        {
            case 10: return "10 (PreValidation)";
            case 20: return "20 (PreOperation)";
            case 40: return "40 (PostOperation)";
            default: return stage.ToString();
        }
    }

    private static string FormatParameterValue(object value)
    {
        if (value == null) return "[grey]null[/]";

        var str = value.ToString();
        if (str.Length > 80) str = str.Substring(0, 77) + "...";

        return $"[white]{Markup.Escape(str)}[/]";
    }

    private static string TruncateStr(string s, int maxLen)
    {
        if (s == null) return "";
        return s.Length <= maxLen ? s : s.Substring(0, maxLen - 3) + "...";
    }
}
