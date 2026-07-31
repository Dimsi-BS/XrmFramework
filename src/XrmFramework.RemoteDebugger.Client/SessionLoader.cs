// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.Client.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client;

/// <summary>
/// Loads <c>.pluginsession.json</c> files from a directory and groups them
/// by Dataverse <c>CorrelationId</c> to feed the <see cref="SessionBrowserUi"/>.
/// </summary>
public static class SessionLoader
{
    /// <summary>
    /// Reads all <c>*.pluginsession.json</c> files from the given directory,
    /// groups them by <c>CorrelationId</c> and returns the list of groups sorted
    /// by first-occurrence date (most recent first).
    /// </summary>
    /// <param name="directory">Directory containing the session files.</param>
    /// <returns>List of correlation groups (empty if the directory does not exist).</returns>
    public static List<CorrelationGroup> LoadCorrelationGroups(string directory)
    {
        if (!Directory.Exists(directory))
            return new List<CorrelationGroup>();

        var sessions = LoadSessions(directory);
        return BuildCorrelationGroups(sessions);
    }

    // ── File loading ──────────────────────────────────────────────────────

    private static List<PluginTestSession> LoadSessions(string directory)
    {
        var sessions = new List<PluginTestSession>();

        var files = Directory.GetFiles(directory, "*.pluginsession.json", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file, System.Text.Encoding.UTF8);
                var session = JsonConvert.DeserializeObject<PluginTestSession>(
                    json,
                    RemoteDebuggerSettings.JsonSerializerSettings);

                if (session != null)
                    sessions.Add(session);
            }
            catch
            {
                // Ignore corrupted or unreadable files
            }
        }

        // Sort by ascending timestamp so that the first element
        // of each correlation corresponds to the first event triggered.
        return sessions.OrderBy(s => s.Timestamp).ToList();
    }

    // ── Group construction ────────────────────────────────────────────────

    private static List<CorrelationGroup> BuildCorrelationGroups(List<PluginTestSession> sessions)
    {
        var groupDict = new Dictionary<Guid, CorrelationGroup>();

        foreach (var session in sessions)
        {
            var correlationId = session.InputContext?.CorrelationId ?? Guid.Empty;

            if (!groupDict.TryGetValue(correlationId, out var group))
            {
                group = new CorrelationGroup(correlationId);
                groupDict[correlationId] = group;

                // The group name is derived from the first element of the correlation.
                var ctx = session.InputContext;
                if (ctx != null)
                {
                    var message = ctx.MessageName ?? "";
                    var entity  = ctx.PrimaryEntityName ?? "";
                    group.Name  = (message + " · " + entity).Trim(' ', '·');
                }

                group.FirstOccurrence = session.Timestamp;
            }

            group.Sessions.Add(session);
            group.LastOccurrence = session.Timestamp;
        }

        // The most recent groups appear first in the interface.
        return groupDict.Values
            .OrderByDescending(g => g.FirstOccurrence)
            .ToList();
    }
}
