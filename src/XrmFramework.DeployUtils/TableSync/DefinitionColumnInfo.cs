// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// One member of an option set enum, as it is declared in the analyzed assembly.
    /// </summary>
    public sealed class DefinitionOptionSetValue
    {
        /// <summary>Numeric value of the member — the key the CRM option is matched on.</summary>
        public int Value { get; }

        /// <summary>C# name of the member, e.g. "EnCours".</summary>
        public string Name { get; }

        public DefinitionOptionSetValue(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }

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
        /// (e.g. "RunAsUser"), or <see langword="null" /> if the column carries no option set.
        /// </summary>
        /// <remarks>
        /// This is an identifier the project's code depends on, and teams rename it freely — the CRM
        /// only knows the logical name. Recovering it from the assembly is the only way a migration can
        /// carry it over into the <c>.table</c>, which becomes the source the generator reads.
        /// </remarks>
        public string OptionSetName { get; }

        /// <summary>
        /// Members of that enum, in declaration order. Empty when the column carries no option set.
        /// </summary>
        /// <remarks>
        /// Same reasoning as <see cref="OptionSetName" />, one level down: the generator derives a
        /// member name from the CRM label (<c>Modèle</c> becomes <c>Modele</c>), but teams rename
        /// them, and every <c>MyEnum.EnCours</c> in the project's code depends on the result. The
        /// numeric value is the stable key the CRM option is matched on.
        /// </remarks>
        public IReadOnlyList<DefinitionOptionSetValue> OptionSetValues { get; }

        public DefinitionColumnInfo(string logicalName, string name, string optionSetName = null,
                                    IReadOnlyList<DefinitionOptionSetValue> optionSetValues = null)
        {
            LogicalName = logicalName;
            Name = name;
            OptionSetName = optionSetName;
            OptionSetValues = optionSetValues ?? new List<DefinitionOptionSetValue>();
        }
    }
}
