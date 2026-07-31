// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.Client.ConsoleUI;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client;

/// <summary>
/// Saves test sessions to disk in JSON format.
/// The produced files can be added as <c>AdditionalFiles</c> in a test project
/// so that the <c>XrmFramework.RemoteDebugger.Generator</c> generator automatically creates
/// the corresponding unit test methods.
/// </summary>
public static class PluginTestSessionRecorder
{
    /// <summary>
    /// Saves a test session to the specified directory.
    /// The generated file name follows the format:
    /// <c>{PluginName}_{yyyyMMdd_HHmmss}_{shortId}.pluginsession.json</c>
    /// </summary>
    /// <param name="directory">Target directory where the session file is saved.</param>
    /// <param name="session">The session to save.</param>
    /// <returns>Full path of the created file.</returns>
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
    /// Extracts the simple type name (without namespace or assembly information).
    /// </summary>
    private static string GetShortTypeName(string assemblyQualifiedName)
    {
        if (string.IsNullOrEmpty(assemblyQualifiedName))
            return "UnknownPlugin";

        // Take the first part (type name without assembly info)
        var typePart = assemblyQualifiedName.Split(new[] { ',' }, 2)[0].Trim();

        // Take the last segment (simple class name)
        var lastDot = typePart.LastIndexOf('.');
        return lastDot >= 0 ? typePart.Substring(lastDot + 1) : typePart;
    }
}
