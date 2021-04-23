// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace XrmFramework.Sdk.Queries
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
