using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XrmFramework.BindingModel;

namespace XrmFramework.Utils
{
    internal static class ModelFactory
    {
        public static ModelDefinition CreateFromType(Type bindingType)
        {
            var modelDefinition = new ModelDefinition();
            modelDefinition.TypeFullName = bindingType.FullName;
            modelDefinition.BindingType = bindingType;
            modelDefinition.ImplementedInterfaces = bindingType.GetInterfaces();

            modelDefinition.IsBindingModel = typeof(IBindingModel).IsAssignableFrom(bindingType);

            modelDefinition.Constructor = bindingType.GetConstructor(new Type[] { });
            if (modelDefinition.Constructor == null)
            {
                throw new Exception("No default Constructor found");
            }

            if (modelDefinition.IsBindingModel)
            {
                modelDefinition.IdProperty = bindingType.GetProperty("Id");
                modelDefinition.MainDefinition = DefinitionCache.GetEntityDefinitionFromModelType(bindingType);
            }

            modelDefinition.XmlMappingAttribute = bindingType.GetCustomAttribute<XmlMappingAttribute>() ??
                                  modelDefinition.ImplementedInterfaces.Where(type => type.GetCustomAttribute<XmlMappingAttribute>() != null).Select(type => type.GetCustomAttribute<XmlMappingAttribute>()).FirstOrDefault();

            foreach (var property in bindingType.GetProperties())
            {
                var attribute = GetAttributeDefinition(modelDefinition, property);

                modelDefinition._attributes.Add(attribute);
            }


            return modelDefinition;
        }



        public static Object GetInstanceFromModel(ModelDefinition model)
        {
            return model.Constructor.Invoke(new object[] { });

        }

        public static void AddAttribute(ModelDefinition model, AttributeDefinition attribute)
        {
            model._attributes.Add(attribute);
        }

        public static void SetId(ModelDefinition model, object instance, Guid id)
        {
            if (instance == null)
            {
                throw new NullReferenceException("SetId cannot be called on a null instance");
            }

            model.IdProperty.SetValue(instance, id);
        }

        public static string GetName(ModelDefinition model)
        {
            return model.TypeFullName;
        }

        private static void InitAttribute(AttributeDefinition attribute, ModelDefinition model, PropertyInfo property)
        {
            attribute.Model = model;
            attribute.Property = property;

            attribute.CrmMappingAttribute = GetAttribute<CrmMappingAttribute>(attribute, property);


            attribute.CrmLookupAttribute = GetAttribute<CrmLookupAttribute>(attribute, property);

            attribute.ModelImplementationAttribute = GetAttribute<CrmModelImplementationAttribute>(attribute, property);

            var extendBindingModel = GetAttribute<ExtendBindingModelAttribute>(attribute, property);

            attribute.IsExtendBindingModel = extendBindingModel != null;

            attribute.ObjectType = attribute.Property.PropertyType;
            attribute.IsNullable = attribute.ObjectType.IsClass;

            var subType = Nullable.GetUnderlyingType(property.PropertyType);
            if (subType != null)
            {
                attribute.ObjectType = subType;
                attribute.IsNullable = true;
            }

            attribute.UpsertOrder = GetAttribute<UpsertOrderAttribute>(attribute, property)?.Order;

            var converterAttribute = GetAttribute<ModelPropertyConverterAttribute>(attribute, property);

            if (converterAttribute != null)
            {
                try
                {
                    attribute._typeConverter = (ModelPropertyConverter)Activator.CreateInstance(converterAttribute.ConverterType, converterAttribute.ConstructorParameters);
                }
                catch (Exception)
                {
                    // erreur d'initialisation du converter
                }
            }

            attribute.XmlMappingAttribute = GetAttribute<XmlMappingAttribute>(attribute, property);

            attribute.RelationshipAttribute = GetAttribute<CrmRelationshipAttribute>(attribute, property);

            var addMethods = attribute.PropertyType.GetMethods().Where(m => m.Name == "Add").ToList();
            if (addMethods.Count == 1)
            {
                attribute._addMethod = addMethods.Single();
            }
        }

        private static T GetAttribute<T>(AttributeDefinition attribute, PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>(true)
                   ?? attribute.Model.ImplementedInterfaces.FirstOrDefault(t => t.GetProperty(property.Name)?.GetCustomAttribute<T>() != null)?.GetProperty(property.Name)?.GetCustomAttribute<T>();
        }


        public static AttributeDefinition GetAttributeDefinition(ModelDefinition model, PropertyInfo property)
        {
            var attribute = new AttributeDefinition();

            InitAttribute(attribute, model, property);
            return attribute;
        }

        public static void SetAttributeValue(AttributeDefinition attribute, object instance, object value)
        {
            if (attribute.Property.SetMethod != null)
            {
                attribute.Property.SetValue(instance, value);
            }
            else if (attribute.PropertyType.GenericTypeArguments.Any())
            {
                if (typeof(ICollection<>).MakeGenericType(attribute.PropertyType.GenericTypeArguments).IsAssignableFrom(attribute.PropertyType)
                    && value is System.Collections.IEnumerable enumValue)
                {
                    var list = attribute.Property.GetValue(instance);
                    var addMethod = list.GetType().GetMethod("Add");

                    foreach (var v in enumValue)
                    {
                        addMethod?.Invoke(list, new[] { v });
                    }
                }
            }
        }

        public static string GetAttributeName(AttributeDefinition attribute)
        {
            return attribute.Name;
        }

        public static object GetAttributeValue(AttributeDefinition attribute, object instance)
        {
            return attribute.Property.GetValue(instance);
        }

        public static bool IsAttributeCollectionProperty(AttributeDefinition attribute, out Type collectionBindingType)
        {
            var retour = false;
            collectionBindingType = null;

            var types = attribute.Property.PropertyType.GenericTypeArguments;

            if (types.Length == 1)
            {
                if (typeof(ICollection<>).MakeGenericType(types).IsAssignableFrom(attribute.Property.PropertyType))
                {
                    retour = true;
                    collectionBindingType = types.Single();
                }
            }

            return retour;
        }

        public static object ConvertAttributeFrom(AttributeDefinition attribute, object initialValue)
        {
            if (!attribute.HasConverter)
            {
                return initialValue;
            }
            return attribute._typeConverter.ConvertFrom(initialValue);
        }

        public static void AddElementToAttribute(AttributeDefinition attribute, object instance, object model)
        {
            attribute._addMethod.Invoke(attribute.Property.GetValue(instance), new[] { model });
        }


    }
}
