// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;
using XrmFramework.LogicApp.Models.Actions;
using XrmFramework.LogicApp.Models.Parameters;
using XrmFramework.LogicApp.Models.Triggers;

namespace XrmFramework.LogicApp.Models;

/// <summary>
/// Represents the inner definition of an Azure Logic App workflow.
/// This maps to the "definition" object in the ARM template or the root of a Consumption JSON file.
/// </summary>
public class WorkflowDefinition
{
    /// <summary>
    /// The workflow definition schema URL (fixed value for all Logic Apps).
    /// </summary>
    [JsonProperty("$schema")]
    public string Schema { get; } =
        "https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#";

    /// <summary>
    /// Content version (always "1.0.0.0").
    /// </summary>
    [JsonProperty("contentVersion")]
    public string ContentVersion { get; } = "1.0.0.0";

    /// <summary>
    /// Parameters declared in the workflow definition.
    /// </summary>
    [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, WorkflowParameter>? Parameters { get; set; }

    /// <summary>
    /// The triggers that start this workflow. Keys are trigger names.
    /// </summary>
    [JsonProperty("triggers")]
    public Dictionary<string, TriggerBase> Triggers { get; set; } = new();

    /// <summary>
    /// The actions in the workflow. Keys are action names.
    /// </summary>
    [JsonProperty("actions")]
    public Dictionary<string, ActionBase> Actions { get; set; } = new();

    /// <summary>
    /// Optional outputs produced by the workflow.
    /// </summary>
    [JsonProperty("outputs", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? Outputs { get; set; }
}

/// <summary>
/// Represents a full Standard Logic App workflow file (workflow.json).
/// Wraps the <see cref="WorkflowDefinition"/> with the hosting kind.
/// </summary>
public class StandardWorkflow
{
    /// <summary>
    /// The inner workflow definition.
    /// </summary>
    [JsonProperty("definition")]
    public WorkflowDefinition Definition { get; set; } = new();

    /// <summary>
    /// The workflow kind: "Stateful" or "Stateless".
    /// </summary>
    [JsonProperty("kind")]
    public string Kind { get; set; } = "Stateful";

    /// <summary>
    /// Optional connection references used by managed API connectors (Standard plan).
    /// </summary>
    [JsonProperty("connectionReferences", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? ConnectionReferences { get; set; }
}

/// <summary>
/// Represents a full Consumption Logic App definition, including the "$connections" parameter.
/// </summary>
public class ConsumptionWorkflow
{
    /// <summary>
    /// The inner workflow definition.
    /// </summary>
    [JsonProperty("definition")]
    public WorkflowDefinition Definition { get; set; } = new();

    /// <summary>
    /// Runtime parameter values for the Consumption workflow
    /// (e.g. <c>$connections</c> referencing managed API connections).
    /// </summary>
    [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? Parameters { get; set; }
}
