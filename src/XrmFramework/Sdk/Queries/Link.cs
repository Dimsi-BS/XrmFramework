// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XrmFramework.Sdk.Queries
{
    public class Link : QueryObject, IQueryToString
    {
        public string ToEntityName { get; }

        public string FromAttributeName { get; }

        public string ToAttributeName { get; }

        public string EntityAlias { get; set; }

        public JoinOperator JoinOperator { get; set; }
        public bool DoAddColumns { get; }

        public bool IsWebApiFriendly => !Links.Any() && !Criteria.HasConditions && (TargetDefinition?.IsFullyValid ?? false) ;

        private EntityDefinition TargetDefinition { get; }

        public bool CanBeRemoved
        {
            get
            {
                var hasNoLinkAndNoCriteria = !Links.Any() && !Criteria.HasConditions && !Criteria.Filters.Any();

                var hasOnlyPrimaryField = Attributes.All(attribute => attribute == TargetDefinition.PrimaryIdAttributeName || attribute == TargetDefinition.PrimaryNameAttributeName);

                return hasNoLinkAndNoCriteria && hasOnlyPrimaryField;
            }
        }

        public Link(string entityName, string toEntityName, string fromAttributeName, string toAttributeName, JoinOperator joinOperator, bool addColumns) : base(entityName)
        {
            ToEntityName = toEntityName;
            FromAttributeName = fromAttributeName;
            ToAttributeName = toAttributeName;
            JoinOperator = joinOperator;
            DoAddColumns = addColumns;
            Criteria = new QueryCriteria(LogicalOperator.And, ToEntityName);

            if (addColumns)
            {
                try
                {
                    TargetDefinition = DefinitionCache.GetEntityDefinition(toEntityName);

                    if (!string.IsNullOrEmpty(TargetDefinition.PrimaryIdAttributeName))
                    {
                        AddColumn(TargetDefinition.PrimaryIdAttributeName);
                    }

                    if (!string.IsNullOrEmpty(TargetDefinition.PrimaryNameAttributeName))
                    {
                        AddColumn(TargetDefinition.PrimaryNameAttributeName);
                    }
                }
                catch (KeyNotFoundException)
                {
                    // définition non trouvée
                }
            }
        }

        public override string ToFetchXmlString()
        {
            var sb = new StringBuilder();
            sb.Append($"<link-entity name=\"{ToEntityName}\"");
            sb.Append($" link-type=\"{JoinOperator.GetDescription()}\"");
            sb.Append($" from=\"{ToAttributeName}\"");
            sb.Append($" to=\"{FromAttributeName}\"");

            if (!string.IsNullOrEmpty(EntityAlias))
            {
                sb.Append($" alias=\"{EntityAlias}\"");
            }

            sb.Append(">");

            foreach (var attribute in Attributes)
            {
                sb.Append($"<attribute name=\"{attribute}\" />");
            }

            if (Criteria.HasConditions)
            {
                sb.Append(Criteria.ToFetchXmlString());
            }

            foreach (var link in Links)
            {
                sb.Append(link.ToFetchXmlString());
            }

            sb.Append("</link-entity>");
            return sb.ToString();
        }

        public override string ToWebApiString(Func<int> aliasIndexer = null)
        {
            var definition = DefinitionCache.GetEntityDefinition(EntityName);
            var targetDefinition = DefinitionCache.GetEntityDefinition(ToEntityName);
            var sb = new StringBuilder();

            var relationship = definition.GetRelationshipByAttributeNameAndTargetEntityName(FromAttributeName, ToEntityName);

            if (relationship == null)
            {
                return string.Empty;
            }

            sb.Append(relationship.NavigationPropertyName);

            if (Attributes.Any())
            {
                sb.Append("($select=");
                foreach (var attribute in Attributes)
                {
                    sb.Append(attribute).Append(",");
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
            }

            return sb.ToString();
        }
    }

    public enum JoinOperator
    {
        [Description("inner")]
        Inner = 0,
        [Description("outer")]
        LeftOuter = 1,
        [Description("inner")]
        Natural = 2
    }
}
