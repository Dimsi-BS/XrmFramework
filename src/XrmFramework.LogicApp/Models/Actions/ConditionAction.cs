// Copyright (c) DIMSI. All rights reserved.

using Newtonsoft.Json;

namespace XrmFramework.LogicApp.Models.Actions;

/// <summary>
/// A Condition (If) action that branches workflow execution based on an expression.
/// </summary>
public class ConditionAction : ActionBase
{
    /// <inheritdoc />
    [JsonProperty("type")]
    public override string Type => "If";

    /// <summary>
    /// The condition expression. Can be a Logic Apps expression string or a structured object.
    /// Example: "@equals(triggerBody()?['status'], 'active')"
    /// </summary>
    [JsonProperty("expression")]
    public object? Expression { get; set; }

    /// <summary>
    /// Actions to execute when the condition evaluates to <c>true</c>.
    /// Keys are action names; values are action definitions.
    /// </summary>
    [JsonProperty("actions")]
    public Dictionary<string, ActionBase> TrueActions { get; set; } = new();

    /// <summary>
    /// Actions to execute when the condition evaluates to <c>false</c>.
    /// Keys are action names; values are action definitions.
    /// </summary>
    [JsonProperty("else")]
    public ConditionElseBranch? ElseBranch { get; set; }
}

/// <summary>
/// The else branch of a Condition action.
/// </summary>
public class ConditionElseBranch
{
    /// <summary>
    /// Actions to run in the else branch.
    /// </summary>
    [JsonProperty("actions")]
    public Dictionary<string, ActionBase> Actions { get; set; } = new();
}
