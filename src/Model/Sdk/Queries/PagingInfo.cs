// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Model.Sdk.Queries
{
    public class PagingInfo
    {
        public int PageNumber { get; set; } = 1;

        public int? PageSize { get; set; }

        public string PagingCookie { get; set; }
    }
}
