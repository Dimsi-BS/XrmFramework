using System.Collections.Generic;

namespace Model.Sdk.Queries
{
    public interface IQueryObject
    {
        ICollection<string> Attributes { get; }

        QueryCriteria Criteria { get; }

        void AddColumn(string columnName);

        void AddColumns(params string[] columnNames);

        IList<Link> Links { get; }

        Link AddLink(string toEntityName, string fromAttributeName, string toAttributeName, JoinOperator joinOperator = JoinOperator.Inner);
    }
}
