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
    public static class TransactionCurrencyDefinition
    {
        public const string EntityName = "transactioncurrency";
        public const string EntityCollectionName = "transactioncurrencies";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(100)]
            public const string CurrencyName = "currencyname";

            /// <summary>
            /// 
            /// Type : Integer
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Integer)]
            [Range(0, 4)]
            public const string CurrencyPrecision = "currencyprecision";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(10)]
            public const string CurrencySymbol = "currencysymbol";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(5)]
            public const string ISOCurrencyCode = "isocurrencycode";

            /// <summary>
            /// 
            /// Type : State (TransactioncurrencyState)
            /// Validity :  Read | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.State)]
            [OptionSet(typeof(TransactioncurrencyState))]
            public const string StateCode = "statecode";

            /// <summary>
            /// 
            /// Type : Status (TransactioncurrencyStatus)
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Status)]
            [OptionSet(typeof(TransactioncurrencyStatus))]
            public const string StatusCode = "statuscode";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "transactioncurrencyid";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_transactioncurrency_createdonbehalfby = "lk_transactioncurrency_createdonbehalfby";
            [Relationship("imagedescriptor", EntityRole.Referencing, "entityimageid_imagedescriptor", "entityimageid")]
            public const string lk_transactioncurrency_entityimage = "lk_transactioncurrency_entityimage";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_transactioncurrency_modifiedonbehalfby = "lk_transactioncurrency_modifiedonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_transactioncurrencybase_createdby = "lk_transactioncurrencybase_createdby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_transactioncurrencybase_modifiedby = "lk_transactioncurrencybase_modifiedby";
            [Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
            public const string organization_transactioncurrencies = "organization_transactioncurrencies";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("adx_alertsubscription", EntityRole.Referenced, "adx_alertsubscription_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string adx_alertsubscription_transactioncurrency_transactioncurrencyid = "adx_alertsubscription_transactioncurrency_transactioncurrencyid";
            [Relationship("adx_inviteredemption", EntityRole.Referenced, "adx_inviteredemption_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string adx_inviteredemption_transactioncurrency_transactioncurrencyid = "adx_inviteredemption_transactioncurrency_transactioncurrencyid";
            [Relationship("adx_portalcomment", EntityRole.Referenced, "adx_portalcomment_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string adx_portalcomment_transactioncurrency_transactioncurrencyid = "adx_portalcomment_transactioncurrency_transactioncurrencyid";
            [Relationship("organization", EntityRole.Referenced, "basecurrency_organization", "basecurrencyid")]
            public const string basecurrency_organization = "basecurrency_organization";
            [Relationship("bulkoperation", EntityRole.Referenced, "bulkoperation_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string bulkoperation_transactioncurrency_transactioncurrencyid = "bulkoperation_transactioncurrency_transactioncurrencyid";
            [Relationship("dynamicpropertyassociation", EntityRole.Referenced, "DynamicPropertyAssociation_TransactionCurrency", "transactioncurrencyid")]
            public const string DynamicPropertyAssociation_TransactionCurrency = "DynamicPropertyAssociation_TransactionCurrency";
            [Relationship("dynamicpropertyoptionsetitem", EntityRole.Referenced, "DynamicPropertyOptionSetItem_TransactionCurrency", "transactioncurrencyid")]
            public const string DynamicPropertyOptionSetItem_TransactionCurrency = "DynamicPropertyOptionSetItem_TransactionCurrency";
            [Relationship("incidentresolution", EntityRole.Referenced, "incidentresolution_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string incidentresolution_transactioncurrency_transactioncurrencyid = "incidentresolution_transactioncurrency_transactioncurrencyid";
            [Relationship("msdyn_bookingalert", EntityRole.Referenced, "msdyn_bookingalert_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string msdyn_bookingalert_transactioncurrency_transactioncurrencyid = "msdyn_bookingalert_transactioncurrency_transactioncurrencyid";
            [Relationship("invoicedetail", EntityRole.Referenced, "msdyn_transactioncurrency_invoicedetail_Currency", "msdyn_currency")]
            public const string msdyn_transactioncurrency_invoicedetail_Currency = "msdyn_transactioncurrency_invoicedetail_Currency";
            [Relationship("msfp_alert", EntityRole.Referenced, "msfp_alert_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string msfp_alert_transactioncurrency_transactioncurrencyid = "msfp_alert_transactioncurrency_transactioncurrencyid";
            [Relationship("msfp_surveyinvite", EntityRole.Referenced, "msfp_surveyinvite_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string msfp_surveyinvite_transactioncurrency_transactioncurrencyid = "msfp_surveyinvite_transactioncurrency_transactioncurrencyid";
            [Relationship("msfp_surveyresponse", EntityRole.Referenced, "msfp_surveyresponse_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string msfp_surveyresponse_transactioncurrency_transactioncurrencyid = "msfp_surveyresponse_transactioncurrency_transactioncurrencyid";
            [Relationship("orderclose", EntityRole.Referenced, "orderclose_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string orderclose_transactioncurrency_transactioncurrencyid = "orderclose_transactioncurrency_transactioncurrencyid";
            [Relationship("quoteclose", EntityRole.Referenced, "quoteclose_transactioncurrency_transactioncurrencyid", "transactioncurrencyid")]
            public const string quoteclose_transactioncurrency_transactioncurrencyid = "quoteclose_transactioncurrency_transactioncurrencyid";
            [Relationship("account", EntityRole.Referenced, "transactioncurrency_account", "transactioncurrencyid")]
            public const string transactioncurrency_account = "transactioncurrency_account";
            [Relationship("actioncard", EntityRole.Referenced, "transactioncurrency_actioncard", "transactioncurrencyid")]
            public const string transactioncurrency_actioncard = "transactioncurrency_actioncard";
            [Relationship("actioncarduserstate", EntityRole.Referenced, "TransactionCurrency_ActionCardUserState", "transactioncurrencyid")]
            public const string TransactionCurrency_ActionCardUserState = "TransactionCurrency_ActionCardUserState";
            [Relationship("activitypointer", EntityRole.Referenced, "TransactionCurrency_ActivityPointer", "transactioncurrencyid")]
            public const string TransactionCurrency_ActivityPointer = "TransactionCurrency_ActivityPointer";
            [Relationship("annualfiscalcalendar", EntityRole.Referenced, "transactioncurrency_annualfiscalcalendar", "transactioncurrencyid")]
            public const string transactioncurrency_annualfiscalcalendar = "transactioncurrency_annualfiscalcalendar";
            [Relationship("appointment", EntityRole.Referenced, "TransactionCurrency_Appointment", "transactioncurrencyid")]
            public const string TransactionCurrency_Appointment = "TransactionCurrency_Appointment";
            [Relationship("asyncoperation", EntityRole.Referenced, "TransactionCurrency_AsyncOperations", "regardingobjectid")]
            public const string TransactionCurrency_AsyncOperations = "TransactionCurrency_AsyncOperations";
            [Relationship("bookableresource", EntityRole.Referenced, "TransactionCurrency_bookableresource", "transactioncurrencyid")]
            public const string TransactionCurrency_bookableresource = "TransactionCurrency_bookableresource";
            [Relationship("bookableresourcebooking", EntityRole.Referenced, "TransactionCurrency_bookableresourcebooking", "transactioncurrencyid")]
            public const string TransactionCurrency_bookableresourcebooking = "TransactionCurrency_bookableresourcebooking";
            [Relationship("bookableresourcebookingheader", EntityRole.Referenced, "TransactionCurrency_bookableresourcebookingheader", "transactioncurrencyid")]
            public const string TransactionCurrency_bookableresourcebookingheader = "TransactionCurrency_bookableresourcebookingheader";
            [Relationship("bookableresourcecategory", EntityRole.Referenced, "TransactionCurrency_bookableresourcecategory", "transactioncurrencyid")]
            public const string TransactionCurrency_bookableresourcecategory = "TransactionCurrency_bookableresourcecategory";
            [Relationship("bookableresourcecategoryassn", EntityRole.Referenced, "TransactionCurrency_bookableresourcecategoryassn", "transactioncurrencyid")]
            public const string TransactionCurrency_bookableresourcecategoryassn = "TransactionCurrency_bookableresourcecategoryassn";
            [Relationship("bookableresourcecharacteristic", EntityRole.Referenced, "TransactionCurrency_bookableresourcecharacteristic", "transactioncurrencyid")]
            public const string TransactionCurrency_bookableresourcecharacteristic = "TransactionCurrency_bookableresourcecharacteristic";
            [Relationship("bookableresourcegroup", EntityRole.Referenced, "TransactionCurrency_bookableresourcegroup", "transactioncurrencyid")]
            public const string TransactionCurrency_bookableresourcegroup = "TransactionCurrency_bookableresourcegroup";
            [Relationship("bookingstatus", EntityRole.Referenced, "TransactionCurrency_bookingstatus", "transactioncurrencyid")]
            public const string TransactionCurrency_bookingstatus = "TransactionCurrency_bookingstatus";
            [Relationship("businessunit", EntityRole.Referenced, "TransactionCurrency_BusinessUnit", "transactioncurrencyid")]
            public const string TransactionCurrency_BusinessUnit = "TransactionCurrency_BusinessUnit";
            [Relationship("campaign", EntityRole.Referenced, "transactioncurrency_campaign", "transactioncurrencyid")]
            public const string transactioncurrency_campaign = "transactioncurrency_campaign";
            [Relationship("campaignactivity", EntityRole.Referenced, "transactioncurrency_campaignactivity", "transactioncurrencyid")]
            public const string transactioncurrency_campaignactivity = "transactioncurrency_campaignactivity";
            [Relationship("campaignresponse", EntityRole.Referenced, "TransactionCurrency_CampaignResponse", "transactioncurrencyid")]
            public const string TransactionCurrency_CampaignResponse = "TransactionCurrency_CampaignResponse";
            [Relationship("cardtype", EntityRole.Referenced, "transactioncurrency_cardtype", "transactioncurrencyid")]
            public const string transactioncurrency_cardtype = "transactioncurrency_cardtype";
            [Relationship("category", EntityRole.Referenced, "transactioncurrency_category", "transactioncurrencyid")]
            public const string transactioncurrency_category = "transactioncurrency_category";
            [Relationship("channelaccessprofile", EntityRole.Referenced, "transactioncurrency_channelaccessprofile", "transactioncurrencyid")]
            public const string TransactionCurrency_ChannelAccessProfile = "TransactionCurrency_ChannelAccessProfile";
            [Relationship("characteristic", EntityRole.Referenced, "TransactionCurrency_characteristic", "transactioncurrencyid")]
            public const string TransactionCurrency_characteristic = "TransactionCurrency_characteristic";
            [Relationship("competitor", EntityRole.Referenced, "transactioncurrency_competitor", "transactioncurrencyid")]
            public const string transactioncurrency_competitor = "transactioncurrency_competitor";
            [Relationship("connection", EntityRole.Referenced, "TransactionCurrency_Connection", "transactioncurrencyid")]
            public const string TransactionCurrency_Connection = "TransactionCurrency_Connection";
            [Relationship("contact", EntityRole.Referenced, "transactioncurrency_contact", "transactioncurrencyid")]
            public const string transactioncurrency_contact = "transactioncurrency_contact";
            [Relationship("contract", EntityRole.Referenced, "transactioncurrency_contract", "transactioncurrencyid")]
            public const string transactioncurrency_contract = "transactioncurrency_contract";
            [Relationship("contractdetail", EntityRole.Referenced, "transactioncurrency_contractdetail", "transactioncurrencyid")]
            public const string transactioncurrency_contractdetail = "transactioncurrency_contractdetail";
            [Relationship("convertrule", EntityRole.Referenced, "TransactionCurrency_ConvertRule", "transactioncurrencyid")]
            public const string TransactionCurrency_ConvertRule = "TransactionCurrency_ConvertRule";
            [Relationship("convertruleitem", EntityRole.Referenced, "transactioncurrency_convertruleitem", "transactioncurrencyid")]
            public const string transactioncurrency_convertruleitem = "transactioncurrency_convertruleitem";
            [Relationship("customeraddress", EntityRole.Referenced, "TransactionCurrency_CustomerAddress", "transactioncurrencyid")]
            public const string TransactionCurrency_CustomerAddress = "TransactionCurrency_CustomerAddress";
            [Relationship("delveactionhub", EntityRole.Referenced, "TransactionCurrency_delveactionhub", "transactioncurrencyid")]
            public const string TransactionCurrency_delveactionhub = "TransactionCurrency_delveactionhub";
            [Relationship("discount", EntityRole.Referenced, "transactioncurrency_discount", "transactioncurrencyid")]
            public const string transactioncurrency_discount = "transactioncurrency_discount";
            [Relationship("discounttype", EntityRole.Referenced, "transactioncurrency_discounttype", "transactioncurrencyid")]
            public const string transactioncurrency_discounttype = "transactioncurrency_discounttype";
            [Relationship("duplicaterecord", EntityRole.Referenced, "TransactionCurrency_DuplicateBaseRecord", "baserecordid")]
            public const string TransactionCurrency_DuplicateBaseRecord = "TransactionCurrency_DuplicateBaseRecord";
            [Relationship("duplicaterecord", EntityRole.Referenced, "TransactionCurrency_DuplicateMatchingRecord", "duplicaterecordid")]
            public const string TransactionCurrency_DuplicateMatchingRecord = "TransactionCurrency_DuplicateMatchingRecord";
            [Relationship("dynamicpropertyinstance", EntityRole.Referenced, "TransactionCurrency_Dynamicpropertyinsatance", "dynamicpropertyinstanceid")]
            public const string TransactionCurrency_Dynamicpropertyinsatance = "TransactionCurrency_Dynamicpropertyinsatance";
            [Relationship("email", EntityRole.Referenced, "TransactionCurrency_Email", "transactioncurrencyid")]
            public const string TransactionCurrency_Email = "TransactionCurrency_Email";
            [Relationship("entitlement", EntityRole.Referenced, "TransactionCurrency_Entitlement", "transactioncurrencyid")]
            public const string TransactionCurrency_Entitlement = "TransactionCurrency_Entitlement";
            [Relationship("entitlementchannel", EntityRole.Referenced, "TransactionCurrency_entitlementchannel", "transactioncurrencyid")]
            public const string TransactionCurrency_entitlementchannel = "TransactionCurrency_entitlementchannel";
            [Relationship("entitlementtemplate", EntityRole.Referenced, "TransactionCurrency_entitlementtemplate", "transactioncurrencyid")]
            public const string TransactionCurrency_entitlementtemplate = "TransactionCurrency_entitlementtemplate";
            [Relationship("entitlementtemplatechannel", EntityRole.Referenced, "TransactionCurrency_entitlementtemplatechannel", "transactioncurrencyid")]
            public const string TransactionCurrency_entitlementtemplatechannel = "TransactionCurrency_entitlementtemplatechannel";
            [Relationship("equipment", EntityRole.Referenced, "TransactionCurrency_Equipment", "transactioncurrencyid")]
            public const string TransactionCurrency_Equipment = "TransactionCurrency_Equipment";
            [Relationship("expiredprocess", EntityRole.Referenced, "transactioncurrency_expiredprocess", "transactioncurrencyid")]
            public const string transactioncurrency_expiredprocess = "transactioncurrency_expiredprocess";
            [Relationship("externalparty", EntityRole.Referenced, "transactioncurrency_externalparty", "transactioncurrencyid")]
            public const string TransactionCurrency_ExternalParty = "TransactionCurrency_ExternalParty";
            [Relationship("externalpartyitem", EntityRole.Referenced, "TransactionCurrency_externalpartyitem", "transactioncurrencyid")]
            public const string TransactionCurrency_externalpartyitem = "TransactionCurrency_externalpartyitem";
            [Relationship("fax", EntityRole.Referenced, "TransactionCurrency_Fax", "transactioncurrencyid")]
            public const string TransactionCurrency_Fax = "TransactionCurrency_Fax";
            [Relationship("feedback", EntityRole.Referenced, "transactioncurrency_feedback", "transactioncurrencyid")]
            public const string transactioncurrency_feedback = "transactioncurrency_feedback";
            [Relationship("fixedmonthlyfiscalcalendar", EntityRole.Referenced, "transactioncurrency_fixedmonthlyfiscalcalendar", "transactioncurrencyid")]
            public const string transactioncurrency_fixedmonthlyfiscalcalendar = "transactioncurrency_fixedmonthlyfiscalcalendar";
            [Relationship("goal", EntityRole.Referenced, "TransactionCurrency_Goal", "transactioncurrencyid")]
            public const string TransactionCurrency_Goal = "TransactionCurrency_Goal";
            [Relationship("incident", EntityRole.Referenced, "TransactionCurrency_Incident", "transactioncurrencyid")]
            public const string TransactionCurrency_Incident = "TransactionCurrency_Incident";
            [Relationship("interactionforemail", EntityRole.Referenced, "TransactionCurrency_InteractionForEmail", "transactioncurrencyid")]
            public const string TransactionCurrency_InteractionForEmail = "TransactionCurrency_InteractionForEmail";
            [Relationship("invoice", EntityRole.Referenced, "transactioncurrency_invoice", "transactioncurrencyid")]
            public const string transactioncurrency_invoice = "transactioncurrency_invoice";
            [Relationship("invoicedetail", EntityRole.Referenced, "transactioncurrency_invoicedetail", "transactioncurrencyid")]
            public const string transactioncurrency_invoicedetail = "transactioncurrency_invoicedetail";
            [Relationship("kbarticle", EntityRole.Referenced, "TransactionCurrency_KbArticle", "transactioncurrencyid")]
            public const string TransactionCurrency_KbArticle = "TransactionCurrency_KbArticle";
            [Relationship("knowledgearticle", EntityRole.Referenced, "transactioncurrency_knowledgearticle", "transactioncurrencyid")]
            public const string TransactionCurrency_knowledgearticle = "TransactionCurrency_knowledgearticle";
            [Relationship("knowledgearticleincident", EntityRole.Referenced, "transactioncurrency_knowledgearticleincident", "transactioncurrencyid")]
            public const string transactioncurrency_knowledgearticleincident = "transactioncurrency_knowledgearticleincident";
            [Relationship("knowledgearticleviews", EntityRole.Referenced, "transactioncurrency_knowledgearticleviews", "transactioncurrencyid")]
            public const string transactioncurrency_knowledgearticleviews = "transactioncurrency_knowledgearticleviews";
            [Relationship("knowledgebaserecord", EntityRole.Referenced, "TransactionCurrency_KnowledgeBaseRecord", "transactioncurrencyid")]
            public const string TransactionCurrency_KnowledgeBaseRecord = "TransactionCurrency_KnowledgeBaseRecord";
            [Relationship("lead", EntityRole.Referenced, "transactioncurrency_lead", "transactioncurrencyid")]
            public const string transactioncurrency_lead = "transactioncurrency_lead";
            [Relationship("leadaddress", EntityRole.Referenced, "TransactionCurrency_LeadAddress", "transactioncurrencyid")]
            public const string TransactionCurrency_LeadAddress = "TransactionCurrency_LeadAddress";
            [Relationship("leadtoopportunitysalesprocess", EntityRole.Referenced, "transactioncurrency_leadtoopportunitysalesprocess", "transactioncurrencyid")]
            public const string transactioncurrency_leadtoopportunitysalesprocess = "transactioncurrency_leadtoopportunitysalesprocess";
            [Relationship("letter", EntityRole.Referenced, "TransactionCurrency_Letter", "transactioncurrencyid")]
            public const string TransactionCurrency_Letter = "TransactionCurrency_Letter";
            [Relationship("list", EntityRole.Referenced, "transactioncurrency_list", "transactioncurrencyid")]
            public const string transactioncurrency_list = "transactioncurrency_list";
            [Relationship("lor_bien", EntityRole.Referenced, "TransactionCurrency_lor_bien", "transactioncurrencyid")]
            public const string TransactionCurrency_lor_bien = "TransactionCurrency_lor_bien";
            [Relationship("lor_maintenance", EntityRole.Referenced, "TransactionCurrency_lor_Maintenance", "transactioncurrencyid")]
            public const string TransactionCurrency_lor_Maintenance = "TransactionCurrency_lor_Maintenance";
            [Relationship("lor_stock", EntityRole.Referenced, "TransactionCurrency_lor_Stock", "transactioncurrencyid")]
            public const string TransactionCurrency_lor_Stock = "TransactionCurrency_lor_Stock";
            [Relationship("mailmergetemplate", EntityRole.Referenced, "TransactionCurrency_MailMergeTemplate", "transactioncurrencyid")]
            public const string TransactionCurrency_MailMergeTemplate = "TransactionCurrency_MailMergeTemplate";
            [Relationship("monthlyfiscalcalendar", EntityRole.Referenced, "transactioncurrency_monthlyfiscalcalendar", "transactioncurrencyid")]
            public const string transactioncurrency_monthlyfiscalcalendar = "transactioncurrency_monthlyfiscalcalendar";
            [Relationship("msdyn_actual", EntityRole.Referenced, "TransactionCurrency_msdyn_actual", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_actual = "TransactionCurrency_msdyn_actual";
            [Relationship("msdyn_agreementbookingproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_agreementbookingproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_agreementbookingproduct = "TransactionCurrency_msdyn_agreementbookingproduct";
            [Relationship("msdyn_agreementbookingservice", EntityRole.Referenced, "TransactionCurrency_msdyn_agreementbookingservice", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_agreementbookingservice = "TransactionCurrency_msdyn_agreementbookingservice";
            [Relationship("msdyn_agreementinvoiceproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_agreementinvoiceproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_agreementinvoiceproduct = "TransactionCurrency_msdyn_agreementinvoiceproduct";
            [Relationship("msdyn_bookingjournal", EntityRole.Referenced, "TransactionCurrency_msdyn_bookingjournal", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_bookingjournal = "TransactionCurrency_msdyn_bookingjournal";
            [Relationship("msdyn_fieldservicepricelistitem", EntityRole.Referenced, "TransactionCurrency_msdyn_fieldservicepricelistitem", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_fieldservicepricelistitem = "TransactionCurrency_msdyn_fieldservicepricelistitem";
            [Relationship("msdyn_forecastinstance", EntityRole.Referenced, "TransactionCurrency_msdyn_forecastinstance", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_forecastinstance = "TransactionCurrency_msdyn_forecastinstance";
            [Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_orderinvoicingproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_orderinvoicingproduct = "TransactionCurrency_msdyn_orderinvoicingproduct";
            [Relationship("msdyn_payment", EntityRole.Referenced, "TransactionCurrency_msdyn_payment", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_payment = "TransactionCurrency_msdyn_payment";
            [Relationship("msdyn_paymentdetail", EntityRole.Referenced, "TransactionCurrency_msdyn_paymentdetail", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_paymentdetail = "TransactionCurrency_msdyn_paymentdetail";
            [Relationship("msdyn_problematicasset", EntityRole.Referenced, "TransactionCurrency_msdyn_problematicasset", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_problematicasset = "TransactionCurrency_msdyn_problematicasset";
            [Relationship("msdyn_purchaseorder", EntityRole.Referenced, "TransactionCurrency_msdyn_purchaseorder", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_purchaseorder = "TransactionCurrency_msdyn_purchaseorder";
            [Relationship("msdyn_purchaseorderbill", EntityRole.Referenced, "TransactionCurrency_msdyn_purchaseorderbill", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_purchaseorderbill = "TransactionCurrency_msdyn_purchaseorderbill";
            [Relationship("msdyn_purchaseorderproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_purchaseorderproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_purchaseorderproduct = "TransactionCurrency_msdyn_purchaseorderproduct";
            [Relationship("msdyn_purchaseorderreceiptproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_purchaseorderreceiptproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_purchaseorderreceiptproduct = "TransactionCurrency_msdyn_purchaseorderreceiptproduct";
            [Relationship("msdyn_quotebookingproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_quotebookingproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_quotebookingproduct = "TransactionCurrency_msdyn_quotebookingproduct";
            [Relationship("msdyn_quotebookingservice", EntityRole.Referenced, "TransactionCurrency_msdyn_quotebookingservice", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_quotebookingservice = "TransactionCurrency_msdyn_quotebookingservice";
            [Relationship("msdyn_quotebookingsetup", EntityRole.Referenced, "TransactionCurrency_msdyn_quotebookingsetup", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_quotebookingsetup = "TransactionCurrency_msdyn_quotebookingsetup";
            [Relationship("msdyn_quoteinvoicingproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_quoteinvoicingproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_quoteinvoicingproduct = "TransactionCurrency_msdyn_quoteinvoicingproduct";
            [Relationship("msdyn_quoteinvoicingsetup", EntityRole.Referenced, "TransactionCurrency_msdyn_quoteinvoicingsetup", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_quoteinvoicingsetup = "TransactionCurrency_msdyn_quoteinvoicingsetup";
            [Relationship("msdyn_rma", EntityRole.Referenced, "TransactionCurrency_msdyn_rma", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_rma = "TransactionCurrency_msdyn_rma";
            [Relationship("msdyn_rmaproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_rmaproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_rmaproduct = "TransactionCurrency_msdyn_rmaproduct";
            [Relationship("msdyn_rtv", EntityRole.Referenced, "TransactionCurrency_msdyn_rtv", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_rtv = "TransactionCurrency_msdyn_rtv";
            [Relationship("msdyn_rtvproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_rtvproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_rtvproduct = "TransactionCurrency_msdyn_rtvproduct";
            [Relationship("msdyn_salessuggestion", EntityRole.Referenced, "TransactionCurrency_msdyn_salessuggestion", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_salessuggestion = "TransactionCurrency_msdyn_salessuggestion";
            [Relationship("msdyn_workorder", EntityRole.Referenced, "TransactionCurrency_msdyn_workorder", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_workorder = "TransactionCurrency_msdyn_workorder";
            [Relationship("msdyn_workorderproduct", EntityRole.Referenced, "TransactionCurrency_msdyn_workorderproduct", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_workorderproduct = "TransactionCurrency_msdyn_workorderproduct";
            [Relationship("msdyn_workorderservice", EntityRole.Referenced, "TransactionCurrency_msdyn_workorderservice", "transactioncurrencyid")]
            public const string TransactionCurrency_msdyn_workorderservice = "TransactionCurrency_msdyn_workorderservice";
            [Relationship("newprocess", EntityRole.Referenced, "transactioncurrency_newprocess", "transactioncurrencyid")]
            public const string transactioncurrency_newprocess = "transactioncurrency_newprocess";
            [Relationship("officegraphdocument", EntityRole.Referenced, "TransactionCurrency_officegraphdocument", "transactioncurrencyid")]
            public const string TransactionCurrency_officegraphdocument = "TransactionCurrency_officegraphdocument";
            [Relationship("opportunity", EntityRole.Referenced, "transactioncurrency_opportunity", "transactioncurrencyid")]
            public const string transactioncurrency_opportunity = "transactioncurrency_opportunity";
            [Relationship("opportunityclose", EntityRole.Referenced, "transactioncurrency_opportunityclose", "transactioncurrencyid")]
            public const string transactioncurrency_opportunityclose = "transactioncurrency_opportunityclose";
            [Relationship("opportunityproduct", EntityRole.Referenced, "transactioncurrency_opportunityproduct", "transactioncurrencyid")]
            public const string transactioncurrency_opportunityproduct = "transactioncurrency_opportunityproduct";
            [Relationship("opportunitysalesprocess", EntityRole.Referenced, "transactioncurrency_opportunitysalesprocess", "transactioncurrencyid")]
            public const string transactioncurrency_opportunitysalesprocess = "transactioncurrency_opportunitysalesprocess";
            [Relationship("phonecall", EntityRole.Referenced, "TransactionCurrency_PhoneCall", "transactioncurrencyid")]
            public const string TransactionCurrency_PhoneCall = "TransactionCurrency_PhoneCall";
            [Relationship("phonetocaseprocess", EntityRole.Referenced, "transactioncurrency_phonetocaseprocess", "transactioncurrencyid")]
            public const string transactioncurrency_phonetocaseprocess = "transactioncurrency_phonetocaseprocess";
            [Relationship("position", EntityRole.Referenced, "transactioncurrency_position", "transactioncurrencyid")]
            public const string transactioncurrency_position = "transactioncurrency_position";
            [Relationship("pricelevel", EntityRole.Referenced, "transactioncurrency_pricelevel", "transactioncurrencyid")]
            public const string transactioncurrency_pricelevel = "transactioncurrency_pricelevel";
            [Relationship("processsession", EntityRole.Referenced, "TransactionCurrency_ProcessSessions", "regardingobjectid")]
            public const string TransactionCurrency_ProcessSessions = "TransactionCurrency_ProcessSessions";
            [Relationship("product", EntityRole.Referenced, "transactioncurrency_product", "transactioncurrencyid")]
            public const string transactioncurrency_product = "transactioncurrency_product";
            [Relationship("productassociation", EntityRole.Referenced, "transactioncurrency_ProductAssociation", "transactioncurrencyid")]
            public const string transactioncurrency_ProductAssociation = "transactioncurrency_ProductAssociation";
            [Relationship("productpricelevel", EntityRole.Referenced, "transactioncurrency_productpricelevel", "transactioncurrencyid")]
            public const string transactioncurrency_productpricelevel = "transactioncurrency_productpricelevel";
            [Relationship("productsubstitute", EntityRole.Referenced, "transactioncurrency_ProductSubstitute", "transactioncurrencyid")]
            public const string transactioncurrency_ProductSubstitute = "transactioncurrency_ProductSubstitute";
            [Relationship("channelaccessprofilerule", EntityRole.Referenced, "TransactionCurrency_profilerule", "transactioncurrencyid")]
            public const string TransactionCurrency_profilerule = "TransactionCurrency_profilerule";
            [Relationship("channelaccessprofileruleitem", EntityRole.Referenced, "TransactionCurrency_profileruleitem", "transactioncurrencyid")]
            public const string TransactionCurrency_profileruleitem = "TransactionCurrency_profileruleitem";
            [Relationship("quarterlyfiscalcalendar", EntityRole.Referenced, "transactioncurrency_quarterlyfiscalcalendar", "transactioncurrencyid")]
            public const string transactioncurrency_quarterlyfiscalcalendar = "transactioncurrency_quarterlyfiscalcalendar";
            [Relationship("queue", EntityRole.Referenced, "TransactionCurrency_Queue", "transactioncurrencyid")]
            public const string TransactionCurrency_Queue = "TransactionCurrency_Queue";
            [Relationship("queueitem", EntityRole.Referenced, "TransactionCurrency_QueueItem", "transactioncurrencyid")]
            public const string TransactionCurrency_QueueItem = "TransactionCurrency_QueueItem";
            [Relationship("quote", EntityRole.Referenced, "transactioncurrency_quote", "transactioncurrencyid")]
            public const string transactioncurrency_quote = "transactioncurrency_quote";
            [Relationship("quotedetail", EntityRole.Referenced, "transactioncurrency_quotedetail", "transactioncurrencyid")]
            public const string transactioncurrency_quotedetail = "transactioncurrency_quotedetail";
            [Relationship("ratingmodel", EntityRole.Referenced, "TransactionCurrency_ratingmodel", "transactioncurrencyid")]
            public const string TransactionCurrency_ratingmodel = "TransactionCurrency_ratingmodel";
            [Relationship("ratingvalue", EntityRole.Referenced, "TransactionCurrency_ratingvalue", "transactioncurrencyid")]
            public const string TransactionCurrency_ratingvalue = "TransactionCurrency_ratingvalue";
            [Relationship("recommendeddocument", EntityRole.Referenced, "TransactionCurrency_recommendeddocument", "transactioncurrencyid")]
            public const string TransactionCurrency_recommendeddocument = "TransactionCurrency_recommendeddocument";
            [Relationship("recurringappointmentmaster", EntityRole.Referenced, "TransactionCurrency_RecurringAppointmentMaster", "transactioncurrencyid")]
            public const string TransactionCurrency_RecurringAppointmentMaster = "TransactionCurrency_RecurringAppointmentMaster";
            [Relationship("reportcategory", EntityRole.Referenced, "TransactionCurrency_ReportCategory", "transactioncurrencyid")]
            public const string TransactionCurrency_ReportCategory = "TransactionCurrency_ReportCategory";
            [Relationship("routingrule", EntityRole.Referenced, "TransactionCurrency_Routingrule", "transactioncurrencyid")]
            public const string TransactionCurrency_Routingrule = "TransactionCurrency_Routingrule";
            [Relationship("routingruleitem", EntityRole.Referenced, "TransactionCurrency_routingruleitem", "transactioncurrencyid")]
            public const string TransactionCurrency_routingruleitem = "TransactionCurrency_routingruleitem";
            [Relationship("salesliterature", EntityRole.Referenced, "TransactionCurrency_SalesLiterature", "transactioncurrencyid")]
            public const string TransactionCurrency_SalesLiterature = "TransactionCurrency_SalesLiterature";
            [Relationship("salesorder", EntityRole.Referenced, "transactioncurrency_salesorder", "transactioncurrencyid")]
            public const string transactioncurrency_salesorder = "transactioncurrency_salesorder";
            [Relationship("salesorderdetail", EntityRole.Referenced, "transactioncurrency_salesorderdetail", "transactioncurrencyid")]
            public const string transactioncurrency_salesorderdetail = "transactioncurrency_salesorderdetail";
            [Relationship("semiannualfiscalcalendar", EntityRole.Referenced, "transactioncurrency_semiannualfiscalcalendar", "transactioncurrencyid")]
            public const string transactioncurrency_semiannualfiscalcalendar = "transactioncurrency_semiannualfiscalcalendar";
            [Relationship("serviceappointment", EntityRole.Referenced, "TransactionCurrency_ServiceAppointment", "transactioncurrencyid")]
            public const string TransactionCurrency_ServiceAppointment = "TransactionCurrency_ServiceAppointment";
            [Relationship("sharepointdocument", EntityRole.Referenced, "TransactionCurrency_SharePointDocument", "transactioncurrencyid")]
            public const string TransactionCurrency_SharePointDocument = "TransactionCurrency_SharePointDocument";
            [Relationship("sharepointdocumentlocation", EntityRole.Referenced, "TransactionCurrency_SharePointDocumentLocation", "transactioncurrencyid")]
            public const string TransactionCurrency_SharePointDocumentLocation = "TransactionCurrency_SharePointDocumentLocation";
            [Relationship("sharepointsite", EntityRole.Referenced, "TransactionCurrency_SharePointSite", "transactioncurrencyid")]
            public const string TransactionCurrency_SharePointSite = "TransactionCurrency_SharePointSite";
            [Relationship("similarityrule", EntityRole.Referenced, "TransactionCurrency_SimilarityRule", "transactioncurrencyid")]
            public const string TransactionCurrency_SimilarityRule = "TransactionCurrency_SimilarityRule";
            [Relationship("sla", EntityRole.Referenced, "TransactionCurrency_SLA", "transactioncurrencyid")]
            public const string TransactionCurrency_SLA = "TransactionCurrency_SLA";
            [Relationship("slaitem", EntityRole.Referenced, "TransactionCurrency_SLAItem", "transactioncurrencyid")]
            public const string TransactionCurrency_SLAItem = "TransactionCurrency_SLAItem";
            [Relationship("slakpiinstance", EntityRole.Referenced, "TransactionCurrency_slakpiinstance", "transactioncurrencyid")]
            public const string TransactionCurrency_slakpiinstance = "TransactionCurrency_slakpiinstance";
            [Relationship("socialactivity", EntityRole.Referenced, "transactioncurrency_socialactivity", "transactioncurrencyid")]
            public const string transactioncurrency_socialactivity = "transactioncurrency_socialactivity";
            [Relationship("socialprofile", EntityRole.Referenced, "transactioncurrency_SocialProfile", "transactioncurrencyid")]
            public const string transactioncurrency_SocialProfile = "transactioncurrency_SocialProfile";
            [Relationship("suggestioncardtemplate", EntityRole.Referenced, "transactioncurrency_suggestioncardtemplate", "transactioncurrencyid")]
            public const string TransactionCurrency_suggestioncardtemplate = "TransactionCurrency_suggestioncardtemplate";
            [Relationship("syncerror", EntityRole.Referenced, "TransactionCurrency_SyncErrors", "regardingobjectid")]
            public const string TransactionCurrency_SyncErrors = "TransactionCurrency_SyncErrors";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referenced, "TransactionCurrency_SystemUser", "transactioncurrencyid")]
            public const string TransactionCurrency_SystemUser = "TransactionCurrency_SystemUser";
            [Relationship("task", EntityRole.Referenced, "TransactionCurrency_Task", "transactioncurrencyid")]
            public const string TransactionCurrency_Task = "TransactionCurrency_Task";
            [Relationship("team", EntityRole.Referenced, "TransactionCurrency_Team", "transactioncurrencyid")]
            public const string TransactionCurrency_Team = "TransactionCurrency_Team";
            [Relationship("territory", EntityRole.Referenced, "TransactionCurrency_Territory", "transactioncurrencyid")]
            public const string TransactionCurrency_Territory = "TransactionCurrency_Territory";
            [Relationship("theme", EntityRole.Referenced, "TransactionCurrency_Theme", "transactioncurrencyid")]
            public const string TransactionCurrency_Theme = "TransactionCurrency_Theme";
            [Relationship("translationprocess", EntityRole.Referenced, "transactioncurrency_translationprocess", "transactioncurrencyid")]
            public const string transactioncurrency_translationprocess = "transactioncurrency_translationprocess";
            [Relationship("untrackedemail", EntityRole.Referenced, "TransactionCurrency_UntrackedEmail", "transactioncurrencyid")]
            public const string TransactionCurrency_UntrackedEmail = "TransactionCurrency_UntrackedEmail";
            [Relationship("userfiscalcalendar", EntityRole.Referenced, "transactioncurrency_userfiscalcalendar", "transactioncurrencyid")]
            public const string transactioncurrency_userfiscalcalendar = "transactioncurrency_userfiscalcalendar";
            [Relationship("usermapping", EntityRole.Referenced, "TransactionCurrency_UserMapping", "transactioncurrencyid")]
            public const string TransactionCurrency_UserMapping = "TransactionCurrency_UserMapping";
            [Relationship("usersettings", EntityRole.Referenced, "transactioncurrency_usersettings", "transactioncurrencyid")]
            public const string transactioncurrency_usersettings = "transactioncurrency_usersettings";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_transactioncurrency", "objectid")]
            public const string userentityinstancedata_transactioncurrency = "userentityinstancedata_transactioncurrency";
        }
    }
}
