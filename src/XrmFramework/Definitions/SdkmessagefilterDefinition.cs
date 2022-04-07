using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using XrmFramework.Definitions.Internal;

namespace XrmFramework.Definitions
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    [DefinitionManagerIgnore]
    public static class SdkMessageFilterDefinition
    {
        public const string EntityName = "sdkmessagefilter";
        public const string EntityCollectionName = "sdkmessagefilters";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsCustomProcessingStepAllowed = "iscustomprocessingstepallowed";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsManaged = "ismanaged";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsVisible = "isvisible";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(256)]
            public const string Name = "name";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "sdkmessagefilterid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            public const string SdkMessageFilterIdUnique = "sdkmessagefilteridunique";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(SdkMessageDefinition.EntityName, SdkMessageDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.sdkmessageid_sdkmessagefilter)]
            public const string SdkMessageId = "sdkmessageid";

            /// <summary>
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind | Dunno
            /// (Not automatically generated, some metadata my be wrong)
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            public const string PrimaryObjectTypeCode = "primaryobjecttypecode";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string createdby_sdkmessagefilter = "createdby_sdkmessagefilter";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_sdkmessagefilter_createdonbehalfby = "lk_sdkmessagefilter_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_sdkmessagefilter_modifiedonbehalfby = "lk_sdkmessagefilter_modifiedonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string modifiedby_sdkmessagefilter = "modifiedby_sdkmessagefilter";
            [Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
            public const string organization_sdkmessagefilter = "organization_sdkmessagefilter";
            [Relationship(SdkMessageDefinition.EntityName, EntityRole.Referencing, "sdkmessageid", SdkMessageFilterDefinition.Columns.SdkMessageId)]
            public const string sdkmessageid_sdkmessagefilter = "sdkmessageid_sdkmessagefilter";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("internalcatalogassignment", EntityRole.Referenced, "InternalCatalogAssignmentId", "object")]
            public const string sdkmessagefilter_internalcatalogassignment = "sdkmessagefilter_internalcatalogassignment";
            [Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "sdkmessagefilterid_sdkmessageprocessingstep", SdkMessageProcessingStepDefinition.Columns.SdkMessageFilterId)]
            public const string sdkmessagefilterid_sdkmessageprocessingstep = "sdkmessagefilterid_sdkmessageprocessingstep";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_sdkmessagefilter", "objectid")]
            public const string userentityinstancedata_sdkmessagefilter = "userentityinstancedata_sdkmessagefilter";
        }
    }
}
