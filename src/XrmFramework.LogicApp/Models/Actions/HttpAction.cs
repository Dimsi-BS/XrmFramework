// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// An action that performs an HTTP request to any endpoint.
/// </summary>
public class HttpAction : ActionBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "Http";

    /// <inheritdoc />
    [JsonProperty("inputs")]
    public override object? Inputs => new HttpActionInputs
    {
        Method = Method,
        Uri = Uri,
        Headers = Headers,
        Body = Body,
        Authentication = Authentication
    };

    /// <summary>HTTP method (GET, POST, PUT, PATCH, DELETE).</summary>
    [JsonIgnore]
    public string Method { get; set; } = "GET";

    /// <summary>Target URI. Supports Logic Apps expressions.</summary>
    [JsonIgnore]
    public string Uri { get; set; } = string.Empty;

    /// <summary>Optional HTTP headers.</summary>
    [JsonIgnore]
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>Optional request body.</summary>
    [JsonIgnore]
    public object? Body { get; set; }

    /// <summary>Optional authentication settings.</summary>
    [JsonIgnore]
    public object? Authentication { get; set; }
}

internal class HttpActionInputs
{
    [JsonProperty("method")]
    public string Method { get; set; } = "GET";

    [JsonProperty("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string>? Headers { get; set; }

    [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
    public object? Body { get; set; }

    [JsonProperty("authentication", NullValueHandling = NullValueHandling.Ignore)]
    public object? Authentication { get; set; }
}
