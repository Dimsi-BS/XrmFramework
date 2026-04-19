// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// A Foreach action that iterates over an array and executes a set of inner actions for each item.
/// </summary>
public class ForEachAction : ActionBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "Foreach";

    /// <summary>
    /// The Logic Apps expression that resolves to the array to iterate over.
    /// Example: "@body('Parse_JSON')?['items']"
    /// </summary>
    [JsonProperty("foreach")]
    public string? CollectionExpression { get; set; }

    /// <summary>
    /// Actions to execute for each element in the collection.
    /// Keys are action names; values are action definitions.
    /// </summary>
    [JsonProperty("actions")]
    public Dictionary<string, ActionBase> Actions { get; set; } = new();

    /// <summary>
    /// When true, iterations run sequentially instead of in parallel.
    /// </summary>
    [JsonProperty("operationOptions", NullValueHandling = NullValueHandling.Ignore)]
    public string? OperationOptions => Sequential ? "Sequential" : null;

    /// <summary>
    /// Controls whether iterations run sequentially (default: false = parallel).
    /// </summary>
    [JsonIgnore]
    public bool Sequential { get; set; }
}
