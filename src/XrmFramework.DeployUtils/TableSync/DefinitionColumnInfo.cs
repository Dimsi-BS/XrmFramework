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

        public DefinitionColumnInfo(string logicalName, string name)
        {
            LogicalName = logicalName;
            Name = name;
        }
    }
}
