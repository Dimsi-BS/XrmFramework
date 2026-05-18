// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client.ConsoleUI;

/// <summary>
/// Statut d'une exécution de plugin dans le débogueur distant.
/// </summary>
public enum ExecutionStatus
{
    /// <summary>Exécution en cours.</summary>
    Running,
    /// <summary>Exécution terminée avec succès.</summary>
    Succeeded,
    /// <summary>Exécution terminée avec une erreur.</summary>
    Failed
}

/// <summary>
/// Représente une exécution de plugin ou d'activité workflow suivie par le débogueur distant.
/// Contient toutes les informations nécessaires pour afficher, analyser et rejouer l'exécution.
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

        // Extraire le nom court du plugin
        PluginShortName = ExtractShortTypeName(inputContext.TypeAssemblyQualifiedName);
    }

    /// <summary>Identifiant séquentiel de l'exécution (1, 2, 3...).</summary>
    public int Id { get; }

    /// <summary>Nom court du type de plugin (sans namespace ni info d'assembly).</summary>
    public string PluginShortName { get; }

    /// <summary>Nom du message CRM (Create, Update, Delete...).</summary>
    public string MessageName => InputContext.MessageName ?? "";

    /// <summary>Nom logique de l'entité principale.</summary>
    public string PrimaryEntityName => InputContext.PrimaryEntityName ?? "";

    /// <summary>ID de l'entité principale.</summary>
    public Guid PrimaryEntityId => InputContext.PrimaryEntityId;

    /// <summary>Statut actuel de l'exécution.</summary>
    public ExecutionStatus Status { get; private set; }

    /// <summary>Heure de début de l'exécution.</summary>
    public DateTime StartTime { get; }

    /// <summary>Durée totale (null si encore en cours).</summary>
    public TimeSpan? Duration { get; private set; }

    /// <summary>Contexte d'entrée (snapshot avant exécution du plugin).</summary>
    public RemoteDebugExecutionContext InputContext { get; }

    /// <summary>Contexte de sortie (après exécution du plugin), null si encore en cours.</summary>
    public RemoteDebugExecutionContext OutputContext { get; private set; }

    /// <summary>Exception levée lors de l'exécution, null si succès.</summary>
    public Exception Error { get; private set; }

    /// <summary>Liste des appels OrgService effectués pendant l'exécution.</summary>
    public IReadOnlyList<OrgServiceCallRecord> OrgServiceCalls => _orgServiceCalls;
    private readonly List<OrgServiceCallRecord> _orgServiceCalls = new();

    /// <summary>Logs de tracing émis par le plugin via <c>ITracingService.Trace</c>.</summary>
    public IReadOnlyList<string> TraceLogs => _traceLogs;
    private readonly List<string> _traceLogs = new();

    /// <summary>
    /// Session de test complète pour rejouer cette exécution.
    /// Disponible uniquement après la fin de l'exécution.
    /// </summary>
    public PluginTestSession TestSession { get; private set; }

    /// <summary>
    /// Durée affichée (en cours ou terminée).
    /// </summary>
    public TimeSpan ElapsedTime => Duration ?? (DateTime.Now - StartTime);

    /// <summary>
    /// Nombre d'appels OrgService effectués (y compris ceux en cours).
    /// </summary>
    public int OrgServiceCallCount
    {
        get { lock (_lock) { return _orgServiceCalls.Count; } }
    }

    /// <summary>
    /// Ajoute une ligne de trace émise par le plugin via <c>ITracingService</c>.
    /// Appelé depuis le callback passé à <c>LocalServiceProvider</c>.
    /// </summary>
    internal void AddTraceLog(string message)
    {
        lock (_lock)
        {
            _traceLogs.Add(message);
        }
    }

    /// <summary>
    /// Crée un nouvel appel OrgService et l'ajoute à la liste.
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
    /// Marque l'exécution comme terminée avec succès et construit la session de test.
    /// </summary>
    internal void Complete(RemoteDebugExecutionContext outputContext)
    {
        OutputContext = outputContext;
        Duration = DateTime.Now - StartTime;
        Status = ExecutionStatus.Succeeded;

        BuildTestSession();
    }

    /// <summary>
    /// Marque l'exécution comme échouée.
    /// </summary>
    internal void Fail(Exception error)
    {
        Error = error;
        Duration = DateTime.Now - StartTime;
        Status = ExecutionStatus.Failed;
    }

    /// <summary>
    /// Construit la session de test à partir des données enregistrées.
    /// </summary>
    private void BuildTestSession()
    {
        // Copie profonde du contexte d'entrée via JSON round-trip
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
            // Heure UTC du début de l'exécution : utilisée lors du rejouage pour
            // injecter FixedDateTimeProvider et rendre reproductibles les dates relatives.
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

    /// <summary>Extrait le nom simple du type depuis le nom qualifié complet.</summary>
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

    /// <summary>Horodatage de l'exécution (heure locale), utilisé pour nommer le fichier de session.</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>Identifiant unique de la session, utilisé pour distinguer plusieurs sessions du même plugin.</summary>
    public Guid SessionId { get; set; } = Guid.NewGuid();

    public DateTime ExecutionDate { get; set; }

    public RemoteDebugExecutionContext InputContext { get; set; }

    public RemoteDebugExecutionContext OutputContext { get; set; }

    public IList<RecordedOrgServiceCall> OrgServiceCalls { get; set; } = new List<RecordedOrgServiceCall>();

    /// <summary>
    /// Logs émis par le plugin via <c>ITracingService.Trace</c> pendant l'exécution.
    /// Conservés dans la session pour analyse et rejouage.
    /// </summary>
    public IList<string> TraceLogs { get; set; } = new List<string>();
}
