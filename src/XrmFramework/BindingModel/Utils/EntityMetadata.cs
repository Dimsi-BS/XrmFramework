// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;

namespace XrmFramework.BindingModel
{
    internal class EntityMetadata
    {
        private static readonly object SyncRoot = new object();

        private static readonly Dictionary<Type, EntityMetadata> MetadataCache = new();

        public EntityDefinition EntityDefinition { get; }

        private Type BindingType { get; }

        public string TypeFullName { get; }

        public static EntityMetadata GetMetadata(Type type)
        {
            lock (SyncRoot)
            {
                if (!MetadataCache.ContainsKey(type))
                {
                    MetadataCache.Add(type, new EntityMetadata(type));
                }
                return MetadataCache[type];
            }
        }

        private EntityMetadata(Type type)
        {
            BindingType = type;
            EntityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(type);
            TypeFullName = type.FullName;

            var crmEntityAttribute = type.GetCustomAttribute<CrmEntityAttribute>();
            
            IsValidForCreate = crmEntityAttribute?.ValidForCreate ?? false;
            AllowDeactivation = crmEntityAttribute?.AllowDeactivation ?? false;

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
