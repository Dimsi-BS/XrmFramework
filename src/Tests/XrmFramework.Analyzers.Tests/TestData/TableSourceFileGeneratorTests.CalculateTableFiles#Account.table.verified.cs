//HintName: Account.table.cs
using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using XrmFramework;

namespace XrmFramework.Definitions
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static partial class AccountDefinition
    {
        public const string EntityName = "account";
        public const string EntityCollectionName = "accounts";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Picklist (Categorie)
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(Categorie))]
            public const string AccountCategoryCode = "accountcategorycode";

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
            [StringLength(20)]
            public const string AccountNumber = "accountnumber";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(50)]
            public const string Address1_County = "address1_county";

            /// <summary>
            /// 
            /// Type : Double
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Double)]
            [Range(-90, 90)]
            public const string Address1_Latitude = "address1_latitude";

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
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(50)]
            public const string Address1_StateOrProvince = "address1_stateorprovince";

            /// <summary>
            /// 
            /// Type : Picklist (Adresse2ConditionsDeTransport)
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(Adresse2ConditionsDeTransport))]
            public const string Address2_FreightTermsCode = "address2_freighttermscode";

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
        public static class ManyToOneRelationships
        {
            [Relationship("contact", EntityRole.Referencing, "primarycontactid", "primarycontactid")]
            public const string account_primary_contact = "account_primary_contact";
            [Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "masterid", "masterid")]
            public const string account_master_account = "account_master_account";
            [Relationship("systemuser", EntityRole.Referencing, "preferredsystemuserid", "preferredsystemuserid")]
            public const string system_user_accounts = "system_user_accounts";
            [Relationship("externalparty", EntityRole.Referencing, "CreatedByExternalParty", "createdbyexternalparty")]
            public const string lk_externalparty_account_createdby = "lk_externalparty_account_createdby";
            [Relationship("systemuser", EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_accountbase_modifiedby = "lk_accountbase_modifiedby";
            [Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "parentaccountid", "parentaccountid")]
            public const string account_parent_account = "account_parent_account";
            [Relationship("imagedescriptor", EntityRole.Referencing, "entityimageid_imagedescriptor", "entityimageid")]
            public const string lk_account_entityimage = "lk_account_entityimage";
            [Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
            public const string business_unit_accounts = "business_unit_accounts";
            [Relationship("transactioncurrency", EntityRole.Referencing, "transactioncurrencyid", "transactioncurrencyid")]
            public const string transactioncurrency_account = "transactioncurrency_account";
            [Relationship("systemuser", EntityRole.Referencing, "owninguser", "owninguser")]
            public const string user_accounts = "user_accounts";
            [Relationship("systemuser", EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_accountbase_createdonbehalfby = "lk_accountbase_createdonbehalfby";
            [Relationship("processstage", EntityRole.Referencing, "stageid_processstage", "stageid")]
            public const string processstage_account = "processstage_account";
            [Relationship("systemuser", EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_accountbase_createdby = "lk_accountbase_createdby";
            [Relationship("externalparty", EntityRole.Referencing, "ModifiedByExternalParty", "modifiedbyexternalparty")]
            public const string lk_externalparty_account_modifiedby = "lk_externalparty_account_modifiedby";
            [Relationship("sla", EntityRole.Referencing, "slainvokedid_account_sla", "slainvokedid")]
            public const string sla_account = "sla_account";
            [Relationship("systemuser", EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_accountbase_modifiedonbehalfby = "lk_accountbase_modifiedonbehalfby";
            [Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
            public const string team_accounts = "team_accounts";
            [Relationship("owner", EntityRole.Referencing, "ownerid", "ownerid")]
            public const string owner_accounts = "owner_accounts";
            [Relationship("sla", EntityRole.Referencing, "sla_account_sla", "slaid")]
            public const string manualsla_account = "manualsla_account";
            [Relationship("lead", EntityRole.Referencing, "originatingleadid", "originatingleadid")]
            public const string account_originating_lead = "account_originating_lead";
            [Relationship("pricelevel", EntityRole.Referencing, "defaultpricelevelid", "defaultpricelevelid")]
            public const string price_level_accounts = "price_level_accounts";
            [Relationship("equipment", EntityRole.Referencing, "preferredequipmentid", "preferredequipmentid")]
            public const string equipment_accounts = "equipment_accounts";
            [Relationship("service", EntityRole.Referencing, "preferredserviceid", "preferredserviceid")]
            public const string service_accounts = "service_accounts";
            [Relationship("territory", EntityRole.Referencing, "territoryid", "territoryid")]
            public const string territory_accounts = "territory_accounts";
            [Relationship("msdyn_accountkpiitem", EntityRole.Referencing, "msdyn_accountkpiid", "msdyn_accountkpiid")]
            public const string msdyn_msdyn_accountkpiitem_account_accountkpiid = "msdyn_msdyn_accountkpiitem_account_accountkpiid";
            [Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "msdyn_billingaccount_account", "msdyn_billingaccount")]
            public const string msdyn_account_account_BillingAccount = "msdyn_account_account_BillingAccount";
            [Relationship("bookableresource", EntityRole.Referencing, "msdyn_PreferredResource", "msdyn_preferredresource")]
            public const string msdyn_bookableresource_account_PreferredResource = "msdyn_bookableresource_account_PreferredResource";
            [Relationship("msdyn_taxcode", EntityRole.Referencing, "msdyn_salestaxcode", "msdyn_salestaxcode")]
            public const string msdyn_msdyn_taxcode_account_SalesTaxCode = "msdyn_msdyn_taxcode_account_SalesTaxCode";
            [Relationship("msdyn_workhourtemplate", EntityRole.Referencing, "msdyn_workhourtemplate", "msdyn_workhourtemplate")]
            public const string msdyn_msdyn_workhourtemplate_account_workhourtemplate = "msdyn_msdyn_workhourtemplate_account_workhourtemplate";
            [Relationship("territory", EntityRole.Referencing, "msdyn_serviceterritory", "msdyn_serviceterritory")]
            public const string msdyn_territory_account_ServiceTerritory = "msdyn_territory_account_ServiceTerritory";
            [Relationship(AccountDefinition.EntityName, EntityRole.Referencing, "msa_managingpartnerid", "msa_managingpartnerid")]
            public const string msa_account_managingpartner = "msa_account_managingpartner";
            [Relationship("lor_tva", EntityRole.Referencing, "lor_TVAId", "lor_tvaid")]
            public const string lor_lor_tva_account_TVAId = "lor_lor_tva_account_TVAId";
            [Relationship("msdyn_segment", EntityRole.Referencing, "msdyn_segmentid", "msdyn_segmentid")]
            public const string msdyn_msdyn_segment_account = "msdyn_msdyn_segment_account";
        }
        public static class ManyToManyRelationships
        {
        }
        public static class OneToManyRelationships
        {
            [Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "account_parent_account", "parentaccountid")]
            public const string account_parent_account = "account_parent_account";
            [Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "account_master_account", "masterid")]
            public const string account_master_account = "account_master_account";
            [Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "msdyn_account_account_BillingAccount", "msdyn_billingaccount")]
            public const string msdyn_account_account_BillingAccount = "msdyn_account_account_BillingAccount";
            [Relationship(AccountDefinition.EntityName, EntityRole.Referenced, "msa_account_managingpartner", "msa_managingpartnerid")]
            public const string msa_account_managingpartner = "msa_account_managingpartner";
        }
    }

    [OptionSetDefinition(AccountDefinition.EntityName, AccountDefinition.Columns.AccountCategoryCode)]
    public enum Categorie
    {
        Null = 0,
        [Description("ClientFavori")]
        ClientFavori = 1,
        [Description("Standard")]
        Standard = 2,
    }







    [OptionSetDefinition(AccountDefinition.EntityName, AccountDefinition.Columns.Address2_FreightTermsCode)]
    public enum Adresse2ConditionsDeTransport
    {
        Null = 0,
        [Description("ValeurParDefaut")]
        ValeurParDefaut = 1,
    }














}
