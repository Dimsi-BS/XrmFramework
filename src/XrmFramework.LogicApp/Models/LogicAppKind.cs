// Copyright (c) DIMSI. All rights reserved.

namespace XrmFramework.LogicApp.Models;

/// <summary>
/// Specifies the hosting plan of the Azure Logic App.
/// </summary>
public enum LogicAppKind
{
    /// <summary>
    /// Consumption-based Logic App (single JSON definition file, pay-per-execution).
    /// </summary>
    Consumption,

    /// <summary>
    /// Standard Logic App (folder-based structure, one workflow.json per workflow).
    /// Supports Stateful and Stateless workflows.
    /// </summary>
    Standard
}
