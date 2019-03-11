using Model.Sdk;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Model
{
	[GeneratedCode("XrmFramework", "1.0")]
	[EntityDefinition]

	[ExcludeFromCodeCoverage]

	public static class SystemUserDefinition
	{
		public const string EntityName = "systemuser";
		public const string EntityCollectionName = "systemusers";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
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
			/// Type : 
			/// Validity :  
			/// </summary>
			public const string Referenceexterne = "pchmcs_referenceexterne";

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
		public static class AlternateKeyNames
		{
			public const string SystemuserReferenceExterne = "pchmcs_systemuserreferenceexterne";
			public const string UserKey = "pchmcs_userkey";
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
			[Relationship("businessunit", EntityRole.Referencing, "pchmcs_agencedaffectationid", "")]
			public const string pchmcs_businessunit_systemuser_Agencedaffectation = "pchmcs_businessunit_systemuser_Agencedaffectation";
			[Relationship("businessunit", EntityRole.Referencing, "pchmcs_allotementderniereagenceid", "")]
			public const string pchmcs_businessunit_systemuser_AllotementderniereAgenceId = "pchmcs_businessunit_systemuser_AllotementderniereAgenceId";
			[Relationship("team", EntityRole.Referencing, "pchmcs_HierarchyTeamId", "")]
			public const string pchmcs_hierarchyteam_systemuser = "pchmcs_hierarchyteam_systemuser";
			[Relationship("pchmcs_fonctionutilisateur", EntityRole.Referencing, "pchmcs_fonctioncommuneid", "")]
			public const string pchmcs_pchmcs_fonctionutilisateur_systemuser = "pchmcs_pchmcs_fonctionutilisateur_systemuser";
			[Relationship("pchmcs_masterbusinessunit", EntityRole.Referencing, "pchmcs_masterbusinessunitid", "")]
			public const string pchmcs_pchmcs_masterbusinessunit_systemuser_MasterBusinessUnitId = "pchmcs_pchmcs_masterbusinessunit_systemuser_MasterBusinessUnitId";
			[Relationship("pchmcs_postalcodecity", EntityRole.Referencing, "pchmcs_cpvilleid", "")]
			public const string pchmcs_pchmcs_postalcodecity_systemuser_CPVille = "pchmcs_pchmcs_postalcodecity_systemuser_CPVille";
			[Relationship("sharepointdocumentlocation", EntityRole.Referencing, "pchmcs_contactdocumentlocationid", "")]
			public const string pchmcs_sharepointdocumentlocation_systemuser_contactdocumentlocationid = "pchmcs_sharepointdocumentlocation_systemuser_contactdocumentlocationid";
			[Relationship("team", EntityRole.Referencing, "pchmcs_contactscontributorid", "")]
			public const string pchmcs_team_systemuser_ContactsContributors = "pchmcs_team_systemuser_ContactsContributors";
			[Relationship("team", EntityRole.Referencing, "pchmcs_contactsownerid", "")]
			public const string pchmcs_team_systemuser_ContactsOwnerId = "pchmcs_team_systemuser_ContactsOwnerId";
			[Relationship("team", EntityRole.Referencing, "pchmcs_contactsreaderid", "")]
			public const string pchmcs_team_systemuser_ContactsReaderId = "pchmcs_team_systemuser_ContactsReaderId";
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

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("actioncardusersettings", EntityRole.Referenced, "actioncardusersettings_owning_user", "owninguser")]
			public const string actioncardusersettings_owning_user = "actioncardusersettings_owning_user";
			[Relationship("annotation", EntityRole.Referenced, "annotation_owning_user", "owninguser")]
			public const string annotation_owning_user = "annotation_owning_user";
			[Relationship("businessunitmap", EntityRole.Referenced, "bizmap_systemuser_businessid", "businessid")]
			public const string bizmap_systemuser_businessid = "bizmap_systemuser_businessid";
			[Relationship("constraintbasedgroup", EntityRole.Referenced, "constraintbasedgroup_systemuser", "businessunitid")]
			public const string constraintbasedgroup_systemuser = "constraintbasedgroup_systemuser";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "contact_owning_user", "owninguser")]
			public const string contact_owning_user = "contact_owning_user";
			[Relationship("attributemap", EntityRole.Referenced, "createdby_attributemap", "createdby")]
			public const string createdby_attributemap = "createdby_attributemap";
			[Relationship("connection", EntityRole.Referenced, "createdby_connection", "createdby")]
			public const string createdby_connection = "createdby_connection";
			[Relationship("connectionrole", EntityRole.Referenced, "createdby_connection_role", "createdby")]
			public const string createdby_connection_role = "createdby_connection_role";
			[Relationship("customerrelationship", EntityRole.Referenced, "createdby_customer_relationship", "createdby")]
			public const string createdby_customer_relationship = "createdby_customer_relationship";
			[Relationship("entitymap", EntityRole.Referenced, "createdby_entitymap", "createdby")]
			public const string createdby_entitymap = "createdby_entitymap";
			[Relationship("expanderevent", EntityRole.Referenced, "createdby_expanderevent", "createdby")]
			public const string createdby_expanderevent = "createdby_expanderevent";
			[Relationship("pluginassembly", EntityRole.Referenced, "createdby_pluginassembly", "createdby")]
			public const string createdby_pluginassembly = "createdby_pluginassembly";
			[Relationship("plugintracelog", EntityRole.Referenced, "createdby_plugintracelog", "createdby")]
			public const string createdby_plugintracelog = "createdby_plugintracelog";
			[Relationship("plugintype", EntityRole.Referenced, "createdby_plugintype", "createdby")]
			public const string createdby_plugintype = "createdby_plugintype";
			[Relationship("plugintypestatistic", EntityRole.Referenced, "createdby_plugintypestatistic", "createdby")]
			public const string createdby_plugintypestatistic = "createdby_plugintypestatistic";
			[Relationship("relationshiprole", EntityRole.Referenced, "createdby_relationship_role", "createdby")]
			public const string createdby_relationship_role = "createdby_relationship_role";
			[Relationship("relationshiprolemap", EntityRole.Referenced, "createdby_relationship_role_map", "createdby")]
			public const string createdby_relationship_role_map = "createdby_relationship_role_map";
			[Relationship("sdkmessage", EntityRole.Referenced, "createdby_sdkmessage", "createdby")]
			public const string createdby_sdkmessage = "createdby_sdkmessage";
			[Relationship("sdkmessagefilter", EntityRole.Referenced, "createdby_sdkmessagefilter", "createdby")]
			public const string createdby_sdkmessagefilter = "createdby_sdkmessagefilter";
			[Relationship("sdkmessagepair", EntityRole.Referenced, "createdby_sdkmessagepair", "createdby")]
			public const string createdby_sdkmessagepair = "createdby_sdkmessagepair";
			[Relationship("sdkmessageprocessingstep", EntityRole.Referenced, "createdby_sdkmessageprocessingstep", "createdby")]
			public const string createdby_sdkmessageprocessingstep = "createdby_sdkmessageprocessingstep";
			[Relationship("sdkmessageprocessingstepimage", EntityRole.Referenced, "createdby_sdkmessageprocessingstepimage", "createdby")]
			public const string createdby_sdkmessageprocessingstepimage = "createdby_sdkmessageprocessingstepimage";
			[Relationship("sdkmessageprocessingstepsecureconfig", EntityRole.Referenced, "createdby_sdkmessageprocessingstepsecureconfig", "createdby")]
			public const string createdby_sdkmessageprocessingstepsecureconfig = "createdby_sdkmessageprocessingstepsecureconfig";
			[Relationship("sdkmessagerequest", EntityRole.Referenced, "createdby_sdkmessagerequest", "createdby")]
			public const string createdby_sdkmessagerequest = "createdby_sdkmessagerequest";
			[Relationship("sdkmessagerequestfield", EntityRole.Referenced, "createdby_sdkmessagerequestfield", "createdby")]
			public const string createdby_sdkmessagerequestfield = "createdby_sdkmessagerequestfield";
			[Relationship("sdkmessageresponse", EntityRole.Referenced, "createdby_sdkmessageresponse", "createdby")]
			public const string createdby_sdkmessageresponse = "createdby_sdkmessageresponse";
			[Relationship("sdkmessageresponsefield", EntityRole.Referenced, "createdby_sdkmessageresponsefield", "createdby")]
			public const string createdby_sdkmessageresponsefield = "createdby_sdkmessageresponsefield";
			[Relationship("serviceendpoint", EntityRole.Referenced, "createdby_serviceendpoint", "createdby")]
			public const string createdby_serviceendpoint = "createdby_serviceendpoint";
			[Relationship("attributemap", EntityRole.Referenced, "createdonbehalfby_attributemap", "createdonbehalfby")]
			public const string createdonbehalfby_attributemap = "createdonbehalfby_attributemap";
			[Relationship("customerrelationship", EntityRole.Referenced, "createdonbehalfby_customer_relationship", "createdonbehalfby")]
			public const string createdonbehalfby_customer_relationship = "createdonbehalfby_customer_relationship";
			[Relationship("dynamicpropertyinstance", EntityRole.Referenced, "Dynamicpropertyinsatance_createdby", "createdby")]
			public const string Dynamicpropertyinsatance_createdby = "Dynamicpropertyinsatance_createdby";
			[Relationship("equipment", EntityRole.Referenced, "equipment_systemuser", "businessunitid")]
			public const string equipment_systemuser = "equipment_systemuser";
			[Relationship("sdkmessageprocessingstep", EntityRole.Referenced, "impersonatinguserid_sdkmessageprocessingstep", "impersonatinguserid")]
			public const string impersonatinguserid_sdkmessageprocessingstep = "impersonatinguserid_sdkmessageprocessingstep";
			[Relationship("importfile", EntityRole.Referenced, "ImportFile_SystemUser", "recordsownerid")]
			public const string ImportFile_SystemUser = "ImportFile_SystemUser";
			[Relationship("knowledgearticle", EntityRole.Referenced, "knowledgearticle_primaryauthorid", "primaryauthorid")]
			public const string knowledgearticle_primaryauthorid = "knowledgearticle_primaryauthorid";
			[Relationship("lead", EntityRole.Referenced, "lead_owning_user", "owninguser")]
			public const string lead_owning_user = "lead_owning_user";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "lk_accountbase_createdby", AccountDefinition.Columns.CreatedBy)]
			public const string lk_accountbase_createdby = "lk_accountbase_createdby";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "lk_accountbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_accountbase_createdonbehalfby = "lk_accountbase_createdonbehalfby";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "lk_accountbase_modifiedby", "modifiedby")]
			public const string lk_accountbase_modifiedby = "lk_accountbase_modifiedby";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "lk_accountbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_accountbase_modifiedonbehalfby = "lk_accountbase_modifiedonbehalfby";
			[Relationship("aciviewmapper", EntityRole.Referenced, "lk_ACIViewMapper_createdby", "createdby")]
			public const string lk_ACIViewMapper_createdby = "lk_ACIViewMapper_createdby";
			[Relationship("aciviewmapper", EntityRole.Referenced, "lk_ACIViewMapper_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ACIViewMapper_createdonbehalfby = "lk_ACIViewMapper_createdonbehalfby";
			[Relationship("aciviewmapper", EntityRole.Referenced, "lk_ACIViewMapper_modifiedby", "modifiedby")]
			public const string lk_ACIViewMapper_modifiedby = "lk_ACIViewMapper_modifiedby";
			[Relationship("aciviewmapper", EntityRole.Referenced, "lk_ACIViewMapper_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ACIViewMapper_modifiedonbehalfby = "lk_ACIViewMapper_modifiedonbehalfby";
			[Relationship("actioncard", EntityRole.Referenced, "lk_actioncardbase_createdby", "createdby")]
			public const string lk_actioncardbase_createdby = "lk_actioncardbase_createdby";
			[Relationship("actioncard", EntityRole.Referenced, "lk_actioncardbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_actioncardbase_createdonbehalfby = "lk_actioncardbase_createdonbehalfby";
			[Relationship("actioncard", EntityRole.Referenced, "lk_actioncardbase_modifiedby", "modifiedby")]
			public const string lk_actioncardbase_modifiedby = "lk_actioncardbase_modifiedby";
			[Relationship("actioncard", EntityRole.Referenced, "lk_actioncardbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_actioncardbase_modifiedonbehalfby = "lk_actioncardbase_modifiedonbehalfby";
			[Relationship("activitypointer", EntityRole.Referenced, "lk_activitypointer_createdby", "createdby")]
			public const string lk_activitypointer_createdby = "lk_activitypointer_createdby";
			[Relationship("activitypointer", EntityRole.Referenced, "lk_activitypointer_createdonbehalfby", "createdonbehalfby")]
			public const string lk_activitypointer_createdonbehalfby = "lk_activitypointer_createdonbehalfby";
			[Relationship("activitypointer", EntityRole.Referenced, "lk_activitypointer_modifiedby", "modifiedby")]
			public const string lk_activitypointer_modifiedby = "lk_activitypointer_modifiedby";
			[Relationship("activitypointer", EntityRole.Referenced, "lk_activitypointer_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_activitypointer_modifiedonbehalfby = "lk_activitypointer_modifiedonbehalfby";
			[Relationship("adminsettingsentity", EntityRole.Referenced, "lk_adminsettingsentity_createdby", "createdby")]
			public const string lk_adminsettingsentity_createdby = "lk_adminsettingsentity_createdby";
			[Relationship("adminsettingsentity", EntityRole.Referenced, "lk_adminsettingsentity_createdonbehalfby", "createdonbehalfby")]
			public const string lk_adminsettingsentity_createdonbehalfby = "lk_adminsettingsentity_createdonbehalfby";
			[Relationship("adminsettingsentity", EntityRole.Referenced, "lk_adminsettingsentity_modifiedby", "modifiedby")]
			public const string lk_adminsettingsentity_modifiedby = "lk_adminsettingsentity_modifiedby";
			[Relationship("adminsettingsentity", EntityRole.Referenced, "lk_adminsettingsentity_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_adminsettingsentity_modifiedonbehalfby = "lk_adminsettingsentity_modifiedonbehalfby";
			[Relationship("advancedsimilarityrule", EntityRole.Referenced, "lk_advancedsimilarityrule_createdby", "createdby")]
			public const string lk_advancedsimilarityrule_createdby = "lk_advancedsimilarityrule_createdby";
			[Relationship("advancedsimilarityrule", EntityRole.Referenced, "lk_advancedsimilarityrule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_advancedsimilarityrule_createdonbehalfby = "lk_advancedsimilarityrule_createdonbehalfby";
			[Relationship("advancedsimilarityrule", EntityRole.Referenced, "lk_advancedsimilarityrule_modifiedby", "modifiedby")]
			public const string lk_advancedsimilarityrule_modifiedby = "lk_advancedsimilarityrule_modifiedby";
			[Relationship("advancedsimilarityrule", EntityRole.Referenced, "lk_advancedsimilarityrule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_advancedsimilarityrule_modifiedonbehalfby = "lk_advancedsimilarityrule_modifiedonbehalfby";
			[Relationship("annotation", EntityRole.Referenced, "lk_annotationbase_createdby", "createdby")]
			public const string lk_annotationbase_createdby = "lk_annotationbase_createdby";
			[Relationship("annotation", EntityRole.Referenced, "lk_annotationbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_annotationbase_createdonbehalfby = "lk_annotationbase_createdonbehalfby";
			[Relationship("annotation", EntityRole.Referenced, "lk_annotationbase_modifiedby", "modifiedby")]
			public const string lk_annotationbase_modifiedby = "lk_annotationbase_modifiedby";
			[Relationship("annotation", EntityRole.Referenced, "lk_annotationbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_annotationbase_modifiedonbehalfby = "lk_annotationbase_modifiedonbehalfby";
			[Relationship("annualfiscalcalendar", EntityRole.Referenced, "lk_annualfiscalcalendar_createdby", "createdby")]
			public const string lk_annualfiscalcalendar_createdby = "lk_annualfiscalcalendar_createdby";
			[Relationship("annualfiscalcalendar", EntityRole.Referenced, "lk_annualfiscalcalendar_createdonbehalfby", "createdonbehalfby")]
			public const string lk_annualfiscalcalendar_createdonbehalfby = "lk_annualfiscalcalendar_createdonbehalfby";
			[Relationship("annualfiscalcalendar", EntityRole.Referenced, "lk_annualfiscalcalendar_modifiedby", "modifiedby")]
			public const string lk_annualfiscalcalendar_modifiedby = "lk_annualfiscalcalendar_modifiedby";
			[Relationship("annualfiscalcalendar", EntityRole.Referenced, "lk_annualfiscalcalendar_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_annualfiscalcalendar_modifiedonbehalfby = "lk_annualfiscalcalendar_modifiedonbehalfby";
			[Relationship("annualfiscalcalendar", EntityRole.Referenced, "lk_annualfiscalcalendar_salespersonid", "salespersonid")]
			public const string lk_annualfiscalcalendar_salespersonid = "lk_annualfiscalcalendar_salespersonid";
			[Relationship("appconfig", EntityRole.Referenced, "systemuser_appconfig_createdby", "createdby")]
			public const string lk_appconfig_createdby = "lk_appconfig_createdby";
			[Relationship("appconfig", EntityRole.Referenced, "systemuser_appconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_appconfig_createdonbehalfby = "lk_appconfig_createdonbehalfby";
			[Relationship("appconfig", EntityRole.Referenced, "systemuser_appconfig_modifiedby", "modifiedby")]
			public const string lk_appconfig_modifiedby = "lk_appconfig_modifiedby";
			[Relationship("appconfig", EntityRole.Referenced, "systemuser_appconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_appconfig_modifiedonbehalfby = "lk_appconfig_modifiedonbehalfby";
			[Relationship("appconfiginstance", EntityRole.Referenced, "systemuser_appconfiginstance_createdby", "createdby")]
			public const string lk_appconfiginstance_createdby = "lk_appconfiginstance_createdby";
			[Relationship("appconfiginstance", EntityRole.Referenced, "systemuser_appconfiginstance_createdonbehalfby", "createdonbehalfby")]
			public const string lk_appconfiginstance_createdonbehalfby = "lk_appconfiginstance_createdonbehalfby";
			[Relationship("appconfiginstance", EntityRole.Referenced, "systemuser_appconfiginstance_modifiedby", "modifiedby")]
			public const string lk_appconfiginstance_modifiedby = "lk_appconfiginstance_modifiedby";
			[Relationship("appconfiginstance", EntityRole.Referenced, "systemuser_appconfiginstance_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_appconfiginstance_modifiedonbehalfby = "lk_appconfiginstance_modifiedonbehalfby";
			[Relationship("appconfigmaster", EntityRole.Referenced, "systemuser_appconfigmaster_createdby", "createdby")]
			public const string lk_appconfigmaster_createdby = "lk_appconfigmaster_createdby";
			[Relationship("appconfigmaster", EntityRole.Referenced, "systemuser_appconfigmaster_createdonbehalfby", "createdonbehalfby")]
			public const string lk_appconfigmaster_createdonbehalfby = "lk_appconfigmaster_createdonbehalfby";
			[Relationship("appconfigmaster", EntityRole.Referenced, "systemuser_appconfigmaster_modifiedby", "modifiedby")]
			public const string lk_appconfigmaster_modifiedby = "lk_appconfigmaster_modifiedby";
			[Relationship("appconfigmaster", EntityRole.Referenced, "systemuser_appconfigmaster_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_appconfigmaster_modifiedonbehalfby = "lk_appconfigmaster_modifiedonbehalfby";
			[Relationship("applicationfile", EntityRole.Referenced, "lk_applicationfile_createdby", "createdby")]
			public const string lk_applicationfile_createdby = "lk_applicationfile_createdby";
			[Relationship("applicationfile", EntityRole.Referenced, "lk_applicationfile_createdonbehalfby", "createdonbehalfby")]
			public const string lk_applicationfile_createdonbehalfby = "lk_applicationfile_createdonbehalfby";
			[Relationship("applicationfile", EntityRole.Referenced, "lk_applicationfile_modifiedby", "modifiedby")]
			public const string lk_applicationfile_modifiedby = "lk_applicationfile_modifiedby";
			[Relationship("applicationfile", EntityRole.Referenced, "lk_applicationfile_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_applicationfile_modifiedonbehalfby = "lk_applicationfile_modifiedonbehalfby";
			[Relationship("appmodule", EntityRole.Referenced, "systemuser_appmodule_createdby", "createdby")]
			public const string lk_appmodule_createdby = "lk_appmodule_createdby";
			[Relationship("appmodule", EntityRole.Referenced, "systemuser_appmodule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_appmodule_createdonbehalfby = "lk_appmodule_createdonbehalfby";
			[Relationship("appmodule", EntityRole.Referenced, "systemuser_appmodule_modifiedby", "modifiedby")]
			public const string lk_appmodule_modifiedby = "lk_appmodule_modifiedby";
			[Relationship("appmodule", EntityRole.Referenced, "systemuser_appmodule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_appmodule_modifiedonbehalfby = "lk_appmodule_modifiedonbehalfby";
			[Relationship("appmodulecomponent", EntityRole.Referenced, "appmodulecomponent_createdby", "createdby")]
			public const string lk_appmodulecomponent_createdby = "lk_appmodulecomponent_createdby";
			[Relationship("appmodulecomponent", EntityRole.Referenced, "lk_appmodulecomponent_createdonbehalfby", "createdonbehalfby")]
			public const string lk_appmodulecomponent_createdonbehalfby = "lk_appmodulecomponent_createdonbehalfby";
			[Relationship("appmodulecomponent", EntityRole.Referenced, "appmodulecomponent_modifiedby", "modifiedby")]
			public const string lk_appmodulecomponent_modifiedby = "lk_appmodulecomponent_modifiedby";
			[Relationship("appmodulecomponent", EntityRole.Referenced, "lk_appmodulecomponent_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_appmodulecomponent_modifiedonbehalfby = "lk_appmodulecomponent_modifiedonbehalfby";
			[Relationship("appointment", EntityRole.Referenced, "lk_appointment_createdby", "createdby")]
			public const string lk_appointment_createdby = "lk_appointment_createdby";
			[Relationship("appointment", EntityRole.Referenced, "lk_appointment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_appointment_createdonbehalfby = "lk_appointment_createdonbehalfby";
			[Relationship("appointment", EntityRole.Referenced, "lk_appointment_modifiedby", "modifiedby")]
			public const string lk_appointment_modifiedby = "lk_appointment_modifiedby";
			[Relationship("appointment", EntityRole.Referenced, "lk_appointment_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_appointment_modifiedonbehalfby = "lk_appointment_modifiedonbehalfby";
			[Relationship("asyncoperation", EntityRole.Referenced, "lk_asyncoperation_createdby", "createdby")]
			public const string lk_asyncoperation_createdby = "lk_asyncoperation_createdby";
			[Relationship("asyncoperation", EntityRole.Referenced, "lk_asyncoperation_createdonbehalfby", "createdonbehalfby")]
			public const string lk_asyncoperation_createdonbehalfby = "lk_asyncoperation_createdonbehalfby";
			[Relationship("asyncoperation", EntityRole.Referenced, "lk_asyncoperation_modifiedby", "modifiedby")]
			public const string lk_asyncoperation_modifiedby = "lk_asyncoperation_modifiedby";
			[Relationship("asyncoperation", EntityRole.Referenced, "lk_asyncoperation_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_asyncoperation_modifiedonbehalfby = "lk_asyncoperation_modifiedonbehalfby";
			[Relationship("audit", EntityRole.Referenced, "lk_audit_callinguserid", "callinguserid")]
			public const string lk_audit_callinguserid = "lk_audit_callinguserid";
			[Relationship("audit", EntityRole.Referenced, "lk_audit_userid", "userid")]
			public const string lk_audit_userid = "lk_audit_userid";
			[Relationship("authorizationserver", EntityRole.Referenced, "lk_authorizationserver_createdby", "createdby")]
			public const string lk_authorizationserver_createdby = "lk_authorizationserver_createdby";
			[Relationship("authorizationserver", EntityRole.Referenced, "lk_authorizationserver_createdonbehalfby", "createdonbehalfby")]
			public const string lk_authorizationserver_createdonbehalfby = "lk_authorizationserver_createdonbehalfby";
			[Relationship("authorizationserver", EntityRole.Referenced, "lk_authorizationserver_modifiedby", "modifiedby")]
			public const string lk_authorizationserver_modifiedby = "lk_authorizationserver_modifiedby";
			[Relationship("authorizationserver", EntityRole.Referenced, "lk_authorizationserver_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_authorizationserver_modifiedonbehalfby = "lk_authorizationserver_modifiedonbehalfby";
			[Relationship("azureserviceconnection", EntityRole.Referenced, "lk_azureserviceconnection_createdby", "createdby")]
			public const string lk_azureserviceconnection_createdby = "lk_azureserviceconnection_createdby";
			[Relationship("azureserviceconnection", EntityRole.Referenced, "lk_azureserviceconnection_createdonbehalfby", "createdonbehalfby")]
			public const string lk_azureserviceconnection_createdonbehalfby = "lk_azureserviceconnection_createdonbehalfby";
			[Relationship("azureserviceconnection", EntityRole.Referenced, "lk_azureserviceconnection_modifiedby", "modifiedby")]
			public const string lk_azureserviceconnection_modifiedby = "lk_azureserviceconnection_modifiedby";
			[Relationship("azureserviceconnection", EntityRole.Referenced, "lk_azureserviceconnection_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_azureserviceconnection_modifiedonbehalfby = "lk_azureserviceconnection_modifiedonbehalfby";
			[Relationship("bookableresource", EntityRole.Referenced, "lk_bookableresource_createdby", "createdby")]
			public const string lk_bookableresource_createdby = "lk_bookableresource_createdby";
			[Relationship("bookableresource", EntityRole.Referenced, "lk_bookableresource_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookableresource_createdonbehalfby = "lk_bookableresource_createdonbehalfby";
			[Relationship("bookableresource", EntityRole.Referenced, "lk_bookableresource_modifiedby", "modifiedby")]
			public const string lk_bookableresource_modifiedby = "lk_bookableresource_modifiedby";
			[Relationship("bookableresource", EntityRole.Referenced, "lk_bookableresource_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookableresource_modifiedonbehalfby = "lk_bookableresource_modifiedonbehalfby";
			[Relationship("bookableresourcebooking", EntityRole.Referenced, "lk_bookableresourcebooking_createdby", "createdby")]
			public const string lk_bookableresourcebooking_createdby = "lk_bookableresourcebooking_createdby";
			[Relationship("bookableresourcebooking", EntityRole.Referenced, "lk_bookableresourcebooking_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookableresourcebooking_createdonbehalfby = "lk_bookableresourcebooking_createdonbehalfby";
			[Relationship("bookableresourcebooking", EntityRole.Referenced, "lk_bookableresourcebooking_modifiedby", "modifiedby")]
			public const string lk_bookableresourcebooking_modifiedby = "lk_bookableresourcebooking_modifiedby";
			[Relationship("bookableresourcebooking", EntityRole.Referenced, "lk_bookableresourcebooking_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookableresourcebooking_modifiedonbehalfby = "lk_bookableresourcebooking_modifiedonbehalfby";
			[Relationship("bookableresourcebookingexchangesyncidmapping", EntityRole.Referenced, "lk_bookableresourcebookingexchangesyncidmapping_createdby", "createdby")]
			public const string lk_bookableresourcebookingexchangesyncidmapping_createdby = "lk_bookableresourcebookingexchangesyncidmapping_createdby";
			[Relationship("bookableresourcebookingexchangesyncidmapping", EntityRole.Referenced, "lk_bookableresourcebookingexchangesyncidmapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookableresourcebookingexchangesyncidmapping_createdonbehalfby = "lk_bookableresourcebookingexchangesyncidmapping_createdonbehalfby";
			[Relationship("bookableresourcebookingexchangesyncidmapping", EntityRole.Referenced, "lk_bookableresourcebookingexchangesyncidmapping_modifiedby", "modifiedby")]
			public const string lk_bookableresourcebookingexchangesyncidmapping_modifiedby = "lk_bookableresourcebookingexchangesyncidmapping_modifiedby";
			[Relationship("bookableresourcebookingexchangesyncidmapping", EntityRole.Referenced, "lk_bookableresourcebookingexchangesyncidmapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookableresourcebookingexchangesyncidmapping_modifiedonbehalfby = "lk_bookableresourcebookingexchangesyncidmapping_modifiedonbehalfby";
			[Relationship("bookableresourcebookingheader", EntityRole.Referenced, "lk_bookableresourcebookingheader_createdby", "createdby")]
			public const string lk_bookableresourcebookingheader_createdby = "lk_bookableresourcebookingheader_createdby";
			[Relationship("bookableresourcebookingheader", EntityRole.Referenced, "lk_bookableresourcebookingheader_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookableresourcebookingheader_createdonbehalfby = "lk_bookableresourcebookingheader_createdonbehalfby";
			[Relationship("bookableresourcebookingheader", EntityRole.Referenced, "lk_bookableresourcebookingheader_modifiedby", "modifiedby")]
			public const string lk_bookableresourcebookingheader_modifiedby = "lk_bookableresourcebookingheader_modifiedby";
			[Relationship("bookableresourcebookingheader", EntityRole.Referenced, "lk_bookableresourcebookingheader_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookableresourcebookingheader_modifiedonbehalfby = "lk_bookableresourcebookingheader_modifiedonbehalfby";
			[Relationship("bookableresourcecategory", EntityRole.Referenced, "lk_bookableresourcecategory_createdby", "createdby")]
			public const string lk_bookableresourcecategory_createdby = "lk_bookableresourcecategory_createdby";
			[Relationship("bookableresourcecategory", EntityRole.Referenced, "lk_bookableresourcecategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookableresourcecategory_createdonbehalfby = "lk_bookableresourcecategory_createdonbehalfby";
			[Relationship("bookableresourcecategory", EntityRole.Referenced, "lk_bookableresourcecategory_modifiedby", "modifiedby")]
			public const string lk_bookableresourcecategory_modifiedby = "lk_bookableresourcecategory_modifiedby";
			[Relationship("bookableresourcecategory", EntityRole.Referenced, "lk_bookableresourcecategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookableresourcecategory_modifiedonbehalfby = "lk_bookableresourcecategory_modifiedonbehalfby";
			[Relationship("bookableresourcecategoryassn", EntityRole.Referenced, "lk_bookableresourcecategoryassn_createdby", "createdby")]
			public const string lk_bookableresourcecategoryassn_createdby = "lk_bookableresourcecategoryassn_createdby";
			[Relationship("bookableresourcecategoryassn", EntityRole.Referenced, "lk_bookableresourcecategoryassn_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookableresourcecategoryassn_createdonbehalfby = "lk_bookableresourcecategoryassn_createdonbehalfby";
			[Relationship("bookableresourcecategoryassn", EntityRole.Referenced, "lk_bookableresourcecategoryassn_modifiedby", "modifiedby")]
			public const string lk_bookableresourcecategoryassn_modifiedby = "lk_bookableresourcecategoryassn_modifiedby";
			[Relationship("bookableresourcecategoryassn", EntityRole.Referenced, "lk_bookableresourcecategoryassn_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookableresourcecategoryassn_modifiedonbehalfby = "lk_bookableresourcecategoryassn_modifiedonbehalfby";
			[Relationship("bookableresourcecharacteristic", EntityRole.Referenced, "lk_bookableresourcecharacteristic_createdby", "createdby")]
			public const string lk_bookableresourcecharacteristic_createdby = "lk_bookableresourcecharacteristic_createdby";
			[Relationship("bookableresourcecharacteristic", EntityRole.Referenced, "lk_bookableresourcecharacteristic_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookableresourcecharacteristic_createdonbehalfby = "lk_bookableresourcecharacteristic_createdonbehalfby";
			[Relationship("bookableresourcecharacteristic", EntityRole.Referenced, "lk_bookableresourcecharacteristic_modifiedby", "modifiedby")]
			public const string lk_bookableresourcecharacteristic_modifiedby = "lk_bookableresourcecharacteristic_modifiedby";
			[Relationship("bookableresourcecharacteristic", EntityRole.Referenced, "lk_bookableresourcecharacteristic_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookableresourcecharacteristic_modifiedonbehalfby = "lk_bookableresourcecharacteristic_modifiedonbehalfby";
			[Relationship("bookableresourcegroup", EntityRole.Referenced, "lk_bookableresourcegroup_createdby", "createdby")]
			public const string lk_bookableresourcegroup_createdby = "lk_bookableresourcegroup_createdby";
			[Relationship("bookableresourcegroup", EntityRole.Referenced, "lk_bookableresourcegroup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookableresourcegroup_createdonbehalfby = "lk_bookableresourcegroup_createdonbehalfby";
			[Relationship("bookableresourcegroup", EntityRole.Referenced, "lk_bookableresourcegroup_modifiedby", "modifiedby")]
			public const string lk_bookableresourcegroup_modifiedby = "lk_bookableresourcegroup_modifiedby";
			[Relationship("bookableresourcegroup", EntityRole.Referenced, "lk_bookableresourcegroup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookableresourcegroup_modifiedonbehalfby = "lk_bookableresourcegroup_modifiedonbehalfby";
			[Relationship("bookingstatus", EntityRole.Referenced, "lk_bookingstatus_createdby", "createdby")]
			public const string lk_bookingstatus_createdby = "lk_bookingstatus_createdby";
			[Relationship("bookingstatus", EntityRole.Referenced, "lk_bookingstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bookingstatus_createdonbehalfby = "lk_bookingstatus_createdonbehalfby";
			[Relationship("bookingstatus", EntityRole.Referenced, "lk_bookingstatus_modifiedby", "modifiedby")]
			public const string lk_bookingstatus_modifiedby = "lk_bookingstatus_modifiedby";
			[Relationship("bookingstatus", EntityRole.Referenced, "lk_bookingstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bookingstatus_modifiedonbehalfby = "lk_bookingstatus_modifiedonbehalfby";
			[Relationship("bulkdeleteoperation", EntityRole.Referenced, "lk_bulkdeleteoperation_createdonbehalfby", "createdonbehalfby")]
			public const string lk_bulkdeleteoperation_createdonbehalfby = "lk_bulkdeleteoperation_createdonbehalfby";
			[Relationship("bulkdeleteoperation", EntityRole.Referenced, "lk_bulkdeleteoperation_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_bulkdeleteoperation_modifiedonbehalfby = "lk_bulkdeleteoperation_modifiedonbehalfby";
			[Relationship("bulkdeleteoperation", EntityRole.Referenced, "lk_bulkdeleteoperationbase_createdby", "createdby")]
			public const string lk_bulkdeleteoperationbase_createdby = "lk_bulkdeleteoperationbase_createdby";
			[Relationship("bulkdeleteoperation", EntityRole.Referenced, "lk_bulkdeleteoperationbase_modifiedby", "modifiedby")]
			public const string lk_bulkdeleteoperationbase_modifiedby = "lk_bulkdeleteoperationbase_modifiedby";
			[Relationship("bulkoperation", EntityRole.Referenced, "lk_BulkOperation_createdby", "createdby")]
			public const string lk_BulkOperation_createdby = "lk_BulkOperation_createdby";
			[Relationship("bulkoperation", EntityRole.Referenced, "lk_BulkOperation_createdonbehalfby", "createdonbehalfby")]
			public const string lk_BulkOperation_createdonbehalfby = "lk_BulkOperation_createdonbehalfby";
			[Relationship("bulkoperation", EntityRole.Referenced, "lk_BulkOperation_modifiedby", "modifiedby")]
			public const string lk_BulkOperation_modifiedby = "lk_BulkOperation_modifiedby";
			[Relationship("bulkoperation", EntityRole.Referenced, "lk_BulkOperation_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_BulkOperation_modifiedonbehalfby = "lk_BulkOperation_modifiedonbehalfby";
			[Relationship("businessprocessflowinstance", EntityRole.Referenced, "lk_businessprocessflowinstancebase_createdby", "createdby")]
			public const string lk_businessprocessflowinstancebase_createdby = "lk_businessprocessflowinstancebase_createdby";
			[Relationship("businessprocessflowinstance", EntityRole.Referenced, "lk_businessprocessflowinstancebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_businessprocessflowinstancebase_createdonbehalfby = "lk_businessprocessflowinstancebase_createdonbehalfby";
			[Relationship("businessprocessflowinstance", EntityRole.Referenced, "lk_businessprocessflowinstancebase_modifiedby", "modifiedby")]
			public const string lk_businessprocessflowinstancebase_modifiedby = "lk_businessprocessflowinstancebase_modifiedby";
			[Relationship("businessprocessflowinstance", EntityRole.Referenced, "lk_businessprocessflowinstancebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_businessprocessflowinstancebase_modifiedonbehalfby = "lk_businessprocessflowinstancebase_modifiedonbehalfby";
			[Relationship("businessunit", EntityRole.Referenced, "lk_businessunit_createdonbehalfby", "createdonbehalfby")]
			public const string lk_businessunit_createdonbehalfby = "lk_businessunit_createdonbehalfby";
			[Relationship("businessunit", EntityRole.Referenced, "lk_businessunit_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_businessunit_modifiedonbehalfby = "lk_businessunit_modifiedonbehalfby";
			[Relationship("businessunit", EntityRole.Referenced, "lk_businessunitbase_createdby", "createdby")]
			public const string lk_businessunitbase_createdby = "lk_businessunitbase_createdby";
			[Relationship("businessunit", EntityRole.Referenced, "lk_businessunitbase_modifiedby", "modifiedby")]
			public const string lk_businessunitbase_modifiedby = "lk_businessunitbase_modifiedby";
			[Relationship("businessunitnewsarticle", EntityRole.Referenced, "lk_businessunitnewsarticle_createdonbehalfby", "createdonbehalfby")]
			public const string lk_businessunitnewsarticle_createdonbehalfby = "lk_businessunitnewsarticle_createdonbehalfby";
			[Relationship("businessunitnewsarticle", EntityRole.Referenced, "lk_businessunitnewsarticle_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_businessunitnewsarticle_modifiedonbehalfby = "lk_businessunitnewsarticle_modifiedonbehalfby";
			[Relationship("businessunitnewsarticle", EntityRole.Referenced, "lk_businessunitnewsarticlebase_createdby", "createdby")]
			public const string lk_businessunitnewsarticlebase_createdby = "lk_businessunitnewsarticlebase_createdby";
			[Relationship("businessunitnewsarticle", EntityRole.Referenced, "lk_businessunitnewsarticlebase_modifiedby", "modifiedby")]
			public const string lk_businessunitnewsarticlebase_modifiedby = "lk_businessunitnewsarticlebase_modifiedby";
			[Relationship("calendar", EntityRole.Referenced, "lk_calendar_createdby", "createdby")]
			public const string lk_calendar_createdby = "lk_calendar_createdby";
			[Relationship("calendar", EntityRole.Referenced, "lk_calendar_createdonbehalfby", "createdonbehalfby")]
			public const string lk_calendar_createdonbehalfby = "lk_calendar_createdonbehalfby";
			[Relationship("calendar", EntityRole.Referenced, "lk_calendar_modifiedby", "modifiedby")]
			public const string lk_calendar_modifiedby = "lk_calendar_modifiedby";
			[Relationship("calendar", EntityRole.Referenced, "lk_calendar_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_calendar_modifiedonbehalfby = "lk_calendar_modifiedonbehalfby";
			[Relationship("calendarrule", EntityRole.Referenced, "lk_calendarrule_createdby", "createdby")]
			public const string lk_calendarrule_createdby = "lk_calendarrule_createdby";
			[Relationship("calendarrule", EntityRole.Referenced, "lk_calendarrule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_calendarrule_createdonbehalfby = "lk_calendarrule_createdonbehalfby";
			[Relationship("calendarrule", EntityRole.Referenced, "lk_calendarrule_modifiedby", "modifiedby")]
			public const string lk_calendarrule_modifiedby = "lk_calendarrule_modifiedby";
			[Relationship("calendarrule", EntityRole.Referenced, "lk_calendarrule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_calendarrule_modifiedonbehalfby = "lk_calendarrule_modifiedonbehalfby";
			[Relationship("callbackregistration", EntityRole.Referenced, "systemuser_callbackregistration_createdby", "createdby")]
			public const string lk_callbackregistration_createdby = "lk_callbackregistration_createdby";
			[Relationship("callbackregistration", EntityRole.Referenced, "systemuser_callbackregistration_createdonbehalfby", "createdonbehalfby")]
			public const string lk_callbackregistration_createdonbehalfby = "lk_callbackregistration_createdonbehalfby";
			[Relationship("callbackregistration", EntityRole.Referenced, "systemuser_callbackregistration_modifiedby", "modifiedby")]
			public const string lk_callbackregistration_modifiedby = "lk_callbackregistration_modifiedby";
			[Relationship("callbackregistration", EntityRole.Referenced, "systemuser_callbackregistration_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_callbackregistration_modifiedonbehalfby = "lk_callbackregistration_modifiedonbehalfby";
			[Relationship("campaign", EntityRole.Referenced, "lk_campaign_createdby", "createdby")]
			public const string lk_campaign_createdby = "lk_campaign_createdby";
			[Relationship("campaign", EntityRole.Referenced, "lk_campaign_createdonbehalfby", "createdonbehalfby")]
			public const string lk_campaign_createdonbehalfby = "lk_campaign_createdonbehalfby";
			[Relationship("campaign", EntityRole.Referenced, "lk_campaign_modifiedby", "modifiedby")]
			public const string lk_campaign_modifiedby = "lk_campaign_modifiedby";
			[Relationship("campaign", EntityRole.Referenced, "lk_campaign_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_campaign_modifiedonbehalfby = "lk_campaign_modifiedonbehalfby";
			[Relationship("campaignactivity", EntityRole.Referenced, "lk_campaignactivity_createdby", "createdby")]
			public const string lk_campaignactivity_createdby = "lk_campaignactivity_createdby";
			[Relationship("campaignactivity", EntityRole.Referenced, "lk_campaignactivity_createdonbehalfby", "createdonbehalfby")]
			public const string lk_campaignactivity_createdonbehalfby = "lk_campaignactivity_createdonbehalfby";
			[Relationship("campaignactivity", EntityRole.Referenced, "lk_campaignactivity_modifiedby", "modifiedby")]
			public const string lk_campaignactivity_modifiedby = "lk_campaignactivity_modifiedby";
			[Relationship("campaignactivity", EntityRole.Referenced, "lk_campaignactivity_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_campaignactivity_modifiedonbehalfby = "lk_campaignactivity_modifiedonbehalfby";
			[Relationship("campaignresponse", EntityRole.Referenced, "lk_campaignresponse_createdby", "createdby")]
			public const string lk_campaignresponse_createdby = "lk_campaignresponse_createdby";
			[Relationship("campaignresponse", EntityRole.Referenced, "lk_campaignresponse_createdonbehalfby", "createdonbehalfby")]
			public const string lk_campaignresponse_createdonbehalfby = "lk_campaignresponse_createdonbehalfby";
			[Relationship("campaignresponse", EntityRole.Referenced, "lk_campaignresponse_modifiedby", "modifiedby")]
			public const string lk_campaignresponse_modifiedby = "lk_campaignresponse_modifiedby";
			[Relationship("campaignresponse", EntityRole.Referenced, "lk_campaignresponse_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_campaignresponse_modifiedonbehalfby = "lk_campaignresponse_modifiedonbehalfby";
			[Relationship("cardtype", EntityRole.Referenced, "lk_cardtype_createdby", "createdby")]
			public const string lk_cardtype_createdby = "lk_cardtype_createdby";
			[Relationship("cardtype", EntityRole.Referenced, "lk_cardtype_createdonbehalfby", "createdonbehalfby")]
			public const string lk_cardtype_createdonbehalfby = "lk_cardtype_createdonbehalfby";
			[Relationship("cardtype", EntityRole.Referenced, "lk_cardtype_modifiedby", "modifiedby")]
			public const string lk_cardtype_modifiedby = "lk_cardtype_modifiedby";
			[Relationship("cardtype", EntityRole.Referenced, "lk_cardtype_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_cardtype_modifiedonbehalfby = "lk_cardtype_modifiedonbehalfby";
			[Relationship("category", EntityRole.Referenced, "lk_category_createdby", "createdby")]
			public const string lk_category_createdby = "lk_category_createdby";
			[Relationship("category", EntityRole.Referenced, "lk_category_createdonbehalfby", "createdonbehalfby")]
			public const string lk_category_createdonbehalfby = "lk_category_createdonbehalfby";
			[Relationship("category", EntityRole.Referenced, "lk_category_modifiedby", "modifiedby")]
			public const string lk_category_modifiedby = "lk_category_modifiedby";
			[Relationship("category", EntityRole.Referenced, "lk_category_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_category_modifiedonbehalfby = "lk_category_modifiedonbehalfby";
			[Relationship("cgo_intervention", EntityRole.Referenced, "lk_cgo_intervention_createdby", "createdby")]
			public const string lk_cgo_intervention_createdby = "lk_cgo_intervention_createdby";
			[Relationship("cgo_intervention", EntityRole.Referenced, "lk_cgo_intervention_createdonbehalfby", "createdonbehalfby")]
			public const string lk_cgo_intervention_createdonbehalfby = "lk_cgo_intervention_createdonbehalfby";
			[Relationship("cgo_intervention", EntityRole.Referenced, "lk_cgo_intervention_modifiedby", "modifiedby")]
			public const string lk_cgo_intervention_modifiedby = "lk_cgo_intervention_modifiedby";
			[Relationship("cgo_intervention", EntityRole.Referenced, "lk_cgo_intervention_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_cgo_intervention_modifiedonbehalfby = "lk_cgo_intervention_modifiedonbehalfby";
			[Relationship("cgo_servicecontract", EntityRole.Referenced, "lk_cgo_servicecontract_createdby", "createdby")]
			public const string lk_cgo_servicecontract_createdby = "lk_cgo_servicecontract_createdby";
			[Relationship("cgo_servicecontract", EntityRole.Referenced, "lk_cgo_servicecontract_createdonbehalfby", "createdonbehalfby")]
			public const string lk_cgo_servicecontract_createdonbehalfby = "lk_cgo_servicecontract_createdonbehalfby";
			[Relationship("cgo_servicecontract", EntityRole.Referenced, "lk_cgo_servicecontract_modifiedby", "modifiedby")]
			public const string lk_cgo_servicecontract_modifiedby = "lk_cgo_servicecontract_modifiedby";
			[Relationship("cgo_servicecontract", EntityRole.Referenced, "lk_cgo_servicecontract_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_cgo_servicecontract_modifiedonbehalfby = "lk_cgo_servicecontract_modifiedonbehalfby";
			[Relationship("cgo_testunmanaged", EntityRole.Referenced, "lk_cgo_testunmanaged_createdby", "createdby")]
			public const string lk_cgo_testunmanaged_createdby = "lk_cgo_testunmanaged_createdby";
			[Relationship("cgo_testunmanaged", EntityRole.Referenced, "lk_cgo_testunmanaged_createdonbehalfby", "createdonbehalfby")]
			public const string lk_cgo_testunmanaged_createdonbehalfby = "lk_cgo_testunmanaged_createdonbehalfby";
			[Relationship("cgo_testunmanaged", EntityRole.Referenced, "lk_cgo_testunmanaged_modifiedby", "modifiedby")]
			public const string lk_cgo_testunmanaged_modifiedby = "lk_cgo_testunmanaged_modifiedby";
			[Relationship("cgo_testunmanaged", EntityRole.Referenced, "lk_cgo_testunmanaged_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_cgo_testunmanaged_modifiedonbehalfby = "lk_cgo_testunmanaged_modifiedonbehalfby";
			[Relationship("channelaccessprofile", EntityRole.Referenced, "channelaccessprofile_createdby", "createdby")]
			public const string lk_channelaccessprofile_createdby = "lk_channelaccessprofile_createdby";
			[Relationship("channelaccessprofile", EntityRole.Referenced, "channelaccessprofile_createdonbehalfby", "createdonbehalfby")]
			public const string lk_channelaccessprofile_createdonbehalfby = "lk_channelaccessprofile_createdonbehalfby";
			[Relationship("channelaccessprofile", EntityRole.Referenced, "channelaccessprofile_modifiedby", "modifiedby")]
			public const string lk_channelaccessprofile_modifiedby = "lk_channelaccessprofile_modifiedby";
			[Relationship("channelaccessprofile", EntityRole.Referenced, "channelaccessprofile_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_channelaccessprofile_modifiedonbehalfby = "lk_channelaccessprofile_modifiedonbehalfby";
			[Relationship("channelproperty", EntityRole.Referenced, "lk_ChannelProperty_createdby", "createdby")]
			public const string lk_ChannelProperty_createdby = "lk_ChannelProperty_createdby";
			[Relationship("channelproperty", EntityRole.Referenced, "lk_ChannelProperty_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ChannelProperty_createdonbehalfby = "lk_ChannelProperty_createdonbehalfby";
			[Relationship("channelproperty", EntityRole.Referenced, "lk_ChannelProperty_modifiedby", "modifiedby")]
			public const string lk_ChannelProperty_modifiedby = "lk_ChannelProperty_modifiedby";
			[Relationship("channelproperty", EntityRole.Referenced, "lk_ChannelProperty_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ChannelProperty_modifiedonbehalfby = "lk_ChannelProperty_modifiedonbehalfby";
			[Relationship("channelpropertygroup", EntityRole.Referenced, "lk_ChannelPropertyGroup_createdby", "createdby")]
			public const string lk_ChannelPropertyGroup_createdby = "lk_ChannelPropertyGroup_createdby";
			[Relationship("channelpropertygroup", EntityRole.Referenced, "lk_ChannelPropertyGroup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ChannelPropertyGroup_createdonbehalfby = "lk_ChannelPropertyGroup_createdonbehalfby";
			[Relationship("channelpropertygroup", EntityRole.Referenced, "lk_ChannelPropertyGroup_modifiedby", "modifiedby")]
			public const string lk_ChannelPropertyGroup_modifiedby = "lk_ChannelPropertyGroup_modifiedby";
			[Relationship("channelpropertygroup", EntityRole.Referenced, "lk_ChannelPropertyGroup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ChannelPropertyGroup_modifiedonbehalfby = "lk_ChannelPropertyGroup_modifiedonbehalfby";
			[Relationship("characteristic", EntityRole.Referenced, "lk_characteristic_createdby", "createdby")]
			public const string lk_characteristic_createdby = "lk_characteristic_createdby";
			[Relationship("characteristic", EntityRole.Referenced, "lk_characteristic_createdonbehalfby", "createdonbehalfby")]
			public const string lk_characteristic_createdonbehalfby = "lk_characteristic_createdonbehalfby";
			[Relationship("characteristic", EntityRole.Referenced, "lk_characteristic_modifiedby", "modifiedby")]
			public const string lk_characteristic_modifiedby = "lk_characteristic_modifiedby";
			[Relationship("characteristic", EntityRole.Referenced, "lk_characteristic_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_characteristic_modifiedonbehalfby = "lk_characteristic_modifiedonbehalfby";
			[Relationship("columnmapping", EntityRole.Referenced, "lk_columnmapping_createdby", "createdby")]
			public const string lk_columnmapping_createdby = "lk_columnmapping_createdby";
			[Relationship("columnmapping", EntityRole.Referenced, "lk_columnmapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_columnmapping_createdonbehalfby = "lk_columnmapping_createdonbehalfby";
			[Relationship("columnmapping", EntityRole.Referenced, "lk_columnmapping_modifiedby", "modifiedby")]
			public const string lk_columnmapping_modifiedby = "lk_columnmapping_modifiedby";
			[Relationship("columnmapping", EntityRole.Referenced, "lk_columnmapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_columnmapping_modifiedonbehalfby = "lk_columnmapping_modifiedonbehalfby";
			[Relationship("competitor", EntityRole.Referenced, "lk_competitor_createdonbehalfby", "createdonbehalfby")]
			public const string lk_competitor_createdonbehalfby = "lk_competitor_createdonbehalfby";
			[Relationship("competitor", EntityRole.Referenced, "lk_competitor_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_competitor_modifiedonbehalfby = "lk_competitor_modifiedonbehalfby";
			[Relationship("competitoraddress", EntityRole.Referenced, "lk_competitoraddress_createdonbehalfby", "createdonbehalfby")]
			public const string lk_competitoraddress_createdonbehalfby = "lk_competitoraddress_createdonbehalfby";
			[Relationship("competitoraddress", EntityRole.Referenced, "lk_competitoraddress_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_competitoraddress_modifiedonbehalfby = "lk_competitoraddress_modifiedonbehalfby";
			[Relationship("competitoraddress", EntityRole.Referenced, "lk_competitoraddressbase_createdby", "createdby")]
			public const string lk_competitoraddressbase_createdby = "lk_competitoraddressbase_createdby";
			[Relationship("competitoraddress", EntityRole.Referenced, "lk_competitoraddressbase_modifiedby", "modifiedby")]
			public const string lk_competitoraddressbase_modifiedby = "lk_competitoraddressbase_modifiedby";
			[Relationship("competitor", EntityRole.Referenced, "lk_competitorbase_createdby", "createdby")]
			public const string lk_competitorbase_createdby = "lk_competitorbase_createdby";
			[Relationship("competitor", EntityRole.Referenced, "lk_competitorbase_modifiedby", "modifiedby")]
			public const string lk_competitorbase_modifiedby = "lk_competitorbase_modifiedby";
			[Relationship("connection", EntityRole.Referenced, "lk_connectionbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_connectionbase_createdonbehalfby = "lk_connectionbase_createdonbehalfby";
			[Relationship("connection", EntityRole.Referenced, "lk_connectionbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_connectionbase_modifiedonbehalfby = "lk_connectionbase_modifiedonbehalfby";
			[Relationship("connectionrole", EntityRole.Referenced, "lk_connectionrolebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_connectionrolebase_createdonbehalfby = "lk_connectionrolebase_createdonbehalfby";
			[Relationship("connectionrole", EntityRole.Referenced, "lk_connectionrolebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_connectionrolebase_modifiedonbehalfby = "lk_connectionrolebase_modifiedonbehalfby";
			[Relationship("constraintbasedgroup", EntityRole.Referenced, "lk_constraintbasedgroup_createdby", "createdby")]
			public const string lk_constraintbasedgroup_createdby = "lk_constraintbasedgroup_createdby";
			[Relationship("constraintbasedgroup", EntityRole.Referenced, "lk_constraintbasedgroup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_constraintbasedgroup_createdonbehalfby = "lk_constraintbasedgroup_createdonbehalfby";
			[Relationship("constraintbasedgroup", EntityRole.Referenced, "lk_constraintbasedgroup_modifiedby", "modifiedby")]
			public const string lk_constraintbasedgroup_modifiedby = "lk_constraintbasedgroup_modifiedby";
			[Relationship("constraintbasedgroup", EntityRole.Referenced, "lk_constraintbasedgroup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_constraintbasedgroup_modifiedonbehalfby = "lk_constraintbasedgroup_modifiedonbehalfby";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "lk_contact_createdonbehalfby", "createdonbehalfby")]
			public const string lk_contact_createdonbehalfby = "lk_contact_createdonbehalfby";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "lk_contact_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_contact_modifiedonbehalfby = "lk_contact_modifiedonbehalfby";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "lk_contactbase_createdby", "createdby")]
			public const string lk_contactbase_createdby = "lk_contactbase_createdby";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "lk_contactbase_modifiedby", "modifiedby")]
			public const string lk_contactbase_modifiedby = "lk_contactbase_modifiedby";
			[Relationship("contract", EntityRole.Referenced, "lk_contract_createdonbehalfby", "createdonbehalfby")]
			public const string lk_contract_createdonbehalfby = "lk_contract_createdonbehalfby";
			[Relationship("contract", EntityRole.Referenced, "lk_contract_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_contract_modifiedonbehalfby = "lk_contract_modifiedonbehalfby";
			[Relationship("contract", EntityRole.Referenced, "lk_contractbase_createdby", "createdby")]
			public const string lk_contractbase_createdby = "lk_contractbase_createdby";
			[Relationship("contract", EntityRole.Referenced, "lk_contractbase_modifiedby", "modifiedby")]
			public const string lk_contractbase_modifiedby = "lk_contractbase_modifiedby";
			[Relationship("contractdetail", EntityRole.Referenced, "lk_contractdetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_contractdetail_createdonbehalfby = "lk_contractdetail_createdonbehalfby";
			[Relationship("contractdetail", EntityRole.Referenced, "lk_contractdetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_contractdetail_modifiedonbehalfby = "lk_contractdetail_modifiedonbehalfby";
			[Relationship("contractdetail", EntityRole.Referenced, "lk_contractdetailbase_createdby", "createdby")]
			public const string lk_contractdetailbase_createdby = "lk_contractdetailbase_createdby";
			[Relationship("contractdetail", EntityRole.Referenced, "lk_contractdetailbase_modifiedby", "modifiedby")]
			public const string lk_contractdetailbase_modifiedby = "lk_contractdetailbase_modifiedby";
			[Relationship("contracttemplate", EntityRole.Referenced, "lk_contracttemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_contracttemplate_createdonbehalfby = "lk_contracttemplate_createdonbehalfby";
			[Relationship("contracttemplate", EntityRole.Referenced, "lk_contracttemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_contracttemplate_modifiedonbehalfby = "lk_contracttemplate_modifiedonbehalfby";
			[Relationship("contracttemplate", EntityRole.Referenced, "lk_contracttemplatebase_createdby", "createdby")]
			public const string lk_contracttemplatebase_createdby = "lk_contracttemplatebase_createdby";
			[Relationship("contracttemplate", EntityRole.Referenced, "lk_contracttemplatebase_modifiedby", "modifiedby")]
			public const string lk_contracttemplatebase_modifiedby = "lk_contracttemplatebase_modifiedby";
			[Relationship("convertrule", EntityRole.Referenced, "lk_convertrule_createdby", "createdby")]
			public const string lk_convertrule_createdby = "lk_convertrule_createdby";
			[Relationship("convertrule", EntityRole.Referenced, "lk_ConvertRule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ConvertRule_createdonbehalfby = "lk_ConvertRule_createdonbehalfby";
			[Relationship("convertrule", EntityRole.Referenced, "lk_ConvertRule_modifiedby", "modifiedby")]
			public const string lk_ConvertRule_modifiedby = "lk_ConvertRule_modifiedby";
			[Relationship("convertrule", EntityRole.Referenced, "lk_ConvertRule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ConvertRule_modifiedonbehalfby = "lk_ConvertRule_modifiedonbehalfby";
			[Relationship("convertruleitem", EntityRole.Referenced, "lk_convertruleitembase_createdby", "createdby")]
			public const string lk_convertruleitembase_createdby = "lk_convertruleitembase_createdby";
			[Relationship("convertruleitem", EntityRole.Referenced, "lk_convertruleitembase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_convertruleitembase_createdonbehalfby = "lk_convertruleitembase_createdonbehalfby";
			[Relationship("convertruleitem", EntityRole.Referenced, "lk_convertruleitembase_modifiedby", "modifiedby")]
			public const string lk_convertruleitembase_modifiedby = "lk_convertruleitembase_modifiedby";
			[Relationship("convertruleitem", EntityRole.Referenced, "lk_convertruleitembase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_convertruleitembase_modifiedonbehalfby = "lk_convertruleitembase_modifiedonbehalfby";
			[Relationship("customcontrol", EntityRole.Referenced, "lk_customcontrol_createdby", "createdby")]
			public const string lk_customcontrol_createdby = "lk_customcontrol_createdby";
			[Relationship("customcontrol", EntityRole.Referenced, "lk_customcontrol_createdonbehalfby", "createdonbehalfby")]
			public const string lk_customcontrol_createdonbehalfby = "lk_customcontrol_createdonbehalfby";
			[Relationship("customcontrol", EntityRole.Referenced, "lk_customcontrol_modifiedby", "modifiedby")]
			public const string lk_customcontrol_modifiedby = "lk_customcontrol_modifiedby";
			[Relationship("customcontrol", EntityRole.Referenced, "lk_customcontrol_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_customcontrol_modifiedonbehalfby = "lk_customcontrol_modifiedonbehalfby";
			[Relationship("customcontroldefaultconfig", EntityRole.Referenced, "lk_customcontroldefaultconfig_createdby", "createdby")]
			public const string lk_customcontroldefaultconfig_createdby = "lk_customcontroldefaultconfig_createdby";
			[Relationship("customcontroldefaultconfig", EntityRole.Referenced, "lk_customcontroldefaultconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_customcontroldefaultconfig_createdonbehalfby = "lk_customcontroldefaultconfig_createdonbehalfby";
			[Relationship("customcontroldefaultconfig", EntityRole.Referenced, "lk_customcontroldefaultconfig_modifiedby", "modifiedby")]
			public const string lk_customcontroldefaultconfig_modifiedby = "lk_customcontroldefaultconfig_modifiedby";
			[Relationship("customcontroldefaultconfig", EntityRole.Referenced, "lk_customcontroldefaultconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_customcontroldefaultconfig_modifiedonbehalfby = "lk_customcontroldefaultconfig_modifiedonbehalfby";
			[Relationship("customcontrolresource", EntityRole.Referenced, "lk_customcontrolresource_createdby", "createdby")]
			public const string lk_customcontrolresource_createdby = "lk_customcontrolresource_createdby";
			[Relationship("customcontrolresource", EntityRole.Referenced, "lk_customcontrolresource_createdonbehalfby", "createdonbehalfby")]
			public const string lk_customcontrolresource_createdonbehalfby = "lk_customcontrolresource_createdonbehalfby";
			[Relationship("customcontrolresource", EntityRole.Referenced, "lk_customcontrolresource_modifiedby", "modifiedby")]
			public const string lk_customcontrolresource_modifiedby = "lk_customcontrolresource_modifiedby";
			[Relationship("customcontrolresource", EntityRole.Referenced, "lk_customcontrolresource_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_customcontrolresource_modifiedonbehalfby = "lk_customcontrolresource_modifiedonbehalfby";
			[Relationship("customeraddress", EntityRole.Referenced, "lk_customeraddress_createdonbehalfby", "createdonbehalfby")]
			public const string lk_customeraddress_createdonbehalfby = "lk_customeraddress_createdonbehalfby";
			[Relationship("customeraddress", EntityRole.Referenced, "lk_customeraddress_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_customeraddress_modifiedonbehalfby = "lk_customeraddress_modifiedonbehalfby";
			[Relationship("customeraddress", EntityRole.Referenced, "lk_customeraddressbase_createdby", "createdby")]
			public const string lk_customeraddressbase_createdby = "lk_customeraddressbase_createdby";
			[Relationship("customeraddress", EntityRole.Referenced, "lk_customeraddressbase_modifiedby", "modifiedby")]
			public const string lk_customeraddressbase_modifiedby = "lk_customeraddressbase_modifiedby";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "lk_customeropportunityrole_createdby", "createdby")]
			public const string lk_customeropportunityrole_createdby = "lk_customeropportunityrole_createdby";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "lk_customeropportunityrole_createdonbehalfby", "createdonbehalfby")]
			public const string lk_customeropportunityrole_createdonbehalfby = "lk_customeropportunityrole_createdonbehalfby";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "lk_customeropportunityrole_modifiedby", "modifiedby")]
			public const string lk_customeropportunityrole_modifiedby = "lk_customeropportunityrole_modifiedby";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "lk_customeropportunityrole_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_customeropportunityrole_modifiedonbehalfby = "lk_customeropportunityrole_modifiedonbehalfby";
			[Relationship("delveactionhub", EntityRole.Referenced, "lk_delveactionhub_createdby", "createdby")]
			public const string lk_delveactionhub_createdby = "lk_delveactionhub_createdby";
			[Relationship("delveactionhub", EntityRole.Referenced, "lk_delveactionhub_createdonbehalfby", "createdonbehalfby")]
			public const string lk_delveactionhub_createdonbehalfby = "lk_delveactionhub_createdonbehalfby";
			[Relationship("delveactionhub", EntityRole.Referenced, "lk_delveactionhub_modifiedby", "modifiedby")]
			public const string lk_delveactionhub_modifiedby = "lk_delveactionhub_modifiedby";
			[Relationship("delveactionhub", EntityRole.Referenced, "lk_delveactionhub_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_delveactionhub_modifiedonbehalfby = "lk_delveactionhub_modifiedonbehalfby";
			[Relationship("discount", EntityRole.Referenced, "lk_discount_createdonbehalfby", "createdonbehalfby")]
			public const string lk_discount_createdonbehalfby = "lk_discount_createdonbehalfby";
			[Relationship("discount", EntityRole.Referenced, "lk_discount_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_discount_modifiedonbehalfby = "lk_discount_modifiedonbehalfby";
			[Relationship("discount", EntityRole.Referenced, "lk_discountbase_createdby", "createdby")]
			public const string lk_discountbase_createdby = "lk_discountbase_createdby";
			[Relationship("discount", EntityRole.Referenced, "lk_discountbase_modifiedby", "modifiedby")]
			public const string lk_discountbase_modifiedby = "lk_discountbase_modifiedby";
			[Relationship("discounttype", EntityRole.Referenced, "lk_discounttype_createdonbehalfby", "createdonbehalfby")]
			public const string lk_discounttype_createdonbehalfby = "lk_discounttype_createdonbehalfby";
			[Relationship("discounttype", EntityRole.Referenced, "lk_discounttype_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_discounttype_modifiedonbehalfby = "lk_discounttype_modifiedonbehalfby";
			[Relationship("discounttype", EntityRole.Referenced, "lk_discounttypebase_createdby", "createdby")]
			public const string lk_discounttypebase_createdby = "lk_discounttypebase_createdby";
			[Relationship("discounttype", EntityRole.Referenced, "lk_discounttypebase_modifiedby", "modifiedby")]
			public const string lk_discounttypebase_modifiedby = "lk_discounttypebase_modifiedby";
			[Relationship("displaystring", EntityRole.Referenced, "lk_DisplayStringbase_createdby", "createdby")]
			public const string lk_DisplayStringbase_createdby = "lk_DisplayStringbase_createdby";
			[Relationship("displaystring", EntityRole.Referenced, "lk_DisplayStringbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_DisplayStringbase_createdonbehalfby = "lk_DisplayStringbase_createdonbehalfby";
			[Relationship("displaystring", EntityRole.Referenced, "lk_DisplayStringbase_modifiedby", "modifiedby")]
			public const string lk_DisplayStringbase_modifiedby = "lk_DisplayStringbase_modifiedby";
			[Relationship("displaystring", EntityRole.Referenced, "lk_DisplayStringbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_DisplayStringbase_modifiedonbehalfby = "lk_DisplayStringbase_modifiedonbehalfby";
			[Relationship("documentindex", EntityRole.Referenced, "lk_documentindex_createdby", "createdby")]
			public const string lk_documentindex_createdby = "lk_documentindex_createdby";
			[Relationship("documentindex", EntityRole.Referenced, "lk_documentindex_createdonbehalfby", "createdonbehalfby")]
			public const string lk_documentindex_createdonbehalfby = "lk_documentindex_createdonbehalfby";
			[Relationship("documentindex", EntityRole.Referenced, "lk_documentindex_modifiedby", "modifiedby")]
			public const string lk_documentindex_modifiedby = "lk_documentindex_modifiedby";
			[Relationship("documentindex", EntityRole.Referenced, "lk_documentindex_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_documentindex_modifiedonbehalfby = "lk_documentindex_modifiedonbehalfby";
			[Relationship("documenttemplate", EntityRole.Referenced, "lk_documenttemplatebase_createdby", "createdby")]
			public const string lk_documenttemplatebase_createdby = "lk_documenttemplatebase_createdby";
			[Relationship("documenttemplate", EntityRole.Referenced, "lk_documenttemplatebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_documenttemplatebase_createdonbehalfby = "lk_documenttemplatebase_createdonbehalfby";
			[Relationship("documenttemplate", EntityRole.Referenced, "lk_documenttemplatebase_modifiedby", "modifiedby")]
			public const string lk_documenttemplatebase_modifiedby = "lk_documenttemplatebase_modifiedby";
			[Relationship("documenttemplate", EntityRole.Referenced, "lk_documenttemplatebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_documenttemplatebase_modifiedonbehalfby = "lk_documenttemplatebase_modifiedonbehalfby";
			[Relationship("duplicaterule", EntityRole.Referenced, "lk_duplicaterule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_duplicaterule_createdonbehalfby = "lk_duplicaterule_createdonbehalfby";
			[Relationship("duplicaterule", EntityRole.Referenced, "lk_duplicaterule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_duplicaterule_modifiedonbehalfby = "lk_duplicaterule_modifiedonbehalfby";
			[Relationship("duplicaterule", EntityRole.Referenced, "lk_duplicaterulebase_createdby", "createdby")]
			public const string lk_duplicaterulebase_createdby = "lk_duplicaterulebase_createdby";
			[Relationship("duplicaterule", EntityRole.Referenced, "lk_duplicaterulebase_modifiedby", "modifiedby")]
			public const string lk_duplicaterulebase_modifiedby = "lk_duplicaterulebase_modifiedby";
			[Relationship("duplicaterulecondition", EntityRole.Referenced, "lk_duplicaterulecondition_createdonbehalfby", "createdonbehalfby")]
			public const string lk_duplicaterulecondition_createdonbehalfby = "lk_duplicaterulecondition_createdonbehalfby";
			[Relationship("duplicaterulecondition", EntityRole.Referenced, "lk_duplicaterulecondition_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_duplicaterulecondition_modifiedonbehalfby = "lk_duplicaterulecondition_modifiedonbehalfby";
			[Relationship("duplicaterulecondition", EntityRole.Referenced, "lk_duplicateruleconditionbase_createdby", "createdby")]
			public const string lk_duplicateruleconditionbase_createdby = "lk_duplicateruleconditionbase_createdby";
			[Relationship("duplicaterulecondition", EntityRole.Referenced, "lk_duplicateruleconditionbase_modifiedby", "modifiedby")]
			public const string lk_duplicateruleconditionbase_modifiedby = "lk_duplicateruleconditionbase_modifiedby";
			[Relationship("dynamicproperty", EntityRole.Referenced, "lk_DynamicProperty_createdby", "createdby")]
			public const string lk_DynamicProperty_createdby = "lk_DynamicProperty_createdby";
			[Relationship("dynamicproperty", EntityRole.Referenced, "lk_DynamicProperty_createdonbehalfby", "createdonbehalfby")]
			public const string lk_DynamicProperty_createdonbehalfby = "lk_DynamicProperty_createdonbehalfby";
			[Relationship("dynamicproperty", EntityRole.Referenced, "lk_DynamicProperty_modifiedby", "modifiedby")]
			public const string lk_DynamicProperty_modifiedby = "lk_DynamicProperty_modifiedby";
			[Relationship("dynamicproperty", EntityRole.Referenced, "lk_DynamicProperty_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_DynamicProperty_modifiedonbehalfby = "lk_DynamicProperty_modifiedonbehalfby";
			[Relationship("dynamicpropertyassociation", EntityRole.Referenced, "lk_DynamicPropertyAssociationattribute_createdby", "createdby")]
			public const string lk_DynamicPropertyAssociationattribute_createdby = "lk_DynamicPropertyAssociationattribute_createdby";
			[Relationship("dynamicpropertyassociation", EntityRole.Referenced, "lk_DynamicPropertyAssociationattribute_CreatedOnBehalfBy", "createdonbehalfby")]
			public const string lk_DynamicPropertyAssociationattribute_CreatedOnBehalfBy = "lk_DynamicPropertyAssociationattribute_CreatedOnBehalfBy";
			[Relationship("dynamicpropertyassociation", EntityRole.Referenced, "lk_DynamicPropertyAssociationattribute_ModifiedBy", "modifiedby")]
			public const string lk_DynamicPropertyAssociationattribute_ModifiedBy = "lk_DynamicPropertyAssociationattribute_ModifiedBy";
			[Relationship("dynamicpropertyassociation", EntityRole.Referenced, "lk_DynamicPropertyAssociationattribute_ModifiedOnBehalfBy", "modifiedonbehalfby")]
			public const string lk_DynamicPropertyAssociationattribute_ModifiedOnBehalfBy = "lk_DynamicPropertyAssociationattribute_ModifiedOnBehalfBy";
			[Relationship("dynamicpropertyinstance", EntityRole.Referenced, "lk_Dynamicpropertyinsatanceattribute_createdonbehalfby", "createdonbehalfby")]
			public const string lk_Dynamicpropertyinsatanceattribute_createdonbehalfby = "lk_Dynamicpropertyinsatanceattribute_createdonbehalfby";
			[Relationship("dynamicpropertyinstance", EntityRole.Referenced, "lk_Dynamicpropertyinsatanceattribute_ModifiedBy", "modifiedby")]
			public const string lk_Dynamicpropertyinsatanceattribute_ModifiedBy = "lk_Dynamicpropertyinsatanceattribute_ModifiedBy";
			[Relationship("dynamicpropertyinstance", EntityRole.Referenced, "lk_Dynamicpropertyinsatanceattribute_ModifiedOnBehalfBy", "modifiedonbehalfby")]
			public const string lk_Dynamicpropertyinsatanceattribute_ModifiedOnBehalfBy = "lk_Dynamicpropertyinsatanceattribute_ModifiedOnBehalfBy";
			[Relationship("dynamicpropertyoptionsetitem", EntityRole.Referenced, "lk_DynamicPropertyOptionSetItem_createdby", "createdby")]
			public const string lk_DynamicPropertyOptionSetItem_createdby = "lk_DynamicPropertyOptionSetItem_createdby";
			[Relationship("dynamicpropertyoptionsetitem", EntityRole.Referenced, "lk_DynamicPropertyOptionSetItem_createdonbehalfby", "createdonbehalfby")]
			public const string lk_DynamicPropertyOptionSetItem_createdonbehalfby = "lk_DynamicPropertyOptionSetItem_createdonbehalfby";
			[Relationship("dynamicpropertyoptionsetitem", EntityRole.Referenced, "lk_DynamicPropertyOptionSetItem_modifiedby", "modifiedby")]
			public const string lk_DynamicPropertyOptionSetItem_modifiedby = "lk_DynamicPropertyOptionSetItem_modifiedby";
			[Relationship("dynamicpropertyoptionsetitem", EntityRole.Referenced, "lk_DynamicPropertyOptionSetItem_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_DynamicPropertyOptionSetItem_modifiedonbehalfby = "lk_DynamicPropertyOptionSetItem_modifiedonbehalfby";
			[Relationship("email", EntityRole.Referenced, "lk_email_createdby", "createdby")]
			public const string lk_email_createdby = "lk_email_createdby";
			[Relationship("email", EntityRole.Referenced, "lk_email_createdonbehalfby", "createdonbehalfby")]
			public const string lk_email_createdonbehalfby = "lk_email_createdonbehalfby";
			[Relationship("email", EntityRole.Referenced, "lk_email_modifiedby", "modifiedby")]
			public const string lk_email_modifiedby = "lk_email_modifiedby";
			[Relationship("email", EntityRole.Referenced, "lk_email_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_email_modifiedonbehalfby = "lk_email_modifiedonbehalfby";
			[Relationship("emailserverprofile", EntityRole.Referenced, "lk_emailserverprofile_createdby", "createdby")]
			public const string lk_emailserverprofile_createdby = "lk_emailserverprofile_createdby";
			[Relationship("emailserverprofile", EntityRole.Referenced, "lk_emailserverprofile_createdonbehalfby", "createdonbehalfby")]
			public const string lk_emailserverprofile_createdonbehalfby = "lk_emailserverprofile_createdonbehalfby";
			[Relationship("emailserverprofile", EntityRole.Referenced, "lk_emailserverprofile_modifiedby", "modifiedby")]
			public const string lk_emailserverprofile_modifiedby = "lk_emailserverprofile_modifiedby";
			[Relationship("emailserverprofile", EntityRole.Referenced, "lk_emailserverprofile_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_emailserverprofile_modifiedonbehalfby = "lk_emailserverprofile_modifiedonbehalfby";
			[Relationship("emailsignature", EntityRole.Referenced, "lk_emailsignaturebase_createdby", "createdby")]
			public const string lk_emailsignaturebase_createdby = "lk_emailsignaturebase_createdby";
			[Relationship("emailsignature", EntityRole.Referenced, "lk_emailsignaturebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_emailsignaturebase_createdonbehalfby = "lk_emailsignaturebase_createdonbehalfby";
			[Relationship("emailsignature", EntityRole.Referenced, "lk_emailsignaturebase_modifiedby", "modifiedby")]
			public const string lk_emailsignaturebase_modifiedby = "lk_emailsignaturebase_modifiedby";
			[Relationship("emailsignature", EntityRole.Referenced, "lk_emailsignaturebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_emailsignaturebase_modifiedonbehalfby = "lk_emailsignaturebase_modifiedonbehalfby";
			[Relationship("entitlement", EntityRole.Referenced, "lk_entitlement_createdby", "createdby")]
			public const string lk_entitlement_createdby = "lk_entitlement_createdby";
			[Relationship("entitlement", EntityRole.Referenced, "lk_entitlement_createdonbehalfby", "createdonbehalfby")]
			public const string lk_entitlement_createdonbehalfby = "lk_entitlement_createdonbehalfby";
			[Relationship("entitlement", EntityRole.Referenced, "lk_entitlement_modifiedby", "modifiedby")]
			public const string lk_entitlement_modifiedby = "lk_entitlement_modifiedby";
			[Relationship("entitlement", EntityRole.Referenced, "lk_entitlement_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_entitlement_modifiedonbehalfby = "lk_entitlement_modifiedonbehalfby";
			[Relationship("entitlementchannel", EntityRole.Referenced, "lk_entitlementchannel_createdby", "createdby")]
			public const string lk_entitlementchannel_createdby = "lk_entitlementchannel_createdby";
			[Relationship("entitlementchannel", EntityRole.Referenced, "lk_entitlementchannel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_entitlementchannel_createdonbehalfby = "lk_entitlementchannel_createdonbehalfby";
			[Relationship("entitlementchannel", EntityRole.Referenced, "lk_entitlementchannel_modifiedby", "modifiedby")]
			public const string lk_entitlementchannel_modifiedby = "lk_entitlementchannel_modifiedby";
			[Relationship("entitlementchannel", EntityRole.Referenced, "lk_entitlementchannel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_entitlementchannel_modifiedonbehalfby = "lk_entitlementchannel_modifiedonbehalfby";
			[Relationship("entitlemententityallocationtypemapping", EntityRole.Referenced, "lk_entitlemententityallocationtypemapping_createdby", "createdby")]
			public const string lk_entitlemententityallocationtypemapping_createdby = "lk_entitlemententityallocationtypemapping_createdby";
			[Relationship("entitlemententityallocationtypemapping", EntityRole.Referenced, "lk_entitlemententityallocationtypemapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_entitlemententityallocationtypemapping_createdonbehalfby = "lk_entitlemententityallocationtypemapping_createdonbehalfby";
			[Relationship("entitlemententityallocationtypemapping", EntityRole.Referenced, "lk_entitlemententityallocationtypemapping_modifiedby", "modifiedby")]
			public const string lk_entitlemententityallocationtypemapping_modifiedby = "lk_entitlemententityallocationtypemapping_modifiedby";
			[Relationship("entitlemententityallocationtypemapping", EntityRole.Referenced, "lk_entitlemententityallocationtypemapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_entitlemententityallocationtypemapping_modifiedonbehalfby = "lk_entitlemententityallocationtypemapping_modifiedonbehalfby";
			[Relationship("entitlementtemplate", EntityRole.Referenced, "lk_entitlementtemplate_createdby", "createdby")]
			public const string lk_entitlementtemplate_createdby = "lk_entitlementtemplate_createdby";
			[Relationship("entitlementtemplate", EntityRole.Referenced, "lk_entitlementtemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_entitlementtemplate_createdonbehalfby = "lk_entitlementtemplate_createdonbehalfby";
			[Relationship("entitlementtemplate", EntityRole.Referenced, "lk_entitlementtemplate_modifiedby", "modifiedby")]
			public const string lk_entitlementtemplate_modifiedby = "lk_entitlementtemplate_modifiedby";
			[Relationship("entitlementtemplate", EntityRole.Referenced, "lk_entitlementtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_entitlementtemplate_modifiedonbehalfby = "lk_entitlementtemplate_modifiedonbehalfby";
			[Relationship("entitlementtemplatechannel", EntityRole.Referenced, "lk_entitlementtemplatechannel_createdby", "createdby")]
			public const string lk_entitlementtemplatechannel_createdby = "lk_entitlementtemplatechannel_createdby";
			[Relationship("entitlementtemplatechannel", EntityRole.Referenced, "lk_entitlementtemplatechannel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_entitlementtemplatechannel_createdonbehalfby = "lk_entitlementtemplatechannel_createdonbehalfby";
			[Relationship("entitlementtemplatechannel", EntityRole.Referenced, "lk_entitlementtemplatechannel_modifiedby", "modifiedby")]
			public const string lk_entitlementtemplatechannel_modifiedby = "lk_entitlementtemplatechannel_modifiedby";
			[Relationship("entitlementtemplatechannel", EntityRole.Referenced, "lk_entitlementtemplatechannel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_entitlementtemplatechannel_modifiedonbehalfby = "lk_entitlementtemplatechannel_modifiedonbehalfby";
			[Relationship("entitymap", EntityRole.Referenced, "lk_entitymap_createdonbehalfby", "createdonbehalfby")]
			public const string lk_entitymap_createdonbehalfby = "lk_entitymap_createdonbehalfby";
			[Relationship("entitymap", EntityRole.Referenced, "lk_entitymap_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_entitymap_modifiedonbehalfby = "lk_entitymap_modifiedonbehalfby";
			[Relationship("equipment", EntityRole.Referenced, "lk_equipment_createdby", "createdby")]
			public const string lk_equipment_createdby = "lk_equipment_createdby";
			[Relationship("equipment", EntityRole.Referenced, "lk_equipment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_equipment_createdonbehalfby = "lk_equipment_createdonbehalfby";
			[Relationship("equipment", EntityRole.Referenced, "lk_equipment_modifiedby", "modifiedby")]
			public const string lk_equipment_modifiedby = "lk_equipment_modifiedby";
			[Relationship("equipment", EntityRole.Referenced, "lk_equipment_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_equipment_modifiedonbehalfby = "lk_equipment_modifiedonbehalfby";
			[Relationship("expanderevent", EntityRole.Referenced, "lk_expanderevent_createdonbehalfby", "createdonbehalfby")]
			public const string lk_expanderevent_createdonbehalfby = "lk_expanderevent_createdonbehalfby";
			[Relationship("expanderevent", EntityRole.Referenced, "lk_expanderevent_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_expanderevent_modifiedonbehalfby = "lk_expanderevent_modifiedonbehalfby";
			[Relationship("expiredprocess", EntityRole.Referenced, "lk_expiredprocess_createdby", "createdby")]
			public const string lk_expiredprocess_createdby = "lk_expiredprocess_createdby";
			[Relationship("expiredprocess", EntityRole.Referenced, "lk_expiredprocess_createdonbehalfby", "createdonbehalfby")]
			public const string lk_expiredprocess_createdonbehalfby = "lk_expiredprocess_createdonbehalfby";
			[Relationship("expiredprocess", EntityRole.Referenced, "lk_expiredprocess_modifiedby", "modifiedby")]
			public const string lk_expiredprocess_modifiedby = "lk_expiredprocess_modifiedby";
			[Relationship("expiredprocess", EntityRole.Referenced, "lk_expiredprocess_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_expiredprocess_modifiedonbehalfby = "lk_expiredprocess_modifiedonbehalfby";
			[Relationship("externalparty", EntityRole.Referenced, "systemuser_externalparty_createdby", "createdby")]
			public const string lk_externalparty_createdby = "lk_externalparty_createdby";
			[Relationship("externalparty", EntityRole.Referenced, "systemuser_externalparty_createdonbehalfby", "createdonbehalfby")]
			public const string lk_externalparty_createdonbehalfby = "lk_externalparty_createdonbehalfby";
			[Relationship("externalparty", EntityRole.Referenced, "systemuser_externalparty_modifiedby", "modifiedby")]
			public const string lk_externalparty_modifiedby = "lk_externalparty_modifiedby";
			[Relationship("externalparty", EntityRole.Referenced, "systemuser_externalparty_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_externalparty_modifiedonbehalfby = "lk_externalparty_modifiedonbehalfby";
			[Relationship("externalpartyitem", EntityRole.Referenced, "lk_externalpartyitem_createdby", "createdby")]
			public const string lk_externalpartyitem_createdby = "lk_externalpartyitem_createdby";
			[Relationship("externalpartyitem", EntityRole.Referenced, "lk_externalpartyitem_createdonbehalfby", "createdonbehalfby")]
			public const string lk_externalpartyitem_createdonbehalfby = "lk_externalpartyitem_createdonbehalfby";
			[Relationship("externalpartyitem", EntityRole.Referenced, "lk_externalpartyitem_modifiedby", "modifiedby")]
			public const string lk_externalpartyitem_modifiedby = "lk_externalpartyitem_modifiedby";
			[Relationship("externalpartyitem", EntityRole.Referenced, "lk_externalpartyitem_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_externalpartyitem_modifiedonbehalfby = "lk_externalpartyitem_modifiedonbehalfby";
			[Relationship("fax", EntityRole.Referenced, "lk_fax_createdby", "createdby")]
			public const string lk_fax_createdby = "lk_fax_createdby";
			[Relationship("fax", EntityRole.Referenced, "lk_fax_createdonbehalfby", "createdonbehalfby")]
			public const string lk_fax_createdonbehalfby = "lk_fax_createdonbehalfby";
			[Relationship("fax", EntityRole.Referenced, "lk_fax_modifiedby", "modifiedby")]
			public const string lk_fax_modifiedby = "lk_fax_modifiedby";
			[Relationship("fax", EntityRole.Referenced, "lk_fax_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_fax_modifiedonbehalfby = "lk_fax_modifiedonbehalfby";
			[Relationship("feedback", EntityRole.Referenced, "lk_feedback_closedby", "closedby")]
			public const string lk_feedback_closedby = "lk_feedback_closedby";
			[Relationship("feedback", EntityRole.Referenced, "lk_feedback_createdby", "createdby")]
			public const string lk_feedback_createdby = "lk_feedback_createdby";
			[Relationship("feedback", EntityRole.Referenced, "lk_feedback_createdonbehalfby", "createdonbehalfby")]
			public const string lk_feedback_createdonbehalfby = "lk_feedback_createdonbehalfby";
			[Relationship("feedback", EntityRole.Referenced, "lk_feedback_modifiedby", "modifiedby")]
			public const string lk_feedback_modifiedby = "lk_feedback_modifiedby";
			[Relationship("feedback", EntityRole.Referenced, "lk_feedback_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_feedback_modifiedonbehalfby = "lk_feedback_modifiedonbehalfby";
			[Relationship("fieldsecurityprofile", EntityRole.Referenced, "lk_fieldsecurityprofile_createdby", "createdby")]
			public const string lk_fieldsecurityprofile_createdby = "lk_fieldsecurityprofile_createdby";
			[Relationship("fieldsecurityprofile", EntityRole.Referenced, "lk_fieldsecurityprofile_createdonbehalfby", "createdonbehalfby")]
			public const string lk_fieldsecurityprofile_createdonbehalfby = "lk_fieldsecurityprofile_createdonbehalfby";
			[Relationship("fieldsecurityprofile", EntityRole.Referenced, "lk_fieldsecurityprofile_modifiedby", "modifiedby")]
			public const string lk_fieldsecurityprofile_modifiedby = "lk_fieldsecurityprofile_modifiedby";
			[Relationship("fieldsecurityprofile", EntityRole.Referenced, "lk_fieldsecurityprofile_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_fieldsecurityprofile_modifiedonbehalfby = "lk_fieldsecurityprofile_modifiedonbehalfby";
			[Relationship("fixedmonthlyfiscalcalendar", EntityRole.Referenced, "lk_fixedmonthlyfiscalcalendar_createdby", "createdby")]
			public const string lk_fixedmonthlyfiscalcalendar_createdby = "lk_fixedmonthlyfiscalcalendar_createdby";
			[Relationship("fixedmonthlyfiscalcalendar", EntityRole.Referenced, "lk_fixedmonthlyfiscalcalendar_createdonbehalfby", "createdonbehalfby")]
			public const string lk_fixedmonthlyfiscalcalendar_createdonbehalfby = "lk_fixedmonthlyfiscalcalendar_createdonbehalfby";
			[Relationship("fixedmonthlyfiscalcalendar", EntityRole.Referenced, "lk_fixedmonthlyfiscalcalendar_modifiedby", "modifiedby")]
			public const string lk_fixedmonthlyfiscalcalendar_modifiedby = "lk_fixedmonthlyfiscalcalendar_modifiedby";
			[Relationship("fixedmonthlyfiscalcalendar", EntityRole.Referenced, "lk_fixedmonthlyfiscalcalendar_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_fixedmonthlyfiscalcalendar_modifiedonbehalfby = "lk_fixedmonthlyfiscalcalendar_modifiedonbehalfby";
			[Relationship("fixedmonthlyfiscalcalendar", EntityRole.Referenced, "lk_fixedmonthlyfiscalcalendar_salespersonid", "salespersonid")]
			public const string lk_fixedmonthlyfiscalcalendar_salespersonid = "lk_fixedmonthlyfiscalcalendar_salespersonid";
			[Relationship("goal", EntityRole.Referenced, "lk_goal_createdby", "createdby")]
			public const string lk_goal_createdby = "lk_goal_createdby";
			[Relationship("goal", EntityRole.Referenced, "lk_goal_createdonbehalfby", "createdonbehalfby")]
			public const string lk_goal_createdonbehalfby = "lk_goal_createdonbehalfby";
			[Relationship("goal", EntityRole.Referenced, "lk_goal_modifiedby", "modifiedby")]
			public const string lk_goal_modifiedby = "lk_goal_modifiedby";
			[Relationship("goal", EntityRole.Referenced, "lk_goal_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_goal_modifiedonbehalfby = "lk_goal_modifiedonbehalfby";
			[Relationship("goalrollupquery", EntityRole.Referenced, "lk_goalrollupquery_createdby", "createdby")]
			public const string lk_goalrollupquery_createdby = "lk_goalrollupquery_createdby";
			[Relationship("goalrollupquery", EntityRole.Referenced, "lk_goalrollupquery_createdonbehalfby", "createdonbehalfby")]
			public const string lk_goalrollupquery_createdonbehalfby = "lk_goalrollupquery_createdonbehalfby";
			[Relationship("goalrollupquery", EntityRole.Referenced, "lk_goalrollupquery_modifiedby", "modifiedby")]
			public const string lk_goalrollupquery_modifiedby = "lk_goalrollupquery_modifiedby";
			[Relationship("goalrollupquery", EntityRole.Referenced, "lk_goalrollupquery_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_goalrollupquery_modifiedonbehalfby = "lk_goalrollupquery_modifiedonbehalfby";
			[Relationship("import", EntityRole.Referenced, "lk_import_createdonbehalfby", "createdonbehalfby")]
			public const string lk_import_createdonbehalfby = "lk_import_createdonbehalfby";
			[Relationship("import", EntityRole.Referenced, "lk_import_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_import_modifiedonbehalfby = "lk_import_modifiedonbehalfby";
			[Relationship("import", EntityRole.Referenced, "lk_importbase_createdby", "createdby")]
			public const string lk_importbase_createdby = "lk_importbase_createdby";
			[Relationship("import", EntityRole.Referenced, "lk_importbase_modifiedby", "modifiedby")]
			public const string lk_importbase_modifiedby = "lk_importbase_modifiedby";
			[Relationship("importdata", EntityRole.Referenced, "lk_importdata_createdonbehalfby", "createdonbehalfby")]
			public const string lk_importdata_createdonbehalfby = "lk_importdata_createdonbehalfby";
			[Relationship("importdata", EntityRole.Referenced, "lk_importdata_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_importdata_modifiedonbehalfby = "lk_importdata_modifiedonbehalfby";
			[Relationship("importdata", EntityRole.Referenced, "lk_importdatabase_createdby", "createdby")]
			public const string lk_importdatabase_createdby = "lk_importdatabase_createdby";
			[Relationship("importdata", EntityRole.Referenced, "lk_importdatabase_modifiedby", "modifiedby")]
			public const string lk_importdatabase_modifiedby = "lk_importdatabase_modifiedby";
			[Relationship("importentitymapping", EntityRole.Referenced, "lk_importentitymapping_createdby", "createdby")]
			public const string lk_importentitymapping_createdby = "lk_importentitymapping_createdby";
			[Relationship("importentitymapping", EntityRole.Referenced, "lk_importentitymapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_importentitymapping_createdonbehalfby = "lk_importentitymapping_createdonbehalfby";
			[Relationship("importentitymapping", EntityRole.Referenced, "lk_importentitymapping_modifiedby", "modifiedby")]
			public const string lk_importentitymapping_modifiedby = "lk_importentitymapping_modifiedby";
			[Relationship("importentitymapping", EntityRole.Referenced, "lk_importentitymapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_importentitymapping_modifiedonbehalfby = "lk_importentitymapping_modifiedonbehalfby";
			[Relationship("importfile", EntityRole.Referenced, "lk_importfilebase_createdby", "createdby")]
			public const string lk_importfilebase_createdby = "lk_importfilebase_createdby";
			[Relationship("importfile", EntityRole.Referenced, "lk_importfilebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_importfilebase_createdonbehalfby = "lk_importfilebase_createdonbehalfby";
			[Relationship("importfile", EntityRole.Referenced, "lk_importfilebase_modifiedby", "modifiedby")]
			public const string lk_importfilebase_modifiedby = "lk_importfilebase_modifiedby";
			[Relationship("importfile", EntityRole.Referenced, "lk_importfilebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_importfilebase_modifiedonbehalfby = "lk_importfilebase_modifiedonbehalfby";
			[Relationship("importjob", EntityRole.Referenced, "lk_importjobbase_createdby", "createdby")]
			public const string lk_importjobbase_createdby = "lk_importjobbase_createdby";
			[Relationship("importjob", EntityRole.Referenced, "lk_importjobbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_importjobbase_createdonbehalfby = "lk_importjobbase_createdonbehalfby";
			[Relationship("importjob", EntityRole.Referenced, "lk_importjobbase_modifiedby", "modifiedby")]
			public const string lk_importjobbase_modifiedby = "lk_importjobbase_modifiedby";
			[Relationship("importjob", EntityRole.Referenced, "lk_importjobbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_importjobbase_modifiedonbehalfby = "lk_importjobbase_modifiedonbehalfby";
			[Relationship("importlog", EntityRole.Referenced, "lk_importlog_createdonbehalfby", "createdonbehalfby")]
			public const string lk_importlog_createdonbehalfby = "lk_importlog_createdonbehalfby";
			[Relationship("importlog", EntityRole.Referenced, "lk_importlog_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_importlog_modifiedonbehalfby = "lk_importlog_modifiedonbehalfby";
			[Relationship("importlog", EntityRole.Referenced, "lk_importlogbase_createdby", "createdby")]
			public const string lk_importlogbase_createdby = "lk_importlogbase_createdby";
			[Relationship("importlog", EntityRole.Referenced, "lk_importlogbase_modifiedby", "modifiedby")]
			public const string lk_importlogbase_modifiedby = "lk_importlogbase_modifiedby";
			[Relationship("importmap", EntityRole.Referenced, "lk_importmap_createdonbehalfby", "createdonbehalfby")]
			public const string lk_importmap_createdonbehalfby = "lk_importmap_createdonbehalfby";
			[Relationship("importmap", EntityRole.Referenced, "lk_importmap_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_importmap_modifiedonbehalfby = "lk_importmap_modifiedonbehalfby";
			[Relationship("importmap", EntityRole.Referenced, "lk_importmapbase_createdby", "createdby")]
			public const string lk_importmapbase_createdby = "lk_importmapbase_createdby";
			[Relationship("importmap", EntityRole.Referenced, "lk_importmapbase_modifiedby", "modifiedby")]
			public const string lk_importmapbase_modifiedby = "lk_importmapbase_modifiedby";
			[Relationship("incident", EntityRole.Referenced, "lk_incidentbase_createdby", "createdby")]
			public const string lk_incidentbase_createdby = "lk_incidentbase_createdby";
			[Relationship("incident", EntityRole.Referenced, "lk_incidentbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_incidentbase_createdonbehalfby = "lk_incidentbase_createdonbehalfby";
			[Relationship("incident", EntityRole.Referenced, "lk_incidentbase_modifiedby", "modifiedby")]
			public const string lk_incidentbase_modifiedby = "lk_incidentbase_modifiedby";
			[Relationship("incident", EntityRole.Referenced, "lk_incidentbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_incidentbase_modifiedonbehalfby = "lk_incidentbase_modifiedonbehalfby";
			[Relationship("incidentresolution", EntityRole.Referenced, "lk_incidentresolution_createdby", "createdby")]
			public const string lk_incidentresolution_createdby = "lk_incidentresolution_createdby";
			[Relationship("incidentresolution", EntityRole.Referenced, "lk_incidentresolution_createdonbehalfby", "createdonbehalfby")]
			public const string lk_incidentresolution_createdonbehalfby = "lk_incidentresolution_createdonbehalfby";
			[Relationship("incidentresolution", EntityRole.Referenced, "lk_incidentresolution_modifiedby", "modifiedby")]
			public const string lk_incidentresolution_modifiedby = "lk_incidentresolution_modifiedby";
			[Relationship("incidentresolution", EntityRole.Referenced, "lk_incidentresolution_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_incidentresolution_modifiedonbehalfby = "lk_incidentresolution_modifiedonbehalfby";
			[Relationship("integrationstatus", EntityRole.Referenced, "lk_integrationstatus_createdby", "createdby")]
			public const string lk_integrationstatus_createdby = "lk_integrationstatus_createdby";
			[Relationship("integrationstatus", EntityRole.Referenced, "lk_integrationstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_integrationstatus_createdonbehalfby = "lk_integrationstatus_createdonbehalfby";
			[Relationship("integrationstatus", EntityRole.Referenced, "lk_integrationstatus_modifiedby", "modifiedby")]
			public const string lk_integrationstatus_modifiedby = "lk_integrationstatus_modifiedby";
			[Relationship("integrationstatus", EntityRole.Referenced, "lk_integrationstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_integrationstatus_modifiedonbehalfby = "lk_integrationstatus_modifiedonbehalfby";
			[Relationship("interactionforemail", EntityRole.Referenced, "lk_new_interactionforemail_createdby", "createdby")]
			public const string lk_interactionforemail_createdby = "lk_interactionforemail_createdby";
			[Relationship("interactionforemail", EntityRole.Referenced, "lk_new_interactionforemail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_interactionforemail_createdonbehalfby = "lk_interactionforemail_createdonbehalfby";
			[Relationship("interactionforemail", EntityRole.Referenced, "lk_new_interactionforemail_modifiedby", "modifiedby")]
			public const string lk_interactionforemail_modifiedby = "lk_interactionforemail_modifiedby";
			[Relationship("interactionforemail", EntityRole.Referenced, "lk_new_interactionforemail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_interactionforemail_modifiedonbehalfby = "lk_interactionforemail_modifiedonbehalfby";
			[Relationship("internaladdress", EntityRole.Referenced, "lk_internaladdress_createdonbehalfby", "createdonbehalfby")]
			public const string lk_internaladdress_createdonbehalfby = "lk_internaladdress_createdonbehalfby";
			[Relationship("internaladdress", EntityRole.Referenced, "lk_internaladdress_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_internaladdress_modifiedonbehalfby = "lk_internaladdress_modifiedonbehalfby";
			[Relationship("internaladdress", EntityRole.Referenced, "lk_internaladdressbase_createdby", "createdby")]
			public const string lk_internaladdressbase_createdby = "lk_internaladdressbase_createdby";
			[Relationship("internaladdress", EntityRole.Referenced, "lk_internaladdressbase_modifiedby", "modifiedby")]
			public const string lk_internaladdressbase_modifiedby = "lk_internaladdressbase_modifiedby";
			[Relationship("invoice", EntityRole.Referenced, "lk_invoice_createdonbehalfby", "createdonbehalfby")]
			public const string lk_invoice_createdonbehalfby = "lk_invoice_createdonbehalfby";
			[Relationship("invoice", EntityRole.Referenced, "lk_invoice_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_invoice_modifiedonbehalfby = "lk_invoice_modifiedonbehalfby";
			[Relationship("invoice", EntityRole.Referenced, "lk_invoicebase_createdby", "createdby")]
			public const string lk_invoicebase_createdby = "lk_invoicebase_createdby";
			[Relationship("invoice", EntityRole.Referenced, "lk_invoicebase_modifiedby", "modifiedby")]
			public const string lk_invoicebase_modifiedby = "lk_invoicebase_modifiedby";
			[Relationship("invoicedetail", EntityRole.Referenced, "lk_invoicedetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_invoicedetail_createdonbehalfby = "lk_invoicedetail_createdonbehalfby";
			[Relationship("invoicedetail", EntityRole.Referenced, "lk_invoicedetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_invoicedetail_modifiedonbehalfby = "lk_invoicedetail_modifiedonbehalfby";
			[Relationship("invoicedetail", EntityRole.Referenced, "lk_invoicedetailbase_createdby", "createdby")]
			public const string lk_invoicedetailbase_createdby = "lk_invoicedetailbase_createdby";
			[Relationship("invoicedetail", EntityRole.Referenced, "lk_invoicedetailbase_modifiedby", "modifiedby")]
			public const string lk_invoicedetailbase_modifiedby = "lk_invoicedetailbase_modifiedby";
			[Relationship("isvconfig", EntityRole.Referenced, "lk_isvconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_isvconfig_createdonbehalfby = "lk_isvconfig_createdonbehalfby";
			[Relationship("isvconfig", EntityRole.Referenced, "lk_isvconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_isvconfig_modifiedonbehalfby = "lk_isvconfig_modifiedonbehalfby";
			[Relationship("isvconfig", EntityRole.Referenced, "lk_isvconfigbase_createdby", "createdby")]
			public const string lk_isvconfigbase_createdby = "lk_isvconfigbase_createdby";
			[Relationship("isvconfig", EntityRole.Referenced, "lk_isvconfigbase_modifiedby", "modifiedby")]
			public const string lk_isvconfigbase_modifiedby = "lk_isvconfigbase_modifiedby";
			[Relationship("kbarticle", EntityRole.Referenced, "lk_kbarticle_createdonbehalfby", "createdonbehalfby")]
			public const string lk_kbarticle_createdonbehalfby = "lk_kbarticle_createdonbehalfby";
			[Relationship("kbarticle", EntityRole.Referenced, "lk_kbarticle_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_kbarticle_modifiedonbehalfby = "lk_kbarticle_modifiedonbehalfby";
			[Relationship("kbarticle", EntityRole.Referenced, "lk_kbarticlebase_createdby", "createdby")]
			public const string lk_kbarticlebase_createdby = "lk_kbarticlebase_createdby";
			[Relationship("kbarticle", EntityRole.Referenced, "lk_kbarticlebase_modifiedby", "modifiedby")]
			public const string lk_kbarticlebase_modifiedby = "lk_kbarticlebase_modifiedby";
			[Relationship("kbarticlecomment", EntityRole.Referenced, "lk_kbarticlecomment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_kbarticlecomment_createdonbehalfby = "lk_kbarticlecomment_createdonbehalfby";
			[Relationship("kbarticlecomment", EntityRole.Referenced, "lk_kbarticlecomment_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_kbarticlecomment_modifiedonbehalfby = "lk_kbarticlecomment_modifiedonbehalfby";
			[Relationship("kbarticlecomment", EntityRole.Referenced, "lk_kbarticlecommentbase_createdby", "createdby")]
			public const string lk_kbarticlecommentbase_createdby = "lk_kbarticlecommentbase_createdby";
			[Relationship("kbarticlecomment", EntityRole.Referenced, "lk_kbarticlecommentbase_modifiedby", "modifiedby")]
			public const string lk_kbarticlecommentbase_modifiedby = "lk_kbarticlecommentbase_modifiedby";
			[Relationship("kbarticletemplate", EntityRole.Referenced, "lk_kbarticletemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_kbarticletemplate_createdonbehalfby = "lk_kbarticletemplate_createdonbehalfby";
			[Relationship("kbarticletemplate", EntityRole.Referenced, "lk_kbarticletemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_kbarticletemplate_modifiedonbehalfby = "lk_kbarticletemplate_modifiedonbehalfby";
			[Relationship("kbarticletemplate", EntityRole.Referenced, "lk_kbarticletemplatebase_createdby", "createdby")]
			public const string lk_kbarticletemplatebase_createdby = "lk_kbarticletemplatebase_createdby";
			[Relationship("kbarticletemplate", EntityRole.Referenced, "lk_kbarticletemplatebase_modifiedby", "modifiedby")]
			public const string lk_kbarticletemplatebase_modifiedby = "lk_kbarticletemplatebase_modifiedby";
			[Relationship("knowledgearticle", EntityRole.Referenced, "lk_knowledgearticle_createdby", "createdby")]
			public const string lk_knowledgearticle_createdby = "lk_knowledgearticle_createdby";
			[Relationship("knowledgearticle", EntityRole.Referenced, "lk_knowledgearticle_createdonbehalfby", "createdonbehalfby")]
			public const string lk_knowledgearticle_createdonbehalfby = "lk_knowledgearticle_createdonbehalfby";
			[Relationship("knowledgearticle", EntityRole.Referenced, "lk_knowledgearticle_modifiedby", "modifiedby")]
			public const string lk_knowledgearticle_modifiedby = "lk_knowledgearticle_modifiedby";
			[Relationship("knowledgearticle", EntityRole.Referenced, "lk_knowledgearticle_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_knowledgearticle_modifiedonbehalfby = "lk_knowledgearticle_modifiedonbehalfby";
			[Relationship("knowledgearticleincident", EntityRole.Referenced, "lk_knowledgearticleincident_createdby", "createdby")]
			public const string lk_knowledgearticleincident_createdby = "lk_knowledgearticleincident_createdby";
			[Relationship("knowledgearticleincident", EntityRole.Referenced, "lk_knowledgearticleincident_createdonbehalfby", "createdonbehalfby")]
			public const string lk_knowledgearticleincident_createdonbehalfby = "lk_knowledgearticleincident_createdonbehalfby";
			[Relationship("knowledgearticleincident", EntityRole.Referenced, "lk_knowledgearticleincident_modifiedby", "modifiedby")]
			public const string lk_knowledgearticleincident_modifiedby = "lk_knowledgearticleincident_modifiedby";
			[Relationship("knowledgearticleincident", EntityRole.Referenced, "lk_knowledgearticleincident_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_knowledgearticleincident_modifiedonbehalfby = "lk_knowledgearticleincident_modifiedonbehalfby";
			[Relationship("knowledgearticleviews", EntityRole.Referenced, "lk_knowledgearticleviews_createdby", "createdby")]
			public const string lk_knowledgearticleviews_createdby = "lk_knowledgearticleviews_createdby";
			[Relationship("knowledgearticleviews", EntityRole.Referenced, "lk_knowledgearticleviews_createdonbehalfby", "createdonbehalfby")]
			public const string lk_knowledgearticleviews_createdonbehalfby = "lk_knowledgearticleviews_createdonbehalfby";
			[Relationship("knowledgearticleviews", EntityRole.Referenced, "lk_knowledgearticleviews_modifiedby", "modifiedby")]
			public const string lk_knowledgearticleviews_modifiedby = "lk_knowledgearticleviews_modifiedby";
			[Relationship("knowledgearticleviews", EntityRole.Referenced, "lk_knowledgearticleviews_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_knowledgearticleviews_modifiedonbehalfby = "lk_knowledgearticleviews_modifiedonbehalfby";
			[Relationship("knowledgebaserecord", EntityRole.Referenced, "lk_KnowledgeBaseRecord_createdby", "createdby")]
			public const string lk_KnowledgeBaseRecord_createdby = "lk_KnowledgeBaseRecord_createdby";
			[Relationship("knowledgebaserecord", EntityRole.Referenced, "lk_KnowledgeBaseRecord_createdonbehalfby", "createdonbehalfby")]
			public const string lk_KnowledgeBaseRecord_createdonbehalfby = "lk_KnowledgeBaseRecord_createdonbehalfby";
			[Relationship("knowledgebaserecord", EntityRole.Referenced, "lk_KnowledgeBaseRecord_modifiedby", "modifiedby")]
			public const string lk_KnowledgeBaseRecord_modifiedby = "lk_KnowledgeBaseRecord_modifiedby";
			[Relationship("knowledgebaserecord", EntityRole.Referenced, "lk_KnowledgeBaseRecord_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_KnowledgeBaseRecord_modifiedonbehalfby = "lk_KnowledgeBaseRecord_modifiedonbehalfby";
			[Relationship("knowledgesearchmodel", EntityRole.Referenced, "lk_knowledgesearchmodel_createdby", "createdby")]
			public const string lk_knowledgesearchmodel_createdby = "lk_knowledgesearchmodel_createdby";
			[Relationship("knowledgesearchmodel", EntityRole.Referenced, "lk_knowledgesearchmodel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_knowledgesearchmodel_createdonbehalfby = "lk_knowledgesearchmodel_createdonbehalfby";
			[Relationship("knowledgesearchmodel", EntityRole.Referenced, "lk_knowledgesearchmodel_modifiedby", "modifiedby")]
			public const string lk_knowledgesearchmodel_modifiedby = "lk_knowledgesearchmodel_modifiedby";
			[Relationship("knowledgesearchmodel", EntityRole.Referenced, "lk_knowledgesearchmodel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_knowledgesearchmodel_modifiedonbehalfby = "lk_knowledgesearchmodel_modifiedonbehalfby";
			[Relationship("lead", EntityRole.Referenced, "lk_lead_createdonbehalfby", "createdonbehalfby")]
			public const string lk_lead_createdonbehalfby = "lk_lead_createdonbehalfby";
			[Relationship("lead", EntityRole.Referenced, "lk_lead_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_lead_modifiedonbehalfby = "lk_lead_modifiedonbehalfby";
			[Relationship("leadaddress", EntityRole.Referenced, "lk_leadaddress_createdonbehalfby", "createdonbehalfby")]
			public const string lk_leadaddress_createdonbehalfby = "lk_leadaddress_createdonbehalfby";
			[Relationship("leadaddress", EntityRole.Referenced, "lk_leadaddress_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_leadaddress_modifiedonbehalfby = "lk_leadaddress_modifiedonbehalfby";
			[Relationship("leadaddress", EntityRole.Referenced, "lk_leadaddressbase_createdby", "createdby")]
			public const string lk_leadaddressbase_createdby = "lk_leadaddressbase_createdby";
			[Relationship("leadaddress", EntityRole.Referenced, "lk_leadaddressbase_modifiedby", "modifiedby")]
			public const string lk_leadaddressbase_modifiedby = "lk_leadaddressbase_modifiedby";
			[Relationship("lead", EntityRole.Referenced, "lk_leadbase_createdby", "createdby")]
			public const string lk_leadbase_createdby = "lk_leadbase_createdby";
			[Relationship("lead", EntityRole.Referenced, "lk_leadbase_modifiedby", "modifiedby")]
			public const string lk_leadbase_modifiedby = "lk_leadbase_modifiedby";
			[Relationship("leadtoopportunitysalesprocess", EntityRole.Referenced, "lk_leadtoopportunitysalesprocess_createdby", "createdby")]
			public const string lk_leadtoopportunitysalesprocess_createdby = "lk_leadtoopportunitysalesprocess_createdby";
			[Relationship("leadtoopportunitysalesprocess", EntityRole.Referenced, "lk_leadtoopportunitysalesprocess_createdonbehalfby", "createdonbehalfby")]
			public const string lk_leadtoopportunitysalesprocess_createdonbehalfby = "lk_leadtoopportunitysalesprocess_createdonbehalfby";
			[Relationship("leadtoopportunitysalesprocess", EntityRole.Referenced, "lk_leadtoopportunitysalesprocess_modifiedby", "modifiedby")]
			public const string lk_leadtoopportunitysalesprocess_modifiedby = "lk_leadtoopportunitysalesprocess_modifiedby";
			[Relationship("leadtoopportunitysalesprocess", EntityRole.Referenced, "lk_leadtoopportunitysalesprocess_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_leadtoopportunitysalesprocess_modifiedonbehalfby = "lk_leadtoopportunitysalesprocess_modifiedonbehalfby";
			[Relationship("letter", EntityRole.Referenced, "lk_letter_createdby", "createdby")]
			public const string lk_letter_createdby = "lk_letter_createdby";
			[Relationship("letter", EntityRole.Referenced, "lk_letter_createdonbehalfby", "createdonbehalfby")]
			public const string lk_letter_createdonbehalfby = "lk_letter_createdonbehalfby";
			[Relationship("letter", EntityRole.Referenced, "lk_letter_modifiedby", "modifiedby")]
			public const string lk_letter_modifiedby = "lk_letter_modifiedby";
			[Relationship("letter", EntityRole.Referenced, "lk_letter_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_letter_modifiedonbehalfby = "lk_letter_modifiedonbehalfby";
			[Relationship("list", EntityRole.Referenced, "lk_list_createdby", "createdby")]
			public const string lk_list_createdby = "lk_list_createdby";
			[Relationship("list", EntityRole.Referenced, "lk_list_createdonbehalfby", "createdonbehalfby")]
			public const string lk_list_createdonbehalfby = "lk_list_createdonbehalfby";
			[Relationship("list", EntityRole.Referenced, "lk_list_modifiedby", "modifiedby")]
			public const string lk_list_modifiedby = "lk_list_modifiedby";
			[Relationship("list", EntityRole.Referenced, "lk_list_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_list_modifiedonbehalfby = "lk_list_modifiedonbehalfby";
			[Relationship("listmember", EntityRole.Referenced, "lk_listmember_createdby", "createdby")]
			public const string lk_listmember_createdby = "lk_listmember_createdby";
			[Relationship("listmember", EntityRole.Referenced, "lk_listmember_createdonbehalfby", "createdonbehalfby")]
			public const string lk_listmember_createdonbehalfby = "lk_listmember_createdonbehalfby";
			[Relationship("listmember", EntityRole.Referenced, "lk_listmember_modifiedby", "modifiedby")]
			public const string lk_listmember_modifiedby = "lk_listmember_modifiedby";
			[Relationship("listmember", EntityRole.Referenced, "lk_listmember_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_listmember_modifiedonbehalfby = "lk_listmember_modifiedonbehalfby";
			[Relationship("lookupmapping", EntityRole.Referenced, "lk_lookupmapping_createdby", "createdby")]
			public const string lk_lookupmapping_createdby = "lk_lookupmapping_createdby";
			[Relationship("lookupmapping", EntityRole.Referenced, "lk_lookupmapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_lookupmapping_createdonbehalfby = "lk_lookupmapping_createdonbehalfby";
			[Relationship("lookupmapping", EntityRole.Referenced, "lk_lookupmapping_modifiedby", "modifiedby")]
			public const string lk_lookupmapping_modifiedby = "lk_lookupmapping_modifiedby";
			[Relationship("lookupmapping", EntityRole.Referenced, "lk_lookupmapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_lookupmapping_modifiedonbehalfby = "lk_lookupmapping_modifiedonbehalfby";
			[Relationship("mailbox", EntityRole.Referenced, "lk_mailbox_createdby", "createdby")]
			public const string lk_mailbox_createdby = "lk_mailbox_createdby";
			[Relationship("mailbox", EntityRole.Referenced, "lk_mailbox_createdonbehalfby", "createdonbehalfby")]
			public const string lk_mailbox_createdonbehalfby = "lk_mailbox_createdonbehalfby";
			[Relationship("mailbox", EntityRole.Referenced, "lk_mailbox_modifiedby", "modifiedby")]
			public const string lk_mailbox_modifiedby = "lk_mailbox_modifiedby";
			[Relationship("mailbox", EntityRole.Referenced, "lk_mailbox_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_mailbox_modifiedonbehalfby = "lk_mailbox_modifiedonbehalfby";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "lk_mailboxtrackingfolder_createdby", "createdby")]
			public const string lk_mailboxtrackingfolder_createdby = "lk_mailboxtrackingfolder_createdby";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "lk_mailboxtrackingfolder_createdonbehalfby", "createdonbehalfby")]
			public const string lk_mailboxtrackingfolder_createdonbehalfby = "lk_mailboxtrackingfolder_createdonbehalfby";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "lk_mailboxtrackingfolder_modifiedby", "modifiedby")]
			public const string lk_mailboxtrackingfolder_modifiedby = "lk_mailboxtrackingfolder_modifiedby";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "lk_mailboxtrackingfolder_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_mailboxtrackingfolder_modifiedonbehalfby = "lk_mailboxtrackingfolder_modifiedonbehalfby";
			[Relationship("mailmergetemplate", EntityRole.Referenced, "lk_mailmergetemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_mailmergetemplate_createdonbehalfby = "lk_mailmergetemplate_createdonbehalfby";
			[Relationship("mailmergetemplate", EntityRole.Referenced, "lk_mailmergetemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_mailmergetemplate_modifiedonbehalfby = "lk_mailmergetemplate_modifiedonbehalfby";
			[Relationship("mailmergetemplate", EntityRole.Referenced, "lk_mailmergetemplatebase_createdby", "createdby")]
			public const string lk_mailmergetemplatebase_createdby = "lk_mailmergetemplatebase_createdby";
			[Relationship("mailmergetemplate", EntityRole.Referenced, "lk_mailmergetemplatebase_modifiedby", "modifiedby")]
			public const string lk_mailmergetemplatebase_modifiedby = "lk_mailmergetemplatebase_modifiedby";
			[Relationship("mbs_pluginprofile", EntityRole.Referenced, "lk_mbs_pluginprofile_createdby", "")]
			public const string lk_mbs_pluginprofile_createdby = "lk_mbs_pluginprofile_createdby";
			[Relationship("mbs_pluginprofile", EntityRole.Referenced, "lk_mbs_pluginprofile_createdonbehalfby", "")]
			public const string lk_mbs_pluginprofile_createdonbehalfby = "lk_mbs_pluginprofile_createdonbehalfby";
			[Relationship("mbs_pluginprofile", EntityRole.Referenced, "lk_mbs_pluginprofile_modifiedby", "")]
			public const string lk_mbs_pluginprofile_modifiedby = "lk_mbs_pluginprofile_modifiedby";
			[Relationship("mbs_pluginprofile", EntityRole.Referenced, "lk_mbs_pluginprofile_modifiedonbehalfby", "")]
			public const string lk_mbs_pluginprofile_modifiedonbehalfby = "lk_mbs_pluginprofile_modifiedonbehalfby";
			[Relationship("metric", EntityRole.Referenced, "lk_metric_createdby", "createdby")]
			public const string lk_metric_createdby = "lk_metric_createdby";
			[Relationship("metric", EntityRole.Referenced, "lk_metric_createdonbehalfby", "createdonbehalfby")]
			public const string lk_metric_createdonbehalfby = "lk_metric_createdonbehalfby";
			[Relationship("metric", EntityRole.Referenced, "lk_metric_modifiedby", "modifiedby")]
			public const string lk_metric_modifiedby = "lk_metric_modifiedby";
			[Relationship("metric", EntityRole.Referenced, "lk_metric_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_metric_modifiedonbehalfby = "lk_metric_modifiedonbehalfby";
			[Relationship("mobileofflineprofile", EntityRole.Referenced, "lk_MobileOfflineProfile_createdby", "createdby")]
			public const string lk_MobileOfflineProfile_createdby = "lk_MobileOfflineProfile_createdby";
			[Relationship("mobileofflineprofile", EntityRole.Referenced, "lk_MobileOfflineProfile_createdonbehalfby", "createdonbehalfby")]
			public const string lk_MobileOfflineProfile_createdonbehalfby = "lk_MobileOfflineProfile_createdonbehalfby";
			[Relationship("mobileofflineprofile", EntityRole.Referenced, "lk_MobileOfflineProfile_modifiedby", "modifiedby")]
			public const string lk_MobileOfflineProfile_modifiedby = "lk_MobileOfflineProfile_modifiedby";
			[Relationship("mobileofflineprofile", EntityRole.Referenced, "lk_MobileOfflineProfile_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_MobileOfflineProfile_modifiedonbehalfby = "lk_MobileOfflineProfile_modifiedonbehalfby";
			[Relationship("mobileofflineprofileitem", EntityRole.Referenced, "lk_MobileOfflineProfileItem_createdby", "createdby")]
			public const string lk_MobileOfflineProfileItem_createdby = "lk_MobileOfflineProfileItem_createdby";
			[Relationship("mobileofflineprofileitem", EntityRole.Referenced, "lk_mobileofflineprofileitem_createdonbehalfby", "createdonbehalfby")]
			public const string lk_mobileofflineprofileitem_createdonbehalfby = "lk_mobileofflineprofileitem_createdonbehalfby";
			[Relationship("mobileofflineprofileitem", EntityRole.Referenced, "lk_mobileofflineprofileitem_modifiedby", "modifiedby")]
			public const string lk_mobileofflineprofileitem_modifiedby = "lk_mobileofflineprofileitem_modifiedby";
			[Relationship("mobileofflineprofileitem", EntityRole.Referenced, "lk_mobileofflineprofileitem_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_mobileofflineprofileitem_modifiedonbehalfby = "lk_mobileofflineprofileitem_modifiedonbehalfby";
			[Relationship("mobileofflineprofileitemassociation", EntityRole.Referenced, "lk_MobileOfflineProfileItemAssociation_createdby", "createdby")]
			public const string lk_MobileOfflineProfileItemAssociation_createdby = "lk_MobileOfflineProfileItemAssociation_createdby";
			[Relationship("mobileofflineprofileitemassociation", EntityRole.Referenced, "lk_mobileofflineprofileitemassociation_createdonbehalfby", "createdonbehalfby")]
			public const string lk_mobileofflineprofileitemassociation_createdonbehalfby = "lk_mobileofflineprofileitemassociation_createdonbehalfby";
			[Relationship("mobileofflineprofileitemassociation", EntityRole.Referenced, "lk_mobileofflineprofileitemassocaition_modifiedby", "modifiedby")]
			public const string lk_mobileofflineprofileitemassociation_modifiedby = "lk_mobileofflineprofileitemassociation_modifiedby";
			[Relationship("mobileofflineprofileitemassociation", EntityRole.Referenced, "lk_mobileofflineprofileitemassocaition_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_mobileofflineprofileitemassociation_modifiedonbehalfby = "lk_mobileofflineprofileitemassociation_modifiedonbehalfby";
			[Relationship("monthlyfiscalcalendar", EntityRole.Referenced, "lk_monthlyfiscalcalendar_createdby", "createdby")]
			public const string lk_monthlyfiscalcalendar_createdby = "lk_monthlyfiscalcalendar_createdby";
			[Relationship("monthlyfiscalcalendar", EntityRole.Referenced, "lk_monthlyfiscalcalendar_createdonbehalfby", "createdonbehalfby")]
			public const string lk_monthlyfiscalcalendar_createdonbehalfby = "lk_monthlyfiscalcalendar_createdonbehalfby";
			[Relationship("monthlyfiscalcalendar", EntityRole.Referenced, "lk_monthlyfiscalcalendar_modifiedby", "modifiedby")]
			public const string lk_monthlyfiscalcalendar_modifiedby = "lk_monthlyfiscalcalendar_modifiedby";
			[Relationship("monthlyfiscalcalendar", EntityRole.Referenced, "lk_monthlyfiscalcalendar_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_monthlyfiscalcalendar_modifiedonbehalfby = "lk_monthlyfiscalcalendar_modifiedonbehalfby";
			[Relationship("monthlyfiscalcalendar", EntityRole.Referenced, "lk_monthlyfiscalcalendar_salespersonid", "salespersonid")]
			public const string lk_monthlyfiscalcalendar_salespersonid = "lk_monthlyfiscalcalendar_salespersonid";
			[Relationship("msdyn_accountpricelist", EntityRole.Referenced, "lk_msdyn_accountpricelist_createdby", "createdby")]
			public const string lk_msdyn_accountpricelist_createdby = "lk_msdyn_accountpricelist_createdby";
			[Relationship("msdyn_accountpricelist", EntityRole.Referenced, "lk_msdyn_accountpricelist_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_accountpricelist_createdonbehalfby = "lk_msdyn_accountpricelist_createdonbehalfby";
			[Relationship("msdyn_accountpricelist", EntityRole.Referenced, "lk_msdyn_accountpricelist_modifiedby", "modifiedby")]
			public const string lk_msdyn_accountpricelist_modifiedby = "lk_msdyn_accountpricelist_modifiedby";
			[Relationship("msdyn_accountpricelist", EntityRole.Referenced, "lk_msdyn_accountpricelist_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_accountpricelist_modifiedonbehalfby = "lk_msdyn_accountpricelist_modifiedonbehalfby";
			[Relationship("msdyn_actual", EntityRole.Referenced, "lk_msdyn_actual_createdby", "createdby")]
			public const string lk_msdyn_actual_createdby = "lk_msdyn_actual_createdby";
			[Relationship("msdyn_actual", EntityRole.Referenced, "lk_msdyn_actual_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_actual_createdonbehalfby = "lk_msdyn_actual_createdonbehalfby";
			[Relationship("msdyn_actual", EntityRole.Referenced, "lk_msdyn_actual_modifiedby", "modifiedby")]
			public const string lk_msdyn_actual_modifiedby = "lk_msdyn_actual_modifiedby";
			[Relationship("msdyn_actual", EntityRole.Referenced, "lk_msdyn_actual_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_actual_modifiedonbehalfby = "lk_msdyn_actual_modifiedonbehalfby";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "lk_msdyn_agreement_createdby", "createdby")]
			public const string lk_msdyn_agreement_createdby = "lk_msdyn_agreement_createdby";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "lk_msdyn_agreement_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreement_createdonbehalfby = "lk_msdyn_agreement_createdonbehalfby";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "lk_msdyn_agreement_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreement_modifiedby = "lk_msdyn_agreement_modifiedby";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "lk_msdyn_agreement_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreement_modifiedonbehalfby = "lk_msdyn_agreement_modifiedonbehalfby";
			[Relationship("msdyn_agreementbookingdate", EntityRole.Referenced, "lk_msdyn_agreementbookingdate_createdby", "createdby")]
			public const string lk_msdyn_agreementbookingdate_createdby = "lk_msdyn_agreementbookingdate_createdby";
			[Relationship("msdyn_agreementbookingdate", EntityRole.Referenced, "lk_msdyn_agreementbookingdate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementbookingdate_createdonbehalfby = "lk_msdyn_agreementbookingdate_createdonbehalfby";
			[Relationship("msdyn_agreementbookingdate", EntityRole.Referenced, "lk_msdyn_agreementbookingdate_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementbookingdate_modifiedby = "lk_msdyn_agreementbookingdate_modifiedby";
			[Relationship("msdyn_agreementbookingdate", EntityRole.Referenced, "lk_msdyn_agreementbookingdate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementbookingdate_modifiedonbehalfby = "lk_msdyn_agreementbookingdate_modifiedonbehalfby";
			[Relationship("msdyn_agreementbookingincident", EntityRole.Referenced, "lk_msdyn_agreementbookingincident_createdby", "createdby")]
			public const string lk_msdyn_agreementbookingincident_createdby = "lk_msdyn_agreementbookingincident_createdby";
			[Relationship("msdyn_agreementbookingincident", EntityRole.Referenced, "lk_msdyn_agreementbookingincident_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementbookingincident_createdonbehalfby = "lk_msdyn_agreementbookingincident_createdonbehalfby";
			[Relationship("msdyn_agreementbookingincident", EntityRole.Referenced, "lk_msdyn_agreementbookingincident_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementbookingincident_modifiedby = "lk_msdyn_agreementbookingincident_modifiedby";
			[Relationship("msdyn_agreementbookingincident", EntityRole.Referenced, "lk_msdyn_agreementbookingincident_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementbookingincident_modifiedonbehalfby = "lk_msdyn_agreementbookingincident_modifiedonbehalfby";
			[Relationship("msdyn_agreementbookingproduct", EntityRole.Referenced, "lk_msdyn_agreementbookingproduct_createdby", "createdby")]
			public const string lk_msdyn_agreementbookingproduct_createdby = "lk_msdyn_agreementbookingproduct_createdby";
			[Relationship("msdyn_agreementbookingproduct", EntityRole.Referenced, "lk_msdyn_agreementbookingproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementbookingproduct_createdonbehalfby = "lk_msdyn_agreementbookingproduct_createdonbehalfby";
			[Relationship("msdyn_agreementbookingproduct", EntityRole.Referenced, "lk_msdyn_agreementbookingproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementbookingproduct_modifiedby = "lk_msdyn_agreementbookingproduct_modifiedby";
			[Relationship("msdyn_agreementbookingproduct", EntityRole.Referenced, "lk_msdyn_agreementbookingproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementbookingproduct_modifiedonbehalfby = "lk_msdyn_agreementbookingproduct_modifiedonbehalfby";
			[Relationship("msdyn_agreementbookingservice", EntityRole.Referenced, "lk_msdyn_agreementbookingservice_createdby", "createdby")]
			public const string lk_msdyn_agreementbookingservice_createdby = "lk_msdyn_agreementbookingservice_createdby";
			[Relationship("msdyn_agreementbookingservice", EntityRole.Referenced, "lk_msdyn_agreementbookingservice_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementbookingservice_createdonbehalfby = "lk_msdyn_agreementbookingservice_createdonbehalfby";
			[Relationship("msdyn_agreementbookingservice", EntityRole.Referenced, "lk_msdyn_agreementbookingservice_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementbookingservice_modifiedby = "lk_msdyn_agreementbookingservice_modifiedby";
			[Relationship("msdyn_agreementbookingservice", EntityRole.Referenced, "lk_msdyn_agreementbookingservice_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementbookingservice_modifiedonbehalfby = "lk_msdyn_agreementbookingservice_modifiedonbehalfby";
			[Relationship("msdyn_agreementbookingservicetask", EntityRole.Referenced, "lk_msdyn_agreementbookingservicetask_createdby", "createdby")]
			public const string lk_msdyn_agreementbookingservicetask_createdby = "lk_msdyn_agreementbookingservicetask_createdby";
			[Relationship("msdyn_agreementbookingservicetask", EntityRole.Referenced, "lk_msdyn_agreementbookingservicetask_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementbookingservicetask_createdonbehalfby = "lk_msdyn_agreementbookingservicetask_createdonbehalfby";
			[Relationship("msdyn_agreementbookingservicetask", EntityRole.Referenced, "lk_msdyn_agreementbookingservicetask_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementbookingservicetask_modifiedby = "lk_msdyn_agreementbookingservicetask_modifiedby";
			[Relationship("msdyn_agreementbookingservicetask", EntityRole.Referenced, "lk_msdyn_agreementbookingservicetask_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementbookingservicetask_modifiedonbehalfby = "lk_msdyn_agreementbookingservicetask_modifiedonbehalfby";
			[Relationship("msdyn_agreementbookingsetup", EntityRole.Referenced, "lk_msdyn_agreementbookingsetup_createdby", "createdby")]
			public const string lk_msdyn_agreementbookingsetup_createdby = "lk_msdyn_agreementbookingsetup_createdby";
			[Relationship("msdyn_agreementbookingsetup", EntityRole.Referenced, "lk_msdyn_agreementbookingsetup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementbookingsetup_createdonbehalfby = "lk_msdyn_agreementbookingsetup_createdonbehalfby";
			[Relationship("msdyn_agreementbookingsetup", EntityRole.Referenced, "lk_msdyn_agreementbookingsetup_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementbookingsetup_modifiedby = "lk_msdyn_agreementbookingsetup_modifiedby";
			[Relationship("msdyn_agreementbookingsetup", EntityRole.Referenced, "lk_msdyn_agreementbookingsetup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementbookingsetup_modifiedonbehalfby = "lk_msdyn_agreementbookingsetup_modifiedonbehalfby";
			[Relationship("msdyn_agreementinvoicedate", EntityRole.Referenced, "lk_msdyn_agreementinvoicedate_createdby", "createdby")]
			public const string lk_msdyn_agreementinvoicedate_createdby = "lk_msdyn_agreementinvoicedate_createdby";
			[Relationship("msdyn_agreementinvoicedate", EntityRole.Referenced, "lk_msdyn_agreementinvoicedate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementinvoicedate_createdonbehalfby = "lk_msdyn_agreementinvoicedate_createdonbehalfby";
			[Relationship("msdyn_agreementinvoicedate", EntityRole.Referenced, "lk_msdyn_agreementinvoicedate_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementinvoicedate_modifiedby = "lk_msdyn_agreementinvoicedate_modifiedby";
			[Relationship("msdyn_agreementinvoicedate", EntityRole.Referenced, "lk_msdyn_agreementinvoicedate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementinvoicedate_modifiedonbehalfby = "lk_msdyn_agreementinvoicedate_modifiedonbehalfby";
			[Relationship("msdyn_agreementinvoiceproduct", EntityRole.Referenced, "lk_msdyn_agreementinvoiceproduct_createdby", "createdby")]
			public const string lk_msdyn_agreementinvoiceproduct_createdby = "lk_msdyn_agreementinvoiceproduct_createdby";
			[Relationship("msdyn_agreementinvoiceproduct", EntityRole.Referenced, "lk_msdyn_agreementinvoiceproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementinvoiceproduct_createdonbehalfby = "lk_msdyn_agreementinvoiceproduct_createdonbehalfby";
			[Relationship("msdyn_agreementinvoiceproduct", EntityRole.Referenced, "lk_msdyn_agreementinvoiceproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementinvoiceproduct_modifiedby = "lk_msdyn_agreementinvoiceproduct_modifiedby";
			[Relationship("msdyn_agreementinvoiceproduct", EntityRole.Referenced, "lk_msdyn_agreementinvoiceproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementinvoiceproduct_modifiedonbehalfby = "lk_msdyn_agreementinvoiceproduct_modifiedonbehalfby";
			[Relationship("msdyn_agreementinvoicesetup", EntityRole.Referenced, "lk_msdyn_agreementinvoicesetup_createdby", "createdby")]
			public const string lk_msdyn_agreementinvoicesetup_createdby = "lk_msdyn_agreementinvoicesetup_createdby";
			[Relationship("msdyn_agreementinvoicesetup", EntityRole.Referenced, "lk_msdyn_agreementinvoicesetup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementinvoicesetup_createdonbehalfby = "lk_msdyn_agreementinvoicesetup_createdonbehalfby";
			[Relationship("msdyn_agreementinvoicesetup", EntityRole.Referenced, "lk_msdyn_agreementinvoicesetup_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementinvoicesetup_modifiedby = "lk_msdyn_agreementinvoicesetup_modifiedby";
			[Relationship("msdyn_agreementinvoicesetup", EntityRole.Referenced, "lk_msdyn_agreementinvoicesetup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementinvoicesetup_modifiedonbehalfby = "lk_msdyn_agreementinvoicesetup_modifiedonbehalfby";
			[Relationship("msdyn_agreementsubstatus", EntityRole.Referenced, "lk_msdyn_agreementsubstatus_createdby", "createdby")]
			public const string lk_msdyn_agreementsubstatus_createdby = "lk_msdyn_agreementsubstatus_createdby";
			[Relationship("msdyn_agreementsubstatus", EntityRole.Referenced, "lk_msdyn_agreementsubstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_agreementsubstatus_createdonbehalfby = "lk_msdyn_agreementsubstatus_createdonbehalfby";
			[Relationship("msdyn_agreementsubstatus", EntityRole.Referenced, "lk_msdyn_agreementsubstatus_modifiedby", "modifiedby")]
			public const string lk_msdyn_agreementsubstatus_modifiedby = "lk_msdyn_agreementsubstatus_modifiedby";
			[Relationship("msdyn_agreementsubstatus", EntityRole.Referenced, "lk_msdyn_agreementsubstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_agreementsubstatus_modifiedonbehalfby = "lk_msdyn_agreementsubstatus_modifiedonbehalfby";
			[Relationship("msdyn_answer", EntityRole.Referenced, "lk_msdyn_answer_createdby", "")]
			public const string lk_msdyn_answer_createdby = "lk_msdyn_answer_createdby";
			[Relationship("msdyn_answer", EntityRole.Referenced, "lk_msdyn_answer_createdonbehalfby", "")]
			public const string lk_msdyn_answer_createdonbehalfby = "lk_msdyn_answer_createdonbehalfby";
			[Relationship("msdyn_answer", EntityRole.Referenced, "lk_msdyn_answer_modifiedby", "")]
			public const string lk_msdyn_answer_modifiedby = "lk_msdyn_answer_modifiedby";
			[Relationship("msdyn_answer", EntityRole.Referenced, "lk_msdyn_answer_modifiedonbehalfby", "")]
			public const string lk_msdyn_answer_modifiedonbehalfby = "lk_msdyn_answer_modifiedonbehalfby";
			[Relationship("msdyn_azuredeployment", EntityRole.Referenced, "lk_msdyn_azuredeployment_createdby", "")]
			public const string lk_msdyn_azuredeployment_createdby = "lk_msdyn_azuredeployment_createdby";
			[Relationship("msdyn_azuredeployment", EntityRole.Referenced, "lk_msdyn_azuredeployment_createdonbehalfby", "")]
			public const string lk_msdyn_azuredeployment_createdonbehalfby = "lk_msdyn_azuredeployment_createdonbehalfby";
			[Relationship("msdyn_azuredeployment", EntityRole.Referenced, "lk_msdyn_azuredeployment_modifiedby", "")]
			public const string lk_msdyn_azuredeployment_modifiedby = "lk_msdyn_azuredeployment_modifiedby";
			[Relationship("msdyn_azuredeployment", EntityRole.Referenced, "lk_msdyn_azuredeployment_modifiedonbehalfby", "")]
			public const string lk_msdyn_azuredeployment_modifiedonbehalfby = "lk_msdyn_azuredeployment_modifiedonbehalfby";
			[Relationship("msdyn_batchjob", EntityRole.Referenced, "lk_msdyn_batchjob_createdby", "createdby")]
			public const string lk_msdyn_batchjob_createdby = "lk_msdyn_batchjob_createdby";
			[Relationship("msdyn_batchjob", EntityRole.Referenced, "lk_msdyn_batchjob_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_batchjob_createdonbehalfby = "lk_msdyn_batchjob_createdonbehalfby";
			[Relationship("msdyn_batchjob", EntityRole.Referenced, "lk_msdyn_batchjob_modifiedby", "modifiedby")]
			public const string lk_msdyn_batchjob_modifiedby = "lk_msdyn_batchjob_modifiedby";
			[Relationship("msdyn_batchjob", EntityRole.Referenced, "lk_msdyn_batchjob_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_batchjob_modifiedonbehalfby = "lk_msdyn_batchjob_modifiedonbehalfby";
			[Relationship("msdyn_bookingalertstatus", EntityRole.Referenced, "lk_msdyn_bookingalertstatus_createdby", "createdby")]
			public const string lk_msdyn_bookingalertstatus_createdby = "lk_msdyn_bookingalertstatus_createdby";
			[Relationship("msdyn_bookingalertstatus", EntityRole.Referenced, "lk_msdyn_bookingalertstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bookingalertstatus_createdonbehalfby = "lk_msdyn_bookingalertstatus_createdonbehalfby";
			[Relationship("msdyn_bookingalertstatus", EntityRole.Referenced, "lk_msdyn_bookingalertstatus_modifiedby", "modifiedby")]
			public const string lk_msdyn_bookingalertstatus_modifiedby = "lk_msdyn_bookingalertstatus_modifiedby";
			[Relationship("msdyn_bookingalertstatus", EntityRole.Referenced, "lk_msdyn_bookingalertstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bookingalertstatus_modifiedonbehalfby = "lk_msdyn_bookingalertstatus_modifiedonbehalfby";
			[Relationship("msdyn_bookingchange", EntityRole.Referenced, "lk_msdyn_bookingchange_createdby", "createdby")]
			public const string lk_msdyn_bookingchange_createdby = "lk_msdyn_bookingchange_createdby";
			[Relationship("msdyn_bookingchange", EntityRole.Referenced, "lk_msdyn_bookingchange_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bookingchange_createdonbehalfby = "lk_msdyn_bookingchange_createdonbehalfby";
			[Relationship("msdyn_bookingchange", EntityRole.Referenced, "lk_msdyn_bookingchange_modifiedby", "modifiedby")]
			public const string lk_msdyn_bookingchange_modifiedby = "lk_msdyn_bookingchange_modifiedby";
			[Relationship("msdyn_bookingchange", EntityRole.Referenced, "lk_msdyn_bookingchange_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bookingchange_modifiedonbehalfby = "lk_msdyn_bookingchange_modifiedonbehalfby";
			[Relationship("msdyn_bookingjournal", EntityRole.Referenced, "lk_msdyn_bookingjournal_createdby", "createdby")]
			public const string lk_msdyn_bookingjournal_createdby = "lk_msdyn_bookingjournal_createdby";
			[Relationship("msdyn_bookingjournal", EntityRole.Referenced, "lk_msdyn_bookingjournal_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bookingjournal_createdonbehalfby = "lk_msdyn_bookingjournal_createdonbehalfby";
			[Relationship("msdyn_bookingjournal", EntityRole.Referenced, "lk_msdyn_bookingjournal_modifiedby", "modifiedby")]
			public const string lk_msdyn_bookingjournal_modifiedby = "lk_msdyn_bookingjournal_modifiedby";
			[Relationship("msdyn_bookingjournal", EntityRole.Referenced, "lk_msdyn_bookingjournal_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bookingjournal_modifiedonbehalfby = "lk_msdyn_bookingjournal_modifiedonbehalfby";
			[Relationship("msdyn_bookingrule", EntityRole.Referenced, "lk_msdyn_bookingrule_createdby", "createdby")]
			public const string lk_msdyn_bookingrule_createdby = "lk_msdyn_bookingrule_createdby";
			[Relationship("msdyn_bookingrule", EntityRole.Referenced, "lk_msdyn_bookingrule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bookingrule_createdonbehalfby = "lk_msdyn_bookingrule_createdonbehalfby";
			[Relationship("msdyn_bookingrule", EntityRole.Referenced, "lk_msdyn_bookingrule_modifiedby", "modifiedby")]
			public const string lk_msdyn_bookingrule_modifiedby = "lk_msdyn_bookingrule_modifiedby";
			[Relationship("msdyn_bookingrule", EntityRole.Referenced, "lk_msdyn_bookingrule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bookingrule_modifiedonbehalfby = "lk_msdyn_bookingrule_modifiedonbehalfby";
			[Relationship("msdyn_bookingsetupmetadata", EntityRole.Referenced, "lk_msdyn_bookingsetupmetadata_createdby", "createdby")]
			public const string lk_msdyn_bookingsetupmetadata_createdby = "lk_msdyn_bookingsetupmetadata_createdby";
			[Relationship("msdyn_bookingsetupmetadata", EntityRole.Referenced, "lk_msdyn_bookingsetupmetadata_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bookingsetupmetadata_createdonbehalfby = "lk_msdyn_bookingsetupmetadata_createdonbehalfby";
			[Relationship("msdyn_bookingsetupmetadata", EntityRole.Referenced, "lk_msdyn_bookingsetupmetadata_modifiedby", "modifiedby")]
			public const string lk_msdyn_bookingsetupmetadata_modifiedby = "lk_msdyn_bookingsetupmetadata_modifiedby";
			[Relationship("msdyn_bookingsetupmetadata", EntityRole.Referenced, "lk_msdyn_bookingsetupmetadata_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bookingsetupmetadata_modifiedonbehalfby = "lk_msdyn_bookingsetupmetadata_modifiedonbehalfby";
			[Relationship("msdyn_bookingtimestamp", EntityRole.Referenced, "lk_msdyn_bookingtimestamp_createdby", "createdby")]
			public const string lk_msdyn_bookingtimestamp_createdby = "lk_msdyn_bookingtimestamp_createdby";
			[Relationship("msdyn_bookingtimestamp", EntityRole.Referenced, "lk_msdyn_bookingtimestamp_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bookingtimestamp_createdonbehalfby = "lk_msdyn_bookingtimestamp_createdonbehalfby";
			[Relationship("msdyn_bookingtimestamp", EntityRole.Referenced, "lk_msdyn_bookingtimestamp_modifiedby", "modifiedby")]
			public const string lk_msdyn_bookingtimestamp_modifiedby = "lk_msdyn_bookingtimestamp_modifiedby";
			[Relationship("msdyn_bookingtimestamp", EntityRole.Referenced, "lk_msdyn_bookingtimestamp_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bookingtimestamp_modifiedonbehalfby = "lk_msdyn_bookingtimestamp_modifiedonbehalfby";
			[Relationship("msdyn_bpf_2c5fe86acc8b414b8322ae571000c799", EntityRole.Referenced, "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_createdby", "createdby")]
			public const string lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_createdby = "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_createdby";
			[Relationship("msdyn_bpf_2c5fe86acc8b414b8322ae571000c799", EntityRole.Referenced, "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_createdonbehalfby = "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_createdonbehalfby";
			[Relationship("msdyn_bpf_2c5fe86acc8b414b8322ae571000c799", EntityRole.Referenced, "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_modifiedby", "modifiedby")]
			public const string lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_modifiedby = "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_modifiedby";
			[Relationship("msdyn_bpf_2c5fe86acc8b414b8322ae571000c799", EntityRole.Referenced, "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_modifiedonbehalfby = "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_modifiedonbehalfby";
			[Relationship("msdyn_bpf_665e73aa18c247d886bfc50499c73b82", EntityRole.Referenced, "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_createdby", "createdby")]
			public const string lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_createdby = "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_createdby";
			[Relationship("msdyn_bpf_665e73aa18c247d886bfc50499c73b82", EntityRole.Referenced, "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_createdonbehalfby = "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_createdonbehalfby";
			[Relationship("msdyn_bpf_665e73aa18c247d886bfc50499c73b82", EntityRole.Referenced, "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_modifiedby", "modifiedby")]
			public const string lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_modifiedby = "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_modifiedby";
			[Relationship("msdyn_bpf_665e73aa18c247d886bfc50499c73b82", EntityRole.Referenced, "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_modifiedonbehalfby = "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_modifiedonbehalfby";
			[Relationship("msdyn_bpf_989e9b1857e24af18787d5143b67523b", EntityRole.Referenced, "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_createdby", "createdby")]
			public const string lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_createdby = "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_createdby";
			[Relationship("msdyn_bpf_989e9b1857e24af18787d5143b67523b", EntityRole.Referenced, "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_createdonbehalfby = "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_createdonbehalfby";
			[Relationship("msdyn_bpf_989e9b1857e24af18787d5143b67523b", EntityRole.Referenced, "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_modifiedby", "modifiedby")]
			public const string lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_modifiedby = "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_modifiedby";
			[Relationship("msdyn_bpf_989e9b1857e24af18787d5143b67523b", EntityRole.Referenced, "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_modifiedonbehalfby = "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_modifiedonbehalfby";
			[Relationship("msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3", EntityRole.Referenced, "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_createdby", "createdby")]
			public const string lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_createdby = "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_createdby";
			[Relationship("msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3", EntityRole.Referenced, "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_createdonbehalfby = "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_createdonbehalfby";
			[Relationship("msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3", EntityRole.Referenced, "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_modifiedby", "modifiedby")]
			public const string lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_modifiedby = "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_modifiedby";
			[Relationship("msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3", EntityRole.Referenced, "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_modifiedonbehalfby = "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_modifiedonbehalfby";
			[Relationship("msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39", EntityRole.Referenced, "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_createdby", "createdby")]
			public const string lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_createdby = "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_createdby";
			[Relationship("msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39", EntityRole.Referenced, "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_createdonbehalfby = "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_createdonbehalfby";
			[Relationship("msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39", EntityRole.Referenced, "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_modifiedby", "modifiedby")]
			public const string lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_modifiedby = "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_modifiedby";
			[Relationship("msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39", EntityRole.Referenced, "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_modifiedonbehalfby = "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_modifiedonbehalfby";
			[Relationship("msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d", EntityRole.Referenced, "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_createdby", "createdby")]
			public const string lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_createdby = "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_createdby";
			[Relationship("msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d", EntityRole.Referenced, "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_createdonbehalfby = "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_createdonbehalfby";
			[Relationship("msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d", EntityRole.Referenced, "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_modifiedby", "modifiedby")]
			public const string lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_modifiedby = "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_modifiedby";
			[Relationship("msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d", EntityRole.Referenced, "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_modifiedonbehalfby = "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_modifiedonbehalfby";
			[Relationship("msdyn_callablecontext", EntityRole.Referenced, "lk_msdyn_callablecontext_createdby", "createdby")]
			public const string lk_msdyn_callablecontext_createdby = "lk_msdyn_callablecontext_createdby";
			[Relationship("msdyn_callablecontext", EntityRole.Referenced, "lk_msdyn_callablecontext_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_callablecontext_createdonbehalfby = "lk_msdyn_callablecontext_createdonbehalfby";
			[Relationship("msdyn_callablecontext", EntityRole.Referenced, "lk_msdyn_callablecontext_modifiedby", "modifiedby")]
			public const string lk_msdyn_callablecontext_modifiedby = "lk_msdyn_callablecontext_modifiedby";
			[Relationship("msdyn_callablecontext", EntityRole.Referenced, "lk_msdyn_callablecontext_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_callablecontext_modifiedonbehalfby = "lk_msdyn_callablecontext_modifiedonbehalfby";
			[Relationship("msdyn_characteristicreqforteammember", EntityRole.Referenced, "lk_msdyn_characteristicreqforteammember_createdby", "createdby")]
			public const string lk_msdyn_characteristicreqforteammember_createdby = "lk_msdyn_characteristicreqforteammember_createdby";
			[Relationship("msdyn_characteristicreqforteammember", EntityRole.Referenced, "lk_msdyn_characteristicreqforteammember_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_characteristicreqforteammember_createdonbehalfby = "lk_msdyn_characteristicreqforteammember_createdonbehalfby";
			[Relationship("msdyn_characteristicreqforteammember", EntityRole.Referenced, "lk_msdyn_characteristicreqforteammember_modifiedby", "modifiedby")]
			public const string lk_msdyn_characteristicreqforteammember_modifiedby = "lk_msdyn_characteristicreqforteammember_modifiedby";
			[Relationship("msdyn_characteristicreqforteammember", EntityRole.Referenced, "lk_msdyn_characteristicreqforteammember_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_characteristicreqforteammember_modifiedonbehalfby = "lk_msdyn_characteristicreqforteammember_modifiedonbehalfby";
			[Relationship("msdyn_contactpricelist", EntityRole.Referenced, "lk_msdyn_contactpricelist_createdby", "createdby")]
			public const string lk_msdyn_contactpricelist_createdby = "lk_msdyn_contactpricelist_createdby";
			[Relationship("msdyn_contactpricelist", EntityRole.Referenced, "lk_msdyn_contactpricelist_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_contactpricelist_createdonbehalfby = "lk_msdyn_contactpricelist_createdonbehalfby";
			[Relationship("msdyn_contactpricelist", EntityRole.Referenced, "lk_msdyn_contactpricelist_modifiedby", "modifiedby")]
			public const string lk_msdyn_contactpricelist_modifiedby = "lk_msdyn_contactpricelist_modifiedby";
			[Relationship("msdyn_contactpricelist", EntityRole.Referenced, "lk_msdyn_contactpricelist_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_contactpricelist_modifiedonbehalfby = "lk_msdyn_contactpricelist_modifiedonbehalfby";
			[Relationship("msdyn_contractlineinvoiceschedule", EntityRole.Referenced, "lk_msdyn_contractlineinvoiceschedule_createdby", "createdby")]
			public const string lk_msdyn_contractlineinvoiceschedule_createdby = "lk_msdyn_contractlineinvoiceschedule_createdby";
			[Relationship("msdyn_contractlineinvoiceschedule", EntityRole.Referenced, "lk_msdyn_contractlineinvoiceschedule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_contractlineinvoiceschedule_createdonbehalfby = "lk_msdyn_contractlineinvoiceschedule_createdonbehalfby";
			[Relationship("msdyn_contractlineinvoiceschedule", EntityRole.Referenced, "lk_msdyn_contractlineinvoiceschedule_modifiedby", "modifiedby")]
			public const string lk_msdyn_contractlineinvoiceschedule_modifiedby = "lk_msdyn_contractlineinvoiceschedule_modifiedby";
			[Relationship("msdyn_contractlineinvoiceschedule", EntityRole.Referenced, "lk_msdyn_contractlineinvoiceschedule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_contractlineinvoiceschedule_modifiedonbehalfby = "lk_msdyn_contractlineinvoiceschedule_modifiedonbehalfby";
			[Relationship("msdyn_contractlinescheduleofvalue", EntityRole.Referenced, "lk_msdyn_contractlinescheduleofvalue_createdby", "createdby")]
			public const string lk_msdyn_contractlinescheduleofvalue_createdby = "lk_msdyn_contractlinescheduleofvalue_createdby";
			[Relationship("msdyn_contractlinescheduleofvalue", EntityRole.Referenced, "lk_msdyn_contractlinescheduleofvalue_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_contractlinescheduleofvalue_createdonbehalfby = "lk_msdyn_contractlinescheduleofvalue_createdonbehalfby";
			[Relationship("msdyn_contractlinescheduleofvalue", EntityRole.Referenced, "lk_msdyn_contractlinescheduleofvalue_modifiedby", "modifiedby")]
			public const string lk_msdyn_contractlinescheduleofvalue_modifiedby = "lk_msdyn_contractlinescheduleofvalue_modifiedby";
			[Relationship("msdyn_contractlinescheduleofvalue", EntityRole.Referenced, "lk_msdyn_contractlinescheduleofvalue_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_contractlinescheduleofvalue_modifiedonbehalfby = "lk_msdyn_contractlinescheduleofvalue_modifiedonbehalfby";
			[Relationship("msdyn_customerasset", EntityRole.Referenced, "lk_msdyn_customerasset_createdby", "createdby")]
			public const string lk_msdyn_customerasset_createdby = "lk_msdyn_customerasset_createdby";
			[Relationship("msdyn_customerasset", EntityRole.Referenced, "lk_msdyn_customerasset_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_customerasset_createdonbehalfby = "lk_msdyn_customerasset_createdonbehalfby";
			[Relationship("msdyn_customerasset", EntityRole.Referenced, "lk_msdyn_customerasset_modifiedby", "modifiedby")]
			public const string lk_msdyn_customerasset_modifiedby = "lk_msdyn_customerasset_modifiedby";
			[Relationship("msdyn_customerasset", EntityRole.Referenced, "lk_msdyn_customerasset_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_customerasset_modifiedonbehalfby = "lk_msdyn_customerasset_modifiedonbehalfby";
			[Relationship("msdyn_databaseversion", EntityRole.Referenced, "lk_msdyn_databaseversion_createdby", "createdby")]
			public const string lk_msdyn_databaseversion_createdby = "lk_msdyn_databaseversion_createdby";
			[Relationship("msdyn_databaseversion", EntityRole.Referenced, "lk_msdyn_databaseversion_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_databaseversion_createdonbehalfby = "lk_msdyn_databaseversion_createdonbehalfby";
			[Relationship("msdyn_databaseversion", EntityRole.Referenced, "lk_msdyn_databaseversion_modifiedby", "modifiedby")]
			public const string lk_msdyn_databaseversion_modifiedby = "lk_msdyn_databaseversion_modifiedby";
			[Relationship("msdyn_databaseversion", EntityRole.Referenced, "lk_msdyn_databaseversion_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_databaseversion_modifiedonbehalfby = "lk_msdyn_databaseversion_modifiedonbehalfby";
			[Relationship("msdyn_dataexport", EntityRole.Referenced, "lk_msdyn_dataexport_createdby", "createdby")]
			public const string lk_msdyn_dataexport_createdby = "lk_msdyn_dataexport_createdby";
			[Relationship("msdyn_dataexport", EntityRole.Referenced, "lk_msdyn_dataexport_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_dataexport_createdonbehalfby = "lk_msdyn_dataexport_createdonbehalfby";
			[Relationship("msdyn_dataexport", EntityRole.Referenced, "lk_msdyn_dataexport_modifiedby", "modifiedby")]
			public const string lk_msdyn_dataexport_modifiedby = "lk_msdyn_dataexport_modifiedby";
			[Relationship("msdyn_dataexport", EntityRole.Referenced, "lk_msdyn_dataexport_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_dataexport_modifiedonbehalfby = "lk_msdyn_dataexport_modifiedonbehalfby";
			[Relationship("msdyn_delegation", EntityRole.Referenced, "lk_msdyn_delegation_createdby", "createdby")]
			public const string lk_msdyn_delegation_createdby = "lk_msdyn_delegation_createdby";
			[Relationship("msdyn_delegation", EntityRole.Referenced, "lk_msdyn_delegation_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_delegation_createdonbehalfby = "lk_msdyn_delegation_createdonbehalfby";
			[Relationship("msdyn_delegation", EntityRole.Referenced, "lk_msdyn_delegation_modifiedby", "modifiedby")]
			public const string lk_msdyn_delegation_modifiedby = "lk_msdyn_delegation_modifiedby";
			[Relationship("msdyn_delegation", EntityRole.Referenced, "lk_msdyn_delegation_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_delegation_modifiedonbehalfby = "lk_msdyn_delegation_modifiedonbehalfby";
			[Relationship("msdyn_estimate", EntityRole.Referenced, "lk_msdyn_estimate_createdby", "createdby")]
			public const string lk_msdyn_estimate_createdby = "lk_msdyn_estimate_createdby";
			[Relationship("msdyn_estimate", EntityRole.Referenced, "lk_msdyn_estimate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_estimate_createdonbehalfby = "lk_msdyn_estimate_createdonbehalfby";
			[Relationship("msdyn_estimate", EntityRole.Referenced, "lk_msdyn_estimate_modifiedby", "modifiedby")]
			public const string lk_msdyn_estimate_modifiedby = "lk_msdyn_estimate_modifiedby";
			[Relationship("msdyn_estimate", EntityRole.Referenced, "lk_msdyn_estimate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_estimate_modifiedonbehalfby = "lk_msdyn_estimate_modifiedonbehalfby";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "lk_msdyn_estimateline_createdby", "createdby")]
			public const string lk_msdyn_estimateline_createdby = "lk_msdyn_estimateline_createdby";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "lk_msdyn_estimateline_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_estimateline_createdonbehalfby = "lk_msdyn_estimateline_createdonbehalfby";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "lk_msdyn_estimateline_modifiedby", "modifiedby")]
			public const string lk_msdyn_estimateline_modifiedby = "lk_msdyn_estimateline_modifiedby";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "lk_msdyn_estimateline_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_estimateline_modifiedonbehalfby = "lk_msdyn_estimateline_modifiedonbehalfby";
			[Relationship("msdyn_expense", EntityRole.Referenced, "lk_msdyn_expense_createdby", "createdby")]
			public const string lk_msdyn_expense_createdby = "lk_msdyn_expense_createdby";
			[Relationship("msdyn_expense", EntityRole.Referenced, "lk_msdyn_expense_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_expense_createdonbehalfby = "lk_msdyn_expense_createdonbehalfby";
			[Relationship("msdyn_expense", EntityRole.Referenced, "lk_msdyn_expense_modifiedby", "modifiedby")]
			public const string lk_msdyn_expense_modifiedby = "lk_msdyn_expense_modifiedby";
			[Relationship("msdyn_expense", EntityRole.Referenced, "lk_msdyn_expense_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_expense_modifiedonbehalfby = "lk_msdyn_expense_modifiedonbehalfby";
			[Relationship("msdyn_expensecategory", EntityRole.Referenced, "lk_msdyn_expensecategory_createdby", "createdby")]
			public const string lk_msdyn_expensecategory_createdby = "lk_msdyn_expensecategory_createdby";
			[Relationship("msdyn_expensecategory", EntityRole.Referenced, "lk_msdyn_expensecategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_expensecategory_createdonbehalfby = "lk_msdyn_expensecategory_createdonbehalfby";
			[Relationship("msdyn_expensecategory", EntityRole.Referenced, "lk_msdyn_expensecategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_expensecategory_modifiedby = "lk_msdyn_expensecategory_modifiedby";
			[Relationship("msdyn_expensecategory", EntityRole.Referenced, "lk_msdyn_expensecategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_expensecategory_modifiedonbehalfby = "lk_msdyn_expensecategory_modifiedonbehalfby";
			[Relationship("msdyn_expensereceipt", EntityRole.Referenced, "lk_msdyn_expensereceipt_createdby", "createdby")]
			public const string lk_msdyn_expensereceipt_createdby = "lk_msdyn_expensereceipt_createdby";
			[Relationship("msdyn_expensereceipt", EntityRole.Referenced, "lk_msdyn_expensereceipt_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_expensereceipt_createdonbehalfby = "lk_msdyn_expensereceipt_createdonbehalfby";
			[Relationship("msdyn_expensereceipt", EntityRole.Referenced, "lk_msdyn_expensereceipt_modifiedby", "modifiedby")]
			public const string lk_msdyn_expensereceipt_modifiedby = "lk_msdyn_expensereceipt_modifiedby";
			[Relationship("msdyn_expensereceipt", EntityRole.Referenced, "lk_msdyn_expensereceipt_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_expensereceipt_modifiedonbehalfby = "lk_msdyn_expensereceipt_modifiedonbehalfby";
			[Relationship("msdyn_fact", EntityRole.Referenced, "lk_msdyn_fact_createdby", "createdby")]
			public const string lk_msdyn_fact_createdby = "lk_msdyn_fact_createdby";
			[Relationship("msdyn_fact", EntityRole.Referenced, "lk_msdyn_fact_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_fact_createdonbehalfby = "lk_msdyn_fact_createdonbehalfby";
			[Relationship("msdyn_fact", EntityRole.Referenced, "lk_msdyn_fact_modifiedby", "modifiedby")]
			public const string lk_msdyn_fact_modifiedby = "lk_msdyn_fact_modifiedby";
			[Relationship("msdyn_fact", EntityRole.Referenced, "lk_msdyn_fact_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_fact_modifiedonbehalfby = "lk_msdyn_fact_modifiedonbehalfby";
			[Relationship("msdyn_feedbackmapping", EntityRole.Referenced, "lk_msdyn_feedbackmapping_createdby", "")]
			public const string lk_msdyn_feedbackmapping_createdby = "lk_msdyn_feedbackmapping_createdby";
			[Relationship("msdyn_feedbackmapping", EntityRole.Referenced, "lk_msdyn_feedbackmapping_createdonbehalfby", "")]
			public const string lk_msdyn_feedbackmapping_createdonbehalfby = "lk_msdyn_feedbackmapping_createdonbehalfby";
			[Relationship("msdyn_feedbackmapping", EntityRole.Referenced, "lk_msdyn_feedbackmapping_modifiedby", "")]
			public const string lk_msdyn_feedbackmapping_modifiedby = "lk_msdyn_feedbackmapping_modifiedby";
			[Relationship("msdyn_feedbackmapping", EntityRole.Referenced, "lk_msdyn_feedbackmapping_modifiedonbehalfby", "")]
			public const string lk_msdyn_feedbackmapping_modifiedonbehalfby = "lk_msdyn_feedbackmapping_modifiedonbehalfby";
			[Relationship("msdyn_feedbacksubsurvey", EntityRole.Referenced, "lk_msdyn_feedbacksubsurvey_createdby", "")]
			public const string lk_msdyn_feedbacksubsurvey_createdby = "lk_msdyn_feedbacksubsurvey_createdby";
			[Relationship("msdyn_feedbacksubsurvey", EntityRole.Referenced, "lk_msdyn_feedbacksubsurvey_createdonbehalfby", "")]
			public const string lk_msdyn_feedbacksubsurvey_createdonbehalfby = "lk_msdyn_feedbacksubsurvey_createdonbehalfby";
			[Relationship("msdyn_feedbacksubsurvey", EntityRole.Referenced, "lk_msdyn_feedbacksubsurvey_modifiedby", "")]
			public const string lk_msdyn_feedbacksubsurvey_modifiedby = "lk_msdyn_feedbacksubsurvey_modifiedby";
			[Relationship("msdyn_feedbacksubsurvey", EntityRole.Referenced, "lk_msdyn_feedbacksubsurvey_modifiedonbehalfby", "")]
			public const string lk_msdyn_feedbacksubsurvey_modifiedonbehalfby = "lk_msdyn_feedbacksubsurvey_modifiedonbehalfby";
			[Relationship("msdyn_fieldcomputation", EntityRole.Referenced, "lk_msdyn_fieldcomputation_createdby", "createdby")]
			public const string lk_msdyn_fieldcomputation_createdby = "lk_msdyn_fieldcomputation_createdby";
			[Relationship("msdyn_fieldcomputation", EntityRole.Referenced, "lk_msdyn_fieldcomputation_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_fieldcomputation_createdonbehalfby = "lk_msdyn_fieldcomputation_createdonbehalfby";
			[Relationship("msdyn_fieldcomputation", EntityRole.Referenced, "lk_msdyn_fieldcomputation_modifiedby", "modifiedby")]
			public const string lk_msdyn_fieldcomputation_modifiedby = "lk_msdyn_fieldcomputation_modifiedby";
			[Relationship("msdyn_fieldcomputation", EntityRole.Referenced, "lk_msdyn_fieldcomputation_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_fieldcomputation_modifiedonbehalfby = "lk_msdyn_fieldcomputation_modifiedonbehalfby";
			[Relationship("msdyn_fieldservicepricelistitem", EntityRole.Referenced, "lk_msdyn_fieldservicepricelistitem_createdby", "createdby")]
			public const string lk_msdyn_fieldservicepricelistitem_createdby = "lk_msdyn_fieldservicepricelistitem_createdby";
			[Relationship("msdyn_fieldservicepricelistitem", EntityRole.Referenced, "lk_msdyn_fieldservicepricelistitem_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_fieldservicepricelistitem_createdonbehalfby = "lk_msdyn_fieldservicepricelistitem_createdonbehalfby";
			[Relationship("msdyn_fieldservicepricelistitem", EntityRole.Referenced, "lk_msdyn_fieldservicepricelistitem_modifiedby", "modifiedby")]
			public const string lk_msdyn_fieldservicepricelistitem_modifiedby = "lk_msdyn_fieldservicepricelistitem_modifiedby";
			[Relationship("msdyn_fieldservicepricelistitem", EntityRole.Referenced, "lk_msdyn_fieldservicepricelistitem_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_fieldservicepricelistitem_modifiedonbehalfby = "lk_msdyn_fieldservicepricelistitem_modifiedonbehalfby";
			[Relationship("msdyn_fieldservicesetting", EntityRole.Referenced, "lk_msdyn_fieldservicesetting_createdby", "createdby")]
			public const string lk_msdyn_fieldservicesetting_createdby = "lk_msdyn_fieldservicesetting_createdby";
			[Relationship("msdyn_fieldservicesetting", EntityRole.Referenced, "lk_msdyn_fieldservicesetting_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_fieldservicesetting_createdonbehalfby = "lk_msdyn_fieldservicesetting_createdonbehalfby";
			[Relationship("msdyn_fieldservicesetting", EntityRole.Referenced, "lk_msdyn_fieldservicesetting_modifiedby", "modifiedby")]
			public const string lk_msdyn_fieldservicesetting_modifiedby = "lk_msdyn_fieldservicesetting_modifiedby";
			[Relationship("msdyn_fieldservicesetting", EntityRole.Referenced, "lk_msdyn_fieldservicesetting_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_fieldservicesetting_modifiedonbehalfby = "lk_msdyn_fieldservicesetting_modifiedonbehalfby";
			[Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "lk_msdyn_fieldservicesystemjob_createdby", "createdby")]
			public const string lk_msdyn_fieldservicesystemjob_createdby = "lk_msdyn_fieldservicesystemjob_createdby";
			[Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "lk_msdyn_fieldservicesystemjob_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_fieldservicesystemjob_createdonbehalfby = "lk_msdyn_fieldservicesystemjob_createdonbehalfby";
			[Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "lk_msdyn_fieldservicesystemjob_modifiedby", "modifiedby")]
			public const string lk_msdyn_fieldservicesystemjob_modifiedby = "lk_msdyn_fieldservicesystemjob_modifiedby";
			[Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "lk_msdyn_fieldservicesystemjob_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_fieldservicesystemjob_modifiedonbehalfby = "lk_msdyn_fieldservicesystemjob_modifiedonbehalfby";
			[Relationship("msdyn_findworkevent", EntityRole.Referenced, "lk_msdyn_findworkevent_createdby", "createdby")]
			public const string lk_msdyn_findworkevent_createdby = "lk_msdyn_findworkevent_createdby";
			[Relationship("msdyn_findworkevent", EntityRole.Referenced, "lk_msdyn_findworkevent_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_findworkevent_createdonbehalfby = "lk_msdyn_findworkevent_createdonbehalfby";
			[Relationship("msdyn_findworkevent", EntityRole.Referenced, "lk_msdyn_findworkevent_modifiedby", "modifiedby")]
			public const string lk_msdyn_findworkevent_modifiedby = "lk_msdyn_findworkevent_modifiedby";
			[Relationship("msdyn_findworkevent", EntityRole.Referenced, "lk_msdyn_findworkevent_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_findworkevent_modifiedonbehalfby = "lk_msdyn_findworkevent_modifiedonbehalfby";
			[Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "lk_msdyn_icebreakersconfig_createdby", "createdby")]
			public const string lk_msdyn_icebreakersconfig_createdby = "lk_msdyn_icebreakersconfig_createdby";
			[Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "lk_msdyn_icebreakersconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_icebreakersconfig_createdonbehalfby = "lk_msdyn_icebreakersconfig_createdonbehalfby";
			[Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "lk_msdyn_icebreakersconfig_modifiedby", "modifiedby")]
			public const string lk_msdyn_icebreakersconfig_modifiedby = "lk_msdyn_icebreakersconfig_modifiedby";
			[Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "lk_msdyn_icebreakersconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_icebreakersconfig_modifiedonbehalfby = "lk_msdyn_icebreakersconfig_modifiedonbehalfby";
			[Relationship("msdyn_image", EntityRole.Referenced, "lk_msdyn_image_createdby", "")]
			public const string lk_msdyn_image_createdby = "lk_msdyn_image_createdby";
			[Relationship("msdyn_image", EntityRole.Referenced, "lk_msdyn_image_createdonbehalfby", "")]
			public const string lk_msdyn_image_createdonbehalfby = "lk_msdyn_image_createdonbehalfby";
			[Relationship("msdyn_image", EntityRole.Referenced, "lk_msdyn_image_modifiedby", "")]
			public const string lk_msdyn_image_modifiedby = "lk_msdyn_image_modifiedby";
			[Relationship("msdyn_image", EntityRole.Referenced, "lk_msdyn_image_modifiedonbehalfby", "")]
			public const string lk_msdyn_image_modifiedonbehalfby = "lk_msdyn_image_modifiedonbehalfby";
			[Relationship("msdyn_imagetokencache", EntityRole.Referenced, "lk_msdyn_imagetokencache_createdby", "")]
			public const string lk_msdyn_imagetokencache_createdby = "lk_msdyn_imagetokencache_createdby";
			[Relationship("msdyn_imagetokencache", EntityRole.Referenced, "lk_msdyn_imagetokencache_createdonbehalfby", "")]
			public const string lk_msdyn_imagetokencache_createdonbehalfby = "lk_msdyn_imagetokencache_createdonbehalfby";
			[Relationship("msdyn_imagetokencache", EntityRole.Referenced, "lk_msdyn_imagetokencache_modifiedby", "")]
			public const string lk_msdyn_imagetokencache_modifiedby = "lk_msdyn_imagetokencache_modifiedby";
			[Relationship("msdyn_imagetokencache", EntityRole.Referenced, "lk_msdyn_imagetokencache_modifiedonbehalfby", "")]
			public const string lk_msdyn_imagetokencache_modifiedonbehalfby = "lk_msdyn_imagetokencache_modifiedonbehalfby";
			[Relationship("msdyn_import", EntityRole.Referenced, "lk_msdyn_import_createdby", "")]
			public const string lk_msdyn_import_createdby = "lk_msdyn_import_createdby";
			[Relationship("msdyn_import", EntityRole.Referenced, "lk_msdyn_import_createdonbehalfby", "")]
			public const string lk_msdyn_import_createdonbehalfby = "lk_msdyn_import_createdonbehalfby";
			[Relationship("msdyn_import", EntityRole.Referenced, "lk_msdyn_import_modifiedby", "")]
			public const string lk_msdyn_import_modifiedby = "lk_msdyn_import_modifiedby";
			[Relationship("msdyn_import", EntityRole.Referenced, "lk_msdyn_import_modifiedonbehalfby", "")]
			public const string lk_msdyn_import_modifiedonbehalfby = "lk_msdyn_import_modifiedonbehalfby";
			[Relationship("msdyn_incidenttype", EntityRole.Referenced, "lk_msdyn_incidenttype_createdby", "createdby")]
			public const string lk_msdyn_incidenttype_createdby = "lk_msdyn_incidenttype_createdby";
			[Relationship("msdyn_incidenttype", EntityRole.Referenced, "lk_msdyn_incidenttype_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_incidenttype_createdonbehalfby = "lk_msdyn_incidenttype_createdonbehalfby";
			[Relationship("msdyn_incidenttype", EntityRole.Referenced, "lk_msdyn_incidenttype_modifiedby", "modifiedby")]
			public const string lk_msdyn_incidenttype_modifiedby = "lk_msdyn_incidenttype_modifiedby";
			[Relationship("msdyn_incidenttype", EntityRole.Referenced, "lk_msdyn_incidenttype_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_incidenttype_modifiedonbehalfby = "lk_msdyn_incidenttype_modifiedonbehalfby";
			[Relationship("msdyn_incidenttypecharacteristic", EntityRole.Referenced, "lk_msdyn_incidenttypecharacteristic_createdby", "createdby")]
			public const string lk_msdyn_incidenttypecharacteristic_createdby = "lk_msdyn_incidenttypecharacteristic_createdby";
			[Relationship("msdyn_incidenttypecharacteristic", EntityRole.Referenced, "lk_msdyn_incidenttypecharacteristic_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_incidenttypecharacteristic_createdonbehalfby = "lk_msdyn_incidenttypecharacteristic_createdonbehalfby";
			[Relationship("msdyn_incidenttypecharacteristic", EntityRole.Referenced, "lk_msdyn_incidenttypecharacteristic_modifiedby", "modifiedby")]
			public const string lk_msdyn_incidenttypecharacteristic_modifiedby = "lk_msdyn_incidenttypecharacteristic_modifiedby";
			[Relationship("msdyn_incidenttypecharacteristic", EntityRole.Referenced, "lk_msdyn_incidenttypecharacteristic_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_incidenttypecharacteristic_modifiedonbehalfby = "lk_msdyn_incidenttypecharacteristic_modifiedonbehalfby";
			[Relationship("msdyn_incidenttypeproduct", EntityRole.Referenced, "lk_msdyn_incidenttypeproduct_createdby", "createdby")]
			public const string lk_msdyn_incidenttypeproduct_createdby = "lk_msdyn_incidenttypeproduct_createdby";
			[Relationship("msdyn_incidenttypeproduct", EntityRole.Referenced, "lk_msdyn_incidenttypeproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_incidenttypeproduct_createdonbehalfby = "lk_msdyn_incidenttypeproduct_createdonbehalfby";
			[Relationship("msdyn_incidenttypeproduct", EntityRole.Referenced, "lk_msdyn_incidenttypeproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_incidenttypeproduct_modifiedby = "lk_msdyn_incidenttypeproduct_modifiedby";
			[Relationship("msdyn_incidenttypeproduct", EntityRole.Referenced, "lk_msdyn_incidenttypeproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_incidenttypeproduct_modifiedonbehalfby = "lk_msdyn_incidenttypeproduct_modifiedonbehalfby";
			[Relationship("msdyn_incidenttypeservice", EntityRole.Referenced, "lk_msdyn_incidenttypeservice_createdby", "createdby")]
			public const string lk_msdyn_incidenttypeservice_createdby = "lk_msdyn_incidenttypeservice_createdby";
			[Relationship("msdyn_incidenttypeservice", EntityRole.Referenced, "lk_msdyn_incidenttypeservice_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_incidenttypeservice_createdonbehalfby = "lk_msdyn_incidenttypeservice_createdonbehalfby";
			[Relationship("msdyn_incidenttypeservice", EntityRole.Referenced, "lk_msdyn_incidenttypeservice_modifiedby", "modifiedby")]
			public const string lk_msdyn_incidenttypeservice_modifiedby = "lk_msdyn_incidenttypeservice_modifiedby";
			[Relationship("msdyn_incidenttypeservice", EntityRole.Referenced, "lk_msdyn_incidenttypeservice_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_incidenttypeservice_modifiedonbehalfby = "lk_msdyn_incidenttypeservice_modifiedonbehalfby";
			[Relationship("msdyn_incidenttypeservicetask", EntityRole.Referenced, "lk_msdyn_incidenttypeservicetask_createdby", "createdby")]
			public const string lk_msdyn_incidenttypeservicetask_createdby = "lk_msdyn_incidenttypeservicetask_createdby";
			[Relationship("msdyn_incidenttypeservicetask", EntityRole.Referenced, "lk_msdyn_incidenttypeservicetask_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_incidenttypeservicetask_createdonbehalfby = "lk_msdyn_incidenttypeservicetask_createdonbehalfby";
			[Relationship("msdyn_incidenttypeservicetask", EntityRole.Referenced, "lk_msdyn_incidenttypeservicetask_modifiedby", "modifiedby")]
			public const string lk_msdyn_incidenttypeservicetask_modifiedby = "lk_msdyn_incidenttypeservicetask_modifiedby";
			[Relationship("msdyn_incidenttypeservicetask", EntityRole.Referenced, "lk_msdyn_incidenttypeservicetask_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_incidenttypeservicetask_modifiedonbehalfby = "lk_msdyn_incidenttypeservicetask_modifiedonbehalfby";
			[Relationship("msdyn_integrationjob", EntityRole.Referenced, "lk_msdyn_integrationjob_createdby", "createdby")]
			public const string lk_msdyn_integrationjob_createdby = "lk_msdyn_integrationjob_createdby";
			[Relationship("msdyn_integrationjob", EntityRole.Referenced, "lk_msdyn_integrationjob_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_integrationjob_createdonbehalfby = "lk_msdyn_integrationjob_createdonbehalfby";
			[Relationship("msdyn_integrationjob", EntityRole.Referenced, "lk_msdyn_integrationjob_modifiedby", "modifiedby")]
			public const string lk_msdyn_integrationjob_modifiedby = "lk_msdyn_integrationjob_modifiedby";
			[Relationship("msdyn_integrationjob", EntityRole.Referenced, "lk_msdyn_integrationjob_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_integrationjob_modifiedonbehalfby = "lk_msdyn_integrationjob_modifiedonbehalfby";
			[Relationship("msdyn_integrationjobdetail", EntityRole.Referenced, "lk_msdyn_integrationjobdetail_createdby", "createdby")]
			public const string lk_msdyn_integrationjobdetail_createdby = "lk_msdyn_integrationjobdetail_createdby";
			[Relationship("msdyn_integrationjobdetail", EntityRole.Referenced, "lk_msdyn_integrationjobdetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_integrationjobdetail_createdonbehalfby = "lk_msdyn_integrationjobdetail_createdonbehalfby";
			[Relationship("msdyn_integrationjobdetail", EntityRole.Referenced, "lk_msdyn_integrationjobdetail_modifiedby", "modifiedby")]
			public const string lk_msdyn_integrationjobdetail_modifiedby = "lk_msdyn_integrationjobdetail_modifiedby";
			[Relationship("msdyn_integrationjobdetail", EntityRole.Referenced, "lk_msdyn_integrationjobdetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_integrationjobdetail_modifiedonbehalfby = "lk_msdyn_integrationjobdetail_modifiedonbehalfby";
			[Relationship("msdyn_inventoryadjustment", EntityRole.Referenced, "lk_msdyn_inventoryadjustment_createdby", "createdby")]
			public const string lk_msdyn_inventoryadjustment_createdby = "lk_msdyn_inventoryadjustment_createdby";
			[Relationship("msdyn_inventoryadjustment", EntityRole.Referenced, "lk_msdyn_inventoryadjustment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_inventoryadjustment_createdonbehalfby = "lk_msdyn_inventoryadjustment_createdonbehalfby";
			[Relationship("msdyn_inventoryadjustment", EntityRole.Referenced, "lk_msdyn_inventoryadjustment_modifiedby", "modifiedby")]
			public const string lk_msdyn_inventoryadjustment_modifiedby = "lk_msdyn_inventoryadjustment_modifiedby";
			[Relationship("msdyn_inventoryadjustment", EntityRole.Referenced, "lk_msdyn_inventoryadjustment_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_inventoryadjustment_modifiedonbehalfby = "lk_msdyn_inventoryadjustment_modifiedonbehalfby";
			[Relationship("msdyn_inventoryadjustmentproduct", EntityRole.Referenced, "lk_msdyn_inventoryadjustmentproduct_createdby", "createdby")]
			public const string lk_msdyn_inventoryadjustmentproduct_createdby = "lk_msdyn_inventoryadjustmentproduct_createdby";
			[Relationship("msdyn_inventoryadjustmentproduct", EntityRole.Referenced, "lk_msdyn_inventoryadjustmentproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_inventoryadjustmentproduct_createdonbehalfby = "lk_msdyn_inventoryadjustmentproduct_createdonbehalfby";
			[Relationship("msdyn_inventoryadjustmentproduct", EntityRole.Referenced, "lk_msdyn_inventoryadjustmentproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_inventoryadjustmentproduct_modifiedby = "lk_msdyn_inventoryadjustmentproduct_modifiedby";
			[Relationship("msdyn_inventoryadjustmentproduct", EntityRole.Referenced, "lk_msdyn_inventoryadjustmentproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_inventoryadjustmentproduct_modifiedonbehalfby = "lk_msdyn_inventoryadjustmentproduct_modifiedonbehalfby";
			[Relationship("msdyn_inventoryjournal", EntityRole.Referenced, "lk_msdyn_inventoryjournal_createdby", "createdby")]
			public const string lk_msdyn_inventoryjournal_createdby = "lk_msdyn_inventoryjournal_createdby";
			[Relationship("msdyn_inventoryjournal", EntityRole.Referenced, "lk_msdyn_inventoryjournal_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_inventoryjournal_createdonbehalfby = "lk_msdyn_inventoryjournal_createdonbehalfby";
			[Relationship("msdyn_inventoryjournal", EntityRole.Referenced, "lk_msdyn_inventoryjournal_modifiedby", "modifiedby")]
			public const string lk_msdyn_inventoryjournal_modifiedby = "lk_msdyn_inventoryjournal_modifiedby";
			[Relationship("msdyn_inventoryjournal", EntityRole.Referenced, "lk_msdyn_inventoryjournal_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_inventoryjournal_modifiedonbehalfby = "lk_msdyn_inventoryjournal_modifiedonbehalfby";
			[Relationship("msdyn_inventorytransfer", EntityRole.Referenced, "lk_msdyn_inventorytransfer_createdby", "createdby")]
			public const string lk_msdyn_inventorytransfer_createdby = "lk_msdyn_inventorytransfer_createdby";
			[Relationship("msdyn_inventorytransfer", EntityRole.Referenced, "lk_msdyn_inventorytransfer_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_inventorytransfer_createdonbehalfby = "lk_msdyn_inventorytransfer_createdonbehalfby";
			[Relationship("msdyn_inventorytransfer", EntityRole.Referenced, "lk_msdyn_inventorytransfer_modifiedby", "modifiedby")]
			public const string lk_msdyn_inventorytransfer_modifiedby = "lk_msdyn_inventorytransfer_modifiedby";
			[Relationship("msdyn_inventorytransfer", EntityRole.Referenced, "lk_msdyn_inventorytransfer_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_inventorytransfer_modifiedonbehalfby = "lk_msdyn_inventorytransfer_modifiedonbehalfby";
			[Relationship("msdyn_invoicefrequency", EntityRole.Referenced, "lk_msdyn_invoicefrequency_createdby", "createdby")]
			public const string lk_msdyn_invoicefrequency_createdby = "lk_msdyn_invoicefrequency_createdby";
			[Relationship("msdyn_invoicefrequency", EntityRole.Referenced, "lk_msdyn_invoicefrequency_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_invoicefrequency_createdonbehalfby = "lk_msdyn_invoicefrequency_createdonbehalfby";
			[Relationship("msdyn_invoicefrequency", EntityRole.Referenced, "lk_msdyn_invoicefrequency_modifiedby", "modifiedby")]
			public const string lk_msdyn_invoicefrequency_modifiedby = "lk_msdyn_invoicefrequency_modifiedby";
			[Relationship("msdyn_invoicefrequency", EntityRole.Referenced, "lk_msdyn_invoicefrequency_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_invoicefrequency_modifiedonbehalfby = "lk_msdyn_invoicefrequency_modifiedonbehalfby";
			[Relationship("msdyn_invoicefrequencydetail", EntityRole.Referenced, "lk_msdyn_invoicefrequencydetail_createdby", "createdby")]
			public const string lk_msdyn_invoicefrequencydetail_createdby = "lk_msdyn_invoicefrequencydetail_createdby";
			[Relationship("msdyn_invoicefrequencydetail", EntityRole.Referenced, "lk_msdyn_invoicefrequencydetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_invoicefrequencydetail_createdonbehalfby = "lk_msdyn_invoicefrequencydetail_createdonbehalfby";
			[Relationship("msdyn_invoicefrequencydetail", EntityRole.Referenced, "lk_msdyn_invoicefrequencydetail_modifiedby", "modifiedby")]
			public const string lk_msdyn_invoicefrequencydetail_modifiedby = "lk_msdyn_invoicefrequencydetail_modifiedby";
			[Relationship("msdyn_invoicefrequencydetail", EntityRole.Referenced, "lk_msdyn_invoicefrequencydetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_invoicefrequencydetail_modifiedonbehalfby = "lk_msdyn_invoicefrequencydetail_modifiedonbehalfby";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "lk_msdyn_invoicelinetransaction_createdby", "createdby")]
			public const string lk_msdyn_invoicelinetransaction_createdby = "lk_msdyn_invoicelinetransaction_createdby";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "lk_msdyn_invoicelinetransaction_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_invoicelinetransaction_createdonbehalfby = "lk_msdyn_invoicelinetransaction_createdonbehalfby";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "lk_msdyn_invoicelinetransaction_modifiedby", "modifiedby")]
			public const string lk_msdyn_invoicelinetransaction_modifiedby = "lk_msdyn_invoicelinetransaction_modifiedby";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "lk_msdyn_invoicelinetransaction_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_invoicelinetransaction_modifiedonbehalfby = "lk_msdyn_invoicelinetransaction_modifiedonbehalfby";
			[Relationship("msdyn_journal", EntityRole.Referenced, "lk_msdyn_journal_createdby", "createdby")]
			public const string lk_msdyn_journal_createdby = "lk_msdyn_journal_createdby";
			[Relationship("msdyn_journal", EntityRole.Referenced, "lk_msdyn_journal_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_journal_createdonbehalfby = "lk_msdyn_journal_createdonbehalfby";
			[Relationship("msdyn_journal", EntityRole.Referenced, "lk_msdyn_journal_modifiedby", "modifiedby")]
			public const string lk_msdyn_journal_modifiedby = "lk_msdyn_journal_modifiedby";
			[Relationship("msdyn_journal", EntityRole.Referenced, "lk_msdyn_journal_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_journal_modifiedonbehalfby = "lk_msdyn_journal_modifiedonbehalfby";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "lk_msdyn_journalline_createdby", "createdby")]
			public const string lk_msdyn_journalline_createdby = "lk_msdyn_journalline_createdby";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "lk_msdyn_journalline_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_journalline_createdonbehalfby = "lk_msdyn_journalline_createdonbehalfby";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "lk_msdyn_journalline_modifiedby", "modifiedby")]
			public const string lk_msdyn_journalline_modifiedby = "lk_msdyn_journalline_modifiedby";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "lk_msdyn_journalline_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_journalline_modifiedonbehalfby = "lk_msdyn_journalline_modifiedonbehalfby";
			[Relationship("msdyn_linkedanswer", EntityRole.Referenced, "lk_msdyn_linkedanswer_createdby", "")]
			public const string lk_msdyn_linkedanswer_createdby = "lk_msdyn_linkedanswer_createdby";
			[Relationship("msdyn_linkedanswer", EntityRole.Referenced, "lk_msdyn_linkedanswer_createdonbehalfby", "")]
			public const string lk_msdyn_linkedanswer_createdonbehalfby = "lk_msdyn_linkedanswer_createdonbehalfby";
			[Relationship("msdyn_linkedanswer", EntityRole.Referenced, "lk_msdyn_linkedanswer_modifiedby", "")]
			public const string lk_msdyn_linkedanswer_modifiedby = "lk_msdyn_linkedanswer_modifiedby";
			[Relationship("msdyn_linkedanswer", EntityRole.Referenced, "lk_msdyn_linkedanswer_modifiedonbehalfby", "")]
			public const string lk_msdyn_linkedanswer_modifiedonbehalfby = "lk_msdyn_linkedanswer_modifiedonbehalfby";
			[Relationship("msdyn_mlresultcache", EntityRole.Referenced, "lk_msdyn_mlresultcache_createdby", "createdby")]
			public const string lk_msdyn_mlresultcache_createdby = "lk_msdyn_mlresultcache_createdby";
			[Relationship("msdyn_mlresultcache", EntityRole.Referenced, "lk_msdyn_mlresultcache_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_mlresultcache_createdonbehalfby = "lk_msdyn_mlresultcache_createdonbehalfby";
			[Relationship("msdyn_mlresultcache", EntityRole.Referenced, "lk_msdyn_mlresultcache_modifiedby", "modifiedby")]
			public const string lk_msdyn_mlresultcache_modifiedby = "lk_msdyn_mlresultcache_modifiedby";
			[Relationship("msdyn_mlresultcache", EntityRole.Referenced, "lk_msdyn_mlresultcache_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_mlresultcache_modifiedonbehalfby = "lk_msdyn_mlresultcache_modifiedonbehalfby";
			[Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "lk_msdyn_notesanalysisconfig_createdby", "createdby")]
			public const string lk_msdyn_notesanalysisconfig_createdby = "lk_msdyn_notesanalysisconfig_createdby";
			[Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "lk_msdyn_notesanalysisconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_notesanalysisconfig_createdonbehalfby = "lk_msdyn_notesanalysisconfig_createdonbehalfby";
			[Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "lk_msdyn_notesanalysisconfig_modifiedby", "modifiedby")]
			public const string lk_msdyn_notesanalysisconfig_modifiedby = "lk_msdyn_notesanalysisconfig_modifiedby";
			[Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "lk_msdyn_notesanalysisconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_notesanalysisconfig_modifiedonbehalfby = "lk_msdyn_notesanalysisconfig_modifiedonbehalfby";
			[Relationship("msdyn_opportunitylineresourcecategory", EntityRole.Referenced, "lk_msdyn_opportunitylineresourcecategory_createdby", "createdby")]
			public const string lk_msdyn_opportunitylineresourcecategory_createdby = "lk_msdyn_opportunitylineresourcecategory_createdby";
			[Relationship("msdyn_opportunitylineresourcecategory", EntityRole.Referenced, "lk_msdyn_opportunitylineresourcecategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_opportunitylineresourcecategory_createdonbehalfby = "lk_msdyn_opportunitylineresourcecategory_createdonbehalfby";
			[Relationship("msdyn_opportunitylineresourcecategory", EntityRole.Referenced, "lk_msdyn_opportunitylineresourcecategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_opportunitylineresourcecategory_modifiedby = "lk_msdyn_opportunitylineresourcecategory_modifiedby";
			[Relationship("msdyn_opportunitylineresourcecategory", EntityRole.Referenced, "lk_msdyn_opportunitylineresourcecategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_opportunitylineresourcecategory_modifiedonbehalfby = "lk_msdyn_opportunitylineresourcecategory_modifiedonbehalfby";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "lk_msdyn_opportunitylinetransaction_createdby", "createdby")]
			public const string lk_msdyn_opportunitylinetransaction_createdby = "lk_msdyn_opportunitylinetransaction_createdby";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "lk_msdyn_opportunitylinetransaction_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_opportunitylinetransaction_createdonbehalfby = "lk_msdyn_opportunitylinetransaction_createdonbehalfby";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "lk_msdyn_opportunitylinetransaction_modifiedby", "modifiedby")]
			public const string lk_msdyn_opportunitylinetransaction_modifiedby = "lk_msdyn_opportunitylinetransaction_modifiedby";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "lk_msdyn_opportunitylinetransaction_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_opportunitylinetransaction_modifiedonbehalfby = "lk_msdyn_opportunitylinetransaction_modifiedonbehalfby";
			[Relationship("msdyn_opportunitylinetransactioncategory", EntityRole.Referenced, "lk_msdyn_opportunitylinetransactioncategory_createdby", "createdby")]
			public const string lk_msdyn_opportunitylinetransactioncategory_createdby = "lk_msdyn_opportunitylinetransactioncategory_createdby";
			[Relationship("msdyn_opportunitylinetransactioncategory", EntityRole.Referenced, "lk_msdyn_opportunitylinetransactioncategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_opportunitylinetransactioncategory_createdonbehalfby = "lk_msdyn_opportunitylinetransactioncategory_createdonbehalfby";
			[Relationship("msdyn_opportunitylinetransactioncategory", EntityRole.Referenced, "lk_msdyn_opportunitylinetransactioncategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_opportunitylinetransactioncategory_modifiedby = "lk_msdyn_opportunitylinetransactioncategory_modifiedby";
			[Relationship("msdyn_opportunitylinetransactioncategory", EntityRole.Referenced, "lk_msdyn_opportunitylinetransactioncategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_opportunitylinetransactioncategory_modifiedonbehalfby = "lk_msdyn_opportunitylinetransactioncategory_modifiedonbehalfby";
			[Relationship("msdyn_opportunitylinetransactionclassificatio", EntityRole.Referenced, "lk_msdyn_opportunitylinetransactionclassificatio_createdby", "createdby")]
			public const string lk_msdyn_opportunitylinetransactionclassificatio_createdby = "lk_msdyn_opportunitylinetransactionclassificatio_createdby";
			[Relationship("msdyn_opportunitylinetransactionclassificatio", EntityRole.Referenced, "lk_msdyn_opportunitylinetransactionclassificatio_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_opportunitylinetransactionclassificatio_createdonbehalfby = "lk_msdyn_opportunitylinetransactionclassificatio_createdonbehalfby";
			[Relationship("msdyn_opportunitylinetransactionclassificatio", EntityRole.Referenced, "lk_msdyn_opportunitylinetransactionclassificatio_modifiedby", "modifiedby")]
			public const string lk_msdyn_opportunitylinetransactionclassificatio_modifiedby = "lk_msdyn_opportunitylinetransactionclassificatio_modifiedby";
			[Relationship("msdyn_opportunitylinetransactionclassificatio", EntityRole.Referenced, "lk_msdyn_opportunitylinetransactionclassificatio_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_opportunitylinetransactionclassificatio_modifiedonbehalfby = "lk_msdyn_opportunitylinetransactionclassificatio_modifiedonbehalfby";
			[Relationship("msdyn_opportunitypricelist", EntityRole.Referenced, "lk_msdyn_opportunitypricelist_createdby", "createdby")]
			public const string lk_msdyn_opportunitypricelist_createdby = "lk_msdyn_opportunitypricelist_createdby";
			[Relationship("msdyn_opportunitypricelist", EntityRole.Referenced, "lk_msdyn_opportunitypricelist_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_opportunitypricelist_createdonbehalfby = "lk_msdyn_opportunitypricelist_createdonbehalfby";
			[Relationship("msdyn_opportunitypricelist", EntityRole.Referenced, "lk_msdyn_opportunitypricelist_modifiedby", "modifiedby")]
			public const string lk_msdyn_opportunitypricelist_modifiedby = "lk_msdyn_opportunitypricelist_modifiedby";
			[Relationship("msdyn_opportunitypricelist", EntityRole.Referenced, "lk_msdyn_opportunitypricelist_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_opportunitypricelist_modifiedonbehalfby = "lk_msdyn_opportunitypricelist_modifiedonbehalfby";
			[Relationship("msdyn_orderinvoicingdate", EntityRole.Referenced, "lk_msdyn_orderinvoicingdate_createdby", "createdby")]
			public const string lk_msdyn_orderinvoicingdate_createdby = "lk_msdyn_orderinvoicingdate_createdby";
			[Relationship("msdyn_orderinvoicingdate", EntityRole.Referenced, "lk_msdyn_orderinvoicingdate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderinvoicingdate_createdonbehalfby = "lk_msdyn_orderinvoicingdate_createdonbehalfby";
			[Relationship("msdyn_orderinvoicingdate", EntityRole.Referenced, "lk_msdyn_orderinvoicingdate_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderinvoicingdate_modifiedby = "lk_msdyn_orderinvoicingdate_modifiedby";
			[Relationship("msdyn_orderinvoicingdate", EntityRole.Referenced, "lk_msdyn_orderinvoicingdate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderinvoicingdate_modifiedonbehalfby = "lk_msdyn_orderinvoicingdate_modifiedonbehalfby";
			[Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "lk_msdyn_orderinvoicingproduct_createdby", "createdby")]
			public const string lk_msdyn_orderinvoicingproduct_createdby = "lk_msdyn_orderinvoicingproduct_createdby";
			[Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "lk_msdyn_orderinvoicingproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderinvoicingproduct_createdonbehalfby = "lk_msdyn_orderinvoicingproduct_createdonbehalfby";
			[Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "lk_msdyn_orderinvoicingproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderinvoicingproduct_modifiedby = "lk_msdyn_orderinvoicingproduct_modifiedby";
			[Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "lk_msdyn_orderinvoicingproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderinvoicingproduct_modifiedonbehalfby = "lk_msdyn_orderinvoicingproduct_modifiedonbehalfby";
			[Relationship("msdyn_orderinvoicingsetup", EntityRole.Referenced, "lk_msdyn_orderinvoicingsetup_createdby", "createdby")]
			public const string lk_msdyn_orderinvoicingsetup_createdby = "lk_msdyn_orderinvoicingsetup_createdby";
			[Relationship("msdyn_orderinvoicingsetup", EntityRole.Referenced, "lk_msdyn_orderinvoicingsetup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderinvoicingsetup_createdonbehalfby = "lk_msdyn_orderinvoicingsetup_createdonbehalfby";
			[Relationship("msdyn_orderinvoicingsetup", EntityRole.Referenced, "lk_msdyn_orderinvoicingsetup_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderinvoicingsetup_modifiedby = "lk_msdyn_orderinvoicingsetup_modifiedby";
			[Relationship("msdyn_orderinvoicingsetup", EntityRole.Referenced, "lk_msdyn_orderinvoicingsetup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderinvoicingsetup_modifiedonbehalfby = "lk_msdyn_orderinvoicingsetup_modifiedonbehalfby";
			[Relationship("msdyn_orderinvoicingsetupdate", EntityRole.Referenced, "lk_msdyn_orderinvoicingsetupdate_createdby", "createdby")]
			public const string lk_msdyn_orderinvoicingsetupdate_createdby = "lk_msdyn_orderinvoicingsetupdate_createdby";
			[Relationship("msdyn_orderinvoicingsetupdate", EntityRole.Referenced, "lk_msdyn_orderinvoicingsetupdate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderinvoicingsetupdate_createdonbehalfby = "lk_msdyn_orderinvoicingsetupdate_createdonbehalfby";
			[Relationship("msdyn_orderinvoicingsetupdate", EntityRole.Referenced, "lk_msdyn_orderinvoicingsetupdate_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderinvoicingsetupdate_modifiedby = "lk_msdyn_orderinvoicingsetupdate_modifiedby";
			[Relationship("msdyn_orderinvoicingsetupdate", EntityRole.Referenced, "lk_msdyn_orderinvoicingsetupdate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderinvoicingsetupdate_modifiedonbehalfby = "lk_msdyn_orderinvoicingsetupdate_modifiedonbehalfby";
			[Relationship("msdyn_orderlineresourcecategory", EntityRole.Referenced, "lk_msdyn_orderlineresourcecategory_createdby", "createdby")]
			public const string lk_msdyn_orderlineresourcecategory_createdby = "lk_msdyn_orderlineresourcecategory_createdby";
			[Relationship("msdyn_orderlineresourcecategory", EntityRole.Referenced, "lk_msdyn_orderlineresourcecategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderlineresourcecategory_createdonbehalfby = "lk_msdyn_orderlineresourcecategory_createdonbehalfby";
			[Relationship("msdyn_orderlineresourcecategory", EntityRole.Referenced, "lk_msdyn_orderlineresourcecategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderlineresourcecategory_modifiedby = "lk_msdyn_orderlineresourcecategory_modifiedby";
			[Relationship("msdyn_orderlineresourcecategory", EntityRole.Referenced, "lk_msdyn_orderlineresourcecategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderlineresourcecategory_modifiedonbehalfby = "lk_msdyn_orderlineresourcecategory_modifiedonbehalfby";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "lk_msdyn_orderlinetransaction_createdby", "createdby")]
			public const string lk_msdyn_orderlinetransaction_createdby = "lk_msdyn_orderlinetransaction_createdby";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "lk_msdyn_orderlinetransaction_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderlinetransaction_createdonbehalfby = "lk_msdyn_orderlinetransaction_createdonbehalfby";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "lk_msdyn_orderlinetransaction_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderlinetransaction_modifiedby = "lk_msdyn_orderlinetransaction_modifiedby";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "lk_msdyn_orderlinetransaction_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderlinetransaction_modifiedonbehalfby = "lk_msdyn_orderlinetransaction_modifiedonbehalfby";
			[Relationship("msdyn_orderlinetransactioncategory", EntityRole.Referenced, "lk_msdyn_orderlinetransactioncategory_createdby", "createdby")]
			public const string lk_msdyn_orderlinetransactioncategory_createdby = "lk_msdyn_orderlinetransactioncategory_createdby";
			[Relationship("msdyn_orderlinetransactioncategory", EntityRole.Referenced, "lk_msdyn_orderlinetransactioncategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderlinetransactioncategory_createdonbehalfby = "lk_msdyn_orderlinetransactioncategory_createdonbehalfby";
			[Relationship("msdyn_orderlinetransactioncategory", EntityRole.Referenced, "lk_msdyn_orderlinetransactioncategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderlinetransactioncategory_modifiedby = "lk_msdyn_orderlinetransactioncategory_modifiedby";
			[Relationship("msdyn_orderlinetransactioncategory", EntityRole.Referenced, "lk_msdyn_orderlinetransactioncategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderlinetransactioncategory_modifiedonbehalfby = "lk_msdyn_orderlinetransactioncategory_modifiedonbehalfby";
			[Relationship("msdyn_orderlinetransactionclassification", EntityRole.Referenced, "lk_msdyn_orderlinetransactionclassification_createdby", "createdby")]
			public const string lk_msdyn_orderlinetransactionclassification_createdby = "lk_msdyn_orderlinetransactionclassification_createdby";
			[Relationship("msdyn_orderlinetransactionclassification", EntityRole.Referenced, "lk_msdyn_orderlinetransactionclassification_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderlinetransactionclassification_createdonbehalfby = "lk_msdyn_orderlinetransactionclassification_createdonbehalfby";
			[Relationship("msdyn_orderlinetransactionclassification", EntityRole.Referenced, "lk_msdyn_orderlinetransactionclassification_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderlinetransactionclassification_modifiedby = "lk_msdyn_orderlinetransactionclassification_modifiedby";
			[Relationship("msdyn_orderlinetransactionclassification", EntityRole.Referenced, "lk_msdyn_orderlinetransactionclassification_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderlinetransactionclassification_modifiedonbehalfby = "lk_msdyn_orderlinetransactionclassification_modifiedonbehalfby";
			[Relationship("msdyn_orderpricelist", EntityRole.Referenced, "lk_msdyn_orderpricelist_createdby", "createdby")]
			public const string lk_msdyn_orderpricelist_createdby = "lk_msdyn_orderpricelist_createdby";
			[Relationship("msdyn_orderpricelist", EntityRole.Referenced, "lk_msdyn_orderpricelist_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_orderpricelist_createdonbehalfby = "lk_msdyn_orderpricelist_createdonbehalfby";
			[Relationship("msdyn_orderpricelist", EntityRole.Referenced, "lk_msdyn_orderpricelist_modifiedby", "modifiedby")]
			public const string lk_msdyn_orderpricelist_modifiedby = "lk_msdyn_orderpricelist_modifiedby";
			[Relationship("msdyn_orderpricelist", EntityRole.Referenced, "lk_msdyn_orderpricelist_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_orderpricelist_modifiedonbehalfby = "lk_msdyn_orderpricelist_modifiedonbehalfby";
			[Relationship("msdyn_organizationalunit", EntityRole.Referenced, "lk_msdyn_organizationalunit_createdby", "createdby")]
			public const string lk_msdyn_organizationalunit_createdby = "lk_msdyn_organizationalunit_createdby";
			[Relationship("msdyn_organizationalunit", EntityRole.Referenced, "lk_msdyn_organizationalunit_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_organizationalunit_createdonbehalfby = "lk_msdyn_organizationalunit_createdonbehalfby";
			[Relationship("msdyn_organizationalunit", EntityRole.Referenced, "lk_msdyn_organizationalunit_modifiedby", "modifiedby")]
			public const string lk_msdyn_organizationalunit_modifiedby = "lk_msdyn_organizationalunit_modifiedby";
			[Relationship("msdyn_organizationalunit", EntityRole.Referenced, "lk_msdyn_organizationalunit_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_organizationalunit_modifiedonbehalfby = "lk_msdyn_organizationalunit_modifiedonbehalfby";
			[Relationship("msdyn_orginsightsuserdashboarddefinition", EntityRole.Referenced, "lk_msdyn_orginsightsuserdashboarddefinition_createdby", "")]
			public const string lk_msdyn_orginsightsuserdashboarddefinition_createdby = "lk_msdyn_orginsightsuserdashboarddefinition_createdby";
			[Relationship("msdyn_orginsightsuserdashboarddefinition", EntityRole.Referenced, "lk_msdyn_orginsightsuserdashboarddefinition_createdonbehalfby", "")]
			public const string lk_msdyn_orginsightsuserdashboarddefinition_createdonbehalfby = "lk_msdyn_orginsightsuserdashboarddefinition_createdonbehalfby";
			[Relationship("msdyn_orginsightsuserdashboarddefinition", EntityRole.Referenced, "lk_msdyn_orginsightsuserdashboarddefinition_modifiedby", "")]
			public const string lk_msdyn_orginsightsuserdashboarddefinition_modifiedby = "lk_msdyn_orginsightsuserdashboarddefinition_modifiedby";
			[Relationship("msdyn_orginsightsuserdashboarddefinition", EntityRole.Referenced, "lk_msdyn_orginsightsuserdashboarddefinition_modifiedonbehalfby", "")]
			public const string lk_msdyn_orginsightsuserdashboarddefinition_modifiedonbehalfby = "lk_msdyn_orginsightsuserdashboarddefinition_modifiedonbehalfby";
			[Relationship("msdyn_page", EntityRole.Referenced, "lk_msdyn_page_createdby", "")]
			public const string lk_msdyn_page_createdby = "lk_msdyn_page_createdby";
			[Relationship("msdyn_page", EntityRole.Referenced, "lk_msdyn_page_createdonbehalfby", "")]
			public const string lk_msdyn_page_createdonbehalfby = "lk_msdyn_page_createdonbehalfby";
			[Relationship("msdyn_page", EntityRole.Referenced, "lk_msdyn_page_modifiedby", "")]
			public const string lk_msdyn_page_modifiedby = "lk_msdyn_page_modifiedby";
			[Relationship("msdyn_page", EntityRole.Referenced, "lk_msdyn_page_modifiedonbehalfby", "")]
			public const string lk_msdyn_page_modifiedonbehalfby = "lk_msdyn_page_modifiedonbehalfby";
			[Relationship("msdyn_payment", EntityRole.Referenced, "lk_msdyn_payment_createdby", "createdby")]
			public const string lk_msdyn_payment_createdby = "lk_msdyn_payment_createdby";
			[Relationship("msdyn_payment", EntityRole.Referenced, "lk_msdyn_payment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_payment_createdonbehalfby = "lk_msdyn_payment_createdonbehalfby";
			[Relationship("msdyn_payment", EntityRole.Referenced, "lk_msdyn_payment_modifiedby", "modifiedby")]
			public const string lk_msdyn_payment_modifiedby = "lk_msdyn_payment_modifiedby";
			[Relationship("msdyn_payment", EntityRole.Referenced, "lk_msdyn_payment_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_payment_modifiedonbehalfby = "lk_msdyn_payment_modifiedonbehalfby";
			[Relationship("msdyn_paymentdetail", EntityRole.Referenced, "lk_msdyn_paymentdetail_createdby", "createdby")]
			public const string lk_msdyn_paymentdetail_createdby = "lk_msdyn_paymentdetail_createdby";
			[Relationship("msdyn_paymentdetail", EntityRole.Referenced, "lk_msdyn_paymentdetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_paymentdetail_createdonbehalfby = "lk_msdyn_paymentdetail_createdonbehalfby";
			[Relationship("msdyn_paymentdetail", EntityRole.Referenced, "lk_msdyn_paymentdetail_modifiedby", "modifiedby")]
			public const string lk_msdyn_paymentdetail_modifiedby = "lk_msdyn_paymentdetail_modifiedby";
			[Relationship("msdyn_paymentdetail", EntityRole.Referenced, "lk_msdyn_paymentdetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_paymentdetail_modifiedonbehalfby = "lk_msdyn_paymentdetail_modifiedonbehalfby";
			[Relationship("msdyn_paymentmethod", EntityRole.Referenced, "lk_msdyn_paymentmethod_createdby", "createdby")]
			public const string lk_msdyn_paymentmethod_createdby = "lk_msdyn_paymentmethod_createdby";
			[Relationship("msdyn_paymentmethod", EntityRole.Referenced, "lk_msdyn_paymentmethod_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_paymentmethod_createdonbehalfby = "lk_msdyn_paymentmethod_createdonbehalfby";
			[Relationship("msdyn_paymentmethod", EntityRole.Referenced, "lk_msdyn_paymentmethod_modifiedby", "modifiedby")]
			public const string lk_msdyn_paymentmethod_modifiedby = "lk_msdyn_paymentmethod_modifiedby";
			[Relationship("msdyn_paymentmethod", EntityRole.Referenced, "lk_msdyn_paymentmethod_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_paymentmethod_modifiedonbehalfby = "lk_msdyn_paymentmethod_modifiedonbehalfby";
			[Relationship("msdyn_paymentterm", EntityRole.Referenced, "lk_msdyn_paymentterm_createdby", "createdby")]
			public const string lk_msdyn_paymentterm_createdby = "lk_msdyn_paymentterm_createdby";
			[Relationship("msdyn_paymentterm", EntityRole.Referenced, "lk_msdyn_paymentterm_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_paymentterm_createdonbehalfby = "lk_msdyn_paymentterm_createdonbehalfby";
			[Relationship("msdyn_paymentterm", EntityRole.Referenced, "lk_msdyn_paymentterm_modifiedby", "modifiedby")]
			public const string lk_msdyn_paymentterm_modifiedby = "lk_msdyn_paymentterm_modifiedby";
			[Relationship("msdyn_paymentterm", EntityRole.Referenced, "lk_msdyn_paymentterm_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_paymentterm_modifiedonbehalfby = "lk_msdyn_paymentterm_modifiedonbehalfby";
			[Relationship("msdyn_playbookactivity", EntityRole.Referenced, "lk_msdyn_playbookactivity_createdby", "createdby")]
			public const string lk_msdyn_playbookactivity_createdby = "lk_msdyn_playbookactivity_createdby";
			[Relationship("msdyn_playbookactivity", EntityRole.Referenced, "lk_msdyn_playbookactivity_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_playbookactivity_createdonbehalfby = "lk_msdyn_playbookactivity_createdonbehalfby";
			[Relationship("msdyn_playbookactivity", EntityRole.Referenced, "lk_msdyn_playbookactivity_modifiedby", "modifiedby")]
			public const string lk_msdyn_playbookactivity_modifiedby = "lk_msdyn_playbookactivity_modifiedby";
			[Relationship("msdyn_playbookactivity", EntityRole.Referenced, "lk_msdyn_playbookactivity_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_playbookactivity_modifiedonbehalfby = "lk_msdyn_playbookactivity_modifiedonbehalfby";
			[Relationship("msdyn_playbookactivityattribute", EntityRole.Referenced, "lk_msdyn_playbookactivityattribute_createdby", "createdby")]
			public const string lk_msdyn_playbookactivityattribute_createdby = "lk_msdyn_playbookactivityattribute_createdby";
			[Relationship("msdyn_playbookactivityattribute", EntityRole.Referenced, "lk_msdyn_playbookactivityattribute_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_playbookactivityattribute_createdonbehalfby = "lk_msdyn_playbookactivityattribute_createdonbehalfby";
			[Relationship("msdyn_playbookactivityattribute", EntityRole.Referenced, "lk_msdyn_playbookactivityattribute_modifiedby", "modifiedby")]
			public const string lk_msdyn_playbookactivityattribute_modifiedby = "lk_msdyn_playbookactivityattribute_modifiedby";
			[Relationship("msdyn_playbookactivityattribute", EntityRole.Referenced, "lk_msdyn_playbookactivityattribute_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_playbookactivityattribute_modifiedonbehalfby = "lk_msdyn_playbookactivityattribute_modifiedonbehalfby";
			[Relationship("msdyn_playbookcategory", EntityRole.Referenced, "lk_msdyn_playbookcategory_createdby", "createdby")]
			public const string lk_msdyn_playbookcategory_createdby = "lk_msdyn_playbookcategory_createdby";
			[Relationship("msdyn_playbookcategory", EntityRole.Referenced, "lk_msdyn_playbookcategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_playbookcategory_createdonbehalfby = "lk_msdyn_playbookcategory_createdonbehalfby";
			[Relationship("msdyn_playbookcategory", EntityRole.Referenced, "lk_msdyn_playbookcategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_playbookcategory_modifiedby = "lk_msdyn_playbookcategory_modifiedby";
			[Relationship("msdyn_playbookcategory", EntityRole.Referenced, "lk_msdyn_playbookcategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_playbookcategory_modifiedonbehalfby = "lk_msdyn_playbookcategory_modifiedonbehalfby";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "lk_msdyn_playbookinstance_createdby", "createdby")]
			public const string lk_msdyn_playbookinstance_createdby = "lk_msdyn_playbookinstance_createdby";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "lk_msdyn_playbookinstance_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_playbookinstance_createdonbehalfby = "lk_msdyn_playbookinstance_createdonbehalfby";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "lk_msdyn_playbookinstance_modifiedby", "modifiedby")]
			public const string lk_msdyn_playbookinstance_modifiedby = "lk_msdyn_playbookinstance_modifiedby";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "lk_msdyn_playbookinstance_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_playbookinstance_modifiedonbehalfby = "lk_msdyn_playbookinstance_modifiedonbehalfby";
			[Relationship("msdyn_playbooktemplate", EntityRole.Referenced, "lk_msdyn_playbooktemplate_createdby", "createdby")]
			public const string lk_msdyn_playbooktemplate_createdby = "lk_msdyn_playbooktemplate_createdby";
			[Relationship("msdyn_playbooktemplate", EntityRole.Referenced, "lk_msdyn_playbooktemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_playbooktemplate_createdonbehalfby = "lk_msdyn_playbooktemplate_createdonbehalfby";
			[Relationship("msdyn_playbooktemplate", EntityRole.Referenced, "lk_msdyn_playbooktemplate_modifiedby", "modifiedby")]
			public const string lk_msdyn_playbooktemplate_modifiedby = "lk_msdyn_playbooktemplate_modifiedby";
			[Relationship("msdyn_playbooktemplate", EntityRole.Referenced, "lk_msdyn_playbooktemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_playbooktemplate_modifiedonbehalfby = "lk_msdyn_playbooktemplate_modifiedonbehalfby";
			[Relationship("msdyn_postalbum", EntityRole.Referenced, "lk_msdyn_postalbum_createdby", "createdby")]
			public const string lk_msdyn_postalbum_createdby = "lk_msdyn_postalbum_createdby";
			[Relationship("msdyn_postalbum", EntityRole.Referenced, "lk_msdyn_postalbum_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_postalbum_createdonbehalfby = "lk_msdyn_postalbum_createdonbehalfby";
			[Relationship("msdyn_postalbum", EntityRole.Referenced, "lk_msdyn_postalbum_modifiedby", "modifiedby")]
			public const string lk_msdyn_postalbum_modifiedby = "lk_msdyn_postalbum_modifiedby";
			[Relationship("msdyn_postalbum", EntityRole.Referenced, "lk_msdyn_postalbum_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_postalbum_modifiedonbehalfby = "lk_msdyn_postalbum_modifiedonbehalfby";
			[Relationship("msdyn_postalcode", EntityRole.Referenced, "lk_msdyn_postalcode_createdby", "createdby")]
			public const string lk_msdyn_postalcode_createdby = "lk_msdyn_postalcode_createdby";
			[Relationship("msdyn_postalcode", EntityRole.Referenced, "lk_msdyn_postalcode_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_postalcode_createdonbehalfby = "lk_msdyn_postalcode_createdonbehalfby";
			[Relationship("msdyn_postalcode", EntityRole.Referenced, "lk_msdyn_postalcode_modifiedby", "modifiedby")]
			public const string lk_msdyn_postalcode_modifiedby = "lk_msdyn_postalcode_modifiedby";
			[Relationship("msdyn_postalcode", EntityRole.Referenced, "lk_msdyn_postalcode_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_postalcode_modifiedonbehalfby = "lk_msdyn_postalcode_modifiedonbehalfby";
			[Relationship("msdyn_postconfig", EntityRole.Referenced, "lk_msdyn_postconfig_createdby", "createdby")]
			public const string lk_msdyn_postconfig_createdby = "lk_msdyn_postconfig_createdby";
			[Relationship("msdyn_postconfig", EntityRole.Referenced, "lk_msdyn_postconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_postconfig_createdonbehalfby = "lk_msdyn_postconfig_createdonbehalfby";
			[Relationship("msdyn_postconfig", EntityRole.Referenced, "lk_msdyn_postconfig_modifiedby", "modifiedby")]
			public const string lk_msdyn_postconfig_modifiedby = "lk_msdyn_postconfig_modifiedby";
			[Relationship("msdyn_postconfig", EntityRole.Referenced, "lk_msdyn_postconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_postconfig_modifiedonbehalfby = "lk_msdyn_postconfig_modifiedonbehalfby";
			[Relationship("msdyn_postruleconfig", EntityRole.Referenced, "lk_msdyn_postruleconfig_createdby", "createdby")]
			public const string lk_msdyn_postruleconfig_createdby = "lk_msdyn_postruleconfig_createdby";
			[Relationship("msdyn_postruleconfig", EntityRole.Referenced, "lk_msdyn_postruleconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_postruleconfig_createdonbehalfby = "lk_msdyn_postruleconfig_createdonbehalfby";
			[Relationship("msdyn_postruleconfig", EntityRole.Referenced, "lk_msdyn_postruleconfig_modifiedby", "modifiedby")]
			public const string lk_msdyn_postruleconfig_modifiedby = "lk_msdyn_postruleconfig_modifiedby";
			[Relationship("msdyn_postruleconfig", EntityRole.Referenced, "lk_msdyn_postruleconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_postruleconfig_modifiedonbehalfby = "lk_msdyn_postruleconfig_modifiedonbehalfby";
			[Relationship("msdyn_priority", EntityRole.Referenced, "lk_msdyn_priority_createdby", "createdby")]
			public const string lk_msdyn_priority_createdby = "lk_msdyn_priority_createdby";
			[Relationship("msdyn_priority", EntityRole.Referenced, "lk_msdyn_priority_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_priority_createdonbehalfby = "lk_msdyn_priority_createdonbehalfby";
			[Relationship("msdyn_priority", EntityRole.Referenced, "lk_msdyn_priority_modifiedby", "modifiedby")]
			public const string lk_msdyn_priority_modifiedby = "lk_msdyn_priority_modifiedby";
			[Relationship("msdyn_priority", EntityRole.Referenced, "lk_msdyn_priority_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_priority_modifiedonbehalfby = "lk_msdyn_priority_modifiedonbehalfby";
			[Relationship("msdyn_processnotes", EntityRole.Referenced, "lk_msdyn_processnotes_createdby", "createdby")]
			public const string lk_msdyn_processnotes_createdby = "lk_msdyn_processnotes_createdby";
			[Relationship("msdyn_processnotes", EntityRole.Referenced, "lk_msdyn_processnotes_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_processnotes_createdonbehalfby = "lk_msdyn_processnotes_createdonbehalfby";
			[Relationship("msdyn_processnotes", EntityRole.Referenced, "lk_msdyn_processnotes_modifiedby", "modifiedby")]
			public const string lk_msdyn_processnotes_modifiedby = "lk_msdyn_processnotes_modifiedby";
			[Relationship("msdyn_processnotes", EntityRole.Referenced, "lk_msdyn_processnotes_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_processnotes_modifiedonbehalfby = "lk_msdyn_processnotes_modifiedonbehalfby";
			[Relationship("msdyn_productinventory", EntityRole.Referenced, "lk_msdyn_productinventory_createdby", "createdby")]
			public const string lk_msdyn_productinventory_createdby = "lk_msdyn_productinventory_createdby";
			[Relationship("msdyn_productinventory", EntityRole.Referenced, "lk_msdyn_productinventory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_productinventory_createdonbehalfby = "lk_msdyn_productinventory_createdonbehalfby";
			[Relationship("msdyn_productinventory", EntityRole.Referenced, "lk_msdyn_productinventory_modifiedby", "modifiedby")]
			public const string lk_msdyn_productinventory_modifiedby = "lk_msdyn_productinventory_modifiedby";
			[Relationship("msdyn_productinventory", EntityRole.Referenced, "lk_msdyn_productinventory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_productinventory_modifiedonbehalfby = "lk_msdyn_productinventory_modifiedonbehalfby";
			[Relationship("msdyn_project", EntityRole.Referenced, "lk_msdyn_project_createdby", "createdby")]
			public const string lk_msdyn_project_createdby = "lk_msdyn_project_createdby";
			[Relationship("msdyn_project", EntityRole.Referenced, "lk_msdyn_project_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_project_createdonbehalfby = "lk_msdyn_project_createdonbehalfby";
			[Relationship("msdyn_project", EntityRole.Referenced, "lk_msdyn_project_modifiedby", "modifiedby")]
			public const string lk_msdyn_project_modifiedby = "lk_msdyn_project_modifiedby";
			[Relationship("msdyn_project", EntityRole.Referenced, "lk_msdyn_project_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_project_modifiedonbehalfby = "lk_msdyn_project_modifiedonbehalfby";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "lk_msdyn_projectapproval_createdby", "createdby")]
			public const string lk_msdyn_projectapproval_createdby = "lk_msdyn_projectapproval_createdby";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "lk_msdyn_projectapproval_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projectapproval_createdonbehalfby = "lk_msdyn_projectapproval_createdonbehalfby";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "lk_msdyn_projectapproval_modifiedby", "modifiedby")]
			public const string lk_msdyn_projectapproval_modifiedby = "lk_msdyn_projectapproval_modifiedby";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "lk_msdyn_projectapproval_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projectapproval_modifiedonbehalfby = "lk_msdyn_projectapproval_modifiedonbehalfby";
			[Relationship("msdyn_projectparameter", EntityRole.Referenced, "lk_msdyn_projectparameter_createdby", "createdby")]
			public const string lk_msdyn_projectparameter_createdby = "lk_msdyn_projectparameter_createdby";
			[Relationship("msdyn_projectparameter", EntityRole.Referenced, "lk_msdyn_projectparameter_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projectparameter_createdonbehalfby = "lk_msdyn_projectparameter_createdonbehalfby";
			[Relationship("msdyn_projectparameter", EntityRole.Referenced, "lk_msdyn_projectparameter_modifiedby", "modifiedby")]
			public const string lk_msdyn_projectparameter_modifiedby = "lk_msdyn_projectparameter_modifiedby";
			[Relationship("msdyn_projectparameter", EntityRole.Referenced, "lk_msdyn_projectparameter_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projectparameter_modifiedonbehalfby = "lk_msdyn_projectparameter_modifiedonbehalfby";
			[Relationship("msdyn_projectparameterpricelist", EntityRole.Referenced, "lk_msdyn_projectparameterpricelist_createdby", "createdby")]
			public const string lk_msdyn_projectparameterpricelist_createdby = "lk_msdyn_projectparameterpricelist_createdby";
			[Relationship("msdyn_projectparameterpricelist", EntityRole.Referenced, "lk_msdyn_projectparameterpricelist_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projectparameterpricelist_createdonbehalfby = "lk_msdyn_projectparameterpricelist_createdonbehalfby";
			[Relationship("msdyn_projectparameterpricelist", EntityRole.Referenced, "lk_msdyn_projectparameterpricelist_modifiedby", "modifiedby")]
			public const string lk_msdyn_projectparameterpricelist_modifiedby = "lk_msdyn_projectparameterpricelist_modifiedby";
			[Relationship("msdyn_projectparameterpricelist", EntityRole.Referenced, "lk_msdyn_projectparameterpricelist_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projectparameterpricelist_modifiedonbehalfby = "lk_msdyn_projectparameterpricelist_modifiedonbehalfby";
			[Relationship("msdyn_projectpricelist", EntityRole.Referenced, "lk_msdyn_projectpricelist_createdby", "createdby")]
			public const string lk_msdyn_projectpricelist_createdby = "lk_msdyn_projectpricelist_createdby";
			[Relationship("msdyn_projectpricelist", EntityRole.Referenced, "lk_msdyn_projectpricelist_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projectpricelist_createdonbehalfby = "lk_msdyn_projectpricelist_createdonbehalfby";
			[Relationship("msdyn_projectpricelist", EntityRole.Referenced, "lk_msdyn_projectpricelist_modifiedby", "modifiedby")]
			public const string lk_msdyn_projectpricelist_modifiedby = "lk_msdyn_projectpricelist_modifiedby";
			[Relationship("msdyn_projectpricelist", EntityRole.Referenced, "lk_msdyn_projectpricelist_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projectpricelist_modifiedonbehalfby = "lk_msdyn_projectpricelist_modifiedonbehalfby";
			[Relationship("msdyn_projecttask", EntityRole.Referenced, "lk_msdyn_projecttask_createdby", "createdby")]
			public const string lk_msdyn_projecttask_createdby = "lk_msdyn_projecttask_createdby";
			[Relationship("msdyn_projecttask", EntityRole.Referenced, "lk_msdyn_projecttask_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projecttask_createdonbehalfby = "lk_msdyn_projecttask_createdonbehalfby";
			[Relationship("msdyn_projecttask", EntityRole.Referenced, "lk_msdyn_projecttask_modifiedby", "modifiedby")]
			public const string lk_msdyn_projecttask_modifiedby = "lk_msdyn_projecttask_modifiedby";
			[Relationship("msdyn_projecttask", EntityRole.Referenced, "lk_msdyn_projecttask_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projecttask_modifiedonbehalfby = "lk_msdyn_projecttask_modifiedonbehalfby";
			[Relationship("msdyn_projecttaskdependency", EntityRole.Referenced, "lk_msdyn_projecttaskdependency_createdby", "createdby")]
			public const string lk_msdyn_projecttaskdependency_createdby = "lk_msdyn_projecttaskdependency_createdby";
			[Relationship("msdyn_projecttaskdependency", EntityRole.Referenced, "lk_msdyn_projecttaskdependency_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projecttaskdependency_createdonbehalfby = "lk_msdyn_projecttaskdependency_createdonbehalfby";
			[Relationship("msdyn_projecttaskdependency", EntityRole.Referenced, "lk_msdyn_projecttaskdependency_modifiedby", "modifiedby")]
			public const string lk_msdyn_projecttaskdependency_modifiedby = "lk_msdyn_projecttaskdependency_modifiedby";
			[Relationship("msdyn_projecttaskdependency", EntityRole.Referenced, "lk_msdyn_projecttaskdependency_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projecttaskdependency_modifiedonbehalfby = "lk_msdyn_projecttaskdependency_modifiedonbehalfby";
			[Relationship("msdyn_projecttaskstatususer", EntityRole.Referenced, "lk_msdyn_projecttaskstatususer_createdby", "createdby")]
			public const string lk_msdyn_projecttaskstatususer_createdby = "lk_msdyn_projecttaskstatususer_createdby";
			[Relationship("msdyn_projecttaskstatususer", EntityRole.Referenced, "lk_msdyn_projecttaskstatususer_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projecttaskstatususer_createdonbehalfby = "lk_msdyn_projecttaskstatususer_createdonbehalfby";
			[Relationship("msdyn_projecttaskstatususer", EntityRole.Referenced, "lk_msdyn_projecttaskstatususer_modifiedby", "modifiedby")]
			public const string lk_msdyn_projecttaskstatususer_modifiedby = "lk_msdyn_projecttaskstatususer_modifiedby";
			[Relationship("msdyn_projecttaskstatususer", EntityRole.Referenced, "lk_msdyn_projecttaskstatususer_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projecttaskstatususer_modifiedonbehalfby = "lk_msdyn_projecttaskstatususer_modifiedonbehalfby";
			[Relationship("msdyn_projectteam", EntityRole.Referenced, "lk_msdyn_projectteam_createdby", "createdby")]
			public const string lk_msdyn_projectteam_createdby = "lk_msdyn_projectteam_createdby";
			[Relationship("msdyn_projectteam", EntityRole.Referenced, "lk_msdyn_projectteam_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projectteam_createdonbehalfby = "lk_msdyn_projectteam_createdonbehalfby";
			[Relationship("msdyn_projectteam", EntityRole.Referenced, "lk_msdyn_projectteam_modifiedby", "modifiedby")]
			public const string lk_msdyn_projectteam_modifiedby = "lk_msdyn_projectteam_modifiedby";
			[Relationship("msdyn_projectteam", EntityRole.Referenced, "lk_msdyn_projectteam_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projectteam_modifiedonbehalfby = "lk_msdyn_projectteam_modifiedonbehalfby";
			[Relationship("msdyn_projectteammembersignup", EntityRole.Referenced, "lk_msdyn_projectteammembersignup_createdby", "createdby")]
			public const string lk_msdyn_projectteammembersignup_createdby = "lk_msdyn_projectteammembersignup_createdby";
			[Relationship("msdyn_projectteammembersignup", EntityRole.Referenced, "lk_msdyn_projectteammembersignup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projectteammembersignup_createdonbehalfby = "lk_msdyn_projectteammembersignup_createdonbehalfby";
			[Relationship("msdyn_projectteammembersignup", EntityRole.Referenced, "lk_msdyn_projectteammembersignup_modifiedby", "modifiedby")]
			public const string lk_msdyn_projectteammembersignup_modifiedby = "lk_msdyn_projectteammembersignup_modifiedby";
			[Relationship("msdyn_projectteammembersignup", EntityRole.Referenced, "lk_msdyn_projectteammembersignup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projectteammembersignup_modifiedonbehalfby = "lk_msdyn_projectteammembersignup_modifiedonbehalfby";
			[Relationship("msdyn_projecttransactioncategory", EntityRole.Referenced, "lk_msdyn_projecttransactioncategory_createdby", "createdby")]
			public const string lk_msdyn_projecttransactioncategory_createdby = "lk_msdyn_projecttransactioncategory_createdby";
			[Relationship("msdyn_projecttransactioncategory", EntityRole.Referenced, "lk_msdyn_projecttransactioncategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_projecttransactioncategory_createdonbehalfby = "lk_msdyn_projecttransactioncategory_createdonbehalfby";
			[Relationship("msdyn_projecttransactioncategory", EntityRole.Referenced, "lk_msdyn_projecttransactioncategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_projecttransactioncategory_modifiedby = "lk_msdyn_projecttransactioncategory_modifiedby";
			[Relationship("msdyn_projecttransactioncategory", EntityRole.Referenced, "lk_msdyn_projecttransactioncategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_projecttransactioncategory_modifiedonbehalfby = "lk_msdyn_projecttransactioncategory_modifiedonbehalfby";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "lk_msdyn_purchaseorder_createdby", "createdby")]
			public const string lk_msdyn_purchaseorder_createdby = "lk_msdyn_purchaseorder_createdby";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "lk_msdyn_purchaseorder_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_purchaseorder_createdonbehalfby = "lk_msdyn_purchaseorder_createdonbehalfby";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "lk_msdyn_purchaseorder_modifiedby", "modifiedby")]
			public const string lk_msdyn_purchaseorder_modifiedby = "lk_msdyn_purchaseorder_modifiedby";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "lk_msdyn_purchaseorder_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_purchaseorder_modifiedonbehalfby = "lk_msdyn_purchaseorder_modifiedonbehalfby";
			[Relationship("msdyn_purchaseorderbill", EntityRole.Referenced, "lk_msdyn_purchaseorderbill_createdby", "createdby")]
			public const string lk_msdyn_purchaseorderbill_createdby = "lk_msdyn_purchaseorderbill_createdby";
			[Relationship("msdyn_purchaseorderbill", EntityRole.Referenced, "lk_msdyn_purchaseorderbill_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_purchaseorderbill_createdonbehalfby = "lk_msdyn_purchaseorderbill_createdonbehalfby";
			[Relationship("msdyn_purchaseorderbill", EntityRole.Referenced, "lk_msdyn_purchaseorderbill_modifiedby", "modifiedby")]
			public const string lk_msdyn_purchaseorderbill_modifiedby = "lk_msdyn_purchaseorderbill_modifiedby";
			[Relationship("msdyn_purchaseorderbill", EntityRole.Referenced, "lk_msdyn_purchaseorderbill_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_purchaseorderbill_modifiedonbehalfby = "lk_msdyn_purchaseorderbill_modifiedonbehalfby";
			[Relationship("msdyn_purchaseorderproduct", EntityRole.Referenced, "lk_msdyn_purchaseorderproduct_createdby", "createdby")]
			public const string lk_msdyn_purchaseorderproduct_createdby = "lk_msdyn_purchaseorderproduct_createdby";
			[Relationship("msdyn_purchaseorderproduct", EntityRole.Referenced, "lk_msdyn_purchaseorderproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_purchaseorderproduct_createdonbehalfby = "lk_msdyn_purchaseorderproduct_createdonbehalfby";
			[Relationship("msdyn_purchaseorderproduct", EntityRole.Referenced, "lk_msdyn_purchaseorderproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_purchaseorderproduct_modifiedby = "lk_msdyn_purchaseorderproduct_modifiedby";
			[Relationship("msdyn_purchaseorderproduct", EntityRole.Referenced, "lk_msdyn_purchaseorderproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_purchaseorderproduct_modifiedonbehalfby = "lk_msdyn_purchaseorderproduct_modifiedonbehalfby";
			[Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "lk_msdyn_purchaseorderreceipt_createdby", "createdby")]
			public const string lk_msdyn_purchaseorderreceipt_createdby = "lk_msdyn_purchaseorderreceipt_createdby";
			[Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "lk_msdyn_purchaseorderreceipt_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_purchaseorderreceipt_createdonbehalfby = "lk_msdyn_purchaseorderreceipt_createdonbehalfby";
			[Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "lk_msdyn_purchaseorderreceipt_modifiedby", "modifiedby")]
			public const string lk_msdyn_purchaseorderreceipt_modifiedby = "lk_msdyn_purchaseorderreceipt_modifiedby";
			[Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "lk_msdyn_purchaseorderreceipt_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_purchaseorderreceipt_modifiedonbehalfby = "lk_msdyn_purchaseorderreceipt_modifiedonbehalfby";
			[Relationship("msdyn_purchaseorderreceiptproduct", EntityRole.Referenced, "lk_msdyn_purchaseorderreceiptproduct_createdby", "createdby")]
			public const string lk_msdyn_purchaseorderreceiptproduct_createdby = "lk_msdyn_purchaseorderreceiptproduct_createdby";
			[Relationship("msdyn_purchaseorderreceiptproduct", EntityRole.Referenced, "lk_msdyn_purchaseorderreceiptproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_purchaseorderreceiptproduct_createdonbehalfby = "lk_msdyn_purchaseorderreceiptproduct_createdonbehalfby";
			[Relationship("msdyn_purchaseorderreceiptproduct", EntityRole.Referenced, "lk_msdyn_purchaseorderreceiptproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_purchaseorderreceiptproduct_modifiedby = "lk_msdyn_purchaseorderreceiptproduct_modifiedby";
			[Relationship("msdyn_purchaseorderreceiptproduct", EntityRole.Referenced, "lk_msdyn_purchaseorderreceiptproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_purchaseorderreceiptproduct_modifiedonbehalfby = "lk_msdyn_purchaseorderreceiptproduct_modifiedonbehalfby";
			[Relationship("msdyn_purchaseordersubstatus", EntityRole.Referenced, "lk_msdyn_purchaseordersubstatus_createdby", "createdby")]
			public const string lk_msdyn_purchaseordersubstatus_createdby = "lk_msdyn_purchaseordersubstatus_createdby";
			[Relationship("msdyn_purchaseordersubstatus", EntityRole.Referenced, "lk_msdyn_purchaseordersubstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_purchaseordersubstatus_createdonbehalfby = "lk_msdyn_purchaseordersubstatus_createdonbehalfby";
			[Relationship("msdyn_purchaseordersubstatus", EntityRole.Referenced, "lk_msdyn_purchaseordersubstatus_modifiedby", "modifiedby")]
			public const string lk_msdyn_purchaseordersubstatus_modifiedby = "lk_msdyn_purchaseordersubstatus_modifiedby";
			[Relationship("msdyn_purchaseordersubstatus", EntityRole.Referenced, "lk_msdyn_purchaseordersubstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_purchaseordersubstatus_modifiedonbehalfby = "lk_msdyn_purchaseordersubstatus_modifiedonbehalfby";
			[Relationship("msdyn_question", EntityRole.Referenced, "lk_msdyn_question_createdby", "")]
			public const string lk_msdyn_question_createdby = "lk_msdyn_question_createdby";
			[Relationship("msdyn_question", EntityRole.Referenced, "lk_msdyn_question_createdonbehalfby", "")]
			public const string lk_msdyn_question_createdonbehalfby = "lk_msdyn_question_createdonbehalfby";
			[Relationship("msdyn_question", EntityRole.Referenced, "lk_msdyn_question_modifiedby", "")]
			public const string lk_msdyn_question_modifiedby = "lk_msdyn_question_modifiedby";
			[Relationship("msdyn_question", EntityRole.Referenced, "lk_msdyn_question_modifiedonbehalfby", "")]
			public const string lk_msdyn_question_modifiedonbehalfby = "lk_msdyn_question_modifiedonbehalfby";
			[Relationship("msdyn_questiongroup", EntityRole.Referenced, "lk_msdyn_questiongroup_createdby", "")]
			public const string lk_msdyn_questiongroup_createdby = "lk_msdyn_questiongroup_createdby";
			[Relationship("msdyn_questiongroup", EntityRole.Referenced, "lk_msdyn_questiongroup_createdonbehalfby", "")]
			public const string lk_msdyn_questiongroup_createdonbehalfby = "lk_msdyn_questiongroup_createdonbehalfby";
			[Relationship("msdyn_questiongroup", EntityRole.Referenced, "lk_msdyn_questiongroup_modifiedby", "")]
			public const string lk_msdyn_questiongroup_modifiedby = "lk_msdyn_questiongroup_modifiedby";
			[Relationship("msdyn_questiongroup", EntityRole.Referenced, "lk_msdyn_questiongroup_modifiedonbehalfby", "")]
			public const string lk_msdyn_questiongroup_modifiedonbehalfby = "lk_msdyn_questiongroup_modifiedonbehalfby";
			[Relationship("msdyn_questionresponse", EntityRole.Referenced, "lk_msdyn_questionresponse_createdby", "")]
			public const string lk_msdyn_questionresponse_createdby = "lk_msdyn_questionresponse_createdby";
			[Relationship("msdyn_questionresponse", EntityRole.Referenced, "lk_msdyn_questionresponse_createdonbehalfby", "")]
			public const string lk_msdyn_questionresponse_createdonbehalfby = "lk_msdyn_questionresponse_createdonbehalfby";
			[Relationship("msdyn_questionresponse", EntityRole.Referenced, "lk_msdyn_questionresponse_modifiedby", "")]
			public const string lk_msdyn_questionresponse_modifiedby = "lk_msdyn_questionresponse_modifiedby";
			[Relationship("msdyn_questionresponse", EntityRole.Referenced, "lk_msdyn_questionresponse_modifiedonbehalfby", "")]
			public const string lk_msdyn_questionresponse_modifiedonbehalfby = "lk_msdyn_questionresponse_modifiedonbehalfby";
			[Relationship("msdyn_questiontype", EntityRole.Referenced, "lk_msdyn_questiontype_createdby", "")]
			public const string lk_msdyn_questiontype_createdby = "lk_msdyn_questiontype_createdby";
			[Relationship("msdyn_questiontype", EntityRole.Referenced, "lk_msdyn_questiontype_createdonbehalfby", "")]
			public const string lk_msdyn_questiontype_createdonbehalfby = "lk_msdyn_questiontype_createdonbehalfby";
			[Relationship("msdyn_questiontype", EntityRole.Referenced, "lk_msdyn_questiontype_modifiedby", "")]
			public const string lk_msdyn_questiontype_modifiedby = "lk_msdyn_questiontype_modifiedby";
			[Relationship("msdyn_questiontype", EntityRole.Referenced, "lk_msdyn_questiontype_modifiedonbehalfby", "")]
			public const string lk_msdyn_questiontype_modifiedonbehalfby = "lk_msdyn_questiontype_modifiedonbehalfby";
			[Relationship("msdyn_quotebookingincident", EntityRole.Referenced, "lk_msdyn_quotebookingincident_createdby", "createdby")]
			public const string lk_msdyn_quotebookingincident_createdby = "lk_msdyn_quotebookingincident_createdby";
			[Relationship("msdyn_quotebookingincident", EntityRole.Referenced, "lk_msdyn_quotebookingincident_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotebookingincident_createdonbehalfby = "lk_msdyn_quotebookingincident_createdonbehalfby";
			[Relationship("msdyn_quotebookingincident", EntityRole.Referenced, "lk_msdyn_quotebookingincident_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotebookingincident_modifiedby = "lk_msdyn_quotebookingincident_modifiedby";
			[Relationship("msdyn_quotebookingincident", EntityRole.Referenced, "lk_msdyn_quotebookingincident_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotebookingincident_modifiedonbehalfby = "lk_msdyn_quotebookingincident_modifiedonbehalfby";
			[Relationship("msdyn_quotebookingproduct", EntityRole.Referenced, "lk_msdyn_quotebookingproduct_createdby", "createdby")]
			public const string lk_msdyn_quotebookingproduct_createdby = "lk_msdyn_quotebookingproduct_createdby";
			[Relationship("msdyn_quotebookingproduct", EntityRole.Referenced, "lk_msdyn_quotebookingproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotebookingproduct_createdonbehalfby = "lk_msdyn_quotebookingproduct_createdonbehalfby";
			[Relationship("msdyn_quotebookingproduct", EntityRole.Referenced, "lk_msdyn_quotebookingproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotebookingproduct_modifiedby = "lk_msdyn_quotebookingproduct_modifiedby";
			[Relationship("msdyn_quotebookingproduct", EntityRole.Referenced, "lk_msdyn_quotebookingproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotebookingproduct_modifiedonbehalfby = "lk_msdyn_quotebookingproduct_modifiedonbehalfby";
			[Relationship("msdyn_quotebookingservice", EntityRole.Referenced, "lk_msdyn_quotebookingservice_createdby", "createdby")]
			public const string lk_msdyn_quotebookingservice_createdby = "lk_msdyn_quotebookingservice_createdby";
			[Relationship("msdyn_quotebookingservice", EntityRole.Referenced, "lk_msdyn_quotebookingservice_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotebookingservice_createdonbehalfby = "lk_msdyn_quotebookingservice_createdonbehalfby";
			[Relationship("msdyn_quotebookingservice", EntityRole.Referenced, "lk_msdyn_quotebookingservice_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotebookingservice_modifiedby = "lk_msdyn_quotebookingservice_modifiedby";
			[Relationship("msdyn_quotebookingservice", EntityRole.Referenced, "lk_msdyn_quotebookingservice_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotebookingservice_modifiedonbehalfby = "lk_msdyn_quotebookingservice_modifiedonbehalfby";
			[Relationship("msdyn_quotebookingservicetask", EntityRole.Referenced, "lk_msdyn_quotebookingservicetask_createdby", "createdby")]
			public const string lk_msdyn_quotebookingservicetask_createdby = "lk_msdyn_quotebookingservicetask_createdby";
			[Relationship("msdyn_quotebookingservicetask", EntityRole.Referenced, "lk_msdyn_quotebookingservicetask_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotebookingservicetask_createdonbehalfby = "lk_msdyn_quotebookingservicetask_createdonbehalfby";
			[Relationship("msdyn_quotebookingservicetask", EntityRole.Referenced, "lk_msdyn_quotebookingservicetask_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotebookingservicetask_modifiedby = "lk_msdyn_quotebookingservicetask_modifiedby";
			[Relationship("msdyn_quotebookingservicetask", EntityRole.Referenced, "lk_msdyn_quotebookingservicetask_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotebookingservicetask_modifiedonbehalfby = "lk_msdyn_quotebookingservicetask_modifiedonbehalfby";
			[Relationship("msdyn_quotebookingsetup", EntityRole.Referenced, "lk_msdyn_quotebookingsetup_createdby", "createdby")]
			public const string lk_msdyn_quotebookingsetup_createdby = "lk_msdyn_quotebookingsetup_createdby";
			[Relationship("msdyn_quotebookingsetup", EntityRole.Referenced, "lk_msdyn_quotebookingsetup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotebookingsetup_createdonbehalfby = "lk_msdyn_quotebookingsetup_createdonbehalfby";
			[Relationship("msdyn_quotebookingsetup", EntityRole.Referenced, "lk_msdyn_quotebookingsetup_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotebookingsetup_modifiedby = "lk_msdyn_quotebookingsetup_modifiedby";
			[Relationship("msdyn_quotebookingsetup", EntityRole.Referenced, "lk_msdyn_quotebookingsetup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotebookingsetup_modifiedonbehalfby = "lk_msdyn_quotebookingsetup_modifiedonbehalfby";
			[Relationship("msdyn_quoteinvoicingproduct", EntityRole.Referenced, "lk_msdyn_quoteinvoicingproduct_createdby", "createdby")]
			public const string lk_msdyn_quoteinvoicingproduct_createdby = "lk_msdyn_quoteinvoicingproduct_createdby";
			[Relationship("msdyn_quoteinvoicingproduct", EntityRole.Referenced, "lk_msdyn_quoteinvoicingproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quoteinvoicingproduct_createdonbehalfby = "lk_msdyn_quoteinvoicingproduct_createdonbehalfby";
			[Relationship("msdyn_quoteinvoicingproduct", EntityRole.Referenced, "lk_msdyn_quoteinvoicingproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_quoteinvoicingproduct_modifiedby = "lk_msdyn_quoteinvoicingproduct_modifiedby";
			[Relationship("msdyn_quoteinvoicingproduct", EntityRole.Referenced, "lk_msdyn_quoteinvoicingproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quoteinvoicingproduct_modifiedonbehalfby = "lk_msdyn_quoteinvoicingproduct_modifiedonbehalfby";
			[Relationship("msdyn_quoteinvoicingsetup", EntityRole.Referenced, "lk_msdyn_quoteinvoicingsetup_createdby", "createdby")]
			public const string lk_msdyn_quoteinvoicingsetup_createdby = "lk_msdyn_quoteinvoicingsetup_createdby";
			[Relationship("msdyn_quoteinvoicingsetup", EntityRole.Referenced, "lk_msdyn_quoteinvoicingsetup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quoteinvoicingsetup_createdonbehalfby = "lk_msdyn_quoteinvoicingsetup_createdonbehalfby";
			[Relationship("msdyn_quoteinvoicingsetup", EntityRole.Referenced, "lk_msdyn_quoteinvoicingsetup_modifiedby", "modifiedby")]
			public const string lk_msdyn_quoteinvoicingsetup_modifiedby = "lk_msdyn_quoteinvoicingsetup_modifiedby";
			[Relationship("msdyn_quoteinvoicingsetup", EntityRole.Referenced, "lk_msdyn_quoteinvoicingsetup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quoteinvoicingsetup_modifiedonbehalfby = "lk_msdyn_quoteinvoicingsetup_modifiedonbehalfby";
			[Relationship("msdyn_quotelineanalyticsbreakdown", EntityRole.Referenced, "lk_msdyn_quotelineanalyticsbreakdown_createdby", "createdby")]
			public const string lk_msdyn_quotelineanalyticsbreakdown_createdby = "lk_msdyn_quotelineanalyticsbreakdown_createdby";
			[Relationship("msdyn_quotelineanalyticsbreakdown", EntityRole.Referenced, "lk_msdyn_quotelineanalyticsbreakdown_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotelineanalyticsbreakdown_createdonbehalfby = "lk_msdyn_quotelineanalyticsbreakdown_createdonbehalfby";
			[Relationship("msdyn_quotelineanalyticsbreakdown", EntityRole.Referenced, "lk_msdyn_quotelineanalyticsbreakdown_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotelineanalyticsbreakdown_modifiedby = "lk_msdyn_quotelineanalyticsbreakdown_modifiedby";
			[Relationship("msdyn_quotelineanalyticsbreakdown", EntityRole.Referenced, "lk_msdyn_quotelineanalyticsbreakdown_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotelineanalyticsbreakdown_modifiedonbehalfby = "lk_msdyn_quotelineanalyticsbreakdown_modifiedonbehalfby";
			[Relationship("msdyn_quotelineinvoiceschedule", EntityRole.Referenced, "lk_msdyn_quotelineinvoiceschedule_createdby", "createdby")]
			public const string lk_msdyn_quotelineinvoiceschedule_createdby = "lk_msdyn_quotelineinvoiceschedule_createdby";
			[Relationship("msdyn_quotelineinvoiceschedule", EntityRole.Referenced, "lk_msdyn_quotelineinvoiceschedule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotelineinvoiceschedule_createdonbehalfby = "lk_msdyn_quotelineinvoiceschedule_createdonbehalfby";
			[Relationship("msdyn_quotelineinvoiceschedule", EntityRole.Referenced, "lk_msdyn_quotelineinvoiceschedule_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotelineinvoiceschedule_modifiedby = "lk_msdyn_quotelineinvoiceschedule_modifiedby";
			[Relationship("msdyn_quotelineinvoiceschedule", EntityRole.Referenced, "lk_msdyn_quotelineinvoiceschedule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotelineinvoiceschedule_modifiedonbehalfby = "lk_msdyn_quotelineinvoiceschedule_modifiedonbehalfby";
			[Relationship("msdyn_quotelineresourcecategory", EntityRole.Referenced, "lk_msdyn_quotelineresourcecategory_createdby", "createdby")]
			public const string lk_msdyn_quotelineresourcecategory_createdby = "lk_msdyn_quotelineresourcecategory_createdby";
			[Relationship("msdyn_quotelineresourcecategory", EntityRole.Referenced, "lk_msdyn_quotelineresourcecategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotelineresourcecategory_createdonbehalfby = "lk_msdyn_quotelineresourcecategory_createdonbehalfby";
			[Relationship("msdyn_quotelineresourcecategory", EntityRole.Referenced, "lk_msdyn_quotelineresourcecategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotelineresourcecategory_modifiedby = "lk_msdyn_quotelineresourcecategory_modifiedby";
			[Relationship("msdyn_quotelineresourcecategory", EntityRole.Referenced, "lk_msdyn_quotelineresourcecategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotelineresourcecategory_modifiedonbehalfby = "lk_msdyn_quotelineresourcecategory_modifiedonbehalfby";
			[Relationship("msdyn_quotelinescheduleofvalue", EntityRole.Referenced, "lk_msdyn_quotelinescheduleofvalue_createdby", "createdby")]
			public const string lk_msdyn_quotelinescheduleofvalue_createdby = "lk_msdyn_quotelinescheduleofvalue_createdby";
			[Relationship("msdyn_quotelinescheduleofvalue", EntityRole.Referenced, "lk_msdyn_quotelinescheduleofvalue_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotelinescheduleofvalue_createdonbehalfby = "lk_msdyn_quotelinescheduleofvalue_createdonbehalfby";
			[Relationship("msdyn_quotelinescheduleofvalue", EntityRole.Referenced, "lk_msdyn_quotelinescheduleofvalue_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotelinescheduleofvalue_modifiedby = "lk_msdyn_quotelinescheduleofvalue_modifiedby";
			[Relationship("msdyn_quotelinescheduleofvalue", EntityRole.Referenced, "lk_msdyn_quotelinescheduleofvalue_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotelinescheduleofvalue_modifiedonbehalfby = "lk_msdyn_quotelinescheduleofvalue_modifiedonbehalfby";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "lk_msdyn_quotelinetransaction_createdby", "createdby")]
			public const string lk_msdyn_quotelinetransaction_createdby = "lk_msdyn_quotelinetransaction_createdby";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "lk_msdyn_quotelinetransaction_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotelinetransaction_createdonbehalfby = "lk_msdyn_quotelinetransaction_createdonbehalfby";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "lk_msdyn_quotelinetransaction_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotelinetransaction_modifiedby = "lk_msdyn_quotelinetransaction_modifiedby";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "lk_msdyn_quotelinetransaction_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotelinetransaction_modifiedonbehalfby = "lk_msdyn_quotelinetransaction_modifiedonbehalfby";
			[Relationship("msdyn_quotelinetransactioncategory", EntityRole.Referenced, "lk_msdyn_quotelinetransactioncategory_createdby", "createdby")]
			public const string lk_msdyn_quotelinetransactioncategory_createdby = "lk_msdyn_quotelinetransactioncategory_createdby";
			[Relationship("msdyn_quotelinetransactioncategory", EntityRole.Referenced, "lk_msdyn_quotelinetransactioncategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotelinetransactioncategory_createdonbehalfby = "lk_msdyn_quotelinetransactioncategory_createdonbehalfby";
			[Relationship("msdyn_quotelinetransactioncategory", EntityRole.Referenced, "lk_msdyn_quotelinetransactioncategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotelinetransactioncategory_modifiedby = "lk_msdyn_quotelinetransactioncategory_modifiedby";
			[Relationship("msdyn_quotelinetransactioncategory", EntityRole.Referenced, "lk_msdyn_quotelinetransactioncategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotelinetransactioncategory_modifiedonbehalfby = "lk_msdyn_quotelinetransactioncategory_modifiedonbehalfby";
			[Relationship("msdyn_quotelinetransactionclassification", EntityRole.Referenced, "lk_msdyn_quotelinetransactionclassification_createdby", "createdby")]
			public const string lk_msdyn_quotelinetransactionclassification_createdby = "lk_msdyn_quotelinetransactionclassification_createdby";
			[Relationship("msdyn_quotelinetransactionclassification", EntityRole.Referenced, "lk_msdyn_quotelinetransactionclassification_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotelinetransactionclassification_createdonbehalfby = "lk_msdyn_quotelinetransactionclassification_createdonbehalfby";
			[Relationship("msdyn_quotelinetransactionclassification", EntityRole.Referenced, "lk_msdyn_quotelinetransactionclassification_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotelinetransactionclassification_modifiedby = "lk_msdyn_quotelinetransactionclassification_modifiedby";
			[Relationship("msdyn_quotelinetransactionclassification", EntityRole.Referenced, "lk_msdyn_quotelinetransactionclassification_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotelinetransactionclassification_modifiedonbehalfby = "lk_msdyn_quotelinetransactionclassification_modifiedonbehalfby";
			[Relationship("msdyn_quotepricelist", EntityRole.Referenced, "lk_msdyn_quotepricelist_createdby", "createdby")]
			public const string lk_msdyn_quotepricelist_createdby = "lk_msdyn_quotepricelist_createdby";
			[Relationship("msdyn_quotepricelist", EntityRole.Referenced, "lk_msdyn_quotepricelist_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_quotepricelist_createdonbehalfby = "lk_msdyn_quotepricelist_createdonbehalfby";
			[Relationship("msdyn_quotepricelist", EntityRole.Referenced, "lk_msdyn_quotepricelist_modifiedby", "modifiedby")]
			public const string lk_msdyn_quotepricelist_modifiedby = "lk_msdyn_quotepricelist_modifiedby";
			[Relationship("msdyn_quotepricelist", EntityRole.Referenced, "lk_msdyn_quotepricelist_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_quotepricelist_modifiedonbehalfby = "lk_msdyn_quotepricelist_modifiedonbehalfby";
			[Relationship("msdyn_relationshipinsightsunifiedconfig", EntityRole.Referenced, "lk_msdyn_relationshipinsightsunifiedconfig_createdby", "createdby")]
			public const string lk_msdyn_relationshipinsightsunifiedconfig_createdby = "lk_msdyn_relationshipinsightsunifiedconfig_createdby";
			[Relationship("msdyn_relationshipinsightsunifiedconfig", EntityRole.Referenced, "lk_msdyn_relationshipinsightsunifiedconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_relationshipinsightsunifiedconfig_createdonbehalfby = "lk_msdyn_relationshipinsightsunifiedconfig_createdonbehalfby";
			[Relationship("msdyn_relationshipinsightsunifiedconfig", EntityRole.Referenced, "lk_msdyn_relationshipinsightsunifiedconfig_modifiedby", "modifiedby")]
			public const string lk_msdyn_relationshipinsightsunifiedconfig_modifiedby = "lk_msdyn_relationshipinsightsunifiedconfig_modifiedby";
			[Relationship("msdyn_relationshipinsightsunifiedconfig", EntityRole.Referenced, "lk_msdyn_relationshipinsightsunifiedconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_relationshipinsightsunifiedconfig_modifiedonbehalfby = "lk_msdyn_relationshipinsightsunifiedconfig_modifiedonbehalfby";
			[Relationship("msdyn_requirementcharacteristic", EntityRole.Referenced, "lk_msdyn_requirementcharacteristic_createdby", "createdby")]
			public const string lk_msdyn_requirementcharacteristic_createdby = "lk_msdyn_requirementcharacteristic_createdby";
			[Relationship("msdyn_requirementcharacteristic", EntityRole.Referenced, "lk_msdyn_requirementcharacteristic_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_requirementcharacteristic_createdonbehalfby = "lk_msdyn_requirementcharacteristic_createdonbehalfby";
			[Relationship("msdyn_requirementcharacteristic", EntityRole.Referenced, "lk_msdyn_requirementcharacteristic_modifiedby", "modifiedby")]
			public const string lk_msdyn_requirementcharacteristic_modifiedby = "lk_msdyn_requirementcharacteristic_modifiedby";
			[Relationship("msdyn_requirementcharacteristic", EntityRole.Referenced, "lk_msdyn_requirementcharacteristic_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_requirementcharacteristic_modifiedonbehalfby = "lk_msdyn_requirementcharacteristic_modifiedonbehalfby";
			[Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "lk_msdyn_requirementorganizationunit_createdby", "createdby")]
			public const string lk_msdyn_requirementorganizationunit_createdby = "lk_msdyn_requirementorganizationunit_createdby";
			[Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "lk_msdyn_requirementorganizationunit_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_requirementorganizationunit_createdonbehalfby = "lk_msdyn_requirementorganizationunit_createdonbehalfby";
			[Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "lk_msdyn_requirementorganizationunit_modifiedby", "modifiedby")]
			public const string lk_msdyn_requirementorganizationunit_modifiedby = "lk_msdyn_requirementorganizationunit_modifiedby";
			[Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "lk_msdyn_requirementorganizationunit_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_requirementorganizationunit_modifiedonbehalfby = "lk_msdyn_requirementorganizationunit_modifiedonbehalfby";
			[Relationship("msdyn_requirementresourcecategory", EntityRole.Referenced, "lk_msdyn_requirementresourcecategory_createdby", "createdby")]
			public const string lk_msdyn_requirementresourcecategory_createdby = "lk_msdyn_requirementresourcecategory_createdby";
			[Relationship("msdyn_requirementresourcecategory", EntityRole.Referenced, "lk_msdyn_requirementresourcecategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_requirementresourcecategory_createdonbehalfby = "lk_msdyn_requirementresourcecategory_createdonbehalfby";
			[Relationship("msdyn_requirementresourcecategory", EntityRole.Referenced, "lk_msdyn_requirementresourcecategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_requirementresourcecategory_modifiedby = "lk_msdyn_requirementresourcecategory_modifiedby";
			[Relationship("msdyn_requirementresourcecategory", EntityRole.Referenced, "lk_msdyn_requirementresourcecategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_requirementresourcecategory_modifiedonbehalfby = "lk_msdyn_requirementresourcecategory_modifiedonbehalfby";
			[Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "lk_msdyn_requirementresourcepreference_createdby", "createdby")]
			public const string lk_msdyn_requirementresourcepreference_createdby = "lk_msdyn_requirementresourcepreference_createdby";
			[Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "lk_msdyn_requirementresourcepreference_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_requirementresourcepreference_createdonbehalfby = "lk_msdyn_requirementresourcepreference_createdonbehalfby";
			[Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "lk_msdyn_requirementresourcepreference_modifiedby", "modifiedby")]
			public const string lk_msdyn_requirementresourcepreference_modifiedby = "lk_msdyn_requirementresourcepreference_modifiedby";
			[Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "lk_msdyn_requirementresourcepreference_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_requirementresourcepreference_modifiedonbehalfby = "lk_msdyn_requirementresourcepreference_modifiedonbehalfby";
			[Relationship("msdyn_requirementstatus", EntityRole.Referenced, "lk_msdyn_requirementstatus_createdby", "createdby")]
			public const string lk_msdyn_requirementstatus_createdby = "lk_msdyn_requirementstatus_createdby";
			[Relationship("msdyn_requirementstatus", EntityRole.Referenced, "lk_msdyn_requirementstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_requirementstatus_createdonbehalfby = "lk_msdyn_requirementstatus_createdonbehalfby";
			[Relationship("msdyn_requirementstatus", EntityRole.Referenced, "lk_msdyn_requirementstatus_modifiedby", "modifiedby")]
			public const string lk_msdyn_requirementstatus_modifiedby = "lk_msdyn_requirementstatus_modifiedby";
			[Relationship("msdyn_requirementstatus", EntityRole.Referenced, "lk_msdyn_requirementstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_requirementstatus_modifiedonbehalfby = "lk_msdyn_requirementstatus_modifiedonbehalfby";
			[Relationship("msdyn_resourceassignment", EntityRole.Referenced, "lk_msdyn_resourceassignment_createdby", "createdby")]
			public const string lk_msdyn_resourceassignment_createdby = "lk_msdyn_resourceassignment_createdby";
			[Relationship("msdyn_resourceassignment", EntityRole.Referenced, "lk_msdyn_resourceassignment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_resourceassignment_createdonbehalfby = "lk_msdyn_resourceassignment_createdonbehalfby";
			[Relationship("msdyn_resourceassignment", EntityRole.Referenced, "lk_msdyn_resourceassignment_modifiedby", "modifiedby")]
			public const string lk_msdyn_resourceassignment_modifiedby = "lk_msdyn_resourceassignment_modifiedby";
			[Relationship("msdyn_resourceassignment", EntityRole.Referenced, "lk_msdyn_resourceassignment_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_resourceassignment_modifiedonbehalfby = "lk_msdyn_resourceassignment_modifiedonbehalfby";
			[Relationship("msdyn_resourceassignmentdetail", EntityRole.Referenced, "lk_msdyn_resourceassignmentdetail_createdby", "createdby")]
			public const string lk_msdyn_resourceassignmentdetail_createdby = "lk_msdyn_resourceassignmentdetail_createdby";
			[Relationship("msdyn_resourceassignmentdetail", EntityRole.Referenced, "lk_msdyn_resourceassignmentdetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_resourceassignmentdetail_createdonbehalfby = "lk_msdyn_resourceassignmentdetail_createdonbehalfby";
			[Relationship("msdyn_resourceassignmentdetail", EntityRole.Referenced, "lk_msdyn_resourceassignmentdetail_modifiedby", "modifiedby")]
			public const string lk_msdyn_resourceassignmentdetail_modifiedby = "lk_msdyn_resourceassignmentdetail_modifiedby";
			[Relationship("msdyn_resourceassignmentdetail", EntityRole.Referenced, "lk_msdyn_resourceassignmentdetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_resourceassignmentdetail_modifiedonbehalfby = "lk_msdyn_resourceassignmentdetail_modifiedonbehalfby";
			[Relationship("msdyn_resourcecategorypricelevel", EntityRole.Referenced, "lk_msdyn_resourcecategorypricelevel_createdby", "createdby")]
			public const string lk_msdyn_resourcecategorypricelevel_createdby = "lk_msdyn_resourcecategorypricelevel_createdby";
			[Relationship("msdyn_resourcecategorypricelevel", EntityRole.Referenced, "lk_msdyn_resourcecategorypricelevel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_resourcecategorypricelevel_createdonbehalfby = "lk_msdyn_resourcecategorypricelevel_createdonbehalfby";
			[Relationship("msdyn_resourcecategorypricelevel", EntityRole.Referenced, "lk_msdyn_resourcecategorypricelevel_modifiedby", "modifiedby")]
			public const string lk_msdyn_resourcecategorypricelevel_modifiedby = "lk_msdyn_resourcecategorypricelevel_modifiedby";
			[Relationship("msdyn_resourcecategorypricelevel", EntityRole.Referenced, "lk_msdyn_resourcecategorypricelevel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_resourcecategorypricelevel_modifiedonbehalfby = "lk_msdyn_resourcecategorypricelevel_modifiedonbehalfby";
			[Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "lk_msdyn_resourcepaytype_createdby", "createdby")]
			public const string lk_msdyn_resourcepaytype_createdby = "lk_msdyn_resourcepaytype_createdby";
			[Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "lk_msdyn_resourcepaytype_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_resourcepaytype_createdonbehalfby = "lk_msdyn_resourcepaytype_createdonbehalfby";
			[Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "lk_msdyn_resourcepaytype_modifiedby", "modifiedby")]
			public const string lk_msdyn_resourcepaytype_modifiedby = "lk_msdyn_resourcepaytype_modifiedby";
			[Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "lk_msdyn_resourcepaytype_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_resourcepaytype_modifiedonbehalfby = "lk_msdyn_resourcepaytype_modifiedonbehalfby";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "lk_msdyn_resourcerequest_createdby", "createdby")]
			public const string lk_msdyn_resourcerequest_createdby = "lk_msdyn_resourcerequest_createdby";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "lk_msdyn_resourcerequest_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_resourcerequest_createdonbehalfby = "lk_msdyn_resourcerequest_createdonbehalfby";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "lk_msdyn_resourcerequest_modifiedby", "modifiedby")]
			public const string lk_msdyn_resourcerequest_modifiedby = "lk_msdyn_resourcerequest_modifiedby";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "lk_msdyn_resourcerequest_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_resourcerequest_modifiedonbehalfby = "lk_msdyn_resourcerequest_modifiedonbehalfby";
			[Relationship("msdyn_resourcerequirement", EntityRole.Referenced, "lk_msdyn_resourcerequirement_createdby", "createdby")]
			public const string lk_msdyn_resourcerequirement_createdby = "lk_msdyn_resourcerequirement_createdby";
			[Relationship("msdyn_resourcerequirement", EntityRole.Referenced, "lk_msdyn_resourcerequirement_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_resourcerequirement_createdonbehalfby = "lk_msdyn_resourcerequirement_createdonbehalfby";
			[Relationship("msdyn_resourcerequirement", EntityRole.Referenced, "lk_msdyn_resourcerequirement_modifiedby", "modifiedby")]
			public const string lk_msdyn_resourcerequirement_modifiedby = "lk_msdyn_resourcerequirement_modifiedby";
			[Relationship("msdyn_resourcerequirement", EntityRole.Referenced, "lk_msdyn_resourcerequirement_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_resourcerequirement_modifiedonbehalfby = "lk_msdyn_resourcerequirement_modifiedonbehalfby";
			[Relationship("msdyn_resourcerequirementdetail", EntityRole.Referenced, "lk_msdyn_resourcerequirementdetail_createdby", "createdby")]
			public const string lk_msdyn_resourcerequirementdetail_createdby = "lk_msdyn_resourcerequirementdetail_createdby";
			[Relationship("msdyn_resourcerequirementdetail", EntityRole.Referenced, "lk_msdyn_resourcerequirementdetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_resourcerequirementdetail_createdonbehalfby = "lk_msdyn_resourcerequirementdetail_createdonbehalfby";
			[Relationship("msdyn_resourcerequirementdetail", EntityRole.Referenced, "lk_msdyn_resourcerequirementdetail_modifiedby", "modifiedby")]
			public const string lk_msdyn_resourcerequirementdetail_modifiedby = "lk_msdyn_resourcerequirementdetail_modifiedby";
			[Relationship("msdyn_resourcerequirementdetail", EntityRole.Referenced, "lk_msdyn_resourcerequirementdetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_resourcerequirementdetail_modifiedonbehalfby = "lk_msdyn_resourcerequirementdetail_modifiedonbehalfby";
			[Relationship("msdyn_resourceterritory", EntityRole.Referenced, "lk_msdyn_resourceterritory_createdby", "createdby")]
			public const string lk_msdyn_resourceterritory_createdby = "lk_msdyn_resourceterritory_createdby";
			[Relationship("msdyn_resourceterritory", EntityRole.Referenced, "lk_msdyn_resourceterritory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_resourceterritory_createdonbehalfby = "lk_msdyn_resourceterritory_createdonbehalfby";
			[Relationship("msdyn_resourceterritory", EntityRole.Referenced, "lk_msdyn_resourceterritory_modifiedby", "modifiedby")]
			public const string lk_msdyn_resourceterritory_modifiedby = "lk_msdyn_resourceterritory_modifiedby";
			[Relationship("msdyn_resourceterritory", EntityRole.Referenced, "lk_msdyn_resourceterritory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_resourceterritory_modifiedonbehalfby = "lk_msdyn_resourceterritory_modifiedonbehalfby";
			[Relationship("msdyn_responseaction", EntityRole.Referenced, "lk_msdyn_responseaction_createdby", "")]
			public const string lk_msdyn_responseaction_createdby = "lk_msdyn_responseaction_createdby";
			[Relationship("msdyn_responseaction", EntityRole.Referenced, "lk_msdyn_responseaction_createdonbehalfby", "")]
			public const string lk_msdyn_responseaction_createdonbehalfby = "lk_msdyn_responseaction_createdonbehalfby";
			[Relationship("msdyn_responseaction", EntityRole.Referenced, "lk_msdyn_responseaction_modifiedby", "")]
			public const string lk_msdyn_responseaction_modifiedby = "lk_msdyn_responseaction_modifiedby";
			[Relationship("msdyn_responseaction", EntityRole.Referenced, "lk_msdyn_responseaction_modifiedonbehalfby", "")]
			public const string lk_msdyn_responseaction_modifiedonbehalfby = "lk_msdyn_responseaction_modifiedonbehalfby";
			[Relationship("msdyn_responseblobstore", EntityRole.Referenced, "lk_msdyn_responseblobstore_createdby", "")]
			public const string lk_msdyn_responseblobstore_createdby = "lk_msdyn_responseblobstore_createdby";
			[Relationship("msdyn_responseblobstore", EntityRole.Referenced, "lk_msdyn_responseblobstore_createdonbehalfby", "")]
			public const string lk_msdyn_responseblobstore_createdonbehalfby = "lk_msdyn_responseblobstore_createdonbehalfby";
			[Relationship("msdyn_responseblobstore", EntityRole.Referenced, "lk_msdyn_responseblobstore_modifiedby", "")]
			public const string lk_msdyn_responseblobstore_modifiedby = "lk_msdyn_responseblobstore_modifiedby";
			[Relationship("msdyn_responseblobstore", EntityRole.Referenced, "lk_msdyn_responseblobstore_modifiedonbehalfby", "")]
			public const string lk_msdyn_responseblobstore_modifiedonbehalfby = "lk_msdyn_responseblobstore_modifiedonbehalfby";
			[Relationship("msdyn_responsecondition", EntityRole.Referenced, "lk_msdyn_responsecondition_createdby", "")]
			public const string lk_msdyn_responsecondition_createdby = "lk_msdyn_responsecondition_createdby";
			[Relationship("msdyn_responsecondition", EntityRole.Referenced, "lk_msdyn_responsecondition_createdonbehalfby", "")]
			public const string lk_msdyn_responsecondition_createdonbehalfby = "lk_msdyn_responsecondition_createdonbehalfby";
			[Relationship("msdyn_responsecondition", EntityRole.Referenced, "lk_msdyn_responsecondition_modifiedby", "")]
			public const string lk_msdyn_responsecondition_modifiedby = "lk_msdyn_responsecondition_modifiedby";
			[Relationship("msdyn_responsecondition", EntityRole.Referenced, "lk_msdyn_responsecondition_modifiedonbehalfby", "")]
			public const string lk_msdyn_responsecondition_modifiedonbehalfby = "lk_msdyn_responsecondition_modifiedonbehalfby";
			[Relationship("msdyn_responseerror", EntityRole.Referenced, "lk_msdyn_responseerror_createdby", "")]
			public const string lk_msdyn_responseerror_createdby = "lk_msdyn_responseerror_createdby";
			[Relationship("msdyn_responseerror", EntityRole.Referenced, "lk_msdyn_responseerror_createdonbehalfby", "")]
			public const string lk_msdyn_responseerror_createdonbehalfby = "lk_msdyn_responseerror_createdonbehalfby";
			[Relationship("msdyn_responseerror", EntityRole.Referenced, "lk_msdyn_responseerror_modifiedby", "")]
			public const string lk_msdyn_responseerror_modifiedby = "lk_msdyn_responseerror_modifiedby";
			[Relationship("msdyn_responseerror", EntityRole.Referenced, "lk_msdyn_responseerror_modifiedonbehalfby", "")]
			public const string lk_msdyn_responseerror_modifiedonbehalfby = "lk_msdyn_responseerror_modifiedonbehalfby";
			[Relationship("msdyn_responseoutcome", EntityRole.Referenced, "lk_msdyn_responseoutcome_createdby", "")]
			public const string lk_msdyn_responseoutcome_createdby = "lk_msdyn_responseoutcome_createdby";
			[Relationship("msdyn_responseoutcome", EntityRole.Referenced, "lk_msdyn_responseoutcome_createdonbehalfby", "")]
			public const string lk_msdyn_responseoutcome_createdonbehalfby = "lk_msdyn_responseoutcome_createdonbehalfby";
			[Relationship("msdyn_responseoutcome", EntityRole.Referenced, "lk_msdyn_responseoutcome_modifiedby", "")]
			public const string lk_msdyn_responseoutcome_modifiedby = "lk_msdyn_responseoutcome_modifiedby";
			[Relationship("msdyn_responseoutcome", EntityRole.Referenced, "lk_msdyn_responseoutcome_modifiedonbehalfby", "")]
			public const string lk_msdyn_responseoutcome_modifiedonbehalfby = "lk_msdyn_responseoutcome_modifiedonbehalfby";
			[Relationship("msdyn_responserouting", EntityRole.Referenced, "lk_msdyn_responserouting_createdby", "")]
			public const string lk_msdyn_responserouting_createdby = "lk_msdyn_responserouting_createdby";
			[Relationship("msdyn_responserouting", EntityRole.Referenced, "lk_msdyn_responserouting_createdonbehalfby", "")]
			public const string lk_msdyn_responserouting_createdonbehalfby = "lk_msdyn_responserouting_createdonbehalfby";
			[Relationship("msdyn_responserouting", EntityRole.Referenced, "lk_msdyn_responserouting_modifiedby", "")]
			public const string lk_msdyn_responserouting_modifiedby = "lk_msdyn_responserouting_modifiedby";
			[Relationship("msdyn_responserouting", EntityRole.Referenced, "lk_msdyn_responserouting_modifiedonbehalfby", "")]
			public const string lk_msdyn_responserouting_modifiedonbehalfby = "lk_msdyn_responserouting_modifiedonbehalfby";
			[Relationship("msdyn_rma", EntityRole.Referenced, "lk_msdyn_rma_createdby", "createdby")]
			public const string lk_msdyn_rma_createdby = "lk_msdyn_rma_createdby";
			[Relationship("msdyn_rma", EntityRole.Referenced, "lk_msdyn_rma_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rma_createdonbehalfby = "lk_msdyn_rma_createdonbehalfby";
			[Relationship("msdyn_rma", EntityRole.Referenced, "lk_msdyn_rma_modifiedby", "modifiedby")]
			public const string lk_msdyn_rma_modifiedby = "lk_msdyn_rma_modifiedby";
			[Relationship("msdyn_rma", EntityRole.Referenced, "lk_msdyn_rma_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rma_modifiedonbehalfby = "lk_msdyn_rma_modifiedonbehalfby";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "lk_msdyn_rmaproduct_createdby", "createdby")]
			public const string lk_msdyn_rmaproduct_createdby = "lk_msdyn_rmaproduct_createdby";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "lk_msdyn_rmaproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rmaproduct_createdonbehalfby = "lk_msdyn_rmaproduct_createdonbehalfby";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "lk_msdyn_rmaproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_rmaproduct_modifiedby = "lk_msdyn_rmaproduct_modifiedby";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "lk_msdyn_rmaproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rmaproduct_modifiedonbehalfby = "lk_msdyn_rmaproduct_modifiedonbehalfby";
			[Relationship("msdyn_rmareceipt", EntityRole.Referenced, "lk_msdyn_rmareceipt_createdby", "createdby")]
			public const string lk_msdyn_rmareceipt_createdby = "lk_msdyn_rmareceipt_createdby";
			[Relationship("msdyn_rmareceipt", EntityRole.Referenced, "lk_msdyn_rmareceipt_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rmareceipt_createdonbehalfby = "lk_msdyn_rmareceipt_createdonbehalfby";
			[Relationship("msdyn_rmareceipt", EntityRole.Referenced, "lk_msdyn_rmareceipt_modifiedby", "modifiedby")]
			public const string lk_msdyn_rmareceipt_modifiedby = "lk_msdyn_rmareceipt_modifiedby";
			[Relationship("msdyn_rmareceipt", EntityRole.Referenced, "lk_msdyn_rmareceipt_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rmareceipt_modifiedonbehalfby = "lk_msdyn_rmareceipt_modifiedonbehalfby";
			[Relationship("msdyn_rmareceiptproduct", EntityRole.Referenced, "lk_msdyn_rmareceiptproduct_createdby", "createdby")]
			public const string lk_msdyn_rmareceiptproduct_createdby = "lk_msdyn_rmareceiptproduct_createdby";
			[Relationship("msdyn_rmareceiptproduct", EntityRole.Referenced, "lk_msdyn_rmareceiptproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rmareceiptproduct_createdonbehalfby = "lk_msdyn_rmareceiptproduct_createdonbehalfby";
			[Relationship("msdyn_rmareceiptproduct", EntityRole.Referenced, "lk_msdyn_rmareceiptproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_rmareceiptproduct_modifiedby = "lk_msdyn_rmareceiptproduct_modifiedby";
			[Relationship("msdyn_rmareceiptproduct", EntityRole.Referenced, "lk_msdyn_rmareceiptproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rmareceiptproduct_modifiedonbehalfby = "lk_msdyn_rmareceiptproduct_modifiedonbehalfby";
			[Relationship("msdyn_rmasubstatus", EntityRole.Referenced, "lk_msdyn_rmasubstatus_createdby", "createdby")]
			public const string lk_msdyn_rmasubstatus_createdby = "lk_msdyn_rmasubstatus_createdby";
			[Relationship("msdyn_rmasubstatus", EntityRole.Referenced, "lk_msdyn_rmasubstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rmasubstatus_createdonbehalfby = "lk_msdyn_rmasubstatus_createdonbehalfby";
			[Relationship("msdyn_rmasubstatus", EntityRole.Referenced, "lk_msdyn_rmasubstatus_modifiedby", "modifiedby")]
			public const string lk_msdyn_rmasubstatus_modifiedby = "lk_msdyn_rmasubstatus_modifiedby";
			[Relationship("msdyn_rmasubstatus", EntityRole.Referenced, "lk_msdyn_rmasubstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rmasubstatus_modifiedonbehalfby = "lk_msdyn_rmasubstatus_modifiedonbehalfby";
			[Relationship("msdyn_rolecompetencyrequirement", EntityRole.Referenced, "lk_msdyn_rolecompetencyrequirement_createdby", "createdby")]
			public const string lk_msdyn_rolecompetencyrequirement_createdby = "lk_msdyn_rolecompetencyrequirement_createdby";
			[Relationship("msdyn_rolecompetencyrequirement", EntityRole.Referenced, "lk_msdyn_rolecompetencyrequirement_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rolecompetencyrequirement_createdonbehalfby = "lk_msdyn_rolecompetencyrequirement_createdonbehalfby";
			[Relationship("msdyn_rolecompetencyrequirement", EntityRole.Referenced, "lk_msdyn_rolecompetencyrequirement_modifiedby", "modifiedby")]
			public const string lk_msdyn_rolecompetencyrequirement_modifiedby = "lk_msdyn_rolecompetencyrequirement_modifiedby";
			[Relationship("msdyn_rolecompetencyrequirement", EntityRole.Referenced, "lk_msdyn_rolecompetencyrequirement_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rolecompetencyrequirement_modifiedonbehalfby = "lk_msdyn_rolecompetencyrequirement_modifiedonbehalfby";
			[Relationship("msdyn_roleutilization", EntityRole.Referenced, "lk_msdyn_roleutilization_createdby", "createdby")]
			public const string lk_msdyn_roleutilization_createdby = "lk_msdyn_roleutilization_createdby";
			[Relationship("msdyn_roleutilization", EntityRole.Referenced, "lk_msdyn_roleutilization_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_roleutilization_createdonbehalfby = "lk_msdyn_roleutilization_createdonbehalfby";
			[Relationship("msdyn_roleutilization", EntityRole.Referenced, "lk_msdyn_roleutilization_modifiedby", "modifiedby")]
			public const string lk_msdyn_roleutilization_modifiedby = "lk_msdyn_roleutilization_modifiedby";
			[Relationship("msdyn_roleutilization", EntityRole.Referenced, "lk_msdyn_roleutilization_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_roleutilization_modifiedonbehalfby = "lk_msdyn_roleutilization_modifiedonbehalfby";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "lk_msdyn_rtv_createdby", "createdby")]
			public const string lk_msdyn_rtv_createdby = "lk_msdyn_rtv_createdby";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "lk_msdyn_rtv_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rtv_createdonbehalfby = "lk_msdyn_rtv_createdonbehalfby";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "lk_msdyn_rtv_modifiedby", "modifiedby")]
			public const string lk_msdyn_rtv_modifiedby = "lk_msdyn_rtv_modifiedby";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "lk_msdyn_rtv_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rtv_modifiedonbehalfby = "lk_msdyn_rtv_modifiedonbehalfby";
			[Relationship("msdyn_rtvproduct", EntityRole.Referenced, "lk_msdyn_rtvproduct_createdby", "createdby")]
			public const string lk_msdyn_rtvproduct_createdby = "lk_msdyn_rtvproduct_createdby";
			[Relationship("msdyn_rtvproduct", EntityRole.Referenced, "lk_msdyn_rtvproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rtvproduct_createdonbehalfby = "lk_msdyn_rtvproduct_createdonbehalfby";
			[Relationship("msdyn_rtvproduct", EntityRole.Referenced, "lk_msdyn_rtvproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_rtvproduct_modifiedby = "lk_msdyn_rtvproduct_modifiedby";
			[Relationship("msdyn_rtvproduct", EntityRole.Referenced, "lk_msdyn_rtvproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rtvproduct_modifiedonbehalfby = "lk_msdyn_rtvproduct_modifiedonbehalfby";
			[Relationship("msdyn_rtvsubstatus", EntityRole.Referenced, "lk_msdyn_rtvsubstatus_createdby", "createdby")]
			public const string lk_msdyn_rtvsubstatus_createdby = "lk_msdyn_rtvsubstatus_createdby";
			[Relationship("msdyn_rtvsubstatus", EntityRole.Referenced, "lk_msdyn_rtvsubstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_rtvsubstatus_createdonbehalfby = "lk_msdyn_rtvsubstatus_createdonbehalfby";
			[Relationship("msdyn_rtvsubstatus", EntityRole.Referenced, "lk_msdyn_rtvsubstatus_modifiedby", "modifiedby")]
			public const string lk_msdyn_rtvsubstatus_modifiedby = "lk_msdyn_rtvsubstatus_modifiedby";
			[Relationship("msdyn_rtvsubstatus", EntityRole.Referenced, "lk_msdyn_rtvsubstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_rtvsubstatus_modifiedonbehalfby = "lk_msdyn_rtvsubstatus_modifiedonbehalfby";
			[Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "lk_msdyn_scheduleboardsetting_createdby", "createdby")]
			public const string lk_msdyn_scheduleboardsetting_createdby = "lk_msdyn_scheduleboardsetting_createdby";
			[Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "lk_msdyn_scheduleboardsetting_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_scheduleboardsetting_createdonbehalfby = "lk_msdyn_scheduleboardsetting_createdonbehalfby";
			[Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "lk_msdyn_scheduleboardsetting_modifiedby", "modifiedby")]
			public const string lk_msdyn_scheduleboardsetting_modifiedby = "lk_msdyn_scheduleboardsetting_modifiedby";
			[Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "lk_msdyn_scheduleboardsetting_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_scheduleboardsetting_modifiedonbehalfby = "lk_msdyn_scheduleboardsetting_modifiedonbehalfby";
			[Relationship("msdyn_schedulingparameter", EntityRole.Referenced, "lk_msdyn_schedulingparameter_createdby", "createdby")]
			public const string lk_msdyn_schedulingparameter_createdby = "lk_msdyn_schedulingparameter_createdby";
			[Relationship("msdyn_schedulingparameter", EntityRole.Referenced, "lk_msdyn_schedulingparameter_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_schedulingparameter_createdonbehalfby = "lk_msdyn_schedulingparameter_createdonbehalfby";
			[Relationship("msdyn_schedulingparameter", EntityRole.Referenced, "lk_msdyn_schedulingparameter_modifiedby", "modifiedby")]
			public const string lk_msdyn_schedulingparameter_modifiedby = "lk_msdyn_schedulingparameter_modifiedby";
			[Relationship("msdyn_schedulingparameter", EntityRole.Referenced, "lk_msdyn_schedulingparameter_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_schedulingparameter_modifiedonbehalfby = "lk_msdyn_schedulingparameter_modifiedonbehalfby";
			[Relationship("msdyn_section", EntityRole.Referenced, "lk_msdyn_section_createdby", "")]
			public const string lk_msdyn_section_createdby = "lk_msdyn_section_createdby";
			[Relationship("msdyn_section", EntityRole.Referenced, "lk_msdyn_section_createdonbehalfby", "")]
			public const string lk_msdyn_section_createdonbehalfby = "lk_msdyn_section_createdonbehalfby";
			[Relationship("msdyn_section", EntityRole.Referenced, "lk_msdyn_section_modifiedby", "")]
			public const string lk_msdyn_section_modifiedby = "lk_msdyn_section_modifiedby";
			[Relationship("msdyn_section", EntityRole.Referenced, "lk_msdyn_section_modifiedonbehalfby", "")]
			public const string lk_msdyn_section_modifiedonbehalfby = "lk_msdyn_section_modifiedonbehalfby";
			[Relationship("msdyn_servicetasktype", EntityRole.Referenced, "lk_msdyn_servicetasktype_createdby", "createdby")]
			public const string lk_msdyn_servicetasktype_createdby = "lk_msdyn_servicetasktype_createdby";
			[Relationship("msdyn_servicetasktype", EntityRole.Referenced, "lk_msdyn_servicetasktype_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_servicetasktype_createdonbehalfby = "lk_msdyn_servicetasktype_createdonbehalfby";
			[Relationship("msdyn_servicetasktype", EntityRole.Referenced, "lk_msdyn_servicetasktype_modifiedby", "modifiedby")]
			public const string lk_msdyn_servicetasktype_modifiedby = "lk_msdyn_servicetasktype_modifiedby";
			[Relationship("msdyn_servicetasktype", EntityRole.Referenced, "lk_msdyn_servicetasktype_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_servicetasktype_modifiedonbehalfby = "lk_msdyn_servicetasktype_modifiedonbehalfby";
			[Relationship("msdyn_shipvia", EntityRole.Referenced, "lk_msdyn_shipvia_createdby", "createdby")]
			public const string lk_msdyn_shipvia_createdby = "lk_msdyn_shipvia_createdby";
			[Relationship("msdyn_shipvia", EntityRole.Referenced, "lk_msdyn_shipvia_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_shipvia_createdonbehalfby = "lk_msdyn_shipvia_createdonbehalfby";
			[Relationship("msdyn_shipvia", EntityRole.Referenced, "lk_msdyn_shipvia_modifiedby", "modifiedby")]
			public const string lk_msdyn_shipvia_modifiedby = "lk_msdyn_shipvia_modifiedby";
			[Relationship("msdyn_shipvia", EntityRole.Referenced, "lk_msdyn_shipvia_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_shipvia_modifiedonbehalfby = "lk_msdyn_shipvia_modifiedonbehalfby";
			[Relationship("msdyn_siconfig", EntityRole.Referenced, "lk_msdyn_siconfig_createdby", "createdby")]
			public const string lk_msdyn_siconfig_createdby = "lk_msdyn_siconfig_createdby";
			[Relationship("msdyn_siconfig", EntityRole.Referenced, "lk_msdyn_siconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_siconfig_createdonbehalfby = "lk_msdyn_siconfig_createdonbehalfby";
			[Relationship("msdyn_siconfig", EntityRole.Referenced, "lk_msdyn_siconfig_modifiedby", "modifiedby")]
			public const string lk_msdyn_siconfig_modifiedby = "lk_msdyn_siconfig_modifiedby";
			[Relationship("msdyn_siconfig", EntityRole.Referenced, "lk_msdyn_siconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_siconfig_modifiedonbehalfby = "lk_msdyn_siconfig_modifiedonbehalfby";
			[Relationship("msdyn_survey", EntityRole.Referenced, "lk_msdyn_survey_createdby", "")]
			public const string lk_msdyn_survey_createdby = "lk_msdyn_survey_createdby";
			[Relationship("msdyn_survey", EntityRole.Referenced, "lk_msdyn_survey_createdonbehalfby", "")]
			public const string lk_msdyn_survey_createdonbehalfby = "lk_msdyn_survey_createdonbehalfby";
			[Relationship("msdyn_survey", EntityRole.Referenced, "lk_msdyn_survey_modifiedby", "")]
			public const string lk_msdyn_survey_modifiedby = "lk_msdyn_survey_modifiedby";
			[Relationship("msdyn_survey", EntityRole.Referenced, "lk_msdyn_survey_modifiedonbehalfby", "")]
			public const string lk_msdyn_survey_modifiedonbehalfby = "lk_msdyn_survey_modifiedonbehalfby";
			[Relationship("msdyn_surveylog", EntityRole.Referenced, "lk_msdyn_surveylog_createdby", "")]
			public const string lk_msdyn_surveylog_createdby = "lk_msdyn_surveylog_createdby";
			[Relationship("msdyn_surveylog", EntityRole.Referenced, "lk_msdyn_surveylog_createdonbehalfby", "")]
			public const string lk_msdyn_surveylog_createdonbehalfby = "lk_msdyn_surveylog_createdonbehalfby";
			[Relationship("msdyn_surveylog", EntityRole.Referenced, "lk_msdyn_surveylog_modifiedby", "")]
			public const string lk_msdyn_surveylog_modifiedby = "lk_msdyn_surveylog_modifiedby";
			[Relationship("msdyn_surveylog", EntityRole.Referenced, "lk_msdyn_surveylog_modifiedonbehalfby", "")]
			public const string lk_msdyn_surveylog_modifiedonbehalfby = "lk_msdyn_surveylog_modifiedonbehalfby";
			[Relationship("msdyn_surveyresponse", EntityRole.Referenced, "lk_msdyn_surveyresponse_createdby", "")]
			public const string lk_msdyn_surveyresponse_createdby = "lk_msdyn_surveyresponse_createdby";
			[Relationship("msdyn_surveyresponse", EntityRole.Referenced, "lk_msdyn_surveyresponse_createdonbehalfby", "")]
			public const string lk_msdyn_surveyresponse_createdonbehalfby = "lk_msdyn_surveyresponse_createdonbehalfby";
			[Relationship("msdyn_surveyresponse", EntityRole.Referenced, "lk_msdyn_surveyresponse_modifiedby", "")]
			public const string lk_msdyn_surveyresponse_modifiedby = "lk_msdyn_surveyresponse_modifiedby";
			[Relationship("msdyn_surveyresponse", EntityRole.Referenced, "lk_msdyn_surveyresponse_modifiedonbehalfby", "")]
			public const string lk_msdyn_surveyresponse_modifiedonbehalfby = "lk_msdyn_surveyresponse_modifiedonbehalfby";
			[Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "lk_msdyn_systemuserschedulersetting_createdby", "createdby")]
			public const string lk_msdyn_systemuserschedulersetting_createdby = "lk_msdyn_systemuserschedulersetting_createdby";
			[Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "lk_msdyn_systemuserschedulersetting_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_systemuserschedulersetting_createdonbehalfby = "lk_msdyn_systemuserschedulersetting_createdonbehalfby";
			[Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "lk_msdyn_systemuserschedulersetting_modifiedby", "modifiedby")]
			public const string lk_msdyn_systemuserschedulersetting_modifiedby = "lk_msdyn_systemuserschedulersetting_modifiedby";
			[Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "lk_msdyn_systemuserschedulersetting_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_systemuserschedulersetting_modifiedonbehalfby = "lk_msdyn_systemuserschedulersetting_modifiedonbehalfby";
			[Relationship("msdyn_taxcode", EntityRole.Referenced, "lk_msdyn_taxcode_createdby", "createdby")]
			public const string lk_msdyn_taxcode_createdby = "lk_msdyn_taxcode_createdby";
			[Relationship("msdyn_taxcode", EntityRole.Referenced, "lk_msdyn_taxcode_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_taxcode_createdonbehalfby = "lk_msdyn_taxcode_createdonbehalfby";
			[Relationship("msdyn_taxcode", EntityRole.Referenced, "lk_msdyn_taxcode_modifiedby", "modifiedby")]
			public const string lk_msdyn_taxcode_modifiedby = "lk_msdyn_taxcode_modifiedby";
			[Relationship("msdyn_taxcode", EntityRole.Referenced, "lk_msdyn_taxcode_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_taxcode_modifiedonbehalfby = "lk_msdyn_taxcode_modifiedonbehalfby";
			[Relationship("msdyn_taxcodedetail", EntityRole.Referenced, "lk_msdyn_taxcodedetail_createdby", "createdby")]
			public const string lk_msdyn_taxcodedetail_createdby = "lk_msdyn_taxcodedetail_createdby";
			[Relationship("msdyn_taxcodedetail", EntityRole.Referenced, "lk_msdyn_taxcodedetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_taxcodedetail_createdonbehalfby = "lk_msdyn_taxcodedetail_createdonbehalfby";
			[Relationship("msdyn_taxcodedetail", EntityRole.Referenced, "lk_msdyn_taxcodedetail_modifiedby", "modifiedby")]
			public const string lk_msdyn_taxcodedetail_modifiedby = "lk_msdyn_taxcodedetail_modifiedby";
			[Relationship("msdyn_taxcodedetail", EntityRole.Referenced, "lk_msdyn_taxcodedetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_taxcodedetail_modifiedonbehalfby = "lk_msdyn_taxcodedetail_modifiedonbehalfby";
			[Relationship("msdyn_teamscollaboration", EntityRole.Referenced, "lk_msdyn_teamscollaboration_createdby", "createdby")]
			public const string lk_msdyn_teamscollaboration_createdby = "lk_msdyn_teamscollaboration_createdby";
			[Relationship("msdyn_teamscollaboration", EntityRole.Referenced, "lk_msdyn_teamscollaboration_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_teamscollaboration_createdonbehalfby = "lk_msdyn_teamscollaboration_createdonbehalfby";
			[Relationship("msdyn_teamscollaboration", EntityRole.Referenced, "lk_msdyn_teamscollaboration_modifiedby", "modifiedby")]
			public const string lk_msdyn_teamscollaboration_modifiedby = "lk_msdyn_teamscollaboration_modifiedby";
			[Relationship("msdyn_teamscollaboration", EntityRole.Referenced, "lk_msdyn_teamscollaboration_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_teamscollaboration_modifiedonbehalfby = "lk_msdyn_teamscollaboration_modifiedonbehalfby";
			[Relationship("msdyn_theme", EntityRole.Referenced, "lk_msdyn_theme_createdby", "")]
			public const string lk_msdyn_theme_createdby = "lk_msdyn_theme_createdby";
			[Relationship("msdyn_theme", EntityRole.Referenced, "lk_msdyn_theme_createdonbehalfby", "")]
			public const string lk_msdyn_theme_createdonbehalfby = "lk_msdyn_theme_createdonbehalfby";
			[Relationship("msdyn_theme", EntityRole.Referenced, "lk_msdyn_theme_modifiedby", "")]
			public const string lk_msdyn_theme_modifiedby = "lk_msdyn_theme_modifiedby";
			[Relationship("msdyn_theme", EntityRole.Referenced, "lk_msdyn_theme_modifiedonbehalfby", "")]
			public const string lk_msdyn_theme_modifiedonbehalfby = "lk_msdyn_theme_modifiedonbehalfby";
			[Relationship("msdyn_timeentry", EntityRole.Referenced, "lk_msdyn_timeentry_createdby", "createdby")]
			public const string lk_msdyn_timeentry_createdby = "lk_msdyn_timeentry_createdby";
			[Relationship("msdyn_timeentry", EntityRole.Referenced, "lk_msdyn_timeentry_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_timeentry_createdonbehalfby = "lk_msdyn_timeentry_createdonbehalfby";
			[Relationship("msdyn_timeentry", EntityRole.Referenced, "lk_msdyn_timeentry_modifiedby", "modifiedby")]
			public const string lk_msdyn_timeentry_modifiedby = "lk_msdyn_timeentry_modifiedby";
			[Relationship("msdyn_timeentry", EntityRole.Referenced, "lk_msdyn_timeentry_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_timeentry_modifiedonbehalfby = "lk_msdyn_timeentry_modifiedonbehalfby";
			[Relationship("msdyn_timegroup", EntityRole.Referenced, "lk_msdyn_timegroup_createdby", "createdby")]
			public const string lk_msdyn_timegroup_createdby = "lk_msdyn_timegroup_createdby";
			[Relationship("msdyn_timegroup", EntityRole.Referenced, "lk_msdyn_timegroup_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_timegroup_createdonbehalfby = "lk_msdyn_timegroup_createdonbehalfby";
			[Relationship("msdyn_timegroup", EntityRole.Referenced, "lk_msdyn_timegroup_modifiedby", "modifiedby")]
			public const string lk_msdyn_timegroup_modifiedby = "lk_msdyn_timegroup_modifiedby";
			[Relationship("msdyn_timegroup", EntityRole.Referenced, "lk_msdyn_timegroup_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_timegroup_modifiedonbehalfby = "lk_msdyn_timegroup_modifiedonbehalfby";
			[Relationship("msdyn_timegroupdetail", EntityRole.Referenced, "lk_msdyn_timegroupdetail_createdby", "createdby")]
			public const string lk_msdyn_timegroupdetail_createdby = "lk_msdyn_timegroupdetail_createdby";
			[Relationship("msdyn_timegroupdetail", EntityRole.Referenced, "lk_msdyn_timegroupdetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_timegroupdetail_createdonbehalfby = "lk_msdyn_timegroupdetail_createdonbehalfby";
			[Relationship("msdyn_timegroupdetail", EntityRole.Referenced, "lk_msdyn_timegroupdetail_modifiedby", "modifiedby")]
			public const string lk_msdyn_timegroupdetail_modifiedby = "lk_msdyn_timegroupdetail_modifiedby";
			[Relationship("msdyn_timegroupdetail", EntityRole.Referenced, "lk_msdyn_timegroupdetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_timegroupdetail_modifiedonbehalfby = "lk_msdyn_timegroupdetail_modifiedonbehalfby";
			[Relationship("msdyn_timeoffcalendar", EntityRole.Referenced, "lk_msdyn_timeoffcalendar_createdby", "createdby")]
			public const string lk_msdyn_timeoffcalendar_createdby = "lk_msdyn_timeoffcalendar_createdby";
			[Relationship("msdyn_timeoffcalendar", EntityRole.Referenced, "lk_msdyn_timeoffcalendar_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_timeoffcalendar_createdonbehalfby = "lk_msdyn_timeoffcalendar_createdonbehalfby";
			[Relationship("msdyn_timeoffcalendar", EntityRole.Referenced, "lk_msdyn_timeoffcalendar_modifiedby", "modifiedby")]
			public const string lk_msdyn_timeoffcalendar_modifiedby = "lk_msdyn_timeoffcalendar_modifiedby";
			[Relationship("msdyn_timeoffcalendar", EntityRole.Referenced, "lk_msdyn_timeoffcalendar_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_timeoffcalendar_modifiedonbehalfby = "lk_msdyn_timeoffcalendar_modifiedonbehalfby";
			[Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "lk_msdyn_timeoffrequest_createdby", "createdby")]
			public const string lk_msdyn_timeoffrequest_createdby = "lk_msdyn_timeoffrequest_createdby";
			[Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "lk_msdyn_timeoffrequest_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_timeoffrequest_createdonbehalfby = "lk_msdyn_timeoffrequest_createdonbehalfby";
			[Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "lk_msdyn_timeoffrequest_modifiedby", "modifiedby")]
			public const string lk_msdyn_timeoffrequest_modifiedby = "lk_msdyn_timeoffrequest_modifiedby";
			[Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "lk_msdyn_timeoffrequest_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_timeoffrequest_modifiedonbehalfby = "lk_msdyn_timeoffrequest_modifiedonbehalfby";
			[Relationship("msdyn_transactioncategory", EntityRole.Referenced, "lk_msdyn_transactioncategory_createdby", "createdby")]
			public const string lk_msdyn_transactioncategory_createdby = "lk_msdyn_transactioncategory_createdby";
			[Relationship("msdyn_transactioncategory", EntityRole.Referenced, "lk_msdyn_transactioncategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_transactioncategory_createdonbehalfby = "lk_msdyn_transactioncategory_createdonbehalfby";
			[Relationship("msdyn_transactioncategory", EntityRole.Referenced, "lk_msdyn_transactioncategory_modifiedby", "modifiedby")]
			public const string lk_msdyn_transactioncategory_modifiedby = "lk_msdyn_transactioncategory_modifiedby";
			[Relationship("msdyn_transactioncategory", EntityRole.Referenced, "lk_msdyn_transactioncategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_transactioncategory_modifiedonbehalfby = "lk_msdyn_transactioncategory_modifiedonbehalfby";
			[Relationship("msdyn_transactioncategoryclassification", EntityRole.Referenced, "lk_msdyn_transactioncategoryclassification_createdby", "createdby")]
			public const string lk_msdyn_transactioncategoryclassification_createdby = "lk_msdyn_transactioncategoryclassification_createdby";
			[Relationship("msdyn_transactioncategoryclassification", EntityRole.Referenced, "lk_msdyn_transactioncategoryclassification_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_transactioncategoryclassification_createdonbehalfby = "lk_msdyn_transactioncategoryclassification_createdonbehalfby";
			[Relationship("msdyn_transactioncategoryclassification", EntityRole.Referenced, "lk_msdyn_transactioncategoryclassification_modifiedby", "modifiedby")]
			public const string lk_msdyn_transactioncategoryclassification_modifiedby = "lk_msdyn_transactioncategoryclassification_modifiedby";
			[Relationship("msdyn_transactioncategoryclassification", EntityRole.Referenced, "lk_msdyn_transactioncategoryclassification_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_transactioncategoryclassification_modifiedonbehalfby = "lk_msdyn_transactioncategoryclassification_modifiedonbehalfby";
			[Relationship("msdyn_transactioncategoryhierarchyelement", EntityRole.Referenced, "lk_msdyn_transactioncategoryhierarchyelement_createdby", "createdby")]
			public const string lk_msdyn_transactioncategoryhierarchyelement_createdby = "lk_msdyn_transactioncategoryhierarchyelement_createdby";
			[Relationship("msdyn_transactioncategoryhierarchyelement", EntityRole.Referenced, "lk_msdyn_transactioncategoryhierarchyelement_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_transactioncategoryhierarchyelement_createdonbehalfby = "lk_msdyn_transactioncategoryhierarchyelement_createdonbehalfby";
			[Relationship("msdyn_transactioncategoryhierarchyelement", EntityRole.Referenced, "lk_msdyn_transactioncategoryhierarchyelement_modifiedby", "modifiedby")]
			public const string lk_msdyn_transactioncategoryhierarchyelement_modifiedby = "lk_msdyn_transactioncategoryhierarchyelement_modifiedby";
			[Relationship("msdyn_transactioncategoryhierarchyelement", EntityRole.Referenced, "lk_msdyn_transactioncategoryhierarchyelement_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_transactioncategoryhierarchyelement_modifiedonbehalfby = "lk_msdyn_transactioncategoryhierarchyelement_modifiedonbehalfby";
			[Relationship("msdyn_transactioncategorypricelevel", EntityRole.Referenced, "lk_msdyn_transactioncategorypricelevel_createdby", "createdby")]
			public const string lk_msdyn_transactioncategorypricelevel_createdby = "lk_msdyn_transactioncategorypricelevel_createdby";
			[Relationship("msdyn_transactioncategorypricelevel", EntityRole.Referenced, "lk_msdyn_transactioncategorypricelevel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_transactioncategorypricelevel_createdonbehalfby = "lk_msdyn_transactioncategorypricelevel_createdonbehalfby";
			[Relationship("msdyn_transactioncategorypricelevel", EntityRole.Referenced, "lk_msdyn_transactioncategorypricelevel_modifiedby", "modifiedby")]
			public const string lk_msdyn_transactioncategorypricelevel_modifiedby = "lk_msdyn_transactioncategorypricelevel_modifiedby";
			[Relationship("msdyn_transactioncategorypricelevel", EntityRole.Referenced, "lk_msdyn_transactioncategorypricelevel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_transactioncategorypricelevel_modifiedonbehalfby = "lk_msdyn_transactioncategorypricelevel_modifiedonbehalfby";
			[Relationship("msdyn_transactionconnection", EntityRole.Referenced, "lk_msdyn_transactionconnection_createdby", "createdby")]
			public const string lk_msdyn_transactionconnection_createdby = "lk_msdyn_transactionconnection_createdby";
			[Relationship("msdyn_transactionconnection", EntityRole.Referenced, "lk_msdyn_transactionconnection_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_transactionconnection_createdonbehalfby = "lk_msdyn_transactionconnection_createdonbehalfby";
			[Relationship("msdyn_transactionconnection", EntityRole.Referenced, "lk_msdyn_transactionconnection_modifiedby", "modifiedby")]
			public const string lk_msdyn_transactionconnection_modifiedby = "lk_msdyn_transactionconnection_modifiedby";
			[Relationship("msdyn_transactionconnection", EntityRole.Referenced, "lk_msdyn_transactionconnection_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_transactionconnection_modifiedonbehalfby = "lk_msdyn_transactionconnection_modifiedonbehalfby";
			[Relationship("msdyn_transactionorigin", EntityRole.Referenced, "lk_msdyn_transactionorigin_createdby", "createdby")]
			public const string lk_msdyn_transactionorigin_createdby = "lk_msdyn_transactionorigin_createdby";
			[Relationship("msdyn_transactionorigin", EntityRole.Referenced, "lk_msdyn_transactionorigin_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_transactionorigin_createdonbehalfby = "lk_msdyn_transactionorigin_createdonbehalfby";
			[Relationship("msdyn_transactionorigin", EntityRole.Referenced, "lk_msdyn_transactionorigin_modifiedby", "modifiedby")]
			public const string lk_msdyn_transactionorigin_modifiedby = "lk_msdyn_transactionorigin_modifiedby";
			[Relationship("msdyn_transactionorigin", EntityRole.Referenced, "lk_msdyn_transactionorigin_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_transactionorigin_modifiedonbehalfby = "lk_msdyn_transactionorigin_modifiedonbehalfby";
			[Relationship("msdyn_transactiontype", EntityRole.Referenced, "lk_msdyn_transactiontype_createdby", "createdby")]
			public const string lk_msdyn_transactiontype_createdby = "lk_msdyn_transactiontype_createdby";
			[Relationship("msdyn_transactiontype", EntityRole.Referenced, "lk_msdyn_transactiontype_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_transactiontype_createdonbehalfby = "lk_msdyn_transactiontype_createdonbehalfby";
			[Relationship("msdyn_transactiontype", EntityRole.Referenced, "lk_msdyn_transactiontype_modifiedby", "modifiedby")]
			public const string lk_msdyn_transactiontype_modifiedby = "lk_msdyn_transactiontype_modifiedby";
			[Relationship("msdyn_transactiontype", EntityRole.Referenced, "lk_msdyn_transactiontype_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_transactiontype_modifiedonbehalfby = "lk_msdyn_transactiontype_modifiedonbehalfby";
			[Relationship("msdyn_uniquenumber", EntityRole.Referenced, "lk_msdyn_uniquenumber_createdby", "createdby")]
			public const string lk_msdyn_uniquenumber_createdby = "lk_msdyn_uniquenumber_createdby";
			[Relationship("msdyn_uniquenumber", EntityRole.Referenced, "lk_msdyn_uniquenumber_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_uniquenumber_createdonbehalfby = "lk_msdyn_uniquenumber_createdonbehalfby";
			[Relationship("msdyn_uniquenumber", EntityRole.Referenced, "lk_msdyn_uniquenumber_modifiedby", "modifiedby")]
			public const string lk_msdyn_uniquenumber_modifiedby = "lk_msdyn_uniquenumber_modifiedby";
			[Relationship("msdyn_uniquenumber", EntityRole.Referenced, "lk_msdyn_uniquenumber_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_uniquenumber_modifiedonbehalfby = "lk_msdyn_uniquenumber_modifiedonbehalfby";
			[Relationship("msdyn_untrackedappointment", EntityRole.Referenced, "lk_msdyn_untrackedappointment_createdby", "createdby")]
			public const string lk_msdyn_untrackedappointment_createdby = "lk_msdyn_untrackedappointment_createdby";
			[Relationship("msdyn_untrackedappointment", EntityRole.Referenced, "lk_msdyn_untrackedappointment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_untrackedappointment_createdonbehalfby = "lk_msdyn_untrackedappointment_createdonbehalfby";
			[Relationship("msdyn_untrackedappointment", EntityRole.Referenced, "lk_msdyn_untrackedappointment_modifiedby", "modifiedby")]
			public const string lk_msdyn_untrackedappointment_modifiedby = "lk_msdyn_untrackedappointment_modifiedby";
			[Relationship("msdyn_untrackedappointment", EntityRole.Referenced, "lk_msdyn_untrackedappointment_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_untrackedappointment_modifiedonbehalfby = "lk_msdyn_untrackedappointment_modifiedonbehalfby";
			[Relationship("msdyn_upgraderun", EntityRole.Referenced, "lk_msdyn_upgraderun_createdby", "createdby")]
			public const string lk_msdyn_upgraderun_createdby = "lk_msdyn_upgraderun_createdby";
			[Relationship("msdyn_upgraderun", EntityRole.Referenced, "lk_msdyn_upgraderun_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_upgraderun_createdonbehalfby = "lk_msdyn_upgraderun_createdonbehalfby";
			[Relationship("msdyn_upgraderun", EntityRole.Referenced, "lk_msdyn_upgraderun_modifiedby", "modifiedby")]
			public const string lk_msdyn_upgraderun_modifiedby = "lk_msdyn_upgraderun_modifiedby";
			[Relationship("msdyn_upgraderun", EntityRole.Referenced, "lk_msdyn_upgraderun_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_upgraderun_modifiedonbehalfby = "lk_msdyn_upgraderun_modifiedonbehalfby";
			[Relationship("msdyn_upgradestep", EntityRole.Referenced, "lk_msdyn_upgradestep_createdby", "createdby")]
			public const string lk_msdyn_upgradestep_createdby = "lk_msdyn_upgradestep_createdby";
			[Relationship("msdyn_upgradestep", EntityRole.Referenced, "lk_msdyn_upgradestep_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_upgradestep_createdonbehalfby = "lk_msdyn_upgradestep_createdonbehalfby";
			[Relationship("msdyn_upgradestep", EntityRole.Referenced, "lk_msdyn_upgradestep_modifiedby", "modifiedby")]
			public const string lk_msdyn_upgradestep_modifiedby = "lk_msdyn_upgradestep_modifiedby";
			[Relationship("msdyn_upgradestep", EntityRole.Referenced, "lk_msdyn_upgradestep_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_upgradestep_modifiedonbehalfby = "lk_msdyn_upgradestep_modifiedonbehalfby";
			[Relationship("msdyn_upgradeversion", EntityRole.Referenced, "lk_msdyn_upgradeversion_createdby", "createdby")]
			public const string lk_msdyn_upgradeversion_createdby = "lk_msdyn_upgradeversion_createdby";
			[Relationship("msdyn_upgradeversion", EntityRole.Referenced, "lk_msdyn_upgradeversion_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_upgradeversion_createdonbehalfby = "lk_msdyn_upgradeversion_createdonbehalfby";
			[Relationship("msdyn_upgradeversion", EntityRole.Referenced, "lk_msdyn_upgradeversion_modifiedby", "modifiedby")]
			public const string lk_msdyn_upgradeversion_modifiedby = "lk_msdyn_upgradeversion_modifiedby";
			[Relationship("msdyn_upgradeversion", EntityRole.Referenced, "lk_msdyn_upgradeversion_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_upgradeversion_modifiedonbehalfby = "lk_msdyn_upgradeversion_modifiedonbehalfby";
			[Relationship("msdyn_userworkhistory", EntityRole.Referenced, "lk_msdyn_userworkhistory_createdby", "createdby")]
			public const string lk_msdyn_userworkhistory_createdby = "lk_msdyn_userworkhistory_createdby";
			[Relationship("msdyn_userworkhistory", EntityRole.Referenced, "lk_msdyn_userworkhistory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_userworkhistory_createdonbehalfby = "lk_msdyn_userworkhistory_createdonbehalfby";
			[Relationship("msdyn_userworkhistory", EntityRole.Referenced, "lk_msdyn_userworkhistory_modifiedby", "modifiedby")]
			public const string lk_msdyn_userworkhistory_modifiedby = "lk_msdyn_userworkhistory_modifiedby";
			[Relationship("msdyn_userworkhistory", EntityRole.Referenced, "lk_msdyn_userworkhistory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_userworkhistory_modifiedonbehalfby = "lk_msdyn_userworkhistory_modifiedonbehalfby";
			[Relationship("msdyn_vocconfiguration", EntityRole.Referenced, "lk_msdyn_vocconfiguration_createdby", "")]
			public const string lk_msdyn_vocconfiguration_createdby = "lk_msdyn_vocconfiguration_createdby";
			[Relationship("msdyn_vocconfiguration", EntityRole.Referenced, "lk_msdyn_vocconfiguration_createdonbehalfby", "")]
			public const string lk_msdyn_vocconfiguration_createdonbehalfby = "lk_msdyn_vocconfiguration_createdonbehalfby";
			[Relationship("msdyn_vocconfiguration", EntityRole.Referenced, "lk_msdyn_vocconfiguration_modifiedby", "")]
			public const string lk_msdyn_vocconfiguration_modifiedby = "lk_msdyn_vocconfiguration_modifiedby";
			[Relationship("msdyn_vocconfiguration", EntityRole.Referenced, "lk_msdyn_vocconfiguration_modifiedonbehalfby", "")]
			public const string lk_msdyn_vocconfiguration_modifiedonbehalfby = "lk_msdyn_vocconfiguration_modifiedonbehalfby";
			[Relationship("msdyn_wallsavedquery", EntityRole.Referenced, "lk_msdyn_wallsavedquery_createdby", "createdby")]
			public const string lk_msdyn_wallsavedquery_createdby = "lk_msdyn_wallsavedquery_createdby";
			[Relationship("msdyn_wallsavedquery", EntityRole.Referenced, "lk_msdyn_wallsavedquery_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_wallsavedquery_createdonbehalfby = "lk_msdyn_wallsavedquery_createdonbehalfby";
			[Relationship("msdyn_wallsavedquery", EntityRole.Referenced, "lk_msdyn_wallsavedquery_modifiedby", "modifiedby")]
			public const string lk_msdyn_wallsavedquery_modifiedby = "lk_msdyn_wallsavedquery_modifiedby";
			[Relationship("msdyn_wallsavedquery", EntityRole.Referenced, "lk_msdyn_wallsavedquery_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_wallsavedquery_modifiedonbehalfby = "lk_msdyn_wallsavedquery_modifiedonbehalfby";
			[Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "lk_msdyn_wallsavedqueryusersettings_createdby", "createdby")]
			public const string lk_msdyn_wallsavedqueryusersettings_createdby = "lk_msdyn_wallsavedqueryusersettings_createdby";
			[Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "lk_msdyn_wallsavedqueryusersettings_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_wallsavedqueryusersettings_createdonbehalfby = "lk_msdyn_wallsavedqueryusersettings_createdonbehalfby";
			[Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "lk_msdyn_wallsavedqueryusersettings_modifiedby", "modifiedby")]
			public const string lk_msdyn_wallsavedqueryusersettings_modifiedby = "lk_msdyn_wallsavedqueryusersettings_modifiedby";
			[Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "lk_msdyn_wallsavedqueryusersettings_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_wallsavedqueryusersettings_modifiedonbehalfby = "lk_msdyn_wallsavedqueryusersettings_modifiedonbehalfby";
			[Relationship("msdyn_warehouse", EntityRole.Referenced, "lk_msdyn_warehouse_createdby", "createdby")]
			public const string lk_msdyn_warehouse_createdby = "lk_msdyn_warehouse_createdby";
			[Relationship("msdyn_warehouse", EntityRole.Referenced, "lk_msdyn_warehouse_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_warehouse_createdonbehalfby = "lk_msdyn_warehouse_createdonbehalfby";
			[Relationship("msdyn_warehouse", EntityRole.Referenced, "lk_msdyn_warehouse_modifiedby", "modifiedby")]
			public const string lk_msdyn_warehouse_modifiedby = "lk_msdyn_warehouse_modifiedby";
			[Relationship("msdyn_warehouse", EntityRole.Referenced, "lk_msdyn_warehouse_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_warehouse_modifiedonbehalfby = "lk_msdyn_warehouse_modifiedonbehalfby";
			[Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "lk_msdyn_workhourtemplate_createdby", "createdby")]
			public const string lk_msdyn_workhourtemplate_createdby = "lk_msdyn_workhourtemplate_createdby";
			[Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "lk_msdyn_workhourtemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workhourtemplate_createdonbehalfby = "lk_msdyn_workhourtemplate_createdonbehalfby";
			[Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "lk_msdyn_workhourtemplate_modifiedby", "modifiedby")]
			public const string lk_msdyn_workhourtemplate_modifiedby = "lk_msdyn_workhourtemplate_modifiedby";
			[Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "lk_msdyn_workhourtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workhourtemplate_modifiedonbehalfby = "lk_msdyn_workhourtemplate_modifiedonbehalfby";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "lk_msdyn_workorder_createdby", "createdby")]
			public const string lk_msdyn_workorder_createdby = "lk_msdyn_workorder_createdby";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "lk_msdyn_workorder_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workorder_createdonbehalfby = "lk_msdyn_workorder_createdonbehalfby";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "lk_msdyn_workorder_modifiedby", "modifiedby")]
			public const string lk_msdyn_workorder_modifiedby = "lk_msdyn_workorder_modifiedby";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "lk_msdyn_workorder_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workorder_modifiedonbehalfby = "lk_msdyn_workorder_modifiedonbehalfby";
			[Relationship("msdyn_workordercharacteristic", EntityRole.Referenced, "lk_msdyn_workordercharacteristic_createdby", "createdby")]
			public const string lk_msdyn_workordercharacteristic_createdby = "lk_msdyn_workordercharacteristic_createdby";
			[Relationship("msdyn_workordercharacteristic", EntityRole.Referenced, "lk_msdyn_workordercharacteristic_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workordercharacteristic_createdonbehalfby = "lk_msdyn_workordercharacteristic_createdonbehalfby";
			[Relationship("msdyn_workordercharacteristic", EntityRole.Referenced, "lk_msdyn_workordercharacteristic_modifiedby", "modifiedby")]
			public const string lk_msdyn_workordercharacteristic_modifiedby = "lk_msdyn_workordercharacteristic_modifiedby";
			[Relationship("msdyn_workordercharacteristic", EntityRole.Referenced, "lk_msdyn_workordercharacteristic_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workordercharacteristic_modifiedonbehalfby = "lk_msdyn_workordercharacteristic_modifiedonbehalfby";
			[Relationship("msdyn_workorderdetailsgenerationqueue", EntityRole.Referenced, "lk_msdyn_workorderdetailsgenerationqueue_createdby", "createdby")]
			public const string lk_msdyn_workorderdetailsgenerationqueue_createdby = "lk_msdyn_workorderdetailsgenerationqueue_createdby";
			[Relationship("msdyn_workorderdetailsgenerationqueue", EntityRole.Referenced, "lk_msdyn_workorderdetailsgenerationqueue_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workorderdetailsgenerationqueue_createdonbehalfby = "lk_msdyn_workorderdetailsgenerationqueue_createdonbehalfby";
			[Relationship("msdyn_workorderdetailsgenerationqueue", EntityRole.Referenced, "lk_msdyn_workorderdetailsgenerationqueue_modifiedby", "modifiedby")]
			public const string lk_msdyn_workorderdetailsgenerationqueue_modifiedby = "lk_msdyn_workorderdetailsgenerationqueue_modifiedby";
			[Relationship("msdyn_workorderdetailsgenerationqueue", EntityRole.Referenced, "lk_msdyn_workorderdetailsgenerationqueue_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workorderdetailsgenerationqueue_modifiedonbehalfby = "lk_msdyn_workorderdetailsgenerationqueue_modifiedonbehalfby";
			[Relationship("msdyn_workorderincident", EntityRole.Referenced, "lk_msdyn_workorderincident_createdby", "createdby")]
			public const string lk_msdyn_workorderincident_createdby = "lk_msdyn_workorderincident_createdby";
			[Relationship("msdyn_workorderincident", EntityRole.Referenced, "lk_msdyn_workorderincident_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workorderincident_createdonbehalfby = "lk_msdyn_workorderincident_createdonbehalfby";
			[Relationship("msdyn_workorderincident", EntityRole.Referenced, "lk_msdyn_workorderincident_modifiedby", "modifiedby")]
			public const string lk_msdyn_workorderincident_modifiedby = "lk_msdyn_workorderincident_modifiedby";
			[Relationship("msdyn_workorderincident", EntityRole.Referenced, "lk_msdyn_workorderincident_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workorderincident_modifiedonbehalfby = "lk_msdyn_workorderincident_modifiedonbehalfby";
			[Relationship("msdyn_workorderproduct", EntityRole.Referenced, "lk_msdyn_workorderproduct_createdby", "createdby")]
			public const string lk_msdyn_workorderproduct_createdby = "lk_msdyn_workorderproduct_createdby";
			[Relationship("msdyn_workorderproduct", EntityRole.Referenced, "lk_msdyn_workorderproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workorderproduct_createdonbehalfby = "lk_msdyn_workorderproduct_createdonbehalfby";
			[Relationship("msdyn_workorderproduct", EntityRole.Referenced, "lk_msdyn_workorderproduct_modifiedby", "modifiedby")]
			public const string lk_msdyn_workorderproduct_modifiedby = "lk_msdyn_workorderproduct_modifiedby";
			[Relationship("msdyn_workorderproduct", EntityRole.Referenced, "lk_msdyn_workorderproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workorderproduct_modifiedonbehalfby = "lk_msdyn_workorderproduct_modifiedonbehalfby";
			[Relationship("msdyn_workorderresourcerestriction", EntityRole.Referenced, "lk_msdyn_workorderresourcerestriction_createdby", "createdby")]
			public const string lk_msdyn_workorderresourcerestriction_createdby = "lk_msdyn_workorderresourcerestriction_createdby";
			[Relationship("msdyn_workorderresourcerestriction", EntityRole.Referenced, "lk_msdyn_workorderresourcerestriction_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workorderresourcerestriction_createdonbehalfby = "lk_msdyn_workorderresourcerestriction_createdonbehalfby";
			[Relationship("msdyn_workorderresourcerestriction", EntityRole.Referenced, "lk_msdyn_workorderresourcerestriction_modifiedby", "modifiedby")]
			public const string lk_msdyn_workorderresourcerestriction_modifiedby = "lk_msdyn_workorderresourcerestriction_modifiedby";
			[Relationship("msdyn_workorderresourcerestriction", EntityRole.Referenced, "lk_msdyn_workorderresourcerestriction_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workorderresourcerestriction_modifiedonbehalfby = "lk_msdyn_workorderresourcerestriction_modifiedonbehalfby";
			[Relationship("msdyn_workorderservice", EntityRole.Referenced, "lk_msdyn_workorderservice_createdby", "createdby")]
			public const string lk_msdyn_workorderservice_createdby = "lk_msdyn_workorderservice_createdby";
			[Relationship("msdyn_workorderservice", EntityRole.Referenced, "lk_msdyn_workorderservice_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workorderservice_createdonbehalfby = "lk_msdyn_workorderservice_createdonbehalfby";
			[Relationship("msdyn_workorderservice", EntityRole.Referenced, "lk_msdyn_workorderservice_modifiedby", "modifiedby")]
			public const string lk_msdyn_workorderservice_modifiedby = "lk_msdyn_workorderservice_modifiedby";
			[Relationship("msdyn_workorderservice", EntityRole.Referenced, "lk_msdyn_workorderservice_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workorderservice_modifiedonbehalfby = "lk_msdyn_workorderservice_modifiedonbehalfby";
			[Relationship("msdyn_workorderservicetask", EntityRole.Referenced, "lk_msdyn_workorderservicetask_createdby", "createdby")]
			public const string lk_msdyn_workorderservicetask_createdby = "lk_msdyn_workorderservicetask_createdby";
			[Relationship("msdyn_workorderservicetask", EntityRole.Referenced, "lk_msdyn_workorderservicetask_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workorderservicetask_createdonbehalfby = "lk_msdyn_workorderservicetask_createdonbehalfby";
			[Relationship("msdyn_workorderservicetask", EntityRole.Referenced, "lk_msdyn_workorderservicetask_modifiedby", "modifiedby")]
			public const string lk_msdyn_workorderservicetask_modifiedby = "lk_msdyn_workorderservicetask_modifiedby";
			[Relationship("msdyn_workorderservicetask", EntityRole.Referenced, "lk_msdyn_workorderservicetask_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workorderservicetask_modifiedonbehalfby = "lk_msdyn_workorderservicetask_modifiedonbehalfby";
			[Relationship("msdyn_workordersubstatus", EntityRole.Referenced, "lk_msdyn_workordersubstatus_createdby", "createdby")]
			public const string lk_msdyn_workordersubstatus_createdby = "lk_msdyn_workordersubstatus_createdby";
			[Relationship("msdyn_workordersubstatus", EntityRole.Referenced, "lk_msdyn_workordersubstatus_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workordersubstatus_createdonbehalfby = "lk_msdyn_workordersubstatus_createdonbehalfby";
			[Relationship("msdyn_workordersubstatus", EntityRole.Referenced, "lk_msdyn_workordersubstatus_modifiedby", "modifiedby")]
			public const string lk_msdyn_workordersubstatus_modifiedby = "lk_msdyn_workordersubstatus_modifiedby";
			[Relationship("msdyn_workordersubstatus", EntityRole.Referenced, "lk_msdyn_workordersubstatus_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workordersubstatus_modifiedonbehalfby = "lk_msdyn_workordersubstatus_modifiedonbehalfby";
			[Relationship("msdyn_workordertype", EntityRole.Referenced, "lk_msdyn_workordertype_createdby", "createdby")]
			public const string lk_msdyn_workordertype_createdby = "lk_msdyn_workordertype_createdby";
			[Relationship("msdyn_workordertype", EntityRole.Referenced, "lk_msdyn_workordertype_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msdyn_workordertype_createdonbehalfby = "lk_msdyn_workordertype_createdonbehalfby";
			[Relationship("msdyn_workordertype", EntityRole.Referenced, "lk_msdyn_workordertype_modifiedby", "modifiedby")]
			public const string lk_msdyn_workordertype_modifiedby = "lk_msdyn_workordertype_modifiedby";
			[Relationship("msdyn_workordertype", EntityRole.Referenced, "lk_msdyn_workordertype_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msdyn_workordertype_modifiedonbehalfby = "lk_msdyn_workordertype_modifiedonbehalfby";
			[Relationship("msfp_emailtemplate", EntityRole.Referenced, "lk_msfp_emailtemplate_createdby", "createdby")]
			public const string lk_msfp_emailtemplate_createdby = "lk_msfp_emailtemplate_createdby";
			[Relationship("msfp_emailtemplate", EntityRole.Referenced, "lk_msfp_emailtemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msfp_emailtemplate_createdonbehalfby = "lk_msfp_emailtemplate_createdonbehalfby";
			[Relationship("msfp_emailtemplate", EntityRole.Referenced, "lk_msfp_emailtemplate_modifiedby", "modifiedby")]
			public const string lk_msfp_emailtemplate_modifiedby = "lk_msfp_emailtemplate_modifiedby";
			[Relationship("msfp_emailtemplate", EntityRole.Referenced, "lk_msfp_emailtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msfp_emailtemplate_modifiedonbehalfby = "lk_msfp_emailtemplate_modifiedonbehalfby";
			[Relationship("msfp_question", EntityRole.Referenced, "lk_msfp_question_createdby", "createdby")]
			public const string lk_msfp_question_createdby = "lk_msfp_question_createdby";
			[Relationship("msfp_question", EntityRole.Referenced, "lk_msfp_question_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msfp_question_createdonbehalfby = "lk_msfp_question_createdonbehalfby";
			[Relationship("msfp_question", EntityRole.Referenced, "lk_msfp_question_modifiedby", "modifiedby")]
			public const string lk_msfp_question_modifiedby = "lk_msfp_question_modifiedby";
			[Relationship("msfp_question", EntityRole.Referenced, "lk_msfp_question_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msfp_question_modifiedonbehalfby = "lk_msfp_question_modifiedonbehalfby";
			[Relationship("msfp_questionresponse", EntityRole.Referenced, "lk_msfp_questionresponse_createdby", "createdby")]
			public const string lk_msfp_questionresponse_createdby = "lk_msfp_questionresponse_createdby";
			[Relationship("msfp_questionresponse", EntityRole.Referenced, "lk_msfp_questionresponse_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msfp_questionresponse_createdonbehalfby = "lk_msfp_questionresponse_createdonbehalfby";
			[Relationship("msfp_questionresponse", EntityRole.Referenced, "lk_msfp_questionresponse_modifiedby", "modifiedby")]
			public const string lk_msfp_questionresponse_modifiedby = "lk_msfp_questionresponse_modifiedby";
			[Relationship("msfp_questionresponse", EntityRole.Referenced, "lk_msfp_questionresponse_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msfp_questionresponse_modifiedonbehalfby = "lk_msfp_questionresponse_modifiedonbehalfby";
			[Relationship("msfp_survey", EntityRole.Referenced, "lk_msfp_survey_createdby", "createdby")]
			public const string lk_msfp_survey_createdby = "lk_msfp_survey_createdby";
			[Relationship("msfp_survey", EntityRole.Referenced, "lk_msfp_survey_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msfp_survey_createdonbehalfby = "lk_msfp_survey_createdonbehalfby";
			[Relationship("msfp_survey", EntityRole.Referenced, "lk_msfp_survey_modifiedby", "modifiedby")]
			public const string lk_msfp_survey_modifiedby = "lk_msfp_survey_modifiedby";
			[Relationship("msfp_survey", EntityRole.Referenced, "lk_msfp_survey_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msfp_survey_modifiedonbehalfby = "lk_msfp_survey_modifiedonbehalfby";
			[Relationship("msfp_unsubscribedrecipient", EntityRole.Referenced, "lk_msfp_unsubscribedrecipient_createdby", "createdby")]
			public const string lk_msfp_unsubscribedrecipient_createdby = "lk_msfp_unsubscribedrecipient_createdby";
			[Relationship("msfp_unsubscribedrecipient", EntityRole.Referenced, "lk_msfp_unsubscribedrecipient_createdonbehalfby", "createdonbehalfby")]
			public const string lk_msfp_unsubscribedrecipient_createdonbehalfby = "lk_msfp_unsubscribedrecipient_createdonbehalfby";
			[Relationship("msfp_unsubscribedrecipient", EntityRole.Referenced, "lk_msfp_unsubscribedrecipient_modifiedby", "modifiedby")]
			public const string lk_msfp_unsubscribedrecipient_modifiedby = "lk_msfp_unsubscribedrecipient_modifiedby";
			[Relationship("msfp_unsubscribedrecipient", EntityRole.Referenced, "lk_msfp_unsubscribedrecipient_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_msfp_unsubscribedrecipient_modifiedonbehalfby = "lk_msfp_unsubscribedrecipient_modifiedonbehalfby";
			[Relationship("navigationsetting", EntityRole.Referenced, "systemuser_navigationsetting_createdby", "createdby")]
			public const string lk_navigationsetting_createdby = "lk_navigationsetting_createdby";
			[Relationship("navigationsetting", EntityRole.Referenced, "systemuser_navigationsetting_createdonbehalfby", "createdonbehalfby")]
			public const string lk_navigationsetting_createdonbehalfby = "lk_navigationsetting_createdonbehalfby";
			[Relationship("navigationsetting", EntityRole.Referenced, "systemuser_navigationsetting_modifiedby", "modifiedby")]
			public const string lk_navigationsetting_modifiedby = "lk_navigationsetting_modifiedby";
			[Relationship("navigationsetting", EntityRole.Referenced, "systemuser_navigationsetting_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_navigationsetting_modifiedonbehalfby = "lk_navigationsetting_modifiedonbehalfby";
			[Relationship("newprocess", EntityRole.Referenced, "lk_newprocess_createdby", "createdby")]
			public const string lk_newprocess_createdby = "lk_newprocess_createdby";
			[Relationship("newprocess", EntityRole.Referenced, "lk_newprocess_createdonbehalfby", "createdonbehalfby")]
			public const string lk_newprocess_createdonbehalfby = "lk_newprocess_createdonbehalfby";
			[Relationship("newprocess", EntityRole.Referenced, "lk_newprocess_modifiedby", "modifiedby")]
			public const string lk_newprocess_modifiedby = "lk_newprocess_modifiedby";
			[Relationship("newprocess", EntityRole.Referenced, "lk_newprocess_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_newprocess_modifiedonbehalfby = "lk_newprocess_modifiedonbehalfby";
			[Relationship("officedocument", EntityRole.Referenced, "lk_officedocumentbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_officedocumentbase_createdonbehalfby = "lk_officedocumentbase_createdonbehalfby";
			[Relationship("officedocument", EntityRole.Referenced, "lk_officedocumentbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_officedocumentbase_modifiedonbehalfby = "lk_officedocumentbase_modifiedonbehalfby";
			[Relationship("officegraphdocument", EntityRole.Referenced, "lk_officegraphdocument_createdonbehalfby", "createdonbehalfby")]
			public const string lk_officegraphdocument_createdonbehalfby = "lk_officegraphdocument_createdonbehalfby";
			[Relationship("officegraphdocument", EntityRole.Referenced, "lk_officegraphdocument_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_officegraphdocument_modifiedonbehalfby = "lk_officegraphdocument_modifiedonbehalfby";
			[Relationship("offlinecommanddefinition", EntityRole.Referenced, "lk_offlinecommanddefinition_createdby", "createdby")]
			public const string lk_offlinecommanddefinition_createdby = "lk_offlinecommanddefinition_createdby";
			[Relationship("offlinecommanddefinition", EntityRole.Referenced, "lk_offlinecommanddefinition_createdonbehalfby", "createdonbehalfby")]
			public const string lk_offlinecommanddefinition_createdonbehalfby = "lk_offlinecommanddefinition_createdonbehalfby";
			[Relationship("offlinecommanddefinition", EntityRole.Referenced, "lk_offlinecommanddefinition_modifiedby", "modifiedby")]
			public const string lk_offlinecommanddefinition_modifiedby = "lk_offlinecommanddefinition_modifiedby";
			[Relationship("offlinecommanddefinition", EntityRole.Referenced, "lk_offlinecommanddefinition_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_offlinecommanddefinition_modifiedonbehalfby = "lk_offlinecommanddefinition_modifiedonbehalfby";
			[Relationship("opportunity", EntityRole.Referenced, "lk_opportunity_createdonbehalfby", "createdonbehalfby")]
			public const string lk_opportunity_createdonbehalfby = "lk_opportunity_createdonbehalfby";
			[Relationship("opportunity", EntityRole.Referenced, "lk_opportunity_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_opportunity_modifiedonbehalfby = "lk_opportunity_modifiedonbehalfby";
			[Relationship("opportunity", EntityRole.Referenced, "lk_opportunitybase_createdby", "createdby")]
			public const string lk_opportunitybase_createdby = "lk_opportunitybase_createdby";
			[Relationship("opportunity", EntityRole.Referenced, "lk_opportunitybase_modifiedby", "modifiedby")]
			public const string lk_opportunitybase_modifiedby = "lk_opportunitybase_modifiedby";
			[Relationship("opportunityclose", EntityRole.Referenced, "lk_opportunityclose_createdby", "createdby")]
			public const string lk_opportunityclose_createdby = "lk_opportunityclose_createdby";
			[Relationship("opportunityclose", EntityRole.Referenced, "lk_opportunityclose_createdonbehalfby", "createdonbehalfby")]
			public const string lk_opportunityclose_createdonbehalfby = "lk_opportunityclose_createdonbehalfby";
			[Relationship("opportunityclose", EntityRole.Referenced, "lk_opportunityclose_modifiedby", "modifiedby")]
			public const string lk_opportunityclose_modifiedby = "lk_opportunityclose_modifiedby";
			[Relationship("opportunityclose", EntityRole.Referenced, "lk_opportunityclose_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_opportunityclose_modifiedonbehalfby = "lk_opportunityclose_modifiedonbehalfby";
			[Relationship("opportunityproduct", EntityRole.Referenced, "lk_opportunityproduct_createdonbehalfby", "createdonbehalfby")]
			public const string lk_opportunityproduct_createdonbehalfby = "lk_opportunityproduct_createdonbehalfby";
			[Relationship("opportunityproduct", EntityRole.Referenced, "lk_opportunityproduct_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_opportunityproduct_modifiedonbehalfby = "lk_opportunityproduct_modifiedonbehalfby";
			[Relationship("opportunityproduct", EntityRole.Referenced, "lk_opportunityproductbase_createdby", "createdby")]
			public const string lk_opportunityproductbase_createdby = "lk_opportunityproductbase_createdby";
			[Relationship("opportunityproduct", EntityRole.Referenced, "lk_opportunityproductbase_modifiedby", "modifiedby")]
			public const string lk_opportunityproductbase_modifiedby = "lk_opportunityproductbase_modifiedby";
			[Relationship("opportunitysalesprocess", EntityRole.Referenced, "lk_opportunitysalesprocess_createdby", "createdby")]
			public const string lk_opportunitysalesprocess_createdby = "lk_opportunitysalesprocess_createdby";
			[Relationship("opportunitysalesprocess", EntityRole.Referenced, "lk_opportunitysalesprocess_createdonbehalfby", "createdonbehalfby")]
			public const string lk_opportunitysalesprocess_createdonbehalfby = "lk_opportunitysalesprocess_createdonbehalfby";
			[Relationship("opportunitysalesprocess", EntityRole.Referenced, "lk_opportunitysalesprocess_modifiedby", "modifiedby")]
			public const string lk_opportunitysalesprocess_modifiedby = "lk_opportunitysalesprocess_modifiedby";
			[Relationship("opportunitysalesprocess", EntityRole.Referenced, "lk_opportunitysalesprocess_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_opportunitysalesprocess_modifiedonbehalfby = "lk_opportunitysalesprocess_modifiedonbehalfby";
			[Relationship("orderclose", EntityRole.Referenced, "lk_orderclose_createdby", "createdby")]
			public const string lk_orderclose_createdby = "lk_orderclose_createdby";
			[Relationship("orderclose", EntityRole.Referenced, "lk_orderclose_createdonbehalfby", "createdonbehalfby")]
			public const string lk_orderclose_createdonbehalfby = "lk_orderclose_createdonbehalfby";
			[Relationship("orderclose", EntityRole.Referenced, "lk_orderclose_modifiedby", "modifiedby")]
			public const string lk_orderclose_modifiedby = "lk_orderclose_modifiedby";
			[Relationship("orderclose", EntityRole.Referenced, "lk_orderclose_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_orderclose_modifiedonbehalfby = "lk_orderclose_modifiedonbehalfby";
			[Relationship("organization", EntityRole.Referenced, "lk_organization_createdonbehalfby", "createdonbehalfby")]
			public const string lk_organization_createdonbehalfby = "lk_organization_createdonbehalfby";
			[Relationship("organization", EntityRole.Referenced, "lk_organization_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_organization_modifiedonbehalfby = "lk_organization_modifiedonbehalfby";
			[Relationship("organization", EntityRole.Referenced, "lk_organizationbase_createdby", "createdby")]
			public const string lk_organizationbase_createdby = "lk_organizationbase_createdby";
			[Relationship("organization", EntityRole.Referenced, "lk_organizationbase_modifiedby", "modifiedby")]
			public const string lk_organizationbase_modifiedby = "lk_organizationbase_modifiedby";
			[Relationship("ownermapping", EntityRole.Referenced, "lk_ownermapping_createdby", "createdby")]
			public const string lk_ownermapping_createdby = "lk_ownermapping_createdby";
			[Relationship("ownermapping", EntityRole.Referenced, "lk_ownermapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ownermapping_createdonbehalfby = "lk_ownermapping_createdonbehalfby";
			[Relationship("ownermapping", EntityRole.Referenced, "lk_ownermapping_modifiedby", "modifiedby")]
			public const string lk_ownermapping_modifiedby = "lk_ownermapping_modifiedby";
			[Relationship("ownermapping", EntityRole.Referenced, "lk_ownermapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ownermapping_modifiedonbehalfby = "lk_ownermapping_modifiedonbehalfby";
			[Relationship("partnerapplication", EntityRole.Referenced, "lk_partnerapplication_createdby", "createdby")]
			public const string lk_partnerapplication_createdby = "lk_partnerapplication_createdby";
			[Relationship("partnerapplication", EntityRole.Referenced, "lk_partnerapplication_createdonbehalfby", "createdonbehalfby")]
			public const string lk_partnerapplication_createdonbehalfby = "lk_partnerapplication_createdonbehalfby";
			[Relationship("partnerapplication", EntityRole.Referenced, "lk_partnerapplication_modifiedby", "modifiedby")]
			public const string lk_partnerapplication_modifiedby = "lk_partnerapplication_modifiedby";
			[Relationship("partnerapplication", EntityRole.Referenced, "lk_partnerapplication_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_partnerapplication_modifiedonbehalfby = "lk_partnerapplication_modifiedonbehalfby";
			[Relationship("pchmcs_address", EntityRole.Referenced, "lk_pchmcs_address_createdby", "")]
			public const string lk_pchmcs_address_createdby = "lk_pchmcs_address_createdby";
			[Relationship("pchmcs_address", EntityRole.Referenced, "lk_pchmcs_address_createdonbehalfby", "")]
			public const string lk_pchmcs_address_createdonbehalfby = "lk_pchmcs_address_createdonbehalfby";
			[Relationship("pchmcs_address", EntityRole.Referenced, "lk_pchmcs_address_modifiedby", "")]
			public const string lk_pchmcs_address_modifiedby = "lk_pchmcs_address_modifiedby";
			[Relationship("pchmcs_address", EntityRole.Referenced, "lk_pchmcs_address_modifiedonbehalfby", "")]
			public const string lk_pchmcs_address_modifiedonbehalfby = "lk_pchmcs_address_modifiedonbehalfby";
			[Relationship("pchmcs_admincontactbo", EntityRole.Referenced, "lk_pchmcs_admincontactbo_createdby", "")]
			public const string lk_pchmcs_admincontactbo_createdby = "lk_pchmcs_admincontactbo_createdby";
			[Relationship("pchmcs_admincontactbo", EntityRole.Referenced, "lk_pchmcs_admincontactbo_createdonbehalfby", "")]
			public const string lk_pchmcs_admincontactbo_createdonbehalfby = "lk_pchmcs_admincontactbo_createdonbehalfby";
			[Relationship("pchmcs_admincontactbo", EntityRole.Referenced, "lk_pchmcs_admincontactbo_modifiedby", "")]
			public const string lk_pchmcs_admincontactbo_modifiedby = "lk_pchmcs_admincontactbo_modifiedby";
			[Relationship("pchmcs_admincontactbo", EntityRole.Referenced, "lk_pchmcs_admincontactbo_modifiedonbehalfby", "")]
			public const string lk_pchmcs_admincontactbo_modifiedonbehalfby = "lk_pchmcs_admincontactbo_modifiedonbehalfby";
			[Relationship("pchmcs_adresseemailgenerique", EntityRole.Referenced, "lk_pchmcs_adresseemailgenerique_createdby", "")]
			public const string lk_pchmcs_adresseemailgenerique_createdby = "lk_pchmcs_adresseemailgenerique_createdby";
			[Relationship("pchmcs_adresseemailgenerique", EntityRole.Referenced, "lk_pchmcs_adresseemailgenerique_createdonbehalfby", "")]
			public const string lk_pchmcs_adresseemailgenerique_createdonbehalfby = "lk_pchmcs_adresseemailgenerique_createdonbehalfby";
			[Relationship("pchmcs_adresseemailgenerique", EntityRole.Referenced, "lk_pchmcs_adresseemailgenerique_modifiedby", "")]
			public const string lk_pchmcs_adresseemailgenerique_modifiedby = "lk_pchmcs_adresseemailgenerique_modifiedby";
			[Relationship("pchmcs_adresseemailgenerique", EntityRole.Referenced, "lk_pchmcs_adresseemailgenerique_modifiedonbehalfby", "")]
			public const string lk_pchmcs_adresseemailgenerique_modifiedonbehalfby = "lk_pchmcs_adresseemailgenerique_modifiedonbehalfby";
			[Relationship("pchmcs_affectation", EntityRole.Referenced, "lk_pchmcs_affectation_createdby", "")]
			public const string lk_pchmcs_affectation_createdby = "lk_pchmcs_affectation_createdby";
			[Relationship("pchmcs_affectation", EntityRole.Referenced, "lk_pchmcs_affectation_createdonbehalfby", "")]
			public const string lk_pchmcs_affectation_createdonbehalfby = "lk_pchmcs_affectation_createdonbehalfby";
			[Relationship("pchmcs_affectation", EntityRole.Referenced, "lk_pchmcs_affectation_modifiedby", "")]
			public const string lk_pchmcs_affectation_modifiedby = "lk_pchmcs_affectation_modifiedby";
			[Relationship("pchmcs_affectation", EntityRole.Referenced, "lk_pchmcs_affectation_modifiedonbehalfby", "")]
			public const string lk_pchmcs_affectation_modifiedonbehalfby = "lk_pchmcs_affectation_modifiedonbehalfby";
			[Relationship("pchmcs_agence", EntityRole.Referenced, "lk_pchmcs_agence_createdby", "")]
			public const string lk_pchmcs_agence_createdby = "lk_pchmcs_agence_createdby";
			[Relationship("pchmcs_agence", EntityRole.Referenced, "lk_pchmcs_agence_createdonbehalfby", "")]
			public const string lk_pchmcs_agence_createdonbehalfby = "lk_pchmcs_agence_createdonbehalfby";
			[Relationship("pchmcs_agence", EntityRole.Referenced, "lk_pchmcs_agence_modifiedby", "")]
			public const string lk_pchmcs_agence_modifiedby = "lk_pchmcs_agence_modifiedby";
			[Relationship("pchmcs_agence", EntityRole.Referenced, "lk_pchmcs_agence_modifiedonbehalfby", "")]
			public const string lk_pchmcs_agence_modifiedonbehalfby = "lk_pchmcs_agence_modifiedonbehalfby";
			[Relationship("pchmcs_aidevente", EntityRole.Referenced, "lk_pchmcs_aidevente_createdby", "")]
			public const string lk_pchmcs_aidevente_createdby = "lk_pchmcs_aidevente_createdby";
			[Relationship("pchmcs_aidevente", EntityRole.Referenced, "lk_pchmcs_aidevente_createdonbehalfby", "")]
			public const string lk_pchmcs_aidevente_createdonbehalfby = "lk_pchmcs_aidevente_createdonbehalfby";
			[Relationship("pchmcs_aidevente", EntityRole.Referenced, "lk_pchmcs_aidevente_modifiedby", "")]
			public const string lk_pchmcs_aidevente_modifiedby = "lk_pchmcs_aidevente_modifiedby";
			[Relationship("pchmcs_aidevente", EntityRole.Referenced, "lk_pchmcs_aidevente_modifiedonbehalfby", "")]
			public const string lk_pchmcs_aidevente_modifiedonbehalfby = "lk_pchmcs_aidevente_modifiedonbehalfby";
			[Relationship("pchmcs_aideventeoffrecyclevente", EntityRole.Referenced, "lk_pchmcs_aideventeoffrecyclevente_createdby", "")]
			public const string lk_pchmcs_aideventeoffrecyclevente_createdby = "lk_pchmcs_aideventeoffrecyclevente_createdby";
			[Relationship("pchmcs_aideventeoffrecyclevente", EntityRole.Referenced, "lk_pchmcs_aideventeoffrecyclevente_createdonbehalfby", "")]
			public const string lk_pchmcs_aideventeoffrecyclevente_createdonbehalfby = "lk_pchmcs_aideventeoffrecyclevente_createdonbehalfby";
			[Relationship("pchmcs_aideventeoffrecyclevente", EntityRole.Referenced, "lk_pchmcs_aideventeoffrecyclevente_modifiedby", "")]
			public const string lk_pchmcs_aideventeoffrecyclevente_modifiedby = "lk_pchmcs_aideventeoffrecyclevente_modifiedby";
			[Relationship("pchmcs_aideventeoffrecyclevente", EntityRole.Referenced, "lk_pchmcs_aideventeoffrecyclevente_modifiedonbehalfby", "")]
			public const string lk_pchmcs_aideventeoffrecyclevente_modifiedonbehalfby = "lk_pchmcs_aideventeoffrecyclevente_modifiedonbehalfby";
			[Relationship("pchmcs_allotement", EntityRole.Referenced, "lk_pchmcs_allotement_createdby", "")]
			public const string lk_pchmcs_allotement_createdby = "lk_pchmcs_allotement_createdby";
			[Relationship("pchmcs_allotement", EntityRole.Referenced, "lk_pchmcs_allotement_createdonbehalfby", "")]
			public const string lk_pchmcs_allotement_createdonbehalfby = "lk_pchmcs_allotement_createdonbehalfby";
			[Relationship("pchmcs_allotement", EntityRole.Referenced, "lk_pchmcs_allotement_modifiedby", "")]
			public const string lk_pchmcs_allotement_modifiedby = "lk_pchmcs_allotement_modifiedby";
			[Relationship("pchmcs_allotement", EntityRole.Referenced, "lk_pchmcs_allotement_modifiedonbehalfby", "")]
			public const string lk_pchmcs_allotement_modifiedonbehalfby = "lk_pchmcs_allotement_modifiedonbehalfby";
			[Relationship("pchmcs_anciennegociateur", EntityRole.Referenced, "lk_pchmcs_anciennegociateur_createdby", "")]
			public const string lk_pchmcs_anciennegociateur_createdby = "lk_pchmcs_anciennegociateur_createdby";
			[Relationship("pchmcs_anciennegociateur", EntityRole.Referenced, "lk_pchmcs_anciennegociateur_createdonbehalfby", "")]
			public const string lk_pchmcs_anciennegociateur_createdonbehalfby = "lk_pchmcs_anciennegociateur_createdonbehalfby";
			[Relationship("pchmcs_anciennegociateur", EntityRole.Referenced, "lk_pchmcs_anciennegociateur_modifiedby", "")]
			public const string lk_pchmcs_anciennegociateur_modifiedby = "lk_pchmcs_anciennegociateur_modifiedby";
			[Relationship("pchmcs_anciennegociateur", EntityRole.Referenced, "lk_pchmcs_anciennegociateur_modifiedonbehalfby", "")]
			public const string lk_pchmcs_anciennegociateur_modifiedonbehalfby = "lk_pchmcs_anciennegociateur_modifiedonbehalfby";
			[Relationship("pchmcs_appelfonds", EntityRole.Referenced, "lk_pchmcs_appelfonds_createdby", "")]
			public const string lk_pchmcs_appelfonds_createdby = "lk_pchmcs_appelfonds_createdby";
			[Relationship("pchmcs_appelfonds", EntityRole.Referenced, "lk_pchmcs_appelfonds_createdonbehalfby", "")]
			public const string lk_pchmcs_appelfonds_createdonbehalfby = "lk_pchmcs_appelfonds_createdonbehalfby";
			[Relationship("pchmcs_appelfonds", EntityRole.Referenced, "lk_pchmcs_appelfonds_modifiedby", "")]
			public const string lk_pchmcs_appelfonds_modifiedby = "lk_pchmcs_appelfonds_modifiedby";
			[Relationship("pchmcs_appelfonds", EntityRole.Referenced, "lk_pchmcs_appelfonds_modifiedonbehalfby", "")]
			public const string lk_pchmcs_appelfonds_modifiedonbehalfby = "lk_pchmcs_appelfonds_modifiedonbehalfby";
			[Relationship("pchmcs_bilanpatrimonial", EntityRole.Referenced, "lk_pchmcs_bilanpatrimonial_createdby", "")]
			public const string lk_pchmcs_bilanpatrimonial_createdby = "lk_pchmcs_bilanpatrimonial_createdby";
			[Relationship("pchmcs_bilanpatrimonial", EntityRole.Referenced, "lk_pchmcs_bilanpatrimonial_createdonbehalfby", "")]
			public const string lk_pchmcs_bilanpatrimonial_createdonbehalfby = "lk_pchmcs_bilanpatrimonial_createdonbehalfby";
			[Relationship("pchmcs_bilanpatrimonial", EntityRole.Referenced, "lk_pchmcs_bilanpatrimonial_modifiedby", "")]
			public const string lk_pchmcs_bilanpatrimonial_modifiedby = "lk_pchmcs_bilanpatrimonial_modifiedby";
			[Relationship("pchmcs_bilanpatrimonial", EntityRole.Referenced, "lk_pchmcs_bilanpatrimonial_modifiedonbehalfby", "")]
			public const string lk_pchmcs_bilanpatrimonial_modifiedonbehalfby = "lk_pchmcs_bilanpatrimonial_modifiedonbehalfby";
			[Relationship("pchmcs_bookmarks", EntityRole.Referenced, "lk_pchmcs_bookmarks_createdby", "")]
			public const string lk_pchmcs_bookmarks_createdby = "lk_pchmcs_bookmarks_createdby";
			[Relationship("pchmcs_bookmarks", EntityRole.Referenced, "lk_pchmcs_bookmarks_createdonbehalfby", "")]
			public const string lk_pchmcs_bookmarks_createdonbehalfby = "lk_pchmcs_bookmarks_createdonbehalfby";
			[Relationship("pchmcs_bookmarks", EntityRole.Referenced, "lk_pchmcs_bookmarks_modifiedby", "")]
			public const string lk_pchmcs_bookmarks_modifiedby = "lk_pchmcs_bookmarks_modifiedby";
			[Relationship("pchmcs_bookmarks", EntityRole.Referenced, "lk_pchmcs_bookmarks_modifiedonbehalfby", "")]
			public const string lk_pchmcs_bookmarks_modifiedonbehalfby = "lk_pchmcs_bookmarks_modifiedonbehalfby";
			[Relationship("pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5", EntityRole.Referenced, "lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_createdby", "")]
			public const string lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_createdby = "lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_createdby";
			[Relationship("pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5", EntityRole.Referenced, "lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_createdonbehalfby", "")]
			public const string lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_createdonbehalfby = "lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_createdonbehalfby";
			[Relationship("pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5", EntityRole.Referenced, "lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_modifiedby", "")]
			public const string lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_modifiedby = "lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_modifiedby";
			[Relationship("pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5", EntityRole.Referenced, "lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_modifiedonbehalfby", "")]
			public const string lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_modifiedonbehalfby = "lk_pchmcs_bpf_0a40d7e390ef43d0a1adbbb855cd39d5_modifiedonbehalfby";
			[Relationship("pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89", EntityRole.Referenced, "lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_createdby", "")]
			public const string lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_createdby = "lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_createdby";
			[Relationship("pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89", EntityRole.Referenced, "lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_createdonbehalfby", "")]
			public const string lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_createdonbehalfby = "lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_createdonbehalfby";
			[Relationship("pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89", EntityRole.Referenced, "lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_modifiedby", "")]
			public const string lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_modifiedby = "lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_modifiedby";
			[Relationship("pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89", EntityRole.Referenced, "lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_modifiedonbehalfby", "")]
			public const string lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_modifiedonbehalfby = "lk_pchmcs_bpf_7a744b7b3e044f26b48f5ade88bf7d89_modifiedonbehalfby";
			[Relationship("pchmcs_canal", EntityRole.Referenced, "lk_pchmcs_canal_createdby", "")]
			public const string lk_pchmcs_canal_createdby = "lk_pchmcs_canal_createdby";
			[Relationship("pchmcs_canal", EntityRole.Referenced, "lk_pchmcs_canal_createdonbehalfby", "")]
			public const string lk_pchmcs_canal_createdonbehalfby = "lk_pchmcs_canal_createdonbehalfby";
			[Relationship("pchmcs_canal", EntityRole.Referenced, "lk_pchmcs_canal_modifiedby", "")]
			public const string lk_pchmcs_canal_modifiedby = "lk_pchmcs_canal_modifiedby";
			[Relationship("pchmcs_canal", EntityRole.Referenced, "lk_pchmcs_canal_modifiedonbehalfby", "")]
			public const string lk_pchmcs_canal_modifiedonbehalfby = "lk_pchmcs_canal_modifiedonbehalfby";
			[Relationship("pchmcs_cdvbuyer", EntityRole.Referenced, "lk_pchmcs_cdvbuyer_createdby", "")]
			public const string lk_pchmcs_cdvbuyer_createdby = "lk_pchmcs_cdvbuyer_createdby";
			[Relationship("pchmcs_cdvbuyer", EntityRole.Referenced, "lk_pchmcs_cdvbuyer_createdonbehalfby", "")]
			public const string lk_pchmcs_cdvbuyer_createdonbehalfby = "lk_pchmcs_cdvbuyer_createdonbehalfby";
			[Relationship("pchmcs_cdvbuyer", EntityRole.Referenced, "lk_pchmcs_cdvbuyer_modifiedby", "")]
			public const string lk_pchmcs_cdvbuyer_modifiedby = "lk_pchmcs_cdvbuyer_modifiedby";
			[Relationship("pchmcs_cdvbuyer", EntityRole.Referenced, "lk_pchmcs_cdvbuyer_modifiedonbehalfby", "")]
			public const string lk_pchmcs_cdvbuyer_modifiedonbehalfby = "lk_pchmcs_cdvbuyer_modifiedonbehalfby";
			[Relationship("pchmcs_chargemensuelle", EntityRole.Referenced, "lk_pchmcs_chargemensuelle_createdby", "")]
			public const string lk_pchmcs_chargemensuelle_createdby = "lk_pchmcs_chargemensuelle_createdby";
			[Relationship("pchmcs_chargemensuelle", EntityRole.Referenced, "lk_pchmcs_chargemensuelle_createdonbehalfby", "")]
			public const string lk_pchmcs_chargemensuelle_createdonbehalfby = "lk_pchmcs_chargemensuelle_createdonbehalfby";
			[Relationship("pchmcs_chargemensuelle", EntityRole.Referenced, "lk_pchmcs_chargemensuelle_modifiedby", "")]
			public const string lk_pchmcs_chargemensuelle_modifiedby = "lk_pchmcs_chargemensuelle_modifiedby";
			[Relationship("pchmcs_chargemensuelle", EntityRole.Referenced, "lk_pchmcs_chargemensuelle_modifiedonbehalfby", "")]
			public const string lk_pchmcs_chargemensuelle_modifiedonbehalfby = "lk_pchmcs_chargemensuelle_modifiedonbehalfby";
			[Relationship("pchmcs_commercialanimation", EntityRole.Referenced, "lk_pchmcs_commercialanimation_createdby", "")]
			public const string lk_pchmcs_commercialanimation_createdby = "lk_pchmcs_commercialanimation_createdby";
			[Relationship("pchmcs_commercialanimation", EntityRole.Referenced, "lk_pchmcs_commercialanimation_createdonbehalfby", "")]
			public const string lk_pchmcs_commercialanimation_createdonbehalfby = "lk_pchmcs_commercialanimation_createdonbehalfby";
			[Relationship("pchmcs_commercialanimation", EntityRole.Referenced, "lk_pchmcs_commercialanimation_modifiedby", "")]
			public const string lk_pchmcs_commercialanimation_modifiedby = "lk_pchmcs_commercialanimation_modifiedby";
			[Relationship("pchmcs_commercialanimation", EntityRole.Referenced, "lk_pchmcs_commercialanimation_modifiedonbehalfby", "")]
			public const string lk_pchmcs_commercialanimation_modifiedonbehalfby = "lk_pchmcs_commercialanimation_modifiedonbehalfby";
			[Relationship("pchmcs_compositions", EntityRole.Referenced, "lk_pchmcs_compositions_createdby", "")]
			public const string lk_pchmcs_compositions_createdby = "lk_pchmcs_compositions_createdby";
			[Relationship("pchmcs_compositions", EntityRole.Referenced, "lk_pchmcs_compositions_createdonbehalfby", "")]
			public const string lk_pchmcs_compositions_createdonbehalfby = "lk_pchmcs_compositions_createdonbehalfby";
			[Relationship("pchmcs_compositions", EntityRole.Referenced, "lk_pchmcs_compositions_modifiedby", "")]
			public const string lk_pchmcs_compositions_modifiedby = "lk_pchmcs_compositions_modifiedby";
			[Relationship("pchmcs_compositions", EntityRole.Referenced, "lk_pchmcs_compositions_modifiedonbehalfby", "")]
			public const string lk_pchmcs_compositions_modifiedonbehalfby = "lk_pchmcs_compositions_modifiedonbehalfby";
			[Relationship("pchmcs_connectionhierarchy", EntityRole.Referenced, "lk_pchmcs_connectionhierarchy_createdby", "")]
			public const string lk_pchmcs_connectionhierarchy_createdby = "lk_pchmcs_connectionhierarchy_createdby";
			[Relationship("pchmcs_connectionhierarchy", EntityRole.Referenced, "lk_pchmcs_connectionhierarchy_createdonbehalfby", "")]
			public const string lk_pchmcs_connectionhierarchy_createdonbehalfby = "lk_pchmcs_connectionhierarchy_createdonbehalfby";
			[Relationship("pchmcs_connectionhierarchy", EntityRole.Referenced, "lk_pchmcs_connectionhierarchy_modifiedby", "")]
			public const string lk_pchmcs_connectionhierarchy_modifiedby = "lk_pchmcs_connectionhierarchy_modifiedby";
			[Relationship("pchmcs_connectionhierarchy", EntityRole.Referenced, "lk_pchmcs_connectionhierarchy_modifiedonbehalfby", "")]
			public const string lk_pchmcs_connectionhierarchy_modifiedonbehalfby = "lk_pchmcs_connectionhierarchy_modifiedonbehalfby";
			[Relationship("pchmcs_conseiller", EntityRole.Referenced, "lk_pchmcs_conseiller_createdby", "")]
			public const string lk_pchmcs_conseiller_createdby = "lk_pchmcs_conseiller_createdby";
			[Relationship("pchmcs_conseiller", EntityRole.Referenced, "lk_pchmcs_conseiller_createdonbehalfby", "")]
			public const string lk_pchmcs_conseiller_createdonbehalfby = "lk_pchmcs_conseiller_createdonbehalfby";
			[Relationship("pchmcs_conseiller", EntityRole.Referenced, "lk_pchmcs_conseiller_modifiedby", "")]
			public const string lk_pchmcs_conseiller_modifiedby = "lk_pchmcs_conseiller_modifiedby";
			[Relationship("pchmcs_conseiller", EntityRole.Referenced, "lk_pchmcs_conseiller_modifiedonbehalfby", "")]
			public const string lk_pchmcs_conseiller_modifiedonbehalfby = "lk_pchmcs_conseiller_modifiedonbehalfby";
			[Relationship("pchmcs_country", EntityRole.Referenced, "lk_pchmcs_country_createdby", "")]
			public const string lk_pchmcs_country_createdby = "lk_pchmcs_country_createdby";
			[Relationship("pchmcs_country", EntityRole.Referenced, "lk_pchmcs_country_createdonbehalfby", "")]
			public const string lk_pchmcs_country_createdonbehalfby = "lk_pchmcs_country_createdonbehalfby";
			[Relationship("pchmcs_country", EntityRole.Referenced, "lk_pchmcs_country_modifiedby", "")]
			public const string lk_pchmcs_country_modifiedby = "lk_pchmcs_country_modifiedby";
			[Relationship("pchmcs_country", EntityRole.Referenced, "lk_pchmcs_country_modifiedonbehalfby", "")]
			public const string lk_pchmcs_country_modifiedonbehalfby = "lk_pchmcs_country_modifiedonbehalfby";
			[Relationship("pchmcs_credentialcache", EntityRole.Referenced, "lk_pchmcs_credentialcache_createdby", "")]
			public const string lk_pchmcs_credentialcache_createdby = "lk_pchmcs_credentialcache_createdby";
			[Relationship("pchmcs_credentialcache", EntityRole.Referenced, "lk_pchmcs_credentialcache_createdonbehalfby", "")]
			public const string lk_pchmcs_credentialcache_createdonbehalfby = "lk_pchmcs_credentialcache_createdonbehalfby";
			[Relationship("pchmcs_credentialcache", EntityRole.Referenced, "lk_pchmcs_credentialcache_modifiedby", "")]
			public const string lk_pchmcs_credentialcache_modifiedby = "lk_pchmcs_credentialcache_modifiedby";
			[Relationship("pchmcs_credentialcache", EntityRole.Referenced, "lk_pchmcs_credentialcache_modifiedonbehalfby", "")]
			public const string lk_pchmcs_credentialcache_modifiedonbehalfby = "lk_pchmcs_credentialcache_modifiedonbehalfby";
			[Relationship("pchmcs_departement", EntityRole.Referenced, "lk_pchmcs_departement_createdby", "")]
			public const string lk_pchmcs_departement_createdby = "lk_pchmcs_departement_createdby";
			[Relationship("pchmcs_departement", EntityRole.Referenced, "lk_pchmcs_departement_createdonbehalfby", "")]
			public const string lk_pchmcs_departement_createdonbehalfby = "lk_pchmcs_departement_createdonbehalfby";
			[Relationship("pchmcs_departement", EntityRole.Referenced, "lk_pchmcs_departement_modifiedby", "")]
			public const string lk_pchmcs_departement_modifiedby = "lk_pchmcs_departement_modifiedby";
			[Relationship("pchmcs_departement", EntityRole.Referenced, "lk_pchmcs_departement_modifiedonbehalfby", "")]
			public const string lk_pchmcs_departement_modifiedonbehalfby = "lk_pchmcs_departement_modifiedonbehalfby";
			[Relationship("pchmcs_dureerdv", EntityRole.Referenced, "lk_pchmcs_dureerdv_createdby", "")]
			public const string lk_pchmcs_dureerdv_createdby = "lk_pchmcs_dureerdv_createdby";
			[Relationship("pchmcs_dureerdv", EntityRole.Referenced, "lk_pchmcs_dureerdv_createdonbehalfby", "")]
			public const string lk_pchmcs_dureerdv_createdonbehalfby = "lk_pchmcs_dureerdv_createdonbehalfby";
			[Relationship("pchmcs_dureerdv", EntityRole.Referenced, "lk_pchmcs_dureerdv_modifiedby", "")]
			public const string lk_pchmcs_dureerdv_modifiedby = "lk_pchmcs_dureerdv_modifiedby";
			[Relationship("pchmcs_dureerdv", EntityRole.Referenced, "lk_pchmcs_dureerdv_modifiedonbehalfby", "")]
			public const string lk_pchmcs_dureerdv_modifiedonbehalfby = "lk_pchmcs_dureerdv_modifiedonbehalfby";
			[Relationship("pchmcs_enfant", EntityRole.Referenced, "lk_pchmcs_enfant_createdby", "")]
			public const string lk_pchmcs_enfant_createdby = "lk_pchmcs_enfant_createdby";
			[Relationship("pchmcs_enfant", EntityRole.Referenced, "lk_pchmcs_enfant_createdonbehalfby", "")]
			public const string lk_pchmcs_enfant_createdonbehalfby = "lk_pchmcs_enfant_createdonbehalfby";
			[Relationship("pchmcs_enfant", EntityRole.Referenced, "lk_pchmcs_enfant_modifiedby", "")]
			public const string lk_pchmcs_enfant_modifiedby = "lk_pchmcs_enfant_modifiedby";
			[Relationship("pchmcs_enfant", EntityRole.Referenced, "lk_pchmcs_enfant_modifiedonbehalfby", "")]
			public const string lk_pchmcs_enfant_modifiedonbehalfby = "lk_pchmcs_enfant_modifiedonbehalfby";
			[Relationship("pchmcs_etagelot", EntityRole.Referenced, "lk_pchmcs_etagelot_createdby", "")]
			public const string lk_pchmcs_etagelot_createdby = "lk_pchmcs_etagelot_createdby";
			[Relationship("pchmcs_etagelot", EntityRole.Referenced, "lk_pchmcs_etagelot_createdonbehalfby", "")]
			public const string lk_pchmcs_etagelot_createdonbehalfby = "lk_pchmcs_etagelot_createdonbehalfby";
			[Relationship("pchmcs_etagelot", EntityRole.Referenced, "lk_pchmcs_etagelot_modifiedby", "")]
			public const string lk_pchmcs_etagelot_modifiedby = "lk_pchmcs_etagelot_modifiedby";
			[Relationship("pchmcs_etagelot", EntityRole.Referenced, "lk_pchmcs_etagelot_modifiedonbehalfby", "")]
			public const string lk_pchmcs_etagelot_modifiedonbehalfby = "lk_pchmcs_etagelot_modifiedonbehalfby";
			[Relationship("pchmcs_etatavancement", EntityRole.Referenced, "lk_pchmcs_etatavancement_createdby", "")]
			public const string lk_pchmcs_etatavancement_createdby = "lk_pchmcs_etatavancement_createdby";
			[Relationship("pchmcs_etatavancement", EntityRole.Referenced, "lk_pchmcs_etatavancement_createdonbehalfby", "")]
			public const string lk_pchmcs_etatavancement_createdonbehalfby = "lk_pchmcs_etatavancement_createdonbehalfby";
			[Relationship("pchmcs_etatavancement", EntityRole.Referenced, "lk_pchmcs_etatavancement_modifiedby", "")]
			public const string lk_pchmcs_etatavancement_modifiedby = "lk_pchmcs_etatavancement_modifiedby";
			[Relationship("pchmcs_etatavancement", EntityRole.Referenced, "lk_pchmcs_etatavancement_modifiedonbehalfby", "")]
			public const string lk_pchmcs_etatavancement_modifiedonbehalfby = "lk_pchmcs_etatavancement_modifiedonbehalfby";
			[Relationship("pchmcs_etatavancementprogramme", EntityRole.Referenced, "lk_pchmcs_etatavancementprogramme_createdby", "")]
			public const string lk_pchmcs_etatavancementprogramme_createdby = "lk_pchmcs_etatavancementprogramme_createdby";
			[Relationship("pchmcs_etatavancementprogramme", EntityRole.Referenced, "lk_pchmcs_etatavancementprogramme_createdonbehalfby", "")]
			public const string lk_pchmcs_etatavancementprogramme_createdonbehalfby = "lk_pchmcs_etatavancementprogramme_createdonbehalfby";
			[Relationship("pchmcs_etatavancementprogramme", EntityRole.Referenced, "lk_pchmcs_etatavancementprogramme_modifiedby", "")]
			public const string lk_pchmcs_etatavancementprogramme_modifiedby = "lk_pchmcs_etatavancementprogramme_modifiedby";
			[Relationship("pchmcs_etatavancementprogramme", EntityRole.Referenced, "lk_pchmcs_etatavancementprogramme_modifiedonbehalfby", "")]
			public const string lk_pchmcs_etatavancementprogramme_modifiedonbehalfby = "lk_pchmcs_etatavancementprogramme_modifiedonbehalfby";
			[Relationship("pchmcs_exposition", EntityRole.Referenced, "lk_pchmcs_exposition_createdby", "")]
			public const string lk_pchmcs_exposition_createdby = "lk_pchmcs_exposition_createdby";
			[Relationship("pchmcs_exposition", EntityRole.Referenced, "lk_pchmcs_exposition_createdonbehalfby", "")]
			public const string lk_pchmcs_exposition_createdonbehalfby = "lk_pchmcs_exposition_createdonbehalfby";
			[Relationship("pchmcs_exposition", EntityRole.Referenced, "lk_pchmcs_exposition_modifiedby", "")]
			public const string lk_pchmcs_exposition_modifiedby = "lk_pchmcs_exposition_modifiedby";
			[Relationship("pchmcs_exposition", EntityRole.Referenced, "lk_pchmcs_exposition_modifiedonbehalfby", "")]
			public const string lk_pchmcs_exposition_modifiedonbehalfby = "lk_pchmcs_exposition_modifiedonbehalfby";
			[Relationship("pchmcs_favoriteprograms", EntityRole.Referenced, "lk_pchmcs_favoriteprograms_createdby", "")]
			public const string lk_pchmcs_favoriteprograms_createdby = "lk_pchmcs_favoriteprograms_createdby";
			[Relationship("pchmcs_favoriteprograms", EntityRole.Referenced, "lk_pchmcs_favoriteprograms_createdonbehalfby", "")]
			public const string lk_pchmcs_favoriteprograms_createdonbehalfby = "lk_pchmcs_favoriteprograms_createdonbehalfby";
			[Relationship("pchmcs_favoriteprograms", EntityRole.Referenced, "lk_pchmcs_favoriteprograms_modifiedby", "")]
			public const string lk_pchmcs_favoriteprograms_modifiedby = "lk_pchmcs_favoriteprograms_modifiedby";
			[Relationship("pchmcs_favoriteprograms", EntityRole.Referenced, "lk_pchmcs_favoriteprograms_modifiedonbehalfby", "")]
			public const string lk_pchmcs_favoriteprograms_modifiedonbehalfby = "lk_pchmcs_favoriteprograms_modifiedonbehalfby";
			[Relationship("pchmcs_fonctionutilisateur", EntityRole.Referenced, "lk_pchmcs_fonctionutilisateur_createdby", "")]
			public const string lk_pchmcs_fonctionutilisateur_createdby = "lk_pchmcs_fonctionutilisateur_createdby";
			[Relationship("pchmcs_fonctionutilisateur", EntityRole.Referenced, "lk_pchmcs_fonctionutilisateur_createdonbehalfby", "")]
			public const string lk_pchmcs_fonctionutilisateur_createdonbehalfby = "lk_pchmcs_fonctionutilisateur_createdonbehalfby";
			[Relationship("pchmcs_fonctionutilisateur", EntityRole.Referenced, "lk_pchmcs_fonctionutilisateur_modifiedby", "")]
			public const string lk_pchmcs_fonctionutilisateur_modifiedby = "lk_pchmcs_fonctionutilisateur_modifiedby";
			[Relationship("pchmcs_fonctionutilisateur", EntityRole.Referenced, "lk_pchmcs_fonctionutilisateur_modifiedonbehalfby", "")]
			public const string lk_pchmcs_fonctionutilisateur_modifiedonbehalfby = "lk_pchmcs_fonctionutilisateur_modifiedonbehalfby";
			[Relationship("pchmcs_gamme", EntityRole.Referenced, "lk_pchmcs_gamme_createdby", "")]
			public const string lk_pchmcs_gamme_createdby = "lk_pchmcs_gamme_createdby";
			[Relationship("pchmcs_gamme", EntityRole.Referenced, "lk_pchmcs_gamme_createdonbehalfby", "")]
			public const string lk_pchmcs_gamme_createdonbehalfby = "lk_pchmcs_gamme_createdonbehalfby";
			[Relationship("pchmcs_gamme", EntityRole.Referenced, "lk_pchmcs_gamme_modifiedby", "")]
			public const string lk_pchmcs_gamme_modifiedby = "lk_pchmcs_gamme_modifiedby";
			[Relationship("pchmcs_gamme", EntityRole.Referenced, "lk_pchmcs_gamme_modifiedonbehalfby", "")]
			public const string lk_pchmcs_gamme_modifiedonbehalfby = "lk_pchmcs_gamme_modifiedonbehalfby";
			[Relationship("pchmcs_historiquestatut", EntityRole.Referenced, "lk_pchmcs_historiquestatut_createdby", "")]
			public const string lk_pchmcs_historiquestatut_createdby = "lk_pchmcs_historiquestatut_createdby";
			[Relationship("pchmcs_historiquestatut", EntityRole.Referenced, "lk_pchmcs_historiquestatut_createdonbehalfby", "")]
			public const string lk_pchmcs_historiquestatut_createdonbehalfby = "lk_pchmcs_historiquestatut_createdonbehalfby";
			[Relationship("pchmcs_historiquestatut", EntityRole.Referenced, "lk_pchmcs_historiquestatut_modifiedby", "")]
			public const string lk_pchmcs_historiquestatut_modifiedby = "lk_pchmcs_historiquestatut_modifiedby";
			[Relationship("pchmcs_historiquestatut", EntityRole.Referenced, "lk_pchmcs_historiquestatut_modifiedonbehalfby", "")]
			public const string lk_pchmcs_historiquestatut_modifiedonbehalfby = "lk_pchmcs_historiquestatut_modifiedonbehalfby";
			[Relationship("pchmcs_historiquestatutinteret", EntityRole.Referenced, "lk_pchmcs_historiquestatutinteret_createdby", "")]
			public const string lk_pchmcs_historiquestatutinteret_createdby = "lk_pchmcs_historiquestatutinteret_createdby";
			[Relationship("pchmcs_historiquestatutinteret", EntityRole.Referenced, "lk_pchmcs_historiquestatutinteret_createdonbehalfby", "")]
			public const string lk_pchmcs_historiquestatutinteret_createdonbehalfby = "lk_pchmcs_historiquestatutinteret_createdonbehalfby";
			[Relationship("pchmcs_historiquestatutinteret", EntityRole.Referenced, "lk_pchmcs_historiquestatutinteret_modifiedby", "")]
			public const string lk_pchmcs_historiquestatutinteret_modifiedby = "lk_pchmcs_historiquestatutinteret_modifiedby";
			[Relationship("pchmcs_historiquestatutinteret", EntityRole.Referenced, "lk_pchmcs_historiquestatutinteret_modifiedonbehalfby", "")]
			public const string lk_pchmcs_historiquestatutinteret_modifiedonbehalfby = "lk_pchmcs_historiquestatutinteret_modifiedonbehalfby";
			[Relationship("pchmcs_interet", EntityRole.Referenced, "lk_pchmcs_interet_createdby", "")]
			public const string lk_pchmcs_interet_createdby = "lk_pchmcs_interet_createdby";
			[Relationship("pchmcs_interet", EntityRole.Referenced, "lk_pchmcs_interet_createdonbehalfby", "")]
			public const string lk_pchmcs_interet_createdonbehalfby = "lk_pchmcs_interet_createdonbehalfby";
			[Relationship("pchmcs_interet", EntityRole.Referenced, "lk_pchmcs_interet_modifiedby", "")]
			public const string lk_pchmcs_interet_modifiedby = "lk_pchmcs_interet_modifiedby";
			[Relationship("pchmcs_interet", EntityRole.Referenced, "lk_pchmcs_interet_modifiedonbehalfby", "")]
			public const string lk_pchmcs_interet_modifiedonbehalfby = "lk_pchmcs_interet_modifiedonbehalfby";
			[Relationship("pchmcs_interfacesed", EntityRole.Referenced, "lk_pchmcs_interfacesed_createdby", "")]
			public const string lk_pchmcs_interfacesed_createdby = "lk_pchmcs_interfacesed_createdby";
			[Relationship("pchmcs_interfacesed", EntityRole.Referenced, "lk_pchmcs_interfacesed_createdonbehalfby", "")]
			public const string lk_pchmcs_interfacesed_createdonbehalfby = "lk_pchmcs_interfacesed_createdonbehalfby";
			[Relationship("pchmcs_interfacesed", EntityRole.Referenced, "lk_pchmcs_interfacesed_modifiedby", "")]
			public const string lk_pchmcs_interfacesed_modifiedby = "lk_pchmcs_interfacesed_modifiedby";
			[Relationship("pchmcs_interfacesed", EntityRole.Referenced, "lk_pchmcs_interfacesed_modifiedonbehalfby", "")]
			public const string lk_pchmcs_interfacesed_modifiedonbehalfby = "lk_pchmcs_interfacesed_modifiedonbehalfby";
			[Relationship("pchmcs_intervenant", EntityRole.Referenced, "lk_pchmcs_intervenant_createdby", "")]
			public const string lk_pchmcs_intervenant_createdby = "lk_pchmcs_intervenant_createdby";
			[Relationship("pchmcs_intervenant", EntityRole.Referenced, "lk_pchmcs_intervenant_createdonbehalfby", "")]
			public const string lk_pchmcs_intervenant_createdonbehalfby = "lk_pchmcs_intervenant_createdonbehalfby";
			[Relationship("pchmcs_intervenant", EntityRole.Referenced, "lk_pchmcs_intervenant_modifiedby", "")]
			public const string lk_pchmcs_intervenant_modifiedby = "lk_pchmcs_intervenant_modifiedby";
			[Relationship("pchmcs_intervenant", EntityRole.Referenced, "lk_pchmcs_intervenant_modifiedonbehalfby", "")]
			public const string lk_pchmcs_intervenant_modifiedonbehalfby = "lk_pchmcs_intervenant_modifiedonbehalfby";
			[Relationship("pchmcs_jourindisponibilitcdc", EntityRole.Referenced, "lk_pchmcs_jourindisponibilitcdc_createdby", "")]
			public const string lk_pchmcs_jourindisponibilitcdc_createdby = "lk_pchmcs_jourindisponibilitcdc_createdby";
			[Relationship("pchmcs_jourindisponibilitcdc", EntityRole.Referenced, "lk_pchmcs_jourindisponibilitcdc_createdonbehalfby", "")]
			public const string lk_pchmcs_jourindisponibilitcdc_createdonbehalfby = "lk_pchmcs_jourindisponibilitcdc_createdonbehalfby";
			[Relationship("pchmcs_jourindisponibilitcdc", EntityRole.Referenced, "lk_pchmcs_jourindisponibilitcdc_modifiedby", "")]
			public const string lk_pchmcs_jourindisponibilitcdc_modifiedby = "lk_pchmcs_jourindisponibilitcdc_modifiedby";
			[Relationship("pchmcs_jourindisponibilitcdc", EntityRole.Referenced, "lk_pchmcs_jourindisponibilitcdc_modifiedonbehalfby", "")]
			public const string lk_pchmcs_jourindisponibilitcdc_modifiedonbehalfby = "lk_pchmcs_jourindisponibilitcdc_modifiedonbehalfby";
			[Relationship("pchmcs_lieudevente", EntityRole.Referenced, "lk_pchmcs_lieudevente_createdby", "")]
			public const string lk_pchmcs_lieudevente_createdby = "lk_pchmcs_lieudevente_createdby";
			[Relationship("pchmcs_lieudevente", EntityRole.Referenced, "lk_pchmcs_lieudevente_createdonbehalfby", "")]
			public const string lk_pchmcs_lieudevente_createdonbehalfby = "lk_pchmcs_lieudevente_createdonbehalfby";
			[Relationship("pchmcs_lieudevente", EntityRole.Referenced, "lk_pchmcs_lieudevente_modifiedby", "")]
			public const string lk_pchmcs_lieudevente_modifiedby = "lk_pchmcs_lieudevente_modifiedby";
			[Relationship("pchmcs_lieudevente", EntityRole.Referenced, "lk_pchmcs_lieudevente_modifiedonbehalfby", "")]
			public const string lk_pchmcs_lieudevente_modifiedonbehalfby = "lk_pchmcs_lieudevente_modifiedonbehalfby";
			[Relationship("pchmcs_logerreurkiamo", EntityRole.Referenced, "lk_pchmcs_logerreurkiamo_createdby", "")]
			public const string lk_pchmcs_logerreurkiamo_createdby = "lk_pchmcs_logerreurkiamo_createdby";
			[Relationship("pchmcs_logerreurkiamo", EntityRole.Referenced, "lk_pchmcs_logerreurkiamo_createdonbehalfby", "")]
			public const string lk_pchmcs_logerreurkiamo_createdonbehalfby = "lk_pchmcs_logerreurkiamo_createdonbehalfby";
			[Relationship("pchmcs_logerreurkiamo", EntityRole.Referenced, "lk_pchmcs_logerreurkiamo_modifiedby", "")]
			public const string lk_pchmcs_logerreurkiamo_modifiedby = "lk_pchmcs_logerreurkiamo_modifiedby";
			[Relationship("pchmcs_logerreurkiamo", EntityRole.Referenced, "lk_pchmcs_logerreurkiamo_modifiedonbehalfby", "")]
			public const string lk_pchmcs_logerreurkiamo_modifiedonbehalfby = "lk_pchmcs_logerreurkiamo_modifiedonbehalfby";
			[Relationship("pchmcs_lot", EntityRole.Referenced, "lk_pchmcs_lot_createdby", "")]
			public const string lk_pchmcs_lot_createdby = "lk_pchmcs_lot_createdby";
			[Relationship("pchmcs_lot", EntityRole.Referenced, "lk_pchmcs_lot_createdonbehalfby", "")]
			public const string lk_pchmcs_lot_createdonbehalfby = "lk_pchmcs_lot_createdonbehalfby";
			[Relationship("pchmcs_lot", EntityRole.Referenced, "lk_pchmcs_lot_modifiedby", "")]
			public const string lk_pchmcs_lot_modifiedby = "lk_pchmcs_lot_modifiedby";
			[Relationship("pchmcs_lot", EntityRole.Referenced, "lk_pchmcs_lot_modifiedonbehalfby", "")]
			public const string lk_pchmcs_lot_modifiedonbehalfby = "lk_pchmcs_lot_modifiedonbehalfby";
			[Relationship("pchmcs_lotconcern", EntityRole.Referenced, "lk_pchmcs_lotconcern_createdby", "")]
			public const string lk_pchmcs_lotconcern_createdby = "lk_pchmcs_lotconcern_createdby";
			[Relationship("pchmcs_lotconcern", EntityRole.Referenced, "lk_pchmcs_lotconcern_createdonbehalfby", "")]
			public const string lk_pchmcs_lotconcern_createdonbehalfby = "lk_pchmcs_lotconcern_createdonbehalfby";
			[Relationship("pchmcs_lotconcern", EntityRole.Referenced, "lk_pchmcs_lotconcern_modifiedby", "")]
			public const string lk_pchmcs_lotconcern_modifiedby = "lk_pchmcs_lotconcern_modifiedby";
			[Relationship("pchmcs_lotconcern", EntityRole.Referenced, "lk_pchmcs_lotconcern_modifiedonbehalfby", "")]
			public const string lk_pchmcs_lotconcern_modifiedonbehalfby = "lk_pchmcs_lotconcern_modifiedonbehalfby";
			[Relationship("pchmcs_masterbusinessunit", EntityRole.Referenced, "lk_pchmcs_masterbusinessunit_createdby", "")]
			public const string lk_pchmcs_masterbusinessunit_createdby = "lk_pchmcs_masterbusinessunit_createdby";
			[Relationship("pchmcs_masterbusinessunit", EntityRole.Referenced, "lk_pchmcs_masterbusinessunit_createdonbehalfby", "")]
			public const string lk_pchmcs_masterbusinessunit_createdonbehalfby = "lk_pchmcs_masterbusinessunit_createdonbehalfby";
			[Relationship("pchmcs_masterbusinessunit", EntityRole.Referenced, "lk_pchmcs_masterbusinessunit_modifiedby", "")]
			public const string lk_pchmcs_masterbusinessunit_modifiedby = "lk_pchmcs_masterbusinessunit_modifiedby";
			[Relationship("pchmcs_masterbusinessunit", EntityRole.Referenced, "lk_pchmcs_masterbusinessunit_modifiedonbehalfby", "")]
			public const string lk_pchmcs_masterbusinessunit_modifiedonbehalfby = "lk_pchmcs_masterbusinessunit_modifiedonbehalfby";
			[Relationship("pchmcs_matricecloturerdv", EntityRole.Referenced, "lk_pchmcs_matricecloturerdv_createdby", "")]
			public const string lk_pchmcs_matricecloturerdv_createdby = "lk_pchmcs_matricecloturerdv_createdby";
			[Relationship("pchmcs_matricecloturerdv", EntityRole.Referenced, "lk_pchmcs_matricecloturerdv_createdonbehalfby", "")]
			public const string lk_pchmcs_matricecloturerdv_createdonbehalfby = "lk_pchmcs_matricecloturerdv_createdonbehalfby";
			[Relationship("pchmcs_matricecloturerdv", EntityRole.Referenced, "lk_pchmcs_matricecloturerdv_modifiedby", "")]
			public const string lk_pchmcs_matricecloturerdv_modifiedby = "lk_pchmcs_matricecloturerdv_modifiedby";
			[Relationship("pchmcs_matricecloturerdv", EntityRole.Referenced, "lk_pchmcs_matricecloturerdv_modifiedonbehalfby", "")]
			public const string lk_pchmcs_matricecloturerdv_modifiedonbehalfby = "lk_pchmcs_matricecloturerdv_modifiedonbehalfby";
			[Relationship("pchmcs_matriceremuneration", EntityRole.Referenced, "lk_pchmcs_matriceremuneration_createdby", "")]
			public const string lk_pchmcs_matriceremuneration_createdby = "lk_pchmcs_matriceremuneration_createdby";
			[Relationship("pchmcs_matriceremuneration", EntityRole.Referenced, "lk_pchmcs_matriceremuneration_createdonbehalfby", "")]
			public const string lk_pchmcs_matriceremuneration_createdonbehalfby = "lk_pchmcs_matriceremuneration_createdonbehalfby";
			[Relationship("pchmcs_matriceremuneration", EntityRole.Referenced, "lk_pchmcs_matriceremuneration_modifiedby", "")]
			public const string lk_pchmcs_matriceremuneration_modifiedby = "lk_pchmcs_matriceremuneration_modifiedby";
			[Relationship("pchmcs_matriceremuneration", EntityRole.Referenced, "lk_pchmcs_matriceremuneration_modifiedonbehalfby", "")]
			public const string lk_pchmcs_matriceremuneration_modifiedonbehalfby = "lk_pchmcs_matriceremuneration_modifiedonbehalfby";
			[Relationship("pchmcs_modelesms", EntityRole.Referenced, "lk_pchmcs_modelesms_createdby", "")]
			public const string lk_pchmcs_modelesms_createdby = "lk_pchmcs_modelesms_createdby";
			[Relationship("pchmcs_modelesms", EntityRole.Referenced, "lk_pchmcs_modelesms_createdonbehalfby", "")]
			public const string lk_pchmcs_modelesms_createdonbehalfby = "lk_pchmcs_modelesms_createdonbehalfby";
			[Relationship("pchmcs_modelesms", EntityRole.Referenced, "lk_pchmcs_modelesms_modifiedby", "")]
			public const string lk_pchmcs_modelesms_modifiedby = "lk_pchmcs_modelesms_modifiedby";
			[Relationship("pchmcs_modelesms", EntityRole.Referenced, "lk_pchmcs_modelesms_modifiedonbehalfby", "")]
			public const string lk_pchmcs_modelesms_modifiedonbehalfby = "lk_pchmcs_modelesms_modifiedonbehalfby";
			[Relationship("pchmcs_natureoffre", EntityRole.Referenced, "lk_pchmcs_natureoffre_createdby", "")]
			public const string lk_pchmcs_natureoffre_createdby = "lk_pchmcs_natureoffre_createdby";
			[Relationship("pchmcs_natureoffre", EntityRole.Referenced, "lk_pchmcs_natureoffre_createdonbehalfby", "")]
			public const string lk_pchmcs_natureoffre_createdonbehalfby = "lk_pchmcs_natureoffre_createdonbehalfby";
			[Relationship("pchmcs_natureoffre", EntityRole.Referenced, "lk_pchmcs_natureoffre_modifiedby", "")]
			public const string lk_pchmcs_natureoffre_modifiedby = "lk_pchmcs_natureoffre_modifiedby";
			[Relationship("pchmcs_natureoffre", EntityRole.Referenced, "lk_pchmcs_natureoffre_modifiedonbehalfby", "")]
			public const string lk_pchmcs_natureoffre_modifiedonbehalfby = "lk_pchmcs_natureoffre_modifiedonbehalfby";
			[Relationship("pchmcs_notification", EntityRole.Referenced, "lk_pchmcs_notification_createdby", "")]
			public const string lk_pchmcs_notification_createdby = "lk_pchmcs_notification_createdby";
			[Relationship("pchmcs_notification", EntityRole.Referenced, "lk_pchmcs_notification_createdonbehalfby", "")]
			public const string lk_pchmcs_notification_createdonbehalfby = "lk_pchmcs_notification_createdonbehalfby";
			[Relationship("pchmcs_notification", EntityRole.Referenced, "lk_pchmcs_notification_modifiedby", "")]
			public const string lk_pchmcs_notification_modifiedby = "lk_pchmcs_notification_modifiedby";
			[Relationship("pchmcs_notification", EntityRole.Referenced, "lk_pchmcs_notification_modifiedonbehalfby", "")]
			public const string lk_pchmcs_notification_modifiedonbehalfby = "lk_pchmcs_notification_modifiedonbehalfby";
			[Relationship("pchmcs_notifieduser", EntityRole.Referenced, "lk_pchmcs_notifieduser_createdby", "")]
			public const string lk_pchmcs_notifieduser_createdby = "lk_pchmcs_notifieduser_createdby";
			[Relationship("pchmcs_notifieduser", EntityRole.Referenced, "lk_pchmcs_notifieduser_createdonbehalfby", "")]
			public const string lk_pchmcs_notifieduser_createdonbehalfby = "lk_pchmcs_notifieduser_createdonbehalfby";
			[Relationship("pchmcs_notifieduser", EntityRole.Referenced, "lk_pchmcs_notifieduser_modifiedby", "")]
			public const string lk_pchmcs_notifieduser_modifiedby = "lk_pchmcs_notifieduser_modifiedby";
			[Relationship("pchmcs_notifieduser", EntityRole.Referenced, "lk_pchmcs_notifieduser_modifiedonbehalfby", "")]
			public const string lk_pchmcs_notifieduser_modifiedonbehalfby = "lk_pchmcs_notifieduser_modifiedonbehalfby";
			[Relationship("pchmcs_offre", EntityRole.Referenced, "lk_pchmcs_offre_createdby", "")]
			public const string lk_pchmcs_offre_createdby = "lk_pchmcs_offre_createdby";
			[Relationship("pchmcs_offre", EntityRole.Referenced, "lk_pchmcs_offre_createdonbehalfby", "")]
			public const string lk_pchmcs_offre_createdonbehalfby = "lk_pchmcs_offre_createdonbehalfby";
			[Relationship("pchmcs_offre", EntityRole.Referenced, "lk_pchmcs_offre_modifiedby", "")]
			public const string lk_pchmcs_offre_modifiedby = "lk_pchmcs_offre_modifiedby";
			[Relationship("pchmcs_offre", EntityRole.Referenced, "lk_pchmcs_offre_modifiedonbehalfby", "")]
			public const string lk_pchmcs_offre_modifiedonbehalfby = "lk_pchmcs_offre_modifiedonbehalfby";
			[Relationship("pchmcs_offresducyclesdevente", EntityRole.Referenced, "lk_pchmcs_offresducyclesdevente_createdby", "")]
			public const string lk_pchmcs_offresducyclesdevente_createdby = "lk_pchmcs_offresducyclesdevente_createdby";
			[Relationship("pchmcs_offresducyclesdevente", EntityRole.Referenced, "lk_pchmcs_offresducyclesdevente_createdonbehalfby", "")]
			public const string lk_pchmcs_offresducyclesdevente_createdonbehalfby = "lk_pchmcs_offresducyclesdevente_createdonbehalfby";
			[Relationship("pchmcs_offresducyclesdevente", EntityRole.Referenced, "lk_pchmcs_offresducyclesdevente_modifiedby", "")]
			public const string lk_pchmcs_offresducyclesdevente_modifiedby = "lk_pchmcs_offresducyclesdevente_modifiedby";
			[Relationship("pchmcs_offresducyclesdevente", EntityRole.Referenced, "lk_pchmcs_offresducyclesdevente_modifiedonbehalfby", "")]
			public const string lk_pchmcs_offresducyclesdevente_modifiedonbehalfby = "lk_pchmcs_offresducyclesdevente_modifiedonbehalfby";
			[Relationship("pchmcs_origine", EntityRole.Referenced, "lk_pchmcs_origine_createdby", "")]
			public const string lk_pchmcs_origine_createdby = "lk_pchmcs_origine_createdby";
			[Relationship("pchmcs_origine", EntityRole.Referenced, "lk_pchmcs_origine_createdonbehalfby", "")]
			public const string lk_pchmcs_origine_createdonbehalfby = "lk_pchmcs_origine_createdonbehalfby";
			[Relationship("pchmcs_origine", EntityRole.Referenced, "lk_pchmcs_origine_modifiedby", "")]
			public const string lk_pchmcs_origine_modifiedby = "lk_pchmcs_origine_modifiedby";
			[Relationship("pchmcs_origine", EntityRole.Referenced, "lk_pchmcs_origine_modifiedonbehalfby", "")]
			public const string lk_pchmcs_origine_modifiedonbehalfby = "lk_pchmcs_origine_modifiedonbehalfby";
			[Relationship("pchmcs_parameters", EntityRole.Referenced, "lk_pchmcs_parameters_createdby", "")]
			public const string lk_pchmcs_parameters_createdby = "lk_pchmcs_parameters_createdby";
			[Relationship("pchmcs_parameters", EntityRole.Referenced, "lk_pchmcs_parameters_createdonbehalfby", "")]
			public const string lk_pchmcs_parameters_createdonbehalfby = "lk_pchmcs_parameters_createdonbehalfby";
			[Relationship("pchmcs_parameters", EntityRole.Referenced, "lk_pchmcs_parameters_modifiedby", "")]
			public const string lk_pchmcs_parameters_modifiedby = "lk_pchmcs_parameters_modifiedby";
			[Relationship("pchmcs_parameters", EntityRole.Referenced, "lk_pchmcs_parameters_modifiedonbehalfby", "")]
			public const string lk_pchmcs_parameters_modifiedonbehalfby = "lk_pchmcs_parameters_modifiedonbehalfby";
			[Relationship("pchmcs_placement", EntityRole.Referenced, "lk_pchmcs_placement_createdby", "")]
			public const string lk_pchmcs_placement_createdby = "lk_pchmcs_placement_createdby";
			[Relationship("pchmcs_placement", EntityRole.Referenced, "lk_pchmcs_placement_createdonbehalfby", "")]
			public const string lk_pchmcs_placement_createdonbehalfby = "lk_pchmcs_placement_createdonbehalfby";
			[Relationship("pchmcs_placement", EntityRole.Referenced, "lk_pchmcs_placement_modifiedby", "")]
			public const string lk_pchmcs_placement_modifiedby = "lk_pchmcs_placement_modifiedby";
			[Relationship("pchmcs_placement", EntityRole.Referenced, "lk_pchmcs_placement_modifiedonbehalfby", "")]
			public const string lk_pchmcs_placement_modifiedonbehalfby = "lk_pchmcs_placement_modifiedonbehalfby";
			[Relationship("pchmcs_populationeligible", EntityRole.Referenced, "lk_pchmcs_populationeligible_createdby", "")]
			public const string lk_pchmcs_populationeligible_createdby = "lk_pchmcs_populationeligible_createdby";
			[Relationship("pchmcs_populationeligible", EntityRole.Referenced, "lk_pchmcs_populationeligible_createdonbehalfby", "")]
			public const string lk_pchmcs_populationeligible_createdonbehalfby = "lk_pchmcs_populationeligible_createdonbehalfby";
			[Relationship("pchmcs_populationeligible", EntityRole.Referenced, "lk_pchmcs_populationeligible_modifiedby", "")]
			public const string lk_pchmcs_populationeligible_modifiedby = "lk_pchmcs_populationeligible_modifiedby";
			[Relationship("pchmcs_populationeligible", EntityRole.Referenced, "lk_pchmcs_populationeligible_modifiedonbehalfby", "")]
			public const string lk_pchmcs_populationeligible_modifiedonbehalfby = "lk_pchmcs_populationeligible_modifiedonbehalfby";
			[Relationship("pchmcs_postalcodecity", EntityRole.Referenced, "lk_pchmcs_postalcodecity_createdby", "")]
			public const string lk_pchmcs_postalcodecity_createdby = "lk_pchmcs_postalcodecity_createdby";
			[Relationship("pchmcs_postalcodecity", EntityRole.Referenced, "lk_pchmcs_postalcodecity_createdonbehalfby", "")]
			public const string lk_pchmcs_postalcodecity_createdonbehalfby = "lk_pchmcs_postalcodecity_createdonbehalfby";
			[Relationship("pchmcs_postalcodecity", EntityRole.Referenced, "lk_pchmcs_postalcodecity_modifiedby", "")]
			public const string lk_pchmcs_postalcodecity_modifiedby = "lk_pchmcs_postalcodecity_modifiedby";
			[Relationship("pchmcs_postalcodecity", EntityRole.Referenced, "lk_pchmcs_postalcodecity_modifiedonbehalfby", "")]
			public const string lk_pchmcs_postalcodecity_modifiedonbehalfby = "lk_pchmcs_postalcodecity_modifiedonbehalfby";
			[Relationship("pchmcs_preferencesespperso", EntityRole.Referenced, "lk_pchmcs_preferencesespperso_createdby", "")]
			public const string lk_pchmcs_preferencesespperso_createdby = "lk_pchmcs_preferencesespperso_createdby";
			[Relationship("pchmcs_preferencesespperso", EntityRole.Referenced, "lk_pchmcs_preferencesespperso_createdonbehalfby", "")]
			public const string lk_pchmcs_preferencesespperso_createdonbehalfby = "lk_pchmcs_preferencesespperso_createdonbehalfby";
			[Relationship("pchmcs_preferencesespperso", EntityRole.Referenced, "lk_pchmcs_preferencesespperso_modifiedby", "")]
			public const string lk_pchmcs_preferencesespperso_modifiedby = "lk_pchmcs_preferencesespperso_modifiedby";
			[Relationship("pchmcs_preferencesespperso", EntityRole.Referenced, "lk_pchmcs_preferencesespperso_modifiedonbehalfby", "")]
			public const string lk_pchmcs_preferencesespperso_modifiedonbehalfby = "lk_pchmcs_preferencesespperso_modifiedonbehalfby";
			[Relationship("pchmcs_preferencesnotificationsep", EntityRole.Referenced, "lk_pchmcs_preferencesnotificationsep_createdby", "")]
			public const string lk_pchmcs_preferencesnotificationsep_createdby = "lk_pchmcs_preferencesnotificationsep_createdby";
			[Relationship("pchmcs_preferencesnotificationsep", EntityRole.Referenced, "lk_pchmcs_preferencesnotificationsep_createdonbehalfby", "")]
			public const string lk_pchmcs_preferencesnotificationsep_createdonbehalfby = "lk_pchmcs_preferencesnotificationsep_createdonbehalfby";
			[Relationship("pchmcs_preferencesnotificationsep", EntityRole.Referenced, "lk_pchmcs_preferencesnotificationsep_modifiedby", "")]
			public const string lk_pchmcs_preferencesnotificationsep_modifiedby = "lk_pchmcs_preferencesnotificationsep_modifiedby";
			[Relationship("pchmcs_preferencesnotificationsep", EntityRole.Referenced, "lk_pchmcs_preferencesnotificationsep_modifiedonbehalfby", "")]
			public const string lk_pchmcs_preferencesnotificationsep_modifiedonbehalfby = "lk_pchmcs_preferencesnotificationsep_modifiedonbehalfby";
			[Relationship("pchmcs_prescripteur", EntityRole.Referenced, "lk_pchmcs_prescripteur_createdby", "")]
			public const string lk_pchmcs_prescripteur_createdby = "lk_pchmcs_prescripteur_createdby";
			[Relationship("pchmcs_prescripteur", EntityRole.Referenced, "lk_pchmcs_prescripteur_createdonbehalfby", "")]
			public const string lk_pchmcs_prescripteur_createdonbehalfby = "lk_pchmcs_prescripteur_createdonbehalfby";
			[Relationship("pchmcs_prescripteur", EntityRole.Referenced, "lk_pchmcs_prescripteur_modifiedby", "")]
			public const string lk_pchmcs_prescripteur_modifiedby = "lk_pchmcs_prescripteur_modifiedby";
			[Relationship("pchmcs_prescripteur", EntityRole.Referenced, "lk_pchmcs_prescripteur_modifiedonbehalfby", "")]
			public const string lk_pchmcs_prescripteur_modifiedonbehalfby = "lk_pchmcs_prescripteur_modifiedonbehalfby";
			[Relationship("pchmcs_programme", EntityRole.Referenced, "lk_pchmcs_programme_createdby", "")]
			public const string lk_pchmcs_programme_createdby = "lk_pchmcs_programme_createdby";
			[Relationship("pchmcs_programme", EntityRole.Referenced, "lk_pchmcs_programme_createdonbehalfby", "")]
			public const string lk_pchmcs_programme_createdonbehalfby = "lk_pchmcs_programme_createdonbehalfby";
			[Relationship("pchmcs_programme", EntityRole.Referenced, "lk_pchmcs_programme_modifiedby", "")]
			public const string lk_pchmcs_programme_modifiedby = "lk_pchmcs_programme_modifiedby";
			[Relationship("pchmcs_programme", EntityRole.Referenced, "lk_pchmcs_programme_modifiedonbehalfby", "")]
			public const string lk_pchmcs_programme_modifiedonbehalfby = "lk_pchmcs_programme_modifiedonbehalfby";
			[Relationship("pchmcs_programmecdc", EntityRole.Referenced, "lk_pchmcs_programmecdc_createdby", "")]
			public const string lk_pchmcs_programmecdc_createdby = "lk_pchmcs_programmecdc_createdby";
			[Relationship("pchmcs_programmecdc", EntityRole.Referenced, "lk_pchmcs_programmecdc_createdonbehalfby", "")]
			public const string lk_pchmcs_programmecdc_createdonbehalfby = "lk_pchmcs_programmecdc_createdonbehalfby";
			[Relationship("pchmcs_programmecdc", EntityRole.Referenced, "lk_pchmcs_programmecdc_modifiedby", "")]
			public const string lk_pchmcs_programmecdc_modifiedby = "lk_pchmcs_programmecdc_modifiedby";
			[Relationship("pchmcs_programmecdc", EntityRole.Referenced, "lk_pchmcs_programmecdc_modifiedonbehalfby", "")]
			public const string lk_pchmcs_programmecdc_modifiedonbehalfby = "lk_pchmcs_programmecdc_modifiedonbehalfby";
			[Relationship("pchmcs_programmesinteret", EntityRole.Referenced, "lk_pchmcs_programmesinteret_createdby", "")]
			public const string lk_pchmcs_programmesinteret_createdby = "lk_pchmcs_programmesinteret_createdby";
			[Relationship("pchmcs_programmesinteret", EntityRole.Referenced, "lk_pchmcs_programmesinteret_createdonbehalfby", "")]
			public const string lk_pchmcs_programmesinteret_createdonbehalfby = "lk_pchmcs_programmesinteret_createdonbehalfby";
			[Relationship("pchmcs_programmesinteret", EntityRole.Referenced, "lk_pchmcs_programmesinteret_modifiedby", "")]
			public const string lk_pchmcs_programmesinteret_modifiedby = "lk_pchmcs_programmesinteret_modifiedby";
			[Relationship("pchmcs_programmesinteret", EntityRole.Referenced, "lk_pchmcs_programmesinteret_modifiedonbehalfby", "")]
			public const string lk_pchmcs_programmesinteret_modifiedonbehalfby = "lk_pchmcs_programmesinteret_modifiedonbehalfby";
			[Relationship("pchmcs_rdvcdc", EntityRole.Referenced, "lk_pchmcs_rdvcdc_createdby", "")]
			public const string lk_pchmcs_rdvcdc_createdby = "lk_pchmcs_rdvcdc_createdby";
			[Relationship("pchmcs_rdvcdc", EntityRole.Referenced, "lk_pchmcs_rdvcdc_createdonbehalfby", "")]
			public const string lk_pchmcs_rdvcdc_createdonbehalfby = "lk_pchmcs_rdvcdc_createdonbehalfby";
			[Relationship("pchmcs_rdvcdc", EntityRole.Referenced, "lk_pchmcs_rdvcdc_modifiedby", "")]
			public const string lk_pchmcs_rdvcdc_modifiedby = "lk_pchmcs_rdvcdc_modifiedby";
			[Relationship("pchmcs_rdvcdc", EntityRole.Referenced, "lk_pchmcs_rdvcdc_modifiedonbehalfby", "")]
			public const string lk_pchmcs_rdvcdc_modifiedonbehalfby = "lk_pchmcs_rdvcdc_modifiedonbehalfby";
			[Relationship("pchmcs_realtynature", EntityRole.Referenced, "lk_pchmcs_realtynature_createdby", "")]
			public const string lk_pchmcs_realtynature_createdby = "lk_pchmcs_realtynature_createdby";
			[Relationship("pchmcs_realtynature", EntityRole.Referenced, "lk_pchmcs_realtynature_createdonbehalfby", "")]
			public const string lk_pchmcs_realtynature_createdonbehalfby = "lk_pchmcs_realtynature_createdonbehalfby";
			[Relationship("pchmcs_realtynature", EntityRole.Referenced, "lk_pchmcs_realtynature_modifiedby", "")]
			public const string lk_pchmcs_realtynature_modifiedby = "lk_pchmcs_realtynature_modifiedby";
			[Relationship("pchmcs_realtynature", EntityRole.Referenced, "lk_pchmcs_realtynature_modifiedonbehalfby", "")]
			public const string lk_pchmcs_realtynature_modifiedonbehalfby = "lk_pchmcs_realtynature_modifiedonbehalfby";
			[Relationship("pchmcs_rechercheoffre", EntityRole.Referenced, "lk_pchmcs_rechercheoffre_createdby", "")]
			public const string lk_pchmcs_rechercheoffre_createdby = "lk_pchmcs_rechercheoffre_createdby";
			[Relationship("pchmcs_rechercheoffre", EntityRole.Referenced, "lk_pchmcs_rechercheoffre_createdonbehalfby", "")]
			public const string lk_pchmcs_rechercheoffre_createdonbehalfby = "lk_pchmcs_rechercheoffre_createdonbehalfby";
			[Relationship("pchmcs_rechercheoffre", EntityRole.Referenced, "lk_pchmcs_rechercheoffre_modifiedby", "")]
			public const string lk_pchmcs_rechercheoffre_modifiedby = "lk_pchmcs_rechercheoffre_modifiedby";
			[Relationship("pchmcs_rechercheoffre", EntityRole.Referenced, "lk_pchmcs_rechercheoffre_modifiedonbehalfby", "")]
			public const string lk_pchmcs_rechercheoffre_modifiedonbehalfby = "lk_pchmcs_rechercheoffre_modifiedonbehalfby";
			[Relationship("pchmcs_region", EntityRole.Referenced, "lk_pchmcs_region_createdby", "")]
			public const string lk_pchmcs_region_createdby = "lk_pchmcs_region_createdby";
			[Relationship("pchmcs_region", EntityRole.Referenced, "lk_pchmcs_region_createdonbehalfby", "")]
			public const string lk_pchmcs_region_createdonbehalfby = "lk_pchmcs_region_createdonbehalfby";
			[Relationship("pchmcs_region", EntityRole.Referenced, "lk_pchmcs_region_modifiedby", "")]
			public const string lk_pchmcs_region_modifiedby = "lk_pchmcs_region_modifiedby";
			[Relationship("pchmcs_region", EntityRole.Referenced, "lk_pchmcs_region_modifiedonbehalfby", "")]
			public const string lk_pchmcs_region_modifiedonbehalfby = "lk_pchmcs_region_modifiedonbehalfby";
			[Relationship("pchmcs_relationbancaire", EntityRole.Referenced, "lk_pchmcs_relationbancaire_createdby", "")]
			public const string lk_pchmcs_relationbancaire_createdby = "lk_pchmcs_relationbancaire_createdby";
			[Relationship("pchmcs_relationbancaire", EntityRole.Referenced, "lk_pchmcs_relationbancaire_createdonbehalfby", "")]
			public const string lk_pchmcs_relationbancaire_createdonbehalfby = "lk_pchmcs_relationbancaire_createdonbehalfby";
			[Relationship("pchmcs_relationbancaire", EntityRole.Referenced, "lk_pchmcs_relationbancaire_modifiedby", "")]
			public const string lk_pchmcs_relationbancaire_modifiedby = "lk_pchmcs_relationbancaire_modifiedby";
			[Relationship("pchmcs_relationbancaire", EntityRole.Referenced, "lk_pchmcs_relationbancaire_modifiedonbehalfby", "")]
			public const string lk_pchmcs_relationbancaire_modifiedonbehalfby = "lk_pchmcs_relationbancaire_modifiedonbehalfby";
			[Relationship("pchmcs_remise", EntityRole.Referenced, "lk_pchmcs_remise_createdby", "")]
			public const string lk_pchmcs_remise_createdby = "lk_pchmcs_remise_createdby";
			[Relationship("pchmcs_remise", EntityRole.Referenced, "lk_pchmcs_remise_createdonbehalfby", "")]
			public const string lk_pchmcs_remise_createdonbehalfby = "lk_pchmcs_remise_createdonbehalfby";
			[Relationship("pchmcs_remise", EntityRole.Referenced, "lk_pchmcs_remise_modifiedby", "")]
			public const string lk_pchmcs_remise_modifiedby = "lk_pchmcs_remise_modifiedby";
			[Relationship("pchmcs_remise", EntityRole.Referenced, "lk_pchmcs_remise_modifiedonbehalfby", "")]
			public const string lk_pchmcs_remise_modifiedonbehalfby = "lk_pchmcs_remise_modifiedonbehalfby";
			[Relationship("pchmcs_remuneration", EntityRole.Referenced, "lk_pchmcs_remuneration_createdby", "")]
			public const string lk_pchmcs_remuneration_createdby = "lk_pchmcs_remuneration_createdby";
			[Relationship("pchmcs_remuneration", EntityRole.Referenced, "lk_pchmcs_remuneration_createdonbehalfby", "")]
			public const string lk_pchmcs_remuneration_createdonbehalfby = "lk_pchmcs_remuneration_createdonbehalfby";
			[Relationship("pchmcs_remuneration", EntityRole.Referenced, "lk_pchmcs_remuneration_modifiedby", "")]
			public const string lk_pchmcs_remuneration_modifiedby = "lk_pchmcs_remuneration_modifiedby";
			[Relationship("pchmcs_remuneration", EntityRole.Referenced, "lk_pchmcs_remuneration_modifiedonbehalfby", "")]
			public const string lk_pchmcs_remuneration_modifiedonbehalfby = "lk_pchmcs_remuneration_modifiedonbehalfby";
			[Relationship("pchmcs_remunerationprogramme", EntityRole.Referenced, "lk_pchmcs_remunerationprogramme_createdby", "")]
			public const string lk_pchmcs_remunerationprogramme_createdby = "lk_pchmcs_remunerationprogramme_createdby";
			[Relationship("pchmcs_remunerationprogramme", EntityRole.Referenced, "lk_pchmcs_remunerationprogramme_createdonbehalfby", "")]
			public const string lk_pchmcs_remunerationprogramme_createdonbehalfby = "lk_pchmcs_remunerationprogramme_createdonbehalfby";
			[Relationship("pchmcs_remunerationprogramme", EntityRole.Referenced, "lk_pchmcs_remunerationprogramme_modifiedby", "")]
			public const string lk_pchmcs_remunerationprogramme_modifiedby = "lk_pchmcs_remunerationprogramme_modifiedby";
			[Relationship("pchmcs_remunerationprogramme", EntityRole.Referenced, "lk_pchmcs_remunerationprogramme_modifiedonbehalfby", "")]
			public const string lk_pchmcs_remunerationprogramme_modifiedonbehalfby = "lk_pchmcs_remunerationprogramme_modifiedonbehalfby";
			[Relationship("pchmcs_reseau", EntityRole.Referenced, "lk_pchmcs_reseau_createdby", "")]
			public const string lk_pchmcs_reseau_createdby = "lk_pchmcs_reseau_createdby";
			[Relationship("pchmcs_reseau", EntityRole.Referenced, "lk_pchmcs_reseau_createdonbehalfby", "")]
			public const string lk_pchmcs_reseau_createdonbehalfby = "lk_pchmcs_reseau_createdonbehalfby";
			[Relationship("pchmcs_reseau", EntityRole.Referenced, "lk_pchmcs_reseau_modifiedby", "")]
			public const string lk_pchmcs_reseau_modifiedby = "lk_pchmcs_reseau_modifiedby";
			[Relationship("pchmcs_reseau", EntityRole.Referenced, "lk_pchmcs_reseau_modifiedonbehalfby", "")]
			public const string lk_pchmcs_reseau_modifiedonbehalfby = "lk_pchmcs_reseau_modifiedonbehalfby";
			[Relationship("pchmcs_sellerstatushistory", EntityRole.Referenced, "lk_pchmcs_sellerstatushistory_createdby", "")]
			public const string lk_pchmcs_sellerstatushistory_createdby = "lk_pchmcs_sellerstatushistory_createdby";
			[Relationship("pchmcs_sellerstatushistory", EntityRole.Referenced, "lk_pchmcs_sellerstatushistory_createdonbehalfby", "")]
			public const string lk_pchmcs_sellerstatushistory_createdonbehalfby = "lk_pchmcs_sellerstatushistory_createdonbehalfby";
			[Relationship("pchmcs_sellerstatushistory", EntityRole.Referenced, "lk_pchmcs_sellerstatushistory_modifiedby", "")]
			public const string lk_pchmcs_sellerstatushistory_modifiedby = "lk_pchmcs_sellerstatushistory_modifiedby";
			[Relationship("pchmcs_sellerstatushistory", EntityRole.Referenced, "lk_pchmcs_sellerstatushistory_modifiedonbehalfby", "")]
			public const string lk_pchmcs_sellerstatushistory_modifiedonbehalfby = "lk_pchmcs_sellerstatushistory_modifiedonbehalfby";
			[Relationship("pchmcs_simulation", EntityRole.Referenced, "lk_pchmcs_simulation_createdby", "")]
			public const string lk_pchmcs_simulation_createdby = "lk_pchmcs_simulation_createdby";
			[Relationship("pchmcs_simulation", EntityRole.Referenced, "lk_pchmcs_simulation_createdonbehalfby", "")]
			public const string lk_pchmcs_simulation_createdonbehalfby = "lk_pchmcs_simulation_createdonbehalfby";
			[Relationship("pchmcs_simulation", EntityRole.Referenced, "lk_pchmcs_simulation_modifiedby", "")]
			public const string lk_pchmcs_simulation_modifiedby = "lk_pchmcs_simulation_modifiedby";
			[Relationship("pchmcs_simulation", EntityRole.Referenced, "lk_pchmcs_simulation_modifiedonbehalfby", "")]
			public const string lk_pchmcs_simulation_modifiedonbehalfby = "lk_pchmcs_simulation_modifiedonbehalfby";
			[Relationship("pchmcs_situationimmobilire", EntityRole.Referenced, "lk_pchmcs_situationimmobilire_createdby", "")]
			public const string lk_pchmcs_situationimmobilire_createdby = "lk_pchmcs_situationimmobilire_createdby";
			[Relationship("pchmcs_situationimmobilire", EntityRole.Referenced, "lk_pchmcs_situationimmobilire_createdonbehalfby", "")]
			public const string lk_pchmcs_situationimmobilire_createdonbehalfby = "lk_pchmcs_situationimmobilire_createdonbehalfby";
			[Relationship("pchmcs_situationimmobilire", EntityRole.Referenced, "lk_pchmcs_situationimmobilire_modifiedby", "")]
			public const string lk_pchmcs_situationimmobilire_modifiedby = "lk_pchmcs_situationimmobilire_modifiedby";
			[Relationship("pchmcs_situationimmobilire", EntityRole.Referenced, "lk_pchmcs_situationimmobilire_modifiedonbehalfby", "")]
			public const string lk_pchmcs_situationimmobilire_modifiedonbehalfby = "lk_pchmcs_situationimmobilire_modifiedonbehalfby";
			[Relationship("pchmcs_sousstatut", EntityRole.Referenced, "lk_pchmcs_sousstatut_createdby", "")]
			public const string lk_pchmcs_sousstatut_createdby = "lk_pchmcs_sousstatut_createdby";
			[Relationship("pchmcs_sousstatut", EntityRole.Referenced, "lk_pchmcs_sousstatut_createdonbehalfby", "")]
			public const string lk_pchmcs_sousstatut_createdonbehalfby = "lk_pchmcs_sousstatut_createdonbehalfby";
			[Relationship("pchmcs_sousstatut", EntityRole.Referenced, "lk_pchmcs_sousstatut_modifiedby", "")]
			public const string lk_pchmcs_sousstatut_modifiedby = "lk_pchmcs_sousstatut_modifiedby";
			[Relationship("pchmcs_sousstatut", EntityRole.Referenced, "lk_pchmcs_sousstatut_modifiedonbehalfby", "")]
			public const string lk_pchmcs_sousstatut_modifiedonbehalfby = "lk_pchmcs_sousstatut_modifiedonbehalfby";
			[Relationship("pchmcs_statut", EntityRole.Referenced, "lk_pchmcs_statut_createdby", "")]
			public const string lk_pchmcs_statut_createdby = "lk_pchmcs_statut_createdby";
			[Relationship("pchmcs_statut", EntityRole.Referenced, "lk_pchmcs_statut_createdonbehalfby", "")]
			public const string lk_pchmcs_statut_createdonbehalfby = "lk_pchmcs_statut_createdonbehalfby";
			[Relationship("pchmcs_statut", EntityRole.Referenced, "lk_pchmcs_statut_modifiedby", "")]
			public const string lk_pchmcs_statut_modifiedby = "lk_pchmcs_statut_modifiedby";
			[Relationship("pchmcs_statut", EntityRole.Referenced, "lk_pchmcs_statut_modifiedonbehalfby", "")]
			public const string lk_pchmcs_statut_modifiedonbehalfby = "lk_pchmcs_statut_modifiedonbehalfby";
			[Relationship("pchmcs_subjectmatrix", EntityRole.Referenced, "lk_pchmcs_subjectmatrix_createdby", "")]
			public const string lk_pchmcs_subjectmatrix_createdby = "lk_pchmcs_subjectmatrix_createdby";
			[Relationship("pchmcs_subjectmatrix", EntityRole.Referenced, "lk_pchmcs_subjectmatrix_createdonbehalfby", "")]
			public const string lk_pchmcs_subjectmatrix_createdonbehalfby = "lk_pchmcs_subjectmatrix_createdonbehalfby";
			[Relationship("pchmcs_subjectmatrix", EntityRole.Referenced, "lk_pchmcs_subjectmatrix_modifiedby", "")]
			public const string lk_pchmcs_subjectmatrix_modifiedby = "lk_pchmcs_subjectmatrix_modifiedby";
			[Relationship("pchmcs_subjectmatrix", EntityRole.Referenced, "lk_pchmcs_subjectmatrix_modifiedonbehalfby", "")]
			public const string lk_pchmcs_subjectmatrix_modifiedonbehalfby = "lk_pchmcs_subjectmatrix_modifiedonbehalfby";
			[Relationship("pchmcs_syncoperation", EntityRole.Referenced, "lk_pchmcs_syncoperation_createdby", "")]
			public const string lk_pchmcs_syncoperation_createdby = "lk_pchmcs_syncoperation_createdby";
			[Relationship("pchmcs_syncoperation", EntityRole.Referenced, "lk_pchmcs_syncoperation_createdonbehalfby", "")]
			public const string lk_pchmcs_syncoperation_createdonbehalfby = "lk_pchmcs_syncoperation_createdonbehalfby";
			[Relationship("pchmcs_syncoperation", EntityRole.Referenced, "lk_pchmcs_syncoperation_modifiedby", "")]
			public const string lk_pchmcs_syncoperation_modifiedby = "lk_pchmcs_syncoperation_modifiedby";
			[Relationship("pchmcs_syncoperation", EntityRole.Referenced, "lk_pchmcs_syncoperation_modifiedonbehalfby", "")]
			public const string lk_pchmcs_syncoperation_modifiedonbehalfby = "lk_pchmcs_syncoperation_modifiedonbehalfby";
			[Relationship("pchmcs_syncoperationdetail", EntityRole.Referenced, "lk_pchmcs_syncoperationdetail_createdby", "")]
			public const string lk_pchmcs_syncoperationdetail_createdby = "lk_pchmcs_syncoperationdetail_createdby";
			[Relationship("pchmcs_syncoperationdetail", EntityRole.Referenced, "lk_pchmcs_syncoperationdetail_createdonbehalfby", "")]
			public const string lk_pchmcs_syncoperationdetail_createdonbehalfby = "lk_pchmcs_syncoperationdetail_createdonbehalfby";
			[Relationship("pchmcs_syncoperationdetail", EntityRole.Referenced, "lk_pchmcs_syncoperationdetail_modifiedby", "")]
			public const string lk_pchmcs_syncoperationdetail_modifiedby = "lk_pchmcs_syncoperationdetail_modifiedby";
			[Relationship("pchmcs_syncoperationdetail", EntityRole.Referenced, "lk_pchmcs_syncoperationdetail_modifiedonbehalfby", "")]
			public const string lk_pchmcs_syncoperationdetail_modifiedonbehalfby = "lk_pchmcs_syncoperationdetail_modifiedonbehalfby";
			[Relationship("pchmcs_systemfunction", EntityRole.Referenced, "lk_pchmcs_systemfunction_createdby", "")]
			public const string lk_pchmcs_systemfunction_createdby = "lk_pchmcs_systemfunction_createdby";
			[Relationship("pchmcs_systemfunction", EntityRole.Referenced, "lk_pchmcs_systemfunction_createdonbehalfby", "")]
			public const string lk_pchmcs_systemfunction_createdonbehalfby = "lk_pchmcs_systemfunction_createdonbehalfby";
			[Relationship("pchmcs_systemfunction", EntityRole.Referenced, "lk_pchmcs_systemfunction_modifiedby", "")]
			public const string lk_pchmcs_systemfunction_modifiedby = "lk_pchmcs_systemfunction_modifiedby";
			[Relationship("pchmcs_systemfunction", EntityRole.Referenced, "lk_pchmcs_systemfunction_modifiedonbehalfby", "")]
			public const string lk_pchmcs_systemfunction_modifiedonbehalfby = "lk_pchmcs_systemfunction_modifiedonbehalfby";
			[Relationship("pchmcs_taxlaw", EntityRole.Referenced, "lk_pchmcs_taxlaw_createdby", "")]
			public const string lk_pchmcs_taxlaw_createdby = "lk_pchmcs_taxlaw_createdby";
			[Relationship("pchmcs_taxlaw", EntityRole.Referenced, "lk_pchmcs_taxlaw_createdonbehalfby", "")]
			public const string lk_pchmcs_taxlaw_createdonbehalfby = "lk_pchmcs_taxlaw_createdonbehalfby";
			[Relationship("pchmcs_taxlaw", EntityRole.Referenced, "lk_pchmcs_taxlaw_modifiedby", "")]
			public const string lk_pchmcs_taxlaw_modifiedby = "lk_pchmcs_taxlaw_modifiedby";
			[Relationship("pchmcs_taxlaw", EntityRole.Referenced, "lk_pchmcs_taxlaw_modifiedonbehalfby", "")]
			public const string lk_pchmcs_taxlaw_modifiedonbehalfby = "lk_pchmcs_taxlaw_modifiedonbehalfby";
			[Relationship("pchmcs_trancheimposition", EntityRole.Referenced, "lk_pchmcs_trancheimposition_createdby", "")]
			public const string lk_pchmcs_trancheimposition_createdby = "lk_pchmcs_trancheimposition_createdby";
			[Relationship("pchmcs_trancheimposition", EntityRole.Referenced, "lk_pchmcs_trancheimposition_createdonbehalfby", "")]
			public const string lk_pchmcs_trancheimposition_createdonbehalfby = "lk_pchmcs_trancheimposition_createdonbehalfby";
			[Relationship("pchmcs_trancheimposition", EntityRole.Referenced, "lk_pchmcs_trancheimposition_modifiedby", "")]
			public const string lk_pchmcs_trancheimposition_modifiedby = "lk_pchmcs_trancheimposition_modifiedby";
			[Relationship("pchmcs_trancheimposition", EntityRole.Referenced, "lk_pchmcs_trancheimposition_modifiedonbehalfby", "")]
			public const string lk_pchmcs_trancheimposition_modifiedonbehalfby = "lk_pchmcs_trancheimposition_modifiedonbehalfby";
			[Relationship("pchmcs_typedelot", EntityRole.Referenced, "lk_pchmcs_typedelot_createdby", "")]
			public const string lk_pchmcs_typedelot_createdby = "lk_pchmcs_typedelot_createdby";
			[Relationship("pchmcs_typedelot", EntityRole.Referenced, "lk_pchmcs_typedelot_createdonbehalfby", "")]
			public const string lk_pchmcs_typedelot_createdonbehalfby = "lk_pchmcs_typedelot_createdonbehalfby";
			[Relationship("pchmcs_typedelot", EntityRole.Referenced, "lk_pchmcs_typedelot_modifiedby", "")]
			public const string lk_pchmcs_typedelot_modifiedby = "lk_pchmcs_typedelot_modifiedby";
			[Relationship("pchmcs_typedelot", EntityRole.Referenced, "lk_pchmcs_typedelot_modifiedonbehalfby", "")]
			public const string lk_pchmcs_typedelot_modifiedonbehalfby = "lk_pchmcs_typedelot_modifiedonbehalfby";
			[Relationship("pchmcs_uniquecontact", EntityRole.Referenced, "lk_pchmcs_uniquecontact_createdby", "")]
			public const string lk_pchmcs_uniquecontact_createdby = "lk_pchmcs_uniquecontact_createdby";
			[Relationship("pchmcs_uniquecontact", EntityRole.Referenced, "lk_pchmcs_uniquecontact_createdonbehalfby", "")]
			public const string lk_pchmcs_uniquecontact_createdonbehalfby = "lk_pchmcs_uniquecontact_createdonbehalfby";
			[Relationship("pchmcs_uniquecontact", EntityRole.Referenced, "lk_pchmcs_uniquecontact_modifiedby", "")]
			public const string lk_pchmcs_uniquecontact_modifiedby = "lk_pchmcs_uniquecontact_modifiedby";
			[Relationship("pchmcs_uniquecontact", EntityRole.Referenced, "lk_pchmcs_uniquecontact_modifiedonbehalfby", "")]
			public const string lk_pchmcs_uniquecontact_modifiedonbehalfby = "lk_pchmcs_uniquecontact_modifiedonbehalfby";
			[Relationship("pchmcs_vendeur", EntityRole.Referenced, "lk_pchmcs_vendeur_createdby", "")]
			public const string lk_pchmcs_vendeur_createdby = "lk_pchmcs_vendeur_createdby";
			[Relationship("pchmcs_vendeur", EntityRole.Referenced, "lk_pchmcs_vendeur_createdonbehalfby", "")]
			public const string lk_pchmcs_vendeur_createdonbehalfby = "lk_pchmcs_vendeur_createdonbehalfby";
			[Relationship("pchmcs_vendeur", EntityRole.Referenced, "lk_pchmcs_vendeur_modifiedby", "")]
			public const string lk_pchmcs_vendeur_modifiedby = "lk_pchmcs_vendeur_modifiedby";
			[Relationship("pchmcs_vendeur", EntityRole.Referenced, "lk_pchmcs_vendeur_modifiedonbehalfby", "")]
			public const string lk_pchmcs_vendeur_modifiedonbehalfby = "lk_pchmcs_vendeur_modifiedonbehalfby";
			[Relationship("pchmcs_workflowsrcg", EntityRole.Referenced, "lk_pchmcs_workflowsrcg_createdby", "")]
			public const string lk_pchmcs_workflowsrcg_createdby = "lk_pchmcs_workflowsrcg_createdby";
			[Relationship("pchmcs_workflowsrcg", EntityRole.Referenced, "lk_pchmcs_workflowsrcg_createdonbehalfby", "")]
			public const string lk_pchmcs_workflowsrcg_createdonbehalfby = "lk_pchmcs_workflowsrcg_createdonbehalfby";
			[Relationship("pchmcs_workflowsrcg", EntityRole.Referenced, "lk_pchmcs_workflowsrcg_modifiedby", "")]
			public const string lk_pchmcs_workflowsrcg_modifiedby = "lk_pchmcs_workflowsrcg_modifiedby";
			[Relationship("pchmcs_workflowsrcg", EntityRole.Referenced, "lk_pchmcs_workflowsrcg_modifiedonbehalfby", "")]
			public const string lk_pchmcs_workflowsrcg_modifiedonbehalfby = "lk_pchmcs_workflowsrcg_modifiedonbehalfby";
			[Relationship("pchmcs_zonefiscale", EntityRole.Referenced, "lk_pchmcs_zonefiscale_createdby", "")]
			public const string lk_pchmcs_zonefiscale_createdby = "lk_pchmcs_zonefiscale_createdby";
			[Relationship("pchmcs_zonefiscale", EntityRole.Referenced, "lk_pchmcs_zonefiscale_createdonbehalfby", "")]
			public const string lk_pchmcs_zonefiscale_createdonbehalfby = "lk_pchmcs_zonefiscale_createdonbehalfby";
			[Relationship("pchmcs_zonefiscale", EntityRole.Referenced, "lk_pchmcs_zonefiscale_modifiedby", "")]
			public const string lk_pchmcs_zonefiscale_modifiedby = "lk_pchmcs_zonefiscale_modifiedby";
			[Relationship("pchmcs_zonefiscale", EntityRole.Referenced, "lk_pchmcs_zonefiscale_modifiedonbehalfby", "")]
			public const string lk_pchmcs_zonefiscale_modifiedonbehalfby = "lk_pchmcs_zonefiscale_modifiedonbehalfby";
			[Relationship("pchmcs_zonegeographique", EntityRole.Referenced, "lk_pchmcs_zonegeographique_createdby", "")]
			public const string lk_pchmcs_zonegeographique_createdby = "lk_pchmcs_zonegeographique_createdby";
			[Relationship("pchmcs_zonegeographique", EntityRole.Referenced, "lk_pchmcs_zonegeographique_createdonbehalfby", "")]
			public const string lk_pchmcs_zonegeographique_createdonbehalfby = "lk_pchmcs_zonegeographique_createdonbehalfby";
			[Relationship("pchmcs_zonegeographique", EntityRole.Referenced, "lk_pchmcs_zonegeographique_modifiedby", "")]
			public const string lk_pchmcs_zonegeographique_modifiedby = "lk_pchmcs_zonegeographique_modifiedby";
			[Relationship("pchmcs_zonegeographique", EntityRole.Referenced, "lk_pchmcs_zonegeographique_modifiedonbehalfby", "")]
			public const string lk_pchmcs_zonegeographique_modifiedonbehalfby = "lk_pchmcs_zonegeographique_modifiedonbehalfby";
			[Relationship("personaldocumenttemplate", EntityRole.Referenced, "lk_personaldocumenttemplatebase_createdby", "createdby")]
			public const string lk_personaldocumenttemplatebase_createdby = "lk_personaldocumenttemplatebase_createdby";
			[Relationship("personaldocumenttemplate", EntityRole.Referenced, "lk_personaldocumenttemplatebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_personaldocumenttemplatebase_createdonbehalfby = "lk_personaldocumenttemplatebase_createdonbehalfby";
			[Relationship("personaldocumenttemplate", EntityRole.Referenced, "lk_personaldocumenttemplatebase_modifiedby", "modifiedby")]
			public const string lk_personaldocumenttemplatebase_modifiedby = "lk_personaldocumenttemplatebase_modifiedby";
			[Relationship("personaldocumenttemplate", EntityRole.Referenced, "lk_personaldocumenttemplatebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_personaldocumenttemplatebase_modifiedonbehalfby = "lk_personaldocumenttemplatebase_modifiedonbehalfby";
			[Relationship("phonecall", EntityRole.Referenced, "lk_phonecall_createdby", "createdby")]
			public const string lk_phonecall_createdby = "lk_phonecall_createdby";
			[Relationship("phonecall", EntityRole.Referenced, "lk_phonecall_createdonbehalfby", "createdonbehalfby")]
			public const string lk_phonecall_createdonbehalfby = "lk_phonecall_createdonbehalfby";
			[Relationship("phonecall", EntityRole.Referenced, "lk_phonecall_modifiedby", "modifiedby")]
			public const string lk_phonecall_modifiedby = "lk_phonecall_modifiedby";
			[Relationship("phonecall", EntityRole.Referenced, "lk_phonecall_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_phonecall_modifiedonbehalfby = "lk_phonecall_modifiedonbehalfby";
			[Relationship("phonetocaseprocess", EntityRole.Referenced, "lk_phonetocaseprocess_createdby", "createdby")]
			public const string lk_phonetocaseprocess_createdby = "lk_phonetocaseprocess_createdby";
			[Relationship("phonetocaseprocess", EntityRole.Referenced, "lk_phonetocaseprocess_createdonbehalfby", "createdonbehalfby")]
			public const string lk_phonetocaseprocess_createdonbehalfby = "lk_phonetocaseprocess_createdonbehalfby";
			[Relationship("phonetocaseprocess", EntityRole.Referenced, "lk_phonetocaseprocess_modifiedby", "modifiedby")]
			public const string lk_phonetocaseprocess_modifiedby = "lk_phonetocaseprocess_modifiedby";
			[Relationship("phonetocaseprocess", EntityRole.Referenced, "lk_phonetocaseprocess_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_phonetocaseprocess_modifiedonbehalfby = "lk_phonetocaseprocess_modifiedonbehalfby";
			[Relationship("picklistmapping", EntityRole.Referenced, "lk_picklistmapping_createdby", "createdby")]
			public const string lk_picklistmapping_createdby = "lk_picklistmapping_createdby";
			[Relationship("picklistmapping", EntityRole.Referenced, "lk_picklistmapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_picklistmapping_createdonbehalfby = "lk_picklistmapping_createdonbehalfby";
			[Relationship("picklistmapping", EntityRole.Referenced, "lk_picklistmapping_modifiedby", "modifiedby")]
			public const string lk_picklistmapping_modifiedby = "lk_picklistmapping_modifiedby";
			[Relationship("picklistmapping", EntityRole.Referenced, "lk_picklistmapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_picklistmapping_modifiedonbehalfby = "lk_picklistmapping_modifiedonbehalfby";
			[Relationship("pluginassembly", EntityRole.Referenced, "lk_pluginassembly_createdonbehalfby", "createdonbehalfby")]
			public const string lk_pluginassembly_createdonbehalfby = "lk_pluginassembly_createdonbehalfby";
			[Relationship("pluginassembly", EntityRole.Referenced, "lk_pluginassembly_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_pluginassembly_modifiedonbehalfby = "lk_pluginassembly_modifiedonbehalfby";
			[Relationship("plugintracelog", EntityRole.Referenced, "lk_plugintracelogbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_plugintracelogbase_createdonbehalfby = "lk_plugintracelogbase_createdonbehalfby";
			[Relationship("plugintype", EntityRole.Referenced, "lk_plugintype_createdonbehalfby", "createdonbehalfby")]
			public const string lk_plugintype_createdonbehalfby = "lk_plugintype_createdonbehalfby";
			[Relationship("plugintype", EntityRole.Referenced, "lk_plugintype_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_plugintype_modifiedonbehalfby = "lk_plugintype_modifiedonbehalfby";
			[Relationship("plugintypestatistic", EntityRole.Referenced, "lk_plugintypestatisticbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_plugintypestatisticbase_createdonbehalfby = "lk_plugintypestatisticbase_createdonbehalfby";
			[Relationship("plugintypestatistic", EntityRole.Referenced, "lk_plugintypestatisticbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_plugintypestatisticbase_modifiedonbehalfby = "lk_plugintypestatisticbase_modifiedonbehalfby";
			[Relationship("position", EntityRole.Referenced, "lk_position_createdby", "createdby")]
			public const string lk_position_createdby = "lk_position_createdby";
			[Relationship("position", EntityRole.Referenced, "lk_position_createdonbehalfby", "createdonbehalfby")]
			public const string lk_position_createdonbehalfby = "lk_position_createdonbehalfby";
			[Relationship("position", EntityRole.Referenced, "lk_position_modifiedby", "modifiedby")]
			public const string lk_position_modifiedby = "lk_position_modifiedby";
			[Relationship("position", EntityRole.Referenced, "lk_position_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_position_modifiedonbehalfby = "lk_position_modifiedonbehalfby";
			[Relationship("post", EntityRole.Referenced, "lk_post_createdby", "createdby")]
			public const string lk_post_createdby = "lk_post_createdby";
			[Relationship("post", EntityRole.Referenced, "lk_post_createdonbehalfby", "createdonbehalfby")]
			public const string lk_post_createdonbehalfby = "lk_post_createdonbehalfby";
			[Relationship("post", EntityRole.Referenced, "lk_post_modifiedby", "modifiedby")]
			public const string lk_post_modifiedby = "lk_post_modifiedby";
			[Relationship("post", EntityRole.Referenced, "lk_post_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_post_modifiedonbehalfby = "lk_post_modifiedonbehalfby";
			[Relationship("postcomment", EntityRole.Referenced, "lk_postcomment_createdby", "createdby")]
			public const string lk_postcomment_createdby = "lk_postcomment_createdby";
			[Relationship("postcomment", EntityRole.Referenced, "lk_postcomment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_postcomment_createdonbehalfby = "lk_postcomment_createdonbehalfby";
			[Relationship("postfollow", EntityRole.Referenced, "lk_PostFollow_createdby", "createdby")]
			public const string lk_PostFollow_createdby = "lk_PostFollow_createdby";
			[Relationship("postfollow", EntityRole.Referenced, "lk_postfollow_createdonbehalfby", "createdonbehalfby")]
			public const string lk_postfollow_createdonbehalfby = "lk_postfollow_createdonbehalfby";
			[Relationship("postlike", EntityRole.Referenced, "lk_postlike_createdby", "createdby")]
			public const string lk_postlike_createdby = "lk_postlike_createdby";
			[Relationship("postlike", EntityRole.Referenced, "lk_postlike_createdonbehalfby", "createdonbehalfby")]
			public const string lk_postlike_createdonbehalfby = "lk_postlike_createdonbehalfby";
			[Relationship("pricelevel", EntityRole.Referenced, "lk_pricelevel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_pricelevel_createdonbehalfby = "lk_pricelevel_createdonbehalfby";
			[Relationship("pricelevel", EntityRole.Referenced, "lk_pricelevel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_pricelevel_modifiedonbehalfby = "lk_pricelevel_modifiedonbehalfby";
			[Relationship("pricelevel", EntityRole.Referenced, "lk_pricelevelbase_createdby", "createdby")]
			public const string lk_pricelevelbase_createdby = "lk_pricelevelbase_createdby";
			[Relationship("pricelevel", EntityRole.Referenced, "lk_pricelevelbase_modifiedby", "modifiedby")]
			public const string lk_pricelevelbase_modifiedby = "lk_pricelevelbase_modifiedby";
			[Relationship("processsession", EntityRole.Referenced, "lk_processsession_canceledby", "canceledby")]
			public const string lk_processsession_canceledby = "lk_processsession_canceledby";
			[Relationship("processsession", EntityRole.Referenced, "lk_processsession_completedby", "completedby")]
			public const string lk_processsession_completedby = "lk_processsession_completedby";
			[Relationship("processsession", EntityRole.Referenced, "lk_processsession_createdby", "createdby")]
			public const string lk_processsession_createdby = "lk_processsession_createdby";
			[Relationship("processsession", EntityRole.Referenced, "lk_processsession_executedby", "executedby")]
			public const string lk_processsession_executedby = "lk_processsession_executedby";
			[Relationship("processsession", EntityRole.Referenced, "lk_processsession_modifiedby", "modifiedby")]
			public const string lk_processsession_modifiedby = "lk_processsession_modifiedby";
			[Relationship("processsession", EntityRole.Referenced, "lk_processsession_startedby", "startedby")]
			public const string lk_processsession_startedby = "lk_processsession_startedby";
			[Relationship("processsession", EntityRole.Referenced, "lk_processsessionbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_processsessionbase_createdonbehalfby = "lk_processsessionbase_createdonbehalfby";
			[Relationship("processsession", EntityRole.Referenced, "lk_processsessionbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_processsessionbase_modifiedonbehalfby = "lk_processsessionbase_modifiedonbehalfby";
			[Relationship("processtrigger", EntityRole.Referenced, "lk_processtriggerbase_createdby", "createdby")]
			public const string lk_processtriggerbase_createdby = "lk_processtriggerbase_createdby";
			[Relationship("processtrigger", EntityRole.Referenced, "lk_processtriggerbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_processtriggerbase_createdonbehalfby = "lk_processtriggerbase_createdonbehalfby";
			[Relationship("processtrigger", EntityRole.Referenced, "lk_processtriggerbase_modifiedby", "modifiedby")]
			public const string lk_processtriggerbase_modifiedby = "lk_processtriggerbase_modifiedby";
			[Relationship("processtrigger", EntityRole.Referenced, "lk_processtriggerbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_processtriggerbase_modifiedonbehalfby = "lk_processtriggerbase_modifiedonbehalfby";
			[Relationship("product", EntityRole.Referenced, "lk_product_createdonbehalfby", "createdonbehalfby")]
			public const string lk_product_createdonbehalfby = "lk_product_createdonbehalfby";
			[Relationship("product", EntityRole.Referenced, "lk_product_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_product_modifiedonbehalfby = "lk_product_modifiedonbehalfby";
			[Relationship("productassociation", EntityRole.Referenced, "lk_ProductAssociate_createdby", "createdby")]
			public const string lk_ProductAssociate_createdby = "lk_ProductAssociate_createdby";
			[Relationship("productassociation", EntityRole.Referenced, "lk_ProductAssociation_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ProductAssociation_createdonbehalfby = "lk_ProductAssociation_createdonbehalfby";
			[Relationship("productassociation", EntityRole.Referenced, "lk_ProductAssociation_modifiedby", "modifiedby")]
			public const string lk_ProductAssociation_modifiedby = "lk_ProductAssociation_modifiedby";
			[Relationship("productassociation", EntityRole.Referenced, "lk_ProductAssociation_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ProductAssociation_modifiedonbehalfby = "lk_ProductAssociation_modifiedonbehalfby";
			[Relationship("product", EntityRole.Referenced, "lk_productbase_createdby", "createdby")]
			public const string lk_productbase_createdby = "lk_productbase_createdby";
			[Relationship("product", EntityRole.Referenced, "lk_productbase_modifiedby", "modifiedby")]
			public const string lk_productbase_modifiedby = "lk_productbase_modifiedby";
			[Relationship("productpricelevel", EntityRole.Referenced, "lk_productpricelevel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_productpricelevel_createdonbehalfby = "lk_productpricelevel_createdonbehalfby";
			[Relationship("productpricelevel", EntityRole.Referenced, "lk_productpricelevel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_productpricelevel_modifiedonbehalfby = "lk_productpricelevel_modifiedonbehalfby";
			[Relationship("productpricelevel", EntityRole.Referenced, "lk_productpricelevelbase_createdby", "createdby")]
			public const string lk_productpricelevelbase_createdby = "lk_productpricelevelbase_createdby";
			[Relationship("productpricelevel", EntityRole.Referenced, "lk_productpricelevelbase_modifiedby", "modifiedby")]
			public const string lk_productpricelevelbase_modifiedby = "lk_productpricelevelbase_modifiedby";
			[Relationship("productsubstitute", EntityRole.Referenced, "lk_ProductSubstitute_createdby", "createdby")]
			public const string lk_ProductSubstitute_createdby = "lk_ProductSubstitute_createdby";
			[Relationship("productsubstitute", EntityRole.Referenced, "lk_ProductSubstitute_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ProductSubstitute_createdonbehalfby = "lk_ProductSubstitute_createdonbehalfby";
			[Relationship("productsubstitute", EntityRole.Referenced, "lk_ProductSubstitute_modifiedby", "modifiedby")]
			public const string lk_ProductSubstitute_modifiedby = "lk_ProductSubstitute_modifiedby";
			[Relationship("productsubstitute", EntityRole.Referenced, "lk_ProductSubstitute_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ProductSubstitute_modifiedonbehalfby = "lk_ProductSubstitute_modifiedonbehalfby";
			[Relationship("channelaccessprofilerule", EntityRole.Referenced, "lk_profilerule_createdby", "createdby")]
			public const string lk_profilerule_createdby = "lk_profilerule_createdby";
			[Relationship("channelaccessprofilerule", EntityRole.Referenced, "lk_profilerule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_profilerule_createdonbehalfby = "lk_profilerule_createdonbehalfby";
			[Relationship("channelaccessprofilerule", EntityRole.Referenced, "lk_profilerule_modifiedby", "modifiedby")]
			public const string lk_profilerule_modifiedby = "lk_profilerule_modifiedby";
			[Relationship("channelaccessprofilerule", EntityRole.Referenced, "lk_profilerule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_profilerule_modifiedonbehalfby = "lk_profilerule_modifiedonbehalfby";
			[Relationship("channelaccessprofileruleitem", EntityRole.Referenced, "lk_profileruleitem_createdby", "createdby")]
			public const string lk_profileruleitem_createdby = "lk_profileruleitem_createdby";
			[Relationship("channelaccessprofileruleitem", EntityRole.Referenced, "lk_profileruleitem_createdonbehalfby", "createdonbehalfby")]
			public const string lk_profileruleitem_createdonbehalfby = "lk_profileruleitem_createdonbehalfby";
			[Relationship("channelaccessprofileruleitem", EntityRole.Referenced, "lk_profileruleitem_modifiedby", "modifiedby")]
			public const string lk_profileruleitem_modifiedby = "lk_profileruleitem_modifiedby";
			[Relationship("channelaccessprofileruleitem", EntityRole.Referenced, "lk_profileruleitem_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_profileruleitem_modifiedonbehalfby = "lk_profileruleitem_modifiedonbehalfby";
			[Relationship("publisher", EntityRole.Referenced, "lk_publisher_createdby", "createdby")]
			public const string lk_publisher_createdby = "lk_publisher_createdby";
			[Relationship("publisher", EntityRole.Referenced, "lk_publisher_modifiedby", "modifiedby")]
			public const string lk_publisher_modifiedby = "lk_publisher_modifiedby";
			[Relationship("publisheraddress", EntityRole.Referenced, "lk_publisheraddressbase_createdby", "createdby")]
			public const string lk_publisheraddressbase_createdby = "lk_publisheraddressbase_createdby";
			[Relationship("publisheraddress", EntityRole.Referenced, "lk_publisheraddressbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_publisheraddressbase_createdonbehalfby = "lk_publisheraddressbase_createdonbehalfby";
			[Relationship("publisheraddress", EntityRole.Referenced, "lk_publisheraddressbase_modifiedby", "modifiedby")]
			public const string lk_publisheraddressbase_modifiedby = "lk_publisheraddressbase_modifiedby";
			[Relationship("publisheraddress", EntityRole.Referenced, "lk_publisheraddressbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_publisheraddressbase_modifiedonbehalfby = "lk_publisheraddressbase_modifiedonbehalfby";
			[Relationship("publisher", EntityRole.Referenced, "lk_publisherbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_publisherbase_createdonbehalfby = "lk_publisherbase_createdonbehalfby";
			[Relationship("publisher", EntityRole.Referenced, "lk_publisherbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_publisherbase_modifiedonbehalfby = "lk_publisherbase_modifiedonbehalfby";
			[Relationship("quarterlyfiscalcalendar", EntityRole.Referenced, "lk_quarterlyfiscalcalendar_createdby", "createdby")]
			public const string lk_quarterlyfiscalcalendar_createdby = "lk_quarterlyfiscalcalendar_createdby";
			[Relationship("quarterlyfiscalcalendar", EntityRole.Referenced, "lk_quarterlyfiscalcalendar_createdonbehalfby", "createdonbehalfby")]
			public const string lk_quarterlyfiscalcalendar_createdonbehalfby = "lk_quarterlyfiscalcalendar_createdonbehalfby";
			[Relationship("quarterlyfiscalcalendar", EntityRole.Referenced, "lk_quarterlyfiscalcalendar_modifiedby", "modifiedby")]
			public const string lk_quarterlyfiscalcalendar_modifiedby = "lk_quarterlyfiscalcalendar_modifiedby";
			[Relationship("quarterlyfiscalcalendar", EntityRole.Referenced, "lk_quarterlyfiscalcalendar_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_quarterlyfiscalcalendar_modifiedonbehalfby = "lk_quarterlyfiscalcalendar_modifiedonbehalfby";
			[Relationship("quarterlyfiscalcalendar", EntityRole.Referenced, "lk_quarterlyfiscalcalendar_salespersonid", "salespersonid")]
			public const string lk_quarterlyfiscalcalendar_salespersonid = "lk_quarterlyfiscalcalendar_salespersonid";
			[Relationship("queue", EntityRole.Referenced, "lk_queue_createdonbehalfby", "createdonbehalfby")]
			public const string lk_queue_createdonbehalfby = "lk_queue_createdonbehalfby";
			[Relationship("queue", EntityRole.Referenced, "lk_queue_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_queue_modifiedonbehalfby = "lk_queue_modifiedonbehalfby";
			[Relationship("queue", EntityRole.Referenced, "lk_queuebase_createdby", "createdby")]
			public const string lk_queuebase_createdby = "lk_queuebase_createdby";
			[Relationship("queue", EntityRole.Referenced, "lk_queuebase_modifiedby", "modifiedby")]
			public const string lk_queuebase_modifiedby = "lk_queuebase_modifiedby";
			[Relationship("queueitem", EntityRole.Referenced, "lk_queueitem_createdonbehalfby", "createdonbehalfby")]
			public const string lk_queueitem_createdonbehalfby = "lk_queueitem_createdonbehalfby";
			[Relationship("queueitem", EntityRole.Referenced, "lk_queueitem_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_queueitem_modifiedonbehalfby = "lk_queueitem_modifiedonbehalfby";
			[Relationship("queueitem", EntityRole.Referenced, "lk_queueitembase_createdby", "createdby")]
			public const string lk_queueitembase_createdby = "lk_queueitembase_createdby";
			[Relationship("queueitem", EntityRole.Referenced, "lk_queueitembase_modifiedby", "modifiedby")]
			public const string lk_queueitembase_modifiedby = "lk_queueitembase_modifiedby";
			[Relationship("queueitem", EntityRole.Referenced, "lk_queueitembase_workerid", "workerid")]
			public const string lk_queueitembase_workerid = "lk_queueitembase_workerid";
			[Relationship("quote", EntityRole.Referenced, "lk_quote_createdonbehalfby", "createdonbehalfby")]
			public const string lk_quote_createdonbehalfby = "lk_quote_createdonbehalfby";
			[Relationship("quote", EntityRole.Referenced, "lk_quote_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_quote_modifiedonbehalfby = "lk_quote_modifiedonbehalfby";
			[Relationship("quote", EntityRole.Referenced, "lk_quotebase_createdby", "createdby")]
			public const string lk_quotebase_createdby = "lk_quotebase_createdby";
			[Relationship("quote", EntityRole.Referenced, "lk_quotebase_modifiedby", "modifiedby")]
			public const string lk_quotebase_modifiedby = "lk_quotebase_modifiedby";
			[Relationship("quoteclose", EntityRole.Referenced, "lk_quoteclose_createdby", "createdby")]
			public const string lk_quoteclose_createdby = "lk_quoteclose_createdby";
			[Relationship("quoteclose", EntityRole.Referenced, "lk_quoteclose_createdonbehalfby", "createdonbehalfby")]
			public const string lk_quoteclose_createdonbehalfby = "lk_quoteclose_createdonbehalfby";
			[Relationship("quoteclose", EntityRole.Referenced, "lk_quoteclose_modifiedby", "modifiedby")]
			public const string lk_quoteclose_modifiedby = "lk_quoteclose_modifiedby";
			[Relationship("quoteclose", EntityRole.Referenced, "lk_quoteclose_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_quoteclose_modifiedonbehalfby = "lk_quoteclose_modifiedonbehalfby";
			[Relationship("quotedetail", EntityRole.Referenced, "lk_quotedetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_quotedetail_createdonbehalfby = "lk_quotedetail_createdonbehalfby";
			[Relationship("quotedetail", EntityRole.Referenced, "lk_quotedetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_quotedetail_modifiedonbehalfby = "lk_quotedetail_modifiedonbehalfby";
			[Relationship("quotedetail", EntityRole.Referenced, "lk_quotedetailbase_createdby", "createdby")]
			public const string lk_quotedetailbase_createdby = "lk_quotedetailbase_createdby";
			[Relationship("quotedetail", EntityRole.Referenced, "lk_quotedetailbase_modifiedby", "modifiedby")]
			public const string lk_quotedetailbase_modifiedby = "lk_quotedetailbase_modifiedby";
			[Relationship("ratingmodel", EntityRole.Referenced, "lk_ratingmodel_createdby", "createdby")]
			public const string lk_ratingmodel_createdby = "lk_ratingmodel_createdby";
			[Relationship("ratingmodel", EntityRole.Referenced, "lk_ratingmodel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ratingmodel_createdonbehalfby = "lk_ratingmodel_createdonbehalfby";
			[Relationship("ratingmodel", EntityRole.Referenced, "lk_ratingmodel_modifiedby", "modifiedby")]
			public const string lk_ratingmodel_modifiedby = "lk_ratingmodel_modifiedby";
			[Relationship("ratingmodel", EntityRole.Referenced, "lk_ratingmodel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ratingmodel_modifiedonbehalfby = "lk_ratingmodel_modifiedonbehalfby";
			[Relationship("ratingvalue", EntityRole.Referenced, "lk_ratingvalue_createdby", "createdby")]
			public const string lk_ratingvalue_createdby = "lk_ratingvalue_createdby";
			[Relationship("ratingvalue", EntityRole.Referenced, "lk_ratingvalue_createdonbehalfby", "createdonbehalfby")]
			public const string lk_ratingvalue_createdonbehalfby = "lk_ratingvalue_createdonbehalfby";
			[Relationship("ratingvalue", EntityRole.Referenced, "lk_ratingvalue_modifiedby", "modifiedby")]
			public const string lk_ratingvalue_modifiedby = "lk_ratingvalue_modifiedby";
			[Relationship("ratingvalue", EntityRole.Referenced, "lk_ratingvalue_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_ratingvalue_modifiedonbehalfby = "lk_ratingvalue_modifiedonbehalfby";
			[Relationship("recommendationmodel", EntityRole.Referenced, "lk_recommendationmodel_createdby", "")]
			public const string lk_recommendationmodel_createdby = "lk_recommendationmodel_createdby";
			[Relationship("recommendationmodel", EntityRole.Referenced, "lk_recommendationmodel_createdonbehalfby", "")]
			public const string lk_recommendationmodel_createdonbehalfby = "lk_recommendationmodel_createdonbehalfby";
			[Relationship("recommendationmodel", EntityRole.Referenced, "lk_recommendationmodel_modifiedby", "")]
			public const string lk_recommendationmodel_modifiedby = "lk_recommendationmodel_modifiedby";
			[Relationship("recommendationmodel", EntityRole.Referenced, "lk_recommendationmodel_modifiedonbehalfby", "")]
			public const string lk_recommendationmodel_modifiedonbehalfby = "lk_recommendationmodel_modifiedonbehalfby";
			[Relationship("recommendationmodelversion", EntityRole.Referenced, "lk_recommendationmodelversion_createdby", "")]
			public const string lk_recommendationmodelversion_createdby = "lk_recommendationmodelversion_createdby";
			[Relationship("recommendationmodelversion", EntityRole.Referenced, "lk_recommendationmodelversion_createdonbehalfby", "")]
			public const string lk_recommendationmodelversion_createdonbehalfby = "lk_recommendationmodelversion_createdonbehalfby";
			[Relationship("recommendationmodelversion", EntityRole.Referenced, "lk_recommendationmodelversion_modifiedby", "")]
			public const string lk_recommendationmodelversion_modifiedby = "lk_recommendationmodelversion_modifiedby";
			[Relationship("recommendationmodelversion", EntityRole.Referenced, "lk_recommendationmodelversion_modifiedonbehalfby", "")]
			public const string lk_recommendationmodelversion_modifiedonbehalfby = "lk_recommendationmodelversion_modifiedonbehalfby";
			[Relationship("recommendeddocument", EntityRole.Referenced, "lk_recommendeddocument_createdby", "createdby")]
			public const string lk_recommendeddocument_createdby = "lk_recommendeddocument_createdby";
			[Relationship("recommendeddocument", EntityRole.Referenced, "lk_recommendeddocument_createdonbehalfby", "createdonbehalfby")]
			public const string lk_recommendeddocument_createdonbehalfby = "lk_recommendeddocument_createdonbehalfby";
			[Relationship("recommendeddocument", EntityRole.Referenced, "lk_recommendeddocument_modifiedby", "modifiedby")]
			public const string lk_recommendeddocument_modifiedby = "lk_recommendeddocument_modifiedby";
			[Relationship("recommendeddocument", EntityRole.Referenced, "lk_recommendeddocument_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_recommendeddocument_modifiedonbehalfby = "lk_recommendeddocument_modifiedonbehalfby";
			[Relationship("recurrencerule", EntityRole.Referenced, "lk_recurrencerule_createdby", "createdby")]
			public const string lk_recurrencerule_createdby = "lk_recurrencerule_createdby";
			[Relationship("recurrencerule", EntityRole.Referenced, "lk_recurrencerule_modifiedby", "modifiedby")]
			public const string lk_recurrencerule_modifiedby = "lk_recurrencerule_modifiedby";
			[Relationship("recurrencerule", EntityRole.Referenced, "lk_recurrencerulebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_recurrencerulebase_createdonbehalfby = "lk_recurrencerulebase_createdonbehalfby";
			[Relationship("recurrencerule", EntityRole.Referenced, "lk_recurrencerulebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_recurrencerulebase_modifiedonbehalfby = "lk_recurrencerulebase_modifiedonbehalfby";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "lk_recurringappointmentmaster_createdby", "createdby")]
			public const string lk_recurringappointmentmaster_createdby = "lk_recurringappointmentmaster_createdby";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "lk_recurringappointmentmaster_createdonbehalfby", "createdonbehalfby")]
			public const string lk_recurringappointmentmaster_createdonbehalfby = "lk_recurringappointmentmaster_createdonbehalfby";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "lk_recurringappointmentmaster_modifiedby", "modifiedby")]
			public const string lk_recurringappointmentmaster_modifiedby = "lk_recurringappointmentmaster_modifiedby";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "lk_recurringappointmentmaster_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_recurringappointmentmaster_modifiedonbehalfby = "lk_recurringappointmentmaster_modifiedonbehalfby";
			[Relationship("relationshiprole", EntityRole.Referenced, "lk_relationshiprole_createdonbehalfby", "createdonbehalfby")]
			public const string lk_relationshiprole_createdonbehalfby = "lk_relationshiprole_createdonbehalfby";
			[Relationship("relationshiprole", EntityRole.Referenced, "lk_relationshiprole_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_relationshiprole_modifiedonbehalfby = "lk_relationshiprole_modifiedonbehalfby";
			[Relationship("relationshiprolemap", EntityRole.Referenced, "lk_relationshiprolemap_createdonbehalfby", "createdonbehalfby")]
			public const string lk_relationshiprolemap_createdonbehalfby = "lk_relationshiprolemap_createdonbehalfby";
			[Relationship("relationshiprolemap", EntityRole.Referenced, "lk_relationshiprolemap_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_relationshiprolemap_modifiedonbehalfby = "lk_relationshiprolemap_modifiedonbehalfby";
			[Relationship("report", EntityRole.Referenced, "lk_report_createdonbehalfby", "createdonbehalfby")]
			public const string lk_report_createdonbehalfby = "lk_report_createdonbehalfby";
			[Relationship("report", EntityRole.Referenced, "lk_report_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_report_modifiedonbehalfby = "lk_report_modifiedonbehalfby";
			[Relationship("report", EntityRole.Referenced, "lk_reportbase_createdby", "createdby")]
			public const string lk_reportbase_createdby = "lk_reportbase_createdby";
			[Relationship("report", EntityRole.Referenced, "lk_reportbase_modifiedby", "modifiedby")]
			public const string lk_reportbase_modifiedby = "lk_reportbase_modifiedby";
			[Relationship("reportcategory", EntityRole.Referenced, "lk_reportcategory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_reportcategory_createdonbehalfby = "lk_reportcategory_createdonbehalfby";
			[Relationship("reportcategory", EntityRole.Referenced, "lk_reportcategory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_reportcategory_modifiedonbehalfby = "lk_reportcategory_modifiedonbehalfby";
			[Relationship("reportcategory", EntityRole.Referenced, "lk_reportcategorybase_createdby", "createdby")]
			public const string lk_reportcategorybase_createdby = "lk_reportcategorybase_createdby";
			[Relationship("reportcategory", EntityRole.Referenced, "lk_reportcategorybase_modifiedby", "modifiedby")]
			public const string lk_reportcategorybase_modifiedby = "lk_reportcategorybase_modifiedby";
			[Relationship("reportentity", EntityRole.Referenced, "lk_reportentity_createdonbehalfby", "createdonbehalfby")]
			public const string lk_reportentity_createdonbehalfby = "lk_reportentity_createdonbehalfby";
			[Relationship("reportentity", EntityRole.Referenced, "lk_reportentity_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_reportentity_modifiedonbehalfby = "lk_reportentity_modifiedonbehalfby";
			[Relationship("reportentity", EntityRole.Referenced, "lk_reportentitybase_createdby", "createdby")]
			public const string lk_reportentitybase_createdby = "lk_reportentitybase_createdby";
			[Relationship("reportentity", EntityRole.Referenced, "lk_reportentitybase_modifiedby", "modifiedby")]
			public const string lk_reportentitybase_modifiedby = "lk_reportentitybase_modifiedby";
			[Relationship("reportlink", EntityRole.Referenced, "lk_reportlink_createdonbehalfby", "createdonbehalfby")]
			public const string lk_reportlink_createdonbehalfby = "lk_reportlink_createdonbehalfby";
			[Relationship("reportlink", EntityRole.Referenced, "lk_reportlink_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_reportlink_modifiedonbehalfby = "lk_reportlink_modifiedonbehalfby";
			[Relationship("reportlink", EntityRole.Referenced, "lk_reportlinkbase_createdby", "createdby")]
			public const string lk_reportlinkbase_createdby = "lk_reportlinkbase_createdby";
			[Relationship("reportlink", EntityRole.Referenced, "lk_reportlinkbase_modifiedby", "modifiedby")]
			public const string lk_reportlinkbase_modifiedby = "lk_reportlinkbase_modifiedby";
			[Relationship("reportvisibility", EntityRole.Referenced, "lk_reportvisibility_createdonbehalfby", "createdonbehalfby")]
			public const string lk_reportvisibility_createdonbehalfby = "lk_reportvisibility_createdonbehalfby";
			[Relationship("reportvisibility", EntityRole.Referenced, "lk_reportvisibility_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_reportvisibility_modifiedonbehalfby = "lk_reportvisibility_modifiedonbehalfby";
			[Relationship("reportvisibility", EntityRole.Referenced, "lk_reportvisibilitybase_createdby", "createdby")]
			public const string lk_reportvisibilitybase_createdby = "lk_reportvisibilitybase_createdby";
			[Relationship("reportvisibility", EntityRole.Referenced, "lk_reportvisibilitybase_modifiedby", "modifiedby")]
			public const string lk_reportvisibilitybase_modifiedby = "lk_reportvisibilitybase_modifiedby";
			[Relationship("resourcespec", EntityRole.Referenced, "lk_resourcespec_createdby", "createdby")]
			public const string lk_resourcespec_createdby = "lk_resourcespec_createdby";
			[Relationship("resourcespec", EntityRole.Referenced, "lk_resourcespec_createdonbehalfby", "createdonbehalfby")]
			public const string lk_resourcespec_createdonbehalfby = "lk_resourcespec_createdonbehalfby";
			[Relationship("resourcespec", EntityRole.Referenced, "lk_resourcespec_modifiedby", "modifiedby")]
			public const string lk_resourcespec_modifiedby = "lk_resourcespec_modifiedby";
			[Relationship("resourcespec", EntityRole.Referenced, "lk_resourcespec_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_resourcespec_modifiedonbehalfby = "lk_resourcespec_modifiedonbehalfby";
			[Relationship("role", EntityRole.Referenced, "lk_role_createdonbehalfby", "createdonbehalfby")]
			public const string lk_role_createdonbehalfby = "lk_role_createdonbehalfby";
			[Relationship("role", EntityRole.Referenced, "lk_role_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_role_modifiedonbehalfby = "lk_role_modifiedonbehalfby";
			[Relationship("role", EntityRole.Referenced, "lk_rolebase_createdby", "createdby")]
			public const string lk_rolebase_createdby = "lk_rolebase_createdby";
			[Relationship("role", EntityRole.Referenced, "lk_rolebase_modifiedby", "modifiedby")]
			public const string lk_rolebase_modifiedby = "lk_rolebase_modifiedby";
			[Relationship("rollupfield", EntityRole.Referenced, "lk_rollupfield_createdby", "createdby")]
			public const string lk_rollupfield_createdby = "lk_rollupfield_createdby";
			[Relationship("rollupfield", EntityRole.Referenced, "lk_rollupfield_createdonbehalfby", "createdonbehalfby")]
			public const string lk_rollupfield_createdonbehalfby = "lk_rollupfield_createdonbehalfby";
			[Relationship("rollupfield", EntityRole.Referenced, "lk_rollupfield_modifiedby", "modifiedby")]
			public const string lk_rollupfield_modifiedby = "lk_rollupfield_modifiedby";
			[Relationship("rollupfield", EntityRole.Referenced, "lk_rollupfield_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_rollupfield_modifiedonbehalfby = "lk_rollupfield_modifiedonbehalfby";
			[Relationship("routingrule", EntityRole.Referenced, "lk_routingrule_createdby", "createdby")]
			public const string lk_routingrule_createdby = "lk_routingrule_createdby";
			[Relationship("routingrule", EntityRole.Referenced, "lk_routingrule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_routingrule_createdonbehalfby = "lk_routingrule_createdonbehalfby";
			[Relationship("routingrule", EntityRole.Referenced, "lk_routingrule_modifiedby", "modifiedby")]
			public const string lk_routingrule_modifiedby = "lk_routingrule_modifiedby";
			[Relationship("routingrule", EntityRole.Referenced, "lk_routingrule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_routingrule_modifiedonbehalfby = "lk_routingrule_modifiedonbehalfby";
			[Relationship("routingruleitem", EntityRole.Referenced, "lk_RoutingRuleItem_createdby", "createdby")]
			public const string lk_RoutingRuleItem_createdby = "lk_RoutingRuleItem_createdby";
			[Relationship("routingruleitem", EntityRole.Referenced, "lk_routingruleitem_createdonbehalfby", "createdonbehalfby")]
			public const string lk_routingruleitem_createdonbehalfby = "lk_routingruleitem_createdonbehalfby";
			[Relationship("routingruleitem", EntityRole.Referenced, "lk_routingruleitem_modifiedby", "modifiedby")]
			public const string lk_routingruleitem_modifiedby = "lk_routingruleitem_modifiedby";
			[Relationship("routingruleitem", EntityRole.Referenced, "lk_routingruleitem_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_routingruleitem_modifiedonbehalfby = "lk_routingruleitem_modifiedonbehalfby";
			[Relationship("salesliterature", EntityRole.Referenced, "lk_salesliterature_createdonbehalfby", "createdonbehalfby")]
			public const string lk_salesliterature_createdonbehalfby = "lk_salesliterature_createdonbehalfby";
			[Relationship("salesliterature", EntityRole.Referenced, "lk_salesliterature_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_salesliterature_modifiedonbehalfby = "lk_salesliterature_modifiedonbehalfby";
			[Relationship("salesliterature", EntityRole.Referenced, "lk_salesliteraturebase_createdby", "createdby")]
			public const string lk_salesliteraturebase_createdby = "lk_salesliteraturebase_createdby";
			[Relationship("salesliterature", EntityRole.Referenced, "lk_salesliteraturebase_modifiedby", "modifiedby")]
			public const string lk_salesliteraturebase_modifiedby = "lk_salesliteraturebase_modifiedby";
			[Relationship("salesliteratureitem", EntityRole.Referenced, "lk_salesliteratureitem_createdonbehalfby", "createdonbehalfby")]
			public const string lk_salesliteratureitem_createdonbehalfby = "lk_salesliteratureitem_createdonbehalfby";
			[Relationship("salesliteratureitem", EntityRole.Referenced, "lk_salesliteratureitem_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_salesliteratureitem_modifiedonbehalfby = "lk_salesliteratureitem_modifiedonbehalfby";
			[Relationship("salesliteratureitem", EntityRole.Referenced, "lk_salesliteratureitembase_createdby", "createdby")]
			public const string lk_salesliteratureitembase_createdby = "lk_salesliteratureitembase_createdby";
			[Relationship("salesliteratureitem", EntityRole.Referenced, "lk_salesliteratureitembase_modifiedby", "modifiedby")]
			public const string lk_salesliteratureitembase_modifiedby = "lk_salesliteratureitembase_modifiedby";
			[Relationship("salesorder", EntityRole.Referenced, "lk_salesorder_createdonbehalfby", "createdonbehalfby")]
			public const string lk_salesorder_createdonbehalfby = "lk_salesorder_createdonbehalfby";
			[Relationship("salesorder", EntityRole.Referenced, "lk_salesorder_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_salesorder_modifiedonbehalfby = "lk_salesorder_modifiedonbehalfby";
			[Relationship("salesorder", EntityRole.Referenced, "lk_salesorderbase_createdby", "createdby")]
			public const string lk_salesorderbase_createdby = "lk_salesorderbase_createdby";
			[Relationship("salesorder", EntityRole.Referenced, "lk_salesorderbase_modifiedby", "modifiedby")]
			public const string lk_salesorderbase_modifiedby = "lk_salesorderbase_modifiedby";
			[Relationship("salesorderdetail", EntityRole.Referenced, "lk_salesorderdetail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_salesorderdetail_createdonbehalfby = "lk_salesorderdetail_createdonbehalfby";
			[Relationship("salesorderdetail", EntityRole.Referenced, "lk_salesorderdetail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_salesorderdetail_modifiedonbehalfby = "lk_salesorderdetail_modifiedonbehalfby";
			[Relationship("salesorderdetail", EntityRole.Referenced, "lk_salesorderdetailbase_createdby", "createdby")]
			public const string lk_salesorderdetailbase_createdby = "lk_salesorderdetailbase_createdby";
			[Relationship("salesorderdetail", EntityRole.Referenced, "lk_salesorderdetailbase_modifiedby", "modifiedby")]
			public const string lk_salesorderdetailbase_modifiedby = "lk_salesorderdetailbase_modifiedby";
			[Relationship("savedorginsightsconfiguration", EntityRole.Referenced, "lk_savedorginsightsconfiguration_createdby", "createdby")]
			public const string lk_savedorginsightsconfiguration_createdby = "lk_savedorginsightsconfiguration_createdby";
			[Relationship("savedorginsightsconfiguration", EntityRole.Referenced, "lk_savedorginsightsconfiguration_createdonbehalfby", "createdonbehalfby")]
			public const string lk_savedorginsightsconfiguration_createdonbehalfby = "lk_savedorginsightsconfiguration_createdonbehalfby";
			[Relationship("savedorginsightsconfiguration", EntityRole.Referenced, "lk_savedorginsightsconfiguration_modifiedby", "modifiedby")]
			public const string lk_savedorginsightsconfiguration_modifiedby = "lk_savedorginsightsconfiguration_modifiedby";
			[Relationship("savedorginsightsconfiguration", EntityRole.Referenced, "lk_savedorginsightsconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_savedorginsightsconfiguration_modifiedonbehalfby = "lk_savedorginsightsconfiguration_modifiedonbehalfby";
			[Relationship("savedquery", EntityRole.Referenced, "lk_savedquery_createdonbehalfby", "createdonbehalfby")]
			public const string lk_savedquery_createdonbehalfby = "lk_savedquery_createdonbehalfby";
			[Relationship("savedquery", EntityRole.Referenced, "lk_savedquery_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_savedquery_modifiedonbehalfby = "lk_savedquery_modifiedonbehalfby";
			[Relationship("savedquery", EntityRole.Referenced, "lk_savedquerybase_createdby", "createdby")]
			public const string lk_savedquerybase_createdby = "lk_savedquerybase_createdby";
			[Relationship("savedquery", EntityRole.Referenced, "lk_savedquerybase_modifiedby", "modifiedby")]
			public const string lk_savedquerybase_modifiedby = "lk_savedquerybase_modifiedby";
			[Relationship("savedqueryvisualization", EntityRole.Referenced, "lk_savedqueryvisualizationbase_createdby", "createdby")]
			public const string lk_savedqueryvisualizationbase_createdby = "lk_savedqueryvisualizationbase_createdby";
			[Relationship("savedqueryvisualization", EntityRole.Referenced, "lk_savedqueryvisualizationbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_savedqueryvisualizationbase_createdonbehalfby = "lk_savedqueryvisualizationbase_createdonbehalfby";
			[Relationship("savedqueryvisualization", EntityRole.Referenced, "lk_savedqueryvisualizationbase_modifiedby", "modifiedby")]
			public const string lk_savedqueryvisualizationbase_modifiedby = "lk_savedqueryvisualizationbase_modifiedby";
			[Relationship("savedqueryvisualization", EntityRole.Referenced, "lk_savedqueryvisualizationbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_savedqueryvisualizationbase_modifiedonbehalfby = "lk_savedqueryvisualizationbase_modifiedonbehalfby";
			[Relationship("sdkmessage", EntityRole.Referenced, "lk_sdkmessage_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessage_createdonbehalfby = "lk_sdkmessage_createdonbehalfby";
			[Relationship("sdkmessage", EntityRole.Referenced, "lk_sdkmessage_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessage_modifiedonbehalfby = "lk_sdkmessage_modifiedonbehalfby";
			[Relationship("sdkmessagefilter", EntityRole.Referenced, "lk_sdkmessagefilter_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessagefilter_createdonbehalfby = "lk_sdkmessagefilter_createdonbehalfby";
			[Relationship("sdkmessagefilter", EntityRole.Referenced, "lk_sdkmessagefilter_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessagefilter_modifiedonbehalfby = "lk_sdkmessagefilter_modifiedonbehalfby";
			[Relationship("sdkmessagepair", EntityRole.Referenced, "lk_sdkmessagepair_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessagepair_createdonbehalfby = "lk_sdkmessagepair_createdonbehalfby";
			[Relationship("sdkmessagepair", EntityRole.Referenced, "lk_sdkmessagepair_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessagepair_modifiedonbehalfby = "lk_sdkmessagepair_modifiedonbehalfby";
			[Relationship("sdkmessageprocessingstep", EntityRole.Referenced, "lk_sdkmessageprocessingstep_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessageprocessingstep_createdonbehalfby = "lk_sdkmessageprocessingstep_createdonbehalfby";
			[Relationship("sdkmessageprocessingstep", EntityRole.Referenced, "lk_sdkmessageprocessingstep_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessageprocessingstep_modifiedonbehalfby = "lk_sdkmessageprocessingstep_modifiedonbehalfby";
			[Relationship("sdkmessageprocessingstepimage", EntityRole.Referenced, "lk_sdkmessageprocessingstepimage_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessageprocessingstepimage_createdonbehalfby = "lk_sdkmessageprocessingstepimage_createdonbehalfby";
			[Relationship("sdkmessageprocessingstepimage", EntityRole.Referenced, "lk_sdkmessageprocessingstepimage_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessageprocessingstepimage_modifiedonbehalfby = "lk_sdkmessageprocessingstepimage_modifiedonbehalfby";
			[Relationship("sdkmessageprocessingstepsecureconfig", EntityRole.Referenced, "lk_sdkmessageprocessingstepsecureconfig_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessageprocessingstepsecureconfig_createdonbehalfby = "lk_sdkmessageprocessingstepsecureconfig_createdonbehalfby";
			[Relationship("sdkmessageprocessingstepsecureconfig", EntityRole.Referenced, "lk_sdkmessageprocessingstepsecureconfig_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessageprocessingstepsecureconfig_modifiedonbehalfby = "lk_sdkmessageprocessingstepsecureconfig_modifiedonbehalfby";
			[Relationship("sdkmessagerequest", EntityRole.Referenced, "lk_sdkmessagerequest_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessagerequest_createdonbehalfby = "lk_sdkmessagerequest_createdonbehalfby";
			[Relationship("sdkmessagerequest", EntityRole.Referenced, "lk_sdkmessagerequest_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessagerequest_modifiedonbehalfby = "lk_sdkmessagerequest_modifiedonbehalfby";
			[Relationship("sdkmessagerequestfield", EntityRole.Referenced, "lk_sdkmessagerequestfield_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessagerequestfield_createdonbehalfby = "lk_sdkmessagerequestfield_createdonbehalfby";
			[Relationship("sdkmessagerequestfield", EntityRole.Referenced, "lk_sdkmessagerequestfield_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessagerequestfield_modifiedonbehalfby = "lk_sdkmessagerequestfield_modifiedonbehalfby";
			[Relationship("sdkmessageresponse", EntityRole.Referenced, "lk_sdkmessageresponse_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessageresponse_createdonbehalfby = "lk_sdkmessageresponse_createdonbehalfby";
			[Relationship("sdkmessageresponse", EntityRole.Referenced, "lk_sdkmessageresponse_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessageresponse_modifiedonbehalfby = "lk_sdkmessageresponse_modifiedonbehalfby";
			[Relationship("sdkmessageresponsefield", EntityRole.Referenced, "lk_sdkmessageresponsefield_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessageresponsefield_createdonbehalfby = "lk_sdkmessageresponsefield_createdonbehalfby";
			[Relationship("sdkmessageresponsefield", EntityRole.Referenced, "lk_sdkmessageresponsefield_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessageresponsefield_modifiedonbehalfby = "lk_sdkmessageresponsefield_modifiedonbehalfby";
			[Relationship("semiannualfiscalcalendar", EntityRole.Referenced, "lk_semiannualfiscalcalendar_createdby", "createdby")]
			public const string lk_semiannualfiscalcalendar_createdby = "lk_semiannualfiscalcalendar_createdby";
			[Relationship("semiannualfiscalcalendar", EntityRole.Referenced, "lk_semiannualfiscalcalendar_createdonbehalfby", "createdonbehalfby")]
			public const string lk_semiannualfiscalcalendar_createdonbehalfby = "lk_semiannualfiscalcalendar_createdonbehalfby";
			[Relationship("semiannualfiscalcalendar", EntityRole.Referenced, "lk_semiannualfiscalcalendar_modifiedby", "modifiedby")]
			public const string lk_semiannualfiscalcalendar_modifiedby = "lk_semiannualfiscalcalendar_modifiedby";
			[Relationship("semiannualfiscalcalendar", EntityRole.Referenced, "lk_semiannualfiscalcalendar_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_semiannualfiscalcalendar_modifiedonbehalfby = "lk_semiannualfiscalcalendar_modifiedonbehalfby";
			[Relationship("semiannualfiscalcalendar", EntityRole.Referenced, "lk_semiannualfiscalcalendar_salespersonid", "salespersonid")]
			public const string lk_semiannualfiscalcalendar_salespersonid = "lk_semiannualfiscalcalendar_salespersonid";
			[Relationship("service", EntityRole.Referenced, "lk_service_createdby", "createdby")]
			public const string lk_service_createdby = "lk_service_createdby";
			[Relationship("service", EntityRole.Referenced, "lk_service_createdonbehalfby", "createdonbehalfby")]
			public const string lk_service_createdonbehalfby = "lk_service_createdonbehalfby";
			[Relationship("service", EntityRole.Referenced, "lk_service_modifiedby", "modifiedby")]
			public const string lk_service_modifiedby = "lk_service_modifiedby";
			[Relationship("service", EntityRole.Referenced, "lk_service_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_service_modifiedonbehalfby = "lk_service_modifiedonbehalfby";
			[Relationship("serviceappointment", EntityRole.Referenced, "lk_serviceappointment_createdby", "createdby")]
			public const string lk_serviceappointment_createdby = "lk_serviceappointment_createdby";
			[Relationship("serviceappointment", EntityRole.Referenced, "lk_serviceappointment_createdonbehalfby", "createdonbehalfby")]
			public const string lk_serviceappointment_createdonbehalfby = "lk_serviceappointment_createdonbehalfby";
			[Relationship("serviceappointment", EntityRole.Referenced, "lk_serviceappointment_modifiedby", "modifiedby")]
			public const string lk_serviceappointment_modifiedby = "lk_serviceappointment_modifiedby";
			[Relationship("serviceappointment", EntityRole.Referenced, "lk_serviceappointment_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_serviceappointment_modifiedonbehalfby = "lk_serviceappointment_modifiedonbehalfby";
			[Relationship("serviceendpoint", EntityRole.Referenced, "lk_serviceendpointbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_serviceendpointbase_createdonbehalfby = "lk_serviceendpointbase_createdonbehalfby";
			[Relationship("serviceendpoint", EntityRole.Referenced, "lk_serviceendpointbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_serviceendpointbase_modifiedonbehalfby = "lk_serviceendpointbase_modifiedonbehalfby";
			[Relationship("sharepointdata", EntityRole.Referenced, "lk_SharePointData_createdby", "createdby")]
			public const string lk_SharePointData_createdby = "lk_SharePointData_createdby";
			[Relationship("sharepointdata", EntityRole.Referenced, "lk_SharePointData_createdonbehalfby", "createdonbehalfby")]
			public const string lk_SharePointData_createdonbehalfby = "lk_SharePointData_createdonbehalfby";
			[Relationship("sharepointdata", EntityRole.Referenced, "lk_SharePointData_modifiedby", "modifiedby")]
			public const string lk_SharePointData_modifiedby = "lk_SharePointData_modifiedby";
			[Relationship("sharepointdata", EntityRole.Referenced, "lk_SharePointData_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_SharePointData_modifiedonbehalfby = "lk_SharePointData_modifiedonbehalfby";
			[Relationship("sharepointdata", EntityRole.Referenced, "lk_sharepointdata_user", "userid")]
			public const string lk_sharepointdata_user = "lk_sharepointdata_user";
			[Relationship("sharepointdocument", EntityRole.Referenced, "lk_sharepointdocumentbase_createdby", "createdby")]
			public const string lk_sharepointdocumentbase_createdby = "lk_sharepointdocumentbase_createdby";
			[Relationship("sharepointdocument", EntityRole.Referenced, "lk_sharepointdocumentbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sharepointdocumentbase_createdonbehalfby = "lk_sharepointdocumentbase_createdonbehalfby";
			[Relationship("sharepointdocument", EntityRole.Referenced, "lk_sharepointdocumentbase_modifiedby", "modifiedby")]
			public const string lk_sharepointdocumentbase_modifiedby = "lk_sharepointdocumentbase_modifiedby";
			[Relationship("sharepointdocument", EntityRole.Referenced, "lk_sharepointdocumentbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sharepointdocumentbase_modifiedonbehalfby = "lk_sharepointdocumentbase_modifiedonbehalfby";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "lk_sharepointdocumentlocationbase_createdby", "createdby")]
			public const string lk_sharepointdocumentlocationbase_createdby = "lk_sharepointdocumentlocationbase_createdby";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "lk_sharepointdocumentlocationbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sharepointdocumentlocationbase_createdonbehalfby = "lk_sharepointdocumentlocationbase_createdonbehalfby";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "lk_sharepointdocumentlocationbase_modifiedby", "modifiedby")]
			public const string lk_sharepointdocumentlocationbase_modifiedby = "lk_sharepointdocumentlocationbase_modifiedby";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "lk_sharepointdocumentlocationbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sharepointdocumentlocationbase_modifiedonbehalfby = "lk_sharepointdocumentlocationbase_modifiedonbehalfby";
			[Relationship("sharepointsite", EntityRole.Referenced, "lk_sharepointsitebase_createdby", "createdby")]
			public const string lk_sharepointsitebase_createdby = "lk_sharepointsitebase_createdby";
			[Relationship("sharepointsite", EntityRole.Referenced, "lk_sharepointsitebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_sharepointsitebase_createdonbehalfby = "lk_sharepointsitebase_createdonbehalfby";
			[Relationship("sharepointsite", EntityRole.Referenced, "lk_sharepointsitebase_modifiedby", "modifiedby")]
			public const string lk_sharepointsitebase_modifiedby = "lk_sharepointsitebase_modifiedby";
			[Relationship("sharepointsite", EntityRole.Referenced, "lk_sharepointsitebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sharepointsitebase_modifiedonbehalfby = "lk_sharepointsitebase_modifiedonbehalfby";
			[Relationship("similarityrule", EntityRole.Referenced, "lk_similarityrule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_similarityrule_createdonbehalfby = "lk_similarityrule_createdonbehalfby";
			[Relationship("similarityrule", EntityRole.Referenced, "lk_similarityrule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_similarityrule_modifiedonbehalfby = "lk_similarityrule_modifiedonbehalfby";
			[Relationship("site", EntityRole.Referenced, "lk_site_createdonbehalfby", "createdonbehalfby")]
			public const string lk_site_createdonbehalfby = "lk_site_createdonbehalfby";
			[Relationship("site", EntityRole.Referenced, "lk_site_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_site_modifiedonbehalfby = "lk_site_modifiedonbehalfby";
			[Relationship("site", EntityRole.Referenced, "lk_sitebase_createdby", "createdby")]
			public const string lk_sitebase_createdby = "lk_sitebase_createdby";
			[Relationship("site", EntityRole.Referenced, "lk_sitebase_modifiedby", "modifiedby")]
			public const string lk_sitebase_modifiedby = "lk_sitebase_modifiedby";
			[Relationship("sitemap", EntityRole.Referenced, "systemuser_SiteMap_createdby", "createdby")]
			public const string lk_SiteMap_createdby = "lk_SiteMap_createdby";
			[Relationship("sitemap", EntityRole.Referenced, "systemuser_SiteMap_createdonbehalfby", "createdonbehalfby")]
			public const string lk_SiteMap_createdonbehalfby = "lk_SiteMap_createdonbehalfby";
			[Relationship("sitemap", EntityRole.Referenced, "systemuser_SiteMap_modifiedby", "modifiedby")]
			public const string lk_SiteMap_modifiedby = "lk_SiteMap_modifiedby";
			[Relationship("sitemap", EntityRole.Referenced, "systemuser_SiteMap_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_SiteMap_modifiedonbehalfby = "lk_SiteMap_modifiedonbehalfby";
			[Relationship("sla", EntityRole.Referenced, "lk_slabase_createdby", "createdby")]
			public const string lk_slabase_createdby = "lk_slabase_createdby";
			[Relationship("sla", EntityRole.Referenced, "lk_slabase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_slabase_createdonbehalfby = "lk_slabase_createdonbehalfby";
			[Relationship("sla", EntityRole.Referenced, "lk_slabase_modifiedby", "modifiedby")]
			public const string lk_slabase_modifiedby = "lk_slabase_modifiedby";
			[Relationship("sla", EntityRole.Referenced, "lk_slabase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_slabase_modifiedonbehalfby = "lk_slabase_modifiedonbehalfby";
			[Relationship("slaitem", EntityRole.Referenced, "lk_slaitembase_createdby", "createdby")]
			public const string lk_slaitembase_createdby = "lk_slaitembase_createdby";
			[Relationship("slaitem", EntityRole.Referenced, "lk_slaitembase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_slaitembase_createdonbehalfby = "lk_slaitembase_createdonbehalfby";
			[Relationship("slaitem", EntityRole.Referenced, "lk_slaitembase_modifiedby", "modifiedby")]
			public const string lk_slaitembase_modifiedby = "lk_slaitembase_modifiedby";
			[Relationship("slaitem", EntityRole.Referenced, "lk_slaitembase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_slaitembase_modifiedonbehalfby = "lk_slaitembase_modifiedonbehalfby";
			[Relationship("slakpiinstance", EntityRole.Referenced, "lk_slakpiinstancebase_createdby", "createdby")]
			public const string lk_slakpiinstancebase_createdby = "lk_slakpiinstancebase_createdby";
			[Relationship("slakpiinstance", EntityRole.Referenced, "lk_slakpiinstancebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_slakpiinstancebase_createdonbehalfby = "lk_slakpiinstancebase_createdonbehalfby";
			[Relationship("slakpiinstance", EntityRole.Referenced, "lk_slakpiinstancebase_modifiedby", "modifiedby")]
			public const string lk_slakpiinstancebase_modifiedby = "lk_slakpiinstancebase_modifiedby";
			[Relationship("slakpiinstance", EntityRole.Referenced, "lk_slakpiinstancebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_slakpiinstancebase_modifiedonbehalfby = "lk_slakpiinstancebase_modifiedonbehalfby";
			[Relationship("socialactivity", EntityRole.Referenced, "lk_socialactivity_createdby", "createdby")]
			public const string lk_socialactivity_createdby = "lk_socialactivity_createdby";
			[Relationship("socialactivity", EntityRole.Referenced, "lk_socialactivity_modifiedby", "modifiedby")]
			public const string lk_socialactivity_modifiedby = "lk_socialactivity_modifiedby";
			[Relationship("socialactivity", EntityRole.Referenced, "lk_socialactivitybase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_socialactivitybase_createdonbehalfby = "lk_socialactivitybase_createdonbehalfby";
			[Relationship("socialactivity", EntityRole.Referenced, "lk_socialactivitybase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_socialactivitybase_modifiedonbehalfby = "lk_socialactivitybase_modifiedonbehalfby";
			[Relationship("socialinsightsconfiguration", EntityRole.Referenced, "lk_socialinsightsconfiguration_createdby", "createdby")]
			public const string lk_socialinsightsconfiguration_createdby = "lk_socialinsightsconfiguration_createdby";
			[Relationship("socialinsightsconfiguration", EntityRole.Referenced, "lk_socialinsightsconfiguration_createdonbehalfby", "createdonbehalfby")]
			public const string lk_socialinsightsconfiguration_createdonbehalfby = "lk_socialinsightsconfiguration_createdonbehalfby";
			[Relationship("socialinsightsconfiguration", EntityRole.Referenced, "lk_socialinsightsconfiguration_modifiedby", "modifiedby")]
			public const string lk_socialinsightsconfiguration_modifiedby = "lk_socialinsightsconfiguration_modifiedby";
			[Relationship("socialinsightsconfiguration", EntityRole.Referenced, "lk_socialinsightsconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_socialinsightsconfiguration_modifiedonbehalfby = "lk_socialinsightsconfiguration_modifiedonbehalfby";
			[Relationship("socialprofile", EntityRole.Referenced, "lk_SocialProfile_createdonbehalfby", "createdonbehalfby")]
			public const string lk_SocialProfile_createdonbehalfby = "lk_SocialProfile_createdonbehalfby";
			[Relationship("socialprofile", EntityRole.Referenced, "lk_SocialProfile_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_SocialProfile_modifiedonbehalfby = "lk_SocialProfile_modifiedonbehalfby";
			[Relationship("solution", EntityRole.Referenced, "lk_solution_createdby", "createdby")]
			public const string lk_solution_createdby = "lk_solution_createdby";
			[Relationship("solution", EntityRole.Referenced, "lk_solution_modifiedby", "modifiedby")]
			public const string lk_solution_modifiedby = "lk_solution_modifiedby";
			[Relationship("solution", EntityRole.Referenced, "lk_solutionbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_solutionbase_createdonbehalfby = "lk_solutionbase_createdonbehalfby";
			[Relationship("solution", EntityRole.Referenced, "lk_solutionbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_solutionbase_modifiedonbehalfby = "lk_solutionbase_modifiedonbehalfby";
			[Relationship("solutioncomponent", EntityRole.Referenced, "lk_solutioncomponentbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_solutioncomponentbase_createdonbehalfby = "lk_solutioncomponentbase_createdonbehalfby";
			[Relationship("solutioncomponent", EntityRole.Referenced, "lk_solutioncomponentbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_solutioncomponentbase_modifiedonbehalfby = "lk_solutioncomponentbase_modifiedonbehalfby";
			[Relationship("subject", EntityRole.Referenced, "lk_subject_createdonbehalfby", "createdonbehalfby")]
			public const string lk_subject_createdonbehalfby = "lk_subject_createdonbehalfby";
			[Relationship("subject", EntityRole.Referenced, "lk_subject_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_subject_modifiedonbehalfby = "lk_subject_modifiedonbehalfby";
			[Relationship("subject", EntityRole.Referenced, "lk_subjectbase_createdby", "createdby")]
			public const string lk_subjectbase_createdby = "lk_subjectbase_createdby";
			[Relationship("subject", EntityRole.Referenced, "lk_subjectbase_modifiedby", "modifiedby")]
			public const string lk_subjectbase_modifiedby = "lk_subjectbase_modifiedby";
			[Relationship("suggestioncardtemplate", EntityRole.Referenced, "lk_suggestioncardtemplate_createdby", "createdby")]
			public const string lk_suggestioncardtemplate_createdby = "lk_suggestioncardtemplate_createdby";
			[Relationship("suggestioncardtemplate", EntityRole.Referenced, "lk_suggestioncardtemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_suggestioncardtemplate_createdonbehalfby = "lk_suggestioncardtemplate_createdonbehalfby";
			[Relationship("suggestioncardtemplate", EntityRole.Referenced, "lk_suggestioncardtemplate_modifiedby", "modifiedby")]
			public const string lk_suggestioncardtemplate_modifiedby = "lk_suggestioncardtemplate_modifiedby";
			[Relationship("suggestioncardtemplate", EntityRole.Referenced, "lk_suggestioncardtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_suggestioncardtemplate_modifiedonbehalfby = "lk_suggestioncardtemplate_modifiedonbehalfby";
			[Relationship("syncattributemappingprofile", EntityRole.Referenced, "lk_syncattributemappingprofile_createdby", "createdby")]
			public const string lk_syncattributemappingprofile_createdby = "lk_syncattributemappingprofile_createdby";
			[Relationship("syncattributemappingprofile", EntityRole.Referenced, "lk_syncattributemappingprofile_createdonbehalfby", "createdonbehalfby")]
			public const string lk_syncattributemappingprofile_createdonbehalfby = "lk_syncattributemappingprofile_createdonbehalfby";
			[Relationship("syncattributemappingprofile", EntityRole.Referenced, "lk_syncattributemappingprofile_modifiedby", "modifiedby")]
			public const string lk_syncattributemappingprofile_modifiedby = "lk_syncattributemappingprofile_modifiedby";
			[Relationship("syncattributemappingprofile", EntityRole.Referenced, "lk_syncattributemappingprofile_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_syncattributemappingprofile_modifiedonbehalfby = "lk_syncattributemappingprofile_modifiedonbehalfby";
			[Relationship("syncerror", EntityRole.Referenced, "lk_syncerrorbase_createdby", "createdby")]
			public const string lk_syncerrorbase_createdby = "lk_syncerrorbase_createdby";
			[Relationship("syncerror", EntityRole.Referenced, "lk_syncerrorbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_syncerrorbase_createdonbehalfby = "lk_syncerrorbase_createdonbehalfby";
			[Relationship("syncerror", EntityRole.Referenced, "lk_syncerrorbase_modifiedby", "modifiedby")]
			public const string lk_syncerrorbase_modifiedby = "lk_syncerrorbase_modifiedby";
			[Relationship("syncerror", EntityRole.Referenced, "lk_syncerrorbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_syncerrorbase_modifiedonbehalfby = "lk_syncerrorbase_modifiedonbehalfby";
			[Relationship("systemapplicationmetadata", EntityRole.Referenced, "lk_systemapplicationmetadata_createdby", "createdby")]
			public const string lk_systemapplicationmetadata_createdby = "lk_systemapplicationmetadata_createdby";
			[Relationship("systemapplicationmetadata", EntityRole.Referenced, "lk_systemapplicationmetadata_createdonbehalfby", "createdonbehalfby")]
			public const string lk_systemapplicationmetadata_createdonbehalfby = "lk_systemapplicationmetadata_createdonbehalfby";
			[Relationship("systemapplicationmetadata", EntityRole.Referenced, "lk_systemapplicationmetadata_modifiedby", "modifiedby")]
			public const string lk_systemapplicationmetadata_modifiedby = "lk_systemapplicationmetadata_modifiedby";
			[Relationship("systemapplicationmetadata", EntityRole.Referenced, "lk_systemapplicationmetadata_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_systemapplicationmetadata_modifiedonbehalfby = "lk_systemapplicationmetadata_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referenced, "lk_systemuser_createdonbehalfby", "createdonbehalfby")]
			public const string lk_systemuser_createdonbehalfby = "lk_systemuser_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referenced, "lk_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_systemuser_modifiedonbehalfby = "lk_systemuser_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referenced, "lk_systemuserbase_createdby", "createdby")]
			public const string lk_systemuserbase_createdby = "lk_systemuserbase_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referenced, "lk_systemuserbase_modifiedby", "modifiedby")]
			public const string lk_systemuserbase_modifiedby = "lk_systemuserbase_modifiedby";
			[Relationship("task", EntityRole.Referenced, "lk_task_createdby", "createdby")]
			public const string lk_task_createdby = "lk_task_createdby";
			[Relationship("task", EntityRole.Referenced, "lk_task_createdonbehalfby", "createdonbehalfby")]
			public const string lk_task_createdonbehalfby = "lk_task_createdonbehalfby";
			[Relationship("task", EntityRole.Referenced, "lk_task_modifiedby", "modifiedby")]
			public const string lk_task_modifiedby = "lk_task_modifiedby";
			[Relationship("task", EntityRole.Referenced, "lk_task_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_task_modifiedonbehalfby = "lk_task_modifiedonbehalfby";
			[Relationship("team", EntityRole.Referenced, "lk_team_createdonbehalfby", "createdonbehalfby")]
			public const string lk_team_createdonbehalfby = "lk_team_createdonbehalfby";
			[Relationship("team", EntityRole.Referenced, "lk_team_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_team_modifiedonbehalfby = "lk_team_modifiedonbehalfby";
			[Relationship("team", EntityRole.Referenced, "lk_teambase_administratorid", "administratorid")]
			public const string lk_teambase_administratorid = "lk_teambase_administratorid";
			[Relationship("team", EntityRole.Referenced, "lk_teambase_createdby", "createdby")]
			public const string lk_teambase_createdby = "lk_teambase_createdby";
			[Relationship("team", EntityRole.Referenced, "lk_teambase_modifiedby", "modifiedby")]
			public const string lk_teambase_modifiedby = "lk_teambase_modifiedby";
			[Relationship("teamtemplate", EntityRole.Referenced, "lk_teamtemplate_createdby", "createdby")]
			public const string lk_teamtemplate_createdby = "lk_teamtemplate_createdby";
			[Relationship("teamtemplate", EntityRole.Referenced, "lk_teamtemplate_createdonbehalfby", "createdonbehalfby")]
			public const string lk_teamtemplate_createdonbehalfby = "lk_teamtemplate_createdonbehalfby";
			[Relationship("teamtemplate", EntityRole.Referenced, "lk_teamtemplate_modifiedby", "modifiedby")]
			public const string lk_teamtemplate_modifiedby = "lk_teamtemplate_modifiedby";
			[Relationship("teamtemplate", EntityRole.Referenced, "lk_teamtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_teamtemplate_modifiedonbehalfby = "lk_teamtemplate_modifiedonbehalfby";
			[Relationship("template", EntityRole.Referenced, "lk_templatebase_createdby", "createdby")]
			public const string lk_templatebase_createdby = "lk_templatebase_createdby";
			[Relationship("template", EntityRole.Referenced, "lk_templatebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_templatebase_createdonbehalfby = "lk_templatebase_createdonbehalfby";
			[Relationship("template", EntityRole.Referenced, "lk_templatebase_modifiedby", "modifiedby")]
			public const string lk_templatebase_modifiedby = "lk_templatebase_modifiedby";
			[Relationship("template", EntityRole.Referenced, "lk_templatebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_templatebase_modifiedonbehalfby = "lk_templatebase_modifiedonbehalfby";
			[Relationship("territory", EntityRole.Referenced, "lk_territory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_territory_createdonbehalfby = "lk_territory_createdonbehalfby";
			[Relationship("territory", EntityRole.Referenced, "lk_territory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_territory_modifiedonbehalfby = "lk_territory_modifiedonbehalfby";
			[Relationship("territory", EntityRole.Referenced, "lk_territorybase_createdby", "createdby")]
			public const string lk_territorybase_createdby = "lk_territorybase_createdby";
			[Relationship("territory", EntityRole.Referenced, "lk_territorybase_modifiedby", "modifiedby")]
			public const string lk_territorybase_modifiedby = "lk_territorybase_modifiedby";
			[Relationship("theme", EntityRole.Referenced, "lk_theme_createdby", "createdby")]
			public const string lk_theme_createdby = "lk_theme_createdby";
			[Relationship("theme", EntityRole.Referenced, "lk_theme_createdonbehalfby", "createdonbehalfby")]
			public const string lk_theme_createdonbehalfby = "lk_theme_createdonbehalfby";
			[Relationship("theme", EntityRole.Referenced, "lk_theme_modifiedby", "modifiedby")]
			public const string lk_theme_modifiedby = "lk_theme_modifiedby";
			[Relationship("theme", EntityRole.Referenced, "lk_theme_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_theme_modifiedonbehalfby = "lk_theme_modifiedonbehalfby";
			[Relationship("timezonedefinition", EntityRole.Referenced, "lk_timezonedefinition_createdby", "createdby")]
			public const string lk_timezonedefinition_createdby = "lk_timezonedefinition_createdby";
			[Relationship("timezonedefinition", EntityRole.Referenced, "lk_timezonedefinition_createdonbehalfby", "createdonbehalfby")]
			public const string lk_timezonedefinition_createdonbehalfby = "lk_timezonedefinition_createdonbehalfby";
			[Relationship("timezonedefinition", EntityRole.Referenced, "lk_timezonedefinition_modifiedby", "modifiedby")]
			public const string lk_timezonedefinition_modifiedby = "lk_timezonedefinition_modifiedby";
			[Relationship("timezonedefinition", EntityRole.Referenced, "lk_timezonedefinition_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_timezonedefinition_modifiedonbehalfby = "lk_timezonedefinition_modifiedonbehalfby";
			[Relationship("timezonelocalizedname", EntityRole.Referenced, "lk_timezonelocalizedname_createdby", "createdby")]
			public const string lk_timezonelocalizedname_createdby = "lk_timezonelocalizedname_createdby";
			[Relationship("timezonelocalizedname", EntityRole.Referenced, "lk_timezonelocalizedname_createdonbehalfby", "createdonbehalfby")]
			public const string lk_timezonelocalizedname_createdonbehalfby = "lk_timezonelocalizedname_createdonbehalfby";
			[Relationship("timezonelocalizedname", EntityRole.Referenced, "lk_timezonelocalizedname_modifiedby", "modifiedby")]
			public const string lk_timezonelocalizedname_modifiedby = "lk_timezonelocalizedname_modifiedby";
			[Relationship("timezonelocalizedname", EntityRole.Referenced, "lk_timezonelocalizedname_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_timezonelocalizedname_modifiedonbehalfby = "lk_timezonelocalizedname_modifiedonbehalfby";
			[Relationship("timezonerule", EntityRole.Referenced, "lk_timezonerule_createdby", "createdby")]
			public const string lk_timezonerule_createdby = "lk_timezonerule_createdby";
			[Relationship("timezonerule", EntityRole.Referenced, "lk_timezonerule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_timezonerule_createdonbehalfby = "lk_timezonerule_createdonbehalfby";
			[Relationship("timezonerule", EntityRole.Referenced, "lk_timezonerule_modifiedby", "modifiedby")]
			public const string lk_timezonerule_modifiedby = "lk_timezonerule_modifiedby";
			[Relationship("timezonerule", EntityRole.Referenced, "lk_timezonerule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_timezonerule_modifiedonbehalfby = "lk_timezonerule_modifiedonbehalfby";
			[Relationship("tit_smssetup", EntityRole.Referenced, "lk_tit_smssetup_createdby", "")]
			public const string lk_tit_smssetup_createdby = "lk_tit_smssetup_createdby";
			[Relationship("tit_smssetup", EntityRole.Referenced, "lk_tit_smssetup_createdonbehalfby", "")]
			public const string lk_tit_smssetup_createdonbehalfby = "lk_tit_smssetup_createdonbehalfby";
			[Relationship("tit_smssetup", EntityRole.Referenced, "lk_tit_smssetup_modifiedby", "")]
			public const string lk_tit_smssetup_modifiedby = "lk_tit_smssetup_modifiedby";
			[Relationship("tit_smssetup", EntityRole.Referenced, "lk_tit_smssetup_modifiedonbehalfby", "")]
			public const string lk_tit_smssetup_modifiedonbehalfby = "lk_tit_smssetup_modifiedonbehalfby";
			[Relationship("topicmodel", EntityRole.Referenced, "lk_topicmodel_createdby", "createdby")]
			public const string lk_topicmodel_createdby = "lk_topicmodel_createdby";
			[Relationship("topicmodel", EntityRole.Referenced, "lk_topicmodel_createdonbehalfby", "createdonbehalfby")]
			public const string lk_topicmodel_createdonbehalfby = "lk_topicmodel_createdonbehalfby";
			[Relationship("topicmodel", EntityRole.Referenced, "lk_topicmodel_modifiedby", "modifiedby")]
			public const string lk_topicmodel_modifiedby = "lk_topicmodel_modifiedby";
			[Relationship("topicmodel", EntityRole.Referenced, "lk_topicmodel_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_topicmodel_modifiedonbehalfby = "lk_topicmodel_modifiedonbehalfby";
			[Relationship("topicmodelexecutionhistory", EntityRole.Referenced, "lk_topicmodelexecutionhistory_createdby", "createdby")]
			public const string lk_topicmodelexecutionhistory_createdby = "lk_topicmodelexecutionhistory_createdby";
			[Relationship("topicmodelexecutionhistory", EntityRole.Referenced, "lk_topicmodelexecutionhistory_createdonbehalfby", "createdonbehalfby")]
			public const string lk_topicmodelexecutionhistory_createdonbehalfby = "lk_topicmodelexecutionhistory_createdonbehalfby";
			[Relationship("topicmodelexecutionhistory", EntityRole.Referenced, "lk_topicmodelexecutionhistory_modifiedby", "modifiedby")]
			public const string lk_topicmodelexecutionhistory_modifiedby = "lk_topicmodelexecutionhistory_modifiedby";
			[Relationship("topicmodelexecutionhistory", EntityRole.Referenced, "lk_topicmodelexecutionhistory_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_topicmodelexecutionhistory_modifiedonbehalfby = "lk_topicmodelexecutionhistory_modifiedonbehalfby";
			[Relationship("tracelog", EntityRole.Referenced, "lk_tracelog_createdby", "createdby")]
			public const string lk_tracelog_createdby = "lk_tracelog_createdby";
			[Relationship("tracelog", EntityRole.Referenced, "lk_tracelog_createdonbehalfby", "createdonbehalfby")]
			public const string lk_tracelog_createdonbehalfby = "lk_tracelog_createdonbehalfby";
			[Relationship("tracelog", EntityRole.Referenced, "lk_tracelog_modifiedby", "modifiedby")]
			public const string lk_tracelog_modifiedby = "lk_tracelog_modifiedby";
			[Relationship("tracelog", EntityRole.Referenced, "lk_tracelog_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_tracelog_modifiedonbehalfby = "lk_tracelog_modifiedonbehalfby";
			[Relationship("transactioncurrency", EntityRole.Referenced, "lk_transactioncurrency_createdonbehalfby", "createdonbehalfby")]
			public const string lk_transactioncurrency_createdonbehalfby = "lk_transactioncurrency_createdonbehalfby";
			[Relationship("transactioncurrency", EntityRole.Referenced, "lk_transactioncurrency_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_transactioncurrency_modifiedonbehalfby = "lk_transactioncurrency_modifiedonbehalfby";
			[Relationship("transactioncurrency", EntityRole.Referenced, "lk_transactioncurrencybase_createdby", "createdby")]
			public const string lk_transactioncurrencybase_createdby = "lk_transactioncurrencybase_createdby";
			[Relationship("transactioncurrency", EntityRole.Referenced, "lk_transactioncurrencybase_modifiedby", "modifiedby")]
			public const string lk_transactioncurrencybase_modifiedby = "lk_transactioncurrencybase_modifiedby";
			[Relationship("transformationmapping", EntityRole.Referenced, "lk_transformationmapping_createdby", "createdby")]
			public const string lk_transformationmapping_createdby = "lk_transformationmapping_createdby";
			[Relationship("transformationmapping", EntityRole.Referenced, "lk_transformationmapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_transformationmapping_createdonbehalfby = "lk_transformationmapping_createdonbehalfby";
			[Relationship("transformationmapping", EntityRole.Referenced, "lk_transformationmapping_modifiedby", "modifiedby")]
			public const string lk_transformationmapping_modifiedby = "lk_transformationmapping_modifiedby";
			[Relationship("transformationmapping", EntityRole.Referenced, "lk_transformationmapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_transformationmapping_modifiedonbehalfby = "lk_transformationmapping_modifiedonbehalfby";
			[Relationship("transformationparametermapping", EntityRole.Referenced, "lk_transformationparametermapping_createdby", "createdby")]
			public const string lk_transformationparametermapping_createdby = "lk_transformationparametermapping_createdby";
			[Relationship("transformationparametermapping", EntityRole.Referenced, "lk_transformationparametermapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_transformationparametermapping_createdonbehalfby = "lk_transformationparametermapping_createdonbehalfby";
			[Relationship("transformationparametermapping", EntityRole.Referenced, "lk_transformationparametermapping_modifiedby", "modifiedby")]
			public const string lk_transformationparametermapping_modifiedby = "lk_transformationparametermapping_modifiedby";
			[Relationship("transformationparametermapping", EntityRole.Referenced, "lk_transformationparametermapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_transformationparametermapping_modifiedonbehalfby = "lk_transformationparametermapping_modifiedonbehalfby";
			[Relationship("translationprocess", EntityRole.Referenced, "lk_translationprocess_createdby", "createdby")]
			public const string lk_translationprocess_createdby = "lk_translationprocess_createdby";
			[Relationship("translationprocess", EntityRole.Referenced, "lk_translationprocess_createdonbehalfby", "createdonbehalfby")]
			public const string lk_translationprocess_createdonbehalfby = "lk_translationprocess_createdonbehalfby";
			[Relationship("translationprocess", EntityRole.Referenced, "lk_translationprocess_modifiedby", "modifiedby")]
			public const string lk_translationprocess_modifiedby = "lk_translationprocess_modifiedby";
			[Relationship("translationprocess", EntityRole.Referenced, "lk_translationprocess_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_translationprocess_modifiedonbehalfby = "lk_translationprocess_modifiedonbehalfby";
			[Relationship("untrackedemail", EntityRole.Referenced, "lk_untrackedemail_createdby", "createdby")]
			public const string lk_untrackedemail_createdby = "lk_untrackedemail_createdby";
			[Relationship("untrackedemail", EntityRole.Referenced, "lk_untrackedemail_createdonbehalfby", "createdonbehalfby")]
			public const string lk_untrackedemail_createdonbehalfby = "lk_untrackedemail_createdonbehalfby";
			[Relationship("untrackedemail", EntityRole.Referenced, "lk_untrackedemail_modifiedby", "modifiedby")]
			public const string lk_untrackedemail_modifiedby = "lk_untrackedemail_modifiedby";
			[Relationship("untrackedemail", EntityRole.Referenced, "lk_untrackedemail_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_untrackedemail_modifiedonbehalfby = "lk_untrackedemail_modifiedonbehalfby";
			[Relationship("uom", EntityRole.Referenced, "lk_uom_createdonbehalfby", "createdonbehalfby")]
			public const string lk_uom_createdonbehalfby = "lk_uom_createdonbehalfby";
			[Relationship("uom", EntityRole.Referenced, "lk_uom_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_uom_modifiedonbehalfby = "lk_uom_modifiedonbehalfby";
			[Relationship("uom", EntityRole.Referenced, "lk_uombase_createdby", "createdby")]
			public const string lk_uombase_createdby = "lk_uombase_createdby";
			[Relationship("uom", EntityRole.Referenced, "lk_uombase_modifiedby", "modifiedby")]
			public const string lk_uombase_modifiedby = "lk_uombase_modifiedby";
			[Relationship("uomschedule", EntityRole.Referenced, "lk_uomschedule_createdonbehalfby", "createdonbehalfby")]
			public const string lk_uomschedule_createdonbehalfby = "lk_uomschedule_createdonbehalfby";
			[Relationship("uomschedule", EntityRole.Referenced, "lk_uomschedule_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_uomschedule_modifiedonbehalfby = "lk_uomschedule_modifiedonbehalfby";
			[Relationship("uomschedule", EntityRole.Referenced, "lk_uomschedulebase_createdby", "createdby")]
			public const string lk_uomschedulebase_createdby = "lk_uomschedulebase_createdby";
			[Relationship("uomschedule", EntityRole.Referenced, "lk_uomschedulebase_modifiedby", "modifiedby")]
			public const string lk_uomschedulebase_modifiedby = "lk_uomschedulebase_modifiedby";
			[Relationship("userapplicationmetadata", EntityRole.Referenced, "lk_userapplicationmetadata_createdby", "createdby")]
			public const string lk_userapplicationmetadata_createdby = "lk_userapplicationmetadata_createdby";
			[Relationship("userapplicationmetadata", EntityRole.Referenced, "lk_userapplicationmetadata_createdonbehalfby", "createdonbehalfby")]
			public const string lk_userapplicationmetadata_createdonbehalfby = "lk_userapplicationmetadata_createdonbehalfby";
			[Relationship("userapplicationmetadata", EntityRole.Referenced, "lk_userapplicationmetadata_modifiedby", "modifiedby")]
			public const string lk_userapplicationmetadata_modifiedby = "lk_userapplicationmetadata_modifiedby";
			[Relationship("userapplicationmetadata", EntityRole.Referenced, "lk_userapplicationmetadata_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_userapplicationmetadata_modifiedonbehalfby = "lk_userapplicationmetadata_modifiedonbehalfby";
			[Relationship("userfiscalcalendar", EntityRole.Referenced, "lk_userfiscalcalendar_createdby", "createdby")]
			public const string lk_userfiscalcalendar_createdby = "lk_userfiscalcalendar_createdby";
			[Relationship("userfiscalcalendar", EntityRole.Referenced, "lk_userfiscalcalendar_createdonbehalfby", "createdonbehalfby")]
			public const string lk_userfiscalcalendar_createdonbehalfby = "lk_userfiscalcalendar_createdonbehalfby";
			[Relationship("userfiscalcalendar", EntityRole.Referenced, "lk_userfiscalcalendar_modifiedby", "modifiedby")]
			public const string lk_userfiscalcalendar_modifiedby = "lk_userfiscalcalendar_modifiedby";
			[Relationship("userfiscalcalendar", EntityRole.Referenced, "lk_userfiscalcalendar_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_userfiscalcalendar_modifiedonbehalfby = "lk_userfiscalcalendar_modifiedonbehalfby";
			[Relationship("userform", EntityRole.Referenced, "lk_userform_createdby", "createdby")]
			public const string lk_userform_createdby = "lk_userform_createdby";
			[Relationship("userform", EntityRole.Referenced, "lk_userform_modifiedby", "modifiedby")]
			public const string lk_userform_modifiedby = "lk_userform_modifiedby";
			[Relationship("userform", EntityRole.Referenced, "lk_userformbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_userformbase_createdonbehalfby = "lk_userformbase_createdonbehalfby";
			[Relationship("userform", EntityRole.Referenced, "lk_userformbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_userformbase_modifiedonbehalfby = "lk_userformbase_modifiedonbehalfby";
			[Relationship("usermapping", EntityRole.Referenced, "lk_usermapping_createdby", "createdby")]
			public const string lk_usermapping_createdby = "lk_usermapping_createdby";
			[Relationship("usermapping", EntityRole.Referenced, "lk_usermapping_createdonbehalfby", "createdonbehalfby")]
			public const string lk_usermapping_createdonbehalfby = "lk_usermapping_createdonbehalfby";
			[Relationship("usermapping", EntityRole.Referenced, "lk_usermapping_modifiedby", "modifiedby")]
			public const string lk_usermapping_modifiedby = "lk_usermapping_modifiedby";
			[Relationship("usermapping", EntityRole.Referenced, "lk_usermapping_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_usermapping_modifiedonbehalfby = "lk_usermapping_modifiedonbehalfby";
			[Relationship("userquery", EntityRole.Referenced, "lk_userquery_createdby", "createdby")]
			public const string lk_userquery_createdby = "lk_userquery_createdby";
			[Relationship("userquery", EntityRole.Referenced, "lk_userquery_createdonbehalfby", "createdonbehalfby")]
			public const string lk_userquery_createdonbehalfby = "lk_userquery_createdonbehalfby";
			[Relationship("userquery", EntityRole.Referenced, "lk_userquery_modifiedby", "modifiedby")]
			public const string lk_userquery_modifiedby = "lk_userquery_modifiedby";
			[Relationship("userquery", EntityRole.Referenced, "lk_userquery_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_userquery_modifiedonbehalfby = "lk_userquery_modifiedonbehalfby";
			[Relationship("userqueryvisualization", EntityRole.Referenced, "lk_userqueryvisualization_createdby", "createdby")]
			public const string lk_userqueryvisualization_createdby = "lk_userqueryvisualization_createdby";
			[Relationship("userqueryvisualization", EntityRole.Referenced, "lk_userqueryvisualization_modifiedby", "modifiedby")]
			public const string lk_userqueryvisualization_modifiedby = "lk_userqueryvisualization_modifiedby";
			[Relationship("userqueryvisualization", EntityRole.Referenced, "lk_userqueryvisualizationbase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_userqueryvisualizationbase_createdonbehalfby = "lk_userqueryvisualizationbase_createdonbehalfby";
			[Relationship("userqueryvisualization", EntityRole.Referenced, "lk_userqueryvisualizationbase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_userqueryvisualizationbase_modifiedonbehalfby = "lk_userqueryvisualizationbase_modifiedonbehalfby";
			[Relationship("usersettings", EntityRole.Referenced, "lk_usersettings_createdonbehalfby", "createdonbehalfby")]
			public const string lk_usersettings_createdonbehalfby = "lk_usersettings_createdonbehalfby";
			[Relationship("usersettings", EntityRole.Referenced, "lk_usersettings_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_usersettings_modifiedonbehalfby = "lk_usersettings_modifiedonbehalfby";
			[Relationship("usersettings", EntityRole.Referenced, "lk_usersettingsbase_createdby", "createdby")]
			public const string lk_usersettingsbase_createdby = "lk_usersettingsbase_createdby";
			[Relationship("usersettings", EntityRole.Referenced, "lk_usersettingsbase_modifiedby", "modifiedby")]
			public const string lk_usersettingsbase_modifiedby = "lk_usersettingsbase_modifiedby";
			[Relationship("webresource", EntityRole.Referenced, "lk_webresourcebase_createdonbehalfby", "createdonbehalfby")]
			public const string lk_webresourcebase_createdonbehalfby = "lk_webresourcebase_createdonbehalfby";
			[Relationship("webresource", EntityRole.Referenced, "lk_webresourcebase_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_webresourcebase_modifiedonbehalfby = "lk_webresourcebase_modifiedonbehalfby";
			[Relationship("webwizard", EntityRole.Referenced, "lk_webwizard_createdby", "createdby")]
			public const string lk_webwizard_createdby = "lk_webwizard_createdby";
			[Relationship("webwizard", EntityRole.Referenced, "lk_webwizard_createdonbehalfby", "createdonbehalfby")]
			public const string lk_webwizard_createdonbehalfby = "lk_webwizard_createdonbehalfby";
			[Relationship("webwizard", EntityRole.Referenced, "lk_webwizard_modifiedby", "modifiedby")]
			public const string lk_webwizard_modifiedby = "lk_webwizard_modifiedby";
			[Relationship("webwizard", EntityRole.Referenced, "lk_webwizard_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_webwizard_modifiedonbehalfby = "lk_webwizard_modifiedonbehalfby";
			[Relationship("wizardaccessprivilege", EntityRole.Referenced, "lk_wizardaccessprivilege_createdby", "createdby")]
			public const string lk_wizardaccessprivilege_createdby = "lk_wizardaccessprivilege_createdby";
			[Relationship("wizardaccessprivilege", EntityRole.Referenced, "lk_wizardaccessprivilege_createdonbehalfby", "createdonbehalfby")]
			public const string lk_wizardaccessprivilege_createdonbehalfby = "lk_wizardaccessprivilege_createdonbehalfby";
			[Relationship("wizardaccessprivilege", EntityRole.Referenced, "lk_wizardaccessprivilege_modifiedby", "modifiedby")]
			public const string lk_wizardaccessprivilege_modifiedby = "lk_wizardaccessprivilege_modifiedby";
			[Relationship("wizardaccessprivilege", EntityRole.Referenced, "lk_wizardaccessprivilege_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_wizardaccessprivilege_modifiedonbehalfby = "lk_wizardaccessprivilege_modifiedonbehalfby";
			[Relationship("wizardpage", EntityRole.Referenced, "lk_wizardpage_createdby", "createdby")]
			public const string lk_wizardpage_createdby = "lk_wizardpage_createdby";
			[Relationship("wizardpage", EntityRole.Referenced, "lk_wizardpage_createdonbehalfby", "createdonbehalfby")]
			public const string lk_wizardpage_createdonbehalfby = "lk_wizardpage_createdonbehalfby";
			[Relationship("wizardpage", EntityRole.Referenced, "lk_wizardpage_modifiedby", "modifiedby")]
			public const string lk_wizardpage_modifiedby = "lk_wizardpage_modifiedby";
			[Relationship("wizardpage", EntityRole.Referenced, "lk_wizardpage_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_wizardpage_modifiedonbehalfby = "lk_wizardpage_modifiedonbehalfby";
			[Relationship("workflowlog", EntityRole.Referenced, "lk_workflowlog_createdby", "createdby")]
			public const string lk_workflowlog_createdby = "lk_workflowlog_createdby";
			[Relationship("workflowlog", EntityRole.Referenced, "lk_workflowlog_createdonbehalfby", "createdonbehalfby")]
			public const string lk_workflowlog_createdonbehalfby = "lk_workflowlog_createdonbehalfby";
			[Relationship("workflowlog", EntityRole.Referenced, "lk_workflowlog_modifiedby", "modifiedby")]
			public const string lk_workflowlog_modifiedby = "lk_workflowlog_modifiedby";
			[Relationship("workflowlog", EntityRole.Referenced, "lk_workflowlog_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_workflowlog_modifiedonbehalfby = "lk_workflowlog_modifiedonbehalfby";
			[Relationship("mailbox", EntityRole.Referenced, "mailbox_regarding_systemuser", "regardingobjectid")]
			public const string mailbox_regarding_systemuser = "mailbox_regarding_systemuser";
			[Relationship("mbs_pluginprofile", EntityRole.Referenced, "mbs_systemuser_mbs_pluginprofile", "")]
			public const string mbs_systemuser_mbs_pluginprofile = "mbs_systemuser_mbs_pluginprofile";
			[Relationship("attributemap", EntityRole.Referenced, "modifiedby_attributemap", "modifiedby")]
			public const string modifiedby_attributemap = "modifiedby_attributemap";
			[Relationship("connection", EntityRole.Referenced, "modifiedby_connection", "modifiedby")]
			public const string modifiedby_connection = "modifiedby_connection";
			[Relationship("connectionrole", EntityRole.Referenced, "modifiedby_connection_role", "modifiedby")]
			public const string modifiedby_connection_role = "modifiedby_connection_role";
			[Relationship("customerrelationship", EntityRole.Referenced, "modifiedby_customer_relationship", "modifiedby")]
			public const string modifiedby_customer_relationship = "modifiedby_customer_relationship";
			[Relationship("entitymap", EntityRole.Referenced, "modifiedby_entitymap", "modifiedby")]
			public const string modifiedby_entitymap = "modifiedby_entitymap";
			[Relationship("expanderevent", EntityRole.Referenced, "modifiedby_expanderevent", "modifiedby")]
			public const string modifiedby_expanderevent = "modifiedby_expanderevent";
			[Relationship("pluginassembly", EntityRole.Referenced, "modifiedby_pluginassembly", "modifiedby")]
			public const string modifiedby_pluginassembly = "modifiedby_pluginassembly";
			[Relationship("plugintype", EntityRole.Referenced, "modifiedby_plugintype", "modifiedby")]
			public const string modifiedby_plugintype = "modifiedby_plugintype";
			[Relationship("plugintypestatistic", EntityRole.Referenced, "modifiedby_plugintypestatistic", "modifiedby")]
			public const string modifiedby_plugintypestatistic = "modifiedby_plugintypestatistic";
			[Relationship("relationshiprole", EntityRole.Referenced, "modifiedby_relationship_role", "modifiedby")]
			public const string modifiedby_relationship_role = "modifiedby_relationship_role";
			[Relationship("relationshiprolemap", EntityRole.Referenced, "modifiedby_relationship_role_map", "modifiedby")]
			public const string modifiedby_relationship_role_map = "modifiedby_relationship_role_map";
			[Relationship("sdkmessage", EntityRole.Referenced, "modifiedby_sdkmessage", "modifiedby")]
			public const string modifiedby_sdkmessage = "modifiedby_sdkmessage";
			[Relationship("sdkmessagefilter", EntityRole.Referenced, "modifiedby_sdkmessagefilter", "modifiedby")]
			public const string modifiedby_sdkmessagefilter = "modifiedby_sdkmessagefilter";
			[Relationship("sdkmessagepair", EntityRole.Referenced, "modifiedby_sdkmessagepair", "modifiedby")]
			public const string modifiedby_sdkmessagepair = "modifiedby_sdkmessagepair";
			[Relationship("sdkmessageprocessingstep", EntityRole.Referenced, "modifiedby_sdkmessageprocessingstep", "modifiedby")]
			public const string modifiedby_sdkmessageprocessingstep = "modifiedby_sdkmessageprocessingstep";
			[Relationship("sdkmessageprocessingstepimage", EntityRole.Referenced, "modifiedby_sdkmessageprocessingstepimage", "modifiedby")]
			public const string modifiedby_sdkmessageprocessingstepimage = "modifiedby_sdkmessageprocessingstepimage";
			[Relationship("sdkmessageprocessingstepsecureconfig", EntityRole.Referenced, "modifiedby_sdkmessageprocessingstepsecureconfig", "modifiedby")]
			public const string modifiedby_sdkmessageprocessingstepsecureconfig = "modifiedby_sdkmessageprocessingstepsecureconfig";
			[Relationship("sdkmessagerequest", EntityRole.Referenced, "modifiedby_sdkmessagerequest", "modifiedby")]
			public const string modifiedby_sdkmessagerequest = "modifiedby_sdkmessagerequest";
			[Relationship("sdkmessagerequestfield", EntityRole.Referenced, "modifiedby_sdkmessagerequestfield", "modifiedby")]
			public const string modifiedby_sdkmessagerequestfield = "modifiedby_sdkmessagerequestfield";
			[Relationship("sdkmessageresponse", EntityRole.Referenced, "modifiedby_sdkmessageresponse", "modifiedby")]
			public const string modifiedby_sdkmessageresponse = "modifiedby_sdkmessageresponse";
			[Relationship("sdkmessageresponsefield", EntityRole.Referenced, "modifiedby_sdkmessageresponsefield", "modifiedby")]
			public const string modifiedby_sdkmessageresponsefield = "modifiedby_sdkmessageresponsefield";
			[Relationship("serviceendpoint", EntityRole.Referenced, "modifiedby_serviceendpoint", "modifiedby")]
			public const string modifiedby_serviceendpoint = "modifiedby_serviceendpoint";
			[Relationship("attributemap", EntityRole.Referenced, "modifiedonbehalfby_attributemap", "modifiedonbehalfby")]
			public const string modifiedonbehalfby_attributemap = "modifiedonbehalfby_attributemap";
			[Relationship("customerrelationship", EntityRole.Referenced, "modifiedonbehalfby_customer_relationship", "modifiedonbehalfby")]
			public const string modifiedonbehalfby_customer_relationship = "modifiedonbehalfby_customer_relationship";
			[Relationship("opportunity", EntityRole.Referenced, "msdyn_accountmanager_opportunity", "msdyn_accountmanagerid")]
			public const string msdyn_accountmanager_opportunity = "msdyn_accountmanager_opportunity";
			[Relationship("quote", EntityRole.Referenced, "msdyn_accountmanager_quote", "msdyn_accountmanagerid")]
			public const string msdyn_accountmanager_quote = "msdyn_accountmanager_quote";
			[Relationship("salesorder", EntityRole.Referenced, "msdyn_accountmanager_salesorder", "msdyn_accountmanagerid")]
			public const string msdyn_accountmanager_salesorder = "msdyn_accountmanager_salesorder";
			[Relationship("msdyn_approval", EntityRole.Referenced, "msdyn_approval_systemuser_createdby", "createdby")]
			public const string msdyn_approval_systemuser_createdby = "msdyn_approval_systemuser_createdby";
			[Relationship("msdyn_approval", EntityRole.Referenced, "msdyn_approval_systemuser_createdonbehalfby", "createdonbehalfby")]
			public const string msdyn_approval_systemuser_createdonbehalfby = "msdyn_approval_systemuser_createdonbehalfby";
			[Relationship("msdyn_approval", EntityRole.Referenced, "msdyn_approval_systemuser_modifiedby", "modifiedby")]
			public const string msdyn_approval_systemuser_modifiedby = "msdyn_approval_systemuser_modifiedby";
			[Relationship("msdyn_approval", EntityRole.Referenced, "msdyn_approval_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string msdyn_approval_systemuser_modifiedonbehalfby = "msdyn_approval_systemuser_modifiedonbehalfby";
			[Relationship("msdyn_approval", EntityRole.Referenced, "msdyn_approval_systemuser_owninguser", "owninguser")]
			public const string msdyn_approval_systemuser_owninguser = "msdyn_approval_systemuser_owninguser";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "msdyn_bookingalert_systemuser_createdby", "createdby")]
			public const string msdyn_bookingalert_systemuser_createdby = "msdyn_bookingalert_systemuser_createdby";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "msdyn_bookingalert_systemuser_createdonbehalfby", "createdonbehalfby")]
			public const string msdyn_bookingalert_systemuser_createdonbehalfby = "msdyn_bookingalert_systemuser_createdonbehalfby";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "msdyn_bookingalert_systemuser_modifiedby", "modifiedby")]
			public const string msdyn_bookingalert_systemuser_modifiedby = "msdyn_bookingalert_systemuser_modifiedby";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "msdyn_bookingalert_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string msdyn_bookingalert_systemuser_modifiedonbehalfby = "msdyn_bookingalert_systemuser_modifiedonbehalfby";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "msdyn_bookingalert_systemuser_owninguser", "owninguser")]
			public const string msdyn_bookingalert_systemuser_owninguser = "msdyn_bookingalert_systemuser_owninguser";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "msdyn_surveyinvite_systemuser_createdby", "")]
			public const string msdyn_surveyinvite_systemuser_createdby = "msdyn_surveyinvite_systemuser_createdby";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "msdyn_surveyinvite_systemuser_createdonbehalfby", "")]
			public const string msdyn_surveyinvite_systemuser_createdonbehalfby = "msdyn_surveyinvite_systemuser_createdonbehalfby";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "msdyn_surveyinvite_systemuser_modifiedby", "")]
			public const string msdyn_surveyinvite_systemuser_modifiedby = "msdyn_surveyinvite_systemuser_modifiedby";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "msdyn_surveyinvite_systemuser_modifiedonbehalfby", "")]
			public const string msdyn_surveyinvite_systemuser_modifiedonbehalfby = "msdyn_surveyinvite_systemuser_modifiedonbehalfby";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "msdyn_surveyinvite_systemuser_owninguser", "")]
			public const string msdyn_surveyinvite_systemuser_owninguser = "msdyn_surveyinvite_systemuser_owninguser";
			[Relationship("msdyn_expense", EntityRole.Referenced, "msdyn_systemuser_msdyn_expense_manager", "msdyn_manager")]
			public const string msdyn_systemuser_msdyn_expense_manager = "msdyn_systemuser_msdyn_expense_manager";
			[Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "msdyn_systemuser_msdyn_fieldservicesystemjob_OwnerId", "msdyn_ownerid")]
			public const string msdyn_systemuser_msdyn_fieldservicesystemjob_OwnerId = "msdyn_systemuser_msdyn_fieldservicesystemjob_OwnerId";
			[Relationship("msdyn_project", EntityRole.Referenced, "msdyn_systemuser_msdyn_project_projectmanager", "msdyn_projectmanager")]
			public const string msdyn_systemuser_msdyn_project_projectmanager = "msdyn_systemuser_msdyn_project_projectmanager";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "msdyn_systemuser_msdyn_projectapproval_ApprovedBy", "msdyn_approvedby")]
			public const string msdyn_systemuser_msdyn_projectapproval_ApprovedBy = "msdyn_systemuser_msdyn_projectapproval_ApprovedBy";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "msdyn_systemuser_msdyn_projectapproval_Manager", "msdyn_manager")]
			public const string msdyn_systemuser_msdyn_projectapproval_Manager = "msdyn_systemuser_msdyn_projectapproval_Manager";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "msdyn_systemuser_msdyn_purchaseorder_ApprovedRejectedBy", "msdyn_approvedrejectedby")]
			public const string msdyn_systemuser_msdyn_purchaseorder_ApprovedRejectedBy = "msdyn_systemuser_msdyn_purchaseorder_ApprovedRejectedBy";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "msdyn_systemuser_msdyn_purchaseorder_OrderedBy", "msdyn_orderedby")]
			public const string msdyn_systemuser_msdyn_purchaseorder_OrderedBy = "msdyn_systemuser_msdyn_purchaseorder_OrderedBy";
			[Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "msdyn_systemuser_msdyn_purchaseorderreceipt_ReceivedBy", "msdyn_receivedby")]
			public const string msdyn_systemuser_msdyn_purchaseorderreceipt_ReceivedBy = "msdyn_systemuser_msdyn_purchaseorderreceipt_ReceivedBy";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "msdyn_systemuser_msdyn_resourcerequest_claimedby", "msdyn_claimedby")]
			public const string msdyn_systemuser_msdyn_resourcerequest_claimedby = "msdyn_systemuser_msdyn_resourcerequest_claimedby";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "msdyn_systemuser_msdyn_resourcerequest_requestedby", "msdyn_requestedby")]
			public const string msdyn_systemuser_msdyn_resourcerequest_requestedby = "msdyn_systemuser_msdyn_resourcerequest_requestedby";
			[Relationship("msdyn_responseaction", EntityRole.Referenced, "msdyn_systemuser_msdyn_responseaction_SenderUser", "")]
			public const string msdyn_systemuser_msdyn_responseaction_SenderUser = "msdyn_systemuser_msdyn_responseaction_SenderUser";
			[Relationship("msdyn_responseaction", EntityRole.Referenced, "msdyn_systemuser_msdyn_responseaction_Usertonotify", "")]
			public const string msdyn_systemuser_msdyn_responseaction_Usertonotify = "msdyn_systemuser_msdyn_responseaction_Usertonotify";
			[Relationship("msdyn_rma", EntityRole.Referenced, "msdyn_systemuser_msdyn_rma_ApprovedBy", "msdyn_approvedby")]
			public const string msdyn_systemuser_msdyn_rma_ApprovedBy = "msdyn_systemuser_msdyn_rma_ApprovedBy";
			[Relationship("msdyn_rmareceipt", EntityRole.Referenced, "msdyn_systemuser_msdyn_rmareceipt_ReceivedBy", "msdyn_receivedby")]
			public const string msdyn_systemuser_msdyn_rmareceipt_ReceivedBy = "msdyn_systemuser_msdyn_rmareceipt_ReceivedBy";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "msdyn_systemuser_msdyn_rtv_ApprovedDeclinedBy", "msdyn_approveddeclinedby")]
			public const string msdyn_systemuser_msdyn_rtv_ApprovedDeclinedBy = "msdyn_systemuser_msdyn_rtv_ApprovedDeclinedBy";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "msdyn_systemuser_msdyn_rtv_ReturnedBy", "msdyn_returnedby")]
			public const string msdyn_systemuser_msdyn_rtv_ReturnedBy = "msdyn_systemuser_msdyn_rtv_ReturnedBy";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "msdyn_systemuser_msdyn_surveyinvite_User", "")]
			public const string msdyn_systemuser_msdyn_surveyinvite_User = "msdyn_systemuser_msdyn_surveyinvite_User";
			[Relationship("msdyn_surveyresponse", EntityRole.Referenced, "msdyn_systemuser_msdyn_surveyresponse_User", "")]
			public const string msdyn_systemuser_msdyn_surveyresponse_User = "msdyn_systemuser_msdyn_surveyresponse_User";
			[Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "msdyn_systemuser_msdyn_systemuserschedulersetting_User", "msdyn_user")]
			public const string msdyn_systemuser_msdyn_systemuserschedulersetting_User = "msdyn_systemuser_msdyn_systemuserschedulersetting_User";
			[Relationship("msdyn_timeentry", EntityRole.Referenced, "msdyn_systemuser_msdyn_timeentry_manager", "msdyn_manager")]
			public const string msdyn_systemuser_msdyn_timeentry_manager = "msdyn_systemuser_msdyn_timeentry_manager";
			[Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "msdyn_systemuser_msdyn_timeoffrequest_Approvedby", "msdyn_approvedby")]
			public const string msdyn_systemuser_msdyn_timeoffrequest_Approvedby = "msdyn_systemuser_msdyn_timeoffrequest_Approvedby";
			[Relationship("msdyn_vocconfiguration", EntityRole.Referenced, "msdyn_systemuser_msdyn_vocconfiguration_SurveyAdmin", "")]
			public const string msdyn_systemuser_msdyn_vocconfiguration_SurveyAdmin = "msdyn_systemuser_msdyn_vocconfiguration_SurveyAdmin";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "msdyn_systemuser_msdyn_workorder_ClosedBy", "msdyn_closedby")]
			public const string msdyn_systemuser_msdyn_workorder_ClosedBy = "msdyn_systemuser_msdyn_workorder_ClosedBy";
			[Relationship("msdyn_survey", EntityRole.Referenced, "msdyn_systemuser_Survey", "")]
			public const string msdyn_systemuser_Survey = "msdyn_systemuser_Survey";
			[Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "msdyn_systemuser_wallsavedqueryusersettings_userid", "msdyn_userid")]
			public const string msdyn_systemuser_wallsavedqueryusersettings_userid = "msdyn_systemuser_wallsavedqueryusersettings_userid";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "msfp_surveyinvite_systemuser_createdby", "createdby")]
			public const string msfp_surveyinvite_systemuser_createdby = "msfp_surveyinvite_systemuser_createdby";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "msfp_surveyinvite_systemuser_createdonbehalfby", "createdonbehalfby")]
			public const string msfp_surveyinvite_systemuser_createdonbehalfby = "msfp_surveyinvite_systemuser_createdonbehalfby";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "msfp_surveyinvite_systemuser_modifiedby", "modifiedby")]
			public const string msfp_surveyinvite_systemuser_modifiedby = "msfp_surveyinvite_systemuser_modifiedby";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "msfp_surveyinvite_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string msfp_surveyinvite_systemuser_modifiedonbehalfby = "msfp_surveyinvite_systemuser_modifiedonbehalfby";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "msfp_surveyinvite_systemuser_owninguser", "owninguser")]
			public const string msfp_surveyinvite_systemuser_owninguser = "msfp_surveyinvite_systemuser_owninguser";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "msfp_surveyresponse_systemuser_createdby", "createdby")]
			public const string msfp_surveyresponse_systemuser_createdby = "msfp_surveyresponse_systemuser_createdby";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "msfp_surveyresponse_systemuser_createdonbehalfby", "createdonbehalfby")]
			public const string msfp_surveyresponse_systemuser_createdonbehalfby = "msfp_surveyresponse_systemuser_createdonbehalfby";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "msfp_surveyresponse_systemuser_modifiedby", "modifiedby")]
			public const string msfp_surveyresponse_systemuser_modifiedby = "msfp_surveyresponse_systemuser_modifiedby";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "msfp_surveyresponse_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string msfp_surveyresponse_systemuser_modifiedonbehalfby = "msfp_surveyresponse_systemuser_modifiedonbehalfby";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "msfp_surveyresponse_systemuser_owninguser", "owninguser")]
			public const string msfp_surveyresponse_systemuser_owninguser = "msfp_surveyresponse_systemuser_owninguser";
			[Relationship("multientitysearch", EntityRole.Referenced, "multientitysearch_createdby", "createdby")]
			public const string multientitysearch_createdby = "multientitysearch_createdby";
			[Relationship("multientitysearch", EntityRole.Referenced, "multientitysearch_createdonbehalfby", "createdonbehalfby")]
			public const string multientitysearch_createdonbehalfby = "multientitysearch_createdonbehalfby";
			[Relationship("multientitysearch", EntityRole.Referenced, "multientitysearch_modifiedby", "modifiedby")]
			public const string multientitysearch_modifiedby = "multientitysearch_modifiedby";
			[Relationship("multientitysearch", EntityRole.Referenced, "multientitysearch_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string multientitysearch_modifiedonbehalfby = "multientitysearch_modifiedonbehalfby";
			[Relationship("opportunity", EntityRole.Referenced, "opportunity_owning_user", "owninguser")]
			public const string opportunity_owning_user = "opportunity_owning_user";
			[Relationship("ownermapping", EntityRole.Referenced, "OwnerMapping_SystemUser", "targetsystemuserid")]
			public const string OwnerMapping_SystemUser = "OwnerMapping_SystemUser";
			[Relationship("dynamicpropertyinstance", EntityRole.Referenced, "OwningUser_Dynamicpropertyinsatance", "dynamicpropertyinstanceid")]
			public const string OwningUser_Dynamicpropertyinsatance = "OwningUser_Dynamicpropertyinsatance";
			[Relationship("pchmcs_email", EntityRole.Referenced, "pchmcs_email_systemuser_createdby", "")]
			public const string pchmcs_email_systemuser_createdby = "pchmcs_email_systemuser_createdby";
			[Relationship("pchmcs_email", EntityRole.Referenced, "pchmcs_email_systemuser_createdonbehalfby", "")]
			public const string pchmcs_email_systemuser_createdonbehalfby = "pchmcs_email_systemuser_createdonbehalfby";
			[Relationship("pchmcs_email", EntityRole.Referenced, "pchmcs_email_systemuser_modifiedby", "")]
			public const string pchmcs_email_systemuser_modifiedby = "pchmcs_email_systemuser_modifiedby";
			[Relationship("pchmcs_email", EntityRole.Referenced, "pchmcs_email_systemuser_modifiedonbehalfby", "")]
			public const string pchmcs_email_systemuser_modifiedonbehalfby = "pchmcs_email_systemuser_modifiedonbehalfby";
			[Relationship("pchmcs_email", EntityRole.Referenced, "pchmcs_email_systemuser_owninguser", "")]
			public const string pchmcs_email_systemuser_owninguser = "pchmcs_email_systemuser_owninguser";
			[Relationship("pchmcs_message", EntityRole.Referenced, "pchmcs_message_systemuser_createdby", "")]
			public const string pchmcs_message_systemuser_createdby = "pchmcs_message_systemuser_createdby";
			[Relationship("pchmcs_message", EntityRole.Referenced, "pchmcs_message_systemuser_createdonbehalfby", "")]
			public const string pchmcs_message_systemuser_createdonbehalfby = "pchmcs_message_systemuser_createdonbehalfby";
			[Relationship("pchmcs_message", EntityRole.Referenced, "pchmcs_message_systemuser_modifiedby", "")]
			public const string pchmcs_message_systemuser_modifiedby = "pchmcs_message_systemuser_modifiedby";
			[Relationship("pchmcs_message", EntityRole.Referenced, "pchmcs_message_systemuser_modifiedonbehalfby", "")]
			public const string pchmcs_message_systemuser_modifiedonbehalfby = "pchmcs_message_systemuser_modifiedonbehalfby";
			[Relationship("pchmcs_message", EntityRole.Referenced, "pchmcs_message_systemuser_owninguser", "")]
			public const string pchmcs_message_systemuser_owninguser = "pchmcs_message_systemuser_owninguser";
			[Relationship("pchmcs_smsactivity", EntityRole.Referenced, "pchmcs_smsactivity_systemuser_createdby", "")]
			public const string pchmcs_smsactivity_systemuser_createdby = "pchmcs_smsactivity_systemuser_createdby";
			[Relationship("pchmcs_smsactivity", EntityRole.Referenced, "pchmcs_smsactivity_systemuser_createdonbehalfby", "")]
			public const string pchmcs_smsactivity_systemuser_createdonbehalfby = "pchmcs_smsactivity_systemuser_createdonbehalfby";
			[Relationship("pchmcs_smsactivity", EntityRole.Referenced, "pchmcs_smsactivity_systemuser_modifiedby", "")]
			public const string pchmcs_smsactivity_systemuser_modifiedby = "pchmcs_smsactivity_systemuser_modifiedby";
			[Relationship("pchmcs_smsactivity", EntityRole.Referenced, "pchmcs_smsactivity_systemuser_modifiedonbehalfby", "")]
			public const string pchmcs_smsactivity_systemuser_modifiedonbehalfby = "pchmcs_smsactivity_systemuser_modifiedonbehalfby";
			[Relationship("pchmcs_smsactivity", EntityRole.Referenced, "pchmcs_smsactivity_systemuser_owninguser", "")]
			public const string pchmcs_smsactivity_systemuser_owninguser = "pchmcs_smsactivity_systemuser_owninguser";
			[Relationship("pchmcs_affectation", EntityRole.Referenced, "pchmcs_systemuser_affectation_Utilisateurid", "")]
			public const string pchmcs_systemuser_affectation_Utilisateurid = "pchmcs_systemuser_affectation_Utilisateurid";
			[Relationship("appointment", EntityRole.Referenced, "pchmcs_systemuser_appointment_conflockedsystemuserid", "")]
			public const string pchmcs_systemuser_appointment_conflockedsystemuserid = "pchmcs_systemuser_appointment_conflockedsystemuserid";
			[Relationship("appointment", EntityRole.Referenced, "pchmcs_systemuser_appointment_NegociateurId", "")]
			public const string pchmcs_systemuser_appointment_NegociateurId = "pchmcs_systemuser_appointment_NegociateurId";
			[Relationship("appointment", EntityRole.Referenced, "pchmcs_systemuser_appointment_PreviousNegociatorId", "")]
			public const string pchmcs_systemuser_appointment_PreviousNegociatorId = "pchmcs_systemuser_appointment_PreviousNegociatorId";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "pchmcs_systemuser_contact_PreviousNegociatorID", "")]
			public const string pchmcs_systemuser_contact_PreviousNegociatorID = "pchmcs_systemuser_contact_PreviousNegociatorID";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "pchmcs_systemuser_contact_Targettedfor", "")]
			public const string pchmcs_systemuser_contact_Targettedfor = "pchmcs_systemuser_contact_Targettedfor";
			[Relationship("pchmcs_notifieduser", EntityRole.Referenced, "pchmcs_systemuser_notifieduser", "")]
			public const string pchmcs_systemuser_notifieduser = "pchmcs_systemuser_notifieduser";
			[Relationship("pchmcs_offre", EntityRole.Referenced, "pchmcs_systemuser_offre_OptionneeParid", "")]
			public const string pchmcs_systemuser_offre_OptionneeParid = "pchmcs_systemuser_offre_OptionneeParid";
			[Relationship("pchmcs_offre", EntityRole.Referenced, "pchmcs_systemuser_offre_ReserveeParid", "")]
			public const string pchmcs_systemuser_offre_ReserveeParid = "pchmcs_systemuser_offre_ReserveeParid";
			[Relationship("pchmcs_offresducyclesdevente", EntityRole.Referenced, "pchmcs_systemuser_offresducyclesdevente_bookbyid", "")]
			public const string pchmcs_systemuser_offresducyclesdevente_bookbyid = "pchmcs_systemuser_offresducyclesdevente_bookbyid";
			[Relationship("opportunity", EntityRole.Referenced, "pchmcs_systemuser_opportunity_bookingrequestbyid", "")]
			public const string pchmcs_systemuser_opportunity_bookingrequestbyid = "pchmcs_systemuser_opportunity_bookingrequestbyid";
			[Relationship("opportunity", EntityRole.Referenced, "pchmcs_systemuser_opportunity_optionrequestedbyid", "")]
			public const string pchmcs_systemuser_opportunity_optionrequestedbyid = "pchmcs_systemuser_opportunity_optionrequestedbyid";
			[Relationship("pchmcs_bilanpatrimonial", EntityRole.Referenced, "pchmcs_systemuser_pchmcs_bilanpatrimonial", "")]
			public const string pchmcs_systemuser_pchmcs_bilanpatrimonial = "pchmcs_systemuser_pchmcs_bilanpatrimonial";
			[Relationship("pchmcs_compositions", EntityRole.Referenced, "pchmcs_systemuser_pchmcs_compositions_OptionneouReservePar", "")]
			public const string pchmcs_systemuser_pchmcs_compositions_OptionneouReservePar = "pchmcs_systemuser_pchmcs_compositions_OptionneouReservePar";
			[Relationship("pchmcs_historiquestatutinteret", EntityRole.Referenced, "pchmcs_systemuser_pchmcs_historiquestatutinteret", "")]
			public const string pchmcs_systemuser_pchmcs_historiquestatutinteret = "pchmcs_systemuser_pchmcs_historiquestatutinteret";
			[Relationship("pchmcs_interet", EntityRole.Referenced, "pchmcs_systemuser_pchmcs_interet", "")]
			public const string pchmcs_systemuser_pchmcs_interet = "pchmcs_systemuser_pchmcs_interet";
			[Relationship("pchmcs_interet", EntityRole.Referenced, "pchmcs_systemuser_pchmcs_interet_modifieparid", "")]
			public const string pchmcs_systemuser_pchmcs_interet_modifieparid = "pchmcs_systemuser_pchmcs_interet_modifieparid";
			[Relationship("pchmcs_offresducyclesdevente", EntityRole.Referenced, "pchmcs_systemuser_pchmcs_offresducyclesdevente_optionrequestedbyid", "")]
			public const string pchmcs_systemuser_pchmcs_offresducyclesdevente_optionrequestedbyid = "pchmcs_systemuser_pchmcs_offresducyclesdevente_optionrequestedbyid";
			[Relationship("pchmcs_rdvcdc", EntityRole.Referenced, "pchmcs_systemuser_pchmcs_rdvcdc_connectedUser", "")]
			public const string pchmcs_systemuser_pchmcs_rdvcdc_connectedUser = "pchmcs_systemuser_pchmcs_rdvcdc_connectedUser";
			[Relationship("pchmcs_vendeur", EntityRole.Referenced, "pchmcs_systemuser_pchmcs_vendeur_vendeuruser", "")]
			public const string pchmcs_systemuser_pchmcs_vendeur_vendeuruser = "pchmcs_systemuser_pchmcs_vendeur_vendeuruser";
			[Relationship("incident", EntityRole.Referenced, "pchmcs_systemuserid", "")]
			public const string pchmcs_systemuserid = "pchmcs_systemuserid";
			[Relationship("queue", EntityRole.Referenced, "queue_primary_user", "primaryuserid")]
			public const string queue_primary_user = "queue_primary_user";
			[Relationship("socialprofile", EntityRole.Referenced, "socialProfile_owning_user", "owninguser")]
			public const string socialProfile_owning_user = "socialProfile_owning_user";
			[Relationship("systemuserprincipals", EntityRole.Referenced, "sup_principalid_systemuser", "systemuserid")]
			public const string sup_principalid_systemuser = "sup_principalid_systemuser";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "system_user_accounts", "preferredsystemuserid")]
			public const string system_user_accounts = "system_user_accounts";
			[Relationship("activityparty", EntityRole.Referenced, "system_user_activity_parties", "partyid")]
			public const string system_user_activity_parties = "system_user_activity_parties";
			[Relationship("asyncoperation", EntityRole.Referenced, "system_user_asyncoperation", "owninguser")]
			public const string system_user_asyncoperation = "system_user_asyncoperation";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "system_user_contacts", "preferredsystemuserid")]
			public const string system_user_contacts = "system_user_contacts";
			[Relationship("template", EntityRole.Referenced, "system_user_email_templates", "owninguser")]
			public const string system_user_email_templates = "system_user_email_templates";
			[Relationship("incident", EntityRole.Referenced, "system_user_incidents", "owninguser")]
			public const string system_user_incidents = "system_user_incidents";
			[Relationship("invoicedetail", EntityRole.Referenced, "system_user_invoicedetail", "salesrepid")]
			public const string system_user_invoicedetail = "system_user_invoicedetail";
			[Relationship("invoice", EntityRole.Referenced, "system_user_invoices", "owninguser")]
			public const string system_user_invoices = "system_user_invoices";
			[Relationship("salesorder", EntityRole.Referenced, "system_user_orders", "owninguser")]
			public const string system_user_orders = "system_user_orders";
			[Relationship("userfiscalcalendar", EntityRole.Referenced, "system_user_quotas", "salespersonid")]
			public const string system_user_quotas = "system_user_quotas";
			[Relationship("quotedetail", EntityRole.Referenced, "system_user_quotedetail", "salesrepid")]
			public const string system_user_quotedetail = "system_user_quotedetail";
			[Relationship("quote", EntityRole.Referenced, "system_user_quotes", "owninguser")]
			public const string system_user_quotes = "system_user_quotes";
			[Relationship("salesliterature", EntityRole.Referenced, "system_user_sales_literature", "employeecontactid")]
			public const string system_user_sales_literature = "system_user_sales_literature";
			[Relationship("salesorderdetail", EntityRole.Referenced, "system_user_salesorderdetail", "salesrepid")]
			public const string system_user_salesorderdetail = "system_user_salesorderdetail";
			[Relationship("serviceappointment", EntityRole.Referenced, "system_user_service_appointments", "owninguser")]
			public const string system_user_service_appointments = "system_user_service_appointments";
			[Relationship("contract", EntityRole.Referenced, "system_user_service_contracts", "owninguser")]
			public const string system_user_service_contracts = "system_user_service_contracts";
			[Relationship("territory", EntityRole.Referenced, "system_user_territories", "managerid")]
			public const string system_user_territories = "system_user_territories";
			[Relationship("workflow", EntityRole.Referenced, "system_user_workflow", "owninguser")]
			public const string system_user_workflow = "system_user_workflow";
			[Relationship("asyncoperation", EntityRole.Referenced, "SystemUser_AsyncOperations", "regardingobjectid")]
			public const string SystemUser_AsyncOperations = "SystemUser_AsyncOperations";
			[Relationship("bookableresource", EntityRole.Referenced, "systemuser_bookableresource_UserId", "userid")]
			public const string systemuser_bookableresource_UserId = "systemuser_bookableresource_UserId";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "SystemUser_BulkDeleteFailures", "regardingobjectid")]
			public const string SystemUser_BulkDeleteFailures = "SystemUser_BulkDeleteFailures";
			[Relationship("campaign", EntityRole.Referenced, "SystemUser_Campaigns", "owninguser")]
			public const string SystemUser_Campaigns = "SystemUser_Campaigns";
			[Relationship("connection", EntityRole.Referenced, "systemuser_connections1", "record1id")]
			public const string systemuser_connections1 = "systemuser_connections1";
			[Relationship("connection", EntityRole.Referenced, "systemuser_connections2", "record2id")]
			public const string systemuser_connections2 = "systemuser_connections2";
			[Relationship("duplicaterecord", EntityRole.Referenced, "SystemUser_DuplicateBaseRecord", "baserecordid")]
			public const string SystemUser_DuplicateBaseRecord = "SystemUser_DuplicateBaseRecord";
			[Relationship("duplicaterecord", EntityRole.Referenced, "SystemUser_DuplicateMatchingRecord", "duplicaterecordid")]
			public const string SystemUser_DuplicateMatchingRecord = "SystemUser_DuplicateMatchingRecord";
			[Relationship("duplicaterule", EntityRole.Referenced, "SystemUser_DuplicateRules", "owninguser")]
			public const string SystemUser_DuplicateRules = "SystemUser_DuplicateRules";
			[Relationship("email", EntityRole.Referenced, "SystemUser_Email_EmailSender", "emailsender")]
			public const string SystemUser_Email_EmailSender = "SystemUser_Email_EmailSender";
			[Relationship("externalpartyitem", EntityRole.Referenced, "SystemUser_ExternalPartyItems", "regardingobjectid")]
			public const string SystemUser_ExternalPartyItems = "SystemUser_ExternalPartyItems";
			[Relationship("importdata", EntityRole.Referenced, "SystemUser_ImportData", "owninguser")]
			public const string SystemUser_ImportData = "SystemUser_ImportData";
			[Relationship("importfile", EntityRole.Referenced, "SystemUser_ImportFiles", "owninguser")]
			public const string SystemUser_ImportFiles = "SystemUser_ImportFiles";
			[Relationship("importlog", EntityRole.Referenced, "SystemUser_ImportLogs", "owninguser")]
			public const string SystemUser_ImportLogs = "SystemUser_ImportLogs";
			[Relationship("importmap", EntityRole.Referenced, "SystemUser_ImportMaps", "owninguser")]
			public const string SystemUser_ImportMaps = "SystemUser_ImportMaps";
			[Relationship("import", EntityRole.Referenced, "SystemUser_Imports", "owninguser")]
			public const string SystemUser_Imports = "SystemUser_Imports";
			[Relationship("internaladdress", EntityRole.Referenced, "systemuser_internal_addresses", "parentid")]
			public const string systemuser_internal_addresses = "systemuser_internal_addresses";
			[Relationship("postfollow", EntityRole.Referenced, "systemuser_PostFollows", "regardingobjectid")]
			public const string systemuser_PostFollows = "systemuser_PostFollows";
			[Relationship("postregarding", EntityRole.Referenced, "systemuser_PostRegardings", "regardingobjectid")]
			public const string systemuser_PostRegardings = "systemuser_PostRegardings";
			[Relationship("postrole", EntityRole.Referenced, "systemuser_PostRoles", "regardingobjectid")]
			public const string systemuser_PostRoles = "systemuser_PostRoles";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "systemuser_principalobjectattributeaccess", "objectid")]
			public const string systemuser_principalobjectattributeaccess = "systemuser_principalobjectattributeaccess";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "systemuser_principalobjectattributeaccess_principalid", "principalid")]
			public const string systemuser_principalobjectattributeaccess_principalid = "systemuser_principalobjectattributeaccess_principalid";
			[Relationship("processsession", EntityRole.Referenced, "SystemUser_ProcessSessions", "regardingobjectid")]
			public const string SystemUser_ProcessSessions = "SystemUser_ProcessSessions";
			[Relationship("resource", EntityRole.Referenced, "systemuser_resources", "resourceid")]
			public const string systemuser_resources = "systemuser_resources";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "systemuser_SharePointDocumentLocations", "")]
			public const string systemuser_SharePointDocumentLocations = "systemuser_SharePointDocumentLocations";
			[Relationship("syncerror", EntityRole.Referenced, "SystemUser_SyncError", "owninguser")]
			public const string SystemUser_SyncError = "SystemUser_SyncError";
			[Relationship("syncerror", EntityRole.Referenced, "SystemUser_SyncErrors", "regardingobjectid")]
			public const string SystemUser_SyncErrors = "SystemUser_SyncErrors";
			[Relationship("systemuserbusinessunitentitymap", EntityRole.Referenced, "systemuserbusinessunitentitymap_systemuserid_systemuser", "systemuserid")]
			public const string systemuserbusinessunitentitymap_systemuserid_systemuser = "systemuserbusinessunitentitymap_systemuserid_systemuser";
			[Relationship("tit_sms", EntityRole.Referenced, "tit_sms_systemuser_createdby", "")]
			public const string tit_sms_systemuser_createdby = "tit_sms_systemuser_createdby";
			[Relationship("tit_sms", EntityRole.Referenced, "tit_sms_systemuser_createdonbehalfby", "")]
			public const string tit_sms_systemuser_createdonbehalfby = "tit_sms_systemuser_createdonbehalfby";
			[Relationship("tit_sms", EntityRole.Referenced, "tit_sms_systemuser_modifiedby", "")]
			public const string tit_sms_systemuser_modifiedby = "tit_sms_systemuser_modifiedby";
			[Relationship("tit_sms", EntityRole.Referenced, "tit_sms_systemuser_modifiedonbehalfby", "")]
			public const string tit_sms_systemuser_modifiedonbehalfby = "tit_sms_systemuser_modifiedonbehalfby";
			[Relationship("tit_sms", EntityRole.Referenced, "tit_sms_systemuser_owninguser", "")]
			public const string tit_sms_systemuser_owninguser = "tit_sms_systemuser_owninguser";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "user_accounts", "owninguser")]
			public const string user_accounts = "user_accounts";
			[Relationship("activitypointer", EntityRole.Referenced, "user_activity", "owninguser")]
			public const string user_activity = "user_activity";
			[Relationship("adminsettingsentity", EntityRole.Referenced, "user_adminsettingsentity", "owninguser")]
			public const string user_adminsettingsentity = "user_adminsettingsentity";
			[Relationship("appointment", EntityRole.Referenced, "user_appointment", "owninguser")]
			public const string user_appointment = "user_appointment";
			[Relationship("bookableresource", EntityRole.Referenced, "user_bookableresource", "owninguser")]
			public const string user_bookableresource = "user_bookableresource";
			[Relationship("bookableresourcebooking", EntityRole.Referenced, "user_bookableresourcebooking", "owninguser")]
			public const string user_bookableresourcebooking = "user_bookableresourcebooking";
			[Relationship("bookableresourcebookingexchangesyncidmapping", EntityRole.Referenced, "user_bookableresourcebookingexchangesyncidmapping", "owninguser")]
			public const string user_bookableresourcebookingexchangesyncidmapping = "user_bookableresourcebookingexchangesyncidmapping";
			[Relationship("bookableresourcebookingheader", EntityRole.Referenced, "user_bookableresourcebookingheader", "owninguser")]
			public const string user_bookableresourcebookingheader = "user_bookableresourcebookingheader";
			[Relationship("bookableresourcecategory", EntityRole.Referenced, "user_bookableresourcecategory", "owninguser")]
			public const string user_bookableresourcecategory = "user_bookableresourcecategory";
			[Relationship("bookableresourcecategoryassn", EntityRole.Referenced, "user_bookableresourcecategoryassn", "owninguser")]
			public const string user_bookableresourcecategoryassn = "user_bookableresourcecategoryassn";
			[Relationship("bookableresourcecharacteristic", EntityRole.Referenced, "user_bookableresourcecharacteristic", "owninguser")]
			public const string user_bookableresourcecharacteristic = "user_bookableresourcecharacteristic";
			[Relationship("bookableresourcegroup", EntityRole.Referenced, "user_bookableresourcegroup", "owninguser")]
			public const string user_bookableresourcegroup = "user_bookableresourcegroup";
			[Relationship("bookingstatus", EntityRole.Referenced, "user_bookingstatus", "owninguser")]
			public const string user_bookingstatus = "user_bookingstatus";
			[Relationship("bulkoperation", EntityRole.Referenced, "user_BulkOperation", "owninguser")]
			public const string user_BulkOperation = "user_BulkOperation";
			[Relationship("bulkoperationlog", EntityRole.Referenced, "user_bulkoperationlog", "owninguser")]
			public const string user_bulkoperationlog = "user_bulkoperationlog";
			[Relationship("campaignactivity", EntityRole.Referenced, "user_campaignactivity", "owninguser")]
			public const string user_campaignactivity = "user_campaignactivity";
			[Relationship("campaignresponse", EntityRole.Referenced, "user_campaignresponse", "owninguser")]
			public const string user_campaignresponse = "user_campaignresponse";
			[Relationship("cgo_intervention", EntityRole.Referenced, "user_cgo_intervention", "owninguser")]
			public const string user_cgo_intervention = "user_cgo_intervention";
			[Relationship("cgo_servicecontract", EntityRole.Referenced, "user_cgo_servicecontract", "owninguser")]
			public const string user_cgo_servicecontract = "user_cgo_servicecontract";
			[Relationship("cgo_testunmanaged", EntityRole.Referenced, "user_cgo_testunmanaged", "owninguser")]
			public const string user_cgo_testunmanaged = "user_cgo_testunmanaged";
			[Relationship("channelaccessprofile", EntityRole.Referenced, "user_channelaccessprofile", "owninguser")]
			public const string user_channelaccessprofile = "user_channelaccessprofile";
			[Relationship("characteristic", EntityRole.Referenced, "user_characteristic", "owninguser")]
			public const string user_characteristic = "user_characteristic";
			[Relationship("contractdetail", EntityRole.Referenced, "user_contractdetail", "owninguser")]
			public const string user_contractdetail = "user_contractdetail";
			[Relationship("convertrule", EntityRole.Referenced, "user_convertrule", "owninguser")]
			public const string user_convertrule = "user_convertrule";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "user_customer_opportunity_roles", "owninguser")]
			public const string user_customer_opportunity_roles = "user_customer_opportunity_roles";
			[Relationship("customerrelationship", EntityRole.Referenced, "user_customer_relationship", "owninguser")]
			public const string user_customer_relationship = "user_customer_relationship";
			[Relationship("email", EntityRole.Referenced, "user_email", "owninguser")]
			public const string user_email = "user_email";
			[Relationship("entitlement", EntityRole.Referenced, "user_entitlement", "owninguser")]
			public const string user_entitlement = "user_entitlement";
			[Relationship("entitlementchannel", EntityRole.Referenced, "user_entitlementchannel", "owninguser")]
			public const string user_entitlementchannel = "user_entitlementchannel";
			[Relationship("entitlemententityallocationtypemapping", EntityRole.Referenced, "user_entitlemententityallocationtypemapping", "owninguser")]
			public const string user_entitlemententityallocationtypemapping = "user_entitlemententityallocationtypemapping";
			[Relationship("exchangesyncidmapping", EntityRole.Referenced, "user_exchangesyncidmapping", "owninguser")]
			public const string user_exchangesyncidmapping = "user_exchangesyncidmapping";
			[Relationship("externalparty", EntityRole.Referenced, "systemuser_user_externalparty", "owninguser")]
			public const string user_externalparty = "user_externalparty";
			[Relationship("fax", EntityRole.Referenced, "user_fax", "owninguser")]
			public const string user_fax = "user_fax";
			[Relationship("goal", EntityRole.Referenced, "user_goal", "owninguser")]
			public const string user_goal = "user_goal";
			[Relationship("goal", EntityRole.Referenced, "user_goal_goalowner", "goalownerid")]
			public const string user_goal_goalowner = "user_goal_goalowner";
			[Relationship("incidentresolution", EntityRole.Referenced, "user_incidentresolution", "owninguser")]
			public const string user_incidentresolution = "user_incidentresolution";
			[Relationship("interactionforemail", EntityRole.Referenced, "user_new_interactionforemail", "owninguser")]
			public const string user_interactionforemail = "user_interactionforemail";
			[Relationship("invoicedetail", EntityRole.Referenced, "user_invoicedetail", "owninguser")]
			public const string user_invoicedetail = "user_invoicedetail";
			[Relationship("knowledgearticle", EntityRole.Referenced, "user_knowledgearticle", "owninguser")]
			public const string user_knowledgearticle = "user_knowledgearticle";
			[Relationship("knowledgearticleincident", EntityRole.Referenced, "user_knowledgearticleincident", "owninguser")]
			public const string user_knowledgearticleincident = "user_knowledgearticleincident";
			[Relationship("letter", EntityRole.Referenced, "user_letter", "owninguser")]
			public const string user_letter = "user_letter";
			[Relationship("list", EntityRole.Referenced, "user_list", "owninguser")]
			public const string user_list = "user_list";
			[Relationship("mailbox", EntityRole.Referenced, "user_mailbox", "owninguser")]
			public const string user_mailbox = "user_mailbox";
			[Relationship("msdyn_accountpricelist", EntityRole.Referenced, "user_msdyn_accountpricelist", "owninguser")]
			public const string user_msdyn_accountpricelist = "user_msdyn_accountpricelist";
			[Relationship("msdyn_actual", EntityRole.Referenced, "user_msdyn_actual", "owninguser")]
			public const string user_msdyn_actual = "user_msdyn_actual";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "user_msdyn_agreement", "owninguser")]
			public const string user_msdyn_agreement = "user_msdyn_agreement";
			[Relationship("msdyn_agreementbookingdate", EntityRole.Referenced, "user_msdyn_agreementbookingdate", "owninguser")]
			public const string user_msdyn_agreementbookingdate = "user_msdyn_agreementbookingdate";
			[Relationship("msdyn_agreementbookingincident", EntityRole.Referenced, "user_msdyn_agreementbookingincident", "owninguser")]
			public const string user_msdyn_agreementbookingincident = "user_msdyn_agreementbookingincident";
			[Relationship("msdyn_agreementbookingproduct", EntityRole.Referenced, "user_msdyn_agreementbookingproduct", "owninguser")]
			public const string user_msdyn_agreementbookingproduct = "user_msdyn_agreementbookingproduct";
			[Relationship("msdyn_agreementbookingservice", EntityRole.Referenced, "user_msdyn_agreementbookingservice", "owninguser")]
			public const string user_msdyn_agreementbookingservice = "user_msdyn_agreementbookingservice";
			[Relationship("msdyn_agreementbookingservicetask", EntityRole.Referenced, "user_msdyn_agreementbookingservicetask", "owninguser")]
			public const string user_msdyn_agreementbookingservicetask = "user_msdyn_agreementbookingservicetask";
			[Relationship("msdyn_agreementbookingsetup", EntityRole.Referenced, "user_msdyn_agreementbookingsetup", "owninguser")]
			public const string user_msdyn_agreementbookingsetup = "user_msdyn_agreementbookingsetup";
			[Relationship("msdyn_agreementinvoicedate", EntityRole.Referenced, "user_msdyn_agreementinvoicedate", "owninguser")]
			public const string user_msdyn_agreementinvoicedate = "user_msdyn_agreementinvoicedate";
			[Relationship("msdyn_agreementinvoiceproduct", EntityRole.Referenced, "user_msdyn_agreementinvoiceproduct", "owninguser")]
			public const string user_msdyn_agreementinvoiceproduct = "user_msdyn_agreementinvoiceproduct";
			[Relationship("msdyn_agreementinvoicesetup", EntityRole.Referenced, "user_msdyn_agreementinvoicesetup", "owninguser")]
			public const string user_msdyn_agreementinvoicesetup = "user_msdyn_agreementinvoicesetup";
			[Relationship("msdyn_agreementsubstatus", EntityRole.Referenced, "user_msdyn_agreementsubstatus", "owninguser")]
			public const string user_msdyn_agreementsubstatus = "user_msdyn_agreementsubstatus";
			[Relationship("msdyn_answer", EntityRole.Referenced, "user_msdyn_answer", "")]
			public const string user_msdyn_answer = "user_msdyn_answer";
			[Relationship("msdyn_bookingalertstatus", EntityRole.Referenced, "user_msdyn_bookingalertstatus", "owninguser")]
			public const string user_msdyn_bookingalertstatus = "user_msdyn_bookingalertstatus";
			[Relationship("msdyn_bookingchange", EntityRole.Referenced, "user_msdyn_bookingchange", "owninguser")]
			public const string user_msdyn_bookingchange = "user_msdyn_bookingchange";
			[Relationship("msdyn_bookingjournal", EntityRole.Referenced, "user_msdyn_bookingjournal", "owninguser")]
			public const string user_msdyn_bookingjournal = "user_msdyn_bookingjournal";
			[Relationship("msdyn_bookingrule", EntityRole.Referenced, "user_msdyn_bookingrule", "owninguser")]
			public const string user_msdyn_bookingrule = "user_msdyn_bookingrule";
			[Relationship("msdyn_bookingsetupmetadata", EntityRole.Referenced, "user_msdyn_bookingsetupmetadata", "owninguser")]
			public const string user_msdyn_bookingsetupmetadata = "user_msdyn_bookingsetupmetadata";
			[Relationship("msdyn_bookingtimestamp", EntityRole.Referenced, "user_msdyn_bookingtimestamp", "owninguser")]
			public const string user_msdyn_bookingtimestamp = "user_msdyn_bookingtimestamp";
			[Relationship("msdyn_callablecontext", EntityRole.Referenced, "user_msdyn_callablecontext", "owninguser")]
			public const string user_msdyn_callablecontext = "user_msdyn_callablecontext";
			[Relationship("msdyn_characteristicreqforteammember", EntityRole.Referenced, "user_msdyn_characteristicreqforteammember", "owninguser")]
			public const string user_msdyn_characteristicreqforteammember = "user_msdyn_characteristicreqforteammember";
			[Relationship("msdyn_contactpricelist", EntityRole.Referenced, "user_msdyn_contactpricelist", "owninguser")]
			public const string user_msdyn_contactpricelist = "user_msdyn_contactpricelist";
			[Relationship("msdyn_contractlinescheduleofvalue", EntityRole.Referenced, "user_msdyn_contractlinescheduleofvalue", "owninguser")]
			public const string user_msdyn_contractlinescheduleofvalue = "user_msdyn_contractlinescheduleofvalue";
			[Relationship("msdyn_customerasset", EntityRole.Referenced, "user_msdyn_customerasset", "owninguser")]
			public const string user_msdyn_customerasset = "user_msdyn_customerasset";
			[Relationship("msdyn_dataexport", EntityRole.Referenced, "user_msdyn_dataexport", "owninguser")]
			public const string user_msdyn_dataexport = "user_msdyn_dataexport";
			[Relationship("msdyn_delegation", EntityRole.Referenced, "user_msdyn_delegation", "owninguser")]
			public const string user_msdyn_delegation = "user_msdyn_delegation";
			[Relationship("msdyn_estimate", EntityRole.Referenced, "user_msdyn_estimate", "owninguser")]
			public const string user_msdyn_estimate = "user_msdyn_estimate";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "user_msdyn_estimateline", "owninguser")]
			public const string user_msdyn_estimateline = "user_msdyn_estimateline";
			[Relationship("msdyn_expense", EntityRole.Referenced, "user_msdyn_expense", "owninguser")]
			public const string user_msdyn_expense = "user_msdyn_expense";
			[Relationship("msdyn_expensereceipt", EntityRole.Referenced, "user_msdyn_expensereceipt", "owninguser")]
			public const string user_msdyn_expensereceipt = "user_msdyn_expensereceipt";
			[Relationship("msdyn_fact", EntityRole.Referenced, "user_msdyn_fact", "owninguser")]
			public const string user_msdyn_fact = "user_msdyn_fact";
			[Relationship("msdyn_feedbackmapping", EntityRole.Referenced, "user_msdyn_feedbackmapping", "")]
			public const string user_msdyn_feedbackmapping = "user_msdyn_feedbackmapping";
			[Relationship("msdyn_feedbacksubsurvey", EntityRole.Referenced, "user_msdyn_feedbacksubsurvey", "")]
			public const string user_msdyn_feedbacksubsurvey = "user_msdyn_feedbacksubsurvey";
			[Relationship("msdyn_fieldcomputation", EntityRole.Referenced, "user_msdyn_fieldcomputation", "owninguser")]
			public const string user_msdyn_fieldcomputation = "user_msdyn_fieldcomputation";
			[Relationship("msdyn_fieldservicesetting", EntityRole.Referenced, "user_msdyn_fieldservicesetting", "owninguser")]
			public const string user_msdyn_fieldservicesetting = "user_msdyn_fieldservicesetting";
			[Relationship("msdyn_findworkevent", EntityRole.Referenced, "user_msdyn_findworkevent", "owninguser")]
			public const string user_msdyn_findworkevent = "user_msdyn_findworkevent";
			[Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "user_msdyn_icebreakersconfig", "owninguser")]
			public const string user_msdyn_icebreakersconfig = "user_msdyn_icebreakersconfig";
			[Relationship("msdyn_image", EntityRole.Referenced, "user_msdyn_image", "")]
			public const string user_msdyn_image = "user_msdyn_image";
			[Relationship("msdyn_import", EntityRole.Referenced, "user_msdyn_import", "")]
			public const string user_msdyn_import = "user_msdyn_import";
			[Relationship("msdyn_incidenttype", EntityRole.Referenced, "user_msdyn_incidenttype", "owninguser")]
			public const string user_msdyn_incidenttype = "user_msdyn_incidenttype";
			[Relationship("msdyn_incidenttypecharacteristic", EntityRole.Referenced, "user_msdyn_incidenttypecharacteristic", "owninguser")]
			public const string user_msdyn_incidenttypecharacteristic = "user_msdyn_incidenttypecharacteristic";
			[Relationship("msdyn_incidenttypeproduct", EntityRole.Referenced, "user_msdyn_incidenttypeproduct", "owninguser")]
			public const string user_msdyn_incidenttypeproduct = "user_msdyn_incidenttypeproduct";
			[Relationship("msdyn_incidenttypeservice", EntityRole.Referenced, "user_msdyn_incidenttypeservice", "owninguser")]
			public const string user_msdyn_incidenttypeservice = "user_msdyn_incidenttypeservice";
			[Relationship("msdyn_incidenttypeservicetask", EntityRole.Referenced, "user_msdyn_incidenttypeservicetask", "owninguser")]
			public const string user_msdyn_incidenttypeservicetask = "user_msdyn_incidenttypeservicetask";
			[Relationship("msdyn_integrationjob", EntityRole.Referenced, "user_msdyn_integrationjob", "owninguser")]
			public const string user_msdyn_integrationjob = "user_msdyn_integrationjob";
			[Relationship("msdyn_integrationjobdetail", EntityRole.Referenced, "user_msdyn_integrationjobdetail", "owninguser")]
			public const string user_msdyn_integrationjobdetail = "user_msdyn_integrationjobdetail";
			[Relationship("msdyn_inventoryadjustment", EntityRole.Referenced, "user_msdyn_inventoryadjustment", "owninguser")]
			public const string user_msdyn_inventoryadjustment = "user_msdyn_inventoryadjustment";
			[Relationship("msdyn_inventoryadjustmentproduct", EntityRole.Referenced, "user_msdyn_inventoryadjustmentproduct", "owninguser")]
			public const string user_msdyn_inventoryadjustmentproduct = "user_msdyn_inventoryadjustmentproduct";
			[Relationship("msdyn_inventoryjournal", EntityRole.Referenced, "user_msdyn_inventoryjournal", "owninguser")]
			public const string user_msdyn_inventoryjournal = "user_msdyn_inventoryjournal";
			[Relationship("msdyn_inventorytransfer", EntityRole.Referenced, "user_msdyn_inventorytransfer", "owninguser")]
			public const string user_msdyn_inventorytransfer = "user_msdyn_inventorytransfer";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "user_msdyn_invoicelinetransaction", "owninguser")]
			public const string user_msdyn_invoicelinetransaction = "user_msdyn_invoicelinetransaction";
			[Relationship("msdyn_journal", EntityRole.Referenced, "user_msdyn_journal", "owninguser")]
			public const string user_msdyn_journal = "user_msdyn_journal";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "user_msdyn_journalline", "owninguser")]
			public const string user_msdyn_journalline = "user_msdyn_journalline";
			[Relationship("msdyn_linkedanswer", EntityRole.Referenced, "user_msdyn_linkedanswer", "")]
			public const string user_msdyn_linkedanswer = "user_msdyn_linkedanswer";
			[Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "user_msdyn_notesanalysisconfig", "owninguser")]
			public const string user_msdyn_notesanalysisconfig = "user_msdyn_notesanalysisconfig";
			[Relationship("msdyn_opportunitylineresourcecategory", EntityRole.Referenced, "user_msdyn_opportunitylineresourcecategory", "owninguser")]
			public const string user_msdyn_opportunitylineresourcecategory = "user_msdyn_opportunitylineresourcecategory";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "user_msdyn_opportunitylinetransaction", "owninguser")]
			public const string user_msdyn_opportunitylinetransaction = "user_msdyn_opportunitylinetransaction";
			[Relationship("msdyn_opportunitylinetransactioncategory", EntityRole.Referenced, "user_msdyn_opportunitylinetransactioncategory", "owninguser")]
			public const string user_msdyn_opportunitylinetransactioncategory = "user_msdyn_opportunitylinetransactioncategory";
			[Relationship("msdyn_opportunitylinetransactionclassificatio", EntityRole.Referenced, "user_msdyn_opportunitylinetransactionclassificatio", "owninguser")]
			public const string user_msdyn_opportunitylinetransactionclassificatio = "user_msdyn_opportunitylinetransactionclassificatio";
			[Relationship("msdyn_opportunitypricelist", EntityRole.Referenced, "user_msdyn_opportunitypricelist", "owninguser")]
			public const string user_msdyn_opportunitypricelist = "user_msdyn_opportunitypricelist";
			[Relationship("msdyn_orderinvoicingdate", EntityRole.Referenced, "user_msdyn_orderinvoicingdate", "owninguser")]
			public const string user_msdyn_orderinvoicingdate = "user_msdyn_orderinvoicingdate";
			[Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "user_msdyn_orderinvoicingproduct", "owninguser")]
			public const string user_msdyn_orderinvoicingproduct = "user_msdyn_orderinvoicingproduct";
			[Relationship("msdyn_orderinvoicingsetup", EntityRole.Referenced, "user_msdyn_orderinvoicingsetup", "owninguser")]
			public const string user_msdyn_orderinvoicingsetup = "user_msdyn_orderinvoicingsetup";
			[Relationship("msdyn_orderinvoicingsetupdate", EntityRole.Referenced, "user_msdyn_orderinvoicingsetupdate", "owninguser")]
			public const string user_msdyn_orderinvoicingsetupdate = "user_msdyn_orderinvoicingsetupdate";
			[Relationship("msdyn_orderlineresourcecategory", EntityRole.Referenced, "user_msdyn_orderlineresourcecategory", "owninguser")]
			public const string user_msdyn_orderlineresourcecategory = "user_msdyn_orderlineresourcecategory";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "user_msdyn_orderlinetransaction", "owninguser")]
			public const string user_msdyn_orderlinetransaction = "user_msdyn_orderlinetransaction";
			[Relationship("msdyn_orderlinetransactioncategory", EntityRole.Referenced, "user_msdyn_orderlinetransactioncategory", "owninguser")]
			public const string user_msdyn_orderlinetransactioncategory = "user_msdyn_orderlinetransactioncategory";
			[Relationship("msdyn_orderlinetransactionclassification", EntityRole.Referenced, "user_msdyn_orderlinetransactionclassification", "owninguser")]
			public const string user_msdyn_orderlinetransactionclassification = "user_msdyn_orderlinetransactionclassification";
			[Relationship("msdyn_orderpricelist", EntityRole.Referenced, "user_msdyn_orderpricelist", "owninguser")]
			public const string user_msdyn_orderpricelist = "user_msdyn_orderpricelist";
			[Relationship("msdyn_orginsightsuserdashboarddefinition", EntityRole.Referenced, "user_msdyn_orginsightsuserdashboarddefinition", "")]
			public const string user_msdyn_orginsightsuserdashboarddefinition = "user_msdyn_orginsightsuserdashboarddefinition";
			[Relationship("msdyn_page", EntityRole.Referenced, "user_msdyn_page", "")]
			public const string user_msdyn_page = "user_msdyn_page";
			[Relationship("msdyn_payment", EntityRole.Referenced, "user_msdyn_payment", "owninguser")]
			public const string user_msdyn_payment = "user_msdyn_payment";
			[Relationship("msdyn_paymentdetail", EntityRole.Referenced, "user_msdyn_paymentdetail", "owninguser")]
			public const string user_msdyn_paymentdetail = "user_msdyn_paymentdetail";
			[Relationship("msdyn_paymentmethod", EntityRole.Referenced, "user_msdyn_paymentmethod", "owninguser")]
			public const string user_msdyn_paymentmethod = "user_msdyn_paymentmethod";
			[Relationship("msdyn_paymentterm", EntityRole.Referenced, "user_msdyn_paymentterm", "owninguser")]
			public const string user_msdyn_paymentterm = "user_msdyn_paymentterm";
			[Relationship("msdyn_playbookactivity", EntityRole.Referenced, "user_msdyn_playbookactivity", "owninguser")]
			public const string user_msdyn_playbookactivity = "user_msdyn_playbookactivity";
			[Relationship("msdyn_playbookactivityattribute", EntityRole.Referenced, "user_msdyn_playbookactivityattribute", "owninguser")]
			public const string user_msdyn_playbookactivityattribute = "user_msdyn_playbookactivityattribute";
			[Relationship("msdyn_playbookcategory", EntityRole.Referenced, "user_msdyn_playbookcategory", "owninguser")]
			public const string user_msdyn_playbookcategory = "user_msdyn_playbookcategory";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "user_msdyn_playbookinstance", "owninguser")]
			public const string user_msdyn_playbookinstance = "user_msdyn_playbookinstance";
			[Relationship("msdyn_playbooktemplate", EntityRole.Referenced, "user_msdyn_playbooktemplate", "owninguser")]
			public const string user_msdyn_playbooktemplate = "user_msdyn_playbooktemplate";
			[Relationship("msdyn_postalbum", EntityRole.Referenced, "user_msdyn_postalbum", "owninguser")]
			public const string user_msdyn_postalbum = "user_msdyn_postalbum";
			[Relationship("msdyn_postalcode", EntityRole.Referenced, "user_msdyn_postalcode", "owninguser")]
			public const string user_msdyn_postalcode = "user_msdyn_postalcode";
			[Relationship("msdyn_priority", EntityRole.Referenced, "user_msdyn_priority", "owninguser")]
			public const string user_msdyn_priority = "user_msdyn_priority";
			[Relationship("msdyn_project", EntityRole.Referenced, "user_msdyn_project", "owninguser")]
			public const string user_msdyn_project = "user_msdyn_project";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "user_msdyn_projectapproval", "owninguser")]
			public const string user_msdyn_projectapproval = "user_msdyn_projectapproval";
			[Relationship("msdyn_projectpricelist", EntityRole.Referenced, "user_msdyn_projectpricelist", "owninguser")]
			public const string user_msdyn_projectpricelist = "user_msdyn_projectpricelist";
			[Relationship("msdyn_projecttask", EntityRole.Referenced, "user_msdyn_projecttask", "owninguser")]
			public const string user_msdyn_projecttask = "user_msdyn_projecttask";
			[Relationship("msdyn_projecttaskdependency", EntityRole.Referenced, "user_msdyn_projecttaskdependency", "owninguser")]
			public const string user_msdyn_projecttaskdependency = "user_msdyn_projecttaskdependency";
			[Relationship("msdyn_projecttaskstatususer", EntityRole.Referenced, "user_msdyn_projecttaskstatususer", "owninguser")]
			public const string user_msdyn_projecttaskstatususer = "user_msdyn_projecttaskstatususer";
			[Relationship("msdyn_projectteam", EntityRole.Referenced, "user_msdyn_projectteam", "owninguser")]
			public const string user_msdyn_projectteam = "user_msdyn_projectteam";
			[Relationship("msdyn_projecttransactioncategory", EntityRole.Referenced, "user_msdyn_projecttransactioncategory", "owninguser")]
			public const string user_msdyn_projecttransactioncategory = "user_msdyn_projecttransactioncategory";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "user_msdyn_purchaseorder", "owninguser")]
			public const string user_msdyn_purchaseorder = "user_msdyn_purchaseorder";
			[Relationship("msdyn_purchaseorderbill", EntityRole.Referenced, "user_msdyn_purchaseorderbill", "owninguser")]
			public const string user_msdyn_purchaseorderbill = "user_msdyn_purchaseorderbill";
			[Relationship("msdyn_purchaseorderproduct", EntityRole.Referenced, "user_msdyn_purchaseorderproduct", "owninguser")]
			public const string user_msdyn_purchaseorderproduct = "user_msdyn_purchaseorderproduct";
			[Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "user_msdyn_purchaseorderreceipt", "owninguser")]
			public const string user_msdyn_purchaseorderreceipt = "user_msdyn_purchaseorderreceipt";
			[Relationship("msdyn_purchaseorderreceiptproduct", EntityRole.Referenced, "user_msdyn_purchaseorderreceiptproduct", "owninguser")]
			public const string user_msdyn_purchaseorderreceiptproduct = "user_msdyn_purchaseorderreceiptproduct";
			[Relationship("msdyn_purchaseordersubstatus", EntityRole.Referenced, "user_msdyn_purchaseordersubstatus", "owninguser")]
			public const string user_msdyn_purchaseordersubstatus = "user_msdyn_purchaseordersubstatus";
			[Relationship("msdyn_question", EntityRole.Referenced, "user_msdyn_question", "")]
			public const string user_msdyn_question = "user_msdyn_question";
			[Relationship("msdyn_questiongroup", EntityRole.Referenced, "user_msdyn_questiongroup", "")]
			public const string user_msdyn_questiongroup = "user_msdyn_questiongroup";
			[Relationship("msdyn_questionresponse", EntityRole.Referenced, "user_msdyn_questionresponse", "")]
			public const string user_msdyn_questionresponse = "user_msdyn_questionresponse";
			[Relationship("msdyn_questiontype", EntityRole.Referenced, "user_msdyn_questiontype", "")]
			public const string user_msdyn_questiontype = "user_msdyn_questiontype";
			[Relationship("msdyn_quotebookingincident", EntityRole.Referenced, "user_msdyn_quotebookingincident", "owninguser")]
			public const string user_msdyn_quotebookingincident = "user_msdyn_quotebookingincident";
			[Relationship("msdyn_quotebookingproduct", EntityRole.Referenced, "user_msdyn_quotebookingproduct", "owninguser")]
			public const string user_msdyn_quotebookingproduct = "user_msdyn_quotebookingproduct";
			[Relationship("msdyn_quotebookingservice", EntityRole.Referenced, "user_msdyn_quotebookingservice", "owninguser")]
			public const string user_msdyn_quotebookingservice = "user_msdyn_quotebookingservice";
			[Relationship("msdyn_quotebookingservicetask", EntityRole.Referenced, "user_msdyn_quotebookingservicetask", "owninguser")]
			public const string user_msdyn_quotebookingservicetask = "user_msdyn_quotebookingservicetask";
			[Relationship("msdyn_quotebookingsetup", EntityRole.Referenced, "user_msdyn_quotebookingsetup", "owninguser")]
			public const string user_msdyn_quotebookingsetup = "user_msdyn_quotebookingsetup";
			[Relationship("msdyn_quoteinvoicingproduct", EntityRole.Referenced, "user_msdyn_quoteinvoicingproduct", "owninguser")]
			public const string user_msdyn_quoteinvoicingproduct = "user_msdyn_quoteinvoicingproduct";
			[Relationship("msdyn_quoteinvoicingsetup", EntityRole.Referenced, "user_msdyn_quoteinvoicingsetup", "owninguser")]
			public const string user_msdyn_quoteinvoicingsetup = "user_msdyn_quoteinvoicingsetup";
			[Relationship("msdyn_quotelineanalyticsbreakdown", EntityRole.Referenced, "user_msdyn_quotelineanalyticsbreakdown", "owninguser")]
			public const string user_msdyn_quotelineanalyticsbreakdown = "user_msdyn_quotelineanalyticsbreakdown";
			[Relationship("msdyn_quotelineresourcecategory", EntityRole.Referenced, "user_msdyn_quotelineresourcecategory", "owninguser")]
			public const string user_msdyn_quotelineresourcecategory = "user_msdyn_quotelineresourcecategory";
			[Relationship("msdyn_quotelinescheduleofvalue", EntityRole.Referenced, "user_msdyn_quotelinescheduleofvalue", "owninguser")]
			public const string user_msdyn_quotelinescheduleofvalue = "user_msdyn_quotelinescheduleofvalue";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "user_msdyn_quotelinetransaction", "owninguser")]
			public const string user_msdyn_quotelinetransaction = "user_msdyn_quotelinetransaction";
			[Relationship("msdyn_quotelinetransactioncategory", EntityRole.Referenced, "user_msdyn_quotelinetransactioncategory", "owninguser")]
			public const string user_msdyn_quotelinetransactioncategory = "user_msdyn_quotelinetransactioncategory";
			[Relationship("msdyn_quotelinetransactionclassification", EntityRole.Referenced, "user_msdyn_quotelinetransactionclassification", "owninguser")]
			public const string user_msdyn_quotelinetransactionclassification = "user_msdyn_quotelinetransactionclassification";
			[Relationship("msdyn_quotepricelist", EntityRole.Referenced, "user_msdyn_quotepricelist", "owninguser")]
			public const string user_msdyn_quotepricelist = "user_msdyn_quotepricelist";
			[Relationship("msdyn_relationshipinsightsunifiedconfig", EntityRole.Referenced, "user_msdyn_relationshipinsightsunifiedconfig", "owninguser")]
			public const string user_msdyn_relationshipinsightsunifiedconfig = "user_msdyn_relationshipinsightsunifiedconfig";
			[Relationship("msdyn_requirementcharacteristic", EntityRole.Referenced, "user_msdyn_requirementcharacteristic", "owninguser")]
			public const string user_msdyn_requirementcharacteristic = "user_msdyn_requirementcharacteristic";
			[Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "user_msdyn_requirementorganizationunit", "owninguser")]
			public const string user_msdyn_requirementorganizationunit = "user_msdyn_requirementorganizationunit";
			[Relationship("msdyn_requirementresourcecategory", EntityRole.Referenced, "user_msdyn_requirementresourcecategory", "owninguser")]
			public const string user_msdyn_requirementresourcecategory = "user_msdyn_requirementresourcecategory";
			[Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "user_msdyn_requirementresourcepreference", "owninguser")]
			public const string user_msdyn_requirementresourcepreference = "user_msdyn_requirementresourcepreference";
			[Relationship("msdyn_requirementstatus", EntityRole.Referenced, "user_msdyn_requirementstatus", "owninguser")]
			public const string user_msdyn_requirementstatus = "user_msdyn_requirementstatus";
			[Relationship("msdyn_resourceassignment", EntityRole.Referenced, "user_msdyn_resourceassignment", "owninguser")]
			public const string user_msdyn_resourceassignment = "user_msdyn_resourceassignment";
			[Relationship("msdyn_resourceassignmentdetail", EntityRole.Referenced, "user_msdyn_resourceassignmentdetail", "owninguser")]
			public const string user_msdyn_resourceassignmentdetail = "user_msdyn_resourceassignmentdetail";
			[Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "user_msdyn_resourcepaytype", "owninguser")]
			public const string user_msdyn_resourcepaytype = "user_msdyn_resourcepaytype";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "user_msdyn_resourcerequest", "owninguser")]
			public const string user_msdyn_resourcerequest = "user_msdyn_resourcerequest";
			[Relationship("msdyn_resourcerequirement", EntityRole.Referenced, "user_msdyn_resourcerequirement", "owninguser")]
			public const string user_msdyn_resourcerequirement = "user_msdyn_resourcerequirement";
			[Relationship("msdyn_resourcerequirementdetail", EntityRole.Referenced, "user_msdyn_resourcerequirementdetail", "owninguser")]
			public const string user_msdyn_resourcerequirementdetail = "user_msdyn_resourcerequirementdetail";
			[Relationship("msdyn_resourceterritory", EntityRole.Referenced, "user_msdyn_resourceterritory", "owninguser")]
			public const string user_msdyn_resourceterritory = "user_msdyn_resourceterritory";
			[Relationship("msdyn_responseaction", EntityRole.Referenced, "user_msdyn_responseaction", "")]
			public const string user_msdyn_responseaction = "user_msdyn_responseaction";
			[Relationship("msdyn_responsecondition", EntityRole.Referenced, "user_msdyn_responsecondition", "")]
			public const string user_msdyn_responsecondition = "user_msdyn_responsecondition";
			[Relationship("msdyn_responseerror", EntityRole.Referenced, "user_msdyn_responseerror", "")]
			public const string user_msdyn_responseerror = "user_msdyn_responseerror";
			[Relationship("msdyn_responseoutcome", EntityRole.Referenced, "user_msdyn_responseoutcome", "")]
			public const string user_msdyn_responseoutcome = "user_msdyn_responseoutcome";
			[Relationship("msdyn_responserouting", EntityRole.Referenced, "user_msdyn_responserouting", "")]
			public const string user_msdyn_responserouting = "user_msdyn_responserouting";
			[Relationship("msdyn_rma", EntityRole.Referenced, "user_msdyn_rma", "owninguser")]
			public const string user_msdyn_rma = "user_msdyn_rma";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "user_msdyn_rmaproduct", "owninguser")]
			public const string user_msdyn_rmaproduct = "user_msdyn_rmaproduct";
			[Relationship("msdyn_rmareceipt", EntityRole.Referenced, "user_msdyn_rmareceipt", "owninguser")]
			public const string user_msdyn_rmareceipt = "user_msdyn_rmareceipt";
			[Relationship("msdyn_rmareceiptproduct", EntityRole.Referenced, "user_msdyn_rmareceiptproduct", "owninguser")]
			public const string user_msdyn_rmareceiptproduct = "user_msdyn_rmareceiptproduct";
			[Relationship("msdyn_rmasubstatus", EntityRole.Referenced, "user_msdyn_rmasubstatus", "owninguser")]
			public const string user_msdyn_rmasubstatus = "user_msdyn_rmasubstatus";
			[Relationship("msdyn_rolecompetencyrequirement", EntityRole.Referenced, "user_msdyn_rolecompetencyrequirement", "owninguser")]
			public const string user_msdyn_rolecompetencyrequirement = "user_msdyn_rolecompetencyrequirement";
			[Relationship("msdyn_roleutilization", EntityRole.Referenced, "user_msdyn_roleutilization", "owninguser")]
			public const string user_msdyn_roleutilization = "user_msdyn_roleutilization";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "user_msdyn_rtv", "owninguser")]
			public const string user_msdyn_rtv = "user_msdyn_rtv";
			[Relationship("msdyn_rtvproduct", EntityRole.Referenced, "user_msdyn_rtvproduct", "owninguser")]
			public const string user_msdyn_rtvproduct = "user_msdyn_rtvproduct";
			[Relationship("msdyn_rtvsubstatus", EntityRole.Referenced, "user_msdyn_rtvsubstatus", "owninguser")]
			public const string user_msdyn_rtvsubstatus = "user_msdyn_rtvsubstatus";
			[Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "user_msdyn_scheduleboardsetting", "owninguser")]
			public const string user_msdyn_scheduleboardsetting = "user_msdyn_scheduleboardsetting";
			[Relationship("msdyn_section", EntityRole.Referenced, "user_msdyn_section", "")]
			public const string user_msdyn_section = "user_msdyn_section";
			[Relationship("msdyn_servicetasktype", EntityRole.Referenced, "user_msdyn_servicetasktype", "owninguser")]
			public const string user_msdyn_servicetasktype = "user_msdyn_servicetasktype";
			[Relationship("msdyn_shipvia", EntityRole.Referenced, "user_msdyn_shipvia", "owninguser")]
			public const string user_msdyn_shipvia = "user_msdyn_shipvia";
			[Relationship("msdyn_siconfig", EntityRole.Referenced, "user_msdyn_siconfig", "owninguser")]
			public const string user_msdyn_siconfig = "user_msdyn_siconfig";
			[Relationship("msdyn_survey", EntityRole.Referenced, "user_msdyn_survey", "")]
			public const string user_msdyn_survey = "user_msdyn_survey";
			[Relationship("msdyn_surveylog", EntityRole.Referenced, "user_msdyn_surveylog", "")]
			public const string user_msdyn_surveylog = "user_msdyn_surveylog";
			[Relationship("msdyn_surveyresponse", EntityRole.Referenced, "user_msdyn_surveyresponse", "")]
			public const string user_msdyn_surveyresponse = "user_msdyn_surveyresponse";
			[Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "user_msdyn_systemuserschedulersetting", "owninguser")]
			public const string user_msdyn_systemuserschedulersetting = "user_msdyn_systemuserschedulersetting";
			[Relationship("msdyn_taxcode", EntityRole.Referenced, "user_msdyn_taxcode", "owninguser")]
			public const string user_msdyn_taxcode = "user_msdyn_taxcode";
			[Relationship("msdyn_taxcodedetail", EntityRole.Referenced, "user_msdyn_taxcodedetail", "owninguser")]
			public const string user_msdyn_taxcodedetail = "user_msdyn_taxcodedetail";
			[Relationship("msdyn_theme", EntityRole.Referenced, "user_msdyn_theme", "")]
			public const string user_msdyn_theme = "user_msdyn_theme";
			[Relationship("msdyn_timeentry", EntityRole.Referenced, "user_msdyn_timeentry", "owninguser")]
			public const string user_msdyn_timeentry = "user_msdyn_timeentry";
			[Relationship("msdyn_timegroup", EntityRole.Referenced, "user_msdyn_timegroup", "owninguser")]
			public const string user_msdyn_timegroup = "user_msdyn_timegroup";
			[Relationship("msdyn_timegroupdetail", EntityRole.Referenced, "user_msdyn_timegroupdetail", "owninguser")]
			public const string user_msdyn_timegroupdetail = "user_msdyn_timegroupdetail";
			[Relationship("msdyn_timeoffcalendar", EntityRole.Referenced, "user_msdyn_timeoffcalendar", "owninguser")]
			public const string user_msdyn_timeoffcalendar = "user_msdyn_timeoffcalendar";
			[Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "user_msdyn_timeoffrequest", "owninguser")]
			public const string user_msdyn_timeoffrequest = "user_msdyn_timeoffrequest";
			[Relationship("msdyn_transactionconnection", EntityRole.Referenced, "user_msdyn_transactionconnection", "owninguser")]
			public const string user_msdyn_transactionconnection = "user_msdyn_transactionconnection";
			[Relationship("msdyn_transactionorigin", EntityRole.Referenced, "user_msdyn_transactionorigin", "owninguser")]
			public const string user_msdyn_transactionorigin = "user_msdyn_transactionorigin";
			[Relationship("msdyn_untrackedappointment", EntityRole.Referenced, "user_msdyn_untrackedappointment", "owninguser")]
			public const string user_msdyn_untrackedappointment = "user_msdyn_untrackedappointment";
			[Relationship("msdyn_userworkhistory", EntityRole.Referenced, "user_msdyn_userworkhistory", "owninguser")]
			public const string user_msdyn_userworkhistory = "user_msdyn_userworkhistory";
			[Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "user_msdyn_wallsavedqueryusersettings", "owninguser")]
			public const string user_msdyn_wallsavedqueryusersettings = "user_msdyn_wallsavedqueryusersettings";
			[Relationship("msdyn_warehouse", EntityRole.Referenced, "user_msdyn_warehouse", "owninguser")]
			public const string user_msdyn_warehouse = "user_msdyn_warehouse";
			[Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "user_msdyn_workhourtemplate", "owninguser")]
			public const string user_msdyn_workhourtemplate = "user_msdyn_workhourtemplate";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "user_msdyn_workorder", "owninguser")]
			public const string user_msdyn_workorder = "user_msdyn_workorder";
			[Relationship("msdyn_workordercharacteristic", EntityRole.Referenced, "user_msdyn_workordercharacteristic", "owninguser")]
			public const string user_msdyn_workordercharacteristic = "user_msdyn_workordercharacteristic";
			[Relationship("msdyn_workorderincident", EntityRole.Referenced, "user_msdyn_workorderincident", "owninguser")]
			public const string user_msdyn_workorderincident = "user_msdyn_workorderincident";
			[Relationship("msdyn_workorderproduct", EntityRole.Referenced, "user_msdyn_workorderproduct", "owninguser")]
			public const string user_msdyn_workorderproduct = "user_msdyn_workorderproduct";
			[Relationship("msdyn_workorderresourcerestriction", EntityRole.Referenced, "user_msdyn_workorderresourcerestriction", "owninguser")]
			public const string user_msdyn_workorderresourcerestriction = "user_msdyn_workorderresourcerestriction";
			[Relationship("msdyn_workorderservice", EntityRole.Referenced, "user_msdyn_workorderservice", "owninguser")]
			public const string user_msdyn_workorderservice = "user_msdyn_workorderservice";
			[Relationship("msdyn_workorderservicetask", EntityRole.Referenced, "user_msdyn_workorderservicetask", "owninguser")]
			public const string user_msdyn_workorderservicetask = "user_msdyn_workorderservicetask";
			[Relationship("msdyn_workordersubstatus", EntityRole.Referenced, "user_msdyn_workordersubstatus", "owninguser")]
			public const string user_msdyn_workordersubstatus = "user_msdyn_workordersubstatus";
			[Relationship("msdyn_workordertype", EntityRole.Referenced, "user_msdyn_workordertype", "owninguser")]
			public const string user_msdyn_workordertype = "user_msdyn_workordertype";
			[Relationship("msfp_emailtemplate", EntityRole.Referenced, "user_msfp_emailtemplate", "owninguser")]
			public const string user_msfp_emailtemplate = "user_msfp_emailtemplate";
			[Relationship("msfp_question", EntityRole.Referenced, "user_msfp_question", "owninguser")]
			public const string user_msfp_question = "user_msfp_question";
			[Relationship("msfp_questionresponse", EntityRole.Referenced, "user_msfp_questionresponse", "owninguser")]
			public const string user_msfp_questionresponse = "user_msfp_questionresponse";
			[Relationship("msfp_survey", EntityRole.Referenced, "user_msfp_survey", "owninguser")]
			public const string user_msfp_survey = "user_msfp_survey";
			[Relationship("msfp_unsubscribedrecipient", EntityRole.Referenced, "user_msfp_unsubscribedrecipient", "owninguser")]
			public const string user_msfp_unsubscribedrecipient = "user_msfp_unsubscribedrecipient";
			[Relationship("opportunityclose", EntityRole.Referenced, "user_opportunityclose", "owninguser")]
			public const string user_opportunityclose = "user_opportunityclose";
			[Relationship("opportunityproduct", EntityRole.Referenced, "user_opportunityproduct", "owninguser")]
			public const string user_opportunityproduct = "user_opportunityproduct";
			[Relationship("orderclose", EntityRole.Referenced, "user_orderclose", "owninguser")]
			public const string user_orderclose = "user_orderclose";
			[Relationship("postfollow", EntityRole.Referenced, "user_owner_postfollows", "owninguser")]
			public const string user_owner_postfollows = "user_owner_postfollows";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referenced, "user_parent_user", "parentsystemuserid")]
			public const string user_parent_user = "user_parent_user";
			[Relationship("pchmcs_address", EntityRole.Referenced, "user_pchmcs_address", "")]
			public const string user_pchmcs_address = "user_pchmcs_address";
			[Relationship("pchmcs_admincontactbo", EntityRole.Referenced, "user_pchmcs_admincontactbo", "")]
			public const string user_pchmcs_admincontactbo = "user_pchmcs_admincontactbo";
			[Relationship("pchmcs_adresseemailgenerique", EntityRole.Referenced, "user_pchmcs_adresseemailgenerique", "")]
			public const string user_pchmcs_adresseemailgenerique = "user_pchmcs_adresseemailgenerique";
			[Relationship("pchmcs_affectation", EntityRole.Referenced, "user_pchmcs_affectation", "")]
			public const string user_pchmcs_affectation = "user_pchmcs_affectation";
			[Relationship("pchmcs_agence", EntityRole.Referenced, "user_pchmcs_agence", "")]
			public const string user_pchmcs_agence = "user_pchmcs_agence";
			[Relationship("pchmcs_aidevente", EntityRole.Referenced, "user_pchmcs_aidevente", "")]
			public const string user_pchmcs_aidevente = "user_pchmcs_aidevente";
			[Relationship("pchmcs_aideventeoffrecyclevente", EntityRole.Referenced, "user_pchmcs_aideventeoffrecyclevente", "")]
			public const string user_pchmcs_aideventeoffrecyclevente = "user_pchmcs_aideventeoffrecyclevente";
			[Relationship("pchmcs_allotement", EntityRole.Referenced, "user_pchmcs_allotement", "")]
			public const string user_pchmcs_allotement = "user_pchmcs_allotement";
			[Relationship("pchmcs_anciennegociateur", EntityRole.Referenced, "user_pchmcs_anciennegociateur", "")]
			public const string user_pchmcs_anciennegociateur = "user_pchmcs_anciennegociateur";
			[Relationship("pchmcs_appelfonds", EntityRole.Referenced, "user_pchmcs_appelfonds", "")]
			public const string user_pchmcs_appelfonds = "user_pchmcs_appelfonds";
			[Relationship("pchmcs_bilanpatrimonial", EntityRole.Referenced, "user_pchmcs_bilanpatrimonial", "")]
			public const string user_pchmcs_bilanpatrimonial = "user_pchmcs_bilanpatrimonial";
			[Relationship("pchmcs_bookmarks", EntityRole.Referenced, "user_pchmcs_bookmarks", "")]
			public const string user_pchmcs_bookmarks = "user_pchmcs_bookmarks";
			[Relationship("pchmcs_canal", EntityRole.Referenced, "user_pchmcs_canal", "")]
			public const string user_pchmcs_canal = "user_pchmcs_canal";
			[Relationship("pchmcs_cdvbuyer", EntityRole.Referenced, "user_pchmcs_cdvbuyer", "")]
			public const string user_pchmcs_cdvbuyer = "user_pchmcs_cdvbuyer";
			[Relationship("pchmcs_chargemensuelle", EntityRole.Referenced, "user_pchmcs_chargemensuelle", "")]
			public const string user_pchmcs_chargemensuelle = "user_pchmcs_chargemensuelle";
			[Relationship("pchmcs_commercialanimation", EntityRole.Referenced, "user_pchmcs_commercialanimation", "")]
			public const string user_pchmcs_commercialanimation = "user_pchmcs_commercialanimation";
			[Relationship("pchmcs_compositions", EntityRole.Referenced, "user_pchmcs_compositions", "")]
			public const string user_pchmcs_compositions = "user_pchmcs_compositions";
			[Relationship("pchmcs_connectionhierarchy", EntityRole.Referenced, "user_pchmcs_connectionhierarchy", "")]
			public const string user_pchmcs_connectionhierarchy = "user_pchmcs_connectionhierarchy";
			[Relationship("pchmcs_conseiller", EntityRole.Referenced, "user_pchmcs_conseiller", "")]
			public const string user_pchmcs_conseiller = "user_pchmcs_conseiller";
			[Relationship("pchmcs_credentialcache", EntityRole.Referenced, "user_pchmcs_credentialcache", "")]
			public const string user_pchmcs_credentialcache = "user_pchmcs_credentialcache";
			[Relationship("pchmcs_departement", EntityRole.Referenced, "user_pchmcs_departement", "")]
			public const string user_pchmcs_departement = "user_pchmcs_departement";
			[Relationship("pchmcs_dureerdv", EntityRole.Referenced, "user_pchmcs_dureerdv", "")]
			public const string user_pchmcs_dureerdv = "user_pchmcs_dureerdv";
			[Relationship("pchmcs_enfant", EntityRole.Referenced, "user_pchmcs_enfant", "")]
			public const string user_pchmcs_enfant = "user_pchmcs_enfant";
			[Relationship("pchmcs_etatavancement", EntityRole.Referenced, "user_pchmcs_etatavancement", "")]
			public const string user_pchmcs_etatavancement = "user_pchmcs_etatavancement";
			[Relationship("pchmcs_etatavancementprogramme", EntityRole.Referenced, "user_pchmcs_etatavancementprogramme", "")]
			public const string user_pchmcs_etatavancementprogramme = "user_pchmcs_etatavancementprogramme";
			[Relationship("pchmcs_favoriteprograms", EntityRole.Referenced, "user_pchmcs_favoriteprograms", "")]
			public const string user_pchmcs_favoriteprograms = "user_pchmcs_favoriteprograms";
			[Relationship("pchmcs_fonctionutilisateur", EntityRole.Referenced, "user_pchmcs_fonctionutilisateur", "")]
			public const string user_pchmcs_fonctionutilisateur = "user_pchmcs_fonctionutilisateur";
			[Relationship("pchmcs_historiquestatut", EntityRole.Referenced, "user_pchmcs_historiquestatut", "")]
			public const string user_pchmcs_historiquestatut = "user_pchmcs_historiquestatut";
			[Relationship("pchmcs_historiquestatutinteret", EntityRole.Referenced, "user_pchmcs_historiquestatutinteret", "")]
			public const string user_pchmcs_historiquestatutinteret = "user_pchmcs_historiquestatutinteret";
			[Relationship("pchmcs_interet", EntityRole.Referenced, "user_pchmcs_interet", "")]
			public const string user_pchmcs_interet = "user_pchmcs_interet";
			[Relationship("pchmcs_interfacesed", EntityRole.Referenced, "user_pchmcs_interfacesed", "")]
			public const string user_pchmcs_interfacesed = "user_pchmcs_interfacesed";
			[Relationship("pchmcs_intervenant", EntityRole.Referenced, "user_pchmcs_intervenant", "")]
			public const string user_pchmcs_intervenant = "user_pchmcs_intervenant";
			[Relationship("pchmcs_lieudevente", EntityRole.Referenced, "user_pchmcs_lieudevente", "")]
			public const string user_pchmcs_lieudevente = "user_pchmcs_lieudevente";
			[Relationship("pchmcs_logerreurkiamo", EntityRole.Referenced, "user_pchmcs_logerreurkiamo", "")]
			public const string user_pchmcs_logerreurkiamo = "user_pchmcs_logerreurkiamo";
			[Relationship("pchmcs_lot", EntityRole.Referenced, "user_pchmcs_lot", "")]
			public const string user_pchmcs_lot = "user_pchmcs_lot";
			[Relationship("pchmcs_lotconcern", EntityRole.Referenced, "user_pchmcs_lotconcern", "")]
			public const string user_pchmcs_lotconcern = "user_pchmcs_lotconcern";
			[Relationship("pchmcs_masterbusinessunit", EntityRole.Referenced, "user_pchmcs_masterbusinessunit", "")]
			public const string user_pchmcs_masterbusinessunit = "user_pchmcs_masterbusinessunit";
			[Relationship("pchmcs_matricecloturerdv", EntityRole.Referenced, "user_pchmcs_matricecloturerdv", "")]
			public const string user_pchmcs_matricecloturerdv = "user_pchmcs_matricecloturerdv";
			[Relationship("pchmcs_matriceremuneration", EntityRole.Referenced, "user_pchmcs_matriceremuneration", "")]
			public const string user_pchmcs_matriceremuneration = "user_pchmcs_matriceremuneration";
			[Relationship("pchmcs_modelesms", EntityRole.Referenced, "user_pchmcs_modelesms", "")]
			public const string user_pchmcs_modelesms = "user_pchmcs_modelesms";
			[Relationship("pchmcs_natureoffre", EntityRole.Referenced, "user_pchmcs_natureoffre", "")]
			public const string user_pchmcs_natureoffre = "user_pchmcs_natureoffre";
			[Relationship("pchmcs_notification", EntityRole.Referenced, "user_pchmcs_notification", "")]
			public const string user_pchmcs_notification = "user_pchmcs_notification";
			[Relationship("pchmcs_notifieduser", EntityRole.Referenced, "user_pchmcs_notifieduser", "")]
			public const string user_pchmcs_notifieduser = "user_pchmcs_notifieduser";
			[Relationship("pchmcs_offre", EntityRole.Referenced, "user_pchmcs_offre", "")]
			public const string user_pchmcs_offre = "user_pchmcs_offre";
			[Relationship("pchmcs_offresducyclesdevente", EntityRole.Referenced, "user_pchmcs_offresducyclesdevente", "")]
			public const string user_pchmcs_offresducyclesdevente = "user_pchmcs_offresducyclesdevente";
			[Relationship("pchmcs_origine", EntityRole.Referenced, "user_pchmcs_origine", "")]
			public const string user_pchmcs_origine = "user_pchmcs_origine";
			[Relationship("pchmcs_placement", EntityRole.Referenced, "user_pchmcs_placement", "")]
			public const string user_pchmcs_placement = "user_pchmcs_placement";
			[Relationship("pchmcs_populationeligible", EntityRole.Referenced, "user_pchmcs_populationeligible", "")]
			public const string user_pchmcs_populationeligible = "user_pchmcs_populationeligible";
			[Relationship("pchmcs_preferencesespperso", EntityRole.Referenced, "user_pchmcs_preferencesespperso", "")]
			public const string user_pchmcs_preferencesespperso = "user_pchmcs_preferencesespperso";
			[Relationship("pchmcs_preferencesnotificationsep", EntityRole.Referenced, "user_pchmcs_preferencesnotificationsep", "")]
			public const string user_pchmcs_preferencesnotificationsep = "user_pchmcs_preferencesnotificationsep";
			[Relationship("pchmcs_prescripteur", EntityRole.Referenced, "user_pchmcs_prescripteur", "")]
			public const string user_pchmcs_prescripteur = "user_pchmcs_prescripteur";
			[Relationship("pchmcs_programme", EntityRole.Referenced, "user_pchmcs_programme", "")]
			public const string user_pchmcs_programme = "user_pchmcs_programme";
			[Relationship("pchmcs_programmecdc", EntityRole.Referenced, "user_pchmcs_programmecdc", "")]
			public const string user_pchmcs_programmecdc = "user_pchmcs_programmecdc";
			[Relationship("pchmcs_programmesinteret", EntityRole.Referenced, "user_pchmcs_programmesinteret", "")]
			public const string user_pchmcs_programmesinteret = "user_pchmcs_programmesinteret";
			[Relationship("pchmcs_rdvcdc", EntityRole.Referenced, "user_pchmcs_rdvcdc", "")]
			public const string user_pchmcs_rdvcdc = "user_pchmcs_rdvcdc";
			[Relationship("pchmcs_rechercheoffre", EntityRole.Referenced, "user_pchmcs_rechercheoffre", "")]
			public const string user_pchmcs_rechercheoffre = "user_pchmcs_rechercheoffre";
			[Relationship("pchmcs_relationbancaire", EntityRole.Referenced, "user_pchmcs_relationbancaire", "")]
			public const string user_pchmcs_relationbancaire = "user_pchmcs_relationbancaire";
			[Relationship("pchmcs_remise", EntityRole.Referenced, "user_pchmcs_remise", "")]
			public const string user_pchmcs_remise = "user_pchmcs_remise";
			[Relationship("pchmcs_remuneration", EntityRole.Referenced, "user_pchmcs_remuneration", "")]
			public const string user_pchmcs_remuneration = "user_pchmcs_remuneration";
			[Relationship("pchmcs_remunerationprogramme", EntityRole.Referenced, "user_pchmcs_remunerationprogramme", "")]
			public const string user_pchmcs_remunerationprogramme = "user_pchmcs_remunerationprogramme";
			[Relationship("pchmcs_reseau", EntityRole.Referenced, "user_pchmcs_reseau", "")]
			public const string user_pchmcs_reseau = "user_pchmcs_reseau";
			[Relationship("pchmcs_sellerstatushistory", EntityRole.Referenced, "user_pchmcs_sellerstatushistory", "")]
			public const string user_pchmcs_sellerstatushistory = "user_pchmcs_sellerstatushistory";
			[Relationship("pchmcs_simulation", EntityRole.Referenced, "user_pchmcs_simulation", "")]
			public const string user_pchmcs_simulation = "user_pchmcs_simulation";
			[Relationship("pchmcs_situationimmobilire", EntityRole.Referenced, "user_pchmcs_situationimmobilire", "")]
			public const string user_pchmcs_situationimmobilire = "user_pchmcs_situationimmobilire";
			[Relationship("pchmcs_sousstatut", EntityRole.Referenced, "user_pchmcs_sousstatut", "")]
			public const string user_pchmcs_sousstatut = "user_pchmcs_sousstatut";
			[Relationship("pchmcs_statut", EntityRole.Referenced, "user_pchmcs_statut", "")]
			public const string user_pchmcs_statut = "user_pchmcs_statut";
			[Relationship("pchmcs_subjectmatrix", EntityRole.Referenced, "user_pchmcs_subjectmatrix", "")]
			public const string user_pchmcs_subjectmatrix = "user_pchmcs_subjectmatrix";
			[Relationship("pchmcs_syncoperation", EntityRole.Referenced, "user_pchmcs_syncoperation", "")]
			public const string user_pchmcs_syncoperation = "user_pchmcs_syncoperation";
			[Relationship("pchmcs_syncoperationdetail", EntityRole.Referenced, "user_pchmcs_syncoperationdetail", "")]
			public const string user_pchmcs_syncoperationdetail = "user_pchmcs_syncoperationdetail";
			[Relationship("pchmcs_systemfunction", EntityRole.Referenced, "user_pchmcs_systemfunction", "")]
			public const string user_pchmcs_systemfunction = "user_pchmcs_systemfunction";
			[Relationship("pchmcs_trancheimposition", EntityRole.Referenced, "user_pchmcs_trancheimposition", "")]
			public const string user_pchmcs_trancheimposition = "user_pchmcs_trancheimposition";
			[Relationship("pchmcs_typedelot", EntityRole.Referenced, "user_pchmcs_typedelot", "")]
			public const string user_pchmcs_typedelot = "user_pchmcs_typedelot";
			[Relationship("pchmcs_uniquecontact", EntityRole.Referenced, "user_pchmcs_uniquecontact", "")]
			public const string user_pchmcs_uniquecontact = "user_pchmcs_uniquecontact";
			[Relationship("pchmcs_vendeur", EntityRole.Referenced, "user_pchmcs_vendeur", "")]
			public const string user_pchmcs_vendeur = "user_pchmcs_vendeur";
			[Relationship("pchmcs_zonegeographique", EntityRole.Referenced, "user_pchmcs_zonegeographique", "")]
			public const string user_pchmcs_zonegeographique = "user_pchmcs_zonegeographique";
			[Relationship("phonecall", EntityRole.Referenced, "user_phonecall", "owninguser")]
			public const string user_phonecall = "user_phonecall";
			[Relationship("channelaccessprofilerule", EntityRole.Referenced, "user_profilerule", "owninguser")]
			public const string user_profilerule = "user_profilerule";
			[Relationship("quoteclose", EntityRole.Referenced, "user_quoteclose", "owninguser")]
			public const string user_quoteclose = "user_quoteclose";
			[Relationship("quotedetail", EntityRole.Referenced, "user_quotedetail", "owninguser")]
			public const string user_quotedetail = "user_quotedetail";
			[Relationship("ratingmodel", EntityRole.Referenced, "user_ratingmodel", "owninguser")]
			public const string user_ratingmodel = "user_ratingmodel";
			[Relationship("ratingvalue", EntityRole.Referenced, "user_ratingvalue", "owninguser")]
			public const string user_ratingvalue = "user_ratingvalue";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "user_recurringappointmentmaster", "owninguser")]
			public const string user_recurringappointmentmaster = "user_recurringappointmentmaster";
			[Relationship("routingrule", EntityRole.Referenced, "user_routingrule", "owninguser")]
			public const string user_routingrule = "user_routingrule";
			[Relationship("routingruleitem", EntityRole.Referenced, "user_routingruleitem", "assignobjectid")]
			public const string user_routingruleitem = "user_routingruleitem";
			[Relationship("salesorderdetail", EntityRole.Referenced, "user_salesorderdetail", "owninguser")]
			public const string user_salesorderdetail = "user_salesorderdetail";
			[Relationship("usersettings", EntityRole.Referenced, "user_settings", "systemuserid")]
			public const string user_settings = "user_settings";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "user_sharepointdocumentlocation", "owninguser")]
			public const string user_sharepointdocumentlocation = "user_sharepointdocumentlocation";
			[Relationship("sharepointsite", EntityRole.Referenced, "user_sharepointsite", "owninguser")]
			public const string user_sharepointsite = "user_sharepointsite";
			[Relationship("sla", EntityRole.Referenced, "user_slabase", "owninguser")]
			public const string user_slabase = "user_slabase";
			[Relationship("socialactivity", EntityRole.Referenced, "user_socialactivity", "owninguser")]
			public const string user_socialactivity = "user_socialactivity";
			[Relationship("task", EntityRole.Referenced, "user_task", "owninguser")]
			public const string user_task = "user_task";
			[Relationship("tit_smssetup", EntityRole.Referenced, "user_tit_smssetup", "")]
			public const string user_tit_smssetup = "user_tit_smssetup";
			[Relationship("untrackedemail", EntityRole.Referenced, "user_untrackedemail", "owninguser")]
			public const string user_untrackedemail = "user_untrackedemail";
			[Relationship("userapplicationmetadata", EntityRole.Referenced, "user_userapplicationmetadata", "owninguser")]
			public const string user_userapplicationmetadata = "user_userapplicationmetadata";
			[Relationship("userform", EntityRole.Referenced, "user_userform", "owninguser")]
			public const string user_userform = "user_userform";
			[Relationship("userquery", EntityRole.Referenced, "user_userquery", "owninguser")]
			public const string user_userquery = "user_userquery";
			[Relationship("userqueryvisualization", EntityRole.Referenced, "user_userqueryvisualizations", "owninguser")]
			public const string user_userqueryvisualizations = "user_userqueryvisualizations";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_owning_user", "owninguser")]
			public const string userentityinstancedata_owning_user = "userentityinstancedata_owning_user";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_systemuser", "objectid")]
			public const string userentityinstancedata_systemuser = "userentityinstancedata_systemuser";
			[Relationship("userentityuisettings", EntityRole.Referenced, "userentityuisettings_owning_user", "owninguser")]
			public const string userentityuisettings_owning_user = "userentityuisettings_owning_user";
			[Relationship("webresource", EntityRole.Referenced, "webresource_createdby", "createdby")]
			public const string webresource_createdby = "webresource_createdby";
			[Relationship("webresource", EntityRole.Referenced, "webresource_modifiedby", "modifiedby")]
			public const string webresource_modifiedby = "webresource_modifiedby";
			[Relationship("workflow", EntityRole.Referenced, "workflow_createdby", "createdby")]
			public const string workflow_createdby = "workflow_createdby";
			[Relationship("workflow", EntityRole.Referenced, "workflow_createdonbehalfby", "createdonbehalfby")]
			public const string workflow_createdonbehalfby = "workflow_createdonbehalfby";
			[Relationship("workflowdependency", EntityRole.Referenced, "workflow_dependency_createdby", "createdby")]
			public const string workflow_dependency_createdby = "workflow_dependency_createdby";
			[Relationship("workflowdependency", EntityRole.Referenced, "workflow_dependency_createdonbehalfby", "createdonbehalfby")]
			public const string workflow_dependency_createdonbehalfby = "workflow_dependency_createdonbehalfby";
			[Relationship("workflowdependency", EntityRole.Referenced, "workflow_dependency_modifiedby", "modifiedby")]
			public const string workflow_dependency_modifiedby = "workflow_dependency_modifiedby";
			[Relationship("workflowdependency", EntityRole.Referenced, "workflow_dependency_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string workflow_dependency_modifiedonbehalfby = "workflow_dependency_modifiedonbehalfby";
			[Relationship("workflow", EntityRole.Referenced, "workflow_modifiedby", "modifiedby")]
			public const string workflow_modifiedby = "workflow_modifiedby";
			[Relationship("workflow", EntityRole.Referenced, "workflow_modifiedonbehalfby", "modifiedonbehalfby")]
			public const string workflow_modifiedonbehalfby = "workflow_modifiedonbehalfby";
		}
	}
}
