// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// A Scope action groups a set of actions so that error handling (e.g. try/catch patterns)
/// can be applied to the group as a whole.
/// </summary>
public class ScopeAction : ActionBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "Scope";

    /// <summary>
    /// Actions within this scope.
    /// Keys are action names; values are action definitions.
    /// </summary>
    [JsonProperty("actions")]
    public Dictionary<string, ActionBase> Actions { get; set; } = new();
}
