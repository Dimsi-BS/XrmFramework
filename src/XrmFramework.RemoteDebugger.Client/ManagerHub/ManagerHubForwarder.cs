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
/// Forwards the remote debugger's execution events to the Manager
/// via a persistent SignalR connection on the <c>DesktopHub</c>.
/// </summary>
/// <remarks>
/// The connection is established identically to the Desktop application:
/// same URL (<c>ApiUrl/desktopHub</c>), same MSAL flow (silent -> interactive),
/// same <c>desktop-connect</c> scope.
/// If the connection is absent or fails, the methods are silent
/// no-ops — the local debugger keeps working normally.
/// </remarks>
internal sealed class ManagerHubForwarder : IDisposable
{
    private readonly HubConnection _connection;
    private readonly Action<string> _log;

    // Parent/child tree tracking: CorrelationId -> (Depth -> current ExecutionId)
    private readonly Dictionary<Guid, Dictionary<int, int>> _correlationTree = new();
    private readonly object _treeLock = new();

    // ── Constructor ───────────────────────────────────────────────────────

    public ManagerHubForwarder(ManagerHubSettings settings, Action<string> log = null)
    {
        _log = log ?? Console.WriteLine;

        // Same authentication flow as the Desktop application:
        // AcquireTokenSilent (cache) -> AcquireTokenInteractive if needed.
        var authService = new ManagerAuthService(settings, _log);

        // URL built the same way as DesktopHubClient:
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
            _log($"[ManagerHub] Connection closed{(ex != null ? $": {ex.Message}" : "")}");
            return Task.CompletedTask;
        };

        _connection.Reconnecting += ex =>
        {
            _log($"[ManagerHub] Reconnecting…{(ex != null ? $" ({ex.Message})" : "")}");
            return Task.CompletedTask;
        };

        _connection.Reconnected += _ =>
        {
            _log("[ManagerHub] Reconnected to the Manager Plugin Monitor.");
            return Task.CompletedTask;
        };
    }

    // ── Connection ────────────────────────────────────────────────────────

    /// <summary>
    /// Establishes the connection to the Manager's DesktopHub.
    /// A failure is not blocking: a message is logged and subsequent sends
    /// will be silently ignored.
    /// </summary>
    public async Task ConnectAsync()
    {
        try
        {
            await _connection.StartAsync();
            _log("[ManagerHub] Connected to the Manager — executions will be visible in Plugin Monitor.");
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Connection failed ({ex.Message}). Events will not be forwarded.");
        }
    }

    /// <summary>Indicates whether the SignalR connection is active.</summary>
    public bool IsConnected => _connection.State == HubConnectionState.Connected;

    // ── Lifecycle events ──────────────────────────────────────────────────

    /// <summary>Forwards the start of a plugin execution.</summary>
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
            _log($"[ManagerHub] Error ForwardPluginExecutionStarted #{record.Id}: {ex.Message}");
        }
    }

    /// <summary>Forwards the successful completion of an execution.</summary>
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
            _log($"[ManagerHub] Error ForwardPluginExecutionCompleted #{record.Id}: {ex.Message}");
        }
    }

    /// <summary>Forwards the failure of an execution.</summary>
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
                ErrorMessage = error?.Message ?? "Unknown error"
            });
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Error ForwardPluginExecutionFailed #{record.Id}: {ex.Message}");
        }
    }

    /// <summary>Forwards the start of an OrgService call.</summary>
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
            _log($"[ManagerHub] Error ForwardOrgServiceCallStarted #{record.Id}/{call.Index}: {ex.Message}");
        }
    }

    /// <summary>Forwards the end of an OrgService call (success or failure).</summary>
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
            _log($"[ManagerHub] Error ForwardOrgServiceCallCompleted #{record.Id}/{call.Index}: {ex.Message}");
        }
    }

    // ── Parent/child tracking ────────────────────────────────────────────────
    //
    // Same CorrelationId -> Depth table -> current ExecutionId.
    // The parent of an execution at depth N is the entry at depth N-1.

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

    // ── IDisposable ───────────────────────────────────────────────────────

    public void Dispose()
    {
        try
        {
            if (_connection.State != HubConnectionState.Disconnected)
                _connection.StopAsync().GetAwaiter().GetResult();
        }
        catch { /* ignore errors on shutdown */ }
        finally
        {
            _connection.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
