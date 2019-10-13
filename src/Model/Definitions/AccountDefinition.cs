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

	public static class AccountDefinition
	{
		public const string EntityName = "account";
		public const string EntityCollectionName = "accounts";

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
			public const string Id = "accountid";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(80)]
			public const string Address1_City = "address1_city";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(80)]
			public const string Address1_Country = "address1_country";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(50)]
			public const string Address1_Fax = "address1_fax";

			/// <summary>
			/// 
			/// Type : Picklist (Address1FreightTerms)
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Picklist)]
			[OptionSet(typeof(Address1FreightTerms))]
			public const string Address1_FreightTermsCode = "address1_freighttermscode";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(250)]
			public const string Address1_Line1 = "address1_line1";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(250)]
			public const string Address1_Line2 = "address1_line2";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(250)]
			public const string Address1_Line3 = "address1_line3";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(20)]
			public const string Address1_PostalCode = "address1_postalcode";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(SystemUserDefinition.EntityName, SystemUserDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.lk_accountbase_createdby)]
			public const string CreatedBy = "createdby";

			/// <summary>
			/// 
			/// Type : Picklist (RelationshipType)
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Picklist)]
			[OptionSet(typeof(RelationshipType))]
			[Key(AlternateKeyNames.Test)]
			public const string CustomerTypeCode = "customertypecode";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[PrimaryAttribute(PrimaryAttributeType.Name)]
			[StringLength(160)]
			[Key(AlternateKeyNames.Test)]
			public const string Name = "name";

			/// <summary>
			/// 
			/// Type : Owner
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Owner)]
			[CrmLookup(SystemUserDefinition.EntityName, SystemUserDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.user_accounts)]
			[CrmLookup("team", "teamid", RelationshipName = "team_accounts")]
			public const string OwnerId = "ownerid";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(ContactDefinition.EntityName, ContactDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.account_primary_contact)]
			public const string PrimaryContactId = "primarycontactid";

			/// <summary>
			/// 
			/// Type : State (AccountState)
			/// Validity :  Read | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.State)]
			[OptionSet(typeof(AccountState))]
			public const string StateCode = "statecode";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class AlternateKeyNames
		{
			public const string Test = "cgo_test";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToManyRelationships
		{
			[Relationship("lead", EntityRole.Referencing, "accountleads", "leadid")]
			public const string accountleads_association = "accountleads_association";
			[Relationship("adx_contentaccesslevel", EntityRole.Referencing, "adx_accountcontentaccesslevel", "")]
			public const string adx_AccountContentAccessLevel = "adx_AccountContentAccessLevel";
			[Relationship("product", EntityRole.Referencing, "adx_accountproduct", "")]
			public const string adx_accountproduct = "adx_accountproduct";
			[Relationship("adx_webrole", EntityRole.Referencing, "adx_webrole_account", "")]
			public const string adx_webrole_account = "adx_webrole_account";
			[Relationship("adx_website", EntityRole.Referencing, "adx_website_sponsor", "")]
			public const string adx_website_sponsor = "adx_website_sponsor";
			[Relationship("bulkoperation", EntityRole.Referencing, "bulkoperationlog", "bulkoperationid")]
			public const string BulkOperation_Accounts = "BulkOperation_Accounts";
			[Relationship("campaignactivity", EntityRole.Referencing, "bulkoperationlog", "campaignactivityid")]
			public const string CampaignActivity_Accounts = "CampaignActivity_Accounts";
			[Relationship("list", EntityRole.Referencing, "listmember", "listid")]
			public const string listaccount_association = "listaccount_association";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "masterid", "masterid")]
			public const string account_master_account = "account_master_account";
			[Relationship("lead", EntityRole.Referencing, "originatingleadid", "originatingleadid")]
			public const string account_originating_lead = "account_originating_lead";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "parentaccountid", "parentaccountid")]
			public const string account_parent_account = "account_parent_account";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referencing, "primarycontactid", AccountDefinition.Columns.PrimaryContactId)]
			public const string account_primary_contact = "account_primary_contact";
			[Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
			public const string business_unit_accounts = "business_unit_accounts";
			[Relationship("equipment", EntityRole.Referencing, "preferredequipmentid", "preferredequipmentid")]
			public const string equipment_accounts = "equipment_accounts";
			[Relationship("imagedescriptor", EntityRole.Referencing, "entityimageid_imagedescriptor", "entityimageid")]
			public const string lk_account_entityimage = "lk_account_entityimage";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", AccountDefinition.Columns.CreatedBy)]
			public const string lk_accountbase_createdby = "lk_accountbase_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_accountbase_createdonbehalfby = "lk_accountbase_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_accountbase_modifiedby = "lk_accountbase_modifiedby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_accountbase_modifiedonbehalfby = "lk_accountbase_modifiedonbehalfby";
			[Relationship("externalparty", EntityRole.Referencing, "CreatedByExternalParty", "createdbyexternalparty")]
			public const string lk_externalparty_account_createdby = "lk_externalparty_account_createdby";
			[Relationship("externalparty", EntityRole.Referencing, "ModifiedByExternalParty", "modifiedbyexternalparty")]
			public const string lk_externalparty_account_modifiedby = "lk_externalparty_account_modifiedby";
			[Relationship("sla", EntityRole.Referencing, "sla_account_sla", "slaid")]
			public const string manualsla_account = "manualsla_account";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "msa_managingpartnerid", "")]
			public const string msa_account_managingpartner = "msa_account_managingpartner";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "msdyn_billingaccount_account", "msdyn_billingaccount")]
			public const string msdyn_account_account_BillingAccount = "msdyn_account_account_BillingAccount";
			[Relationship("bookableresource", EntityRole.Referencing, "msdyn_PreferredResource", "msdyn_preferredresource")]
			public const string msdyn_bookableresource_account_PreferredResource = "msdyn_bookableresource_account_PreferredResource";
			[Relationship("msdyn_taxcode", EntityRole.Referencing, "msdyn_salestaxcode", "msdyn_salestaxcode")]
			public const string msdyn_msdyn_taxcode_account_SalesTaxCode = "msdyn_msdyn_taxcode_account_SalesTaxCode";
			[Relationship("territory", EntityRole.Referencing, "msdyn_serviceterritory", "msdyn_serviceterritory")]
			public const string msdyn_territory_account_ServiceTerritory = "msdyn_territory_account_ServiceTerritory";
			[Relationship("owner", EntityRole.Referencing, "ownerid", AccountDefinition.Columns.OwnerId)]
			public const string owner_accounts = "owner_accounts";
			[Relationship("pricelevel", EntityRole.Referencing, "defaultpricelevelid", "defaultpricelevelid")]
			public const string price_level_accounts = "price_level_accounts";
			[Relationship("processstage", EntityRole.Referencing, "stageid_processstage", "stageid")]
			public const string processstage_account = "processstage_account";
			[Relationship("service", EntityRole.Referencing, "preferredserviceid", "preferredserviceid")]
			public const string service_accounts = "service_accounts";
			[Relationship("sla", EntityRole.Referencing, "slainvokedid_account_sla", "slainvokedid")]
			public const string sla_account = "sla_account";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "preferredsystemuserid", "preferredsystemuserid")]
			public const string system_user_accounts = "system_user_accounts";
			[Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
			public const string team_accounts = "team_accounts";
			[Relationship("territory", EntityRole.Referencing, "territoryid", "territoryid")]
			public const string territory_accounts = "territory_accounts";
			[Relationship("transactioncurrency", EntityRole.Referencing, "transactioncurrencyid", "transactioncurrencyid")]
			public const string transactioncurrency_account = "transactioncurrency_account";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
			public const string user_accounts = "user_accounts";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("actioncard", EntityRole.Referenced, "account_actioncard", "regardingobjectid")]
			public const string account_actioncard = "account_actioncard";
			[Relationship("activityparty", EntityRole.Referenced, "account_activity_parties", "partyid")]
			public const string account_activity_parties = "account_activity_parties";
			[Relationship("activitypointer", EntityRole.Referenced, "Account_ActivityPointers", "regardingobjectid")]
			public const string Account_ActivityPointers = "Account_ActivityPointers";
			[Relationship("adx_alertsubscription", EntityRole.Referenced, "account_adx_alertsubscriptions", "")]
			public const string account_adx_alertsubscriptions = "account_adx_alertsubscriptions";
			[Relationship("adx_inviteredemption", EntityRole.Referenced, "account_adx_inviteredemptions", "")]
			public const string account_adx_inviteredemptions = "account_adx_inviteredemptions";
			[Relationship("adx_portalcomment", EntityRole.Referenced, "account_adx_portalcomments", "")]
			public const string account_adx_portalcomments = "account_adx_portalcomments";
			[Relationship("annotation", EntityRole.Referenced, "Account_Annotation", "objectid")]
			public const string Account_Annotation = "Account_Annotation";
			[Relationship("appointment", EntityRole.Referenced, "Account_Appointments", "regardingobjectid")]
			public const string Account_Appointments = "Account_Appointments";
			[Relationship("asyncoperation", EntityRole.Referenced, "Account_AsyncOperations", "regardingobjectid")]
			public const string Account_AsyncOperations = "Account_AsyncOperations";
			[Relationship("bookableresource", EntityRole.Referenced, "account_bookableresource_AccountId", "accountid")]
			public const string account_bookableresource_AccountId = "account_bookableresource_AccountId";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "Account_BulkDeleteFailures", "regardingobjectid")]
			public const string Account_BulkDeleteFailures = "Account_BulkDeleteFailures";
			[Relationship("bulkoperation", EntityRole.Referenced, "account_BulkOperations", "regardingobjectid")]
			public const string account_BulkOperations = "account_BulkOperations";
			[Relationship("campaignresponse", EntityRole.Referenced, "account_CampaignResponses", "regardingobjectid")]
			public const string account_CampaignResponses = "account_CampaignResponses";
			[Relationship("connection", EntityRole.Referenced, "account_connections1", "record1id")]
			public const string account_connections1 = "account_connections1";
			[Relationship("connection", EntityRole.Referenced, "account_connections2", "record2id")]
			public const string account_connections2 = "account_connections2";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "account_customer_opportunity_roles", "customerid")]
			public const string account_customer_opportunity_roles = "account_customer_opportunity_roles";
			[Relationship("customerrelationship", EntityRole.Referenced, "account_customer_relationship_customer", "customerid")]
			public const string account_customer_relationship_customer = "account_customer_relationship_customer";
			[Relationship("customerrelationship", EntityRole.Referenced, "account_customer_relationship_partner", "partnerid")]
			public const string account_customer_relationship_partner = "account_customer_relationship_partner";
			[Relationship("customeraddress", EntityRole.Referenced, "Account_CustomerAddress", "parentid")]
			public const string Account_CustomerAddress = "Account_CustomerAddress";
			[Relationship("duplicaterecord", EntityRole.Referenced, "Account_DuplicateBaseRecord", "baserecordid")]
			public const string Account_DuplicateBaseRecord = "Account_DuplicateBaseRecord";
			[Relationship("duplicaterecord", EntityRole.Referenced, "Account_DuplicateMatchingRecord", "duplicaterecordid")]
			public const string Account_DuplicateMatchingRecord = "Account_DuplicateMatchingRecord";
			[Relationship("email", EntityRole.Referenced, "Account_Email_EmailSender", "emailsender")]
			public const string Account_Email_EmailSender = "Account_Email_EmailSender";
			[Relationship("email", EntityRole.Referenced, "Account_Email_SendersAccount", "sendersaccount")]
			public const string Account_Email_SendersAccount = "Account_Email_SendersAccount";
			[Relationship("email", EntityRole.Referenced, "Account_Emails", "regardingobjectid")]
			public const string Account_Emails = "Account_Emails";
			[Relationship("entitlement", EntityRole.Referenced, "account_entitlement_Account", "accountid")]
			public const string account_entitlement_Account = "account_entitlement_Account";
			[Relationship("entitlement", EntityRole.Referenced, "account_entitlement_Customer", "customerid")]
			public const string account_entitlement_Customer = "account_entitlement_Customer";
			[Relationship("fax", EntityRole.Referenced, "Account_Faxes", "regardingobjectid")]
			public const string Account_Faxes = "Account_Faxes";
			[Relationship("fileattachment", EntityRole.Referenced, "Account_FileAttachment", "objectid")]
			public const string Account_FileAttachment = "Account_FileAttachment";
			[Relationship("incidentresolution", EntityRole.Referenced, "account_IncidentResolutions", "regardingobjectid")]
			public const string account_IncidentResolutions = "account_IncidentResolutions";
			[Relationship("letter", EntityRole.Referenced, "Account_Letters", "regardingobjectid")]
			public const string Account_Letters = "Account_Letters";
			[Relationship("li_inmail", EntityRole.Referenced, "account_li_inmails", "regardingobjectid")]
			public const string account_li_inmails = "account_li_inmails";
			[Relationship("li_message", EntityRole.Referenced, "account_li_messages", "regardingobjectid")]
			public const string account_li_messages = "account_li_messages";
			[Relationship("li_pointdrivepresentationviewed", EntityRole.Referenced, "account_li_pointdrivepresentationvieweds", "regardingobjectid")]
			public const string account_li_pointdrivepresentationvieweds = "account_li_pointdrivepresentationvieweds";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "Account_MailboxTrackingFolder", "regardingobjectid")]
			public const string Account_MailboxTrackingFolder = "Account_MailboxTrackingFolder";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "account_master_account", "masterid")]
			public const string account_master_account = "account_master_account";
			[Relationship("msdyn_approval", EntityRole.Referenced, "account_msdyn_approvals", "regardingobjectid")]
			public const string account_msdyn_approvals = "account_msdyn_approvals";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "account_msdyn_bookingalerts", "regardingobjectid")]
			public const string account_msdyn_bookingalerts = "account_msdyn_bookingalerts";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "account_msdyn_surveyinvites", "")]
			public const string account_msdyn_surveyinvites = "account_msdyn_surveyinvites";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "account_msfp_surveyinvites", "regardingobjectid")]
			public const string account_msfp_surveyinvites = "account_msfp_surveyinvites";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "account_msfp_surveyresponses", "regardingobjectid")]
			public const string account_msfp_surveyresponses = "account_msfp_surveyresponses";
			[Relationship("opportunityclose", EntityRole.Referenced, "account_OpportunityCloses", "regardingobjectid")]
			public const string account_OpportunityCloses = "account_OpportunityCloses";
			[Relationship("orderclose", EntityRole.Referenced, "account_OrderCloses", "regardingobjectid")]
			public const string account_OrderCloses = "account_OrderCloses";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "account_parent_account", "parentaccountid")]
			public const string account_parent_account = "account_parent_account";
			[Relationship("phonecall", EntityRole.Referenced, "Account_Phonecalls", "regardingobjectid")]
			public const string Account_Phonecalls = "Account_Phonecalls";
			[Relationship("postfollow", EntityRole.Referenced, "account_PostFollows", "regardingobjectid")]
			public const string account_PostFollows = "account_PostFollows";
			[Relationship("postregarding", EntityRole.Referenced, "account_PostRegardings", "regardingobjectid")]
			public const string account_PostRegardings = "account_PostRegardings";
			[Relationship("postrole", EntityRole.Referenced, "account_PostRoles", "regardingobjectid")]
			public const string account_PostRoles = "account_PostRoles";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "account_principalobjectattributeaccess", "objectid")]
			public const string account_principalobjectattributeaccess = "account_principalobjectattributeaccess";
			[Relationship("processsession", EntityRole.Referenced, "Account_ProcessSessions", "regardingobjectid")]
			public const string Account_ProcessSessions = "Account_ProcessSessions";
			[Relationship("quoteclose", EntityRole.Referenced, "account_QuoteCloses", "regardingobjectid")]
			public const string account_QuoteCloses = "account_QuoteCloses";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "Account_RecurringAppointmentMasters", "regardingobjectid")]
			public const string Account_RecurringAppointmentMasters = "Account_RecurringAppointmentMasters";
			[Relationship("serviceappointment", EntityRole.Referenced, "Account_ServiceAppointments", "regardingobjectid")]
			public const string Account_ServiceAppointments = "Account_ServiceAppointments";
			[Relationship("sharepointdocument", EntityRole.Referenced, "Account_SharepointDocument", "regardingobjectid")]
			public const string Account_SharepointDocument = "Account_SharepointDocument";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "Account_SharepointDocumentLocation", "regardingobjectid")]
			public const string Account_SharepointDocumentLocation = "Account_SharepointDocumentLocation";
			[Relationship("socialactivity", EntityRole.Referenced, "Account_SocialActivities", "regardingobjectid")]
			public const string Account_SocialActivities = "Account_SocialActivities";
			[Relationship("syncerror", EntityRole.Referenced, "Account_SyncErrors", "regardingobjectid")]
			public const string Account_SyncErrors = "Account_SyncErrors";
			[Relationship("task", EntityRole.Referenced, "Account_Tasks", "regardingobjectid")]
			public const string Account_Tasks = "Account_Tasks";
			[Relationship("tit_sms", EntityRole.Referenced, "account_tit_smses", "")]
			public const string account_tit_smses = "account_tit_smses";
			[Relationship("adx_badge", EntityRole.Referenced, "adx_account_badge", "")]
			public const string adx_account_badge = "adx_account_badge";
			[Relationship("cgo_servicecontract", EntityRole.Referenced, "cgo_account_servicecontract", "cgo_accountid")]
			public const string cgo_account_servicecontract = "cgo_account_servicecontract";
			[Relationship("cgo_testpowerapps", EntityRole.Referenced, "cgo_testpowerapps_Account2Id_Account", "cgo_account2id")]
			public const string cgo_testpowerapps_Account2Id_Account = "cgo_testpowerapps_Account2Id_Account";
			[Relationship("cgo_testpowerapps", EntityRole.Referenced, "cgo_testpowerapps_AccountId_Account", "cgo_accountid")]
			public const string cgo_testpowerapps_AccountId_Account = "cgo_testpowerapps_AccountId_Account";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "contact_customer_accounts", ContactDefinition.Columns.ParentCustomerId)]
			public const string contact_customer_accounts = "contact_customer_accounts";
			[Relationship("contract", EntityRole.Referenced, "contract_billingcustomer_accounts", "billingcustomerid")]
			public const string contract_billingcustomer_accounts = "contract_billingcustomer_accounts";
			[Relationship("contract", EntityRole.Referenced, "contract_customer_accounts", "customerid")]
			public const string contract_customer_accounts = "contract_customer_accounts";
			[Relationship("contractdetail", EntityRole.Referenced, "contractlineitem_customer_accounts", "customerid")]
			public const string contractlineitem_customer_accounts = "contractlineitem_customer_accounts";
			[Relationship("bulkoperationlog", EntityRole.Referenced, "CreatedAccount_BulkOperationLogs2", "createdobjectid")]
			public const string CreatedAccount_BulkOperationLogs2 = "CreatedAccount_BulkOperationLogs2";
			[Relationship("incident", EntityRole.Referenced, "incident_customer_accounts", "customerid")]
			public const string incident_customer_accounts = "incident_customer_accounts";
			[Relationship("invoice", EntityRole.Referenced, "invoice_customer_accounts", "customerid")]
			public const string invoice_customer_accounts = "invoice_customer_accounts";
			[Relationship("lead", EntityRole.Referenced, "lead_customer_accounts", "customerid")]
			public const string lead_customer_accounts = "lead_customer_accounts";
			[Relationship("lead", EntityRole.Referenced, "lead_parent_account", "parentaccountid")]
			public const string lead_parent_account = "lead_parent_account";
			[Relationship("incident", EntityRole.Referenced, "msa_account_incident", "")]
			public const string msa_account_incident = "msa_account_incident";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "msa_account_managingpartner", "")]
			public const string msa_account_managingpartner = "msa_account_managingpartner";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "msa_contact_managingpartner", "")]
			public const string msa_contact_managingpartner = "msa_contact_managingpartner";
			[Relationship("opportunity", EntityRole.Referenced, "msa_partner_opportunity", "")]
			public const string msa_partner_opportunity = "msa_partner_opportunity";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "msdyn_account_account_BillingAccount", "msdyn_billingaccount")]
			public const string msdyn_account_account_BillingAccount = "msdyn_account_account_BillingAccount";
			[Relationship("msdyn_accountpricelist", EntityRole.Referenced, "msdyn_account_msdyn_accountpricelist_Account", "msdyn_account")]
			public const string msdyn_account_msdyn_accountpricelist_Account = "msdyn_account_msdyn_accountpricelist_Account";
			[Relationship("msdyn_actual", EntityRole.Referenced, "msdyn_account_msdyn_actual_AccountCustomer", "msdyn_accountcustomer")]
			public const string msdyn_account_msdyn_actual_AccountCustomer = "msdyn_account_msdyn_actual_AccountCustomer";
			[Relationship("msdyn_actual", EntityRole.Referenced, "msdyn_account_msdyn_actual_AccountVendor", "msdyn_accountvendor")]
			public const string msdyn_account_msdyn_actual_AccountVendor = "msdyn_account_msdyn_actual_AccountVendor";
			[Relationship("msdyn_actual", EntityRole.Referenced, "msdyn_account_msdyn_actual_ServiceAccount", "msdyn_serviceaccount")]
			public const string msdyn_account_msdyn_actual_ServiceAccount = "msdyn_account_msdyn_actual_ServiceAccount";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "msdyn_account_msdyn_agreement_BillingAccount", "msdyn_billingaccount")]
			public const string msdyn_account_msdyn_agreement_BillingAccount = "msdyn_account_msdyn_agreement_BillingAccount";
			[Relationship("msdyn_agreement", EntityRole.Referenced, "msdyn_account_msdyn_agreement_ServiceAccount", "msdyn_serviceaccount")]
			public const string msdyn_account_msdyn_agreement_ServiceAccount = "msdyn_account_msdyn_agreement_ServiceAccount";
			[Relationship("msdyn_customerasset", EntityRole.Referenced, "msdyn_account_msdyn_customerasset_Account", "msdyn_account")]
			public const string msdyn_account_msdyn_customerasset_Account = "msdyn_account_msdyn_customerasset_Account";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "msdyn_account_msdyn_estimateline_AccountCustomer", "msdyn_accountcustomer")]
			public const string msdyn_account_msdyn_estimateline_AccountCustomer = "msdyn_account_msdyn_estimateline_AccountCustomer";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "msdyn_account_msdyn_estimateline_AccountVendor", "msdyn_accountvendor")]
			public const string msdyn_account_msdyn_estimateline_AccountVendor = "msdyn_account_msdyn_estimateline_AccountVendor";
			[Relationship("msdyn_fact", EntityRole.Referenced, "msdyn_account_msdyn_fact_AccountCustomer", "msdyn_accountcustomer")]
			public const string msdyn_account_msdyn_fact_AccountCustomer = "msdyn_account_msdyn_fact_AccountCustomer";
			[Relationship("msdyn_fact", EntityRole.Referenced, "msdyn_account_msdyn_fact_AccountVendor", "msdyn_accountvendor")]
			public const string msdyn_account_msdyn_fact_AccountVendor = "msdyn_account_msdyn_fact_AccountVendor";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "msdyn_account_msdyn_invoicelinetransaction_AccountCustomer", "msdyn_accountcustomer")]
			public const string msdyn_account_msdyn_invoicelinetransaction_AccountCustomer = "msdyn_account_msdyn_invoicelinetransaction_AccountCustomer";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "msdyn_account_msdyn_invoicelinetransaction_AccountVendor", "msdyn_accountvendor")]
			public const string msdyn_account_msdyn_invoicelinetransaction_AccountVendor = "msdyn_account_msdyn_invoicelinetransaction_AccountVendor";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "msdyn_account_msdyn_journalline_AccountCustomer", "msdyn_accountcustomer")]
			public const string msdyn_account_msdyn_journalline_AccountCustomer = "msdyn_account_msdyn_journalline_AccountCustomer";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "msdyn_account_msdyn_journalline_AccountVendor", "msdyn_accountvendor")]
			public const string msdyn_account_msdyn_journalline_AccountVendor = "msdyn_account_msdyn_journalline_AccountVendor";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "msdyn_account_msdyn_opportunitylinetransaction_AccountCustomer", "msdyn_accountcustomer")]
			public const string msdyn_account_msdyn_opportunitylinetransaction_AccountCustomer = "msdyn_account_msdyn_opportunitylinetransaction_AccountCustomer";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "msdyn_account_msdyn_opportunitylinetransaction_AccountVendor", "msdyn_accountvendor")]
			public const string msdyn_account_msdyn_opportunitylinetransaction_AccountVendor = "msdyn_account_msdyn_opportunitylinetransaction_AccountVendor";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "msdyn_account_msdyn_orderlinetransaction_AccountCustomer", "msdyn_accountcustomer")]
			public const string msdyn_account_msdyn_orderlinetransaction_AccountCustomer = "msdyn_account_msdyn_orderlinetransaction_AccountCustomer";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "msdyn_account_msdyn_orderlinetransaction_AccountVendor", "msdyn_accountvendor")]
			public const string msdyn_account_msdyn_orderlinetransaction_AccountVendor = "msdyn_account_msdyn_orderlinetransaction_AccountVendor";
			[Relationship("msdyn_payment", EntityRole.Referenced, "msdyn_account_msdyn_payment_Account", "msdyn_account")]
			public const string msdyn_account_msdyn_payment_Account = "msdyn_account_msdyn_payment_Account";
			[Relationship("msdyn_project", EntityRole.Referenced, "msdyn_account_msdyn_project_Customer", "msdyn_customer")]
			public const string msdyn_account_msdyn_project_Customer = "msdyn_account_msdyn_project_Customer";
			[Relationship("msdyn_purchaseorder", EntityRole.Referenced, "msdyn_account_msdyn_purchaseorder_Vendor", "msdyn_vendor")]
			public const string msdyn_account_msdyn_purchaseorder_Vendor = "msdyn_account_msdyn_purchaseorder_Vendor";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "msdyn_account_msdyn_quotelinetransaction_AccountCustomer", "msdyn_accountcustomer")]
			public const string msdyn_account_msdyn_quotelinetransaction_AccountCustomer = "msdyn_account_msdyn_quotelinetransaction_AccountCustomer";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "msdyn_account_msdyn_quotelinetransaction_AccountVendor", "msdyn_accountvendor")]
			public const string msdyn_account_msdyn_quotelinetransaction_AccountVendor = "msdyn_account_msdyn_quotelinetransaction_AccountVendor";
			[Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "msdyn_account_msdyn_requirementresourcepreference_Account", "msdyn_account")]
			public const string msdyn_account_msdyn_requirementresourcepreference_Account = "msdyn_account_msdyn_requirementresourcepreference_Account";
			[Relationship("msdyn_responseoutcome", EntityRole.Referenced, "msdyn_account_msdyn_responseoutcome_Account", "")]
			public const string msdyn_account_msdyn_responseoutcome_Account = "msdyn_account_msdyn_responseoutcome_Account";
			[Relationship("msdyn_rma", EntityRole.Referenced, "msdyn_account_msdyn_rma_BillingAccount", "msdyn_billingaccount")]
			public const string msdyn_account_msdyn_rma_BillingAccount = "msdyn_account_msdyn_rma_BillingAccount";
			[Relationship("msdyn_rma", EntityRole.Referenced, "msdyn_account_msdyn_rma_ServiceAccount", "msdyn_serviceaccount")]
			public const string msdyn_account_msdyn_rma_ServiceAccount = "msdyn_account_msdyn_rma_ServiceAccount";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "msdyn_account_msdyn_rmaproduct_Changeownership", "msdyn_changeownership")]
			public const string msdyn_account_msdyn_rmaproduct_Changeownership = "msdyn_account_msdyn_rmaproduct_Changeownership";
			[Relationship("msdyn_rmaproduct", EntityRole.Referenced, "msdyn_account_msdyn_rmaproduct_ReturntoVendor", "msdyn_returntovendor")]
			public const string msdyn_account_msdyn_rmaproduct_ReturntoVendor = "msdyn_account_msdyn_rmaproduct_ReturntoVendor";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "msdyn_account_msdyn_rtv_Vendor", "msdyn_vendor")]
			public const string msdyn_account_msdyn_rtv_Vendor = "msdyn_account_msdyn_rtv_Vendor";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "msdyn_account_msdyn_surveyinvite_AccountId", "")]
			public const string msdyn_account_msdyn_surveyinvite_AccountId = "msdyn_account_msdyn_surveyinvite_AccountId";
			[Relationship("msdyn_surveyresponse", EntityRole.Referenced, "msdyn_account_msdyn_surveyresponse_Account", "")]
			public const string msdyn_account_msdyn_surveyresponse_Account = "msdyn_account_msdyn_surveyresponse_Account";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "msdyn_account_msdyn_workorder_BillingAccount", "msdyn_billingaccount")]
			public const string msdyn_account_msdyn_workorder_BillingAccount = "msdyn_account_msdyn_workorder_BillingAccount";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "msdyn_account_msdyn_workorder_ServiceAccount", "msdyn_serviceaccount")]
			public const string msdyn_account_msdyn_workorder_ServiceAccount = "msdyn_account_msdyn_workorder_ServiceAccount";
			[Relationship("msdyn_workorderresourcerestriction", EntityRole.Referenced, "msdyn_account_msdyn_workorderresourcerestriction_Account", "msdyn_account")]
			public const string msdyn_account_msdyn_workorderresourcerestriction_Account = "msdyn_account_msdyn_workorderresourcerestriction_Account";
			[Relationship("opportunityproduct", EntityRole.Referenced, "msdyn_account_opportunityproduct_ServiceAccount", "msdyn_serviceaccount")]
			public const string msdyn_account_opportunityproduct_ServiceAccount = "msdyn_account_opportunityproduct_ServiceAccount";
			[Relationship("product", EntityRole.Referenced, "msdyn_account_product_DefaultVendor", "msdyn_defaultvendor")]
			public const string msdyn_account_product_DefaultVendor = "msdyn_account_product_DefaultVendor";
			[Relationship("quote", EntityRole.Referenced, "msdyn_account_quote_Account", "msdyn_account")]
			public const string msdyn_account_quote_Account = "msdyn_account_quote_Account";
			[Relationship("quotedetail", EntityRole.Referenced, "msdyn_account_quotedetail_ServiceAccount", "msdyn_serviceaccount")]
			public const string msdyn_account_quotedetail_ServiceAccount = "msdyn_account_quotedetail_ServiceAccount";
			[Relationship("salesorder", EntityRole.Referenced, "msdyn_account_salesorder_Account", "msdyn_account")]
			public const string msdyn_account_salesorder_Account = "msdyn_account_salesorder_Account";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "msdyn_playbookinstance_account", "regarding")]
			public const string msdyn_playbookinstance_account = "msdyn_playbookinstance_account";
			[Relationship("opportunity", EntityRole.Referenced, "opportunity_customer_accounts", "customerid")]
			public const string opportunity_customer_accounts = "opportunity_customer_accounts";
			[Relationship("opportunity", EntityRole.Referenced, "opportunity_parent_account", "parentaccountid")]
			public const string opportunity_parent_account = "opportunity_parent_account";
			[Relationship("salesorder", EntityRole.Referenced, "order_customer_accounts", "customerid")]
			public const string order_customer_accounts = "order_customer_accounts";
			[Relationship("quote", EntityRole.Referenced, "quote_customer_accounts", "customerid")]
			public const string quote_customer_accounts = "quote_customer_accounts";
			[Relationship("slakpiinstance", EntityRole.Referenced, "slakpiinstance_account", "regarding")]
			public const string slakpiinstance_account = "slakpiinstance_account";
			[Relationship("socialactivity", EntityRole.Referenced, "SocialActivity_PostAuthor_accounts", "postauthor")]
			public const string SocialActivity_PostAuthor_accounts = "SocialActivity_PostAuthor_accounts";
			[Relationship("socialactivity", EntityRole.Referenced, "SocialActivity_PostAuthorAccount_accounts", "postauthoraccount")]
			public const string SocialActivity_PostAuthorAccount_accounts = "SocialActivity_PostAuthorAccount_accounts";
			[Relationship("socialprofile", EntityRole.Referenced, "Socialprofile_customer_accounts", "customerid")]
			public const string Socialprofile_customer_accounts = "Socialprofile_customer_accounts";
			[Relationship("bulkoperationlog", EntityRole.Referenced, "SourceAccount_BulkOperationLogs", "regardingobjectid")]
			public const string SourceAccount_BulkOperationLogs = "SourceAccount_BulkOperationLogs";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_account", "objectid")]
			public const string userentityinstancedata_account = "userentityinstancedata_account";
		}
	}
}
