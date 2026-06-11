// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Tests.Sdk.Queries.Fakes
{
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static class FakeContactDefinition
    {
        public const string EntityName = "fake_contact";
        public const string EntityCollectionName = "fake_contacts";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "fake_contactid";

            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            public const string FullName = "fullname";

            [AttributeMetadata(AttributeTypeCode.String)]
            public const string Email = "emailaddress1";

            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsActive = "xrm_isactive";

            [AttributeMetadata(AttributeTypeCode.DateTime)]
            public const string BirthDate = "birthdate";

            [AttributeMetadata(AttributeTypeCode.Money)]
            public const string Revenue = "revenue";

            [AttributeMetadata(AttributeTypeCode.Picklist)]
            public const string StatusCode = "statuscode";

            [AttributeMetadata(AttributeTypeCode.Integer)]
            public const string Age = "xrm_age";

            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(FakeAccountDefinition.EntityName, FakeAccountDefinition.Columns.Id,
                RelationshipName = ManyToOneRelationships.contact_customer_accounts)]
            public const string FakeAccountId = "parentcustomerid";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship(FakeAccountDefinition.EntityName, EntityRole.Referencing,
                "contact_customer_accounts_nav", "parentcustomerid")]
            public const string contact_customer_accounts = "contact_customer_accounts";
        }
    }
}
