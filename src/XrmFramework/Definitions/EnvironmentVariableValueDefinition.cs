using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Definitions
{
	[GeneratedCode("XrmFramework", "1.0")]
	[EntityDefinition]

	[DefinitionManagerIgnore]

	[ExcludeFromCodeCoverage]

	public static class EnvironmentVariableValueDefinition
	{
		public const string EntityName = "environmentvariablevalue";
		public const string EntityCollectionName = "environmentvariablevalues";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(EnvironmentVariableDefinition.EntityName, EnvironmentVariableDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.environmentvariabledefinition_environmentvariablevalue)]
			public const string EnvironmentVariableDefinitionId = "environmentvariabledefinitionid";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "environmentvariablevalueid";

			/// <summary>
			/// 
			/// Type : Owner
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Owner)]
			[CrmLookup(SystemUserDefinition.EntityName, SystemUserDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.user_environmentvariablevalue)]
			[CrmLookup(TeamDefinition.EntityName, TeamDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.team_environmentvariablevalue)]
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
			/// Type : State (EnvironmentVariableValueState)
			/// Validity :  Read | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.State)]
			[OptionSet(typeof(EnvironmentVariableValueState))]
			public const string Statecode = "statecode";

			/// <summary>
			/// 
			/// Type : Status (EnvironmentVariableValueStatus)
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Status)]
			[OptionSet(typeof(EnvironmentVariableValueStatus))]
			public const string Statuscode = "statuscode";

			/// <summary>
			/// 
			/// Type : Memo
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Memo)]
			[StringLength(2000)]
			public const string Value = "value";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(BusinessUnitDefinition.EntityName, EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
			public const string business_unit_environmentvariablevalue = "business_unit_environmentvariablevalue";
			[Relationship(EnvironmentVariableDefinition.EntityName, EntityRole.Referencing, "EnvironmentVariableDefinitionId", EnvironmentVariableValueDefinition.Columns.EnvironmentVariableDefinitionId)]
			public const string environmentvariabledefinition_environmentvariablevalue = "environmentvariabledefinition_environmentvariablevalue";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string lk_environmentvariablevalue_createdby = "lk_environmentvariablevalue_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_environmentvariablevalue_createdonbehalfby = "lk_environmentvariablevalue_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_environmentvariablevalue_modifiedby = "lk_environmentvariablevalue_modifiedby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_environmentvariablevalue_modifiedonbehalfby = "lk_environmentvariablevalue_modifiedonbehalfby";
			[Relationship("owner", EntityRole.Referencing, "ownerid", EnvironmentVariableValueDefinition.Columns.OwnerId)]
			public const string owner_environmentvariablevalue = "owner_environmentvariablevalue";
			[Relationship(TeamDefinition.EntityName, EntityRole.Referencing, "owningteam", "owningteam")]
			public const string team_environmentvariablevalue = "team_environmentvariablevalue";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
			public const string user_environmentvariablevalue = "user_environmentvariablevalue";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("asyncoperation", EntityRole.Referenced, "environmentvariablevalue_AsyncOperations", "regardingobjectid")]
			public const string environmentvariablevalue_AsyncOperations = "environmentvariablevalue_AsyncOperations";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "environmentvariablevalue_BulkDeleteFailures", "regardingobjectid")]
			public const string environmentvariablevalue_BulkDeleteFailures = "environmentvariablevalue_BulkDeleteFailures";
			[Relationship("duplicaterecord", EntityRole.Referenced, "environmentvariablevalue_DuplicateBaseRecord", "baserecordid")]
			public const string environmentvariablevalue_DuplicateBaseRecord = "environmentvariablevalue_DuplicateBaseRecord";
			[Relationship("duplicaterecord", EntityRole.Referenced, "environmentvariablevalue_DuplicateMatchingRecord", "duplicaterecordid")]
			public const string environmentvariablevalue_DuplicateMatchingRecord = "environmentvariablevalue_DuplicateMatchingRecord";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "environmentvariablevalue_MailboxTrackingFolders", "regardingobjectid")]
			public const string environmentvariablevalue_MailboxTrackingFolders = "environmentvariablevalue_MailboxTrackingFolders";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "environmentvariablevalue_PrincipalObjectAttributeAccesses", "objectid")]
			public const string environmentvariablevalue_PrincipalObjectAttributeAccesses = "environmentvariablevalue_PrincipalObjectAttributeAccesses";
			[Relationship("processsession", EntityRole.Referenced, "environmentvariablevalue_ProcessSession", "regardingobjectid")]
			public const string environmentvariablevalue_ProcessSession = "environmentvariablevalue_ProcessSession";
			[Relationship("syncerror", EntityRole.Referenced, "environmentvariablevalue_SyncErrors", "regardingobjectid")]
			public const string environmentvariablevalue_SyncErrors = "environmentvariablevalue_SyncErrors";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "environmentvariablevalue_UserEntityInstanceDatas", "objectid")]
			public const string environmentvariablevalue_UserEntityInstanceDatas = "environmentvariablevalue_UserEntityInstanceDatas";
		}
	}

    [OptionSetDefinition(EnvironmentVariableValueDefinition.EntityName, EnvironmentVariableValueDefinition.Columns.Statecode)]
    public enum EnvironmentVariableValueState
    {
        [Description("Active")]
        Active = 0,
        [Description("Inactive")]
        Inactive = 1,
    }

    [OptionSetDefinition(EnvironmentVariableValueDefinition.EntityName, EnvironmentVariableValueDefinition.Columns.Statuscode)]
    public enum EnvironmentVariableValueStatus
    {
        [Description("Active")]
        Active = 1,
        [Description("Inactive")]
        Inactive = 2,
    }
}
