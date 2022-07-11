// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XrmFramework.Sdk.Queries
{
    public class QueryCriteria : IQueryToString
    {
        public string EntityName { private get; set; }

        public LogicalOperator Operator { get; set; }

        public IList<Condition> Conditions { get; } = new List<Condition>();

        public IList<QueryCriteria> Filters { get; } = new List<QueryCriteria>();

        public string WebApiAliasString
        {
            get
            {
                var sb = new StringBuilder();

                var definition = DefinitionCache.GetEntityDefinition(EntityName);

                foreach (var condition in Conditions.Where(c => c.UseAlias))
                {
                    sb.Append($"&p{condition.AliasIndex}={definition.GetWebApiAttributeName(condition.AttributeName)}");
                }

                foreach (var filter in Filters)
                {
                    sb.Append(filter.WebApiAliasString);
                }

                return sb.ToString();
            }
        }

        internal QueryCriteria(LogicalOperator logicalOperator, string entityName)
        {
            Operator = logicalOperator;
            EntityName = entityName;
        }

        public QueryCriteria AddFilter(LogicalOperator logicalOperator)
        {
            var filter = new QueryCriteria(logicalOperator, EntityName);
            Filters.Add(filter);
            return filter;
        }

        public string ToFetchXmlString()
        {
            var sb = new StringBuilder();

            sb.Append($"<filter type=\"{Operator.ToString().ToLowerInvariant()}\">");

            foreach (var condition in Conditions)
            {
                sb.Append(condition.ToFetchXmlString());
            }

            foreach (var filter in Filters)
            {
                sb.Append(filter.ToFetchXmlString());
            }

            sb.Append("</filter>");
            return sb.ToString();
        }

        public string ToWebApiString(Func<int> aliasIndexer)
        {
            var sb = new StringBuilder();

            foreach (var condition in Conditions)
            {
                sb.Append(condition.ToWebApiString(aliasIndexer));

                sb.Append(" ").Append(WebApiOperator).Append(" ");
            }

            foreach (var filter in Filters)
            {
                sb.Append("(").Append(filter.ToWebApiString(aliasIndexer)).Append(")").Append(" ").Append(WebApiOperator).Append(" ");
            }

            sb.Remove(sb.Length - (WebApiOperator.Length + 2), WebApiOperator.Length + 2);

            return sb.ToString();
        }

        public void AddCondition(string attributeName, ConditionOperator conditionOperator)
        {
            Conditions.Add(new Condition(EntityName, null, attributeName, conditionOperator));
        }

        public void AddCondition(string attributeName, ConditionOperator conditionOperator, object value)
        {
            Conditions.Add(new Condition(EntityName, attributeName, conditionOperator, value));
        }

        public void AddCondition(string entityAlias, string attributeName, ConditionOperator conditionOperator, object value)
        {
            Conditions.Add(new Condition(EntityName, entityAlias, attributeName, conditionOperator, value));
        }

        public void AddCondition(string attributeName, ConditionOperator conditionOperator, IEnumerable<object> values)
        {
            Conditions.Add(new Condition(EntityName, attributeName, conditionOperator, values));
        }

        private string WebApiOperator => Operator.ToString().ToLowerInvariant();

        public bool IsWebApiFriendly => Conditions.All(c => c.IsWebApiFriendly);

        public bool HasConditions => Conditions.Any() || Filters.Any(f => f.HasConditions);
    }

    public enum LogicalOperator
    {
        And,
        Or
    }
}
