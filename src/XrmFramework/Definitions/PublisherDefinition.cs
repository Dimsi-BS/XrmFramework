using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using XrmFramework.Definitions.Internal;

namespace XrmFramework.Definitions
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static class PublisherDefinition
    {
        public const string EntityName = "publisher";
        public const string EntityCollectionName = "publishers";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(8)]
            public const string CustomizationPrefix = "customizationprefix";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(256)]
            public const string FriendlyName = "friendlyname";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "publisherid";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_publisher_createdby = "lk_publisher_createdby";
            [Relationship("imagedescriptor", EntityRole.Referencing, "entityimageid_imagedescriptor", "entityimageid")]
            public const string lk_publisher_entityimage = "lk_publisher_entityimage";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_publisher_modifiedby = "lk_publisher_modifiedby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_publisherbase_createdonbehalfby = "lk_publisherbase_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_publisherbase_modifiedonbehalfby = "lk_publisherbase_modifiedonbehalfby";
            [Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
            public const string organization_publisher = "organization_publisher";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("appmodule", EntityRole.Referenced, "publisher_appmodule", "publisherid")]
            public const string publisher_appmodule = "publisher_appmodule";
            [Relationship("duplicaterecord", EntityRole.Referenced, "Publisher_DuplicateBaseRecord", "baserecordid")]
            public const string Publisher_DuplicateBaseRecord = "Publisher_DuplicateBaseRecord";
            [Relationship("duplicaterecord", EntityRole.Referenced, "Publisher_DuplicateMatchingRecord", "duplicaterecordid")]
            public const string Publisher_DuplicateMatchingRecord = "Publisher_DuplicateMatchingRecord";
            [Relationship("publisheraddress", EntityRole.Referenced, "Publisher_PublisherAddress", "parentid")]
            public const string Publisher_PublisherAddress = "Publisher_PublisherAddress";
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referenced, "publisher_solution", SolutionDefinition.Columns.PublisherId)]
            public const string publisher_solution = "publisher_solution";
            [Relationship("syncerror", EntityRole.Referenced, "Publisher_SyncErrors", "regardingobjectid")]
            public const string Publisher_SyncErrors = "Publisher_SyncErrors";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_publisher", "objectid")]
            public const string userentityinstancedata_publisher = "userentityinstancedata_publisher";
        }
    }
}
