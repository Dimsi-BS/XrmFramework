// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class OptionSetDefinitionAttribute : Attribute
    {
        public OptionSetDefinitionAttribute(string logicalName)
        {
            LogicalName = logicalName;
        }
        public OptionSetDefinitionAttribute(string entityName, string fieldName)
        {
            EntityName = entityName;
            LogicalName = fieldName;
        }

        public string LogicalName { get; private set; }

        public string EntityName { get; private set; }
    }
}
