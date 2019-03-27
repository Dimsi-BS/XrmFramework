// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;

namespace Model.Sdk.Queries
{
    public interface IQueryToString
    {
        string ToFetchXmlString();

        string ToWebApiString(Func<int> aliasIndexer = null);
    }
}
