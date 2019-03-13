// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class EntityMetadata
    {
        private static readonly object SyncRoot = new object();

        private static readonly Dictionary<string, EntityMetadata> MetadataCache = new Dictionary<string, EntityMetadata>();

        public EntityDefinition EntityDefinition { get; }

        private Type BindingType { get; }

        public string TypeFullName { get; }

        public static EntityMetadata GetMetadata(Type type)
        {
            lock (SyncRoot)
            {
                if (!MetadataCache.ContainsKey(type.FullName))
                {
                    MetadataCache.Add(type.FullName, new EntityMetadata(type));
                }
                return MetadataCache[type.FullName];
            }
        }

        private EntityMetadata(Type type)
        {
            BindingType = type;
            EntityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(type);
            TypeFullName = type.FullName;

            IsValidForCreate = type.GetCustomAttribute<CrmEntityAttribute>().ValidForCreate;
            AllowDeactivation = type.GetCustomAttribute<CrmEntityAttribute>().AllowDeactivation;

            IdProperty = type.GetProperty("Id");

            var attributes = new List<AttributeMetadata>();
            var extendBindingProperties = new List<PropertyInfo>();
            var crmRelationshipProperties = new List<PropertyInfo>();
            var allProperties = new List<PropertyInfo>();

            foreach (var property in type.GetProperties())
            {
                if (property.GetCustomAttribute<CrmMappingAttribute>() != null)
                {
                    attributes.Add(new AttributeMetadata(EntityDefinition, property));
                }
                else if (property.GetCustomAttributes<ExtendBindingModelAttribute>().Any())
                {
                    extendBindingProperties.Add(property);
                }
                else if (property.GetCustomAttributes<CrmRelationshipAttribute>().Any())
                {
                    crmRelationshipProperties.Add(property);
                }
                allProperties.Add(property);
            }

            CrmAttributes = attributes;
            ExtendBindingProperties = extendBindingProperties;
            CrmRelationshipProperties = crmRelationshipProperties;
            AllProperties = allProperties;

            IsBindingModelBase = typeof(BindingModelBase).IsAssignableFrom(type);

        }

        public bool IsValidForCreate { get; }

        public PropertyInfo IdProperty { get; }

        public IReadOnlyCollection<AttributeMetadata> CrmAttributes { get; }

        public bool HasPreferedKey => CrmAttributes.Any(a => a.IsPreferedKey);

        public IReadOnlyCollection<PropertyInfo> ExtendBindingProperties { get; }

        public IReadOnlyCollection<PropertyInfo> CrmRelationshipProperties { get; }

        public IReadOnlyCollection<PropertyInfo> AllProperties { get; }

        public bool IsBindingModelBase { get; }
        public bool AllowDeactivation { get; }

        public object GetNewInstance()
        {
            return Activator.CreateInstance(BindingType);
        }
    }
}
