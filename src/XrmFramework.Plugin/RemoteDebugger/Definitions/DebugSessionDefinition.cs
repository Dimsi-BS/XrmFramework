using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Definitions
{
	[GeneratedCode("XrmFramework", "1.0")]
	[EntityDefinition]
	[ExcludeFromCodeCoverage]
	public static class DebugSessionDefinition
	{
		public const string EntityName = "dimsi_debugsession";
		public const string EntityCollectionName = "dimsi_debugsessions";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
			/// <summary>
			/// 
			/// Type : DateTime
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.DateTime)]
			[DateTimeBehavior(DateTimeBehavior.UserLocal)]
			public const string CreatedOn = "createdon";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(SystemUserDefinition.EntityName, SystemUserDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.dimsi_debugsession_user)]
			public const string DebugeeId = "dimsi_debugeeid";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "dimsi_debugsessionid";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100)]
			public const string HybridConnectionName = "dimsi_hybridconnectionname";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[PrimaryAttribute(PrimaryAttributeType.Name)]
			[StringLength(100)]
			public const string Name = "dimsi_name";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100)]
			public const string RelayUrl = "dimsi_relayurl";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100)]
			public const string SasConnectionKey = "dimsi_sasconnectionkey";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100)]
			public const string SasKeyName = "dimsi_saskeyname";

			/// <summary>
			/// 
			/// Type : DateTime
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.DateTime)]
			[DateTimeBehavior(DateTimeBehavior.UserLocal)]
			public const string SessionEnd = "dimsi_sessionend";

			/// <summary>
			/// 
			/// Type : State (DebugSessionState)
			/// Validity :  Read | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.State)]
			[OptionSet(typeof(DebugSessionState))]
			public const string StateCode = "statecode";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(BusinessUnitDefinition.EntityName, EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
			public const string business_unit_dimsi_debugsession = "business_unit_dimsi_debugsession";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "dimsi_DebugeeId", DebugSessionDefinition.Columns.DebugeeId)]
			public const string dimsi_debugsession_user = "dimsi_debugsession_user";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string lk_dimsi_debugsession_createdby = "lk_dimsi_debugsession_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_dimsi_debugsession_createdonbehalfby = "lk_dimsi_debugsession_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_dimsi_debugsession_modifiedby = "lk_dimsi_debugsession_modifiedby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_dimsi_debugsession_modifiedonbehalfby = "lk_dimsi_debugsession_modifiedonbehalfby";
			[Relationship("owner", EntityRole.Referencing, "ownerid", "ownerid")]
			public const string owner_dimsi_debugsession = "owner_dimsi_debugsession";
			[Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
			public const string team_dimsi_debugsession = "team_dimsi_debugsession";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
			public const string user_dimsi_debugsession = "user_dimsi_debugsession";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("asyncoperation", EntityRole.Referenced, "dimsi_debugsession_AsyncOperations", "regardingobjectid")]
			public const string dimsi_debugsession_AsyncOperations = "dimsi_debugsession_AsyncOperations";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "dimsi_debugsession_BulkDeleteFailures", "regardingobjectid")]
			public const string dimsi_debugsession_BulkDeleteFailures = "dimsi_debugsession_BulkDeleteFailures";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "dimsi_debugsession_MailboxTrackingFolders", "regardingobjectid")]
			public const string dimsi_debugsession_MailboxTrackingFolders = "dimsi_debugsession_MailboxTrackingFolders";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "dimsi_debugsession_PrincipalObjectAttributeAccesses", "objectid")]
			public const string dimsi_debugsession_PrincipalObjectAttributeAccesses = "dimsi_debugsession_PrincipalObjectAttributeAccesses";
			[Relationship("processsession", EntityRole.Referenced, "dimsi_debugsession_ProcessSession", "regardingobjectid")]
			public const string dimsi_debugsession_ProcessSession = "dimsi_debugsession_ProcessSession";
			[Relationship("syncerror", EntityRole.Referenced, "dimsi_debugsession_SyncErrors", "regardingobjectid")]
			public const string dimsi_debugsession_SyncErrors = "dimsi_debugsession_SyncErrors";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "dimsi_debugsession_UserEntityInstanceDatas", "objectid")]
			public const string dimsi_debugsession_UserEntityInstanceDatas = "dimsi_debugsession_UserEntityInstanceDatas";
		}
	}


    [OptionSetDefinition(DebugSessionDefinition.EntityName, DebugSessionDefinition.Columns.StateCode)]
    public enum DebugSessionState
    {
        [Description("Active")]
        Active = 0,
        [Description("Inactive")]
        Inactive = 1,
    }
}
