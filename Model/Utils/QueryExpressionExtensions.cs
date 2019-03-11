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
            object value = null;

            if (!GetConditionValue(criteria, attributeName, out value))
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
    }
}
