using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using XrmFramework.BindingModel;

namespace XrmFramework
{
    [JsonObject(MemberSerialization.OptIn)]
    [JsonArray]
    public class ModelDefinition
    {
        public Type BindingType { get; set; }
        [JsonProperty]
        public string TypeFullName { get; set; }

        public Type[] ImplementedInterfaces { get; set; }
        [JsonProperty]
        public readonly IList<AttributeDefinition> _attributes = new List<AttributeDefinition>();
        public IReadOnlyCollection<AttributeDefinition> CrmAttributes => new ReadOnlyCollection<AttributeDefinition>(_attributes.Where(a => a.CrmMappingAttribute != null).ToList());
        public IReadOnlyCollection<AttributeDefinition> ExtendBindingAttributes => new ReadOnlyCollection<AttributeDefinition>(_attributes.Where(a => a.IsExtendBindingModel).ToList());
        public IReadOnlyCollection<AttributeDefinition> RelationshipAttributes => new ReadOnlyCollection<AttributeDefinition>(_attributes.Where(a => a.RelationshipAttribute != null).ToList());
        public IReadOnlyCollection<AttributeDefinition> XmlMappingAttributes => new ReadOnlyCollection<AttributeDefinition>(_attributes.Where(a => a.XmlMappingAttribute != null).ToList());
        public IReadOnlyCollection<AttributeDefinition> UpsertableAttributes => new ReadOnlyCollection<AttributeDefinition>(_attributes.Where(a => a.IsUpsertable()).ToList());

        [JsonProperty]
        public EntityDefinition MainDefinition { get; set; }

        public XmlMappingAttribute XmlMappingAttribute { get; set; }

        public ModelDefinition(Type bindingType)
        {
            /*
            BindingType = bindingType;
            ImplementedInterfaces = bindingType.GetInterfaces();

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

            XmlMappingAttribute = bindingType.GetCustomAttribute<XmlMappingAttribute>() ??
                                  ImplementedInterfaces.Where(type => type.GetCustomAttribute<XmlMappingAttribute>() != null).Select(type => type.GetCustomAttribute<XmlMappingAttribute>()).FirstOrDefault();

            foreach (var property in bindingType.GetProperties())
            {
                var attribute = AttributeDefinition.GetDefinition(this, property);

                _attributes.Add(attribute);
            }
            */
        }

        [JsonConstructor]
        public ModelDefinition()
        {

        }



        /*public void AddAttribute(AttributeDefinition attribute)
        {
            _attributes.Add(attribute);
        }*/

        public bool IsBindingModel { get; set; }

        public PropertyInfo IdProperty { get; set; }

        public ConstructorInfo Constructor { get; set; }

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

        public override string ToString()
        {
            return BindingType.Name;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class AttributeDefinition
    {
        public ModelDefinition Model { get; internal set; }

        public PropertyInfo Property { get; internal set; }

        public Type PropertyType => ModelImplementationAttribute?.ImplementationType ?? Property.PropertyType;

        public Type ObjectType { get; internal set; }

        public int? UpsertOrder { get; internal set; }
        [JsonProperty]
        public string Name => Property.Name;
        [JsonProperty]
        public bool IsNullable { get; internal set; }

        public bool IsExtendBindingModel { get; internal set; }

        public CrmModelImplementationAttribute ModelImplementationAttribute { get; internal set; }

        public CrmMappingAttribute CrmMappingAttribute { get; internal set; }

        public CrmLookupAttribute CrmLookupAttribute { get; internal set; }
        internal AttributeDefinition()
        {

        }

        /*private void InitAttribute(ModelDefinition model, PropertyInfo property)
        {
            Model = model;
            Property = property;

            CrmMappingAttribute = GetAttribute<CrmMappingAttribute>(property);


            CrmLookupAttribute = GetAttribute<CrmLookupAttribute>(property);

            ModelImplementationAttribute = GetAttribute<CrmModelImplementationAttribute>(property);

            var extendBindingModel = GetAttribute<ExtendBindingModelAttribute>(property);

            IsExtendBindingModel = extendBindingModel != null;

            ObjectType = Property.PropertyType;
            IsNullable = ObjectType.IsClass;

            var subType = Nullable.GetUnderlyingType(property.PropertyType);
            if (subType != null)
            {
                ObjectType = subType;
                IsNullable = true;
            }

            UpsertOrder = GetAttribute<UpsertOrderAttribute>(property)?.Order;

            var converterAttribute = GetAttribute<ModelPropertyConverterAttribute>(property);

            if (converterAttribute != null)
            {
                try
                {
                    _typeConverter = (ModelPropertyConverter) Activator.CreateInstance(converterAttribute.ConverterType, converterAttribute.ConstructorParameters);
                }
                catch (Exception)
                {
                    // erreur d'initialisation du converter
                }
            }

            XmlMappingAttribute = GetAttribute<XmlMappingAttribute>(property);

            RelationshipAttribute = GetAttribute<CrmRelationshipAttribute>(property);

            var addMethods = PropertyType.GetMethods().Where(m => m.Name == "Add").ToList();
            if (addMethods.Count == 1)
            {
                _addMethod = addMethods.Single();
            }
        }*/

        /*public static AttributeDefinition GetDefinition(ModelDefinition model, PropertyInfo property)
        {
            var attribute = new AttributeDefinition();

            attribute.InitAttribute(model, property);
            return attribute;
        }*/

        /*private T GetAttribute<T>(PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>(true)
                   ?? Model.ImplementedInterfaces.FirstOrDefault(t => t.GetProperty(property.Name)?.GetCustomAttribute<T>() != null)?.GetProperty(property.Name)?.GetCustomAttribute<T>();
        }*/


        public void SetValue(object instance, object value)
        {
            if (Property.SetMethod != null)
            {
                Property.SetValue(instance, value);
            }
            else if (PropertyType.GenericTypeArguments.Any())
            {
                if (typeof(ICollection<>).MakeGenericType(PropertyType.GenericTypeArguments).IsAssignableFrom(PropertyType)
                    && value is IEnumerable enumValue)
                {
                    var list = Property.GetValue(instance);
                    var addMethod = list.GetType().GetMethod("Add");

                    foreach (var v in enumValue)
                    {
                        addMethod?.Invoke(list, new[] { v });
                    }
                }
            }
        }

        /*public override string ToString()
        {
            return $"{PropertyType.Name} {Property.Name}";
        }*/

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

        public bool HasConverter => _typeConverter != null;

        public object ConvertFrom(object initialValue)
        {
            if (!HasConverter)
            {
                return initialValue;
            }
            return _typeConverter.ConvertFrom(initialValue);
        }

        public bool IsUpsertable()
        {
            //IsCollectionProperty(out collectionType)
            Type collectionType;
            return !IsExtendBindingModel && ((CrmMappingAttribute != null && typeof(IBindingModel).IsAssignableFrom(PropertyType)) || (RelationshipAttribute != null && IsCollectionProperty(out collectionType) && typeof(IBindingModel).IsAssignableFrom(collectionType)));
        }

        internal ModelPropertyConverter _typeConverter;

        public Relationship Relationship => RelationshipAttribute?.GetRelationship();

        public CrmRelationshipAttribute RelationshipAttribute { get; internal set; }

        public bool IsBindingModel => typeof(IBindingModel).IsAssignableFrom(PropertyType);

        public ModelDefinition TargettedModelDefinition => DefinitionCache.GetModelDefinition(PropertyType);

        internal MethodInfo _addMethod;

        public void AddElement(object instance, object model)
        {
            _addMethod.Invoke(Property.GetValue(instance), new[] { model });
        }

        public XmlMappingAttribute XmlMappingAttribute { get; set; }
    }
}
