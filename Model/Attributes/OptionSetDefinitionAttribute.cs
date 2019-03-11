using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
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
