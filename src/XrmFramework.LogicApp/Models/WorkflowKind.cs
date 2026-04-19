// Copyright (c) DIMSI. All rights reserved.

namespace XrmFramework.LogicApp.Models;

/// <summary>
/// Specifies whether a Standard Logic App workflow is Stateful or Stateless.
/// Only applicable when <see cref="LogicAppKind"/> is <see cref="LogicAppKind.Standard"/>.
/// </summary>
public enum WorkflowKind
{
    /// <summary>
    /// Stateful workflow: persists run history and supports long-running operations.
    /// </summary>
    Stateful,

    /// <summary>
    /// Stateless workflow: optimised for speed, no run history persistence.
    /// </summary>
    Stateless
}
