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
    public static class WebResourceDefinition
    {
        public const string EntityName = "webresource";
        public const string EntityCollectionName = "webresources";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(1073741823)]
            public const string Content = "content";

            /// <summary>
            /// 
            /// Type : Memo
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Memo)]
            [StringLength(1073741823)]
            public const string ContentJson = "contentjson";

            /// <summary>
            /// 
            /// Type : Memo
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Memo)]
            [StringLength(2000)]
            public const string Description = "description";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(200)]
            public const string DisplayName = "displayname";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(256)]
            public const string Name = "name";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup("organization", "organizationid", RelationshipName = "webresource_organization")]
            public const string OrganizationId = "organizationid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            public const string SolutionId = "solutionid";

            /// <summary>
            /// 
            /// Type : BigInt
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.BigInt)]
            public const string VersionNumber = "versionnumber";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "webresourceid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            public const string WebResourceIdUnique = "webresourceidunique";

            /// <summary>
            /// 
            /// Type : Picklist (Type)
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(WebResourceType))]
            public const string WebResourceType = "webresourcetype";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship("fileattachment", EntityRole.Referencing, "ContentFileRef", "contentfileref")]
            public const string FileAttachment_WebResource_ContentFileRef = "FileAttachment_WebResource_ContentFileRef";
            [Relationship("fileattachment", EntityRole.Referencing, "ContentJsonFileRef", "contentjsonfileref")]
            public const string FileAttachment_WebResource_ContentJsonFileRef = "FileAttachment_WebResource_ContentJsonFileRef";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_webresourcebase_createdonbehalfby = "lk_webresourcebase_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_webresourcebase_modifiedonbehalfby = "lk_webresourcebase_modifiedonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string webresource_createdby = "webresource_createdby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string webresource_modifiedby = "webresource_modifiedby";
            [Relationship("organization", EntityRole.Referencing, "organizationid", WebResourceDefinition.Columns.OrganizationId)]
            public const string webresource_organization = "webresource_organization";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("theme", EntityRole.Referenced, "lk_theme_logoid", "logoid")]
            public const string lk_theme_logoid = "lk_theme_logoid";
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referenced, "solution_configuration_webresource", "configurationpageid")]
            public const string solution_configuration_webresource = "solution_configuration_webresource";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_webresource", "objectid")]
            public const string userentityinstancedata_webresource = "userentityinstancedata_webresource";
            [Relationship("appaction", EntityRole.Referenced, "webresource_appaction_iconwebresourceid", "iconwebresourceid")]
            public const string webresource_appaction_iconwebresourceid = "webresource_appaction_iconwebresourceid";
            [Relationship("appaction", EntityRole.Referenced, "webresource_appaction_onclickeventjavascriptwebresourceid", "onclickeventjavascriptwebresourceid")]
            public const string webresource_appaction_onclickeventjavascriptwebresourceid = "webresource_appaction_onclickeventjavascriptwebresourceid";
            [Relationship("fileattachment", EntityRole.Referenced, "webresource_FileAttachments", "objectid")]
            public const string webresource_FileAttachments = "webresource_FileAttachments";
            [Relationship("savedqueryvisualization", EntityRole.Referenced, "webresource_savedqueryvisualizations", "webresourceid")]
            public const string webresource_savedqueryvisualizations = "webresource_savedqueryvisualizations";
            [Relationship("userqueryvisualization", EntityRole.Referenced, "webresource_userqueryvisualizations", "webresourceid")]
            public const string webresource_userqueryvisualizations = "webresource_userqueryvisualizations";
        }
    }
}
