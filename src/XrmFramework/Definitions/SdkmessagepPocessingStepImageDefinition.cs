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
    [DefinitionManagerIgnore]
    public static class SdkMessageProcessingStepImageDefinition
    {
        public const string EntityName = "sdkmessageprocessingstepimage";
        public const string EntityCollectionName = "sdkmessageprocessingstepimages";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100000)]
            public const string Attributes = "attributes";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(256)]
            public const string EntityAlias = "entityalias";

            /// <summary>
            /// 
            /// Type : Picklist (ImageType)
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(ImageType))]
            public const string ImageType = "imagetype";

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
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "sdkmessageprocessingstepimageid";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(SdkMessageProcessingStepDefinition.EntityName, SdkMessageProcessingStepDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.sdkmessageprocessingstepid_sdkmessageprocessingstepimage)]
            public const string SdkMessageProcessingStepId = "sdkmessageprocessingstepid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            public const string SdkMessageProcessingStepImageIdUnique = "sdkmessageprocessingstepimageidunique";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string createdby_sdkmessageprocessingstepimage = "createdby_sdkmessageprocessingstepimage";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_sdkmessageprocessingstepimage_createdonbehalfby = "lk_sdkmessageprocessingstepimage_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_sdkmessageprocessingstepimage_modifiedonbehalfby = "lk_sdkmessageprocessingstepimage_modifiedonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string modifiedby_sdkmessageprocessingstepimage = "modifiedby_sdkmessageprocessingstepimage";
            [Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
            public const string organization_sdkmessageprocessingstepimage = "organization_sdkmessageprocessingstepimage";
            [Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referencing, "sdkmessageprocessingstepid", "sdkmessageprocessingstepid")]
            public const string sdkmessageprocessingstepid_sdkmessageprocessingstepimage = "sdkmessageprocessingstepid_sdkmessageprocessingstepimage";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_sdkmessageprocessingstepimage", "objectid")]
            public const string userentityinstancedata_sdkmessageprocessingstepimage = "userentityinstancedata_sdkmessageprocessingstepimage";
        }
    }

    [OptionSetDefinition(SdkMessageProcessingStepImageDefinition.EntityName, SdkMessageProcessingStepImageDefinition.Columns.ImageType)]
    [DefinitionManagerIgnore]
    public enum ImageType
    {
        [Description("PreImage")]
        Preimage = 0,
        [Description("PostImage")]
        Postimage = 1,
        [Description("Both")]
        Both = 2,
    }
}
