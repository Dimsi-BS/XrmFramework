// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XrmFramework.LogicApp.Models;

namespace XrmFramework.LogicApp.Serialization;

/// <summary>
/// Serialises Logic App definitions to JSON files.
/// Supports both Consumption (single file) and Standard (one workflow.json per workflow) plans.
/// </summary>
public static class LogicAppSerializer
{
    private static readonly JsonSerializerSettings DefaultSettings = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    // ──────────────────────────────────────────────
    //  Consumption
    // ──────────────────────────────────────────────

    /// <summary>
    /// Serialises a <see cref="ConsumptionWorkflow"/> to a JSON string.
    /// </summary>
    public static string SerializeConsumption(ConsumptionWorkflow workflow, JsonSerializerSettings? settings = null)
    {
        return JsonConvert.SerializeObject(workflow, settings ?? DefaultSettings);
    }

    /// <summary>
    /// Serialises a <see cref="ConsumptionWorkflow"/> and writes it to <paramref name="outputPath"/>.
    /// </summary>
    /// <param name="workflow">The Consumption workflow to serialise.</param>
    /// <param name="outputPath">Full path to the output JSON file (e.g. "MyApp.json").</param>
    /// <param name="settings">Optional custom serialiser settings.</param>
    public static void WriteConsumption(ConsumptionWorkflow workflow, string outputPath, JsonSerializerSettings? settings = null)
    {
        var json = SerializeConsumption(workflow, settings);
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(outputPath, json);
    }

    // ──────────────────────────────────────────────
    //  Standard
    // ──────────────────────────────────────────────

    /// <summary>
    /// Serialises a <see cref="StandardWorkflow"/> to a JSON string.
    /// </summary>
    public static string SerializeStandard(StandardWorkflow workflow, JsonSerializerSettings? settings = null)
    {
        return JsonConvert.SerializeObject(workflow, settings ?? DefaultSettings);
    }

    /// <summary>
    /// Serialises a <see cref="StandardWorkflow"/> and writes it to
    /// <c>{outputDirectory}/{workflowName}/workflow.json</c>,
    /// following the Standard Logic App project structure.
    /// </summary>
    /// <param name="workflow">The Standard workflow to serialise.</param>
    /// <param name="workflowName">The folder name for this workflow (also used as the workflow's logical name).</param>
    /// <param name="outputDirectory">Root output directory of the Standard Logic App project.</param>
    /// <param name="settings">Optional custom serialiser settings.</param>
    /// <returns>The full path to the written workflow.json file.</returns>
    public static string WriteStandard(StandardWorkflow workflow, string workflowName, string outputDirectory, JsonSerializerSettings? settings = null)
    {
        var workflowDirectory = Path.Combine(outputDirectory, workflowName);
        Directory.CreateDirectory(workflowDirectory);

        var outputPath = Path.Combine(workflowDirectory, "workflow.json");
        var json = SerializeStandard(workflow, settings);
        File.WriteAllText(outputPath, json);
        return outputPath;
    }

    // ──────────────────────────────────────────────
    //  Generic / auto-detect
    // ──────────────────────────────────────────────

    /// <summary>
    /// Serialises any Logic App object returned by <see cref="Builders.LogicAppBuilder.Build()"/>
    /// to a JSON string, regardless of plan type.
    /// </summary>
    public static string Serialize(object workflowObject, JsonSerializerSettings? settings = null)
    {
        return JsonConvert.SerializeObject(workflowObject, settings ?? DefaultSettings);
    }
}
