// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace XrmFramework.RemoteDebugger.Common.ConsoleUI
{
    /// <summary>
    /// Represents a recorded call to the CRM organization service
    /// made during a plugin's execution.
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

        /// <summary>Sequential number of the call within the execution (1-based).</summary>
        public int Index { get; set; }

        /// <summary>Raw JSON of the OrganizationRequest.</summary>
        public string RequestJson { get; }

        /// <summary>Raw JSON of the OrganizationResponse.</summary>
        public string ResponseJson { get; private set; }

        /// <summary>Name of the CRM message (Retrieve, Create, Update, Delete, Execute...).</summary>
        public string RequestType { get; private set; } = "Execute";

        /// <summary>Logical name of the target entity (if applicable).</summary>
        public string EntityLogicalName { get; private set; } = "";

        /// <summary>ID of the target entity (if applicable).</summary>
        public Guid EntityId { get; private set; }

        /// <summary>Start time of the call.</summary>
        public DateTime StartTime { get; }

        /// <summary>Duration of the call (null if still running).</summary>
        public TimeSpan? Duration { get; private set; }

        /// <summary>Indicates whether the call completed successfully.</summary>
        public bool? Success { get; private set; }

        /// <summary>Error message if the call failed.</summary>
        public string ErrorMessage { get; private set; }

        /// <summary>Indicates whether the call is still running.</summary>
        public bool IsRunning => !Duration.HasValue;

        internal void Complete(string responseJson)
        {
            // Idempotent: a call can only complete once
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
        /// Returns a short description for display: "Retrieve contact (3f4a…)"
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

            // Extract the RequestName (e.g. "Retrieve", "Create", "Update", "Delete")
            var nameMatch = RequestNameRegex.Match(json);
            if (nameMatch.Success)
            {
                RequestType = nameMatch.Groups[1].Value;
            }

            // Extract the LogicalName of the target entity
            var entityMatch = LogicalNameRegex.Match(json);
            if (entityMatch.Success)
            {
                EntityLogicalName = entityMatch.Groups[1].Value;
            }

            // Extract the entity ID
            var idMatch = EntityIdRegex.Match(json);
            if (idMatch.Success && Guid.TryParse(idMatch.Groups[1].Value, out var id))
            {
                EntityId = id;
            }
        }
    }
}
