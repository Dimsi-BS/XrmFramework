//HintName: Contact.table.cs
using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

namespace XrmFramework
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static partial class ContactDefinition
    {
        public const string EntityName = "contact";
        public const string EntityCollectionName = "contacts";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(AccountDefinition.EntityName, AccountDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.contact_account)]
            public const string AccountId = "accountid";

            /// <summary>
            /// 
            /// Type : DateTime
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.DateTime)]
            [DateTimeBehavior(DateTimeBehavior.DateOnly)]
            public const string BirthDate = "birthdate";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "contactid";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(160)]
            public const string FullName = "fullname";

            /// <summary>
            /// 
            /// Type : Virtual (ContactInterest)
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Virtual)]
            [OptionSet(typeof(ContactInterest))]
            public const string Interests = "interests";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsActive = "isactive";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            public const string OrphanLookup = "orphanlookup";

            /// <summary>
            /// 
            /// Type : Money
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Money)]
            public const string Revenue = "revenue";

            /// <summary>
            /// 
            /// Type : Status (ContactStatus)
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Status)]
            [OptionSet(typeof(ContactStatus))]
            public const string StatusCode = "statuscode";

        }
        public static class ManyToOneRelationships
        {
            [Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "AccountId", ContactDefinition.Columns.AccountId)]
            public const string contact_account = "contact_account";
        }
    }

    [OptionSetDefinition(ContactDefinition.EntityName, ContactDefinition.Columns.StatusCode)]
    public enum ContactStatus
    {
        Null = 0,
        [Description("Active")]
        Active = 1,
        [Description("Inactive")]
        Inactive = 2,
    }

    [OptionSetDefinition(ContactDefinition.EntityName, ContactDefinition.Columns.Interests)]
    public enum ContactInterest
    {
        [Description("Sports")]
        Sports = 1,
        [Description("Music")]
        Music = 2,
    }
}
