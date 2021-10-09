using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Definitions.Internal
{
	[GeneratedCode("XrmFramework", "2.0")]
	[EntityDefinition]
	[ExcludeFromCodeCoverage]
	public static class BusinessUnitDefinition
	{
		public const string EntityName = "businessunit";
		public const string EntityCollectionName = "businessunits";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "businessunitid";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100)]
			public const string DivisionName = "divisionname";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[PrimaryAttribute(PrimaryAttributeType.Name)]
			[StringLength(160)]
			public const string Name = "name";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship("businessunit", EntityRole.Referencing, "parentbusinessunitid", "parentbusinessunitid")]
			public const string business_unit_parent_business_unit = "business_unit_parent_business_unit";
			[Relationship("calendar", EntityRole.Referencing, "calendarid", "calendarid")]
			public const string BusinessUnit_Calendar = "BusinessUnit_Calendar";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_businessunit_createdonbehalfby = "lk_businessunit_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_businessunit_modifiedonbehalfby = "lk_businessunit_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string lk_businessunitbase_createdby = "lk_businessunitbase_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_businessunitbase_modifiedby = "lk_businessunitbase_modifiedby";
			[Relationship("msdyn_warehouse", EntityRole.Referencing, "msdyn_warehouse", "msdyn_warehouse")]
			public const string msdyn_msdyn_warehouse_businessunit_Warehouse = "msdyn_msdyn_warehouse_businessunit_Warehouse";
			[Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
			public const string organization_business_units = "organization_business_units";
			[Relationship("transactioncurrency", EntityRole.Referencing, "transactioncurrencyid", "transactioncurrencyid")]
			public const string TransactionCurrency_BusinessUnit = "TransactionCurrency_BusinessUnit";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("actioncardusersettings", EntityRole.Referenced, "actioncardusersettings_businessunit", "owningbusinessunit")]
			public const string actioncardusersettings_businessunit = "actioncardusersettings_businessunit";
			[Relationship("businessunitmap", EntityRole.Referenced, "bizmap_businessid_businessunit", "businessid")]
			public const string bizmap_businessid_businessunit = "bizmap_businessid_businessunit";
			[Relationship("businessunitmap", EntityRole.Referenced, "bizmap_subbusinessid_businessunit", "subbusinessid")]
			public const string bizmap_subbusinessid_businessunit = "bizmap_subbusinessid_businessunit";
			[Relationship("bulkdeleteoperation", EntityRole.Referenced, "BulkDeleteOperation_BusinessUnit", "owningbusinessunit")]
			public const string BulkDeleteOperation_BusinessUnit = "BulkDeleteOperation_BusinessUnit";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "business_customer_opportunity_roles", "owningbusinessunit")]
			public const string business_customer_opportunity_roles = "business_customer_opportunity_roles";
			[Relationship("account", EntityRole.Referenced, "business_unit_accounts", "owningbusinessunit")]
			public const string business_unit_accounts = "business_unit_accounts";
			[Relationship("actioncard", EntityRole.Referenced, "business_unit_actioncards", "owningbusinessunit")]
			public const string business_unit_actioncards = "business_unit_actioncards";
			[Relationship("activitypointer", EntityRole.Referenced, "business_unit_activitypointer", "owningbusinessunit")]
			public const string business_unit_activitypointer = "business_unit_activitypointer";
			[Relationship("adminsettingsentity", EntityRole.Referenced, "business_unit_adminsettingsentity", "owningbusinessunit")]
			public const string business_unit_adminsettingsentity = "business_unit_adminsettingsentity";
			[Relationship("annotation", EntityRole.Referenced, "business_unit_annotations", "owningbusinessunit")]
			public const string business_unit_annotations = "business_unit_annotations";
			[Relationship("appointment", EntityRole.Referenced, "business_unit_appointment_activities", "owningbusinessunit")]
			public const string business_unit_appointment_activities = "business_unit_appointment_activities";
			[Relationship("asyncoperation", EntityRole.Referenced, "business_unit_asyncoperation", "owningbusinessunit")]
			public const string business_unit_asyncoperation = "business_unit_asyncoperation";
			[Relationship("bookableresource", EntityRole.Referenced, "business_unit_bookableresource", "owningbusinessunit")]
			public const string business_unit_bookableresource = "business_unit_bookableresource";
			[Relationship("bookableresourcebooking", EntityRole.Referenced, "business_unit_bookableresourcebooking", "owningbusinessunit")]
			public const string business_unit_bookableresourcebooking = "business_unit_bookableresourcebooking";
			[Relationship("bookableresourcebookingexchangesyncidmapping", EntityRole.Referenced, "business_unit_bookableresourcebookingexchangesyncidmapping", "owningbusinessunit")]
			public const string business_unit_bookableresourcebookingexchangesyncidmapping = "business_unit_bookableresourcebookingexchangesyncidmapping";
			[Relationship("bookableresourcebookingheader", EntityRole.Referenced, "business_unit_bookableresourcebookingheader", "owningbusinessunit")]
			public const string business_unit_bookableresourcebookingheader = "business_unit_bookableresourcebookingheader";
			[Relationship("bookableresourcecategory", EntityRole.Referenced, "business_unit_bookableresourcecategory", "owningbusinessunit")]
			public const string business_unit_bookableresourcecategory = "business_unit_bookableresourcecategory";
			[Relationship("bookableresourcecategoryassn", EntityRole.Referenced, "business_unit_bookableresourcecategoryassn", "owningbusinessunit")]
			public const string business_unit_bookableresourcecategoryassn = "business_unit_bookableresourcecategoryassn";
			[Relationship("bookableresourcecharacteristic", EntityRole.Referenced, "business_unit_bookableresourcecharacteristic", "owningbusinessunit")]
			public const string business_unit_bookableresourcecharacteristic = "business_unit_bookableresourcecharacteristic";
			[Relationship("bookableresourcegroup", EntityRole.Referenced, "business_unit_bookableresourcegroup", "owningbusinessunit")]
			public const string business_unit_bookableresourcegroup = "business_unit_bookableresourcegroup";
			[Relationship("bookingstatus", EntityRole.Referenced, "business_unit_bookingstatus", "owningbusinessunit")]
			public const string business_unit_bookingstatus = "business_unit_bookingstatus";
			[Relationship("bulkoperation", EntityRole.Referenced, "business_unit_BulkOperation_activities", "owningbusinessunit")]
			public const string business_unit_BulkOperation_activities = "business_unit_BulkOperation_activities";
			[Relationship("calendar", EntityRole.Referenced, "business_unit_calendars", "businessunitid")]
			public const string business_unit_calendars = "business_unit_calendars";
			[Relationship("campaignactivity", EntityRole.Referenced, "business_unit_campaignactivity_activities", "owningbusinessunit")]
			public const string business_unit_campaignactivity_activities = "business_unit_campaignactivity_activities";
			[Relationship("campaignresponse", EntityRole.Referenced, "business_unit_campaignresponse_activities", "owningbusinessunit")]
			public const string business_unit_campaignresponse_activities = "business_unit_campaignresponse_activities";
			[Relationship("category", EntityRole.Referenced, "business_unit_category", "owningbusinessunit")]
			public const string business_unit_category = "business_unit_category";
			[Relationship("cgo_intervention", EntityRole.Referenced, "business_unit_cgo_intervention", "owningbusinessunit")]
			public const string business_unit_cgo_intervention = "business_unit_cgo_intervention";
			[Relationship("cgo_servicecontract", EntityRole.Referenced, "business_unit_cgo_servicecontract", "owningbusinessunit")]
			public const string business_unit_cgo_servicecontract = "business_unit_cgo_servicecontract";
			[Relationship("cgo_testpowerapps", EntityRole.Referenced, "business_unit_cgo_testpowerapps", "owningbusinessunit")]
			public const string business_unit_cgo_testpowerapps = "business_unit_cgo_testpowerapps";
			[Relationship("cgo_testunmanaged", EntityRole.Referenced, "business_unit_cgo_testunmanaged", "owningbusinessunit")]
			public const string business_unit_cgo_testunmanaged = "business_unit_cgo_testunmanaged";
			[Relationship("channelaccessprofile", EntityRole.Referenced, "business_unit_channelaccessprofile", "owningbusinessunit")]
			public const string business_unit_channelaccessprofile = "business_unit_channelaccessprofile";
			[Relationship("characteristic", EntityRole.Referenced, "business_unit_characteristic", "owningbusinessunit")]
			public const string business_unit_characteristic = "business_unit_characteristic";
			[Relationship("connection", EntityRole.Referenced, "business_unit_connections", "owningbusinessunit")]
			public const string business_unit_connections = "business_unit_connections";
			[Relationship("connector", EntityRole.Referenced, "business_unit_connector", "owningbusinessunit")]
			public const string business_unit_connector = "business_unit_connector";
			[Relationship("constraintbasedgroup", EntityRole.Referenced, "business_unit_constraint_based_groups", "businessunitid")]
			public const string business_unit_constraint_based_groups = "business_unit_constraint_based_groups";
			[Relationship("contact", EntityRole.Referenced, "business_unit_contacts", "owningbusinessunit")]
			public const string business_unit_contacts = "business_unit_contacts";
			[Relationship("convertrule", EntityRole.Referenced, "business_unit_convertrule", "owningbusinessunit")]
			public const string business_unit_convertrule = "business_unit_convertrule";
			[Relationship("customerrelationship", EntityRole.Referenced, "business_unit_customer_relationship", "owningbusinessunit")]
			public const string business_unit_customer_relationship = "business_unit_customer_relationship";
			[Relationship("dynamicpropertyinstance", EntityRole.Referenced, "business_unit_dynamicproperyinstance", "owningbusinessunit")]
			public const string business_unit_dynamicproperyinstance = "business_unit_dynamicproperyinstance";
			[Relationship("email", EntityRole.Referenced, "business_unit_email_activities", "owningbusinessunit")]
			public const string business_unit_email_activities = "business_unit_email_activities";
			[Relationship("emailserverprofile", EntityRole.Referenced, "business_unit_emailserverprofile", "owningbusinessunit")]
			public const string business_unit_emailserverprofile = "business_unit_emailserverprofile";
			[Relationship("emailsignature", EntityRole.Referenced, "business_unit_emailsignatures", "owningbusinessunit")]
			public const string business_unit_emailsignatures = "business_unit_emailsignatures";
			[Relationship("entitlement", EntityRole.Referenced, "business_unit_entitlement", "owningbusinessunit")]
			public const string business_unit_entitlement = "business_unit_entitlement";
			[Relationship("entitlemententityallocationtypemapping", EntityRole.Referenced, "business_unit_entitlemententityallocationtypemapping", "owningbusinessunit")]
			public const string business_unit_entitlemententityallocationtypemapping = "business_unit_entitlemententityallocationtypemapping";
			[Relationship("entityanalyticsconfig", EntityRole.Referenced, "business_unit_entityanalyticsconfig", "owningbusinessunit")]
			public const string business_unit_entityanalyticsconfig = "business_unit_entityanalyticsconfig";
			[Relationship("equipment", EntityRole.Referenced, "business_unit_equipment", "businessunitid")]
			public const string business_unit_equipment = "business_unit_equipment";
			[Relationship("exchangesyncidmapping", EntityRole.Referenced, "business_unit_exchangesyncidmapping", "owningbusinessunit")]
			public const string business_unit_exchangesyncidmapping = "business_unit_exchangesyncidmapping";
			[Relationship("externalparty", EntityRole.Referenced, "business_unit_externalparty", "owningbusinessunit")]
			public const string business_unit_externalparty = "business_unit_externalparty";
			[Relationship("fax", EntityRole.Referenced, "business_unit_fax_activities", "owningbusinessunit")]
			public const string business_unit_fax_activities = "business_unit_fax_activities";
			[Relationship("feedback", EntityRole.Referenced, "business_unit_feedback", "owningbusinessunit")]
			public const string business_unit_feedback = "business_unit_feedback";
			[Relationship("goal", EntityRole.Referenced, "business_unit_goal", "owningbusinessunit")]
			public const string business_unit_goal = "business_unit_goal";
			[Relationship("goalrollupquery", EntityRole.Referenced, "business_unit_goalrollupquery", "owningbusinessunit")]
			public const string business_unit_goalrollupquery = "business_unit_goalrollupquery";
			[Relationship("incidentresolution", EntityRole.Referenced, "business_unit_incident_resolution_activities", "owningbusinessunit")]
			public const string business_unit_incident_resolution_activities = "business_unit_incident_resolution_activities";
			[Relationship("incident", EntityRole.Referenced, "business_unit_incidents", "owningbusinessunit")]
			public const string business_unit_incidents = "business_unit_incidents";
			[Relationship("interactionforemail", EntityRole.Referenced, "business_unit_new_interactionforemail", "owningbusinessunit")]
			public const string business_unit_interactionforemail = "business_unit_interactionforemail";
			[Relationship("invoice", EntityRole.Referenced, "business_unit_invoices", "owningbusinessunit")]
			public const string business_unit_invoices = "business_unit_invoices";
			[Relationship("knowledgearticle", EntityRole.Referenced, "business_unit_knowledgearticle", "owningbusinessunit")]
			public const string business_unit_knowledgearticle = "business_unit_knowledgearticle";
			[Relationship("lead", EntityRole.Referenced, "business_unit_leads", "owningbusinessunit")]
			public const string business_unit_leads = "business_unit_leads";
			[Relationship("letter", EntityRole.Referenced, "business_unit_letter_activities", "owningbusinessunit")]
			public const string business_unit_letter_activities = "business_unit_letter_activities";
			[Relationship("list", EntityRole.Referenced, "business_unit_list", "owningbusinessunit")]
			public const string business_unit_list = "business_unit_list";
			[Relationship("mailbox", EntityRole.Referenced, "business_unit_mailbox", "owningbusinessunit")]
			public const string business_unit_mailbox = "business_unit_mailbox";
			[Relationship("mailmergetemplate", EntityRole.Referenced, "business_unit_mailmergetemplates", "owningbusinessunit")]
			public const string business_unit_mailmergetemplates = "business_unit_mailmergetemplates";
			[Relationship("msdyn_accountpricelist", EntityRole.Referenced, "business_unit_msdyn_accountpricelist", "owningbusinessunit")]
			public const string business_unit_msdyn_accountpricelist = "business_unit_msdyn_accountpricelist";
			[Relationship("msdyn_actioncardregarding", EntityRole.Referenced, "business_unit_msdyn_actioncardregarding", "owningbusinessunit")]
			public const string business_unit_msdyn_actioncardregarding = "business_unit_msdyn_actioncardregarding";
			[Relationship("msdyn_actioncardrolesetting", EntityRole.Referenced, "business_unit_msdyn_actioncardrolesetting", "owningbusinessunit")]
			public const string business_unit_msdyn_actioncardrolesetting = "business_unit_msdyn_actioncardrolesetting";
			[Relationship("msdyn_actual", EntityRole.Referenced, "business_unit_msdyn_actual", "owningbusinessunit")]
			public const string business_unit_msdyn_actual = "business_unit_msdyn_actual";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "business_unit_msdyn_agreement", "owningbusinessunit")]
			public const string business_unit_msdyn_agreement = "business_unit_msdyn_agreement";
			[Relationship("msdyn_agreementbookingdate", EntityRole.Referenced, "business_unit_msdyn_agreementbookingdate", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementbookingdate = "business_unit_msdyn_agreementbookingdate";
			[Relationship("msdyn_agreementbookingincident", EntityRole.Referenced, "business_unit_msdyn_agreementbookingincident", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementbookingincident = "business_unit_msdyn_agreementbookingincident";
			[Relationship("msdyn_agreementbookingproduct", EntityRole.Referenced, "business_unit_msdyn_agreementbookingproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementbookingproduct = "business_unit_msdyn_agreementbookingproduct";
			[Relationship("msdyn_agreementbookingservice", EntityRole.Referenced, "business_unit_msdyn_agreementbookingservice", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementbookingservice = "business_unit_msdyn_agreementbookingservice";
			[Relationship("msdyn_agreementbookingservicetask", EntityRole.Referenced, "business_unit_msdyn_agreementbookingservicetask", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementbookingservicetask = "business_unit_msdyn_agreementbookingservicetask";
			[Relationship("msdyn_agreementbookingsetup", EntityRole.Referenced, "business_unit_msdyn_agreementbookingsetup", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementbookingsetup = "business_unit_msdyn_agreementbookingsetup";
			[Relationship("msdyn_agreementinvoicedate", EntityRole.Referenced, "business_unit_msdyn_agreementinvoicedate", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementinvoicedate = "business_unit_msdyn_agreementinvoicedate";
			[Relationship("msdyn_agreementinvoiceproduct", EntityRole.Referenced, "business_unit_msdyn_agreementinvoiceproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementinvoiceproduct = "business_unit_msdyn_agreementinvoiceproduct";
			[Relationship("msdyn_agreementinvoicesetup", EntityRole.Referenced, "business_unit_msdyn_agreementinvoicesetup", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementinvoicesetup = "business_unit_msdyn_agreementinvoicesetup";
			[Relationship("msdyn_agreementsubstatus", EntityRole.Referenced, "business_unit_msdyn_agreementsubstatus", "owningbusinessunit")]
			public const string business_unit_msdyn_agreementsubstatus = "business_unit_msdyn_agreementsubstatus";
			[Relationship("msdyn_aiconfiguration", EntityRole.Referenced, "business_unit_msdyn_aiconfiguration", "owningbusinessunit")]
			public const string business_unit_msdyn_aiconfiguration = "business_unit_msdyn_aiconfiguration";
			[Relationship("msdyn_aifptrainingdocument", EntityRole.Referenced, "business_unit_msdyn_aifptrainingdocument", "owningbusinessunit")]
			public const string business_unit_msdyn_aifptrainingdocument = "business_unit_msdyn_aifptrainingdocument";
			[Relationship("msdyn_aimodel", EntityRole.Referenced, "business_unit_msdyn_aimodel", "owningbusinessunit")]
			public const string business_unit_msdyn_aimodel = "business_unit_msdyn_aimodel";
			[Relationship("msdyn_aiodimage", EntityRole.Referenced, "business_unit_msdyn_aiodimage", "owningbusinessunit")]
			public const string business_unit_msdyn_aiodimage = "business_unit_msdyn_aiodimage";
			[Relationship("msdyn_aiodlabel", EntityRole.Referenced, "business_unit_msdyn_aiodlabel", "owningbusinessunit")]
			public const string business_unit_msdyn_aiodlabel = "business_unit_msdyn_aiodlabel";
			[Relationship("msdyn_aiodtrainingboundingbox", EntityRole.Referenced, "business_unit_msdyn_aiodtrainingboundingbox", "owningbusinessunit")]
			public const string business_unit_msdyn_aiodtrainingboundingbox = "business_unit_msdyn_aiodtrainingboundingbox";
			[Relationship("msdyn_aiodtrainingimage", EntityRole.Referenced, "business_unit_msdyn_aiodtrainingimage", "owningbusinessunit")]
			public const string business_unit_msdyn_aiodtrainingimage = "business_unit_msdyn_aiodtrainingimage";
			[Relationship("msdyn_aitemplate", EntityRole.Referenced, "business_unit_msdyn_aitemplate", "owningbusinessunit")]
			public const string business_unit_msdyn_aitemplate = "business_unit_msdyn_aitemplate";
			[Relationship("msdyn_analysiscomponent", EntityRole.Referenced, "business_unit_msdyn_analysiscomponent", "owningbusinessunit")]
			public const string business_unit_msdyn_analysiscomponent = "business_unit_msdyn_analysiscomponent";
			[Relationship("msdyn_analysisjob", EntityRole.Referenced, "business_unit_msdyn_analysisjob", "owningbusinessunit")]
			public const string business_unit_msdyn_analysisjob = "business_unit_msdyn_analysisjob";
			[Relationship("msdyn_analysisresult", EntityRole.Referenced, "business_unit_msdyn_analysisresult", "owningbusinessunit")]
			public const string business_unit_msdyn_analysisresult = "business_unit_msdyn_analysisresult";
			[Relationship("msdyn_analysisresultdetail", EntityRole.Referenced, "business_unit_msdyn_analysisresultdetail", "owningbusinessunit")]
			public const string business_unit_msdyn_analysisresultdetail = "business_unit_msdyn_analysisresultdetail";
			[Relationship("msdyn_bookingalertstatus", EntityRole.Referenced, "business_unit_msdyn_bookingalertstatus", "owningbusinessunit")]
			public const string business_unit_msdyn_bookingalertstatus = "business_unit_msdyn_bookingalertstatus";
			[Relationship("msdyn_bookingchange", EntityRole.Referenced, "business_unit_msdyn_bookingchange", "owningbusinessunit")]
			public const string business_unit_msdyn_bookingchange = "business_unit_msdyn_bookingchange";
			[Relationship("msdyn_bookingjournal", EntityRole.Referenced, "business_unit_msdyn_bookingjournal", "owningbusinessunit")]
			public const string business_unit_msdyn_bookingjournal = "business_unit_msdyn_bookingjournal";
			[Relationship("msdyn_bookingrule", EntityRole.Referenced, "business_unit_msdyn_bookingrule", "owningbusinessunit")]
			public const string business_unit_msdyn_bookingrule = "business_unit_msdyn_bookingrule";
			[Relationship("msdyn_bookingsetupmetadata", EntityRole.Referenced, "business_unit_msdyn_bookingsetupmetadata", "owningbusinessunit")]
			public const string business_unit_msdyn_bookingsetupmetadata = "business_unit_msdyn_bookingsetupmetadata";
			[Relationship("msdyn_bookingtimestamp", EntityRole.Referenced, "business_unit_msdyn_bookingtimestamp", "owningbusinessunit")]
			public const string business_unit_msdyn_bookingtimestamp = "business_unit_msdyn_bookingtimestamp";
			[Relationship("msdyn_callablecontext", EntityRole.Referenced, "business_unit_msdyn_callablecontext", "owningbusinessunit")]
			public const string business_unit_msdyn_callablecontext = "business_unit_msdyn_callablecontext";
			[Relationship("msdyn_characteristicreqforteammember", EntityRole.Referenced, "business_unit_msdyn_characteristicreqforteammember", "owningbusinessunit")]
			public const string business_unit_msdyn_characteristicreqforteammember = "business_unit_msdyn_characteristicreqforteammember";
			[Relationship("msdyn_connector", EntityRole.Referenced, "business_unit_msdyn_connector", "owningbusinessunit")]
			public const string business_unit_msdyn_connector = "business_unit_msdyn_connector";
			[Relationship("msdyn_contactpricelist", EntityRole.Referenced, "business_unit_msdyn_contactpricelist", "owningbusinessunit")]
			public const string business_unit_msdyn_contactpricelist = "business_unit_msdyn_contactpricelist";
			[Relationship("msdyn_contractlinescheduleofvalue", EntityRole.Referenced, "business_unit_msdyn_contractlinescheduleofvalue", "owningbusinessunit")]
			public const string business_unit_msdyn_contractlinescheduleofvalue = "business_unit_msdyn_contractlinescheduleofvalue";
			[Relationship("msdyn_customerasset", EntityRole.Referenced, "business_unit_msdyn_customerasset", "owningbusinessunit")]
			public const string business_unit_msdyn_customerasset = "business_unit_msdyn_customerasset";
			[Relationship("msdyn_dataexport", EntityRole.Referenced, "business_unit_msdyn_dataexport", "owningbusinessunit")]
			public const string business_unit_msdyn_dataexport = "business_unit_msdyn_dataexport";
			[Relationship("msdyn_delegation", EntityRole.Referenced, "business_unit_msdyn_delegation", "owningbusinessunit")]
			public const string business_unit_msdyn_delegation = "business_unit_msdyn_delegation";
			[Relationship("msdyn_entityrankingrule", EntityRole.Referenced, "business_unit_msdyn_entityrankingrule", "owningbusinessunit")]
			public const string business_unit_msdyn_entityrankingrule = "business_unit_msdyn_entityrankingrule";
			[Relationship("msdyn_estimate", EntityRole.Referenced, "business_unit_msdyn_estimate", "owningbusinessunit")]
			public const string business_unit_msdyn_estimate = "business_unit_msdyn_estimate";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "business_unit_msdyn_estimateline", "owningbusinessunit")]
			public const string business_unit_msdyn_estimateline = "business_unit_msdyn_estimateline";
			[Relationship("msdyn_expense", EntityRole.Referenced, "business_unit_msdyn_expense", "owningbusinessunit")]
			public const string business_unit_msdyn_expense = "business_unit_msdyn_expense";
			[Relationship("msdyn_expensereceipt", EntityRole.Referenced, "business_unit_msdyn_expensereceipt", "owningbusinessunit")]
			public const string business_unit_msdyn_expensereceipt = "business_unit_msdyn_expensereceipt";
			[Relationship("msdyn_fact", EntityRole.Referenced, "business_unit_msdyn_fact", "owningbusinessunit")]
			public const string business_unit_msdyn_fact = "business_unit_msdyn_fact";
			[Relationship("msdyn_fieldcomputation", EntityRole.Referenced, "business_unit_msdyn_fieldcomputation", "owningbusinessunit")]
			public const string business_unit_msdyn_fieldcomputation = "business_unit_msdyn_fieldcomputation";
			[Relationship("msdyn_fieldservicesetting", EntityRole.Referenced, "business_unit_msdyn_fieldservicesetting", "owningbusinessunit")]
			public const string business_unit_msdyn_fieldservicesetting = "business_unit_msdyn_fieldservicesetting";
			[Relationship("msdyn_findworkevent", EntityRole.Referenced, "business_unit_msdyn_findworkevent", "owningbusinessunit")]
			public const string business_unit_msdyn_findworkevent = "business_unit_msdyn_findworkevent";
			[Relationship("msdyn_flowcardtype", EntityRole.Referenced, "business_unit_msdyn_flowcardtype", "owningbusinessunit")]
			public const string business_unit_msdyn_flowcardtype = "business_unit_msdyn_flowcardtype";
			[Relationship("msdyn_forecastdefinition", EntityRole.Referenced, "business_unit_msdyn_forecastdefinition", "owningbusinessunit")]
			public const string business_unit_msdyn_forecastdefinition = "business_unit_msdyn_forecastdefinition";
			[Relationship("msdyn_forecastinstance", EntityRole.Referenced, "business_unit_msdyn_forecastinstance", "owningbusinessunit")]
			public const string business_unit_msdyn_forecastinstance = "business_unit_msdyn_forecastinstance";
			[Relationship("msdyn_forecastrecurrence", EntityRole.Referenced, "business_unit_msdyn_forecastrecurrence", "owningbusinessunit")]
			public const string business_unit_msdyn_forecastrecurrence = "business_unit_msdyn_forecastrecurrence";
			[Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "business_unit_msdyn_icebreakersconfig", "owningbusinessunit")]
			public const string business_unit_msdyn_icebreakersconfig = "business_unit_msdyn_icebreakersconfig";
			[Relationship("msdyn_incidenttype", EntityRole.Referenced, "business_unit_msdyn_incidenttype", "owningbusinessunit")]
			public const string business_unit_msdyn_incidenttype = "business_unit_msdyn_incidenttype";
			[Relationship("msdyn_incidenttypecharacteristic", EntityRole.Referenced, "business_unit_msdyn_incidenttypecharacteristic", "owningbusinessunit")]
			public const string business_unit_msdyn_incidenttypecharacteristic = "business_unit_msdyn_incidenttypecharacteristic";
			[Relationship("msdyn_incidenttypeproduct", EntityRole.Referenced, "business_unit_msdyn_incidenttypeproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_incidenttypeproduct = "business_unit_msdyn_incidenttypeproduct";
			[Relationship("msdyn_incidenttypeservice", EntityRole.Referenced, "business_unit_msdyn_incidenttypeservice", "owningbusinessunit")]
			public const string business_unit_msdyn_incidenttypeservice = "business_unit_msdyn_incidenttypeservice";
			[Relationship("msdyn_incidenttypeservicetask", EntityRole.Referenced, "business_unit_msdyn_incidenttypeservicetask", "owningbusinessunit")]
			public const string business_unit_msdyn_incidenttypeservicetask = "business_unit_msdyn_incidenttypeservicetask";
			[Relationship("msdyn_integrationjob", EntityRole.Referenced, "business_unit_msdyn_integrationjob", "owningbusinessunit")]
			public const string business_unit_msdyn_integrationjob = "business_unit_msdyn_integrationjob";
			[Relationship("msdyn_integrationjobdetail", EntityRole.Referenced, "business_unit_msdyn_integrationjobdetail", "owningbusinessunit")]
			public const string business_unit_msdyn_integrationjobdetail = "business_unit_msdyn_integrationjobdetail";
			[Relationship("msdyn_inventoryadjustment", EntityRole.Referenced, "business_unit_msdyn_inventoryadjustment", "owningbusinessunit")]
			public const string business_unit_msdyn_inventoryadjustment = "business_unit_msdyn_inventoryadjustment";
			[Relationship("msdyn_inventoryadjustmentproduct", EntityRole.Referenced, "business_unit_msdyn_inventoryadjustmentproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_inventoryadjustmentproduct = "business_unit_msdyn_inventoryadjustmentproduct";
			[Relationship("msdyn_inventoryjournal", EntityRole.Referenced, "business_unit_msdyn_inventoryjournal", "owningbusinessunit")]
			public const string business_unit_msdyn_inventoryjournal = "business_unit_msdyn_inventoryjournal";
			[Relationship("msdyn_inventorytransfer", EntityRole.Referenced, "business_unit_msdyn_inventorytransfer", "owningbusinessunit")]
			public const string business_unit_msdyn_inventorytransfer = "business_unit_msdyn_inventorytransfer";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "business_unit_msdyn_invoicelinetransaction", "owningbusinessunit")]
			public const string business_unit_msdyn_invoicelinetransaction = "business_unit_msdyn_invoicelinetransaction";
			[Relationship("msdyn_journal", EntityRole.Referenced, "business_unit_msdyn_journal", "owningbusinessunit")]
			public const string business_unit_msdyn_journal = "business_unit_msdyn_journal";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "business_unit_msdyn_journalline", "owningbusinessunit")]
			public const string business_unit_msdyn_journalline = "business_unit_msdyn_journalline";
			[Relationship("msdyn_knowledgearticletemplate", EntityRole.Referenced, "business_unit_msdyn_knowledgearticletemplate", "owningbusinessunit")]
			public const string business_unit_msdyn_knowledgearticletemplate = "business_unit_msdyn_knowledgearticletemplate";
			[Relationship("msdyn_msteamssetting", EntityRole.Referenced, "business_unit_msdyn_msteamssetting", "owningbusinessunit")]
			public const string business_unit_msdyn_msteamssetting = "business_unit_msdyn_msteamssetting";
			[Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "business_unit_msdyn_notesanalysisconfig", "owningbusinessunit")]
			public const string business_unit_msdyn_notesanalysisconfig = "business_unit_msdyn_notesanalysisconfig";
			[Relationship("msdyn_opportunitylineresourcecategory", EntityRole.Referenced, "business_unit_msdyn_opportunitylineresourcecategory", "owningbusinessunit")]
			public const string business_unit_msdyn_opportunitylineresourcecategory = "business_unit_msdyn_opportunitylineresourcecategory";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "business_unit_msdyn_opportunitylinetransaction", "owningbusinessunit")]
			public const string business_unit_msdyn_opportunitylinetransaction = "business_unit_msdyn_opportunitylinetransaction";
			[Relationship("msdyn_opportunitylinetransactioncategory", EntityRole.Referenced, "business_unit_msdyn_opportunitylinetransactioncategory", "owningbusinessunit")]
			public const string business_unit_msdyn_opportunitylinetransactioncategory = "business_unit_msdyn_opportunitylinetransactioncategory";
			[Relationship("msdyn_opportunitylinetransactionclassificatio", EntityRole.Referenced, "business_unit_msdyn_opportunitylinetransactionclassificatio", "owningbusinessunit")]
			public const string business_unit_msdyn_opportunitylinetransactionclassificatio = "business_unit_msdyn_opportunitylinetransactionclassificatio";
			[Relationship("msdyn_opportunitypricelist", EntityRole.Referenced, "business_unit_msdyn_opportunitypricelist", "owningbusinessunit")]
			public const string business_unit_msdyn_opportunitypricelist = "business_unit_msdyn_opportunitypricelist";
			[Relationship("msdyn_orderinvoicingdate", EntityRole.Referenced, "business_unit_msdyn_orderinvoicingdate", "owningbusinessunit")]
			public const string business_unit_msdyn_orderinvoicingdate = "business_unit_msdyn_orderinvoicingdate";
			[Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "business_unit_msdyn_orderinvoicingproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_orderinvoicingproduct = "business_unit_msdyn_orderinvoicingproduct";
			[Relationship("msdyn_orderinvoicingsetup", EntityRole.Referenced, "business_unit_msdyn_orderinvoicingsetup", "owningbusinessunit")]
			public const string business_unit_msdyn_orderinvoicingsetup = "business_unit_msdyn_orderinvoicingsetup";
			[Relationship("msdyn_orderinvoicingsetupdate", EntityRole.Referenced, "business_unit_msdyn_orderinvoicingsetupdate", "owningbusinessunit")]
			public const string business_unit_msdyn_orderinvoicingsetupdate = "business_unit_msdyn_orderinvoicingsetupdate";
			[Relationship("msdyn_orderlineresourcecategory", EntityRole.Referenced, "business_unit_msdyn_orderlineresourcecategory", "owningbusinessunit")]
			public const string business_unit_msdyn_orderlineresourcecategory = "business_unit_msdyn_orderlineresourcecategory";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "business_unit_msdyn_orderlinetransaction", "owningbusinessunit")]
			public const string business_unit_msdyn_orderlinetransaction = "business_unit_msdyn_orderlinetransaction";
			[Relationship("msdyn_orderlinetransactioncategory", EntityRole.Referenced, "business_unit_msdyn_orderlinetransactioncategory", "owningbusinessunit")]
			public const string business_unit_msdyn_orderlinetransactioncategory = "business_unit_msdyn_orderlinetransactioncategory";
			[Relationship("msdyn_orderlinetransactionclassification", EntityRole.Referenced, "business_unit_msdyn_orderlinetransactionclassification", "owningbusinessunit")]
			public const string business_unit_msdyn_orderlinetransactionclassification = "business_unit_msdyn_orderlinetransactionclassification";
			[Relationship("msdyn_orderpricelist", EntityRole.Referenced, "business_unit_msdyn_orderpricelist", "owningbusinessunit")]
			public const string business_unit_msdyn_orderpricelist = "business_unit_msdyn_orderpricelist";
			[Relationship("msdyn_payment", EntityRole.Referenced, "business_unit_msdyn_payment", "owningbusinessunit")]
			public const string business_unit_msdyn_payment = "business_unit_msdyn_payment";
			[Relationship("msdyn_paymentdetail", EntityRole.Referenced, "business_unit_msdyn_paymentdetail", "owningbusinessunit")]
			public const string business_unit_msdyn_paymentdetail = "business_unit_msdyn_paymentdetail";
			[Relationship("msdyn_paymentmethod", EntityRole.Referenced, "business_unit_msdyn_paymentmethod", "owningbusinessunit")]
			public const string business_unit_msdyn_paymentmethod = "business_unit_msdyn_paymentmethod";
			[Relationship("msdyn_paymentterm", EntityRole.Referenced, "business_unit_msdyn_paymentterm", "owningbusinessunit")]
			public const string business_unit_msdyn_paymentterm = "business_unit_msdyn_paymentterm";
			[Relationship("msdyn_playbookactivity", EntityRole.Referenced, "business_unit_msdyn_playbookactivity", "owningbusinessunit")]
			public const string business_unit_msdyn_playbookactivity = "business_unit_msdyn_playbookactivity";
			[Relationship("msdyn_playbookactivityattribute", EntityRole.Referenced, "business_unit_msdyn_playbookactivityattribute", "owningbusinessunit")]
			public const string business_unit_msdyn_playbookactivityattribute = "business_unit_msdyn_playbookactivityattribute";
			[Relationship("msdyn_playbookcategory", EntityRole.Referenced, "business_unit_msdyn_playbookcategory", "owningbusinessunit")]
			public const string business_unit_msdyn_playbookcategory = "business_unit_msdyn_playbookcategory";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "business_unit_msdyn_playbookinstance", "owningbusinessunit")]
			public const string business_unit_msdyn_playbookinstance = "business_unit_msdyn_playbookinstance";
			[Relationship("msdyn_playbooktemplate", EntityRole.Referenced, "business_unit_msdyn_playbooktemplate", "owningbusinessunit")]
			public const string business_unit_msdyn_playbooktemplate = "business_unit_msdyn_playbooktemplate";
			[Relationship("msdyn_postalbum", EntityRole.Referenced, "business_unit_msdyn_postalbum", "owningbusinessunit")]
			public const string business_unit_msdyn_postalbum = "business_unit_msdyn_postalbum";
			[Relationship("msdyn_postalcode", EntityRole.Referenced, "business_unit_msdyn_postalcode", "owningbusinessunit")]
			public const string business_unit_msdyn_postalcode = "business_unit_msdyn_postalcode";
			[Relationship("msdyn_priority", EntityRole.Referenced, "business_unit_msdyn_priority", "owningbusinessunit")]
			public const string business_unit_msdyn_priority = "business_unit_msdyn_priority";
			[Relationship("msdyn_project", EntityRole.Referenced, "business_unit_msdyn_project", "owningbusinessunit")]
			public const string business_unit_msdyn_project = "business_unit_msdyn_project";
			[Relationship("msdyn_projectapproval", EntityRole.Referenced, "business_unit_msdyn_projectapproval", "owningbusinessunit")]
			public const string business_unit_msdyn_projectapproval = "business_unit_msdyn_projectapproval";
			[Relationship("msdyn_projectpricelist", EntityRole.Referenced, "business_unit_msdyn_projectpricelist", "owningbusinessunit")]
			public const string business_unit_msdyn_projectpricelist = "business_unit_msdyn_projectpricelist";
			[Relationship("msdyn_projecttask", EntityRole.Referenced, "business_unit_msdyn_projecttask", "owningbusinessunit")]
			public const string business_unit_msdyn_projecttask = "business_unit_msdyn_projecttask";
			[Relationship("msdyn_projecttaskdependency", EntityRole.Referenced, "business_unit_msdyn_projecttaskdependency", "owningbusinessunit")]
			public const string business_unit_msdyn_projecttaskdependency = "business_unit_msdyn_projecttaskdependency";
			[Relationship("msdyn_projecttaskstatususer", EntityRole.Referenced, "business_unit_msdyn_projecttaskstatususer", "owningbusinessunit")]
			public const string business_unit_msdyn_projecttaskstatususer = "business_unit_msdyn_projecttaskstatususer";
			[Relationship("msdyn_projectteam", EntityRole.Referenced, "business_unit_msdyn_projectteam", "owningbusinessunit")]
			public const string business_unit_msdyn_projectteam = "business_unit_msdyn_projectteam";
			[Relationship("msdyn_projecttransactioncategory", EntityRole.Referenced, "business_unit_msdyn_projecttransactioncategory", "owningbusinessunit")]
			public const string business_unit_msdyn_projecttransactioncategory = "business_unit_msdyn_projecttransactioncategory";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "business_unit_msdyn_purchaseorder", "owningbusinessunit")]
			public const string business_unit_msdyn_purchaseorder = "business_unit_msdyn_purchaseorder";
			[Relationship("msdyn_purchaseorderbill", EntityRole.Referenced, "business_unit_msdyn_purchaseorderbill", "owningbusinessunit")]
			public const string business_unit_msdyn_purchaseorderbill = "business_unit_msdyn_purchaseorderbill";
			[Relationship("msdyn_purchaseorderproduct", EntityRole.Referenced, "business_unit_msdyn_purchaseorderproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_purchaseorderproduct = "business_unit_msdyn_purchaseorderproduct";
			[Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "business_unit_msdyn_purchaseorderreceipt", "owningbusinessunit")]
			public const string business_unit_msdyn_purchaseorderreceipt = "business_unit_msdyn_purchaseorderreceipt";
			[Relationship("msdyn_purchaseorderreceiptproduct", EntityRole.Referenced, "business_unit_msdyn_purchaseorderreceiptproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_purchaseorderreceiptproduct = "business_unit_msdyn_purchaseorderreceiptproduct";
			[Relationship("msdyn_purchaseordersubstatus", EntityRole.Referenced, "business_unit_msdyn_purchaseordersubstatus", "owningbusinessunit")]
			public const string business_unit_msdyn_purchaseordersubstatus = "business_unit_msdyn_purchaseordersubstatus";
			[Relationship("msdyn_quotebookingincident", EntityRole.Referenced, "business_unit_msdyn_quotebookingincident", "owningbusinessunit")]
			public const string business_unit_msdyn_quotebookingincident = "business_unit_msdyn_quotebookingincident";
			[Relationship("msdyn_quotebookingproduct", EntityRole.Referenced, "business_unit_msdyn_quotebookingproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_quotebookingproduct = "business_unit_msdyn_quotebookingproduct";
			[Relationship("msdyn_quotebookingservice", EntityRole.Referenced, "business_unit_msdyn_quotebookingservice", "owningbusinessunit")]
			public const string business_unit_msdyn_quotebookingservice = "business_unit_msdyn_quotebookingservice";
			[Relationship("msdyn_quotebookingservicetask", EntityRole.Referenced, "business_unit_msdyn_quotebookingservicetask", "owningbusinessunit")]
			public const string business_unit_msdyn_quotebookingservicetask = "business_unit_msdyn_quotebookingservicetask";
			[Relationship("msdyn_quotebookingsetup", EntityRole.Referenced, "business_unit_msdyn_quotebookingsetup", "owningbusinessunit")]
			public const string business_unit_msdyn_quotebookingsetup = "business_unit_msdyn_quotebookingsetup";
			[Relationship("msdyn_quoteinvoicingproduct", EntityRole.Referenced, "business_unit_msdyn_quoteinvoicingproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_quoteinvoicingproduct = "business_unit_msdyn_quoteinvoicingproduct";
			[Relationship("msdyn_quoteinvoicingsetup", EntityRole.Referenced, "business_unit_msdyn_quoteinvoicingsetup", "owningbusinessunit")]
			public const string business_unit_msdyn_quoteinvoicingsetup = "business_unit_msdyn_quoteinvoicingsetup";
			[Relationship("msdyn_quotelineanalyticsbreakdown", EntityRole.Referenced, "business_unit_msdyn_quotelineanalyticsbreakdown", "owningbusinessunit")]
			public const string business_unit_msdyn_quotelineanalyticsbreakdown = "business_unit_msdyn_quotelineanalyticsbreakdown";
			[Relationship("msdyn_quotelineresourcecategory", EntityRole.Referenced, "business_unit_msdyn_quotelineresourcecategory", "owningbusinessunit")]
			public const string business_unit_msdyn_quotelineresourcecategory = "business_unit_msdyn_quotelineresourcecategory";
			[Relationship("msdyn_quotelinescheduleofvalue", EntityRole.Referenced, "business_unit_msdyn_quotelinescheduleofvalue", "owningbusinessunit")]
			public const string business_unit_msdyn_quotelinescheduleofvalue = "business_unit_msdyn_quotelinescheduleofvalue";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "business_unit_msdyn_quotelinetransaction", "owningbusinessunit")]
			public const string business_unit_msdyn_quotelinetransaction = "business_unit_msdyn_quotelinetransaction";
			[Relationship("msdyn_quotelinetransactioncategory", EntityRole.Referenced, "business_unit_msdyn_quotelinetransactioncategory", "owningbusinessunit")]
			public const string business_unit_msdyn_quotelinetransactioncategory = "business_unit_msdyn_quotelinetransactioncategory";
			[Relationship("msdyn_quotelinetransactionclassification", EntityRole.Referenced, "business_unit_msdyn_quotelinetransactionclassification", "owningbusinessunit")]
			public const string business_unit_msdyn_quotelinetransactionclassification = "business_unit_msdyn_quotelinetransactionclassification";
			[Relationship("msdyn_quotepricelist", EntityRole.Referenced, "business_unit_msdyn_quotepricelist", "owningbusinessunit")]
			public const string business_unit_msdyn_quotepricelist = "business_unit_msdyn_quotepricelist";
			[Relationship("msdyn_relationshipinsightsunifiedconfig", EntityRole.Referenced, "business_unit_msdyn_relationshipinsightsunifiedconfig", "owningbusinessunit")]
			public const string business_unit_msdyn_relationshipinsightsunifiedconfig = "business_unit_msdyn_relationshipinsightsunifiedconfig";
			[Relationship("msdyn_requirementcharacteristic", EntityRole.Referenced, "business_unit_msdyn_requirementcharacteristic", "owningbusinessunit")]
			public const string business_unit_msdyn_requirementcharacteristic = "business_unit_msdyn_requirementcharacteristic";
			[Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "business_unit_msdyn_requirementorganizationunit", "owningbusinessunit")]
			public const string business_unit_msdyn_requirementorganizationunit = "business_unit_msdyn_requirementorganizationunit";
			[Relationship("msdyn_requirementresourcecategory", EntityRole.Referenced, "business_unit_msdyn_requirementresourcecategory", "owningbusinessunit")]
			public const string business_unit_msdyn_requirementresourcecategory = "business_unit_msdyn_requirementresourcecategory";
			[Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "business_unit_msdyn_requirementresourcepreference", "owningbusinessunit")]
			public const string business_unit_msdyn_requirementresourcepreference = "business_unit_msdyn_requirementresourcepreference";
			[Relationship("msdyn_requirementstatus", EntityRole.Referenced, "business_unit_msdyn_requirementstatus", "owningbusinessunit")]
			public const string business_unit_msdyn_requirementstatus = "business_unit_msdyn_requirementstatus";
			[Relationship("msdyn_resourceassignment", EntityRole.Referenced, "business_unit_msdyn_resourceassignment", "owningbusinessunit")]
			public const string business_unit_msdyn_resourceassignment = "business_unit_msdyn_resourceassignment";
			[Relationship("msdyn_resourceassignmentdetail", EntityRole.Referenced, "business_unit_msdyn_resourceassignmentdetail", "owningbusinessunit")]
			public const string business_unit_msdyn_resourceassignmentdetail = "business_unit_msdyn_resourceassignmentdetail";
			[Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "business_unit_msdyn_resourcepaytype", "owningbusinessunit")]
			public const string business_unit_msdyn_resourcepaytype = "business_unit_msdyn_resourcepaytype";
			[Relationship("msdyn_resourcerequest", EntityRole.Referenced, "business_unit_msdyn_resourcerequest", "owningbusinessunit")]
			public const string business_unit_msdyn_resourcerequest = "business_unit_msdyn_resourcerequest";
			[Relationship("msdyn_resourcerequirement", EntityRole.Referenced, "business_unit_msdyn_resourcerequirement", "owningbusinessunit")]
			public const string business_unit_msdyn_resourcerequirement = "business_unit_msdyn_resourcerequirement";
			[Relationship("msdyn_resourcerequirementdetail", EntityRole.Referenced, "business_unit_msdyn_resourcerequirementdetail", "owningbusinessunit")]
			public const string business_unit_msdyn_resourcerequirementdetail = "business_unit_msdyn_resourcerequirementdetail";
			[Relationship("msdyn_resourceterritory", EntityRole.Referenced, "business_unit_msdyn_resourceterritory", "owningbusinessunit")]
			public const string business_unit_msdyn_resourceterritory = "business_unit_msdyn_resourceterritory";
			[Relationship("msdyn_rma", EntityRole.Referenced, "business_unit_msdyn_rma", "owningbusinessunit")]
			public const string business_unit_msdyn_rma = "business_unit_msdyn_rma";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "business_unit_msdyn_rmaproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_rmaproduct = "business_unit_msdyn_rmaproduct";
			[Relationship("msdyn_rmareceipt", EntityRole.Referenced, "business_unit_msdyn_rmareceipt", "owningbusinessunit")]
			public const string business_unit_msdyn_rmareceipt = "business_unit_msdyn_rmareceipt";
			[Relationship("msdyn_rmareceiptproduct", EntityRole.Referenced, "business_unit_msdyn_rmareceiptproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_rmareceiptproduct = "business_unit_msdyn_rmareceiptproduct";
			[Relationship("msdyn_rmasubstatus", EntityRole.Referenced, "business_unit_msdyn_rmasubstatus", "owningbusinessunit")]
			public const string business_unit_msdyn_rmasubstatus = "business_unit_msdyn_rmasubstatus";
			[Relationship("msdyn_rolecompetencyrequirement", EntityRole.Referenced, "business_unit_msdyn_rolecompetencyrequirement", "owningbusinessunit")]
			public const string business_unit_msdyn_rolecompetencyrequirement = "business_unit_msdyn_rolecompetencyrequirement";
			[Relationship("msdyn_roleutilization", EntityRole.Referenced, "business_unit_msdyn_roleutilization", "owningbusinessunit")]
			public const string business_unit_msdyn_roleutilization = "business_unit_msdyn_roleutilization";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "business_unit_msdyn_rtv", "owningbusinessunit")]
			public const string business_unit_msdyn_rtv = "business_unit_msdyn_rtv";
			[Relationship("msdyn_rtvproduct", EntityRole.Referenced, "business_unit_msdyn_rtvproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_rtvproduct = "business_unit_msdyn_rtvproduct";
			[Relationship("msdyn_rtvsubstatus", EntityRole.Referenced, "business_unit_msdyn_rtvsubstatus", "owningbusinessunit")]
			public const string business_unit_msdyn_rtvsubstatus = "business_unit_msdyn_rtvsubstatus";
			[Relationship("msdyn_salesinsightssettings", EntityRole.Referenced, "business_unit_msdyn_salesinsightssettings", "owningbusinessunit")]
			public const string business_unit_msdyn_salesinsightssettings = "business_unit_msdyn_salesinsightssettings";
			[Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "business_unit_msdyn_scheduleboardsetting", "owningbusinessunit")]
			public const string business_unit_msdyn_scheduleboardsetting = "business_unit_msdyn_scheduleboardsetting";
			[Relationship("msdyn_servicetasktype", EntityRole.Referenced, "business_unit_msdyn_servicetasktype", "owningbusinessunit")]
			public const string business_unit_msdyn_servicetasktype = "business_unit_msdyn_servicetasktype";
			[Relationship("msdyn_shipvia", EntityRole.Referenced, "business_unit_msdyn_shipvia", "owningbusinessunit")]
			public const string business_unit_msdyn_shipvia = "business_unit_msdyn_shipvia";
			[Relationship("msdyn_siconfig", EntityRole.Referenced, "business_unit_msdyn_siconfig", "owningbusinessunit")]
			public const string business_unit_msdyn_siconfig = "business_unit_msdyn_siconfig";
			[Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "business_unit_msdyn_solutionhealthrule", "owningbusinessunit")]
			public const string business_unit_msdyn_solutionhealthrule = "business_unit_msdyn_solutionhealthrule";
			[Relationship("msdyn_solutionhealthruleargument", EntityRole.Referenced, "business_unit_msdyn_solutionhealthruleargument", "owningbusinessunit")]
			public const string business_unit_msdyn_solutionhealthruleargument = "business_unit_msdyn_solutionhealthruleargument";
			[Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "business_unit_msdyn_systemuserschedulersetting", "owningbusinessunit")]
			public const string business_unit_msdyn_systemuserschedulersetting = "business_unit_msdyn_systemuserschedulersetting";
			[Relationship("msdyn_taxcode", EntityRole.Referenced, "business_unit_msdyn_taxcode", "owningbusinessunit")]
			public const string business_unit_msdyn_taxcode = "business_unit_msdyn_taxcode";
			[Relationship("msdyn_taxcodedetail", EntityRole.Referenced, "business_unit_msdyn_taxcodedetail", "owningbusinessunit")]
			public const string business_unit_msdyn_taxcodedetail = "business_unit_msdyn_taxcodedetail";
			[Relationship("msdyn_timeentry", EntityRole.Referenced, "business_unit_msdyn_timeentry", "owningbusinessunit")]
			public const string business_unit_msdyn_timeentry = "business_unit_msdyn_timeentry";
			[Relationship("msdyn_timegroup", EntityRole.Referenced, "business_unit_msdyn_timegroup", "owningbusinessunit")]
			public const string business_unit_msdyn_timegroup = "business_unit_msdyn_timegroup";
			[Relationship("msdyn_timegroupdetail", EntityRole.Referenced, "business_unit_msdyn_timegroupdetail", "owningbusinessunit")]
			public const string business_unit_msdyn_timegroupdetail = "business_unit_msdyn_timegroupdetail";
			[Relationship("msdyn_timeoffcalendar", EntityRole.Referenced, "business_unit_msdyn_timeoffcalendar", "owningbusinessunit")]
			public const string business_unit_msdyn_timeoffcalendar = "business_unit_msdyn_timeoffcalendar";
			[Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "business_unit_msdyn_timeoffrequest", "owningbusinessunit")]
			public const string business_unit_msdyn_timeoffrequest = "business_unit_msdyn_timeoffrequest";
			[Relationship("msdyn_transactionconnection", EntityRole.Referenced, "business_unit_msdyn_transactionconnection", "owningbusinessunit")]
			public const string business_unit_msdyn_transactionconnection = "business_unit_msdyn_transactionconnection";
			[Relationship("msdyn_transactionorigin", EntityRole.Referenced, "business_unit_msdyn_transactionorigin", "owningbusinessunit")]
			public const string business_unit_msdyn_transactionorigin = "business_unit_msdyn_transactionorigin";
			[Relationship("msdyn_untrackedappointment", EntityRole.Referenced, "business_unit_msdyn_untrackedappointment", "owningbusinessunit")]
			public const string business_unit_msdyn_untrackedappointment = "business_unit_msdyn_untrackedappointment";
			[Relationship("msdyn_userworkhistory", EntityRole.Referenced, "business_unit_msdyn_userworkhistory", "owningbusinessunit")]
			public const string business_unit_msdyn_userworkhistory = "business_unit_msdyn_userworkhistory";
			[Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "business_unit_msdyn_wallsavedqueryusersettings", "owningbusinessunit")]
			public const string business_unit_msdyn_wallsavedqueryusersettings = "business_unit_msdyn_wallsavedqueryusersettings";
			[Relationship("msdyn_warehouse", EntityRole.Referenced, "business_unit_msdyn_warehouse", "owningbusinessunit")]
			public const string business_unit_msdyn_warehouse = "business_unit_msdyn_warehouse";
			[Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "business_unit_msdyn_workhourtemplate", "owningbusinessunit")]
			public const string business_unit_msdyn_workhourtemplate = "business_unit_msdyn_workhourtemplate";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "business_unit_msdyn_workorder", "owningbusinessunit")]
			public const string business_unit_msdyn_workorder = "business_unit_msdyn_workorder";
			[Relationship("msdyn_workordercharacteristic", EntityRole.Referenced, "business_unit_msdyn_workordercharacteristic", "owningbusinessunit")]
			public const string business_unit_msdyn_workordercharacteristic = "business_unit_msdyn_workordercharacteristic";
			[Relationship("msdyn_workorderincident", EntityRole.Referenced, "business_unit_msdyn_workorderincident", "owningbusinessunit")]
			public const string business_unit_msdyn_workorderincident = "business_unit_msdyn_workorderincident";
			[Relationship("msdyn_workorderproduct", EntityRole.Referenced, "business_unit_msdyn_workorderproduct", "owningbusinessunit")]
			public const string business_unit_msdyn_workorderproduct = "business_unit_msdyn_workorderproduct";
			[Relationship("msdyn_workorderresourcerestriction", EntityRole.Referenced, "business_unit_msdyn_workorderresourcerestriction", "owningbusinessunit")]
			public const string business_unit_msdyn_workorderresourcerestriction = "business_unit_msdyn_workorderresourcerestriction";
			[Relationship("msdyn_workorderservice", EntityRole.Referenced, "business_unit_msdyn_workorderservice", "owningbusinessunit")]
			public const string business_unit_msdyn_workorderservice = "business_unit_msdyn_workorderservice";
			[Relationship("msdyn_workorderservicetask", EntityRole.Referenced, "business_unit_msdyn_workorderservicetask", "owningbusinessunit")]
			public const string business_unit_msdyn_workorderservicetask = "business_unit_msdyn_workorderservicetask";
			[Relationship("msdyn_workordersubstatus", EntityRole.Referenced, "business_unit_msdyn_workordersubstatus", "owningbusinessunit")]
			public const string business_unit_msdyn_workordersubstatus = "business_unit_msdyn_workordersubstatus";
			[Relationship("msdyn_workordertype", EntityRole.Referenced, "business_unit_msdyn_workordertype", "owningbusinessunit")]
			public const string business_unit_msdyn_workordertype = "business_unit_msdyn_workordertype";
			[Relationship("msfp_emailtemplate", EntityRole.Referenced, "business_unit_msfp_emailtemplate", "owningbusinessunit")]
			public const string business_unit_msfp_emailtemplate = "business_unit_msfp_emailtemplate";
			[Relationship("msfp_question", EntityRole.Referenced, "business_unit_msfp_question", "owningbusinessunit")]
			public const string business_unit_msfp_question = "business_unit_msfp_question";
			[Relationship("msfp_questionresponse", EntityRole.Referenced, "business_unit_msfp_questionresponse", "owningbusinessunit")]
			public const string business_unit_msfp_questionresponse = "business_unit_msfp_questionresponse";
			[Relationship("msfp_survey", EntityRole.Referenced, "business_unit_msfp_survey", "owningbusinessunit")]
			public const string business_unit_msfp_survey = "business_unit_msfp_survey";
			[Relationship("msfp_unsubscribedrecipient", EntityRole.Referenced, "business_unit_msfp_unsubscribedrecipient", "owningbusinessunit")]
			public const string business_unit_msfp_unsubscribedrecipient = "business_unit_msfp_unsubscribedrecipient";
			[Relationship("opportunity", EntityRole.Referenced, "business_unit_opportunities", "owningbusinessunit")]
			public const string business_unit_opportunities = "business_unit_opportunities";
			[Relationship("opportunityclose", EntityRole.Referenced, "business_unit_opportunity_close_activities", "owningbusinessunit")]
			public const string business_unit_opportunity_close_activities = "business_unit_opportunity_close_activities";
			[Relationship("orderclose", EntityRole.Referenced, "business_unit_order_close_activities", "owningbusinessunit")]
			public const string business_unit_order_close_activities = "business_unit_order_close_activities";
			[Relationship("salesorder", EntityRole.Referenced, "business_unit_orders", "owningbusinessunit")]
			public const string business_unit_orders = "business_unit_orders";
			[Relationship("businessunit", EntityRole.Referenced, "business_unit_parent_business_unit", "parentbusinessunitid")]
			public const string business_unit_parent_business_unit = "business_unit_parent_business_unit";
			[Relationship("personaldocumenttemplate", EntityRole.Referenced, "business_unit_personaldocumenttemplates", "owningbusinessunit")]
			public const string business_unit_personaldocumenttemplates = "business_unit_personaldocumenttemplates";
			[Relationship("phonecall", EntityRole.Referenced, "business_unit_phone_call_activities", "owningbusinessunit")]
			public const string business_unit_phone_call_activities = "business_unit_phone_call_activities";
			[Relationship("postfollow", EntityRole.Referenced, "business_unit_postfollows", "owningbusinessunit")]
			public const string business_unit_postfollows = "business_unit_postfollows";
			[Relationship("postregarding", EntityRole.Referenced, "business_unit_PostRegarding", "regardingobjectowningbusinessunit")]
			public const string business_unit_PostRegarding = "business_unit_PostRegarding";
			[Relationship("channelaccessprofilerule", EntityRole.Referenced, "business_unit_profilerule", "owningbusinessunit")]
			public const string business_unit_profilerule = "business_unit_profilerule";
			[Relationship("queue", EntityRole.Referenced, "business_unit_queues", "businessunitid")]
			public const string business_unit_queues = "business_unit_queues";
			[Relationship("queue", EntityRole.Referenced, "business_unit_queues2", "owningbusinessunit")]
			public const string business_unit_queues2 = "business_unit_queues2";
			[Relationship("quoteclose", EntityRole.Referenced, "business_unit_quote_close_activities", "owningbusinessunit")]
			public const string business_unit_quote_close_activities = "business_unit_quote_close_activities";
			[Relationship("quote", EntityRole.Referenced, "business_unit_quotes", "owningbusinessunit")]
			public const string business_unit_quotes = "business_unit_quotes";
			[Relationship("ratingmodel", EntityRole.Referenced, "business_unit_ratingmodel", "owningbusinessunit")]
			public const string business_unit_ratingmodel = "business_unit_ratingmodel";
			[Relationship("ratingvalue", EntityRole.Referenced, "business_unit_ratingvalue", "owningbusinessunit")]
			public const string business_unit_ratingvalue = "business_unit_ratingvalue";
			[Relationship("recurrencerule", EntityRole.Referenced, "business_unit_recurrencerule", "owningbusinessunit")]
			public const string business_unit_recurrencerule = "business_unit_recurrencerule";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "business_unit_recurringappointmentmaster_activities", "owningbusinessunit")]
			public const string business_unit_recurringappointmentmaster_activities = "business_unit_recurringappointmentmaster_activities";
			[Relationship("report", EntityRole.Referenced, "business_unit_reports", "owningbusinessunit")]
			public const string business_unit_reports = "business_unit_reports";
			[Relationship("resourcegroup", EntityRole.Referenced, "business_unit_resource_groups", "businessunitid")]
			public const string business_unit_resource_groups = "business_unit_resource_groups";
			[Relationship("resourcespec", EntityRole.Referenced, "business_unit_resource_specs", "businessunitid")]
			public const string business_unit_resource_specs = "business_unit_resource_specs";
			[Relationship("resource", EntityRole.Referenced, "business_unit_resources", "businessunitid")]
			public const string business_unit_resources = "business_unit_resources";
			[Relationship(RoleDefinition.EntityName, EntityRole.Referenced, "business_unit_roles", "businessunitid")]
			public const string business_unit_roles = "business_unit_roles";
			[Relationship("routingrule", EntityRole.Referenced, "business_unit_routingrule", "owningbusinessunit")]
			public const string business_unit_routingrule = "business_unit_routingrule";
			[Relationship("sales_linkedin_profileassociations", EntityRole.Referenced, "business_unit_sales_linkedin_profileassociations", "owningbusinessunit")]
			public const string business_unit_sales_linkedin_profileassociations = "business_unit_sales_linkedin_profileassociations";
			[Relationship("salesprocessinstance", EntityRole.Referenced, "business_unit_salesprocessinstance", "businessunitid")]
			public const string business_unit_salesprocessinstance = "business_unit_salesprocessinstance";
			[Relationship("serviceappointment", EntityRole.Referenced, "business_unit_service_appointments", "owningbusinessunit")]
			public const string business_unit_service_appointments = "business_unit_service_appointments";
			[Relationship("contract", EntityRole.Referenced, "business_unit_service_contracts", "owningbusinessunit")]
			public const string business_unit_service_contracts = "business_unit_service_contracts";
			[Relationship("sharepointdocument", EntityRole.Referenced, "business_unit_sharepointdocument", "owningbusinessunit")]
			public const string business_unit_sharepointdocument = "business_unit_sharepointdocument";
			[Relationship("sharepointdocument", EntityRole.Referenced, "business_unit_sharepointdocument2", "businessunitid")]
			public const string business_unit_sharepointdocument2 = "business_unit_sharepointdocument2";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "business_unit_sharepointdocumentlocation", "owningbusinessunit")]
			public const string business_unit_sharepointdocumentlocation = "business_unit_sharepointdocumentlocation";
			[Relationship("sharepointsite", EntityRole.Referenced, "business_unit_sharepointsites", "owningbusinessunit")]
			public const string business_unit_sharepointsites = "business_unit_sharepointsites";
			[Relationship("sla", EntityRole.Referenced, "business_unit_slabase", "owningbusinessunit")]
			public const string business_unit_slabase = "business_unit_slabase";
			[Relationship("slakpiinstance", EntityRole.Referenced, "business_unit_slakpiinstance", "owningbusinessunit")]
			public const string business_unit_slakpiinstance = "business_unit_slakpiinstance";
			[Relationship("socialactivity", EntityRole.Referenced, "business_unit_socialactivity", "owningbusinessunit")]
			public const string business_unit_socialactivity = "business_unit_socialactivity";
			[Relationship("socialprofile", EntityRole.Referenced, "business_unit_socialprofiles", "owningbusinessunit")]
			public const string business_unit_socialprofiles = "business_unit_socialprofiles";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referenced, "business_unit_system_users", "businessunitid")]
			public const string business_unit_system_users = "business_unit_system_users";
			[Relationship("task", EntityRole.Referenced, "business_unit_task_activities", "owningbusinessunit")]
			public const string business_unit_task_activities = "business_unit_task_activities";
			[Relationship(TeamDefinition.EntityName, EntityRole.Referenced, "business_unit_teams", TeamDefinition.Columns.BusinessUnitId)]
			public const string business_unit_teams = "business_unit_teams";
			[Relationship("template", EntityRole.Referenced, "business_unit_templates", "owningbusinessunit")]
			public const string business_unit_templates = "business_unit_templates";
			[Relationship("tit_compteurparametrage", EntityRole.Referenced, "business_unit_tit_compteurparametrage", "")]
			public const string business_unit_tit_compteurparametrage = "business_unit_tit_compteurparametrage";
			[Relationship("tit_compteurrequests", EntityRole.Referenced, "business_unit_tit_compteurrequests", "")]
			public const string business_unit_tit_compteurrequests = "business_unit_tit_compteurrequests";
			[Relationship("tit_concurrentlocker", EntityRole.Referenced, "business_unit_tit_concurrentlocker", "")]
			public const string business_unit_tit_concurrentlocker = "business_unit_tit_concurrentlocker";
			[Relationship("tit_counter", EntityRole.Referenced, "business_unit_tit_counter", "")]
			public const string business_unit_tit_counter = "business_unit_tit_counter";
			[Relationship("tit_mappingfluxentrant", EntityRole.Referenced, "business_unit_tit_mappingfluxentrant", "")]
			public const string business_unit_tit_mappingfluxentrant = "business_unit_tit_mappingfluxentrant";
			[Relationship("tit_smssetup", EntityRole.Referenced, "business_unit_tit_smssetup", "")]
			public const string business_unit_tit_smssetup = "business_unit_tit_smssetup";
			[Relationship("traceregarding", EntityRole.Referenced, "business_unit_TraceRegarding", "regardingobjectowningbusinessunit")]
			public const string business_unit_TraceRegarding = "business_unit_TraceRegarding";
			[Relationship("untrackedemail", EntityRole.Referenced, "business_unit_untrackedemail_activities", "owningbusinessunit")]
			public const string business_unit_untrackedemail_activities = "business_unit_untrackedemail_activities";
			[Relationship("usersettings", EntityRole.Referenced, "business_unit_user_settings", "businessunitid")]
			public const string business_unit_user_settings = "business_unit_user_settings";
			[Relationship("userapplicationmetadata", EntityRole.Referenced, "business_unit_userapplicationmetadata", "owningbusinessunit")]
			public const string business_unit_userapplicationmetadata = "business_unit_userapplicationmetadata";
			[Relationship("userform", EntityRole.Referenced, "business_unit_userform", "owningbusinessunit")]
			public const string business_unit_userform = "business_unit_userform";
			[Relationship("userquery", EntityRole.Referenced, "business_unit_userquery", "owningbusinessunit")]
			public const string business_unit_userquery = "business_unit_userquery";
			[Relationship("userqueryvisualization", EntityRole.Referenced, "business_unit_userqueryvisualizations", "owningbusinessunit")]
			public const string business_unit_userqueryvisualizations = "business_unit_userqueryvisualizations";
			[Relationship("workflow", EntityRole.Referenced, "business_unit_workflow", "owningbusinessunit")]
			public const string business_unit_workflow = "business_unit_workflow";
			[Relationship("workflowlog", EntityRole.Referenced, "business_unit_workflowlogs", "owningbusinessunit")]
			public const string business_unit_workflowlogs = "business_unit_workflowlogs";
			[Relationship("asyncoperation", EntityRole.Referenced, "BusinessUnit_AsyncOperations", "regardingobjectid")]
			public const string BusinessUnit_AsyncOperations = "BusinessUnit_AsyncOperations";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "BusinessUnit_BulkDeleteFailures", "regardingobjectid")]
			public const string BusinessUnit_BulkDeleteFailures = "BusinessUnit_BulkDeleteFailures";
			[Relationship("callbackregistration", EntityRole.Referenced, "businessunit_callbackregistration", "owningbusinessunit")]
			public const string businessunit_callbackregistration = "businessunit_callbackregistration";
			[Relationship("campaign", EntityRole.Referenced, "BusinessUnit_Campaigns", "owningbusinessunit")]
			public const string BusinessUnit_Campaigns = "BusinessUnit_Campaigns";
			[Relationship("canvasapp", EntityRole.Referenced, "businessunit_canvasapp", "owningbusinessunit")]
			public const string businessunit_canvasapp = "businessunit_canvasapp";
			[Relationship("duplicaterule", EntityRole.Referenced, "BusinessUnit_DuplicateRules", "owningbusinessunit")]
			public const string BusinessUnit_DuplicateRules = "BusinessUnit_DuplicateRules";
			[Relationship("importdata", EntityRole.Referenced, "BusinessUnit_ImportData", "owningbusinessunit")]
			public const string BusinessUnit_ImportData = "BusinessUnit_ImportData";
			[Relationship("importfile", EntityRole.Referenced, "BusinessUnit_ImportFiles", "owningbusinessunit")]
			public const string BusinessUnit_ImportFiles = "BusinessUnit_ImportFiles";
			[Relationship("importlog", EntityRole.Referenced, "BusinessUnit_ImportLogs", "owningbusinessunit")]
			public const string BusinessUnit_ImportLogs = "BusinessUnit_ImportLogs";
			[Relationship("importmap", EntityRole.Referenced, "BusinessUnit_ImportMaps", "owningbusinessunit")]
			public const string BusinessUnit_ImportMaps = "BusinessUnit_ImportMaps";
			[Relationship("import", EntityRole.Referenced, "BusinessUnit_Imports", "owningbusinessunit")]
			public const string BusinessUnit_Imports = "BusinessUnit_Imports";
			[Relationship("internaladdress", EntityRole.Referenced, "businessunit_internal_addresses", "parentid")]
			public const string businessunit_internal_addresses = "businessunit_internal_addresses";
			[Relationship("mailboxtrackingcategory", EntityRole.Referenced, "businessunit_mailboxtrackingcategory", "owningbusinessunit")]
			public const string businessunit_mailboxtrackingcategory = "businessunit_mailboxtrackingcategory";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "businessunit_mailboxtrackingfolder", "owningbusinessunit")]
			public const string businessunit_mailboxtrackingfolder = "businessunit_mailboxtrackingfolder";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "businessunit_principalobjectattributeaccess", "objectid")]
			public const string businessunit_principalobjectattributeaccess = "businessunit_principalobjectattributeaccess";
			[Relationship("processsession", EntityRole.Referenced, "BusinessUnit_ProcessSessions", "regardingobjectid")]
			public const string BusinessUnit_ProcessSessions = "BusinessUnit_ProcessSessions";
			[Relationship("syncerror", EntityRole.Referenced, "BusinessUnit_SyncError", "owningbusinessunit")]
			public const string BusinessUnit_SyncError = "BusinessUnit_SyncError";
			[Relationship("syncerror", EntityRole.Referenced, "BusinessUnit_SyncErrors", "regardingobjectid")]
			public const string BusinessUnit_SyncErrors = "BusinessUnit_SyncErrors";
			[Relationship("li_inmail", EntityRole.Referenced, "li_inmail_businessunit_owningbusinessunit", "owningbusinessunit")]
			public const string li_inmail_businessunit_owningbusinessunit = "li_inmail_businessunit_owningbusinessunit";
			[Relationship("li_message", EntityRole.Referenced, "li_message_businessunit_owningbusinessunit", "owningbusinessunit")]
			public const string li_message_businessunit_owningbusinessunit = "li_message_businessunit_owningbusinessunit";
			[Relationship("li_pointdrivepresentationviewed", EntityRole.Referenced, "li_pointdrivepresentationviewed_businessunit_owningbusinessunit", "owningbusinessunit")]
			public const string li_pointdrivepresentationviewed_businessunit_owningbusinessunit = "li_pointdrivepresentationviewed_businessunit_owningbusinessunit";
			[Relationship("userfiscalcalendar", EntityRole.Referenced, "lk_userfiscalcalendar_businessunit", "businessunitid")]
			public const string lk_userfiscalcalendar_businessunit = "lk_userfiscalcalendar_businessunit";
			[Relationship("mbs_pluginprofile", EntityRole.Referenced, "mbs_businessunit_mbs_pluginprofile", "")]
			public const string mbs_businessunit_mbs_pluginprofile = "mbs_businessunit_mbs_pluginprofile";
			[Relationship("msdyn_approval", EntityRole.Referenced, "msdyn_approval_businessunit_owningbusinessunit", "owningbusinessunit")]
			public const string msdyn_approval_businessunit_owningbusinessunit = "msdyn_approval_businessunit_owningbusinessunit";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "msdyn_bookingalert_businessunit_owningbusinessunit", "owningbusinessunit")]
			public const string msdyn_bookingalert_businessunit_owningbusinessunit = "msdyn_bookingalert_businessunit_owningbusinessunit";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "msfp_surveyinvite_businessunit_owningbusinessunit", "owningbusinessunit")]
			public const string msfp_surveyinvite_businessunit_owningbusinessunit = "msfp_surveyinvite_businessunit_owningbusinessunit";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "msfp_surveyresponse_businessunit_owningbusinessunit", "owningbusinessunit")]
			public const string msfp_surveyresponse_businessunit_owningbusinessunit = "msfp_surveyresponse_businessunit_owningbusinessunit";
			[Relationship("processsession", EntityRole.Referenced, "Owning_businessunit_processsessions", "owningbusinessunit")]
			public const string Owning_businessunit_processsessions = "Owning_businessunit_processsessions";
			[Relationship("systemuserbusinessunitentitymap", EntityRole.Referenced, "systemuserbusinessunitentitymap_businessunitid_businessunit", "businessunitid")]
			public const string systemuserbusinessunitentitymap_businessunitid_businessunit = "systemuserbusinessunitentitymap_businessunitid_businessunit";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_businessunit", "owningbusinessunit")]
			public const string userentityinstancedata_businessunit = "userentityinstancedata_businessunit";
			[Relationship("userentityuisettings", EntityRole.Referenced, "userentityuisettings_businessunit", "owningbusinessunit")]
			public const string userentityuisettings_businessunit = "userentityuisettings_businessunit";
		}
	}
}
