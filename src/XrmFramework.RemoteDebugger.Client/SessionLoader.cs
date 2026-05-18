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
/// Charge les fichiers <c>.pluginsession.json</c> depuis un répertoire et les regroupe
/// par <c>CorrelationId</c> Dataverse pour alimenter le <see cref="SessionBrowserUi"/>.
/// </summary>
public static class SessionLoader
{
    /// <summary>
    /// Lit tous les fichiers <c>*.pluginsession.json</c> du répertoire indiqué,
    /// les regroupe par <c>CorrelationId</c> et retourne la liste de groupes triée
    /// par date de première occurrence (la plus récente en premier).
    /// </summary>
    /// <param name="directory">Répertoire contenant les fichiers de sessions.</param>
    /// <returns>Liste de groupes de corrélation (vide si le répertoire n'existe pas).</returns>
    public static List<CorrelationGroup> LoadCorrelationGroups(string directory)
    {
        if (!Directory.Exists(directory))
            return new List<CorrelationGroup>();

        var sessions = LoadSessions(directory);
        return BuildCorrelationGroups(sessions);
    }

    // ── Chargement des fichiers ──────────────────────────────────────────

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
                // Ignorer les fichiers corrompus ou illisibles
            }
        }

        // Trier par horodatage croissant afin que le premier élément
        // de chaque corrélation corresponde bien au premier événement déclenché.
        return sessions.OrderBy(s => s.Timestamp).ToList();
    }

    // ── Construction des groupes ─────────────────────────────────────────

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

                // Le nom du groupe est dérivé du premier élément de la corrélation.
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

        // Les groupes les plus récents apparaissent en premier dans l'interface.
        return groupDict.Values
            .OrderByDescending(g => g.FirstOccurrence)
            .ToList();
    }
}
