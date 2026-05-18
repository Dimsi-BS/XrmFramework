// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace XrmFramework.BindingModel
{
    /// <summary>
    /// Converts between <see cref="XElement"/> and <see cref="IXmlModel"/> binding models.
    /// </summary>
    internal static class XmlBindingModelMapper
    {
        // ---------------------------------------------------------------------
        // Deserialization: XElement -> IXmlModel
        // ---------------------------------------------------------------------

        public static object FromXElement(XElement element, Type type)
        {
            if (element == null)
            {
                return null;
            }

            var modelDefinition = DefinitionCache.GetModelDefinition(type);
            var bindingModel = modelDefinition.GetInstance();

            var rootElement = ResolveRootElement(element, modelDefinition);

            // We use a queue because some properties depend on parent values (CopyFromParent) and
            // may need to be retried after others are initialised.
            var pending = new Queue<AttributeDefinition>(modelDefinition.XmlMappingAttributes);
            var initializedProperties = new HashSet<string>();

            while (pending.Count > 0)
            {
                var property = pending.Dequeue();
                var xmlAttribute = property.XmlMappingAttribute;

                var propElement = ResolvePropertyNode(rootElement, xmlAttribute);
                if (propElement == null)
                {
                    initializedProperties.Add(property.Name);
                    continue;
                }

                var outcome = TryAssignFromXml(type, bindingModel, property, xmlAttribute, propElement, initializedProperties);

                switch (outcome)
                {
                    case AssignOutcome.Assigned:
                        initializedProperties.Add(property.Name);
                        break;
                    case AssignOutcome.Deferred:
                        pending.Enqueue(property);
                        break;
                    case AssignOutcome.Skipped:
                        // Matches original behaviour: invalid collection properties are silently dropped
                        // without being marked initialized.
                        break;
                }
            }

            ApplyPostXmlTransform(type, bindingModel);
            return bindingModel;
        }

        private static XElement ResolveRootElement(XElement element, ModelDefinition modelDefinition)
        {
            var rootElementName = modelDefinition.XmlMappingAttribute?.RelativePath;
            if (rootElementName == null)
            {
                return element;
            }

            var path = rootElementName.Split('/');
            for (var i = 1; i < path.Length; i++)
            {
                element = element.Element(path[i]);
                if (element == null)
                {
                    return null;
                }
            }

            return element;
        }

        /// <summary>Returns either an <see cref="XAttribute"/> or an <see cref="XElement"/> matching the mapping's relative path.</summary>
        private static dynamic ResolvePropertyNode(XElement element, XmlMappingAttribute xmlAttribute)
        {
            if (element == null)
            {
                return null;
            }

            dynamic node;
            if (xmlAttribute.IsAttribute)
            {
                node = element.Attribute(xmlAttribute.RelativePath);
            }
            else if (string.IsNullOrEmpty(xmlAttribute.RelativePath))
            {
                node = element;
            }
            else
            {
                node = element.Element(xmlAttribute.RelativePath);
            }

            if (!string.IsNullOrEmpty(xmlAttribute.AlternateRelativePath) && string.IsNullOrEmpty(node?.Value))
            {
                node = element.Element(xmlAttribute.AlternateRelativePath);
            }

            return node;
        }

        private enum AssignOutcome { Assigned, Deferred, Skipped }

        /// <summary>Assigns a single property from XML, returning how the outer queue should treat it.</summary>
        private static AssignOutcome TryAssignFromXml(
            Type parentType,
            object bindingModel,
            AttributeDefinition property,
            XmlMappingAttribute xmlAttribute,
            dynamic propElement,
            HashSet<string> initializedProperties)
        {
            if (xmlAttribute.ConverterType != null)
            {
                ApplyXmlConverter(bindingModel, property, xmlAttribute, propElement);
                return AssignOutcome.Assigned;
            }

            if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                return TryAssignCollection(parentType, bindingModel, property, propElement, initializedProperties);
            }

            if (typeof(IXmlModel).IsAssignableFrom(property.PropertyType))
            {
                var nested = FromXElement((XElement)propElement, property.PropertyType);
                property.SetValue(bindingModel, nested);
                return AssignOutcome.Assigned;
            }

            property.SetValue(bindingModel, ParseScalar(property, (string)propElement.Value));
            return AssignOutcome.Assigned;
        }

        private static void ApplyXmlConverter(object bindingModel, AttributeDefinition property, XmlMappingAttribute xmlAttribute, dynamic propElement)
        {
            var converter = (IXmlConverter)xmlAttribute.ConverterType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());

            XElement target = xmlAttribute.IsAttribute
                ? new XElement((XName)propElement.Name, (string)propElement.Value)
                : (XElement)propElement;

            property.SetValue(bindingModel, converter.ConvertFromXElement(target));
        }

        private static AssignOutcome TryAssignCollection(
            Type parentType,
            object bindingModel,
            AttributeDefinition property,
            dynamic propElement,
            HashSet<string> initializedProperties)
        {
            var types = property.PropertyType.GenericTypeArguments;
            if (types.Length != 1)
            {
                return AssignOutcome.Skipped;
            }

            if (!typeof(ICollection<>).MakeGenericType(types).IsAssignableFrom(property.PropertyType))
            {
                return AssignOutcome.Skipped;
            }

            var bindingType = types[0];
            var childXmlAttribute = bindingType.GetCustomAttribute<XmlMappingAttribute>();

            if (bindingType != typeof(string) && (!typeof(IXmlModel).IsAssignableFrom(bindingType) || childXmlAttribute == null))
            {
                return AssignOutcome.Skipped;
            }

            // If the child elements carry CopyFromParent values that haven't been filled yet on the parent,
            // defer this property so the parent's values exist by the time we materialize the children.
            var referencedParentProperties = GetReferencedParentProperties(parentType, bindingType);
            referencedParentProperties.ExceptWith(initializedProperties);

            if (referencedParentProperties.Count > 0)
            {
                return AssignOutcome.Deferred;
            }

            var list = property.GetValue(bindingModel);
            var addMethod = list.GetType().GetMethod("Add");

            if (bindingType == typeof(string))
            {
                foreach (XElement el in propElement.Elements())
                {
                    addMethod.Invoke(list, new object[] { el.Value });
                }
                return AssignOutcome.Assigned;
            }

            IEnumerable<XElement> elements = propElement.Elements(childXmlAttribute.RelativePath);
            if (!elements.Any())
            {
                elements = propElement.Elements(childXmlAttribute.AlternateRelativePath);
            }

            foreach (var el in elements)
            {
                var child = FromXElement(el, bindingType);
                CopyParentValues(parentType, bindingModel, bindingType, child);
                addMethod.Invoke(list, new[] { child });
            }

            return AssignOutcome.Assigned;
        }

        private static void CopyParentValues(Type parentType, object parentBindingModel, Type childType, object childBindingModel)
        {
            foreach (var prop in childType.GetProperties())
            {
                var copyFromParents = prop.GetCustomAttributes<CopyFromParentAttribute>();
                foreach (var copyFromParent in copyFromParents)
                {
                    if (copyFromParent.ParentType != parentType)
                    {
                        continue;
                    }

                    var parentProperty = parentType.GetProperty(copyFromParent.ParentPropertyName);
                    if (parentProperty == null)
                    {
                        continue;
                    }

                    prop.SetValue(childBindingModel, parentProperty.GetValue(parentBindingModel));
                }
            }
        }

        private static ISet<string> GetReferencedParentProperties(Type parentType, Type modelType)
        {
            var list = new HashSet<string>();

            foreach (var property in modelType.GetProperties())
            {
                foreach (var copyFromParent in property.GetCustomAttributes<CopyFromParentAttribute>())
                {
                    if (copyFromParent != null && copyFromParent.ParentType == parentType)
                    {
                        list.Add(copyFromParent.ParentPropertyName);
                    }
                }
            }

            return list;
        }

        private static object ParseScalar(AttributeDefinition property, string rawValue)
        {
            var objectType = property.ObjectType;

            switch (objectType.Name)
            {
                case "Int32":
                    return int.TryParse(rawValue, out var intValue) ? (object)intValue : null;

                case "Decimal":
                    if (string.IsNullOrEmpty(rawValue))
                    {
                        return property.IsNullable ? null : (object)default(decimal);
                    }
                    // The original format detection: if the value contains a '.', treat it as en-US (dot decimal),
                    // otherwise assume fr-FR (comma decimal).
                    return rawValue.Contains(".")
                        ? decimal.Parse(rawValue, CultureInfo.GetCultureInfo("en-US"))
                        : decimal.Parse(rawValue, CultureInfo.GetCultureInfo("fr-FR"));

                case "Boolean":
                    if (property.IsNullable)
                    {
                        return string.IsNullOrEmpty(rawValue) ? (bool?)null : bool.Parse(rawValue);
                    }
                    return bool.Parse(rawValue);

                case "DateTime":
                    if (string.IsNullOrEmpty(rawValue))
                    {
                        return null;
                    }

                    if (DateTime.TryParseExact(rawValue, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsedDate))
                    {
                        return parsedDate;
                    }
                    return DateTime.Parse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);

                case "Guid":
                    if (Guid.TryParse(rawValue, out var guidValue))
                    {
                        return guidValue;
                    }
                    if (string.IsNullOrEmpty(rawValue))
                    {
                        return Guid.Empty;
                    }
                    throw new Exception($"Field {property.Name} should contain a Guid value.");

                default:
                    if (objectType.IsEnum && int.TryParse(rawValue, out var enumInt))
                    {
                        return enumInt;
                    }
                    if (objectType.IsEnum)
                    {
                        return null;
                    }
                    return rawValue;
            }
        }

        private static void ApplyPostXmlTransform(Type type, object bindingModel)
        {
            var transformAttribute = type.GetCustomAttribute<XmlTransformAttribute>();
            var constructor = transformAttribute?.ActionType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                return;
            }

            var transform = (IXmlTransform)constructor.Invoke(Array.Empty<object>());
            transform.PostXmlConvertion(type, bindingModel);
        }

        // ---------------------------------------------------------------------
        // Serialization: IXmlModel -> XElement
        // ---------------------------------------------------------------------

        public static XElement ToXElement(Type type, object bindingModel)
        {
            var modelDefinition = DefinitionCache.GetModelDefinition(type);
            ApplyPreXmlTransform(type, bindingModel);

            var elementName = type.GetCustomAttribute<XmlMappingAttribute>()?.RelativePath ?? "data";
            var (root, leaf) = BuildRootChain(elementName);

            foreach (var property in modelDefinition.XmlMappingAttributes)
            {
                WriteProperty(property, bindingModel, leaf);
            }

            return root;
        }

        private static void ApplyPreXmlTransform(Type type, object bindingModel)
        {
            var transformAttribute = type.GetCustomAttribute<XmlTransformAttribute>();
            var constructor = transformAttribute?.ActionType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                return;
            }

            var transform = (IXmlTransform)constructor.Invoke(Array.Empty<object>());
            transform.PreXmlConvertion(type, bindingModel);
        }

        private static (XElement root, XElement leaf) BuildRootChain(string elementName)
        {
            if (!elementName.Contains('/'))
            {
                var root = new XElement(elementName);
                return (root, root);
            }

            var path = elementName.Split('/');
            var top = new XElement(path[0]);
            var current = top;
            for (var i = 1; i < path.Length; i++)
            {
                var next = new XElement(path[i]);
                current.Add(next);
                current = next;
            }
            return (top, current);
        }

        private static void WriteProperty(AttributeDefinition property, object bindingModel, XElement parent)
        {
            var xmlAttribute = property.XmlMappingAttribute;
            var element = new XElement(xmlAttribute.RelativePath);
            parent.Add(element);

            var propertyValue = property.GetValue(bindingModel);

            if (xmlAttribute.ConverterType != null)
            {
                var constructor = xmlAttribute.ConverterType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    var converter = (IXmlConverter)constructor.Invoke(Array.Empty<object>());
                    converter.FillXElement(element, propertyValue);
                }
                return;
            }

            if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                WriteCollection(property, bindingModel, element);
                return;
            }

            WriteScalar(property, propertyValue, element);
        }

        private static void WriteCollection(AttributeDefinition property, object bindingModel, XElement element)
        {
            if (!property.IsCollectionProperty(out var bindingType))
            {
                return;
            }

            var childXmlAttribute = bindingType.GetCustomAttribute<XmlMappingAttribute>();
            if (!typeof(IXmlModel).IsAssignableFrom(bindingType) || childXmlAttribute == null)
            {
                return;
            }

            if (property.GetValue(bindingModel) is not IEnumerable values)
            {
                return;
            }

            foreach (var value in values)
            {
                element.Add(ToXElement(bindingType, value));
            }
        }

        private static void WriteScalar(AttributeDefinition property, object propertyValue, XElement element)
        {
            if (propertyValue == null)
            {
                return;
            }

            if (typeof(IBindingModel).IsAssignableFrom(property.PropertyType))
            {
                element.Add(ToXElement(property.PropertyType, propertyValue).Descendants());
                return;
            }

            element.Value = property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?)
                ? string.Format(CultureInfo.InvariantCulture, "{0:s}", propertyValue)
                : string.Format(CultureInfo.InvariantCulture, "{0}", propertyValue);
        }
    }
}
