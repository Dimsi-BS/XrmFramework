// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Triggers;

/// <summary>
/// A trigger that fires when an HTTP request is received (formerly "manual" trigger).
/// Generates a "Request" type trigger with kind "Http".
/// </summary>
public class HttpRequestTrigger : TriggerBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "Request";

    /// <inheritdoc />
    [JsonProperty("kind")]
    public override string Kind => "Http";

    /// <inheritdoc />
    [JsonProperty("inputs")]
    public override object? Inputs => new HttpRequestTriggerInputs
    {
        Schema = Schema
    };

    /// <summary>
    /// Optional JSON Schema describing the expected request body.
    /// Leave null to accept any body.
    /// </summary>
    [JsonIgnore]
    public object? Schema { get; set; }
}

internal class HttpRequestTriggerInputs
{
    [JsonProperty("schema")]
    public object? Schema { get; set; }
}
