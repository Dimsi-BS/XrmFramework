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

	public static class ContactDefinition
	{
		public const string EntityName = "contact";
		public const string EntityCollectionName = "contacts";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
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
			[StringLength(200)]
			public const string Address1_Name = "address1_name";

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
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(50)]
			public const string Address1_StateOrProvince = "address1_stateorprovince";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(80)]
			public const string Address3_City = "address3_city";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(80)]
			public const string Address3_Country = "address3_country";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(250)]
			public const string Address3_Line1 = "address3_line1";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(250)]
			public const string Address3_Line2 = "address3_line2";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(250)]
			public const string Address3_Line3 = "address3_line3";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(20)]
			public const string Address3_PostalCode = "address3_postalcode";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(50)]
			public const string Address3_StateOrProvince = "address3_stateorprovince";

			/// <summary>
			/// 
			/// Type : 
			/// Validity :  
			/// </summary>
			public const string Adx_identity_logonenabled = "adx_identity_logonenabled";

			/// <summary>
			/// 
			/// Type : DateTime
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.DateTime)]
			[DateTimeBehavior(DateTimeBehavior.DateOnly)]
			public const string BirthDate = "birthdate";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "contactid";

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
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100)]
			public const string EMailAddress1 = "emailaddress1";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(50)]
			public const string FirstName = "firstname";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[PrimaryAttribute(PrimaryAttributeType.Name)]
			[StringLength(160)]
			public const string FullName = "fullname";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(50)]
			public const string LastName = "lastname";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(50)]
			public const string MobilePhone = "mobilephone";

			/// <summary>
			/// 
			/// Type : DateTime
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.DateTime)]
			[DateTimeBehavior(DateTimeBehavior.UserLocal)]
			public const string ModifiedOn = "modifiedon";

			/// <summary>
			/// 
			/// Type : Integer
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Integer)]
			[Range(0, 1000000000)]
			public const string NumberOfChildren = "numberofchildren";

			/// <summary>
			/// 
			/// Type : Owner
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Owner)]
			[CrmLookup(SystemUserDefinition.EntityName, SystemUserDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.contact_owning_user)]
			[CrmLookup("team", "teamid", RelationshipName = "team_contacts")]
			public const string OwnerId = "ownerid";

			/// <summary>
			/// 
			/// Type : Customer
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Customer)]
			[CrmLookup(AccountDefinition.EntityName, AccountDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.contact_customer_accounts)]
			[CrmLookup(ContactDefinition.EntityName, ContactDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.contact_customer_contacts)]
			public const string ParentCustomerId = "parentcustomerid";

			/// <summary>
			/// 
			/// Type : State (ContactState)
			/// Validity :  Read | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.State)]
			[OptionSet(typeof(ContactState))]
			public const string StateCode = "statecode";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(50)]
			public const string Telephone1 = "telephone1";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(50)]
			public const string Telephone2 = "telephone2";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToManyRelationships
		{
			[Relationship("adx_communityforumthread", EntityRole.Referencing, "adx_communityforumthread_contact", "")]
			public const string adx_communityforumthread_contact = "adx_communityforumthread_contact";
			[Relationship("adx_contentaccesslevel", EntityRole.Referencing, "adx_contactcontentaccesslevel", "")]
			public const string adx_ContactContentAccessLevel = "adx_ContactContentAccessLevel";
			[Relationship("product", EntityRole.Referencing, "adx_contactproduct", "")]
			public const string adx_contactproduct = "adx_contactproduct";
			[Relationship("adx_communityforum", EntityRole.Referencing, "adx_forum_activityalert", "")]
			public const string adx_forum_activityalert = "adx_forum_activityalert";
			[Relationship("adx_webrole", EntityRole.Referencing, "adx_webrole_contact", "")]
			public const string adx_webrole_contact = "adx_webrole_contact";
			[Relationship("bulkoperation", EntityRole.Referencing, "bulkoperationlog", "bulkoperationid")]
			public const string BulkOperation_Contacts = "BulkOperation_Contacts";
			[Relationship("campaignactivity", EntityRole.Referencing, "bulkoperationlog", "campaignactivityid")]
			public const string CampaignActivity_Contacts = "CampaignActivity_Contacts";
			[Relationship("subscription", EntityRole.Referencing, "subscriptionmanuallytrackedobject", "subscriptionid")]
			public const string contact_subscription_association = "contact_subscription_association";
			[Relationship("invoice", EntityRole.Referencing, "contactinvoices", "invoiceid")]
			public const string contactinvoices_association = "contactinvoices_association";
			[Relationship("lead", EntityRole.Referencing, "contactleads", "leadid")]
			public const string contactleads_association = "contactleads_association";
			[Relationship("salesorder", EntityRole.Referencing, "contactorders", "salesorderid")]
			public const string contactorders_association = "contactorders_association";
			[Relationship("quote", EntityRole.Referencing, "contactquotes", "quoteid")]
			public const string contactquotes_association = "contactquotes_association";
			[Relationship("entitlement", EntityRole.Referencing, "entitlementcontacts", "entitlementid")]
			public const string entitlementcontacts_association = "entitlementcontacts_association";
			[Relationship("list", EntityRole.Referencing, "listmember", "listid")]
			public const string listcontact_association = "listcontact_association";
			[Relationship("contract", EntityRole.Referencing, "servicecontractcontacts", "contractid")]
			public const string servicecontractcontacts_association = "servicecontractcontacts_association";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship("adx_portallanguage", EntityRole.Referencing, "adx_preferredlanguageid", "")]
			public const string adx_portallanguage_contact = "adx_portallanguage_contact";
			[Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
			public const string business_unit_contacts = "business_unit_contacts";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "parentcustomerid_account", ContactDefinition.Columns.ParentCustomerId)]
			public const string contact_customer_accounts = "contact_customer_accounts";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referencing, "parentcustomerid_contact", ContactDefinition.Columns.ParentCustomerId)]
			public const string contact_customer_contacts = "contact_customer_contacts";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referencing, "masterid", "masterid")]
			public const string contact_master_contact = "contact_master_contact";
			[Relationship("lead", EntityRole.Referencing, "originatingleadid", "originatingleadid")]
			public const string contact_originating_lead = "contact_originating_lead";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
			public const string contact_owning_user = "contact_owning_user";
			[Relationship("equipment", EntityRole.Referencing, "preferredequipmentid", "preferredequipmentid")]
			public const string equipment_contacts = "equipment_contacts";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_contact_createdonbehalfby = "lk_contact_createdonbehalfby";
			[Relationship("imagedescriptor", EntityRole.Referencing, "entityimageid_imagedescriptor", "entityimageid")]
			public const string lk_contact_entityimage = "lk_contact_entityimage";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_contact_modifiedonbehalfby = "lk_contact_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string lk_contactbase_createdby = "lk_contactbase_createdby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string lk_contactbase_modifiedby = "lk_contactbase_modifiedby";
			[Relationship("externalparty", EntityRole.Referencing, "CreatedByExternalParty", "createdbyexternalparty")]
			public const string lk_externalparty_contact_createdby = "lk_externalparty_contact_createdby";
			[Relationship("externalparty", EntityRole.Referencing, "ModifiedByExternalParty", "modifiedbyexternalparty")]
			public const string lk_externalparty_contact_modifiedby = "lk_externalparty_contact_modifiedby";
			[Relationship("sla", EntityRole.Referencing, "sla_contact_sla", "slaid")]
			public const string manualsla_contact = "manualsla_contact";
			[Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "msa_managingpartnerid", "")]
			public const string msa_contact_managingpartner = "msa_contact_managingpartner";
			[Relationship("owner", EntityRole.Referencing, "ownerid", ContactDefinition.Columns.OwnerId)]
			public const string owner_contacts = "owner_contacts";
			[Relationship("pricelevel", EntityRole.Referencing, "defaultpricelevelid", "defaultpricelevelid")]
			public const string price_level_contacts = "price_level_contacts";
			[Relationship("processstage", EntityRole.Referencing, "stageid_processstage", "stageid")]
			public const string processstage_contact = "processstage_contact";
			[Relationship("service", EntityRole.Referencing, "preferredserviceid", "preferredserviceid")]
			public const string service_contacts = "service_contacts";
			[Relationship("sla", EntityRole.Referencing, "slainvokedid_contact_sla", "slainvokedid")]
			public const string sla_contact = "sla_contact";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "preferredsystemuserid", "preferredsystemuserid")]
			public const string system_user_contacts = "system_user_contacts";
			[Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
			public const string team_contacts = "team_contacts";
			[Relationship("transactioncurrency", EntityRole.Referencing, "transactioncurrencyid", "transactioncurrencyid")]
			public const string transactioncurrency_contact = "transactioncurrency_contact";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "account_primary_contact", AccountDefinition.Columns.PrimaryContactId)]
			public const string account_primary_contact = "account_primary_contact";
			[Relationship("adx_webpagehistory", EntityRole.Referenced, "adx_changedcontact_webpagehistory", "")]
			public const string adx_changedcontact_webpagehistory = "adx_changedcontact_webpagehistory";
			[Relationship("adx_casedeflection", EntityRole.Referenced, "adx_contact_adx_casedeflection_Contact", "")]
			public const string adx_contact_adx_casedeflection_Contact = "adx_contact_adx_casedeflection_Contact";
			[Relationship("adx_badge", EntityRole.Referenced, "adx_contact_badge", "")]
			public const string adx_contact_badge = "adx_contact_badge";
			[Relationship("adx_blogpost", EntityRole.Referenced, "adx_contact_blogpost", "")]
			public const string adx_contact_blogpost = "adx_contact_blogpost";
			[Relationship("adx_communityforumalert", EntityRole.Referenced, "adx_contact_communityforumalert", "")]
			public const string adx_contact_communityforumalert = "adx_contact_communityforumalert";
			[Relationship("adx_communityforumannouncement", EntityRole.Referenced, "adx_contact_communityforumannouncement", "")]
			public const string adx_contact_communityforumannouncement = "adx_contact_communityforumannouncement";
			[Relationship("adx_communityforumpost", EntityRole.Referenced, "adx_contact_communityforumpost", "")]
			public const string adx_contact_communityforumpost = "adx_contact_communityforumpost";
			[Relationship("adx_externalidentity", EntityRole.Referenced, "adx_contact_externalidentity", "")]
			public const string adx_contact_externalidentity = "adx_contact_externalidentity";
			[Relationship("adx_forumnotification", EntityRole.Referenced, "adx_contact_forumnotification", "")]
			public const string adx_contact_forumnotification = "adx_contact_forumnotification";
			[Relationship("adx_idea", EntityRole.Referenced, "adx_contact_idea", "")]
			public const string adx_contact_idea = "adx_contact_idea";
			[Relationship("adx_idea", EntityRole.Referenced, "adx_contact_idea_statusauthor", "")]
			public const string adx_contact_idea_statusauthor = "adx_contact_idea_statusauthor";
			[Relationship("adx_pagenotification", EntityRole.Referenced, "adx_contact_pagenotification", "")]
			public const string adx_contact_pagenotification = "adx_contact_pagenotification";
			[Relationship("adx_pollsubmission", EntityRole.Referenced, "adx_contact_pollsubmission", "")]
			public const string adx_contact_pollsubmission = "adx_contact_pollsubmission";
			[Relationship("adx_webfilelog", EntityRole.Referenced, "adx_contact_webfilelog", "")]
			public const string adx_contact_webfilelog = "adx_contact_webfilelog";
			[Relationship("adx_webpage", EntityRole.Referenced, "adx_contact_webpage", "")]
			public const string adx_contact_webpage = "adx_contact_webpage";
			[Relationship("adx_webpagelog", EntityRole.Referenced, "adx_contact_webpagelog", "")]
			public const string adx_contact_webpagelog = "adx_contact_webpagelog";
			[Relationship("adx_pagealert", EntityRole.Referenced, "adx_subscribercontact_pagealert", "")]
			public const string adx_subscribercontact_pagealert = "adx_subscribercontact_pagealert";
			[Relationship("adx_webformsession", EntityRole.Referenced, "adx_webformsession_contact", "")]
			public const string adx_webformsession_contact = "adx_webformsession_contact";
			[Relationship("adx_bpf_c2857b638fa7473d8e2f112c232cebd8", EntityRole.Referenced, "bpf_contact_adx_bpf_c2857b638fa7473d8e2f112c232cebd8", "")]
			public const string bpf_contact_adx_bpf_c2857b638fa7473d8e2f112c232cebd8 = "bpf_contact_adx_bpf_c2857b638fa7473d8e2f112c232cebd8";
			[Relationship("actioncard", EntityRole.Referenced, "contact_actioncard", "regardingobjectid")]
			public const string contact_actioncard = "contact_actioncard";
			[Relationship("activityparty", EntityRole.Referenced, "contact_activity_parties", "partyid")]
			public const string contact_activity_parties = "contact_activity_parties";
			[Relationship("activitypointer", EntityRole.Referenced, "Contact_ActivityPointers", "regardingobjectid")]
			public const string Contact_ActivityPointers = "Contact_ActivityPointers";
			[Relationship("adx_alertsubscription", EntityRole.Referenced, "contact_adx_alertsubscriptions", "")]
			public const string contact_adx_alertsubscriptions = "contact_adx_alertsubscriptions";
			[Relationship("adx_inviteredemption", EntityRole.Referenced, "contact_adx_inviteredemptions", "")]
			public const string contact_adx_inviteredemptions = "contact_adx_inviteredemptions";
			[Relationship("adx_portalcomment", EntityRole.Referenced, "contact_adx_portalcomments", "")]
			public const string contact_adx_portalcomments = "contact_adx_portalcomments";
			[Relationship("annotation", EntityRole.Referenced, "Contact_Annotation", "objectid")]
			public const string Contact_Annotation = "Contact_Annotation";
			[Relationship("appointment", EntityRole.Referenced, "Contact_Appointments", "regardingobjectid")]
			public const string Contact_Appointments = "Contact_Appointments";
			[Relationship("incident", EntityRole.Referenced, "contact_as_primary_contact", "primarycontactid")]
			public const string contact_as_primary_contact = "contact_as_primary_contact";
			[Relationship("incident", EntityRole.Referenced, "contact_as_responsible_contact", "responsiblecontactid")]
			public const string contact_as_responsible_contact = "contact_as_responsible_contact";
			[Relationship("asyncoperation", EntityRole.Referenced, "Contact_AsyncOperations", "regardingobjectid")]
			public const string Contact_AsyncOperations = "Contact_AsyncOperations";
			[Relationship("bookableresource", EntityRole.Referenced, "contact_bookableresource_ContactId", "contactid")]
			public const string contact_bookableresource_ContactId = "contact_bookableresource_ContactId";
			[Relationship("bulkdeletefailure", EntityRole.Referenced, "Contact_BulkDeleteFailures", "regardingobjectid")]
			public const string Contact_BulkDeleteFailures = "Contact_BulkDeleteFailures";
			[Relationship("bulkoperation", EntityRole.Referenced, "contact_BulkOperations", "regardingobjectid")]
			public const string contact_BulkOperations = "contact_BulkOperations";
			[Relationship("campaignresponse", EntityRole.Referenced, "contact_CampaignResponses", "regardingobjectid")]
			public const string contact_CampaignResponses = "contact_CampaignResponses";
			[Relationship("connection", EntityRole.Referenced, "contact_connections1", "record1id")]
			public const string contact_connections1 = "contact_connections1";
			[Relationship("connection", EntityRole.Referenced, "contact_connections2", "record2id")]
			public const string contact_connections2 = "contact_connections2";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "contact_customer_contacts", ContactDefinition.Columns.ParentCustomerId)]
			public const string contact_customer_contacts = "contact_customer_contacts";
			[Relationship("customeropportunityrole", EntityRole.Referenced, "contact_customer_opportunity_roles", "customerid")]
			public const string contact_customer_opportunity_roles = "contact_customer_opportunity_roles";
			[Relationship("customerrelationship", EntityRole.Referenced, "contact_customer_relationship_customer", "customerid")]
			public const string contact_customer_relationship_customer = "contact_customer_relationship_customer";
			[Relationship("customerrelationship", EntityRole.Referenced, "contact_customer_relationship_partner", "partnerid")]
			public const string contact_customer_relationship_partner = "contact_customer_relationship_partner";
			[Relationship("customeraddress", EntityRole.Referenced, "Contact_CustomerAddress", "parentid")]
			public const string Contact_CustomerAddress = "Contact_CustomerAddress";
			[Relationship("duplicaterecord", EntityRole.Referenced, "Contact_DuplicateBaseRecord", "baserecordid")]
			public const string Contact_DuplicateBaseRecord = "Contact_DuplicateBaseRecord";
			[Relationship("duplicaterecord", EntityRole.Referenced, "Contact_DuplicateMatchingRecord", "duplicaterecordid")]
			public const string Contact_DuplicateMatchingRecord = "Contact_DuplicateMatchingRecord";
			[Relationship("email", EntityRole.Referenced, "Contact_Email_EmailSender", "emailsender")]
			public const string Contact_Email_EmailSender = "Contact_Email_EmailSender";
			[Relationship("email", EntityRole.Referenced, "Contact_Emails", "regardingobjectid")]
			public const string Contact_Emails = "Contact_Emails";
			[Relationship("entitlement", EntityRole.Referenced, "contact_entitlement_ContactId", "contactid")]
			public const string contact_entitlement_ContactId = "contact_entitlement_ContactId";
			[Relationship("entitlement", EntityRole.Referenced, "contact_entitlement_Customer", "customerid")]
			public const string contact_entitlement_Customer = "contact_entitlement_Customer";
			[Relationship("externalpartyitem", EntityRole.Referenced, "Contact_ExternalPartyItems", "regardingobjectid")]
			public const string Contact_ExternalPartyItems = "Contact_ExternalPartyItems";
			[Relationship("fax", EntityRole.Referenced, "Contact_Faxes", "regardingobjectid")]
			public const string Contact_Faxes = "Contact_Faxes";
			[Relationship("feedback", EntityRole.Referenced, "Contact_Feedback", "regardingobjectid")]
			public const string Contact_Feedback = "Contact_Feedback";
			[Relationship("fileattachment", EntityRole.Referenced, "Contact_FileAttachment", "objectid")]
			public const string Contact_FileAttachment = "Contact_FileAttachment";
			[Relationship("letter", EntityRole.Referenced, "Contact_Letters", "regardingobjectid")]
			public const string Contact_Letters = "Contact_Letters";
			[Relationship("li_inmail", EntityRole.Referenced, "contact_li_inmails", "regardingobjectid")]
			public const string contact_li_inmails = "contact_li_inmails";
			[Relationship("li_message", EntityRole.Referenced, "contact_li_messages", "regardingobjectid")]
			public const string contact_li_messages = "contact_li_messages";
			[Relationship("li_pointdrivepresentationviewed", EntityRole.Referenced, "contact_li_pointdrivepresentationvieweds", "regardingobjectid")]
			public const string contact_li_pointdrivepresentationvieweds = "contact_li_pointdrivepresentationvieweds";
			[Relationship("mailboxtrackingfolder", EntityRole.Referenced, "Contact_MailboxTrackingFolder", "regardingobjectid")]
			public const string Contact_MailboxTrackingFolder = "Contact_MailboxTrackingFolder";
			[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, "contact_master_contact", "masterid")]
			public const string contact_master_contact = "contact_master_contact";
			[Relationship("msdyn_approval", EntityRole.Referenced, "contact_msdyn_approvals", "regardingobjectid")]
			public const string contact_msdyn_approvals = "contact_msdyn_approvals";
			[Relationship("msdyn_bookingalert", EntityRole.Referenced, "contact_msdyn_bookingalerts", "regardingobjectid")]
			public const string contact_msdyn_bookingalerts = "contact_msdyn_bookingalerts";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "contact_msdyn_surveyinvites", "")]
			public const string contact_msdyn_surveyinvites = "contact_msdyn_surveyinvites";
			[Relationship("msfp_surveyinvite", EntityRole.Referenced, "contact_msfp_surveyinvites", "regardingobjectid")]
			public const string contact_msfp_surveyinvites = "contact_msfp_surveyinvites";
			[Relationship("msfp_surveyresponse", EntityRole.Referenced, "contact_msfp_surveyresponses", "regardingobjectid")]
			public const string contact_msfp_surveyresponses = "contact_msfp_surveyresponses";
			[Relationship("phonecall", EntityRole.Referenced, "Contact_Phonecalls", "regardingobjectid")]
			public const string Contact_Phonecalls = "Contact_Phonecalls";
			[Relationship("postfollow", EntityRole.Referenced, "contact_PostFollows", "regardingobjectid")]
			public const string contact_PostFollows = "contact_PostFollows";
			[Relationship("postregarding", EntityRole.Referenced, "contact_PostRegardings", "regardingobjectid")]
			public const string contact_PostRegardings = "contact_PostRegardings";
			[Relationship("postrole", EntityRole.Referenced, "contact_PostRoles", "regardingobjectid")]
			public const string contact_PostRoles = "contact_PostRoles";
			[Relationship("principalobjectattributeaccess", EntityRole.Referenced, "contact_principalobjectattributeaccess", "objectid")]
			public const string contact_principalobjectattributeaccess = "contact_principalobjectattributeaccess";
			[Relationship("processsession", EntityRole.Referenced, "Contact_ProcessSessions", "regardingobjectid")]
			public const string Contact_ProcessSessions = "Contact_ProcessSessions";
			[Relationship("recurringappointmentmaster", EntityRole.Referenced, "Contact_RecurringAppointmentMasters", "regardingobjectid")]
			public const string Contact_RecurringAppointmentMasters = "Contact_RecurringAppointmentMasters";
			[Relationship("serviceappointment", EntityRole.Referenced, "Contact_ServiceAppointments", "regardingobjectid")]
			public const string Contact_ServiceAppointments = "Contact_ServiceAppointments";
			[Relationship("sharepointdocumentlocation", EntityRole.Referenced, "contact_SharePointDocumentLocations", "regardingobjectid")]
			public const string contact_SharePointDocumentLocations = "contact_SharePointDocumentLocations";
			[Relationship("sharepointdocument", EntityRole.Referenced, "contact_SharePointDocuments", "regardingobjectid")]
			public const string contact_SharePointDocuments = "contact_SharePointDocuments";
			[Relationship("socialactivity", EntityRole.Referenced, "Contact_SocialActivities", "regardingobjectid")]
			public const string Contact_SocialActivities = "Contact_SocialActivities";
			[Relationship("syncerror", EntityRole.Referenced, "Contact_SyncErrors", "regardingobjectid")]
			public const string Contact_SyncErrors = "Contact_SyncErrors";
			[Relationship("task", EntityRole.Referenced, "Contact_Tasks", "regardingobjectid")]
			public const string Contact_Tasks = "Contact_Tasks";
			[Relationship("tit_sms", EntityRole.Referenced, "contact_tit_smses", "")]
			public const string contact_tit_smses = "contact_tit_smses";
			[Relationship("contract", EntityRole.Referenced, "contract_billingcustomer_contacts", "billingcustomerid")]
			public const string contract_billingcustomer_contacts = "contract_billingcustomer_contacts";
			[Relationship("contract", EntityRole.Referenced, "contract_customer_contacts", "customerid")]
			public const string contract_customer_contacts = "contract_customer_contacts";
			[Relationship("contractdetail", EntityRole.Referenced, "contractlineitem_customer_contacts", "customerid")]
			public const string contractlineitem_customer_contacts = "contractlineitem_customer_contacts";
			[Relationship("bulkoperationlog", EntityRole.Referenced, "CreatedContact_BulkOperationLogs", "createdobjectid")]
			public const string CreatedContact_BulkOperationLogs = "CreatedContact_BulkOperationLogs";
			[Relationship("incident", EntityRole.Referenced, "incident_customer_contacts", "customerid")]
			public const string incident_customer_contacts = "incident_customer_contacts";
			[Relationship("invoice", EntityRole.Referenced, "invoice_customer_contacts", "customerid")]
			public const string invoice_customer_contacts = "invoice_customer_contacts";
			[Relationship("lead", EntityRole.Referenced, "lead_customer_contacts", "customerid")]
			public const string lead_customer_contacts = "lead_customer_contacts";
			[Relationship("lead", EntityRole.Referenced, "lead_parent_contact", "parentcontactid")]
			public const string lead_parent_contact = "lead_parent_contact";
			[Relationship("feedback", EntityRole.Referenced, "lk_contact_feedback_createdby", "createdbycontact")]
			public const string lk_contact_feedback_createdby = "lk_contact_feedback_createdby";
			[Relationship("feedback", EntityRole.Referenced, "lk_contact_feedback_createdonbehalfby", "createdonbehalfbycontact")]
			public const string lk_contact_feedback_createdonbehalfby = "lk_contact_feedback_createdonbehalfby";
			[Relationship("incident", EntityRole.Referenced, "msa_contact_incident", "")]
			public const string msa_contact_incident = "msa_contact_incident";
			[Relationship("opportunity", EntityRole.Referenced, "msa_contact_opportunity", "")]
			public const string msa_contact_opportunity = "msa_contact_opportunity";
			[Relationship("msdyn_actual", EntityRole.Referenced, "msdyn_contact_msdyn_actual_ContactCustomer", "msdyn_contactcustomer")]
			public const string msdyn_contact_msdyn_actual_ContactCustomer = "msdyn_contact_msdyn_actual_ContactCustomer";
			[Relationship("msdyn_actual", EntityRole.Referenced, "msdyn_contact_msdyn_actual_ContactVendor", "msdyn_contactvendor")]
			public const string msdyn_contact_msdyn_actual_ContactVendor = "msdyn_contact_msdyn_actual_ContactVendor";
			[Relationship("msdyn_contactpricelist", EntityRole.Referenced, "msdyn_contact_msdyn_contactpricelist_Contact", "msdyn_contact")]
			public const string msdyn_contact_msdyn_contactpricelist_Contact = "msdyn_contact_msdyn_contactpricelist_Contact";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "msdyn_contact_msdyn_estimateline_ContactCustomer", "msdyn_contactcustomer")]
			public const string msdyn_contact_msdyn_estimateline_ContactCustomer = "msdyn_contact_msdyn_estimateline_ContactCustomer";
			[Relationship("msdyn_estimateline", EntityRole.Referenced, "msdyn_contact_msdyn_estimateline_ContactVendor", "msdyn_contactvendor")]
			public const string msdyn_contact_msdyn_estimateline_ContactVendor = "msdyn_contact_msdyn_estimateline_ContactVendor";
			[Relationship("msdyn_fact", EntityRole.Referenced, "msdyn_contact_msdyn_fact_ContactCustomer", "msdyn_contactcustomer")]
			public const string msdyn_contact_msdyn_fact_ContactCustomer = "msdyn_contact_msdyn_fact_ContactCustomer";
			[Relationship("msdyn_fact", EntityRole.Referenced, "msdyn_contact_msdyn_fact_ContactVendor", "msdyn_contactvendor")]
			public const string msdyn_contact_msdyn_fact_ContactVendor = "msdyn_contact_msdyn_fact_ContactVendor";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "msdyn_contact_msdyn_invoicelinetransaction_ContactCustomer", "msdyn_contactcustomer")]
			public const string msdyn_contact_msdyn_invoicelinetransaction_ContactCustomer = "msdyn_contact_msdyn_invoicelinetransaction_ContactCustomer";
			[Relationship("msdyn_invoicelinetransaction", EntityRole.Referenced, "msdyn_contact_msdyn_invoicelinetransaction_ContactVendor", "msdyn_contactvendor")]
			public const string msdyn_contact_msdyn_invoicelinetransaction_ContactVendor = "msdyn_contact_msdyn_invoicelinetransaction_ContactVendor";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "msdyn_contact_msdyn_journalline_ContactCustomer", "msdyn_contactcustomer")]
			public const string msdyn_contact_msdyn_journalline_ContactCustomer = "msdyn_contact_msdyn_journalline_ContactCustomer";
			[Relationship("msdyn_journalline", EntityRole.Referenced, "msdyn_contact_msdyn_journalline_ContactVendor", "msdyn_contactvendor")]
			public const string msdyn_contact_msdyn_journalline_ContactVendor = "msdyn_contact_msdyn_journalline_ContactVendor";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "msdyn_contact_msdyn_opportunitylinetransaction_ContactCustomer", "msdyn_contactcustomer")]
			public const string msdyn_contact_msdyn_opportunitylinetransaction_ContactCustomer = "msdyn_contact_msdyn_opportunitylinetransaction_ContactCustomer";
			[Relationship("msdyn_opportunitylinetransaction", EntityRole.Referenced, "msdyn_contact_msdyn_opportunitylinetransaction_ContactVendor", "msdyn_contactvendor")]
			public const string msdyn_contact_msdyn_opportunitylinetransaction_ContactVendor = "msdyn_contact_msdyn_opportunitylinetransaction_ContactVendor";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "msdyn_contact_msdyn_orderlinetransaction_ContactCustomer", "msdyn_contactcustomer")]
			public const string msdyn_contact_msdyn_orderlinetransaction_ContactCustomer = "msdyn_contact_msdyn_orderlinetransaction_ContactCustomer";
			[Relationship("msdyn_orderlinetransaction", EntityRole.Referenced, "msdyn_contact_msdyn_orderlinetransaction_ContactVendor", "msdyn_contactvendor")]
			public const string msdyn_contact_msdyn_orderlinetransaction_ContactVendor = "msdyn_contact_msdyn_orderlinetransaction_ContactVendor";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "msdyn_contact_msdyn_quotelinetransaction_ContactCustomer", "msdyn_contactcustomer")]
			public const string msdyn_contact_msdyn_quotelinetransaction_ContactCustomer = "msdyn_contact_msdyn_quotelinetransaction_ContactCustomer";
			[Relationship("msdyn_quotelinetransaction", EntityRole.Referenced, "msdyn_contact_msdyn_quotelinetransaction_ContactVendor", "msdyn_contactvendor")]
			public const string msdyn_contact_msdyn_quotelinetransaction_ContactVendor = "msdyn_contact_msdyn_quotelinetransaction_ContactVendor";
			[Relationship("msdyn_responseoutcome", EntityRole.Referenced, "msdyn_contact_msdyn_responseoutcome_Contact", "")]
			public const string msdyn_contact_msdyn_responseoutcome_Contact = "msdyn_contact_msdyn_responseoutcome_Contact";
			[Relationship("msdyn_rma", EntityRole.Referenced, "msdyn_contact_msdyn_rma_RequestedByContact", "msdyn_requestedbycontact")]
			public const string msdyn_contact_msdyn_rma_RequestedByContact = "msdyn_contact_msdyn_rma_RequestedByContact";
			[Relationship("msdyn_rtv", EntityRole.Referenced, "msdyn_contact_msdyn_rtv_VendorContact", "msdyn_vendorcontact")]
			public const string msdyn_contact_msdyn_rtv_VendorContact = "msdyn_contact_msdyn_rtv_VendorContact";
			[Relationship("msdyn_surveyinvite", EntityRole.Referenced, "msdyn_contact_msdyn_surveyinvite_ContactId", "")]
			public const string msdyn_contact_msdyn_surveyinvite_ContactId = "msdyn_contact_msdyn_surveyinvite_ContactId";
			[Relationship("msdyn_surveyresponse", EntityRole.Referenced, "msdyn_contact_msdyn_surveyresponse_Contact", "")]
			public const string msdyn_contact_msdyn_surveyresponse_Contact = "msdyn_contact_msdyn_surveyresponse_Contact";
			[Relationship("msdyn_workorder", EntityRole.Referenced, "msdyn_contact_msdyn_workorder_ReportedByContact", "msdyn_reportedbycontact")]
			public const string msdyn_contact_msdyn_workorder_ReportedByContact = "msdyn_contact_msdyn_workorder_ReportedByContact";
			[Relationship("msdyn_playbookinstance", EntityRole.Referenced, "msdyn_playbookinstance_contact", "regarding")]
			public const string msdyn_playbookinstance_contact = "msdyn_playbookinstance_contact";
			[Relationship("opportunity", EntityRole.Referenced, "opportunity_customer_contacts", "customerid")]
			public const string opportunity_customer_contacts = "opportunity_customer_contacts";
			[Relationship("opportunity", EntityRole.Referenced, "opportunity_parent_contact", "parentcontactid")]
			public const string opportunity_parent_contact = "opportunity_parent_contact";
			[Relationship("salesorder", EntityRole.Referenced, "order_customer_contacts", "customerid")]
			public const string order_customer_contacts = "order_customer_contacts";
			[Relationship("quote", EntityRole.Referenced, "quote_customer_contacts", "customerid")]
			public const string quote_customer_contacts = "quote_customer_contacts";
			[Relationship("slakpiinstance", EntityRole.Referenced, "slakpiinstance_contact", "regarding")]
			public const string slakpiinstance_contact = "slakpiinstance_contact";
			[Relationship("socialactivity", EntityRole.Referenced, "socialactivity_postauthor_contacts", "postauthor")]
			public const string socialactivity_postauthor_contacts = "socialactivity_postauthor_contacts";
			[Relationship("socialactivity", EntityRole.Referenced, "socialactivity_postauthoraccount_contacts", "postauthoraccount")]
			public const string socialactivity_postauthoraccount_contacts = "socialactivity_postauthoraccount_contacts";
			[Relationship("socialprofile", EntityRole.Referenced, "Socialprofile_customer_contacts", "customerid")]
			public const string Socialprofile_customer_contacts = "Socialprofile_customer_contacts";
			[Relationship("bulkoperationlog", EntityRole.Referenced, "SourceContact_BulkOperationLogs", "regardingobjectid")]
			public const string SourceContact_BulkOperationLogs = "SourceContact_BulkOperationLogs";
			[Relationship("salesorder", EntityRole.Referenced, "tsr_contact_salesorder", "tsr_contactprincipalid")]
			public const string tsr_contact_salesorder = "tsr_contact_salesorder";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_contact", "objectid")]
			public const string userentityinstancedata_contact = "userentityinstancedata_contact";
		}
	}
}
