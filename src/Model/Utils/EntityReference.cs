// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Sdk
{
    public class EntityReference
    {
        public EntityReference(string logicalName)
        {
            LogicalName = logicalName;
        }

        public EntityReference(string logicalName, Guid id) : this(logicalName)
        {
            Id = id;
        }

        public EntityReference(string logicalName, string keyAttributeName, object value) : this(logicalName)
        {
            KeyAttributes.Add(keyAttributeName, value);
        }

        public Guid Id { get; set; }

        public string LogicalName { get; set; }

        public IDictionary<string, object> KeyAttributes { get; } = new Dictionary<string, object>();

        public string UriString
        {
            get
            {
                var definition = DefinitionCache.GetEntityDefinition(LogicalName);

                return $"{definition.EntityCollectionName}({IdString})";
            }
        }

        public string IdString
        {
            get
            {
                if (Id != Guid.Empty)
                {
                    return Id.ToString();
                }
                
                var sb = new StringBuilder();

                foreach (var key in KeyAttributes.Keys)
                {
                    sb.Append($"{key}=");

                    var value = KeyAttributes[key];

                    if (value is string)
                    {
                        sb.Append($"'{value}'");
                    }
                    else
                    {
                        sb.Append(value);
                    }

                    sb.Append(",");
                }

                sb.Remove(sb.Length - 1, 1);

                return sb.ToString();
            }
        }

        public string Name { get; set; }
    }
}
