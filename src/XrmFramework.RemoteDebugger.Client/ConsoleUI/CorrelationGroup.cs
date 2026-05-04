// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger.Client.ConsoleUI;

/// <summary>
/// Groupe de sessions de plugin partageant le même <see cref="CorrelationId"/> Dataverse.
/// Le groupe est nommé d'après le premier élément de la corrélation :
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

    /// <summary>Identifiant séquentiel du groupe (1, 2, 3…).</summary>
    public int Id { get; }

    /// <summary>CorrelationId Dataverse commun à toutes les sessions du groupe.</summary>
    public Guid CorrelationId { get; }

    /// <summary>
    /// Nom du groupe : "MessageName · PrimaryEntityName" du premier élément de la corrélation.
    /// </summary>
    public string Name { get; set; } = "Inconnu";

    /// <summary>
    /// Sessions de plugin appartenant à ce groupe, triées par horodatage croissant.
    /// </summary>
    public List<PluginTestSession> Sessions { get; } = new();

    /// <summary>Date de la première session dans le groupe.</summary>
    public DateTime FirstOccurrence { get; set; }

    /// <summary>Date de la dernière session dans le groupe.</summary>
    public DateTime LastOccurrence { get; set; }

    /// <summary>Nombre de sessions dans le groupe.</summary>
    public int SessionCount => Sessions.Count;
}
