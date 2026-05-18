// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// The Parse JSON action parses a JSON payload and makes its properties
/// available as tokens in subsequent steps.
/// </summary>
public class ParseJsonAction : ActionBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "ParseJson";

    /// <inheritdoc />
    [JsonProperty("inputs")]
    public override object? Inputs => new ParseJsonInputs
    {
        Content = Content,
        Schema = Schema
    };

    /// <summary>
    /// The JSON content to parse. Usually a Logic Apps expression referencing a previous action's body.
    /// </summary>
    [JsonIgnore]
    public object? Content { get; set; }

    /// <summary>
    /// The JSON Schema that describes the expected structure of the content.
    /// </summary>
    [JsonIgnore]
    public object? Schema { get; set; }
}

internal class ParseJsonInputs
{
    [JsonProperty("content")]
    public object? Content { get; set; }

    [JsonProperty("schema")]
    public object? Schema { get; set; }
}
