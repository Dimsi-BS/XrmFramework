using System;
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
	public static class SystemUserDefinition
	{
		public const string EntityName = "systemuser";
		public const string EntityCollectionName = "systemusers";

		public const String TeamMembershipRelationName = "teammembership";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(BusinessUnitDefinition.EntityName, BusinessUnitDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.business_unit_system_users)]
            public const string BusinessUnitId = "businessunitid";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[PrimaryAttribute(PrimaryAttributeType.Name)]
			[StringLength(200)]
			public const string FullName = "fullname";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100)]
			public const string InternalEMailAddress = "internalemailaddress";

			/// <summary>
			/// 
			/// Type : Boolean
			/// Validity :  Read | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Boolean)]
			public const string IsDisabled = "isdisabled";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "systemuserid";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToManyRelationships
		{
			[Relationship("msdyn_resourcerequirement", EntityRole.Referencing, "msdyn_resourcerequirement_systemuser", "msdyn_resourcerequirementid")]
			public const string msdyn_resourcerequirement_systemuser = "msdyn_resourcerequirement_systemuser";
			[Relationship("queue", EntityRole.Referencing, "queuemembership", "queueid")]
			public const string queuemembership_association = "queuemembership_association";
			[Relationship("fieldsecurityprofile", EntityRole.Referencing, "systemuserprofiles", "fieldsecurityprofileid")]
			public const string systemuserprofiles_association = "systemuserprofiles_association";
			[Relationship("role", EntityRole.Referencing, "systemuserroles", "roleid")]
			public const string systemuserroles_association = "systemuserroles_association";
			[Relationship("syncattributemappingprofile", EntityRole.Referencing, "systemusersyncmappingprofiles", "syncattributemappingprofileid")]
			public const string systemusersyncmappingprofiles_association = "systemusersyncmappingprofiles_association";
			[Relationship("team", EntityRole.Referencing, "teammembership", "teamid")]
			public const string teammembership_association = "teammembership_association";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship("businessunit", EntityRole.Referencing, "businessunitid", "businessunitid")]
			public const string business_unit_system_users = "business_unit_system_users";
			[Relationship("calendar", EntityRole.Referencing, "calendarid", "calendarid")]
			public const string calendar_system_users = "calendar_system_users";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_systemuser_createdonbehalfby = "lk_systemuser_createdonbehalfby";
			[Relationship("imagedescriptor", EntityRole.Referencing, "entityimageid_imagedescriptor", "entityimageid")]
			public const string lk_systemuser_entityimage = "lk_systemuser_entityimage";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_systemuser_modifiedonbehalfby = "lk_systemuser_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string lk_systemuserbase_createdby = "lk_systemuserbase_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_systemuserbase_modifiedby = "lk_systemuserbase_modifiedby";
			[Relationship("mobileofflineprofile", EntityRole.Referencing, "mobileofflineprofileid", "mobileofflineprofileid")]
			public const string MobileOfflineProfile_SystemUser = "MobileOfflineProfile_SystemUser";
			[Relationship("organization", EntityRole.Referencing, "organizationid_organization", "organizationid")]
			public const string organization_system_users = "organization_system_users";
			[Relationship("position", EntityRole.Referencing, "positionid", "positionid")]
			public const string position_users = "position_users";
			[Relationship("processstage", EntityRole.Referencing, "stageid_processstage", "stageid")]
			public const string processstage_systemusers = "processstage_systemusers";
			[Relationship("queue", EntityRole.Referencing, "queueid", "queueid")]
			public const string queue_system_user = "queue_system_user";
			[Relationship("site", EntityRole.Referencing, "siteid", "siteid")]
			public const string site_system_users = "site_system_users";
			[Relationship("mailbox", EntityRole.Referencing, "defaultmailbox", "defaultmailbox")]
			public const string systemuser_defaultmailbox_mailbox = "systemuser_defaultmailbox_mailbox";
			[Relationship("territory", EntityRole.Referencing, "territoryid", "territoryid")]
			public const string territory_system_users = "territory_system_users";
			[Relationship("transactioncurrency", EntityRole.Referencing, "transactioncurrencyid", "transactioncurrencyid")]
			public const string TransactionCurrency_SystemUser = "TransactionCurrency_SystemUser";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "parentsystemuserid", "parentsystemuserid")]
			public const string user_parent_user = "user_parent_user";
		}
	}
}
