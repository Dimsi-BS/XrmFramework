using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using EntityReference = Microsoft.Xrm.Sdk.EntityReference;

namespace XrmFramework.BindingModel
{
    public static class BindingModelHelper
    {
        private static T FindBinding<T>(Dictionary<string, Dictionary<Guid, object>> cache, string typeName, Guid idEntity)
        {
            T returnBinding = default;
            if (!string.IsNullOrEmpty(typeName) && !idEntity.Equals(Guid.Empty))
            {
                cache.TryGetValue(typeName, out var cachedEntities);

                if (cachedEntities != null)
                {
                    cachedEntities.TryGetValue(idEntity, out var cachedBinding);
                    returnBinding = (T)cachedBinding;
                }
            }
            return returnBinding;
        }

        private static bool BindingExists(Dictionary<string, Dictionary<Guid, object>> cache, string typeName, Guid idEntity)
        {
            var bRet = false;
            var res = FindBinding<object>(cache, typeName, idEntity);
            if (res != null)
            {
                bRet = true;
            }
            return bRet;
        }

        private static void AddBinding(Dictionary<string, Dictionary<Guid, object>> cache, string typeName, Guid idEntity, object binding)
        {
            if (!string.IsNullOrEmpty(typeName)
                && !idEntity.Equals(Guid.Empty)
                && !BindingExists(cache, typeName, idEntity))
            {
                cache.TryGetValue(typeName, out var cachedEntities);
                if (cachedEntities != null)
                {
                    cachedEntities[idEntity] = binding;
                }
                else
                {
                    cachedEntities = new Dictionary<Guid, object>();
                    cachedEntities.Add(idEntity, binding);
                    cache.Add(typeName, cachedEntities);
                }
            }
        }

        public static IEnumerable<T> ToBindingModel<T>(this IEnumerable<Entity> entity) where T : IBindingModel
        {
            var cache = new Dictionary<string, Dictionary<Guid, object>>();

            return entity.Select(e => (T)CachedToBindingModel(e, typeof(T), cache));
        }

        public static T ToBindingModel<T>(this Entity entity) where T : IBindingModel
        {
            return (T)CachedToBindingModel(entity, typeof(T), null);
        }

        public static EntityReference ToEntityReference<T>(this T model, IOrganizationService service) where T : IBindingModel
        {
            var entity = model.ToEntity(service);

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

        public static IBindingModel ToBindingModel(this Entity entity, Type type)
        {
            return CachedToBindingModel(entity, type, null);
        }

        //public static IBindingModel ToBindingModel(this Entity entity, Type type)
        //{
        //    return CachedToBindingModel(entity, type);
        //}

        //private static IBindingModel CachedToBindingModel(this Entity entity, Type type)
        //{
        //    return CachedToBindingModel(entity, type);
        //}

        private static IBindingModel CachedToBindingModel(this Entity entity, Type type, Dictionary<string, Dictionary<Guid, object>> cache)
        {
            if (entity == null)
            {
                return null;
            }

            var swGlobal = new Stopwatch();
            var swLocal = new Stopwatch();
            swGlobal.Start();
            swLocal.Start();

            var modelDefinition = DefinitionCache.GetModelDefinition(type);

            var bindingModel = modelDefinition.GetInstance();

            modelDefinition.SetId(bindingModel, entity.Id);

            if (bindingModel is IEntityModel m)
            {
                m.Entity = entity;
            }

            if (cache != null)
            {
                object binding = FindBinding<object>(cache, type.FullName, entity.Id);

                if (binding != null)
                {
                    return (IBindingModel)binding;
                }
            }

            var entityDefinition = modelDefinition.MainDefinition;

            foreach (var property in modelDefinition.CrmAttributes)
            {
                var crmAttribute = property.CrmMappingAttribute;

                var attributeName = crmAttribute.AttributeName;

                var isKey = entityDefinition.IsKey(attributeName);

                if (((isKey && !entity.KeyAttributes.ContainsKey(attributeName)) || (!isKey)) && !entity.Contains(attributeName))
                {
                    continue;
                }

                object value = null;

                if (property.HasConverter)
                {
                    value = property.ConvertFrom(entity[crmAttribute.AttributeName]);
                }
                else
                {
                    var objectType = property.ObjectType;

                    switch (entityDefinition.GetAttributeType(attributeName))
                    {
                        case AttributeTypeCode.State:
                        case AttributeTypeCode.Status:
                        case AttributeTypeCode.Picklist:
                            value = GetPicklist(entity, objectType, attributeName);
                            break;
                        case AttributeTypeCode.MultiSelectPicklist:
                            value = GetPicklistValues(entity, objectType, attributeName);
                            break;
                        case AttributeTypeCode.Lookup:
                        case AttributeTypeCode.Owner:
                        case AttributeTypeCode.Customer:
                            //var relationships = entityDefinition.GetRelationshipsFromAttributeName(attributeName);
                            var modelLookupAttributes = entityDefinition.GetCrmLookupAttributes(attributeName);

                            var crmLookupAttribute = property.CrmLookupAttribute;
                            var isEntityReference = false;

                            if (crmLookupAttribute != null)
                            {
                                var targetEntityDefinition = DefinitionCache.GetEntityDefinition(crmLookupAttribute.TargetEntityName);

                                var fieldName = string.Format("{0}.{1}", attributeName, crmLookupAttribute.AttributeName);
                                if (entity.Contains(fieldName))
                                {
                                    var relatedAttributeType = targetEntityDefinition.GetAttributeType(crmLookupAttribute.AttributeName);

                                    switch (relatedAttributeType)
                                    {
                                        case AttributeTypeCode.Picklist:
                                        case AttributeTypeCode.State:
                                        case AttributeTypeCode.Status:
                                            value = GetPicklist(entity, objectType, fieldName);
                                            break;
                                        case AttributeTypeCode.MultiSelectPicklist:
                                            value = GetPicklistValues(entity, objectType, fieldName);
                                            break;
                                        case AttributeTypeCode.Money:
                                            value = entity.GetAliasedValue<Money>(fieldName)?.Value;
                                            break;
                                        case AttributeTypeCode.Lookup:
                                        case AttributeTypeCode.Owner:
                                        case AttributeTypeCode.Customer:
                                            isEntityReference = true;
                                            value = entity.GetAliasedValue<EntityReference>(fieldName);
                                            break;
                                        default:
                                            value = entity.GetAliasedValue<object>(fieldName);
                                            break;
                                    }
                                }
                                else if (targetEntityDefinition.IsPrimaryAttribute(crmLookupAttribute.AttributeName, PrimaryAttributeType.Name))
                                {
                                    if (entity.Contains(attributeName) && entity[attributeName] != null)
                                    {
                                        value = entity.GetAttributeValue<EntityReference>(attributeName).Name;
                                    }
                                }
                                else
                                {
                                    foreach (var relationship in modelLookupAttributes)
                                    {
                                        if (entity.RelatedEntities.Keys.Any(r => r.SchemaName == relationship.RelationshipName))
                                        {
                                            var relatedEntity = entity.RelatedEntities.First(r => r.Key.SchemaName == relationship.RelationshipName).Value?.Entities.FirstOrDefault();

                                            if (relatedEntity != null)
                                            {
                                                var relatedEntityDefinition = DefinitionCache.GetEntityDefinition(relatedEntity.LogicalName);

                                                var relatedAttributeType = relatedEntityDefinition.GetAttributeType(crmLookupAttribute.AttributeName);

                                                switch (relatedAttributeType)
                                                {
                                                    case AttributeTypeCode.Picklist:
                                                    case AttributeTypeCode.State:
                                                    case AttributeTypeCode.Status:
                                                        value = GetPicklist(relatedEntity, objectType, crmLookupAttribute.AttributeName);
                                                        break;
                                                    case AttributeTypeCode.MultiSelectPicklist:
                                                        value = GetPicklistValues(relatedEntity, objectType, crmLookupAttribute.AttributeName);
                                                        break;
                                                    case AttributeTypeCode.Money:
                                                        value = relatedEntity.GetAttributeValue<Money>(crmLookupAttribute.AttributeName)?.Value;
                                                        break;
                                                    case AttributeTypeCode.Lookup:
                                                    case AttributeTypeCode.Owner:
                                                    case AttributeTypeCode.Customer:
                                                        isEntityReference = true;
                                                        value = relatedEntity.GetAttributeValue<EntityReference>(crmLookupAttribute.AttributeName);
                                                        break;
                                                    default:
                                                        value = relatedEntity.GetAttributeValue<object>(crmLookupAttribute.AttributeName);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (typeof(IBindingModel).IsAssignableFrom(property.ObjectType))
                            {

                                if (entity.Contains(attributeName) && entity[attributeName] != null)
                                {
                                    var entityReference = entity.GetAttributeValue<EntityReference>(attributeName);

                                    var entityTemp = new Entity(entityReference.LogicalName);
                                    entityTemp.Id = entityReference.Id;

                                    var prefix = string.Format("{0}", attributeName);

                                    var isEmbed = false;

                                    foreach (var keyName in entity.Attributes.Keys)
                                    {
                                        if (keyName.StartsWith($"{prefix}.") && keyName != prefix)
                                        {
                                            var newKeyName = keyName.Substring(prefix.Length).Substring(1);
                                            isEmbed = true;

                                            object tempValue;
                                            if (newKeyName.IndexOf('.') == -1)
                                            {
                                                tempValue = entity.GetAttributeValue<AliasedValue>(keyName).Value;
                                            }
                                            else
                                            {
                                                tempValue = entity.GetAttributeValue<AliasedValue>(keyName);
                                            }

                                            entityTemp[newKeyName] = tempValue;
                                        }
                                    }

                                    if (!isEmbed)
                                    {
                                        foreach (var relationship in modelLookupAttributes)
                                        {
                                            if (entity.RelatedEntities.Keys.Any(r => r.SchemaName == relationship.RelationshipName))
                                            {
                                                entityTemp = entity.RelatedEntities.First(r => r.Key.SchemaName == relationship.RelationshipName).Value?.Entities.FirstOrDefault();
                                                if (entityTemp != null && DefinitionCache.TryGetModelDefinition(property.ObjectType, out var modelDefinitionTemp))
                                                {
                                                    if (modelDefinitionTemp.MainDefinition.EntityName == entityTemp.LogicalName)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    value = entityTemp.CachedToBindingModel(property.PropertyType, cache);
                                }
                            }
                            else
                            {
                                value = entity.GetAttributeValue<EntityReference>(attributeName);
                                isEntityReference = true;
                            }

                            if (isEntityReference && objectType == typeof(Guid))
                            {
                                value = ((EntityReference)value)?.Id;
                            }

                            break;
                        case AttributeTypeCode.Money:

                            if (entity.Contains(attributeName) && entity[attributeName] != null)
                            {
                                var moneyValue = entity.GetAttributeValue<Money>(attributeName);
                                if (objectType == typeof(decimal))
                                {
                                    value = moneyValue.Value;
                                }
                                else if (objectType == typeof(Money))
                                {
                                    value = moneyValue;
                                }
                            }
                            break;
                        default:
                            if (isKey && entity.KeyAttributes.ContainsKey(attributeName))
                            {
                                value = entity.KeyAttributes[attributeName];
                            }
                            else
                            {
                                value = entity[attributeName];
                            }
                            break;
                    }
                }
                property.SetValue(bindingModel, value);
            }


            foreach (var property in modelDefinition.ExtendBindingAttributes)
            {
                property.SetValue(bindingModel, entity.CachedToBindingModel(property.PropertyType, cache));
            }

            foreach (var property in modelDefinition.RelationshipAttributes)
            {
                var relationship = property.Relationship;
                Type bindingType;

                if (entity.RelatedEntities.Keys.Any(r => r.SchemaName == relationship.SchemaName) && property.IsCollectionProperty(out bindingType))
                {
                    var entityList = entity.RelatedEntities.First(r => r.Key.SchemaName == relationship.SchemaName).Value ?? new EntityCollection();

                    foreach (var entityTemp in entityList.Entities)
                    {
                        var model = entityTemp.CachedToBindingModel(bindingType, cache);

                        property.AddElement(bindingModel, model);
                    }
                }
            }

            if (cache != null)
            {
                AddBinding(cache, type.FullName, entity.Id, bindingModel);
            }

            return (IBindingModel)bindingModel;
        }

        private static object GetPicklist(Entity entity, Type objectType, string attributeName)
        {
            object value = null;
            if (objectType == typeof(int))
            {
                if (entity.Contains(attributeName) && entity[attributeName] != null)
                {
                    if (entity[attributeName] is AliasedValue)
                    {
                        value = entity.GetAliasedValue<OptionSetValue>(attributeName).Value;
                    }
                    else
                    {
                        value = entity.GetAttributeValue<OptionSetValue>(attributeName).Value;
                    }
                }
            }
            else if (entity.Contains(attributeName) && entity[attributeName] != null)
            {
                if (entity[attributeName] is AliasedValue)
                {
                    value = Enum.ToObject(objectType, entity.GetAliasedValue<OptionSetValue>(attributeName).Value);
                }
                else
                {
                    value = Enum.ToObject(objectType, entity.GetAttributeValue<OptionSetValue>(attributeName).Value);
                }
            }
            return value;
        }

        private static object GetPicklistValues(Entity entity, Type objectType, string attributeName)
        {
            object value = null;

            var genericType = objectType.GenericTypeArguments.First();

            if (genericType == typeof(int))
            {
                if (entity.Contains(attributeName) && entity[attributeName] != null)
                {
                    if (entity[attributeName] is AliasedValue)
                    {
                        value = entity.GetAliasedValue<OptionSetValueCollection>(attributeName).Select(o => o.Value).ToList();
                    }
                    else
                    {
                        value = entity.GetAttributeValue<OptionSetValueCollection>(attributeName).Select(o => o.Value).ToList();
                    }
                }
            }
            else if (entity.Contains(attributeName) && entity[attributeName] != null)
            {
                if (entity[attributeName] is AliasedValue)
                {
                    value = entity.GetAliasedValue<OptionSetValueCollection>(attributeName).ToEnumCollection(genericType);
                }
                else
                {
                    value = entity.GetAttributeValue<OptionSetValueCollection>(attributeName).ToEnumCollection(genericType);
                }
            }
            return value;
        }

        public static Entity ToEntity(this IBindingModel bindingModel, IOrganizationService service, bool fillRelatedEntities = true)
        {
            return ToEntity(bindingModel.GetType(), bindingModel, service, fillRelatedEntities);
        }

        public static Entity ToEntity(Type type, object bindingModel, IOrganizationService service, bool fillRelatedEntities = true)
        {
            var entityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(type);

            var entity = new Entity(entityDefinition.EntityName);
            FillEntity(bindingModel, service, fillRelatedEntities, entity, null);

            return entity;
        }

        private static void FillEntity(object bindingModel, IOrganizationService service, bool fillRelatedEntities, Entity entity, KeyInfos keyInfos)
        {
            var entityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(bindingModel.GetType());
            if (entity.Id == Guid.Empty)
            {
                entity.Id = ((IBindingModel)bindingModel).Id;
            }

            var checkKeys = keyInfos == null;
            if (keyInfos == null)
            {
                keyInfos = entityDefinition.GetKeyInfos();
            }

            var modelDefinition = DefinitionCache.GetModelDefinition(bindingModel.GetType());

            foreach (var property in modelDefinition.CrmAttributes)
            {
                var crmAttribute = property.CrmMappingAttribute;

                // Filtering out not initialized fields
                if (bindingModel is BindingModelBase @base)
                {
                    if (!@base.InitializedProperties.Contains(property.Name) && property.Name != "Id")
                    {
                        continue;
                    }
                }

                var isKey = entityDefinition.IsKey(crmAttribute.AttributeName);

                if (!isKey && !crmAttribute.IsValidForUpdate)
                {
                    continue;
                }

                var objectType = property.ObjectType;

                if (typeof(IXmlModel).IsAssignableFrom(objectType))
                {
                    continue;
                }

                var value = property.GetValue(bindingModel);

                if (property.HasConverter)
                {
                    SetValue(entity, crmAttribute.AttributeName, property.ConvertFrom(value), keyInfos, isKey);
                }
                else
                {
                    var attributeType = entityDefinition.GetAttributeType(crmAttribute.AttributeName);
                    switch (attributeType)
                    {
                        case AttributeTypeCode.State:
                        case AttributeTypeCode.Status:
                        case AttributeTypeCode.Picklist:
                            OptionSetValue pickListValue = null;
                            if (objectType.IsEnum)
                            {
                                if (Enum.GetName(objectType, value) != "Null" || (int)value != 0)
                                {
                                    pickListValue = new OptionSetValue((int)value);
                                }
                            }
                            else if (value != null)
                            {
                                pickListValue = new OptionSetValue((int)value);
                            }
                            SetValue(entity, crmAttribute.AttributeName, pickListValue, keyInfos, isKey);
                            break;
                        case AttributeTypeCode.MultiSelectPicklist:
                            var list = new List<OptionSetValue>();
                            if (value is IEnumerable enumValues)
                            {
                                foreach (var v in enumValues)
                                {

                                    if (objectType.IsEnum)
                                    {
                                        if (Enum.GetName(objectType, value) != "Null" || (int)v != 0)
                                        {
                                            list.Add(new OptionSetValue((int)v));
                                        }
                                    }
                                    else if (value != null)
                                    {
                                        list.Add(new OptionSetValue((int)v));
                                    }
                                }
                            }
                            var pickListValues = list.Any() ? new OptionSetValueCollection(list) : null;

                            SetValue(entity, crmAttribute.AttributeName, pickListValues, keyInfos, isKey);
                            break;
                        case AttributeTypeCode.Lookup:
                        case AttributeTypeCode.Customer:
                        case AttributeTypeCode.Owner:
                            var crmLookupAttribute = property.CrmLookupAttribute;

                            EntityReference refValue = null;

                            if (attributeType == AttributeTypeCode.Owner && value == null)
                            {
                                // Do Nothing
                            }
                            else if (crmLookupAttribute == null)
                            {
                                if (value is EntityReference)
                                {
                                    refValue = value as EntityReference;
                                }
                                else if (objectType == typeof(Guid))
                                {
                                    var tempLookupAttribute = entityDefinition.GetCrmLookupAttributes(crmAttribute.AttributeName).FirstOrDefault();
                                    if (tempLookupAttribute != null)
                                    {
                                        Guid tempGuidValue = value as Guid? ?? ((Guid?)value).Value;

                                        refValue = tempGuidValue == Guid.Empty ? null : new EntityReference(tempLookupAttribute.TargetEntityName, tempGuidValue);
                                    }
                                }

                                SetValue(entity, crmAttribute.AttributeName, refValue, keyInfos, isKey);
                            }
                            else
                            {
                                SetValue(entity, crmAttribute.AttributeName, GetEntityReferenceValue(service, crmAttribute, crmLookupAttribute, value), keyInfos, isKey);
                            }
                            break;
                        case AttributeTypeCode.Money:
                            SetValue(entity, crmAttribute.AttributeName, value == null ? null : new Money((decimal)value), keyInfos, isKey);
                            break;
                        case AttributeTypeCode.Uniqueidentifier:
                            var guidValue = (Guid)value;
                            SetValue(entity, crmAttribute.AttributeName, guidValue, keyInfos, isKey);
                            entity.Id = guidValue;
                            break;
                        case AttributeTypeCode.DateTime:
                            if (value == null)
                            {
                                SetValue(entity, crmAttribute.AttributeName, null, keyInfos, isKey);
                            }
                            else
                            {
                                var dateValue = (DateTime)value;
                                SetValue(entity, crmAttribute.AttributeName, dateValue == DateTime.MinValue ? null : dateValue, keyInfos, isKey);
                            }
                            break;
                        default:
                            SetValue(entity, crmAttribute.AttributeName, value, keyInfos, isKey);
                            break;
                    }
                }
            }

            foreach (var property in modelDefinition.ExtendBindingAttributes)
            {
                FillEntity(property.GetValue(bindingModel), service, fillRelatedEntities, entity, keyInfos);
            }

            if (fillRelatedEntities)
            {
                foreach (var property in modelDefinition.RelationshipAttributes)
                {
                    var relationshipAttribute = property.RelationshipAttribute;
                    if (!relationshipAttribute.IsValidForUpdate)
                    {
                        continue;
                    }
                    //if (relationshipAttribute is ManyToManyRelationshipAttribute)
                    //{
                    //    continue;
                    //}

                    var entityCollection = new EntityCollection();

                    var values = property.GetValue(bindingModel) as IEnumerable;
                    var bindingType = property.PropertyType.GenericTypeArguments.First();

                    foreach (var value in values)
                    {
                        entityCollection.Entities.Add(ToEntity(bindingType, value, service));
                    }

                    entity.RelatedEntities.Add(new Microsoft.Xrm.Sdk.Relationship(property.Relationship.SchemaName), entityCollection);
                }
            }

            if (checkKeys)
            {
                var checkedKeys = keyInfos.CheckedKeys;
                var keptKeyColumns = new List<string>();

                if (entity.Id == Guid.Empty)
                {
                    foreach (var keyName in checkedKeys)
                    {
                        var columns = keyInfos.GetKeyColumns(keyName);
                        if (keptKeyColumns.Count == 0)
                        {
                            keptKeyColumns.AddRange(columns);
                        }
                        else if (columns.Count < keptKeyColumns.Count)
                        {
                            keptKeyColumns.Clear();
                            keptKeyColumns.AddRange(columns);
                        }
                    }
                }

                foreach (var attributeName in keptKeyColumns)
                {
                    if (entity.Contains(attributeName))
                    {
                        entity.KeyAttributes[attributeName] = entity[attributeName];
                    }
                }
            }
        }

        private static void SetValue(Entity entity, string attributeName, object value, KeyInfos keyInfos, bool isKey)
        {
            if (value != null)
            {
                keyInfos.CheckColumn(attributeName);
            }
            entity[attributeName] = value;
        }

        private static EntityReference GetEntityReferenceValue(IOrganizationService service, CrmMappingAttribute crmAttribute, CrmLookupAttribute crmLookupAttribute, object value)
        {
            EntityReference reference = null;
            if (!(value == null || (value is string && string.IsNullOrEmpty((string)value))))
            {
                if (crmLookupAttribute.AllowNotExisting)
                {
                    var query = new QueryExpression(crmLookupAttribute.TargetEntityName);
                    query.ColumnSet.AddColumn(crmLookupAttribute.AttributeName);
                    query.Criteria.AddCondition(crmLookupAttribute.AttributeName, ConditionOperator.Equal, value);

                    reference = service.RetrieveMultiple(query).Entities.Select(e => e.ToEntityReference()).FirstOrDefault();
                }
                else
                {
                    reference = new EntityReference(crmLookupAttribute.TargetEntityName, crmLookupAttribute.AttributeName, value);
                }
            }
            return reference;
        }

        public static QueryExpression GetRetrieveAllQuery<T>() where T : IBindingModel
        {
            return GetRetrieveAllQuery(typeof(T));
        }

        public static QueryExpression GetQueryToFilter(Type bindingModelType, Func<Relationship, LinkEntity, JoinOperator> filter)
        {
            var entityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(bindingModelType);

            var query = new QueryExpression(entityDefinition.EntityName);

            AddQueryFilter(bindingModelType, filter, query.ColumnSet, query.LinkEntities);

            return query;
        }

        private static void AddQueryFilter(Type bindingModelType, Func<Relationship, LinkEntity, JoinOperator> filter, ColumnSet columnSet, DataCollection<LinkEntity> links, int depth = 1, string linkAlias = "")
        {
            var modelDefinition = DefinitionCache.GetModelDefinition(bindingModelType);
            var entityDefinition = modelDefinition.MainDefinition;


            foreach (var property in modelDefinition.CrmAttributes)
            {
                var crmAttribute = property.CrmMappingAttribute;
                if (!columnSet.Columns.Contains(crmAttribute.AttributeName))
                {
                    columnSet.AddColumn(crmAttribute.AttributeName);
                }

                var linkAliasName = $"{linkAlias}{(!string.IsNullOrEmpty(linkAlias) ? "." : string.Empty)}{crmAttribute.AttributeName}";

                if (entityDefinition.IsLookupAttribute(crmAttribute.AttributeName))
                {
                    if (depth > 1 && !crmAttribute.FollowLink)
                    {
                        continue;
                    }

                    var lookupAttributes = entityDefinition.GetCrmLookupAttributes(crmAttribute.AttributeName).ToList();

                    var hasOneLookupAttribute = lookupAttributes.Count == 1;
                    var crmLookupAttribute = property.CrmLookupAttribute;

                    var targetEntityName = property.IsBindingModel ? property.TargettedModelDefinition.MainDefinition.EntityName :
                        property.CrmLookupAttribute?.TargetEntityName;

                    if (hasOneLookupAttribute && crmLookupAttribute == null && typeof(Guid).IsAssignableFrom(property.ObjectType))
                    {
                        continue;
                    }

                    if (hasOneLookupAttribute && targetEntityName == null)
                    {
                        targetEntityName = lookupAttributes.Single().TargetEntityName;
                    }

                    foreach (var lookupAttribute in lookupAttributes)
                    {
                        if ((crmLookupAttribute != null && crmLookupAttribute.TargetEntityName != lookupAttribute.TargetEntityName)
                            || (crmLookupAttribute == null && lookupAttribute.TargetEntityName != targetEntityName))
                        {
                            continue;
                        }

                        var relationship = entityDefinition.GetRelationshipByAttributeNameAndTargetEntityName(crmAttribute.AttributeName, targetEntityName);

                        LinkEntity link;
                        if (links.Any(l => l.EntityAlias == linkAliasName))
                        {
                            link = links.Single(l => l.EntityAlias == linkAliasName);
                        }
                        else
                        {
                            link = new LinkEntity
                            {
                                LinkFromEntityName = entityDefinition.EntityName,
                                LinkFromAttributeName = crmAttribute.AttributeName,
                                LinkToEntityName = lookupAttribute.TargetEntityName,
                                LinkToAttributeName = lookupAttribute.AttributeName
                            };

                            links.Add(link);
                            link.JoinOperator = JoinOperator.LeftOuter;
                            link.EntityAlias = linkAliasName;

                            if (filter != null)
                            {
                                link.JoinOperator = filter(relationship, link);
                            }
                        }

                        if (crmLookupAttribute != null)
                        {
                            link.Columns.AddColumn(crmLookupAttribute.AttributeName);
                        }
                        else if (typeof(IBindingModel).IsAssignableFrom(property.PropertyType))
                        {
                            AddQueryFilter(property.PropertyType, filter, link.Columns, link.LinkEntities, depth + 1, linkAliasName);
                        }
                    }
                }
            }

            foreach (var property in bindingModelType.GetProperties().Where(p => p.GetCustomAttributes(typeof(ExtendBindingModelAttribute), false).Any()))
            {
                AddQueryFilter(property.PropertyType, filter, columnSet, links, depth, linkAlias);
            }
        }

        public static QueryExpression GetRetrieveAllQuery(Type bindingModelType)
        {
            return GetQueryToFilter(bindingModelType, null);
        }

        public static T ToBindingModel<T>(this XElement element) where T : IXmlModel
        {
            if (element == null)
            {
                return default(T);
            }

            return (T)ToBindingModel(element, typeof(T));
        }

        public static object ToBindingModel(this XElement element, Type type)
        {
            if (element == null)
            {
                return null;
            }

            var modelDefinition = DefinitionCache.GetModelDefinition(type);

            var bindingModel = modelDefinition.GetInstance();

            var properties = new Queue<AttributeDefinition>();

            foreach (var prop in modelDefinition.XmlMappingAttributes)
            {
                properties.Enqueue(prop);
            }

            var rootElementName = modelDefinition.XmlMappingAttribute?.RelativePath;

            if (rootElementName != null)
            {
                var split = rootElementName.Split('/');
                for (var i = 1; i < split.Length; i++)
                {
                    element = element.Element(split[i]);
                }
            }

            var initializedProperties = new HashSet<string>();

            while (properties.Any())
            {
                var property = properties.Dequeue();
                var xmlAttribute = property.XmlMappingAttribute;

                var objectType = property.ObjectType;

                dynamic propElement;
                if (xmlAttribute.IsAttribute)
                {
                    propElement = element.Attribute(xmlAttribute.RelativePath);
                }
                else if (string.IsNullOrEmpty(xmlAttribute.RelativePath))
                {
                    propElement = element;
                }
                else
                {
                    propElement = element.Element(xmlAttribute.RelativePath);
                }

                if (!string.IsNullOrEmpty(xmlAttribute.AlternateRelativePath) && (string.IsNullOrEmpty(propElement?.Value)))
                {
                    propElement = element.Element(xmlAttribute.AlternateRelativePath);
                }

                if (propElement != null)
                {
                    if (xmlAttribute.ConverterType != null)
                    {
                        var converter = (IXmlConverter)xmlAttribute.ConverterType.GetConstructor(new Type[] { }).Invoke(new object[] { });

                        if (xmlAttribute.IsAttribute)
                        {
                            property.SetValue(bindingModel, converter.ConvertFromXElement(new XElement(propElement.Name, propElement.Value)));
                        }
                        else
                        {
                            property.SetValue(bindingModel, converter.ConvertFromXElement(propElement));
                        }

                    }
                    else if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    {
                        var types = property.PropertyType.GenericTypeArguments;

                        if (types.Count() != 1)
                        {
                            continue;
                        }

                        if (!typeof(ICollection<>).MakeGenericType(types).IsAssignableFrom(property.PropertyType))
                        {
                            continue;
                        }

                        var bindingType = types.First();

                        var xmlAttributeTemp = bindingType.GetCustomAttribute<XmlMappingAttribute>();

                        if (bindingType != typeof(string) && (!typeof(IXmlModel).IsAssignableFrom(bindingType) || xmlAttributeTemp == null))
                        {
                            continue;
                        }

                        var referencedProperties = GetReferencedParentProperties(type, bindingType);

                        referencedProperties.ExceptWith(initializedProperties);

                        if (referencedProperties.Any())
                        {
                            properties.Enqueue(property);
                            continue;
                        }

                        var list = property.GetValue(bindingModel);

                        if (bindingType == typeof(string))
                        {
                            foreach (XElement el in propElement.Elements())
                            {

                                list.GetType().GetMethod("Add").Invoke(list, new object[] { el.Value });
                            }
                        }
                        else
                        {
                            var elements = propElement.Elements(xmlAttributeTemp.RelativePath);

                            if (!Enumerable.Any(elements))
                            {
                                elements = propElement.Elements(xmlAttributeTemp.AlternateRelativePath);
                            }

                            foreach (XElement el in elements)
                            {
                                var b = el.ToBindingModel(bindingType);

                                foreach (var prop in bindingType.GetProperties().Where(p => p.GetCustomAttributes<CopyFromParentAttribute>().Any()))
                                {
                                    var copyFromParentAttributes = prop.GetCustomAttributes<CopyFromParentAttribute>();
                                    foreach (var copyFromParentAttribute in copyFromParentAttributes)
                                    {
                                        if (copyFromParentAttribute.ParentType == type)
                                        {
                                            prop.SetValue(b, type.GetProperty(copyFromParentAttribute.ParentPropertyName).GetValue(bindingModel));
                                        }
                                    }
                                }

                                list.GetType().GetMethod("Add").Invoke(list, new[] { b });
                            }
                        }
                    }
                    else if (typeof(IXmlModel).IsAssignableFrom(property.PropertyType))
                    {
                        var b = ((XElement)propElement).ToBindingModel(property.PropertyType);

                        property.SetValue(bindingModel, b);
                    }
                    else
                    {
                        object value = propElement.Value;

                        switch (objectType.Name)
                        {
                            case "Int32":
                                int intValue;
                                if (int.TryParse(propElement.Value, out intValue))
                                {
                                    value = intValue;
                                }
                                else
                                {
                                    value = null;
                                }
                                break;
                            case "Decimal":
                                if (string.IsNullOrEmpty(propElement.Value))
                                {
                                    if (property.IsNullable)
                                    {
                                        value = null;
                                    }
                                    else
                                    {
                                        value = default(decimal);
                                    }
                                }
                                else
                                {
                                    if (propElement.Value.Contains("."))
                                    {
                                        value = decimal.Parse(propElement.Value, CultureInfo.GetCultureInfo("en-US"));
                                    }
                                    else
                                    {
                                        value = decimal.Parse(propElement.Value, CultureInfo.GetCultureInfo("fr-FR"));
                                    }
                                }
                                break;
                            case "Boolean":
                                if (property.IsNullable)
                                {
                                    value = string.IsNullOrEmpty(propElement.Value) ? null : (bool?)bool.Parse(propElement.Value);
                                }
                                else
                                {
                                    value = bool.Parse(propElement.Value);
                                }
                                break;
                            case "DateTime":
                                if (string.IsNullOrEmpty(propElement.Value))
                                {
                                    value = null;
                                }
                                else
                                {
                                    DateTime dateTemp;

                                    if (DateTime.TryParseExact(propElement.Value, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTemp))
                                    {
                                        value = dateTemp;
                                    }
                                    else
                                    {
                                        value = DateTime.Parse(propElement.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                                    }
                                }
                                break;
                            case "Guid":
                                if (Guid.TryParse(propElement.Value, out Guid guidValue))
                                {
                                    value = guidValue;
                                }
                                else if (string.IsNullOrEmpty(propElement.Value))
                                {
                                    value = Guid.Empty;
                                }
                                else
                                {
                                    throw new Exception(string.Format("Field {0} should contain a Guid value.", property.Name));
                                }
                                break;
                            default:
                                if (objectType.IsEnum)
                                {
                                    int tempIntValue;
                                    if (int.TryParse(propElement.Value, out tempIntValue))
                                    {
                                        value = tempIntValue;
                                    }
                                    else
                                    {
                                        value = null;
                                    }
                                }

                                break;
                        }

                        property.SetValue(bindingModel, value);
                    }
                }

                initializedProperties.Add(property.Name);
            }

            var transformAttribute = type.GetCustomAttribute<XmlTransformAttribute>();
            if (transformAttribute != null)
            {
                var constructorInfo = (transformAttribute.ActionType).GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    var transform = (IXmlTransform)constructorInfo.Invoke(new object[] { });

                    transform.PostXmlConvertion(type, bindingModel);
                }
            }

            return bindingModel;
        }

        private static ISet<string> GetReferencedParentProperties(Type parentType, Type modelType)
        {
            var list = new HashSet<string>();

            foreach (var property in modelType.GetProperties())
            {
                var copyFromParentAttributes = property.GetCustomAttributes<CopyFromParentAttribute>();

                foreach (var copyFromParentAttribute in copyFromParentAttributes)
                {
                    if (copyFromParentAttribute != null && copyFromParentAttribute.ParentType == parentType)
                    {
                        list.Add(copyFromParentAttribute.ParentPropertyName);
                    }
                }
            }

            return list;
        }

        public static XElement ToXElement<T>(this T bindingModel) where T : IXmlModel
        {
            if (bindingModel == null)
            {
                return null;
            }

            return ToXElement(bindingModel.GetType(), bindingModel);
        }

        public static XElement ToXElement(Type type, object bindingModel)
        {
            var elementName = "data";

            var modelDefinition = DefinitionCache.GetModelDefinition(type);

            var transformAttribute = type.GetCustomAttribute<XmlTransformAttribute>();
            if (transformAttribute != null)
            {
                var constructorInfo = (transformAttribute.ActionType).GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    var transform = (IXmlTransform)constructorInfo.Invoke(new object[] { });

                    transform.PreXmlConvertion(type, bindingModel);
                }
            }

            var modelXmlAttribute = type.GetCustomAttribute<XmlMappingAttribute>();
            if (modelXmlAttribute != null)
            {
                elementName = modelXmlAttribute.RelativePath;
            }
            XElement el, root;

            if (elementName.Contains('/'))
            {
                var elementNameSplit = elementName.Split('/');
                root = new XElement(elementNameSplit[0]);
                el = root;

                for (var i = 1; i < elementNameSplit.Length; i++)
                {
                    var tempElement = new XElement(elementNameSplit[i]);
                    el.Add(tempElement);
                    el = tempElement;
                }
            }
            else
            {
                root = new XElement(elementName);
                el = root;
            }

            foreach (var property in modelDefinition.XmlMappingAttributes)
            {
                var xmlAttribute = property.XmlMappingAttribute;

                var element = new XElement(xmlAttribute.RelativePath);
                el.Add(element);

                var propertyValue = property.GetValue(bindingModel);

                if (xmlAttribute.ConverterType != null)
                {
                    var constructorInfo = xmlAttribute.ConverterType.GetConstructor(new Type[] { });
                    if (constructorInfo != null)
                    {
                        var converter = (IXmlConverter)constructorInfo.Invoke(new object[] { });

                        converter.FillXElement(element, propertyValue);
                    }
                }
                else if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    Type bindingType;

                    if (property.IsCollectionProperty(out bindingType))
                    {
                        var xmlAttributeTemp = bindingType.GetCustomAttribute<XmlMappingAttribute>();

                        if (!typeof(IXmlModel).IsAssignableFrom(bindingType) || xmlAttributeTemp == null)
                        {
                            continue;
                        }

                        var values = property.GetValue(bindingModel) as IEnumerable;

                        foreach (var value in values)
                        {
                            var tempElement = ToXElement(bindingType, value);

                            element.Add(tempElement);
                        }
                    }
                }
                else
                {
                    if (propertyValue != null)
                    {
                        if (typeof(IBindingModel).IsAssignableFrom(property.PropertyType))
                        {
                            element.Add(ToXElement(property.PropertyType, propertyValue).Descendants());
                        }
                        else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        {
                            element.Value = string.Format(CultureInfo.InvariantCulture, "{0:s}", propertyValue);
                        }
                        else
                        {
                            var textValue = string.Format(CultureInfo.InvariantCulture, "{0}", propertyValue);

                            element.Value = textValue;
                        }
                    }
                }
            }

            return root;
        }

        public static RequestContainer GetUpsertRequests(this IXmlModel xmlModel, IOrganizationService service, bool disablePluginsExecution = false)
        {
            var container = new RequestContainer(disablePluginsExecution);

            FillUpsertRequests(xmlModel, service, container);

            return container;
        }

        private static void FillUpsertRequests(IXmlModel xmlModel, IOrganizationService service, RequestContainer container, IBindingModel extendedModel = null)
        {
            if (xmlModel == null)
            {
                return;
            }

            var modelType = xmlModel.GetType();

            var modelDefinition = DefinitionCache.GetModelDefinition(modelType);

            if (xmlModel is IBindingModel model)
            {
                container.AddModel(model, service, extendedModel);
            }

            var upsertableProperties = modelDefinition.UpsertableAttributes;
            if (upsertableProperties.Any())
            {
                if (upsertableProperties.Count > 1)
                {
                    var properties = new SortedDictionary<int, AttributeDefinition>();
                    var otherProperties = new List<AttributeDefinition>();
                    foreach (var property in upsertableProperties)
                    {
                        var order = property.UpsertOrder;
                        if (order.HasValue)
                        {
                            properties.Add(order.Value, property);
                        }
                        else
                        {
                            if (!property.CrmMappingAttribute?.IsValidForUpdate ?? false)
                            {
                                continue;
                            }

                            otherProperties.Add(property);
                        }
                    }

                    foreach (var order in properties.Keys)
                    {
                        FillUpsertRequests(xmlModel, properties[order], service, container);
                    }

                    foreach (var property in otherProperties)
                    {
                        FillUpsertRequests(xmlModel, property, service, container);
                    }
                }
                else
                {
                    FillUpsertRequests(xmlModel, upsertableProperties.Single(), service, container);
                }
            }

        }

        private static void FillUpsertRequests(object model, AttributeDefinition property, IOrganizationService service, RequestContainer container)
        {
            var extendedModel = property.IsExtendBindingModel ? model as IBindingModel : null;

            if (!property.CrmMappingAttribute?.IsValidForUpdate ?? false)
            {
                return;
            }

            if (typeof(IBindingModel).IsAssignableFrom(property.PropertyType))
            {
                var bindingModel = (IBindingModel)property.GetValue(model);

                if (bindingModel == null)
                {
                    return;
                }

                if (property.IsExtendBindingModel && model is IBindingModel parentModel)
                {
                    bindingModel.Id = parentModel.Id;
                }

                FillUpsertRequests(bindingModel, service, container, extendedModel);
            }
            else
            {
                foreach (IXmlModel value in (IEnumerable)property.GetValue(model))
                {
                    FillUpsertRequests(value, service, container, extendedModel);
                }
            }
        }

        private static UpsertRequest GetUpsertRequest(this IBindingModel bindingModel, IOrganizationService service)
        {
            var entity = bindingModel.ToEntity(service);

            var request = new UpsertRequest
            {
                Target = entity
            };

            return request;
        }

        public static IBindingModel ToBindingModel(object dto, Type bindingType = null)
        {
            var dtoType = dto.GetType();

            if (bindingType == null)
            {
                bindingType = GetCorrespondingBindingType(dtoType);
            }

            if (bindingType == null)
            {
                throw new Exception(string.Format("No BindingModel associated with type {0}", dtoType.Name));
            }

            var bindingModel = bindingType.GetConstructor(new Type[] { }).Invoke(new object[] { });

            foreach (var bindingProperty in bindingType.GetProperties())
            {
                var mappingAttribute = bindingProperty.GetCustomAttribute<DtoFieldMappingAttribute>();
                if (mappingAttribute == null)
                {
                    continue;
                }

                var dtoProperty = dtoType.GetProperty(mappingAttribute.RelativePath);
                if (dtoProperty == null)
                {
                    throw new Exception(string.Format("The Property {0} does not exist on type {1}, modify BindingModel accordingly", mappingAttribute.RelativePath, dtoType.Name));
                }

                var value = dtoProperty.GetValue(dto);

                if (dtoProperty.PropertyType == bindingProperty.PropertyType)
                {
                    bindingProperty.SetValue(bindingModel, value);
                }
                else if (mappingAttribute.ConverterType != null)
                {
                    var converter = (IDtoAttributeConverter)mappingAttribute.ConverterType.GetConstructor(new Type[] { }).Invoke(new object[] { });

                    var bindingValue = converter.ConvertFromDtoAttribute(value);
                    bindingProperty.SetValue(bindingModel, bindingValue);
                }



            }

            return bindingModel as IBindingModel;
        }

        public static Type GetCorrespondingBindingType(Type dtoType)
        {
            var bindingType = typeof(IService).Assembly.GetTypes().Where(t => typeof(IXmlModel).IsAssignableFrom(t)).Where(t =>
            {
                var attr = t.GetCustomAttribute<DtoObjectMappingAttribute>();

                return attr != null && attr.RelativePath == dtoType.Name;
            }).FirstOrDefault();
            return bindingType;
        }

        private static RetrieveRequest GetRetrieveRequest(Type type, EntityReference reference)
        {
            var entityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(type);

            var request = new RetrieveRequest
            {
                Target = reference,
                ColumnSet = new ColumnSet(),
                RelatedEntitiesQuery = new RelationshipQueryCollection()
            };

            FillRetrieveRequest(type, request, entityDefinition);

            return request;
        }

        private static void FillRetrieveRequest(Type type, RetrieveRequest request, EntityDefinition entityDefinition)
        {
            var modelDefinition = DefinitionCache.GetModelDefinition(type);

            foreach (var property in modelDefinition.CrmAttributes)
            {
                var mappingAttribute = property.CrmMappingAttribute;

                if (!request.ColumnSet.Columns.Contains(mappingAttribute.AttributeName))
                {
                    request.ColumnSet.AddColumn(mappingAttribute.AttributeName);
                }

                if (!entityDefinition.IsLookupAttribute(mappingAttribute.AttributeName) || property.CrmLookupAttribute == null && (property.ObjectType == typeof(Guid) || property.ObjectType == typeof(EntityReference)))
                {
                    continue;
                }

                var definitionLookupAttributes = entityDefinition.GetCrmLookupAttributes(mappingAttribute.AttributeName).ToList();
                var lookupAttribute = property.CrmLookupAttribute;
                var isBindingModel = typeof(IBindingModel).IsAssignableFrom(property.PropertyType);

                var subModelDefinition = isBindingModel ? DefinitionCache.GetModelDefinition(property.PropertyType) : null;

                var targetEntityName = isBindingModel ? subModelDefinition.MainDefinition.EntityName : lookupAttribute?.TargetEntityName;

                if (definitionLookupAttributes.Count == 1 && targetEntityName == null)
                {
                    targetEntityName = definitionLookupAttributes.Single().TargetEntityName;
                }

                foreach (var definitionLookupAttribute in definitionLookupAttributes)
                {
                    if (string.IsNullOrEmpty(targetEntityName))
                    {
                        targetEntityName = definitionLookupAttribute.TargetEntityName;
                    }

                    if (definitionLookupAttribute.TargetEntityName != targetEntityName)
                    {
                        continue;
                    }

                    QueryExpression query;

                    if (request.RelatedEntitiesQuery.Keys.All(r => r.SchemaName != definitionLookupAttribute.RelationshipName))
                    {
                        query = new QueryExpression(targetEntityName);
                        request.RelatedEntitiesQuery.Add(new Microsoft.Xrm.Sdk.Relationship(definitionLookupAttribute.RelationshipName), query);
                    }
                    else
                    {
                        query = (QueryExpression)request.RelatedEntitiesQuery.First(r => r.Key.SchemaName == definitionLookupAttribute.RelationshipName).Value;
                    }

                    if (isBindingModel)
                    {
                        AddQueryFilter(property.PropertyType, null, query.ColumnSet, query.LinkEntities, 2, string.Empty);
                    }
                    else if (lookupAttribute != null)
                    {
                        query.ColumnSet.AddColumn(lookupAttribute.AttributeName);
                    }
                }
            }


            foreach (var property in modelDefinition.ExtendBindingAttributes)
            {
                FillRetrieveRequest(property.PropertyType, request, entityDefinition);
            }

            foreach (var property in modelDefinition.RelationshipAttributes)
            {
                Type bindingType;
                if (property.IsCollectionProperty(out bindingType))
                {
                    var query = GetRetrieveAllQuery(bindingType);

                    request.RelatedEntitiesQuery.Add(new Microsoft.Xrm.Sdk.Relationship(property.Relationship.SchemaName), query);
                }
            }
        }

        public static T GetById<T>(this IOrganizationService service, Guid id) where T : IBindingModel
        {
            var type = typeof(T);
            var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
            return (T)GetById(type, service, new EntityReference(definition.EntityName, id));
        }

        public static T GetById<T>(this IOrganizationService service, EntityReference reference) where T : IBindingModel
        {
            var type = typeof(T);
            return (T)GetById(type, service, reference);
        }

        public static IBindingModel GetById(Type type, IOrganizationService service, Guid id)
        {
            var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
            return GetById(type, service, new EntityReference(definition.EntityName, id));
        }

        public static IBindingModel GetById(Type type, IOrganizationService service, EntityReference reference)
        {
            var request = GetRetrieveRequest(type, reference);

            var response = (RetrieveResponse)service.Execute(request);

            return response.Entity.ToBindingModel(type);
        }

        public static bool TryGetById<T>(this IOrganizationService service, Guid id, out T result) where T : IBindingModel
        {
            var type = typeof(T);
            var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
            IBindingModel resultTemp;

            var isOk = TryGetById(type, service, new EntityReference(definition.EntityName, id), out resultTemp);
            result = (T)resultTemp;
            return isOk;
        }

        public static bool TryGetById<T>(this IOrganizationService service, EntityReference reference, out T result) where T : IBindingModel
        {
            var type = typeof(T);

            IBindingModel resultTemp;

            var isOk = TryGetById(type, service, reference, out resultTemp);
            result = (T)resultTemp;
            return isOk;
        }

        public static bool TryGetById(Type type, IOrganizationService service, Guid id, out IBindingModel result)
        {
            var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
            return TryGetById(type, service, new EntityReference(definition.EntityName, id), out result);
        }

        public static bool TryGetById(Type type, IOrganizationService service, EntityReference reference, out IBindingModel result)
        {
            bool bSuccess = false;
            try
            {
                result = GetById(type, service, reference);
                bSuccess = true;
            }
            catch (Exception)
            {
                result = null;
            }
            return bSuccess;
        }


        public static T ToDto<T>(IBindingModel model) where T : new()
        {
            var dtoType = typeof(T);

            var bindingType = model.GetType();

            var dto = new T();

            foreach (var bindingProperty in bindingType.GetProperties())
            {
                var mappingAttribute = bindingProperty.GetCustomAttribute<DtoFieldMappingAttribute>();
                if (mappingAttribute == null)
                {
                    continue;
                }

                var dtoProperty = dtoType.GetProperty(mappingAttribute.RelativePath);
                if (dtoProperty == null)
                {
                    throw new Exception(string.Format("The Property {0} does not exist on type {1}, modify BindingModel accordingly", mappingAttribute.RelativePath, dtoType.Name));
                }

                var tempValue = bindingProperty.GetValue(model);

                object value = null;

                if (bindingProperty.PropertyType == dtoProperty.PropertyType)
                {
                    value = tempValue;
                }
                else if (mappingAttribute.ConverterType != null)
                {
                    var converter = (IDtoAttributeConverter)mappingAttribute.ConverterType.GetConstructor(new Type[] { }).Invoke(new object[] { });

                    value = converter.ConvertToDtoAttribute(tempValue);
                }
                else if (dtoProperty.PropertyType == typeof(string) && tempValue != null)
                {
                    value = tempValue.ToString();
                }

                dtoProperty.SetValue(dto, value);
            }

            return dto;
        }

        public static U FromDto<T, U>(T dto) where T : new() where U : IBindingModel, new()
        {
            var model = new U();

            var bindingType = typeof(U);

            var dtoType = typeof(T);

            foreach (var bindingProperty in bindingType.GetProperties())
            {
                var mappingAttribute = bindingProperty.GetCustomAttribute<DtoFieldMappingAttribute>();
                if (mappingAttribute == null)
                {
                    continue;
                }

                var dtoProperty = dtoType.GetProperty(mappingAttribute.RelativePath);
                if (dtoProperty == null)
                {
                    throw new Exception(string.Format("The Property {0} does not exist on type {1}, modify BindingModel accordingly", mappingAttribute.RelativePath, dtoType.Name));
                }

                var tempValue = dtoProperty.GetValue(dto);

                object value = null;

                if (bindingProperty.PropertyType == dtoProperty.PropertyType)
                {
                    value = tempValue;
                }
                else if (mappingAttribute.ConverterType != null)
                {
                    var converter = (IDtoAttributeConverter)mappingAttribute.ConverterType.GetConstructor(new Type[] { }).Invoke(new object[] { });

                    value = converter.ConvertFromDtoAttribute(tempValue);
                }
                else if (dtoProperty.PropertyType == typeof(string) && tempValue != null)
                {
                    value = tempValue.ToString();
                }

                bindingProperty.SetValue(model, value);
            }

            return model;
        }

        public static T Upsert<T>(this IOrganizationService service, XDocument doc, UpsertSettings settings = null) where T : IXmlModel
        {
            return Upsert<T>(service, doc.Root, settings);
        }

        public static T Upsert<T>(this IOrganizationService service, XElement doc, UpsertSettings settings = null) where T : IXmlModel
        {
            var model = doc.ToBindingModel<T>();
            return Upsert<T>(service, model, settings);
        }

        public static T Upsert<T>(this IOrganizationService service, T model, UpsertSettings settings = null) where T : IXmlModel
        {
            return (T)UpsertModel(service, typeof(T), model, settings);
        }

        private static object UpsertModel(IOrganizationService service, Type type, object model, UpsertSettings settings = null)
        {
            settings = settings ?? new UpsertSettings();

            var behaviourAttribute = type.GetCustomAttribute<UpsertBehaviourAttribute>();
            if (behaviourAttribute != null)
            {
                var constructor = behaviourAttribute.BehaviourType.GetConstructor(new Type[] { });
                if (constructor != null)
                {
                    var behaviour = constructor.Invoke(new object[] { });
                    if (behaviour != null)
                    {
                        var method = behaviourAttribute.BehaviourType.GetMethod("ApplyBehaviour");
                        if (method != null)
                        {
                            method.Invoke(behaviour, new object[] { service, model });
                        }
                    }
                }
            }
            else
            {
                var listRequests = ((IXmlModel)model).GetUpsertRequests(service, settings.DisablePluginsExecution);

                var skip = 0;

                if (settings.UseTransactionRequest)
                {
                    while (listRequests.Count > skip)
                    {
                        var list = listRequests.Skip(skip).Take(200).ToList();

                        var responses = ExecuteTransactionRequests<UpsertResponse>(service, list);

                        for (var i = 0; i < list.Count; i++)
                        {
                            listRequests.UpdateIds(list[i], responses[i].Target.Id);
                        }

                        skip += 200;
                    }
                }
                else
                {
                    var requestList = listRequests.ToList();

                    var jobResult = service.ExecuteMultiple(requestList, request => request, "Update", 200, settings.ContinueOnError);

                    foreach (var pair in jobResult.Responses)
                    {
                        var request = requestList[pair.Key];

                        if (pair.Value is UpsertResponse response)
                        {
                            listRequests.UpdateIds(request, response.Target.Id);
                        }
                    }

                    if (jobResult.ErrorMessages.Any())
                    {
                        var errorContent = new StringBuilder();

                        foreach (var errorMessage in jobResult.ErrorMessages)
                        {
                            errorContent.AppendLine($"====> requête {errorMessage.Key} : {errorMessage.Value}\r\n");
                        }

                        throw new Exception(errorContent.ToString());
                    }
                }
            }
            return model;
        }

        private static IList<T> ExecuteTransactionRequests<T>(IOrganizationService service, IEnumerable<OrganizationRequest> requests) where T : OrganizationResponse
        {
            var request = new ExecuteTransactionRequest
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };
            request.Requests.AddRange(requests);
            var response = (ExecuteTransactionResponse)service.Execute(request);

            return response.Responses.Cast<T>().ToList();
        }

        public static IList<T> RetrieveAll<T>(this IOrganizationService service) where T : IBindingModel
        {
            var query = GetRetrieveAllQuery(typeof(T));
            return RetrieveAll(service, query).ToBindingModel<T>().ToList();
        }

        public static IList<T> RetrieveAll<T>(this IOrganizationService service, QueryExpression query, bool cleanLinks = false) where T : IBindingModel
        {
            return RetrieveAll(service, query, cleanLinks).ToBindingModel<T>().ToList();
        }

        public static IList<Entity> RetrieveAll(this IOrganizationService service, QueryExpression query, bool cleanLinks = true)
        {
            if (!query.TopCount.HasValue)
            {
                query.PageInfo = new PagingInfo { Count = 5000, PageNumber = 1 };
            }

            var result = new List<Entity>();

            EntityCollection ec;

            if (cleanLinks)
            {
                query.CleanLinks();
            }

            do
            {
                ec = service.RetrieveMultiple(query);
                Debug.WriteLine($"Récupération de la page {query.PageInfo?.PageNumber} de {query.PageInfo?.Count} enregistrements.");

                result.AddRange(ec.Entities);

                if (query.PageInfo != null)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = ec.PagingCookie;
                }

            } while (ec.MoreRecords);

            return result;
        }

        public class JobResult
        {

            public int NbCreated { get; set; }

            public int NbUpdated { get; set; }

            public int NbRejected { get; set; }

            public ICollection<KeyValuePair<int, string>> ErrorMessages { get; } = new List<KeyValuePair<int, string>>();

            public ICollection<KeyValuePair<int, OrganizationResponse>> Responses { get; } = new List<KeyValuePair<int, OrganizationResponse>>();
        }

        public static JobResult ExecuteMultiple<T>(this IOrganizationService service, IList<T> objects, Func<T, OrganizationRequest> RequestBuilder, string message, int nbRequest = 500, bool continueOnError = true)
        {
            int createdRecord = 0;
            int updatedRecord = 0;
            int rejectedRecord = 0;

            var result = new JobResult();

            var _sw = new Stopwatch();

            if (objects.Count == 0)
            {
                return result;
            }
            Console.Write("Updating {0}...", message);
            var request = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = continueOnError,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };
            _sw.Restart();
            var errorCount = 0;
            var offset = 0;
            var errorsMessage = new List<string>();
            while (offset < objects.Count)
            {
                request.Requests.Clear();

                request.Requests.AddRange(objects.Skip(offset).Take(nbRequest).Select(RequestBuilder).Select(r =>
                {
                    if (r is CreateRequest tempRequest)
                    {
                        var entity = tempRequest.Target;

                        for (var i = entity.KeyAttributes.Count - 1; i >= 0; i--)
                        {
                            var keyName = entity.KeyAttributes.Keys.ElementAt(i);
                            entity[keyName] = entity.KeyAttributes[keyName];
                            entity.KeyAttributes.Remove(keyName);
                        }
                    }

                    return r;
                }));

                var response = (ExecuteMultipleResponse)service.Execute(request);

                if (response.IsFaulted)
                {
                    errorCount = response.Responses.Count(r => r.Fault != null);
                    rejectedRecord += errorCount;
                }
                foreach (var res in response.Responses)
                {
                    if (res.Response != null)
                    {
                        result.Responses.Add(new KeyValuePair<int, OrganizationResponse>(res.RequestIndex, res.Response));

                        if (res.Response is UpsertResponse upsertResponse)
                        {
                            if (upsertResponse.RecordCreated)
                            {
                                createdRecord++;
                            }
                            else
                            {
                                updatedRecord++;
                            }
                        }
                        else if (res.Response is UpdateResponse)
                        {
                            updatedRecord++;
                        }
                        else if (res.Response is CreateResponse)
                        {
                            createdRecord++;
                        }
                    }
                    else if (res.Fault != null)
                    {
                        if (res.Fault.InnerFault != null)
                        {
                            result.ErrorMessages.Add(new KeyValuePair<int, string>(res.RequestIndex, res.Fault.InnerFault.Message));
                        }
                        else
                        {
                            var errorMessageTemp = res.Fault.Message;

                            if (!string.IsNullOrEmpty(res.Fault.TraceText))
                            {
                                errorMessageTemp += "\r\nDetail:\r\n" + res.Fault.TraceText;
                            }
                            result.ErrorMessages.Add(new KeyValuePair<int, string>(res.RequestIndex, errorMessageTemp));
                        }
                    }
                }

                var errorMessage = errorCount == 0 ? string.Empty : $" ({errorCount} erreurs)";
                var remainingTime = TimeSpan.FromMilliseconds((_sw.ElapsedMilliseconds * objects.Count / (offset + request.Requests.Count)) - _sw.ElapsedMilliseconds);

                Console.Write("\rUpdated {0}/{1} {2} in {3}{4} (ETA {5})", offset + request.Requests.Count, objects.Count, message, _sw.Elapsed, errorMessage, remainingTime);
                offset += nbRequest;
            }

            if (errorsMessage.Count > 0)
            {
                Console.WriteLine("\nRejected Records reason : ");
                foreach (string s in errorsMessage)
                    Console.WriteLine(s);
            }
            _sw.Stop();
            Console.WriteLine();

            result.NbCreated = createdRecord;
            result.NbUpdated = updatedRecord;
            result.NbRejected = rejectedRecord;

            return result;
        }

        private class BindingModelWrapper
        {
            private IXmlModel Model { get; set; }

            public BindingModelWrapper(IXmlModel model)
            {
                Model = model;
            }
        }
    }

    public class UpsertSettings
    {
        private bool _continueOnError = false;
        private bool _useTransactionRequest = true;

        public bool UseTransactionRequest
        {
            get { return _useTransactionRequest; }
            set
            {
                _useTransactionRequest = value;
                if (value)
                    ContinueOnError = false;
            }
        }

        public bool DisablePluginsExecution { get; set; }

        public bool ContinueOnError
        {
            get { return _continueOnError; }
            set
            {
                _continueOnError = value;
                if (value)
                {
                    UseTransactionRequest = false;
                }
            }
        }
    }
}