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
/// Interface console moderne pour le débogueur distant XrmFramework.
/// Affiche en temps réel les exécutions de plugins, permet de zoomer
/// dans chaque exécution pour analyser les appels OrgService,
/// et propose de rejouer une exécution en mode debug.
/// </summary>
public class DebuggerConsoleUi(
    Action<ExecutionRecord> onSave = null,
    Action<ExecutionRecord, bool> onReplay = null)
{
    // ── État de la vue ───────────────────────────────────────────────
    private enum View { List, Detail }

    private View _currentView = View.List;
    private readonly List<ExecutionRecord> _executions = new();
    private int _selectedIndex;
    private readonly object _lock = new();

    // ── Journal de messages ──────────────────────────────────────────
    private readonly List<string> _logs = new();
    private const int MaxLogs = 6;

    // ── Interception de Console.Out ──────────────────────────────────
    private System.IO.TextWriter _originalConsoleOut;

    // ── Contrôle du cycle de vie ─────────────────────────────────────
    private CancellationTokenSource _cts;

    // ── Action de sauvegarde (fournie par RemoteDebugger<T>) ─────────
    // bool = debugMode

    // ── Titre de l'application ───────────────────────────────────────
    private const string AppTitle = "XrmFramework Remote Debugger";

    // ════════════════════════════════════════════════════════════════
    // API publique — appelée depuis RemoteDebugger<T>
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Enregistre le début d'une nouvelle exécution de plugin.
    /// </summary>
    public ExecutionRecord NotifyExecutionStarted(RemoteDebugExecutionContext context)
    {
        var record = new ExecutionRecord(context);
        lock (_lock)
        {
            _executions.Add(record);
            // Sélectionner automatiquement la dernière exécution
            _selectedIndex = _executions.Count - 1;
        }
        AddLog($"[grey]Exécution #{record.Id} démarrée :[/] [cyan]{record.PluginShortName}[/] · {record.MessageName} · {record.PrimaryEntityName}");
        return record;
    }

    /// <summary>
    /// Enregistre un appel OrgService sur l'exécution en cours.
    /// </summary>
    public OrgServiceCallRecord NotifyOrgServiceCallStarted(ExecutionRecord record, string requestJson)
    {
        return record.BeginOrgServiceCall(requestJson);
    }

    /// <summary>
    /// Marque un appel OrgService comme terminé avec succès.
    /// </summary>
    public void NotifyOrgServiceCallCompleted(OrgServiceCallRecord call, string responseJson)
    {
        call.Complete(responseJson);
    }

    /// <summary>
    /// Marque une exécution comme terminée avec succès.
    /// </summary>
    public void NotifyExecutionCompleted(ExecutionRecord record, RemoteDebugExecutionContext outputContext)
    {
        record.Complete(outputContext);
        AddLog($"[grey]Exécution #{record.Id} terminée :[/] [green]✅ {record.Duration?.TotalMilliseconds:F0}ms[/] ({record.OrgServiceCallCount} appels CRM)");
    }

    /// <summary>
    /// Marque une exécution comme échouée.
    /// </summary>
    public void NotifyExecutionFailed(ExecutionRecord record, Exception error)
    {
        record.Fail(error);
        var shortError = error?.Message?.Split('\n')[0] ?? "Erreur inconnue";
        if (shortError.Length > 60) shortError = shortError.Substring(0, 57) + "...";
        AddLog($"[grey]Exécution #{record.Id} :[/] [red]❌ {Markup.Escape(shortError)}[/]");
    }

    /// <summary>
    /// Ajoute un message au journal.
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
    // Boucle principale de l'interface
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Lance l'interface console. Bloque jusqu'à ce que l'utilisateur quitte.
    /// </summary>
    public void Run()
    {
        _cts = new CancellationTokenSource();

        // Intercepter Console.Out pour capturer les logs de l'infrastructure
        _originalConsoleOut = Console.Out;
        Console.SetOut(new LogCaptureWriter(this, _originalConsoleOut));

        try
        {
            AnsiConsole.Cursor.Hide();
            AnsiConsole.Clear();

            // Lancer la boucle de rendu dans une tâche séparée
            var renderTask = Task.Run(async () => await RunRenderLoopAsync(), _cts.Token);

            // Lire le clavier dans le thread principal
            RunKeyboardLoop();

            // Attendre la fin du rendu
            try { renderTask.GetAwaiter().GetResult(); }
            catch (OperationCanceledException) { }
        }
        finally
        {
            Console.SetOut(_originalConsoleOut);
            AnsiConsole.Cursor.Show();
            AnsiConsole.Clear();
        }
    }

    private async Task RunRenderLoopAsync()
    {
        await AnsiConsole.Live(new Text("Initialisation..."))
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
                        await Task.Delay(120, _cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch
                    {
                        // Ne jamais faire crasher le rendu
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
                    _currentView = View.Detail;
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
                _currentView = View.List;
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
            AddLog($"[yellow]🔄 Rejouage #{record.Id} {(debugMode ? "en mode debug" : "")} lancé...[/]");
        }
        else if (record.TestSession != null)
        {
            Task.Run(() =>
            {
                if (debugMode)
                {
                    AddLog($"[yellow]🔗 Attachez le débogueur au PID [bold]{Process.GetCurrentProcess().Id}[/] puis appuyez sur une touche...[/]");
                    Debugger.Launch();
                }

                try
                {
                    var result = PluginTestRunner.Run(record.TestSession);
                    AddLog($"[green]✅ Rejouage #{record.Id} terminé ({result.OutputParameters?.Count ?? 0} OutputParams)[/]");
                }
                catch (Exception ex)
                {
                    AddLog($"[red]❌ Rejouage #{record.Id} échoué : {Markup.Escape(ex.Message)}[/]");
                }
            });
            AddLog($"[yellow]🔄 Rejouage #{record.Id} lancé...[/]");
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
    // Rendu — Vue principale (liste)
    // ════════════════════════════════════════════════════════════════

    private IRenderable BuildMainView()
    {
        var rows = new List<IRenderable>();

        // ── En-tête ──────────────────────────────────────────────────
        rows.Add(new Panel(
                new Markup($"[bold deepskyblue1]🛠  {AppTitle}[/]  [grey]|[/]  PID: [white]{Process.GetCurrentProcess().Id}[/]  [grey]|[/]  {DateTime.Now:HH:mm:ss}"))
            .Border(BoxBorder.None)
            .Padding(0, 0));

        // ── Table des exécutions ─────────────────────────────────────
        rows.Add(BuildExecutionTable());

        // ── Panel de logs ────────────────────────────────────────────
        rows.Add(BuildLogPanel());

        // ── Barre de raccourcis ──────────────────────────────────────
        rows.Add(new Rule("[grey][[↑↓]] Naviguer   [[Entrée]] Zoom in   [[R]] Rejouer   [[D]] Debug   [[S]] Sauvegarder   [[Q]] Quitter[/]")
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
            .AddColumn(new TableColumn("[white]Entité[/]").Width(22))
            .AddColumn(new TableColumn("[grey]Appels[/]").Centered().Width(7))
            .AddColumn(new TableColumn("[white]Statut[/]").Width(14));

        if (_executions.Count == 0)
        {
            table.AddRow(
                new Markup("[grey]─[/]"),
                new Markup("[grey]En attente d'exécutions…[/]"),
                new Text(""), new Text(""), new Text(""), new Text(""));
            return table;
        }

        // Afficher les 20 dernières exécutions
        var startIdx = Math.Max(0, _executions.Count - 20);
        for (int i = startIdx; i < _executions.Count; i++)
        {
            var rec = _executions[i];
            var isSelected = i == _selectedIndex;

            var idStr = isSelected
                ? $"[bold yellow]▶ {rec.Id}[/]"
                : $"[grey]{rec.Id}[/]";

            var pluginStr = isSelected
                ? $"[bold white]{Markup.Escape(rec.PluginShortName)}[/]"
                : $"[white]{Markup.Escape(rec.PluginShortName)}[/]";

            var entityStr = FormatEntityColumn(rec);

            var callsStr = rec.OrgServiceCallCount > 0
                ? $"[grey]{rec.OrgServiceCallCount}[/]"
                : "[grey]0[/]";

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
            return $"[white]{entity}[/] [grey]({shortId}…)[/]";
        }
        return $"[white]{entity}[/]";
    }

    private static string FormatStatus(ExecutionRecord rec)
    {
        switch (rec.Status)
        {
            case ExecutionStatus.Running:
                return $"[yellow]⏳ {rec.ElapsedTime.TotalMilliseconds:F0}ms…[/]";
            case ExecutionStatus.Succeeded:
                return $"[green]✅ {rec.Duration?.TotalMilliseconds:F0}ms[/]";
            case ExecutionStatus.Failed:
                return $"[red]❌ {rec.Duration?.TotalMilliseconds:F0}ms[/]";
            default:
                return "";
        }
    }

    private IRenderable BuildLogPanel()
    {
        var recentLogs = _logs.Skip(Math.Max(0, _logs.Count - MaxLogs)).ToList();
        var logContent = recentLogs.Count > 0
            ? string.Join("\n", recentLogs)
            : "[grey](aucun log)[/]";

        return new Panel(new Markup(logContent))
            .Header("[grey] Logs [/]")
            .Border(BoxBorder.Ascii)
            .BorderColor(Color.Grey23)
            .Padding(1, 0);
    }

    // ════════════════════════════════════════════════════════════════
    // Rendu — Vue détail (zoom in)
    // ════════════════════════════════════════════════════════════════

    private IRenderable BuildDetailView(ExecutionRecord rec)
    {
        var rows = new List<IRenderable>();

        // ── En-tête ──────────────────────────────────────────────────
        var headerStatus = FormatStatus(rec);
        rows.Add(new Panel(
                new Markup(
                    $"[bold]#{rec.Id}[/]  [deepskyblue1]{Markup.Escape(rec.PluginShortName)}[/]  " +
                    $"[grey]·[/]  [cyan]{Markup.Escape(rec.MessageName)}[/]  " +
                    $"[grey]·[/]  [white]{Markup.Escape(rec.PrimaryEntityName)}[/]" +
                    $"  [grey]({rec.PrimaryEntityId.ToString("D").Substring(0, 8)}…)[/]" +
                    $"    {headerStatus}"))
            .Border(BoxBorder.None)
            .Padding(0, 0));

        // ── Contexte d'entrée ─────────────────────────────────────────
        rows.Add(BuildInputContextPanel(rec));

        // ── Appels OrgService ─────────────────────────────────────────
        rows.Add(BuildOrgCallsPanel(rec));

        // ── Contexte de sortie ou erreur ──────────────────────────────
        if (rec.Status == ExecutionStatus.Succeeded)
            rows.Add(BuildOutputContextPanel(rec));
        else if (rec.Status == ExecutionStatus.Failed)
            rows.Add(BuildErrorPanel(rec));
        else
            rows.Add(new Panel(new Markup("[yellow]⏳ Exécution en cours…[/]"))
                .Header("[yellow] En cours [/]").Padding(1, 0));

        // ── Barre de raccourcis ───────────────────────────────────────
        var canReplay = rec.TestSession != null ? "" : "[grey strikethrough]";
        var canReplayEnd = rec.TestSession != null ? "" : "[/]";
        rows.Add(new Rule(
                $"[grey][[ESC]] Retour   [/]{canReplay}[[R]] Rejouer   [[D]] Debug{canReplayEnd}[grey]   [[S]] Sauvegarder   [[Q]] Quitter[/]")
            .Border(BoxBorder.None)
            .RuleStyle(Style.Parse("grey")));

        return new Rows(rows);
    }

    private IRenderable BuildInputContextPanel(ExecutionRecord rec)
    {
        var ctx = rec.InputContext;
        var sb = new StringBuilder();

        AppendField(sb, "Stage", FormatStage(ctx.Stage, ctx.IsWorkflowContext));
        AppendField(sb, "UserId", ctx.UserId.ToString("D").Substring(0, 8) + "…");
        AppendField(sb, "Entity", $"{ctx.PrimaryEntityName} ({ctx.PrimaryEntityId:D})");
        AppendField(sb, "Depth", ctx.Depth.ToString());

        if (ctx.InputParameters?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]InputParameters[/]");
            foreach (var param in ctx.InputParameters)
            {
                var value = FormatParameterValue(param.Value);
                sb.AppendLine($"    [grey]•[/] [cyan]{Markup.Escape(param.Key)}[/] = {value}");
            }
        }

        if (ctx.PreEntityImages?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]PreEntityImages[/]");
            foreach (var img in ctx.PreEntityImages)
                sb.AppendLine($"    [grey]•[/] [cyan]{Markup.Escape(img.Key)}[/] ({img.Value?.Attributes?.Count ?? 0} attributs)");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header("[deepskyblue1] Contexte d'entrée [/]")
            .BorderColor(Color.DeepSkyBlue1)
            .Padding(1, 0);
    }

    private IRenderable BuildOrgCallsPanel(ExecutionRecord rec)
    {
        var calls = rec.OrgServiceCalls;

        if (calls.Count == 0)
        {
            return new Panel(new Markup("[grey](aucun appel OrgService)[/]"))
                .Header($"[blue] Appels OrgService (0) [/]")
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
                statusIcon = "[yellow]⏳[/]";
            }
            else if (call.Success == true)
            {
                statusIcon = "[green]✅[/]";
                durationStr = $"  [grey]{call.Duration?.TotalMilliseconds:F0}ms[/]";
            }
            else
            {
                statusIcon = "[red]❌[/]";
                durationStr = call.ErrorMessage != null
                    ? $"  [red]{Markup.Escape(TruncateStr(call.ErrorMessage, 50))}[/]"
                    : "";
            }

            sb.AppendLine(
                $"  {statusIcon}  [grey]{call.Index}.[/]  " +
                $"[cyan]{Markup.Escape(call.RequestType)}[/]  " +
                $"[white]{Markup.Escape(call.EntityLogicalName)}[/]" +
                (call.EntityId != Guid.Empty ? $" [grey]({call.EntityId.ToString("D").Substring(0, 8)}…)[/]" : "") +
                durationStr);
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header($"[blue] Appels OrgService ({calls.Count}) [/]")
            .BorderColor(Color.Blue)
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
                sb.AppendLine($"    [grey]•[/] [cyan]{Markup.Escape(param.Key)}[/] = {value}");
            }
        }
        else
        {
            sb.AppendLine("  [grey]OutputParameters : (aucun)[/]");
        }

        if (ctx?.SharedVariables?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]SharedVariables[/]");
            foreach (var v in ctx.SharedVariables)
                sb.AppendLine($"    [grey]•[/] [cyan]{Markup.Escape(v.Key)}[/] = {FormatParameterValue(v.Value)}");
        }

        if (ctx?.PostEntityImages?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]PostEntityImages[/]");
            foreach (var img in ctx.PostEntityImages)
                sb.AppendLine($"    [grey]•[/] [cyan]{Markup.Escape(img.Key)}[/] ({img.Value?.Attributes?.Count ?? 0} attributs)");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header("[green] Contexte de sortie [/]")
            .BorderColor(Color.Green)
            .Padding(1, 0);
    }

    private IRenderable BuildErrorPanel(ExecutionRecord rec)
    {
        var ex = rec.Error;
        if (ex == null)
            return new Panel(new Markup("[red](erreur inconnue)[/]"))
                .Header("[red] Erreur [/]").Padding(1, 0);

        var sb = new StringBuilder();
        sb.AppendLine($"  [bold red]{Markup.Escape(ex.GetType().Name)}[/]");
        sb.AppendLine($"  [white]{Markup.Escape(TruncateStr(ex.Message, 200))}[/]");

        if (ex.StackTrace != null)
        {
            sb.AppendLine();
            sb.AppendLine("  [grey]Stack Trace :[/]");
            foreach (var line in ex.StackTrace.Split('\n').Take(8))
                sb.AppendLine($"  [grey]{Markup.Escape(line.TrimEnd())}[/]");
        }

        if (ex.InnerException != null)
        {
            sb.AppendLine();
            sb.AppendLine($"  [grey]Caused by :[/] [red]{Markup.Escape(ex.InnerException.GetType().Name)}[/]");
            sb.AppendLine($"  [grey]{Markup.Escape(TruncateStr(ex.InnerException.Message, 100))}[/]");
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header("[red] Erreur [/]")
            .BorderColor(Color.Red)
            .Padding(1, 0);
    }

    // ════════════════════════════════════════════════════════════════
    // Helpers de formatage
    // ════════════════════════════════════════════════════════════════

    private static void AppendField(StringBuilder sb, string label, string value)
    {
        sb.AppendLine($"  [grey]{label,-14}[/] [white]{Markup.Escape(value)}[/]");
    }

    private static string FormatStage(int stage, bool isWorkflow)
    {
        if (isWorkflow) return $"{stage} (Workflow)";
        return stage switch
        {
            10 => "10 (PreValidation)",
            20 => "20 (PreOperation)",
            40 => "40 (PostOperation)",
            _ => stage.ToString()
        };
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

    // ════════════════════════════════════════════════════════════════
    // Interception de Console.Out → journal interne
    // ════════════════════════════════════════════════════════════════

    private class LogCaptureWriter : System.IO.TextWriter
    {
        private readonly DebuggerConsoleUi _ui;
        private readonly System.IO.TextWriter _original;

        public LogCaptureWriter(DebuggerConsoleUi ui, System.IO.TextWriter original)
        {
            _ui = ui;
            _original = original;
        }

        public override Encoding Encoding => _original.Encoding;

        public override void WriteLine(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                _ui.AddLog($"[grey]{Markup.Escape(value)}[/]");
        }

        public override void Write(string value) { /* ignoré */ }
        public override void Write(char value) { /* ignoré */ }
    }
}
