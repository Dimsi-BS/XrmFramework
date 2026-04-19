// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger;

/// <summary>
/// Représente une session de débogage distant enregistrée.
/// Contient le contexte d'entrée, le contexte de sortie et tous les appels
/// au service d'organisation CRM effectués pendant l'exécution du plugin.
/// Ces sessions sont utilisées par le générateur de source pour créer des tests unitaires reproductibles.
/// </summary>
public class PluginTestSession
{
    /// <summary>Identifiant unique de la session enregistrée.</summary>
    public Guid SessionId { get; set; } = Guid.NewGuid();

    /// <summary>Date et heure d'enregistrement de la session (UTC).</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date et heure UTC du début de l'exécution originale du plugin.
    /// <para>
    /// Lors du rejouage, cette valeur est injectée dans le <see cref="RemoteDebugExecutionContext"/>
    /// afin que <c>IDateTimeProvider</c> retourne cette date fixe à la place de l'heure système.
    /// Cela rend reproductibles les calculs de dates relatives
    /// (ex : <c>clock.UtcNow.AddDays(30)</c>).
    /// </para>
    /// </summary>
    public DateTime ExecutionDate { get; set; }

    /// <summary>
    /// Nom qualifié complet du type du plugin ou de l'activité workflow.
    /// Utilisé pour instancier le type lors de la rejouée du test.
    /// </summary>
    public string PluginTypeAssemblyQualifiedName { get; set; }

    /// <summary>
    /// Le contexte d'exécution tel que reçu par le plugin, avant son exécution.
    /// Contient les InputParameters, PreEntityImages, etc.
    /// </summary>
    public RemoteDebugExecutionContext InputContext { get; set; }

    /// <summary>
    /// Le contexte d'exécution après que le plugin a terminé son exécution.
    /// Contient les OutputParameters, SharedVariables modifiés, etc.
    /// </summary>
    public RemoteDebugExecutionContext OutputContext { get; set; }

    /// <summary>
    /// Tous les appels au service d'organisation CRM effectués pendant l'exécution du plugin,
    /// dans l'ordre chronologique. Utilisés pour rejouer les réponses CRM sans connexion réseau.
    /// </summary>
    public List<RecordedOrgServiceCall> OrgServiceCalls { get; set; } = new List<RecordedOrgServiceCall>();
}

/// <summary>
/// Un appel enregistré au service d'organisation CRM effectué pendant l'exécution du plugin.
/// </summary>
public class RecordedOrgServiceCall
{
    /// <summary>
    /// L'OrganizationRequest sérialisé en JSON.
    /// Correspond au contenu du message envoyé au cloud CRM.
    /// </summary>
    public string RequestJson { get; set; }

    /// <summary>
    /// L'OrganizationResponse sérialisé en JSON, tel que retourné par CRM.
    /// Rejoué à l'identique lors de l'exécution des tests unitaires.
    /// </summary>
    public string ResponseJson { get; set; }
}
