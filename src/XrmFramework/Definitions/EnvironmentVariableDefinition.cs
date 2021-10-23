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
	public static class EnvironmentVariableDefinition
	{
		public const string EntityName = "environmentvariabledefinition";
		public const string EntityCollectionName = "environmentvariabledefinitions";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
            /// <summary>
            /// 
            /// Type : Memo
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Memo)]
            [StringLength(2000)]
            public const string DefaultValue = "defaultvalue";

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
			[StringLength(100)]
			public const string DisplayName = "displayname";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "environmentvariabledefinitionid";

			/// <summary>
			/// 
			/// Type : Owner
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Owner)]
			[CrmLookup(SystemUserDefinition.EntityName, SystemUserDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.user_environmentvariabledefinition)]
			[CrmLookup(TeamDefinition.EntityName, TeamDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.team_environmentvariabledefinition)]
			public const string OwnerId = "ownerid";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[PrimaryAttribute(PrimaryAttributeType.Name)]
			[StringLength(100)]
			public const string SchemaName = "schemaname";

			/// <summary>
			/// 
			/// Type : State (EnvironmentVariableDefinitionState)
			/// Validity :  Read | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.State)]
			[OptionSet(typeof(EnvironmentVariableDefinitionState))]
			public const string StateCode = "statecode";

			/// <summary>
			/// 
			/// Type : Status (EnvironmentVariableDefinitionStatus)
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Status)]
			[OptionSet(typeof(EnvironmentVariableDefinitionStatus))]
			public const string StatusCode = "statuscode";

			/// <summary>
			/// 
			/// Type : Picklist (EnvironmentVariableType)
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Picklist)]
			[OptionSet(typeof(EnvironmentVariableType))]
			public const string Type = "type";

			/// <summary>
			/// 
			/// Type : Memo
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Memo)]
			[StringLength(2000)]
			public const string ValueSchema = "valueschema";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(BusinessUnitDefinition.EntityName, EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
			public const string business_unit_environmentvariabledefinition = "business_unit_environmentvariabledefinition";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string lk_environmentvariabledefinition_createdby = "lk_environmentvariabledefinition_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_environmentvariabledefinition_createdonbehalfby = "lk_environmentvariabledefinition_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_environmentvariabledefinition_modifiedby = "lk_environmentvariabledefinition_modifiedby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_environmentvariabledefinition_modifiedonbehalfby = "lk_environmentvariabledefinition_modifiedonbehalfby";
			[Relationship("owner", EntityRole.Referencing, "ownerid", EnvironmentVariableDefinition.Columns.OwnerId)]
			public const string owner_environmentvariabledefinition = "owner_environmentvariabledefinition";
			[Relationship(TeamDefinition.EntityName, EntityRole.Referencing, "owningteam", "owningteam")]
			public const string team_environmentvariabledefinition = "team_environmentvariabledefinition";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
			public const string user_environmentvariabledefinition = "user_environmentvariabledefinition";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("asyncoperation", EntityRole.Referenced, "environmentvariabledefinition_AsyncOperations", "regardingobjectid")]
			public const string environmentvariabledefinition_AsyncOperations = "environmentvariabledefinition_AsyncOperations";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "environmentvariabledefinition_BulkDeleteFailures", "regardingobjectid")]
			public const string environmentvariabledefinition_BulkDeleteFailures = "environmentvariabledefinition_BulkDeleteFailures";
			[Relationship("duplicaterecord", EntityRole.Referenced, "environmentvariabledefinition_DuplicateBaseRecord", "baserecordid")]
			public const string environmentvariabledefinition_DuplicateBaseRecord = "environmentvariabledefinition_DuplicateBaseRecord";
			[Relationship("duplicaterecord", EntityRole.Referenced, "environmentvariabledefinition_DuplicateMatchingRecord", "duplicaterecordid")]
			public const string environmentvariabledefinition_DuplicateMatchingRecord = "environmentvariabledefinition_DuplicateMatchingRecord";
			[Relationship(EnvironmentVariableValueDefinition.EntityName, EntityRole.Referenced, "environmentvariabledefinition_environmentvariablevalue", EnvironmentVariableValueDefinition.Columns.EnvironmentVariableDefinitionId)]
			public const string environmentvariabledefinition_environmentvariablevalue = "environmentvariabledefinition_environmentvariablevalue";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "environmentvariabledefinition_MailboxTrackingFolders", "regardingobjectid")]
			public const string environmentvariabledefinition_MailboxTrackingFolders = "environmentvariabledefinition_MailboxTrackingFolders";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "environmentvariabledefinition_PrincipalObjectAttributeAccesses", "objectid")]
			public const string environmentvariabledefinition_PrincipalObjectAttributeAccesses = "environmentvariabledefinition_PrincipalObjectAttributeAccesses";
			[Relationship("processsession", EntityRole.Referenced, "environmentvariabledefinition_ProcessSession", "regardingobjectid")]
			public const string environmentvariabledefinition_ProcessSession = "environmentvariabledefinition_ProcessSession";
			[Relationship("syncerror", EntityRole.Referenced, "environmentvariabledefinition_SyncErrors", "regardingobjectid")]
			public const string environmentvariabledefinition_SyncErrors = "environmentvariabledefinition_SyncErrors";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "environmentvariabledefinition_UserEntityInstanceDatas", "objectid")]
			public const string environmentvariabledefinition_UserEntityInstanceDatas = "environmentvariabledefinition_UserEntityInstanceDatas";
		}
	}

    [OptionSetDefinition(EnvironmentVariableDefinition.EntityName, EnvironmentVariableDefinition.Columns.StateCode)]
    [DefinitionManagerIgnore]
	public enum EnvironmentVariableDefinitionState
    {
        [Description("Active")]
        Active = 0,
        [Description("Inactive")]
        Inactive = 1,
    }

    [OptionSetDefinition(EnvironmentVariableDefinition.EntityName, EnvironmentVariableDefinition.Columns.StatusCode)]
    [DefinitionManagerIgnore]
	public enum EnvironmentVariableDefinitionStatus
    {
        [Description("Active")]
        Active = 1,
        [Description("Inactive")]
        Inactive = 2,
    }

    [OptionSetDefinition(EnvironmentVariableDefinition.EntityName, EnvironmentVariableDefinition.Columns.Type)]
    [DefinitionManagerIgnore]
	public enum EnvironmentVariableType
    {
        Null = 0,
        [Description("String")]
        String = 100000000,
        [Description("Number")]
        Number = 100000001,
        [Description("Boolean")]
        Boolean = 100000002,
        [Description("JSON")]
        Json = 100000003,
        [Description("Connection reference")]
        ConnectionReference = 100000004,
    }
}
