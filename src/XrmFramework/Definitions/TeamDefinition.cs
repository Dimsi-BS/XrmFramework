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

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("importfile", EntityRole.Referenced, "ImportFile_Team", "recordsownerid")]
			public const string ImportFile_Team = "ImportFile_Team";
			[Relationship("lead", EntityRole.Referenced, "lead_owning_team", "owningteam")]
			public const string lead_owning_team = "lead_owning_team";
			[Relationship("li_inmail", EntityRole.Referenced, "li_inmail_team_owningteam", "owningteam")]
			public const string li_inmail_team_owningteam = "li_inmail_team_owningteam";
			[Relationship("li_message", EntityRole.Referenced, "li_message_team_owningteam", "owningteam")]
			public const string li_message_team_owningteam = "li_message_team_owningteam";
			[Relationship("li_pointdrivepresentationviewed", EntityRole.Referenced, "li_pointdrivepresentationviewed_team_owningteam", "owningteam")]
			public const string li_pointdrivepresentationviewed_team_owningteam = "li_pointdrivepresentationviewed_team_owningteam";
			[Relationship("msdyn_approval", EntityRole.Referenced, "msdyn_approval_team_owningteam", "owningteam")]
			public const string msdyn_approval_team_owningteam = "msdyn_approval_team_owningteam";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "msdyn_bookingalert_team_owningteam", "owningteam")]
			public const string msdyn_bookingalert_team_owningteam = "msdyn_bookingalert_team_owningteam";
			[Relationship("msdyn_project", EntityRole.Referenced, "msdyn_team_msdyn_project_projectteamid", "msdyn_projectteamid")]
			public const string msdyn_team_msdyn_project_projectteamid = "msdyn_team_msdyn_project_projectteamid";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "msfp_surveyinvite_team_owningteam", "owningteam")]
			public const string msfp_surveyinvite_team_owningteam = "msfp_surveyinvite_team_owningteam";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "msfp_surveyresponse_team_owningteam", "owningteam")]
			public const string msfp_surveyresponse_team_owningteam = "msfp_surveyresponse_team_owningteam";
			[Relationship("postfollow", EntityRole.Referenced, "OwningTeam_postfollows", "owningteam")]
			public const string OwningTeam_postfollows = "OwningTeam_postfollows";
			[Relationship("account", EntityRole.Referenced, "team_accounts", "owningteam")]
			public const string team_accounts = "team_accounts";
			[Relationship("actioncardusersettings", EntityRole.Referenced, "team_actioncardusersettings", "owningteam")]
			public const string team_actioncardusersettings = "team_actioncardusersettings";
			[Relationship("activitypointer", EntityRole.Referenced, "team_activity", "owningteam")]
			public const string team_activity = "team_activity";
			[Relationship("adminsettingsentity", EntityRole.Referenced, "team_adminsettingsentity", "owningteam")]
			public const string team_adminsettingsentity = "team_adminsettingsentity";
			[Relationship("annotation", EntityRole.Referenced, "team_annotations", "owningteam")]
			public const string team_annotations = "team_annotations";
			[Relationship("appointment", EntityRole.Referenced, "team_appointment", "owningteam")]
			public const string team_appointment = "team_appointment";
			[Relationship("asyncoperation", EntityRole.Referenced, "team_asyncoperation", "owningteam")]
			public const string team_asyncoperation = "team_asyncoperation";
			[Relationship("asyncoperation", EntityRole.Referenced, "Team_AsyncOperations", "regardingobjectid")]
			public const string Team_AsyncOperations = "Team_AsyncOperations";
			[Relationship("bookableresource", EntityRole.Referenced, "team_bookableresource", "owningteam")]
			public const string team_bookableresource = "team_bookableresource";
			[Relationship("bookableresourcebooking", EntityRole.Referenced, "team_bookableresourcebooking", "owningteam")]
			public const string team_bookableresourcebooking = "team_bookableresourcebooking";
			[Relationship("bookableresourcebookingexchangesyncidmapping", EntityRole.Referenced, "team_bookableresourcebookingexchangesyncidmapping", "owningteam")]
			public const string team_bookableresourcebookingexchangesyncidmapping = "team_bookableresourcebookingexchangesyncidmapping";
			[Relationship("bookableresourcebookingheader", EntityRole.Referenced, "team_bookableresourcebookingheader", "owningteam")]
			public const string team_bookableresourcebookingheader = "team_bookableresourcebookingheader";
			[Relationship("bookableresourcecategory", EntityRole.Referenced, "team_bookableresourcecategory", "owningteam")]
			public const string team_bookableresourcecategory = "team_bookableresourcecategory";
			[Relationship("bookableresourcecategoryassn", EntityRole.Referenced, "team_bookableresourcecategoryassn", "owningteam")]
			public const string team_bookableresourcecategoryassn = "team_bookableresourcecategoryassn";
			[Relationship("bookableresourcecharacteristic", EntityRole.Referenced, "team_bookableresourcecharacteristic", "owningteam")]
			public const string team_bookableresourcecharacteristic = "team_bookableresourcecharacteristic";
			[Relationship("bookableresourcegroup", EntityRole.Referenced, "team_bookableresourcegroup", "owningteam")]
			public const string team_bookableresourcegroup = "team_bookableresourcegroup";
			[Relationship("bookingstatus", EntityRole.Referenced, "team_bookingstatus", "owningteam")]
			public const string team_bookingstatus = "team_bookingstatus";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "Team_BulkDeleteFailures", "regardingobjectid")]
			public const string Team_BulkDeleteFailures = "Team_BulkDeleteFailures";
			[Relationship("bulkoperation", EntityRole.Referenced, "team_BulkOperation", "owningteam")]
			public const string team_BulkOperation = "team_BulkOperation";
			[Relationship("bulkoperationlog", EntityRole.Referenced, "team_bulkoperationlog", "owningteam")]
			public const string team_bulkoperationlog = "team_bulkoperationlog";
			[Relationship("campaignactivity", EntityRole.Referenced, "team_campaignactivity", "owningteam")]
			public const string team_campaignactivity = "team_campaignactivity";
			[Relationship("campaignresponse", EntityRole.Referenced, "team_campaignresponse", "owningteam")]
			public const string team_campaignresponse = "team_campaignresponse";
			[Relationship("campaign", EntityRole.Referenced, "team_Campaigns", "owningteam")]
			public const string team_Campaigns = "team_Campaigns";
			[Relationship("cgo_intervention", EntityRole.Referenced, "team_cgo_intervention", "owningteam")]
			public const string team_cgo_intervention = "team_cgo_intervention";
			[Relationship("cgo_servicecontract", EntityRole.Referenced, "team_cgo_servicecontract", "owningteam")]
			public const string team_cgo_servicecontract = "team_cgo_servicecontract";
			[Relationship("cgo_testpowerapps", EntityRole.Referenced, "team_cgo_testpowerapps", "owningteam")]
			public const string team_cgo_testpowerapps = "team_cgo_testpowerapps";
			[Relationship("cgo_testunmanaged", EntityRole.Referenced, "team_cgo_testunmanaged", "owningteam")]
			public const string team_cgo_testunmanaged = "team_cgo_testunmanaged";
			[Relationship("channelaccessprofile", EntityRole.Referenced, "team_channelaccessprofile", "owningteam")]
			public const string team_channelaccessprofile = "team_channelaccessprofile";
			[Relationship("characteristic", EntityRole.Referenced, "team_characteristic", "owningteam")]
			public const string team_characteristic = "team_characteristic";
			[Relationship("connection", EntityRole.Referenced, "team_connections1", "record1id")]
			public const string team_connections1 = "team_connections1";
			[Relationship("connection", EntityRole.Referenced, "team_connections2", "record2id")]
			public const string team_connections2 = "team_connections2";
			[Relationship("connector", EntityRole.Referenced, "team_connector", "owningteam")]
			public const string team_connector = "team_connector";
			[Relationship("contact", EntityRole.Referenced, "team_contacts", "owningteam")]
			public const string team_contacts = "team_contacts";
			[Relationship("contractdetail", EntityRole.Referenced, "team_contractdetail", "owningteam")]
			public const string team_contractdetail = "team_contractdetail";
			[Relationship("convertrule", EntityRole.Referenced, "team_convertrule", "owningteam")]
			public const string team_convertrule = "team_convertrule";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "team_customer_opportunity_roles", "owningteam")]
			public const string team_customer_opportunity_roles = "team_customer_opportunity_roles";
			[Relationship("customerrelationship", EntityRole.Referenced, "team_customer_relationship", "owningteam")]
			public const string team_customer_relationship = "team_customer_relationship";
			[Relationship("duplicaterecord", EntityRole.Referenced, "Team_DuplicateBaseRecord", "baserecordid")]
			public const string Team_DuplicateBaseRecord = "Team_DuplicateBaseRecord";
			[Relationship("duplicaterecord", EntityRole.Referenced, "Team_DuplicateMatchingRecord", "duplicaterecordid")]
			public const string Team_DuplicateMatchingRecord = "Team_DuplicateMatchingRecord";
			[Relationship("duplicaterule", EntityRole.Referenced, "team_DuplicateRules", "owningteam")]
			public const string team_DuplicateRules = "team_DuplicateRules";
			[Relationship("dynamicpropertyinstance", EntityRole.Referenced, "team_DynamicPropertyInstance", "owningteam")]
			public const string team_DynamicPropertyInstance = "team_DynamicPropertyInstance";
			[Relationship("email", EntityRole.Referenced, "team_email", "owningteam")]
			public const string team_email = "team_email";
			[Relationship("template", EntityRole.Referenced, "team_email_templates", "owningteam")]
			public const string team_email_templates = "team_email_templates";
			[Relationship("emailserverprofile", EntityRole.Referenced, "team_emailserverprofile", "owningteam")]
			public const string team_emailserverprofile = "team_emailserverprofile";
			[Relationship("entitlement", EntityRole.Referenced, "team_entitlement", "owningteam")]
			public const string team_entitlement = "team_entitlement";
			[Relationship("entitlementchannel", EntityRole.Referenced, "team_entitlementchannel", "owningteam")]
			public const string team_entitlementchannel = "team_entitlementchannel";
			[Relationship("entitlemententityallocationtypemapping", EntityRole.Referenced, "team_entitlemententityallocationtypemapping", "owningteam")]
			public const string team_entitlemententityallocationtypemapping = "team_entitlemententityallocationtypemapping";
			[Relationship("entityanalyticsconfig", EntityRole.Referenced, "team_entityanalyticsconfig", "owningteam")]
			public const string team_entityanalyticsconfig = "team_entityanalyticsconfig";
			[Relationship("exchangesyncidmapping", EntityRole.Referenced, "team_exchangesyncidmapping", "owningteam")]
			public const string team_exchangesyncidmapping = "team_exchangesyncidmapping";
			[Relationship("externalparty", EntityRole.Referenced, "team_externalparty", "owningteam")]
			public const string team_externalparty = "team_externalparty";
			[Relationship("fax", EntityRole.Referenced, "team_fax", "owningteam")]
			public const string team_fax = "team_fax";
			[Relationship("goal", EntityRole.Referenced, "team_goal", "owningteam")]
			public const string team_goal = "team_goal";
			[Relationship("goal", EntityRole.Referenced, "team_goal_goalowner", "goalownerid")]
			public const string team_goal_goalowner = "team_goal_goalowner";
			[Relationship("goalrollupquery", EntityRole.Referenced, "team_goalrollupquery", "owningteam")]
			public const string team_goalrollupquery = "team_goalrollupquery";
			[Relationship("importdata", EntityRole.Referenced, "team_ImportData", "owningteam")]
			public const string team_ImportData = "team_ImportData";
			[Relationship("importfile", EntityRole.Referenced, "team_ImportFiles", "owningteam")]
			public const string team_ImportFiles = "team_ImportFiles";
			[Relationship("importlog", EntityRole.Referenced, "team_ImportLogs", "owningteam")]
			public const string team_ImportLogs = "team_ImportLogs";
			[Relationship("importmap", EntityRole.Referenced, "team_ImportMaps", "owningteam")]
			public const string team_ImportMaps = "team_ImportMaps";
			[Relationship("import", EntityRole.Referenced, "team_Imports", "owningteam")]
			public const string team_Imports = "team_Imports";
			[Relationship("incidentresolution", EntityRole.Referenced, "team_incidentresolution", "owningteam")]
			public const string team_incidentresolution = "team_incidentresolution";
			[Relationship("incident", EntityRole.Referenced, "team_incidents", "owningteam")]
			public const string team_incidents = "team_incidents";
			[Relationship("interactionforemail", EntityRole.Referenced, "team_new_interactionforemail", "owningteam")]
			public const string team_interactionforemail = "team_interactionforemail";
			[Relationship("invoicedetail", EntityRole.Referenced, "team_invoicedetail", "owningteam")]
			public const string team_invoicedetail = "team_invoicedetail";
			[Relationship("invoice", EntityRole.Referenced, "team_invoices", "owningteam")]
			public const string team_invoices = "team_invoices";
			[Relationship("knowledgearticle", EntityRole.Referenced, "team_knowledgearticle", "owningteam")]
			public const string team_knowledgearticle = "team_knowledgearticle";
			[Relationship("knowledgearticleincident", EntityRole.Referenced, "team_knowledgearticleincident", "owningteam")]
			public const string team_knowledgearticleincident = "team_knowledgearticleincident";
			[Relationship("letter", EntityRole.Referenced, "team_letter", "owningteam")]
			public const string team_letter = "team_letter";
			[Relationship("list", EntityRole.Referenced, "team_list", "owningteam")]
			public const string team_list = "team_list";
			[Relationship("mailbox", EntityRole.Referenced, "team_mailbox", "owningteam")]
			public const string team_mailbox = "team_mailbox";
			[Relationship("mailboxtrackingcategory", EntityRole.Referenced, "team_mailboxtrackingcategory", "owningteam")]
			public const string team_mailboxtrackingcategory = "team_mailboxtrackingcategory";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "team_mailboxtrackingfolder", "owningteam")]
			public const string team_mailboxtrackingfolder = "team_mailboxtrackingfolder";
			[Relationship("msdyn_accountpricelist", EntityRole.Referenced, "team_msdyn_accountpricelist", "owningteam")]
			public const string team_msdyn_accountpricelist = "team_msdyn_accountpricelist";
			[Relationship("msdyn_actioncardregarding", EntityRole.Referenced, "team_msdyn_actioncardregarding", "owningteam")]
			public const string team_msdyn_actioncardregarding = "team_msdyn_actioncardregarding";
			[Relationship("msdyn_actioncardrolesetting", EntityRole.Referenced, "team_msdyn_actioncardrolesetting", "owningteam")]
			public const string team_msdyn_actioncardrolesetting = "team_msdyn_actioncardrolesetting";
			[Relationship("msdyn_actual", EntityRole.Referenced, "team_msdyn_actual", "owningteam")]
			public const string team_msdyn_actual = "team_msdyn_actual";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "team_msdyn_agreement", "owningteam")]
			public const string team_msdyn_agreement = "team_msdyn_agreement";
			[Relationship("msdyn_agreementbookingdate", EntityRole.Referenced, "team_msdyn_agreementbookingdate", "owningteam")]
			public const string team_msdyn_agreementbookingdate = "team_msdyn_agreementbookingdate";
			[Relationship("msdyn_agreementbookingincident", EntityRole.Referenced, "team_msdyn_agreementbookingincident", "owningteam")]
			public const string team_msdyn_agreementbookingincident = "team_msdyn_agreementbookingincident";
			[Relationship("msdyn_agreementbookingproduct", EntityRole.Referenced, "team_msdyn_agreementbookingproduct", "owningteam")]
			public const string team_msdyn_agreementbookingproduct = "team_msdyn_agreementbookingproduct";
			[Relationship("msdyn_agreementbookingservice", EntityRole.Referenced, "team_msdyn_agreementbookingservice", "owningteam")]
			public const string team_msdyn_agreementbookingservice = "team_msdyn_agreementbookingservice";
			[Relationship("msdyn_agreementbookingservicetask", EntityRole.Referenced, "team_msdyn_agreementbookingservicetask", "owningteam")]
			public const string team_msdyn_agreementbookingservicetask = "team_msdyn_agreementbookingservicetask";
			[Relationship("msdyn_agreementbookingsetup", EntityRole.Referenced, "team_msdyn_agreementbookingsetup", "owningteam")]
			public const string team_msdyn_agreementbookingsetup = "team_msdyn_agreementbookingsetup";
			[Relationship("msdyn_agreementinvoicedate", EntityRole.Referenced, "team_msdyn_agreementinvoicedate", "owningteam")]
			public const string team_msdyn_agreementinvoicedate = "team_msdyn_agreementinvoicedate";
			[Relationship("msdyn_agreementinvoiceproduct", EntityRole.Referenced, "team_msdyn_agreementinvoiceproduct", "owningteam")]
			public const string team_msdyn_agreementinvoiceproduct = "team_msdyn_agreementinvoiceproduct";
			[Relationship("msdyn_agreementinvoicesetup", EntityRole.Referenced, "team_msdyn_agreementinvoicesetup", "owningteam")]
			public const string team_msdyn_agreementinvoicesetup = "team_msdyn_agreementinvoicesetup";
			[Relationship("msdyn_agreementsubstatus", EntityRole.Referenced, "team_msdyn_agreementsubstatus", "owningteam")]
			public const string team_msdyn_agreementsubstatus = "team_msdyn_agreementsubstatus";
			[Relationship("msdyn_aiconfiguration", EntityRole.Referenced, "team_msdyn_aiconfiguration", "owningteam")]
			public const string team_msdyn_aiconfiguration = "team_msdyn_aiconfiguration";
			[Relationship("msdyn_aifptrainingdocument", EntityRole.Referenced, "team_msdyn_aifptrainingdocument", "owningteam")]
			public const string team_msdyn_aifptrainingdocument = "team_msdyn_aifptrainingdocument";
			[Relationship("msdyn_aimodel", EntityRole.Referenced, "team_msdyn_aimodel", "owningteam")]
			public const string team_msdyn_aimodel = "team_msdyn_aimodel";
			[Relationship("msdyn_aiodimage", EntityRole.Referenced, "team_msdyn_aiodimage", "owningteam")]
			public const string team_msdyn_aiodimage = "team_msdyn_aiodimage";
			[Relationship("msdyn_aiodlabel", EntityRole.Referenced, "team_msdyn_aiodlabel", "owningteam")]
			public const string team_msdyn_aiodlabel = "team_msdyn_aiodlabel";
			[Relationship("msdyn_aiodtrainingboundingbox", EntityRole.Referenced, "team_msdyn_aiodtrainingboundingbox", "owningteam")]
			public const string team_msdyn_aiodtrainingboundingbox = "team_msdyn_aiodtrainingboundingbox";
			[Relationship("msdyn_aiodtrainingimage", EntityRole.Referenced, "team_msdyn_aiodtrainingimage", "owningteam")]
			public const string team_msdyn_aiodtrainingimage = "team_msdyn_aiodtrainingimage";
			[Relationship("msdyn_aitemplate", EntityRole.Referenced, "team_msdyn_aitemplate", "owningteam")]
			public const string team_msdyn_aitemplate = "team_msdyn_aitemplate";
			[Relationship("msdyn_analysiscomponent", EntityRole.Referenced, "team_msdyn_analysiscomponent", "owningteam")]
			public const string team_msdyn_analysiscomponent = "team_msdyn_analysiscomponent";
			[Relationship("msdyn_analysisjob", EntityRole.Referenced, "team_msdyn_analysisjob", "owningteam")]
			public const string team_msdyn_analysisjob = "team_msdyn_analysisjob";
			[Relationship("msdyn_analysisresult", EntityRole.Referenced, "team_msdyn_analysisresult", "owningteam")]
			public const string team_msdyn_analysisresult = "team_msdyn_analysisresult";
			[Relationship("msdyn_analysisresultdetail", EntityRole.Referenced, "team_msdyn_analysisresultdetail", "owningteam")]
			public const string team_msdyn_analysisresultdetail = "team_msdyn_analysisresultdetail";
			[Relationship("msdyn_bookingalertstatus", EntityRole.Referenced, "team_msdyn_bookingalertstatus", "owningteam")]
			public const string team_msdyn_bookingalertstatus = "team_msdyn_bookingalertstatus";
			[Relationship("msdyn_bookingchange", EntityRole.Referenced, "team_msdyn_bookingchange", "owningteam")]
			public const string team_msdyn_bookingchange = "team_msdyn_bookingchange";
			[Relationship("msdyn_bookingjournal", EntityRole.Referenced, "team_msdyn_bookingjournal", "owningteam")]
			public const string team_msdyn_bookingjournal = "team_msdyn_bookingjournal";
			[Relationship("msdyn_bookingrule", EntityRole.Referenced, "team_msdyn_bookingrule", "owningteam")]
			public const string team_msdyn_bookingrule = "team_msdyn_bookingrule";
			[Relationship("msdyn_bookingsetupmetadata", EntityRole.Referenced, "team_msdyn_bookingsetupmetadata", "owningteam")]
			public const string team_msdyn_bookingsetupmetadata = "team_msdyn_bookingsetupmetadata";
			[Relationship("msdyn_bookingtimestamp", EntityRole.Referenced, "team_msdyn_bookingtimestamp", "owningteam")]
			public const string team_msdyn_bookingtimestamp = "team_msdyn_bookingtimestamp";
			[Relationship("msdyn_callablecontext", EntityRole.Referenced, "team_msdyn_callablecontext", "owningteam")]
			public const string team_msdyn_callablecontext = "team_msdyn_callablecontext";
			[Relationship("msdyn_characteristicreqforteammember", EntityRole.Referenced, "team_msdyn_characteristicreqforteammember", "owningteam")]
			public const string team_msdyn_characteristicreqforteammember = "team_msdyn_characteristicreqforteammember";
			[Relationship("msdyn_connector", EntityRole.Referenced, "team_msdyn_connector", "owningteam")]
			public const string team_msdyn_connector = "team_msdyn_connector";
			[Relationship("msdyn_contactpricelist", EntityRole.Referenced, "team_msdyn_contactpricelist", "owningteam")]
			public const string team_msdyn_contactpricelist = "team_msdyn_contactpricelist";
			[Relationship("msdyn_contractlinescheduleofvalue", EntityRole.Referenced, "team_msdyn_contractlinescheduleofvalue", "owningteam")]
			public const string team_msdyn_contractlinescheduleofvalue = "team_msdyn_contractlinescheduleofvalue";
			[Relationship("msdyn_customerasset", EntityRole.Referenced, "team_msdyn_customerasset", "owningteam")]
			public const string team_msdyn_customerasset = "team_msdyn_customerasset";
			[Relationship("msdyn_dataexport", EntityRole.Referenced, "team_msdyn_dataexport", "owningteam")]
			public const string team_msdyn_dataexport = "team_msdyn_dataexport";
			[Relationship("msdyn_delegation", EntityRole.Referenced, "team_msdyn_delegation", "owningteam")]
			public const string team_msdyn_delegation = "team_msdyn_delegation";
			[Relationship("msdyn_entityrankingrule", EntityRole.Referenced, "team_msdyn_entityrankingrule", "owningteam")]
			public const string team_msdyn_entityrankingrule = "team_msdyn_entityrankingrule";
			[Relationship("msdyn_estimate", EntityRole.Referenced, "team_msdyn_estimate", "owningteam")]
			public const string team_msdyn_estimate = "team_msdyn_estimate";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "team_msdyn_estimateline", "owningteam")]
			public const string team_msdyn_estimateline = "team_msdyn_estimateline";
			[Relationship("msdyn_expense", EntityRole.Referenced, "team_msdyn_expense", "owningteam")]
			public const string team_msdyn_expense = "team_msdyn_expense";
			[Relationship("msdyn_expensereceipt", EntityRole.Referenced, "team_msdyn_expensereceipt", "owningteam")]
			public const string team_msdyn_expensereceipt = "team_msdyn_expensereceipt";
			[Relationship("msdyn_fact", EntityRole.Referenced, "team_msdyn_fact", "owningteam")]
			public const string team_msdyn_fact = "team_msdyn_fact";
			[Relationship("msdyn_fieldcomputation", EntityRole.Referenced, "team_msdyn_fieldcomputation", "owningteam")]
			public const string team_msdyn_fieldcomputation = "team_msdyn_fieldcomputation";
			[Relationship("msdyn_fieldservicesetting", EntityRole.Referenced, "team_msdyn_fieldservicesetting", "owningteam")]
			public const string team_msdyn_fieldservicesetting = "team_msdyn_fieldservicesetting";
			[Relationship("msdyn_findworkevent", EntityRole.Referenced, "team_msdyn_findworkevent", "owningteam")]
			public const string team_msdyn_findworkevent = "team_msdyn_findworkevent";
			[Relationship("msdyn_flowcardtype", EntityRole.Referenced, "team_msdyn_flowcardtype", "owningteam")]
			public const string team_msdyn_flowcardtype = "team_msdyn_flowcardtype";
			[Relationship("msdyn_forecastdefinition", EntityRole.Referenced, "team_msdyn_forecastdefinition", "owningteam")]
			public const string team_msdyn_forecastdefinition = "team_msdyn_forecastdefinition";
			[Relationship("msdyn_forecastinstance", EntityRole.Referenced, "team_msdyn_forecastinstance", "owningteam")]
			public const string team_msdyn_forecastinstance = "team_msdyn_forecastinstance";
			[Relationship("msdyn_forecastrecurrence", EntityRole.Referenced, "team_msdyn_forecastrecurrence", "owningteam")]
			public const string team_msdyn_forecastrecurrence = "team_msdyn_forecastrecurrence";
			[Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "team_msdyn_icebreakersconfig", "owningteam")]
			public const string team_msdyn_icebreakersconfig = "team_msdyn_icebreakersconfig";
			[Relationship("msdyn_incidenttype", EntityRole.Referenced, "team_msdyn_incidenttype", "owningteam")]
			public const string team_msdyn_incidenttype = "team_msdyn_incidenttype";
			[Relationship("msdyn_incidenttypecharacteristic", EntityRole.Referenced, "team_msdyn_incidenttypecharacteristic", "owningteam")]
			public const string team_msdyn_incidenttypecharacteristic = "team_msdyn_incidenttypecharacteristic";
			[Relationship("msdyn_incidenttypeproduct", EntityRole.Referenced, "team_msdyn_incidenttypeproduct", "owningteam")]
			public const string team_msdyn_incidenttypeproduct = "team_msdyn_incidenttypeproduct";
			[Relationship("msdyn_incidenttypeservice", EntityRole.Referenced, "team_msdyn_incidenttypeservice", "owningteam")]
			public const string team_msdyn_incidenttypeservice = "team_msdyn_incidenttypeservice";
			[Relationship("msdyn_incidenttypeservicetask", EntityRole.Referenced, "team_msdyn_incidenttypeservicetask", "owningteam")]
			public const string team_msdyn_incidenttypeservicetask = "team_msdyn_incidenttypeservicetask";
			[Relationship("msdyn_integrationjob", EntityRole.Referenced, "team_msdyn_integrationjob", "owningteam")]
			public const string team_msdyn_integrationjob = "team_msdyn_integrationjob";
			[Relationship("msdyn_integrationjobdetail", EntityRole.Referenced, "team_msdyn_integrationjobdetail", "owningteam")]
			public const string team_msdyn_integrationjobdetail = "team_msdyn_integrationjobdetail";
			[Relationship("msdyn_inventoryadjustment", EntityRole.Referenced, "team_msdyn_inventoryadjustment", "owningteam")]
			public const string team_msdyn_inventoryadjustment = "team_msdyn_inventoryadjustment";
			[Relationship("msdyn_inventoryadjustmentproduct", EntityRole.Referenced, "team_msdyn_inventoryadjustmentproduct", "owningteam")]
			public const string team_msdyn_inventoryadjustmentproduct = "team_msdyn_inventoryadjustmentproduct";
			[Relationship("msdyn_inventoryjournal", EntityRole.Referenced, "team_msdyn_inventoryjournal", "owningteam")]
			public const string team_msdyn_inventoryjournal = "team_msdyn_inventoryjournal";
			[Relationship("msdyn_inventorytransfer", EntityRole.Referenced, "team_msdyn_inventorytransfer", "owningteam")]
			public const string team_msdyn_inventorytransfer = "team_msdyn_inventorytransfer";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "team_msdyn_invoicelinetransaction", "owningteam")]
			public const string team_msdyn_invoicelinetransaction = "team_msdyn_invoicelinetransaction";
			[Relationship("msdyn_journal", EntityRole.Referenced, "team_msdyn_journal", "owningteam")]
			public const string team_msdyn_journal = "team_msdyn_journal";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "team_msdyn_journalline", "owningteam")]
			public const string team_msdyn_journalline = "team_msdyn_journalline";
			[Relationship("msdyn_knowledgearticletemplate", EntityRole.Referenced, "team_msdyn_knowledgearticletemplate", "owningteam")]
			public const string team_msdyn_knowledgearticletemplate = "team_msdyn_knowledgearticletemplate";
			[Relationship("msdyn_msteamssetting", EntityRole.Referenced, "team_msdyn_msteamssetting", "owningteam")]
			public const string team_msdyn_msteamssetting = "team_msdyn_msteamssetting";
			[Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "team_msdyn_notesanalysisconfig", "owningteam")]
			public const string team_msdyn_notesanalysisconfig = "team_msdyn_notesanalysisconfig";
			[Relationship("msdyn_opportunitylineresourcecategory", EntityRole.Referenced, "team_msdyn_opportunitylineresourcecategory", "owningteam")]
			public const string team_msdyn_opportunitylineresourcecategory = "team_msdyn_opportunitylineresourcecategory";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "team_msdyn_opportunitylinetransaction", "owningteam")]
			public const string team_msdyn_opportunitylinetransaction = "team_msdyn_opportunitylinetransaction";
			[Relationship("msdyn_opportunitylinetransactioncategory", EntityRole.Referenced, "team_msdyn_opportunitylinetransactioncategory", "owningteam")]
			public const string team_msdyn_opportunitylinetransactioncategory = "team_msdyn_opportunitylinetransactioncategory";
			[Relationship("msdyn_opportunitylinetransactionclassificatio", EntityRole.Referenced, "team_msdyn_opportunitylinetransactionclassificatio", "owningteam")]
			public const string team_msdyn_opportunitylinetransactionclassificatio = "team_msdyn_opportunitylinetransactionclassificatio";
			[Relationship("msdyn_opportunitypricelist", EntityRole.Referenced, "team_msdyn_opportunitypricelist", "owningteam")]
			public const string team_msdyn_opportunitypricelist = "team_msdyn_opportunitypricelist";
			[Relationship("msdyn_orderinvoicingdate", EntityRole.Referenced, "team_msdyn_orderinvoicingdate", "owningteam")]
			public const string team_msdyn_orderinvoicingdate = "team_msdyn_orderinvoicingdate";
			[Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "team_msdyn_orderinvoicingproduct", "owningteam")]
			public const string team_msdyn_orderinvoicingproduct = "team_msdyn_orderinvoicingproduct";
			[Relationship("msdyn_orderinvoicingsetup", EntityRole.Referenced, "team_msdyn_orderinvoicingsetup", "owningteam")]
			public const string team_msdyn_orderinvoicingsetup = "team_msdyn_orderinvoicingsetup";
			[Relationship("msdyn_orderinvoicingsetupdate", EntityRole.Referenced, "team_msdyn_orderinvoicingsetupdate", "owningteam")]
			public const string team_msdyn_orderinvoicingsetupdate = "team_msdyn_orderinvoicingsetupdate";
			[Relationship("msdyn_orderlineresourcecategory", EntityRole.Referenced, "team_msdyn_orderlineresourcecategory", "owningteam")]
			public const string team_msdyn_orderlineresourcecategory = "team_msdyn_orderlineresourcecategory";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "team_msdyn_orderlinetransaction", "owningteam")]
			public const string team_msdyn_orderlinetransaction = "team_msdyn_orderlinetransaction";
			[Relationship("msdyn_orderlinetransactioncategory", EntityRole.Referenced, "team_msdyn_orderlinetransactioncategory", "owningteam")]
			public const string team_msdyn_orderlinetransactioncategory = "team_msdyn_orderlinetransactioncategory";
			[Relationship("msdyn_orderlinetransactionclassification", EntityRole.Referenced, "team_msdyn_orderlinetransactionclassification", "owningteam")]
			public const string team_msdyn_orderlinetransactionclassification = "team_msdyn_orderlinetransactionclassification";
			[Relationship("msdyn_orderpricelist", EntityRole.Referenced, "team_msdyn_orderpricelist", "owningteam")]
			public const string team_msdyn_orderpricelist = "team_msdyn_orderpricelist";
			[Relationship("msdyn_payment", EntityRole.Referenced, "team_msdyn_payment", "owningteam")]
			public const string team_msdyn_payment = "team_msdyn_payment";
			[Relationship("msdyn_paymentdetail", EntityRole.Referenced, "team_msdyn_paymentdetail", "owningteam")]
			public const string team_msdyn_paymentdetail = "team_msdyn_paymentdetail";
			[Relationship("msdyn_paymentmethod", EntityRole.Referenced, "team_msdyn_paymentmethod", "owningteam")]
			public const string team_msdyn_paymentmethod = "team_msdyn_paymentmethod";
			[Relationship("msdyn_paymentterm", EntityRole.Referenced, "team_msdyn_paymentterm", "owningteam")]
			public const string team_msdyn_paymentterm = "team_msdyn_paymentterm";
			[Relationship("msdyn_playbookactivity", EntityRole.Referenced, "team_msdyn_playbookactivity", "owningteam")]
			public const string team_msdyn_playbookactivity = "team_msdyn_playbookactivity";
			[Relationship("msdyn_playbookactivityattribute", EntityRole.Referenced, "team_msdyn_playbookactivityattribute", "owningteam")]
			public const string team_msdyn_playbookactivityattribute = "team_msdyn_playbookactivityattribute";
			[Relationship("msdyn_playbookcategory", EntityRole.Referenced, "team_msdyn_playbookcategory", "owningteam")]
			public const string team_msdyn_playbookcategory = "team_msdyn_playbookcategory";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "team_msdyn_playbookinstance", "owningteam")]
			public const string team_msdyn_playbookinstance = "team_msdyn_playbookinstance";
			[Relationship("msdyn_playbooktemplate", EntityRole.Referenced, "team_msdyn_playbooktemplate", "owningteam")]
			public const string team_msdyn_playbooktemplate = "team_msdyn_playbooktemplate";
			[Relationship("msdyn_postalbum", EntityRole.Referenced, "team_msdyn_postalbum", "owningteam")]
			public const string team_msdyn_postalbum = "team_msdyn_postalbum";
			[Relationship("msdyn_postalcode", EntityRole.Referenced, "team_msdyn_postalcode", "owningteam")]
			public const string team_msdyn_postalcode = "team_msdyn_postalcode";
			[Relationship("msdyn_priority", EntityRole.Referenced, "team_msdyn_priority", "owningteam")]
			public const string team_msdyn_priority = "team_msdyn_priority";
			[Relationship("msdyn_project", EntityRole.Referenced, "team_msdyn_project", "owningteam")]
			public const string team_msdyn_project = "team_msdyn_project";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "team_msdyn_projectapproval", "owningteam")]
			public const string team_msdyn_projectapproval = "team_msdyn_projectapproval";
			[Relationship("msdyn_projectpricelist", EntityRole.Referenced, "team_msdyn_projectpricelist", "owningteam")]
			public const string team_msdyn_projectpricelist = "team_msdyn_projectpricelist";
			[Relationship("msdyn_projecttask", EntityRole.Referenced, "team_msdyn_projecttask", "owningteam")]
			public const string team_msdyn_projecttask = "team_msdyn_projecttask";
			[Relationship("msdyn_projecttaskdependency", EntityRole.Referenced, "team_msdyn_projecttaskdependency", "owningteam")]
			public const string team_msdyn_projecttaskdependency = "team_msdyn_projecttaskdependency";
			[Relationship("msdyn_projecttaskstatususer", EntityRole.Referenced, "team_msdyn_projecttaskstatususer", "owningteam")]
			public const string team_msdyn_projecttaskstatususer = "team_msdyn_projecttaskstatususer";
			[Relationship("msdyn_projectteam", EntityRole.Referenced, "team_msdyn_projectteam", "owningteam")]
			public const string team_msdyn_projectteam = "team_msdyn_projectteam";
			[Relationship("msdyn_projecttransactioncategory", EntityRole.Referenced, "team_msdyn_projecttransactioncategory", "owningteam")]
			public const string team_msdyn_projecttransactioncategory = "team_msdyn_projecttransactioncategory";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "team_msdyn_purchaseorder", "owningteam")]
			public const string team_msdyn_purchaseorder = "team_msdyn_purchaseorder";
			[Relationship("msdyn_purchaseorderbill", EntityRole.Referenced, "team_msdyn_purchaseorderbill", "owningteam")]
			public const string team_msdyn_purchaseorderbill = "team_msdyn_purchaseorderbill";
			[Relationship("msdyn_purchaseorderproduct", EntityRole.Referenced, "team_msdyn_purchaseorderproduct", "owningteam")]
			public const string team_msdyn_purchaseorderproduct = "team_msdyn_purchaseorderproduct";
			[Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "team_msdyn_purchaseorderreceipt", "owningteam")]
			public const string team_msdyn_purchaseorderreceipt = "team_msdyn_purchaseorderreceipt";
			[Relationship("msdyn_purchaseorderreceiptproduct", EntityRole.Referenced, "team_msdyn_purchaseorderreceiptproduct", "owningteam")]
			public const string team_msdyn_purchaseorderreceiptproduct = "team_msdyn_purchaseorderreceiptproduct";
			[Relationship("msdyn_purchaseordersubstatus", EntityRole.Referenced, "team_msdyn_purchaseordersubstatus", "owningteam")]
			public const string team_msdyn_purchaseordersubstatus = "team_msdyn_purchaseordersubstatus";
			[Relationship("msdyn_quotebookingincident", EntityRole.Referenced, "team_msdyn_quotebookingincident", "owningteam")]
			public const string team_msdyn_quotebookingincident = "team_msdyn_quotebookingincident";
			[Relationship("msdyn_quotebookingproduct", EntityRole.Referenced, "team_msdyn_quotebookingproduct", "owningteam")]
			public const string team_msdyn_quotebookingproduct = "team_msdyn_quotebookingproduct";
			[Relationship("msdyn_quotebookingservice", EntityRole.Referenced, "team_msdyn_quotebookingservice", "owningteam")]
			public const string team_msdyn_quotebookingservice = "team_msdyn_quotebookingservice";
			[Relationship("msdyn_quotebookingservicetask", EntityRole.Referenced, "team_msdyn_quotebookingservicetask", "owningteam")]
			public const string team_msdyn_quotebookingservicetask = "team_msdyn_quotebookingservicetask";
			[Relationship("msdyn_quotebookingsetup", EntityRole.Referenced, "team_msdyn_quotebookingsetup", "owningteam")]
			public const string team_msdyn_quotebookingsetup = "team_msdyn_quotebookingsetup";
			[Relationship("msdyn_quoteinvoicingproduct", EntityRole.Referenced, "team_msdyn_quoteinvoicingproduct", "owningteam")]
			public const string team_msdyn_quoteinvoicingproduct = "team_msdyn_quoteinvoicingproduct";
			[Relationship("msdyn_quoteinvoicingsetup", EntityRole.Referenced, "team_msdyn_quoteinvoicingsetup", "owningteam")]
			public const string team_msdyn_quoteinvoicingsetup = "team_msdyn_quoteinvoicingsetup";
			[Relationship("msdyn_quotelineanalyticsbreakdown", EntityRole.Referenced, "team_msdyn_quotelineanalyticsbreakdown", "owningteam")]
			public const string team_msdyn_quotelineanalyticsbreakdown = "team_msdyn_quotelineanalyticsbreakdown";
			[Relationship("msdyn_quotelineresourcecategory", EntityRole.Referenced, "team_msdyn_quotelineresourcecategory", "owningteam")]
			public const string team_msdyn_quotelineresourcecategory = "team_msdyn_quotelineresourcecategory";
			[Relationship("msdyn_quotelinescheduleofvalue", EntityRole.Referenced, "team_msdyn_quotelinescheduleofvalue", "owningteam")]
			public const string team_msdyn_quotelinescheduleofvalue = "team_msdyn_quotelinescheduleofvalue";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "team_msdyn_quotelinetransaction", "owningteam")]
			public const string team_msdyn_quotelinetransaction = "team_msdyn_quotelinetransaction";
			[Relationship("msdyn_quotelinetransactioncategory", EntityRole.Referenced, "team_msdyn_quotelinetransactioncategory", "owningteam")]
			public const string team_msdyn_quotelinetransactioncategory = "team_msdyn_quotelinetransactioncategory";
			[Relationship("msdyn_quotelinetransactionclassification", EntityRole.Referenced, "team_msdyn_quotelinetransactionclassification", "owningteam")]
			public const string team_msdyn_quotelinetransactionclassification = "team_msdyn_quotelinetransactionclassification";
			[Relationship("msdyn_quotepricelist", EntityRole.Referenced, "team_msdyn_quotepricelist", "owningteam")]
			public const string team_msdyn_quotepricelist = "team_msdyn_quotepricelist";
			[Relationship("msdyn_relationshipinsightsunifiedconfig", EntityRole.Referenced, "team_msdyn_relationshipinsightsunifiedconfig", "owningteam")]
			public const string team_msdyn_relationshipinsightsunifiedconfig = "team_msdyn_relationshipinsightsunifiedconfig";
			[Relationship("msdyn_requirementcharacteristic", EntityRole.Referenced, "team_msdyn_requirementcharacteristic", "owningteam")]
			public const string team_msdyn_requirementcharacteristic = "team_msdyn_requirementcharacteristic";
			[Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "team_msdyn_requirementorganizationunit", "owningteam")]
			public const string team_msdyn_requirementorganizationunit = "team_msdyn_requirementorganizationunit";
			[Relationship("msdyn_requirementresourcecategory", EntityRole.Referenced, "team_msdyn_requirementresourcecategory", "owningteam")]
			public const string team_msdyn_requirementresourcecategory = "team_msdyn_requirementresourcecategory";
			[Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "team_msdyn_requirementresourcepreference", "owningteam")]
			public const string team_msdyn_requirementresourcepreference = "team_msdyn_requirementresourcepreference";
			[Relationship("msdyn_requirementstatus", EntityRole.Referenced, "team_msdyn_requirementstatus", "owningteam")]
			public const string team_msdyn_requirementstatus = "team_msdyn_requirementstatus";
			[Relationship("msdyn_resourceassignment", EntityRole.Referenced, "team_msdyn_resourceassignment", "owningteam")]
			public const string team_msdyn_resourceassignment = "team_msdyn_resourceassignment";
			[Relationship("msdyn_resourceassignmentdetail", EntityRole.Referenced, "team_msdyn_resourceassignmentdetail", "owningteam")]
			public const string team_msdyn_resourceassignmentdetail = "team_msdyn_resourceassignmentdetail";
			[Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "team_msdyn_resourcepaytype", "owningteam")]
			public const string team_msdyn_resourcepaytype = "team_msdyn_resourcepaytype";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "team_msdyn_resourcerequest", "owningteam")]
			public const string team_msdyn_resourcerequest = "team_msdyn_resourcerequest";
			[Relationship("msdyn_resourcerequirement", EntityRole.Referenced, "team_msdyn_resourcerequirement", "owningteam")]
			public const string team_msdyn_resourcerequirement = "team_msdyn_resourcerequirement";
			[Relationship("msdyn_resourcerequirementdetail", EntityRole.Referenced, "team_msdyn_resourcerequirementdetail", "owningteam")]
			public const string team_msdyn_resourcerequirementdetail = "team_msdyn_resourcerequirementdetail";
			[Relationship("msdyn_resourceterritory", EntityRole.Referenced, "team_msdyn_resourceterritory", "owningteam")]
			public const string team_msdyn_resourceterritory = "team_msdyn_resourceterritory";
			[Relationship("msdyn_rma", EntityRole.Referenced, "team_msdyn_rma", "owningteam")]
			public const string team_msdyn_rma = "team_msdyn_rma";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "team_msdyn_rmaproduct", "owningteam")]
			public const string team_msdyn_rmaproduct = "team_msdyn_rmaproduct";
			[Relationship("msdyn_rmareceipt", EntityRole.Referenced, "team_msdyn_rmareceipt", "owningteam")]
			public const string team_msdyn_rmareceipt = "team_msdyn_rmareceipt";
			[Relationship("msdyn_rmareceiptproduct", EntityRole.Referenced, "team_msdyn_rmareceiptproduct", "owningteam")]
			public const string team_msdyn_rmareceiptproduct = "team_msdyn_rmareceiptproduct";
			[Relationship("msdyn_rmasubstatus", EntityRole.Referenced, "team_msdyn_rmasubstatus", "owningteam")]
			public const string team_msdyn_rmasubstatus = "team_msdyn_rmasubstatus";
			[Relationship("msdyn_rolecompetencyrequirement", EntityRole.Referenced, "team_msdyn_rolecompetencyrequirement", "owningteam")]
			public const string team_msdyn_rolecompetencyrequirement = "team_msdyn_rolecompetencyrequirement";
			[Relationship("msdyn_roleutilization", EntityRole.Referenced, "team_msdyn_roleutilization", "owningteam")]
			public const string team_msdyn_roleutilization = "team_msdyn_roleutilization";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "team_msdyn_rtv", "owningteam")]
			public const string team_msdyn_rtv = "team_msdyn_rtv";
			[Relationship("msdyn_rtvproduct", EntityRole.Referenced, "team_msdyn_rtvproduct", "owningteam")]
			public const string team_msdyn_rtvproduct = "team_msdyn_rtvproduct";
			[Relationship("msdyn_rtvsubstatus", EntityRole.Referenced, "team_msdyn_rtvsubstatus", "owningteam")]
			public const string team_msdyn_rtvsubstatus = "team_msdyn_rtvsubstatus";
			[Relationship("msdyn_salesinsightssettings", EntityRole.Referenced, "team_msdyn_salesinsightssettings", "owningteam")]
			public const string team_msdyn_salesinsightssettings = "team_msdyn_salesinsightssettings";
			[Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "team_msdyn_scheduleboardsetting", "owningteam")]
			public const string team_msdyn_scheduleboardsetting = "team_msdyn_scheduleboardsetting";
			[Relationship("msdyn_servicetasktype", EntityRole.Referenced, "team_msdyn_servicetasktype", "owningteam")]
			public const string team_msdyn_servicetasktype = "team_msdyn_servicetasktype";
			[Relationship("msdyn_shipvia", EntityRole.Referenced, "team_msdyn_shipvia", "owningteam")]
			public const string team_msdyn_shipvia = "team_msdyn_shipvia";
			[Relationship("msdyn_siconfig", EntityRole.Referenced, "team_msdyn_siconfig", "owningteam")]
			public const string team_msdyn_siconfig = "team_msdyn_siconfig";
			[Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "team_msdyn_solutionhealthrule", "owningteam")]
			public const string team_msdyn_solutionhealthrule = "team_msdyn_solutionhealthrule";
			[Relationship("msdyn_solutionhealthruleargument", EntityRole.Referenced, "team_msdyn_solutionhealthruleargument", "owningteam")]
			public const string team_msdyn_solutionhealthruleargument = "team_msdyn_solutionhealthruleargument";
			[Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "team_msdyn_systemuserschedulersetting", "owningteam")]
			public const string team_msdyn_systemuserschedulersetting = "team_msdyn_systemuserschedulersetting";
			[Relationship("msdyn_taxcode", EntityRole.Referenced, "team_msdyn_taxcode", "owningteam")]
			public const string team_msdyn_taxcode = "team_msdyn_taxcode";
			[Relationship("msdyn_taxcodedetail", EntityRole.Referenced, "team_msdyn_taxcodedetail", "owningteam")]
			public const string team_msdyn_taxcodedetail = "team_msdyn_taxcodedetail";
			[Relationship("msdyn_timeentry", EntityRole.Referenced, "team_msdyn_timeentry", "owningteam")]
			public const string team_msdyn_timeentry = "team_msdyn_timeentry";
			[Relationship("msdyn_timegroup", EntityRole.Referenced, "team_msdyn_timegroup", "owningteam")]
			public const string team_msdyn_timegroup = "team_msdyn_timegroup";
			[Relationship("msdyn_timegroupdetail", EntityRole.Referenced, "team_msdyn_timegroupdetail", "owningteam")]
			public const string team_msdyn_timegroupdetail = "team_msdyn_timegroupdetail";
			[Relationship("msdyn_timeoffcalendar", EntityRole.Referenced, "team_msdyn_timeoffcalendar", "owningteam")]
			public const string team_msdyn_timeoffcalendar = "team_msdyn_timeoffcalendar";
			[Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "team_msdyn_timeoffrequest", "owningteam")]
			public const string team_msdyn_timeoffrequest = "team_msdyn_timeoffrequest";
			[Relationship("msdyn_transactionconnection", EntityRole.Referenced, "team_msdyn_transactionconnection", "owningteam")]
			public const string team_msdyn_transactionconnection = "team_msdyn_transactionconnection";
			[Relationship("msdyn_transactionorigin", EntityRole.Referenced, "team_msdyn_transactionorigin", "owningteam")]
			public const string team_msdyn_transactionorigin = "team_msdyn_transactionorigin";
			[Relationship("msdyn_untrackedappointment", EntityRole.Referenced, "team_msdyn_untrackedappointment", "owningteam")]
			public const string team_msdyn_untrackedappointment = "team_msdyn_untrackedappointment";
			[Relationship("msdyn_userworkhistory", EntityRole.Referenced, "team_msdyn_userworkhistory", "owningteam")]
			public const string team_msdyn_userworkhistory = "team_msdyn_userworkhistory";
			[Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "team_msdyn_wallsavedqueryusersettings", "owningteam")]
			public const string team_msdyn_wallsavedqueryusersettings = "team_msdyn_wallsavedqueryusersettings";
			[Relationship("msdyn_warehouse", EntityRole.Referenced, "team_msdyn_warehouse", "owningteam")]
			public const string team_msdyn_warehouse = "team_msdyn_warehouse";
			[Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "team_msdyn_workhourtemplate", "owningteam")]
			public const string team_msdyn_workhourtemplate = "team_msdyn_workhourtemplate";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "team_msdyn_workorder", "owningteam")]
			public const string team_msdyn_workorder = "team_msdyn_workorder";
			[Relationship("msdyn_workordercharacteristic", EntityRole.Referenced, "team_msdyn_workordercharacteristic", "owningteam")]
			public const string team_msdyn_workordercharacteristic = "team_msdyn_workordercharacteristic";
			[Relationship("msdyn_workorderincident", EntityRole.Referenced, "team_msdyn_workorderincident", "owningteam")]
			public const string team_msdyn_workorderincident = "team_msdyn_workorderincident";
			[Relationship("msdyn_workorderproduct", EntityRole.Referenced, "team_msdyn_workorderproduct", "owningteam")]
			public const string team_msdyn_workorderproduct = "team_msdyn_workorderproduct";
			[Relationship("msdyn_workorderresourcerestriction", EntityRole.Referenced, "team_msdyn_workorderresourcerestriction", "owningteam")]
			public const string team_msdyn_workorderresourcerestriction = "team_msdyn_workorderresourcerestriction";
			[Relationship("msdyn_workorderservice", EntityRole.Referenced, "team_msdyn_workorderservice", "owningteam")]
			public const string team_msdyn_workorderservice = "team_msdyn_workorderservice";
			[Relationship("msdyn_workorderservicetask", EntityRole.Referenced, "team_msdyn_workorderservicetask", "owningteam")]
			public const string team_msdyn_workorderservicetask = "team_msdyn_workorderservicetask";
			[Relationship("msdyn_workordersubstatus", EntityRole.Referenced, "team_msdyn_workordersubstatus", "owningteam")]
			public const string team_msdyn_workordersubstatus = "team_msdyn_workordersubstatus";
			[Relationship("msdyn_workordertype", EntityRole.Referenced, "team_msdyn_workordertype", "owningteam")]
			public const string team_msdyn_workordertype = "team_msdyn_workordertype";
			[Relationship("msfp_emailtemplate", EntityRole.Referenced, "team_msfp_emailtemplate", "owningteam")]
			public const string team_msfp_emailtemplate = "team_msfp_emailtemplate";
			[Relationship("msfp_question", EntityRole.Referenced, "team_msfp_question", "owningteam")]
			public const string team_msfp_question = "team_msfp_question";
			[Relationship("msfp_questionresponse", EntityRole.Referenced, "team_msfp_questionresponse", "owningteam")]
			public const string team_msfp_questionresponse = "team_msfp_questionresponse";
			[Relationship("msfp_survey", EntityRole.Referenced, "team_msfp_survey", "owningteam")]
			public const string team_msfp_survey = "team_msfp_survey";
			[Relationship("msfp_unsubscribedrecipient", EntityRole.Referenced, "team_msfp_unsubscribedrecipient", "owningteam")]
			public const string team_msfp_unsubscribedrecipient = "team_msfp_unsubscribedrecipient";
			[Relationship("opportunity", EntityRole.Referenced, "team_opportunities", "owningteam")]
			public const string team_opportunities = "team_opportunities";
			[Relationship("opportunityclose", EntityRole.Referenced, "team_opportunityclose", "owningteam")]
			public const string team_opportunityclose = "team_opportunityclose";
			[Relationship("opportunityproduct", EntityRole.Referenced, "team_opportunityproduct", "owningteam")]
			public const string team_opportunityproduct = "team_opportunityproduct";
			[Relationship("orderclose", EntityRole.Referenced, "team_orderclose", "owningteam")]
			public const string team_orderclose = "team_orderclose";
			[Relationship("salesorder", EntityRole.Referenced, "team_orders", "owningteam")]
			public const string team_orders = "team_orders";
			[Relationship("phonecall", EntityRole.Referenced, "team_phonecall", "owningteam")]
			public const string team_phonecall = "team_phonecall";
			[Relationship("postregarding", EntityRole.Referenced, "team_PostRegardings", "regardingobjectid")]
			public const string team_PostRegardings = "team_PostRegardings";
			[Relationship("postrole", EntityRole.Referenced, "team_PostRoles", "regardingobjectid")]
			public const string team_PostRoles = "team_PostRoles";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "team_principalobjectattributeaccess", "objectid")]
			public const string team_principalobjectattributeaccess = "team_principalobjectattributeaccess";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "team_principalobjectattributeaccess_principalid", "principalid")]
			public const string team_principalobjectattributeaccess_principalid = "team_principalobjectattributeaccess_principalid";
			[Relationship("processsession", EntityRole.Referenced, "team_processsession", "owningteam")]
			public const string team_processsession = "team_processsession";
			[Relationship("processsession", EntityRole.Referenced, "Team_ProcessSessions", "regardingobjectid")]
			public const string Team_ProcessSessions = "Team_ProcessSessions";
			[Relationship("channelaccessprofilerule", EntityRole.Referenced, "team_profilerule", "owningteam")]
			public const string team_profilerule = "team_profilerule";
			[Relationship("queueitem", EntityRole.Referenced, "team_queueitembase_workerid", "workerid")]
			public const string team_queueitembase_workerid = "team_queueitembase_workerid";
			[Relationship("quoteclose", EntityRole.Referenced, "team_quoteclose", "owningteam")]
			public const string team_quoteclose = "team_quoteclose";
			[Relationship("quotedetail", EntityRole.Referenced, "team_quotedetail", "owningteam")]
			public const string team_quotedetail = "team_quotedetail";
			[Relationship("quote", EntityRole.Referenced, "team_quotes", "owningteam")]
			public const string team_quotes = "team_quotes";
			[Relationship("ratingmodel", EntityRole.Referenced, "team_ratingmodel", "owningteam")]
			public const string team_ratingmodel = "team_ratingmodel";
			[Relationship("ratingvalue", EntityRole.Referenced, "team_ratingvalue", "owningteam")]
			public const string team_ratingvalue = "team_ratingvalue";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "team_recurringappointmentmaster", "owningteam")]
			public const string team_recurringappointmentmaster = "team_recurringappointmentmaster";
			[Relationship("resourcegroup", EntityRole.Referenced, "team_resource_groups", "resourcegroupid")]
			public const string team_resource_groups = "team_resource_groups";
			[Relationship("resourcespec", EntityRole.Referenced, "team_resource_specs", "groupobjectid")]
			public const string team_resource_specs = "team_resource_specs";
			[Relationship("routingrule", EntityRole.Referenced, "team_routingrule", "owningteam")]
			public const string team_routingrule = "team_routingrule";
			[Relationship("routingruleitem", EntityRole.Referenced, "team_routingruleitem", "assignobjectid")]
			public const string team_routingruleitem = "team_routingruleitem";
			[Relationship("sales_linkedin_profileassociations", EntityRole.Referenced, "team_sales_linkedin_profileassociations", "owningteam")]
			public const string team_sales_linkedin_profileassociations = "team_sales_linkedin_profileassociations";
			[Relationship("salesorderdetail", EntityRole.Referenced, "team_salesorderdetail", "owningteam")]
			public const string team_salesorderdetail = "team_salesorderdetail";
			[Relationship("serviceappointment", EntityRole.Referenced, "team_service_appointments", "owningteam")]
			public const string team_service_appointments = "team_service_appointments";
			[Relationship("contract", EntityRole.Referenced, "team_service_contracts", "owningteam")]
			public const string team_service_contracts = "team_service_contracts";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "team_sharepointdocumentlocation", "owningteam")]
			public const string team_sharepointdocumentlocation = "team_sharepointdocumentlocation";
			[Relationship("sharepointsite", EntityRole.Referenced, "team_sharepointsite", "owningteam")]
			public const string team_sharepointsite = "team_sharepointsite";
			[Relationship("sla", EntityRole.Referenced, "team_slaBase", "owningteam")]
			public const string team_slaBase = "team_slaBase";
			[Relationship("socialactivity", EntityRole.Referenced, "team_socialactivity", "owningteam")]
			public const string team_socialactivity = "team_socialactivity";
			[Relationship("syncerror", EntityRole.Referenced, "team_SyncError", "owningteam")]
			public const string team_SyncError = "team_SyncError";
			[Relationship("syncerror", EntityRole.Referenced, "Team_SyncErrors", "regardingobjectid")]
			public const string Team_SyncErrors = "Team_SyncErrors";
			[Relationship("task", EntityRole.Referenced, "team_task", "owningteam")]
			public const string team_task = "team_task";
			[Relationship("tit_compteurparametrage", EntityRole.Referenced, "team_tit_compteurparametrage", "")]
			public const string team_tit_compteurparametrage = "team_tit_compteurparametrage";
			[Relationship("tit_compteurrequests", EntityRole.Referenced, "team_tit_compteurrequests", "")]
			public const string team_tit_compteurrequests = "team_tit_compteurrequests";
			[Relationship("tit_concurrentlocker", EntityRole.Referenced, "team_tit_concurrentlocker", "")]
			public const string team_tit_concurrentlocker = "team_tit_concurrentlocker";
			[Relationship("tit_counter", EntityRole.Referenced, "team_tit_counter", "")]
			public const string team_tit_counter = "team_tit_counter";
			[Relationship("tit_mappingfluxentrant", EntityRole.Referenced, "team_tit_mappingfluxentrant", "")]
			public const string team_tit_mappingfluxentrant = "team_tit_mappingfluxentrant";
			[Relationship("tit_smssetup", EntityRole.Referenced, "team_tit_smssetup", "")]
			public const string team_tit_smssetup = "team_tit_smssetup";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "team_userentityinstancedata", "owningteam")]
			public const string team_userentityinstancedata = "team_userentityinstancedata";
			[Relationship("userentityuisettings", EntityRole.Referenced, "team_userentityuisettings", "owningteam")]
			public const string team_userentityuisettings = "team_userentityuisettings";
			[Relationship("userform", EntityRole.Referenced, "team_userform", "owningteam")]
			public const string team_userform = "team_userform";
			[Relationship("userquery", EntityRole.Referenced, "team_userquery", "owningteam")]
			public const string team_userquery = "team_userquery";
			[Relationship("userqueryvisualization", EntityRole.Referenced, "team_userqueryvisualizations", "owningteam")]
			public const string team_userqueryvisualizations = "team_userqueryvisualizations";
			[Relationship("workflow", EntityRole.Referenced, "team_workflow", "owningteam")]
			public const string team_workflow = "team_workflow";
			[Relationship("workflowlog", EntityRole.Referenced, "team_workflowlog", "owningteam")]
			public const string team_workflowlog = "team_workflowlog";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_team", "objectid")]
			public const string userentityinstancedata_team = "userentityinstancedata_team";
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
