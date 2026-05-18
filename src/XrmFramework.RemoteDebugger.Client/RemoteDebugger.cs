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
    /// Débogueur distant qui reçoit les contextes d'exécution de plugins via Azure Relay,
    /// exécute les plugins localement, et retourne les contextes modifiés.
    ///
    /// <para><b>Mode standard</b> (sans TUI) :</para>
    /// <code>
    /// var debugger = new RemoteDebugger&lt;AzureRelayHybridConnectionMessageManager&gt;();
    /// debugger.Start();
    /// </code>
    ///
    /// <para><b>Mode TUI interactif</b> — interface console moderne :</para>
    /// <code>
    /// var debugger = new RemoteDebugger&lt;AzureRelayHybridConnectionMessageManager&gt;();
    /// debugger.SessionSavePath = @".\PluginTestSessions";
    /// debugger.StartWithConsoleUI();
    /// </code>
    /// </summary>
    public class RemoteDebugger<T> where T : IRemoteDebuggerMessageManager, new()
    {
        // ── Infrastructure ──────────────────────────────────────────────

        /// <summary>Gestionnaire de messages pour la communication Azure Relay.</summary>
        public T Manager { get; }

        // ── Enregistrement de sessions ──────────────────────────────────

        /// <summary>
        /// Répertoire de sauvegarde des sessions de test (.pluginsession.json).
        /// Si <c>null</c> (défaut), aucune sauvegarde automatique n'est effectuée.
        /// En mode TUI, la sauvegarde peut aussi être déclenchée manuellement via [S].
        /// </summary>
        public string SessionSavePath { get; set; }

        /// <summary>
        /// Répertoire de sauvegarde des messages échangés (.json).
        /// Si <c>null</c> (défaut), aucune sauvegarde automatique n'est effectuée.
        /// Chaque message (contexte entrant/sortant, requête/réponse OrgService, exception)
        /// est sauvegardé dans un fichier JSON distinct.
        /// </summary>
        public string MessageLogPath { get; set; }

        // ── Événements lifecycle ────────────────────────────────────────

        /// <summary>Déclenché au début de chaque exécution de plugin.</summary>
        public event Action<ExecutionRecord> ExecutionStarted;

        /// <summary>Déclenché au début de chaque appel OrgService.</summary>
        public event Action<ExecutionRecord, OrgServiceCallRecord> OrgServiceCallStarted;

        /// <summary>Déclenché à la fin de chaque appel OrgService.</summary>
        public event Action<ExecutionRecord, OrgServiceCallRecord> OrgServiceCallCompleted;

        /// <summary>Déclenché quand une exécution se termine avec succès.</summary>
        public event Action<ExecutionRecord> ExecutionCompleted;

        /// <summary>Déclenché quand une exécution échoue avec une exception.</summary>
        public event Action<ExecutionRecord, Exception> ExecutionFailed;

        // ── Événements messages ─────────────────────────────────────────

        /// <summary>
        /// Déclenché quand un message est reçu depuis le plugin (contexte entrant,
        /// réponse OrgService, etc.).
        /// </summary>
        public event Action<RemoteDebuggerMessage> MessageReceived;

        /// <summary>
        /// Déclenché quand un message est envoyé vers le plugin (contexte sortant,
        /// requête OrgService, exception).
        /// </summary>
        public event Action<RemoteDebuggerMessage> MessageSent;

        // ── Log interne ─────────────────────────────────────────────────

        // En mode standard : Console.WriteLine. En mode TUI : ui.AddLog (sans markup Spectre).
        private Action<string> _log = Console.WriteLine;

        // ── Connexion Manager (Plugin Monitor) ───────────────────────────

        /// <summary>
        /// Paramètres de connexion SignalR vers le Manager.
        /// Lorsqu'ils sont renseignés (URL + token non vides), les événements
        /// d'exécution sont transmis en temps réel à l'interface Plugin Monitor.
        /// Si la propriété est <c>null</c> ou que <see cref="ManagerHubSettings.IsConfigured"/>
        /// est <c>false</c>, le comportement du débogueur reste inchangé.
        /// </summary>
        public ManagerHubSettings ManagerHub { get; set; }

        private ManagerHubForwarder _hubForwarder;

        // ── Constructeur ────────────────────────────────────────────────

        public RemoteDebugger()
        {
            Manager = new T();
        }

        // ════════════════════════════════════════════════════════════════
        // Mode standard — sans TUI
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lance le débogueur en mode console standard.
        /// Bloque jusqu'à ce que l'utilisateur appuie sur Entrée.
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
        // Mode TUI — interface console interactive
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lance le débogueur avec l'interface console interactive (TUI Spectre.Console).
        /// <para>
        /// Fonctionnalités :
        /// <list type="bullet">
        ///   <item>Table live de toutes les exécutions (statut, durée, appels CRM)</item>
        ///   <item>[Entrée] Zoom in — détail de l'exécution sélectionnée</item>
        ///   <item>[ESC]   Zoom out — retour à la liste</item>
        ///   <item>[↑↓]   Navigation dans la liste</item>
        ///   <item>[R]    Rejouer l'exécution (sans débogueur)</item>
        ///   <item>[D]    Rejouer en mode debug (attache le débogueur)</item>
        ///   <item>[S]    Sauvegarder la session comme fichier .pluginsession.json</item>
        ///   <item>[Q]    Quitter</item>
        /// </list>
        /// </para>
        /// Bloque jusqu'à ce que l'utilisateur quitte avec [Q].
        /// </summary>
        public void StartWithConsoleUI()
        {
            var ui = new DebuggerConsoleUi(
                onSave: SaveSession,
                onReplay: ReplayExecution);

            // Rediriger les logs internes vers le TUI (sans markup Spectre)
            _log = msg => ui.AddLog($"[grey]{Markup.Escape(msg)}[/]");

            // ── Brancher les événements sur le TUI ───────────────────────
            // Note : la boucle de rendu (120ms) lit directement OrgServiceCalls
            // depuis ExecutionRecord — pas besoin de brancher OrgServiceCallStarted/Completed.

            Manager.ContextReceived += remoteContext =>
            {
                // Le TUI ajoute le record à sa liste interne et le retourne
                var record = ui.NotifyExecutionStarted(remoteContext);
                ExecutionStarted?.Invoke(record);
                RunExecution(remoteContext, record);
            };

            ExecutionCompleted += rec => ui.NotifyExecutionCompleted(rec, rec.OutputContext);
            ExecutionFailed   += (rec, ex) => ui.NotifyExecutionFailed(rec, ex);

            // ── Connexion Manager (Plugin Monitor) ──────────────────────
            InitHubForwarder(msg => ui.AddLog($"[grey]{Markup.Escape(msg)}[/]"));

            // ── Connexion Azure Relay ────────────────────────────────────
            ui.AddLog("[grey]Ouverture de la connexion Azure Relay…[/]");
            Manager.OpenAsync().GetAwaiter().GetResult();
            ui.AddLog($"[green]Connexion établie — PID {System.Diagnostics.Process.GetCurrentProcess().Id}[/]");
            ui.AddLog("[grey]En attente d'exécutions de plugins…[/]");

            // ── Lancer l'interface (bloque jusqu'au [Q]) ─────────────────
            ui.Run();

            // ── Fermeture propre ─────────────────────────────────────────
            Manager.CloseAsync().GetAwaiter().GetResult();
        }

        // ════════════════════════════════════════════════════════════════
        // Cœur de l'exécution (partagé entre les deux modes)
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Exécute un plugin localement en capturant tous les appels OrgService.
        /// Déclenche les événements lifecycle à chaque étape.
        /// </summary>
        private void RunExecution(RemoteDebugExecutionContext remoteContext, ExecutionRecord record)
        {
            // ── Contexte entrant ─────────────────────────────────────────
            var incomingContextMsg = new RemoteDebuggerMessage(
                RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
            OnMessageReceived(incomingContextMsg);

            // ── Service provider avec interception des appels OrgService ─
            var serviceProvider = new LocalServiceProvider(remoteContext, record.AddTraceLog);

            serviceProvider.RequestSent += request =>
            {
                // Création et suivi de l'appel CRM
                var call = record.BeginOrgServiceCall(request.Content);
                OrgServiceCallStarted?.Invoke(record, call);

                // Le message de requête OrgService est émis vers le cloud
                OnMessageSent(request);

                // Transmission au cloud CRM
                var response = Manager.SendMessageWithResponse(request).GetAwaiter().GetResult();

                // La réponse OrgService est reçue depuis le cloud
                OnMessageReceived(response);

                // Fin de l'appel (idempotent — ne pose pas de problème si rappelé)
                call.Complete(response.Content);
                OrgServiceCallCompleted?.Invoke(record, call);

                return response;
            };

            // ── Exécution du plugin dans un thread dédié ─────────────────
            var task = Task.Run(() => ExecutePluginType(remoteContext, serviceProvider));

            try
            {
                var pluginFound = task.GetAwaiter().GetResult();

                if (!pluginFound)
                {
                    // ── Type inconnu localement → renvoyer le contexte inchangé ──
                    // Marquer le record comme terminé pour stopper le timer dans le TUI.
                    record.Complete(remoteContext);
                    ExecutionCompleted?.Invoke(record);

                    var passthroughMsg = new RemoteDebuggerMessage(
                        RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
                    OnMessageSent(passthroughMsg);
                    Manager.SendMessage(passthroughMsg);
                    return;
                }

                // ── Succès ───────────────────────────────────────────────────
                // record.Complete() construit aussi la PluginTestSession pour le replay
                record.Complete(remoteContext);
                ExecutionCompleted?.Invoke(record);

                // Contexte sortant (après exécution locale)
                var outgoingContextMsg = new RemoteDebuggerMessage(
                    RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
                OnMessageSent(outgoingContextMsg);

                // Sauvegarde automatique sur disque si SessionSavePath est configuré
                if (SessionSavePath != null && record.TestSession != null)
                {
                    TrySaveSession(record.TestSession);
                }
            }
            catch (Exception e)
            {
                // ── Erreur ───────────────────────────────────────────────────
                record.Fail(e);
                ExecutionFailed?.Invoke(record, e);

                var exceptionMsg = new RemoteDebuggerMessage(
                    RemoteDebuggerMessageType.Exception, e, remoteContext.Id);
                OnMessageSent(exceptionMsg);
                Manager.SendMessage(exceptionMsg);
            }
        }

        /// <summary>
        /// Résout le type du plugin depuis son AssemblyQualifiedName et l'exécute.
        /// Retourne <c>true</c> si le plugin a été trouvé et exécuté,
        /// <c>false</c> si le type est inconnu localement (contexte à renvoyer inchangé).
        /// </summary>
        private bool ExecutePluginType(
            RemoteDebugExecutionContext remoteContext,
            LocalServiceProvider serviceProvider)
        {
            // Nettoyer le nom de type (supprimer Version= et PublicKeyToken= pour la portabilité)
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
        // Connexion Hub Manager
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Initialise et connecte le <see cref="ManagerHubForwarder"/> si
        /// <see cref="ManagerHub"/> est configuré. Sans effet sinon.
        /// </summary>
        private void InitHubForwarder(Action<string> logAction)
        {
            if (ManagerHub?.IsConfigured != true) return;

            _hubForwarder = new ManagerHubForwarder(ManagerHub, logAction);
            _hubForwarder.ConnectAsync().GetAwaiter().GetResult();
            WireHubForwarder();
        }

        /// <summary>
        /// Abonne le <see cref="ManagerHubForwarder"/> à tous les événements
        /// de cycle de vie en mode fire-and-forget (n'interrompt pas le flux local).
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
        // Journalisation des messages
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Déclenche l'événement <see cref="MessageReceived"/> et sauvegarde le message
        /// sur disque si <see cref="MessageLogPath"/> est configuré.
        /// </summary>
        private void OnMessageReceived(RemoteDebuggerMessage message)
        {
            MessageReceived?.Invoke(message);
            if (MessageLogPath != null)
                TryLogMessage(message, "IN");
        }

        /// <summary>
        /// Déclenche l'événement <see cref="MessageSent"/> et sauvegarde le message
        /// sur disque si <see cref="MessageLogPath"/> est configuré.
        /// </summary>
        private void OnMessageSent(RemoteDebuggerMessage message)
        {
            MessageSent?.Invoke(message);
            if (MessageLogPath != null)
                TryLogMessage(message, "OUT");
        }

        /// <summary>
        /// Sauvegarde un message dans un fichier JSON horodaté.
        /// Nom du fichier : <c>{PluginExecutionId}_{direction}_{MessageType}_{timestamp}.json</c>
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
                // Journalisation non bloquante — ignorer silencieusement les erreurs
            }
        }

        // ════════════════════════════════════════════════════════════════
        // Sauvegarde et Replay
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Sauvegarde la session d'un ExecutionRecord sur disque.
        /// Appelé depuis le TUI ([S]) ou automatiquement si SessionSavePath est défini.
        /// </summary>
        private void SaveSession(ExecutionRecord record)
        {
            if (record?.TestSession == null)
            {
                _log("Impossible de sauvegarder : pas de session disponible.");
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
                _log($"Session sauvegardee : {filePath}  ({session.OrgServiceCalls.Count} appel(s) OrgService)");
            }
            catch (Exception ex)
            {
                _log($"Erreur de sauvegarde : {ex.Message}");
            }
        }

        /// <summary>
        /// Rejoue une exécution enregistrée, en attachant optionnellement le débogueur.
        /// Appelé depuis le TUI ([R] ou [D]).
        /// </summary>
        private void ReplayExecution(ExecutionRecord record, bool debugMode)
        {
            if (record?.TestSession == null)
            {
                _log("Impossible de rejouer : session non disponible.");
                return;
            }

            if (debugMode)
            {
                _log($"Attachez le debogueur au PID {System.Diagnostics.Process.GetCurrentProcess().Id}, puis l'execution demarrera.");
                System.Diagnostics.Debugger.Launch();
            }

            try
            {
                _log($"Rejouage #{record.Id} ({record.PluginShortName})...");
                var output = PluginTestRunner.Run(record.TestSession);
                _log($"Rejouage #{record.Id} termine — {output.OutputParameters?.Count ?? 0} OutputParam(s), {output.SharedVariables?.Count ?? 0} SharedVar(s).");
            }
            catch (Exception ex)
            {
                _log($"Rejouage #{record.Id} echoue : {ex.GetType().Name}: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════════
        // Navigateur de sessions sauvegardées
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lance le navigateur interactif de sessions de test sauvegardées sur disque.
        /// <para>
        /// Affiche une interface console à trois niveaux :
        /// <list type="bullet">
        ///   <item>Groupes par <b>CorrelationId</b> — nommés d'après le premier plugin déclenché dans la corrélation.</item>
        ///   <item>Liste des <b>sessions</b> dans le groupe sélectionné.</item>
        ///   <item><b>Détail</b> complet d'une session (contexte d'entrée / appels OrgService / contexte de sortie).</item>
        /// </list>
        /// </para>
        /// Raccourcis : [↑↓] naviguer · [Entrée] zoomer · [Échap] remonter ·
        ///              [R] rejouer · [D] rejouer en debug · [F5] recharger · [Q] quitter.
        /// </summary>
        /// <param name="sessionPath">
        ///   Répertoire contenant les fichiers <c>*.pluginsession.json</c>.
        ///   Utilise <see cref="SessionSavePath"/> si non renseigné.
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
                        _log($"Attachez le debogueur au PID {System.Diagnostics.Process.GetCurrentProcess().Id}, puis l'execution demarrera.");
                        System.Diagnostics.Debugger.Launch();
                    }

                    try
                    {
                        var output = PluginTestRunner.Run(session);
                        _log($"Rejouage termine — {output.OutputParameters?.Count ?? 0} OutputParam(s), {output.SharedVariables?.Count ?? 0} SharedVar(s).");
                    }
                    catch (Exception ex)
                    {
                        _log($"Rejouage echoue : {ex.GetType().Name}: {ex.Message}");
                    }
                });

            ui.Run();
        }

        // ── Helpers ──────────────────────────────────────────────────────

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
