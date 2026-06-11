// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Tests.Sdk.Queries.Fakes
{
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static class FakeAccountDefinition
    {
        public const string EntityName = "fake_account";
        public const string EntityCollectionName = "fake_accounts";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "fake_accountid";

            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            public const string Name = "name";

            [AttributeMetadata(AttributeTypeCode.String)]
            public const string City = "address1_city";
        }
    }
}
