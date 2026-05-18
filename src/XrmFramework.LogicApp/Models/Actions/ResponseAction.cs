// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// A Response action that sends an HTTP response back to the caller of an HTTP Request trigger.
/// </summary>
public class ResponseAction : ActionBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "Response";

    /// <inheritdoc />
    [JsonProperty("inputs")]
    public override object? Inputs => new ResponseActionInputs
    {
        StatusCode = StatusCode,
        Body = Body,
        Headers = Headers
    };

    /// <summary>HTTP status code to return (e.g. 200, 400, 500).</summary>
    [JsonIgnore]
    public int StatusCode { get; set; } = 200;

    /// <summary>Optional response body.</summary>
    [JsonIgnore]
    public object? Body { get; set; }

    /// <summary>Optional response headers.</summary>
    [JsonIgnore]
    public Dictionary<string, string>? Headers { get; set; }
}

internal class ResponseActionInputs
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
    public object? Body { get; set; }

    [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string>? Headers { get; set; }
}
