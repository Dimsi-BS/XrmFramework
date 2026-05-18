// Copyright (c) DIMSI. All rights reserved.

using XrmFramework.LogicApp.Models;
using XrmFramework.LogicApp.Models.Actions;
using XrmFramework.LogicApp.Models.Parameters;
using XrmFramework.LogicApp.Models.Triggers;

namespace XrmFramework.LogicApp.Builders;

/// <summary>
/// Fluent builder for constructing an Azure Logic App <see cref="WorkflowDefinition"/>.
/// </summary>
public class WorkflowBuilder
{
    private readonly WorkflowDefinition _definition = new();

    // ──────────────────────────────────────────────
    //  Triggers
    // ──────────────────────────────────────────────

    /// <summary>
    /// Adds a trigger to the workflow.
    /// </summary>
    /// <param name="name">The trigger name used in the workflow JSON.</param>
    /// <param name="trigger">The trigger instance to add.</param>
    public WorkflowBuilder AddTrigger(string name, TriggerBase trigger)
    {
        _definition.Triggers[name] = trigger;
        return this;
    }

    /// <summary>
    /// Configures a trigger using a factory function.
    /// </summary>
    public WorkflowBuilder AddTrigger<T>(string name, Action<T> configure) where T : TriggerBase, new()
    {
        var trigger = new T();
        configure(trigger);
        return AddTrigger(name, trigger);
    }

    // ──────────────────────────────────────────────
    //  Actions
    // ──────────────────────────────────────────────

    /// <summary>
    /// Adds an action to the workflow.
    /// </summary>
    /// <param name="name">The action name used in the workflow JSON and in RunAfter references.</param>
    /// <param name="action">The action instance to add.</param>
    public WorkflowBuilder AddAction(string name, ActionBase action)
    {
        _definition.Actions[name] = action;
        return this;
    }

    /// <summary>
    /// Configures and adds an action using a factory function.
    /// </summary>
    public WorkflowBuilder AddAction<T>(string name, Action<T> configure) where T : ActionBase, new()
    {
        var action = new T();
        configure(action);
        return AddAction(name, action);
    }

    // ──────────────────────────────────────────────
    //  Parameters
    // ──────────────────────────────────────────────

    /// <summary>
    /// Declares a workflow parameter (visible inside the definition).
    /// </summary>
    public WorkflowBuilder AddParameter(string name, WorkflowParameter parameter)
    {
        _definition.Parameters ??= new Dictionary<string, WorkflowParameter>();
        _definition.Parameters[name] = parameter;
        return this;
    }

    /// <summary>
    /// Declares a simple typed parameter with an optional default value.
    /// </summary>
    public WorkflowBuilder AddParameter(string name, string type, object? defaultValue = null, string? description = null)
    {
        return AddParameter(name, new WorkflowParameter
        {
            Type = type,
            DefaultValue = defaultValue,
            Metadata = description is not null ? new WorkflowParameterMetadata { Description = description } : null
        });
    }

    // ──────────────────────────────────────────────
    //  Outputs
    // ──────────────────────────────────────────────

    /// <summary>
    /// Adds an output to the workflow.
    /// </summary>
    public WorkflowBuilder AddOutput(string name, object value)
    {
        _definition.Outputs ??= new Dictionary<string, object>();
        _definition.Outputs[name] = value;
        return this;
    }

    // ──────────────────────────────────────────────
    //  Build
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns the built <see cref="WorkflowDefinition"/>.
    /// </summary>
    public WorkflowDefinition Build() => _definition;
}
