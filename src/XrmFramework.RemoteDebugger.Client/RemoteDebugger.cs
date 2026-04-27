// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;
using System.Threading.Tasks;
using XrmFramework.RemoteDebugger;
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
            var ui = new DebuggerConsoleUI(
                onSave: SaveSession,
                onReplay: ReplayExecution);

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

            // ── Connexion Azure Relay ────────────────────────────────────
            ui.AddLog("[grey]Ouverture de la connexion Azure Relay…[/]");
            Manager.OpenAsync().GetAwaiter().GetResult();
            ui.AddLog($"[green]✅ Connexion établie — PID {System.Diagnostics.Process.GetCurrentProcess().Id}[/]");
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
            // ── Service provider avec interception des appels OrgService ─
            var serviceProvider = new LocalServiceProvider(remoteContext);

            serviceProvider.RequestSent += request =>
            {
                // Création et suivi de l'appel CRM
                var call = record.BeginOrgServiceCall(request.Content);
                OrgServiceCallStarted?.Invoke(record, call);

                // Transmission au cloud CRM
                var response = Manager.SendMessageWithResponse(request).GetAwaiter().GetResult();

                // Fin de l'appel (idempotent — ne pose pas de problème si rappelé)
                call.Complete(response.Content);
                OrgServiceCallCompleted?.Invoke(record, call);

                return response;
            };

            // ── Exécution du plugin dans un thread dédié ─────────────────
            var task = Task.Run(() => ExecutePluginType(remoteContext, serviceProvider));

            try
            {
                task.GetAwaiter().GetResult();

                // ── Succès ───────────────────────────────────────────────────
                // record.Complete() construit aussi la PluginTestSession pour le replay
                record.Complete(remoteContext);
                ExecutionCompleted?.Invoke(record);

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

                Manager.SendMessage(new RemoteDebuggerMessage(
                    RemoteDebuggerMessageType.Exception, e, remoteContext.Id));
            }
        }

        /// <summary>
        /// Résout le type du plugin depuis son AssemblyQualifiedName et l'exécute.
        /// </summary>
        private void ExecutePluginType(
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
            {
                // Type inconnu localement → renvoyer le contexte inchangé
                Manager.SendMessage(new RemoteDebuggerMessage(
                    RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id));
                return;
            }

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
                Console.WriteLine("Impossible de sauvegarder : pas de session disponible.");
                return;
            }

            var path = SessionSavePath ?? ".";
            TrySaveSession(record.TestSession, path);
        }

        private void TrySaveSession(PluginTestSession session, string path = null)
        {
            path = path ?? SessionSavePath ?? ".";
            try
            {
                PluginTestSessionRecorder.Save(path, session);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RemoteDebugger] Erreur de sauvegarde : {ex.Message}");
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
                Console.WriteLine("Impossible de rejouer : session non disponible.");
                return;
            }

            if (debugMode)
            {
                Console.WriteLine(
                    $"[Debug] Attachez le débogueur au processus PID {System.Diagnostics.Process.GetCurrentProcess().Id}, " +
                    "puis l'exécution démarrera automatiquement.");
                System.Diagnostics.Debugger.Launch();
            }

            try
            {
                Console.WriteLine($"Rejouage de l'exécution #{record.Id} ({record.PluginShortName})…");
                var output = PluginTestRunner.Run(record.TestSession);
                Console.WriteLine(
                    $"Rejouage #{record.Id} terminé — " +
                    $"{output.OutputParameters?.Count ?? 0} OutputParameter(s), " +
                    $"{output.SharedVariables?.Count ?? 0} SharedVariable(s).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rejouage #{record.Id} échoué : {ex.GetType().Name}: {ex.Message}");
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private static void AddWorkflowExtension<TService>(
            IServiceProvider provider, WorkflowInvoker invoker)
            where TService : class
        {
            var service = provider.GetService(typeof(TService));
            if (service != null)
                invoker.Extensions.Add<TService>(() => (TService)service);
        }
    }
}
