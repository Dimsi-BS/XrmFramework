// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XrmFramework.Utils;

namespace XrmFramework
{
    internal static class DefinitionCache
    {
        private static readonly ConcurrentDictionary<string, EntityDefinition> InternalDefinitionCache = new ConcurrentDictionary<string, EntityDefinition>();

        private static readonly ConcurrentDictionary<Type, ModelDefinition> InternalModelDefinitionCache = new ConcurrentDictionary<Type, ModelDefinition>();

        public static EntityDefinition GetEntityDefinition(string entityName)
        {
            if (TryGetEntityDefinition(entityName, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"No definition found for entity {entityName}");
        }

        public static EntityDefinition GetEntityDefinition(Type type)
        {
            var entityDefinitionAttribute = type.GetCustomAttribute<EntityDefinitionAttribute>();

            if (entityDefinitionAttribute == null)
            {
                throw new Exception($"Type {type.Name} does not have a EntityDefinitionAttribute defined.");
            }

            var entityName = type.GetField("EntityName")?.GetValue(null) as string;

            if (string.IsNullOrEmpty(entityName))
            {
                throw new Exception($"Type {type.Name} does not have a proper EntityName const field defined.");
            }

            if (InternalDefinitionCache.ContainsKey(entityName))
            {
                return InternalDefinitionCache[entityName];
            }

            var definition = new EntityDefinition(type);
            InternalDefinitionCache[entityName] = definition;

            return definition;
        }

        public static bool TryGetEntityDefinition(string entityName, out EntityDefinition definition)
        {
            definition = InternalDefinitionCache.GetOrAdd(entityName, (name) =>
            {
                var definitionTypes = typeof(DefinitionCache)
                    .Assembly
                    .GetTypes()
                    .Where(t => t.GetCustomAttribute<EntityDefinitionAttribute>() != null)
                    .Where(t => t.GetField("EntityName") != null)
                    .Where(t =>
                        t.GetField("EntityName").FieldType == typeof(string)
                        && (string)t.GetField("EntityName").GetValue(null) == name);

                var definitionType = definitionTypes.OrderBy(t => t.Namespace?.Contains("XrmFramework") ?? false).FirstOrDefault();

                if (definitionType == default)
                {
                    return null;
                }

                return new EntityDefinition(definitionType);
            });

            return definition != null;
        }

        public static EntityDefinition GetEntityDefinitionFromModelType(Type type)
        {
            var crmEntityAttribute = type.GetCustomAttribute<CrmEntityAttribute>(true);

            if (crmEntityAttribute == null)
            {
                var interfaceType = type.GetInterfaces()
                    .FirstOrDefault(t => CustomAttributeExtensions.GetCustomAttribute<CrmEntityAttribute>((MemberInfo)t, true) != null);

                if (interfaceType != null)
                {
                    crmEntityAttribute = interfaceType.GetCustomAttribute<CrmEntityAttribute>(true);
                }
            }

            if (crmEntityAttribute == null)
            {
                throw new Exception($"Type {type.Name} does not have a CrmEntityAttribute defined.");
            }
            return GetEntityDefinition(crmEntityAttribute.EntityName);
        }



        public static bool TryGetModelDefinition(Type type, out ModelDefinition modelDefinition)
        {
            modelDefinition = InternalModelDefinitionCache
                .GetOrAdd(type, t =>
                {
                    try
                    {
                        return ModelFactory.CreateFromType(t); // new ModelDefinition(t);
                    }
                    catch
                    {
                        return null;
                    }
                });

            return modelDefinition != null;
        }

        public static ModelDefinition GetModelDefinition(Type type)
        {
            if (TryGetModelDefinition(type, out var modelDefinition))
            {
                return modelDefinition;
            }
            throw new KeyNotFoundException($"No model definition found for type {type.FullName}");
        }
    }
}
