// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Représente une colonne extraite d'une classe *Definition par réflexion.
    /// </summary>
    public sealed class DefinitionColumnInfo
    {
        /// <summary>Valeur de la constante (ex. "contactid") — clé de matching dans le .table.</summary>
        public string LogicalName { get; }

        /// <summary>Nom du champ C# (ex. "Id") — utilisé lors de la création d'une entrée absente du .table.</summary>
        public string Name { get; }

        public DefinitionColumnInfo(string logicalName, string name)
        {
            LogicalName = logicalName;
            Name = name;
        }
    }
}
