using System;

namespace Model.Sdk.Queries
{
    public interface IQueryToString
    {
        string ToFetchXmlString();

        string ToWebApiString(Func<int> aliasIndexer = null);
    }
}
