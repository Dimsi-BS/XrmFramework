// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;
using XrmFramework.LogicApp.Models.Triggers;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// An action driven by a managed API connector (Dataverse, SharePoint, Service Bus, Office 365, …).
/// </summary>
public class ApiConnectionAction : ActionBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "ApiConnection";

    /// <inheritdoc />
    [JsonProperty("inputs")]
    public override object? Inputs => ConnectionInputs;

    /// <summary>
    /// Connection host, method, path and optional body / queries.
    /// </summary>
    [JsonIgnore]
    public ApiConnectionInputs ConnectionInputs { get; set; } = new();
}
