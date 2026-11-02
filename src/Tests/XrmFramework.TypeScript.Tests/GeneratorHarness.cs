// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MsBuildTypeScript;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XrmFramework.TypeScript.Tests;

/// <summary>
/// Collects what the generator reports, so a test can assert on a warning or an error instead of
/// only on the files written.
/// </summary>
internal sealed class RecordingLog : IGeneratorLog
{
    public List<string> Warnings { get; } = new();

    public List<string> Errors { get; } = new();

    public void Warning(string message) => Warnings.Add(message);

    public void Error(string message) => Errors.Add(message);

    public void Message(string message) { }
}

/// <summary>
/// Runs <see cref="TsGenerator" /> over <c>.table</c> files written to a temporary directory, and
/// reads back what it generated.
/// </summary>
internal sealed class GeneratorHarness : IDisposable
{
    private readonly string _root;
    private readonly string _sources;
    private readonly List<string> _paths = new();

    public GeneratorHarness()
    {
        _root = Path.Combine(Path.GetTempPath(), "XrmFramework.Ts_" + Guid.NewGuid().ToString("N"));
        _sources = Path.Combine(_root, "sources");
        OutputDirectory = Path.Combine(_root, "out");

        Directory.CreateDirectory(_sources);
    }

    public string OutputDirectory { get; }

    public RecordingLog Log { get; } = new();

    /// <summary>Writes a <c>.table</c> the run will read, under the given file name.</summary>
    public GeneratorHarness WithTable(string fileName, object table)
        => WithRawTable(fileName, JsonConvert.SerializeObject(table, Formatting.Indented));

    /// <summary>Writes a <c>.table</c> verbatim, for content the model would not produce.</summary>
    public GeneratorHarness WithRawTable(string fileName, string json)
    {
        var path = Path.Combine(_sources, fileName + ".table");

        File.WriteAllText(path, json);
        _paths.Add(path);

        return this;
    }

    /// <summary>
    /// Runs the generator. Files are passed in the order they were declared, as the
    /// <c>.targets</c> passes the framework's tables before the project's.
    /// </summary>
    /// <returns>Whether it ran without reporting an error, as the task reports it.</returns>
    public bool Run()
    {
        new TsGenerator(Log, OutputDirectory, "type").Generate(_paths);

        return Log.Errors.Count == 0;
    }

    public IReadOnlyList<string> DefinitionNames()
        => Directory.GetFiles(Path.Combine(OutputDirectory, "definitions"), "*.d.ts")
                    .Select(path => Path.GetFileName(path)!)
                    .Select(name => name.Substring(0, name.Length - ".d.ts".Length))
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToList();

    /// <summary>The JSON a definition carries, which is the table as the generator declares it.</summary>
    public JObject Definition(string definitionName)
    {
        var text = File.ReadAllText(
            Path.Combine(OutputDirectory, "definitions", definitionName + ".d.ts"));

        return JObject.Parse(text.Substring(text.IndexOf('=') + 1).Trim());
    }

    public string Enums() => File.ReadAllText(Path.Combine(OutputDirectory, "enums.ts"));

    public string Tables() => File.ReadAllText(Path.Combine(OutputDirectory, "tables.d.ts"));

    /// <summary>Names of the enums <c>enums.ts</c> declares.</summary>
    public IReadOnlyList<string> EnumNames()
        => Enums().Split('\n')
                  .Where(line => line.StartsWith("export const enum ", StringComparison.Ordinal))
                  .Select(line => line.Substring("export const enum ".Length).Trim())
                  .OrderBy(name => name, StringComparer.Ordinal)
                  .ToList();

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);
        }
        catch { /* best effort */ }
    }
}
