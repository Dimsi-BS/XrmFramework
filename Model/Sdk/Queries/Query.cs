using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Sdk.Queries
{
    public class Query : QueryObject
    {
        private IList<Tuple<string, bool>> _orders = new List<Tuple<string, bool>>();

        public Query(string entityName) : base(entityName)
        {
            Criteria = new QueryCriteria(LogicalOperator.And, EntityName);
            var definition = DefinitionCache.GetEntityDefinition(entityName);
            AddColumns(definition.PrimaryIdAttributeName, definition.PrimaryNameAttributeName);
        }

        public int? TopCount { get; set; }

        public void AddOrder(string attributeName, bool descending = false)
        {
            _orders.Add(new Tuple<string, bool>(attributeName, descending));
        }

        public bool IsWebApiFriendlyQuery => Criteria.IsWebApiFriendly && Links.All(l => l.IsWebApiFriendly);

        public override string ToFetchXmlString()
        {
            var sb = new StringBuilder();
            sb.Append($"<fetch distinct=\"true\" mapping=\"logical\" output-format=\"xml-platform\" version=\"1.0\"");

            if (TopCount.HasValue)
            {
                sb.Append($" count=\"{TopCount}\"");
            }
            else if (PagingInfo.PageSize.HasValue)
            {
                sb.Append($" count=\"{PagingInfo.PageSize}\"");
                sb.Append($" page=\"{PagingInfo.PageNumber}\"");

                if (!string.IsNullOrEmpty(PagingInfo.PagingCookie))
                {
                    sb.Append($" paging-cookie=\"{PagingInfo.PagingCookie}\"");
                }
            }

            sb.Append(">");

            sb.Append($"<entity name=\"{EntityName}\">");

            foreach (var attribute in Attributes)
            {
                sb.Append($"<attribute name=\"{attribute}\" />");
            }

            foreach (var order in _orders)
            {

                sb.Append($"<order attribute=\"{order.Item1}\" descending=\"{order.Item2.ToString().ToLowerInvariant()}\" />");
            }

            if (Criteria.Conditions.Any() || Criteria.Filters.Any())
            {
                sb.Append(Criteria.ToFetchXmlString());
            }

            foreach (var link in Links)
            {
                sb.Append(link.ToFetchXmlString());
            }

            sb.Append("</entity></fetch>");
            return sb.ToString();
        }

        public override string ToWebApiString(Func<int> aliasIndexer = null)
        {
            var definition = DefinitionCache.GetEntityDefinition(EntityName);

            if (!string.IsNullOrEmpty(PagingInfo.PagingCookie))
            {
                return PagingInfo.PagingCookie.Substring(definition.EntityCollectionName.Length + 1);
            }

            var sb = new StringBuilder();
            sb.Append("$select=");
            foreach (var attribute in Attributes)
            {
                sb.Append(definition.GetWebApiAttributeName(attribute)).Append(",");
            }
            sb.Remove(sb.Length - 1, 1);

            if (Links.Any(l => l.Attributes.Any()))
            {
                sb.Append("&$expand=");
                foreach (var link in Links.Where(l => l.Attributes.Any()))
                {
                    sb.Append(link.ToWebApiString(() => _indexValue++)).Append(",");
                }

                sb.Remove(sb.Length - 1, 1);
            }

            if (Criteria.Conditions.Any() || Criteria.Filters.Any())
            {
                sb.Append("&$filter=");
                sb.Append(Criteria.ToWebApiString(() => _indexValue++));
            }

            if (_orders.Any())
            {
                sb.Append("&$orderby=");
                foreach (var order in _orders)
                {
                    sb.Append($"{definition.GetWebApiAttributeName(order.Item1)} ").Append(order.Item2 ? "desc," : "asc,");
                }

                sb.Remove(sb.Length - 1, 1);
            }

            if (TopCount.HasValue)
            {
                sb.Append($"&$top={TopCount}");
            }

            sb.Append(Criteria.WebApiAliasString);

            return sb.ToString();
        }

        private int _indexValue = 1;

        public PagingInfo PagingInfo { get; } = new PagingInfo();

        public static string ToWebApiValue(AttributeTypeCode attributeType, object value)
        {
            if (value == null)
            {
                return "null";
            }

            switch (attributeType)
            {
                case AttributeTypeCode.Boolean:
                    return value.ToString().ToLowerInvariant();
                case AttributeTypeCode.Decimal:
                case AttributeTypeCode.Double:
                case AttributeTypeCode.Integer:
                case AttributeTypeCode.Money:
                case AttributeTypeCode.Uniqueidentifier:
                case AttributeTypeCode.BigInt:
                    return value.ToString();
                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.State:
                    return ((int)value).ToString();
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                    return value.ToString();
                case AttributeTypeCode.DateTime:
                    return ((DateTime)value).ToString("yyyy/MM/ddTHH:mm:ss");
                case AttributeTypeCode.Memo:
                case AttributeTypeCode.String:
                case AttributeTypeCode.EntityName:
                    return $"'{value}'";
            }

            return string.Empty;
        }
    }
}
