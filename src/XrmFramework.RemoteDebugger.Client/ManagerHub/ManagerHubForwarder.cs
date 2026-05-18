// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using XrmFramework.RemoteDebugger.Client.ConsoleUI;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client.ManagerHub;

/// <summary>
/// Transmet les événements d'exécution du débogueur distant vers le Manager
/// via une connexion SignalR persistante sur le <c>DesktopHub</c>.
/// </summary>
/// <remarks>
/// La connexion est établie de façon identique à l'application Desktop :
/// même URL (<c>ApiUrl/desktopHub</c>), même flux MSAL (silent → interactive),
/// même scope <c>desktop-connect</c>.
/// Si la connexion est absente ou échoue, les méthodes sont des no-op
/// silencieux — le débogueur local continue de fonctionner normalement.
/// </remarks>
internal sealed class ManagerHubForwarder : IDisposable
{
    private readonly HubConnection _connection;
    private readonly Action<string> _log;

    // Suivi des arbres parent/enfant : CorrelationId → (Depth → ExecutionId en cours)
    private readonly Dictionary<Guid, Dictionary<int, int>> _correlationTree = new();
    private readonly object _treeLock = new();

    // ── Constructeur ────────────────────────────────────────────────────────

    public ManagerHubForwarder(ManagerHubSettings settings, Action<string> log = null)
    {
        _log = log ?? Console.WriteLine;

        // Même flux d'authentification que l'application Desktop :
        // AcquireTokenSilent (cache) → AcquireTokenInteractive si nécessaire.
        var authService = new ManagerAuthService(settings, _log);

        // URL construite de la même façon que DesktopHubClient :
        //   new Uri(new Uri(hubOptions.ApiUrl), "/desktopHub")
        var hubUrl = new Uri(new Uri(settings.ApiUrl), "/desktopHub");

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => authService.AcquireTokenAsync();
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.Closed += ex =>
        {
            _log($"[ManagerHub] Connexion fermée{(ex != null ? $" : {ex.Message}" : "")}");
            return Task.CompletedTask;
        };

        _connection.Reconnecting += ex =>
        {
            _log($"[ManagerHub] Reconnexion en cours…{(ex != null ? $" ({ex.Message})" : "")}");
            return Task.CompletedTask;
        };

        _connection.Reconnected += _ =>
        {
            _log("[ManagerHub] Reconnecté au Manager Plugin Monitor.");
            return Task.CompletedTask;
        };
    }

    // ── Connexion ───────────────────────────────────────────────────────────

    /// <summary>
    /// Établit la connexion vers le DesktopHub du Manager.
    /// Un échec n'est pas bloquant : un message est loggé et les envois suivants
    /// seront ignorés silencieusement.
    /// </summary>
    public async Task ConnectAsync()
    {
        try
        {
            await _connection.StartAsync();
            _log("[ManagerHub] Connecté au Manager — les exécutions seront visibles dans Plugin Monitor.");
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Connexion échouée ({ex.Message}). Les événements ne seront pas transmis.");
        }
    }

    /// <summary>Indique si la connexion SignalR est active.</summary>
    public bool IsConnected => _connection.State == HubConnectionState.Connected;

    // ── Événements de cycle de vie ──────────────────────────────────────────

    /// <summary>Transmet le démarrage d'une exécution de plugin.</summary>
    public async Task OnExecutionStartedAsync(ExecutionRecord record)
    {
        if (!IsConnected) return;

        var parentId = ResolveParentAndRegister(record);

        try
        {
            await _connection.SendAsync("ForwardPluginExecutionStarted", new
            {
                Id                = record.Id,
                PluginShortName   = record.PluginShortName,
                MessageName       = record.MessageName,
                PrimaryEntityName = record.PrimaryEntityName,
                PrimaryEntityId   = record.PrimaryEntityId.ToString(),
                StartTime         = record.StartTime.ToUniversalTime(),
                CorrelationId     = record.InputContext.CorrelationId,
                Depth             = record.InputContext.Depth,
                ParentExecutionId = parentId,
                Stage             = record.InputContext.Stage
            });
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Erreur ForwardPluginExecutionStarted #{record.Id} : {ex.Message}");
        }
    }

    /// <summary>Transmet la fin réussie d'une exécution.</summary>
    public async Task OnExecutionCompletedAsync(ExecutionRecord record)
    {
        if (!IsConnected) return;

        UnregisterExecution(record);

        try
        {
            await _connection.SendAsync("ForwardPluginExecutionCompleted", new
            {
                Id                  = record.Id,
                DurationMs          = (long)(record.Duration?.TotalMilliseconds ?? 0),
                OrgServiceCallCount = record.OrgServiceCallCount
            });
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Erreur ForwardPluginExecutionCompleted #{record.Id} : {ex.Message}");
        }
    }

    /// <summary>Transmet l'échec d'une exécution.</summary>
    public async Task OnExecutionFailedAsync(ExecutionRecord record, Exception error)
    {
        if (!IsConnected) return;

        UnregisterExecution(record);

        try
        {
            await _connection.SendAsync("ForwardPluginExecutionFailed", new
            {
                Id           = record.Id,
                DurationMs   = (long)(record.Duration?.TotalMilliseconds ?? 0),
                ErrorMessage = error?.Message ?? "Erreur inconnue"
            });
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Erreur ForwardPluginExecutionFailed #{record.Id} : {ex.Message}");
        }
    }

    /// <summary>Transmet le démarrage d'un appel OrgService.</summary>
    public async Task OnOrgServiceCallStartedAsync(ExecutionRecord record, OrgServiceCallRecord call)
    {
        if (!IsConnected) return;

        try
        {
            await _connection.SendAsync("ForwardOrgServiceCallStarted", new
            {
                ExecutionId       = record.Id,
                Index             = call.Index,
                RequestType       = call.RequestType,
                EntityLogicalName = call.EntityLogicalName,
                EntityId          = call.EntityId.ToString(),
                StartTime         = call.StartTime.ToUniversalTime()
            });
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Erreur ForwardOrgServiceCallStarted #{record.Id}/{call.Index} : {ex.Message}");
        }
    }

    /// <summary>Transmet la fin d'un appel OrgService (succès ou échec).</summary>
    public async Task OnOrgServiceCallCompletedAsync(ExecutionRecord record, OrgServiceCallRecord call)
    {
        if (!IsConnected) return;

        try
        {
            await _connection.SendAsync("ForwardOrgServiceCallCompleted", new
            {
                ExecutionId       = record.Id,
                Index             = call.Index,
                DurationMs        = (long)(call.Duration?.TotalMilliseconds ?? 0),
                Success           = call.Success ?? false,
                ErrorMessage      = call.ErrorMessage,
                RequestJson       = call.RequestJson,
                ResponseJson      = call.ResponseJson
            });
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Erreur ForwardOrgServiceCallCompleted #{record.Id}/{call.Index} : {ex.Message}");
        }
    }

    // ── Suivi parent/enfant ─────────────────────────────────────────────────
    //
    // Même CorrelationId → table Depth → ExecutionId courant.
    // Le parent d'une exécution à profondeur N est l'entrée à profondeur N-1.

    private int? ResolveParentAndRegister(ExecutionRecord record)
    {
        var correlationId = record.InputContext.CorrelationId;
        var depth         = record.InputContext.Depth;

        lock (_treeLock)
        {
            if (!_correlationTree.TryGetValue(correlationId, out var depths))
            {
                depths = new Dictionary<int, int>();
                _correlationTree[correlationId] = depths;
            }

            int? parentId = null;
            if (depth > 1 && depths.TryGetValue(depth - 1, out var pid))
                parentId = pid;

            depths[depth] = record.Id;
            return parentId;
        }
    }

    private void UnregisterExecution(ExecutionRecord record)
    {
        var correlationId = record.InputContext.CorrelationId;
        var depth         = record.InputContext.Depth;

        lock (_treeLock)
        {
            if (!_correlationTree.TryGetValue(correlationId, out var depths)) return;
            depths.Remove(depth);
            if (depths.Count == 0)
                _correlationTree.Remove(correlationId);
        }
    }

    // ── IDisposable ─────────────────────────────────────────────────────────

    public void Dispose()
    {
        try
        {
            if (_connection.State != HubConnectionState.Disconnected)
                _connection.StopAsync().GetAwaiter().GetResult();
        }
        catch { /* ignorer les erreurs à la fermeture */ }
        finally
        {
            _connection.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
