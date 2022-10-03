// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using XrmFramework.BindingModel;

namespace XrmFramework
{
    internal class AttributeMetadata
    {
        public string AttributeName { get; private set; }

        public string PropertyName { get; private set; }

        private EntityDefinition EntityDefinition { get; set; }

        public AttributeMetadata(EntityDefinition entityDefinition, PropertyInfo property)
        {
            EntityDefinition = entityDefinition;

            CrmMapping = property.GetCustomAttribute<CrmMappingAttribute>();

            PropertyName = property.Name;

            if (CrmMapping == null)
            {
                return;
            }
            AttributeName = CrmMapping.AttributeName;

            IsKey = EntityDefinition.IsKey(AttributeName);

            IsPreferedKey = property.GetCustomAttribute<AlternateKeyAttribute>() != null;

            AttributeType = EntityDefinition.GetAttributeType(AttributeName);

            var typeConverterAttribute = property.GetCustomAttribute<TypeConverterAttribute>();
            if (typeConverterAttribute != null)
            {
                var type1 = Type.GetType(typeConverterAttribute.ConverterTypeName.Substring(0, typeConverterAttribute.ConverterTypeName.IndexOf(',')));
                if (type1 != null)
                {
                    TypeConverter = type1;
                }
            }

            ObjectType = property.PropertyType;
            if (property.PropertyType.GenericTypeArguments.Any())
            {
                var subType = property.PropertyType.GenericTypeArguments.First();
                if (typeof(Nullable<>).MakeGenericType(subType).IsAssignableFrom(ObjectType))
                {
                    ObjectType = subType;
                }
            }

            //Relationships = EntityDefinition.GetRelationshipsFromAttributeName(AttributeName).ToList();
            LookupAttribute = property.GetCustomAttribute<CrmLookupAttribute>();


            if (typeof(IBindingModel).IsAssignableFrom(property.PropertyType))
            {
                SubEntity = EntityMetadata.GetMetadata(property.PropertyType);
            }

            SetMethod = property.SetMethod;
            Property = property;

            if (AttributeType == AttributeTypeCode.DateTime)
            {
                DateTimeBehavior = EntityDefinition.GetDateTimeBehavior(AttributeName);
            }
        }

        public Type ObjectType { get; private set; }

        public AttributeTypeCode AttributeType { get; private set; }

        public Type TypeConverter { get; private set; }

        public CrmMappingAttribute CrmMapping { get; private set; }

        public bool IsKey { get; private set; }

        public bool IsPreferedKey { get; }

        public bool QueryLookupTarget
        {
            get
            {
                if (LookupAttribute != null)
                {
                    if (LookupAttribute.AllowNotExisting)
                    {
                        return true;
                    }

                    var targetDefinition = DefinitionCache.GetEntityDefinition(LookupAttribute.TargetEntityName);
                    return !targetDefinition.IsKey(LookupAttribute.AttributeName);
                }
                return false;
            }
        }

        //public ICollection<Relationship> Relationships { get; private set; }

        public CrmLookupAttribute LookupAttribute { get; private set; }

        public EntityMetadata SubEntity { get; private set; }

        public MethodInfo SetMethod { get; private set; }

        public PropertyInfo Property { get; private set; }

        public DateTimeBehavior DateTimeBehavior { get; private set; }
    }
}
