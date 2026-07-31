// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using EntityReference = Microsoft.Xrm.Sdk.EntityReference;

namespace XrmFramework.BindingModel
{
    /// <summary>
    /// Converts a Dataverse <see cref="Entity"/> (and its related/aliased data) into an <see cref="IBindingModel"/>.
    /// </summary>
    /// <remarks>
    /// This mapper is split out of <c>BindingModelHelper</c> to keep a single responsibility: deserialize a CRM
    /// entity into a strongly-typed binding model. Cycle protection for self-referencing graphs is provided by
    /// an optional <see cref="BindingCache"/> passed in from the enumerable entry point.
    /// </remarks>
    internal static class EntityToBindingModelMapper
    {
        public static IEnumerable<T> MapMany<T>(IEnumerable<Entity> entities) where T : IBindingModel
        {
            var cache = new BindingCache();
            return entities.Select(e => (T)MapInternal(e, typeof(T), cache));
        }

        public static IBindingModel Map(Entity entity, Type type)
            => MapInternal(entity, type, null);

        private static IBindingModel MapInternal(Entity entity, Type type, BindingCache cache)
        {
            if (entity == null)
            {
                return null;
            }

            var modelDefinition = DefinitionCache.GetModelDefinition(type);

            if (modelDefinition.MainDefinition.EntityName != entity.LogicalName)
            {
                return null;
            }

            // Return the cached instance (if any) before doing any work: otherwise we end up
            // re-materialising the same binding model multiple times when navigating a graph.
            if (cache != null)
            {
                var cached = cache.Find(type, entity.Id);
                if (cached != null)
                {
                    return cached;
                }
            }

            var bindingModel = modelDefinition.GetInstance();
            modelDefinition.SetId(bindingModel, entity.Id);

            if (bindingModel is IEntityModel entityModel)
            {
                entityModel.Entity = entity;
            }

            MapCrmAttributes(entity, modelDefinition, bindingModel, cache);
            MapExtendedBindings(entity, modelDefinition, bindingModel, cache);
            MapRelationships(entity, modelDefinition, bindingModel, cache);

            cache?.Add(type, entity.Id, bindingModel);

            return (IBindingModel)bindingModel;
        }

        // ---------------------------------------------------------------------
        // CRM attributes
        // ---------------------------------------------------------------------

        private static void MapCrmAttributes(Entity entity, ModelDefinition modelDefinition, object bindingModel, BindingCache cache)
        {
            var entityDefinition = modelDefinition.MainDefinition;

            foreach (var property in modelDefinition.CrmAttributes)
            {
                var crmAttribute = property.CrmMappingAttribute;
                var attributeName = crmAttribute.AttributeName;
                var isKey = entityDefinition.IsKey(attributeName);

                if (!HasValueFor(entity, attributeName, isKey))
                {
                    continue;
                }

                var value = ReadAttributeValue(entity, property, entityDefinition, attributeName, isKey, cache);
                property.SetValue(bindingModel, value);
            }
        }

        private static bool HasValueFor(Entity entity, string attributeName, bool isKey)
        {
            if (isKey && entity.KeyAttributes.ContainsKey(attributeName))
            {
                return true;
            }

            return entity.Contains(attributeName);
        }

        private static object ReadAttributeValue(
            Entity entity,
            AttributeDefinition property,
            EntityDefinition entityDefinition,
            string attributeName,
            bool isKey,
            BindingCache cache)
        {
            var crmAttribute = property.CrmMappingAttribute;

            if (property.HasConverter)
            {
                return property.ConvertFrom(entity[crmAttribute.AttributeName]);
            }

            var objectType = property.ObjectType;

            return entityDefinition.GetAttributeType(attributeName) switch
            {
                AttributeTypeCode.State
                    or AttributeTypeCode.Status
                    or AttributeTypeCode.Picklist => ReadPicklist(entity, objectType, attributeName),
                AttributeTypeCode.MultiSelectPicklist => ReadPicklistValues(entity, objectType, attributeName),
                AttributeTypeCode.Lookup
                    or AttributeTypeCode.Owner
                    or AttributeTypeCode.Customer => ReadLookup(entity, property, entityDefinition, attributeName, cache),
                AttributeTypeCode.Money => ReadMoney(entity, objectType, attributeName),
                _ => ReadDefault(entity, attributeName, isKey),
            };
        }

        private static object ReadDefault(Entity entity, string attributeName, bool isKey)
        {
            if (isKey && entity.KeyAttributes.ContainsKey(attributeName))
            {
                return entity.KeyAttributes[attributeName];
            }

            return entity[attributeName];
        }

        private static object ReadMoney(Entity entity, Type objectType, string attributeName)
        {
            if (!entity.Contains(attributeName) || entity[attributeName] == null)
            {
                return null;
            }

            var moneyValue = entity.GetAttributeValue<Money>(attributeName);

            if (objectType == typeof(decimal))
            {
                return moneyValue.Value;
            }

            if (objectType == typeof(Money))
            {
                return moneyValue;
            }

            return null;
        }

        // ---------------------------------------------------------------------
        // Lookup attributes
        // ---------------------------------------------------------------------

        private static object ReadLookup(
            Entity entity,
            AttributeDefinition property,
            EntityDefinition entityDefinition,
            string attributeName,
            BindingCache cache)
        {
            var modelLookupAttributes = entityDefinition.GetCrmLookupAttributes(attributeName);
            var objectType = property.ObjectType;
            var crmLookupAttribute = property.CrmLookupAttribute;

            object value;
            bool isEntityReference;

            if (crmLookupAttribute != null)
            {
                value = ReadValueFromLookupAttribute(entity, property, attributeName, modelLookupAttributes, out isEntityReference);
            }
            else if (typeof(IBindingModel).IsAssignableFrom(objectType))
            {
                value = ReadEmbeddedBindingModel(entity, property, attributeName, modelLookupAttributes, cache);
                isEntityReference = false;
            }
            else
            {
                value = entity.GetAttributeValue<EntityReference>(attributeName);
                isEntityReference = true;
            }

            if (isEntityReference && objectType == typeof(Guid))
            {
                return ((EntityReference)value)?.Id;
            }

            return value;
        }

        /// <summary>Reads a value pointed to by a <see cref="CrmLookupAttribute"/>, either via aliased columns or via the related entity collection.</summary>
        private static object ReadValueFromLookupAttribute(
            Entity entity,
            AttributeDefinition property,
            string attributeName,
            IEnumerable<CrmLookupAttribute> modelLookupAttributes,
            out bool isEntityReference)
        {
            isEntityReference = false;

            var crmLookupAttribute = property.CrmLookupAttribute;
            var targetEntityDefinition = DefinitionCache.GetEntityDefinition(crmLookupAttribute.TargetEntityName);
            var aliasedFieldName = $"{attributeName}.{crmLookupAttribute.AttributeName}";

            // Case 1 - the value was brought in via a link + alias in the query.
            if (entity.Contains(aliasedFieldName))
            {
                var relatedAttributeType = targetEntityDefinition.GetAttributeType(crmLookupAttribute.AttributeName);
                return ReadRelatedAttributeValue(entity, aliasedFieldName, relatedAttributeType, property.ObjectType, useAliased: true, out isEntityReference);
            }

            // Case 2 - we need the primary Name / Id: read it from the EntityReference itself.
            if (targetEntityDefinition.IsPrimaryAttribute(crmLookupAttribute.AttributeName, PrimaryAttributeType.Name))
            {
                return entity.GetAttributeValue<EntityReference>(attributeName)?.Name;
            }

            if (targetEntityDefinition.IsPrimaryAttribute(crmLookupAttribute.AttributeName, PrimaryAttributeType.Id))
            {
                return entity.GetAttributeValue<EntityReference>(attributeName)?.Id;
            }

            // Case 3 - the value is on the related entity brought back in RelatedEntities.
            var relatedEntity = FindRelatedEntity(entity, modelLookupAttributes);
            if (relatedEntity == null)
            {
                return null;
            }

            var relatedDefinition = DefinitionCache.GetEntityDefinition(relatedEntity.LogicalName);
            var attrType = relatedDefinition.GetAttributeType(crmLookupAttribute.AttributeName);
            return ReadRelatedAttributeValue(relatedEntity, crmLookupAttribute.AttributeName, attrType, property.ObjectType, useAliased: false, out isEntityReference);
        }

        private static object ReadRelatedAttributeValue(
            Entity entity,
            string attributeName,
            AttributeTypeCode attributeType,
            Type objectType,
            bool useAliased,
            out bool isEntityReference)
        {
            isEntityReference = false;

            switch (attributeType)
            {
                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                    return ReadPicklist(entity, objectType, attributeName);

                case AttributeTypeCode.MultiSelectPicklist:
                    return ReadPicklistValues(entity, objectType, attributeName);

                case AttributeTypeCode.Money:
                    return useAliased
                        ? entity.GetAliasedValue<Money>(attributeName)?.Value
                        : (object)entity.GetAttributeValue<Money>(attributeName)?.Value;

                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Customer:
                    isEntityReference = true;
                    return useAliased
                        ? entity.GetAliasedValue<EntityReference>(attributeName)
                        : (object)entity.GetAttributeValue<EntityReference>(attributeName);

                default:
                    return useAliased
                        ? entity.GetAliasedValue<object>(attributeName)
                        : entity.GetAttributeValue<object>(attributeName);
            }
        }

        private static Entity FindRelatedEntity(Entity entity, IEnumerable<CrmLookupAttribute> modelLookupAttributes)
        {
            foreach (var relationship in modelLookupAttributes)
            {
                var related = entity.RelatedEntities
                    .FirstOrDefault(r => r.Key.SchemaName == relationship.RelationshipName)
                    .Value?.Entities.FirstOrDefault();

                if (related != null)
                {
                    return related;
                }
            }

            return null;
        }

        /// <summary>Builds a nested binding model from a lookup attribute, either from aliased columns or from a related entity.</summary>
        private static IBindingModel ReadEmbeddedBindingModel(
            Entity entity,
            AttributeDefinition property,
            string attributeName,
            IEnumerable<CrmLookupAttribute> modelLookupAttributes,
            BindingCache cache)
        {
            if (!entity.Contains(attributeName) || entity[attributeName] == null)
            {
                return null;
            }

            var entityReference = entity.GetAttributeValue<EntityReference>(attributeName);

            // Start with a placeholder that just has the logical name & id; we'll either
            // populate it from aliased attributes or swap it for the RelatedEntities one.
            var embeddedEntity = new Entity(entityReference.LogicalName) { Id = entityReference.Id };

            var lookupAttributes = modelLookupAttributes as IList<CrmLookupAttribute> ?? modelLookupAttributes.ToList();
            var prefix = lookupAttributes.Count > 1
                ? $"{attributeName}__{entityReference.LogicalName}"
                : attributeName;

            var isEmbed = TryPopulateFromAliasedColumns(entity, embeddedEntity, prefix);

            if (!isEmbed)
            {
                embeddedEntity = FindRelatedEntityForProperty(entity, property, lookupAttributes) ?? embeddedEntity;
            }

            return MapInternal(embeddedEntity, property.PropertyType, cache);
        }

        private static bool TryPopulateFromAliasedColumns(Entity entity, Entity embeddedEntity, string prefix)
        {
            var isEmbed = false;
            var prefixDot = prefix + ".";

            foreach (var keyName in entity.Attributes.Keys)
            {
                if (keyName == prefix || !keyName.StartsWith(prefixDot))
                {
                    continue;
                }

                isEmbed = true;
                var newKeyName = keyName.Substring(prefixDot.Length);

                // A nested aliased column like "account.primarycontactid.fullname" must retain
                // its AliasedValue wrapper so the nested mapping pass can inspect the chain;
                // a leaf column is unwrapped to its underlying value.
                object value = newKeyName.IndexOf('.') == -1
                    ? entity.GetAttributeValue<AliasedValue>(keyName).Value
                    : entity.GetAttributeValue<AliasedValue>(keyName);

                embeddedEntity[newKeyName] = value;
            }

            return isEmbed;
        }

        private static Entity FindRelatedEntityForProperty(
            Entity entity,
            AttributeDefinition property,
            IEnumerable<CrmLookupAttribute> modelLookupAttributes)
        {
            foreach (var relationship in modelLookupAttributes)
            {
                var relatedEntity = entity.RelatedEntities
                    .FirstOrDefault(r => r.Key.SchemaName == relationship.RelationshipName)
                    .Value?.Entities.FirstOrDefault();

                if (relatedEntity != null
                    && DefinitionCache.TryGetModelDefinition(property.ObjectType, out var modelDefinitionTemp)
                    && modelDefinitionTemp.MainDefinition.EntityName == relatedEntity.LogicalName)
                {
                    return relatedEntity;
                }
            }

            return null;
        }

        // ---------------------------------------------------------------------
        // Picklists (single + multi)
        // ---------------------------------------------------------------------

        private static object ReadPicklist(Entity entity, Type objectType, string attributeName)
        {
            var optionSet = GetOptionSetValue(entity, attributeName);
            if (optionSet == null)
            {
                return null;
            }

            return objectType == typeof(int)
                ? optionSet.Value
                : Enum.ToObject(objectType, optionSet.Value);
        }

        private static object ReadPicklistValues(Entity entity, Type objectType, string attributeName)
        {
            var collection = GetOptionSetValueCollection(entity, attributeName);
            if (collection == null)
            {
                return null;
            }

            var genericType = objectType.GenericTypeArguments[0];

            return genericType == typeof(int)
                ? collection.Select(o => o.Value).ToList()
                : collection.ToEnumCollection(genericType);
        }

        private static OptionSetValue GetOptionSetValue(Entity entity, string attributeName)
        {
            if (!entity.Contains(attributeName) || entity[attributeName] == null)
            {
                return null;
            }

            return entity[attributeName] is AliasedValue
                ? entity.GetAliasedValue<OptionSetValue>(attributeName)
                : entity.GetAttributeValue<OptionSetValue>(attributeName);
        }

        private static OptionSetValueCollection GetOptionSetValueCollection(Entity entity, string attributeName)
        {
            if (!entity.Contains(attributeName) || entity[attributeName] == null)
            {
                return null;
            }

            return entity[attributeName] is AliasedValue
                ? entity.GetAliasedValue<OptionSetValueCollection>(attributeName)
                : entity.GetAttributeValue<OptionSetValueCollection>(attributeName);
        }

        // ---------------------------------------------------------------------
        // Extend + Relationship bindings
        // ---------------------------------------------------------------------

        private static void MapExtendedBindings(Entity entity, ModelDefinition modelDefinition, object bindingModel, BindingCache cache)
        {
            foreach (var property in modelDefinition.ExtendBindingAttributes)
            {
                property.SetValue(bindingModel, MapInternal(entity, property.PropertyType, cache));
            }
        }

        private static void MapRelationships(Entity entity, ModelDefinition modelDefinition, object bindingModel, BindingCache cache)
        {
            foreach (var property in modelDefinition.RelationshipAttributes)
            {
                if (!property.IsCollectionProperty(out var bindingType))
                {
                    continue;
                }

                var schemaName = property.Relationship.SchemaName;
                var relatedPair = entity.RelatedEntities.FirstOrDefault(r => r.Key.SchemaName == schemaName);
                if (relatedPair.Key == null)
                {
                    continue;
                }

                var entityList = relatedPair.Value ?? new EntityCollection();

                foreach (var relatedEntity in entityList.Entities)
                {
                    var model = MapInternal(relatedEntity, bindingType, cache);
                    if (model != null)
                    {
                        property.AddElement(bindingModel, model);
                    }
                }
            }
        }

        // ---------------------------------------------------------------------
        // Cycle protection cache
        // ---------------------------------------------------------------------

        /// <summary>
        /// Per-traversal cache used to break cycles when a binding model graph is self-referencing
        /// (e.g. contact -> parentcustomerid -> account -> primarycontactid -> contact).
        /// </summary>
        internal sealed class BindingCache
        {
            private readonly Dictionary<string, Dictionary<Guid, IBindingModel>> _byType
                = new Dictionary<string, Dictionary<Guid, IBindingModel>>();

            public IBindingModel Find(Type type, Guid id)
            {
                if (id == Guid.Empty || type?.FullName == null)
                {
                    return null;
                }

                return _byType.TryGetValue(type.FullName, out var forType) && forType.TryGetValue(id, out var binding)
                    ? binding
                    : null;
            }

            public void Add(Type type, Guid id, object binding)
            {
                if (id == Guid.Empty || type?.FullName == null || binding is not IBindingModel model)
                {
                    return;
                }

                if (!_byType.TryGetValue(type.FullName, out var forType))
                {
                    forType = new Dictionary<Guid, IBindingModel>();
                    _byType[type.FullName] = forType;
                }

                forType[id] = model;
            }
        }
    }
}
