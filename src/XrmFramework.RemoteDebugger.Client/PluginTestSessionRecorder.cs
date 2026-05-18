// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.Client.ConsoleUI;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client;

/// <summary>
/// Sauvegarde les sessions de test sur disque au format JSON.
/// Les fichiers produits peuvent être ajoutés comme <c>AdditionalFiles</c> dans un projet de test
/// pour que le générateur <c>XrmFramework.RemoteDebugger.Generator</c> crée automatiquement
/// les méthodes de tests unitaires correspondantes.
/// </summary>
public static class PluginTestSessionRecorder
{
    /// <summary>
    /// Sauvegarde une session de test dans le répertoire spécifié.
    /// Le nom du fichier généré suit le format :
    /// <c>{NomDuPlugin}_{yyyyMMdd_HHmmss}_{idCourt}.pluginsession.json</c>
    /// </summary>
    /// <param name="directory">Répertoire cible où sauvegarder le fichier de session.</param>
    /// <param name="session">La session à sauvegarder.</param>
    /// <returns>Chemin complet du fichier créé.</returns>
    public static string Save(string directory, PluginTestSession session)
    {
        if (directory == null) throw new ArgumentNullException(nameof(directory));
        if (session == null) throw new ArgumentNullException(nameof(session));

        Directory.CreateDirectory(directory);

        var pluginName = GetShortTypeName(session.PluginTypeAssemblyQualifiedName);
        var timestamp = session.Timestamp.ToString("yyyyMMdd_HHmmss");
        var shortId = session.SessionId.ToString("N").Substring(0, 8);

        var fileName = $"{pluginName}_{timestamp}_{shortId}.pluginsession.json";
        var filePath = Path.Combine(directory, fileName);

        var json = JsonConvert.SerializeObject(
            session,
            Formatting.Indented,
            RemoteDebuggerSettings.JsonSerializerSettings);

        File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);

        return filePath;
    }

    /// <summary>
    /// Extrait le nom simple du type (sans namespace ni informations d'assembly).
    /// </summary>
    private static string GetShortTypeName(string assemblyQualifiedName)
    {
        if (string.IsNullOrEmpty(assemblyQualifiedName))
            return "UnknownPlugin";

        // Prendre la première partie (nom du type sans infos d'assembly)
        var typePart = assemblyQualifiedName.Split(new[] { ',' }, 2)[0].Trim();

        // Prendre le dernier segment (nom de classe simple)
        var lastDot = typePart.LastIndexOf('.');
        return lastDot >= 0 ? typePart.Substring(lastDot + 1) : typePart;
    }
}
