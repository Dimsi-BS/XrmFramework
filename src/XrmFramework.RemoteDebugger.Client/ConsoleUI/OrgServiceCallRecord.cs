// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace XrmFramework.RemoteDebugger.Common.ConsoleUI
{
    /// <summary>
    /// Représente un appel enregistré au service d'organisation CRM
    /// effectué pendant l'exécution d'un plugin.
    /// </summary>
    public class OrgServiceCallRecord
    {
        private static readonly Regex RequestNameRegex =
            new(@"""RequestName""\s*:\s*""([^""]+)""", RegexOptions.Compiled);

        private static readonly Regex LogicalNameRegex =
            new(@"""LogicalName""\s*:\s*""([^""]+)""", RegexOptions.Compiled);

        private static readonly Regex EntityIdRegex =
            new(@"""Id""\s*:\s*""([0-9a-fA-F\-]{36})""", RegexOptions.Compiled);

        public OrgServiceCallRecord(string requestJson)
        {
            RequestJson = requestJson;
            StartTime = DateTime.Now;
            ParseRequestInfo(requestJson);
        }

        /// <summary>Numéro séquentiel de l'appel dans l'exécution (1-based).</summary>
        public int Index { get; set; }

        /// <summary>JSON brut de la requête OrganizationRequest.</summary>
        public string RequestJson { get; }

        /// <summary>JSON brut de la réponse OrganizationResponse.</summary>
        public string ResponseJson { get; private set; }

        /// <summary>Nom du message CRM (Retrieve, Create, Update, Delete, Execute...).</summary>
        public string RequestType { get; private set; } = "Execute";

        /// <summary>Nom logique de l'entité cible (si applicable).</summary>
        public string EntityLogicalName { get; private set; } = "";

        /// <summary>ID de l'entité cible (si applicable).</summary>
        public Guid EntityId { get; private set; }

        /// <summary>Heure de début de l'appel.</summary>
        public DateTime StartTime { get; }

        /// <summary>Durée de l'appel (null si en cours).</summary>
        public TimeSpan? Duration { get; private set; }

        /// <summary>Indique si l'appel s'est terminé avec succès.</summary>
        public bool? Success { get; private set; }

        /// <summary>Message d'erreur si l'appel a échoué.</summary>
        public string ErrorMessage { get; private set; }

        /// <summary>Indique si l'appel est encore en cours.</summary>
        public bool IsRunning => !Duration.HasValue;

        internal void Complete(string responseJson)
        {
            // Idempotent : un appel ne peut se terminer qu'une seule fois
            if (Duration.HasValue) return;
            ResponseJson = responseJson;
            Duration = DateTime.Now - StartTime;
            Success = true;
        }

        internal void Fail(string errorMessage)
        {
            // Idempotent
            if (Duration.HasValue) return;
            Duration = DateTime.Now - StartTime;
            Success = false;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Retourne une description courte pour affichage : "Retrieve contact (3f4a…)"
        /// </summary>
        public string GetShortDescription()
        {
            var sb = new System.Text.StringBuilder(RequestType);

            if (!string.IsNullOrEmpty(EntityLogicalName))
            {
                sb.Append(' ');
                sb.Append(EntityLogicalName);
            }

            if (EntityId != Guid.Empty)
            {
                sb.Append(" (");
                sb.Append(EntityId.ToString("D").Substring(0, 8));
                sb.Append("…)");
            }

            return sb.ToString();
        }

        private void ParseRequestInfo(string json)
        {
            if (string.IsNullOrEmpty(json)) return;

            // Extraire le RequestName (ex: "Retrieve", "Create", "Update", "Delete")
            var nameMatch = RequestNameRegex.Match(json);
            if (nameMatch.Success)
            {
                RequestType = nameMatch.Groups[1].Value;
            }

            // Extraire le LogicalName de l'entité cible
            var entityMatch = LogicalNameRegex.Match(json);
            if (entityMatch.Success)
            {
                EntityLogicalName = entityMatch.Groups[1].Value;
            }

            // Extraire l'ID de l'entité
            var idMatch = EntityIdRegex.Match(json);
            if (idMatch.Success && Guid.TryParse(idMatch.Groups[1].Value, out var id))
            {
                EntityId = id;
            }
        }
    }
}
