// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Triggers;

/// <summary>
/// A trigger driven by a managed API connector (e.g. Service Bus, SharePoint, Dataverse).
/// </summary>
public class ApiConnectionTrigger : TriggerBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "ApiConnection";

    /// <summary>
    /// The API connection inputs (host, method, path, queries, body).
    /// </summary>
    [JsonProperty("inputs")]
    public override object? Inputs => ConnectionInputs;

    /// <summary>
    /// The recurrence polling interval for this connector trigger.
    /// </summary>
    [JsonProperty("recurrence")]
    public override object? Recurrence => PollingSchedule;

    /// <summary>
    /// Connection and method inputs.
    /// </summary>
    [JsonIgnore]
    public ApiConnectionInputs ConnectionInputs { get; set; } = new();

    /// <summary>
    /// Polling schedule for connector triggers that poll a resource.
    /// </summary>
    [JsonIgnore]
    public RecurrenceSchedule? PollingSchedule { get; set; }
}

/// <summary>
/// Inputs for an ApiConnection trigger or action.
/// </summary>
public class ApiConnectionInputs
{
    /// <summary>
    /// Reference to the managed API connection resource.
    /// </summary>
    [JsonProperty("host")]
    public ApiConnectionHost Host { get; set; } = new();

    /// <summary>
    /// HTTP method (GET, POST, DELETE, …).
    /// </summary>
    [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
    public string? Method { get; set; }

    /// <summary>
    /// Relative path on the connector API.
    /// </summary>
    [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
    public string? Path { get; set; }

    /// <summary>
    /// Optional query parameters.
    /// </summary>
    [JsonProperty("queries", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string>? Queries { get; set; }

    /// <summary>
    /// Optional request body.
    /// </summary>
    [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
    public object? Body { get; set; }
}

/// <summary>
/// Identifies the managed API connection.
/// </summary>
public class ApiConnectionHost
{
    /// <summary>
    /// ARM resource reference to the managed connection, e.g.
    /// "@parameters('$connections')['servicebus']['connectionId']".
    /// </summary>
    [JsonProperty("connection")]
    public ApiConnectionReference Connection { get; set; } = new();

    /// <summary>
    /// Connector API identifier, e.g. "servicebus".
    /// </summary>
    [JsonProperty("api")]
    public ApiReference Api { get; set; } = new();
}

/// <summary>
/// Reference to a specific managed API connection instance.
/// </summary>
public class ApiConnectionReference
{
    /// <summary>
    /// Expression resolving to the connection resource ID.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Reference to a connector API.
/// </summary>
public class ApiReference
{
    /// <summary>
    /// The connector API identifier expression.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
}
