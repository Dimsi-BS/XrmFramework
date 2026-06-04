// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Informations extraites d'une classe *Definition ([EntityDefinition]) dans un assembly.
    /// </summary>
    public sealed class DefinitionInfo
    {
        /// <summary>Nom de la table (.table filename sans extension), ex. "Contact".</summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>Logical name de l'entité CRM, ex. "contact".</summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>Collection name de l'entité CRM, ex. "contacts". Null si absent.</summary>
        public string EntityCollectionName { get; set; }

        /// <summary>Colonnes déclarées dans la nested class Columns.</summary>
        public IReadOnlyList<DefinitionColumnInfo> Columns { get; set; } = new List<DefinitionColumnInfo>();

        /// <summary>
        /// True si la classe est décorée par [GeneratedCode("XrmFramework", "2.0")] —
        /// c'est-à-dire entièrement générée par le TableSourceFileGenerator Roslyn.
        /// </summary>
        public bool IsFullyGenerated { get; set; }
    }
}
