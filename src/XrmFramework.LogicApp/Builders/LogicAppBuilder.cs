// Copyright (c) DIMSI. All rights reserved.

using XrmFramework.LogicApp.Models;

namespace XrmFramework.LogicApp.Builders;

/// <summary>
/// Entry-point builder for an Azure Logic App.
/// Supports both Consumption and Standard hosting plans.
/// </summary>
/// <example>
/// <code>
/// // Consumption example
/// var workflow = LogicAppBuilder
///     .ForConsumption()
///     .WithWorkflow(w => w
///         .AddTrigger("manual", new HttpRequestTrigger())
///         .AddAction("Send_HTTP", new HttpAction { Method = "POST", Uri = "https://example.com" })
///     )
///     .Build();
///
/// // Standard example
/// var workflow = LogicAppBuilder
///     .ForStandard(WorkflowKind.Stateful)
///     .WithWorkflow(w => w
///         .AddTrigger("recurrence", new RecurrenceTrigger { Schedule = new RecurrenceSchedule { Frequency = "Hour" } })
///     )
///     .Build();
/// </code>
/// </example>
public class LogicAppBuilder
{
    private readonly LogicAppKind _kind;
    private readonly WorkflowKind _workflowKind;
    private WorkflowBuilder? _workflowBuilder;

    private LogicAppBuilder(LogicAppKind kind, WorkflowKind workflowKind = WorkflowKind.Stateful)
    {
        _kind = kind;
        _workflowKind = workflowKind;
    }

    // ──────────────────────────────────────────────
    //  Static factories
    // ──────────────────────────────────────────────

    /// <summary>
    /// Creates a builder targeting a Consumption Logic App (single JSON definition file).
    /// </summary>
    public static LogicAppBuilder ForConsumption() =>
        new(LogicAppKind.Consumption);

    /// <summary>
    /// Creates a builder targeting a Standard Logic App (workflow.json per workflow).
    /// </summary>
    /// <param name="workflowKind">Whether the workflow is Stateful or Stateless.</param>
    public static LogicAppBuilder ForStandard(WorkflowKind workflowKind = WorkflowKind.Stateful) =>
        new(LogicAppKind.Standard, workflowKind);

    // ──────────────────────────────────────────────
    //  Workflow configuration
    // ──────────────────────────────────────────────

    /// <summary>
    /// Configures the workflow definition using a <see cref="WorkflowBuilder"/>.
    /// </summary>
    public LogicAppBuilder WithWorkflow(Action<WorkflowBuilder> configure)
    {
        _workflowBuilder = new WorkflowBuilder();
        configure(_workflowBuilder);
        return this;
    }

    // ──────────────────────────────────────────────
    //  Connections (Consumption)
    // ──────────────────────────────────────────────

    private readonly Dictionary<string, object> _connections = new();

    /// <summary>
    /// Registers a managed API connection reference for use inside the workflow expressions.
    /// Only used for Consumption Logic Apps (stored in the root "parameters.$connections" property).
    /// </summary>
    /// <param name="connectionKey">
    /// The key used in expressions, e.g. <c>servicebus</c> → <c>@parameters('$connections')['servicebus']</c>.
    /// </param>
    /// <param name="connectionId">ARM resource ID of the API connection resource.</param>
    /// <param name="connectorId">ARM resource ID of the managed connector, e.g. the Service Bus connector.</param>
    public LogicAppBuilder AddConnection(string connectionKey, string connectionId, string connectorId)
    {
        _connections[connectionKey] = new
        {
            connectionId,
            connectionName = connectionKey,
            id = connectorId
        };
        return this;
    }

    // ──────────────────────────────────────────────
    //  Connection references (Standard)
    // ──────────────────────────────────────────────

    private readonly Dictionary<string, object> _connectionReferences = new();

    /// <summary>
    /// Registers a connection reference for a Standard Logic App.
    /// </summary>
    /// <param name="name">The connection reference name used in the workflow.</param>
    /// <param name="reference">
    /// The connection reference object (typically an object with
    /// <c>api.id</c>, <c>connection.id</c> and optional <c>authentication</c> properties).
    /// </param>
    public LogicAppBuilder AddConnectionReference(string name, object reference)
    {
        _connectionReferences[name] = reference;
        return this;
    }

    // ──────────────────────────────────────────────
    //  Build
    // ──────────────────────────────────────────────

    /// <summary>
    /// Builds and returns the Logic App object, ready to be serialised to JSON.
    /// Returns a <see cref="ConsumptionWorkflow"/> or a <see cref="StandardWorkflow"/>
    /// depending on the hosting plan.
    /// </summary>
    public object Build()
    {
        var definition = (_workflowBuilder ?? new WorkflowBuilder()).Build();

        if (_kind == LogicAppKind.Consumption)
        {
            var consumption = new ConsumptionWorkflow { Definition = definition };

            if (_connections.Count > 0)
            {
                consumption.Parameters = new Dictionary<string, object>
                {
                    ["$connections"] = new
                    {
                        value = _connections
                    }
                };
            }

            return consumption;
        }
        else
        {
            var standard = new StandardWorkflow
            {
                Definition = definition,
                Kind = _workflowKind.ToString()
            };

            if (_connectionReferences.Count > 0)
                standard.ConnectionReferences = _connectionReferences;

            return standard;
        }
    }

    /// <summary>
    /// Builds the Logic App and returns it typed as a <see cref="ConsumptionWorkflow"/>.
    /// Throws if the builder was not configured for Consumption.
    /// </summary>
    public ConsumptionWorkflow BuildConsumption()
    {
        if (_kind != LogicAppKind.Consumption)
            throw new InvalidOperationException("This builder is configured for Standard, not Consumption.");

        return (ConsumptionWorkflow)Build();
    }

    /// <summary>
    /// Builds the Logic App and returns it typed as a <see cref="StandardWorkflow"/>.
    /// Throws if the builder was not configured for Standard.
    /// </summary>
    public StandardWorkflow BuildStandard()
    {
        if (_kind != LogicAppKind.Standard)
            throw new InvalidOperationException("This builder is configured for Consumption, not Standard.");

        return (StandardWorkflow)Build();
    }
}
