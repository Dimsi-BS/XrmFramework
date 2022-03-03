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
    public static class CustomApiDefinition
    {
        public const string EntityName = "customapi";
        public const string EntityCollectionName = "customapis";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Picklist (BindingType)
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(BindingType))]
            public const string BindingType = "bindingtype";

            /// <summary>
            /// 
            /// Type : Picklist (EtatDuComposant)
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(EtatDuComposant))]
            [AlternateKey(AlternateKeyNames.CustomAPIExportKey)]
            public const string ComponentState = "componentstate";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "customapiid";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100)]
            public const string DisplayName = "displayname";

            /// <summary>
            /// 
            /// Type : ManagedProperty
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.ManagedProperty)]
            public const string IsCustomizable = "iscustomizable";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsFunction = "isfunction";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsManaged = "ismanaged";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsPrivate = "isprivate";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(100)]
            public const string Name = "name";

            /// <summary>
            /// 
            /// Type : DateTime
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.DateTime)]
            [AlternateKey(AlternateKeyNames.CustomAPIExportKey)]
            [DateTimeBehavior(DateTimeBehavior.UserLocal)]
            public const string OverwriteTime = "overwritetime";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(PluginTypeDefinition.EntityName, PluginTypeDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.plugintype_customapi)]
            public const string PluginTypeId = "plugintypeid";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(SdkMessageDefinition.EntityName, SdkMessageDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.sdkmessage_customapi)]
            public const string SdkMessageId = "sdkmessageid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            public const string SolutionId = "solutionid";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(128)]
            [AlternateKey(AlternateKeyNames.CustomAPIExportKey)]
            public const string UniqueName = "uniquename";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class AlternateKeyNames
        {
            public const string CustomAPIExportKey = "custom api export key";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
            public const string business_unit_customapi = "business_unit_customapi";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_customapi_createdby = "lk_customapi_createdby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_customapi_createdonbehalfby = "lk_customapi_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_customapi_modifiedby = "lk_customapi_modifiedby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_customapi_modifiedonbehalfby = "lk_customapi_modifiedonbehalfby";
            [Relationship("owner", EntityRole.Referencing, "ownerid", "ownerid")]
            public const string owner_customapi = "owner_customapi";
            [Relationship(PluginTypeDefinition.EntityName, EntityRole.Referencing, "PluginTypeId", CustomApiDefinition.Columns.PluginTypeId)]
            public const string plugintype_customapi = "plugintype_customapi";
            [Relationship(SdkMessageDefinition.EntityName, EntityRole.Referencing, "SdkMessageId", CustomApiDefinition.Columns.SdkMessageId)]
            public const string sdkmessage_customapi = "sdkmessage_customapi";
            [Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
            public const string team_customapi = "team_customapi";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
            public const string user_customapi = "user_customapi";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("catalogassignment", EntityRole.Referenced, "CatalogAssignments", "object")]
            public const string catalogassignment_customapi = "catalogassignment_customapi";
            [Relationship("asyncoperation", EntityRole.Referenced, "customapi_AsyncOperations", "regardingobjectid")]
            public const string customapi_AsyncOperations = "customapi_AsyncOperations";
            [Relationship("bulkdeletefailure", EntityRole.Referenced, "customapi_BulkDeleteFailures", "regardingobjectid")]
            public const string customapi_BulkDeleteFailures = "customapi_BulkDeleteFailures";
            [Relationship(CustomApiRequestParameterDefinition.EntityName, EntityRole.Referenced, "CustomAPIRequestParameters", CustomApiRequestParameterDefinition.Columns.CustomAPIId)]
            public const string customapi_customapirequestparameter = "customapi_customapirequestparameter";
            [Relationship(CustomApiResponsePropertyDefinition.EntityName, EntityRole.Referenced, "CustomAPIResponseProperties", CustomApiResponsePropertyDefinition.Columns.CustomAPIId)]
            public const string customapi_customapiresponseproperty = "customapi_customapiresponseproperty";
            [Relationship("mailboxtrackingfolder", EntityRole.Referenced, "customapi_MailboxTrackingFolders", "regardingobjectid")]
            public const string customapi_MailboxTrackingFolders = "customapi_MailboxTrackingFolders";
            [Relationship("principalobjectattributeaccess", EntityRole.Referenced, "customapi_PrincipalObjectAttributeAccesses", "objectid")]
            public const string customapi_PrincipalObjectAttributeAccesses = "customapi_PrincipalObjectAttributeAccesses";
            [Relationship("processsession", EntityRole.Referenced, "customapi_ProcessSession", "regardingobjectid")]
            public const string customapi_ProcessSession = "customapi_ProcessSession";
            [Relationship("syncerror", EntityRole.Referenced, "customapi_SyncErrors", "regardingobjectid")]
            public const string customapi_SyncErrors = "customapi_SyncErrors";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "customapi_UserEntityInstanceDatas", "objectid")]
            public const string customapi_UserEntityInstanceDatas = "customapi_UserEntityInstanceDatas";
        }
    }
}
