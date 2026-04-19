// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Triggers;

/// <summary>
/// Base class for all Azure Logic App triggers.
/// </summary>
public abstract class TriggerBase
{
    /// <summary>
    /// The type of the trigger (e.g. "Request", "Recurrence", "ApiConnection").
    /// </summary>
    [JsonProperty("type")]
    public abstract string Type { get; }

    /// <summary>
    /// Optional: the trigger kind (e.g. "Http" for a Request trigger).
    /// </summary>
    [JsonProperty("kind", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Kind => null;

    /// <summary>
    /// Inputs passed to the trigger.
    /// </summary>
    [JsonProperty("inputs", NullValueHandling = NullValueHandling.Ignore)]
    public virtual object? Inputs => null;

    /// <summary>
    /// Recurrence settings (used by Recurrence triggers).
    /// </summary>
    [JsonProperty("recurrence", NullValueHandling = NullValueHandling.Ignore)]
    public virtual object? Recurrence => null;
}
