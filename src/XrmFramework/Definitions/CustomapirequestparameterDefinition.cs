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
    public static class CustomApiRequestParameterDefinition
    {
        public const string EntityName = "customapirequestparameter";
        public const string EntityCollectionName = "customapirequestparameters";

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
            [AlternateKey(AlternateKeyNames.CustomAPIRequestParameterExportKey)]
            public const string ComponentState = "componentstate";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [AlternateKey(AlternateKeyNames.CustomAPIRequestParameterExportKey)]
            [CrmLookup(CustomApiDefinition.EntityName, CustomApiDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.customapi_customapirequestparameter)]
            public const string CustomAPIId = "customapiid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "customapirequestparameterid";

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
            /// Validity :  Read | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsManaged = "ismanaged";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsOptional = "isoptional";

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
            [AlternateKey(AlternateKeyNames.CustomAPIRequestParameterExportKey)]
            [DateTimeBehavior(DateTimeBehavior.UserLocal)]
            public const string OverwriteTime = "overwritetime";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(128)]
            [AlternateKey(AlternateKeyNames.CustomAPIRequestParameterExportKey)]
            public const string UniqueName = "uniquename";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class AlternateKeyNames
        {
            public const string CustomAPIRequestParameterExportKey = "custom api request parameter export key";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
            public const string business_unit_customapirequestparameter = "business_unit_customapirequestparameter";
            [Relationship(CustomApiDefinition.EntityName, EntityRole.Referencing, "CustomAPIId", CustomApiRequestParameterDefinition.Columns.CustomAPIId)]
            public const string customapi_customapirequestparameter = "customapi_customapirequestparameter";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_customapirequestparameter_createdby = "lk_customapirequestparameter_createdby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_customapirequestparameter_createdonbehalfby = "lk_customapirequestparameter_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_customapirequestparameter_modifiedby = "lk_customapirequestparameter_modifiedby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_customapirequestparameter_modifiedonbehalfby = "lk_customapirequestparameter_modifiedonbehalfby";
            [Relationship("owner", EntityRole.Referencing, "ownerid", "ownerid")]
            public const string owner_customapirequestparameter = "owner_customapirequestparameter";
            [Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
            public const string team_customapirequestparameter = "team_customapirequestparameter";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
            public const string user_customapirequestparameter = "user_customapirequestparameter";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("asyncoperation", EntityRole.Referenced, "customapirequestparameter_AsyncOperations", "regardingobjectid")]
            public const string customapirequestparameter_AsyncOperations = "customapirequestparameter_AsyncOperations";
            [Relationship("bulkdeletefailure", EntityRole.Referenced, "customapirequestparameter_BulkDeleteFailures", "regardingobjectid")]
            public const string customapirequestparameter_BulkDeleteFailures = "customapirequestparameter_BulkDeleteFailures";
            [Relationship("mailboxtrackingfolder", EntityRole.Referenced, "customapirequestparameter_MailboxTrackingFolders", "regardingobjectid")]
            public const string customapirequestparameter_MailboxTrackingFolders = "customapirequestparameter_MailboxTrackingFolders";
            [Relationship("principalobjectattributeaccess", EntityRole.Referenced, "customapirequestparameter_PrincipalObjectAttributeAccesses", "objectid")]
            public const string customapirequestparameter_PrincipalObjectAttributeAccesses = "customapirequestparameter_PrincipalObjectAttributeAccesses";
            [Relationship("processsession", EntityRole.Referenced, "customapirequestparameter_ProcessSession", "regardingobjectid")]
            public const string customapirequestparameter_ProcessSession = "customapirequestparameter_ProcessSession";
            [Relationship("syncerror", EntityRole.Referenced, "customapirequestparameter_SyncErrors", "regardingobjectid")]
            public const string customapirequestparameter_SyncErrors = "customapirequestparameter_SyncErrors";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "customapirequestparameter_UserEntityInstanceDatas", "objectid")]
            public const string customapirequestparameter_UserEntityInstanceDatas = "customapirequestparameter_UserEntityInstanceDatas";
        }
    }
}
