// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.BindingModel
{
    /// <summary>
    /// Builds <see cref="QueryExpression"/> instances from a binding model type — adding the relevant columns
    /// and the LinkEntity chain needed to materialize lookup / extend properties.
    /// </summary>
    internal static class BindingModelQueryBuilder
    {
        public static QueryExpression BuildRetrieveAll(Type bindingModelType) => BuildFiltered(bindingModelType, filter: null);

        public static QueryExpression BuildFiltered(Type bindingModelType, Func<Relationship, LinkEntity, JoinOperator> filter)
        {
            var entityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(bindingModelType);
            var query = new QueryExpression(entityDefinition.EntityName);

            AppendFilter(bindingModelType, filter, query.ColumnSet, query.LinkEntities);
            return query;
        }

        public static void AppendFilter(
            Type bindingModelType,
            Func<Relationship, LinkEntity, JoinOperator> filter,
            ColumnSet columnSet,
            DataCollection<LinkEntity> links,
            int depth = 1,
            string linkAlias = "")
        {
            var modelDefinition = DefinitionCache.GetModelDefinition(bindingModelType);
            var entityDefinition = modelDefinition.MainDefinition;

            foreach (var property in modelDefinition.CrmAttributes)
            {
                AppendProperty(property, entityDefinition, filter, columnSet, links, depth, linkAlias);
            }

            foreach (var property in bindingModelType.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ExtendBindingModelAttribute), false).Any()))
            {
                AppendFilter(property.PropertyType, filter, columnSet, links, depth, linkAlias);
            }
        }

        private static void AppendProperty(
            AttributeDefinition property,
            EntityDefinition entityDefinition,
            Func<Relationship, LinkEntity, JoinOperator> filter,
            ColumnSet columnSet,
            DataCollection<LinkEntity> links,
            int depth,
            string linkAlias)
        {
            var crmAttribute = property.CrmMappingAttribute;

            if (!columnSet.Columns.Contains(crmAttribute.AttributeName))
            {
                columnSet.AddColumn(crmAttribute.AttributeName);
            }

            if (!entityDefinition.IsLookupAttribute(crmAttribute.AttributeName))
            {
                return;
            }

            if (depth > 1 && !crmAttribute.FollowLink)
            {
                return;
            }

            var linkAliasName = string.IsNullOrEmpty(linkAlias)
                ? crmAttribute.AttributeName
                : $"{linkAlias}.{crmAttribute.AttributeName}";

            var lookupAttributes = entityDefinition.GetCrmLookupAttributes(crmAttribute.AttributeName).ToList();
            var hasOneLookupAttribute = lookupAttributes.Count == 1;
            var crmLookupAttribute = property.CrmLookupAttribute;

            var targetEntityName = property.IsBindingModel
                ? property.TargettedModelDefinition.MainDefinition.EntityName
                : property.CrmLookupAttribute?.TargetEntityName;

            // Single Guid-typed lookup with no explicit CrmLookupAttribute: nothing to link.
            if (hasOneLookupAttribute && crmLookupAttribute == null && typeof(Guid).IsAssignableFrom(property.ObjectType))
            {
                return;
            }

            if (hasOneLookupAttribute && targetEntityName == null)
            {
                targetEntityName = lookupAttributes.Single().TargetEntityName;
            }

            foreach (var lookupAttribute in lookupAttributes)
            {
                if (!MatchesTargetEntity(crmLookupAttribute, lookupAttribute, targetEntityName))
                {
                    continue;
                }

                var relationship = entityDefinition.GetRelationshipByAttributeNameAndTargetEntityName(crmAttribute.AttributeName, targetEntityName);
                var aliasName = linkAliasName;
                if (!hasOneLookupAttribute)
                {
                    aliasName += $"__{targetEntityName}";
                }

                var link = GetOrCreateLink(links, filter, entityDefinition, crmAttribute, lookupAttribute, aliasName, relationship);

                if (crmLookupAttribute != null)
                {
                    link.Columns.AddColumn(crmLookupAttribute.AttributeName);
                }
                else if (typeof(IBindingModel).IsAssignableFrom(property.PropertyType))
                {
                    AppendFilter(property.PropertyType, filter, link.Columns, link.LinkEntities, depth + 1, aliasName);
                }
            }
        }

        private static bool MatchesTargetEntity(CrmLookupAttribute crmLookupAttribute, CrmLookupAttribute lookupAttribute, string targetEntityName)
        {
            if (crmLookupAttribute != null)
            {
                return crmLookupAttribute.TargetEntityName == lookupAttribute.TargetEntityName;
            }
            return lookupAttribute.TargetEntityName == targetEntityName;
        }

        private static LinkEntity GetOrCreateLink(
            DataCollection<LinkEntity> links,
            Func<Relationship, LinkEntity, JoinOperator> filter,
            EntityDefinition entityDefinition,
            CrmMappingAttribute crmAttribute,
            CrmLookupAttribute lookupAttribute,
            string aliasName,
            Relationship relationship)
        {
            var existing = links.FirstOrDefault(l => l.EntityAlias == aliasName);
            if (existing != null)
            {
                return existing;
            }

            var link = new LinkEntity
            {
                LinkFromEntityName = entityDefinition.EntityName,
                LinkFromAttributeName = crmAttribute.AttributeName,
                LinkToEntityName = lookupAttribute.TargetEntityName,
                LinkToAttributeName = lookupAttribute.AttributeName,
                JoinOperator = JoinOperator.LeftOuter,
                EntityAlias = aliasName
            };

            links.Add(link);

            if (filter != null)
            {
                link.JoinOperator = filter(relationship, link);
            }

            return link;
        }
    }
}
