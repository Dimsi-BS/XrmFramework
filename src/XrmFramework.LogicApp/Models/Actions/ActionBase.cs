// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// Base class for all Azure Logic App actions.
/// </summary>
public abstract class ActionBase
{
    /// <summary>
    /// The type of the action (e.g. "Http", "ApiConnection", "If", "Foreach", "ParseJson", "Compose", …).
    /// </summary>
    [JsonProperty("type")]
    public abstract string Type { get; }

    /// <summary>
    /// Optional list of action names that must complete before this action runs.
    /// </summary>
    [JsonProperty("runAfter", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string[]>? RunAfter { get; set; }

    /// <summary>
    /// Inputs for the action.
    /// </summary>
    [JsonProperty("inputs", NullValueHandling = NullValueHandling.Ignore)]
    public virtual object? Inputs => null;

    /// <summary>
    /// Optional description for documentation purposes.
    /// </summary>
    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }

    /// <summary>
    /// Adds a dependency: this action runs after <paramref name="actionName"/> succeeds.
    /// </summary>
    public ActionBase RunsAfter(string actionName, params string[] statuses)
    {
        RunAfter ??= new Dictionary<string, string[]>();
        RunAfter[actionName] = statuses.Length > 0 ? statuses : new[] { "Succeeded" };
        return this;
    }
}
