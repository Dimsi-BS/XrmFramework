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
    public static class CustomApiResponsePropertyDefinition
    {
        public const string EntityName = "customapiresponseproperty";
        public const string EntityCollectionName = "customapiresponseproperties";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Picklist (EtatDuComposant)
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(EtatDuComposant))]
            [AlternateKey(AlternateKeyNames.CustomAPIResponsePropertyExportKey)]
            public const string ComponentState = "componentstate";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [AlternateKey(AlternateKeyNames.CustomAPIResponsePropertyExportKey)]
            [CrmLookup(CustomApiDefinition.EntityName, CustomApiDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.customapi_customapiresponseproperty)]
            public const string CustomAPIId = "customapiid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "customapiresponsepropertyid";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsManaged = "ismanaged";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100)]
            public const string LogicalEntityName = "logicalentityname";

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
            [AlternateKey(AlternateKeyNames.CustomAPIResponsePropertyExportKey)]
            [DateTimeBehavior(DateTimeBehavior.UserLocal)]
            public const string OverwriteTime = "overwritetime";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(128)]
            [AlternateKey(AlternateKeyNames.CustomAPIResponsePropertyExportKey)]
            public const string UniqueName = "uniquename";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class AlternateKeyNames
        {
            public const string CustomAPIResponsePropertyExportKey = "custom api response property export key";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
            public const string business_unit_customapiresponseproperty = "business_unit_customapiresponseproperty";
            [Relationship(CustomApiDefinition.EntityName, EntityRole.Referencing, "CustomAPIId", CustomApiResponsePropertyDefinition.Columns.CustomAPIId)]
            public const string customapi_customapiresponseproperty = "customapi_customapiresponseproperty";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_customapiresponseproperty_createdby = "lk_customapiresponseproperty_createdby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_customapiresponseproperty_createdonbehalfby = "lk_customapiresponseproperty_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_customapiresponseproperty_modifiedby = "lk_customapiresponseproperty_modifiedby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_customapiresponseproperty_modifiedonbehalfby = "lk_customapiresponseproperty_modifiedonbehalfby";
            [Relationship("owner", EntityRole.Referencing, "ownerid", "ownerid")]
            public const string owner_customapiresponseproperty = "owner_customapiresponseproperty";
            [Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
            public const string team_customapiresponseproperty = "team_customapiresponseproperty";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
            public const string user_customapiresponseproperty = "user_customapiresponseproperty";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("asyncoperation", EntityRole.Referenced, "customapiresponseproperty_AsyncOperations", "regardingobjectid")]
            public const string customapiresponseproperty_AsyncOperations = "customapiresponseproperty_AsyncOperations";
            [Relationship("bulkdeletefailure", EntityRole.Referenced, "customapiresponseproperty_BulkDeleteFailures", "regardingobjectid")]
            public const string customapiresponseproperty_BulkDeleteFailures = "customapiresponseproperty_BulkDeleteFailures";
            [Relationship("mailboxtrackingfolder", EntityRole.Referenced, "customapiresponseproperty_MailboxTrackingFolders", "regardingobjectid")]
            public const string customapiresponseproperty_MailboxTrackingFolders = "customapiresponseproperty_MailboxTrackingFolders";
            [Relationship("principalobjectattributeaccess", EntityRole.Referenced, "customapiresponseproperty_PrincipalObjectAttributeAccesses", "objectid")]
            public const string customapiresponseproperty_PrincipalObjectAttributeAccesses = "customapiresponseproperty_PrincipalObjectAttributeAccesses";
            [Relationship("processsession", EntityRole.Referenced, "customapiresponseproperty_ProcessSession", "regardingobjectid")]
            public const string customapiresponseproperty_ProcessSession = "customapiresponseproperty_ProcessSession";
            [Relationship("syncerror", EntityRole.Referenced, "customapiresponseproperty_SyncErrors", "regardingobjectid")]
            public const string customapiresponseproperty_SyncErrors = "customapiresponseproperty_SyncErrors";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "customapiresponseproperty_UserEntityInstanceDatas", "objectid")]
            public const string customapiresponseproperty_UserEntityInstanceDatas = "customapiresponseproperty_UserEntityInstanceDatas";
        }
    }
}
