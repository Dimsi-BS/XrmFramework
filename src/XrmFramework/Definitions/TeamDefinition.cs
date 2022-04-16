using System;
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
	public static class TeamDefinition
	{
		public const string EntityName = "team";
		public const string EntityCollectionName = "teams";

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
			[CrmLookup(BusinessUnitDefinition.EntityName, BusinessUnitDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.business_unit_teams)]
			public const string BusinessUnitId = "businessunitid";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[PrimaryAttribute(PrimaryAttributeType.Name)]
			[StringLength(160)]
			public const string Name = "name";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup("knowledgearticle", "knowledgearticleid", RelationshipName = "knowledgearticle_Teams")]
			[CrmLookup("opportunity", "opportunityid", RelationshipName = "opportunity_Teams")]
			public const string RegardingObjectId = "regardingobjectid";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "teamid";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup("teamtemplate", "teamtemplateid", RelationshipName = "teamtemplate_Teams")]
			public const string TeamTemplateId = "teamtemplateid";

			/// <summary>
			/// 
			/// Type : Picklist (TypeDEquipe)
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Picklist)]
			[OptionSet(typeof(TypeDEquipe))]
			public const string TeamType = "teamtype";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToManyRelationships
		{
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "teammembership", "systemuserid")]
			public const string teammembership_association = "teammembership_association";
			[Relationship("fieldsecurityprofile", EntityRole.Referencing, "teamprofiles", "fieldsecurityprofileid")]
			public const string teamprofiles_association = "teamprofiles_association";
			[Relationship(RoleDefinition.EntityName, EntityRole.Referencing, "teamroles", "roleid")]
			public const string teamroles_association = "teamroles_association";
			[Relationship("syncattributemappingprofile", EntityRole.Referencing, "teamsyncattributemappingprofiles", "syncattributemappingprofileid")]
			public const string teamsyncattributemappingprofiles_association = "teamsyncattributemappingprofiles_association";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(BusinessUnitDefinition.EntityName, EntityRole.Referencing, "businessunitid", TeamDefinition.Columns.BusinessUnitId)]
			public const string business_unit_teams = "business_unit_teams";
			[Relationship("contact", EntityRole.Referencing, "regardingobjectid_contact", "")]
			public const string contact_Teams = "contact_Teams";
			[Relationship("knowledgearticle", EntityRole.Referencing, "regardingobjectid_knowledgearticle", TeamDefinition.Columns.RegardingObjectId)]
			public const string knowledgearticle_Teams = "knowledgearticle_Teams";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_team_createdonbehalfby = "lk_team_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_team_modifiedonbehalfby = "lk_team_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "administratorid", "administratorid")]
			public const string lk_teambase_administratorid = "lk_teambase_administratorid";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string lk_teambase_createdby = "lk_teambase_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_teambase_modifiedby = "lk_teambase_modifiedby";
			[Relationship("opportunity", EntityRole.Referencing, "regardingobjectid_opportunity", TeamDefinition.Columns.RegardingObjectId)]
			public const string opportunity_Teams = "opportunity_Teams";
			[Relationship("organization", EntityRole.Referencing, "organizationid_organization", "organizationid")]
			public const string organization_teams = "organization_teams";
			[Relationship("processstage", EntityRole.Referencing, "stageid_processstage", "stageid")]
			public const string processstage_teams = "processstage_teams";
			[Relationship("queue", EntityRole.Referencing, "queueid", "queueid")]
			public const string queue_team = "queue_team";
			[Relationship("teamtemplate", EntityRole.Referencing, "associatedteamtemplateid", TeamDefinition.Columns.TeamTemplateId)]
			public const string teamtemplate_Teams = "teamtemplate_Teams";
			[Relationship("transactioncurrency", EntityRole.Referencing, "transactioncurrencyid", "transactioncurrencyid")]
			public const string TransactionCurrency_Team = "TransactionCurrency_Team";
		}

	}

	[OptionSetDefinition(TeamDefinition.EntityName, TeamDefinition.Columns.TeamType)]
	[DefinitionManagerIgnore]
	public enum TypeDEquipe
	{
		[Description("Owner")]
		Proprietaire = 0,
		[Description("Access")]
		Acces = 1,
		[Description("AAD Security Group")]
		GroupeDeSecurite_AAD = 2,
		[Description("AAD Office Group")]
		GroupeOffice_AAD = 3,
	}
}
