// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.BindingModel.Tests.Fakes
{
    /// <summary>
    /// In-test entity definition for "contact", covering all attribute types exercised by the mapper tests.
    /// </summary>
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static class ContactDefinition
    {
        public const string EntityName = "contact";
        public const string EntityCollectionName = "contacts";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>Primary identifier.</summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "contactid";

            /// <summary>Full name (primary name attribute).</summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            public const string FullName = "fullname";

            /// <summary>Email address.</summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            public const string Email = "emailaddress1";

            /// <summary>Whether the contact is active.</summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsActive = "xrm_isactive";

            /// <summary>Date of birth.</summary>
            [AttributeMetadata(AttributeTypeCode.DateTime)]
            public const string BirthDate = "birthdate";

            /// <summary>Revenue (money field).</summary>
            [AttributeMetadata(AttributeTypeCode.Money)]
            public const string Revenue = "revenue";

            /// <summary>Status code (picklist).</summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            public const string StatusCode = "statuscode";

            /// <summary>Multi-select interests field.</summary>
            [AttributeMetadata(AttributeTypeCode.MultiSelectPicklist)]
            public const string Interests = "xrm_interests";

            /// <summary>Parent account lookup.</summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(AccountDefinition.EntityName, AccountDefinition.Columns.Id,
                RelationshipName = ManyToOneRelationships.contact_customer_accounts)]
            public const string AccountId = "parentcustomerid";

            /// <summary>
            /// Polymorphic "preferred customer" lookup (account or contact). Deliberately left without a
            /// <see cref="CrmLookupAttribute"/>: a plain EntityReference/Guid-typed property must be able
            /// to read/write this column without that metadata (TableSourceFileGenerator does emit it for
            /// Customer/Owner columns too, but a property that doesn't need target-table resolution has
            /// no business requiring it either).
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Customer)]
            public const string PreferredCustomerId = "preferredcustomerid";

            /// <summary>
            /// Another polymorphic "customer" lookup, this time carrying Definition-side metadata for
            /// both possible targets — exactly what TableSourceFileGenerator now emits for a real
            /// Customer/Owner column reaching more than one table. Used to exercise the guard against
            /// mapping it to a writable <see cref="System.Guid"/> property with nothing to say which
            /// table it means.
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Customer)]
            [CrmLookup(AccountDefinition.EntityName, AccountDefinition.Columns.Id,
                RelationshipName = ManyToOneRelationships.contact_backupcustomer_accounts)]
            [CrmLookup(ContactDefinition.EntityName, ContactDefinition.Columns.Id,
                RelationshipName = ManyToOneRelationships.contact_backupcustomer_contacts)]
            public const string BackupCustomerId = "backupcustomerid";

            /// <summary>Alternate key column (e-mail).</summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [AlternateKey("contact_email_key")]
            public const string EmailKey = "emailaddress1_key";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship(AccountDefinition.EntityName, EntityRole.Referencing,
                "contact_customer_accounts", "parentcustomerid")]
            public const string contact_customer_accounts = "contact_customer_accounts";

            [Relationship(AccountDefinition.EntityName, EntityRole.Referencing,
                "contact_backupcustomer_accounts", "backupcustomerid")]
            public const string contact_backupcustomer_accounts = "contact_backupcustomer_accounts";

            [Relationship(ContactDefinition.EntityName, EntityRole.Referencing,
                "contact_backupcustomer_contacts", "backupcustomerid")]
            public const string contact_backupcustomer_contacts = "contact_backupcustomer_contacts";
        }
    }
}
