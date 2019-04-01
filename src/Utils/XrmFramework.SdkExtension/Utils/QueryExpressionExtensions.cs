using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class QueryExpressionExtensions
    {

        public static FilterExpression GetRootFilterExpression(this QueryExpression query)
        {
            return GetRootFilterExpression(query.Criteria);
        }

        public static FilterExpression GetRootFilterExpression(FilterExpression criteria)
        {
            var value = criteria;

            if (!value.Conditions.Any())
            {
                value = value.Filters.FirstOrDefault();
            }

            return value;
        }

        public static T GetConditionValue<T>(this FilterExpression criteria, string attributeName)
        {
            if (!GetConditionValue(criteria, attributeName, out var value))
            {
                value = default(T);
            }

            return (T)value;
        }

        private static bool GetConditionValue(FilterExpression filter, string attributeName, out object value)
        {
            value = null;
            var retour = false;

            var condition = filter.Conditions.FirstOrDefault(c => c.AttributeName == attributeName);

            if (condition != null)
            {
                value = condition.Values.FirstOrDefault();
                retour = true;
            }
            else
            {
                foreach (var fe in filter.Filters)
                {
                    retour = GetConditionValue(fe, attributeName, out value);
                    if (retour)
                    {
                        break;
                    }
                }
            }

            return retour;
        }

        public static int GetLinkCount(this QueryExpression query) => query.LinkEntities.Sum(GetLinkCount);

        private static int GetLinkCount(LinkEntity link) => 1 + link.LinkEntities.Sum(GetLinkCount);



        public static void CleanLinks(this QueryExpression query)
        {
            for (var i = query.LinkEntities.Count - 1; i >= 0; i--)
            {
                var link = query.LinkEntities[i];
                if (CanBeRemoved(link))
                {
                    query.LinkEntities.Remove(link);
                }
                else
                {
                    CleanLinks(link);
                }
            }
        }

        private static void CleanLinks(LinkEntity link)
        {
            for (var i = link.LinkEntities.Count - 1; i >= 0; i--)
            {
                var tempLink = link.LinkEntities[i];
                if (CanBeRemoved(tempLink))
                {
                    link.LinkEntities.Remove(tempLink);
                }
                else
                {
                    CleanLinks(tempLink);
                }
            }
        }

        private static bool CanBeRemoved(LinkEntity link)
        {
            var targetDefinition = DefinitionCache.GetEntityDefinition(link.LinkToEntityName);

            var hasNoLinkAndNoCriteria = !link.LinkEntities.Any() && !HasConditions(link.LinkCriteria) && !link.LinkCriteria.Filters.Any();

            var hasOnlyPrimaryField = link.Columns.Columns.All(attribute => attribute == targetDefinition.PrimaryIdAttributeName || attribute == targetDefinition.PrimaryNameAttributeName);

            return hasNoLinkAndNoCriteria && hasOnlyPrimaryField;
        }

        private static bool HasConditions(FilterExpression filter) => filter.Conditions.Any() || filter.Filters.Any(HasConditions);
    }
}
