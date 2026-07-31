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
using Spectre.Console.Rendering; // IRenderable
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client.ConsoleUI;

/// <summary>
/// Console interface (TUI) for browsing test sessions saved on disk.
/// <para>
/// Three-level navigation:
/// <list type="bullet">
///   <item><b>Level 1 — Correlations</b>: groups of sessions sharing the same CorrelationId.</item>
///   <item><b>Level 2 — Sessions</b>    : list of files in a selected correlation.</item>
///   <item><b>Level 3 — Detail</b>      : full context of a session (input / OrgService / output).</item>
/// </list>
/// </para>
/// Keyboard shortcuts:
/// <list type="bullet">
///   <item>[Up/Down] Navigate the current list</item>
///   <item>[Enter]   Go down one level</item>
///   <item>[Esc]     Go up one level</item>
///   <item>[R]       Replay the session without the debugger</item>
///   <item>[D]       Replay in debug mode (attach the debugger)</item>
///   <item>[F5]      Reload the files from disk</item>
///   <item>[Q]       Quit</item>
/// </list>
/// </summary>
public class SessionBrowserUi
{
    // ── Views ─────────────────────────────────────────────────────────────
    private enum View { Correlations, Sessions, Detail }

    private View _currentView = View.Correlations;

    // ── Data ──────────────────────────────────────────────────────────────
    private List<CorrelationGroup> _groups = new();
    private int _groupIndex;
    private int _sessionIndex;

    private readonly string _sessionPath;
    private readonly Action<PluginTestSession, bool> _onReplay;

    // ── Lock & log ──────────────────────────────────────────────────────────
    private readonly object _lock = new();
    private readonly List<string> _logs = new();
    private const int MaxLogs = 6;

    // ── Lifecycle ─────────────────────────────────────────────────────────
    private CancellationTokenSource _cts;

    private const string AppTitle = "XrmFramework Session Browser";

    // ════════════════════════════════════════════════════════════════════
    // Constructor
    // ════════════════════════════════════════════════════════════════════

    /// <param name="sessionPath">
    ///   Directory containing the <c>*.pluginsession.json</c> files.
    /// </param>
    /// <param name="onReplay">
    ///   Callback invoked when the user starts a replay.
    ///   If <c>null</c>, the replay is performed locally via <see cref="PluginTestRunner"/>.
    /// </param>
    public SessionBrowserUi(string sessionPath, Action<PluginTestSession, bool> onReplay = null)
    {
        _sessionPath = sessionPath ?? ".";
        _onReplay    = onReplay;
    }

    // ════════════════════════════════════════════════════════════════════
    // Main entry point
    // ════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Starts the console interface. Blocks until the user quits with [Q].
    /// </summary>
    public void Run()
    {
        _cts = new CancellationTokenSource();

        // Load the sessions before starting the interface
        Reload();

        // Hide the cursor before the interface starts.
        AnsiConsole.Cursor.Hide();

        // Start the render loop on a separate thread.
        // On a fatal render failure, cancel the token to exit cleanly.
        var renderTask = Task.Run(async () =>
        {
            try
            {
                await RunRenderLoopAsync();
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation — not an error.
            }
            catch (Exception ex)
            {
                // Critical failure: log it and unblock the keyboard loop
                AddLog($"[red bold]Critical render error: {Markup.Escape(ex.GetType().Name)}[/]");
                AddLog($"[red]{Markup.Escape(ex.Message.Split('\n')[0])}[/]");
                _cts.Cancel();
            }
        });

        try
        {
            // Read the keyboard on the main thread (blocks until [Q])
            RunKeyboardLoop();
        }
        finally
        {
            // Wait for the render loop to finish cleanly
            _cts.Cancel();
            try { renderTask.GetAwaiter().GetResult(); }
            catch { /* already handled in the task */ }

            AnsiConsole.Cursor.Show();
            AnsiConsole.Clear();
        }
    }

    // ── Loading ───────────────────────────────────────────────────────────

    private void Reload()
    {
        List<CorrelationGroup> loaded;
        try
        {
            loaded = SessionLoader.LoadCorrelationGroups(_sessionPath);
        }
        catch (Exception ex)
        {
            loaded = new List<CorrelationGroup>();
            AddLog($"[red]Loading error: {Markup.Escape(ex.Message)}[/]");
        }

        lock (_lock)
        {
            _groups     = loaded;
            _groupIndex = Math.Min(_groupIndex, Math.Max(0, _groups.Count - 1));
        }

        var total = loaded.Sum(g => g.SessionCount);
        AddLog(
            $"[grey]Directory:[/] [cyan]{Markup.Escape(_sessionPath)}[/]  " +
            $"[grey]|[/] [white]{total}[/] [grey]session(s) / [/][white]{loaded.Count}[/] [grey]correlation(s)[/]");
    }

    // ════════════════════════════════════════════════════════════════════
    // Render loop (separate thread)
    // ════════════════════════════════════════════════════════════════════

    private async Task RunRenderLoopAsync()
    {
        await AnsiConsole.Live(new Text("Initializing…"))
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
                            view = _currentView switch
                            {
                                View.Sessions when TryGetSelectedGroup(out var g) => BuildSessionsView(g),
                                View.Detail when TryGetSelectedSession(out var s) => BuildDetailView(s),
                                _ => BuildCorrelationsView()
                            };
                        }
                        ctx.UpdateTarget(view);
                        await Task.Delay(200, _cts.Token);
                    }
                    catch (OperationCanceledException) { break; }
                    catch { /* never let the render crash */ }
                }
            });
    }

    // ════════════════════════════════════════════════════════════════════
    // Keyboard loop (main thread)
    // ════════════════════════════════════════════════════════════════════

    private void RunKeyboardLoop()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);
                lock (_lock) { HandleKey(key); }
            }
            Thread.Sleep(50);
        }
    }

    private void HandleKey(ConsoleKeyInfo key)
    {
        switch (_currentView)
        {
            case View.Correlations: HandleCorrelationsKey(key); break;
            case View.Sessions:     HandleSessionsKey(key);     break;
            case View.Detail:       HandleDetailKey(key);       break;
        }
    }

    private void HandleCorrelationsKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                if (_groupIndex > 0) _groupIndex--;
                break;

            case ConsoleKey.DownArrow:
                if (_groupIndex < _groups.Count - 1) _groupIndex++;
                break;

            case ConsoleKey.Enter:
                if (_groups.Count > 0)
                {
                    _sessionIndex = 0;
                    _currentView  = View.Sessions;
                }
                break;

            case ConsoleKey.F5:
                Task.Run(() => Reload());
                break;

            case ConsoleKey.Q:
            case ConsoleKey.Escape:
                _cts.Cancel();
                break;
        }
    }

    private void HandleSessionsKey(ConsoleKeyInfo key)
    {
        if (!TryGetSelectedGroup(out var group)) return;

        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                if (_sessionIndex > 0) _sessionIndex--;
                break;

            case ConsoleKey.DownArrow:
                if (_sessionIndex < group.Sessions.Count - 1) _sessionIndex++;
                break;

            case ConsoleKey.Enter:
                if (group.Sessions.Count > 0)
                    _currentView = View.Detail;
                break;

            case ConsoleKey.Escape:
                _currentView = View.Correlations;
                break;

            case ConsoleKey.R:
                if (TryGetSelectedSession(out var toReplay))
                    LaunchReplay(toReplay, debugMode: false);
                break;

            case ConsoleKey.D:
                if (TryGetSelectedSession(out var toDebug))
                    LaunchReplay(toDebug, debugMode: true);
                break;

            case ConsoleKey.Q:
                _cts.Cancel();
                break;
        }
    }

    private void HandleDetailKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.Escape:
                _currentView = View.Sessions;
                break;

            case ConsoleKey.R:
                if (TryGetSelectedSession(out var toReplay))
                    LaunchReplay(toReplay, debugMode: false);
                break;

            case ConsoleKey.D:
                if (TryGetSelectedSession(out var toDebug))
                    LaunchReplay(toDebug, debugMode: true);
                break;

            case ConsoleKey.Q:
                _cts.Cancel();
                break;
        }
    }

    // ── Replay ────────────────────────────────────────────────────────────

    private void LaunchReplay(PluginTestSession session, bool debugMode)
    {
        var pluginName = session.InputContext != null
            ? ExtractShortTypeName(session.InputContext.TypeAssemblyQualifiedName)
            : "plugin";

        AddLog($"[yellow]Replaying {Markup.Escape(pluginName)}{(debugMode ? " (debug)" : "")} launched...[/]");

        if (_onReplay != null)
        {
            Task.Run(() => _onReplay(session, debugMode));
            return;
        }

        Task.Run(() =>
        {
            if (debugMode)
            {
                AddLog($"[yellow]Attach the debugger to PID {Process.GetCurrentProcess().Id}...[/]");
                Debugger.Launch();
            }

            try
            {
                var result = PluginTestRunner.Run(session);
                AddLog(
                    $"[green]Replay completed[/] [grey]--[/] " +
                    $"[white]{result.OutputParameters?.Count ?? 0}[/] [grey]OutputParam(s)[/]");
            }
            catch (Exception ex)
            {
                AddLog($"[red]Replay failed: {Markup.Escape(ex.Message)}[/]");
            }
        });
    }

    // ════════════════════════════════════════════════════════════════════
    // Current selection
    // ════════════════════════════════════════════════════════════════════

    private bool TryGetSelectedGroup(out CorrelationGroup group)
    {
        if (_groups.Count > 0 && _groupIndex >= 0 && _groupIndex < _groups.Count)
        {
            group = _groups[_groupIndex];
            return true;
        }
        group = null;
        return false;
    }

    private bool TryGetSelectedSession(out PluginTestSession session)
    {
        if (!TryGetSelectedGroup(out var group) ||
            _sessionIndex < 0 || _sessionIndex >= group.Sessions.Count)
        {
            session = null;
            return false;
        }
        session = group.Sessions[_sessionIndex];
        return true;
    }

    // ════════════════════════════════════════════════════════════════════
    // View 1 — Correlation list
    // ════════════════════════════════════════════════════════════════════

    private IRenderable BuildCorrelationsView()
    {
        var rows = new List<IRenderable>();

        rows.Add(new Panel(
                new Markup(
                    $"[bold deepskyblue1]{AppTitle}[/]  " +
                    $"[grey]|[/]  [cyan]{Markup.Escape(_sessionPath)}[/]  " +
                    $"[grey]|[/]  [white]{DateTime.Now:HH:mm:ss}[/]"))
            .Border(BoxBorder.None)
            .Padding(0, 0));

        rows.Add(BuildCorrelationTable());
        rows.Add(BuildLogPanel());
        rows.Add(new Rule("[grey][[Up/Down]] Nav   [[Enter]] Open   [[F5]] Reload   [[Q]] Quit[/]")
            .Border(BoxBorder.None)
            .RuleStyle(Style.Parse("grey")));

        return new Rows(rows);
    }

    private IRenderable BuildCorrelationTable()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey37)
            .AddColumn(new TableColumn("[grey]#[/]").RightAligned().Width(4))
            .AddColumn(new TableColumn("[white]Correlation[/]").Width(28))
            .AddColumn(new TableColumn("[grey]Cnt[/]").Centered().Width(4))
            .AddColumn(new TableColumn("[white]First[/]").Width(17))
            .AddColumn(new TableColumn("[white]Last[/]").Width(17))
            .AddColumn(new TableColumn("[grey]CorrelId[/]").Width(10));

        if (_groups.Count == 0)
        {
            table.AddRow(
                new Markup("[grey]-[/]"),
                new Markup("[grey]No sessions found. Press [[F5]] to reload.[/]"),
                new Text(""), new Text(""), new Text(""), new Text(""));
            return table;
        }

        var startIdx = Math.Max(0, _groups.Count - 20);
        for (var i = startIdx; i < _groups.Count; i++)
        {
            var g          = _groups[i];
            var isSelected = i == _groupIndex;

            var idStr   = isSelected ? $"[bold yellow]> {g.Id}[/]" : $"[grey]{g.Id}[/]";
            var nameStr = isSelected
                ? $"[bold white]{Markup.Escape(g.Name)}[/]"
                : $"[white]{Markup.Escape(g.Name)}[/]";

            var lastStr = g.SessionCount > 1
                ? $"[grey]{g.LastOccurrence:dd/MM HH:mm:ss}[/]"
                : "[grey]-[/]";

            table.AddRow(
                new Markup(idStr),
                new Markup(nameStr),
                new Markup($"[grey]{g.SessionCount}[/]"),
                new Markup($"[grey]{g.FirstOccurrence:dd/MM HH:mm:ss}[/]"),
                new Markup(lastStr),
                new Markup($"[grey]{g.CorrelationId.ToString("N").Substring(0, 8)}[/]"));
        }

        return table;
    }

    // ════════════════════════════════════════════════════════════════════
    // View 2 — Sessions within a correlation
    // ════════════════════════════════════════════════════════════════════

    private IRenderable BuildSessionsView(CorrelationGroup group)
    {
        var rows = new List<IRenderable>();

        rows.Add(new Panel(
                new Markup(
                    $"[bold deepskyblue1]{AppTitle}[/]  [grey]>[/]  " +
                    $"[bold white]{Markup.Escape(group.Name)}[/]  " +
                    $"[grey]({group.SessionCount} session(s))[/]  " +
                    $"[grey]|[/]  [white]{DateTime.Now:HH:mm:ss}[/]"))
            .Border(BoxBorder.None)
            .Padding(0, 0));

        rows.Add(BuildSessionTable(group));
        rows.Add(BuildLogPanel());
        rows.Add(new Rule("[grey][[Esc]] Back   [[Up/Down]] Nav   [[Enter]] Detail   [[R]] Replay   [[D]] Debug   [[Q]] Quit[/]")
            .Border(BoxBorder.None)
            .RuleStyle(Style.Parse("grey")));

        return new Rows(rows);
    }

    private IRenderable BuildSessionTable(CorrelationGroup group)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey37)
            .AddColumn(new TableColumn("[grey]#[/]").RightAligned().Width(4))
            .AddColumn(new TableColumn("[white]Plugin[/]").Width(20))
            .AddColumn(new TableColumn("[white]Message[/]").Width(12))
            .AddColumn(new TableColumn("[white]Entity[/]").Width(18))
            .AddColumn(new TableColumn("[grey]CRM[/]").Centered().Width(5))
            .AddColumn(new TableColumn("[white]Timestamp[/]").Width(17));

        for (var i = 0; i < group.Sessions.Count; i++)
        {
            var s          = group.Sessions[i];
            var isSelected = i == _sessionIndex;
            var ctx        = s.InputContext;

            var pluginShort = ctx != null ? ExtractShortTypeName(ctx.TypeAssemblyQualifiedName) : "?";
            var idStr       = isSelected ? $"[bold yellow]> {i + 1}[/]" : $"[grey]{i + 1}[/]";
            var pluginStr   = isSelected
                ? $"[bold white]{Markup.Escape(pluginShort)}[/]"
                : $"[white]{Markup.Escape(pluginShort)}[/]";

            table.AddRow(
                new Markup(idStr),
                new Markup(pluginStr),
                new Markup($"[cyan]{Markup.Escape(ctx?.MessageName ?? "")}[/]"),
                new Markup(FormatEntityColumn(ctx)),
                new Markup($"[grey]{s.OrgServiceCalls?.Count ?? 0}[/]"),
                new Markup($"[grey]{s.Timestamp:dd/MM HH:mm:ss}[/]"));
        }

        return table;
    }

    // ════════════════════════════════════════════════════════════════════
    // View 3 — Session detail
    // ════════════════════════════════════════════════════════════════════

    private IRenderable BuildDetailView(PluginTestSession session)
    {
        var ctx         = session.InputContext;
        var pluginShort = ctx != null ? ExtractShortTypeName(ctx.TypeAssemblyQualifiedName) : "?";

        var rows = new List<IRenderable>();

        rows.Add(new Panel(
                new Markup(
                    $"[bold deepskyblue1]{AppTitle}[/]  [grey]>[/]  " +
                    $"[deepskyblue1]{Markup.Escape(pluginShort)}[/]  " +
                    $"[grey].[/]  [cyan]{Markup.Escape(ctx?.MessageName ?? "")}[/]  " +
                    $"[grey].[/]  [white]{Markup.Escape(ctx?.PrimaryEntityName ?? "")}[/]  " +
                    $"[grey]|[/]  [white]{session.Timestamp:dd/MM HH:mm:ss}[/]"))
            .Border(BoxBorder.None)
            .Padding(0, 0));

        if (ctx != null)
            rows.Add(BuildInputContextPanel(ctx));

        rows.Add(BuildOrgCallsPanel(session));

        if (session.OutputContext != null)
            rows.Add(BuildOutputContextPanel(session.OutputContext));

        rows.Add(new Rule("[grey][[Esc]] Back   [[R]] Replay   [[D]] Debug   [[Q]] Quit[/]")
            .Border(BoxBorder.None)
            .RuleStyle(Style.Parse("grey")));

        return new Rows(rows);
    }

    private IRenderable BuildInputContextPanel(RemoteDebugExecutionContext ctx)
    {
        var sb = new StringBuilder();

        AppendField(sb, "Stage",         FormatStage(ctx.Stage, ctx.IsWorkflowContext));
        AppendField(sb, "UserId",        ctx.UserId.ToString("N").Substring(0, 8) + "...");
        AppendField(sb, "Entity",        $"{ctx.PrimaryEntityName} ({ctx.PrimaryEntityId:D})");
        AppendField(sb, "Depth",         ctx.Depth.ToString());
        AppendField(sb, "CorrelationId", ctx.CorrelationId.ToString("D"));

        if (ctx.InputParameters?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]InputParameters[/]");
            foreach (var p in ctx.InputParameters)
                sb.AppendLine($"    [grey]- [/][cyan]{Markup.Escape(p.Key)}[/] = {FormatParamValue(p.Value)}");
        }

        if (ctx.PreEntityImages?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]PreEntityImages[/]");
            foreach (var img in ctx.PreEntityImages)
                sb.AppendLine($"    [grey]- [/][cyan]{Markup.Escape(img.Key)}[/] ({img.Value?.Attributes?.Count ?? 0} attr)");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header("[deepskyblue1] Input Context [/]")
            .BorderColor(Color.DeepSkyBlue1)
            .Padding(1, 0);
    }

    private IRenderable BuildOrgCallsPanel(PluginTestSession session)
    {
        var calls = session.OrgServiceCalls;

        if (calls == null || calls.Count == 0)
        {
            return new Panel(new Markup("[grey](no OrgService call recorded)[/]"))
                .Header("[blue] OrgService Calls (0) [/]")
                .BorderColor(Color.Blue)
                .Padding(1, 0);
        }

        var sb = new StringBuilder();
        for (var i = 0; i < calls.Count; i++)
        {
            var rec = new OrgServiceCallRecord(calls[i].RequestJson ?? "");
            var entity = string.IsNullOrEmpty(rec.EntityLogicalName) ? "" : $" {Markup.Escape(rec.EntityLogicalName)}";
            var id     = rec.EntityId != Guid.Empty ? $" ({rec.EntityId.ToString("N").Substring(0, 8)}...)" : "";

            sb.AppendLine($"  [green]OK[/]  [grey]{i + 1}.[/]  [cyan]{Markup.Escape(rec.RequestType)}[/]{entity}[grey]{id}[/]");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header($"[blue] OrgService Calls ({calls.Count}) [/]")
            .BorderColor(Color.Blue)
            .Padding(1, 0);
    }

    private IRenderable BuildOutputContextPanel(RemoteDebugExecutionContext ctx)
    {
        var sb = new StringBuilder();

        if (ctx.OutputParameters?.Count > 0)
        {
            sb.AppendLine("  [underline]OutputParameters[/]");
            foreach (var p in ctx.OutputParameters)
                sb.AppendLine($"    [grey]- [/][cyan]{Markup.Escape(p.Key)}[/] = {FormatParamValue(p.Value)}");
        }
        else
        {
            sb.AppendLine("  [grey]OutputParameters: (none)[/]");
        }

        if (ctx.SharedVariables?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]SharedVariables[/]");
            foreach (var v in ctx.SharedVariables)
                sb.AppendLine($"    [grey]- [/][cyan]{Markup.Escape(v.Key)}[/] = {FormatParamValue(v.Value)}");
        }

        if (ctx.PostEntityImages?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]PostEntityImages[/]");
            foreach (var img in ctx.PostEntityImages)
                sb.AppendLine($"    [grey]- [/][cyan]{Markup.Escape(img.Key)}[/] ({img.Value?.Attributes?.Count ?? 0} attr)");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header("[green] Output Context [/]")
            .BorderColor(Color.Green)
            .Padding(1, 0);
    }

    // ════════════════════════════════════════════════════════════════════
    // Formatting helpers
    // ════════════════════════════════════════════════════════════════════

    private static string FormatEntityColumn(RemoteDebugExecutionContext ctx)
    {
        if (ctx == null) return "";
        var name = Markup.Escape(ctx.PrimaryEntityName ?? "");
        return ctx.PrimaryEntityId != Guid.Empty
            ? $"[white]{name}[/] [grey]({ctx.PrimaryEntityId.ToString("N").Substring(0, 8)}...)[/]"
            : $"[white]{name}[/]";
    }

    private static string FormatStage(int stage, bool isWorkflow)
    {
        if (isWorkflow) return $"{stage} (Workflow)";
        return stage switch
        {
            10 => "10 (PreValidation)",
            20 => "20 (PreOperation)",
            40 => "40 (PostOperation)",
            _  => stage.ToString()
        };
    }

    private static string FormatParamValue(object value)
    {
        if (value == null) return "[grey]null[/]";
        var str = value.ToString() ?? "";
        if (str.Length > 80) str = str.Substring(0, 77) + "...";
        return $"[white]{Markup.Escape(str)}[/]";
    }

    private static void AppendField(StringBuilder sb, string label, string value)
        => sb.AppendLine($"  [grey]{label,-14}[/] [white]{Markup.Escape(value)}[/]");

    private static string ExtractShortTypeName(string assemblyQualifiedName)
    {
        if (string.IsNullOrEmpty(assemblyQualifiedName)) return "UnknownPlugin";
        var typePart = assemblyQualifiedName.Split(new[] { ',' }, 2)[0].Trim();
        var lastDot  = typePart.LastIndexOf('.');
        return lastDot >= 0 ? typePart.Substring(lastDot + 1) : typePart;
    }

    // ════════════════════════════════════════════════════════════════════
    // Log
    // ════════════════════════════════════════════════════════════════════

    /// <summary>Adds a timestamped message to the internal log.</summary>
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

    private IRenderable BuildLogPanel()
    {
        var recent  = _logs.Skip(Math.Max(0, _logs.Count - MaxLogs)).ToList();
        var content = recent.Count > 0 ? string.Join("\n", recent) : "[grey](no log)[/]";

        return new Panel(new Markup(content))
            .Header("[grey] Logs [/]")
            .Border(BoxBorder.Ascii)
            .BorderColor(Color.Grey23)
            .Padding(1, 0);
    }
}
