// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using EntityReference = Microsoft.Xrm.Sdk.EntityReference;
using SdkRelationship = Microsoft.Xrm.Sdk.Relationship;
using SdkEntityRole = Microsoft.Xrm.Sdk.EntityRole;

namespace XrmFramework.BindingModel
{
    /// <summary>
    /// Converts an <see cref="IBindingModel"/> (and any extend/related models) into a Dataverse <see cref="Entity"/>.
    /// </summary>
    internal static class BindingModelToEntityMapper
    {
        public static Entity Map(Type type, object bindingModel, IOrganizationService service, bool fillRelatedEntities)
        {
            var entityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(type);
            var entity = new Entity(entityDefinition.EntityName);

            FillEntity(bindingModel, service, fillRelatedEntities, entity, keyInfos: null);
            return entity;
        }

        public static EntityReference ToEntityReference<T>(T model, IOrganizationService service) where T : IBindingModel
        {
            var entity = Map(typeof(T), model, service, fillRelatedEntities: true);
            var entityReference = entity.ToEntityReference();

            if (entityReference.Id == Guid.Empty)
            {
                foreach (var key in entity.KeyAttributes.Keys)
                {
                    entityReference.KeyAttributes[key] = entity.KeyAttributes[key];
                }
            }

            return entityReference;
        }

        private static void FillEntity(
            object bindingModel,
            IOrganizationService service,
            bool fillRelatedEntities,
            Entity entity,
            KeyInfos keyInfos)
        {
            var entityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(bindingModel.GetType());

            if (entity.Id == Guid.Empty)
            {
                entity.Id = ((IBindingModel)bindingModel).Id;
            }

            // The topmost call is responsible for key resolution so we only do it once
            // regardless of how many ExtendBindingAttributes we recurse through.
            var isTopLevelCall = keyInfos == null;
            keyInfos ??= entityDefinition.GetKeyInfos();

            var modelDefinition = DefinitionCache.GetModelDefinition(bindingModel.GetType());

            WriteCrmAttributes(bindingModel, service, entity, entityDefinition, keyInfos, modelDefinition);
            WriteExtendBindings(bindingModel, service, fillRelatedEntities, entity, keyInfos, modelDefinition);

            if (fillRelatedEntities)
            {
                WriteRelationships(bindingModel, service, entity, modelDefinition);
            }

            if (isTopLevelCall)
            {
                FinalizeKeyAttributes(entity, keyInfos);
            }
        }

        // ---------------------------------------------------------------------
        // CRM attributes
        // ---------------------------------------------------------------------

        private static void WriteCrmAttributes(
            object bindingModel,
            IOrganizationService service,
            Entity entity,
            EntityDefinition entityDefinition,
            KeyInfos keyInfos,
            ModelDefinition modelDefinition)
        {
            foreach (var property in modelDefinition.CrmAttributes)
            {
                if (ShouldSkipCrmAttribute(bindingModel, property, entityDefinition, out var crmAttribute, out var isKey))
                {
                    continue;
                }

                var value = property.GetValue(bindingModel);

                if (property.HasConverter)
                {
                    SetValue(entity, crmAttribute.AttributeName, property.ConvertFrom(value), keyInfos, isKey);
                    continue;
                }

                WriteTypedAttribute(entity, service, entityDefinition, keyInfos, property, crmAttribute, isKey, value);
            }
        }

        private static bool ShouldSkipCrmAttribute(
            object bindingModel,
            AttributeDefinition property,
            EntityDefinition entityDefinition,
            out CrmMappingAttribute crmAttribute,
            out bool isKey)
        {
            crmAttribute = property.CrmMappingAttribute;
            isKey = entityDefinition.IsKey(crmAttribute.AttributeName);

            // Skip uninitialised properties (BindingModelBase tracks which fields the caller actually set).
            if (bindingModel is BindingModelBase @base
                && !@base.InitializedProperties.Contains(property.Name)
                && property.Name != "Id")
            {
                return true;
            }

            if (!isKey && !crmAttribute.IsValidForUpdate)
            {
                return true;
            }

            if (typeof(IXmlModel).IsAssignableFrom(property.ObjectType))
            {
                return true;
            }

            return false;
        }

        private static void WriteTypedAttribute(
            Entity entity,
            IOrganizationService service,
            EntityDefinition entityDefinition,
            KeyInfos keyInfos,
            AttributeDefinition property,
            CrmMappingAttribute crmAttribute,
            bool isKey,
            object value)
        {
            var attributeName = crmAttribute.AttributeName;
            var attributeType = entityDefinition.GetAttributeType(attributeName);

            switch (attributeType)
            {
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.Picklist:
                    SetValue(entity, attributeName, BuildOptionSetValue(property.ObjectType, value), keyInfos, isKey);
                    break;

                case AttributeTypeCode.MultiSelectPicklist:
                    SetValue(entity, attributeName, BuildOptionSetValueCollection(property.ObjectType, value), keyInfos, isKey);
                    break;

                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Owner:
                    SetValue(entity, attributeName,
                        BuildLookupValue(service, entityDefinition, property, crmAttribute, attributeType, value),
                        keyInfos, isKey);
                    break;

                case AttributeTypeCode.Money:
                    SetValue(entity, attributeName,
                        value == null ? null : new Money((decimal)value),
                        keyInfos, isKey);
                    break;

                case AttributeTypeCode.Uniqueidentifier:
                    var guidValue = (Guid)value;
                    SetValue(entity, attributeName, guidValue, keyInfos, isKey);
                    entity.Id = guidValue;
                    break;

                case AttributeTypeCode.DateTime:
                    SetValue(entity, attributeName, NormalizeDateTime(value), keyInfos, isKey);
                    break;

                default:
                    SetValue(entity, attributeName, value, keyInfos, isKey);
                    break;
            }
        }

        private static object NormalizeDateTime(object value)
        {
            if (value == null)
            {
                return null;
            }

            var dateValue = (DateTime)value;
            return dateValue == DateTime.MinValue ? null : (object)dateValue;
        }

        // ---------------------------------------------------------------------
        // Option set / multi option set
        // ---------------------------------------------------------------------

        private static OptionSetValue BuildOptionSetValue(Type objectType, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (objectType.IsEnum)
            {
                // Treat Enum value "Null" with integer value 0 as an unset option.
                if (Enum.GetName(objectType, value) == "Null" && (int)value == 0)
                {
                    return null;
                }

                return new OptionSetValue((int)value);
            }

            return new OptionSetValue((int)value);
        }

        private static OptionSetValueCollection BuildOptionSetValueCollection(Type objectType, object value)
        {
            if (value is not IEnumerable enumValues)
            {
                return null;
            }

            var list = new List<OptionSetValue>();

            foreach (var v in enumValues)
            {
                if (v == null)
                {
                    continue;
                }

                if (objectType.IsEnum)
                {
                    // Preserve the (odd) original behaviour: skip the "Null" name with value 0.
                    if (Enum.GetName(objectType, v) == "Null" && (int)v == 0)
                    {
                        continue;
                    }
                }

                list.Add(new OptionSetValue((int)v));
            }

            return list.Count == 0 ? null : new OptionSetValueCollection(list);
        }

        // ---------------------------------------------------------------------
        // Lookup / customer / owner
        // ---------------------------------------------------------------------

        private static EntityReference BuildLookupValue(
            IOrganizationService service,
            EntityDefinition entityDefinition,
            AttributeDefinition property,
            CrmMappingAttribute crmAttribute,
            AttributeTypeCode attributeType,
            object value)
        {
            // Preserve original quirk: null owner writes nothing (not even a null overwrite).
            if (attributeType == AttributeTypeCode.Owner && value == null)
            {
                return null;
            }

            var crmLookupAttribute = property.CrmLookupAttribute;
            if (crmLookupAttribute == null)
            {
                return BuildLookupFromDirectValue(entityDefinition, crmAttribute, value);
            }

            return GetEntityReferenceValue(service, crmLookupAttribute, value);
        }

        private static EntityReference BuildLookupFromDirectValue(
            EntityDefinition entityDefinition,
            CrmMappingAttribute crmAttribute,
            object value)
        {
            if (value is EntityReference directReference)
            {
                return directReference;
            }

            if (value is Guid guid && guid != Guid.Empty)
            {
                var lookup = entityDefinition.GetCrmLookupAttributes(crmAttribute.AttributeName).FirstOrDefault();
                return lookup == null ? null : new EntityReference(lookup.TargetEntityName, guid);
            }

            return null;
        }

        private static EntityReference GetEntityReferenceValue(IOrganizationService service, CrmLookupAttribute crmLookupAttribute, object value)
        {
            if (value == null || (value is string s && string.IsNullOrEmpty(s)))
            {
                return null;
            }

            if (!crmLookupAttribute.AllowNotExisting)
            {
                return new EntityReference(crmLookupAttribute.TargetEntityName, crmLookupAttribute.AttributeName, value);
            }

            var query = new QueryExpression(crmLookupAttribute.TargetEntityName);
            query.ColumnSet.AddColumn(crmLookupAttribute.AttributeName);
            query.Criteria.AddCondition(crmLookupAttribute.AttributeName, ConditionOperator.Equal, value);

            return service.RetrieveMultiple(query).Entities
                .Select(e => e.ToEntityReference())
                .FirstOrDefault();
        }

        // ---------------------------------------------------------------------
        // Extend + Relationship bindings
        // ---------------------------------------------------------------------

        private static void WriteExtendBindings(
            object bindingModel,
            IOrganizationService service,
            bool fillRelatedEntities,
            Entity entity,
            KeyInfos keyInfos,
            ModelDefinition modelDefinition)
        {
            foreach (var property in modelDefinition.ExtendBindingAttributes)
            {
                var extendBindingModel = property.GetValue(bindingModel);
                if (extendBindingModel == null)
                {
                    continue;
                }

                FillEntity(extendBindingModel, service, fillRelatedEntities, entity, keyInfos);
            }
        }

        private static void WriteRelationships(
            object bindingModel,
            IOrganizationService service,
            Entity entity,
            ModelDefinition modelDefinition)
        {
            foreach (var property in modelDefinition.RelationshipAttributes)
            {
                var relationshipAttribute = property.RelationshipAttribute;
                if (!relationshipAttribute.IsValidForUpdate)
                {
                    continue;
                }

                if (property.GetValue(bindingModel) is not IEnumerable values)
                {
                    continue;
                }

                var entityCollection = new EntityCollection();
                var bindingType = property.PropertyType.GenericTypeArguments[0];

                foreach (var value in values)
                {
                    entityCollection.Entities.Add(Map(bindingType, value, service, fillRelatedEntities: true));
                }

                var relationship = new SdkRelationship(property.Relationship.SchemaName)
                {
                    PrimaryEntityRole = property.Relationship.PrimaryEntityRole == EntityRole.Referenced
                        ? SdkEntityRole.Referenced
                        : SdkEntityRole.Referencing
                };

                entity.RelatedEntities.Add(relationship, entityCollection);
            }
        }

        // ---------------------------------------------------------------------
        // Key resolution
        // ---------------------------------------------------------------------

        private static void FinalizeKeyAttributes(Entity entity, KeyInfos keyInfos)
        {
            if (entity.Id != Guid.Empty)
            {
                return;
            }

            var keptKeyColumns = PickSmallestCheckedKeySet(keyInfos);
            foreach (var attributeName in keptKeyColumns.Where(entity.Contains))
            {
                entity.KeyAttributes[attributeName] = entity[attributeName];
            }
        }

        private static List<string> PickSmallestCheckedKeySet(KeyInfos keyInfos)
        {
            // Among the alternate keys whose columns are all populated, keep the one with
            // the fewest columns – mimicking the original tie-break behaviour.
            var kept = new List<string>();

            foreach (var keyName in keyInfos.CheckedKeys)
            {
                var columns = keyInfos.GetKeyColumns(keyName);
                if (kept.Count == 0 || columns.Count < kept.Count)
                {
                    kept.Clear();
                    kept.AddRange(columns);
                }
            }

            return kept;
        }

        private static void SetValue(Entity entity, string attributeName, object value, KeyInfos keyInfos, bool isKey)
        {
            if (value != null)
            {
                keyInfos.CheckColumn(attributeName);
            }

            entity[attributeName] = value;
        }
    }
}
