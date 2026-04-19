// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Parameters;

/// <summary>
/// Represents a parameter in an Azure Logic App workflow definition.
/// </summary>
public class WorkflowParameter
{
    /// <summary>
    /// The JSON type of the parameter (e.g. "string", "int", "bool", "object", "array", "securestring").
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "string";

    /// <summary>
    /// The default value of the parameter.
    /// </summary>
    [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Optional metadata for the parameter.
    /// </summary>
    [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
    public WorkflowParameterMetadata? Metadata { get; set; }
}

/// <summary>
/// Optional metadata attached to a workflow parameter.
/// </summary>
public class WorkflowParameterMetadata
{
    /// <summary>
    /// Human-readable description of the parameter.
    /// </summary>
    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }
}
