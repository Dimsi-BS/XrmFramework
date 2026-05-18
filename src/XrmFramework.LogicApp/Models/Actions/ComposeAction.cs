// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// The Compose action constructs any data object or expression and stores the result.
/// Use <c>@outputs('ActionName')</c> to reference the result in later actions.
/// </summary>
public class ComposeAction : ActionBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "Compose";

    /// <inheritdoc />
    [JsonProperty("inputs")]
    public override object? Inputs => Value;

    /// <summary>
    /// The value or expression to compose. May be a literal, an object, or a Logic Apps expression string.
    /// </summary>
    [JsonIgnore]
    public object? Value { get; set; }
}
