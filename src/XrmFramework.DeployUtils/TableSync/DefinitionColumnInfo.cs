// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Represents a column extracted from a *Definition class via reflection.
    /// </summary>
    public sealed class DefinitionColumnInfo
    {
        /// <summary>Value of the constant (e.g. "contactid") — matching key in the .table.</summary>
        public string LogicalName { get; }

        /// <summary>C# field name (e.g. "Id") — used when creating an entry missing from the .table.</summary>
        public string Name { get; }

        /// <summary>
        /// C# name of the enum declared by <c>[OptionSet(typeof(...))]</c> on the field
        /// (e.g. "UtilisateurExecutant"), or <see langword="null" /> if the column carries no option set.
        /// </summary>
        /// <remarks>
        /// This is an identifier the project's code depends on, and teams rename it freely — the CRM
        /// only knows the logical name. Recovering it from the assembly is the only way a migration can
        /// carry it over into the <c>.table</c>, which becomes the source the generator reads.
        /// </remarks>
        public string OptionSetName { get; }

        public DefinitionColumnInfo(string logicalName, string name, string optionSetName = null)
        {
            LogicalName = logicalName;
            Name = name;
            OptionSetName = optionSetName;
        }
    }
}
