// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Model.Sdk;

namespace Model
{
    public class ModelDefinition
    {
        public readonly IList<CrmModelAttributeDefinition> _crmAttributes = new List<CrmModelAttributeDefinition>();

        public IReadOnlyCollection<CrmModelAttributeDefinition> CrmAttributes => new ReadOnlyCollection<CrmModelAttributeDefinition>(_crmAttributes);


        public readonly IList<ModelAttributeDefinition> _extendBindingAttributes = new List<ModelAttributeDefinition>();
        public IReadOnlyCollection<ModelAttributeDefinition> ExtendBindingAttributes => new ReadOnlyCollection<ModelAttributeDefinition>(_extendBindingAttributes);


        public readonly IList<RelationshipModelAttributeDefinition> _relationshipAttributes = new List<RelationshipModelAttributeDefinition>();
        public IReadOnlyCollection<RelationshipModelAttributeDefinition> RelationshipAttributes => new ReadOnlyCollection<RelationshipModelAttributeDefinition>(_relationshipAttributes);

        public readonly IList<XmlMappingModelAttributeDefinition> _xmlMappingAttributes = new List<XmlMappingModelAttributeDefinition>();
        public IReadOnlyCollection<XmlMappingModelAttributeDefinition> XmlMappingAttributes => new ReadOnlyCollection<XmlMappingModelAttributeDefinition>(_xmlMappingAttributes);

        public readonly IList<ModelAttributeDefinition> _upsertableAttributes = new List<ModelAttributeDefinition>();
        public IReadOnlyCollection<ModelAttributeDefinition> UpsertableAttributes => new ReadOnlyCollection<ModelAttributeDefinition>(_upsertableAttributes);

        public EntityDefinition MainDefinition { get; }

        public ModelDefinition(Type bindingType)
        {
            IsBindingModel = typeof(IBindingModel).IsAssignableFrom(bindingType);

            Constructor = bindingType.GetConstructor(new Type[] { });
            if (Constructor == null)
            {
                throw new Exception("No default Constructor found");
            }

            if (IsBindingModel)
            {
                IdProperty = bindingType.GetProperty("Id");
                MainDefinition = DefinitionCache.GetEntityDefinitionFromModelType(bindingType);
            }

            foreach (var property in bindingType.GetProperties())
            {
                ModelAttributeDefinition modelAttribute = null;

                var crmAttribute = property.GetCustomAttribute<CrmMappingAttribute>();
                if (crmAttribute != null)
                {
                    modelAttribute = new CrmModelAttributeDefinition(crmAttribute, property);
                    _crmAttributes.Add((CrmModelAttributeDefinition)modelAttribute);
                }

                var extendBindingAttribute = property.GetCustomAttribute<ExtendBindingModelAttribute>();
                if (extendBindingAttribute != null)
                {
                    modelAttribute = new ModelAttributeDefinition(property);
                    _extendBindingAttributes.Add(modelAttribute);
                }

                var relationshipAttribute = property.GetCustomAttribute<CrmRelationshipAttribute>();
                if (relationshipAttribute != null)
                {
                    modelAttribute = new RelationshipModelAttributeDefinition(relationshipAttribute, property);
                    _relationshipAttributes.Add((RelationshipModelAttributeDefinition)modelAttribute);
                }

                var xmlMappingAttribute = property.GetCustomAttribute<XmlMappingAttribute>();
                if (xmlMappingAttribute != null)
                {
                    modelAttribute = new XmlMappingModelAttributeDefinition(xmlMappingAttribute, property);
                    _xmlMappingAttributes.Add((XmlMappingModelAttributeDefinition)modelAttribute);
                }

                if (modelAttribute != null && modelAttribute.IsUpsertable())
                {
                    _upsertableAttributes.Add(modelAttribute);
                }

            }

        }

        public bool IsBindingModel { get; }

        private PropertyInfo IdProperty { get; }

        private ConstructorInfo Constructor { get; }

        public object GetInstance()
        {
            return Constructor.Invoke(new object[] { });
        }

        public void SetId(object instance, Guid id)
        {
            if (instance == null)
            {
                throw new NullReferenceException("SetId cannot be called on a null instance");
            }

            IdProperty.SetValue(instance, id);
        }
    }

    public class ModelAttributeDefinition
    {
        public PropertyInfo Property { get; }

        public Type PropertyType => Property.PropertyType;

        public Type ObjectType { get; }

        public int? UpsertOrder { get; }

        public string Name => Property.Name;

        public bool IsNullable { get; }

        public bool IsExtendBindingModel { get; }

        public ModelAttributeDefinition(PropertyInfo property)
        {
            Property = property;

            ObjectType = Property.PropertyType;
            IsNullable = ObjectType.IsClass;

            var subType = Nullable.GetUnderlyingType(property.PropertyType);
            if (subType != null)
            {
                ObjectType = subType;
                IsNullable = true;
            }

            UpsertOrder = property.GetCustomAttribute<UpsertOrderAttribute>()?.Order;

            IsExtendBindingModel = property.GetCustomAttribute<ExtendBindingModelAttribute>() != null;
        }


        public void SetValue(object instance, object value)
        {
            if (Property.SetMethod != null)
            {
                Property.SetValue(instance, value);
            }
        }

        public object GetValue(object instance)
        {
            return Property.GetValue(instance);
        }

        public bool IsCollectionProperty(out Type collectionBindingType)
        {
            var retour = false;
            collectionBindingType = null;

            var types = Property.PropertyType.GenericTypeArguments;

            if (types.Length == 1)
            {
                if (typeof(ICollection<>).MakeGenericType(types).IsAssignableFrom(Property.PropertyType))
                {
                    retour = true;
                    collectionBindingType = types.Single();
                }
            }

            return retour;
        }

        public bool IsUpsertable()
        {
            Type collectionType;

            if (Property.GetCustomAttribute<ChildRelationshipAttribute>()?.IsValidForUpdate == false)
            {
                return false;
            }

            if (Property.GetCustomAttribute<CrmMappingAttribute>()?.IsValidForUpdate == false)
            {
                return false;
            }

            return Property.GetType() != typeof(ModelAttributeDefinition) && (typeof(IBindingModel).IsAssignableFrom(PropertyType) || (IsCollectionProperty(out collectionType) && typeof(IBindingModel).IsAssignableFrom(collectionType)));
        }

    }

    public class CrmModelAttributeDefinition : ModelAttributeDefinition
    {
        private readonly TypeConverter _typeConverter;

        public CrmMappingAttribute CrmMappingAttribute { get; }

        public CrmLookupAttribute CrmLookupAttribute { get; }

        public CrmModelAttributeDefinition(CrmMappingAttribute crmAttribute, PropertyInfo property) : base(property)
        {
            CrmMappingAttribute = crmAttribute;

            CrmLookupAttribute = Property.GetCustomAttribute<CrmLookupAttribute>();

            var typeConverterAttribute = Property.GetCustomAttribute<TypeConverterAttribute>();
            if (typeConverterAttribute != null)
            {
                var type1 = Type.GetType(typeConverterAttribute.ConverterTypeName);
                if (type1 != null)
                {
                    var constructorInfo = type1.GetConstructor(new Type[] { });
                    if (constructorInfo != null)
                    {
                        _typeConverter = (TypeConverter)(constructorInfo.Invoke(new object[] { }));
                    }
                }
            }

        }

        public bool HasConverter => _typeConverter != null;

        public object ConvertFrom(object initialValue)
        {
            if (!HasConverter)
            {
                return initialValue;
            }
            return _typeConverter.ConvertFrom(initialValue);
        }

        public object ConvertTo(object initialValue, Type destinationType)
        {
            if (!HasConverter)
            {
                return initialValue;
            }
            return _typeConverter.ConvertTo(initialValue, destinationType);
        }

        public override string ToString() => $"{Name} ({CrmMappingAttribute.AttributeName})";
    }

    public class RelationshipModelAttributeDefinition : ModelAttributeDefinition
    {
        public Relationship Relationship { get; }
        public CrmRelationshipAttribute RelationshipAttribute { get; }

        private readonly MethodInfo _addMethod;

        public RelationshipModelAttributeDefinition(CrmRelationshipAttribute attribute, PropertyInfo property) : base(property)
        {
            Relationship = attribute.GetRelationship();

            RelationshipAttribute = attribute;

            _addMethod = PropertyType.GetMethod("Add");
        }

        public void AddElement(object instance, object model)
        {
            _addMethod.Invoke(Property.GetValue(instance), new object[] { model });
        }
    }

    public class XmlMappingModelAttributeDefinition : ModelAttributeDefinition
    {
        public XmlMappingAttribute XmlMappingAttribute { get; }

        public XmlMappingModelAttributeDefinition(XmlMappingAttribute mappingAttribute, PropertyInfo property) : base(property)
        {
            XmlMappingAttribute = mappingAttribute;
        }
    }
}
