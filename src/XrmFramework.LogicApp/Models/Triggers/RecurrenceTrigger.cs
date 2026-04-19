// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Triggers;

/// <summary>
/// A trigger that fires on a schedule.
/// </summary>
public class RecurrenceTrigger : TriggerBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "Recurrence";

    /// <summary>
    /// The recurrence schedule settings.
    /// </summary>
    [JsonProperty("recurrence")]
    public override object? Recurrence => Schedule;

    /// <summary>
    /// The recurrence schedule.
    /// </summary>
    [JsonIgnore]
    public RecurrenceSchedule Schedule { get; set; } = new();
}

/// <summary>
/// Defines the recurrence interval and frequency.
/// </summary>
public class RecurrenceSchedule
{
    /// <summary>
    /// The interval between executions (e.g. 1, 5, 30).
    /// </summary>
    [JsonProperty("interval")]
    public int Interval { get; set; } = 1;

    /// <summary>
    /// The frequency unit: "Second", "Minute", "Hour", "Day", "Week", "Month".
    /// </summary>
    [JsonProperty("frequency")]
    public string Frequency { get; set; } = "Hour";

    /// <summary>
    /// Optional start time in ISO 8601 format.
    /// </summary>
    [JsonProperty("startTime", NullValueHandling = NullValueHandling.Ignore)]
    public string? StartTime { get; set; }

    /// <summary>
    /// Optional time zone identifier (e.g. "Romance Standard Time").
    /// </summary>
    [JsonProperty("timeZone", NullValueHandling = NullValueHandling.Ignore)]
    public string? TimeZone { get; set; }
}
