// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XrmFramework
{
    public static class DefinitionCache
    {
        private static readonly object _syncRoot = new object();

        private static readonly Dictionary<string, EntityDefinition> _definitionCache = new Dictionary<string, EntityDefinition>();

        private static readonly Dictionary<Type, ModelDefinition> _modelDefinitionCache = new Dictionary<Type, ModelDefinition>();

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

            if (_definitionCache.ContainsKey(entityName))
            {
                return _definitionCache[entityName];
            }

            var definition = new EntityDefinition(type);
            _definitionCache[entityName] = definition;

            return definition;
        }

        public static bool TryGetEntityDefinition(string entityName, out EntityDefinition definition)
        {
            definition = null;

            if (_definitionCache.ContainsKey(entityName))
            {
                definition = _definitionCache[entityName];
            }
            else
            {
                var definitionTypes = typeof(DefinitionCache).Assembly.GetTypes().Where(t => t.GetField("EntityName") != null).Where(t => t.GetField("EntityName").FieldType == typeof(string) && (string)t.GetField("EntityName").GetValue(null) == entityName);

                var definitionType = definitionTypes.OrderBy(t => t.Namespace?.Contains("XrmFramework.Common") ?? false).FirstOrDefault();

                if (definitionType == null)
                {
                    return false;
                }

                definition = new EntityDefinition(definitionType);
                _definitionCache[entityName] = definition;
            }

            return true;
        }

        public static EntityDefinition GetEntityDefinitionFromModelType(Type type)
        {
            var crmEntityAttribute = type.GetCustomAttribute<CrmEntityAttribute>();

            if (crmEntityAttribute == null)
            {
                throw new Exception($"Type {type.Name} does not have a CrmEntityAttribute defined.");
            }
            return GetEntityDefinition(crmEntityAttribute.EntityName);
        }



        public static bool TryGetModelDefinition(Type type, out ModelDefinition modelDefinition)
        {
            modelDefinition = null;

            if (!_modelDefinitionCache.ContainsKey(type))
            {
                lock (_syncRoot)
                {
                    if (!_modelDefinitionCache.ContainsKey(type))
                    {
                        try
                        {
                            _modelDefinitionCache[type] = new ModelDefinition(type);
                        }
                        catch
                        {

                            return false;
                        }
                    }
                }
            }

            modelDefinition = _modelDefinitionCache[type];
            return true;

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
