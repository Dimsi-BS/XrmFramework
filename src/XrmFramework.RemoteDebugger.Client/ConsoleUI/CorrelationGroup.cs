// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger.Client.ConsoleUI;

/// <summary>
/// Group of plugin sessions sharing the same Dataverse <see cref="CorrelationId"/>.
/// The group is named after the first element of the correlation:
/// "<c>MessageName · PrimaryEntityName</c>".
/// </summary>
public class CorrelationGroup
{
    private static int _nextId;

    public CorrelationGroup(Guid correlationId)
    {
        CorrelationId = correlationId;
        Id = System.Threading.Interlocked.Increment(ref _nextId);
    }

    /// <summary>Sequential identifier of the group (1, 2, 3…).</summary>
    public int Id { get; }

    /// <summary>Dataverse CorrelationId common to all sessions in the group.</summary>
    public Guid CorrelationId { get; }

    /// <summary>
    /// Name of the group: "MessageName · PrimaryEntityName" of the first element of the correlation.
    /// </summary>
    public string Name { get; set; } = "Unknown";

    /// <summary>
    /// Plugin sessions belonging to this group, sorted by ascending timestamp.
    /// </summary>
    public List<PluginTestSession> Sessions { get; } = new();

    /// <summary>Date of the first session in the group.</summary>
    public DateTime FirstOccurrence { get; set; }

    /// <summary>Date of the last session in the group.</summary>
    public DateTime LastOccurrence { get; set; }

    /// <summary>Number of sessions in the group.</summary>
    public int SessionCount => Sessions.Count;
}
