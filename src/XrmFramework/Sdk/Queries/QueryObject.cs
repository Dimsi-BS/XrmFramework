// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.Sdk.Queries
{
    public abstract class QueryObject : IQueryToString
    {
        public string EntityName { get; }

        public ICollection<string> Attributes { get; } = new SortedSet<string>();

        public QueryCriteria Criteria { get; protected set; }

        public IList<Link> Links { get; } = new List<Link>();

        protected QueryObject(string entityName)
        {
            EntityName = entityName;
        }

        public Link AddLink(string toEntityName, string fromAttributeName, string toAttributeName, JoinOperator joinOperator = JoinOperator.Inner, bool addColumns = true)
        {
            var link = new Link(EntityName, toEntityName, fromAttributeName, toAttributeName, joinOperator, addColumns);

            Links.Add(link);
            return link;
        }

        public void AddColumn(string columnName)
        {
            Attributes.Add(columnName);
        }

        public void AddColumns(params string[] columnNames)
        {
            foreach (var columnName in columnNames)
            {
                Attributes.Add(columnName);
            }
        }

        public abstract string ToFetchXmlString();

        public abstract string ToWebApiString(Func<int> aliasIndexer = null);

        internal void CleanLinks()
        {
            for (var i = Links.Count - 1; i >= 0; i--)
            {
                var link = Links[i];
                if (link.CanBeRemoved)
                {
                    Links.Remove(link);
                }
                else
                {
                    link.CleanLinks();
                }
            }
        }
    }
}
