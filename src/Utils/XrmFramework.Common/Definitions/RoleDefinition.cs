using Model.Sdk;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Model;

namespace XrmFramework.Common
{
	[GeneratedCode("XrmFramework", "1.0")]
	[EntityDefinition]

	[ExcludeFromCodeCoverage]
    [DefinitionManagerIgnore]

	public static class RoleDefinition
	{
		public const string EntityName = "role";
		public const string EntityCollectionName = "roles";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(BusinessUnitDefinition.EntityName, BusinessUnitDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.business_unit_roles)]
            public const string BusinessUnitId = "businessunitid";

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
			/// Type : Lookup
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(RoleDefinition.EntityName, RoleDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.role_parent_root_role)]
			public const string ParentRootRoleId = "parentrootroleid";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "roleid";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup("roletemplate", "roletemplateid", RelationshipName = "role_template_roles")]
			public const string RoleTemplateId = "roletemplateid";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToManyRelationships
		{
			[Relationship("appmodule", EntityRole.Referencing, "appmoduleroles", "appmoduleid")]
			public const string appmoduleroles_association = "appmoduleroles_association";
			[Relationship("privilege", EntityRole.Referencing, "roleprivileges", "privilegeid")]
			public const string roleprivileges_association = "roleprivileges_association";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "systemuserroles", "systemuserid")]
			public const string systemuserroles_association = "systemuserroles_association";
			[Relationship(TeamDefinition.EntityName, EntityRole.Referencing, "teamroles", "teamid")]
			public const string teamroles_association = "teamroles_association";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(BusinessUnitDefinition.EntityName, EntityRole.Referencing, "businessunitid", "businessunitid")]
			public const string business_unit_roles = "business_unit_roles";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_role_createdonbehalfby = "lk_role_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_role_modifiedonbehalfby = "lk_role_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string lk_rolebase_createdby = "lk_rolebase_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_rolebase_modifiedby = "lk_rolebase_modifiedby";
			[Relationship("organization", EntityRole.Referencing, "organizationid_organization", "organizationid")]
			public const string organization_roles = "organization_roles";
			[Relationship(RoleDefinition.EntityName, EntityRole.Referencing, "parentroleid", "parentroleid")]
			public const string role_parent_role = "role_parent_role";
			[Relationship(RoleDefinition.EntityName, EntityRole.Referencing, "parentrootroleid", RoleDefinition.Columns.ParentRootRoleId)]
			public const string role_parent_root_role = "role_parent_root_role";
			[Relationship("roletemplate", EntityRole.Referencing, "roletemplateid", RoleDefinition.Columns.RoleTemplateId)]
			public const string role_template_roles = "role_template_roles";
			[Relationship("solution", EntityRole.Referencing, "solution_role", "solutionid")]
			public const string solution_role = "solution_role";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("msdyn_actioncardrolesetting", EntityRole.Referenced, "msdyn_role_msdyn_actioncardrolesetting", "msdyn_roleid")]
			public const string lk_msdyn_roleid = "lk_msdyn_roleid";
			[Relationship("asyncoperation", EntityRole.Referenced, "Role_AsyncOperations", "regardingobjectid")]
			public const string Role_AsyncOperations = "Role_AsyncOperations";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "Role_BulkDeleteFailures", "regardingobjectid")]
			public const string Role_BulkDeleteFailures = "Role_BulkDeleteFailures";
			[Relationship(RoleDefinition.EntityName, EntityRole.Referenced, "role_parent_role", "parentroleid")]
			public const string role_parent_role = "role_parent_role";
			[Relationship(RoleDefinition.EntityName, EntityRole.Referenced, "role_parent_root_role", RoleDefinition.Columns.ParentRootRoleId)]
			public const string role_parent_root_role = "role_parent_root_role";
			[Relationship("syncerror", EntityRole.Referenced, "Role_SyncErrors", "regardingobjectid")]
			public const string Role_SyncErrors = "Role_SyncErrors";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_role", "objectid")]
			public const string userentityinstancedata_role = "userentityinstancedata_role";
		}
	}
}
