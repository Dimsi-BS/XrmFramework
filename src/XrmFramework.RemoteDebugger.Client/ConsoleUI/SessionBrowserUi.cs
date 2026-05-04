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
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client.ConsoleUI;

/// <summary>
/// Interface console (TUI) pour naviguer dans les sessions de test sauvegardées sur disque.
/// <para>
/// Navigation à trois niveaux :
/// <list type="bullet">
///   <item><b>Niveau 1 — Corrélations</b> : groupes de sessions partageant le même CorrelationId.</item>
///   <item><b>Niveau 2 — Sessions</b>     : liste des fichiers dans une corrélation sélectionnée.</item>
///   <item><b>Niveau 3 — Détail</b>       : contexte complet d'une session (entrée / OrgService / sortie).</item>
/// </list>
/// </para>
/// Raccourcis clavier :
/// <list type="bullet">
///   <item>[↑↓]     Naviguer dans la liste courante</item>
///   <item>[Entrée] Descendre d'un niveau</item>
///   <item>[Échap]  Remonter d'un niveau</item>
///   <item>[R]      Rejouer la session sélectionnée (sans débogueur)</item>
///   <item>[D]      Rejouer en mode debug (attach debugger)</item>
///   <item>[F5]     Recharger les fichiers depuis le disque</item>
///   <item>[Q]      Quitter</item>
/// </list>
/// </summary>
public class SessionBrowserUi
{
    // ── Vues ─────────────────────────────────────────────────────────────
    private enum View { Correlations, Sessions, Detail }

    private View _currentView = View.Correlations;

    // ── Données ───────────────────────────────────────────────────────────
    private List<CorrelationGroup> _groups = new();
    private int _groupIndex;
    private int _sessionIndex;

    private readonly string _sessionPath;
    private readonly Action<PluginTestSession, bool> _onReplay;

    // ── Verrou & journal ──────────────────────────────────────────────────
    private readonly object _lock = new();
    private readonly List<string> _logs = new();
    private const int MaxLogs = 6;

    // ── Cycle de vie ──────────────────────────────────────────────────────
    private CancellationTokenSource _cts;
    private System.IO.TextWriter _originalOut;

    private const string AppTitle = "XrmFramework Session Browser";

    // ════════════════════════════════════════════════════════════════════
    // Constructeur
    // ════════════════════════════════════════════════════════════════════

    /// <param name="sessionPath">
    ///   Répertoire contenant les fichiers <c>*.pluginsession.json</c>.
    /// </param>
    /// <param name="onReplay">
    ///   Callback appelé lorsque l'utilisateur lance un rejouage.
    ///   Si <c>null</c>, le rejouage est effectué en local via <see cref="PluginTestRunner"/>.
    /// </param>
    public SessionBrowserUi(string sessionPath, Action<PluginTestSession, bool> onReplay = null)
    {
        _sessionPath = sessionPath;
        _onReplay    = onReplay;
    }

    // ════════════════════════════════════════════════════════════════════
    // Entrée principale
    // ════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Lance l'interface console. Bloque jusqu'à ce que l'utilisateur quitte avec [Q].
    /// </summary>
    public void Run()
    {
        _cts = new CancellationTokenSource();
        Reload();

        _originalOut = Console.Out;
        Console.SetOut(new LogCaptureWriter(this, _originalOut));

        try
        {
            AnsiConsole.Cursor.Hide();
            AnsiConsole.Clear();

            var renderTask = Task.Run(async () => await RunRenderLoopAsync(), _cts.Token);
            RunKeyboardLoop();

            try { renderTask.GetAwaiter().GetResult(); }
            catch (OperationCanceledException) { }
        }
        finally
        {
            Console.SetOut(_originalOut);
            AnsiConsole.Cursor.Show();
            AnsiConsole.Clear();
        }
    }

    // ── Chargement ────────────────────────────────────────────────────────

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
            AddLog($"[red]Erreur de chargement : {Markup.Escape(ex.Message)}[/]");
        }

        lock (_lock)
        {
            _groups     = loaded;
            _groupIndex = Math.Min(_groupIndex, Math.Max(0, _groups.Count - 1));
        }

        var totalSessions = loaded.Sum(g => g.SessionCount);
        AddLog(
            $"[grey]Chargé[/] [white]{totalSessions}[/] [grey]session(s) dans[/] " +
            $"[white]{loaded.Count}[/] [grey]corrélation(s) depuis[/] " +
            $"[cyan]{Markup.Escape(_sessionPath)}[/]");
    }

    // ════════════════════════════════════════════════════════════════════
    // Boucle de rendu (thread séparé)
    // ════════════════════════════════════════════════════════════════════

    private async Task RunRenderLoopAsync()
    {
        await AnsiConsole.Live(new Text("Initialisation…"))
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
                                View.Detail   when TryGetSelectedSession(out var s) => BuildDetailView(s),
                                _                                                   => BuildCorrelationsView()
                            };
                        }
                        ctx.UpdateTarget(view);
                        await Task.Delay(200, _cts.Token);
                    }
                    catch (OperationCanceledException) { break; }
                    catch { /* ne jamais faire crasher le rendu */ }
                }
            });
    }

    // ════════════════════════════════════════════════════════════════════
    // Boucle clavier (thread principal)
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
                Task.Run(Reload);
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

    // ── Rejouage ──────────────────────────────────────────────────────────

    private void LaunchReplay(PluginTestSession session, bool debugMode)
    {
        var pluginName = session.InputContext != null
            ? ExtractShortTypeName(session.InputContext.TypeAssemblyQualifiedName)
            : "plugin";

        AddLog($"[yellow]🔄 Rejouage de [bold]{Markup.Escape(pluginName)}[/]{(debugMode ? " [grey](debug)[/]" : "")} lancé…[/]");

        if (_onReplay != null)
        {
            Task.Run(() => _onReplay(session, debugMode));
            return;
        }

        Task.Run(() =>
        {
            if (debugMode)
            {
                AddLog($"[yellow]🔗 Attachez le débogueur au PID [bold]{Process.GetCurrentProcess().Id}[/]…[/]");
                Debugger.Launch();
            }

            try
            {
                var result = PluginTestRunner.Run(session);
                AddLog(
                    $"[green]✅ Rejouage terminé[/] [grey]—[/] " +
                    $"[white]{result.OutputParameters?.Count ?? 0}[/] [grey]OutputParam(s)[/]");
            }
            catch (Exception ex)
            {
                AddLog($"[red]❌ Rejouage échoué : {Markup.Escape(ex.Message)}[/]");
            }
        });
    }

    // ════════════════════════════════════════════════════════════════════
    // Sélection courante
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
    // Vue 1 — Liste des corrélations
    // ════════════════════════════════════════════════════════════════════

    private IRenderable BuildCorrelationsView()
    {
        var rows = new List<IRenderable>();

        rows.Add(BuildHeader(
            $"[bold deepskyblue1]🗂  {AppTitle}[/]  " +
            $"[grey]|[/]  [cyan]{Markup.Escape(_sessionPath)}[/]  " +
            $"[grey]|[/]  {DateTime.Now:HH:mm:ss}"));

        rows.Add(BuildCorrelationTable());
        rows.Add(BuildLogPanel());
        rows.Add(BuildShortcutsBar(
            "[[↑↓]] Naviguer   [[Entrée]] Ouvrir   [[F5]] Recharger   [[Q]] Quitter"));

        return new Rows(rows);
    }

    private IRenderable BuildCorrelationTable()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey37)
            .AddColumn(new TableColumn("[grey]#[/]").RightAligned().Width(4))
            .AddColumn(new TableColumn("[white]Corrélation[/]").Width(32))
            .AddColumn(new TableColumn("[grey]Sessions[/]").Centered().Width(9))
            .AddColumn(new TableColumn("[white]Première occurrence[/]").Width(20))
            .AddColumn(new TableColumn("[white]Dernière occurrence[/]").Width(20))
            .AddColumn(new TableColumn("[grey]CorrelationId[/]").Width(14));

        if (_groups.Count == 0)
        {
            table.AddRow(
                new Markup("[grey]─[/]"),
                new Markup("[grey]Aucune session trouvée — vérifiez le chemin ou appuyez sur [[F5]][/]"),
                new Text(""), new Text(""), new Text(""), new Text(""));
            return table;
        }

        // Afficher les 20 derniers groupes
        var startIdx = Math.Max(0, _groups.Count - 20);
        for (int i = startIdx; i < _groups.Count; i++)
        {
            var g          = _groups[i];
            var isSelected = i == _groupIndex;

            var idStr   = isSelected ? $"[bold yellow]▶ {g.Id}[/]" : $"[grey]{g.Id}[/]";
            var nameStr = isSelected
                ? $"[bold white]{Markup.Escape(g.Name)}[/]"
                : $"[white]{Markup.Escape(g.Name)}[/]";

            var lastStr = g.SessionCount > 1
                ? $"[grey]{g.LastOccurrence:dd/MM/yy HH:mm:ss}[/]"
                : "[grey]─[/]";

            table.AddRow(
                new Markup(idStr),
                new Markup(nameStr),
                new Markup($"[grey]{g.SessionCount}[/]"),
                new Markup($"[grey]{g.FirstOccurrence:dd/MM/yy HH:mm:ss}[/]"),
                new Markup(lastStr),
                new Markup($"[grey]{g.CorrelationId.ToString("D").Substring(0, 8)}…[/]"));
        }

        return table;
    }

    // ════════════════════════════════════════════════════════════════════
    // Vue 2 — Sessions dans une corrélation
    // ════════════════════════════════════════════════════════════════════

    private IRenderable BuildSessionsView(CorrelationGroup group)
    {
        var rows = new List<IRenderable>();

        rows.Add(BuildHeader(
            $"[bold deepskyblue1]🗂  {AppTitle}[/]  " +
            $"[grey]›[/]  [bold white]{Markup.Escape(group.Name)}[/]  " +
            $"[grey]({group.CorrelationId.ToString("D").Substring(0, 8)}…)[/]  " +
            $"[grey]|[/]  {DateTime.Now:HH:mm:ss}"));

        rows.Add(BuildSessionTable(group));
        rows.Add(BuildLogPanel());
        rows.Add(BuildShortcutsBar(
            "[[ESC]] Retour   [[↑↓]] Naviguer   [[Entrée]] Détail   [[R]] Rejouer   [[D]] Debug   [[Q]] Quitter"));

        return new Rows(rows);
    }

    private IRenderable BuildSessionTable(CorrelationGroup group)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey37)
            .AddColumn(new TableColumn("[grey]#[/]").RightAligned().Width(4))
            .AddColumn(new TableColumn("[white]Plugin[/]").Width(22))
            .AddColumn(new TableColumn("[white]Message[/]").Width(12))
            .AddColumn(new TableColumn("[white]Entité[/]").Width(22))
            .AddColumn(new TableColumn("[grey]Appels[/]").Centered().Width(7))
            .AddColumn(new TableColumn("[white]Horodatage[/]").Width(18));

        for (int i = 0; i < group.Sessions.Count; i++)
        {
            var s          = group.Sessions[i];
            var isSelected = i == _sessionIndex;
            var ctx        = s.InputContext;

            var pluginShort = ctx != null
                ? ExtractShortTypeName(ctx.TypeAssemblyQualifiedName)
                : "?";

            var idStr = isSelected
                ? $"[bold yellow]▶ {i + 1}[/]"
                : $"[grey]{i + 1}[/]";

            var pluginStr = isSelected
                ? $"[bold white]{Markup.Escape(pluginShort)}[/]"
                : $"[white]{Markup.Escape(pluginShort)}[/]";

            table.AddRow(
                new Markup(idStr),
                new Markup(pluginStr),
                new Markup($"[cyan]{Markup.Escape(ctx?.MessageName ?? "")}[/]"),
                new Markup(FormatEntityColumn(ctx)),
                new Markup($"[grey]{s.OrgServiceCalls?.Count ?? 0}[/]"),
                new Markup($"[grey]{s.Timestamp:dd/MM/yy HH:mm:ss}[/]"));
        }

        return table;
    }

    // ════════════════════════════════════════════════════════════════════
    // Vue 3 — Détail d'une session
    // ════════════════════════════════════════════════════════════════════

    private IRenderable BuildDetailView(PluginTestSession session)
    {
        var ctx         = session.InputContext;
        var pluginShort = ctx != null ? ExtractShortTypeName(ctx.TypeAssemblyQualifiedName) : "?";

        var rows = new List<IRenderable>();

        rows.Add(BuildHeader(
            $"[bold deepskyblue1]🗂  {AppTitle}[/]  [grey]›[/]  " +
            $"[deepskyblue1]{Markup.Escape(pluginShort)}[/]  " +
            $"[grey]·[/]  [cyan]{Markup.Escape(ctx?.MessageName ?? "")}[/]  " +
            $"[grey]·[/]  [white]{Markup.Escape(ctx?.PrimaryEntityName ?? "")}[/]  " +
            $"[grey]|[/]  {session.Timestamp:dd/MM/yy HH:mm:ss}"));

        if (ctx != null)
            rows.Add(BuildInputContextPanel(ctx));

        rows.Add(BuildOrgCallsPanel(session));

        if (session.OutputContext != null)
            rows.Add(BuildOutputContextPanel(session.OutputContext));

        rows.Add(BuildShortcutsBar(
            "[[ESC]] Retour   [[R]] Rejouer   [[D]] Debug   [[Q]] Quitter"));

        return new Rows(rows);
    }

    private IRenderable BuildInputContextPanel(RemoteDebugExecutionContext ctx)
    {
        var sb = new StringBuilder();

        AppendField(sb, "Stage",         FormatStage(ctx.Stage, ctx.IsWorkflowContext));
        AppendField(sb, "UserId",        ctx.UserId.ToString("D").Substring(0, 8) + "…");
        AppendField(sb, "Entity",        $"{ctx.PrimaryEntityName} ({ctx.PrimaryEntityId:D})");
        AppendField(sb, "Depth",         ctx.Depth.ToString());
        AppendField(sb, "CorrelationId", ctx.CorrelationId.ToString("D"));

        if (ctx.InputParameters?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]InputParameters[/]");
            foreach (var param in ctx.InputParameters)
                sb.AppendLine($"    [grey]•[/] [cyan]{Markup.Escape(param.Key)}[/] = {FormatParamValue(param.Value)}");
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

    private IRenderable BuildOrgCallsPanel(PluginTestSession session)
    {
        var calls = session.OrgServiceCalls;

        if (calls == null || calls.Count == 0)
        {
            return new Panel(new Markup("[grey](aucun appel OrgService enregistré)[/]"))
                .Header("[blue] Appels OrgService (0) [/]")
                .BorderColor(Color.Blue)
                .Padding(1, 0);
        }

        var sb = new StringBuilder();
        for (int i = 0; i < calls.Count; i++)
        {
            // Réutiliser OrgServiceCallRecord pour analyser le JSON de requête
            var rec = new OrgServiceCallRecord(calls[i].RequestJson ?? "");
            sb.AppendLine(
                $"  [green]✅[/]  [grey]{i + 1}.[/]  " +
                $"[cyan]{Markup.Escape(rec.RequestType)}[/]  " +
                $"[white]{Markup.Escape(rec.EntityLogicalName)}[/]" +
                (rec.EntityId != Guid.Empty
                    ? $" [grey]({rec.EntityId.ToString("D").Substring(0, 8)}…)[/]"
                    : ""));
        }

        return new Panel(new Markup(sb.ToString().TrimEnd()))
            .Header($"[blue] Appels OrgService ({calls.Count}) [/]")
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
                sb.AppendLine($"    [grey]•[/] [cyan]{Markup.Escape(p.Key)}[/] = {FormatParamValue(p.Value)}");
        }
        else
        {
            sb.AppendLine("  [grey]OutputParameters : (aucun)[/]");
        }

        if (ctx.SharedVariables?.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("  [underline]SharedVariables[/]");
            foreach (var v in ctx.SharedVariables)
                sb.AppendLine($"    [grey]•[/] [cyan]{Markup.Escape(v.Key)}[/] = {FormatParamValue(v.Value)}");
        }

        if (ctx.PostEntityImages?.Count > 0)
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

    // ════════════════════════════════════════════════════════════════════
    // Helpers de rendu communs
    // ════════════════════════════════════════════════════════════════════

    private static IRenderable BuildHeader(string markup)
        => new Panel(new Markup(markup))
            .Border(BoxBorder.None)
            .Padding(0, 0);

    private static IRenderable BuildShortcutsBar(string shortcuts)
        => new Rule($"[grey]{shortcuts}[/]")
            .Border(BoxBorder.None)
            .RuleStyle(Style.Parse("grey"));

    private IRenderable BuildLogPanel()
    {
        var recent  = _logs.Skip(Math.Max(0, _logs.Count - MaxLogs)).ToList();
        var content = recent.Count > 0 ? string.Join("\n", recent) : "[grey](aucun log)[/]";

        return new Panel(new Markup(content))
            .Header("[grey] Logs [/]")
            .Border(BoxBorder.Ascii)
            .BorderColor(Color.Grey23)
            .Padding(1, 0);
    }

    // ════════════════════════════════════════════════════════════════════
    // Helpers de formatage
    // ════════════════════════════════════════════════════════════════════

    private static string FormatEntityColumn(RemoteDebugExecutionContext ctx)
    {
        if (ctx == null) return "";
        var entity = Markup.Escape(ctx.PrimaryEntityName ?? "");
        return ctx.PrimaryEntityId != Guid.Empty
            ? $"[white]{entity}[/] [grey]({ctx.PrimaryEntityId.ToString("D").Substring(0, 8)}…)[/]"
            : $"[white]{entity}[/]";
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
        var str = value.ToString();
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
    // Journal & interception Console.Out
    // ════════════════════════════════════════════════════════════════════

    /// <summary>Ajoute un message horodaté au journal interne.</summary>
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

    private class LogCaptureWriter : System.IO.TextWriter
    {
        private readonly SessionBrowserUi    _ui;
        private readonly System.IO.TextWriter _original;

        public LogCaptureWriter(SessionBrowserUi ui, System.IO.TextWriter original)
        {
            _ui      = ui;
            _original = original;
        }

        public override Encoding Encoding => _original.Encoding;

        public override void WriteLine(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                _ui.AddLog($"[grey]{Markup.Escape(value)}[/]");
        }

        public override void Write(string value) { }
        public override void Write(char value)   { }
    }
}
