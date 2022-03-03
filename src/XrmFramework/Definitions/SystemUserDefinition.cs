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

            /// <summary>
            /// 
            /// Type : Picklist (ModeDAcces)
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(ModeDAcces))]
            public const string AccessMode = "accessmode";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            public const string ApplicationId = "applicationid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [AlternateKey(AlternateKeyNames.AADObjectid)]
            public const string AzureActiveDirectoryObjectId = "azureactivedirectoryobjectid";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(1024)]
            public const string DomainName = "domainname";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100)]
            public const string EmployeeId = "employeeid";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(256)]
            public const string FirstName = "firstname";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class AlternateKeyNames
        {
            public const string AADObjectid = "aadobjectid";
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
            [Relationship("adx_webrole", EntityRole.Referencing, "adx_webrole_systemuser", "adx_webroleid")]
            public const string adx_webrole_systemuser = "adx_webrole_systemuser";
            [Relationship("msdyn_appconfiguration", EntityRole.Referencing, "msdyn_appconfiguration_systemuser", "msdyn_appconfigurationid")]
            public const string msdyn_appconfiguration_systemuser = "msdyn_appconfiguration_systemuser";
            [Relationship("msdyn_attributevalue", EntityRole.Referencing, "msdyn_msdyn_attributevalue_systemuser", "msdyn_attributevalueid")]
            public const string msdyn_msdyn_attributevalue_systemuser = "msdyn_msdyn_attributevalue_systemuser";
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
            [Relationship(TransactionCurrencyDefinition.EntityName, EntityRole.Referencing, "transactioncurrencyid", "transactioncurrencyid")]
            public const string TransactionCurrency_SystemUser = "TransactionCurrency_SystemUser";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "parentsystemuserid", "parentsystemuserid")]
            public const string user_parent_user = "user_parent_user";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("actioncardusersettings", EntityRole.Referenced, "actioncardusersettings_owning_user", "owninguser")]
            public const string actioncardusersettings_owning_user = "actioncardusersettings_owning_user";
            [Relationship("adx_alertsubscription", EntityRole.Referenced, "adx_alertsubscription_systemuser_createdby", "createdby")]
            public const string adx_alertsubscription_systemuser_createdby = "adx_alertsubscription_systemuser_createdby";
            [Relationship("adx_alertsubscription", EntityRole.Referenced, "adx_alertsubscription_systemuser_createdonbehalfby", "createdonbehalfby")]
            public const string adx_alertsubscription_systemuser_createdonbehalfby = "adx_alertsubscription_systemuser_createdonbehalfby";
            [Relationship("adx_alertsubscription", EntityRole.Referenced, "adx_alertsubscription_systemuser_modifiedby", "modifiedby")]
            public const string adx_alertsubscription_systemuser_modifiedby = "adx_alertsubscription_systemuser_modifiedby";
            [Relationship("adx_alertsubscription", EntityRole.Referenced, "adx_alertsubscription_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string adx_alertsubscription_systemuser_modifiedonbehalfby = "adx_alertsubscription_systemuser_modifiedonbehalfby";
            [Relationship("adx_alertsubscription", EntityRole.Referenced, "adx_alertsubscription_systemuser_owninguser", "owninguser")]
            public const string adx_alertsubscription_systemuser_owninguser = "adx_alertsubscription_systemuser_owninguser";
            [Relationship("adx_inviteredemption", EntityRole.Referenced, "adx_inviteredemption_systemuser_createdby", "createdby")]
            public const string adx_inviteredemption_systemuser_createdby = "adx_inviteredemption_systemuser_createdby";
            [Relationship("adx_inviteredemption", EntityRole.Referenced, "adx_inviteredemption_systemuser_createdonbehalfby", "createdonbehalfby")]
            public const string adx_inviteredemption_systemuser_createdonbehalfby = "adx_inviteredemption_systemuser_createdonbehalfby";
            [Relationship("adx_inviteredemption", EntityRole.Referenced, "adx_inviteredemption_systemuser_modifiedby", "modifiedby")]
            public const string adx_inviteredemption_systemuser_modifiedby = "adx_inviteredemption_systemuser_modifiedby";
            [Relationship("adx_inviteredemption", EntityRole.Referenced, "adx_inviteredemption_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string adx_inviteredemption_systemuser_modifiedonbehalfby = "adx_inviteredemption_systemuser_modifiedonbehalfby";
            [Relationship("adx_inviteredemption", EntityRole.Referenced, "adx_inviteredemption_systemuser_owninguser", "owninguser")]
            public const string adx_inviteredemption_systemuser_owninguser = "adx_inviteredemption_systemuser_owninguser";
            [Relationship("adx_portalcomment", EntityRole.Referenced, "adx_portalcomment_systemuser_createdby", "createdby")]
            public const string adx_portalcomment_systemuser_createdby = "adx_portalcomment_systemuser_createdby";
            [Relationship("adx_portalcomment", EntityRole.Referenced, "adx_portalcomment_systemuser_createdonbehalfby", "createdonbehalfby")]
            public const string adx_portalcomment_systemuser_createdonbehalfby = "adx_portalcomment_systemuser_createdonbehalfby";
            [Relationship("adx_portalcomment", EntityRole.Referenced, "adx_portalcomment_systemuser_modifiedby", "modifiedby")]
            public const string adx_portalcomment_systemuser_modifiedby = "adx_portalcomment_systemuser_modifiedby";
            [Relationship("adx_portalcomment", EntityRole.Referenced, "adx_portalcomment_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string adx_portalcomment_systemuser_modifiedonbehalfby = "adx_portalcomment_systemuser_modifiedonbehalfby";
            [Relationship("adx_portalcomment", EntityRole.Referenced, "adx_portalcomment_systemuser_owninguser", "owninguser")]
            public const string adx_portalcomment_systemuser_owninguser = "adx_portalcomment_systemuser_owninguser";
            [Relationship("adx_webformsession", EntityRole.Referenced, "adx_webformsession_systemuser", "adx_systemuser")]
            public const string adx_webformsession_systemuser = "adx_webformsession_systemuser";
            [Relationship("annotation", EntityRole.Referenced, "annotation_owning_user", "owninguser")]
            public const string annotation_owning_user = "annotation_owning_user";
            [Relationship("businessunitmap", EntityRole.Referenced, "bizmap_systemuser_businessid", "businessid")]
            public const string bizmap_systemuser_businessid = "bizmap_systemuser_businessid";
            [Relationship("constraintbasedgroup", EntityRole.Referenced, "constraintbasedgroup_systemuser", "businessunitid")]
            public const string constraintbasedgroup_systemuser = "constraintbasedgroup_systemuser";
            [Relationship("contact", EntityRole.Referenced, "contact_owning_user", "owninguser")]
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
            [Relationship(PluginAssemblyDefinition.EntityName, EntityRole.Referenced, "createdby_pluginassembly", "createdby")]
            public const string createdby_pluginassembly = "createdby_pluginassembly";
            [Relationship("plugintracelog", EntityRole.Referenced, "createdby_plugintracelog", "createdby")]
            public const string createdby_plugintracelog = "createdby_plugintracelog";
            [Relationship(PluginTypeDefinition.EntityName, EntityRole.Referenced, "createdby_plugintype", "createdby")]
            public const string createdby_plugintype = "createdby_plugintype";
            [Relationship("plugintypestatistic", EntityRole.Referenced, "createdby_plugintypestatistic", "createdby")]
            public const string createdby_plugintypestatistic = "createdby_plugintypestatistic";
            [Relationship("relationshiprole", EntityRole.Referenced, "createdby_relationship_role", "createdby")]
            public const string createdby_relationship_role = "createdby_relationship_role";
            [Relationship("relationshiprolemap", EntityRole.Referenced, "createdby_relationship_role_map", "createdby")]
            public const string createdby_relationship_role_map = "createdby_relationship_role_map";
            [Relationship(SdkMessageDefinition.EntityName, EntityRole.Referenced, "createdby_sdkmessage", "createdby")]
            public const string createdby_sdkmessage = "createdby_sdkmessage";
            [Relationship(SdkMessageFilterDefinition.EntityName, EntityRole.Referenced, "createdby_sdkmessagefilter", "createdby")]
            public const string createdby_sdkmessagefilter = "createdby_sdkmessagefilter";
            [Relationship("sdkmessagepair", EntityRole.Referenced, "createdby_sdkmessagepair", "createdby")]
            public const string createdby_sdkmessagepair = "createdby_sdkmessagepair";
            [Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "createdby_sdkmessageprocessingstep", "createdby")]
            public const string createdby_sdkmessageprocessingstep = "createdby_sdkmessageprocessingstep";
            [Relationship(SdkMessageProcessingStepImageDefinition.EntityName, EntityRole.Referenced, "createdby_sdkmessageprocessingstepimage", "createdby")]
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
            [Relationship("dimsi_debugsession", EntityRole.Referenced, "dimsi_debugsession_user", "dimsi_debugeeid")]
            public const string dimsi_debugsession_user = "dimsi_debugsession_user";
            [Relationship("dynamicpropertyinstance", EntityRole.Referenced, "Dynamicpropertyinsatance_createdby", "createdby")]
            public const string Dynamicpropertyinsatance_createdby = "Dynamicpropertyinsatance_createdby";
            [Relationship("email", EntityRole.Referenced, "email_acceptingentity_systemuser", "acceptingentityid")]
            public const string email_acceptingentity_systemuser = "email_acceptingentity_systemuser";
            [Relationship("equipment", EntityRole.Referenced, "equipment_systemuser", "businessunitid")]
            public const string equipment_systemuser = "equipment_systemuser";
            [Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "impersonatinguserid_sdkmessageprocessingstep", "impersonatinguserid")]
            public const string impersonatinguserid_sdkmessageprocessingstep = "impersonatinguserid_sdkmessageprocessingstep";
            [Relationship("importfile", EntityRole.Referenced, "ImportFile_SystemUser", "recordsownerid")]
            public const string ImportFile_SystemUser = "ImportFile_SystemUser";
            [Relationship("knowledgearticle", EntityRole.Referenced, "knowledgearticle_primaryauthorid", "primaryauthorid")]
            public const string knowledgearticle_primaryauthorid = "knowledgearticle_primaryauthorid";
            [Relationship("lead", EntityRole.Referenced, "lead_owning_user", "owninguser")]
            public const string lead_owning_user = "lead_owning_user";
            [Relationship("account", EntityRole.Referenced, "lk_accountbase_createdby", "createdby")]
            public const string lk_accountbase_createdby = "lk_accountbase_createdby";
            [Relationship("account", EntityRole.Referenced, "lk_accountbase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_accountbase_createdonbehalfby = "lk_accountbase_createdonbehalfby";
            [Relationship("account", EntityRole.Referenced, "lk_accountbase_modifiedby", "modifiedby")]
            public const string lk_accountbase_modifiedby = "lk_accountbase_modifiedby";
            [Relationship("account", EntityRole.Referenced, "lk_accountbase_modifiedonbehalfby", "modifiedonbehalfby")]
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
            [Relationship("activityfileattachment", EntityRole.Referenced, "lk_activityfileattachment_createdby", "createdby")]
            public const string lk_activityfileattachment_createdby = "lk_activityfileattachment_createdby";
            [Relationship("activityfileattachment", EntityRole.Referenced, "lk_activityfileattachment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_activityfileattachment_createdonbehalfby = "lk_activityfileattachment_createdonbehalfby";
            [Relationship("activityfileattachment", EntityRole.Referenced, "lk_activityfileattachment_modifiedby", "modifiedby")]
            public const string lk_activityfileattachment_modifiedby = "lk_activityfileattachment_modifiedby";
            [Relationship("activityfileattachment", EntityRole.Referenced, "lk_activityfileattachment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_activityfileattachment_modifiedonbehalfby = "lk_activityfileattachment_modifiedonbehalfby";
            [Relationship("activitymonitor", EntityRole.Referenced, "lk_activitymonitor_createdby", "createdby")]
            public const string lk_activitymonitor_createdby = "lk_activitymonitor_createdby";
            [Relationship("activitymonitor", EntityRole.Referenced, "lk_activitymonitor_createdonbehalfby", "createdonbehalfby")]
            public const string lk_activitymonitor_createdonbehalfby = "lk_activitymonitor_createdonbehalfby";
            [Relationship("activitymonitor", EntityRole.Referenced, "lk_activitymonitor_modifiedby", "modifiedby")]
            public const string lk_activitymonitor_modifiedby = "lk_activitymonitor_modifiedby";
            [Relationship("activitymonitor", EntityRole.Referenced, "lk_activitymonitor_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_activitymonitor_modifiedonbehalfby = "lk_activitymonitor_modifiedonbehalfby";
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
            [Relationship("adx_ad", EntityRole.Referenced, "lk_adx_ad_createdby", "createdby")]
            public const string lk_adx_ad_createdby = "lk_adx_ad_createdby";
            [Relationship("adx_ad", EntityRole.Referenced, "lk_adx_ad_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_ad_createdonbehalfby = "lk_adx_ad_createdonbehalfby";
            [Relationship("adx_ad", EntityRole.Referenced, "lk_adx_ad_modifiedby", "modifiedby")]
            public const string lk_adx_ad_modifiedby = "lk_adx_ad_modifiedby";
            [Relationship("adx_ad", EntityRole.Referenced, "lk_adx_ad_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_ad_modifiedonbehalfby = "lk_adx_ad_modifiedonbehalfby";
            [Relationship("adx_adplacement", EntityRole.Referenced, "lk_adx_adplacement_createdby", "createdby")]
            public const string lk_adx_adplacement_createdby = "lk_adx_adplacement_createdby";
            [Relationship("adx_adplacement", EntityRole.Referenced, "lk_adx_adplacement_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_adplacement_createdonbehalfby = "lk_adx_adplacement_createdonbehalfby";
            [Relationship("adx_adplacement", EntityRole.Referenced, "lk_adx_adplacement_modifiedby", "modifiedby")]
            public const string lk_adx_adplacement_modifiedby = "lk_adx_adplacement_modifiedby";
            [Relationship("adx_adplacement", EntityRole.Referenced, "lk_adx_adplacement_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_adplacement_modifiedonbehalfby = "lk_adx_adplacement_modifiedonbehalfby";
            [Relationship("adx_badge", EntityRole.Referenced, "lk_adx_badge_createdby", "createdby")]
            public const string lk_adx_badge_createdby = "lk_adx_badge_createdby";
            [Relationship("adx_badge", EntityRole.Referenced, "lk_adx_badge_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_badge_createdonbehalfby = "lk_adx_badge_createdonbehalfby";
            [Relationship("adx_badge", EntityRole.Referenced, "lk_adx_badge_modifiedby", "modifiedby")]
            public const string lk_adx_badge_modifiedby = "lk_adx_badge_modifiedby";
            [Relationship("adx_badge", EntityRole.Referenced, "lk_adx_badge_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_badge_modifiedonbehalfby = "lk_adx_badge_modifiedonbehalfby";
            [Relationship("adx_badgetype", EntityRole.Referenced, "lk_adx_badgetype_createdby", "createdby")]
            public const string lk_adx_badgetype_createdby = "lk_adx_badgetype_createdby";
            [Relationship("adx_badgetype", EntityRole.Referenced, "lk_adx_badgetype_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_badgetype_createdonbehalfby = "lk_adx_badgetype_createdonbehalfby";
            [Relationship("adx_badgetype", EntityRole.Referenced, "lk_adx_badgetype_modifiedby", "modifiedby")]
            public const string lk_adx_badgetype_modifiedby = "lk_adx_badgetype_modifiedby";
            [Relationship("adx_badgetype", EntityRole.Referenced, "lk_adx_badgetype_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_badgetype_modifiedonbehalfby = "lk_adx_badgetype_modifiedonbehalfby";
            [Relationship("adx_bingmaplookup", EntityRole.Referenced, "lk_adx_bingmaplookup_createdby", "createdby")]
            public const string lk_adx_bingmaplookup_createdby = "lk_adx_bingmaplookup_createdby";
            [Relationship("adx_bingmaplookup", EntityRole.Referenced, "lk_adx_bingmaplookup_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_bingmaplookup_createdonbehalfby = "lk_adx_bingmaplookup_createdonbehalfby";
            [Relationship("adx_bingmaplookup", EntityRole.Referenced, "lk_adx_bingmaplookup_modifiedby", "modifiedby")]
            public const string lk_adx_bingmaplookup_modifiedby = "lk_adx_bingmaplookup_modifiedby";
            [Relationship("adx_bingmaplookup", EntityRole.Referenced, "lk_adx_bingmaplookup_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_bingmaplookup_modifiedonbehalfby = "lk_adx_bingmaplookup_modifiedonbehalfby";
            [Relationship("adx_blog", EntityRole.Referenced, "lk_adx_blog_createdby", "createdby")]
            public const string lk_adx_blog_createdby = "lk_adx_blog_createdby";
            [Relationship("adx_blog", EntityRole.Referenced, "lk_adx_blog_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_blog_createdonbehalfby = "lk_adx_blog_createdonbehalfby";
            [Relationship("adx_blog", EntityRole.Referenced, "lk_adx_blog_modifiedby", "modifiedby")]
            public const string lk_adx_blog_modifiedby = "lk_adx_blog_modifiedby";
            [Relationship("adx_blog", EntityRole.Referenced, "lk_adx_blog_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_blog_modifiedonbehalfby = "lk_adx_blog_modifiedonbehalfby";
            [Relationship("adx_blogpost", EntityRole.Referenced, "lk_adx_blogpost_createdby", "createdby")]
            public const string lk_adx_blogpost_createdby = "lk_adx_blogpost_createdby";
            [Relationship("adx_blogpost", EntityRole.Referenced, "lk_adx_blogpost_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_blogpost_createdonbehalfby = "lk_adx_blogpost_createdonbehalfby";
            [Relationship("adx_blogpost", EntityRole.Referenced, "lk_adx_blogpost_modifiedby", "modifiedby")]
            public const string lk_adx_blogpost_modifiedby = "lk_adx_blogpost_modifiedby";
            [Relationship("adx_blogpost", EntityRole.Referenced, "lk_adx_blogpost_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_blogpost_modifiedonbehalfby = "lk_adx_blogpost_modifiedonbehalfby";
            [Relationship("adx_botconsumer", EntityRole.Referenced, "lk_adx_botconsumer_createdby", "createdby")]
            public const string lk_adx_botconsumer_createdby = "lk_adx_botconsumer_createdby";
            [Relationship("adx_botconsumer", EntityRole.Referenced, "lk_adx_botconsumer_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_botconsumer_createdonbehalfby = "lk_adx_botconsumer_createdonbehalfby";
            [Relationship("adx_botconsumer", EntityRole.Referenced, "lk_adx_botconsumer_modifiedby", "modifiedby")]
            public const string lk_adx_botconsumer_modifiedby = "lk_adx_botconsumer_modifiedby";
            [Relationship("adx_botconsumer", EntityRole.Referenced, "lk_adx_botconsumer_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_botconsumer_modifiedonbehalfby = "lk_adx_botconsumer_modifiedonbehalfby";
            [Relationship("adx_bpf_c2857b638fa7473d8e2f112c232cebd8", EntityRole.Referenced, "lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_createdby", "createdby")]
            public const string lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_createdby = "lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_createdby";
            [Relationship("adx_bpf_c2857b638fa7473d8e2f112c232cebd8", EntityRole.Referenced, "lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_createdonbehalfby = "lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_createdonbehalfby";
            [Relationship("adx_bpf_c2857b638fa7473d8e2f112c232cebd8", EntityRole.Referenced, "lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_modifiedby", "modifiedby")]
            public const string lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_modifiedby = "lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_modifiedby";
            [Relationship("adx_bpf_c2857b638fa7473d8e2f112c232cebd8", EntityRole.Referenced, "lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_modifiedonbehalfby = "lk_adx_bpf_c2857b638fa7473d8e2f112c232cebd8_modifiedonbehalfby";
            [Relationship("adx_casedeflection", EntityRole.Referenced, "lk_adx_casedeflection_createdby", "createdby")]
            public const string lk_adx_casedeflection_createdby = "lk_adx_casedeflection_createdby";
            [Relationship("adx_casedeflection", EntityRole.Referenced, "lk_adx_casedeflection_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_casedeflection_createdonbehalfby = "lk_adx_casedeflection_createdonbehalfby";
            [Relationship("adx_casedeflection", EntityRole.Referenced, "lk_adx_casedeflection_modifiedby", "modifiedby")]
            public const string lk_adx_casedeflection_modifiedby = "lk_adx_casedeflection_modifiedby";
            [Relationship("adx_casedeflection", EntityRole.Referenced, "lk_adx_casedeflection_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_casedeflection_modifiedonbehalfby = "lk_adx_casedeflection_modifiedonbehalfby";
            [Relationship("adx_cmpallowedreason", EntityRole.Referenced, "lk_adx_cmpallowedreason_createdby", "createdby")]
            public const string lk_adx_cmpallowedreason_createdby = "lk_adx_cmpallowedreason_createdby";
            [Relationship("adx_cmpallowedreason", EntityRole.Referenced, "lk_adx_cmpallowedreason_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_cmpallowedreason_createdonbehalfby = "lk_adx_cmpallowedreason_createdonbehalfby";
            [Relationship("adx_cmpallowedreason", EntityRole.Referenced, "lk_adx_cmpallowedreason_modifiedby", "modifiedby")]
            public const string lk_adx_cmpallowedreason_modifiedby = "lk_adx_cmpallowedreason_modifiedby";
            [Relationship("adx_cmpallowedreason", EntityRole.Referenced, "lk_adx_cmpallowedreason_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_cmpallowedreason_modifiedonbehalfby = "lk_adx_cmpallowedreason_modifiedonbehalfby";
            [Relationship("adx_columnpermission", EntityRole.Referenced, "lk_adx_columnpermission_createdby", "createdby")]
            public const string lk_adx_columnpermission_createdby = "lk_adx_columnpermission_createdby";
            [Relationship("adx_columnpermission", EntityRole.Referenced, "lk_adx_columnpermission_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_columnpermission_createdonbehalfby = "lk_adx_columnpermission_createdonbehalfby";
            [Relationship("adx_columnpermission", EntityRole.Referenced, "lk_adx_columnpermission_modifiedby", "modifiedby")]
            public const string lk_adx_columnpermission_modifiedby = "lk_adx_columnpermission_modifiedby";
            [Relationship("adx_columnpermission", EntityRole.Referenced, "lk_adx_columnpermission_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_columnpermission_modifiedonbehalfby = "lk_adx_columnpermission_modifiedonbehalfby";
            [Relationship("adx_columnpermissionprofile", EntityRole.Referenced, "lk_adx_columnpermissionprofile_createdby", "createdby")]
            public const string lk_adx_columnpermissionprofile_createdby = "lk_adx_columnpermissionprofile_createdby";
            [Relationship("adx_columnpermissionprofile", EntityRole.Referenced, "lk_adx_columnpermissionprofile_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_columnpermissionprofile_createdonbehalfby = "lk_adx_columnpermissionprofile_createdonbehalfby";
            [Relationship("adx_columnpermissionprofile", EntityRole.Referenced, "lk_adx_columnpermissionprofile_modifiedby", "modifiedby")]
            public const string lk_adx_columnpermissionprofile_modifiedby = "lk_adx_columnpermissionprofile_modifiedby";
            [Relationship("adx_columnpermissionprofile", EntityRole.Referenced, "lk_adx_columnpermissionprofile_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_columnpermissionprofile_modifiedonbehalfby = "lk_adx_columnpermissionprofile_modifiedonbehalfby";
            [Relationship("adx_communityforum", EntityRole.Referenced, "lk_adx_communityforum_createdby", "createdby")]
            public const string lk_adx_communityforum_createdby = "lk_adx_communityforum_createdby";
            [Relationship("adx_communityforum", EntityRole.Referenced, "lk_adx_communityforum_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_communityforum_createdonbehalfby = "lk_adx_communityforum_createdonbehalfby";
            [Relationship("adx_communityforum", EntityRole.Referenced, "lk_adx_communityforum_modifiedby", "modifiedby")]
            public const string lk_adx_communityforum_modifiedby = "lk_adx_communityforum_modifiedby";
            [Relationship("adx_communityforum", EntityRole.Referenced, "lk_adx_communityforum_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_communityforum_modifiedonbehalfby = "lk_adx_communityforum_modifiedonbehalfby";
            [Relationship("adx_communityforumaccesspermission", EntityRole.Referenced, "lk_adx_communityforumaccesspermission_createdby", "createdby")]
            public const string lk_adx_communityforumaccesspermission_createdby = "lk_adx_communityforumaccesspermission_createdby";
            [Relationship("adx_communityforumaccesspermission", EntityRole.Referenced, "lk_adx_communityforumaccesspermission_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_communityforumaccesspermission_createdonbehalfby = "lk_adx_communityforumaccesspermission_createdonbehalfby";
            [Relationship("adx_communityforumaccesspermission", EntityRole.Referenced, "lk_adx_communityforumaccesspermission_modifiedby", "modifiedby")]
            public const string lk_adx_communityforumaccesspermission_modifiedby = "lk_adx_communityforumaccesspermission_modifiedby";
            [Relationship("adx_communityforumaccesspermission", EntityRole.Referenced, "lk_adx_communityforumaccesspermission_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_communityforumaccesspermission_modifiedonbehalfby = "lk_adx_communityforumaccesspermission_modifiedonbehalfby";
            [Relationship("adx_communityforumalert", EntityRole.Referenced, "lk_adx_communityforumalert_createdby", "createdby")]
            public const string lk_adx_communityforumalert_createdby = "lk_adx_communityforumalert_createdby";
            [Relationship("adx_communityforumalert", EntityRole.Referenced, "lk_adx_communityforumalert_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_communityforumalert_createdonbehalfby = "lk_adx_communityforumalert_createdonbehalfby";
            [Relationship("adx_communityforumalert", EntityRole.Referenced, "lk_adx_communityforumalert_modifiedby", "modifiedby")]
            public const string lk_adx_communityforumalert_modifiedby = "lk_adx_communityforumalert_modifiedby";
            [Relationship("adx_communityforumalert", EntityRole.Referenced, "lk_adx_communityforumalert_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_communityforumalert_modifiedonbehalfby = "lk_adx_communityforumalert_modifiedonbehalfby";
            [Relationship("adx_communityforumannouncement", EntityRole.Referenced, "lk_adx_communityforumannouncement_createdby", "createdby")]
            public const string lk_adx_communityforumannouncement_createdby = "lk_adx_communityforumannouncement_createdby";
            [Relationship("adx_communityforumannouncement", EntityRole.Referenced, "lk_adx_communityforumannouncement_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_communityforumannouncement_createdonbehalfby = "lk_adx_communityforumannouncement_createdonbehalfby";
            [Relationship("adx_communityforumannouncement", EntityRole.Referenced, "lk_adx_communityforumannouncement_modifiedby", "modifiedby")]
            public const string lk_adx_communityforumannouncement_modifiedby = "lk_adx_communityforumannouncement_modifiedby";
            [Relationship("adx_communityforumannouncement", EntityRole.Referenced, "lk_adx_communityforumannouncement_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_communityforumannouncement_modifiedonbehalfby = "lk_adx_communityforumannouncement_modifiedonbehalfby";
            [Relationship("adx_communityforumpost", EntityRole.Referenced, "lk_adx_communityforumpost_createdby", "createdby")]
            public const string lk_adx_communityforumpost_createdby = "lk_adx_communityforumpost_createdby";
            [Relationship("adx_communityforumpost", EntityRole.Referenced, "lk_adx_communityforumpost_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_communityforumpost_createdonbehalfby = "lk_adx_communityforumpost_createdonbehalfby";
            [Relationship("adx_communityforumpost", EntityRole.Referenced, "lk_adx_communityforumpost_modifiedby", "modifiedby")]
            public const string lk_adx_communityforumpost_modifiedby = "lk_adx_communityforumpost_modifiedby";
            [Relationship("adx_communityforumpost", EntityRole.Referenced, "lk_adx_communityforumpost_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_communityforumpost_modifiedonbehalfby = "lk_adx_communityforumpost_modifiedonbehalfby";
            [Relationship("adx_communityforumthread", EntityRole.Referenced, "lk_adx_communityforumthread_createdby", "createdby")]
            public const string lk_adx_communityforumthread_createdby = "lk_adx_communityforumthread_createdby";
            [Relationship("adx_communityforumthread", EntityRole.Referenced, "lk_adx_communityforumthread_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_communityforumthread_createdonbehalfby = "lk_adx_communityforumthread_createdonbehalfby";
            [Relationship("adx_communityforumthread", EntityRole.Referenced, "lk_adx_communityforumthread_modifiedby", "modifiedby")]
            public const string lk_adx_communityforumthread_modifiedby = "lk_adx_communityforumthread_modifiedby";
            [Relationship("adx_communityforumthread", EntityRole.Referenced, "lk_adx_communityforumthread_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_communityforumthread_modifiedonbehalfby = "lk_adx_communityforumthread_modifiedonbehalfby";
            [Relationship("adx_contentaccesslevel", EntityRole.Referenced, "lk_adx_contentaccesslevel_createdby", "createdby")]
            public const string lk_adx_contentaccesslevel_createdby = "lk_adx_contentaccesslevel_createdby";
            [Relationship("adx_contentaccesslevel", EntityRole.Referenced, "lk_adx_contentaccesslevel_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_contentaccesslevel_createdonbehalfby = "lk_adx_contentaccesslevel_createdonbehalfby";
            [Relationship("adx_contentaccesslevel", EntityRole.Referenced, "lk_adx_contentaccesslevel_modifiedby", "modifiedby")]
            public const string lk_adx_contentaccesslevel_modifiedby = "lk_adx_contentaccesslevel_modifiedby";
            [Relationship("adx_contentaccesslevel", EntityRole.Referenced, "lk_adx_contentaccesslevel_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_contentaccesslevel_modifiedonbehalfby = "lk_adx_contentaccesslevel_modifiedonbehalfby";
            [Relationship("adx_contentmoderationpolicy", EntityRole.Referenced, "lk_adx_contentmoderationpolicy_createdby", "createdby")]
            public const string lk_adx_contentmoderationpolicy_createdby = "lk_adx_contentmoderationpolicy_createdby";
            [Relationship("adx_contentmoderationpolicy", EntityRole.Referenced, "lk_adx_contentmoderationpolicy_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_contentmoderationpolicy_createdonbehalfby = "lk_adx_contentmoderationpolicy_createdonbehalfby";
            [Relationship("adx_contentmoderationpolicy", EntityRole.Referenced, "lk_adx_contentmoderationpolicy_modifiedby", "modifiedby")]
            public const string lk_adx_contentmoderationpolicy_modifiedby = "lk_adx_contentmoderationpolicy_modifiedby";
            [Relationship("adx_contentmoderationpolicy", EntityRole.Referenced, "lk_adx_contentmoderationpolicy_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_contentmoderationpolicy_modifiedonbehalfby = "lk_adx_contentmoderationpolicy_modifiedonbehalfby";
            [Relationship("adx_contentsnippet", EntityRole.Referenced, "lk_adx_contentsnippet_createdby", "createdby")]
            public const string lk_adx_contentsnippet_createdby = "lk_adx_contentsnippet_createdby";
            [Relationship("adx_contentsnippet", EntityRole.Referenced, "lk_adx_contentsnippet_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_contentsnippet_createdonbehalfby = "lk_adx_contentsnippet_createdonbehalfby";
            [Relationship("adx_contentsnippet", EntityRole.Referenced, "lk_adx_contentsnippet_modifiedby", "modifiedby")]
            public const string lk_adx_contentsnippet_modifiedby = "lk_adx_contentsnippet_modifiedby";
            [Relationship("adx_contentsnippet", EntityRole.Referenced, "lk_adx_contentsnippet_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_contentsnippet_modifiedonbehalfby = "lk_adx_contentsnippet_modifiedonbehalfby";
            [Relationship("adx_entityform", EntityRole.Referenced, "lk_adx_entityform_createdby", "createdby")]
            public const string lk_adx_entityform_createdby = "lk_adx_entityform_createdby";
            [Relationship("adx_entityform", EntityRole.Referenced, "lk_adx_entityform_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_entityform_createdonbehalfby = "lk_adx_entityform_createdonbehalfby";
            [Relationship("adx_entityform", EntityRole.Referenced, "lk_adx_entityform_modifiedby", "modifiedby")]
            public const string lk_adx_entityform_modifiedby = "lk_adx_entityform_modifiedby";
            [Relationship("adx_entityform", EntityRole.Referenced, "lk_adx_entityform_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_entityform_modifiedonbehalfby = "lk_adx_entityform_modifiedonbehalfby";
            [Relationship("adx_entityformmetadata", EntityRole.Referenced, "lk_adx_entityformmetadata_createdby", "createdby")]
            public const string lk_adx_entityformmetadata_createdby = "lk_adx_entityformmetadata_createdby";
            [Relationship("adx_entityformmetadata", EntityRole.Referenced, "lk_adx_entityformmetadata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_entityformmetadata_createdonbehalfby = "lk_adx_entityformmetadata_createdonbehalfby";
            [Relationship("adx_entityformmetadata", EntityRole.Referenced, "lk_adx_entityformmetadata_modifiedby", "modifiedby")]
            public const string lk_adx_entityformmetadata_modifiedby = "lk_adx_entityformmetadata_modifiedby";
            [Relationship("adx_entityformmetadata", EntityRole.Referenced, "lk_adx_entityformmetadata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_entityformmetadata_modifiedonbehalfby = "lk_adx_entityformmetadata_modifiedonbehalfby";
            [Relationship("adx_entitylist", EntityRole.Referenced, "lk_adx_entitylist_createdby", "createdby")]
            public const string lk_adx_entitylist_createdby = "lk_adx_entitylist_createdby";
            [Relationship("adx_entitylist", EntityRole.Referenced, "lk_adx_entitylist_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_entitylist_createdonbehalfby = "lk_adx_entitylist_createdonbehalfby";
            [Relationship("adx_entitylist", EntityRole.Referenced, "lk_adx_entitylist_modifiedby", "modifiedby")]
            public const string lk_adx_entitylist_modifiedby = "lk_adx_entitylist_modifiedby";
            [Relationship("adx_entitylist", EntityRole.Referenced, "lk_adx_entitylist_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_entitylist_modifiedonbehalfby = "lk_adx_entitylist_modifiedonbehalfby";
            [Relationship("adx_entitypermission", EntityRole.Referenced, "lk_adx_entitypermission_createdby", "createdby")]
            public const string lk_adx_entitypermission_createdby = "lk_adx_entitypermission_createdby";
            [Relationship("adx_entitypermission", EntityRole.Referenced, "lk_adx_entitypermission_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_entitypermission_createdonbehalfby = "lk_adx_entitypermission_createdonbehalfby";
            [Relationship("adx_entitypermission", EntityRole.Referenced, "lk_adx_entitypermission_modifiedby", "modifiedby")]
            public const string lk_adx_entitypermission_modifiedby = "lk_adx_entitypermission_modifiedby";
            [Relationship("adx_entitypermission", EntityRole.Referenced, "lk_adx_entitypermission_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_entitypermission_modifiedonbehalfby = "lk_adx_entitypermission_modifiedonbehalfby";
            [Relationship("adx_externalidentity", EntityRole.Referenced, "lk_adx_externalidentity_createdby", "createdby")]
            public const string lk_adx_externalidentity_createdby = "lk_adx_externalidentity_createdby";
            [Relationship("adx_externalidentity", EntityRole.Referenced, "lk_adx_externalidentity_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_externalidentity_createdonbehalfby = "lk_adx_externalidentity_createdonbehalfby";
            [Relationship("adx_externalidentity", EntityRole.Referenced, "lk_adx_externalidentity_modifiedby", "modifiedby")]
            public const string lk_adx_externalidentity_modifiedby = "lk_adx_externalidentity_modifiedby";
            [Relationship("adx_externalidentity", EntityRole.Referenced, "lk_adx_externalidentity_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_externalidentity_modifiedonbehalfby = "lk_adx_externalidentity_modifiedonbehalfby";
            [Relationship("adx_forumnotification", EntityRole.Referenced, "lk_adx_forumnotification_createdby", "createdby")]
            public const string lk_adx_forumnotification_createdby = "lk_adx_forumnotification_createdby";
            [Relationship("adx_forumnotification", EntityRole.Referenced, "lk_adx_forumnotification_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_forumnotification_createdonbehalfby = "lk_adx_forumnotification_createdonbehalfby";
            [Relationship("adx_forumnotification", EntityRole.Referenced, "lk_adx_forumnotification_modifiedby", "modifiedby")]
            public const string lk_adx_forumnotification_modifiedby = "lk_adx_forumnotification_modifiedby";
            [Relationship("adx_forumnotification", EntityRole.Referenced, "lk_adx_forumnotification_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_forumnotification_modifiedonbehalfby = "lk_adx_forumnotification_modifiedonbehalfby";
            [Relationship("adx_forumthreadtype", EntityRole.Referenced, "lk_adx_forumthreadtype_createdby", "createdby")]
            public const string lk_adx_forumthreadtype_createdby = "lk_adx_forumthreadtype_createdby";
            [Relationship("adx_forumthreadtype", EntityRole.Referenced, "lk_adx_forumthreadtype_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_forumthreadtype_createdonbehalfby = "lk_adx_forumthreadtype_createdonbehalfby";
            [Relationship("adx_forumthreadtype", EntityRole.Referenced, "lk_adx_forumthreadtype_modifiedby", "modifiedby")]
            public const string lk_adx_forumthreadtype_modifiedby = "lk_adx_forumthreadtype_modifiedby";
            [Relationship("adx_forumthreadtype", EntityRole.Referenced, "lk_adx_forumthreadtype_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_forumthreadtype_modifiedonbehalfby = "lk_adx_forumthreadtype_modifiedonbehalfby";
            [Relationship("adx_idea", EntityRole.Referenced, "lk_adx_idea_createdby", "createdby")]
            public const string lk_adx_idea_createdby = "lk_adx_idea_createdby";
            [Relationship("adx_idea", EntityRole.Referenced, "lk_adx_idea_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_idea_createdonbehalfby = "lk_adx_idea_createdonbehalfby";
            [Relationship("adx_idea", EntityRole.Referenced, "lk_adx_idea_modifiedby", "modifiedby")]
            public const string lk_adx_idea_modifiedby = "lk_adx_idea_modifiedby";
            [Relationship("adx_idea", EntityRole.Referenced, "lk_adx_idea_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_idea_modifiedonbehalfby = "lk_adx_idea_modifiedonbehalfby";
            [Relationship("adx_ideaforum", EntityRole.Referenced, "lk_adx_ideaforum_createdby", "createdby")]
            public const string lk_adx_ideaforum_createdby = "lk_adx_ideaforum_createdby";
            [Relationship("adx_ideaforum", EntityRole.Referenced, "lk_adx_ideaforum_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_ideaforum_createdonbehalfby = "lk_adx_ideaforum_createdonbehalfby";
            [Relationship("adx_ideaforum", EntityRole.Referenced, "lk_adx_ideaforum_modifiedby", "modifiedby")]
            public const string lk_adx_ideaforum_modifiedby = "lk_adx_ideaforum_modifiedby";
            [Relationship("adx_ideaforum", EntityRole.Referenced, "lk_adx_ideaforum_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_ideaforum_modifiedonbehalfby = "lk_adx_ideaforum_modifiedonbehalfby";
            [Relationship("adx_invitation", EntityRole.Referenced, "lk_adx_invitation_createdby", "createdby")]
            public const string lk_adx_invitation_createdby = "lk_adx_invitation_createdby";
            [Relationship("adx_invitation", EntityRole.Referenced, "lk_adx_invitation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_invitation_createdonbehalfby = "lk_adx_invitation_createdonbehalfby";
            [Relationship("adx_invitation", EntityRole.Referenced, "lk_adx_invitation_modifiedby", "modifiedby")]
            public const string lk_adx_invitation_modifiedby = "lk_adx_invitation_modifiedby";
            [Relationship("adx_invitation", EntityRole.Referenced, "lk_adx_invitation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_invitation_modifiedonbehalfby = "lk_adx_invitation_modifiedonbehalfby";
            [Relationship("adx_pagealert", EntityRole.Referenced, "lk_adx_pagealert_createdby", "createdby")]
            public const string lk_adx_pagealert_createdby = "lk_adx_pagealert_createdby";
            [Relationship("adx_pagealert", EntityRole.Referenced, "lk_adx_pagealert_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_pagealert_createdonbehalfby = "lk_adx_pagealert_createdonbehalfby";
            [Relationship("adx_pagealert", EntityRole.Referenced, "lk_adx_pagealert_modifiedby", "modifiedby")]
            public const string lk_adx_pagealert_modifiedby = "lk_adx_pagealert_modifiedby";
            [Relationship("adx_pagealert", EntityRole.Referenced, "lk_adx_pagealert_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_pagealert_modifiedonbehalfby = "lk_adx_pagealert_modifiedonbehalfby";
            [Relationship("adx_pagenotification", EntityRole.Referenced, "lk_adx_pagenotification_createdby", "createdby")]
            public const string lk_adx_pagenotification_createdby = "lk_adx_pagenotification_createdby";
            [Relationship("adx_pagenotification", EntityRole.Referenced, "lk_adx_pagenotification_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_pagenotification_createdonbehalfby = "lk_adx_pagenotification_createdonbehalfby";
            [Relationship("adx_pagenotification", EntityRole.Referenced, "lk_adx_pagenotification_modifiedby", "modifiedby")]
            public const string lk_adx_pagenotification_modifiedby = "lk_adx_pagenotification_modifiedby";
            [Relationship("adx_pagenotification", EntityRole.Referenced, "lk_adx_pagenotification_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_pagenotification_modifiedonbehalfby = "lk_adx_pagenotification_modifiedonbehalfby";
            [Relationship("adx_pagetag", EntityRole.Referenced, "lk_adx_pagetag_createdby", "createdby")]
            public const string lk_adx_pagetag_createdby = "lk_adx_pagetag_createdby";
            [Relationship("adx_pagetag", EntityRole.Referenced, "lk_adx_pagetag_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_pagetag_createdonbehalfby = "lk_adx_pagetag_createdonbehalfby";
            [Relationship("adx_pagetag", EntityRole.Referenced, "lk_adx_pagetag_modifiedby", "modifiedby")]
            public const string lk_adx_pagetag_modifiedby = "lk_adx_pagetag_modifiedby";
            [Relationship("adx_pagetag", EntityRole.Referenced, "lk_adx_pagetag_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_pagetag_modifiedonbehalfby = "lk_adx_pagetag_modifiedonbehalfby";
            [Relationship("adx_pagetemplate", EntityRole.Referenced, "lk_adx_pagetemplate_createdby", "createdby")]
            public const string lk_adx_pagetemplate_createdby = "lk_adx_pagetemplate_createdby";
            [Relationship("adx_pagetemplate", EntityRole.Referenced, "lk_adx_pagetemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_pagetemplate_createdonbehalfby = "lk_adx_pagetemplate_createdonbehalfby";
            [Relationship("adx_pagetemplate", EntityRole.Referenced, "lk_adx_pagetemplate_modifiedby", "modifiedby")]
            public const string lk_adx_pagetemplate_modifiedby = "lk_adx_pagetemplate_modifiedby";
            [Relationship("adx_pagetemplate", EntityRole.Referenced, "lk_adx_pagetemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_pagetemplate_modifiedonbehalfby = "lk_adx_pagetemplate_modifiedonbehalfby";
            [Relationship("adx_poll", EntityRole.Referenced, "lk_adx_poll_createdby", "createdby")]
            public const string lk_adx_poll_createdby = "lk_adx_poll_createdby";
            [Relationship("adx_poll", EntityRole.Referenced, "lk_adx_poll_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_poll_createdonbehalfby = "lk_adx_poll_createdonbehalfby";
            [Relationship("adx_poll", EntityRole.Referenced, "lk_adx_poll_modifiedby", "modifiedby")]
            public const string lk_adx_poll_modifiedby = "lk_adx_poll_modifiedby";
            [Relationship("adx_poll", EntityRole.Referenced, "lk_adx_poll_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_poll_modifiedonbehalfby = "lk_adx_poll_modifiedonbehalfby";
            [Relationship("adx_polloption", EntityRole.Referenced, "lk_adx_polloption_createdby", "createdby")]
            public const string lk_adx_polloption_createdby = "lk_adx_polloption_createdby";
            [Relationship("adx_polloption", EntityRole.Referenced, "lk_adx_polloption_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_polloption_createdonbehalfby = "lk_adx_polloption_createdonbehalfby";
            [Relationship("adx_polloption", EntityRole.Referenced, "lk_adx_polloption_modifiedby", "modifiedby")]
            public const string lk_adx_polloption_modifiedby = "lk_adx_polloption_modifiedby";
            [Relationship("adx_polloption", EntityRole.Referenced, "lk_adx_polloption_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_polloption_modifiedonbehalfby = "lk_adx_polloption_modifiedonbehalfby";
            [Relationship("adx_pollplacement", EntityRole.Referenced, "lk_adx_pollplacement_createdby", "createdby")]
            public const string lk_adx_pollplacement_createdby = "lk_adx_pollplacement_createdby";
            [Relationship("adx_pollplacement", EntityRole.Referenced, "lk_adx_pollplacement_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_pollplacement_createdonbehalfby = "lk_adx_pollplacement_createdonbehalfby";
            [Relationship("adx_pollplacement", EntityRole.Referenced, "lk_adx_pollplacement_modifiedby", "modifiedby")]
            public const string lk_adx_pollplacement_modifiedby = "lk_adx_pollplacement_modifiedby";
            [Relationship("adx_pollplacement", EntityRole.Referenced, "lk_adx_pollplacement_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_pollplacement_modifiedonbehalfby = "lk_adx_pollplacement_modifiedonbehalfby";
            [Relationship("adx_pollsubmission", EntityRole.Referenced, "lk_adx_pollsubmission_createdby", "createdby")]
            public const string lk_adx_pollsubmission_createdby = "lk_adx_pollsubmission_createdby";
            [Relationship("adx_pollsubmission", EntityRole.Referenced, "lk_adx_pollsubmission_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_pollsubmission_createdonbehalfby = "lk_adx_pollsubmission_createdonbehalfby";
            [Relationship("adx_pollsubmission", EntityRole.Referenced, "lk_adx_pollsubmission_modifiedby", "modifiedby")]
            public const string lk_adx_pollsubmission_modifiedby = "lk_adx_pollsubmission_modifiedby";
            [Relationship("adx_pollsubmission", EntityRole.Referenced, "lk_adx_pollsubmission_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_pollsubmission_modifiedonbehalfby = "lk_adx_pollsubmission_modifiedonbehalfby";
            [Relationship("adx_portallanguage", EntityRole.Referenced, "lk_adx_portallanguage_createdby", "createdby")]
            public const string lk_adx_portallanguage_createdby = "lk_adx_portallanguage_createdby";
            [Relationship("adx_portallanguage", EntityRole.Referenced, "lk_adx_portallanguage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_portallanguage_createdonbehalfby = "lk_adx_portallanguage_createdonbehalfby";
            [Relationship("adx_portallanguage", EntityRole.Referenced, "lk_adx_portallanguage_modifiedby", "modifiedby")]
            public const string lk_adx_portallanguage_modifiedby = "lk_adx_portallanguage_modifiedby";
            [Relationship("adx_portallanguage", EntityRole.Referenced, "lk_adx_portallanguage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_portallanguage_modifiedonbehalfby = "lk_adx_portallanguage_modifiedonbehalfby";
            [Relationship("adx_publishingstate", EntityRole.Referenced, "lk_adx_publishingstate_createdby", "createdby")]
            public const string lk_adx_publishingstate_createdby = "lk_adx_publishingstate_createdby";
            [Relationship("adx_publishingstate", EntityRole.Referenced, "lk_adx_publishingstate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_publishingstate_createdonbehalfby = "lk_adx_publishingstate_createdonbehalfby";
            [Relationship("adx_publishingstate", EntityRole.Referenced, "lk_adx_publishingstate_modifiedby", "modifiedby")]
            public const string lk_adx_publishingstate_modifiedby = "lk_adx_publishingstate_modifiedby";
            [Relationship("adx_publishingstate", EntityRole.Referenced, "lk_adx_publishingstate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_publishingstate_modifiedonbehalfby = "lk_adx_publishingstate_modifiedonbehalfby";
            [Relationship("adx_publishingstatetransitionrule", EntityRole.Referenced, "lk_adx_publishingstatetransitionrule_createdby", "createdby")]
            public const string lk_adx_publishingstatetransitionrule_createdby = "lk_adx_publishingstatetransitionrule_createdby";
            [Relationship("adx_publishingstatetransitionrule", EntityRole.Referenced, "lk_adx_publishingstatetransitionrule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_publishingstatetransitionrule_createdonbehalfby = "lk_adx_publishingstatetransitionrule_createdonbehalfby";
            [Relationship("adx_publishingstatetransitionrule", EntityRole.Referenced, "lk_adx_publishingstatetransitionrule_modifiedby", "modifiedby")]
            public const string lk_adx_publishingstatetransitionrule_modifiedby = "lk_adx_publishingstatetransitionrule_modifiedby";
            [Relationship("adx_publishingstatetransitionrule", EntityRole.Referenced, "lk_adx_publishingstatetransitionrule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_publishingstatetransitionrule_modifiedonbehalfby = "lk_adx_publishingstatetransitionrule_modifiedonbehalfby";
            [Relationship("adx_redirect", EntityRole.Referenced, "lk_adx_redirect_createdby", "createdby")]
            public const string lk_adx_redirect_createdby = "lk_adx_redirect_createdby";
            [Relationship("adx_redirect", EntityRole.Referenced, "lk_adx_redirect_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_redirect_createdonbehalfby = "lk_adx_redirect_createdonbehalfby";
            [Relationship("adx_redirect", EntityRole.Referenced, "lk_adx_redirect_modifiedby", "modifiedby")]
            public const string lk_adx_redirect_modifiedby = "lk_adx_redirect_modifiedby";
            [Relationship("adx_redirect", EntityRole.Referenced, "lk_adx_redirect_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_redirect_modifiedonbehalfby = "lk_adx_redirect_modifiedonbehalfby";
            [Relationship("adx_setting", EntityRole.Referenced, "lk_adx_setting_createdby", "createdby")]
            public const string lk_adx_setting_createdby = "lk_adx_setting_createdby";
            [Relationship("adx_setting", EntityRole.Referenced, "lk_adx_setting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_setting_createdonbehalfby = "lk_adx_setting_createdonbehalfby";
            [Relationship("adx_setting", EntityRole.Referenced, "lk_adx_setting_modifiedby", "modifiedby")]
            public const string lk_adx_setting_modifiedby = "lk_adx_setting_modifiedby";
            [Relationship("adx_setting", EntityRole.Referenced, "lk_adx_setting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_setting_modifiedonbehalfby = "lk_adx_setting_modifiedonbehalfby";
            [Relationship("adx_shortcut", EntityRole.Referenced, "lk_adx_shortcut_createdby", "createdby")]
            public const string lk_adx_shortcut_createdby = "lk_adx_shortcut_createdby";
            [Relationship("adx_shortcut", EntityRole.Referenced, "lk_adx_shortcut_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_shortcut_createdonbehalfby = "lk_adx_shortcut_createdonbehalfby";
            [Relationship("adx_shortcut", EntityRole.Referenced, "lk_adx_shortcut_modifiedby", "modifiedby")]
            public const string lk_adx_shortcut_modifiedby = "lk_adx_shortcut_modifiedby";
            [Relationship("adx_shortcut", EntityRole.Referenced, "lk_adx_shortcut_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_shortcut_modifiedonbehalfby = "lk_adx_shortcut_modifiedonbehalfby";
            [Relationship("adx_sitemarker", EntityRole.Referenced, "lk_adx_sitemarker_createdby", "createdby")]
            public const string lk_adx_sitemarker_createdby = "lk_adx_sitemarker_createdby";
            [Relationship("adx_sitemarker", EntityRole.Referenced, "lk_adx_sitemarker_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_sitemarker_createdonbehalfby = "lk_adx_sitemarker_createdonbehalfby";
            [Relationship("adx_sitemarker", EntityRole.Referenced, "lk_adx_sitemarker_modifiedby", "modifiedby")]
            public const string lk_adx_sitemarker_modifiedby = "lk_adx_sitemarker_modifiedby";
            [Relationship("adx_sitemarker", EntityRole.Referenced, "lk_adx_sitemarker_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_sitemarker_modifiedonbehalfby = "lk_adx_sitemarker_modifiedonbehalfby";
            [Relationship("adx_sitesetting", EntityRole.Referenced, "lk_adx_sitesetting_createdby", "createdby")]
            public const string lk_adx_sitesetting_createdby = "lk_adx_sitesetting_createdby";
            [Relationship("adx_sitesetting", EntityRole.Referenced, "lk_adx_sitesetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_sitesetting_createdonbehalfby = "lk_adx_sitesetting_createdonbehalfby";
            [Relationship("adx_sitesetting", EntityRole.Referenced, "lk_adx_sitesetting_modifiedby", "modifiedby")]
            public const string lk_adx_sitesetting_modifiedby = "lk_adx_sitesetting_modifiedby";
            [Relationship("adx_sitesetting", EntityRole.Referenced, "lk_adx_sitesetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_sitesetting_modifiedonbehalfby = "lk_adx_sitesetting_modifiedonbehalfby";
            [Relationship("adx_tag", EntityRole.Referenced, "lk_adx_tag_createdby", "createdby")]
            public const string lk_adx_tag_createdby = "lk_adx_tag_createdby";
            [Relationship("adx_tag", EntityRole.Referenced, "lk_adx_tag_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_tag_createdonbehalfby = "lk_adx_tag_createdonbehalfby";
            [Relationship("adx_tag", EntityRole.Referenced, "lk_adx_tag_modifiedby", "modifiedby")]
            public const string lk_adx_tag_modifiedby = "lk_adx_tag_modifiedby";
            [Relationship("adx_tag", EntityRole.Referenced, "lk_adx_tag_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_tag_modifiedonbehalfby = "lk_adx_tag_modifiedonbehalfby";
            [Relationship("adx_urlhistory", EntityRole.Referenced, "lk_adx_urlhistory_createdby", "createdby")]
            public const string lk_adx_urlhistory_createdby = "lk_adx_urlhistory_createdby";
            [Relationship("adx_urlhistory", EntityRole.Referenced, "lk_adx_urlhistory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_urlhistory_createdonbehalfby = "lk_adx_urlhistory_createdonbehalfby";
            [Relationship("adx_urlhistory", EntityRole.Referenced, "lk_adx_urlhistory_modifiedby", "modifiedby")]
            public const string lk_adx_urlhistory_modifiedby = "lk_adx_urlhistory_modifiedby";
            [Relationship("adx_urlhistory", EntityRole.Referenced, "lk_adx_urlhistory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_urlhistory_modifiedonbehalfby = "lk_adx_urlhistory_modifiedonbehalfby";
            [Relationship("adx_webfile", EntityRole.Referenced, "lk_adx_webfile_createdby", "createdby")]
            public const string lk_adx_webfile_createdby = "lk_adx_webfile_createdby";
            [Relationship("adx_webfile", EntityRole.Referenced, "lk_adx_webfile_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webfile_createdonbehalfby = "lk_adx_webfile_createdonbehalfby";
            [Relationship("adx_webfile", EntityRole.Referenced, "lk_adx_webfile_modifiedby", "modifiedby")]
            public const string lk_adx_webfile_modifiedby = "lk_adx_webfile_modifiedby";
            [Relationship("adx_webfile", EntityRole.Referenced, "lk_adx_webfile_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webfile_modifiedonbehalfby = "lk_adx_webfile_modifiedonbehalfby";
            [Relationship("adx_webfilelog", EntityRole.Referenced, "lk_adx_webfilelog_createdby", "createdby")]
            public const string lk_adx_webfilelog_createdby = "lk_adx_webfilelog_createdby";
            [Relationship("adx_webfilelog", EntityRole.Referenced, "lk_adx_webfilelog_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webfilelog_createdonbehalfby = "lk_adx_webfilelog_createdonbehalfby";
            [Relationship("adx_webfilelog", EntityRole.Referenced, "lk_adx_webfilelog_modifiedby", "modifiedby")]
            public const string lk_adx_webfilelog_modifiedby = "lk_adx_webfilelog_modifiedby";
            [Relationship("adx_webfilelog", EntityRole.Referenced, "lk_adx_webfilelog_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webfilelog_modifiedonbehalfby = "lk_adx_webfilelog_modifiedonbehalfby";
            [Relationship("adx_webform", EntityRole.Referenced, "lk_adx_webform_createdby", "createdby")]
            public const string lk_adx_webform_createdby = "lk_adx_webform_createdby";
            [Relationship("adx_webform", EntityRole.Referenced, "lk_adx_webform_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webform_createdonbehalfby = "lk_adx_webform_createdonbehalfby";
            [Relationship("adx_webform", EntityRole.Referenced, "lk_adx_webform_modifiedby", "modifiedby")]
            public const string lk_adx_webform_modifiedby = "lk_adx_webform_modifiedby";
            [Relationship("adx_webform", EntityRole.Referenced, "lk_adx_webform_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webform_modifiedonbehalfby = "lk_adx_webform_modifiedonbehalfby";
            [Relationship("adx_webformmetadata", EntityRole.Referenced, "lk_adx_webformmetadata_createdby", "createdby")]
            public const string lk_adx_webformmetadata_createdby = "lk_adx_webformmetadata_createdby";
            [Relationship("adx_webformmetadata", EntityRole.Referenced, "lk_adx_webformmetadata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webformmetadata_createdonbehalfby = "lk_adx_webformmetadata_createdonbehalfby";
            [Relationship("adx_webformmetadata", EntityRole.Referenced, "lk_adx_webformmetadata_modifiedby", "modifiedby")]
            public const string lk_adx_webformmetadata_modifiedby = "lk_adx_webformmetadata_modifiedby";
            [Relationship("adx_webformmetadata", EntityRole.Referenced, "lk_adx_webformmetadata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webformmetadata_modifiedonbehalfby = "lk_adx_webformmetadata_modifiedonbehalfby";
            [Relationship("adx_webformsession", EntityRole.Referenced, "lk_adx_webformsession_createdby", "createdby")]
            public const string lk_adx_webformsession_createdby = "lk_adx_webformsession_createdby";
            [Relationship("adx_webformsession", EntityRole.Referenced, "lk_adx_webformsession_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webformsession_createdonbehalfby = "lk_adx_webformsession_createdonbehalfby";
            [Relationship("adx_webformsession", EntityRole.Referenced, "lk_adx_webformsession_modifiedby", "modifiedby")]
            public const string lk_adx_webformsession_modifiedby = "lk_adx_webformsession_modifiedby";
            [Relationship("adx_webformsession", EntityRole.Referenced, "lk_adx_webformsession_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webformsession_modifiedonbehalfby = "lk_adx_webformsession_modifiedonbehalfby";
            [Relationship("adx_webformstep", EntityRole.Referenced, "lk_adx_webformstep_createdby", "createdby")]
            public const string lk_adx_webformstep_createdby = "lk_adx_webformstep_createdby";
            [Relationship("adx_webformstep", EntityRole.Referenced, "lk_adx_webformstep_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webformstep_createdonbehalfby = "lk_adx_webformstep_createdonbehalfby";
            [Relationship("adx_webformstep", EntityRole.Referenced, "lk_adx_webformstep_modifiedby", "modifiedby")]
            public const string lk_adx_webformstep_modifiedby = "lk_adx_webformstep_modifiedby";
            [Relationship("adx_webformstep", EntityRole.Referenced, "lk_adx_webformstep_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webformstep_modifiedonbehalfby = "lk_adx_webformstep_modifiedonbehalfby";
            [Relationship("adx_weblink", EntityRole.Referenced, "lk_adx_weblink_createdby", "createdby")]
            public const string lk_adx_weblink_createdby = "lk_adx_weblink_createdby";
            [Relationship("adx_weblink", EntityRole.Referenced, "lk_adx_weblink_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_weblink_createdonbehalfby = "lk_adx_weblink_createdonbehalfby";
            [Relationship("adx_weblink", EntityRole.Referenced, "lk_adx_weblink_modifiedby", "modifiedby")]
            public const string lk_adx_weblink_modifiedby = "lk_adx_weblink_modifiedby";
            [Relationship("adx_weblink", EntityRole.Referenced, "lk_adx_weblink_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_weblink_modifiedonbehalfby = "lk_adx_weblink_modifiedonbehalfby";
            [Relationship("adx_weblinkset", EntityRole.Referenced, "lk_adx_weblinkset_createdby", "createdby")]
            public const string lk_adx_weblinkset_createdby = "lk_adx_weblinkset_createdby";
            [Relationship("adx_weblinkset", EntityRole.Referenced, "lk_adx_weblinkset_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_weblinkset_createdonbehalfby = "lk_adx_weblinkset_createdonbehalfby";
            [Relationship("adx_weblinkset", EntityRole.Referenced, "lk_adx_weblinkset_modifiedby", "modifiedby")]
            public const string lk_adx_weblinkset_modifiedby = "lk_adx_weblinkset_modifiedby";
            [Relationship("adx_weblinkset", EntityRole.Referenced, "lk_adx_weblinkset_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_weblinkset_modifiedonbehalfby = "lk_adx_weblinkset_modifiedonbehalfby";
            [Relationship("adx_webnotificationentity", EntityRole.Referenced, "lk_adx_webnotificationentity_createdby", "createdby")]
            public const string lk_adx_webnotificationentity_createdby = "lk_adx_webnotificationentity_createdby";
            [Relationship("adx_webnotificationentity", EntityRole.Referenced, "lk_adx_webnotificationentity_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webnotificationentity_createdonbehalfby = "lk_adx_webnotificationentity_createdonbehalfby";
            [Relationship("adx_webnotificationentity", EntityRole.Referenced, "lk_adx_webnotificationentity_modifiedby", "modifiedby")]
            public const string lk_adx_webnotificationentity_modifiedby = "lk_adx_webnotificationentity_modifiedby";
            [Relationship("adx_webnotificationentity", EntityRole.Referenced, "lk_adx_webnotificationentity_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webnotificationentity_modifiedonbehalfby = "lk_adx_webnotificationentity_modifiedonbehalfby";
            [Relationship("adx_webnotificationurl", EntityRole.Referenced, "lk_adx_webnotificationurl_createdby", "createdby")]
            public const string lk_adx_webnotificationurl_createdby = "lk_adx_webnotificationurl_createdby";
            [Relationship("adx_webnotificationurl", EntityRole.Referenced, "lk_adx_webnotificationurl_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webnotificationurl_createdonbehalfby = "lk_adx_webnotificationurl_createdonbehalfby";
            [Relationship("adx_webnotificationurl", EntityRole.Referenced, "lk_adx_webnotificationurl_modifiedby", "modifiedby")]
            public const string lk_adx_webnotificationurl_modifiedby = "lk_adx_webnotificationurl_modifiedby";
            [Relationship("adx_webnotificationurl", EntityRole.Referenced, "lk_adx_webnotificationurl_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webnotificationurl_modifiedonbehalfby = "lk_adx_webnotificationurl_modifiedonbehalfby";
            [Relationship("adx_webpage", EntityRole.Referenced, "lk_adx_webpage_createdby", "createdby")]
            public const string lk_adx_webpage_createdby = "lk_adx_webpage_createdby";
            [Relationship("adx_webpage", EntityRole.Referenced, "lk_adx_webpage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webpage_createdonbehalfby = "lk_adx_webpage_createdonbehalfby";
            [Relationship("adx_webpage", EntityRole.Referenced, "lk_adx_webpage_modifiedby", "modifiedby")]
            public const string lk_adx_webpage_modifiedby = "lk_adx_webpage_modifiedby";
            [Relationship("adx_webpage", EntityRole.Referenced, "lk_adx_webpage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webpage_modifiedonbehalfby = "lk_adx_webpage_modifiedonbehalfby";
            [Relationship("adx_webpageaccesscontrolrule", EntityRole.Referenced, "lk_adx_webpageaccesscontrolrule_createdby", "createdby")]
            public const string lk_adx_webpageaccesscontrolrule_createdby = "lk_adx_webpageaccesscontrolrule_createdby";
            [Relationship("adx_webpageaccesscontrolrule", EntityRole.Referenced, "lk_adx_webpageaccesscontrolrule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webpageaccesscontrolrule_createdonbehalfby = "lk_adx_webpageaccesscontrolrule_createdonbehalfby";
            [Relationship("adx_webpageaccesscontrolrule", EntityRole.Referenced, "lk_adx_webpageaccesscontrolrule_modifiedby", "modifiedby")]
            public const string lk_adx_webpageaccesscontrolrule_modifiedby = "lk_adx_webpageaccesscontrolrule_modifiedby";
            [Relationship("adx_webpageaccesscontrolrule", EntityRole.Referenced, "lk_adx_webpageaccesscontrolrule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webpageaccesscontrolrule_modifiedonbehalfby = "lk_adx_webpageaccesscontrolrule_modifiedonbehalfby";
            [Relationship("adx_webpagehistory", EntityRole.Referenced, "lk_adx_webpagehistory_createdby", "createdby")]
            public const string lk_adx_webpagehistory_createdby = "lk_adx_webpagehistory_createdby";
            [Relationship("adx_webpagehistory", EntityRole.Referenced, "lk_adx_webpagehistory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webpagehistory_createdonbehalfby = "lk_adx_webpagehistory_createdonbehalfby";
            [Relationship("adx_webpagehistory", EntityRole.Referenced, "lk_adx_webpagehistory_modifiedby", "modifiedby")]
            public const string lk_adx_webpagehistory_modifiedby = "lk_adx_webpagehistory_modifiedby";
            [Relationship("adx_webpagehistory", EntityRole.Referenced, "lk_adx_webpagehistory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webpagehistory_modifiedonbehalfby = "lk_adx_webpagehistory_modifiedonbehalfby";
            [Relationship("adx_webpagelog", EntityRole.Referenced, "lk_adx_webpagelog_createdby", "createdby")]
            public const string lk_adx_webpagelog_createdby = "lk_adx_webpagelog_createdby";
            [Relationship("adx_webpagelog", EntityRole.Referenced, "lk_adx_webpagelog_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webpagelog_createdonbehalfby = "lk_adx_webpagelog_createdonbehalfby";
            [Relationship("adx_webpagelog", EntityRole.Referenced, "lk_adx_webpagelog_modifiedby", "modifiedby")]
            public const string lk_adx_webpagelog_modifiedby = "lk_adx_webpagelog_modifiedby";
            [Relationship("adx_webpagelog", EntityRole.Referenced, "lk_adx_webpagelog_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webpagelog_modifiedonbehalfby = "lk_adx_webpagelog_modifiedonbehalfby";
            [Relationship("adx_webrole", EntityRole.Referenced, "lk_adx_webrole_createdby", "createdby")]
            public const string lk_adx_webrole_createdby = "lk_adx_webrole_createdby";
            [Relationship("adx_webrole", EntityRole.Referenced, "lk_adx_webrole_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webrole_createdonbehalfby = "lk_adx_webrole_createdonbehalfby";
            [Relationship("adx_webrole", EntityRole.Referenced, "lk_adx_webrole_modifiedby", "modifiedby")]
            public const string lk_adx_webrole_modifiedby = "lk_adx_webrole_modifiedby";
            [Relationship("adx_webrole", EntityRole.Referenced, "lk_adx_webrole_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webrole_modifiedonbehalfby = "lk_adx_webrole_modifiedonbehalfby";
            [Relationship("adx_website", EntityRole.Referenced, "lk_adx_website_createdby", "createdby")]
            public const string lk_adx_website_createdby = "lk_adx_website_createdby";
            [Relationship("adx_website", EntityRole.Referenced, "lk_adx_website_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_website_createdonbehalfby = "lk_adx_website_createdonbehalfby";
            [Relationship("adx_website", EntityRole.Referenced, "lk_adx_website_modifiedby", "modifiedby")]
            public const string lk_adx_website_modifiedby = "lk_adx_website_modifiedby";
            [Relationship("adx_website", EntityRole.Referenced, "lk_adx_website_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_website_modifiedonbehalfby = "lk_adx_website_modifiedonbehalfby";
            [Relationship("adx_websiteaccess", EntityRole.Referenced, "lk_adx_websiteaccess_createdby", "createdby")]
            public const string lk_adx_websiteaccess_createdby = "lk_adx_websiteaccess_createdby";
            [Relationship("adx_websiteaccess", EntityRole.Referenced, "lk_adx_websiteaccess_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_websiteaccess_createdonbehalfby = "lk_adx_websiteaccess_createdonbehalfby";
            [Relationship("adx_websiteaccess", EntityRole.Referenced, "lk_adx_websiteaccess_modifiedby", "modifiedby")]
            public const string lk_adx_websiteaccess_modifiedby = "lk_adx_websiteaccess_modifiedby";
            [Relationship("adx_websiteaccess", EntityRole.Referenced, "lk_adx_websiteaccess_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_websiteaccess_modifiedonbehalfby = "lk_adx_websiteaccess_modifiedonbehalfby";
            [Relationship("adx_websitebinding", EntityRole.Referenced, "lk_adx_websitebinding_createdby", "createdby")]
            public const string lk_adx_websitebinding_createdby = "lk_adx_websitebinding_createdby";
            [Relationship("adx_websitebinding", EntityRole.Referenced, "lk_adx_websitebinding_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_websitebinding_createdonbehalfby = "lk_adx_websitebinding_createdonbehalfby";
            [Relationship("adx_websitebinding", EntityRole.Referenced, "lk_adx_websitebinding_modifiedby", "modifiedby")]
            public const string lk_adx_websitebinding_modifiedby = "lk_adx_websitebinding_modifiedby";
            [Relationship("adx_websitebinding", EntityRole.Referenced, "lk_adx_websitebinding_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_websitebinding_modifiedonbehalfby = "lk_adx_websitebinding_modifiedonbehalfby";
            [Relationship("adx_websitelanguage", EntityRole.Referenced, "lk_adx_websitelanguage_createdby", "createdby")]
            public const string lk_adx_websitelanguage_createdby = "lk_adx_websitelanguage_createdby";
            [Relationship("adx_websitelanguage", EntityRole.Referenced, "lk_adx_websitelanguage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_websitelanguage_createdonbehalfby = "lk_adx_websitelanguage_createdonbehalfby";
            [Relationship("adx_websitelanguage", EntityRole.Referenced, "lk_adx_websitelanguage_modifiedby", "modifiedby")]
            public const string lk_adx_websitelanguage_modifiedby = "lk_adx_websitelanguage_modifiedby";
            [Relationship("adx_websitelanguage", EntityRole.Referenced, "lk_adx_websitelanguage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_websitelanguage_modifiedonbehalfby = "lk_adx_websitelanguage_modifiedonbehalfby";
            [Relationship("adx_webtemplate", EntityRole.Referenced, "lk_adx_webtemplate_createdby", "createdby")]
            public const string lk_adx_webtemplate_createdby = "lk_adx_webtemplate_createdby";
            [Relationship("adx_webtemplate", EntityRole.Referenced, "lk_adx_webtemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_adx_webtemplate_createdonbehalfby = "lk_adx_webtemplate_createdonbehalfby";
            [Relationship("adx_webtemplate", EntityRole.Referenced, "lk_adx_webtemplate_modifiedby", "modifiedby")]
            public const string lk_adx_webtemplate_modifiedby = "lk_adx_webtemplate_modifiedby";
            [Relationship("adx_webtemplate", EntityRole.Referenced, "lk_adx_webtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_adx_webtemplate_modifiedonbehalfby = "lk_adx_webtemplate_modifiedonbehalfby";
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
            [Relationship("apisettings", EntityRole.Referenced, "lk_apisettings_createdby", "createdby")]
            public const string lk_apisettings_createdby = "lk_apisettings_createdby";
            [Relationship("apisettings", EntityRole.Referenced, "lk_apisettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_apisettings_createdonbehalfby = "lk_apisettings_createdonbehalfby";
            [Relationship("apisettings", EntityRole.Referenced, "lk_apisettings_modifiedby", "modifiedby")]
            public const string lk_apisettings_modifiedby = "lk_apisettings_modifiedby";
            [Relationship("apisettings", EntityRole.Referenced, "lk_apisettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_apisettings_modifiedonbehalfby = "lk_apisettings_modifiedonbehalfby";
            [Relationship("appaction", EntityRole.Referenced, "lk_appaction_createdby", "createdby")]
            public const string lk_appaction_createdby = "lk_appaction_createdby";
            [Relationship("appaction", EntityRole.Referenced, "lk_appaction_createdonbehalfby", "createdonbehalfby")]
            public const string lk_appaction_createdonbehalfby = "lk_appaction_createdonbehalfby";
            [Relationship("appaction", EntityRole.Referenced, "lk_appaction_modifiedby", "modifiedby")]
            public const string lk_appaction_modifiedby = "lk_appaction_modifiedby";
            [Relationship("appaction", EntityRole.Referenced, "lk_appaction_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_appaction_modifiedonbehalfby = "lk_appaction_modifiedonbehalfby";
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
            [Relationship("appelement", EntityRole.Referenced, "lk_appelement_createdby", "createdby")]
            public const string lk_appelement_createdby = "lk_appelement_createdby";
            [Relationship("appelement", EntityRole.Referenced, "lk_appelement_createdonbehalfby", "createdonbehalfby")]
            public const string lk_appelement_createdonbehalfby = "lk_appelement_createdonbehalfby";
            [Relationship("appelement", EntityRole.Referenced, "lk_appelement_modifiedby", "modifiedby")]
            public const string lk_appelement_modifiedby = "lk_appelement_modifiedby";
            [Relationship("appelement", EntityRole.Referenced, "lk_appelement_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_appelement_modifiedonbehalfby = "lk_appelement_modifiedonbehalfby";
            [Relationship("applicationfile", EntityRole.Referenced, "lk_applicationfile_createdby", "createdby")]
            public const string lk_applicationfile_createdby = "lk_applicationfile_createdby";
            [Relationship("applicationfile", EntityRole.Referenced, "lk_applicationfile_createdonbehalfby", "createdonbehalfby")]
            public const string lk_applicationfile_createdonbehalfby = "lk_applicationfile_createdonbehalfby";
            [Relationship("applicationfile", EntityRole.Referenced, "lk_applicationfile_modifiedby", "modifiedby")]
            public const string lk_applicationfile_modifiedby = "lk_applicationfile_modifiedby";
            [Relationship("applicationfile", EntityRole.Referenced, "lk_applicationfile_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_applicationfile_modifiedonbehalfby = "lk_applicationfile_modifiedonbehalfby";
            [Relationship("applicationuser", EntityRole.Referenced, "lk_applicationuser_createdby", "createdby")]
            public const string lk_applicationuser_createdby = "lk_applicationuser_createdby";
            [Relationship("applicationuser", EntityRole.Referenced, "lk_applicationuser_createdonbehalfby", "createdonbehalfby")]
            public const string lk_applicationuser_createdonbehalfby = "lk_applicationuser_createdonbehalfby";
            [Relationship("applicationuser", EntityRole.Referenced, "lk_applicationuser_modifiedby", "modifiedby")]
            public const string lk_applicationuser_modifiedby = "lk_applicationuser_modifiedby";
            [Relationship("applicationuser", EntityRole.Referenced, "lk_applicationuser_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_applicationuser_modifiedonbehalfby = "lk_applicationuser_modifiedonbehalfby";
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
            [Relationship("appmodulecomponentedge", EntityRole.Referenced, "lk_appmodulecomponentedge_createdby", "createdby")]
            public const string lk_appmodulecomponentedge_createdby = "lk_appmodulecomponentedge_createdby";
            [Relationship("appmodulecomponentedge", EntityRole.Referenced, "lk_appmodulecomponentedge_createdonbehalfby", "createdonbehalfby")]
            public const string lk_appmodulecomponentedge_createdonbehalfby = "lk_appmodulecomponentedge_createdonbehalfby";
            [Relationship("appmodulecomponentedge", EntityRole.Referenced, "lk_appmodulecomponentedge_modifiedby", "modifiedby")]
            public const string lk_appmodulecomponentedge_modifiedby = "lk_appmodulecomponentedge_modifiedby";
            [Relationship("appmodulecomponentedge", EntityRole.Referenced, "lk_appmodulecomponentedge_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_appmodulecomponentedge_modifiedonbehalfby = "lk_appmodulecomponentedge_modifiedonbehalfby";
            [Relationship("appmodulecomponentnode", EntityRole.Referenced, "lk_appmodulecomponentnode_createdby", "createdby")]
            public const string lk_appmodulecomponentnode_createdby = "lk_appmodulecomponentnode_createdby";
            [Relationship("appmodulecomponentnode", EntityRole.Referenced, "lk_appmodulecomponentnode_createdonbehalfby", "createdonbehalfby")]
            public const string lk_appmodulecomponentnode_createdonbehalfby = "lk_appmodulecomponentnode_createdonbehalfby";
            [Relationship("appmodulecomponentnode", EntityRole.Referenced, "lk_appmodulecomponentnode_modifiedby", "modifiedby")]
            public const string lk_appmodulecomponentnode_modifiedby = "lk_appmodulecomponentnode_modifiedby";
            [Relationship("appmodulecomponentnode", EntityRole.Referenced, "lk_appmodulecomponentnode_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_appmodulecomponentnode_modifiedonbehalfby = "lk_appmodulecomponentnode_modifiedonbehalfby";
            [Relationship("appnotification", EntityRole.Referenced, "lk_appnotification_createdby", "createdby")]
            public const string lk_appnotification_createdby = "lk_appnotification_createdby";
            [Relationship("appnotification", EntityRole.Referenced, "lk_appnotification_createdonbehalfby", "createdonbehalfby")]
            public const string lk_appnotification_createdonbehalfby = "lk_appnotification_createdonbehalfby";
            [Relationship("appnotification", EntityRole.Referenced, "lk_appnotification_modifiedby", "modifiedby")]
            public const string lk_appnotification_modifiedby = "lk_appnotification_modifiedby";
            [Relationship("appnotification", EntityRole.Referenced, "lk_appnotification_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_appnotification_modifiedonbehalfby = "lk_appnotification_modifiedonbehalfby";
            [Relationship("appointment", EntityRole.Referenced, "lk_appointment_createdby", "createdby")]
            public const string lk_appointment_createdby = "lk_appointment_createdby";
            [Relationship("appointment", EntityRole.Referenced, "lk_appointment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_appointment_createdonbehalfby = "lk_appointment_createdonbehalfby";
            [Relationship("appointment", EntityRole.Referenced, "lk_appointment_modifiedby", "modifiedby")]
            public const string lk_appointment_modifiedby = "lk_appointment_modifiedby";
            [Relationship("appointment", EntityRole.Referenced, "lk_appointment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_appointment_modifiedonbehalfby = "lk_appointment_modifiedonbehalfby";
            [Relationship("appsetting", EntityRole.Referenced, "lk_appsetting_createdby", "createdby")]
            public const string lk_appsetting_createdby = "lk_appsetting_createdby";
            [Relationship("appsetting", EntityRole.Referenced, "lk_appsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_appsetting_createdonbehalfby = "lk_appsetting_createdonbehalfby";
            [Relationship("appsetting", EntityRole.Referenced, "lk_appsetting_modifiedby", "modifiedby")]
            public const string lk_appsetting_modifiedby = "lk_appsetting_modifiedby";
            [Relationship("appsetting", EntityRole.Referenced, "lk_appsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_appsetting_modifiedonbehalfby = "lk_appsetting_modifiedonbehalfby";
            [Relationship("appusersetting", EntityRole.Referenced, "lk_appusersetting_createdby", "createdby")]
            public const string lk_appusersetting_createdby = "lk_appusersetting_createdby";
            [Relationship("appusersetting", EntityRole.Referenced, "lk_appusersetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_appusersetting_createdonbehalfby = "lk_appusersetting_createdonbehalfby";
            [Relationship("appusersetting", EntityRole.Referenced, "lk_appusersetting_modifiedby", "modifiedby")]
            public const string lk_appusersetting_modifiedby = "lk_appusersetting_modifiedby";
            [Relationship("appusersetting", EntityRole.Referenced, "lk_appusersetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_appusersetting_modifiedonbehalfby = "lk_appusersetting_modifiedonbehalfby";
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
            [Relationship("bot", EntityRole.Referenced, "lk_bot_createdby", "createdby")]
            public const string lk_bot_createdby = "lk_bot_createdby";
            [Relationship("bot", EntityRole.Referenced, "lk_bot_createdonbehalfby", "createdonbehalfby")]
            public const string lk_bot_createdonbehalfby = "lk_bot_createdonbehalfby";
            [Relationship("bot", EntityRole.Referenced, "lk_bot_modifiedby", "modifiedby")]
            public const string lk_bot_modifiedby = "lk_bot_modifiedby";
            [Relationship("bot", EntityRole.Referenced, "lk_bot_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_bot_modifiedonbehalfby = "lk_bot_modifiedonbehalfby";
            [Relationship("botcomponent", EntityRole.Referenced, "lk_botcomponent_createdby", "createdby")]
            public const string lk_botcomponent_createdby = "lk_botcomponent_createdby";
            [Relationship("botcomponent", EntityRole.Referenced, "lk_botcomponent_createdonbehalfby", "createdonbehalfby")]
            public const string lk_botcomponent_createdonbehalfby = "lk_botcomponent_createdonbehalfby";
            [Relationship("botcomponent", EntityRole.Referenced, "lk_botcomponent_modifiedby", "modifiedby")]
            public const string lk_botcomponent_modifiedby = "lk_botcomponent_modifiedby";
            [Relationship("botcomponent", EntityRole.Referenced, "lk_botcomponent_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_botcomponent_modifiedonbehalfby = "lk_botcomponent_modifiedonbehalfby";
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
            [Relationship("canvasappextendedmetadata", EntityRole.Referenced, "lk_canvasappextendedmetadata_createdby", "createdby")]
            public const string lk_canvasappextendedmetadata_createdby = "lk_canvasappextendedmetadata_createdby";
            [Relationship("canvasappextendedmetadata", EntityRole.Referenced, "lk_canvasappextendedmetadata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_canvasappextendedmetadata_createdonbehalfby = "lk_canvasappextendedmetadata_createdonbehalfby";
            [Relationship("canvasappextendedmetadata", EntityRole.Referenced, "lk_canvasappextendedmetadata_modifiedby", "modifiedby")]
            public const string lk_canvasappextendedmetadata_modifiedby = "lk_canvasappextendedmetadata_modifiedby";
            [Relationship("canvasappextendedmetadata", EntityRole.Referenced, "lk_canvasappextendedmetadata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_canvasappextendedmetadata_modifiedonbehalfby = "lk_canvasappextendedmetadata_modifiedonbehalfby";
            [Relationship("cardtype", EntityRole.Referenced, "lk_cardtype_createdby", "createdby")]
            public const string lk_cardtype_createdby = "lk_cardtype_createdby";
            [Relationship("cardtype", EntityRole.Referenced, "lk_cardtype_createdonbehalfby", "createdonbehalfby")]
            public const string lk_cardtype_createdonbehalfby = "lk_cardtype_createdonbehalfby";
            [Relationship("cardtype", EntityRole.Referenced, "lk_cardtype_modifiedby", "modifiedby")]
            public const string lk_cardtype_modifiedby = "lk_cardtype_modifiedby";
            [Relationship("cardtype", EntityRole.Referenced, "lk_cardtype_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_cardtype_modifiedonbehalfby = "lk_cardtype_modifiedonbehalfby";
            [Relationship("catalog", EntityRole.Referenced, "lk_catalog_createdby", "createdby")]
            public const string lk_catalog_createdby = "lk_catalog_createdby";
            [Relationship("catalog", EntityRole.Referenced, "lk_catalog_createdonbehalfby", "createdonbehalfby")]
            public const string lk_catalog_createdonbehalfby = "lk_catalog_createdonbehalfby";
            [Relationship("catalog", EntityRole.Referenced, "lk_catalog_modifiedby", "modifiedby")]
            public const string lk_catalog_modifiedby = "lk_catalog_modifiedby";
            [Relationship("catalog", EntityRole.Referenced, "lk_catalog_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_catalog_modifiedonbehalfby = "lk_catalog_modifiedonbehalfby";
            [Relationship("catalogassignment", EntityRole.Referenced, "lk_catalogassignment_createdby", "createdby")]
            public const string lk_catalogassignment_createdby = "lk_catalogassignment_createdby";
            [Relationship("catalogassignment", EntityRole.Referenced, "lk_catalogassignment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_catalogassignment_createdonbehalfby = "lk_catalogassignment_createdonbehalfby";
            [Relationship("catalogassignment", EntityRole.Referenced, "lk_catalogassignment_modifiedby", "modifiedby")]
            public const string lk_catalogassignment_modifiedby = "lk_catalogassignment_modifiedby";
            [Relationship("catalogassignment", EntityRole.Referenced, "lk_catalogassignment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_catalogassignment_modifiedonbehalfby = "lk_catalogassignment_modifiedonbehalfby";
            [Relationship("category", EntityRole.Referenced, "lk_category_createdby", "createdby")]
            public const string lk_category_createdby = "lk_category_createdby";
            [Relationship("category", EntityRole.Referenced, "lk_category_createdonbehalfby", "createdonbehalfby")]
            public const string lk_category_createdonbehalfby = "lk_category_createdonbehalfby";
            [Relationship("category", EntityRole.Referenced, "lk_category_modifiedby", "modifiedby")]
            public const string lk_category_modifiedby = "lk_category_modifiedby";
            [Relationship("category", EntityRole.Referenced, "lk_category_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_category_modifiedonbehalfby = "lk_category_modifiedonbehalfby";
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
            [Relationship("comment", EntityRole.Referenced, "lk_comment_createdby", "createdby")]
            public const string lk_comment_createdby = "lk_comment_createdby";
            [Relationship("comment", EntityRole.Referenced, "lk_comment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_comment_createdonbehalfby = "lk_comment_createdonbehalfby";
            [Relationship("comment", EntityRole.Referenced, "lk_comment_modifiedby", "modifiedby")]
            public const string lk_comment_modifiedby = "lk_comment_modifiedby";
            [Relationship("comment", EntityRole.Referenced, "lk_comment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_comment_modifiedonbehalfby = "lk_comment_modifiedonbehalfby";
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
            [Relationship("connectionreference", EntityRole.Referenced, "lk_connectionreference_createdby", "createdby")]
            public const string lk_connectionreference_createdby = "lk_connectionreference_createdby";
            [Relationship("connectionreference", EntityRole.Referenced, "lk_connectionreference_createdonbehalfby", "createdonbehalfby")]
            public const string lk_connectionreference_createdonbehalfby = "lk_connectionreference_createdonbehalfby";
            [Relationship("connectionreference", EntityRole.Referenced, "lk_connectionreference_modifiedby", "modifiedby")]
            public const string lk_connectionreference_modifiedby = "lk_connectionreference_modifiedby";
            [Relationship("connectionreference", EntityRole.Referenced, "lk_connectionreference_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_connectionreference_modifiedonbehalfby = "lk_connectionreference_modifiedonbehalfby";
            [Relationship("connectionrole", EntityRole.Referenced, "lk_connectionrolebase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_connectionrolebase_createdonbehalfby = "lk_connectionrolebase_createdonbehalfby";
            [Relationship("connectionrole", EntityRole.Referenced, "lk_connectionrolebase_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_connectionrolebase_modifiedonbehalfby = "lk_connectionrolebase_modifiedonbehalfby";
            [Relationship("connector", EntityRole.Referenced, "lk_connector_createdby", "createdby")]
            public const string lk_connector_createdby = "lk_connector_createdby";
            [Relationship("connector", EntityRole.Referenced, "lk_connector_createdonbehalfby", "createdonbehalfby")]
            public const string lk_connector_createdonbehalfby = "lk_connector_createdonbehalfby";
            [Relationship("connector", EntityRole.Referenced, "lk_connector_modifiedby", "modifiedby")]
            public const string lk_connector_modifiedby = "lk_connector_modifiedby";
            [Relationship("connector", EntityRole.Referenced, "lk_connector_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_connector_modifiedonbehalfby = "lk_connector_modifiedonbehalfby";
            [Relationship("constraintbasedgroup", EntityRole.Referenced, "lk_constraintbasedgroup_createdby", "createdby")]
            public const string lk_constraintbasedgroup_createdby = "lk_constraintbasedgroup_createdby";
            [Relationship("constraintbasedgroup", EntityRole.Referenced, "lk_constraintbasedgroup_createdonbehalfby", "createdonbehalfby")]
            public const string lk_constraintbasedgroup_createdonbehalfby = "lk_constraintbasedgroup_createdonbehalfby";
            [Relationship("constraintbasedgroup", EntityRole.Referenced, "lk_constraintbasedgroup_modifiedby", "modifiedby")]
            public const string lk_constraintbasedgroup_modifiedby = "lk_constraintbasedgroup_modifiedby";
            [Relationship("constraintbasedgroup", EntityRole.Referenced, "lk_constraintbasedgroup_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_constraintbasedgroup_modifiedonbehalfby = "lk_constraintbasedgroup_modifiedonbehalfby";
            [Relationship("contact", EntityRole.Referenced, "lk_contact_createdonbehalfby", "createdonbehalfby")]
            public const string lk_contact_createdonbehalfby = "lk_contact_createdonbehalfby";
            [Relationship("contact", EntityRole.Referenced, "lk_contact_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_contact_modifiedonbehalfby = "lk_contact_modifiedonbehalfby";
            [Relationship("contact", EntityRole.Referenced, "lk_contactbase_createdby", "createdby")]
            public const string lk_contactbase_createdby = "lk_contactbase_createdby";
            [Relationship("contact", EntityRole.Referenced, "lk_contactbase_modifiedby", "modifiedby")]
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
            [Relationship("conversationtranscript", EntityRole.Referenced, "lk_conversationtranscript_createdby", "createdby")]
            public const string lk_conversationtranscript_createdby = "lk_conversationtranscript_createdby";
            [Relationship("conversationtranscript", EntityRole.Referenced, "lk_conversationtranscript_createdonbehalfby", "createdonbehalfby")]
            public const string lk_conversationtranscript_createdonbehalfby = "lk_conversationtranscript_createdonbehalfby";
            [Relationship("conversationtranscript", EntityRole.Referenced, "lk_conversationtranscript_modifiedby", "modifiedby")]
            public const string lk_conversationtranscript_modifiedby = "lk_conversationtranscript_modifiedby";
            [Relationship("conversationtranscript", EntityRole.Referenced, "lk_conversationtranscript_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_conversationtranscript_modifiedonbehalfby = "lk_conversationtranscript_modifiedonbehalfby";
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
            [Relationship("cr9a1_test", EntityRole.Referenced, "lk_cr9a1_test_createdby", "createdby")]
            public const string lk_cr9a1_test_createdby = "lk_cr9a1_test_createdby";
            [Relationship("cr9a1_test", EntityRole.Referenced, "lk_cr9a1_test_createdonbehalfby", "createdonbehalfby")]
            public const string lk_cr9a1_test_createdonbehalfby = "lk_cr9a1_test_createdonbehalfby";
            [Relationship("cr9a1_test", EntityRole.Referenced, "lk_cr9a1_test_modifiedby", "modifiedby")]
            public const string lk_cr9a1_test_modifiedby = "lk_cr9a1_test_modifiedby";
            [Relationship("cr9a1_test", EntityRole.Referenced, "lk_cr9a1_test_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_cr9a1_test_modifiedonbehalfby = "lk_cr9a1_test_modifiedonbehalfby";
            [Relationship(CustomApiDefinition.EntityName, EntityRole.Referenced, "lk_customapi_createdby", "createdby")]
            public const string lk_customapi_createdby = "lk_customapi_createdby";
            [Relationship(CustomApiDefinition.EntityName, EntityRole.Referenced, "lk_customapi_createdonbehalfby", "createdonbehalfby")]
            public const string lk_customapi_createdonbehalfby = "lk_customapi_createdonbehalfby";
            [Relationship(CustomApiDefinition.EntityName, EntityRole.Referenced, "lk_customapi_modifiedby", "modifiedby")]
            public const string lk_customapi_modifiedby = "lk_customapi_modifiedby";
            [Relationship(CustomApiDefinition.EntityName, EntityRole.Referenced, "lk_customapi_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_customapi_modifiedonbehalfby = "lk_customapi_modifiedonbehalfby";
            [Relationship(CustomApiRequestParameterDefinition.EntityName, EntityRole.Referenced, "lk_customapirequestparameter_createdby", "createdby")]
            public const string lk_customapirequestparameter_createdby = "lk_customapirequestparameter_createdby";
            [Relationship(CustomApiRequestParameterDefinition.EntityName, EntityRole.Referenced, "lk_customapirequestparameter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_customapirequestparameter_createdonbehalfby = "lk_customapirequestparameter_createdonbehalfby";
            [Relationship(CustomApiRequestParameterDefinition.EntityName, EntityRole.Referenced, "lk_customapirequestparameter_modifiedby", "modifiedby")]
            public const string lk_customapirequestparameter_modifiedby = "lk_customapirequestparameter_modifiedby";
            [Relationship(CustomApiRequestParameterDefinition.EntityName, EntityRole.Referenced, "lk_customapirequestparameter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_customapirequestparameter_modifiedonbehalfby = "lk_customapirequestparameter_modifiedonbehalfby";
            [Relationship(CustomApiResponsePropertyDefinition.EntityName, EntityRole.Referenced, "lk_customapiresponseproperty_createdby", "createdby")]
            public const string lk_customapiresponseproperty_createdby = "lk_customapiresponseproperty_createdby";
            [Relationship(CustomApiResponsePropertyDefinition.EntityName, EntityRole.Referenced, "lk_customapiresponseproperty_createdonbehalfby", "createdonbehalfby")]
            public const string lk_customapiresponseproperty_createdonbehalfby = "lk_customapiresponseproperty_createdonbehalfby";
            [Relationship(CustomApiResponsePropertyDefinition.EntityName, EntityRole.Referenced, "lk_customapiresponseproperty_modifiedby", "modifiedby")]
            public const string lk_customapiresponseproperty_modifiedby = "lk_customapiresponseproperty_modifiedby";
            [Relationship(CustomApiResponsePropertyDefinition.EntityName, EntityRole.Referenced, "lk_customapiresponseproperty_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_customapiresponseproperty_modifiedonbehalfby = "lk_customapiresponseproperty_modifiedonbehalfby";
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
            [Relationship("datalakefolder", EntityRole.Referenced, "lk_datalakefolder_createdby", "createdby")]
            public const string lk_datalakefolder_createdby = "lk_datalakefolder_createdby";
            [Relationship("datalakefolder", EntityRole.Referenced, "lk_datalakefolder_createdonbehalfby", "createdonbehalfby")]
            public const string lk_datalakefolder_createdonbehalfby = "lk_datalakefolder_createdonbehalfby";
            [Relationship("datalakefolder", EntityRole.Referenced, "lk_datalakefolder_modifiedby", "modifiedby")]
            public const string lk_datalakefolder_modifiedby = "lk_datalakefolder_modifiedby";
            [Relationship("datalakefolder", EntityRole.Referenced, "lk_datalakefolder_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_datalakefolder_modifiedonbehalfby = "lk_datalakefolder_modifiedonbehalfby";
            [Relationship("datalakefolderpermission", EntityRole.Referenced, "lk_datalakefolderpermission_createdby", "createdby")]
            public const string lk_datalakefolderpermission_createdby = "lk_datalakefolderpermission_createdby";
            [Relationship("datalakefolderpermission", EntityRole.Referenced, "lk_datalakefolderpermission_createdonbehalfby", "createdonbehalfby")]
            public const string lk_datalakefolderpermission_createdonbehalfby = "lk_datalakefolderpermission_createdonbehalfby";
            [Relationship("datalakefolderpermission", EntityRole.Referenced, "lk_datalakefolderpermission_modifiedby", "modifiedby")]
            public const string lk_datalakefolderpermission_modifiedby = "lk_datalakefolderpermission_modifiedby";
            [Relationship("datalakefolderpermission", EntityRole.Referenced, "lk_datalakefolderpermission_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_datalakefolderpermission_modifiedonbehalfby = "lk_datalakefolderpermission_modifiedonbehalfby";
            [Relationship("datalakeworkspace", EntityRole.Referenced, "lk_datalakeworkspace_createdby", "createdby")]
            public const string lk_datalakeworkspace_createdby = "lk_datalakeworkspace_createdby";
            [Relationship("datalakeworkspace", EntityRole.Referenced, "lk_datalakeworkspace_createdonbehalfby", "createdonbehalfby")]
            public const string lk_datalakeworkspace_createdonbehalfby = "lk_datalakeworkspace_createdonbehalfby";
            [Relationship("datalakeworkspace", EntityRole.Referenced, "lk_datalakeworkspace_modifiedby", "modifiedby")]
            public const string lk_datalakeworkspace_modifiedby = "lk_datalakeworkspace_modifiedby";
            [Relationship("datalakeworkspace", EntityRole.Referenced, "lk_datalakeworkspace_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_datalakeworkspace_modifiedonbehalfby = "lk_datalakeworkspace_modifiedonbehalfby";
            [Relationship("datalakeworkspacepermission", EntityRole.Referenced, "lk_datalakeworkspacepermission_createdby", "createdby")]
            public const string lk_datalakeworkspacepermission_createdby = "lk_datalakeworkspacepermission_createdby";
            [Relationship("datalakeworkspacepermission", EntityRole.Referenced, "lk_datalakeworkspacepermission_createdonbehalfby", "createdonbehalfby")]
            public const string lk_datalakeworkspacepermission_createdonbehalfby = "lk_datalakeworkspacepermission_createdonbehalfby";
            [Relationship("datalakeworkspacepermission", EntityRole.Referenced, "lk_datalakeworkspacepermission_modifiedby", "modifiedby")]
            public const string lk_datalakeworkspacepermission_modifiedby = "lk_datalakeworkspacepermission_modifiedby";
            [Relationship("datalakeworkspacepermission", EntityRole.Referenced, "lk_datalakeworkspacepermission_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_datalakeworkspacepermission_modifiedonbehalfby = "lk_datalakeworkspacepermission_modifiedonbehalfby";
            [Relationship("datasyncstate", EntityRole.Referenced, "lk_datasyncstate_createdby", "createdby")]
            public const string lk_datasyncstate_createdby = "lk_datasyncstate_createdby";
            [Relationship("datasyncstate", EntityRole.Referenced, "lk_datasyncstate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_datasyncstate_createdonbehalfby = "lk_datasyncstate_createdonbehalfby";
            [Relationship("datasyncstate", EntityRole.Referenced, "lk_datasyncstate_modifiedby", "modifiedby")]
            public const string lk_datasyncstate_modifiedby = "lk_datasyncstate_modifiedby";
            [Relationship("datasyncstate", EntityRole.Referenced, "lk_datasyncstate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_datasyncstate_modifiedonbehalfby = "lk_datasyncstate_modifiedonbehalfby";
            [Relationship("delveactionhub", EntityRole.Referenced, "lk_delveactionhub_createdby", "createdby")]
            public const string lk_delveactionhub_createdby = "lk_delveactionhub_createdby";
            [Relationship("delveactionhub", EntityRole.Referenced, "lk_delveactionhub_createdonbehalfby", "createdonbehalfby")]
            public const string lk_delveactionhub_createdonbehalfby = "lk_delveactionhub_createdonbehalfby";
            [Relationship("delveactionhub", EntityRole.Referenced, "lk_delveactionhub_modifiedby", "modifiedby")]
            public const string lk_delveactionhub_modifiedby = "lk_delveactionhub_modifiedby";
            [Relationship("delveactionhub", EntityRole.Referenced, "lk_delveactionhub_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_delveactionhub_modifiedonbehalfby = "lk_delveactionhub_modifiedonbehalfby";
            [Relationship("dimsi_debugsession", EntityRole.Referenced, "lk_dimsi_debugsession_createdby", "createdby")]
            public const string lk_dimsi_debugsession_createdby = "lk_dimsi_debugsession_createdby";
            [Relationship("dimsi_debugsession", EntityRole.Referenced, "lk_dimsi_debugsession_createdonbehalfby", "createdonbehalfby")]
            public const string lk_dimsi_debugsession_createdonbehalfby = "lk_dimsi_debugsession_createdonbehalfby";
            [Relationship("dimsi_debugsession", EntityRole.Referenced, "lk_dimsi_debugsession_modifiedby", "modifiedby")]
            public const string lk_dimsi_debugsession_modifiedby = "lk_dimsi_debugsession_modifiedby";
            [Relationship("dimsi_debugsession", EntityRole.Referenced, "lk_dimsi_debugsession_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_dimsi_debugsession_modifiedonbehalfby = "lk_dimsi_debugsession_modifiedonbehalfby";
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
            [Relationship("environmentvariabledefinition", EntityRole.Referenced, "lk_environmentvariabledefinition_createdby", "createdby")]
            public const string lk_environmentvariabledefinition_createdby = "lk_environmentvariabledefinition_createdby";
            [Relationship("environmentvariabledefinition", EntityRole.Referenced, "lk_environmentvariabledefinition_createdonbehalfby", "createdonbehalfby")]
            public const string lk_environmentvariabledefinition_createdonbehalfby = "lk_environmentvariabledefinition_createdonbehalfby";
            [Relationship("environmentvariabledefinition", EntityRole.Referenced, "lk_environmentvariabledefinition_modifiedby", "modifiedby")]
            public const string lk_environmentvariabledefinition_modifiedby = "lk_environmentvariabledefinition_modifiedby";
            [Relationship("environmentvariabledefinition", EntityRole.Referenced, "lk_environmentvariabledefinition_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_environmentvariabledefinition_modifiedonbehalfby = "lk_environmentvariabledefinition_modifiedonbehalfby";
            [Relationship("environmentvariablevalue", EntityRole.Referenced, "lk_environmentvariablevalue_createdby", "createdby")]
            public const string lk_environmentvariablevalue_createdby = "lk_environmentvariablevalue_createdby";
            [Relationship("environmentvariablevalue", EntityRole.Referenced, "lk_environmentvariablevalue_createdonbehalfby", "createdonbehalfby")]
            public const string lk_environmentvariablevalue_createdonbehalfby = "lk_environmentvariablevalue_createdonbehalfby";
            [Relationship("environmentvariablevalue", EntityRole.Referenced, "lk_environmentvariablevalue_modifiedby", "modifiedby")]
            public const string lk_environmentvariablevalue_modifiedby = "lk_environmentvariablevalue_modifiedby";
            [Relationship("environmentvariablevalue", EntityRole.Referenced, "lk_environmentvariablevalue_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_environmentvariablevalue_modifiedonbehalfby = "lk_environmentvariablevalue_modifiedonbehalfby";
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
            [Relationship("exportsolutionupload", EntityRole.Referenced, "lk_exportsolutionupload_createdby", "createdby")]
            public const string lk_exportsolutionupload_createdby = "lk_exportsolutionupload_createdby";
            [Relationship("exportsolutionupload", EntityRole.Referenced, "lk_exportsolutionupload_createdonbehalfby", "createdonbehalfby")]
            public const string lk_exportsolutionupload_createdonbehalfby = "lk_exportsolutionupload_createdonbehalfby";
            [Relationship("exportsolutionupload", EntityRole.Referenced, "lk_exportsolutionupload_modifiedby", "modifiedby")]
            public const string lk_exportsolutionupload_modifiedby = "lk_exportsolutionupload_modifiedby";
            [Relationship("exportsolutionupload", EntityRole.Referenced, "lk_exportsolutionupload_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_exportsolutionupload_modifiedonbehalfby = "lk_exportsolutionupload_modifiedonbehalfby";
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
            [Relationship("featurecontrolsetting", EntityRole.Referenced, "lk_featurecontrolsetting_createdby", "createdby")]
            public const string lk_featurecontrolsetting_createdby = "lk_featurecontrolsetting_createdby";
            [Relationship("featurecontrolsetting", EntityRole.Referenced, "lk_featurecontrolsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_featurecontrolsetting_createdonbehalfby = "lk_featurecontrolsetting_createdonbehalfby";
            [Relationship("featurecontrolsetting", EntityRole.Referenced, "lk_featurecontrolsetting_modifiedby", "modifiedby")]
            public const string lk_featurecontrolsetting_modifiedby = "lk_featurecontrolsetting_modifiedby";
            [Relationship("featurecontrolsetting", EntityRole.Referenced, "lk_featurecontrolsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_featurecontrolsetting_modifiedonbehalfby = "lk_featurecontrolsetting_modifiedonbehalfby";
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
            [Relationship("flowmachine", EntityRole.Referenced, "lk_flowmachine_createdby", "createdby")]
            public const string lk_flowmachine_createdby = "lk_flowmachine_createdby";
            [Relationship("flowmachine", EntityRole.Referenced, "lk_flowmachine_createdonbehalfby", "createdonbehalfby")]
            public const string lk_flowmachine_createdonbehalfby = "lk_flowmachine_createdonbehalfby";
            [Relationship("flowmachine", EntityRole.Referenced, "lk_flowmachine_modifiedby", "modifiedby")]
            public const string lk_flowmachine_modifiedby = "lk_flowmachine_modifiedby";
            [Relationship("flowmachine", EntityRole.Referenced, "lk_flowmachine_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_flowmachine_modifiedonbehalfby = "lk_flowmachine_modifiedonbehalfby";
            [Relationship("flowmachinegroup", EntityRole.Referenced, "lk_flowmachinegroup_createdby", "createdby")]
            public const string lk_flowmachinegroup_createdby = "lk_flowmachinegroup_createdby";
            [Relationship("flowmachinegroup", EntityRole.Referenced, "lk_flowmachinegroup_createdonbehalfby", "createdonbehalfby")]
            public const string lk_flowmachinegroup_createdonbehalfby = "lk_flowmachinegroup_createdonbehalfby";
            [Relationship("flowmachinegroup", EntityRole.Referenced, "lk_flowmachinegroup_modifiedby", "modifiedby")]
            public const string lk_flowmachinegroup_modifiedby = "lk_flowmachinegroup_modifiedby";
            [Relationship("flowmachinegroup", EntityRole.Referenced, "lk_flowmachinegroup_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_flowmachinegroup_modifiedonbehalfby = "lk_flowmachinegroup_modifiedonbehalfby";
            [Relationship("flowsession", EntityRole.Referenced, "lk_flowsession_createdby", "createdby")]
            public const string lk_flowsession_createdby = "lk_flowsession_createdby";
            [Relationship("flowsession", EntityRole.Referenced, "lk_flowsession_createdonbehalfby", "createdonbehalfby")]
            public const string lk_flowsession_createdonbehalfby = "lk_flowsession_createdonbehalfby";
            [Relationship("flowsession", EntityRole.Referenced, "lk_flowsession_modifiedby", "modifiedby")]
            public const string lk_flowsession_modifiedby = "lk_flowsession_modifiedby";
            [Relationship("flowsession", EntityRole.Referenced, "lk_flowsession_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_flowsession_modifiedonbehalfby = "lk_flowsession_modifiedonbehalfby";
            [Relationship("ftp_agentimmobilier", EntityRole.Referenced, "lk_ftp_agentimmobilier_createdby", "createdby")]
            public const string lk_ftp_agentimmobilier_createdby = "lk_ftp_agentimmobilier_createdby";
            [Relationship("ftp_agentimmobilier", EntityRole.Referenced, "lk_ftp_agentimmobilier_createdonbehalfby", "createdonbehalfby")]
            public const string lk_ftp_agentimmobilier_createdonbehalfby = "lk_ftp_agentimmobilier_createdonbehalfby";
            [Relationship("ftp_agentimmobilier", EntityRole.Referenced, "lk_ftp_agentimmobilier_modifiedby", "modifiedby")]
            public const string lk_ftp_agentimmobilier_modifiedby = "lk_ftp_agentimmobilier_modifiedby";
            [Relationship("ftp_agentimmobilier", EntityRole.Referenced, "lk_ftp_agentimmobilier_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ftp_agentimmobilier_modifiedonbehalfby = "lk_ftp_agentimmobilier_modifiedonbehalfby";
            [Relationship("ftp_appartement", EntityRole.Referenced, "lk_ftp_appartement_createdby", "createdby")]
            public const string lk_ftp_appartement_createdby = "lk_ftp_appartement_createdby";
            [Relationship("ftp_appartement", EntityRole.Referenced, "lk_ftp_appartement_createdonbehalfby", "createdonbehalfby")]
            public const string lk_ftp_appartement_createdonbehalfby = "lk_ftp_appartement_createdonbehalfby";
            [Relationship("ftp_appartement", EntityRole.Referenced, "lk_ftp_appartement_modifiedby", "modifiedby")]
            public const string lk_ftp_appartement_modifiedby = "lk_ftp_appartement_modifiedby";
            [Relationship("ftp_appartement", EntityRole.Referenced, "lk_ftp_appartement_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ftp_appartement_modifiedonbehalfby = "lk_ftp_appartement_modifiedonbehalfby";
            [Relationship("ftp_contrat", EntityRole.Referenced, "lk_ftp_contrat_createdby", "createdby")]
            public const string lk_ftp_contrat_createdby = "lk_ftp_contrat_createdby";
            [Relationship("ftp_contrat", EntityRole.Referenced, "lk_ftp_contrat_createdonbehalfby", "createdonbehalfby")]
            public const string lk_ftp_contrat_createdonbehalfby = "lk_ftp_contrat_createdonbehalfby";
            [Relationship("ftp_contrat", EntityRole.Referenced, "lk_ftp_contrat_modifiedby", "modifiedby")]
            public const string lk_ftp_contrat_modifiedby = "lk_ftp_contrat_modifiedby";
            [Relationship("ftp_contrat", EntityRole.Referenced, "lk_ftp_contrat_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ftp_contrat_modifiedonbehalfby = "lk_ftp_contrat_modifiedonbehalfby";
            [Relationship("ftp_contratdelocation", EntityRole.Referenced, "lk_ftp_contratdelocation_createdby", "createdby")]
            public const string lk_ftp_contratdelocation_createdby = "lk_ftp_contratdelocation_createdby";
            [Relationship("ftp_contratdelocation", EntityRole.Referenced, "lk_ftp_contratdelocation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_ftp_contratdelocation_createdonbehalfby = "lk_ftp_contratdelocation_createdonbehalfby";
            [Relationship("ftp_contratdelocation", EntityRole.Referenced, "lk_ftp_contratdelocation_modifiedby", "modifiedby")]
            public const string lk_ftp_contratdelocation_modifiedby = "lk_ftp_contratdelocation_modifiedby";
            [Relationship("ftp_contratdelocation", EntityRole.Referenced, "lk_ftp_contratdelocation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ftp_contratdelocation_modifiedonbehalfby = "lk_ftp_contratdelocation_modifiedonbehalfby";
            [Relationship("ftp_particulier", EntityRole.Referenced, "lk_ftp_particulier_createdby", "createdby")]
            public const string lk_ftp_particulier_createdby = "lk_ftp_particulier_createdby";
            [Relationship("ftp_particulier", EntityRole.Referenced, "lk_ftp_particulier_createdonbehalfby", "createdonbehalfby")]
            public const string lk_ftp_particulier_createdonbehalfby = "lk_ftp_particulier_createdonbehalfby";
            [Relationship("ftp_particulier", EntityRole.Referenced, "lk_ftp_particulier_modifiedby", "modifiedby")]
            public const string lk_ftp_particulier_modifiedby = "lk_ftp_particulier_modifiedby";
            [Relationship("ftp_particulier", EntityRole.Referenced, "lk_ftp_particulier_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ftp_particulier_modifiedonbehalfby = "lk_ftp_particulier_modifiedonbehalfby";
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
            [Relationship("internalcatalogassignment", EntityRole.Referenced, "lk_internalcatalogassignment_createdby", "createdby")]
            public const string lk_internalcatalogassignment_createdby = "lk_internalcatalogassignment_createdby";
            [Relationship("internalcatalogassignment", EntityRole.Referenced, "lk_internalcatalogassignment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_internalcatalogassignment_createdonbehalfby = "lk_internalcatalogassignment_createdonbehalfby";
            [Relationship("internalcatalogassignment", EntityRole.Referenced, "lk_internalcatalogassignment_modifiedby", "modifiedby")]
            public const string lk_internalcatalogassignment_modifiedby = "lk_internalcatalogassignment_modifiedby";
            [Relationship("internalcatalogassignment", EntityRole.Referenced, "lk_internalcatalogassignment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_internalcatalogassignment_modifiedonbehalfby = "lk_internalcatalogassignment_modifiedonbehalfby";
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
            [Relationship("keyvaultreference", EntityRole.Referenced, "lk_keyvaultreference_createdby", "createdby")]
            public const string lk_keyvaultreference_createdby = "lk_keyvaultreference_createdby";
            [Relationship("keyvaultreference", EntityRole.Referenced, "lk_keyvaultreference_createdonbehalfby", "createdonbehalfby")]
            public const string lk_keyvaultreference_createdonbehalfby = "lk_keyvaultreference_createdonbehalfby";
            [Relationship("keyvaultreference", EntityRole.Referenced, "lk_keyvaultreference_modifiedby", "modifiedby")]
            public const string lk_keyvaultreference_modifiedby = "lk_keyvaultreference_modifiedby";
            [Relationship("keyvaultreference", EntityRole.Referenced, "lk_keyvaultreference_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_keyvaultreference_modifiedonbehalfby = "lk_keyvaultreference_modifiedonbehalfby";
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
            [Relationship("listoperation", EntityRole.Referenced, "lk_listoperation_createdby", "createdby")]
            public const string lk_listoperation_createdby = "lk_listoperation_createdby";
            [Relationship("listoperation", EntityRole.Referenced, "lk_listoperation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_listoperation_createdonbehalfby = "lk_listoperation_createdonbehalfby";
            [Relationship("listoperation", EntityRole.Referenced, "lk_listoperation_modifiedby", "modifiedby")]
            public const string lk_listoperation_modifiedby = "lk_listoperation_modifiedby";
            [Relationship("listoperation", EntityRole.Referenced, "lk_listoperation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_listoperation_modifiedonbehalfby = "lk_listoperation_modifiedonbehalfby";
            [Relationship("lookupmapping", EntityRole.Referenced, "lk_lookupmapping_createdby", "createdby")]
            public const string lk_lookupmapping_createdby = "lk_lookupmapping_createdby";
            [Relationship("lookupmapping", EntityRole.Referenced, "lk_lookupmapping_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lookupmapping_createdonbehalfby = "lk_lookupmapping_createdonbehalfby";
            [Relationship("lookupmapping", EntityRole.Referenced, "lk_lookupmapping_modifiedby", "modifiedby")]
            public const string lk_lookupmapping_modifiedby = "lk_lookupmapping_modifiedby";
            [Relationship("lookupmapping", EntityRole.Referenced, "lk_lookupmapping_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lookupmapping_modifiedonbehalfby = "lk_lookupmapping_modifiedonbehalfby";
            [Relationship("lor_bien", EntityRole.Referenced, "lk_lor_bien_createdby", "createdby")]
            public const string lk_lor_bien_createdby = "lk_lor_bien_createdby";
            [Relationship("lor_bien", EntityRole.Referenced, "lk_lor_bien_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_bien_createdonbehalfby = "lk_lor_bien_createdonbehalfby";
            [Relationship("lor_bien", EntityRole.Referenced, "lk_lor_bien_modifiedby", "modifiedby")]
            public const string lk_lor_bien_modifiedby = "lk_lor_bien_modifiedby";
            [Relationship("lor_bien", EntityRole.Referenced, "lk_lor_bien_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_bien_modifiedonbehalfby = "lk_lor_bien_modifiedonbehalfby";
            [Relationship("lor_camion", EntityRole.Referenced, "lk_lor_camion_createdby", "createdby")]
            public const string lk_lor_camion_createdby = "lk_lor_camion_createdby";
            [Relationship("lor_camion", EntityRole.Referenced, "lk_lor_camion_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_camion_createdonbehalfby = "lk_lor_camion_createdonbehalfby";
            [Relationship("lor_camion", EntityRole.Referenced, "lk_lor_camion_modifiedby", "modifiedby")]
            public const string lk_lor_camion_modifiedby = "lk_lor_camion_modifiedby";
            [Relationship("lor_camion", EntityRole.Referenced, "lk_lor_camion_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_camion_modifiedonbehalfby = "lk_lor_camion_modifiedonbehalfby";
            [Relationship("lor_demande", EntityRole.Referenced, "lk_lor_demande_createdby", "createdby")]
            public const string lk_lor_demande_createdby = "lk_lor_demande_createdby";
            [Relationship("lor_demande", EntityRole.Referenced, "lk_lor_demande_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_demande_createdonbehalfby = "lk_lor_demande_createdonbehalfby";
            [Relationship("lor_demande", EntityRole.Referenced, "lk_lor_demande_modifiedby", "modifiedby")]
            public const string lk_lor_demande_modifiedby = "lk_lor_demande_modifiedby";
            [Relationship("lor_demande", EntityRole.Referenced, "lk_lor_demande_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_demande_modifiedonbehalfby = "lk_lor_demande_modifiedonbehalfby";
            [Relationship("lor_immeuble", EntityRole.Referenced, "lk_lor_immeuble_createdby", "createdby")]
            public const string lk_lor_immeuble_createdby = "lk_lor_immeuble_createdby";
            [Relationship("lor_immeuble", EntityRole.Referenced, "lk_lor_immeuble_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_immeuble_createdonbehalfby = "lk_lor_immeuble_createdonbehalfby";
            [Relationship("lor_immeuble", EntityRole.Referenced, "lk_lor_immeuble_modifiedby", "modifiedby")]
            public const string lk_lor_immeuble_modifiedby = "lk_lor_immeuble_modifiedby";
            [Relationship("lor_immeuble", EntityRole.Referenced, "lk_lor_immeuble_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_immeuble_modifiedonbehalfby = "lk_lor_immeuble_modifiedonbehalfby";
            [Relationship("lor_log", EntityRole.Referenced, "lk_lor_log_createdby", "createdby")]
            public const string lk_lor_log_createdby = "lk_lor_log_createdby";
            [Relationship("lor_log", EntityRole.Referenced, "lk_lor_log_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_log_createdonbehalfby = "lk_lor_log_createdonbehalfby";
            [Relationship("lor_log", EntityRole.Referenced, "lk_lor_log_modifiedby", "modifiedby")]
            public const string lk_lor_log_modifiedby = "lk_lor_log_modifiedby";
            [Relationship("lor_log", EntityRole.Referenced, "lk_lor_log_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_log_modifiedonbehalfby = "lk_lor_log_modifiedonbehalfby";
            [Relationship("lor_maintenance", EntityRole.Referenced, "lk_lor_maintenance_createdby", "createdby")]
            public const string lk_lor_maintenance_createdby = "lk_lor_maintenance_createdby";
            [Relationship("lor_maintenance", EntityRole.Referenced, "lk_lor_maintenance_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_maintenance_createdonbehalfby = "lk_lor_maintenance_createdonbehalfby";
            [Relationship("lor_maintenance", EntityRole.Referenced, "lk_lor_maintenance_modifiedby", "modifiedby")]
            public const string lk_lor_maintenance_modifiedby = "lk_lor_maintenance_modifiedby";
            [Relationship("lor_maintenance", EntityRole.Referenced, "lk_lor_maintenance_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_maintenance_modifiedonbehalfby = "lk_lor_maintenance_modifiedonbehalfby";
            [Relationship("lor_parainage", EntityRole.Referenced, "lk_lor_parainage_createdby", "createdby")]
            public const string lk_lor_parainage_createdby = "lk_lor_parainage_createdby";
            [Relationship("lor_parainage", EntityRole.Referenced, "lk_lor_parainage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_parainage_createdonbehalfby = "lk_lor_parainage_createdonbehalfby";
            [Relationship("lor_parainage", EntityRole.Referenced, "lk_lor_parainage_modifiedby", "modifiedby")]
            public const string lk_lor_parainage_modifiedby = "lk_lor_parainage_modifiedby";
            [Relationship("lor_parainage", EntityRole.Referenced, "lk_lor_parainage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_parainage_modifiedonbehalfby = "lk_lor_parainage_modifiedonbehalfby";
            [Relationship("lor_plaqueimmatriculation", EntityRole.Referenced, "lk_lor_plaqueimmatriculation_createdby", "createdby")]
            public const string lk_lor_plaqueimmatriculation_createdby = "lk_lor_plaqueimmatriculation_createdby";
            [Relationship("lor_plaqueimmatriculation", EntityRole.Referenced, "lk_lor_plaqueimmatriculation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_plaqueimmatriculation_createdonbehalfby = "lk_lor_plaqueimmatriculation_createdonbehalfby";
            [Relationship("lor_plaqueimmatriculation", EntityRole.Referenced, "lk_lor_plaqueimmatriculation_modifiedby", "modifiedby")]
            public const string lk_lor_plaqueimmatriculation_modifiedby = "lk_lor_plaqueimmatriculation_modifiedby";
            [Relationship("lor_plaqueimmatriculation", EntityRole.Referenced, "lk_lor_plaqueimmatriculation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_plaqueimmatriculation_modifiedonbehalfby = "lk_lor_plaqueimmatriculation_modifiedonbehalfby";
            [Relationship("lor_quartier", EntityRole.Referenced, "lk_lor_quartier_createdby", "createdby")]
            public const string lk_lor_quartier_createdby = "lk_lor_quartier_createdby";
            [Relationship("lor_quartier", EntityRole.Referenced, "lk_lor_quartier_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_quartier_createdonbehalfby = "lk_lor_quartier_createdonbehalfby";
            [Relationship("lor_quartier", EntityRole.Referenced, "lk_lor_quartier_modifiedby", "modifiedby")]
            public const string lk_lor_quartier_modifiedby = "lk_lor_quartier_modifiedby";
            [Relationship("lor_quartier", EntityRole.Referenced, "lk_lor_quartier_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_quartier_modifiedonbehalfby = "lk_lor_quartier_modifiedonbehalfby";
            [Relationship("lor_route", EntityRole.Referenced, "lk_lor_route_createdby", "createdby")]
            public const string lk_lor_route_createdby = "lk_lor_route_createdby";
            [Relationship("lor_route", EntityRole.Referenced, "lk_lor_route_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_route_createdonbehalfby = "lk_lor_route_createdonbehalfby";
            [Relationship("lor_route", EntityRole.Referenced, "lk_lor_route_modifiedby", "modifiedby")]
            public const string lk_lor_route_modifiedby = "lk_lor_route_modifiedby";
            [Relationship("lor_route", EntityRole.Referenced, "lk_lor_route_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_route_modifiedonbehalfby = "lk_lor_route_modifiedonbehalfby";
            [Relationship("lor_stock", EntityRole.Referenced, "lk_lor_stock_createdby", "createdby")]
            public const string lk_lor_stock_createdby = "lk_lor_stock_createdby";
            [Relationship("lor_stock", EntityRole.Referenced, "lk_lor_stock_createdonbehalfby", "createdonbehalfby")]
            public const string lk_lor_stock_createdonbehalfby = "lk_lor_stock_createdonbehalfby";
            [Relationship("lor_stock", EntityRole.Referenced, "lk_lor_stock_modifiedby", "modifiedby")]
            public const string lk_lor_stock_modifiedby = "lk_lor_stock_modifiedby";
            [Relationship("lor_stock", EntityRole.Referenced, "lk_lor_stock_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_lor_stock_modifiedonbehalfby = "lk_lor_stock_modifiedonbehalfby";
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
            [Relationship("managedidentity", EntityRole.Referenced, "lk_managedidentity_createdby", "createdby")]
            public const string lk_managedidentity_createdby = "lk_managedidentity_createdby";
            [Relationship("managedidentity", EntityRole.Referenced, "lk_managedidentity_createdonbehalfby", "createdonbehalfby")]
            public const string lk_managedidentity_createdonbehalfby = "lk_managedidentity_createdonbehalfby";
            [Relationship("managedidentity", EntityRole.Referenced, "lk_managedidentity_modifiedby", "modifiedby")]
            public const string lk_managedidentity_modifiedby = "lk_managedidentity_modifiedby";
            [Relationship("managedidentity", EntityRole.Referenced, "lk_managedidentity_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_managedidentity_modifiedonbehalfby = "lk_managedidentity_modifiedonbehalfby";
            [Relationship("marketingformdisplayattributes", EntityRole.Referenced, "lk_marketingformdisplayattributes_createdby", "createdby")]
            public const string lk_marketingformdisplayattributes_createdby = "lk_marketingformdisplayattributes_createdby";
            [Relationship("marketingformdisplayattributes", EntityRole.Referenced, "lk_marketingformdisplayattributes_createdonbehalfby", "createdonbehalfby")]
            public const string lk_marketingformdisplayattributes_createdonbehalfby = "lk_marketingformdisplayattributes_createdonbehalfby";
            [Relationship("marketingformdisplayattributes", EntityRole.Referenced, "lk_marketingformdisplayattributes_modifiedby", "modifiedby")]
            public const string lk_marketingformdisplayattributes_modifiedby = "lk_marketingformdisplayattributes_modifiedby";
            [Relationship("marketingformdisplayattributes", EntityRole.Referenced, "lk_marketingformdisplayattributes_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_marketingformdisplayattributes_modifiedonbehalfby = "lk_marketingformdisplayattributes_modifiedonbehalfby";
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
            [Relationship("msdyn_3dmodel", EntityRole.Referenced, "lk_msdyn_3dmodel_createdby", "createdby")]
            public const string lk_msdyn_3dmodel_createdby = "lk_msdyn_3dmodel_createdby";
            [Relationship("msdyn_3dmodel", EntityRole.Referenced, "lk_msdyn_3dmodel_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_3dmodel_createdonbehalfby = "lk_msdyn_3dmodel_createdonbehalfby";
            [Relationship("msdyn_3dmodel", EntityRole.Referenced, "lk_msdyn_3dmodel_modifiedby", "modifiedby")]
            public const string lk_msdyn_3dmodel_modifiedby = "lk_msdyn_3dmodel_modifiedby";
            [Relationship("msdyn_3dmodel", EntityRole.Referenced, "lk_msdyn_3dmodel_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_3dmodel_modifiedonbehalfby = "lk_msdyn_3dmodel_modifiedonbehalfby";
            [Relationship("msdyn_accountkpiitem", EntityRole.Referenced, "lk_msdyn_accountkpiitem_createdby", "createdby")]
            public const string lk_msdyn_accountkpiitem_createdby = "lk_msdyn_accountkpiitem_createdby";
            [Relationship("msdyn_accountkpiitem", EntityRole.Referenced, "lk_msdyn_accountkpiitem_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_accountkpiitem_createdonbehalfby = "lk_msdyn_accountkpiitem_createdonbehalfby";
            [Relationship("msdyn_accountkpiitem", EntityRole.Referenced, "lk_msdyn_accountkpiitem_modifiedby", "modifiedby")]
            public const string lk_msdyn_accountkpiitem_modifiedby = "lk_msdyn_accountkpiitem_modifiedby";
            [Relationship("msdyn_accountkpiitem", EntityRole.Referenced, "lk_msdyn_accountkpiitem_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_accountkpiitem_modifiedonbehalfby = "lk_msdyn_accountkpiitem_modifiedonbehalfby";
            [Relationship("msdyn_actioncardactionstat", EntityRole.Referenced, "lk_msdyn_actioncardactionstat_createdby", "createdby")]
            public const string lk_msdyn_actioncardactionstat_createdby = "lk_msdyn_actioncardactionstat_createdby";
            [Relationship("msdyn_actioncardactionstat", EntityRole.Referenced, "lk_msdyn_actioncardactionstat_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_actioncardactionstat_createdonbehalfby = "lk_msdyn_actioncardactionstat_createdonbehalfby";
            [Relationship("msdyn_actioncardactionstat", EntityRole.Referenced, "lk_msdyn_actioncardactionstat_modifiedby", "modifiedby")]
            public const string lk_msdyn_actioncardactionstat_modifiedby = "lk_msdyn_actioncardactionstat_modifiedby";
            [Relationship("msdyn_actioncardactionstat", EntityRole.Referenced, "lk_msdyn_actioncardactionstat_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_actioncardactionstat_modifiedonbehalfby = "lk_msdyn_actioncardactionstat_modifiedonbehalfby";
            [Relationship("msdyn_actioncardregarding", EntityRole.Referenced, "lk_msdyn_actioncardregarding_createdby", "createdby")]
            public const string lk_msdyn_actioncardregarding_createdby = "lk_msdyn_actioncardregarding_createdby";
            [Relationship("msdyn_actioncardregarding", EntityRole.Referenced, "lk_msdyn_actioncardregarding_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_actioncardregarding_createdonbehalfby = "lk_msdyn_actioncardregarding_createdonbehalfby";
            [Relationship("msdyn_actioncardregarding", EntityRole.Referenced, "lk_msdyn_actioncardregarding_modifiedby", "modifiedby")]
            public const string lk_msdyn_actioncardregarding_modifiedby = "lk_msdyn_actioncardregarding_modifiedby";
            [Relationship("msdyn_actioncardregarding", EntityRole.Referenced, "lk_msdyn_actioncardregarding_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_actioncardregarding_modifiedonbehalfby = "lk_msdyn_actioncardregarding_modifiedonbehalfby";
            [Relationship("msdyn_actioncardrolesetting", EntityRole.Referenced, "lk_msdyn_actioncardrolesetting_createdby", "createdby")]
            public const string lk_msdyn_actioncardrolesetting_createdby = "lk_msdyn_actioncardrolesetting_createdby";
            [Relationship("msdyn_actioncardrolesetting", EntityRole.Referenced, "lk_msdyn_actioncardrolesetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_actioncardrolesetting_createdonbehalfby = "lk_msdyn_actioncardrolesetting_createdonbehalfby";
            [Relationship("msdyn_actioncardrolesetting", EntityRole.Referenced, "lk_msdyn_actioncardrolesetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_actioncardrolesetting_modifiedby = "lk_msdyn_actioncardrolesetting_modifiedby";
            [Relationship("msdyn_actioncardrolesetting", EntityRole.Referenced, "lk_msdyn_actioncardrolesetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_actioncardrolesetting_modifiedonbehalfby = "lk_msdyn_actioncardrolesetting_modifiedonbehalfby";
            [Relationship("msdyn_actioncardstataggregation", EntityRole.Referenced, "lk_msdyn_actioncardstataggregation_createdby", "createdby")]
            public const string lk_msdyn_actioncardstataggregation_createdby = "lk_msdyn_actioncardstataggregation_createdby";
            [Relationship("msdyn_actioncardstataggregation", EntityRole.Referenced, "lk_msdyn_actioncardstataggregation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_actioncardstataggregation_createdonbehalfby = "lk_msdyn_actioncardstataggregation_createdonbehalfby";
            [Relationship("msdyn_actioncardstataggregation", EntityRole.Referenced, "lk_msdyn_actioncardstataggregation_modifiedby", "modifiedby")]
            public const string lk_msdyn_actioncardstataggregation_modifiedby = "lk_msdyn_actioncardstataggregation_modifiedby";
            [Relationship("msdyn_actioncardstataggregation", EntityRole.Referenced, "lk_msdyn_actioncardstataggregation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_actioncardstataggregation_modifiedonbehalfby = "lk_msdyn_actioncardstataggregation_modifiedonbehalfby";
            [Relationship("msdyn_activityanalysiscleanupstate", EntityRole.Referenced, "lk_msdyn_activityanalysiscleanupstate_createdby", "createdby")]
            public const string lk_msdyn_activityanalysiscleanupstate_createdby = "lk_msdyn_activityanalysiscleanupstate_createdby";
            [Relationship("msdyn_activityanalysiscleanupstate", EntityRole.Referenced, "lk_msdyn_activityanalysiscleanupstate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_activityanalysiscleanupstate_createdonbehalfby = "lk_msdyn_activityanalysiscleanupstate_createdonbehalfby";
            [Relationship("msdyn_activityanalysiscleanupstate", EntityRole.Referenced, "lk_msdyn_activityanalysiscleanupstate_modifiedby", "modifiedby")]
            public const string lk_msdyn_activityanalysiscleanupstate_modifiedby = "lk_msdyn_activityanalysiscleanupstate_modifiedby";
            [Relationship("msdyn_activityanalysiscleanupstate", EntityRole.Referenced, "lk_msdyn_activityanalysiscleanupstate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_activityanalysiscleanupstate_modifiedonbehalfby = "lk_msdyn_activityanalysiscleanupstate_modifiedonbehalfby";
            [Relationship("msdyn_activityanalysisconfig", EntityRole.Referenced, "lk_msdyn_activityanalysisconfig_createdby", "createdby")]
            public const string lk_msdyn_activityanalysisconfig_createdby = "lk_msdyn_activityanalysisconfig_createdby";
            [Relationship("msdyn_activityanalysisconfig", EntityRole.Referenced, "lk_msdyn_activityanalysisconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_activityanalysisconfig_createdonbehalfby = "lk_msdyn_activityanalysisconfig_createdonbehalfby";
            [Relationship("msdyn_activityanalysisconfig", EntityRole.Referenced, "lk_msdyn_activityanalysisconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_activityanalysisconfig_modifiedby = "lk_msdyn_activityanalysisconfig_modifiedby";
            [Relationship("msdyn_activityanalysisconfig", EntityRole.Referenced, "lk_msdyn_activityanalysisconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_activityanalysisconfig_modifiedonbehalfby = "lk_msdyn_activityanalysisconfig_modifiedonbehalfby";
            [Relationship("msdyn_actual", EntityRole.Referenced, "lk_msdyn_actual_createdby", "createdby")]
            public const string lk_msdyn_actual_createdby = "lk_msdyn_actual_createdby";
            [Relationship("msdyn_actual", EntityRole.Referenced, "lk_msdyn_actual_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_actual_createdonbehalfby = "lk_msdyn_actual_createdonbehalfby";
            [Relationship("msdyn_actual", EntityRole.Referenced, "lk_msdyn_actual_modifiedby", "modifiedby")]
            public const string lk_msdyn_actual_modifiedby = "lk_msdyn_actual_modifiedby";
            [Relationship("msdyn_actual", EntityRole.Referenced, "lk_msdyn_actual_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_actual_modifiedonbehalfby = "lk_msdyn_actual_modifiedonbehalfby";
            [Relationship("msdyn_adaptivecardconfiguration", EntityRole.Referenced, "lk_msdyn_adaptivecardconfiguration_createdby", "createdby")]
            public const string lk_msdyn_adaptivecardconfiguration_createdby = "lk_msdyn_adaptivecardconfiguration_createdby";
            [Relationship("msdyn_adaptivecardconfiguration", EntityRole.Referenced, "lk_msdyn_adaptivecardconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_adaptivecardconfiguration_createdonbehalfby = "lk_msdyn_adaptivecardconfiguration_createdonbehalfby";
            [Relationship("msdyn_adaptivecardconfiguration", EntityRole.Referenced, "lk_msdyn_adaptivecardconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_adaptivecardconfiguration_modifiedby = "lk_msdyn_adaptivecardconfiguration_modifiedby";
            [Relationship("msdyn_adaptivecardconfiguration", EntityRole.Referenced, "lk_msdyn_adaptivecardconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_adaptivecardconfiguration_modifiedonbehalfby = "lk_msdyn_adaptivecardconfiguration_modifiedonbehalfby";
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
            [Relationship("msdyn_aibdataset", EntityRole.Referenced, "lk_msdyn_aibdataset_createdby", "createdby")]
            public const string lk_msdyn_aibdataset_createdby = "lk_msdyn_aibdataset_createdby";
            [Relationship("msdyn_aibdataset", EntityRole.Referenced, "lk_msdyn_aibdataset_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aibdataset_createdonbehalfby = "lk_msdyn_aibdataset_createdonbehalfby";
            [Relationship("msdyn_aibdataset", EntityRole.Referenced, "lk_msdyn_aibdataset_modifiedby", "modifiedby")]
            public const string lk_msdyn_aibdataset_modifiedby = "lk_msdyn_aibdataset_modifiedby";
            [Relationship("msdyn_aibdataset", EntityRole.Referenced, "lk_msdyn_aibdataset_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aibdataset_modifiedonbehalfby = "lk_msdyn_aibdataset_modifiedonbehalfby";
            [Relationship("msdyn_aibdatasetfile", EntityRole.Referenced, "lk_msdyn_aibdatasetfile_createdby", "createdby")]
            public const string lk_msdyn_aibdatasetfile_createdby = "lk_msdyn_aibdatasetfile_createdby";
            [Relationship("msdyn_aibdatasetfile", EntityRole.Referenced, "lk_msdyn_aibdatasetfile_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aibdatasetfile_createdonbehalfby = "lk_msdyn_aibdatasetfile_createdonbehalfby";
            [Relationship("msdyn_aibdatasetfile", EntityRole.Referenced, "lk_msdyn_aibdatasetfile_modifiedby", "modifiedby")]
            public const string lk_msdyn_aibdatasetfile_modifiedby = "lk_msdyn_aibdatasetfile_modifiedby";
            [Relationship("msdyn_aibdatasetfile", EntityRole.Referenced, "lk_msdyn_aibdatasetfile_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aibdatasetfile_modifiedonbehalfby = "lk_msdyn_aibdatasetfile_modifiedonbehalfby";
            [Relationship("msdyn_aibdatasetrecord", EntityRole.Referenced, "lk_msdyn_aibdatasetrecord_createdby", "createdby")]
            public const string lk_msdyn_aibdatasetrecord_createdby = "lk_msdyn_aibdatasetrecord_createdby";
            [Relationship("msdyn_aibdatasetrecord", EntityRole.Referenced, "lk_msdyn_aibdatasetrecord_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aibdatasetrecord_createdonbehalfby = "lk_msdyn_aibdatasetrecord_createdonbehalfby";
            [Relationship("msdyn_aibdatasetrecord", EntityRole.Referenced, "lk_msdyn_aibdatasetrecord_modifiedby", "modifiedby")]
            public const string lk_msdyn_aibdatasetrecord_modifiedby = "lk_msdyn_aibdatasetrecord_modifiedby";
            [Relationship("msdyn_aibdatasetrecord", EntityRole.Referenced, "lk_msdyn_aibdatasetrecord_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aibdatasetrecord_modifiedonbehalfby = "lk_msdyn_aibdatasetrecord_modifiedonbehalfby";
            [Relationship("msdyn_aibdatasetscontainer", EntityRole.Referenced, "lk_msdyn_aibdatasetscontainer_createdby", "createdby")]
            public const string lk_msdyn_aibdatasetscontainer_createdby = "lk_msdyn_aibdatasetscontainer_createdby";
            [Relationship("msdyn_aibdatasetscontainer", EntityRole.Referenced, "lk_msdyn_aibdatasetscontainer_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aibdatasetscontainer_createdonbehalfby = "lk_msdyn_aibdatasetscontainer_createdonbehalfby";
            [Relationship("msdyn_aibdatasetscontainer", EntityRole.Referenced, "lk_msdyn_aibdatasetscontainer_modifiedby", "modifiedby")]
            public const string lk_msdyn_aibdatasetscontainer_modifiedby = "lk_msdyn_aibdatasetscontainer_modifiedby";
            [Relationship("msdyn_aibdatasetscontainer", EntityRole.Referenced, "lk_msdyn_aibdatasetscontainer_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aibdatasetscontainer_modifiedonbehalfby = "lk_msdyn_aibdatasetscontainer_modifiedonbehalfby";
            [Relationship("msdyn_aibfile", EntityRole.Referenced, "lk_msdyn_aibfile_createdby", "createdby")]
            public const string lk_msdyn_aibfile_createdby = "lk_msdyn_aibfile_createdby";
            [Relationship("msdyn_aibfile", EntityRole.Referenced, "lk_msdyn_aibfile_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aibfile_createdonbehalfby = "lk_msdyn_aibfile_createdonbehalfby";
            [Relationship("msdyn_aibfile", EntityRole.Referenced, "lk_msdyn_aibfile_modifiedby", "modifiedby")]
            public const string lk_msdyn_aibfile_modifiedby = "lk_msdyn_aibfile_modifiedby";
            [Relationship("msdyn_aibfile", EntityRole.Referenced, "lk_msdyn_aibfile_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aibfile_modifiedonbehalfby = "lk_msdyn_aibfile_modifiedonbehalfby";
            [Relationship("msdyn_aibfileattacheddata", EntityRole.Referenced, "lk_msdyn_aibfileattacheddata_createdby", "createdby")]
            public const string lk_msdyn_aibfileattacheddata_createdby = "lk_msdyn_aibfileattacheddata_createdby";
            [Relationship("msdyn_aibfileattacheddata", EntityRole.Referenced, "lk_msdyn_aibfileattacheddata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aibfileattacheddata_createdonbehalfby = "lk_msdyn_aibfileattacheddata_createdonbehalfby";
            [Relationship("msdyn_aibfileattacheddata", EntityRole.Referenced, "lk_msdyn_aibfileattacheddata_modifiedby", "modifiedby")]
            public const string lk_msdyn_aibfileattacheddata_modifiedby = "lk_msdyn_aibfileattacheddata_modifiedby";
            [Relationship("msdyn_aibfileattacheddata", EntityRole.Referenced, "lk_msdyn_aibfileattacheddata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aibfileattacheddata_modifiedonbehalfby = "lk_msdyn_aibfileattacheddata_modifiedonbehalfby";
            [Relationship("msdyn_aiconfiguration", EntityRole.Referenced, "lk_msdyn_aiconfiguration_createdby", "createdby")]
            public const string lk_msdyn_aiconfiguration_createdby = "lk_msdyn_aiconfiguration_createdby";
            [Relationship("msdyn_aiconfiguration", EntityRole.Referenced, "lk_msdyn_aiconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aiconfiguration_createdonbehalfby = "lk_msdyn_aiconfiguration_createdonbehalfby";
            [Relationship("msdyn_aiconfiguration", EntityRole.Referenced, "lk_msdyn_aiconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_aiconfiguration_modifiedby = "lk_msdyn_aiconfiguration_modifiedby";
            [Relationship("msdyn_aiconfiguration", EntityRole.Referenced, "lk_msdyn_aiconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aiconfiguration_modifiedonbehalfby = "lk_msdyn_aiconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_aicontactsuggestion", EntityRole.Referenced, "lk_msdyn_aicontactsuggestion_createdby", "createdby")]
            public const string lk_msdyn_aicontactsuggestion_createdby = "lk_msdyn_aicontactsuggestion_createdby";
            [Relationship("msdyn_aicontactsuggestion", EntityRole.Referenced, "lk_msdyn_aicontactsuggestion_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aicontactsuggestion_createdonbehalfby = "lk_msdyn_aicontactsuggestion_createdonbehalfby";
            [Relationship("msdyn_aicontactsuggestion", EntityRole.Referenced, "lk_msdyn_aicontactsuggestion_modifiedby", "modifiedby")]
            public const string lk_msdyn_aicontactsuggestion_modifiedby = "lk_msdyn_aicontactsuggestion_modifiedby";
            [Relationship("msdyn_aicontactsuggestion", EntityRole.Referenced, "lk_msdyn_aicontactsuggestion_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aicontactsuggestion_modifiedonbehalfby = "lk_msdyn_aicontactsuggestion_modifiedonbehalfby";
            [Relationship("msdyn_aifptrainingdocument", EntityRole.Referenced, "lk_msdyn_aifptrainingdocument_createdby", "createdby")]
            public const string lk_msdyn_aifptrainingdocument_createdby = "lk_msdyn_aifptrainingdocument_createdby";
            [Relationship("msdyn_aifptrainingdocument", EntityRole.Referenced, "lk_msdyn_aifptrainingdocument_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aifptrainingdocument_createdonbehalfby = "lk_msdyn_aifptrainingdocument_createdonbehalfby";
            [Relationship("msdyn_aifptrainingdocument", EntityRole.Referenced, "lk_msdyn_aifptrainingdocument_modifiedby", "modifiedby")]
            public const string lk_msdyn_aifptrainingdocument_modifiedby = "lk_msdyn_aifptrainingdocument_modifiedby";
            [Relationship("msdyn_aifptrainingdocument", EntityRole.Referenced, "lk_msdyn_aifptrainingdocument_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aifptrainingdocument_modifiedonbehalfby = "lk_msdyn_aifptrainingdocument_modifiedonbehalfby";
            [Relationship("msdyn_aimodel", EntityRole.Referenced, "lk_msdyn_aimodel_createdby", "createdby")]
            public const string lk_msdyn_aimodel_createdby = "lk_msdyn_aimodel_createdby";
            [Relationship("msdyn_aimodel", EntityRole.Referenced, "lk_msdyn_aimodel_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aimodel_createdonbehalfby = "lk_msdyn_aimodel_createdonbehalfby";
            [Relationship("msdyn_aimodel", EntityRole.Referenced, "lk_msdyn_aimodel_modifiedby", "modifiedby")]
            public const string lk_msdyn_aimodel_modifiedby = "lk_msdyn_aimodel_modifiedby";
            [Relationship("msdyn_aimodel", EntityRole.Referenced, "lk_msdyn_aimodel_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aimodel_modifiedonbehalfby = "lk_msdyn_aimodel_modifiedonbehalfby";
            [Relationship("msdyn_aiodimage", EntityRole.Referenced, "lk_msdyn_aiodimage_createdby", "createdby")]
            public const string lk_msdyn_aiodimage_createdby = "lk_msdyn_aiodimage_createdby";
            [Relationship("msdyn_aiodimage", EntityRole.Referenced, "lk_msdyn_aiodimage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aiodimage_createdonbehalfby = "lk_msdyn_aiodimage_createdonbehalfby";
            [Relationship("msdyn_aiodimage", EntityRole.Referenced, "lk_msdyn_aiodimage_modifiedby", "modifiedby")]
            public const string lk_msdyn_aiodimage_modifiedby = "lk_msdyn_aiodimage_modifiedby";
            [Relationship("msdyn_aiodimage", EntityRole.Referenced, "lk_msdyn_aiodimage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aiodimage_modifiedonbehalfby = "lk_msdyn_aiodimage_modifiedonbehalfby";
            [Relationship("msdyn_aiodlabel", EntityRole.Referenced, "lk_msdyn_aiodlabel_createdby", "createdby")]
            public const string lk_msdyn_aiodlabel_createdby = "lk_msdyn_aiodlabel_createdby";
            [Relationship("msdyn_aiodlabel", EntityRole.Referenced, "lk_msdyn_aiodlabel_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aiodlabel_createdonbehalfby = "lk_msdyn_aiodlabel_createdonbehalfby";
            [Relationship("msdyn_aiodlabel", EntityRole.Referenced, "lk_msdyn_aiodlabel_modifiedby", "modifiedby")]
            public const string lk_msdyn_aiodlabel_modifiedby = "lk_msdyn_aiodlabel_modifiedby";
            [Relationship("msdyn_aiodlabel", EntityRole.Referenced, "lk_msdyn_aiodlabel_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aiodlabel_modifiedonbehalfby = "lk_msdyn_aiodlabel_modifiedonbehalfby";
            [Relationship("msdyn_aiodtrainingboundingbox", EntityRole.Referenced, "lk_msdyn_aiodtrainingboundingbox_createdby", "createdby")]
            public const string lk_msdyn_aiodtrainingboundingbox_createdby = "lk_msdyn_aiodtrainingboundingbox_createdby";
            [Relationship("msdyn_aiodtrainingboundingbox", EntityRole.Referenced, "lk_msdyn_aiodtrainingboundingbox_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aiodtrainingboundingbox_createdonbehalfby = "lk_msdyn_aiodtrainingboundingbox_createdonbehalfby";
            [Relationship("msdyn_aiodtrainingboundingbox", EntityRole.Referenced, "lk_msdyn_aiodtrainingboundingbox_modifiedby", "modifiedby")]
            public const string lk_msdyn_aiodtrainingboundingbox_modifiedby = "lk_msdyn_aiodtrainingboundingbox_modifiedby";
            [Relationship("msdyn_aiodtrainingboundingbox", EntityRole.Referenced, "lk_msdyn_aiodtrainingboundingbox_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aiodtrainingboundingbox_modifiedonbehalfby = "lk_msdyn_aiodtrainingboundingbox_modifiedonbehalfby";
            [Relationship("msdyn_aiodtrainingimage", EntityRole.Referenced, "lk_msdyn_aiodtrainingimage_createdby", "createdby")]
            public const string lk_msdyn_aiodtrainingimage_createdby = "lk_msdyn_aiodtrainingimage_createdby";
            [Relationship("msdyn_aiodtrainingimage", EntityRole.Referenced, "lk_msdyn_aiodtrainingimage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aiodtrainingimage_createdonbehalfby = "lk_msdyn_aiodtrainingimage_createdonbehalfby";
            [Relationship("msdyn_aiodtrainingimage", EntityRole.Referenced, "lk_msdyn_aiodtrainingimage_modifiedby", "modifiedby")]
            public const string lk_msdyn_aiodtrainingimage_modifiedby = "lk_msdyn_aiodtrainingimage_modifiedby";
            [Relationship("msdyn_aiodtrainingimage", EntityRole.Referenced, "lk_msdyn_aiodtrainingimage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aiodtrainingimage_modifiedonbehalfby = "lk_msdyn_aiodtrainingimage_modifiedonbehalfby";
            [Relationship("msdyn_aitemplate", EntityRole.Referenced, "lk_msdyn_aitemplate_createdby", "createdby")]
            public const string lk_msdyn_aitemplate_createdby = "lk_msdyn_aitemplate_createdby";
            [Relationship("msdyn_aitemplate", EntityRole.Referenced, "lk_msdyn_aitemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_aitemplate_createdonbehalfby = "lk_msdyn_aitemplate_createdonbehalfby";
            [Relationship("msdyn_aitemplate", EntityRole.Referenced, "lk_msdyn_aitemplate_modifiedby", "modifiedby")]
            public const string lk_msdyn_aitemplate_modifiedby = "lk_msdyn_aitemplate_modifiedby";
            [Relationship("msdyn_aitemplate", EntityRole.Referenced, "lk_msdyn_aitemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_aitemplate_modifiedonbehalfby = "lk_msdyn_aitemplate_modifiedonbehalfby";
            [Relationship("msdyn_analysiscomponent", EntityRole.Referenced, "lk_msdyn_analysiscomponent_createdby", "createdby")]
            public const string lk_msdyn_analysiscomponent_createdby = "lk_msdyn_analysiscomponent_createdby";
            [Relationship("msdyn_analysiscomponent", EntityRole.Referenced, "lk_msdyn_analysiscomponent_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_analysiscomponent_createdonbehalfby = "lk_msdyn_analysiscomponent_createdonbehalfby";
            [Relationship("msdyn_analysiscomponent", EntityRole.Referenced, "lk_msdyn_analysiscomponent_modifiedby", "modifiedby")]
            public const string lk_msdyn_analysiscomponent_modifiedby = "lk_msdyn_analysiscomponent_modifiedby";
            [Relationship("msdyn_analysiscomponent", EntityRole.Referenced, "lk_msdyn_analysiscomponent_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_analysiscomponent_modifiedonbehalfby = "lk_msdyn_analysiscomponent_modifiedonbehalfby";
            [Relationship("msdyn_analysisjob", EntityRole.Referenced, "lk_msdyn_analysisjob_createdby", "createdby")]
            public const string lk_msdyn_analysisjob_createdby = "lk_msdyn_analysisjob_createdby";
            [Relationship("msdyn_analysisjob", EntityRole.Referenced, "lk_msdyn_analysisjob_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_analysisjob_createdonbehalfby = "lk_msdyn_analysisjob_createdonbehalfby";
            [Relationship("msdyn_analysisjob", EntityRole.Referenced, "lk_msdyn_analysisjob_modifiedby", "modifiedby")]
            public const string lk_msdyn_analysisjob_modifiedby = "lk_msdyn_analysisjob_modifiedby";
            [Relationship("msdyn_analysisjob", EntityRole.Referenced, "lk_msdyn_analysisjob_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_analysisjob_modifiedonbehalfby = "lk_msdyn_analysisjob_modifiedonbehalfby";
            [Relationship("msdyn_analysisresult", EntityRole.Referenced, "lk_msdyn_analysisresult_createdby", "createdby")]
            public const string lk_msdyn_analysisresult_createdby = "lk_msdyn_analysisresult_createdby";
            [Relationship("msdyn_analysisresult", EntityRole.Referenced, "lk_msdyn_analysisresult_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_analysisresult_createdonbehalfby = "lk_msdyn_analysisresult_createdonbehalfby";
            [Relationship("msdyn_analysisresult", EntityRole.Referenced, "lk_msdyn_analysisresult_modifiedby", "modifiedby")]
            public const string lk_msdyn_analysisresult_modifiedby = "lk_msdyn_analysisresult_modifiedby";
            [Relationship("msdyn_analysisresult", EntityRole.Referenced, "lk_msdyn_analysisresult_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_analysisresult_modifiedonbehalfby = "lk_msdyn_analysisresult_modifiedonbehalfby";
            [Relationship("msdyn_analysisresultdetail", EntityRole.Referenced, "lk_msdyn_analysisresultdetail_createdby", "createdby")]
            public const string lk_msdyn_analysisresultdetail_createdby = "lk_msdyn_analysisresultdetail_createdby";
            [Relationship("msdyn_analysisresultdetail", EntityRole.Referenced, "lk_msdyn_analysisresultdetail_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_analysisresultdetail_createdonbehalfby = "lk_msdyn_analysisresultdetail_createdonbehalfby";
            [Relationship("msdyn_analysisresultdetail", EntityRole.Referenced, "lk_msdyn_analysisresultdetail_modifiedby", "modifiedby")]
            public const string lk_msdyn_analysisresultdetail_modifiedby = "lk_msdyn_analysisresultdetail_modifiedby";
            [Relationship("msdyn_analysisresultdetail", EntityRole.Referenced, "lk_msdyn_analysisresultdetail_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_analysisresultdetail_modifiedonbehalfby = "lk_msdyn_analysisresultdetail_modifiedonbehalfby";
            [Relationship("msdyn_analyticsadminsettings", EntityRole.Referenced, "lk_msdyn_analyticsadminsettings_createdby", "createdby")]
            public const string lk_msdyn_analyticsadminsettings_createdby = "lk_msdyn_analyticsadminsettings_createdby";
            [Relationship("msdyn_analyticsadminsettings", EntityRole.Referenced, "lk_msdyn_analyticsadminsettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_analyticsadminsettings_createdonbehalfby = "lk_msdyn_analyticsadminsettings_createdonbehalfby";
            [Relationship("msdyn_analyticsadminsettings", EntityRole.Referenced, "lk_msdyn_analyticsadminsettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_analyticsadminsettings_modifiedby = "lk_msdyn_analyticsadminsettings_modifiedby";
            [Relationship("msdyn_analyticsadminsettings", EntityRole.Referenced, "lk_msdyn_analyticsadminsettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_analyticsadminsettings_modifiedonbehalfby = "lk_msdyn_analyticsadminsettings_modifiedonbehalfby";
            [Relationship("msdyn_analyticsforcs", EntityRole.Referenced, "lk_msdyn_analyticsforcs_createdby", "createdby")]
            public const string lk_msdyn_analyticsforcs_createdby = "lk_msdyn_analyticsforcs_createdby";
            [Relationship("msdyn_analyticsforcs", EntityRole.Referenced, "lk_msdyn_analyticsforcs_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_analyticsforcs_createdonbehalfby = "lk_msdyn_analyticsforcs_createdonbehalfby";
            [Relationship("msdyn_analyticsforcs", EntityRole.Referenced, "lk_msdyn_analyticsforcs_modifiedby", "modifiedby")]
            public const string lk_msdyn_analyticsforcs_modifiedby = "lk_msdyn_analyticsforcs_modifiedby";
            [Relationship("msdyn_analyticsforcs", EntityRole.Referenced, "lk_msdyn_analyticsforcs_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_analyticsforcs_modifiedonbehalfby = "lk_msdyn_analyticsforcs_modifiedonbehalfby";
            [Relationship("msdyn_appconfiguration", EntityRole.Referenced, "lk_msdyn_appconfiguration_createdby", "createdby")]
            public const string lk_msdyn_appconfiguration_createdby = "lk_msdyn_appconfiguration_createdby";
            [Relationship("msdyn_appconfiguration", EntityRole.Referenced, "lk_msdyn_appconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_appconfiguration_createdonbehalfby = "lk_msdyn_appconfiguration_createdonbehalfby";
            [Relationship("msdyn_appconfiguration", EntityRole.Referenced, "lk_msdyn_appconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_appconfiguration_modifiedby = "lk_msdyn_appconfiguration_modifiedby";
            [Relationship("msdyn_appconfiguration", EntityRole.Referenced, "lk_msdyn_appconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_appconfiguration_modifiedonbehalfby = "lk_msdyn_appconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_applicationextension", EntityRole.Referenced, "lk_msdyn_applicationextension_createdby", "createdby")]
            public const string lk_msdyn_applicationextension_createdby = "lk_msdyn_applicationextension_createdby";
            [Relationship("msdyn_applicationextension", EntityRole.Referenced, "lk_msdyn_applicationextension_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_applicationextension_createdonbehalfby = "lk_msdyn_applicationextension_createdonbehalfby";
            [Relationship("msdyn_applicationextension", EntityRole.Referenced, "lk_msdyn_applicationextension_modifiedby", "modifiedby")]
            public const string lk_msdyn_applicationextension_modifiedby = "lk_msdyn_applicationextension_modifiedby";
            [Relationship("msdyn_applicationextension", EntityRole.Referenced, "lk_msdyn_applicationextension_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_applicationextension_modifiedonbehalfby = "lk_msdyn_applicationextension_modifiedonbehalfby";
            [Relationship("msdyn_applicationtabtemplate", EntityRole.Referenced, "lk_msdyn_applicationtabtemplate_createdby", "createdby")]
            public const string lk_msdyn_applicationtabtemplate_createdby = "lk_msdyn_applicationtabtemplate_createdby";
            [Relationship("msdyn_applicationtabtemplate", EntityRole.Referenced, "lk_msdyn_applicationtabtemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_applicationtabtemplate_createdonbehalfby = "lk_msdyn_applicationtabtemplate_createdonbehalfby";
            [Relationship("msdyn_applicationtabtemplate", EntityRole.Referenced, "lk_msdyn_applicationtabtemplate_modifiedby", "modifiedby")]
            public const string lk_msdyn_applicationtabtemplate_modifiedby = "lk_msdyn_applicationtabtemplate_modifiedby";
            [Relationship("msdyn_applicationtabtemplate", EntityRole.Referenced, "lk_msdyn_applicationtabtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_applicationtabtemplate_modifiedonbehalfby = "lk_msdyn_applicationtabtemplate_modifiedonbehalfby";
            [Relationship("msdyn_assetcategorytemplateassociation", EntityRole.Referenced, "lk_msdyn_assetcategorytemplateassociation_createdby", "createdby")]
            public const string lk_msdyn_assetcategorytemplateassociation_createdby = "lk_msdyn_assetcategorytemplateassociation_createdby";
            [Relationship("msdyn_assetcategorytemplateassociation", EntityRole.Referenced, "lk_msdyn_assetcategorytemplateassociation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_assetcategorytemplateassociation_createdonbehalfby = "lk_msdyn_assetcategorytemplateassociation_createdonbehalfby";
            [Relationship("msdyn_assetcategorytemplateassociation", EntityRole.Referenced, "lk_msdyn_assetcategorytemplateassociation_modifiedby", "modifiedby")]
            public const string lk_msdyn_assetcategorytemplateassociation_modifiedby = "lk_msdyn_assetcategorytemplateassociation_modifiedby";
            [Relationship("msdyn_assetcategorytemplateassociation", EntityRole.Referenced, "lk_msdyn_assetcategorytemplateassociation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_assetcategorytemplateassociation_modifiedonbehalfby = "lk_msdyn_assetcategorytemplateassociation_modifiedonbehalfby";
            [Relationship("msdyn_assetsuggestionssetting", EntityRole.Referenced, "lk_msdyn_assetsuggestionssetting_createdby", "createdby")]
            public const string lk_msdyn_assetsuggestionssetting_createdby = "lk_msdyn_assetsuggestionssetting_createdby";
            [Relationship("msdyn_assetsuggestionssetting", EntityRole.Referenced, "lk_msdyn_assetsuggestionssetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_assetsuggestionssetting_createdonbehalfby = "lk_msdyn_assetsuggestionssetting_createdonbehalfby";
            [Relationship("msdyn_assetsuggestionssetting", EntityRole.Referenced, "lk_msdyn_assetsuggestionssetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_assetsuggestionssetting_modifiedby = "lk_msdyn_assetsuggestionssetting_modifiedby";
            [Relationship("msdyn_assetsuggestionssetting", EntityRole.Referenced, "lk_msdyn_assetsuggestionssetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_assetsuggestionssetting_modifiedonbehalfby = "lk_msdyn_assetsuggestionssetting_modifiedonbehalfby";
            [Relationship("msdyn_assettemplateassociation", EntityRole.Referenced, "lk_msdyn_assettemplateassociation_createdby", "createdby")]
            public const string lk_msdyn_assettemplateassociation_createdby = "lk_msdyn_assettemplateassociation_createdby";
            [Relationship("msdyn_assettemplateassociation", EntityRole.Referenced, "lk_msdyn_assettemplateassociation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_assettemplateassociation_createdonbehalfby = "lk_msdyn_assettemplateassociation_createdonbehalfby";
            [Relationship("msdyn_assettemplateassociation", EntityRole.Referenced, "lk_msdyn_assettemplateassociation_modifiedby", "modifiedby")]
            public const string lk_msdyn_assettemplateassociation_modifiedby = "lk_msdyn_assettemplateassociation_modifiedby";
            [Relationship("msdyn_assettemplateassociation", EntityRole.Referenced, "lk_msdyn_assettemplateassociation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_assettemplateassociation_modifiedonbehalfby = "lk_msdyn_assettemplateassociation_modifiedonbehalfby";
            [Relationship("msdyn_assignmentmap", EntityRole.Referenced, "lk_msdyn_assignmentmap_createdby", "createdby")]
            public const string lk_msdyn_assignmentmap_createdby = "lk_msdyn_assignmentmap_createdby";
            [Relationship("msdyn_assignmentmap", EntityRole.Referenced, "lk_msdyn_assignmentmap_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_assignmentmap_createdonbehalfby = "lk_msdyn_assignmentmap_createdonbehalfby";
            [Relationship("msdyn_assignmentmap", EntityRole.Referenced, "lk_msdyn_assignmentmap_modifiedby", "modifiedby")]
            public const string lk_msdyn_assignmentmap_modifiedby = "lk_msdyn_assignmentmap_modifiedby";
            [Relationship("msdyn_assignmentmap", EntityRole.Referenced, "lk_msdyn_assignmentmap_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_assignmentmap_modifiedonbehalfby = "lk_msdyn_assignmentmap_modifiedonbehalfby";
            [Relationship("msdyn_assignmentrule", EntityRole.Referenced, "lk_msdyn_assignmentrule_createdby", "createdby")]
            public const string lk_msdyn_assignmentrule_createdby = "lk_msdyn_assignmentrule_createdby";
            [Relationship("msdyn_assignmentrule", EntityRole.Referenced, "lk_msdyn_assignmentrule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_assignmentrule_createdonbehalfby = "lk_msdyn_assignmentrule_createdonbehalfby";
            [Relationship("msdyn_assignmentrule", EntityRole.Referenced, "lk_msdyn_assignmentrule_modifiedby", "modifiedby")]
            public const string lk_msdyn_assignmentrule_modifiedby = "lk_msdyn_assignmentrule_modifiedby";
            [Relationship("msdyn_assignmentrule", EntityRole.Referenced, "lk_msdyn_assignmentrule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_assignmentrule_modifiedonbehalfby = "lk_msdyn_assignmentrule_modifiedonbehalfby";
            [Relationship("msdyn_attribute", EntityRole.Referenced, "lk_msdyn_attribute_createdby", "createdby")]
            public const string lk_msdyn_attribute_createdby = "lk_msdyn_attribute_createdby";
            [Relationship("msdyn_attribute", EntityRole.Referenced, "lk_msdyn_attribute_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_attribute_createdonbehalfby = "lk_msdyn_attribute_createdonbehalfby";
            [Relationship("msdyn_attribute", EntityRole.Referenced, "lk_msdyn_attribute_modifiedby", "modifiedby")]
            public const string lk_msdyn_attribute_modifiedby = "lk_msdyn_attribute_modifiedby";
            [Relationship("msdyn_attribute", EntityRole.Referenced, "lk_msdyn_attribute_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_attribute_modifiedonbehalfby = "lk_msdyn_attribute_modifiedonbehalfby";
            [Relationship("msdyn_attributevalue", EntityRole.Referenced, "lk_msdyn_attributevalue_createdby", "createdby")]
            public const string lk_msdyn_attributevalue_createdby = "lk_msdyn_attributevalue_createdby";
            [Relationship("msdyn_attributevalue", EntityRole.Referenced, "lk_msdyn_attributevalue_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_attributevalue_createdonbehalfby = "lk_msdyn_attributevalue_createdonbehalfby";
            [Relationship("msdyn_attributevalue", EntityRole.Referenced, "lk_msdyn_attributevalue_modifiedby", "modifiedby")]
            public const string lk_msdyn_attributevalue_modifiedby = "lk_msdyn_attributevalue_modifiedby";
            [Relationship("msdyn_attributevalue", EntityRole.Referenced, "lk_msdyn_attributevalue_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_attributevalue_modifiedonbehalfby = "lk_msdyn_attributevalue_modifiedonbehalfby";
            [Relationship("msdyn_autocapturerule", EntityRole.Referenced, "lk_msdyn_autocapturerule_createdby", "createdby")]
            public const string lk_msdyn_autocapturerule_createdby = "lk_msdyn_autocapturerule_createdby";
            [Relationship("msdyn_autocapturerule", EntityRole.Referenced, "lk_msdyn_autocapturerule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_autocapturerule_createdonbehalfby = "lk_msdyn_autocapturerule_createdonbehalfby";
            [Relationship("msdyn_autocapturerule", EntityRole.Referenced, "lk_msdyn_autocapturerule_modifiedby", "modifiedby")]
            public const string lk_msdyn_autocapturerule_modifiedby = "lk_msdyn_autocapturerule_modifiedby";
            [Relationship("msdyn_autocapturerule", EntityRole.Referenced, "lk_msdyn_autocapturerule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_autocapturerule_modifiedonbehalfby = "lk_msdyn_autocapturerule_modifiedonbehalfby";
            [Relationship("msdyn_autocapturesettings", EntityRole.Referenced, "lk_msdyn_autocapturesettings_createdby", "createdby")]
            public const string lk_msdyn_autocapturesettings_createdby = "lk_msdyn_autocapturesettings_createdby";
            [Relationship("msdyn_autocapturesettings", EntityRole.Referenced, "lk_msdyn_autocapturesettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_autocapturesettings_createdonbehalfby = "lk_msdyn_autocapturesettings_createdonbehalfby";
            [Relationship("msdyn_autocapturesettings", EntityRole.Referenced, "lk_msdyn_autocapturesettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_autocapturesettings_modifiedby = "lk_msdyn_autocapturesettings_modifiedby";
            [Relationship("msdyn_autocapturesettings", EntityRole.Referenced, "lk_msdyn_autocapturesettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_autocapturesettings_modifiedonbehalfby = "lk_msdyn_autocapturesettings_modifiedonbehalfby";
            [Relationship("msdyn_bookableresourceassociation", EntityRole.Referenced, "lk_msdyn_bookableresourceassociation_createdby", "createdby")]
            public const string lk_msdyn_bookableresourceassociation_createdby = "lk_msdyn_bookableresourceassociation_createdby";
            [Relationship("msdyn_bookableresourceassociation", EntityRole.Referenced, "lk_msdyn_bookableresourceassociation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_bookableresourceassociation_createdonbehalfby = "lk_msdyn_bookableresourceassociation_createdonbehalfby";
            [Relationship("msdyn_bookableresourceassociation", EntityRole.Referenced, "lk_msdyn_bookableresourceassociation_modifiedby", "modifiedby")]
            public const string lk_msdyn_bookableresourceassociation_modifiedby = "lk_msdyn_bookableresourceassociation_modifiedby";
            [Relationship("msdyn_bookableresourceassociation", EntityRole.Referenced, "lk_msdyn_bookableresourceassociation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_bookableresourceassociation_modifiedonbehalfby = "lk_msdyn_bookableresourceassociation_modifiedonbehalfby";
            [Relationship("msdyn_bookableresourcebookingquicknote", EntityRole.Referenced, "lk_msdyn_bookableresourcebookingquicknote_createdby", "createdby")]
            public const string lk_msdyn_bookableresourcebookingquicknote_createdby = "lk_msdyn_bookableresourcebookingquicknote_createdby";
            [Relationship("msdyn_bookableresourcebookingquicknote", EntityRole.Referenced, "lk_msdyn_bookableresourcebookingquicknote_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_bookableresourcebookingquicknote_createdonbehalfby = "lk_msdyn_bookableresourcebookingquicknote_createdonbehalfby";
            [Relationship("msdyn_bookableresourcebookingquicknote", EntityRole.Referenced, "lk_msdyn_bookableresourcebookingquicknote_modifiedby", "modifiedby")]
            public const string lk_msdyn_bookableresourcebookingquicknote_modifiedby = "lk_msdyn_bookableresourcebookingquicknote_modifiedby";
            [Relationship("msdyn_bookableresourcebookingquicknote", EntityRole.Referenced, "lk_msdyn_bookableresourcebookingquicknote_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_bookableresourcebookingquicknote_modifiedonbehalfby = "lk_msdyn_bookableresourcebookingquicknote_modifiedonbehalfby";
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
            [Relationship("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b", EntityRole.Referenced, "lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_createdby", "createdby")]
            public const string lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_createdby = "lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_createdby";
            [Relationship("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b", EntityRole.Referenced, "lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_createdonbehalfby = "lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_createdonbehalfby";
            [Relationship("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b", EntityRole.Referenced, "lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_modifiedby", "modifiedby")]
            public const string lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_modifiedby = "lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_modifiedby";
            [Relationship("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b", EntityRole.Referenced, "lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_modifiedonbehalfby = "lk_msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b_modifiedonbehalfby";
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
            [Relationship("msdyn_businessclosure", EntityRole.Referenced, "lk_msdyn_businessclosure_createdby", "createdby")]
            public const string lk_msdyn_businessclosure_createdby = "lk_msdyn_businessclosure_createdby";
            [Relationship("msdyn_businessclosure", EntityRole.Referenced, "lk_msdyn_businessclosure_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_businessclosure_createdonbehalfby = "lk_msdyn_businessclosure_createdonbehalfby";
            [Relationship("msdyn_businessclosure", EntityRole.Referenced, "lk_msdyn_businessclosure_modifiedby", "modifiedby")]
            public const string lk_msdyn_businessclosure_modifiedby = "lk_msdyn_businessclosure_modifiedby";
            [Relationship("msdyn_businessclosure", EntityRole.Referenced, "lk_msdyn_businessclosure_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_businessclosure_modifiedonbehalfby = "lk_msdyn_businessclosure_modifiedonbehalfby";
            [Relationship("msdyn_callablecontext", EntityRole.Referenced, "lk_msdyn_callablecontext_createdby", "createdby")]
            public const string lk_msdyn_callablecontext_createdby = "lk_msdyn_callablecontext_createdby";
            [Relationship("msdyn_callablecontext", EntityRole.Referenced, "lk_msdyn_callablecontext_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_callablecontext_createdonbehalfby = "lk_msdyn_callablecontext_createdonbehalfby";
            [Relationship("msdyn_callablecontext", EntityRole.Referenced, "lk_msdyn_callablecontext_modifiedby", "modifiedby")]
            public const string lk_msdyn_callablecontext_modifiedby = "lk_msdyn_callablecontext_modifiedby";
            [Relationship("msdyn_callablecontext", EntityRole.Referenced, "lk_msdyn_callablecontext_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_callablecontext_modifiedonbehalfby = "lk_msdyn_callablecontext_modifiedonbehalfby";
            [Relationship("msdyn_caseenrichment", EntityRole.Referenced, "lk_msdyn_caseenrichment_createdby", "createdby")]
            public const string lk_msdyn_caseenrichment_createdby = "lk_msdyn_caseenrichment_createdby";
            [Relationship("msdyn_caseenrichment", EntityRole.Referenced, "lk_msdyn_caseenrichment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_caseenrichment_createdonbehalfby = "lk_msdyn_caseenrichment_createdonbehalfby";
            [Relationship("msdyn_caseenrichment", EntityRole.Referenced, "lk_msdyn_caseenrichment_modifiedby", "modifiedby")]
            public const string lk_msdyn_caseenrichment_modifiedby = "lk_msdyn_caseenrichment_modifiedby";
            [Relationship("msdyn_caseenrichment", EntityRole.Referenced, "lk_msdyn_caseenrichment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_caseenrichment_modifiedonbehalfby = "lk_msdyn_caseenrichment_modifiedonbehalfby";
            [Relationship("msdyn_casesuggestionrequestpayload", EntityRole.Referenced, "lk_msdyn_casesuggestionrequestpayload_createdby", "createdby")]
            public const string lk_msdyn_casesuggestionrequestpayload_createdby = "lk_msdyn_casesuggestionrequestpayload_createdby";
            [Relationship("msdyn_casesuggestionrequestpayload", EntityRole.Referenced, "lk_msdyn_casesuggestionrequestpayload_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_casesuggestionrequestpayload_createdonbehalfby = "lk_msdyn_casesuggestionrequestpayload_createdonbehalfby";
            [Relationship("msdyn_casesuggestionrequestpayload", EntityRole.Referenced, "lk_msdyn_casesuggestionrequestpayload_modifiedby", "modifiedby")]
            public const string lk_msdyn_casesuggestionrequestpayload_modifiedby = "lk_msdyn_casesuggestionrequestpayload_modifiedby";
            [Relationship("msdyn_casesuggestionrequestpayload", EntityRole.Referenced, "lk_msdyn_casesuggestionrequestpayload_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_casesuggestionrequestpayload_modifiedonbehalfby = "lk_msdyn_casesuggestionrequestpayload_modifiedonbehalfby";
            [Relationship("msdyn_casetopic", EntityRole.Referenced, "lk_msdyn_casetopic_createdby", "createdby")]
            public const string lk_msdyn_casetopic_createdby = "lk_msdyn_casetopic_createdby";
            [Relationship("msdyn_casetopic", EntityRole.Referenced, "lk_msdyn_casetopic_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_casetopic_createdonbehalfby = "lk_msdyn_casetopic_createdonbehalfby";
            [Relationship("msdyn_casetopic_incident", EntityRole.Referenced, "lk_msdyn_casetopic_incident_createdby", "createdby")]
            public const string lk_msdyn_casetopic_incident_createdby = "lk_msdyn_casetopic_incident_createdby";
            [Relationship("msdyn_casetopic_incident", EntityRole.Referenced, "lk_msdyn_casetopic_incident_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_casetopic_incident_createdonbehalfby = "lk_msdyn_casetopic_incident_createdonbehalfby";
            [Relationship("msdyn_casetopic_incident", EntityRole.Referenced, "lk_msdyn_casetopic_incident_modifiedby", "modifiedby")]
            public const string lk_msdyn_casetopic_incident_modifiedby = "lk_msdyn_casetopic_incident_modifiedby";
            [Relationship("msdyn_casetopic_incident", EntityRole.Referenced, "lk_msdyn_casetopic_incident_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_casetopic_incident_modifiedonbehalfby = "lk_msdyn_casetopic_incident_modifiedonbehalfby";
            [Relationship("msdyn_casetopic", EntityRole.Referenced, "lk_msdyn_casetopic_modifiedby", "modifiedby")]
            public const string lk_msdyn_casetopic_modifiedby = "lk_msdyn_casetopic_modifiedby";
            [Relationship("msdyn_casetopic", EntityRole.Referenced, "lk_msdyn_casetopic_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_casetopic_modifiedonbehalfby = "lk_msdyn_casetopic_modifiedonbehalfby";
            [Relationship("msdyn_casetopicsetting", EntityRole.Referenced, "lk_msdyn_casetopicsetting_createdby", "createdby")]
            public const string lk_msdyn_casetopicsetting_createdby = "lk_msdyn_casetopicsetting_createdby";
            [Relationship("msdyn_casetopicsetting", EntityRole.Referenced, "lk_msdyn_casetopicsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_casetopicsetting_createdonbehalfby = "lk_msdyn_casetopicsetting_createdonbehalfby";
            [Relationship("msdyn_casetopicsetting", EntityRole.Referenced, "lk_msdyn_casetopicsetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_casetopicsetting_modifiedby = "lk_msdyn_casetopicsetting_modifiedby";
            [Relationship("msdyn_casetopicsetting", EntityRole.Referenced, "lk_msdyn_casetopicsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_casetopicsetting_modifiedonbehalfby = "lk_msdyn_casetopicsetting_modifiedonbehalfby";
            [Relationship("msdyn_casetopicsummary", EntityRole.Referenced, "lk_msdyn_casetopicsummary_createdby", "createdby")]
            public const string lk_msdyn_casetopicsummary_createdby = "lk_msdyn_casetopicsummary_createdby";
            [Relationship("msdyn_casetopicsummary", EntityRole.Referenced, "lk_msdyn_casetopicsummary_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_casetopicsummary_createdonbehalfby = "lk_msdyn_casetopicsummary_createdonbehalfby";
            [Relationship("msdyn_casetopicsummary", EntityRole.Referenced, "lk_msdyn_casetopicsummary_modifiedby", "modifiedby")]
            public const string lk_msdyn_casetopicsummary_modifiedby = "lk_msdyn_casetopicsummary_modifiedby";
            [Relationship("msdyn_casetopicsummary", EntityRole.Referenced, "lk_msdyn_casetopicsummary_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_casetopicsummary_modifiedonbehalfby = "lk_msdyn_casetopicsummary_modifiedonbehalfby";
            [Relationship("msdyn_channelprovider", EntityRole.Referenced, "lk_msdyn_channelprovider_createdby", "createdby")]
            public const string lk_msdyn_channelprovider_createdby = "lk_msdyn_channelprovider_createdby";
            [Relationship("msdyn_channelprovider", EntityRole.Referenced, "lk_msdyn_channelprovider_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_channelprovider_createdonbehalfby = "lk_msdyn_channelprovider_createdonbehalfby";
            [Relationship("msdyn_channelprovider", EntityRole.Referenced, "lk_msdyn_channelprovider_modifiedby", "modifiedby")]
            public const string lk_msdyn_channelprovider_modifiedby = "lk_msdyn_channelprovider_modifiedby";
            [Relationship("msdyn_channelprovider", EntityRole.Referenced, "lk_msdyn_channelprovider_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_channelprovider_modifiedonbehalfby = "lk_msdyn_channelprovider_modifiedonbehalfby";
            [Relationship("msdyn_clientextension", EntityRole.Referenced, "lk_msdyn_clientextension_createdby", "createdby")]
            public const string lk_msdyn_clientextension_createdby = "lk_msdyn_clientextension_createdby";
            [Relationship("msdyn_clientextension", EntityRole.Referenced, "lk_msdyn_clientextension_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_clientextension_createdonbehalfby = "lk_msdyn_clientextension_createdonbehalfby";
            [Relationship("msdyn_clientextension", EntityRole.Referenced, "lk_msdyn_clientextension_modifiedby", "modifiedby")]
            public const string lk_msdyn_clientextension_modifiedby = "lk_msdyn_clientextension_modifiedby";
            [Relationship("msdyn_clientextension", EntityRole.Referenced, "lk_msdyn_clientextension_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_clientextension_modifiedonbehalfby = "lk_msdyn_clientextension_modifiedonbehalfby";
            [Relationship("msdyn_collabgraphresource", EntityRole.Referenced, "lk_msdyn_collabgraphresource_createdby", "createdby")]
            public const string lk_msdyn_collabgraphresource_createdby = "lk_msdyn_collabgraphresource_createdby";
            [Relationship("msdyn_collabgraphresource", EntityRole.Referenced, "lk_msdyn_collabgraphresource_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_collabgraphresource_createdonbehalfby = "lk_msdyn_collabgraphresource_createdonbehalfby";
            [Relationship("msdyn_collabgraphresource", EntityRole.Referenced, "lk_msdyn_collabgraphresource_modifiedby", "modifiedby")]
            public const string lk_msdyn_collabgraphresource_modifiedby = "lk_msdyn_collabgraphresource_modifiedby";
            [Relationship("msdyn_collabgraphresource", EntityRole.Referenced, "lk_msdyn_collabgraphresource_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_collabgraphresource_modifiedonbehalfby = "lk_msdyn_collabgraphresource_modifiedonbehalfby";
            [Relationship("msdyn_configuration", EntityRole.Referenced, "lk_msdyn_configuration_createdby", "createdby")]
            public const string lk_msdyn_configuration_createdby = "lk_msdyn_configuration_createdby";
            [Relationship("msdyn_configuration", EntityRole.Referenced, "lk_msdyn_configuration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_configuration_createdonbehalfby = "lk_msdyn_configuration_createdonbehalfby";
            [Relationship("msdyn_configuration", EntityRole.Referenced, "lk_msdyn_configuration_modifiedby", "modifiedby")]
            public const string lk_msdyn_configuration_modifiedby = "lk_msdyn_configuration_modifiedby";
            [Relationship("msdyn_configuration", EntityRole.Referenced, "lk_msdyn_configuration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_configuration_modifiedonbehalfby = "lk_msdyn_configuration_modifiedonbehalfby";
            [Relationship("msdyn_contactkpiitem", EntityRole.Referenced, "lk_msdyn_contactkpiitem_createdby", "createdby")]
            public const string lk_msdyn_contactkpiitem_createdby = "lk_msdyn_contactkpiitem_createdby";
            [Relationship("msdyn_contactkpiitem", EntityRole.Referenced, "lk_msdyn_contactkpiitem_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_contactkpiitem_createdonbehalfby = "lk_msdyn_contactkpiitem_createdonbehalfby";
            [Relationship("msdyn_contactkpiitem", EntityRole.Referenced, "lk_msdyn_contactkpiitem_modifiedby", "modifiedby")]
            public const string lk_msdyn_contactkpiitem_modifiedby = "lk_msdyn_contactkpiitem_modifiedby";
            [Relationship("msdyn_contactkpiitem", EntityRole.Referenced, "lk_msdyn_contactkpiitem_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_contactkpiitem_modifiedonbehalfby = "lk_msdyn_contactkpiitem_modifiedonbehalfby";
            [Relationship("msdyn_contactsuggestionrule", EntityRole.Referenced, "lk_msdyn_contactsuggestionrule_createdby", "createdby")]
            public const string lk_msdyn_contactsuggestionrule_createdby = "lk_msdyn_contactsuggestionrule_createdby";
            [Relationship("msdyn_contactsuggestionrule", EntityRole.Referenced, "lk_msdyn_contactsuggestionrule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_contactsuggestionrule_createdonbehalfby = "lk_msdyn_contactsuggestionrule_createdonbehalfby";
            [Relationship("msdyn_contactsuggestionrule", EntityRole.Referenced, "lk_msdyn_contactsuggestionrule_modifiedby", "modifiedby")]
            public const string lk_msdyn_contactsuggestionrule_modifiedby = "lk_msdyn_contactsuggestionrule_modifiedby";
            [Relationship("msdyn_contactsuggestionrule", EntityRole.Referenced, "lk_msdyn_contactsuggestionrule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_contactsuggestionrule_modifiedonbehalfby = "lk_msdyn_contactsuggestionrule_modifiedonbehalfby";
            [Relationship("msdyn_contactsuggestionruleset", EntityRole.Referenced, "lk_msdyn_contactsuggestionruleset_createdby", "createdby")]
            public const string lk_msdyn_contactsuggestionruleset_createdby = "lk_msdyn_contactsuggestionruleset_createdby";
            [Relationship("msdyn_contactsuggestionruleset", EntityRole.Referenced, "lk_msdyn_contactsuggestionruleset_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_contactsuggestionruleset_createdonbehalfby = "lk_msdyn_contactsuggestionruleset_createdonbehalfby";
            [Relationship("msdyn_contactsuggestionruleset", EntityRole.Referenced, "lk_msdyn_contactsuggestionruleset_modifiedby", "modifiedby")]
            public const string lk_msdyn_contactsuggestionruleset_modifiedby = "lk_msdyn_contactsuggestionruleset_modifiedby";
            [Relationship("msdyn_contactsuggestionruleset", EntityRole.Referenced, "lk_msdyn_contactsuggestionruleset_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_contactsuggestionruleset_modifiedonbehalfby = "lk_msdyn_contactsuggestionruleset_modifiedonbehalfby";
            [Relationship("msdyn_conversationdata", EntityRole.Referenced, "lk_msdyn_conversationdata_createdby", "createdby")]
            public const string lk_msdyn_conversationdata_createdby = "lk_msdyn_conversationdata_createdby";
            [Relationship("msdyn_conversationdata", EntityRole.Referenced, "lk_msdyn_conversationdata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_conversationdata_createdonbehalfby = "lk_msdyn_conversationdata_createdonbehalfby";
            [Relationship("msdyn_conversationdata", EntityRole.Referenced, "lk_msdyn_conversationdata_modifiedby", "modifiedby")]
            public const string lk_msdyn_conversationdata_modifiedby = "lk_msdyn_conversationdata_modifiedby";
            [Relationship("msdyn_conversationdata", EntityRole.Referenced, "lk_msdyn_conversationdata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_conversationdata_modifiedonbehalfby = "lk_msdyn_conversationdata_modifiedonbehalfby";
            [Relationship("msdyn_customerasset", EntityRole.Referenced, "lk_msdyn_customerasset_createdby", "createdby")]
            public const string lk_msdyn_customerasset_createdby = "lk_msdyn_customerasset_createdby";
            [Relationship("msdyn_customerasset", EntityRole.Referenced, "lk_msdyn_customerasset_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_customerasset_createdonbehalfby = "lk_msdyn_customerasset_createdonbehalfby";
            [Relationship("msdyn_customerasset", EntityRole.Referenced, "lk_msdyn_customerasset_modifiedby", "modifiedby")]
            public const string lk_msdyn_customerasset_modifiedby = "lk_msdyn_customerasset_modifiedby";
            [Relationship("msdyn_customerasset", EntityRole.Referenced, "lk_msdyn_customerasset_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_customerasset_modifiedonbehalfby = "lk_msdyn_customerasset_modifiedonbehalfby";
            [Relationship("msdyn_customerassetattachment", EntityRole.Referenced, "lk_msdyn_customerassetattachment_createdby", "createdby")]
            public const string lk_msdyn_customerassetattachment_createdby = "lk_msdyn_customerassetattachment_createdby";
            [Relationship("msdyn_customerassetattachment", EntityRole.Referenced, "lk_msdyn_customerassetattachment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_customerassetattachment_createdonbehalfby = "lk_msdyn_customerassetattachment_createdonbehalfby";
            [Relationship("msdyn_customerassetattachment", EntityRole.Referenced, "lk_msdyn_customerassetattachment_modifiedby", "modifiedby")]
            public const string lk_msdyn_customerassetattachment_modifiedby = "lk_msdyn_customerassetattachment_modifiedby";
            [Relationship("msdyn_customerassetattachment", EntityRole.Referenced, "lk_msdyn_customerassetattachment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_customerassetattachment_modifiedonbehalfby = "lk_msdyn_customerassetattachment_modifiedonbehalfby";
            [Relationship("msdyn_customerassetcategory", EntityRole.Referenced, "lk_msdyn_customerassetcategory_createdby", "createdby")]
            public const string lk_msdyn_customerassetcategory_createdby = "lk_msdyn_customerassetcategory_createdby";
            [Relationship("msdyn_customerassetcategory", EntityRole.Referenced, "lk_msdyn_customerassetcategory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_customerassetcategory_createdonbehalfby = "lk_msdyn_customerassetcategory_createdonbehalfby";
            [Relationship("msdyn_customerassetcategory", EntityRole.Referenced, "lk_msdyn_customerassetcategory_modifiedby", "modifiedby")]
            public const string lk_msdyn_customerassetcategory_modifiedby = "lk_msdyn_customerassetcategory_modifiedby";
            [Relationship("msdyn_customerassetcategory", EntityRole.Referenced, "lk_msdyn_customerassetcategory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_customerassetcategory_modifiedonbehalfby = "lk_msdyn_customerassetcategory_modifiedonbehalfby";
            [Relationship("msdyn_dataanalyticsreport", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_createdby", "createdby")]
            public const string lk_msdyn_dataanalyticsreport_createdby = "lk_msdyn_dataanalyticsreport_createdby";
            [Relationship("msdyn_dataanalyticsreport", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_createdonbehalfby = "lk_msdyn_dataanalyticsreport_createdonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_csrmanager", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_csrmanager_createdby", "createdby")]
            public const string lk_msdyn_dataanalyticsreport_csrmanager_createdby = "lk_msdyn_dataanalyticsreport_csrmanager_createdby";
            [Relationship("msdyn_dataanalyticsreport_csrmanager", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_csrmanager_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_csrmanager_createdonbehalfby = "lk_msdyn_dataanalyticsreport_csrmanager_createdonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_csrmanager", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_csrmanager_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataanalyticsreport_csrmanager_modifiedby = "lk_msdyn_dataanalyticsreport_csrmanager_modifiedby";
            [Relationship("msdyn_dataanalyticsreport_csrmanager", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_csrmanager_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_csrmanager_modifiedonbehalfby = "lk_msdyn_dataanalyticsreport_csrmanager_modifiedonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_fs", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fs_createdby", "createdby")]
            public const string lk_msdyn_dataanalyticsreport_fs_createdby = "lk_msdyn_dataanalyticsreport_fs_createdby";
            [Relationship("msdyn_dataanalyticsreport_fs", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fs_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_fs_createdonbehalfby = "lk_msdyn_dataanalyticsreport_fs_createdonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_fs", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fs_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataanalyticsreport_fs_modifiedby = "lk_msdyn_dataanalyticsreport_fs_modifiedby";
            [Relationship("msdyn_dataanalyticsreport_fs", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fs_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_fs_modifiedonbehalfby = "lk_msdyn_dataanalyticsreport_fs_modifiedonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_fspredictrs", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fspredictrs_createdby", "createdby")]
            public const string lk_msdyn_dataanalyticsreport_fspredictrs_createdby = "lk_msdyn_dataanalyticsreport_fspredictrs_createdby";
            [Relationship("msdyn_dataanalyticsreport_fspredictrs", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fspredictrs_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_fspredictrs_createdonbehalfby = "lk_msdyn_dataanalyticsreport_fspredictrs_createdonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_fspredictrs", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fspredictrs_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataanalyticsreport_fspredictrs_modifiedby = "lk_msdyn_dataanalyticsreport_fspredictrs_modifiedby";
            [Relationship("msdyn_dataanalyticsreport_fspredictrs", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fspredictrs_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_fspredictrs_modifiedonbehalfby = "lk_msdyn_dataanalyticsreport_fspredictrs_modifiedonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_fspredictwhd", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fspredictwhd_createdby", "createdby")]
            public const string lk_msdyn_dataanalyticsreport_fspredictwhd_createdby = "lk_msdyn_dataanalyticsreport_fspredictwhd_createdby";
            [Relationship("msdyn_dataanalyticsreport_fspredictwhd", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fspredictwhd_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_fspredictwhd_createdonbehalfby = "lk_msdyn_dataanalyticsreport_fspredictwhd_createdonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_fspredictwhd", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fspredictwhd_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataanalyticsreport_fspredictwhd_modifiedby = "lk_msdyn_dataanalyticsreport_fspredictwhd_modifiedby";
            [Relationship("msdyn_dataanalyticsreport_fspredictwhd", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_fspredictwhd_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_fspredictwhd_modifiedonbehalfby = "lk_msdyn_dataanalyticsreport_fspredictwhd_modifiedonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_ksinsights", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_ksinsights_createdby", "createdby")]
            public const string lk_msdyn_dataanalyticsreport_ksinsights_createdby = "lk_msdyn_dataanalyticsreport_ksinsights_createdby";
            [Relationship("msdyn_dataanalyticsreport_ksinsights", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_ksinsights_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_ksinsights_createdonbehalfby = "lk_msdyn_dataanalyticsreport_ksinsights_createdonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_ksinsights", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_ksinsights_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataanalyticsreport_ksinsights_modifiedby = "lk_msdyn_dataanalyticsreport_ksinsights_modifiedby";
            [Relationship("msdyn_dataanalyticsreport_ksinsights", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_ksinsights_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_ksinsights_modifiedonbehalfby = "lk_msdyn_dataanalyticsreport_ksinsights_modifiedonbehalfby";
            [Relationship("msdyn_dataanalyticsreport", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataanalyticsreport_modifiedby = "lk_msdyn_dataanalyticsreport_modifiedby";
            [Relationship("msdyn_dataanalyticsreport", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_modifiedonbehalfby = "lk_msdyn_dataanalyticsreport_modifiedonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_sareporting", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_sareporting_createdby", "createdby")]
            public const string lk_msdyn_dataanalyticsreport_sareporting_createdby = "lk_msdyn_dataanalyticsreport_sareporting_createdby";
            [Relationship("msdyn_dataanalyticsreport_sareporting", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_sareporting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_sareporting_createdonbehalfby = "lk_msdyn_dataanalyticsreport_sareporting_createdonbehalfby";
            [Relationship("msdyn_dataanalyticsreport_sareporting", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_sareporting_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataanalyticsreport_sareporting_modifiedby = "lk_msdyn_dataanalyticsreport_sareporting_modifiedby";
            [Relationship("msdyn_dataanalyticsreport_sareporting", EntityRole.Referenced, "lk_msdyn_dataanalyticsreport_sareporting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataanalyticsreport_sareporting_modifiedonbehalfby = "lk_msdyn_dataanalyticsreport_sareporting_modifiedonbehalfby";
            [Relationship("msdyn_databaseversion", EntityRole.Referenced, "lk_msdyn_databaseversion_createdby", "createdby")]
            public const string lk_msdyn_databaseversion_createdby = "lk_msdyn_databaseversion_createdby";
            [Relationship("msdyn_databaseversion", EntityRole.Referenced, "lk_msdyn_databaseversion_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_databaseversion_createdonbehalfby = "lk_msdyn_databaseversion_createdonbehalfby";
            [Relationship("msdyn_databaseversion", EntityRole.Referenced, "lk_msdyn_databaseversion_modifiedby", "modifiedby")]
            public const string lk_msdyn_databaseversion_modifiedby = "lk_msdyn_databaseversion_modifiedby";
            [Relationship("msdyn_databaseversion", EntityRole.Referenced, "lk_msdyn_databaseversion_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_databaseversion_modifiedonbehalfby = "lk_msdyn_databaseversion_modifiedonbehalfby";
            [Relationship("msdyn_dataflow", EntityRole.Referenced, "lk_msdyn_dataflow_createdby", "createdby")]
            public const string lk_msdyn_dataflow_createdby = "lk_msdyn_dataflow_createdby";
            [Relationship("msdyn_dataflow", EntityRole.Referenced, "lk_msdyn_dataflow_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataflow_createdonbehalfby = "lk_msdyn_dataflow_createdonbehalfby";
            [Relationship("msdyn_dataflow", EntityRole.Referenced, "lk_msdyn_dataflow_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataflow_modifiedby = "lk_msdyn_dataflow_modifiedby";
            [Relationship("msdyn_dataflow", EntityRole.Referenced, "lk_msdyn_dataflow_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataflow_modifiedonbehalfby = "lk_msdyn_dataflow_modifiedonbehalfby";
            [Relationship("msdyn_dataflowrefreshhistory", EntityRole.Referenced, "lk_msdyn_dataflowrefreshhistory_createdby", "createdby")]
            public const string lk_msdyn_dataflowrefreshhistory_createdby = "lk_msdyn_dataflowrefreshhistory_createdby";
            [Relationship("msdyn_dataflowrefreshhistory", EntityRole.Referenced, "lk_msdyn_dataflowrefreshhistory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dataflowrefreshhistory_createdonbehalfby = "lk_msdyn_dataflowrefreshhistory_createdonbehalfby";
            [Relationship("msdyn_dataflowrefreshhistory", EntityRole.Referenced, "lk_msdyn_dataflowrefreshhistory_modifiedby", "modifiedby")]
            public const string lk_msdyn_dataflowrefreshhistory_modifiedby = "lk_msdyn_dataflowrefreshhistory_modifiedby";
            [Relationship("msdyn_dataflowrefreshhistory", EntityRole.Referenced, "lk_msdyn_dataflowrefreshhistory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dataflowrefreshhistory_modifiedonbehalfby = "lk_msdyn_dataflowrefreshhistory_modifiedonbehalfby";
            [Relationship("msdyn_datainsightsandanalyticsfeature", EntityRole.Referenced, "lk_msdyn_datainsightsandanalyticsfeature_createdby", "createdby")]
            public const string lk_msdyn_datainsightsandanalyticsfeature_createdby = "lk_msdyn_datainsightsandanalyticsfeature_createdby";
            [Relationship("msdyn_datainsightsandanalyticsfeature", EntityRole.Referenced, "lk_msdyn_datainsightsandanalyticsfeature_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_datainsightsandanalyticsfeature_createdonbehalfby = "lk_msdyn_datainsightsandanalyticsfeature_createdonbehalfby";
            [Relationship("msdyn_datainsightsandanalyticsfeature", EntityRole.Referenced, "lk_msdyn_datainsightsandanalyticsfeature_modifiedby", "modifiedby")]
            public const string lk_msdyn_datainsightsandanalyticsfeature_modifiedby = "lk_msdyn_datainsightsandanalyticsfeature_modifiedby";
            [Relationship("msdyn_datainsightsandanalyticsfeature", EntityRole.Referenced, "lk_msdyn_datainsightsandanalyticsfeature_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_datainsightsandanalyticsfeature_modifiedonbehalfby = "lk_msdyn_datainsightsandanalyticsfeature_modifiedonbehalfby";
            [Relationship("msdyn_dealmanageraccess", EntityRole.Referenced, "lk_msdyn_dealmanageraccess_createdby", "createdby")]
            public const string lk_msdyn_dealmanageraccess_createdby = "lk_msdyn_dealmanageraccess_createdby";
            [Relationship("msdyn_dealmanageraccess", EntityRole.Referenced, "lk_msdyn_dealmanageraccess_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dealmanageraccess_createdonbehalfby = "lk_msdyn_dealmanageraccess_createdonbehalfby";
            [Relationship("msdyn_dealmanageraccess", EntityRole.Referenced, "lk_msdyn_dealmanageraccess_modifiedby", "modifiedby")]
            public const string lk_msdyn_dealmanageraccess_modifiedby = "lk_msdyn_dealmanageraccess_modifiedby";
            [Relationship("msdyn_dealmanageraccess", EntityRole.Referenced, "lk_msdyn_dealmanageraccess_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dealmanageraccess_modifiedonbehalfby = "lk_msdyn_dealmanageraccess_modifiedonbehalfby";
            [Relationship("msdyn_dealmanagersettings", EntityRole.Referenced, "lk_msdyn_dealmanagersettings_createdby", "createdby")]
            public const string lk_msdyn_dealmanagersettings_createdby = "lk_msdyn_dealmanagersettings_createdby";
            [Relationship("msdyn_dealmanagersettings", EntityRole.Referenced, "lk_msdyn_dealmanagersettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_dealmanagersettings_createdonbehalfby = "lk_msdyn_dealmanagersettings_createdonbehalfby";
            [Relationship("msdyn_dealmanagersettings", EntityRole.Referenced, "lk_msdyn_dealmanagersettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_dealmanagersettings_modifiedby = "lk_msdyn_dealmanagersettings_modifiedby";
            [Relationship("msdyn_dealmanagersettings", EntityRole.Referenced, "lk_msdyn_dealmanagersettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_dealmanagersettings_modifiedonbehalfby = "lk_msdyn_dealmanagersettings_modifiedonbehalfby";
            [Relationship("msdyn_duplicatedetectionpluginrun", EntityRole.Referenced, "lk_msdyn_duplicatedetectionpluginrun_createdby", "createdby")]
            public const string lk_msdyn_duplicatedetectionpluginrun_createdby = "lk_msdyn_duplicatedetectionpluginrun_createdby";
            [Relationship("msdyn_duplicatedetectionpluginrun", EntityRole.Referenced, "lk_msdyn_duplicatedetectionpluginrun_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_duplicatedetectionpluginrun_createdonbehalfby = "lk_msdyn_duplicatedetectionpluginrun_createdonbehalfby";
            [Relationship("msdyn_duplicatedetectionpluginrun", EntityRole.Referenced, "lk_msdyn_duplicatedetectionpluginrun_modifiedby", "modifiedby")]
            public const string lk_msdyn_duplicatedetectionpluginrun_modifiedby = "lk_msdyn_duplicatedetectionpluginrun_modifiedby";
            [Relationship("msdyn_duplicatedetectionpluginrun", EntityRole.Referenced, "lk_msdyn_duplicatedetectionpluginrun_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_duplicatedetectionpluginrun_modifiedonbehalfby = "lk_msdyn_duplicatedetectionpluginrun_modifiedonbehalfby";
            [Relationship("msdyn_duplicateleadmapping", EntityRole.Referenced, "lk_msdyn_duplicateleadmapping_createdby", "createdby")]
            public const string lk_msdyn_duplicateleadmapping_createdby = "lk_msdyn_duplicateleadmapping_createdby";
            [Relationship("msdyn_duplicateleadmapping", EntityRole.Referenced, "lk_msdyn_duplicateleadmapping_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_duplicateleadmapping_createdonbehalfby = "lk_msdyn_duplicateleadmapping_createdonbehalfby";
            [Relationship("msdyn_duplicateleadmapping", EntityRole.Referenced, "lk_msdyn_duplicateleadmapping_modifiedby", "modifiedby")]
            public const string lk_msdyn_duplicateleadmapping_modifiedby = "lk_msdyn_duplicateleadmapping_modifiedby";
            [Relationship("msdyn_duplicateleadmapping", EntityRole.Referenced, "lk_msdyn_duplicateleadmapping_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_duplicateleadmapping_modifiedonbehalfby = "lk_msdyn_duplicateleadmapping_modifiedonbehalfby";
            [Relationship("msdyn_entitlementapplication", EntityRole.Referenced, "lk_msdyn_entitlementapplication_createdby", "createdby")]
            public const string lk_msdyn_entitlementapplication_createdby = "lk_msdyn_entitlementapplication_createdby";
            [Relationship("msdyn_entitlementapplication", EntityRole.Referenced, "lk_msdyn_entitlementapplication_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_entitlementapplication_createdonbehalfby = "lk_msdyn_entitlementapplication_createdonbehalfby";
            [Relationship("msdyn_entitlementapplication", EntityRole.Referenced, "lk_msdyn_entitlementapplication_modifiedby", "modifiedby")]
            public const string lk_msdyn_entitlementapplication_modifiedby = "lk_msdyn_entitlementapplication_modifiedby";
            [Relationship("msdyn_entitlementapplication", EntityRole.Referenced, "lk_msdyn_entitlementapplication_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_entitlementapplication_modifiedonbehalfby = "lk_msdyn_entitlementapplication_modifiedonbehalfby";
            [Relationship("msdyn_entityconfiguration", EntityRole.Referenced, "lk_msdyn_entityconfiguration_createdby", "createdby")]
            public const string lk_msdyn_entityconfiguration_createdby = "lk_msdyn_entityconfiguration_createdby";
            [Relationship("msdyn_entityconfiguration", EntityRole.Referenced, "lk_msdyn_entityconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_entityconfiguration_createdonbehalfby = "lk_msdyn_entityconfiguration_createdonbehalfby";
            [Relationship("msdyn_entityconfiguration", EntityRole.Referenced, "lk_msdyn_entityconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_entityconfiguration_modifiedby = "lk_msdyn_entityconfiguration_modifiedby";
            [Relationship("msdyn_entityconfiguration", EntityRole.Referenced, "lk_msdyn_entityconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_entityconfiguration_modifiedonbehalfby = "lk_msdyn_entityconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_entitylinkchatconfiguration", EntityRole.Referenced, "lk_msdyn_entitylinkchatconfiguration_createdby", "createdby")]
            public const string lk_msdyn_entitylinkchatconfiguration_createdby = "lk_msdyn_entitylinkchatconfiguration_createdby";
            [Relationship("msdyn_entitylinkchatconfiguration", EntityRole.Referenced, "lk_msdyn_entitylinkchatconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_entitylinkchatconfiguration_createdonbehalfby = "lk_msdyn_entitylinkchatconfiguration_createdonbehalfby";
            [Relationship("msdyn_entitylinkchatconfiguration", EntityRole.Referenced, "lk_msdyn_entitylinkchatconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_entitylinkchatconfiguration_modifiedby = "lk_msdyn_entitylinkchatconfiguration_modifiedby";
            [Relationship("msdyn_entitylinkchatconfiguration", EntityRole.Referenced, "lk_msdyn_entitylinkchatconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_entitylinkchatconfiguration_modifiedonbehalfby = "lk_msdyn_entitylinkchatconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_entityrankingrule", EntityRole.Referenced, "lk_msdyn_entityrankingrule_createdby", "createdby")]
            public const string lk_msdyn_entityrankingrule_createdby = "lk_msdyn_entityrankingrule_createdby";
            [Relationship("msdyn_entityrankingrule", EntityRole.Referenced, "lk_msdyn_entityrankingrule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_entityrankingrule_createdonbehalfby = "lk_msdyn_entityrankingrule_createdonbehalfby";
            [Relationship("msdyn_entityrankingrule", EntityRole.Referenced, "lk_msdyn_entityrankingrule_modifiedby", "modifiedby")]
            public const string lk_msdyn_entityrankingrule_modifiedby = "lk_msdyn_entityrankingrule_modifiedby";
            [Relationship("msdyn_entityrankingrule", EntityRole.Referenced, "lk_msdyn_entityrankingrule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_entityrankingrule_modifiedonbehalfby = "lk_msdyn_entityrankingrule_modifiedonbehalfby";
            [Relationship("msdyn_entityrefreshhistory", EntityRole.Referenced, "lk_msdyn_entityrefreshhistory_createdby", "createdby")]
            public const string lk_msdyn_entityrefreshhistory_createdby = "lk_msdyn_entityrefreshhistory_createdby";
            [Relationship("msdyn_entityrefreshhistory", EntityRole.Referenced, "lk_msdyn_entityrefreshhistory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_entityrefreshhistory_createdonbehalfby = "lk_msdyn_entityrefreshhistory_createdonbehalfby";
            [Relationship("msdyn_entityrefreshhistory", EntityRole.Referenced, "lk_msdyn_entityrefreshhistory_modifiedby", "modifiedby")]
            public const string lk_msdyn_entityrefreshhistory_modifiedby = "lk_msdyn_entityrefreshhistory_modifiedby";
            [Relationship("msdyn_entityrefreshhistory", EntityRole.Referenced, "lk_msdyn_entityrefreshhistory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_entityrefreshhistory_modifiedonbehalfby = "lk_msdyn_entityrefreshhistory_modifiedonbehalfby";
            [Relationship("msdyn_extendedusersetting", EntityRole.Referenced, "lk_msdyn_extendedusersetting_createdby", "createdby")]
            public const string lk_msdyn_extendedusersetting_createdby = "lk_msdyn_extendedusersetting_createdby";
            [Relationship("msdyn_extendedusersetting", EntityRole.Referenced, "lk_msdyn_extendedusersetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_extendedusersetting_createdonbehalfby = "lk_msdyn_extendedusersetting_createdonbehalfby";
            [Relationship("msdyn_extendedusersetting", EntityRole.Referenced, "lk_msdyn_extendedusersetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_extendedusersetting_modifiedby = "lk_msdyn_extendedusersetting_modifiedby";
            [Relationship("msdyn_extendedusersetting", EntityRole.Referenced, "lk_msdyn_extendedusersetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_extendedusersetting_modifiedonbehalfby = "lk_msdyn_extendedusersetting_modifiedonbehalfby";
            [Relationship("msdyn_federatedarticle", EntityRole.Referenced, "lk_msdyn_federatedarticle_createdby", "createdby")]
            public const string lk_msdyn_federatedarticle_createdby = "lk_msdyn_federatedarticle_createdby";
            [Relationship("msdyn_federatedarticle", EntityRole.Referenced, "lk_msdyn_federatedarticle_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_federatedarticle_createdonbehalfby = "lk_msdyn_federatedarticle_createdonbehalfby";
            [Relationship("msdyn_federatedarticle", EntityRole.Referenced, "lk_msdyn_federatedarticle_modifiedby", "modifiedby")]
            public const string lk_msdyn_federatedarticle_modifiedby = "lk_msdyn_federatedarticle_modifiedby";
            [Relationship("msdyn_federatedarticle", EntityRole.Referenced, "lk_msdyn_federatedarticle_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_federatedarticle_modifiedonbehalfby = "lk_msdyn_federatedarticle_modifiedonbehalfby";
            [Relationship("msdyn_federatedarticleincident", EntityRole.Referenced, "lk_msdyn_federatedarticleincident_createdby", "createdby")]
            public const string lk_msdyn_federatedarticleincident_createdby = "lk_msdyn_federatedarticleincident_createdby";
            [Relationship("msdyn_federatedarticleincident", EntityRole.Referenced, "lk_msdyn_federatedarticleincident_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_federatedarticleincident_createdonbehalfby = "lk_msdyn_federatedarticleincident_createdonbehalfby";
            [Relationship("msdyn_federatedarticleincident", EntityRole.Referenced, "lk_msdyn_federatedarticleincident_modifiedby", "modifiedby")]
            public const string lk_msdyn_federatedarticleincident_modifiedby = "lk_msdyn_federatedarticleincident_modifiedby";
            [Relationship("msdyn_federatedarticleincident", EntityRole.Referenced, "lk_msdyn_federatedarticleincident_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_federatedarticleincident_modifiedonbehalfby = "lk_msdyn_federatedarticleincident_modifiedonbehalfby";
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
            [Relationship("msdyn_fieldserviceslaconfiguration", EntityRole.Referenced, "lk_msdyn_fieldserviceslaconfiguration_createdby", "createdby")]
            public const string lk_msdyn_fieldserviceslaconfiguration_createdby = "lk_msdyn_fieldserviceslaconfiguration_createdby";
            [Relationship("msdyn_fieldserviceslaconfiguration", EntityRole.Referenced, "lk_msdyn_fieldserviceslaconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_fieldserviceslaconfiguration_createdonbehalfby = "lk_msdyn_fieldserviceslaconfiguration_createdonbehalfby";
            [Relationship("msdyn_fieldserviceslaconfiguration", EntityRole.Referenced, "lk_msdyn_fieldserviceslaconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_fieldserviceslaconfiguration_modifiedby = "lk_msdyn_fieldserviceslaconfiguration_modifiedby";
            [Relationship("msdyn_fieldserviceslaconfiguration", EntityRole.Referenced, "lk_msdyn_fieldserviceslaconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_fieldserviceslaconfiguration_modifiedonbehalfby = "lk_msdyn_fieldserviceslaconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "lk_msdyn_fieldservicesystemjob_createdby", "createdby")]
            public const string lk_msdyn_fieldservicesystemjob_createdby = "lk_msdyn_fieldservicesystemjob_createdby";
            [Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "lk_msdyn_fieldservicesystemjob_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_fieldservicesystemjob_createdonbehalfby = "lk_msdyn_fieldservicesystemjob_createdonbehalfby";
            [Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "lk_msdyn_fieldservicesystemjob_modifiedby", "modifiedby")]
            public const string lk_msdyn_fieldservicesystemjob_modifiedby = "lk_msdyn_fieldservicesystemjob_modifiedby";
            [Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "lk_msdyn_fieldservicesystemjob_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_fieldservicesystemjob_modifiedonbehalfby = "lk_msdyn_fieldservicesystemjob_modifiedonbehalfby";
            [Relationship("msdyn_flowcardtype", EntityRole.Referenced, "lk_msdyn_flowcardtype_createdby", "createdby")]
            public const string lk_msdyn_flowcardtype_createdby = "lk_msdyn_flowcardtype_createdby";
            [Relationship("msdyn_flowcardtype", EntityRole.Referenced, "lk_msdyn_flowcardtype_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_flowcardtype_createdonbehalfby = "lk_msdyn_flowcardtype_createdonbehalfby";
            [Relationship("msdyn_flowcardtype", EntityRole.Referenced, "lk_msdyn_flowcardtype_modifiedby", "modifiedby")]
            public const string lk_msdyn_flowcardtype_modifiedby = "lk_msdyn_flowcardtype_modifiedby";
            [Relationship("msdyn_flowcardtype", EntityRole.Referenced, "lk_msdyn_flowcardtype_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_flowcardtype_modifiedonbehalfby = "lk_msdyn_flowcardtype_modifiedonbehalfby";
            [Relationship("msdyn_forecastconfiguration", EntityRole.Referenced, "lk_msdyn_forecastconfiguration_createdby", "createdby")]
            public const string lk_msdyn_forecastconfiguration_createdby = "lk_msdyn_forecastconfiguration_createdby";
            [Relationship("msdyn_forecastconfiguration", EntityRole.Referenced, "lk_msdyn_forecastconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_forecastconfiguration_createdonbehalfby = "lk_msdyn_forecastconfiguration_createdonbehalfby";
            [Relationship("msdyn_forecastconfiguration", EntityRole.Referenced, "lk_msdyn_forecastconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_forecastconfiguration_modifiedby = "lk_msdyn_forecastconfiguration_modifiedby";
            [Relationship("msdyn_forecastconfiguration", EntityRole.Referenced, "lk_msdyn_forecastconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_forecastconfiguration_modifiedonbehalfby = "lk_msdyn_forecastconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_forecastdefinition", EntityRole.Referenced, "lk_msdyn_forecastdefinition_createdby", "createdby")]
            public const string lk_msdyn_forecastdefinition_createdby = "lk_msdyn_forecastdefinition_createdby";
            [Relationship("msdyn_forecastdefinition", EntityRole.Referenced, "lk_msdyn_forecastdefinition_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_forecastdefinition_createdonbehalfby = "lk_msdyn_forecastdefinition_createdonbehalfby";
            [Relationship("msdyn_forecastdefinition", EntityRole.Referenced, "lk_msdyn_forecastdefinition_modifiedby", "modifiedby")]
            public const string lk_msdyn_forecastdefinition_modifiedby = "lk_msdyn_forecastdefinition_modifiedby";
            [Relationship("msdyn_forecastdefinition", EntityRole.Referenced, "lk_msdyn_forecastdefinition_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_forecastdefinition_modifiedonbehalfby = "lk_msdyn_forecastdefinition_modifiedonbehalfby";
            [Relationship("msdyn_forecastinstance", EntityRole.Referenced, "lk_msdyn_forecastinstance_createdby", "createdby")]
            public const string lk_msdyn_forecastinstance_createdby = "lk_msdyn_forecastinstance_createdby";
            [Relationship("msdyn_forecastinstance", EntityRole.Referenced, "lk_msdyn_forecastinstance_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_forecastinstance_createdonbehalfby = "lk_msdyn_forecastinstance_createdonbehalfby";
            [Relationship("msdyn_forecastinstance", EntityRole.Referenced, "lk_msdyn_forecastinstance_modifiedby", "modifiedby")]
            public const string lk_msdyn_forecastinstance_modifiedby = "lk_msdyn_forecastinstance_modifiedby";
            [Relationship("msdyn_forecastinstance", EntityRole.Referenced, "lk_msdyn_forecastinstance_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_forecastinstance_modifiedonbehalfby = "lk_msdyn_forecastinstance_modifiedonbehalfby";
            [Relationship("msdyn_forecastrecurrence", EntityRole.Referenced, "lk_msdyn_forecastrecurrence_createdby", "createdby")]
            public const string lk_msdyn_forecastrecurrence_createdby = "lk_msdyn_forecastrecurrence_createdby";
            [Relationship("msdyn_forecastrecurrence", EntityRole.Referenced, "lk_msdyn_forecastrecurrence_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_forecastrecurrence_createdonbehalfby = "lk_msdyn_forecastrecurrence_createdonbehalfby";
            [Relationship("msdyn_forecastrecurrence", EntityRole.Referenced, "lk_msdyn_forecastrecurrence_modifiedby", "modifiedby")]
            public const string lk_msdyn_forecastrecurrence_modifiedby = "lk_msdyn_forecastrecurrence_modifiedby";
            [Relationship("msdyn_forecastrecurrence", EntityRole.Referenced, "lk_msdyn_forecastrecurrence_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_forecastrecurrence_modifiedonbehalfby = "lk_msdyn_forecastrecurrence_modifiedonbehalfby";
            [Relationship("msdyn_functionallocation", EntityRole.Referenced, "lk_msdyn_functionallocation_createdby", "createdby")]
            public const string lk_msdyn_functionallocation_createdby = "lk_msdyn_functionallocation_createdby";
            [Relationship("msdyn_functionallocation", EntityRole.Referenced, "lk_msdyn_functionallocation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_functionallocation_createdonbehalfby = "lk_msdyn_functionallocation_createdonbehalfby";
            [Relationship("msdyn_functionallocation", EntityRole.Referenced, "lk_msdyn_functionallocation_modifiedby", "modifiedby")]
            public const string lk_msdyn_functionallocation_modifiedby = "lk_msdyn_functionallocation_modifiedby";
            [Relationship("msdyn_functionallocation", EntityRole.Referenced, "lk_msdyn_functionallocation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_functionallocation_modifiedonbehalfby = "lk_msdyn_functionallocation_modifiedonbehalfby";
            [Relationship("msdyn_gdprdata", EntityRole.Referenced, "lk_msdyn_gdprdata_createdby", "createdby")]
            public const string lk_msdyn_gdprdata_createdby = "lk_msdyn_gdprdata_createdby";
            [Relationship("msdyn_gdprdata", EntityRole.Referenced, "lk_msdyn_gdprdata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_gdprdata_createdonbehalfby = "lk_msdyn_gdprdata_createdonbehalfby";
            [Relationship("msdyn_gdprdata", EntityRole.Referenced, "lk_msdyn_gdprdata_modifiedby", "modifiedby")]
            public const string lk_msdyn_gdprdata_modifiedby = "lk_msdyn_gdprdata_modifiedby";
            [Relationship("msdyn_gdprdata", EntityRole.Referenced, "lk_msdyn_gdprdata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_gdprdata_modifiedonbehalfby = "lk_msdyn_gdprdata_modifiedonbehalfby";
            [Relationship("msdyn_geofence", EntityRole.Referenced, "lk_msdyn_geofence_createdby", "createdby")]
            public const string lk_msdyn_geofence_createdby = "lk_msdyn_geofence_createdby";
            [Relationship("msdyn_geofence", EntityRole.Referenced, "lk_msdyn_geofence_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_geofence_createdonbehalfby = "lk_msdyn_geofence_createdonbehalfby";
            [Relationship("msdyn_geofence", EntityRole.Referenced, "lk_msdyn_geofence_modifiedby", "modifiedby")]
            public const string lk_msdyn_geofence_modifiedby = "lk_msdyn_geofence_modifiedby";
            [Relationship("msdyn_geofence", EntityRole.Referenced, "lk_msdyn_geofence_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_geofence_modifiedonbehalfby = "lk_msdyn_geofence_modifiedonbehalfby";
            [Relationship("msdyn_geofenceevent", EntityRole.Referenced, "lk_msdyn_geofenceevent_createdby", "createdby")]
            public const string lk_msdyn_geofenceevent_createdby = "lk_msdyn_geofenceevent_createdby";
            [Relationship("msdyn_geofenceevent", EntityRole.Referenced, "lk_msdyn_geofenceevent_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_geofenceevent_createdonbehalfby = "lk_msdyn_geofenceevent_createdonbehalfby";
            [Relationship("msdyn_geofenceevent", EntityRole.Referenced, "lk_msdyn_geofenceevent_modifiedby", "modifiedby")]
            public const string lk_msdyn_geofenceevent_modifiedby = "lk_msdyn_geofenceevent_modifiedby";
            [Relationship("msdyn_geofenceevent", EntityRole.Referenced, "lk_msdyn_geofenceevent_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_geofenceevent_modifiedonbehalfby = "lk_msdyn_geofenceevent_modifiedonbehalfby";
            [Relationship("msdyn_geofencingsettings", EntityRole.Referenced, "lk_msdyn_geofencingsettings_createdby", "createdby")]
            public const string lk_msdyn_geofencingsettings_createdby = "lk_msdyn_geofencingsettings_createdby";
            [Relationship("msdyn_geofencingsettings", EntityRole.Referenced, "lk_msdyn_geofencingsettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_geofencingsettings_createdonbehalfby = "lk_msdyn_geofencingsettings_createdonbehalfby";
            [Relationship("msdyn_geofencingsettings", EntityRole.Referenced, "lk_msdyn_geofencingsettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_geofencingsettings_modifiedby = "lk_msdyn_geofencingsettings_modifiedby";
            [Relationship("msdyn_geofencingsettings", EntityRole.Referenced, "lk_msdyn_geofencingsettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_geofencingsettings_modifiedonbehalfby = "lk_msdyn_geofencingsettings_modifiedonbehalfby";
            [Relationship("msdyn_geolocationsettings", EntityRole.Referenced, "lk_msdyn_geolocationsettings_createdby", "createdby")]
            public const string lk_msdyn_geolocationsettings_createdby = "lk_msdyn_geolocationsettings_createdby";
            [Relationship("msdyn_geolocationsettings", EntityRole.Referenced, "lk_msdyn_geolocationsettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_geolocationsettings_createdonbehalfby = "lk_msdyn_geolocationsettings_createdonbehalfby";
            [Relationship("msdyn_geolocationsettings", EntityRole.Referenced, "lk_msdyn_geolocationsettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_geolocationsettings_modifiedby = "lk_msdyn_geolocationsettings_modifiedby";
            [Relationship("msdyn_geolocationsettings", EntityRole.Referenced, "lk_msdyn_geolocationsettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_geolocationsettings_modifiedonbehalfby = "lk_msdyn_geolocationsettings_modifiedonbehalfby";
            [Relationship("msdyn_geolocationtracking", EntityRole.Referenced, "lk_msdyn_geolocationtracking_createdby", "createdby")]
            public const string lk_msdyn_geolocationtracking_createdby = "lk_msdyn_geolocationtracking_createdby";
            [Relationship("msdyn_geolocationtracking", EntityRole.Referenced, "lk_msdyn_geolocationtracking_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_geolocationtracking_createdonbehalfby = "lk_msdyn_geolocationtracking_createdonbehalfby";
            [Relationship("msdyn_geolocationtracking", EntityRole.Referenced, "lk_msdyn_geolocationtracking_modifiedby", "modifiedby")]
            public const string lk_msdyn_geolocationtracking_modifiedby = "lk_msdyn_geolocationtracking_modifiedby";
            [Relationship("msdyn_geolocationtracking", EntityRole.Referenced, "lk_msdyn_geolocationtracking_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_geolocationtracking_modifiedonbehalfby = "lk_msdyn_geolocationtracking_modifiedonbehalfby";
            [Relationship("msdyn_helppage", EntityRole.Referenced, "lk_msdyn_helppage_createdby", "createdby")]
            public const string lk_msdyn_helppage_createdby = "lk_msdyn_helppage_createdby";
            [Relationship("msdyn_helppage", EntityRole.Referenced, "lk_msdyn_helppage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_helppage_createdonbehalfby = "lk_msdyn_helppage_createdonbehalfby";
            [Relationship("msdyn_helppage", EntityRole.Referenced, "lk_msdyn_helppage_modifiedby", "modifiedby")]
            public const string lk_msdyn_helppage_modifiedby = "lk_msdyn_helppage_modifiedby";
            [Relationship("msdyn_helppage", EntityRole.Referenced, "lk_msdyn_helppage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_helppage_modifiedonbehalfby = "lk_msdyn_helppage_modifiedonbehalfby";
            [Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "lk_msdyn_icebreakersconfig_createdby", "createdby")]
            public const string lk_msdyn_icebreakersconfig_createdby = "lk_msdyn_icebreakersconfig_createdby";
            [Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "lk_msdyn_icebreakersconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_icebreakersconfig_createdonbehalfby = "lk_msdyn_icebreakersconfig_createdonbehalfby";
            [Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "lk_msdyn_icebreakersconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_icebreakersconfig_modifiedby = "lk_msdyn_icebreakersconfig_modifiedby";
            [Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "lk_msdyn_icebreakersconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_icebreakersconfig_modifiedonbehalfby = "lk_msdyn_icebreakersconfig_modifiedonbehalfby";
            [Relationship("msdyn_incidenttype", EntityRole.Referenced, "lk_msdyn_incidenttype_createdby", "createdby")]
            public const string lk_msdyn_incidenttype_createdby = "lk_msdyn_incidenttype_createdby";
            [Relationship("msdyn_incidenttype", EntityRole.Referenced, "lk_msdyn_incidenttype_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_incidenttype_createdonbehalfby = "lk_msdyn_incidenttype_createdonbehalfby";
            [Relationship("msdyn_incidenttype", EntityRole.Referenced, "lk_msdyn_incidenttype_modifiedby", "modifiedby")]
            public const string lk_msdyn_incidenttype_modifiedby = "lk_msdyn_incidenttype_modifiedby";
            [Relationship("msdyn_incidenttype", EntityRole.Referenced, "lk_msdyn_incidenttype_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_incidenttype_modifiedonbehalfby = "lk_msdyn_incidenttype_modifiedonbehalfby";
            [Relationship("msdyn_incidenttype_requirementgroup", EntityRole.Referenced, "lk_msdyn_incidenttype_requirementgroup_createdby", "createdby")]
            public const string lk_msdyn_incidenttype_requirementgroup_createdby = "lk_msdyn_incidenttype_requirementgroup_createdby";
            [Relationship("msdyn_incidenttype_requirementgroup", EntityRole.Referenced, "lk_msdyn_incidenttype_requirementgroup_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_incidenttype_requirementgroup_createdonbehalfby = "lk_msdyn_incidenttype_requirementgroup_createdonbehalfby";
            [Relationship("msdyn_incidenttype_requirementgroup", EntityRole.Referenced, "lk_msdyn_incidenttype_requirementgroup_modifiedby", "modifiedby")]
            public const string lk_msdyn_incidenttype_requirementgroup_modifiedby = "lk_msdyn_incidenttype_requirementgroup_modifiedby";
            [Relationship("msdyn_incidenttype_requirementgroup", EntityRole.Referenced, "lk_msdyn_incidenttype_requirementgroup_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_incidenttype_requirementgroup_modifiedonbehalfby = "lk_msdyn_incidenttype_requirementgroup_modifiedonbehalfby";
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
            [Relationship("msdyn_incidenttyperecommendationresult", EntityRole.Referenced, "lk_msdyn_incidenttyperecommendationresult_createdby", "createdby")]
            public const string lk_msdyn_incidenttyperecommendationresult_createdby = "lk_msdyn_incidenttyperecommendationresult_createdby";
            [Relationship("msdyn_incidenttyperecommendationresult", EntityRole.Referenced, "lk_msdyn_incidenttyperecommendationresult_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_incidenttyperecommendationresult_createdonbehalfby = "lk_msdyn_incidenttyperecommendationresult_createdonbehalfby";
            [Relationship("msdyn_incidenttyperecommendationresult", EntityRole.Referenced, "lk_msdyn_incidenttyperecommendationresult_modifiedby", "modifiedby")]
            public const string lk_msdyn_incidenttyperecommendationresult_modifiedby = "lk_msdyn_incidenttyperecommendationresult_modifiedby";
            [Relationship("msdyn_incidenttyperecommendationresult", EntityRole.Referenced, "lk_msdyn_incidenttyperecommendationresult_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_incidenttyperecommendationresult_modifiedonbehalfby = "lk_msdyn_incidenttyperecommendationresult_modifiedonbehalfby";
            [Relationship("msdyn_incidenttyperecommendationrunhistory", EntityRole.Referenced, "lk_msdyn_incidenttyperecommendationrunhistory_createdby", "createdby")]
            public const string lk_msdyn_incidenttyperecommendationrunhistory_createdby = "lk_msdyn_incidenttyperecommendationrunhistory_createdby";
            [Relationship("msdyn_incidenttyperecommendationrunhistory", EntityRole.Referenced, "lk_msdyn_incidenttyperecommendationrunhistory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_incidenttyperecommendationrunhistory_createdonbehalfby = "lk_msdyn_incidenttyperecommendationrunhistory_createdonbehalfby";
            [Relationship("msdyn_incidenttyperecommendationrunhistory", EntityRole.Referenced, "lk_msdyn_incidenttyperecommendationrunhistory_modifiedby", "modifiedby")]
            public const string lk_msdyn_incidenttyperecommendationrunhistory_modifiedby = "lk_msdyn_incidenttyperecommendationrunhistory_modifiedby";
            [Relationship("msdyn_incidenttyperecommendationrunhistory", EntityRole.Referenced, "lk_msdyn_incidenttyperecommendationrunhistory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_incidenttyperecommendationrunhistory_modifiedonbehalfby = "lk_msdyn_incidenttyperecommendationrunhistory_modifiedonbehalfby";
            [Relationship("msdyn_incidenttyperesolution", EntityRole.Referenced, "lk_msdyn_incidenttyperesolution_createdby", "createdby")]
            public const string lk_msdyn_incidenttyperesolution_createdby = "lk_msdyn_incidenttyperesolution_createdby";
            [Relationship("msdyn_incidenttyperesolution", EntityRole.Referenced, "lk_msdyn_incidenttyperesolution_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_incidenttyperesolution_createdonbehalfby = "lk_msdyn_incidenttyperesolution_createdonbehalfby";
            [Relationship("msdyn_incidenttyperesolution", EntityRole.Referenced, "lk_msdyn_incidenttyperesolution_modifiedby", "modifiedby")]
            public const string lk_msdyn_incidenttyperesolution_modifiedby = "lk_msdyn_incidenttyperesolution_modifiedby";
            [Relationship("msdyn_incidenttyperesolution", EntityRole.Referenced, "lk_msdyn_incidenttyperesolution_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_incidenttyperesolution_modifiedonbehalfby = "lk_msdyn_incidenttyperesolution_modifiedonbehalfby";
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
            [Relationship("msdyn_incidenttypessetup", EntityRole.Referenced, "lk_msdyn_incidenttypessetup_createdby", "createdby")]
            public const string lk_msdyn_incidenttypessetup_createdby = "lk_msdyn_incidenttypessetup_createdby";
            [Relationship("msdyn_incidenttypessetup", EntityRole.Referenced, "lk_msdyn_incidenttypessetup_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_incidenttypessetup_createdonbehalfby = "lk_msdyn_incidenttypessetup_createdonbehalfby";
            [Relationship("msdyn_incidenttypessetup", EntityRole.Referenced, "lk_msdyn_incidenttypessetup_modifiedby", "modifiedby")]
            public const string lk_msdyn_incidenttypessetup_modifiedby = "lk_msdyn_incidenttypessetup_modifiedby";
            [Relationship("msdyn_incidenttypessetup", EntityRole.Referenced, "lk_msdyn_incidenttypessetup_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_incidenttypessetup_modifiedonbehalfby = "lk_msdyn_incidenttypessetup_modifiedonbehalfby";
            [Relationship("msdyn_inspection", EntityRole.Referenced, "lk_msdyn_inspection_createdby", "createdby")]
            public const string lk_msdyn_inspection_createdby = "lk_msdyn_inspection_createdby";
            [Relationship("msdyn_inspection", EntityRole.Referenced, "lk_msdyn_inspection_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_inspection_createdonbehalfby = "lk_msdyn_inspection_createdonbehalfby";
            [Relationship("msdyn_inspection", EntityRole.Referenced, "lk_msdyn_inspection_modifiedby", "modifiedby")]
            public const string lk_msdyn_inspection_modifiedby = "lk_msdyn_inspection_modifiedby";
            [Relationship("msdyn_inspection", EntityRole.Referenced, "lk_msdyn_inspection_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_inspection_modifiedonbehalfby = "lk_msdyn_inspection_modifiedonbehalfby";
            [Relationship("msdyn_inspectionattachment", EntityRole.Referenced, "lk_msdyn_inspectionattachment_createdby", "createdby")]
            public const string lk_msdyn_inspectionattachment_createdby = "lk_msdyn_inspectionattachment_createdby";
            [Relationship("msdyn_inspectionattachment", EntityRole.Referenced, "lk_msdyn_inspectionattachment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_inspectionattachment_createdonbehalfby = "lk_msdyn_inspectionattachment_createdonbehalfby";
            [Relationship("msdyn_inspectionattachment", EntityRole.Referenced, "lk_msdyn_inspectionattachment_modifiedby", "modifiedby")]
            public const string lk_msdyn_inspectionattachment_modifiedby = "lk_msdyn_inspectionattachment_modifiedby";
            [Relationship("msdyn_inspectionattachment", EntityRole.Referenced, "lk_msdyn_inspectionattachment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_inspectionattachment_modifiedonbehalfby = "lk_msdyn_inspectionattachment_modifiedonbehalfby";
            [Relationship("msdyn_inspectiondefinition", EntityRole.Referenced, "lk_msdyn_inspectiondefinition_createdby", "createdby")]
            public const string lk_msdyn_inspectiondefinition_createdby = "lk_msdyn_inspectiondefinition_createdby";
            [Relationship("msdyn_inspectiondefinition", EntityRole.Referenced, "lk_msdyn_inspectiondefinition_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_inspectiondefinition_createdonbehalfby = "lk_msdyn_inspectiondefinition_createdonbehalfby";
            [Relationship("msdyn_inspectiondefinition", EntityRole.Referenced, "lk_msdyn_inspectiondefinition_modifiedby", "modifiedby")]
            public const string lk_msdyn_inspectiondefinition_modifiedby = "lk_msdyn_inspectiondefinition_modifiedby";
            [Relationship("msdyn_inspectiondefinition", EntityRole.Referenced, "lk_msdyn_inspectiondefinition_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_inspectiondefinition_modifiedonbehalfby = "lk_msdyn_inspectiondefinition_modifiedonbehalfby";
            [Relationship("msdyn_inspectioninstance", EntityRole.Referenced, "lk_msdyn_inspectioninstance_createdby", "createdby")]
            public const string lk_msdyn_inspectioninstance_createdby = "lk_msdyn_inspectioninstance_createdby";
            [Relationship("msdyn_inspectioninstance", EntityRole.Referenced, "lk_msdyn_inspectioninstance_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_inspectioninstance_createdonbehalfby = "lk_msdyn_inspectioninstance_createdonbehalfby";
            [Relationship("msdyn_inspectioninstance", EntityRole.Referenced, "lk_msdyn_inspectioninstance_modifiedby", "modifiedby")]
            public const string lk_msdyn_inspectioninstance_modifiedby = "lk_msdyn_inspectioninstance_modifiedby";
            [Relationship("msdyn_inspectioninstance", EntityRole.Referenced, "lk_msdyn_inspectioninstance_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_inspectioninstance_modifiedonbehalfby = "lk_msdyn_inspectioninstance_modifiedonbehalfby";
            [Relationship("msdyn_inspectionresponse", EntityRole.Referenced, "lk_msdyn_inspectionresponse_createdby", "createdby")]
            public const string lk_msdyn_inspectionresponse_createdby = "lk_msdyn_inspectionresponse_createdby";
            [Relationship("msdyn_inspectionresponse", EntityRole.Referenced, "lk_msdyn_inspectionresponse_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_inspectionresponse_createdonbehalfby = "lk_msdyn_inspectionresponse_createdonbehalfby";
            [Relationship("msdyn_inspectionresponse", EntityRole.Referenced, "lk_msdyn_inspectionresponse_modifiedby", "modifiedby")]
            public const string lk_msdyn_inspectionresponse_modifiedby = "lk_msdyn_inspectionresponse_modifiedby";
            [Relationship("msdyn_inspectionresponse", EntityRole.Referenced, "lk_msdyn_inspectionresponse_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_inspectionresponse_modifiedonbehalfby = "lk_msdyn_inspectionresponse_modifiedonbehalfby";
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
            [Relationship("msdyn_iotalert", EntityRole.Referenced, "lk_msdyn_iotalert_createdby", "createdby")]
            public const string lk_msdyn_iotalert_createdby = "lk_msdyn_iotalert_createdby";
            [Relationship("msdyn_iotalert", EntityRole.Referenced, "lk_msdyn_iotalert_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotalert_createdonbehalfby = "lk_msdyn_iotalert_createdonbehalfby";
            [Relationship("msdyn_iotalert", EntityRole.Referenced, "lk_msdyn_iotalert_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotalert_modifiedby = "lk_msdyn_iotalert_modifiedby";
            [Relationship("msdyn_iotalert", EntityRole.Referenced, "lk_msdyn_iotalert_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotalert_modifiedonbehalfby = "lk_msdyn_iotalert_modifiedonbehalfby";
            [Relationship("msdyn_iotdevice", EntityRole.Referenced, "lk_msdyn_iotdevice_createdby", "createdby")]
            public const string lk_msdyn_iotdevice_createdby = "lk_msdyn_iotdevice_createdby";
            [Relationship("msdyn_iotdevice", EntityRole.Referenced, "lk_msdyn_iotdevice_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotdevice_createdonbehalfby = "lk_msdyn_iotdevice_createdonbehalfby";
            [Relationship("msdyn_iotdevice", EntityRole.Referenced, "lk_msdyn_iotdevice_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotdevice_modifiedby = "lk_msdyn_iotdevice_modifiedby";
            [Relationship("msdyn_iotdevice", EntityRole.Referenced, "lk_msdyn_iotdevice_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotdevice_modifiedonbehalfby = "lk_msdyn_iotdevice_modifiedonbehalfby";
            [Relationship("msdyn_iotdevicecategory", EntityRole.Referenced, "lk_msdyn_iotdevicecategory_createdby", "createdby")]
            public const string lk_msdyn_iotdevicecategory_createdby = "lk_msdyn_iotdevicecategory_createdby";
            [Relationship("msdyn_iotdevicecategory", EntityRole.Referenced, "lk_msdyn_iotdevicecategory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotdevicecategory_createdonbehalfby = "lk_msdyn_iotdevicecategory_createdonbehalfby";
            [Relationship("msdyn_iotdevicecategory", EntityRole.Referenced, "lk_msdyn_iotdevicecategory_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotdevicecategory_modifiedby = "lk_msdyn_iotdevicecategory_modifiedby";
            [Relationship("msdyn_iotdevicecategory", EntityRole.Referenced, "lk_msdyn_iotdevicecategory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotdevicecategory_modifiedonbehalfby = "lk_msdyn_iotdevicecategory_modifiedonbehalfby";
            [Relationship("msdyn_iotdevicecommand", EntityRole.Referenced, "lk_msdyn_iotdevicecommand_createdby", "createdby")]
            public const string lk_msdyn_iotdevicecommand_createdby = "lk_msdyn_iotdevicecommand_createdby";
            [Relationship("msdyn_iotdevicecommand", EntityRole.Referenced, "lk_msdyn_iotdevicecommand_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotdevicecommand_createdonbehalfby = "lk_msdyn_iotdevicecommand_createdonbehalfby";
            [Relationship("msdyn_iotdevicecommand", EntityRole.Referenced, "lk_msdyn_iotdevicecommand_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotdevicecommand_modifiedby = "lk_msdyn_iotdevicecommand_modifiedby";
            [Relationship("msdyn_iotdevicecommand", EntityRole.Referenced, "lk_msdyn_iotdevicecommand_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotdevicecommand_modifiedonbehalfby = "lk_msdyn_iotdevicecommand_modifiedonbehalfby";
            [Relationship("msdyn_iotdevicecommanddefinition", EntityRole.Referenced, "lk_msdyn_iotdevicecommanddefinition_createdby", "createdby")]
            public const string lk_msdyn_iotdevicecommanddefinition_createdby = "lk_msdyn_iotdevicecommanddefinition_createdby";
            [Relationship("msdyn_iotdevicecommanddefinition", EntityRole.Referenced, "lk_msdyn_iotdevicecommanddefinition_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotdevicecommanddefinition_createdonbehalfby = "lk_msdyn_iotdevicecommanddefinition_createdonbehalfby";
            [Relationship("msdyn_iotdevicecommanddefinition", EntityRole.Referenced, "lk_msdyn_iotdevicecommanddefinition_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotdevicecommanddefinition_modifiedby = "lk_msdyn_iotdevicecommanddefinition_modifiedby";
            [Relationship("msdyn_iotdevicecommanddefinition", EntityRole.Referenced, "lk_msdyn_iotdevicecommanddefinition_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotdevicecommanddefinition_modifiedonbehalfby = "lk_msdyn_iotdevicecommanddefinition_modifiedonbehalfby";
            [Relationship("msdyn_iotdevicedatahistory", EntityRole.Referenced, "lk_msdyn_iotdevicedatahistory_createdby", "createdby")]
            public const string lk_msdyn_iotdevicedatahistory_createdby = "lk_msdyn_iotdevicedatahistory_createdby";
            [Relationship("msdyn_iotdevicedatahistory", EntityRole.Referenced, "lk_msdyn_iotdevicedatahistory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotdevicedatahistory_createdonbehalfby = "lk_msdyn_iotdevicedatahistory_createdonbehalfby";
            [Relationship("msdyn_iotdevicedatahistory", EntityRole.Referenced, "lk_msdyn_iotdevicedatahistory_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotdevicedatahistory_modifiedby = "lk_msdyn_iotdevicedatahistory_modifiedby";
            [Relationship("msdyn_iotdevicedatahistory", EntityRole.Referenced, "lk_msdyn_iotdevicedatahistory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotdevicedatahistory_modifiedonbehalfby = "lk_msdyn_iotdevicedatahistory_modifiedonbehalfby";
            [Relationship("msdyn_iotdeviceproperty", EntityRole.Referenced, "lk_msdyn_iotdeviceproperty_createdby", "createdby")]
            public const string lk_msdyn_iotdeviceproperty_createdby = "lk_msdyn_iotdeviceproperty_createdby";
            [Relationship("msdyn_iotdeviceproperty", EntityRole.Referenced, "lk_msdyn_iotdeviceproperty_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotdeviceproperty_createdonbehalfby = "lk_msdyn_iotdeviceproperty_createdonbehalfby";
            [Relationship("msdyn_iotdeviceproperty", EntityRole.Referenced, "lk_msdyn_iotdeviceproperty_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotdeviceproperty_modifiedby = "lk_msdyn_iotdeviceproperty_modifiedby";
            [Relationship("msdyn_iotdeviceproperty", EntityRole.Referenced, "lk_msdyn_iotdeviceproperty_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotdeviceproperty_modifiedonbehalfby = "lk_msdyn_iotdeviceproperty_modifiedonbehalfby";
            [Relationship("msdyn_iotdeviceregistrationhistory", EntityRole.Referenced, "lk_msdyn_iotdeviceregistrationhistory_createdby", "createdby")]
            public const string lk_msdyn_iotdeviceregistrationhistory_createdby = "lk_msdyn_iotdeviceregistrationhistory_createdby";
            [Relationship("msdyn_iotdeviceregistrationhistory", EntityRole.Referenced, "lk_msdyn_iotdeviceregistrationhistory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotdeviceregistrationhistory_createdonbehalfby = "lk_msdyn_iotdeviceregistrationhistory_createdonbehalfby";
            [Relationship("msdyn_iotdeviceregistrationhistory", EntityRole.Referenced, "lk_msdyn_iotdeviceregistrationhistory_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotdeviceregistrationhistory_modifiedby = "lk_msdyn_iotdeviceregistrationhistory_modifiedby";
            [Relationship("msdyn_iotdeviceregistrationhistory", EntityRole.Referenced, "lk_msdyn_iotdeviceregistrationhistory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotdeviceregistrationhistory_modifiedonbehalfby = "lk_msdyn_iotdeviceregistrationhistory_modifiedonbehalfby";
            [Relationship("msdyn_iotdevicevisualizationconfiguration", EntityRole.Referenced, "lk_msdyn_iotdevicevisualizationconfiguration_createdby", "createdby")]
            public const string lk_msdyn_iotdevicevisualizationconfiguration_createdby = "lk_msdyn_iotdevicevisualizationconfiguration_createdby";
            [Relationship("msdyn_iotdevicevisualizationconfiguration", EntityRole.Referenced, "lk_msdyn_iotdevicevisualizationconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotdevicevisualizationconfiguration_createdonbehalfby = "lk_msdyn_iotdevicevisualizationconfiguration_createdonbehalfby";
            [Relationship("msdyn_iotdevicevisualizationconfiguration", EntityRole.Referenced, "lk_msdyn_iotdevicevisualizationconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotdevicevisualizationconfiguration_modifiedby = "lk_msdyn_iotdevicevisualizationconfiguration_modifiedby";
            [Relationship("msdyn_iotdevicevisualizationconfiguration", EntityRole.Referenced, "lk_msdyn_iotdevicevisualizationconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotdevicevisualizationconfiguration_modifiedonbehalfby = "lk_msdyn_iotdevicevisualizationconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_iotfieldmapping", EntityRole.Referenced, "lk_msdyn_iotfieldmapping_createdby", "createdby")]
            public const string lk_msdyn_iotfieldmapping_createdby = "lk_msdyn_iotfieldmapping_createdby";
            [Relationship("msdyn_iotfieldmapping", EntityRole.Referenced, "lk_msdyn_iotfieldmapping_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotfieldmapping_createdonbehalfby = "lk_msdyn_iotfieldmapping_createdonbehalfby";
            [Relationship("msdyn_iotfieldmapping", EntityRole.Referenced, "lk_msdyn_iotfieldmapping_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotfieldmapping_modifiedby = "lk_msdyn_iotfieldmapping_modifiedby";
            [Relationship("msdyn_iotfieldmapping", EntityRole.Referenced, "lk_msdyn_iotfieldmapping_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotfieldmapping_modifiedonbehalfby = "lk_msdyn_iotfieldmapping_modifiedonbehalfby";
            [Relationship("msdyn_iotpropertydefinition", EntityRole.Referenced, "lk_msdyn_iotpropertydefinition_createdby", "createdby")]
            public const string lk_msdyn_iotpropertydefinition_createdby = "lk_msdyn_iotpropertydefinition_createdby";
            [Relationship("msdyn_iotpropertydefinition", EntityRole.Referenced, "lk_msdyn_iotpropertydefinition_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotpropertydefinition_createdonbehalfby = "lk_msdyn_iotpropertydefinition_createdonbehalfby";
            [Relationship("msdyn_iotpropertydefinition", EntityRole.Referenced, "lk_msdyn_iotpropertydefinition_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotpropertydefinition_modifiedby = "lk_msdyn_iotpropertydefinition_modifiedby";
            [Relationship("msdyn_iotpropertydefinition", EntityRole.Referenced, "lk_msdyn_iotpropertydefinition_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotpropertydefinition_modifiedonbehalfby = "lk_msdyn_iotpropertydefinition_modifiedonbehalfby";
            [Relationship("msdyn_iotprovider", EntityRole.Referenced, "lk_msdyn_iotprovider_createdby", "createdby")]
            public const string lk_msdyn_iotprovider_createdby = "lk_msdyn_iotprovider_createdby";
            [Relationship("msdyn_iotprovider", EntityRole.Referenced, "lk_msdyn_iotprovider_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotprovider_createdonbehalfby = "lk_msdyn_iotprovider_createdonbehalfby";
            [Relationship("msdyn_iotprovider", EntityRole.Referenced, "lk_msdyn_iotprovider_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotprovider_modifiedby = "lk_msdyn_iotprovider_modifiedby";
            [Relationship("msdyn_iotprovider", EntityRole.Referenced, "lk_msdyn_iotprovider_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotprovider_modifiedonbehalfby = "lk_msdyn_iotprovider_modifiedonbehalfby";
            [Relationship("msdyn_iotproviderinstance", EntityRole.Referenced, "lk_msdyn_iotproviderinstance_createdby", "createdby")]
            public const string lk_msdyn_iotproviderinstance_createdby = "lk_msdyn_iotproviderinstance_createdby";
            [Relationship("msdyn_iotproviderinstance", EntityRole.Referenced, "lk_msdyn_iotproviderinstance_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotproviderinstance_createdonbehalfby = "lk_msdyn_iotproviderinstance_createdonbehalfby";
            [Relationship("msdyn_iotproviderinstance", EntityRole.Referenced, "lk_msdyn_iotproviderinstance_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotproviderinstance_modifiedby = "lk_msdyn_iotproviderinstance_modifiedby";
            [Relationship("msdyn_iotproviderinstance", EntityRole.Referenced, "lk_msdyn_iotproviderinstance_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotproviderinstance_modifiedonbehalfby = "lk_msdyn_iotproviderinstance_modifiedonbehalfby";
            [Relationship("msdyn_iotsettings", EntityRole.Referenced, "lk_msdyn_iotsettings_createdby", "createdby")]
            public const string lk_msdyn_iotsettings_createdby = "lk_msdyn_iotsettings_createdby";
            [Relationship("msdyn_iotsettings", EntityRole.Referenced, "lk_msdyn_iotsettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iotsettings_createdonbehalfby = "lk_msdyn_iotsettings_createdonbehalfby";
            [Relationship("msdyn_iotsettings", EntityRole.Referenced, "lk_msdyn_iotsettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_iotsettings_modifiedby = "lk_msdyn_iotsettings_modifiedby";
            [Relationship("msdyn_iotsettings", EntityRole.Referenced, "lk_msdyn_iotsettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iotsettings_modifiedonbehalfby = "lk_msdyn_iotsettings_modifiedonbehalfby";
            [Relationship("msdyn_iottocaseprocess", EntityRole.Referenced, "lk_msdyn_iottocaseprocess_createdby", "createdby")]
            public const string lk_msdyn_iottocaseprocess_createdby = "lk_msdyn_iottocaseprocess_createdby";
            [Relationship("msdyn_iottocaseprocess", EntityRole.Referenced, "lk_msdyn_iottocaseprocess_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_iottocaseprocess_createdonbehalfby = "lk_msdyn_iottocaseprocess_createdonbehalfby";
            [Relationship("msdyn_iottocaseprocess", EntityRole.Referenced, "lk_msdyn_iottocaseprocess_modifiedby", "modifiedby")]
            public const string lk_msdyn_iottocaseprocess_modifiedby = "lk_msdyn_iottocaseprocess_modifiedby";
            [Relationship("msdyn_iottocaseprocess", EntityRole.Referenced, "lk_msdyn_iottocaseprocess_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_iottocaseprocess_modifiedonbehalfby = "lk_msdyn_iottocaseprocess_modifiedonbehalfby";
            [Relationship("msdyn_kalanguagesetting", EntityRole.Referenced, "lk_msdyn_kalanguagesetting_createdby", "createdby")]
            public const string lk_msdyn_kalanguagesetting_createdby = "lk_msdyn_kalanguagesetting_createdby";
            [Relationship("msdyn_kalanguagesetting", EntityRole.Referenced, "lk_msdyn_kalanguagesetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_kalanguagesetting_createdonbehalfby = "lk_msdyn_kalanguagesetting_createdonbehalfby";
            [Relationship("msdyn_kalanguagesetting", EntityRole.Referenced, "lk_msdyn_kalanguagesetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_kalanguagesetting_modifiedby = "lk_msdyn_kalanguagesetting_modifiedby";
            [Relationship("msdyn_kalanguagesetting", EntityRole.Referenced, "lk_msdyn_kalanguagesetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_kalanguagesetting_modifiedonbehalfby = "lk_msdyn_kalanguagesetting_modifiedonbehalfby";
            [Relationship("msdyn_kbattachment", EntityRole.Referenced, "lk_msdyn_kbattachment_createdby", "createdby")]
            public const string lk_msdyn_kbattachment_createdby = "lk_msdyn_kbattachment_createdby";
            [Relationship("msdyn_kbattachment", EntityRole.Referenced, "lk_msdyn_kbattachment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_kbattachment_createdonbehalfby = "lk_msdyn_kbattachment_createdonbehalfby";
            [Relationship("msdyn_kbattachment", EntityRole.Referenced, "lk_msdyn_kbattachment_modifiedby", "modifiedby")]
            public const string lk_msdyn_kbattachment_modifiedby = "lk_msdyn_kbattachment_modifiedby";
            [Relationship("msdyn_kbattachment", EntityRole.Referenced, "lk_msdyn_kbattachment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_kbattachment_modifiedonbehalfby = "lk_msdyn_kbattachment_modifiedonbehalfby";
            [Relationship("msdyn_kbenrichment", EntityRole.Referenced, "lk_msdyn_kbenrichment_createdby", "createdby")]
            public const string lk_msdyn_kbenrichment_createdby = "lk_msdyn_kbenrichment_createdby";
            [Relationship("msdyn_kbenrichment", EntityRole.Referenced, "lk_msdyn_kbenrichment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_kbenrichment_createdonbehalfby = "lk_msdyn_kbenrichment_createdonbehalfby";
            [Relationship("msdyn_kbenrichment", EntityRole.Referenced, "lk_msdyn_kbenrichment_modifiedby", "modifiedby")]
            public const string lk_msdyn_kbenrichment_modifiedby = "lk_msdyn_kbenrichment_modifiedby";
            [Relationship("msdyn_kbenrichment", EntityRole.Referenced, "lk_msdyn_kbenrichment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_kbenrichment_modifiedonbehalfby = "lk_msdyn_kbenrichment_modifiedonbehalfby";
            [Relationship("msdyn_kbkeywordsdescsuggestionsetting", EntityRole.Referenced, "lk_msdyn_kbkeywordsdescsuggestionsetting_createdby", "createdby")]
            public const string lk_msdyn_kbkeywordsdescsuggestionsetting_createdby = "lk_msdyn_kbkeywordsdescsuggestionsetting_createdby";
            [Relationship("msdyn_kbkeywordsdescsuggestionsetting", EntityRole.Referenced, "lk_msdyn_kbkeywordsdescsuggestionsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_kbkeywordsdescsuggestionsetting_createdonbehalfby = "lk_msdyn_kbkeywordsdescsuggestionsetting_createdonbehalfby";
            [Relationship("msdyn_kbkeywordsdescsuggestionsetting", EntityRole.Referenced, "lk_msdyn_kbkeywordsdescsuggestionsetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_kbkeywordsdescsuggestionsetting_modifiedby = "lk_msdyn_kbkeywordsdescsuggestionsetting_modifiedby";
            [Relationship("msdyn_kbkeywordsdescsuggestionsetting", EntityRole.Referenced, "lk_msdyn_kbkeywordsdescsuggestionsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_kbkeywordsdescsuggestionsetting_modifiedonbehalfby = "lk_msdyn_kbkeywordsdescsuggestionsetting_modifiedonbehalfby";
            [Relationship("msdyn_kmfederatedsearchconfig", EntityRole.Referenced, "lk_msdyn_kmfederatedsearchconfig_createdby", "createdby")]
            public const string lk_msdyn_kmfederatedsearchconfig_createdby = "lk_msdyn_kmfederatedsearchconfig_createdby";
            [Relationship("msdyn_kmfederatedsearchconfig", EntityRole.Referenced, "lk_msdyn_kmfederatedsearchconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_kmfederatedsearchconfig_createdonbehalfby = "lk_msdyn_kmfederatedsearchconfig_createdonbehalfby";
            [Relationship("msdyn_kmfederatedsearchconfig", EntityRole.Referenced, "lk_msdyn_kmfederatedsearchconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_kmfederatedsearchconfig_modifiedby = "lk_msdyn_kmfederatedsearchconfig_modifiedby";
            [Relationship("msdyn_kmfederatedsearchconfig", EntityRole.Referenced, "lk_msdyn_kmfederatedsearchconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_kmfederatedsearchconfig_modifiedonbehalfby = "lk_msdyn_kmfederatedsearchconfig_modifiedonbehalfby";
            [Relationship("msdyn_kmpersonalizationsetting", EntityRole.Referenced, "lk_msdyn_kmpersonalizationsetting_createdby", "createdby")]
            public const string lk_msdyn_kmpersonalizationsetting_createdby = "lk_msdyn_kmpersonalizationsetting_createdby";
            [Relationship("msdyn_kmpersonalizationsetting", EntityRole.Referenced, "lk_msdyn_kmpersonalizationsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_kmpersonalizationsetting_createdonbehalfby = "lk_msdyn_kmpersonalizationsetting_createdonbehalfby";
            [Relationship("msdyn_kmpersonalizationsetting", EntityRole.Referenced, "lk_msdyn_kmpersonalizationsetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_kmpersonalizationsetting_modifiedby = "lk_msdyn_kmpersonalizationsetting_modifiedby";
            [Relationship("msdyn_kmpersonalizationsetting", EntityRole.Referenced, "lk_msdyn_kmpersonalizationsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_kmpersonalizationsetting_modifiedonbehalfby = "lk_msdyn_kmpersonalizationsetting_modifiedonbehalfby";
            [Relationship("msdyn_knowledgearticleimage", EntityRole.Referenced, "lk_msdyn_knowledgearticleimage_createdby", "createdby")]
            public const string lk_msdyn_knowledgearticleimage_createdby = "lk_msdyn_knowledgearticleimage_createdby";
            [Relationship("msdyn_knowledgearticleimage", EntityRole.Referenced, "lk_msdyn_knowledgearticleimage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_knowledgearticleimage_createdonbehalfby = "lk_msdyn_knowledgearticleimage_createdonbehalfby";
            [Relationship("msdyn_knowledgearticleimage", EntityRole.Referenced, "lk_msdyn_knowledgearticleimage_modifiedby", "modifiedby")]
            public const string lk_msdyn_knowledgearticleimage_modifiedby = "lk_msdyn_knowledgearticleimage_modifiedby";
            [Relationship("msdyn_knowledgearticleimage", EntityRole.Referenced, "lk_msdyn_knowledgearticleimage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_knowledgearticleimage_modifiedonbehalfby = "lk_msdyn_knowledgearticleimage_modifiedonbehalfby";
            [Relationship("msdyn_knowledgearticletemplate", EntityRole.Referenced, "lk_msdyn_knowledgearticletemplate_createdby", "createdby")]
            public const string lk_msdyn_knowledgearticletemplate_createdby = "lk_msdyn_knowledgearticletemplate_createdby";
            [Relationship("msdyn_knowledgearticletemplate", EntityRole.Referenced, "lk_msdyn_knowledgearticletemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_knowledgearticletemplate_createdonbehalfby = "lk_msdyn_knowledgearticletemplate_createdonbehalfby";
            [Relationship("msdyn_knowledgearticletemplate", EntityRole.Referenced, "lk_msdyn_knowledgearticletemplate_modifiedby", "modifiedby")]
            public const string lk_msdyn_knowledgearticletemplate_modifiedby = "lk_msdyn_knowledgearticletemplate_modifiedby";
            [Relationship("msdyn_knowledgearticletemplate", EntityRole.Referenced, "lk_msdyn_knowledgearticletemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_knowledgearticletemplate_modifiedonbehalfby = "lk_msdyn_knowledgearticletemplate_modifiedonbehalfby";
            [Relationship("msdyn_knowledgeinteractioninsight", EntityRole.Referenced, "lk_msdyn_knowledgeinteractioninsight_createdby", "createdby")]
            public const string lk_msdyn_knowledgeinteractioninsight_createdby = "lk_msdyn_knowledgeinteractioninsight_createdby";
            [Relationship("msdyn_knowledgeinteractioninsight", EntityRole.Referenced, "lk_msdyn_knowledgeinteractioninsight_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_knowledgeinteractioninsight_createdonbehalfby = "lk_msdyn_knowledgeinteractioninsight_createdonbehalfby";
            [Relationship("msdyn_knowledgeinteractioninsight", EntityRole.Referenced, "lk_msdyn_knowledgeinteractioninsight_modifiedby", "modifiedby")]
            public const string lk_msdyn_knowledgeinteractioninsight_modifiedby = "lk_msdyn_knowledgeinteractioninsight_modifiedby";
            [Relationship("msdyn_knowledgeinteractioninsight", EntityRole.Referenced, "lk_msdyn_knowledgeinteractioninsight_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_knowledgeinteractioninsight_modifiedonbehalfby = "lk_msdyn_knowledgeinteractioninsight_modifiedonbehalfby";
            [Relationship("msdyn_knowledgemanagementsetting", EntityRole.Referenced, "lk_msdyn_knowledgemanagementsetting_createdby", "createdby")]
            public const string lk_msdyn_knowledgemanagementsetting_createdby = "lk_msdyn_knowledgemanagementsetting_createdby";
            [Relationship("msdyn_knowledgemanagementsetting", EntityRole.Referenced, "lk_msdyn_knowledgemanagementsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_knowledgemanagementsetting_createdonbehalfby = "lk_msdyn_knowledgemanagementsetting_createdonbehalfby";
            [Relationship("msdyn_knowledgemanagementsetting", EntityRole.Referenced, "lk_msdyn_knowledgemanagementsetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_knowledgemanagementsetting_modifiedby = "lk_msdyn_knowledgemanagementsetting_modifiedby";
            [Relationship("msdyn_knowledgemanagementsetting", EntityRole.Referenced, "lk_msdyn_knowledgemanagementsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_knowledgemanagementsetting_modifiedonbehalfby = "lk_msdyn_knowledgemanagementsetting_modifiedonbehalfby";
            [Relationship("msdyn_knowledgepersonalfilter", EntityRole.Referenced, "lk_msdyn_knowledgepersonalfilter_createdby", "createdby")]
            public const string lk_msdyn_knowledgepersonalfilter_createdby = "lk_msdyn_knowledgepersonalfilter_createdby";
            [Relationship("msdyn_knowledgepersonalfilter", EntityRole.Referenced, "lk_msdyn_knowledgepersonalfilter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_knowledgepersonalfilter_createdonbehalfby = "lk_msdyn_knowledgepersonalfilter_createdonbehalfby";
            [Relationship("msdyn_knowledgepersonalfilter", EntityRole.Referenced, "lk_msdyn_knowledgepersonalfilter_modifiedby", "modifiedby")]
            public const string lk_msdyn_knowledgepersonalfilter_modifiedby = "lk_msdyn_knowledgepersonalfilter_modifiedby";
            [Relationship("msdyn_knowledgepersonalfilter", EntityRole.Referenced, "lk_msdyn_knowledgepersonalfilter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_knowledgepersonalfilter_modifiedonbehalfby = "lk_msdyn_knowledgepersonalfilter_modifiedonbehalfby";
            [Relationship("msdyn_knowledgesearchfilter", EntityRole.Referenced, "lk_msdyn_knowledgesearchfilter_createdby", "createdby")]
            public const string lk_msdyn_knowledgesearchfilter_createdby = "lk_msdyn_knowledgesearchfilter_createdby";
            [Relationship("msdyn_knowledgesearchfilter", EntityRole.Referenced, "lk_msdyn_knowledgesearchfilter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_knowledgesearchfilter_createdonbehalfby = "lk_msdyn_knowledgesearchfilter_createdonbehalfby";
            [Relationship("msdyn_knowledgesearchfilter", EntityRole.Referenced, "lk_msdyn_knowledgesearchfilter_modifiedby", "modifiedby")]
            public const string lk_msdyn_knowledgesearchfilter_modifiedby = "lk_msdyn_knowledgesearchfilter_modifiedby";
            [Relationship("msdyn_knowledgesearchfilter", EntityRole.Referenced, "lk_msdyn_knowledgesearchfilter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_knowledgesearchfilter_modifiedonbehalfby = "lk_msdyn_knowledgesearchfilter_modifiedonbehalfby";
            [Relationship("msdyn_knowledgesearchinsight", EntityRole.Referenced, "lk_msdyn_knowledgesearchinsight_createdby", "createdby")]
            public const string lk_msdyn_knowledgesearchinsight_createdby = "lk_msdyn_knowledgesearchinsight_createdby";
            [Relationship("msdyn_knowledgesearchinsight", EntityRole.Referenced, "lk_msdyn_knowledgesearchinsight_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_knowledgesearchinsight_createdonbehalfby = "lk_msdyn_knowledgesearchinsight_createdonbehalfby";
            [Relationship("msdyn_knowledgesearchinsight", EntityRole.Referenced, "lk_msdyn_knowledgesearchinsight_modifiedby", "modifiedby")]
            public const string lk_msdyn_knowledgesearchinsight_modifiedby = "lk_msdyn_knowledgesearchinsight_modifiedby";
            [Relationship("msdyn_knowledgesearchinsight", EntityRole.Referenced, "lk_msdyn_knowledgesearchinsight_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_knowledgesearchinsight_modifiedonbehalfby = "lk_msdyn_knowledgesearchinsight_modifiedonbehalfby";
            [Relationship("msdyn_kpieventdata", EntityRole.Referenced, "lk_msdyn_kpieventdata_createdby", "createdby")]
            public const string lk_msdyn_kpieventdata_createdby = "lk_msdyn_kpieventdata_createdby";
            [Relationship("msdyn_kpieventdata", EntityRole.Referenced, "lk_msdyn_kpieventdata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_kpieventdata_createdonbehalfby = "lk_msdyn_kpieventdata_createdonbehalfby";
            [Relationship("msdyn_kpieventdata", EntityRole.Referenced, "lk_msdyn_kpieventdata_modifiedby", "modifiedby")]
            public const string lk_msdyn_kpieventdata_modifiedby = "lk_msdyn_kpieventdata_modifiedby";
            [Relationship("msdyn_kpieventdata", EntityRole.Referenced, "lk_msdyn_kpieventdata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_kpieventdata_modifiedonbehalfby = "lk_msdyn_kpieventdata_modifiedonbehalfby";
            [Relationship("msdyn_kpieventdefinition", EntityRole.Referenced, "lk_msdyn_kpieventdefinition_createdby", "createdby")]
            public const string lk_msdyn_kpieventdefinition_createdby = "lk_msdyn_kpieventdefinition_createdby";
            [Relationship("msdyn_kpieventdefinition", EntityRole.Referenced, "lk_msdyn_kpieventdefinition_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_kpieventdefinition_createdonbehalfby = "lk_msdyn_kpieventdefinition_createdonbehalfby";
            [Relationship("msdyn_kpieventdefinition", EntityRole.Referenced, "lk_msdyn_kpieventdefinition_modifiedby", "modifiedby")]
            public const string lk_msdyn_kpieventdefinition_modifiedby = "lk_msdyn_kpieventdefinition_modifiedby";
            [Relationship("msdyn_kpieventdefinition", EntityRole.Referenced, "lk_msdyn_kpieventdefinition_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_kpieventdefinition_modifiedonbehalfby = "lk_msdyn_kpieventdefinition_modifiedonbehalfby";
            [Relationship("msdyn_leadhygienesetting", EntityRole.Referenced, "lk_msdyn_leadhygienesetting_createdby", "createdby")]
            public const string lk_msdyn_leadhygienesetting_createdby = "lk_msdyn_leadhygienesetting_createdby";
            [Relationship("msdyn_leadhygienesetting", EntityRole.Referenced, "lk_msdyn_leadhygienesetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_leadhygienesetting_createdonbehalfby = "lk_msdyn_leadhygienesetting_createdonbehalfby";
            [Relationship("msdyn_leadhygienesetting", EntityRole.Referenced, "lk_msdyn_leadhygienesetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_leadhygienesetting_modifiedby = "lk_msdyn_leadhygienesetting_modifiedby";
            [Relationship("msdyn_leadhygienesetting", EntityRole.Referenced, "lk_msdyn_leadhygienesetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_leadhygienesetting_modifiedonbehalfby = "lk_msdyn_leadhygienesetting_modifiedonbehalfby";
            [Relationship("msdyn_leadkpiitem", EntityRole.Referenced, "lk_msdyn_leadkpiitem_createdby", "createdby")]
            public const string lk_msdyn_leadkpiitem_createdby = "lk_msdyn_leadkpiitem_createdby";
            [Relationship("msdyn_leadkpiitem", EntityRole.Referenced, "lk_msdyn_leadkpiitem_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_leadkpiitem_createdonbehalfby = "lk_msdyn_leadkpiitem_createdonbehalfby";
            [Relationship("msdyn_leadkpiitem", EntityRole.Referenced, "lk_msdyn_leadkpiitem_modifiedby", "modifiedby")]
            public const string lk_msdyn_leadkpiitem_modifiedby = "lk_msdyn_leadkpiitem_modifiedby";
            [Relationship("msdyn_leadkpiitem", EntityRole.Referenced, "lk_msdyn_leadkpiitem_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_leadkpiitem_modifiedonbehalfby = "lk_msdyn_leadkpiitem_modifiedonbehalfby";
            [Relationship("msdyn_leadmodelconfig", EntityRole.Referenced, "lk_msdyn_leadmodelconfig_createdby", "createdby")]
            public const string lk_msdyn_leadmodelconfig_createdby = "lk_msdyn_leadmodelconfig_createdby";
            [Relationship("msdyn_leadmodelconfig", EntityRole.Referenced, "lk_msdyn_leadmodelconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_leadmodelconfig_createdonbehalfby = "lk_msdyn_leadmodelconfig_createdonbehalfby";
            [Relationship("msdyn_leadmodelconfig", EntityRole.Referenced, "lk_msdyn_leadmodelconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_leadmodelconfig_modifiedby = "lk_msdyn_leadmodelconfig_modifiedby";
            [Relationship("msdyn_leadmodelconfig", EntityRole.Referenced, "lk_msdyn_leadmodelconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_leadmodelconfig_modifiedonbehalfby = "lk_msdyn_leadmodelconfig_modifiedonbehalfby";
            [Relationship("msdyn_macrosession", EntityRole.Referenced, "lk_msdyn_macrosession_createdby", "createdby")]
            public const string lk_msdyn_macrosession_createdby = "lk_msdyn_macrosession_createdby";
            [Relationship("msdyn_macrosession", EntityRole.Referenced, "lk_msdyn_macrosession_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_macrosession_createdonbehalfby = "lk_msdyn_macrosession_createdonbehalfby";
            [Relationship("msdyn_macrosession", EntityRole.Referenced, "lk_msdyn_macrosession_modifiedby", "modifiedby")]
            public const string lk_msdyn_macrosession_modifiedby = "lk_msdyn_macrosession_modifiedby";
            [Relationship("msdyn_macrosession", EntityRole.Referenced, "lk_msdyn_macrosession_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_macrosession_modifiedonbehalfby = "lk_msdyn_macrosession_modifiedonbehalfby";
            [Relationship("msdyn_migrationtracker", EntityRole.Referenced, "lk_msdyn_migrationtracker_createdby", "createdby")]
            public const string lk_msdyn_migrationtracker_createdby = "lk_msdyn_migrationtracker_createdby";
            [Relationship("msdyn_migrationtracker", EntityRole.Referenced, "lk_msdyn_migrationtracker_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_migrationtracker_createdonbehalfby = "lk_msdyn_migrationtracker_createdonbehalfby";
            [Relationship("msdyn_migrationtracker", EntityRole.Referenced, "lk_msdyn_migrationtracker_modifiedby", "modifiedby")]
            public const string lk_msdyn_migrationtracker_modifiedby = "lk_msdyn_migrationtracker_modifiedby";
            [Relationship("msdyn_migrationtracker", EntityRole.Referenced, "lk_msdyn_migrationtracker_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_migrationtracker_modifiedonbehalfby = "lk_msdyn_migrationtracker_modifiedonbehalfby";
            [Relationship("msdyn_modelpreviewstatus", EntityRole.Referenced, "lk_msdyn_modelpreviewstatus_createdby", "createdby")]
            public const string lk_msdyn_modelpreviewstatus_createdby = "lk_msdyn_modelpreviewstatus_createdby";
            [Relationship("msdyn_modelpreviewstatus", EntityRole.Referenced, "lk_msdyn_modelpreviewstatus_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_modelpreviewstatus_createdonbehalfby = "lk_msdyn_modelpreviewstatus_createdonbehalfby";
            [Relationship("msdyn_modelpreviewstatus", EntityRole.Referenced, "lk_msdyn_modelpreviewstatus_modifiedby", "modifiedby")]
            public const string lk_msdyn_modelpreviewstatus_modifiedby = "lk_msdyn_modelpreviewstatus_modifiedby";
            [Relationship("msdyn_modelpreviewstatus", EntityRole.Referenced, "lk_msdyn_modelpreviewstatus_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_modelpreviewstatus_modifiedonbehalfby = "lk_msdyn_modelpreviewstatus_modifiedonbehalfby";
            [Relationship("msdyn_mostcontacted", EntityRole.Referenced, "lk_msdyn_mostcontacted_createdby", "createdby")]
            public const string lk_msdyn_mostcontacted_createdby = "lk_msdyn_mostcontacted_createdby";
            [Relationship("msdyn_mostcontacted", EntityRole.Referenced, "lk_msdyn_mostcontacted_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_mostcontacted_createdonbehalfby = "lk_msdyn_mostcontacted_createdonbehalfby";
            [Relationship("msdyn_mostcontacted", EntityRole.Referenced, "lk_msdyn_mostcontacted_modifiedby", "modifiedby")]
            public const string lk_msdyn_mostcontacted_modifiedby = "lk_msdyn_mostcontacted_modifiedby";
            [Relationship("msdyn_mostcontacted", EntityRole.Referenced, "lk_msdyn_mostcontacted_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_mostcontacted_modifiedonbehalfby = "lk_msdyn_mostcontacted_modifiedonbehalfby";
            [Relationship("msdyn_mostcontactedby", EntityRole.Referenced, "lk_msdyn_mostcontactedby_createdby", "createdby")]
            public const string lk_msdyn_mostcontactedby_createdby = "lk_msdyn_mostcontactedby_createdby";
            [Relationship("msdyn_mostcontactedby", EntityRole.Referenced, "lk_msdyn_mostcontactedby_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_mostcontactedby_createdonbehalfby = "lk_msdyn_mostcontactedby_createdonbehalfby";
            [Relationship("msdyn_mostcontactedby", EntityRole.Referenced, "lk_msdyn_mostcontactedby_modifiedby", "modifiedby")]
            public const string lk_msdyn_mostcontactedby_modifiedby = "lk_msdyn_mostcontactedby_modifiedby";
            [Relationship("msdyn_mostcontactedby", EntityRole.Referenced, "lk_msdyn_mostcontactedby_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_mostcontactedby_modifiedonbehalfby = "lk_msdyn_mostcontactedby_modifiedonbehalfby";
            [Relationship("msdyn_msteamssetting", EntityRole.Referenced, "lk_msdyn_msteamssetting_createdby", "createdby")]
            public const string lk_msdyn_msteamssetting_createdby = "lk_msdyn_msteamssetting_createdby";
            [Relationship("msdyn_msteamssetting", EntityRole.Referenced, "lk_msdyn_msteamssetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_msteamssetting_createdonbehalfby = "lk_msdyn_msteamssetting_createdonbehalfby";
            [Relationship("msdyn_msteamssetting", EntityRole.Referenced, "lk_msdyn_msteamssetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_msteamssetting_modifiedby = "lk_msdyn_msteamssetting_modifiedby";
            [Relationship("msdyn_msteamssetting", EntityRole.Referenced, "lk_msdyn_msteamssetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_msteamssetting_modifiedonbehalfby = "lk_msdyn_msteamssetting_modifiedonbehalfby";
            [Relationship("msdyn_msteamssettingsv2", EntityRole.Referenced, "lk_msdyn_msteamssettingsv2_createdby", "createdby")]
            public const string lk_msdyn_msteamssettingsv2_createdby = "lk_msdyn_msteamssettingsv2_createdby";
            [Relationship("msdyn_msteamssettingsv2", EntityRole.Referenced, "lk_msdyn_msteamssettingsv2_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_msteamssettingsv2_createdonbehalfby = "lk_msdyn_msteamssettingsv2_createdonbehalfby";
            [Relationship("msdyn_msteamssettingsv2", EntityRole.Referenced, "lk_msdyn_msteamssettingsv2_modifiedby", "modifiedby")]
            public const string lk_msdyn_msteamssettingsv2_modifiedby = "lk_msdyn_msteamssettingsv2_modifiedby";
            [Relationship("msdyn_msteamssettingsv2", EntityRole.Referenced, "lk_msdyn_msteamssettingsv2_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_msteamssettingsv2_modifiedonbehalfby = "lk_msdyn_msteamssettingsv2_modifiedonbehalfby";
            [Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "lk_msdyn_notesanalysisconfig_createdby", "createdby")]
            public const string lk_msdyn_notesanalysisconfig_createdby = "lk_msdyn_notesanalysisconfig_createdby";
            [Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "lk_msdyn_notesanalysisconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_notesanalysisconfig_createdonbehalfby = "lk_msdyn_notesanalysisconfig_createdonbehalfby";
            [Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "lk_msdyn_notesanalysisconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_notesanalysisconfig_modifiedby = "lk_msdyn_notesanalysisconfig_modifiedby";
            [Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "lk_msdyn_notesanalysisconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_notesanalysisconfig_modifiedonbehalfby = "lk_msdyn_notesanalysisconfig_modifiedonbehalfby";
            [Relationship("msdyn_notificationfield", EntityRole.Referenced, "lk_msdyn_notificationfield_createdby", "createdby")]
            public const string lk_msdyn_notificationfield_createdby = "lk_msdyn_notificationfield_createdby";
            [Relationship("msdyn_notificationfield", EntityRole.Referenced, "lk_msdyn_notificationfield_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_notificationfield_createdonbehalfby = "lk_msdyn_notificationfield_createdonbehalfby";
            [Relationship("msdyn_notificationfield", EntityRole.Referenced, "lk_msdyn_notificationfield_modifiedby", "modifiedby")]
            public const string lk_msdyn_notificationfield_modifiedby = "lk_msdyn_notificationfield_modifiedby";
            [Relationship("msdyn_notificationfield", EntityRole.Referenced, "lk_msdyn_notificationfield_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_notificationfield_modifiedonbehalfby = "lk_msdyn_notificationfield_modifiedonbehalfby";
            [Relationship("msdyn_notificationtemplate", EntityRole.Referenced, "lk_msdyn_notificationtemplate_createdby", "createdby")]
            public const string lk_msdyn_notificationtemplate_createdby = "lk_msdyn_notificationtemplate_createdby";
            [Relationship("msdyn_notificationtemplate", EntityRole.Referenced, "lk_msdyn_notificationtemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_notificationtemplate_createdonbehalfby = "lk_msdyn_notificationtemplate_createdonbehalfby";
            [Relationship("msdyn_notificationtemplate", EntityRole.Referenced, "lk_msdyn_notificationtemplate_modifiedby", "modifiedby")]
            public const string lk_msdyn_notificationtemplate_modifiedby = "lk_msdyn_notificationtemplate_modifiedby";
            [Relationship("msdyn_notificationtemplate", EntityRole.Referenced, "lk_msdyn_notificationtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_notificationtemplate_modifiedonbehalfby = "lk_msdyn_notificationtemplate_modifiedonbehalfby";
            [Relationship("msdyn_opportunitykpiitem", EntityRole.Referenced, "lk_msdyn_opportunitykpiitem_createdby", "createdby")]
            public const string lk_msdyn_opportunitykpiitem_createdby = "lk_msdyn_opportunitykpiitem_createdby";
            [Relationship("msdyn_opportunitykpiitem", EntityRole.Referenced, "lk_msdyn_opportunitykpiitem_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_opportunitykpiitem_createdonbehalfby = "lk_msdyn_opportunitykpiitem_createdonbehalfby";
            [Relationship("msdyn_opportunitykpiitem", EntityRole.Referenced, "lk_msdyn_opportunitykpiitem_modifiedby", "modifiedby")]
            public const string lk_msdyn_opportunitykpiitem_modifiedby = "lk_msdyn_opportunitykpiitem_modifiedby";
            [Relationship("msdyn_opportunitykpiitem", EntityRole.Referenced, "lk_msdyn_opportunitykpiitem_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_opportunitykpiitem_modifiedonbehalfby = "lk_msdyn_opportunitykpiitem_modifiedonbehalfby";
            [Relationship("msdyn_opportunitymodelconfig", EntityRole.Referenced, "lk_msdyn_opportunitymodelconfig_createdby", "createdby")]
            public const string lk_msdyn_opportunitymodelconfig_createdby = "lk_msdyn_opportunitymodelconfig_createdby";
            [Relationship("msdyn_opportunitymodelconfig", EntityRole.Referenced, "lk_msdyn_opportunitymodelconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_opportunitymodelconfig_createdonbehalfby = "lk_msdyn_opportunitymodelconfig_createdonbehalfby";
            [Relationship("msdyn_opportunitymodelconfig", EntityRole.Referenced, "lk_msdyn_opportunitymodelconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_opportunitymodelconfig_modifiedby = "lk_msdyn_opportunitymodelconfig_modifiedby";
            [Relationship("msdyn_opportunitymodelconfig", EntityRole.Referenced, "lk_msdyn_opportunitymodelconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_opportunitymodelconfig_modifiedonbehalfby = "lk_msdyn_opportunitymodelconfig_modifiedonbehalfby";
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
            [Relationship("msdyn_organizationalunit", EntityRole.Referenced, "lk_msdyn_organizationalunit_createdby", "createdby")]
            public const string lk_msdyn_organizationalunit_createdby = "lk_msdyn_organizationalunit_createdby";
            [Relationship("msdyn_organizationalunit", EntityRole.Referenced, "lk_msdyn_organizationalunit_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_organizationalunit_createdonbehalfby = "lk_msdyn_organizationalunit_createdonbehalfby";
            [Relationship("msdyn_organizationalunit", EntityRole.Referenced, "lk_msdyn_organizationalunit_modifiedby", "modifiedby")]
            public const string lk_msdyn_organizationalunit_modifiedby = "lk_msdyn_organizationalunit_modifiedby";
            [Relationship("msdyn_organizationalunit", EntityRole.Referenced, "lk_msdyn_organizationalunit_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_organizationalunit_modifiedonbehalfby = "lk_msdyn_organizationalunit_modifiedonbehalfby";
            [Relationship("msdyn_paneconfiguration", EntityRole.Referenced, "lk_msdyn_paneconfiguration_createdby", "createdby")]
            public const string lk_msdyn_paneconfiguration_createdby = "lk_msdyn_paneconfiguration_createdby";
            [Relationship("msdyn_paneconfiguration", EntityRole.Referenced, "lk_msdyn_paneconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_paneconfiguration_createdonbehalfby = "lk_msdyn_paneconfiguration_createdonbehalfby";
            [Relationship("msdyn_paneconfiguration", EntityRole.Referenced, "lk_msdyn_paneconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_paneconfiguration_modifiedby = "lk_msdyn_paneconfiguration_modifiedby";
            [Relationship("msdyn_paneconfiguration", EntityRole.Referenced, "lk_msdyn_paneconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_paneconfiguration_modifiedonbehalfby = "lk_msdyn_paneconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_panetabconfiguration", EntityRole.Referenced, "lk_msdyn_panetabconfiguration_createdby", "createdby")]
            public const string lk_msdyn_panetabconfiguration_createdby = "lk_msdyn_panetabconfiguration_createdby";
            [Relationship("msdyn_panetabconfiguration", EntityRole.Referenced, "lk_msdyn_panetabconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_panetabconfiguration_createdonbehalfby = "lk_msdyn_panetabconfiguration_createdonbehalfby";
            [Relationship("msdyn_panetabconfiguration", EntityRole.Referenced, "lk_msdyn_panetabconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_panetabconfiguration_modifiedby = "lk_msdyn_panetabconfiguration_modifiedby";
            [Relationship("msdyn_panetabconfiguration", EntityRole.Referenced, "lk_msdyn_panetabconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_panetabconfiguration_modifiedonbehalfby = "lk_msdyn_panetabconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_panetoolconfiguration", EntityRole.Referenced, "lk_msdyn_panetoolconfiguration_createdby", "createdby")]
            public const string lk_msdyn_panetoolconfiguration_createdby = "lk_msdyn_panetoolconfiguration_createdby";
            [Relationship("msdyn_panetoolconfiguration", EntityRole.Referenced, "lk_msdyn_panetoolconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_panetoolconfiguration_createdonbehalfby = "lk_msdyn_panetoolconfiguration_createdonbehalfby";
            [Relationship("msdyn_panetoolconfiguration", EntityRole.Referenced, "lk_msdyn_panetoolconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_panetoolconfiguration_modifiedby = "lk_msdyn_panetoolconfiguration_modifiedby";
            [Relationship("msdyn_panetoolconfiguration", EntityRole.Referenced, "lk_msdyn_panetoolconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_panetoolconfiguration_modifiedonbehalfby = "lk_msdyn_panetoolconfiguration_modifiedonbehalfby";
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
            [Relationship("msdyn_pminferredtask", EntityRole.Referenced, "lk_msdyn_pminferredtask_createdby", "createdby")]
            public const string lk_msdyn_pminferredtask_createdby = "lk_msdyn_pminferredtask_createdby";
            [Relationship("msdyn_pminferredtask", EntityRole.Referenced, "lk_msdyn_pminferredtask_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_pminferredtask_createdonbehalfby = "lk_msdyn_pminferredtask_createdonbehalfby";
            [Relationship("msdyn_pminferredtask", EntityRole.Referenced, "lk_msdyn_pminferredtask_modifiedby", "modifiedby")]
            public const string lk_msdyn_pminferredtask_modifiedby = "lk_msdyn_pminferredtask_modifiedby";
            [Relationship("msdyn_pminferredtask", EntityRole.Referenced, "lk_msdyn_pminferredtask_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_pminferredtask_modifiedonbehalfby = "lk_msdyn_pminferredtask_modifiedonbehalfby";
            [Relationship("msdyn_pmrecording", EntityRole.Referenced, "lk_msdyn_pmrecording_createdby", "createdby")]
            public const string lk_msdyn_pmrecording_createdby = "lk_msdyn_pmrecording_createdby";
            [Relationship("msdyn_pmrecording", EntityRole.Referenced, "lk_msdyn_pmrecording_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_pmrecording_createdonbehalfby = "lk_msdyn_pmrecording_createdonbehalfby";
            [Relationship("msdyn_pmrecording", EntityRole.Referenced, "lk_msdyn_pmrecording_modifiedby", "modifiedby")]
            public const string lk_msdyn_pmrecording_modifiedby = "lk_msdyn_pmrecording_modifiedby";
            [Relationship("msdyn_pmrecording", EntityRole.Referenced, "lk_msdyn_pmrecording_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_pmrecording_modifiedonbehalfby = "lk_msdyn_pmrecording_modifiedonbehalfby";
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
            [Relationship("msdyn_predictivemodelscore", EntityRole.Referenced, "lk_msdyn_predictivemodelscore_createdby", "createdby")]
            public const string lk_msdyn_predictivemodelscore_createdby = "lk_msdyn_predictivemodelscore_createdby";
            [Relationship("msdyn_predictivemodelscore", EntityRole.Referenced, "lk_msdyn_predictivemodelscore_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_predictivemodelscore_createdonbehalfby = "lk_msdyn_predictivemodelscore_createdonbehalfby";
            [Relationship("msdyn_predictivemodelscore", EntityRole.Referenced, "lk_msdyn_predictivemodelscore_modifiedby", "modifiedby")]
            public const string lk_msdyn_predictivemodelscore_modifiedby = "lk_msdyn_predictivemodelscore_modifiedby";
            [Relationship("msdyn_predictivemodelscore", EntityRole.Referenced, "lk_msdyn_predictivemodelscore_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_predictivemodelscore_modifiedonbehalfby = "lk_msdyn_predictivemodelscore_modifiedonbehalfby";
            [Relationship("msdyn_predictivescore", EntityRole.Referenced, "lk_msdyn_predictivescore_createdby", "createdby")]
            public const string lk_msdyn_predictivescore_createdby = "lk_msdyn_predictivescore_createdby";
            [Relationship("msdyn_predictivescore", EntityRole.Referenced, "lk_msdyn_predictivescore_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_predictivescore_createdonbehalfby = "lk_msdyn_predictivescore_createdonbehalfby";
            [Relationship("msdyn_predictivescore", EntityRole.Referenced, "lk_msdyn_predictivescore_modifiedby", "modifiedby")]
            public const string lk_msdyn_predictivescore_modifiedby = "lk_msdyn_predictivescore_modifiedby";
            [Relationship("msdyn_predictivescore", EntityRole.Referenced, "lk_msdyn_predictivescore_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_predictivescore_modifiedonbehalfby = "lk_msdyn_predictivescore_modifiedonbehalfby";
            [Relationship("msdyn_predictworkhourdurationsetting", EntityRole.Referenced, "lk_msdyn_predictworkhourdurationsetting_createdby", "createdby")]
            public const string lk_msdyn_predictworkhourdurationsetting_createdby = "lk_msdyn_predictworkhourdurationsetting_createdby";
            [Relationship("msdyn_predictworkhourdurationsetting", EntityRole.Referenced, "lk_msdyn_predictworkhourdurationsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_predictworkhourdurationsetting_createdonbehalfby = "lk_msdyn_predictworkhourdurationsetting_createdonbehalfby";
            [Relationship("msdyn_predictworkhourdurationsetting", EntityRole.Referenced, "lk_msdyn_predictworkhourdurationsetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_predictworkhourdurationsetting_modifiedby = "lk_msdyn_predictworkhourdurationsetting_modifiedby";
            [Relationship("msdyn_predictworkhourdurationsetting", EntityRole.Referenced, "lk_msdyn_predictworkhourdurationsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_predictworkhourdurationsetting_modifiedonbehalfby = "lk_msdyn_predictworkhourdurationsetting_modifiedonbehalfby";
            [Relationship("msdyn_priority", EntityRole.Referenced, "lk_msdyn_priority_createdby", "createdby")]
            public const string lk_msdyn_priority_createdby = "lk_msdyn_priority_createdby";
            [Relationship("msdyn_priority", EntityRole.Referenced, "lk_msdyn_priority_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_priority_createdonbehalfby = "lk_msdyn_priority_createdonbehalfby";
            [Relationship("msdyn_priority", EntityRole.Referenced, "lk_msdyn_priority_modifiedby", "modifiedby")]
            public const string lk_msdyn_priority_modifiedby = "lk_msdyn_priority_modifiedby";
            [Relationship("msdyn_priority", EntityRole.Referenced, "lk_msdyn_priority_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_priority_modifiedonbehalfby = "lk_msdyn_priority_modifiedonbehalfby";
            [Relationship("msdyn_problematicasset", EntityRole.Referenced, "lk_msdyn_problematicasset_createdby", "createdby")]
            public const string lk_msdyn_problematicasset_createdby = "lk_msdyn_problematicasset_createdby";
            [Relationship("msdyn_problematicasset", EntityRole.Referenced, "lk_msdyn_problematicasset_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_problematicasset_createdonbehalfby = "lk_msdyn_problematicasset_createdonbehalfby";
            [Relationship("msdyn_problematicasset", EntityRole.Referenced, "lk_msdyn_problematicasset_modifiedby", "modifiedby")]
            public const string lk_msdyn_problematicasset_modifiedby = "lk_msdyn_problematicasset_modifiedby";
            [Relationship("msdyn_problematicasset", EntityRole.Referenced, "lk_msdyn_problematicasset_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_problematicasset_modifiedonbehalfby = "lk_msdyn_problematicasset_modifiedonbehalfby";
            [Relationship("msdyn_problematicassetfeedback", EntityRole.Referenced, "lk_msdyn_problematicassetfeedback_createdby", "createdby")]
            public const string lk_msdyn_problematicassetfeedback_createdby = "lk_msdyn_problematicassetfeedback_createdby";
            [Relationship("msdyn_problematicassetfeedback", EntityRole.Referenced, "lk_msdyn_problematicassetfeedback_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_problematicassetfeedback_createdonbehalfby = "lk_msdyn_problematicassetfeedback_createdonbehalfby";
            [Relationship("msdyn_problematicassetfeedback", EntityRole.Referenced, "lk_msdyn_problematicassetfeedback_modifiedby", "modifiedby")]
            public const string lk_msdyn_problematicassetfeedback_modifiedby = "lk_msdyn_problematicassetfeedback_modifiedby";
            [Relationship("msdyn_problematicassetfeedback", EntityRole.Referenced, "lk_msdyn_problematicassetfeedback_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_problematicassetfeedback_modifiedonbehalfby = "lk_msdyn_problematicassetfeedback_modifiedonbehalfby";
            [Relationship("msdyn_productinventory", EntityRole.Referenced, "lk_msdyn_productinventory_createdby", "createdby")]
            public const string lk_msdyn_productinventory_createdby = "lk_msdyn_productinventory_createdby";
            [Relationship("msdyn_productinventory", EntityRole.Referenced, "lk_msdyn_productinventory_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productinventory_createdonbehalfby = "lk_msdyn_productinventory_createdonbehalfby";
            [Relationship("msdyn_productinventory", EntityRole.Referenced, "lk_msdyn_productinventory_modifiedby", "modifiedby")]
            public const string lk_msdyn_productinventory_modifiedby = "lk_msdyn_productinventory_modifiedby";
            [Relationship("msdyn_productinventory", EntityRole.Referenced, "lk_msdyn_productinventory_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productinventory_modifiedonbehalfby = "lk_msdyn_productinventory_modifiedonbehalfby";
            [Relationship("msdyn_productivityactioninputparameter", EntityRole.Referenced, "lk_msdyn_productivityactioninputparameter_createdby", "createdby")]
            public const string lk_msdyn_productivityactioninputparameter_createdby = "lk_msdyn_productivityactioninputparameter_createdby";
            [Relationship("msdyn_productivityactioninputparameter", EntityRole.Referenced, "lk_msdyn_productivityactioninputparameter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productivityactioninputparameter_createdonbehalfby = "lk_msdyn_productivityactioninputparameter_createdonbehalfby";
            [Relationship("msdyn_productivityactioninputparameter", EntityRole.Referenced, "lk_msdyn_productivityactioninputparameter_modifiedby", "modifiedby")]
            public const string lk_msdyn_productivityactioninputparameter_modifiedby = "lk_msdyn_productivityactioninputparameter_modifiedby";
            [Relationship("msdyn_productivityactioninputparameter", EntityRole.Referenced, "lk_msdyn_productivityactioninputparameter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productivityactioninputparameter_modifiedonbehalfby = "lk_msdyn_productivityactioninputparameter_modifiedonbehalfby";
            [Relationship("msdyn_productivityactionoutputparameter", EntityRole.Referenced, "lk_msdyn_productivityactionoutputparameter_createdby", "createdby")]
            public const string lk_msdyn_productivityactionoutputparameter_createdby = "lk_msdyn_productivityactionoutputparameter_createdby";
            [Relationship("msdyn_productivityactionoutputparameter", EntityRole.Referenced, "lk_msdyn_productivityactionoutputparameter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productivityactionoutputparameter_createdonbehalfby = "lk_msdyn_productivityactionoutputparameter_createdonbehalfby";
            [Relationship("msdyn_productivityactionoutputparameter", EntityRole.Referenced, "lk_msdyn_productivityactionoutputparameter_modifiedby", "modifiedby")]
            public const string lk_msdyn_productivityactionoutputparameter_modifiedby = "lk_msdyn_productivityactionoutputparameter_modifiedby";
            [Relationship("msdyn_productivityactionoutputparameter", EntityRole.Referenced, "lk_msdyn_productivityactionoutputparameter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productivityactionoutputparameter_modifiedonbehalfby = "lk_msdyn_productivityactionoutputparameter_modifiedonbehalfby";
            [Relationship("msdyn_productivityagentscript", EntityRole.Referenced, "lk_msdyn_productivityagentscript_createdby", "createdby")]
            public const string lk_msdyn_productivityagentscript_createdby = "lk_msdyn_productivityagentscript_createdby";
            [Relationship("msdyn_productivityagentscript", EntityRole.Referenced, "lk_msdyn_productivityagentscript_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productivityagentscript_createdonbehalfby = "lk_msdyn_productivityagentscript_createdonbehalfby";
            [Relationship("msdyn_productivityagentscript", EntityRole.Referenced, "lk_msdyn_productivityagentscript_modifiedby", "modifiedby")]
            public const string lk_msdyn_productivityagentscript_modifiedby = "lk_msdyn_productivityagentscript_modifiedby";
            [Relationship("msdyn_productivityagentscript", EntityRole.Referenced, "lk_msdyn_productivityagentscript_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productivityagentscript_modifiedonbehalfby = "lk_msdyn_productivityagentscript_modifiedonbehalfby";
            [Relationship("msdyn_productivityagentscriptstep", EntityRole.Referenced, "lk_msdyn_productivityagentscriptstep_createdby", "createdby")]
            public const string lk_msdyn_productivityagentscriptstep_createdby = "lk_msdyn_productivityagentscriptstep_createdby";
            [Relationship("msdyn_productivityagentscriptstep", EntityRole.Referenced, "lk_msdyn_productivityagentscriptstep_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productivityagentscriptstep_createdonbehalfby = "lk_msdyn_productivityagentscriptstep_createdonbehalfby";
            [Relationship("msdyn_productivityagentscriptstep", EntityRole.Referenced, "lk_msdyn_productivityagentscriptstep_modifiedby", "modifiedby")]
            public const string lk_msdyn_productivityagentscriptstep_modifiedby = "lk_msdyn_productivityagentscriptstep_modifiedby";
            [Relationship("msdyn_productivityagentscriptstep", EntityRole.Referenced, "lk_msdyn_productivityagentscriptstep_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productivityagentscriptstep_modifiedonbehalfby = "lk_msdyn_productivityagentscriptstep_modifiedonbehalfby";
            [Relationship("msdyn_productivitymacroactiontemplate", EntityRole.Referenced, "lk_msdyn_productivitymacroactiontemplate_createdby", "createdby")]
            public const string lk_msdyn_productivitymacroactiontemplate_createdby = "lk_msdyn_productivitymacroactiontemplate_createdby";
            [Relationship("msdyn_productivitymacroactiontemplate", EntityRole.Referenced, "lk_msdyn_productivitymacroactiontemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productivitymacroactiontemplate_createdonbehalfby = "lk_msdyn_productivitymacroactiontemplate_createdonbehalfby";
            [Relationship("msdyn_productivitymacroactiontemplate", EntityRole.Referenced, "lk_msdyn_productivitymacroactiontemplate_modifiedby", "modifiedby")]
            public const string lk_msdyn_productivitymacroactiontemplate_modifiedby = "lk_msdyn_productivitymacroactiontemplate_modifiedby";
            [Relationship("msdyn_productivitymacroactiontemplate", EntityRole.Referenced, "lk_msdyn_productivitymacroactiontemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productivitymacroactiontemplate_modifiedonbehalfby = "lk_msdyn_productivitymacroactiontemplate_modifiedonbehalfby";
            [Relationship("msdyn_productivitymacroconnector", EntityRole.Referenced, "lk_msdyn_productivitymacroconnector_createdby", "createdby")]
            public const string lk_msdyn_productivitymacroconnector_createdby = "lk_msdyn_productivitymacroconnector_createdby";
            [Relationship("msdyn_productivitymacroconnector", EntityRole.Referenced, "lk_msdyn_productivitymacroconnector_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productivitymacroconnector_createdonbehalfby = "lk_msdyn_productivitymacroconnector_createdonbehalfby";
            [Relationship("msdyn_productivitymacroconnector", EntityRole.Referenced, "lk_msdyn_productivitymacroconnector_modifiedby", "modifiedby")]
            public const string lk_msdyn_productivitymacroconnector_modifiedby = "lk_msdyn_productivitymacroconnector_modifiedby";
            [Relationship("msdyn_productivitymacroconnector", EntityRole.Referenced, "lk_msdyn_productivitymacroconnector_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productivitymacroconnector_modifiedonbehalfby = "lk_msdyn_productivitymacroconnector_modifiedonbehalfby";
            [Relationship("msdyn_productivitymacrosolutionconfiguration", EntityRole.Referenced, "lk_msdyn_productivitymacrosolutionconfiguration_createdby", "createdby")]
            public const string lk_msdyn_productivitymacrosolutionconfiguration_createdby = "lk_msdyn_productivitymacrosolutionconfiguration_createdby";
            [Relationship("msdyn_productivitymacrosolutionconfiguration", EntityRole.Referenced, "lk_msdyn_productivitymacrosolutionconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productivitymacrosolutionconfiguration_createdonbehalfby = "lk_msdyn_productivitymacrosolutionconfiguration_createdonbehalfby";
            [Relationship("msdyn_productivitymacrosolutionconfiguration", EntityRole.Referenced, "lk_msdyn_productivitymacrosolutionconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_productivitymacrosolutionconfiguration_modifiedby = "lk_msdyn_productivitymacrosolutionconfiguration_modifiedby";
            [Relationship("msdyn_productivitymacrosolutionconfiguration", EntityRole.Referenced, "lk_msdyn_productivitymacrosolutionconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productivitymacrosolutionconfiguration_modifiedonbehalfby = "lk_msdyn_productivitymacrosolutionconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_productivityparameterdefinition", EntityRole.Referenced, "lk_msdyn_productivityparameterdefinition_createdby", "createdby")]
            public const string lk_msdyn_productivityparameterdefinition_createdby = "lk_msdyn_productivityparameterdefinition_createdby";
            [Relationship("msdyn_productivityparameterdefinition", EntityRole.Referenced, "lk_msdyn_productivityparameterdefinition_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_productivityparameterdefinition_createdonbehalfby = "lk_msdyn_productivityparameterdefinition_createdonbehalfby";
            [Relationship("msdyn_productivityparameterdefinition", EntityRole.Referenced, "lk_msdyn_productivityparameterdefinition_modifiedby", "modifiedby")]
            public const string lk_msdyn_productivityparameterdefinition_modifiedby = "lk_msdyn_productivityparameterdefinition_modifiedby";
            [Relationship("msdyn_productivityparameterdefinition", EntityRole.Referenced, "lk_msdyn_productivityparameterdefinition_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_productivityparameterdefinition_modifiedonbehalfby = "lk_msdyn_productivityparameterdefinition_modifiedonbehalfby";
            [Relationship("msdyn_property", EntityRole.Referenced, "lk_msdyn_property_createdby", "createdby")]
            public const string lk_msdyn_property_createdby = "lk_msdyn_property_createdby";
            [Relationship("msdyn_property", EntityRole.Referenced, "lk_msdyn_property_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_property_createdonbehalfby = "lk_msdyn_property_createdonbehalfby";
            [Relationship("msdyn_property", EntityRole.Referenced, "lk_msdyn_property_modifiedby", "modifiedby")]
            public const string lk_msdyn_property_modifiedby = "lk_msdyn_property_modifiedby";
            [Relationship("msdyn_property", EntityRole.Referenced, "lk_msdyn_property_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_property_modifiedonbehalfby = "lk_msdyn_property_modifiedonbehalfby";
            [Relationship("msdyn_propertyassetassociation", EntityRole.Referenced, "lk_msdyn_propertyassetassociation_createdby", "createdby")]
            public const string lk_msdyn_propertyassetassociation_createdby = "lk_msdyn_propertyassetassociation_createdby";
            [Relationship("msdyn_propertyassetassociation", EntityRole.Referenced, "lk_msdyn_propertyassetassociation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_propertyassetassociation_createdonbehalfby = "lk_msdyn_propertyassetassociation_createdonbehalfby";
            [Relationship("msdyn_propertyassetassociation", EntityRole.Referenced, "lk_msdyn_propertyassetassociation_modifiedby", "modifiedby")]
            public const string lk_msdyn_propertyassetassociation_modifiedby = "lk_msdyn_propertyassetassociation_modifiedby";
            [Relationship("msdyn_propertyassetassociation", EntityRole.Referenced, "lk_msdyn_propertyassetassociation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_propertyassetassociation_modifiedonbehalfby = "lk_msdyn_propertyassetassociation_modifiedonbehalfby";
            [Relationship("msdyn_propertylog", EntityRole.Referenced, "lk_msdyn_propertylog_createdby", "createdby")]
            public const string lk_msdyn_propertylog_createdby = "lk_msdyn_propertylog_createdby";
            [Relationship("msdyn_propertylog", EntityRole.Referenced, "lk_msdyn_propertylog_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_propertylog_createdonbehalfby = "lk_msdyn_propertylog_createdonbehalfby";
            [Relationship("msdyn_propertylog", EntityRole.Referenced, "lk_msdyn_propertylog_modifiedby", "modifiedby")]
            public const string lk_msdyn_propertylog_modifiedby = "lk_msdyn_propertylog_modifiedby";
            [Relationship("msdyn_propertylog", EntityRole.Referenced, "lk_msdyn_propertylog_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_propertylog_modifiedonbehalfby = "lk_msdyn_propertylog_modifiedonbehalfby";
            [Relationship("msdyn_propertytemplateassociation", EntityRole.Referenced, "lk_msdyn_propertytemplateassociation_createdby", "createdby")]
            public const string lk_msdyn_propertytemplateassociation_createdby = "lk_msdyn_propertytemplateassociation_createdby";
            [Relationship("msdyn_propertytemplateassociation", EntityRole.Referenced, "lk_msdyn_propertytemplateassociation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_propertytemplateassociation_createdonbehalfby = "lk_msdyn_propertytemplateassociation_createdonbehalfby";
            [Relationship("msdyn_propertytemplateassociation", EntityRole.Referenced, "lk_msdyn_propertytemplateassociation_modifiedby", "modifiedby")]
            public const string lk_msdyn_propertytemplateassociation_modifiedby = "lk_msdyn_propertytemplateassociation_modifiedby";
            [Relationship("msdyn_propertytemplateassociation", EntityRole.Referenced, "lk_msdyn_propertytemplateassociation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_propertytemplateassociation_modifiedonbehalfby = "lk_msdyn_propertytemplateassociation_modifiedonbehalfby";
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
            [Relationship("msdyn_recording", EntityRole.Referenced, "lk_msdyn_recording_createdby", "createdby")]
            public const string lk_msdyn_recording_createdby = "lk_msdyn_recording_createdby";
            [Relationship("msdyn_recording", EntityRole.Referenced, "lk_msdyn_recording_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_recording_createdonbehalfby = "lk_msdyn_recording_createdonbehalfby";
            [Relationship("msdyn_recording", EntityRole.Referenced, "lk_msdyn_recording_modifiedby", "modifiedby")]
            public const string lk_msdyn_recording_modifiedby = "lk_msdyn_recording_modifiedby";
            [Relationship("msdyn_recording", EntityRole.Referenced, "lk_msdyn_recording_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_recording_modifiedonbehalfby = "lk_msdyn_recording_modifiedonbehalfby";
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
            [Relationship("msdyn_requirementdependency", EntityRole.Referenced, "lk_msdyn_requirementdependency_createdby", "createdby")]
            public const string lk_msdyn_requirementdependency_createdby = "lk_msdyn_requirementdependency_createdby";
            [Relationship("msdyn_requirementdependency", EntityRole.Referenced, "lk_msdyn_requirementdependency_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_requirementdependency_createdonbehalfby = "lk_msdyn_requirementdependency_createdonbehalfby";
            [Relationship("msdyn_requirementdependency", EntityRole.Referenced, "lk_msdyn_requirementdependency_modifiedby", "modifiedby")]
            public const string lk_msdyn_requirementdependency_modifiedby = "lk_msdyn_requirementdependency_modifiedby";
            [Relationship("msdyn_requirementdependency", EntityRole.Referenced, "lk_msdyn_requirementdependency_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_requirementdependency_modifiedonbehalfby = "lk_msdyn_requirementdependency_modifiedonbehalfby";
            [Relationship("msdyn_requirementgroup", EntityRole.Referenced, "lk_msdyn_requirementgroup_createdby", "createdby")]
            public const string lk_msdyn_requirementgroup_createdby = "lk_msdyn_requirementgroup_createdby";
            [Relationship("msdyn_requirementgroup", EntityRole.Referenced, "lk_msdyn_requirementgroup_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_requirementgroup_createdonbehalfby = "lk_msdyn_requirementgroup_createdonbehalfby";
            [Relationship("msdyn_requirementgroup", EntityRole.Referenced, "lk_msdyn_requirementgroup_modifiedby", "modifiedby")]
            public const string lk_msdyn_requirementgroup_modifiedby = "lk_msdyn_requirementgroup_modifiedby";
            [Relationship("msdyn_requirementgroup", EntityRole.Referenced, "lk_msdyn_requirementgroup_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_requirementgroup_modifiedonbehalfby = "lk_msdyn_requirementgroup_modifiedonbehalfby";
            [Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "lk_msdyn_requirementorganizationunit_createdby", "createdby")]
            public const string lk_msdyn_requirementorganizationunit_createdby = "lk_msdyn_requirementorganizationunit_createdby";
            [Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "lk_msdyn_requirementorganizationunit_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_requirementorganizationunit_createdonbehalfby = "lk_msdyn_requirementorganizationunit_createdonbehalfby";
            [Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "lk_msdyn_requirementorganizationunit_modifiedby", "modifiedby")]
            public const string lk_msdyn_requirementorganizationunit_modifiedby = "lk_msdyn_requirementorganizationunit_modifiedby";
            [Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "lk_msdyn_requirementorganizationunit_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_requirementorganizationunit_modifiedonbehalfby = "lk_msdyn_requirementorganizationunit_modifiedonbehalfby";
            [Relationship("msdyn_requirementrelationship", EntityRole.Referenced, "lk_msdyn_requirementrelationship_createdby", "createdby")]
            public const string lk_msdyn_requirementrelationship_createdby = "lk_msdyn_requirementrelationship_createdby";
            [Relationship("msdyn_requirementrelationship", EntityRole.Referenced, "lk_msdyn_requirementrelationship_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_requirementrelationship_createdonbehalfby = "lk_msdyn_requirementrelationship_createdonbehalfby";
            [Relationship("msdyn_requirementrelationship", EntityRole.Referenced, "lk_msdyn_requirementrelationship_modifiedby", "modifiedby")]
            public const string lk_msdyn_requirementrelationship_modifiedby = "lk_msdyn_requirementrelationship_modifiedby";
            [Relationship("msdyn_requirementrelationship", EntityRole.Referenced, "lk_msdyn_requirementrelationship_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_requirementrelationship_modifiedonbehalfby = "lk_msdyn_requirementrelationship_modifiedonbehalfby";
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
            [Relationship("msdyn_resolution", EntityRole.Referenced, "lk_msdyn_resolution_createdby", "createdby")]
            public const string lk_msdyn_resolution_createdby = "lk_msdyn_resolution_createdby";
            [Relationship("msdyn_resolution", EntityRole.Referenced, "lk_msdyn_resolution_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_resolution_createdonbehalfby = "lk_msdyn_resolution_createdonbehalfby";
            [Relationship("msdyn_resolution", EntityRole.Referenced, "lk_msdyn_resolution_modifiedby", "modifiedby")]
            public const string lk_msdyn_resolution_modifiedby = "lk_msdyn_resolution_modifiedby";
            [Relationship("msdyn_resolution", EntityRole.Referenced, "lk_msdyn_resolution_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_resolution_modifiedonbehalfby = "lk_msdyn_resolution_modifiedonbehalfby";
            [Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "lk_msdyn_resourcepaytype_createdby", "createdby")]
            public const string lk_msdyn_resourcepaytype_createdby = "lk_msdyn_resourcepaytype_createdby";
            [Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "lk_msdyn_resourcepaytype_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_resourcepaytype_createdonbehalfby = "lk_msdyn_resourcepaytype_createdonbehalfby";
            [Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "lk_msdyn_resourcepaytype_modifiedby", "modifiedby")]
            public const string lk_msdyn_resourcepaytype_modifiedby = "lk_msdyn_resourcepaytype_modifiedby";
            [Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "lk_msdyn_resourcepaytype_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_resourcepaytype_modifiedonbehalfby = "lk_msdyn_resourcepaytype_modifiedonbehalfby";
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
            [Relationship("msdyn_richtextfile", EntityRole.Referenced, "lk_msdyn_richtextfile_createdby", "createdby")]
            public const string lk_msdyn_richtextfile_createdby = "lk_msdyn_richtextfile_createdby";
            [Relationship("msdyn_richtextfile", EntityRole.Referenced, "lk_msdyn_richtextfile_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_richtextfile_createdonbehalfby = "lk_msdyn_richtextfile_createdonbehalfby";
            [Relationship("msdyn_richtextfile", EntityRole.Referenced, "lk_msdyn_richtextfile_modifiedby", "modifiedby")]
            public const string lk_msdyn_richtextfile_modifiedby = "lk_msdyn_richtextfile_modifiedby";
            [Relationship("msdyn_richtextfile", EntityRole.Referenced, "lk_msdyn_richtextfile_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_richtextfile_modifiedonbehalfby = "lk_msdyn_richtextfile_modifiedonbehalfby";
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
            [Relationship("msdyn_salesaccelerationsettings", EntityRole.Referenced, "lk_msdyn_salesaccelerationsettings_createdby", "createdby")]
            public const string lk_msdyn_salesaccelerationsettings_createdby = "lk_msdyn_salesaccelerationsettings_createdby";
            [Relationship("msdyn_salesaccelerationsettings", EntityRole.Referenced, "lk_msdyn_salesaccelerationsettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_salesaccelerationsettings_createdonbehalfby = "lk_msdyn_salesaccelerationsettings_createdonbehalfby";
            [Relationship("msdyn_salesaccelerationsettings", EntityRole.Referenced, "lk_msdyn_salesaccelerationsettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_salesaccelerationsettings_modifiedby = "lk_msdyn_salesaccelerationsettings_modifiedby";
            [Relationship("msdyn_salesaccelerationsettings", EntityRole.Referenced, "lk_msdyn_salesaccelerationsettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_salesaccelerationsettings_modifiedonbehalfby = "lk_msdyn_salesaccelerationsettings_modifiedonbehalfby";
            [Relationship("msdyn_salesassignmentsetting", EntityRole.Referenced, "lk_msdyn_salesassignmentsetting_createdby", "createdby")]
            public const string lk_msdyn_salesassignmentsetting_createdby = "lk_msdyn_salesassignmentsetting_createdby";
            [Relationship("msdyn_salesassignmentsetting", EntityRole.Referenced, "lk_msdyn_salesassignmentsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_salesassignmentsetting_createdonbehalfby = "lk_msdyn_salesassignmentsetting_createdonbehalfby";
            [Relationship("msdyn_salesassignmentsetting", EntityRole.Referenced, "lk_msdyn_salesassignmentsetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_salesassignmentsetting_modifiedby = "lk_msdyn_salesassignmentsetting_modifiedby";
            [Relationship("msdyn_salesassignmentsetting", EntityRole.Referenced, "lk_msdyn_salesassignmentsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_salesassignmentsetting_modifiedonbehalfby = "lk_msdyn_salesassignmentsetting_modifiedonbehalfby";
            [Relationship("msdyn_salesinsightssettings", EntityRole.Referenced, "lk_msdyn_salesinsightssettings_createdby", "createdby")]
            public const string lk_msdyn_salesinsightssettings_createdby = "lk_msdyn_salesinsightssettings_createdby";
            [Relationship("msdyn_salesinsightssettings", EntityRole.Referenced, "lk_msdyn_salesinsightssettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_salesinsightssettings_createdonbehalfby = "lk_msdyn_salesinsightssettings_createdonbehalfby";
            [Relationship("msdyn_salesinsightssettings", EntityRole.Referenced, "lk_msdyn_salesinsightssettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_salesinsightssettings_modifiedby = "lk_msdyn_salesinsightssettings_modifiedby";
            [Relationship("msdyn_salesinsightssettings", EntityRole.Referenced, "lk_msdyn_salesinsightssettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_salesinsightssettings_modifiedonbehalfby = "lk_msdyn_salesinsightssettings_modifiedonbehalfby";
            [Relationship("msdyn_salesroutingrun", EntityRole.Referenced, "lk_msdyn_salesroutingrun_createdby", "createdby")]
            public const string lk_msdyn_salesroutingrun_createdby = "lk_msdyn_salesroutingrun_createdby";
            [Relationship("msdyn_salesroutingrun", EntityRole.Referenced, "lk_msdyn_salesroutingrun_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_salesroutingrun_createdonbehalfby = "lk_msdyn_salesroutingrun_createdonbehalfby";
            [Relationship("msdyn_salesroutingrun", EntityRole.Referenced, "lk_msdyn_salesroutingrun_modifiedby", "modifiedby")]
            public const string lk_msdyn_salesroutingrun_modifiedby = "lk_msdyn_salesroutingrun_modifiedby";
            [Relationship("msdyn_salesroutingrun", EntityRole.Referenced, "lk_msdyn_salesroutingrun_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_salesroutingrun_modifiedonbehalfby = "lk_msdyn_salesroutingrun_modifiedonbehalfby";
            [Relationship("msdyn_salessuggestion", EntityRole.Referenced, "lk_msdyn_salessuggestion_createdby", "createdby")]
            public const string lk_msdyn_salessuggestion_createdby = "lk_msdyn_salessuggestion_createdby";
            [Relationship("msdyn_salessuggestion", EntityRole.Referenced, "lk_msdyn_salessuggestion_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_salessuggestion_createdonbehalfby = "lk_msdyn_salessuggestion_createdonbehalfby";
            [Relationship("msdyn_salessuggestion", EntityRole.Referenced, "lk_msdyn_salessuggestion_modifiedby", "modifiedby")]
            public const string lk_msdyn_salessuggestion_modifiedby = "lk_msdyn_salessuggestion_modifiedby";
            [Relationship("msdyn_salessuggestion", EntityRole.Referenced, "lk_msdyn_salessuggestion_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_salessuggestion_modifiedonbehalfby = "lk_msdyn_salessuggestion_modifiedonbehalfby";
            [Relationship("msdyn_salestag", EntityRole.Referenced, "lk_msdyn_salestag_createdby", "createdby")]
            public const string lk_msdyn_salestag_createdby = "lk_msdyn_salestag_createdby";
            [Relationship("msdyn_salestag", EntityRole.Referenced, "lk_msdyn_salestag_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_salestag_createdonbehalfby = "lk_msdyn_salestag_createdonbehalfby";
            [Relationship("msdyn_salestag", EntityRole.Referenced, "lk_msdyn_salestag_modifiedby", "modifiedby")]
            public const string lk_msdyn_salestag_modifiedby = "lk_msdyn_salestag_modifiedby";
            [Relationship("msdyn_salestag", EntityRole.Referenced, "lk_msdyn_salestag_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_salestag_modifiedonbehalfby = "lk_msdyn_salestag_modifiedonbehalfby";
            [Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "lk_msdyn_scheduleboardsetting_createdby", "createdby")]
            public const string lk_msdyn_scheduleboardsetting_createdby = "lk_msdyn_scheduleboardsetting_createdby";
            [Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "lk_msdyn_scheduleboardsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_scheduleboardsetting_createdonbehalfby = "lk_msdyn_scheduleboardsetting_createdonbehalfby";
            [Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "lk_msdyn_scheduleboardsetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_scheduleboardsetting_modifiedby = "lk_msdyn_scheduleboardsetting_modifiedby";
            [Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "lk_msdyn_scheduleboardsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_scheduleboardsetting_modifiedonbehalfby = "lk_msdyn_scheduleboardsetting_modifiedonbehalfby";
            [Relationship("msdyn_schedulingfeatureflag", EntityRole.Referenced, "lk_msdyn_schedulingfeatureflag_createdby", "createdby")]
            public const string lk_msdyn_schedulingfeatureflag_createdby = "lk_msdyn_schedulingfeatureflag_createdby";
            [Relationship("msdyn_schedulingfeatureflag", EntityRole.Referenced, "lk_msdyn_schedulingfeatureflag_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_schedulingfeatureflag_createdonbehalfby = "lk_msdyn_schedulingfeatureflag_createdonbehalfby";
            [Relationship("msdyn_schedulingfeatureflag", EntityRole.Referenced, "lk_msdyn_schedulingfeatureflag_modifiedby", "modifiedby")]
            public const string lk_msdyn_schedulingfeatureflag_modifiedby = "lk_msdyn_schedulingfeatureflag_modifiedby";
            [Relationship("msdyn_schedulingfeatureflag", EntityRole.Referenced, "lk_msdyn_schedulingfeatureflag_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_schedulingfeatureflag_modifiedonbehalfby = "lk_msdyn_schedulingfeatureflag_modifiedonbehalfby";
            [Relationship("msdyn_schedulingparameter", EntityRole.Referenced, "lk_msdyn_schedulingparameter_createdby", "createdby")]
            public const string lk_msdyn_schedulingparameter_createdby = "lk_msdyn_schedulingparameter_createdby";
            [Relationship("msdyn_schedulingparameter", EntityRole.Referenced, "lk_msdyn_schedulingparameter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_schedulingparameter_createdonbehalfby = "lk_msdyn_schedulingparameter_createdonbehalfby";
            [Relationship("msdyn_schedulingparameter", EntityRole.Referenced, "lk_msdyn_schedulingparameter_modifiedby", "modifiedby")]
            public const string lk_msdyn_schedulingparameter_modifiedby = "lk_msdyn_schedulingparameter_modifiedby";
            [Relationship("msdyn_schedulingparameter", EntityRole.Referenced, "lk_msdyn_schedulingparameter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_schedulingparameter_modifiedonbehalfby = "lk_msdyn_schedulingparameter_modifiedonbehalfby";
            [Relationship("msdyn_segment", EntityRole.Referenced, "lk_msdyn_segment_createdby", "createdby")]
            public const string lk_msdyn_segment_createdby = "lk_msdyn_segment_createdby";
            [Relationship("msdyn_segment", EntityRole.Referenced, "lk_msdyn_segment_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_segment_createdonbehalfby = "lk_msdyn_segment_createdonbehalfby";
            [Relationship("msdyn_segment", EntityRole.Referenced, "lk_msdyn_segment_modifiedby", "modifiedby")]
            public const string lk_msdyn_segment_modifiedby = "lk_msdyn_segment_modifiedby";
            [Relationship("msdyn_segment", EntityRole.Referenced, "lk_msdyn_segment_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_segment_modifiedonbehalfby = "lk_msdyn_segment_modifiedonbehalfby";
            [Relationship("msdyn_segmentcatalogue", EntityRole.Referenced, "lk_msdyn_segmentcatalogue_createdby", "createdby")]
            public const string lk_msdyn_segmentcatalogue_createdby = "lk_msdyn_segmentcatalogue_createdby";
            [Relationship("msdyn_segmentcatalogue", EntityRole.Referenced, "lk_msdyn_segmentcatalogue_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_segmentcatalogue_createdonbehalfby = "lk_msdyn_segmentcatalogue_createdonbehalfby";
            [Relationship("msdyn_segmentcatalogue", EntityRole.Referenced, "lk_msdyn_segmentcatalogue_modifiedby", "modifiedby")]
            public const string lk_msdyn_segmentcatalogue_modifiedby = "lk_msdyn_segmentcatalogue_modifiedby";
            [Relationship("msdyn_segmentcatalogue", EntityRole.Referenced, "lk_msdyn_segmentcatalogue_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_segmentcatalogue_modifiedonbehalfby = "lk_msdyn_segmentcatalogue_modifiedonbehalfby";
            [Relationship("msdyn_sequence", EntityRole.Referenced, "lk_msdyn_sequence_createdby", "createdby")]
            public const string lk_msdyn_sequence_createdby = "lk_msdyn_sequence_createdby";
            [Relationship("msdyn_sequence", EntityRole.Referenced, "lk_msdyn_sequence_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_sequence_createdonbehalfby = "lk_msdyn_sequence_createdonbehalfby";
            [Relationship("msdyn_sequence", EntityRole.Referenced, "lk_msdyn_sequence_modifiedby", "modifiedby")]
            public const string lk_msdyn_sequence_modifiedby = "lk_msdyn_sequence_modifiedby";
            [Relationship("msdyn_sequence", EntityRole.Referenced, "lk_msdyn_sequence_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_sequence_modifiedonbehalfby = "lk_msdyn_sequence_modifiedonbehalfby";
            [Relationship("msdyn_sequencestat", EntityRole.Referenced, "lk_msdyn_sequencestat_createdby", "createdby")]
            public const string lk_msdyn_sequencestat_createdby = "lk_msdyn_sequencestat_createdby";
            [Relationship("msdyn_sequencestat", EntityRole.Referenced, "lk_msdyn_sequencestat_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_sequencestat_createdonbehalfby = "lk_msdyn_sequencestat_createdonbehalfby";
            [Relationship("msdyn_sequencestat", EntityRole.Referenced, "lk_msdyn_sequencestat_modifiedby", "modifiedby")]
            public const string lk_msdyn_sequencestat_modifiedby = "lk_msdyn_sequencestat_modifiedby";
            [Relationship("msdyn_sequencestat", EntityRole.Referenced, "lk_msdyn_sequencestat_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_sequencestat_modifiedonbehalfby = "lk_msdyn_sequencestat_modifiedonbehalfby";
            [Relationship("msdyn_sequencetarget", EntityRole.Referenced, "lk_msdyn_sequencetarget_createdby", "createdby")]
            public const string lk_msdyn_sequencetarget_createdby = "lk_msdyn_sequencetarget_createdby";
            [Relationship("msdyn_sequencetarget", EntityRole.Referenced, "lk_msdyn_sequencetarget_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_sequencetarget_createdonbehalfby = "lk_msdyn_sequencetarget_createdonbehalfby";
            [Relationship("msdyn_sequencetarget", EntityRole.Referenced, "lk_msdyn_sequencetarget_modifiedby", "modifiedby")]
            public const string lk_msdyn_sequencetarget_modifiedby = "lk_msdyn_sequencetarget_modifiedby";
            [Relationship("msdyn_sequencetarget", EntityRole.Referenced, "lk_msdyn_sequencetarget_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_sequencetarget_modifiedonbehalfby = "lk_msdyn_sequencetarget_modifiedonbehalfby";
            [Relationship("msdyn_sequencetargetstep", EntityRole.Referenced, "lk_msdyn_sequencetargetstep_createdby", "createdby")]
            public const string lk_msdyn_sequencetargetstep_createdby = "lk_msdyn_sequencetargetstep_createdby";
            [Relationship("msdyn_sequencetargetstep", EntityRole.Referenced, "lk_msdyn_sequencetargetstep_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_sequencetargetstep_createdonbehalfby = "lk_msdyn_sequencetargetstep_createdonbehalfby";
            [Relationship("msdyn_sequencetargetstep", EntityRole.Referenced, "lk_msdyn_sequencetargetstep_modifiedby", "modifiedby")]
            public const string lk_msdyn_sequencetargetstep_modifiedby = "lk_msdyn_sequencetargetstep_modifiedby";
            [Relationship("msdyn_sequencetargetstep", EntityRole.Referenced, "lk_msdyn_sequencetargetstep_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_sequencetargetstep_modifiedonbehalfby = "lk_msdyn_sequencetargetstep_modifiedonbehalfby";
            [Relationship("msdyn_serviceconfiguration", EntityRole.Referenced, "lk_msdyn_serviceconfiguration_createdby", "createdby")]
            public const string lk_msdyn_serviceconfiguration_createdby = "lk_msdyn_serviceconfiguration_createdby";
            [Relationship("msdyn_serviceconfiguration", EntityRole.Referenced, "lk_msdyn_serviceconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_serviceconfiguration_createdonbehalfby = "lk_msdyn_serviceconfiguration_createdonbehalfby";
            [Relationship("msdyn_serviceconfiguration", EntityRole.Referenced, "lk_msdyn_serviceconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_serviceconfiguration_modifiedby = "lk_msdyn_serviceconfiguration_modifiedby";
            [Relationship("msdyn_serviceconfiguration", EntityRole.Referenced, "lk_msdyn_serviceconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_serviceconfiguration_modifiedonbehalfby = "lk_msdyn_serviceconfiguration_modifiedonbehalfby";
            [Relationship("msdyn_servicetasktype", EntityRole.Referenced, "lk_msdyn_servicetasktype_createdby", "createdby")]
            public const string lk_msdyn_servicetasktype_createdby = "lk_msdyn_servicetasktype_createdby";
            [Relationship("msdyn_servicetasktype", EntityRole.Referenced, "lk_msdyn_servicetasktype_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_servicetasktype_createdonbehalfby = "lk_msdyn_servicetasktype_createdonbehalfby";
            [Relationship("msdyn_servicetasktype", EntityRole.Referenced, "lk_msdyn_servicetasktype_modifiedby", "modifiedby")]
            public const string lk_msdyn_servicetasktype_modifiedby = "lk_msdyn_servicetasktype_modifiedby";
            [Relationship("msdyn_servicetasktype", EntityRole.Referenced, "lk_msdyn_servicetasktype_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_servicetasktype_modifiedonbehalfby = "lk_msdyn_servicetasktype_modifiedonbehalfby";
            [Relationship("msdyn_sessiondata", EntityRole.Referenced, "lk_msdyn_sessiondata_createdby", "createdby")]
            public const string lk_msdyn_sessiondata_createdby = "lk_msdyn_sessiondata_createdby";
            [Relationship("msdyn_sessiondata", EntityRole.Referenced, "lk_msdyn_sessiondata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_sessiondata_createdonbehalfby = "lk_msdyn_sessiondata_createdonbehalfby";
            [Relationship("msdyn_sessiondata", EntityRole.Referenced, "lk_msdyn_sessiondata_modifiedby", "modifiedby")]
            public const string lk_msdyn_sessiondata_modifiedby = "lk_msdyn_sessiondata_modifiedby";
            [Relationship("msdyn_sessiondata", EntityRole.Referenced, "lk_msdyn_sessiondata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_sessiondata_modifiedonbehalfby = "lk_msdyn_sessiondata_modifiedonbehalfby";
            [Relationship("msdyn_sessionparticipantdata", EntityRole.Referenced, "lk_msdyn_sessionparticipantdata_createdby", "createdby")]
            public const string lk_msdyn_sessionparticipantdata_createdby = "lk_msdyn_sessionparticipantdata_createdby";
            [Relationship("msdyn_sessionparticipantdata", EntityRole.Referenced, "lk_msdyn_sessionparticipantdata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_sessionparticipantdata_createdonbehalfby = "lk_msdyn_sessionparticipantdata_createdonbehalfby";
            [Relationship("msdyn_sessionparticipantdata", EntityRole.Referenced, "lk_msdyn_sessionparticipantdata_modifiedby", "modifiedby")]
            public const string lk_msdyn_sessionparticipantdata_modifiedby = "lk_msdyn_sessionparticipantdata_modifiedby";
            [Relationship("msdyn_sessionparticipantdata", EntityRole.Referenced, "lk_msdyn_sessionparticipantdata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_sessionparticipantdata_modifiedonbehalfby = "lk_msdyn_sessionparticipantdata_modifiedonbehalfby";
            [Relationship("msdyn_sessiontemplate", EntityRole.Referenced, "lk_msdyn_sessiontemplate_createdby", "createdby")]
            public const string lk_msdyn_sessiontemplate_createdby = "lk_msdyn_sessiontemplate_createdby";
            [Relationship("msdyn_sessiontemplate", EntityRole.Referenced, "lk_msdyn_sessiontemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_sessiontemplate_createdonbehalfby = "lk_msdyn_sessiontemplate_createdonbehalfby";
            [Relationship("msdyn_sessiontemplate", EntityRole.Referenced, "lk_msdyn_sessiontemplate_modifiedby", "modifiedby")]
            public const string lk_msdyn_sessiontemplate_modifiedby = "lk_msdyn_sessiontemplate_modifiedby";
            [Relationship("msdyn_sessiontemplate", EntityRole.Referenced, "lk_msdyn_sessiontemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_sessiontemplate_modifiedonbehalfby = "lk_msdyn_sessiontemplate_modifiedonbehalfby";
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
            [Relationship("msdyn_sikeyvalueconfig", EntityRole.Referenced, "lk_msdyn_sikeyvalueconfig_createdby", "createdby")]
            public const string lk_msdyn_sikeyvalueconfig_createdby = "lk_msdyn_sikeyvalueconfig_createdby";
            [Relationship("msdyn_sikeyvalueconfig", EntityRole.Referenced, "lk_msdyn_sikeyvalueconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_sikeyvalueconfig_createdonbehalfby = "lk_msdyn_sikeyvalueconfig_createdonbehalfby";
            [Relationship("msdyn_sikeyvalueconfig", EntityRole.Referenced, "lk_msdyn_sikeyvalueconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_sikeyvalueconfig_modifiedby = "lk_msdyn_sikeyvalueconfig_modifiedby";
            [Relationship("msdyn_sikeyvalueconfig", EntityRole.Referenced, "lk_msdyn_sikeyvalueconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_sikeyvalueconfig_modifiedonbehalfby = "lk_msdyn_sikeyvalueconfig_modifiedonbehalfby";
            [Relationship("msdyn_slakpi", EntityRole.Referenced, "lk_msdyn_slakpi_createdby", "createdby")]
            public const string lk_msdyn_slakpi_createdby = "lk_msdyn_slakpi_createdby";
            [Relationship("msdyn_slakpi", EntityRole.Referenced, "lk_msdyn_slakpi_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_slakpi_createdonbehalfby = "lk_msdyn_slakpi_createdonbehalfby";
            [Relationship("msdyn_slakpi", EntityRole.Referenced, "lk_msdyn_slakpi_modifiedby", "modifiedby")]
            public const string lk_msdyn_slakpi_modifiedby = "lk_msdyn_slakpi_modifiedby";
            [Relationship("msdyn_slakpi", EntityRole.Referenced, "lk_msdyn_slakpi_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_slakpi_modifiedonbehalfby = "lk_msdyn_slakpi_modifiedonbehalfby";
            [Relationship("msdyn_smartassistconfig", EntityRole.Referenced, "lk_msdyn_smartassistconfig_createdby", "createdby")]
            public const string lk_msdyn_smartassistconfig_createdby = "lk_msdyn_smartassistconfig_createdby";
            [Relationship("msdyn_smartassistconfig", EntityRole.Referenced, "lk_msdyn_smartassistconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_smartassistconfig_createdonbehalfby = "lk_msdyn_smartassistconfig_createdonbehalfby";
            [Relationship("msdyn_smartassistconfig", EntityRole.Referenced, "lk_msdyn_smartassistconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_smartassistconfig_modifiedby = "lk_msdyn_smartassistconfig_modifiedby";
            [Relationship("msdyn_smartassistconfig", EntityRole.Referenced, "lk_msdyn_smartassistconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_smartassistconfig_modifiedonbehalfby = "lk_msdyn_smartassistconfig_modifiedonbehalfby";
            [Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "lk_msdyn_solutionhealthrule_createdby", "createdby")]
            public const string lk_msdyn_solutionhealthrule_createdby = "lk_msdyn_solutionhealthrule_createdby";
            [Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "lk_msdyn_solutionhealthrule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_solutionhealthrule_createdonbehalfby = "lk_msdyn_solutionhealthrule_createdonbehalfby";
            [Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "lk_msdyn_solutionhealthrule_modifiedby", "modifiedby")]
            public const string lk_msdyn_solutionhealthrule_modifiedby = "lk_msdyn_solutionhealthrule_modifiedby";
            [Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "lk_msdyn_solutionhealthrule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_solutionhealthrule_modifiedonbehalfby = "lk_msdyn_solutionhealthrule_modifiedonbehalfby";
            [Relationship("msdyn_solutionhealthruleargument", EntityRole.Referenced, "lk_msdyn_solutionhealthruleargument_createdby", "createdby")]
            public const string lk_msdyn_solutionhealthruleargument_createdby = "lk_msdyn_solutionhealthruleargument_createdby";
            [Relationship("msdyn_solutionhealthruleargument", EntityRole.Referenced, "lk_msdyn_solutionhealthruleargument_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_solutionhealthruleargument_createdonbehalfby = "lk_msdyn_solutionhealthruleargument_createdonbehalfby";
            [Relationship("msdyn_solutionhealthruleargument", EntityRole.Referenced, "lk_msdyn_solutionhealthruleargument_modifiedby", "modifiedby")]
            public const string lk_msdyn_solutionhealthruleargument_modifiedby = "lk_msdyn_solutionhealthruleargument_modifiedby";
            [Relationship("msdyn_solutionhealthruleargument", EntityRole.Referenced, "lk_msdyn_solutionhealthruleargument_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_solutionhealthruleargument_modifiedonbehalfby = "lk_msdyn_solutionhealthruleargument_modifiedonbehalfby";
            [Relationship("msdyn_solutionhealthruleset", EntityRole.Referenced, "lk_msdyn_solutionhealthruleset_createdby", "createdby")]
            public const string lk_msdyn_solutionhealthruleset_createdby = "lk_msdyn_solutionhealthruleset_createdby";
            [Relationship("msdyn_solutionhealthruleset", EntityRole.Referenced, "lk_msdyn_solutionhealthruleset_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_solutionhealthruleset_createdonbehalfby = "lk_msdyn_solutionhealthruleset_createdonbehalfby";
            [Relationship("msdyn_solutionhealthruleset", EntityRole.Referenced, "lk_msdyn_solutionhealthruleset_modifiedby", "modifiedby")]
            public const string lk_msdyn_solutionhealthruleset_modifiedby = "lk_msdyn_solutionhealthruleset_modifiedby";
            [Relationship("msdyn_solutionhealthruleset", EntityRole.Referenced, "lk_msdyn_solutionhealthruleset_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_solutionhealthruleset_modifiedonbehalfby = "lk_msdyn_solutionhealthruleset_modifiedonbehalfby";
            [Relationship("msdyn_suggestioninteraction", EntityRole.Referenced, "lk_msdyn_suggestioninteraction_createdby", "createdby")]
            public const string lk_msdyn_suggestioninteraction_createdby = "lk_msdyn_suggestioninteraction_createdby";
            [Relationship("msdyn_suggestioninteraction", EntityRole.Referenced, "lk_msdyn_suggestioninteraction_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_suggestioninteraction_createdonbehalfby = "lk_msdyn_suggestioninteraction_createdonbehalfby";
            [Relationship("msdyn_suggestioninteraction", EntityRole.Referenced, "lk_msdyn_suggestioninteraction_modifiedby", "modifiedby")]
            public const string lk_msdyn_suggestioninteraction_modifiedby = "lk_msdyn_suggestioninteraction_modifiedby";
            [Relationship("msdyn_suggestioninteraction", EntityRole.Referenced, "lk_msdyn_suggestioninteraction_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_suggestioninteraction_modifiedonbehalfby = "lk_msdyn_suggestioninteraction_modifiedonbehalfby";
            [Relationship("msdyn_suggestionrequestpayload", EntityRole.Referenced, "lk_msdyn_suggestionrequestpayload_createdby", "createdby")]
            public const string lk_msdyn_suggestionrequestpayload_createdby = "lk_msdyn_suggestionrequestpayload_createdby";
            [Relationship("msdyn_suggestionrequestpayload", EntityRole.Referenced, "lk_msdyn_suggestionrequestpayload_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_suggestionrequestpayload_createdonbehalfby = "lk_msdyn_suggestionrequestpayload_createdonbehalfby";
            [Relationship("msdyn_suggestionrequestpayload", EntityRole.Referenced, "lk_msdyn_suggestionrequestpayload_modifiedby", "modifiedby")]
            public const string lk_msdyn_suggestionrequestpayload_modifiedby = "lk_msdyn_suggestionrequestpayload_modifiedby";
            [Relationship("msdyn_suggestionrequestpayload", EntityRole.Referenced, "lk_msdyn_suggestionrequestpayload_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_suggestionrequestpayload_modifiedonbehalfby = "lk_msdyn_suggestionrequestpayload_modifiedonbehalfby";
            [Relationship("msdyn_suggestionsmodelsummary", EntityRole.Referenced, "lk_msdyn_suggestionsmodelsummary_createdby", "createdby")]
            public const string lk_msdyn_suggestionsmodelsummary_createdby = "lk_msdyn_suggestionsmodelsummary_createdby";
            [Relationship("msdyn_suggestionsmodelsummary", EntityRole.Referenced, "lk_msdyn_suggestionsmodelsummary_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_suggestionsmodelsummary_createdonbehalfby = "lk_msdyn_suggestionsmodelsummary_createdonbehalfby";
            [Relationship("msdyn_suggestionsmodelsummary", EntityRole.Referenced, "lk_msdyn_suggestionsmodelsummary_modifiedby", "modifiedby")]
            public const string lk_msdyn_suggestionsmodelsummary_modifiedby = "lk_msdyn_suggestionsmodelsummary_modifiedby";
            [Relationship("msdyn_suggestionsmodelsummary", EntityRole.Referenced, "lk_msdyn_suggestionsmodelsummary_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_suggestionsmodelsummary_modifiedonbehalfby = "lk_msdyn_suggestionsmodelsummary_modifiedonbehalfby";
            [Relationship("msdyn_suggestionssetting", EntityRole.Referenced, "lk_msdyn_suggestionssetting_createdby", "createdby")]
            public const string lk_msdyn_suggestionssetting_createdby = "lk_msdyn_suggestionssetting_createdby";
            [Relationship("msdyn_suggestionssetting", EntityRole.Referenced, "lk_msdyn_suggestionssetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_suggestionssetting_createdonbehalfby = "lk_msdyn_suggestionssetting_createdonbehalfby";
            [Relationship("msdyn_suggestionssetting", EntityRole.Referenced, "lk_msdyn_suggestionssetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_suggestionssetting_modifiedby = "lk_msdyn_suggestionssetting_modifiedby";
            [Relationship("msdyn_suggestionssetting", EntityRole.Referenced, "lk_msdyn_suggestionssetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_suggestionssetting_modifiedonbehalfby = "lk_msdyn_suggestionssetting_modifiedonbehalfby";
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
            [Relationship("msdyn_teamschatassociation", EntityRole.Referenced, "lk_msdyn_teamschatassociation_createdby", "createdby")]
            public const string lk_msdyn_teamschatassociation_createdby = "lk_msdyn_teamschatassociation_createdby";
            [Relationship("msdyn_teamschatassociation", EntityRole.Referenced, "lk_msdyn_teamschatassociation_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_teamschatassociation_createdonbehalfby = "lk_msdyn_teamschatassociation_createdonbehalfby";
            [Relationship("msdyn_teamschatassociation", EntityRole.Referenced, "lk_msdyn_teamschatassociation_modifiedby", "modifiedby")]
            public const string lk_msdyn_teamschatassociation_modifiedby = "lk_msdyn_teamschatassociation_modifiedby";
            [Relationship("msdyn_teamschatassociation", EntityRole.Referenced, "lk_msdyn_teamschatassociation_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_teamschatassociation_modifiedonbehalfby = "lk_msdyn_teamschatassociation_modifiedonbehalfby";
            [Relationship("msdyn_teamschatsuggestion", EntityRole.Referenced, "lk_msdyn_teamschatsuggestion_createdby", "createdby")]
            public const string lk_msdyn_teamschatsuggestion_createdby = "lk_msdyn_teamschatsuggestion_createdby";
            [Relationship("msdyn_teamschatsuggestion", EntityRole.Referenced, "lk_msdyn_teamschatsuggestion_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_teamschatsuggestion_createdonbehalfby = "lk_msdyn_teamschatsuggestion_createdonbehalfby";
            [Relationship("msdyn_teamschatsuggestion", EntityRole.Referenced, "lk_msdyn_teamschatsuggestion_modifiedby", "modifiedby")]
            public const string lk_msdyn_teamschatsuggestion_modifiedby = "lk_msdyn_teamschatsuggestion_modifiedby";
            [Relationship("msdyn_teamschatsuggestion", EntityRole.Referenced, "lk_msdyn_teamschatsuggestion_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_teamschatsuggestion_modifiedonbehalfby = "lk_msdyn_teamschatsuggestion_modifiedonbehalfby";
            [Relationship("msdyn_teamscollaboration", EntityRole.Referenced, "lk_msdyn_teamscollaboration_createdby", "createdby")]
            public const string lk_msdyn_teamscollaboration_createdby = "lk_msdyn_teamscollaboration_createdby";
            [Relationship("msdyn_teamscollaboration", EntityRole.Referenced, "lk_msdyn_teamscollaboration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_teamscollaboration_createdonbehalfby = "lk_msdyn_teamscollaboration_createdonbehalfby";
            [Relationship("msdyn_teamscollaboration", EntityRole.Referenced, "lk_msdyn_teamscollaboration_modifiedby", "modifiedby")]
            public const string lk_msdyn_teamscollaboration_modifiedby = "lk_msdyn_teamscollaboration_modifiedby";
            [Relationship("msdyn_teamscollaboration", EntityRole.Referenced, "lk_msdyn_teamscollaboration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_teamscollaboration_modifiedonbehalfby = "lk_msdyn_teamscollaboration_modifiedonbehalfby";
            [Relationship("msdyn_teamsdialeradminsettings", EntityRole.Referenced, "lk_msdyn_teamsdialeradminsettings_createdby", "createdby")]
            public const string lk_msdyn_teamsdialeradminsettings_createdby = "lk_msdyn_teamsdialeradminsettings_createdby";
            [Relationship("msdyn_teamsdialeradminsettings", EntityRole.Referenced, "lk_msdyn_teamsdialeradminsettings_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_teamsdialeradminsettings_createdonbehalfby = "lk_msdyn_teamsdialeradminsettings_createdonbehalfby";
            [Relationship("msdyn_teamsdialeradminsettings", EntityRole.Referenced, "lk_msdyn_teamsdialeradminsettings_modifiedby", "modifiedby")]
            public const string lk_msdyn_teamsdialeradminsettings_modifiedby = "lk_msdyn_teamsdialeradminsettings_modifiedby";
            [Relationship("msdyn_teamsdialeradminsettings", EntityRole.Referenced, "lk_msdyn_teamsdialeradminsettings_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_teamsdialeradminsettings_modifiedonbehalfby = "lk_msdyn_teamsdialeradminsettings_modifiedonbehalfby";
            [Relationship("msdyn_templateforproperties", EntityRole.Referenced, "lk_msdyn_templateforproperties_createdby", "createdby")]
            public const string lk_msdyn_templateforproperties_createdby = "lk_msdyn_templateforproperties_createdby";
            [Relationship("msdyn_templateforproperties", EntityRole.Referenced, "lk_msdyn_templateforproperties_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_templateforproperties_createdonbehalfby = "lk_msdyn_templateforproperties_createdonbehalfby";
            [Relationship("msdyn_templateforproperties", EntityRole.Referenced, "lk_msdyn_templateforproperties_modifiedby", "modifiedby")]
            public const string lk_msdyn_templateforproperties_modifiedby = "lk_msdyn_templateforproperties_modifiedby";
            [Relationship("msdyn_templateforproperties", EntityRole.Referenced, "lk_msdyn_templateforproperties_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_templateforproperties_modifiedonbehalfby = "lk_msdyn_templateforproperties_modifiedonbehalfby";
            [Relationship("msdyn_templateparameter", EntityRole.Referenced, "lk_msdyn_templateparameter_createdby", "createdby")]
            public const string lk_msdyn_templateparameter_createdby = "lk_msdyn_templateparameter_createdby";
            [Relationship("msdyn_templateparameter", EntityRole.Referenced, "lk_msdyn_templateparameter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_templateparameter_createdonbehalfby = "lk_msdyn_templateparameter_createdonbehalfby";
            [Relationship("msdyn_templateparameter", EntityRole.Referenced, "lk_msdyn_templateparameter_modifiedby", "modifiedby")]
            public const string lk_msdyn_templateparameter_modifiedby = "lk_msdyn_templateparameter_modifiedby";
            [Relationship("msdyn_templateparameter", EntityRole.Referenced, "lk_msdyn_templateparameter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_templateparameter_modifiedonbehalfby = "lk_msdyn_templateparameter_modifiedonbehalfby";
            [Relationship("msdyn_timeentry", EntityRole.Referenced, "lk_msdyn_timeentry_createdby", "createdby")]
            public const string lk_msdyn_timeentry_createdby = "lk_msdyn_timeentry_createdby";
            [Relationship("msdyn_timeentry", EntityRole.Referenced, "lk_msdyn_timeentry_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_timeentry_createdonbehalfby = "lk_msdyn_timeentry_createdonbehalfby";
            [Relationship("msdyn_timeentry", EntityRole.Referenced, "lk_msdyn_timeentry_modifiedby", "modifiedby")]
            public const string lk_msdyn_timeentry_modifiedby = "lk_msdyn_timeentry_modifiedby";
            [Relationship("msdyn_timeentry", EntityRole.Referenced, "lk_msdyn_timeentry_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_timeentry_modifiedonbehalfby = "lk_msdyn_timeentry_modifiedonbehalfby";
            [Relationship("msdyn_timeentrysetting", EntityRole.Referenced, "lk_msdyn_timeentrysetting_createdby", "createdby")]
            public const string lk_msdyn_timeentrysetting_createdby = "lk_msdyn_timeentrysetting_createdby";
            [Relationship("msdyn_timeentrysetting", EntityRole.Referenced, "lk_msdyn_timeentrysetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_timeentrysetting_createdonbehalfby = "lk_msdyn_timeentrysetting_createdonbehalfby";
            [Relationship("msdyn_timeentrysetting", EntityRole.Referenced, "lk_msdyn_timeentrysetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_timeentrysetting_modifiedby = "lk_msdyn_timeentrysetting_modifiedby";
            [Relationship("msdyn_timeentrysetting", EntityRole.Referenced, "lk_msdyn_timeentrysetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_timeentrysetting_modifiedonbehalfby = "lk_msdyn_timeentrysetting_modifiedonbehalfby";
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
            [Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "lk_msdyn_timeoffrequest_createdby", "createdby")]
            public const string lk_msdyn_timeoffrequest_createdby = "lk_msdyn_timeoffrequest_createdby";
            [Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "lk_msdyn_timeoffrequest_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_timeoffrequest_createdonbehalfby = "lk_msdyn_timeoffrequest_createdonbehalfby";
            [Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "lk_msdyn_timeoffrequest_modifiedby", "modifiedby")]
            public const string lk_msdyn_timeoffrequest_modifiedby = "lk_msdyn_timeoffrequest_modifiedby";
            [Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "lk_msdyn_timeoffrequest_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_timeoffrequest_modifiedonbehalfby = "lk_msdyn_timeoffrequest_modifiedonbehalfby";
            [Relationship("msdyn_tour", EntityRole.Referenced, "lk_msdyn_tour_createdby", "createdby")]
            public const string lk_msdyn_tour_createdby = "lk_msdyn_tour_createdby";
            [Relationship("msdyn_tour", EntityRole.Referenced, "lk_msdyn_tour_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_tour_createdonbehalfby = "lk_msdyn_tour_createdonbehalfby";
            [Relationship("msdyn_tour", EntityRole.Referenced, "lk_msdyn_tour_modifiedby", "modifiedby")]
            public const string lk_msdyn_tour_modifiedby = "lk_msdyn_tour_modifiedby";
            [Relationship("msdyn_tour", EntityRole.Referenced, "lk_msdyn_tour_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_tour_modifiedonbehalfby = "lk_msdyn_tour_modifiedonbehalfby";
            [Relationship("msdyn_transactionorigin", EntityRole.Referenced, "lk_msdyn_transactionorigin_createdby", "createdby")]
            public const string lk_msdyn_transactionorigin_createdby = "lk_msdyn_transactionorigin_createdby";
            [Relationship("msdyn_transactionorigin", EntityRole.Referenced, "lk_msdyn_transactionorigin_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_transactionorigin_createdonbehalfby = "lk_msdyn_transactionorigin_createdonbehalfby";
            [Relationship("msdyn_transactionorigin", EntityRole.Referenced, "lk_msdyn_transactionorigin_modifiedby", "modifiedby")]
            public const string lk_msdyn_transactionorigin_modifiedby = "lk_msdyn_transactionorigin_modifiedby";
            [Relationship("msdyn_transactionorigin", EntityRole.Referenced, "lk_msdyn_transactionorigin_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_transactionorigin_modifiedonbehalfby = "lk_msdyn_transactionorigin_modifiedonbehalfby";
            [Relationship("msdyn_unifiedroutingsetuptracker", EntityRole.Referenced, "lk_msdyn_unifiedroutingsetuptracker_createdby", "createdby")]
            public const string lk_msdyn_unifiedroutingsetuptracker_createdby = "lk_msdyn_unifiedroutingsetuptracker_createdby";
            [Relationship("msdyn_unifiedroutingsetuptracker", EntityRole.Referenced, "lk_msdyn_unifiedroutingsetuptracker_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_unifiedroutingsetuptracker_createdonbehalfby = "lk_msdyn_unifiedroutingsetuptracker_createdonbehalfby";
            [Relationship("msdyn_unifiedroutingsetuptracker", EntityRole.Referenced, "lk_msdyn_unifiedroutingsetuptracker_modifiedby", "modifiedby")]
            public const string lk_msdyn_unifiedroutingsetuptracker_modifiedby = "lk_msdyn_unifiedroutingsetuptracker_modifiedby";
            [Relationship("msdyn_unifiedroutingsetuptracker", EntityRole.Referenced, "lk_msdyn_unifiedroutingsetuptracker_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_unifiedroutingsetuptracker_modifiedonbehalfby = "lk_msdyn_unifiedroutingsetuptracker_modifiedonbehalfby";
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
            [Relationship("msdyn_usagemetric", EntityRole.Referenced, "lk_msdyn_usagemetric_createdby", "createdby")]
            public const string lk_msdyn_usagemetric_createdby = "lk_msdyn_usagemetric_createdby";
            [Relationship("msdyn_usagemetric", EntityRole.Referenced, "lk_msdyn_usagemetric_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_usagemetric_createdonbehalfby = "lk_msdyn_usagemetric_createdonbehalfby";
            [Relationship("msdyn_usagemetric", EntityRole.Referenced, "lk_msdyn_usagemetric_modifiedby", "modifiedby")]
            public const string lk_msdyn_usagemetric_modifiedby = "lk_msdyn_usagemetric_modifiedby";
            [Relationship("msdyn_usagemetric", EntityRole.Referenced, "lk_msdyn_usagemetric_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_usagemetric_modifiedonbehalfby = "lk_msdyn_usagemetric_modifiedonbehalfby";
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
            [Relationship("msdyn_wkwconfig", EntityRole.Referenced, "lk_msdyn_wkwconfig_createdby", "createdby")]
            public const string lk_msdyn_wkwconfig_createdby = "lk_msdyn_wkwconfig_createdby";
            [Relationship("msdyn_wkwconfig", EntityRole.Referenced, "lk_msdyn_wkwconfig_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_wkwconfig_createdonbehalfby = "lk_msdyn_wkwconfig_createdonbehalfby";
            [Relationship("msdyn_wkwconfig", EntityRole.Referenced, "lk_msdyn_wkwconfig_modifiedby", "modifiedby")]
            public const string lk_msdyn_wkwconfig_modifiedby = "lk_msdyn_wkwconfig_modifiedby";
            [Relationship("msdyn_wkwconfig", EntityRole.Referenced, "lk_msdyn_wkwconfig_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_wkwconfig_modifiedonbehalfby = "lk_msdyn_wkwconfig_modifiedonbehalfby";
            [Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "lk_msdyn_workhourtemplate_createdby", "createdby")]
            public const string lk_msdyn_workhourtemplate_createdby = "lk_msdyn_workhourtemplate_createdby";
            [Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "lk_msdyn_workhourtemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_workhourtemplate_createdonbehalfby = "lk_msdyn_workhourtemplate_createdonbehalfby";
            [Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "lk_msdyn_workhourtemplate_modifiedby", "modifiedby")]
            public const string lk_msdyn_workhourtemplate_modifiedby = "lk_msdyn_workhourtemplate_modifiedby";
            [Relationship("msdyn_workhourtemplate", EntityRole.Referenced, "lk_msdyn_workhourtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_workhourtemplate_modifiedonbehalfby = "lk_msdyn_workhourtemplate_modifiedonbehalfby";
            [Relationship("msdyn_worklistviewconfiguration", EntityRole.Referenced, "lk_msdyn_worklistviewconfiguration_createdby", "createdby")]
            public const string lk_msdyn_worklistviewconfiguration_createdby = "lk_msdyn_worklistviewconfiguration_createdby";
            [Relationship("msdyn_worklistviewconfiguration", EntityRole.Referenced, "lk_msdyn_worklistviewconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_worklistviewconfiguration_createdonbehalfby = "lk_msdyn_worklistviewconfiguration_createdonbehalfby";
            [Relationship("msdyn_worklistviewconfiguration", EntityRole.Referenced, "lk_msdyn_worklistviewconfiguration_modifiedby", "modifiedby")]
            public const string lk_msdyn_worklistviewconfiguration_modifiedby = "lk_msdyn_worklistviewconfiguration_modifiedby";
            [Relationship("msdyn_worklistviewconfiguration", EntityRole.Referenced, "lk_msdyn_worklistviewconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_worklistviewconfiguration_modifiedonbehalfby = "lk_msdyn_worklistviewconfiguration_modifiedonbehalfby";
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
            [Relationship("msdyn_workorderresolution", EntityRole.Referenced, "lk_msdyn_workorderresolution_createdby", "createdby")]
            public const string lk_msdyn_workorderresolution_createdby = "lk_msdyn_workorderresolution_createdby";
            [Relationship("msdyn_workorderresolution", EntityRole.Referenced, "lk_msdyn_workorderresolution_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_workorderresolution_createdonbehalfby = "lk_msdyn_workorderresolution_createdonbehalfby";
            [Relationship("msdyn_workorderresolution", EntityRole.Referenced, "lk_msdyn_workorderresolution_modifiedby", "modifiedby")]
            public const string lk_msdyn_workorderresolution_modifiedby = "lk_msdyn_workorderresolution_modifiedby";
            [Relationship("msdyn_workorderresolution", EntityRole.Referenced, "lk_msdyn_workorderresolution_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_workorderresolution_modifiedonbehalfby = "lk_msdyn_workorderresolution_modifiedonbehalfby";
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
            [Relationship("msdyn_workqueuestate", EntityRole.Referenced, "lk_msdyn_workqueuestate_createdby", "createdby")]
            public const string lk_msdyn_workqueuestate_createdby = "lk_msdyn_workqueuestate_createdby";
            [Relationship("msdyn_workqueuestate", EntityRole.Referenced, "lk_msdyn_workqueuestate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_workqueuestate_createdonbehalfby = "lk_msdyn_workqueuestate_createdonbehalfby";
            [Relationship("msdyn_workqueuestate", EntityRole.Referenced, "lk_msdyn_workqueuestate_modifiedby", "modifiedby")]
            public const string lk_msdyn_workqueuestate_modifiedby = "lk_msdyn_workqueuestate_modifiedby";
            [Relationship("msdyn_workqueuestate", EntityRole.Referenced, "lk_msdyn_workqueuestate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_workqueuestate_modifiedonbehalfby = "lk_msdyn_workqueuestate_modifiedonbehalfby";
            [Relationship("msdyn_workqueueusersetting", EntityRole.Referenced, "lk_msdyn_workqueueusersetting_createdby", "createdby")]
            public const string lk_msdyn_workqueueusersetting_createdby = "lk_msdyn_workqueueusersetting_createdby";
            [Relationship("msdyn_workqueueusersetting", EntityRole.Referenced, "lk_msdyn_workqueueusersetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdyn_workqueueusersetting_createdonbehalfby = "lk_msdyn_workqueueusersetting_createdonbehalfby";
            [Relationship("msdyn_workqueueusersetting", EntityRole.Referenced, "lk_msdyn_workqueueusersetting_modifiedby", "modifiedby")]
            public const string lk_msdyn_workqueueusersetting_modifiedby = "lk_msdyn_workqueueusersetting_modifiedby";
            [Relationship("msdyn_workqueueusersetting", EntityRole.Referenced, "lk_msdyn_workqueueusersetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdyn_workqueueusersetting_modifiedonbehalfby = "lk_msdyn_workqueueusersetting_modifiedonbehalfby";
            [Relationship("msdynce_botcontent", EntityRole.Referenced, "lk_msdynce_botcontent_createdby", "createdby")]
            public const string lk_msdynce_botcontent_createdby = "lk_msdynce_botcontent_createdby";
            [Relationship("msdynce_botcontent", EntityRole.Referenced, "lk_msdynce_botcontent_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msdynce_botcontent_createdonbehalfby = "lk_msdynce_botcontent_createdonbehalfby";
            [Relationship("msdynce_botcontent", EntityRole.Referenced, "lk_msdynce_botcontent_modifiedby", "modifiedby")]
            public const string lk_msdynce_botcontent_modifiedby = "lk_msdynce_botcontent_modifiedby";
            [Relationship("msdynce_botcontent", EntityRole.Referenced, "lk_msdynce_botcontent_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msdynce_botcontent_modifiedonbehalfby = "lk_msdynce_botcontent_modifiedonbehalfby";
            [Relationship("msfp_alertrule", EntityRole.Referenced, "lk_msfp_alertrule_createdby", "createdby")]
            public const string lk_msfp_alertrule_createdby = "lk_msfp_alertrule_createdby";
            [Relationship("msfp_alertrule", EntityRole.Referenced, "lk_msfp_alertrule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msfp_alertrule_createdonbehalfby = "lk_msfp_alertrule_createdonbehalfby";
            [Relationship("msfp_alertrule", EntityRole.Referenced, "lk_msfp_alertrule_modifiedby", "modifiedby")]
            public const string lk_msfp_alertrule_modifiedby = "lk_msfp_alertrule_modifiedby";
            [Relationship("msfp_alertrule", EntityRole.Referenced, "lk_msfp_alertrule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msfp_alertrule_modifiedonbehalfby = "lk_msfp_alertrule_modifiedonbehalfby";
            [Relationship("msfp_emailtemplate", EntityRole.Referenced, "lk_msfp_emailtemplate_createdby", "createdby")]
            public const string lk_msfp_emailtemplate_createdby = "lk_msfp_emailtemplate_createdby";
            [Relationship("msfp_emailtemplate", EntityRole.Referenced, "lk_msfp_emailtemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msfp_emailtemplate_createdonbehalfby = "lk_msfp_emailtemplate_createdonbehalfby";
            [Relationship("msfp_emailtemplate", EntityRole.Referenced, "lk_msfp_emailtemplate_modifiedby", "modifiedby")]
            public const string lk_msfp_emailtemplate_modifiedby = "lk_msfp_emailtemplate_modifiedby";
            [Relationship("msfp_emailtemplate", EntityRole.Referenced, "lk_msfp_emailtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msfp_emailtemplate_modifiedonbehalfby = "lk_msfp_emailtemplate_modifiedonbehalfby";
            [Relationship("msfp_fileresponse", EntityRole.Referenced, "lk_msfp_fileresponse_createdby", "createdby")]
            public const string lk_msfp_fileresponse_createdby = "lk_msfp_fileresponse_createdby";
            [Relationship("msfp_fileresponse", EntityRole.Referenced, "lk_msfp_fileresponse_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msfp_fileresponse_createdonbehalfby = "lk_msfp_fileresponse_createdonbehalfby";
            [Relationship("msfp_fileresponse", EntityRole.Referenced, "lk_msfp_fileresponse_modifiedby", "modifiedby")]
            public const string lk_msfp_fileresponse_modifiedby = "lk_msfp_fileresponse_modifiedby";
            [Relationship("msfp_fileresponse", EntityRole.Referenced, "lk_msfp_fileresponse_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msfp_fileresponse_modifiedonbehalfby = "lk_msfp_fileresponse_modifiedonbehalfby";
            [Relationship("msfp_localizedemailtemplate", EntityRole.Referenced, "lk_msfp_localizedemailtemplate_createdby", "createdby")]
            public const string lk_msfp_localizedemailtemplate_createdby = "lk_msfp_localizedemailtemplate_createdby";
            [Relationship("msfp_localizedemailtemplate", EntityRole.Referenced, "lk_msfp_localizedemailtemplate_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msfp_localizedemailtemplate_createdonbehalfby = "lk_msfp_localizedemailtemplate_createdonbehalfby";
            [Relationship("msfp_localizedemailtemplate", EntityRole.Referenced, "lk_msfp_localizedemailtemplate_modifiedby", "modifiedby")]
            public const string lk_msfp_localizedemailtemplate_modifiedby = "lk_msfp_localizedemailtemplate_modifiedby";
            [Relationship("msfp_localizedemailtemplate", EntityRole.Referenced, "lk_msfp_localizedemailtemplate_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msfp_localizedemailtemplate_modifiedonbehalfby = "lk_msfp_localizedemailtemplate_modifiedonbehalfby";
            [Relationship("msfp_project", EntityRole.Referenced, "lk_msfp_project_createdby", "createdby")]
            public const string lk_msfp_project_createdby = "lk_msfp_project_createdby";
            [Relationship("msfp_project", EntityRole.Referenced, "lk_msfp_project_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msfp_project_createdonbehalfby = "lk_msfp_project_createdonbehalfby";
            [Relationship("msfp_project", EntityRole.Referenced, "lk_msfp_project_modifiedby", "modifiedby")]
            public const string lk_msfp_project_modifiedby = "lk_msfp_project_modifiedby";
            [Relationship("msfp_project", EntityRole.Referenced, "lk_msfp_project_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msfp_project_modifiedonbehalfby = "lk_msfp_project_modifiedonbehalfby";
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
            [Relationship("msfp_satisfactionmetric", EntityRole.Referenced, "lk_msfp_satisfactionmetric_createdby", "createdby")]
            public const string lk_msfp_satisfactionmetric_createdby = "lk_msfp_satisfactionmetric_createdby";
            [Relationship("msfp_satisfactionmetric", EntityRole.Referenced, "lk_msfp_satisfactionmetric_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msfp_satisfactionmetric_createdonbehalfby = "lk_msfp_satisfactionmetric_createdonbehalfby";
            [Relationship("msfp_satisfactionmetric", EntityRole.Referenced, "lk_msfp_satisfactionmetric_modifiedby", "modifiedby")]
            public const string lk_msfp_satisfactionmetric_modifiedby = "lk_msfp_satisfactionmetric_modifiedby";
            [Relationship("msfp_satisfactionmetric", EntityRole.Referenced, "lk_msfp_satisfactionmetric_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msfp_satisfactionmetric_modifiedonbehalfby = "lk_msfp_satisfactionmetric_modifiedonbehalfby";
            [Relationship("msfp_survey", EntityRole.Referenced, "lk_msfp_survey_createdby", "createdby")]
            public const string lk_msfp_survey_createdby = "lk_msfp_survey_createdby";
            [Relationship("msfp_survey", EntityRole.Referenced, "lk_msfp_survey_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msfp_survey_createdonbehalfby = "lk_msfp_survey_createdonbehalfby";
            [Relationship("msfp_survey", EntityRole.Referenced, "lk_msfp_survey_modifiedby", "modifiedby")]
            public const string lk_msfp_survey_modifiedby = "lk_msfp_survey_modifiedby";
            [Relationship("msfp_survey", EntityRole.Referenced, "lk_msfp_survey_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msfp_survey_modifiedonbehalfby = "lk_msfp_survey_modifiedonbehalfby";
            [Relationship("msfp_surveyreminder", EntityRole.Referenced, "lk_msfp_surveyreminder_createdby", "createdby")]
            public const string lk_msfp_surveyreminder_createdby = "lk_msfp_surveyreminder_createdby";
            [Relationship("msfp_surveyreminder", EntityRole.Referenced, "lk_msfp_surveyreminder_createdonbehalfby", "createdonbehalfby")]
            public const string lk_msfp_surveyreminder_createdonbehalfby = "lk_msfp_surveyreminder_createdonbehalfby";
            [Relationship("msfp_surveyreminder", EntityRole.Referenced, "lk_msfp_surveyreminder_modifiedby", "modifiedby")]
            public const string lk_msfp_surveyreminder_modifiedby = "lk_msfp_surveyreminder_modifiedby";
            [Relationship("msfp_surveyreminder", EntityRole.Referenced, "lk_msfp_surveyreminder_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_msfp_surveyreminder_modifiedonbehalfby = "lk_msfp_surveyreminder_modifiedonbehalfby";
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
            [Relationship("new_etudient", EntityRole.Referenced, "lk_new_etudient_createdby", "createdby")]
            public const string lk_new_etudient_createdby = "lk_new_etudient_createdby";
            [Relationship("new_etudient", EntityRole.Referenced, "lk_new_etudient_createdonbehalfby", "createdonbehalfby")]
            public const string lk_new_etudient_createdonbehalfby = "lk_new_etudient_createdonbehalfby";
            [Relationship("new_etudient", EntityRole.Referenced, "lk_new_etudient_modifiedby", "modifiedby")]
            public const string lk_new_etudient_modifiedby = "lk_new_etudient_modifiedby";
            [Relationship("new_etudient", EntityRole.Referenced, "lk_new_etudient_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_new_etudient_modifiedonbehalfby = "lk_new_etudient_modifiedonbehalfby";
            [Relationship("new_m_apply", EntityRole.Referenced, "lk_new_m_apply_createdby", "createdby")]
            public const string lk_new_m_apply_createdby = "lk_new_m_apply_createdby";
            [Relationship("new_m_apply", EntityRole.Referenced, "lk_new_m_apply_createdonbehalfby", "createdonbehalfby")]
            public const string lk_new_m_apply_createdonbehalfby = "lk_new_m_apply_createdonbehalfby";
            [Relationship("new_m_apply", EntityRole.Referenced, "lk_new_m_apply_modifiedby", "modifiedby")]
            public const string lk_new_m_apply_modifiedby = "lk_new_m_apply_modifiedby";
            [Relationship("new_m_apply", EntityRole.Referenced, "lk_new_m_apply_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_new_m_apply_modifiedonbehalfby = "lk_new_m_apply_modifiedonbehalfby";
            [Relationship("new_m_grade", EntityRole.Referenced, "lk_new_m_grade_createdby", "createdby")]
            public const string lk_new_m_grade_createdby = "lk_new_m_grade_createdby";
            [Relationship("new_m_grade", EntityRole.Referenced, "lk_new_m_grade_createdonbehalfby", "createdonbehalfby")]
            public const string lk_new_m_grade_createdonbehalfby = "lk_new_m_grade_createdonbehalfby";
            [Relationship("new_m_grade", EntityRole.Referenced, "lk_new_m_grade_modifiedby", "modifiedby")]
            public const string lk_new_m_grade_modifiedby = "lk_new_m_grade_modifiedby";
            [Relationship("new_m_grade", EntityRole.Referenced, "lk_new_m_grade_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_new_m_grade_modifiedonbehalfby = "lk_new_m_grade_modifiedonbehalfby";
            [Relationship("new_m_lesson", EntityRole.Referenced, "lk_new_m_lesson_createdby", "createdby")]
            public const string lk_new_m_lesson_createdby = "lk_new_m_lesson_createdby";
            [Relationship("new_m_lesson", EntityRole.Referenced, "lk_new_m_lesson_createdonbehalfby", "createdonbehalfby")]
            public const string lk_new_m_lesson_createdonbehalfby = "lk_new_m_lesson_createdonbehalfby";
            [Relationship("new_m_lesson", EntityRole.Referenced, "lk_new_m_lesson_modifiedby", "modifiedby")]
            public const string lk_new_m_lesson_modifiedby = "lk_new_m_lesson_modifiedby";
            [Relationship("new_m_lesson", EntityRole.Referenced, "lk_new_m_lesson_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_new_m_lesson_modifiedonbehalfby = "lk_new_m_lesson_modifiedonbehalfby";
            [Relationship("new_m_school", EntityRole.Referenced, "lk_new_m_school_createdby", "createdby")]
            public const string lk_new_m_school_createdby = "lk_new_m_school_createdby";
            [Relationship("new_m_school", EntityRole.Referenced, "lk_new_m_school_createdonbehalfby", "createdonbehalfby")]
            public const string lk_new_m_school_createdonbehalfby = "lk_new_m_school_createdonbehalfby";
            [Relationship("new_m_school", EntityRole.Referenced, "lk_new_m_school_modifiedby", "modifiedby")]
            public const string lk_new_m_school_modifiedby = "lk_new_m_school_modifiedby";
            [Relationship("new_m_school", EntityRole.Referenced, "lk_new_m_school_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_new_m_school_modifiedonbehalfby = "lk_new_m_school_modifiedonbehalfby";
            [Relationship("new_m_student", EntityRole.Referenced, "lk_new_m_student_createdby", "createdby")]
            public const string lk_new_m_student_createdby = "lk_new_m_student_createdby";
            [Relationship("new_m_student", EntityRole.Referenced, "lk_new_m_student_createdonbehalfby", "createdonbehalfby")]
            public const string lk_new_m_student_createdonbehalfby = "lk_new_m_student_createdonbehalfby";
            [Relationship("new_m_student", EntityRole.Referenced, "lk_new_m_student_modifiedby", "modifiedby")]
            public const string lk_new_m_student_modifiedby = "lk_new_m_student_modifiedby";
            [Relationship("new_m_student", EntityRole.Referenced, "lk_new_m_student_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_new_m_student_modifiedonbehalfby = "lk_new_m_student_modifiedonbehalfby";
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
            [Relationship("organizationdatasyncsubscription", EntityRole.Referenced, "lk_organizationdatasyncsubscription_createdby", "createdby")]
            public const string lk_organizationdatasyncsubscription_createdby = "lk_organizationdatasyncsubscription_createdby";
            [Relationship("organizationdatasyncsubscription", EntityRole.Referenced, "lk_organizationdatasyncsubscription_createdonbehalfby", "createdonbehalfby")]
            public const string lk_organizationdatasyncsubscription_createdonbehalfby = "lk_organizationdatasyncsubscription_createdonbehalfby";
            [Relationship("organizationdatasyncsubscription", EntityRole.Referenced, "lk_organizationdatasyncsubscription_modifiedby", "modifiedby")]
            public const string lk_organizationdatasyncsubscription_modifiedby = "lk_organizationdatasyncsubscription_modifiedby";
            [Relationship("organizationdatasyncsubscription", EntityRole.Referenced, "lk_organizationdatasyncsubscription_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_organizationdatasyncsubscription_modifiedonbehalfby = "lk_organizationdatasyncsubscription_modifiedonbehalfby";
            [Relationship("organizationdatasyncsubscriptionentity", EntityRole.Referenced, "lk_organizationdatasyncsubscriptionentity_createdby", "createdby")]
            public const string lk_organizationdatasyncsubscriptionentity_createdby = "lk_organizationdatasyncsubscriptionentity_createdby";
            [Relationship("organizationdatasyncsubscriptionentity", EntityRole.Referenced, "lk_organizationdatasyncsubscriptionentity_createdonbehalfby", "createdonbehalfby")]
            public const string lk_organizationdatasyncsubscriptionentity_createdonbehalfby = "lk_organizationdatasyncsubscriptionentity_createdonbehalfby";
            [Relationship("organizationdatasyncsubscriptionentity", EntityRole.Referenced, "lk_organizationdatasyncsubscriptionentity_modifiedby", "modifiedby")]
            public const string lk_organizationdatasyncsubscriptionentity_modifiedby = "lk_organizationdatasyncsubscriptionentity_modifiedby";
            [Relationship("organizationdatasyncsubscriptionentity", EntityRole.Referenced, "lk_organizationdatasyncsubscriptionentity_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_organizationdatasyncsubscriptionentity_modifiedonbehalfby = "lk_organizationdatasyncsubscriptionentity_modifiedonbehalfby";
            [Relationship("organizationsetting", EntityRole.Referenced, "lk_organizationsetting_createdby", "createdby")]
            public const string lk_organizationsetting_createdby = "lk_organizationsetting_createdby";
            [Relationship("organizationsetting", EntityRole.Referenced, "lk_organizationsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_organizationsetting_createdonbehalfby = "lk_organizationsetting_createdonbehalfby";
            [Relationship("organizationsetting", EntityRole.Referenced, "lk_organizationsetting_modifiedby", "modifiedby")]
            public const string lk_organizationsetting_modifiedby = "lk_organizationsetting_modifiedby";
            [Relationship("organizationsetting", EntityRole.Referenced, "lk_organizationsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_organizationsetting_modifiedonbehalfby = "lk_organizationsetting_modifiedonbehalfby";
            [Relationship("ownermapping", EntityRole.Referenced, "lk_ownermapping_createdby", "createdby")]
            public const string lk_ownermapping_createdby = "lk_ownermapping_createdby";
            [Relationship("ownermapping", EntityRole.Referenced, "lk_ownermapping_createdonbehalfby", "createdonbehalfby")]
            public const string lk_ownermapping_createdonbehalfby = "lk_ownermapping_createdonbehalfby";
            [Relationship("ownermapping", EntityRole.Referenced, "lk_ownermapping_modifiedby", "modifiedby")]
            public const string lk_ownermapping_modifiedby = "lk_ownermapping_modifiedby";
            [Relationship("ownermapping", EntityRole.Referenced, "lk_ownermapping_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ownermapping_modifiedonbehalfby = "lk_ownermapping_modifiedonbehalfby";
            [Relationship("package", EntityRole.Referenced, "lk_package_createdby", "createdby")]
            public const string lk_package_createdby = "lk_package_createdby";
            [Relationship("package", EntityRole.Referenced, "lk_package_createdonbehalfby", "createdonbehalfby")]
            public const string lk_package_createdonbehalfby = "lk_package_createdonbehalfby";
            [Relationship("package", EntityRole.Referenced, "lk_package_modifiedby", "modifiedby")]
            public const string lk_package_modifiedby = "lk_package_modifiedby";
            [Relationship("package", EntityRole.Referenced, "lk_package_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_package_modifiedonbehalfby = "lk_package_modifiedonbehalfby";
            [Relationship("partnerapplication", EntityRole.Referenced, "lk_partnerapplication_createdby", "createdby")]
            public const string lk_partnerapplication_createdby = "lk_partnerapplication_createdby";
            [Relationship("partnerapplication", EntityRole.Referenced, "lk_partnerapplication_createdonbehalfby", "createdonbehalfby")]
            public const string lk_partnerapplication_createdonbehalfby = "lk_partnerapplication_createdonbehalfby";
            [Relationship("partnerapplication", EntityRole.Referenced, "lk_partnerapplication_modifiedby", "modifiedby")]
            public const string lk_partnerapplication_modifiedby = "lk_partnerapplication_modifiedby";
            [Relationship("partnerapplication", EntityRole.Referenced, "lk_partnerapplication_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_partnerapplication_modifiedonbehalfby = "lk_partnerapplication_modifiedonbehalfby";
            [Relationship("pdfsetting", EntityRole.Referenced, "lk_pdfsetting_createdby", "createdby")]
            public const string lk_pdfsetting_createdby = "lk_pdfsetting_createdby";
            [Relationship("pdfsetting", EntityRole.Referenced, "lk_pdfsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_pdfsetting_createdonbehalfby = "lk_pdfsetting_createdonbehalfby";
            [Relationship("pdfsetting", EntityRole.Referenced, "lk_pdfsetting_modifiedby", "modifiedby")]
            public const string lk_pdfsetting_modifiedby = "lk_pdfsetting_modifiedby";
            [Relationship("pdfsetting", EntityRole.Referenced, "lk_pdfsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_pdfsetting_modifiedonbehalfby = "lk_pdfsetting_modifiedonbehalfby";
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
            [Relationship("pi_course", EntityRole.Referenced, "lk_pi_course_createdby", "createdby")]
            public const string lk_pi_course_createdby = "lk_pi_course_createdby";
            [Relationship("pi_course", EntityRole.Referenced, "lk_pi_course_createdonbehalfby", "createdonbehalfby")]
            public const string lk_pi_course_createdonbehalfby = "lk_pi_course_createdonbehalfby";
            [Relationship("pi_course", EntityRole.Referenced, "lk_pi_course_modifiedby", "modifiedby")]
            public const string lk_pi_course_modifiedby = "lk_pi_course_modifiedby";
            [Relationship("pi_course", EntityRole.Referenced, "lk_pi_course_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_pi_course_modifiedonbehalfby = "lk_pi_course_modifiedonbehalfby";
            [Relationship("pi_courseinscription", EntityRole.Referenced, "lk_pi_courseinscription_createdby", "createdby")]
            public const string lk_pi_courseinscription_createdby = "lk_pi_courseinscription_createdby";
            [Relationship("pi_courseinscription", EntityRole.Referenced, "lk_pi_courseinscription_createdonbehalfby", "createdonbehalfby")]
            public const string lk_pi_courseinscription_createdonbehalfby = "lk_pi_courseinscription_createdonbehalfby";
            [Relationship("pi_courseinscription", EntityRole.Referenced, "lk_pi_courseinscription_modifiedby", "modifiedby")]
            public const string lk_pi_courseinscription_modifiedby = "lk_pi_courseinscription_modifiedby";
            [Relationship("pi_courseinscription", EntityRole.Referenced, "lk_pi_courseinscription_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_pi_courseinscription_modifiedonbehalfby = "lk_pi_courseinscription_modifiedonbehalfby";
            [Relationship("pi_school", EntityRole.Referenced, "lk_pi_school_createdby", "createdby")]
            public const string lk_pi_school_createdby = "lk_pi_school_createdby";
            [Relationship("pi_school", EntityRole.Referenced, "lk_pi_school_createdonbehalfby", "createdonbehalfby")]
            public const string lk_pi_school_createdonbehalfby = "lk_pi_school_createdonbehalfby";
            [Relationship("pi_school", EntityRole.Referenced, "lk_pi_school_modifiedby", "modifiedby")]
            public const string lk_pi_school_modifiedby = "lk_pi_school_modifiedby";
            [Relationship("pi_school", EntityRole.Referenced, "lk_pi_school_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_pi_school_modifiedonbehalfby = "lk_pi_school_modifiedonbehalfby";
            [Relationship("pi_student", EntityRole.Referenced, "lk_pi_student_createdby", "createdby")]
            public const string lk_pi_student_createdby = "lk_pi_student_createdby";
            [Relationship("pi_student", EntityRole.Referenced, "lk_pi_student_createdonbehalfby", "createdonbehalfby")]
            public const string lk_pi_student_createdonbehalfby = "lk_pi_student_createdonbehalfby";
            [Relationship("pi_student", EntityRole.Referenced, "lk_pi_student_modifiedby", "modifiedby")]
            public const string lk_pi_student_modifiedby = "lk_pi_student_modifiedby";
            [Relationship("pi_student", EntityRole.Referenced, "lk_pi_student_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_pi_student_modifiedonbehalfby = "lk_pi_student_modifiedonbehalfby";
            [Relationship("pi_studentgroup", EntityRole.Referenced, "lk_pi_studentgroup_createdby", "createdby")]
            public const string lk_pi_studentgroup_createdby = "lk_pi_studentgroup_createdby";
            [Relationship("pi_studentgroup", EntityRole.Referenced, "lk_pi_studentgroup_createdonbehalfby", "createdonbehalfby")]
            public const string lk_pi_studentgroup_createdonbehalfby = "lk_pi_studentgroup_createdonbehalfby";
            [Relationship("pi_studentgroup", EntityRole.Referenced, "lk_pi_studentgroup_modifiedby", "modifiedby")]
            public const string lk_pi_studentgroup_modifiedby = "lk_pi_studentgroup_modifiedby";
            [Relationship("pi_studentgroup", EntityRole.Referenced, "lk_pi_studentgroup_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_pi_studentgroup_modifiedonbehalfby = "lk_pi_studentgroup_modifiedonbehalfby";
            [Relationship("picklistmapping", EntityRole.Referenced, "lk_picklistmapping_createdby", "createdby")]
            public const string lk_picklistmapping_createdby = "lk_picklistmapping_createdby";
            [Relationship("picklistmapping", EntityRole.Referenced, "lk_picklistmapping_createdonbehalfby", "createdonbehalfby")]
            public const string lk_picklistmapping_createdonbehalfby = "lk_picklistmapping_createdonbehalfby";
            [Relationship("picklistmapping", EntityRole.Referenced, "lk_picklistmapping_modifiedby", "modifiedby")]
            public const string lk_picklistmapping_modifiedby = "lk_picklistmapping_modifiedby";
            [Relationship("picklistmapping", EntityRole.Referenced, "lk_picklistmapping_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_picklistmapping_modifiedonbehalfby = "lk_picklistmapping_modifiedonbehalfby";
            [Relationship(PluginAssemblyDefinition.EntityName, EntityRole.Referenced, "lk_pluginassembly_createdonbehalfby", "createdonbehalfby")]
            public const string lk_pluginassembly_createdonbehalfby = "lk_pluginassembly_createdonbehalfby";
            [Relationship(PluginAssemblyDefinition.EntityName, EntityRole.Referenced, "lk_pluginassembly_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_pluginassembly_modifiedonbehalfby = "lk_pluginassembly_modifiedonbehalfby";
            [Relationship("pluginpackage", EntityRole.Referenced, "lk_pluginpackage_createdby", "createdby")]
            public const string lk_pluginpackage_createdby = "lk_pluginpackage_createdby";
            [Relationship("pluginpackage", EntityRole.Referenced, "lk_pluginpackage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_pluginpackage_createdonbehalfby = "lk_pluginpackage_createdonbehalfby";
            [Relationship("pluginpackage", EntityRole.Referenced, "lk_pluginpackage_modifiedby", "modifiedby")]
            public const string lk_pluginpackage_modifiedby = "lk_pluginpackage_modifiedby";
            [Relationship("pluginpackage", EntityRole.Referenced, "lk_pluginpackage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_pluginpackage_modifiedonbehalfby = "lk_pluginpackage_modifiedonbehalfby";
            [Relationship("plugintracelog", EntityRole.Referenced, "lk_plugintracelogbase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_plugintracelogbase_createdonbehalfby = "lk_plugintracelogbase_createdonbehalfby";
            [Relationship(PluginTypeDefinition.EntityName, EntityRole.Referenced, "lk_plugintype_createdonbehalfby", "createdonbehalfby")]
            public const string lk_plugintype_createdonbehalfby = "lk_plugintype_createdonbehalfby";
            [Relationship(PluginTypeDefinition.EntityName, EntityRole.Referenced, "lk_plugintype_modifiedonbehalfby", "modifiedonbehalfby")]
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
            [Relationship("privilegesremovalsetting", EntityRole.Referenced, "lk_privilegesremovalsetting_createdby", "createdby")]
            public const string lk_privilegesremovalsetting_createdby = "lk_privilegesremovalsetting_createdby";
            [Relationship("privilegesremovalsetting", EntityRole.Referenced, "lk_privilegesremovalsetting_createdonbehalfby", "createdonbehalfby")]
            public const string lk_privilegesremovalsetting_createdonbehalfby = "lk_privilegesremovalsetting_createdonbehalfby";
            [Relationship("privilegesremovalsetting", EntityRole.Referenced, "lk_privilegesremovalsetting_modifiedby", "modifiedby")]
            public const string lk_privilegesremovalsetting_modifiedby = "lk_privilegesremovalsetting_modifiedby";
            [Relationship("privilegesremovalsetting", EntityRole.Referenced, "lk_privilegesremovalsetting_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_privilegesremovalsetting_modifiedonbehalfby = "lk_privilegesremovalsetting_modifiedonbehalfby";
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
            [Relationship("processstageparameter", EntityRole.Referenced, "lk_processstageparameter_createdby", "createdby")]
            public const string lk_processstageparameter_createdby = "lk_processstageparameter_createdby";
            [Relationship("processstageparameter", EntityRole.Referenced, "lk_processstageparameter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_processstageparameter_createdonbehalfby = "lk_processstageparameter_createdonbehalfby";
            [Relationship("processstageparameter", EntityRole.Referenced, "lk_processstageparameter_modifiedby", "modifiedby")]
            public const string lk_processstageparameter_modifiedby = "lk_processstageparameter_modifiedby";
            [Relationship("processstageparameter", EntityRole.Referenced, "lk_processstageparameter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_processstageparameter_modifiedonbehalfby = "lk_processstageparameter_modifiedonbehalfby";
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
            [Relationship(PublisherDefinition.EntityName, EntityRole.Referenced, "lk_publisher_createdby", "createdby")]
            public const string lk_publisher_createdby = "lk_publisher_createdby";
            [Relationship(PublisherDefinition.EntityName, EntityRole.Referenced, "lk_publisher_modifiedby", "modifiedby")]
            public const string lk_publisher_modifiedby = "lk_publisher_modifiedby";
            [Relationship("publisheraddress", EntityRole.Referenced, "lk_publisheraddressbase_createdby", "createdby")]
            public const string lk_publisheraddressbase_createdby = "lk_publisheraddressbase_createdby";
            [Relationship("publisheraddress", EntityRole.Referenced, "lk_publisheraddressbase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_publisheraddressbase_createdonbehalfby = "lk_publisheraddressbase_createdonbehalfby";
            [Relationship("publisheraddress", EntityRole.Referenced, "lk_publisheraddressbase_modifiedby", "modifiedby")]
            public const string lk_publisheraddressbase_modifiedby = "lk_publisheraddressbase_modifiedby";
            [Relationship("publisheraddress", EntityRole.Referenced, "lk_publisheraddressbase_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_publisheraddressbase_modifiedonbehalfby = "lk_publisheraddressbase_modifiedonbehalfby";
            [Relationship(PublisherDefinition.EntityName, EntityRole.Referenced, "lk_publisherbase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_publisherbase_createdonbehalfby = "lk_publisherbase_createdonbehalfby";
            [Relationship(PublisherDefinition.EntityName, EntityRole.Referenced, "lk_publisherbase_modifiedonbehalfby", "modifiedonbehalfby")]
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
            [Relationship("revokeinheritedaccessrecordstracker", EntityRole.Referenced, "lk_revokeinheritedaccessrecordstracker_createdby", "createdby")]
            public const string lk_revokeinheritedaccessrecordstracker_createdby = "lk_revokeinheritedaccessrecordstracker_createdby";
            [Relationship("revokeinheritedaccessrecordstracker", EntityRole.Referenced, "lk_revokeinheritedaccessrecordstracker_createdonbehalfby", "createdonbehalfby")]
            public const string lk_revokeinheritedaccessrecordstracker_createdonbehalfby = "lk_revokeinheritedaccessrecordstracker_createdonbehalfby";
            [Relationship("revokeinheritedaccessrecordstracker", EntityRole.Referenced, "lk_revokeinheritedaccessrecordstracker_modifiedby", "modifiedby")]
            public const string lk_revokeinheritedaccessrecordstracker_modifiedby = "lk_revokeinheritedaccessrecordstracker_modifiedby";
            [Relationship("revokeinheritedaccessrecordstracker", EntityRole.Referenced, "lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby = "lk_revokeinheritedaccessrecordstracker_modifiedonbehalfby";
            [Relationship("ribboncommand", EntityRole.Referenced, "lk_ribboncommand_createdby", "createdby")]
            public const string lk_ribboncommand_createdby = "lk_ribboncommand_createdby";
            [Relationship("ribboncommand", EntityRole.Referenced, "lk_ribboncommand_createdonbehalfby", "createdonbehalfby")]
            public const string lk_ribboncommand_createdonbehalfby = "lk_ribboncommand_createdonbehalfby";
            [Relationship("ribboncommand", EntityRole.Referenced, "lk_ribboncommand_modifiedby", "modifiedby")]
            public const string lk_ribboncommand_modifiedby = "lk_ribboncommand_modifiedby";
            [Relationship("ribboncommand", EntityRole.Referenced, "lk_ribboncommand_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ribboncommand_modifiedonbehalfby = "lk_ribboncommand_modifiedonbehalfby";
            [Relationship("ribbonrule", EntityRole.Referenced, "lk_ribbonrule_createdby", "createdby")]
            public const string lk_ribbonrule_createdby = "lk_ribbonrule_createdby";
            [Relationship("ribbonrule", EntityRole.Referenced, "lk_ribbonrule_createdonbehalfby", "createdonbehalfby")]
            public const string lk_ribbonrule_createdonbehalfby = "lk_ribbonrule_createdonbehalfby";
            [Relationship("ribbonrule", EntityRole.Referenced, "lk_ribbonrule_modifiedby", "modifiedby")]
            public const string lk_ribbonrule_modifiedby = "lk_ribbonrule_modifiedby";
            [Relationship("ribbonrule", EntityRole.Referenced, "lk_ribbonrule_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ribbonrule_modifiedonbehalfby = "lk_ribbonrule_modifiedonbehalfby";
            [Relationship(RoleDefinition.EntityName, EntityRole.Referenced, "lk_role_createdonbehalfby", "createdonbehalfby")]
            public const string lk_role_createdonbehalfby = "lk_role_createdonbehalfby";
            [Relationship(RoleDefinition.EntityName, EntityRole.Referenced, "lk_role_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_role_modifiedonbehalfby = "lk_role_modifiedonbehalfby";
            [Relationship(RoleDefinition.EntityName, EntityRole.Referenced, "lk_rolebase_createdby", "createdby")]
            public const string lk_rolebase_createdby = "lk_rolebase_createdby";
            [Relationship(RoleDefinition.EntityName, EntityRole.Referenced, "lk_rolebase_modifiedby", "modifiedby")]
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
            [Relationship(SdkMessageDefinition.EntityName, EntityRole.Referenced, "lk_sdkmessage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_sdkmessage_createdonbehalfby = "lk_sdkmessage_createdonbehalfby";
            [Relationship(SdkMessageDefinition.EntityName, EntityRole.Referenced, "lk_sdkmessage_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_sdkmessage_modifiedonbehalfby = "lk_sdkmessage_modifiedonbehalfby";
            [Relationship(SdkMessageFilterDefinition.EntityName, EntityRole.Referenced, "lk_sdkmessagefilter_createdonbehalfby", "createdonbehalfby")]
            public const string lk_sdkmessagefilter_createdonbehalfby = "lk_sdkmessagefilter_createdonbehalfby";
            [Relationship(SdkMessageFilterDefinition.EntityName, EntityRole.Referenced, "lk_sdkmessagefilter_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_sdkmessagefilter_modifiedonbehalfby = "lk_sdkmessagefilter_modifiedonbehalfby";
            [Relationship("sdkmessagepair", EntityRole.Referenced, "lk_sdkmessagepair_createdonbehalfby", "createdonbehalfby")]
            public const string lk_sdkmessagepair_createdonbehalfby = "lk_sdkmessagepair_createdonbehalfby";
            [Relationship("sdkmessagepair", EntityRole.Referenced, "lk_sdkmessagepair_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_sdkmessagepair_modifiedonbehalfby = "lk_sdkmessagepair_modifiedonbehalfby";
            [Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "lk_sdkmessageprocessingstep_createdonbehalfby", "createdonbehalfby")]
            public const string lk_sdkmessageprocessingstep_createdonbehalfby = "lk_sdkmessageprocessingstep_createdonbehalfby";
            [Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "lk_sdkmessageprocessingstep_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_sdkmessageprocessingstep_modifiedonbehalfby = "lk_sdkmessageprocessingstep_modifiedonbehalfby";
            [Relationship(SdkMessageProcessingStepImageDefinition.EntityName, EntityRole.Referenced, "lk_sdkmessageprocessingstepimage_createdonbehalfby", "createdonbehalfby")]
            public const string lk_sdkmessageprocessingstepimage_createdonbehalfby = "lk_sdkmessageprocessingstepimage_createdonbehalfby";
            [Relationship(SdkMessageProcessingStepImageDefinition.EntityName, EntityRole.Referenced, "lk_sdkmessageprocessingstepimage_modifiedonbehalfby", "modifiedonbehalfby")]
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
            [Relationship("serviceplan", EntityRole.Referenced, "lk_serviceplan_createdby", "createdby")]
            public const string lk_serviceplan_createdby = "lk_serviceplan_createdby";
            [Relationship("serviceplan", EntityRole.Referenced, "lk_serviceplan_createdonbehalfby", "createdonbehalfby")]
            public const string lk_serviceplan_createdonbehalfby = "lk_serviceplan_createdonbehalfby";
            [Relationship("serviceplan", EntityRole.Referenced, "lk_serviceplan_modifiedby", "modifiedby")]
            public const string lk_serviceplan_modifiedby = "lk_serviceplan_modifiedby";
            [Relationship("serviceplan", EntityRole.Referenced, "lk_serviceplan_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_serviceplan_modifiedonbehalfby = "lk_serviceplan_modifiedonbehalfby";
            [Relationship("serviceplanmapping", EntityRole.Referenced, "lk_serviceplanmapping_createdby", "createdby")]
            public const string lk_serviceplanmapping_createdby = "lk_serviceplanmapping_createdby";
            [Relationship("serviceplanmapping", EntityRole.Referenced, "lk_serviceplanmapping_createdonbehalfby", "createdonbehalfby")]
            public const string lk_serviceplanmapping_createdonbehalfby = "lk_serviceplanmapping_createdonbehalfby";
            [Relationship("serviceplanmapping", EntityRole.Referenced, "lk_serviceplanmapping_modifiedby", "modifiedby")]
            public const string lk_serviceplanmapping_modifiedby = "lk_serviceplanmapping_modifiedby";
            [Relationship("serviceplanmapping", EntityRole.Referenced, "lk_serviceplanmapping_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_serviceplanmapping_modifiedonbehalfby = "lk_serviceplanmapping_modifiedonbehalfby";
            [Relationship("settingdefinition", EntityRole.Referenced, "lk_settingdefinition_createdby", "createdby")]
            public const string lk_settingdefinition_createdby = "lk_settingdefinition_createdby";
            [Relationship("settingdefinition", EntityRole.Referenced, "lk_settingdefinition_createdonbehalfby", "createdonbehalfby")]
            public const string lk_settingdefinition_createdonbehalfby = "lk_settingdefinition_createdonbehalfby";
            [Relationship("settingdefinition", EntityRole.Referenced, "lk_settingdefinition_modifiedby", "modifiedby")]
            public const string lk_settingdefinition_modifiedby = "lk_settingdefinition_modifiedby";
            [Relationship("settingdefinition", EntityRole.Referenced, "lk_settingdefinition_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_settingdefinition_modifiedonbehalfby = "lk_settingdefinition_modifiedonbehalfby";
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
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referenced, "lk_solution_createdby", "createdby")]
            public const string lk_solution_createdby = "lk_solution_createdby";
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referenced, "lk_solution_modifiedby", "modifiedby")]
            public const string lk_solution_modifiedby = "lk_solution_modifiedby";
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referenced, "lk_solutionbase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_solutionbase_createdonbehalfby = "lk_solutionbase_createdonbehalfby";
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referenced, "lk_solutionbase_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_solutionbase_modifiedonbehalfby = "lk_solutionbase_modifiedonbehalfby";
            [Relationship("solutioncomponentattributeconfiguration", EntityRole.Referenced, "lk_solutioncomponentattributeconfiguration_createdby", "createdby")]
            public const string lk_solutioncomponentattributeconfiguration_createdby = "lk_solutioncomponentattributeconfiguration_createdby";
            [Relationship("solutioncomponentattributeconfiguration", EntityRole.Referenced, "lk_solutioncomponentattributeconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_solutioncomponentattributeconfiguration_createdonbehalfby = "lk_solutioncomponentattributeconfiguration_createdonbehalfby";
            [Relationship("solutioncomponentattributeconfiguration", EntityRole.Referenced, "lk_solutioncomponentattributeconfiguration_modifiedby", "modifiedby")]
            public const string lk_solutioncomponentattributeconfiguration_modifiedby = "lk_solutioncomponentattributeconfiguration_modifiedby";
            [Relationship("solutioncomponentattributeconfiguration", EntityRole.Referenced, "lk_solutioncomponentattributeconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_solutioncomponentattributeconfiguration_modifiedonbehalfby = "lk_solutioncomponentattributeconfiguration_modifiedonbehalfby";
            [Relationship(SolutionComponentDefinition.EntityName, EntityRole.Referenced, "lk_solutioncomponentbase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_solutioncomponentbase_createdonbehalfby = "lk_solutioncomponentbase_createdonbehalfby";
            [Relationship(SolutionComponentDefinition.EntityName, EntityRole.Referenced, "lk_solutioncomponentbase_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_solutioncomponentbase_modifiedonbehalfby = "lk_solutioncomponentbase_modifiedonbehalfby";
            [Relationship("solutioncomponentbatchconfiguration", EntityRole.Referenced, "lk_solutioncomponentbatchconfiguration_createdby", "createdby")]
            public const string lk_solutioncomponentbatchconfiguration_createdby = "lk_solutioncomponentbatchconfiguration_createdby";
            [Relationship("solutioncomponentbatchconfiguration", EntityRole.Referenced, "lk_solutioncomponentbatchconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_solutioncomponentbatchconfiguration_createdonbehalfby = "lk_solutioncomponentbatchconfiguration_createdonbehalfby";
            [Relationship("solutioncomponentbatchconfiguration", EntityRole.Referenced, "lk_solutioncomponentbatchconfiguration_modifiedby", "modifiedby")]
            public const string lk_solutioncomponentbatchconfiguration_modifiedby = "lk_solutioncomponentbatchconfiguration_modifiedby";
            [Relationship("solutioncomponentbatchconfiguration", EntityRole.Referenced, "lk_solutioncomponentbatchconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_solutioncomponentbatchconfiguration_modifiedonbehalfby = "lk_solutioncomponentbatchconfiguration_modifiedonbehalfby";
            [Relationship("solutioncomponentconfiguration", EntityRole.Referenced, "lk_solutioncomponentconfiguration_createdby", "createdby")]
            public const string lk_solutioncomponentconfiguration_createdby = "lk_solutioncomponentconfiguration_createdby";
            [Relationship("solutioncomponentconfiguration", EntityRole.Referenced, "lk_solutioncomponentconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_solutioncomponentconfiguration_createdonbehalfby = "lk_solutioncomponentconfiguration_createdonbehalfby";
            [Relationship("solutioncomponentconfiguration", EntityRole.Referenced, "lk_solutioncomponentconfiguration_modifiedby", "modifiedby")]
            public const string lk_solutioncomponentconfiguration_modifiedby = "lk_solutioncomponentconfiguration_modifiedby";
            [Relationship("solutioncomponentconfiguration", EntityRole.Referenced, "lk_solutioncomponentconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_solutioncomponentconfiguration_modifiedonbehalfby = "lk_solutioncomponentconfiguration_modifiedonbehalfby";
            [Relationship("solutioncomponentrelationshipconfiguration", EntityRole.Referenced, "lk_solutioncomponentrelationshipconfiguration_createdby", "createdby")]
            public const string lk_solutioncomponentrelationshipconfiguration_createdby = "lk_solutioncomponentrelationshipconfiguration_createdby";
            [Relationship("solutioncomponentrelationshipconfiguration", EntityRole.Referenced, "lk_solutioncomponentrelationshipconfiguration_createdonbehalfby", "createdonbehalfby")]
            public const string lk_solutioncomponentrelationshipconfiguration_createdonbehalfby = "lk_solutioncomponentrelationshipconfiguration_createdonbehalfby";
            [Relationship("solutioncomponentrelationshipconfiguration", EntityRole.Referenced, "lk_solutioncomponentrelationshipconfiguration_modifiedby", "modifiedby")]
            public const string lk_solutioncomponentrelationshipconfiguration_modifiedby = "lk_solutioncomponentrelationshipconfiguration_modifiedby";
            [Relationship("solutioncomponentrelationshipconfiguration", EntityRole.Referenced, "lk_solutioncomponentrelationshipconfiguration_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_solutioncomponentrelationshipconfiguration_modifiedonbehalfby = "lk_solutioncomponentrelationshipconfiguration_modifiedonbehalfby";
            [Relationship("stagesolutionupload", EntityRole.Referenced, "lk_stagesolutionupload_createdby", "createdby")]
            public const string lk_stagesolutionupload_createdby = "lk_stagesolutionupload_createdby";
            [Relationship("stagesolutionupload", EntityRole.Referenced, "lk_stagesolutionupload_createdonbehalfby", "createdonbehalfby")]
            public const string lk_stagesolutionupload_createdonbehalfby = "lk_stagesolutionupload_createdonbehalfby";
            [Relationship("stagesolutionupload", EntityRole.Referenced, "lk_stagesolutionupload_modifiedby", "modifiedby")]
            public const string lk_stagesolutionupload_modifiedby = "lk_stagesolutionupload_modifiedby";
            [Relationship("stagesolutionupload", EntityRole.Referenced, "lk_stagesolutionupload_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_stagesolutionupload_modifiedonbehalfby = "lk_stagesolutionupload_modifiedonbehalfby";
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
            [Relationship("synapsedatabase", EntityRole.Referenced, "lk_synapsedatabase_createdby", "createdby")]
            public const string lk_synapsedatabase_createdby = "lk_synapsedatabase_createdby";
            [Relationship("synapsedatabase", EntityRole.Referenced, "lk_synapsedatabase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_synapsedatabase_createdonbehalfby = "lk_synapsedatabase_createdonbehalfby";
            [Relationship("synapsedatabase", EntityRole.Referenced, "lk_synapsedatabase_modifiedby", "modifiedby")]
            public const string lk_synapsedatabase_modifiedby = "lk_synapsedatabase_modifiedby";
            [Relationship("synapsedatabase", EntityRole.Referenced, "lk_synapsedatabase_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_synapsedatabase_modifiedonbehalfby = "lk_synapsedatabase_modifiedonbehalfby";
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
            [Relationship("teammobileofflineprofilemembership", EntityRole.Referenced, "lk_teammobileofflineprofilemembership_createdby", "createdby")]
            public const string lk_teammobileofflineprofilemembership_createdby = "lk_teammobileofflineprofilemembership_createdby";
            [Relationship("teammobileofflineprofilemembership", EntityRole.Referenced, "lk_teammobileofflineprofilemembership_createdonbehalfby", "createdonbehalfby")]
            public const string lk_teammobileofflineprofilemembership_createdonbehalfby = "lk_teammobileofflineprofilemembership_createdonbehalfby";
            [Relationship("teammobileofflineprofilemembership", EntityRole.Referenced, "lk_teammobileofflineprofilemembership_modifiedby", "modifiedby")]
            public const string lk_teammobileofflineprofilemembership_modifiedby = "lk_teammobileofflineprofilemembership_modifiedby";
            [Relationship("teammobileofflineprofilemembership", EntityRole.Referenced, "lk_teammobileofflineprofilemembership_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_teammobileofflineprofilemembership_modifiedonbehalfby = "lk_teammobileofflineprofilemembership_modifiedonbehalfby";
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
            [Relationship(TransactionCurrencyDefinition.EntityName, EntityRole.Referenced, "lk_transactioncurrency_createdonbehalfby", "createdonbehalfby")]
            public const string lk_transactioncurrency_createdonbehalfby = "lk_transactioncurrency_createdonbehalfby";
            [Relationship(TransactionCurrencyDefinition.EntityName, EntityRole.Referenced, "lk_transactioncurrency_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_transactioncurrency_modifiedonbehalfby = "lk_transactioncurrency_modifiedonbehalfby";
            [Relationship(TransactionCurrencyDefinition.EntityName, EntityRole.Referenced, "lk_transactioncurrencybase_createdby", "createdby")]
            public const string lk_transactioncurrencybase_createdby = "lk_transactioncurrencybase_createdby";
            [Relationship(TransactionCurrencyDefinition.EntityName, EntityRole.Referenced, "lk_transactioncurrencybase_modifiedby", "modifiedby")]
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
            [Relationship("usermobileofflineprofilemembership", EntityRole.Referenced, "lk_usermobileofflineprofilemembership_createdby", "createdby")]
            public const string lk_usermobileofflineprofilemembership_createdby = "lk_usermobileofflineprofilemembership_createdby";
            [Relationship("usermobileofflineprofilemembership", EntityRole.Referenced, "lk_usermobileofflineprofilemembership_createdonbehalfby", "createdonbehalfby")]
            public const string lk_usermobileofflineprofilemembership_createdonbehalfby = "lk_usermobileofflineprofilemembership_createdonbehalfby";
            [Relationship("usermobileofflineprofilemembership", EntityRole.Referenced, "lk_usermobileofflineprofilemembership_modifiedby", "modifiedby")]
            public const string lk_usermobileofflineprofilemembership_modifiedby = "lk_usermobileofflineprofilemembership_modifiedby";
            [Relationship("usermobileofflineprofilemembership", EntityRole.Referenced, "lk_usermobileofflineprofilemembership_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_usermobileofflineprofilemembership_modifiedonbehalfby = "lk_usermobileofflineprofilemembership_modifiedonbehalfby";
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
            [Relationship("virtualentitymetadata", EntityRole.Referenced, "lk_virtualentitymetadata_createdby", "createdby")]
            public const string lk_virtualentitymetadata_createdby = "lk_virtualentitymetadata_createdby";
            [Relationship("virtualentitymetadata", EntityRole.Referenced, "lk_virtualentitymetadata_createdonbehalfby", "createdonbehalfby")]
            public const string lk_virtualentitymetadata_createdonbehalfby = "lk_virtualentitymetadata_createdonbehalfby";
            [Relationship("virtualentitymetadata", EntityRole.Referenced, "lk_virtualentitymetadata_modifiedby", "modifiedby")]
            public const string lk_virtualentitymetadata_modifiedby = "lk_virtualentitymetadata_modifiedby";
            [Relationship("virtualentitymetadata", EntityRole.Referenced, "lk_virtualentitymetadata_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_virtualentitymetadata_modifiedonbehalfby = "lk_virtualentitymetadata_modifiedonbehalfby";
            [Relationship(WebResourceDefinition.EntityName, EntityRole.Referenced, "lk_webresourcebase_createdonbehalfby", "createdonbehalfby")]
            public const string lk_webresourcebase_createdonbehalfby = "lk_webresourcebase_createdonbehalfby";
            [Relationship(WebResourceDefinition.EntityName, EntityRole.Referenced, "lk_webresourcebase_modifiedonbehalfby", "modifiedonbehalfby")]
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
            [Relationship("workflowbinary", EntityRole.Referenced, "lk_workflowbinary_createdby", "createdby")]
            public const string lk_workflowbinary_createdby = "lk_workflowbinary_createdby";
            [Relationship("workflowbinary", EntityRole.Referenced, "lk_workflowbinary_createdonbehalfby", "createdonbehalfby")]
            public const string lk_workflowbinary_createdonbehalfby = "lk_workflowbinary_createdonbehalfby";
            [Relationship("workflowbinary", EntityRole.Referenced, "lk_workflowbinary_modifiedby", "modifiedby")]
            public const string lk_workflowbinary_modifiedby = "lk_workflowbinary_modifiedby";
            [Relationship("workflowbinary", EntityRole.Referenced, "lk_workflowbinary_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_workflowbinary_modifiedonbehalfby = "lk_workflowbinary_modifiedonbehalfby";
            [Relationship("workflowlog", EntityRole.Referenced, "lk_workflowlog_createdby", "createdby")]
            public const string lk_workflowlog_createdby = "lk_workflowlog_createdby";
            [Relationship("workflowlog", EntityRole.Referenced, "lk_workflowlog_createdonbehalfby", "createdonbehalfby")]
            public const string lk_workflowlog_createdonbehalfby = "lk_workflowlog_createdonbehalfby";
            [Relationship("workflowlog", EntityRole.Referenced, "lk_workflowlog_modifiedby", "modifiedby")]
            public const string lk_workflowlog_modifiedby = "lk_workflowlog_modifiedby";
            [Relationship("workflowlog", EntityRole.Referenced, "lk_workflowlog_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_workflowlog_modifiedonbehalfby = "lk_workflowlog_modifiedonbehalfby";
            [Relationship("lor_log", EntityRole.Referenced, "lor_log_utilisateur_SystemUser", "lor_utilisateur")]
            public const string lor_log_utilisateur_SystemUser = "lor_log_utilisateur_SystemUser";
            [Relationship("mailbox", EntityRole.Referenced, "mailbox_regarding_systemuser", "regardingobjectid")]
            public const string mailbox_regarding_systemuser = "mailbox_regarding_systemuser";
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
            [Relationship(PluginAssemblyDefinition.EntityName, EntityRole.Referenced, "modifiedby_pluginassembly", "modifiedby")]
            public const string modifiedby_pluginassembly = "modifiedby_pluginassembly";
            [Relationship(PluginTypeDefinition.EntityName, EntityRole.Referenced, "modifiedby_plugintype", "modifiedby")]
            public const string modifiedby_plugintype = "modifiedby_plugintype";
            [Relationship("plugintypestatistic", EntityRole.Referenced, "modifiedby_plugintypestatistic", "modifiedby")]
            public const string modifiedby_plugintypestatistic = "modifiedby_plugintypestatistic";
            [Relationship("relationshiprole", EntityRole.Referenced, "modifiedby_relationship_role", "modifiedby")]
            public const string modifiedby_relationship_role = "modifiedby_relationship_role";
            [Relationship("relationshiprolemap", EntityRole.Referenced, "modifiedby_relationship_role_map", "modifiedby")]
            public const string modifiedby_relationship_role_map = "modifiedby_relationship_role_map";
            [Relationship(SdkMessageDefinition.EntityName, EntityRole.Referenced, "modifiedby_sdkmessage", "modifiedby")]
            public const string modifiedby_sdkmessage = "modifiedby_sdkmessage";
            [Relationship(SdkMessageFilterDefinition.EntityName, EntityRole.Referenced, "modifiedby_sdkmessagefilter", "modifiedby")]
            public const string modifiedby_sdkmessagefilter = "modifiedby_sdkmessagefilter";
            [Relationship("sdkmessagepair", EntityRole.Referenced, "modifiedby_sdkmessagepair", "modifiedby")]
            public const string modifiedby_sdkmessagepair = "modifiedby_sdkmessagepair";
            [Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "modifiedby_sdkmessageprocessingstep", "modifiedby")]
            public const string modifiedby_sdkmessageprocessingstep = "modifiedby_sdkmessageprocessingstep";
            [Relationship(SdkMessageProcessingStepImageDefinition.EntityName, EntityRole.Referenced, "modifiedby_sdkmessageprocessingstepimage", "modifiedby")]
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
            [Relationship("msdyn_duplicateleadmapping", EntityRole.Referenced, "msdyn_systemuser_msdyn_duplicateleadmapping_DismissedBy", "msdyn_dismissedby")]
            public const string msdyn_systemuser_msdyn_duplicateleadmapping_DismissedBy = "msdyn_systemuser_msdyn_duplicateleadmapping_DismissedBy";
            [Relationship("msdyn_fieldservicesystemjob", EntityRole.Referenced, "msdyn_systemuser_msdyn_fieldservicesystemjob_OwnerId", "msdyn_ownerid")]
            public const string msdyn_systemuser_msdyn_fieldservicesystemjob_OwnerId = "msdyn_systemuser_msdyn_fieldservicesystemjob_OwnerId";
            [Relationship("msdyn_geolocationtracking", EntityRole.Referenced, "msdyn_systemuser_msdyn_geolocationtracking_UserId", "msdyn_userid")]
            public const string msdyn_systemuser_msdyn_geolocationtracking_UserId = "msdyn_systemuser_msdyn_geolocationtracking_UserId";
            [Relationship("msdyn_assignmentmap", EntityRole.Referenced, "msdyn_systemuser_msdyn_assignmentmap_systemuserid", "msdyn_systemuserid")]
            public const string msdyn_systemuser_msdyn_msdyn_assignmentmap_systemuserid = "msdyn_systemuser_msdyn_msdyn_assignmentmap_systemuserid";
            [Relationship("msdyn_purchaseorder", EntityRole.Referenced, "msdyn_systemuser_msdyn_purchaseorder_ApprovedRejectedBy", "msdyn_approvedrejectedby")]
            public const string msdyn_systemuser_msdyn_purchaseorder_ApprovedRejectedBy = "msdyn_systemuser_msdyn_purchaseorder_ApprovedRejectedBy";
            [Relationship("msdyn_purchaseorder", EntityRole.Referenced, "msdyn_systemuser_msdyn_purchaseorder_OrderedBy", "msdyn_orderedby")]
            public const string msdyn_systemuser_msdyn_purchaseorder_OrderedBy = "msdyn_systemuser_msdyn_purchaseorder_OrderedBy";
            [Relationship("msdyn_purchaseorderreceipt", EntityRole.Referenced, "msdyn_systemuser_msdyn_purchaseorderreceipt_ReceivedBy", "msdyn_receivedby")]
            public const string msdyn_systemuser_msdyn_purchaseorderreceipt_ReceivedBy = "msdyn_systemuser_msdyn_purchaseorderreceipt_ReceivedBy";
            [Relationship("msdyn_rma", EntityRole.Referenced, "msdyn_systemuser_msdyn_rma_ApprovedBy", "msdyn_approvedby")]
            public const string msdyn_systemuser_msdyn_rma_ApprovedBy = "msdyn_systemuser_msdyn_rma_ApprovedBy";
            [Relationship("msdyn_rmareceipt", EntityRole.Referenced, "msdyn_systemuser_msdyn_rmareceipt_ReceivedBy", "msdyn_receivedby")]
            public const string msdyn_systemuser_msdyn_rmareceipt_ReceivedBy = "msdyn_systemuser_msdyn_rmareceipt_ReceivedBy";
            [Relationship("msdyn_rtv", EntityRole.Referenced, "msdyn_systemuser_msdyn_rtv_ApprovedDeclinedBy", "msdyn_approveddeclinedby")]
            public const string msdyn_systemuser_msdyn_rtv_ApprovedDeclinedBy = "msdyn_systemuser_msdyn_rtv_ApprovedDeclinedBy";
            [Relationship("msdyn_rtv", EntityRole.Referenced, "msdyn_systemuser_msdyn_rtv_ReturnedBy", "msdyn_returnedby")]
            public const string msdyn_systemuser_msdyn_rtv_ReturnedBy = "msdyn_systemuser_msdyn_rtv_ReturnedBy";
            [Relationship("msdyn_salesroutingrun", EntityRole.Referenced, "msdyn_systemuser_msdyn_salesroutingrun_ownerassigned", "msdyn_ownerassigned")]
            public const string msdyn_systemuser_msdyn_salesroutingrun_ownerassigned = "msdyn_systemuser_msdyn_salesroutingrun_ownerassigned";
            [Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "msdyn_systemuser_msdyn_systemuserschedulersetting_User", "msdyn_user")]
            public const string msdyn_systemuser_msdyn_systemuserschedulersetting_User = "msdyn_systemuser_msdyn_systemuserschedulersetting_User";
            [Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "msdyn_systemuser_msdyn_timeoffrequest_Approvedby", "msdyn_approvedby")]
            public const string msdyn_systemuser_msdyn_timeoffrequest_Approvedby = "msdyn_systemuser_msdyn_timeoffrequest_Approvedby";
            [Relationship("msdyn_workorder", EntityRole.Referenced, "msdyn_systemuser_msdyn_workorder_ClosedBy", "msdyn_closedby")]
            public const string msdyn_systemuser_msdyn_workorder_ClosedBy = "msdyn_systemuser_msdyn_workorder_ClosedBy";
            [Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "msdyn_systemuser_wallsavedqueryusersettings_userid", "msdyn_userid")]
            public const string msdyn_systemuser_wallsavedqueryusersettings_userid = "msdyn_systemuser_wallsavedqueryusersettings_userid";
            [Relationship("msfp_alert", EntityRole.Referenced, "msfp_alert_systemuser_createdby", "createdby")]
            public const string msfp_alert_systemuser_createdby = "msfp_alert_systemuser_createdby";
            [Relationship("msfp_alert", EntityRole.Referenced, "msfp_alert_systemuser_createdonbehalfby", "createdonbehalfby")]
            public const string msfp_alert_systemuser_createdonbehalfby = "msfp_alert_systemuser_createdonbehalfby";
            [Relationship("msfp_alert", EntityRole.Referenced, "msfp_alert_systemuser_modifiedby", "modifiedby")]
            public const string msfp_alert_systemuser_modifiedby = "msfp_alert_systemuser_modifiedby";
            [Relationship("msfp_alert", EntityRole.Referenced, "msfp_alert_systemuser_modifiedonbehalfby", "modifiedonbehalfby")]
            public const string msfp_alert_systemuser_modifiedonbehalfby = "msfp_alert_systemuser_modifiedonbehalfby";
            [Relationship("msfp_alert", EntityRole.Referenced, "msfp_alert_systemuser_owninguser", "owninguser")]
            public const string msfp_alert_systemuser_owninguser = "msfp_alert_systemuser_owninguser";
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
            [Relationship("msfp_survey", EntityRole.Referenced, "msfp_systemuser_msfp_survey_publishedby", "msfp_publishedby")]
            public const string msfp_systemuser_msfp_survey_publishedby = "msfp_systemuser_msfp_survey_publishedby";
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
            [Relationship("queue", EntityRole.Referenced, "queue_primary_user", "primaryuserid")]
            public const string queue_primary_user = "queue_primary_user";
            [Relationship("socialprofile", EntityRole.Referenced, "socialProfile_owning_user", "owninguser")]
            public const string socialProfile_owning_user = "socialProfile_owning_user";
            [Relationship("systemuserprincipals", EntityRole.Referenced, "sup_principalid_systemuser", "systemuserid")]
            public const string sup_principalid_systemuser = "sup_principalid_systemuser";
            [Relationship("account", EntityRole.Referenced, "system_user_accounts", "preferredsystemuserid")]
            public const string system_user_accounts = "system_user_accounts";
            [Relationship("activityparty", EntityRole.Referenced, "system_user_activity_parties", "partyid")]
            public const string system_user_activity_parties = "system_user_activity_parties";
            [Relationship("asyncoperation", EntityRole.Referenced, "system_user_asyncoperation", "owninguser")]
            public const string system_user_asyncoperation = "system_user_asyncoperation";
            [Relationship("contact", EntityRole.Referenced, "system_user_contacts", "preferredsystemuserid")]
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
            [Relationship("appusersetting", EntityRole.Referenced, "systemuser_appusersetting_userId", "userid")]
            public const string systemuser_appusersetting_userId = "systemuser_appusersetting_userId";
            [Relationship("asyncoperation", EntityRole.Referenced, "SystemUser_AsyncOperations", "regardingobjectid")]
            public const string SystemUser_AsyncOperations = "SystemUser_AsyncOperations";
            [Relationship("bookableresource", EntityRole.Referenced, "systemuser_bookableresource_UserId", "userid")]
            public const string systemuser_bookableresource_UserId = "systemuser_bookableresource_UserId";
            [Relationship("bot", EntityRole.Referenced, "systemuser_bot_publishedby", "publishedby")]
            public const string systemuser_bot_publishedby = "systemuser_bot_publishedby";
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
            [Relationship("syncerror", EntityRole.Referenced, "SystemUser_SyncError", "owninguser")]
            public const string SystemUser_SyncError = "SystemUser_SyncError";
            [Relationship("syncerror", EntityRole.Referenced, "SystemUser_SyncErrors", "regardingobjectid")]
            public const string SystemUser_SyncErrors = "SystemUser_SyncErrors";
            [Relationship("usermobileofflineprofilemembership", EntityRole.Referenced, "systemuser_usermobileofflineprofilemembership_SystemUserId", "systemuserid")]
            public const string systemuser_usermobileofflineprofilemembership_SystemUserId = "systemuser_usermobileofflineprofilemembership_SystemUserId";
            [Relationship("systemuserbusinessunitentitymap", EntityRole.Referenced, "systemuserbusinessunitentitymap_systemuserid_systemuser", "systemuserid")]
            public const string systemuserbusinessunitentitymap_systemuserid_systemuser = "systemuserbusinessunitentitymap_systemuserid_systemuser";
            [Relationship("account", EntityRole.Referenced, "user_accounts", "owninguser")]
            public const string user_accounts = "user_accounts";
            [Relationship("activitypointer", EntityRole.Referenced, "user_activity", "owninguser")]
            public const string user_activity = "user_activity";
            [Relationship("activityfileattachment", EntityRole.Referenced, "user_activityfileattachment", "owninguser")]
            public const string user_activityfileattachment = "user_activityfileattachment";
            [Relationship("activitymonitor", EntityRole.Referenced, "user_activitymonitor", "owninguser")]
            public const string user_activitymonitor = "user_activitymonitor";
            [Relationship("adminsettingsentity", EntityRole.Referenced, "user_adminsettingsentity", "owninguser")]
            public const string user_adminsettingsentity = "user_adminsettingsentity";
            [Relationship("adx_ad", EntityRole.Referenced, "user_adx_ad", "owninguser")]
            public const string user_adx_ad = "user_adx_ad";
            [Relationship("adx_adplacement", EntityRole.Referenced, "user_adx_adplacement", "owninguser")]
            public const string user_adx_adplacement = "user_adx_adplacement";
            [Relationship("adx_blog", EntityRole.Referenced, "user_adx_blog", "owninguser")]
            public const string user_adx_blog = "user_adx_blog";
            [Relationship("adx_blogpost", EntityRole.Referenced, "user_adx_blogpost", "owninguser")]
            public const string user_adx_blogpost = "user_adx_blogpost";
            [Relationship("adx_botconsumer", EntityRole.Referenced, "user_adx_botconsumer", "owninguser")]
            public const string user_adx_botconsumer = "user_adx_botconsumer";
            [Relationship("adx_casedeflection", EntityRole.Referenced, "user_adx_casedeflection", "owninguser")]
            public const string user_adx_casedeflection = "user_adx_casedeflection";
            [Relationship("adx_cmpallowedreason", EntityRole.Referenced, "user_adx_cmpallowedreason", "owninguser")]
            public const string user_adx_cmpallowedreason = "user_adx_cmpallowedreason";
            [Relationship("adx_communityforum", EntityRole.Referenced, "user_adx_communityforum", "owninguser")]
            public const string user_adx_communityforum = "user_adx_communityforum";
            [Relationship("adx_communityforumaccesspermission", EntityRole.Referenced, "user_adx_communityforumaccesspermission", "owninguser")]
            public const string user_adx_communityforumaccesspermission = "user_adx_communityforumaccesspermission";
            [Relationship("adx_communityforumalert", EntityRole.Referenced, "user_adx_communityforumalert", "owninguser")]
            public const string user_adx_communityforumalert = "user_adx_communityforumalert";
            [Relationship("adx_communityforumannouncement", EntityRole.Referenced, "user_adx_communityforumannouncement", "owninguser")]
            public const string user_adx_communityforumannouncement = "user_adx_communityforumannouncement";
            [Relationship("adx_communityforumpost", EntityRole.Referenced, "user_adx_communityforumpost", "owninguser")]
            public const string user_adx_communityforumpost = "user_adx_communityforumpost";
            [Relationship("adx_communityforumthread", EntityRole.Referenced, "user_adx_communityforumthread", "owninguser")]
            public const string user_adx_communityforumthread = "user_adx_communityforumthread";
            [Relationship("adx_contentaccesslevel", EntityRole.Referenced, "user_adx_contentaccesslevel", "owninguser")]
            public const string user_adx_contentaccesslevel = "user_adx_contentaccesslevel";
            [Relationship("adx_contentmoderationpolicy", EntityRole.Referenced, "user_adx_contentmoderationpolicy", "owninguser")]
            public const string user_adx_contentmoderationpolicy = "user_adx_contentmoderationpolicy";
            [Relationship("adx_contentsnippet", EntityRole.Referenced, "user_adx_contentsnippet", "owninguser")]
            public const string user_adx_contentsnippet = "user_adx_contentsnippet";
            [Relationship("adx_forumthreadtype", EntityRole.Referenced, "user_adx_forumthreadtype", "owninguser")]
            public const string user_adx_forumthreadtype = "user_adx_forumthreadtype";
            [Relationship("adx_idea", EntityRole.Referenced, "user_adx_idea", "owninguser")]
            public const string user_adx_idea = "user_adx_idea";
            [Relationship("adx_ideaforum", EntityRole.Referenced, "user_adx_ideaforum", "owninguser")]
            public const string user_adx_ideaforum = "user_adx_ideaforum";
            [Relationship("adx_invitation", EntityRole.Referenced, "user_adx_invitation", "owninguser")]
            public const string user_adx_invitation = "user_adx_invitation";
            [Relationship("adx_pagealert", EntityRole.Referenced, "user_adx_pagealert", "owninguser")]
            public const string user_adx_pagealert = "user_adx_pagealert";
            [Relationship("adx_pagenotification", EntityRole.Referenced, "user_adx_pagenotification", "owninguser")]
            public const string user_adx_pagenotification = "user_adx_pagenotification";
            [Relationship("adx_pagetag", EntityRole.Referenced, "user_adx_pagetag", "owninguser")]
            public const string user_adx_pagetag = "user_adx_pagetag";
            [Relationship("adx_pagetemplate", EntityRole.Referenced, "user_adx_pagetemplate", "owninguser")]
            public const string user_adx_pagetemplate = "user_adx_pagetemplate";
            [Relationship("adx_poll", EntityRole.Referenced, "user_adx_poll", "owninguser")]
            public const string user_adx_poll = "user_adx_poll";
            [Relationship("adx_polloption", EntityRole.Referenced, "user_adx_polloption", "owninguser")]
            public const string user_adx_polloption = "user_adx_polloption";
            [Relationship("adx_pollplacement", EntityRole.Referenced, "user_adx_pollplacement", "owninguser")]
            public const string user_adx_pollplacement = "user_adx_pollplacement";
            [Relationship("adx_pollsubmission", EntityRole.Referenced, "user_adx_pollsubmission", "owninguser")]
            public const string user_adx_pollsubmission = "user_adx_pollsubmission";
            [Relationship("adx_portallanguage", EntityRole.Referenced, "user_adx_portallanguage", "owninguser")]
            public const string user_adx_portallanguage = "user_adx_portallanguage";
            [Relationship("adx_publishingstate", EntityRole.Referenced, "user_adx_publishingstate", "owninguser")]
            public const string user_adx_publishingstate = "user_adx_publishingstate";
            [Relationship("adx_publishingstatetransitionrule", EntityRole.Referenced, "user_adx_publishingstatetransitionrule", "owninguser")]
            public const string user_adx_publishingstatetransitionrule = "user_adx_publishingstatetransitionrule";
            [Relationship("adx_redirect", EntityRole.Referenced, "user_adx_redirect", "owninguser")]
            public const string user_adx_redirect = "user_adx_redirect";
            [Relationship("adx_setting", EntityRole.Referenced, "user_adx_setting", "owninguser")]
            public const string user_adx_setting = "user_adx_setting";
            [Relationship("adx_shortcut", EntityRole.Referenced, "user_adx_shortcut", "owninguser")]
            public const string user_adx_shortcut = "user_adx_shortcut";
            [Relationship("adx_sitemarker", EntityRole.Referenced, "user_adx_sitemarker", "owninguser")]
            public const string user_adx_sitemarker = "user_adx_sitemarker";
            [Relationship("adx_sitesetting", EntityRole.Referenced, "user_adx_sitesetting", "owninguser")]
            public const string user_adx_sitesetting = "user_adx_sitesetting";
            [Relationship("adx_tag", EntityRole.Referenced, "user_adx_tag", "owninguser")]
            public const string user_adx_tag = "user_adx_tag";
            [Relationship("adx_urlhistory", EntityRole.Referenced, "user_adx_urlhistory", "owninguser")]
            public const string user_adx_urlhistory = "user_adx_urlhistory";
            [Relationship("adx_webfile", EntityRole.Referenced, "user_adx_webfile", "owninguser")]
            public const string user_adx_webfile = "user_adx_webfile";
            [Relationship("adx_webfilelog", EntityRole.Referenced, "user_adx_webfilelog", "owninguser")]
            public const string user_adx_webfilelog = "user_adx_webfilelog";
            [Relationship("adx_webform", EntityRole.Referenced, "user_adx_webform", "owninguser")]
            public const string user_adx_webform = "user_adx_webform";
            [Relationship("adx_weblink", EntityRole.Referenced, "user_adx_weblink", "owninguser")]
            public const string user_adx_weblink = "user_adx_weblink";
            [Relationship("adx_weblinkset", EntityRole.Referenced, "user_adx_weblinkset", "owninguser")]
            public const string user_adx_weblinkset = "user_adx_weblinkset";
            [Relationship("adx_webnotificationurl", EntityRole.Referenced, "user_adx_webnotificationurl", "owninguser")]
            public const string user_adx_webnotificationurl = "user_adx_webnotificationurl";
            [Relationship("adx_webpage", EntityRole.Referenced, "user_adx_webpage", "owninguser")]
            public const string user_adx_webpage = "user_adx_webpage";
            [Relationship("adx_webpageaccesscontrolrule", EntityRole.Referenced, "user_adx_webpageaccesscontrolrule", "owninguser")]
            public const string user_adx_webpageaccesscontrolrule = "user_adx_webpageaccesscontrolrule";
            [Relationship("adx_webpagehistory", EntityRole.Referenced, "user_adx_webpagehistory", "owninguser")]
            public const string user_adx_webpagehistory = "user_adx_webpagehistory";
            [Relationship("adx_webpagelog", EntityRole.Referenced, "user_adx_webpagelog", "owninguser")]
            public const string user_adx_webpagelog = "user_adx_webpagelog";
            [Relationship("adx_webrole", EntityRole.Referenced, "user_adx_webrole", "owninguser")]
            public const string user_adx_webrole = "user_adx_webrole";
            [Relationship("adx_website", EntityRole.Referenced, "user_adx_website", "owninguser")]
            public const string user_adx_website = "user_adx_website";
            [Relationship("adx_websiteaccess", EntityRole.Referenced, "user_adx_websiteaccess", "owninguser")]
            public const string user_adx_websiteaccess = "user_adx_websiteaccess";
            [Relationship("adx_websitelanguage", EntityRole.Referenced, "user_adx_websitelanguage", "owninguser")]
            public const string user_adx_websitelanguage = "user_adx_websitelanguage";
            [Relationship("appnotification", EntityRole.Referenced, "user_appnotification", "owninguser")]
            public const string user_appnotification = "user_appnotification";
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
            [Relationship("bot", EntityRole.Referenced, "user_bot", "owninguser")]
            public const string user_bot = "user_bot";
            [Relationship("botcomponent", EntityRole.Referenced, "user_botcomponent", "owninguser")]
            public const string user_botcomponent = "user_botcomponent";
            [Relationship("bulkoperation", EntityRole.Referenced, "user_BulkOperation", "owninguser")]
            public const string user_BulkOperation = "user_BulkOperation";
            [Relationship("bulkoperationlog", EntityRole.Referenced, "user_bulkoperationlog", "owninguser")]
            public const string user_bulkoperationlog = "user_bulkoperationlog";
            [Relationship("campaignactivity", EntityRole.Referenced, "user_campaignactivity", "owninguser")]
            public const string user_campaignactivity = "user_campaignactivity";
            [Relationship("campaignresponse", EntityRole.Referenced, "user_campaignresponse", "owninguser")]
            public const string user_campaignresponse = "user_campaignresponse";
            [Relationship("canvasappextendedmetadata", EntityRole.Referenced, "user_canvasappextendedmetadata", "owninguser")]
            public const string user_canvasappextendedmetadata = "user_canvasappextendedmetadata";
            [Relationship("channelaccessprofile", EntityRole.Referenced, "user_channelaccessprofile", "owninguser")]
            public const string user_channelaccessprofile = "user_channelaccessprofile";
            [Relationship("characteristic", EntityRole.Referenced, "user_characteristic", "owninguser")]
            public const string user_characteristic = "user_characteristic";
            [Relationship("comment", EntityRole.Referenced, "user_comment", "owninguser")]
            public const string user_comment = "user_comment";
            [Relationship("connectionreference", EntityRole.Referenced, "user_connectionreference", "owninguser")]
            public const string user_connectionreference = "user_connectionreference";
            [Relationship("connector", EntityRole.Referenced, "user_connector", "owninguser")]
            public const string user_connector = "user_connector";
            [Relationship("contractdetail", EntityRole.Referenced, "user_contractdetail", "owninguser")]
            public const string user_contractdetail = "user_contractdetail";
            [Relationship("conversationtranscript", EntityRole.Referenced, "user_conversationtranscript", "owninguser")]
            public const string user_conversationtranscript = "user_conversationtranscript";
            [Relationship("convertrule", EntityRole.Referenced, "user_convertrule", "owninguser")]
            public const string user_convertrule = "user_convertrule";
            [Relationship("cr9a1_test", EntityRole.Referenced, "user_cr9a1_test", "owninguser")]
            public const string user_cr9a1_test = "user_cr9a1_test";
            [Relationship(CustomApiDefinition.EntityName, EntityRole.Referenced, "user_customapi", "owninguser")]
            public const string user_customapi = "user_customapi";
            [Relationship(CustomApiRequestParameterDefinition.EntityName, EntityRole.Referenced, "user_customapirequestparameter", "owninguser")]
            public const string user_customapirequestparameter = "user_customapirequestparameter";
            [Relationship(CustomApiResponsePropertyDefinition.EntityName, EntityRole.Referenced, "user_customapiresponseproperty", "owninguser")]
            public const string user_customapiresponseproperty = "user_customapiresponseproperty";
            [Relationship("customeropportunityrole", EntityRole.Referenced, "user_customer_opportunity_roles", "owninguser")]
            public const string user_customer_opportunity_roles = "user_customer_opportunity_roles";
            [Relationship("customerrelationship", EntityRole.Referenced, "user_customer_relationship", "owninguser")]
            public const string user_customer_relationship = "user_customer_relationship";
            [Relationship("datalakefolder", EntityRole.Referenced, "user_datalakefolder", "owninguser")]
            public const string user_datalakefolder = "user_datalakefolder";
            [Relationship("datalakefolderpermission", EntityRole.Referenced, "user_datalakefolderpermission", "owninguser")]
            public const string user_datalakefolderpermission = "user_datalakefolderpermission";
            [Relationship("datasyncstate", EntityRole.Referenced, "user_datasyncstate", "owninguser")]
            public const string user_datasyncstate = "user_datasyncstate";
            [Relationship("dimsi_debugsession", EntityRole.Referenced, "user_dimsi_debugsession", "owninguser")]
            public const string user_dimsi_debugsession = "user_dimsi_debugsession";
            [Relationship("email", EntityRole.Referenced, "user_email", "owninguser")]
            public const string user_email = "user_email";
            [Relationship("entitlement", EntityRole.Referenced, "user_entitlement", "owninguser")]
            public const string user_entitlement = "user_entitlement";
            [Relationship("entitlementchannel", EntityRole.Referenced, "user_entitlementchannel", "owninguser")]
            public const string user_entitlementchannel = "user_entitlementchannel";
            [Relationship("entitlemententityallocationtypemapping", EntityRole.Referenced, "user_entitlemententityallocationtypemapping", "owninguser")]
            public const string user_entitlemententityallocationtypemapping = "user_entitlemententityallocationtypemapping";
            [Relationship("environmentvariabledefinition", EntityRole.Referenced, "user_environmentvariabledefinition", "owninguser")]
            public const string user_environmentvariabledefinition = "user_environmentvariabledefinition";
            [Relationship("environmentvariablevalue", EntityRole.Referenced, "user_environmentvariablevalue", "owninguser")]
            public const string user_environmentvariablevalue = "user_environmentvariablevalue";
            [Relationship("exchangesyncidmapping", EntityRole.Referenced, "user_exchangesyncidmapping", "owninguser")]
            public const string user_exchangesyncidmapping = "user_exchangesyncidmapping";
            [Relationship("exportsolutionupload", EntityRole.Referenced, "user_exportsolutionupload", "owninguser")]
            public const string user_exportsolutionupload = "user_exportsolutionupload";
            [Relationship("externalparty", EntityRole.Referenced, "systemuser_user_externalparty", "owninguser")]
            public const string user_externalparty = "user_externalparty";
            [Relationship("fax", EntityRole.Referenced, "user_fax", "owninguser")]
            public const string user_fax = "user_fax";
            [Relationship("featurecontrolsetting", EntityRole.Referenced, "user_featurecontrolsetting", "owninguser")]
            public const string user_featurecontrolsetting = "user_featurecontrolsetting";
            [Relationship("flowmachine", EntityRole.Referenced, "user_flowmachine", "owninguser")]
            public const string user_flowmachine = "user_flowmachine";
            [Relationship("flowmachinegroup", EntityRole.Referenced, "user_flowmachinegroup", "owninguser")]
            public const string user_flowmachinegroup = "user_flowmachinegroup";
            [Relationship("flowsession", EntityRole.Referenced, "user_flowsession", "owninguser")]
            public const string user_flowsession = "user_flowsession";
            [Relationship("ftp_agentimmobilier", EntityRole.Referenced, "user_ftp_agentimmobilier", "owninguser")]
            public const string user_ftp_agentimmobilier = "user_ftp_agentimmobilier";
            [Relationship("ftp_appartement", EntityRole.Referenced, "user_ftp_appartement", "owninguser")]
            public const string user_ftp_appartement = "user_ftp_appartement";
            [Relationship("ftp_contrat", EntityRole.Referenced, "user_ftp_contrat", "owninguser")]
            public const string user_ftp_contrat = "user_ftp_contrat";
            [Relationship("ftp_contratdelocation", EntityRole.Referenced, "user_ftp_contratdelocation", "owninguser")]
            public const string user_ftp_contratdelocation = "user_ftp_contratdelocation";
            [Relationship("ftp_particulier", EntityRole.Referenced, "user_ftp_particulier", "owninguser")]
            public const string user_ftp_particulier = "user_ftp_particulier";
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
            [Relationship("keyvaultreference", EntityRole.Referenced, "user_keyvaultreference", "owninguser")]
            public const string user_keyvaultreference = "user_keyvaultreference";
            [Relationship("knowledgearticle", EntityRole.Referenced, "user_knowledgearticle", "owninguser")]
            public const string user_knowledgearticle = "user_knowledgearticle";
            [Relationship("knowledgearticleincident", EntityRole.Referenced, "user_knowledgearticleincident", "owninguser")]
            public const string user_knowledgearticleincident = "user_knowledgearticleincident";
            [Relationship("letter", EntityRole.Referenced, "user_letter", "owninguser")]
            public const string user_letter = "user_letter";
            [Relationship("list", EntityRole.Referenced, "user_list", "owninguser")]
            public const string user_list = "user_list";
            [Relationship("listoperation", EntityRole.Referenced, "user_listoperation", "owninguser")]
            public const string user_listoperation = "user_listoperation";
            [Relationship("lor_bien", EntityRole.Referenced, "user_lor_bien", "owninguser")]
            public const string user_lor_bien = "user_lor_bien";
            [Relationship("lor_camion", EntityRole.Referenced, "user_lor_camion", "owninguser")]
            public const string user_lor_camion = "user_lor_camion";
            [Relationship("lor_demande", EntityRole.Referenced, "user_lor_demande", "owninguser")]
            public const string user_lor_demande = "user_lor_demande";
            [Relationship("lor_immeuble", EntityRole.Referenced, "user_lor_immeuble", "owninguser")]
            public const string user_lor_immeuble = "user_lor_immeuble";
            [Relationship("lor_log", EntityRole.Referenced, "user_lor_log", "owninguser")]
            public const string user_lor_log = "user_lor_log";
            [Relationship("lor_maintenance", EntityRole.Referenced, "user_lor_maintenance", "owninguser")]
            public const string user_lor_maintenance = "user_lor_maintenance";
            [Relationship("lor_parainage", EntityRole.Referenced, "user_lor_parainage", "owninguser")]
            public const string user_lor_parainage = "user_lor_parainage";
            [Relationship("lor_plaqueimmatriculation", EntityRole.Referenced, "user_lor_plaqueimmatriculation", "owninguser")]
            public const string user_lor_plaqueimmatriculation = "user_lor_plaqueimmatriculation";
            [Relationship("lor_quartier", EntityRole.Referenced, "user_lor_quartier", "owninguser")]
            public const string user_lor_quartier = "user_lor_quartier";
            [Relationship("lor_route", EntityRole.Referenced, "user_lor_route", "owninguser")]
            public const string user_lor_route = "user_lor_route";
            [Relationship("lor_stock", EntityRole.Referenced, "user_lor_stock", "owninguser")]
            public const string user_lor_stock = "user_lor_stock";
            [Relationship("mailbox", EntityRole.Referenced, "user_mailbox", "owninguser")]
            public const string user_mailbox = "user_mailbox";
            [Relationship("managedidentity", EntityRole.Referenced, "user_managedidentity", "owninguser")]
            public const string user_managedidentity = "user_managedidentity";
            [Relationship("msdyn_actioncardactionstat", EntityRole.Referenced, "user_msdyn_actioncardactionstat", "owninguser")]
            public const string user_msdyn_actioncardactionstat = "user_msdyn_actioncardactionstat";
            [Relationship("msdyn_actioncardregarding", EntityRole.Referenced, "user_msdyn_actioncardregarding", "owninguser")]
            public const string user_msdyn_actioncardregarding = "user_msdyn_actioncardregarding";
            [Relationship("msdyn_actioncardrolesetting", EntityRole.Referenced, "user_msdyn_actioncardrolesetting", "owninguser")]
            public const string user_msdyn_actioncardrolesetting = "user_msdyn_actioncardrolesetting";
            [Relationship("msdyn_actioncardstataggregation", EntityRole.Referenced, "user_msdyn_actioncardstataggregation", "owninguser")]
            public const string user_msdyn_actioncardstataggregation = "user_msdyn_actioncardstataggregation";
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
            [Relationship("msdyn_aibdataset", EntityRole.Referenced, "user_msdyn_aibdataset", "owninguser")]
            public const string user_msdyn_aibdataset = "user_msdyn_aibdataset";
            [Relationship("msdyn_aibdatasetfile", EntityRole.Referenced, "user_msdyn_aibdatasetfile", "owninguser")]
            public const string user_msdyn_aibdatasetfile = "user_msdyn_aibdatasetfile";
            [Relationship("msdyn_aibdatasetrecord", EntityRole.Referenced, "user_msdyn_aibdatasetrecord", "owninguser")]
            public const string user_msdyn_aibdatasetrecord = "user_msdyn_aibdatasetrecord";
            [Relationship("msdyn_aibdatasetscontainer", EntityRole.Referenced, "user_msdyn_aibdatasetscontainer", "owninguser")]
            public const string user_msdyn_aibdatasetscontainer = "user_msdyn_aibdatasetscontainer";
            [Relationship("msdyn_aibfile", EntityRole.Referenced, "user_msdyn_aibfile", "owninguser")]
            public const string user_msdyn_aibfile = "user_msdyn_aibfile";
            [Relationship("msdyn_aibfileattacheddata", EntityRole.Referenced, "user_msdyn_aibfileattacheddata", "owninguser")]
            public const string user_msdyn_aibfileattacheddata = "user_msdyn_aibfileattacheddata";
            [Relationship("msdyn_aiconfiguration", EntityRole.Referenced, "user_msdyn_aiconfiguration", "owninguser")]
            public const string user_msdyn_aiconfiguration = "user_msdyn_aiconfiguration";
            [Relationship("msdyn_aicontactsuggestion", EntityRole.Referenced, "user_msdyn_aicontactsuggestion", "owninguser")]
            public const string user_msdyn_aicontactsuggestion = "user_msdyn_aicontactsuggestion";
            [Relationship("msdyn_aifptrainingdocument", EntityRole.Referenced, "user_msdyn_aifptrainingdocument", "owninguser")]
            public const string user_msdyn_aifptrainingdocument = "user_msdyn_aifptrainingdocument";
            [Relationship("msdyn_aimodel", EntityRole.Referenced, "user_msdyn_aimodel", "owninguser")]
            public const string user_msdyn_aimodel = "user_msdyn_aimodel";
            [Relationship("msdyn_aiodimage", EntityRole.Referenced, "user_msdyn_aiodimage", "owninguser")]
            public const string user_msdyn_aiodimage = "user_msdyn_aiodimage";
            [Relationship("msdyn_aiodlabel", EntityRole.Referenced, "user_msdyn_aiodlabel", "owninguser")]
            public const string user_msdyn_aiodlabel = "user_msdyn_aiodlabel";
            [Relationship("msdyn_aiodtrainingboundingbox", EntityRole.Referenced, "user_msdyn_aiodtrainingboundingbox", "owninguser")]
            public const string user_msdyn_aiodtrainingboundingbox = "user_msdyn_aiodtrainingboundingbox";
            [Relationship("msdyn_aiodtrainingimage", EntityRole.Referenced, "user_msdyn_aiodtrainingimage", "owninguser")]
            public const string user_msdyn_aiodtrainingimage = "user_msdyn_aiodtrainingimage";
            [Relationship("msdyn_aitemplate", EntityRole.Referenced, "user_msdyn_aitemplate", "owninguser")]
            public const string user_msdyn_aitemplate = "user_msdyn_aitemplate";
            [Relationship("msdyn_analysiscomponent", EntityRole.Referenced, "user_msdyn_analysiscomponent", "owninguser")]
            public const string user_msdyn_analysiscomponent = "user_msdyn_analysiscomponent";
            [Relationship("msdyn_analysisjob", EntityRole.Referenced, "user_msdyn_analysisjob", "owninguser")]
            public const string user_msdyn_analysisjob = "user_msdyn_analysisjob";
            [Relationship("msdyn_analysisresult", EntityRole.Referenced, "user_msdyn_analysisresult", "owninguser")]
            public const string user_msdyn_analysisresult = "user_msdyn_analysisresult";
            [Relationship("msdyn_analysisresultdetail", EntityRole.Referenced, "user_msdyn_analysisresultdetail", "owninguser")]
            public const string user_msdyn_analysisresultdetail = "user_msdyn_analysisresultdetail";
            [Relationship("msdyn_analyticsadminsettings", EntityRole.Referenced, "user_msdyn_analyticsadminsettings", "owninguser")]
            public const string user_msdyn_analyticsadminsettings = "user_msdyn_analyticsadminsettings";
            [Relationship("msdyn_analyticsforcs", EntityRole.Referenced, "user_msdyn_analyticsforcs", "owninguser")]
            public const string user_msdyn_analyticsforcs = "user_msdyn_analyticsforcs";
            [Relationship("msdyn_appconfiguration", EntityRole.Referenced, "user_msdyn_appconfiguration", "owninguser")]
            public const string user_msdyn_appconfiguration = "user_msdyn_appconfiguration";
            [Relationship("msdyn_applicationextension", EntityRole.Referenced, "user_msdyn_applicationextension", "owninguser")]
            public const string user_msdyn_applicationextension = "user_msdyn_applicationextension";
            [Relationship("msdyn_applicationtabtemplate", EntityRole.Referenced, "user_msdyn_applicationtabtemplate", "owninguser")]
            public const string user_msdyn_applicationtabtemplate = "user_msdyn_applicationtabtemplate";
            [Relationship("msdyn_assetcategorytemplateassociation", EntityRole.Referenced, "user_msdyn_assetcategorytemplateassociation", "owninguser")]
            public const string user_msdyn_assetcategorytemplateassociation = "user_msdyn_assetcategorytemplateassociation";
            [Relationship("msdyn_assettemplateassociation", EntityRole.Referenced, "user_msdyn_assettemplateassociation", "owninguser")]
            public const string user_msdyn_assettemplateassociation = "user_msdyn_assettemplateassociation";
            [Relationship("msdyn_assignmentmap", EntityRole.Referenced, "user_msdyn_assignmentmap", "owninguser")]
            public const string user_msdyn_assignmentmap = "user_msdyn_assignmentmap";
            [Relationship("msdyn_assignmentrule", EntityRole.Referenced, "user_msdyn_assignmentrule", "owninguser")]
            public const string user_msdyn_assignmentrule = "user_msdyn_assignmentrule";
            [Relationship("msdyn_attribute", EntityRole.Referenced, "user_msdyn_attribute", "owninguser")]
            public const string user_msdyn_attribute = "user_msdyn_attribute";
            [Relationship("msdyn_attributevalue", EntityRole.Referenced, "user_msdyn_attributevalue", "owninguser")]
            public const string user_msdyn_attributevalue = "user_msdyn_attributevalue";
            [Relationship("msdyn_autocapturerule", EntityRole.Referenced, "user_msdyn_autocapturerule", "owninguser")]
            public const string user_msdyn_autocapturerule = "user_msdyn_autocapturerule";
            [Relationship("msdyn_autocapturesettings", EntityRole.Referenced, "user_msdyn_autocapturesettings", "owninguser")]
            public const string user_msdyn_autocapturesettings = "user_msdyn_autocapturesettings";
            [Relationship("msdyn_bookableresourceassociation", EntityRole.Referenced, "user_msdyn_bookableresourceassociation", "owninguser")]
            public const string user_msdyn_bookableresourceassociation = "user_msdyn_bookableresourceassociation";
            [Relationship("msdyn_bookableresourcebookingquicknote", EntityRole.Referenced, "user_msdyn_bookableresourcebookingquicknote", "owninguser")]
            public const string user_msdyn_bookableresourcebookingquicknote = "user_msdyn_bookableresourcebookingquicknote";
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
            [Relationship("msdyn_businessclosure", EntityRole.Referenced, "user_msdyn_businessclosure", "owninguser")]
            public const string user_msdyn_businessclosure = "user_msdyn_businessclosure";
            [Relationship("msdyn_callablecontext", EntityRole.Referenced, "user_msdyn_callablecontext", "owninguser")]
            public const string user_msdyn_callablecontext = "user_msdyn_callablecontext";
            [Relationship("msdyn_channelprovider", EntityRole.Referenced, "user_msdyn_channelprovider", "owninguser")]
            public const string user_msdyn_channelprovider = "user_msdyn_channelprovider";
            [Relationship("msdyn_clientextension", EntityRole.Referenced, "user_msdyn_clientextension", "owninguser")]
            public const string user_msdyn_clientextension = "user_msdyn_clientextension";
            [Relationship("msdyn_configuration", EntityRole.Referenced, "user_msdyn_configuration", "owninguser")]
            public const string user_msdyn_configuration = "user_msdyn_configuration";
            [Relationship("msdyn_contactsuggestionrule", EntityRole.Referenced, "user_msdyn_contactsuggestionrule", "owninguser")]
            public const string user_msdyn_contactsuggestionrule = "user_msdyn_contactsuggestionrule";
            [Relationship("msdyn_contactsuggestionruleset", EntityRole.Referenced, "user_msdyn_contactsuggestionruleset", "owninguser")]
            public const string user_msdyn_contactsuggestionruleset = "user_msdyn_contactsuggestionruleset";
            [Relationship("msdyn_conversationdata", EntityRole.Referenced, "user_msdyn_conversationdata", "owninguser")]
            public const string user_msdyn_conversationdata = "user_msdyn_conversationdata";
            [Relationship("msdyn_customerasset", EntityRole.Referenced, "user_msdyn_customerasset", "owninguser")]
            public const string user_msdyn_customerasset = "user_msdyn_customerasset";
            [Relationship("msdyn_customerassetattachment", EntityRole.Referenced, "user_msdyn_customerassetattachment", "owninguser")]
            public const string user_msdyn_customerassetattachment = "user_msdyn_customerassetattachment";
            [Relationship("msdyn_customerassetcategory", EntityRole.Referenced, "user_msdyn_customerassetcategory", "owninguser")]
            public const string user_msdyn_customerassetcategory = "user_msdyn_customerassetcategory";
            [Relationship("msdyn_dataanalyticsreport", EntityRole.Referenced, "user_msdyn_dataanalyticsreport", "owninguser")]
            public const string user_msdyn_dataanalyticsreport = "user_msdyn_dataanalyticsreport";
            [Relationship("msdyn_dataflow", EntityRole.Referenced, "user_msdyn_dataflow", "owninguser")]
            public const string user_msdyn_dataflow = "user_msdyn_dataflow";
            [Relationship("msdyn_dataflowrefreshhistory", EntityRole.Referenced, "user_msdyn_dataflowrefreshhistory", "owninguser")]
            public const string user_msdyn_dataflowrefreshhistory = "user_msdyn_dataflowrefreshhistory";
            [Relationship("msdyn_dealmanageraccess", EntityRole.Referenced, "user_msdyn_dealmanageraccess", "owninguser")]
            public const string user_msdyn_dealmanageraccess = "user_msdyn_dealmanageraccess";
            [Relationship("msdyn_dealmanagersettings", EntityRole.Referenced, "user_msdyn_dealmanagersettings", "owninguser")]
            public const string user_msdyn_dealmanagersettings = "user_msdyn_dealmanagersettings";
            [Relationship("msdyn_duplicateleadmapping", EntityRole.Referenced, "user_msdyn_duplicateleadmapping", "owninguser")]
            public const string user_msdyn_duplicateleadmapping = "user_msdyn_duplicateleadmapping";
            [Relationship("msdyn_entitlementapplication", EntityRole.Referenced, "user_msdyn_entitlementapplication", "owninguser")]
            public const string user_msdyn_entitlementapplication = "user_msdyn_entitlementapplication";
            [Relationship("msdyn_entityconfiguration", EntityRole.Referenced, "user_msdyn_entityconfiguration", "owninguser")]
            public const string user_msdyn_entityconfiguration = "user_msdyn_entityconfiguration";
            [Relationship("msdyn_entitylinkchatconfiguration", EntityRole.Referenced, "user_msdyn_entitylinkchatconfiguration", "owninguser")]
            public const string user_msdyn_entitylinkchatconfiguration = "user_msdyn_entitylinkchatconfiguration";
            [Relationship("msdyn_entityrankingrule", EntityRole.Referenced, "user_msdyn_entityrankingrule", "owninguser")]
            public const string user_msdyn_entityrankingrule = "user_msdyn_entityrankingrule";
            [Relationship("msdyn_entityrefreshhistory", EntityRole.Referenced, "user_msdyn_entityrefreshhistory", "owninguser")]
            public const string user_msdyn_entityrefreshhistory = "user_msdyn_entityrefreshhistory";
            [Relationship("msdyn_extendedusersetting", EntityRole.Referenced, "user_msdyn_extendedusersetting", "owninguser")]
            public const string user_msdyn_extendedusersetting = "user_msdyn_extendedusersetting";
            [Relationship("msdyn_federatedarticle", EntityRole.Referenced, "user_msdyn_federatedarticle", "owninguser")]
            public const string user_msdyn_federatedarticle = "user_msdyn_federatedarticle";
            [Relationship("msdyn_fieldservicesetting", EntityRole.Referenced, "user_msdyn_fieldservicesetting", "owninguser")]
            public const string user_msdyn_fieldservicesetting = "user_msdyn_fieldservicesetting";
            [Relationship("msdyn_fieldserviceslaconfiguration", EntityRole.Referenced, "user_msdyn_fieldserviceslaconfiguration", "owninguser")]
            public const string user_msdyn_fieldserviceslaconfiguration = "user_msdyn_fieldserviceslaconfiguration";
            [Relationship("msdyn_flowcardtype", EntityRole.Referenced, "user_msdyn_flowcardtype", "owninguser")]
            public const string user_msdyn_flowcardtype = "user_msdyn_flowcardtype";
            [Relationship("msdyn_forecastconfiguration", EntityRole.Referenced, "user_msdyn_forecastconfiguration", "owninguser")]
            public const string user_msdyn_forecastconfiguration = "user_msdyn_forecastconfiguration";
            [Relationship("msdyn_forecastdefinition", EntityRole.Referenced, "user_msdyn_forecastdefinition", "owninguser")]
            public const string user_msdyn_forecastdefinition = "user_msdyn_forecastdefinition";
            [Relationship("msdyn_forecastinstance", EntityRole.Referenced, "user_msdyn_forecastinstance", "owninguser")]
            public const string user_msdyn_forecastinstance = "user_msdyn_forecastinstance";
            [Relationship("msdyn_forecastrecurrence", EntityRole.Referenced, "user_msdyn_forecastrecurrence", "owninguser")]
            public const string user_msdyn_forecastrecurrence = "user_msdyn_forecastrecurrence";
            [Relationship("msdyn_functionallocation", EntityRole.Referenced, "user_msdyn_functionallocation", "owninguser")]
            public const string user_msdyn_functionallocation = "user_msdyn_functionallocation";
            [Relationship("msdyn_gdprdata", EntityRole.Referenced, "user_msdyn_gdprdata", "owninguser")]
            public const string user_msdyn_gdprdata = "user_msdyn_gdprdata";
            [Relationship("msdyn_geofence", EntityRole.Referenced, "user_msdyn_geofence", "owninguser")]
            public const string user_msdyn_geofence = "user_msdyn_geofence";
            [Relationship("msdyn_geofenceevent", EntityRole.Referenced, "user_msdyn_geofenceevent", "owninguser")]
            public const string user_msdyn_geofenceevent = "user_msdyn_geofenceevent";
            [Relationship("msdyn_geofencingsettings", EntityRole.Referenced, "user_msdyn_geofencingsettings", "owninguser")]
            public const string user_msdyn_geofencingsettings = "user_msdyn_geofencingsettings";
            [Relationship("msdyn_icebreakersconfig", EntityRole.Referenced, "user_msdyn_icebreakersconfig", "owninguser")]
            public const string user_msdyn_icebreakersconfig = "user_msdyn_icebreakersconfig";
            [Relationship("msdyn_incidenttype", EntityRole.Referenced, "user_msdyn_incidenttype", "owninguser")]
            public const string user_msdyn_incidenttype = "user_msdyn_incidenttype";
            [Relationship("msdyn_incidenttype_requirementgroup", EntityRole.Referenced, "user_msdyn_incidenttype_requirementgroup", "owninguser")]
            public const string user_msdyn_incidenttype_requirementgroup = "user_msdyn_incidenttype_requirementgroup";
            [Relationship("msdyn_incidenttypecharacteristic", EntityRole.Referenced, "user_msdyn_incidenttypecharacteristic", "owninguser")]
            public const string user_msdyn_incidenttypecharacteristic = "user_msdyn_incidenttypecharacteristic";
            [Relationship("msdyn_incidenttypeproduct", EntityRole.Referenced, "user_msdyn_incidenttypeproduct", "owninguser")]
            public const string user_msdyn_incidenttypeproduct = "user_msdyn_incidenttypeproduct";
            [Relationship("msdyn_incidenttyperecommendationresult", EntityRole.Referenced, "user_msdyn_incidenttyperecommendationresult", "owninguser")]
            public const string user_msdyn_incidenttyperecommendationresult = "user_msdyn_incidenttyperecommendationresult";
            [Relationship("msdyn_incidenttyperecommendationrunhistory", EntityRole.Referenced, "user_msdyn_incidenttyperecommendationrunhistory", "owninguser")]
            public const string user_msdyn_incidenttyperecommendationrunhistory = "user_msdyn_incidenttyperecommendationrunhistory";
            [Relationship("msdyn_incidenttyperesolution", EntityRole.Referenced, "user_msdyn_incidenttyperesolution", "owninguser")]
            public const string user_msdyn_incidenttyperesolution = "user_msdyn_incidenttyperesolution";
            [Relationship("msdyn_incidenttypeservice", EntityRole.Referenced, "user_msdyn_incidenttypeservice", "owninguser")]
            public const string user_msdyn_incidenttypeservice = "user_msdyn_incidenttypeservice";
            [Relationship("msdyn_incidenttypeservicetask", EntityRole.Referenced, "user_msdyn_incidenttypeservicetask", "owninguser")]
            public const string user_msdyn_incidenttypeservicetask = "user_msdyn_incidenttypeservicetask";
            [Relationship("msdyn_incidenttypessetup", EntityRole.Referenced, "user_msdyn_incidenttypessetup", "owninguser")]
            public const string user_msdyn_incidenttypessetup = "user_msdyn_incidenttypessetup";
            [Relationship("msdyn_inspection", EntityRole.Referenced, "user_msdyn_inspection", "owninguser")]
            public const string user_msdyn_inspection = "user_msdyn_inspection";
            [Relationship("msdyn_inspectionattachment", EntityRole.Referenced, "user_msdyn_inspectionattachment", "owninguser")]
            public const string user_msdyn_inspectionattachment = "user_msdyn_inspectionattachment";
            [Relationship("msdyn_inspectiondefinition", EntityRole.Referenced, "user_msdyn_inspectiondefinition", "owninguser")]
            public const string user_msdyn_inspectiondefinition = "user_msdyn_inspectiondefinition";
            [Relationship("msdyn_inspectioninstance", EntityRole.Referenced, "user_msdyn_inspectioninstance", "owninguser")]
            public const string user_msdyn_inspectioninstance = "user_msdyn_inspectioninstance";
            [Relationship("msdyn_inspectionresponse", EntityRole.Referenced, "user_msdyn_inspectionresponse", "owninguser")]
            public const string user_msdyn_inspectionresponse = "user_msdyn_inspectionresponse";
            [Relationship("msdyn_inventoryadjustment", EntityRole.Referenced, "user_msdyn_inventoryadjustment", "owninguser")]
            public const string user_msdyn_inventoryadjustment = "user_msdyn_inventoryadjustment";
            [Relationship("msdyn_inventoryadjustmentproduct", EntityRole.Referenced, "user_msdyn_inventoryadjustmentproduct", "owninguser")]
            public const string user_msdyn_inventoryadjustmentproduct = "user_msdyn_inventoryadjustmentproduct";
            [Relationship("msdyn_inventoryjournal", EntityRole.Referenced, "user_msdyn_inventoryjournal", "owninguser")]
            public const string user_msdyn_inventoryjournal = "user_msdyn_inventoryjournal";
            [Relationship("msdyn_inventorytransfer", EntityRole.Referenced, "user_msdyn_inventorytransfer", "owninguser")]
            public const string user_msdyn_inventorytransfer = "user_msdyn_inventorytransfer";
            [Relationship("msdyn_iotalert", EntityRole.Referenced, "user_msdyn_iotalert", "owninguser")]
            public const string user_msdyn_iotalert = "user_msdyn_iotalert";
            [Relationship("msdyn_iotdevice", EntityRole.Referenced, "user_msdyn_iotdevice", "owninguser")]
            public const string user_msdyn_iotdevice = "user_msdyn_iotdevice";
            [Relationship("msdyn_iotdevicecategory", EntityRole.Referenced, "user_msdyn_iotdevicecategory", "owninguser")]
            public const string user_msdyn_iotdevicecategory = "user_msdyn_iotdevicecategory";
            [Relationship("msdyn_iotdevicecommand", EntityRole.Referenced, "user_msdyn_iotdevicecommand", "owninguser")]
            public const string user_msdyn_iotdevicecommand = "user_msdyn_iotdevicecommand";
            [Relationship("msdyn_iotdevicecommanddefinition", EntityRole.Referenced, "user_msdyn_iotdevicecommanddefinition", "owninguser")]
            public const string user_msdyn_iotdevicecommanddefinition = "user_msdyn_iotdevicecommanddefinition";
            [Relationship("msdyn_iotdevicedatahistory", EntityRole.Referenced, "user_msdyn_iotdevicedatahistory", "owninguser")]
            public const string user_msdyn_iotdevicedatahistory = "user_msdyn_iotdevicedatahistory";
            [Relationship("msdyn_iotdeviceproperty", EntityRole.Referenced, "user_msdyn_iotdeviceproperty", "owninguser")]
            public const string user_msdyn_iotdeviceproperty = "user_msdyn_iotdeviceproperty";
            [Relationship("msdyn_iotdeviceregistrationhistory", EntityRole.Referenced, "user_msdyn_iotdeviceregistrationhistory", "owninguser")]
            public const string user_msdyn_iotdeviceregistrationhistory = "user_msdyn_iotdeviceregistrationhistory";
            [Relationship("msdyn_iotdevicevisualizationconfiguration", EntityRole.Referenced, "user_msdyn_iotdevicevisualizationconfiguration", "owninguser")]
            public const string user_msdyn_iotdevicevisualizationconfiguration = "user_msdyn_iotdevicevisualizationconfiguration";
            [Relationship("msdyn_iotfieldmapping", EntityRole.Referenced, "user_msdyn_iotfieldmapping", "owninguser")]
            public const string user_msdyn_iotfieldmapping = "user_msdyn_iotfieldmapping";
            [Relationship("msdyn_iotpropertydefinition", EntityRole.Referenced, "user_msdyn_iotpropertydefinition", "owninguser")]
            public const string user_msdyn_iotpropertydefinition = "user_msdyn_iotpropertydefinition";
            [Relationship("msdyn_iotprovider", EntityRole.Referenced, "user_msdyn_iotprovider", "owninguser")]
            public const string user_msdyn_iotprovider = "user_msdyn_iotprovider";
            [Relationship("msdyn_iotproviderinstance", EntityRole.Referenced, "user_msdyn_iotproviderinstance", "owninguser")]
            public const string user_msdyn_iotproviderinstance = "user_msdyn_iotproviderinstance";
            [Relationship("msdyn_iotsettings", EntityRole.Referenced, "user_msdyn_iotsettings", "owninguser")]
            public const string user_msdyn_iotsettings = "user_msdyn_iotsettings";
            [Relationship("msdyn_kalanguagesetting", EntityRole.Referenced, "user_msdyn_kalanguagesetting", "owninguser")]
            public const string user_msdyn_kalanguagesetting = "user_msdyn_kalanguagesetting";
            [Relationship("msdyn_kbattachment", EntityRole.Referenced, "user_msdyn_kbattachment", "owninguser")]
            public const string user_msdyn_kbattachment = "user_msdyn_kbattachment";
            [Relationship("msdyn_kmfederatedsearchconfig", EntityRole.Referenced, "user_msdyn_kmfederatedsearchconfig", "owninguser")]
            public const string user_msdyn_kmfederatedsearchconfig = "user_msdyn_kmfederatedsearchconfig";
            [Relationship("msdyn_knowledgearticleimage", EntityRole.Referenced, "user_msdyn_knowledgearticleimage", "owninguser")]
            public const string user_msdyn_knowledgearticleimage = "user_msdyn_knowledgearticleimage";
            [Relationship("msdyn_knowledgearticletemplate", EntityRole.Referenced, "user_msdyn_knowledgearticletemplate", "owninguser")]
            public const string user_msdyn_knowledgearticletemplate = "user_msdyn_knowledgearticletemplate";
            [Relationship("msdyn_knowledgeinteractioninsight", EntityRole.Referenced, "user_msdyn_knowledgeinteractioninsight", "owninguser")]
            public const string user_msdyn_knowledgeinteractioninsight = "user_msdyn_knowledgeinteractioninsight";
            [Relationship("msdyn_knowledgemanagementsetting", EntityRole.Referenced, "user_msdyn_knowledgemanagementsetting", "owninguser")]
            public const string user_msdyn_knowledgemanagementsetting = "user_msdyn_knowledgemanagementsetting";
            [Relationship("msdyn_knowledgepersonalfilter", EntityRole.Referenced, "user_msdyn_knowledgepersonalfilter", "owninguser")]
            public const string user_msdyn_knowledgepersonalfilter = "user_msdyn_knowledgepersonalfilter";
            [Relationship("msdyn_knowledgesearchfilter", EntityRole.Referenced, "user_msdyn_knowledgesearchfilter", "owninguser")]
            public const string user_msdyn_knowledgesearchfilter = "user_msdyn_knowledgesearchfilter";
            [Relationship("msdyn_knowledgesearchinsight", EntityRole.Referenced, "user_msdyn_knowledgesearchinsight", "owninguser")]
            public const string user_msdyn_knowledgesearchinsight = "user_msdyn_knowledgesearchinsight";
            [Relationship("msdyn_kpieventdata", EntityRole.Referenced, "user_msdyn_kpieventdata", "owninguser")]
            public const string user_msdyn_kpieventdata = "user_msdyn_kpieventdata";
            [Relationship("msdyn_kpieventdefinition", EntityRole.Referenced, "user_msdyn_kpieventdefinition", "owninguser")]
            public const string user_msdyn_kpieventdefinition = "user_msdyn_kpieventdefinition";
            [Relationship("msdyn_leadmodelconfig", EntityRole.Referenced, "user_msdyn_leadmodelconfig", "owninguser")]
            public const string user_msdyn_leadmodelconfig = "user_msdyn_leadmodelconfig";
            [Relationship("msdyn_macrosession", EntityRole.Referenced, "user_msdyn_macrosession", "owninguser")]
            public const string user_msdyn_macrosession = "user_msdyn_macrosession";
            [Relationship("msdyn_migrationtracker", EntityRole.Referenced, "user_msdyn_migrationtracker", "owninguser")]
            public const string user_msdyn_migrationtracker = "user_msdyn_migrationtracker";
            [Relationship("msdyn_modelpreviewstatus", EntityRole.Referenced, "user_msdyn_modelpreviewstatus", "owninguser")]
            public const string user_msdyn_modelpreviewstatus = "user_msdyn_modelpreviewstatus";
            [Relationship("msdyn_msteamssetting", EntityRole.Referenced, "user_msdyn_msteamssetting", "owninguser")]
            public const string user_msdyn_msteamssetting = "user_msdyn_msteamssetting";
            [Relationship("msdyn_notesanalysisconfig", EntityRole.Referenced, "user_msdyn_notesanalysisconfig", "owninguser")]
            public const string user_msdyn_notesanalysisconfig = "user_msdyn_notesanalysisconfig";
            [Relationship("msdyn_notificationfield", EntityRole.Referenced, "user_msdyn_notificationfield", "owninguser")]
            public const string user_msdyn_notificationfield = "user_msdyn_notificationfield";
            [Relationship("msdyn_notificationtemplate", EntityRole.Referenced, "user_msdyn_notificationtemplate", "owninguser")]
            public const string user_msdyn_notificationtemplate = "user_msdyn_notificationtemplate";
            [Relationship("msdyn_opportunitymodelconfig", EntityRole.Referenced, "user_msdyn_opportunitymodelconfig", "owninguser")]
            public const string user_msdyn_opportunitymodelconfig = "user_msdyn_opportunitymodelconfig";
            [Relationship("msdyn_orderinvoicingdate", EntityRole.Referenced, "user_msdyn_orderinvoicingdate", "owninguser")]
            public const string user_msdyn_orderinvoicingdate = "user_msdyn_orderinvoicingdate";
            [Relationship("msdyn_orderinvoicingproduct", EntityRole.Referenced, "user_msdyn_orderinvoicingproduct", "owninguser")]
            public const string user_msdyn_orderinvoicingproduct = "user_msdyn_orderinvoicingproduct";
            [Relationship("msdyn_orderinvoicingsetup", EntityRole.Referenced, "user_msdyn_orderinvoicingsetup", "owninguser")]
            public const string user_msdyn_orderinvoicingsetup = "user_msdyn_orderinvoicingsetup";
            [Relationship("msdyn_orderinvoicingsetupdate", EntityRole.Referenced, "user_msdyn_orderinvoicingsetupdate", "owninguser")]
            public const string user_msdyn_orderinvoicingsetupdate = "user_msdyn_orderinvoicingsetupdate";
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
            [Relationship("msdyn_pminferredtask", EntityRole.Referenced, "user_msdyn_pminferredtask", "owninguser")]
            public const string user_msdyn_pminferredtask = "user_msdyn_pminferredtask";
            [Relationship("msdyn_pmrecording", EntityRole.Referenced, "user_msdyn_pmrecording", "owninguser")]
            public const string user_msdyn_pmrecording = "user_msdyn_pmrecording";
            [Relationship("msdyn_postalbum", EntityRole.Referenced, "user_msdyn_postalbum", "owninguser")]
            public const string user_msdyn_postalbum = "user_msdyn_postalbum";
            [Relationship("msdyn_postalcode", EntityRole.Referenced, "user_msdyn_postalcode", "owninguser")]
            public const string user_msdyn_postalcode = "user_msdyn_postalcode";
            [Relationship("msdyn_priority", EntityRole.Referenced, "user_msdyn_priority", "owninguser")]
            public const string user_msdyn_priority = "user_msdyn_priority";
            [Relationship("msdyn_problematicasset", EntityRole.Referenced, "user_msdyn_problematicasset", "owninguser")]
            public const string user_msdyn_problematicasset = "user_msdyn_problematicasset";
            [Relationship("msdyn_problematicassetfeedback", EntityRole.Referenced, "user_msdyn_problematicassetfeedback", "owninguser")]
            public const string user_msdyn_problematicassetfeedback = "user_msdyn_problematicassetfeedback";
            [Relationship("msdyn_productivityactioninputparameter", EntityRole.Referenced, "user_msdyn_productivityactioninputparameter", "owninguser")]
            public const string user_msdyn_productivityactioninputparameter = "user_msdyn_productivityactioninputparameter";
            [Relationship("msdyn_productivityactionoutputparameter", EntityRole.Referenced, "user_msdyn_productivityactionoutputparameter", "owninguser")]
            public const string user_msdyn_productivityactionoutputparameter = "user_msdyn_productivityactionoutputparameter";
            [Relationship("msdyn_productivityagentscript", EntityRole.Referenced, "user_msdyn_productivityagentscript", "owninguser")]
            public const string user_msdyn_productivityagentscript = "user_msdyn_productivityagentscript";
            [Relationship("msdyn_productivityagentscriptstep", EntityRole.Referenced, "user_msdyn_productivityagentscriptstep", "owninguser")]
            public const string user_msdyn_productivityagentscriptstep = "user_msdyn_productivityagentscriptstep";
            [Relationship("msdyn_productivitymacroactiontemplate", EntityRole.Referenced, "user_msdyn_productivitymacroactiontemplate", "owninguser")]
            public const string user_msdyn_productivitymacroactiontemplate = "user_msdyn_productivitymacroactiontemplate";
            [Relationship("msdyn_productivitymacroconnector", EntityRole.Referenced, "user_msdyn_productivitymacroconnector", "owninguser")]
            public const string user_msdyn_productivitymacroconnector = "user_msdyn_productivitymacroconnector";
            [Relationship("msdyn_productivitymacrosolutionconfiguration", EntityRole.Referenced, "user_msdyn_productivitymacrosolutionconfiguration", "owninguser")]
            public const string user_msdyn_productivitymacrosolutionconfiguration = "user_msdyn_productivitymacrosolutionconfiguration";
            [Relationship("msdyn_productivityparameterdefinition", EntityRole.Referenced, "user_msdyn_productivityparameterdefinition", "owninguser")]
            public const string user_msdyn_productivityparameterdefinition = "user_msdyn_productivityparameterdefinition";
            [Relationship("msdyn_property", EntityRole.Referenced, "user_msdyn_property", "owninguser")]
            public const string user_msdyn_property = "user_msdyn_property";
            [Relationship("msdyn_propertyassetassociation", EntityRole.Referenced, "user_msdyn_propertyassetassociation", "owninguser")]
            public const string user_msdyn_propertyassetassociation = "user_msdyn_propertyassetassociation";
            [Relationship("msdyn_propertylog", EntityRole.Referenced, "user_msdyn_propertylog", "owninguser")]
            public const string user_msdyn_propertylog = "user_msdyn_propertylog";
            [Relationship("msdyn_propertytemplateassociation", EntityRole.Referenced, "user_msdyn_propertytemplateassociation", "owninguser")]
            public const string user_msdyn_propertytemplateassociation = "user_msdyn_propertytemplateassociation";
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
            [Relationship("msdyn_recording", EntityRole.Referenced, "user_msdyn_recording", "owninguser")]
            public const string user_msdyn_recording = "user_msdyn_recording";
            [Relationship("msdyn_relationshipinsightsunifiedconfig", EntityRole.Referenced, "user_msdyn_relationshipinsightsunifiedconfig", "owninguser")]
            public const string user_msdyn_relationshipinsightsunifiedconfig = "user_msdyn_relationshipinsightsunifiedconfig";
            [Relationship("msdyn_requirementcharacteristic", EntityRole.Referenced, "user_msdyn_requirementcharacteristic", "owninguser")]
            public const string user_msdyn_requirementcharacteristic = "user_msdyn_requirementcharacteristic";
            [Relationship("msdyn_requirementdependency", EntityRole.Referenced, "user_msdyn_requirementdependency", "owninguser")]
            public const string user_msdyn_requirementdependency = "user_msdyn_requirementdependency";
            [Relationship("msdyn_requirementgroup", EntityRole.Referenced, "user_msdyn_requirementgroup", "owninguser")]
            public const string user_msdyn_requirementgroup = "user_msdyn_requirementgroup";
            [Relationship("msdyn_requirementorganizationunit", EntityRole.Referenced, "user_msdyn_requirementorganizationunit", "owninguser")]
            public const string user_msdyn_requirementorganizationunit = "user_msdyn_requirementorganizationunit";
            [Relationship("msdyn_requirementrelationship", EntityRole.Referenced, "user_msdyn_requirementrelationship", "owninguser")]
            public const string user_msdyn_requirementrelationship = "user_msdyn_requirementrelationship";
            [Relationship("msdyn_requirementresourcecategory", EntityRole.Referenced, "user_msdyn_requirementresourcecategory", "owninguser")]
            public const string user_msdyn_requirementresourcecategory = "user_msdyn_requirementresourcecategory";
            [Relationship("msdyn_requirementresourcepreference", EntityRole.Referenced, "user_msdyn_requirementresourcepreference", "owninguser")]
            public const string user_msdyn_requirementresourcepreference = "user_msdyn_requirementresourcepreference";
            [Relationship("msdyn_requirementstatus", EntityRole.Referenced, "user_msdyn_requirementstatus", "owninguser")]
            public const string user_msdyn_requirementstatus = "user_msdyn_requirementstatus";
            [Relationship("msdyn_resolution", EntityRole.Referenced, "user_msdyn_resolution", "owninguser")]
            public const string user_msdyn_resolution = "user_msdyn_resolution";
            [Relationship("msdyn_resourcepaytype", EntityRole.Referenced, "user_msdyn_resourcepaytype", "owninguser")]
            public const string user_msdyn_resourcepaytype = "user_msdyn_resourcepaytype";
            [Relationship("msdyn_resourcerequirement", EntityRole.Referenced, "user_msdyn_resourcerequirement", "owninguser")]
            public const string user_msdyn_resourcerequirement = "user_msdyn_resourcerequirement";
            [Relationship("msdyn_resourcerequirementdetail", EntityRole.Referenced, "user_msdyn_resourcerequirementdetail", "owninguser")]
            public const string user_msdyn_resourcerequirementdetail = "user_msdyn_resourcerequirementdetail";
            [Relationship("msdyn_resourceterritory", EntityRole.Referenced, "user_msdyn_resourceterritory", "owninguser")]
            public const string user_msdyn_resourceterritory = "user_msdyn_resourceterritory";
            [Relationship("msdyn_richtextfile", EntityRole.Referenced, "user_msdyn_richtextfile", "owninguser")]
            public const string user_msdyn_richtextfile = "user_msdyn_richtextfile";
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
            [Relationship("msdyn_rtv", EntityRole.Referenced, "user_msdyn_rtv", "owninguser")]
            public const string user_msdyn_rtv = "user_msdyn_rtv";
            [Relationship("msdyn_rtvproduct", EntityRole.Referenced, "user_msdyn_rtvproduct", "owninguser")]
            public const string user_msdyn_rtvproduct = "user_msdyn_rtvproduct";
            [Relationship("msdyn_rtvsubstatus", EntityRole.Referenced, "user_msdyn_rtvsubstatus", "owninguser")]
            public const string user_msdyn_rtvsubstatus = "user_msdyn_rtvsubstatus";
            [Relationship("msdyn_salesinsightssettings", EntityRole.Referenced, "user_msdyn_salesinsightssettings", "owninguser")]
            public const string user_msdyn_salesinsightssettings = "user_msdyn_salesinsightssettings";
            [Relationship("msdyn_salesroutingrun", EntityRole.Referenced, "user_msdyn_salesroutingrun", "owninguser")]
            public const string user_msdyn_salesroutingrun = "user_msdyn_salesroutingrun";
            [Relationship("msdyn_salessuggestion", EntityRole.Referenced, "user_msdyn_salessuggestion", "owninguser")]
            public const string user_msdyn_salessuggestion = "user_msdyn_salessuggestion";
            [Relationship("msdyn_salestag", EntityRole.Referenced, "user_msdyn_salestag", "owninguser")]
            public const string user_msdyn_salestag = "user_msdyn_salestag";
            [Relationship("msdyn_scheduleboardsetting", EntityRole.Referenced, "user_msdyn_scheduleboardsetting", "owninguser")]
            public const string user_msdyn_scheduleboardsetting = "user_msdyn_scheduleboardsetting";
            [Relationship("msdyn_schedulingfeatureflag", EntityRole.Referenced, "user_msdyn_schedulingfeatureflag", "owninguser")]
            public const string user_msdyn_schedulingfeatureflag = "user_msdyn_schedulingfeatureflag";
            [Relationship("msdyn_segment", EntityRole.Referenced, "user_msdyn_segment", "owninguser")]
            public const string user_msdyn_segment = "user_msdyn_segment";
            [Relationship("msdyn_sequence", EntityRole.Referenced, "user_msdyn_sequence", "owninguser")]
            public const string user_msdyn_sequence = "user_msdyn_sequence";
            [Relationship("msdyn_sequencestat", EntityRole.Referenced, "user_msdyn_sequencestat", "owninguser")]
            public const string user_msdyn_sequencestat = "user_msdyn_sequencestat";
            [Relationship("msdyn_sequencetarget", EntityRole.Referenced, "user_msdyn_sequencetarget", "owninguser")]
            public const string user_msdyn_sequencetarget = "user_msdyn_sequencetarget";
            [Relationship("msdyn_sequencetargetstep", EntityRole.Referenced, "user_msdyn_sequencetargetstep", "owninguser")]
            public const string user_msdyn_sequencetargetstep = "user_msdyn_sequencetargetstep";
            [Relationship("msdyn_serviceconfiguration", EntityRole.Referenced, "user_msdyn_serviceconfiguration", "owninguser")]
            public const string user_msdyn_serviceconfiguration = "user_msdyn_serviceconfiguration";
            [Relationship("msdyn_servicetasktype", EntityRole.Referenced, "user_msdyn_servicetasktype", "owninguser")]
            public const string user_msdyn_servicetasktype = "user_msdyn_servicetasktype";
            [Relationship("msdyn_sessiondata", EntityRole.Referenced, "user_msdyn_sessiondata", "owninguser")]
            public const string user_msdyn_sessiondata = "user_msdyn_sessiondata";
            [Relationship("msdyn_sessionparticipantdata", EntityRole.Referenced, "user_msdyn_sessionparticipantdata", "owninguser")]
            public const string user_msdyn_sessionparticipantdata = "user_msdyn_sessionparticipantdata";
            [Relationship("msdyn_sessiontemplate", EntityRole.Referenced, "user_msdyn_sessiontemplate", "owninguser")]
            public const string user_msdyn_sessiontemplate = "user_msdyn_sessiontemplate";
            [Relationship("msdyn_shipvia", EntityRole.Referenced, "user_msdyn_shipvia", "owninguser")]
            public const string user_msdyn_shipvia = "user_msdyn_shipvia";
            [Relationship("msdyn_siconfig", EntityRole.Referenced, "user_msdyn_siconfig", "owninguser")]
            public const string user_msdyn_siconfig = "user_msdyn_siconfig";
            [Relationship("msdyn_slakpi", EntityRole.Referenced, "user_msdyn_slakpi", "owninguser")]
            public const string user_msdyn_slakpi = "user_msdyn_slakpi";
            [Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "user_msdyn_solutionhealthrule", "owninguser")]
            public const string user_msdyn_solutionhealthrule = "user_msdyn_solutionhealthrule";
            [Relationship("msdyn_solutionhealthruleargument", EntityRole.Referenced, "user_msdyn_solutionhealthruleargument", "owninguser")]
            public const string user_msdyn_solutionhealthruleargument = "user_msdyn_solutionhealthruleargument";
            [Relationship("msdyn_systemuserschedulersetting", EntityRole.Referenced, "user_msdyn_systemuserschedulersetting", "owninguser")]
            public const string user_msdyn_systemuserschedulersetting = "user_msdyn_systemuserschedulersetting";
            [Relationship("msdyn_taxcode", EntityRole.Referenced, "user_msdyn_taxcode", "owninguser")]
            public const string user_msdyn_taxcode = "user_msdyn_taxcode";
            [Relationship("msdyn_taxcodedetail", EntityRole.Referenced, "user_msdyn_taxcodedetail", "owninguser")]
            public const string user_msdyn_taxcodedetail = "user_msdyn_taxcodedetail";
            [Relationship("msdyn_templateforproperties", EntityRole.Referenced, "user_msdyn_templateforproperties", "owninguser")]
            public const string user_msdyn_templateforproperties = "user_msdyn_templateforproperties";
            [Relationship("msdyn_templateparameter", EntityRole.Referenced, "user_msdyn_templateparameter", "owninguser")]
            public const string user_msdyn_templateparameter = "user_msdyn_templateparameter";
            [Relationship("msdyn_timeentry", EntityRole.Referenced, "user_msdyn_timeentry", "owninguser")]
            public const string user_msdyn_timeentry = "user_msdyn_timeentry";
            [Relationship("msdyn_timeentrysetting", EntityRole.Referenced, "user_msdyn_timeentrysetting", "owninguser")]
            public const string user_msdyn_timeentrysetting = "user_msdyn_timeentrysetting";
            [Relationship("msdyn_timegroup", EntityRole.Referenced, "user_msdyn_timegroup", "owninguser")]
            public const string user_msdyn_timegroup = "user_msdyn_timegroup";
            [Relationship("msdyn_timegroupdetail", EntityRole.Referenced, "user_msdyn_timegroupdetail", "owninguser")]
            public const string user_msdyn_timegroupdetail = "user_msdyn_timegroupdetail";
            [Relationship("msdyn_timeoffrequest", EntityRole.Referenced, "user_msdyn_timeoffrequest", "owninguser")]
            public const string user_msdyn_timeoffrequest = "user_msdyn_timeoffrequest";
            [Relationship("msdyn_transactionorigin", EntityRole.Referenced, "user_msdyn_transactionorigin", "owninguser")]
            public const string user_msdyn_transactionorigin = "user_msdyn_transactionorigin";
            [Relationship("msdyn_untrackedappointment", EntityRole.Referenced, "user_msdyn_untrackedappointment", "owninguser")]
            public const string user_msdyn_untrackedappointment = "user_msdyn_untrackedappointment";
            [Relationship("msdyn_wallsavedqueryusersettings", EntityRole.Referenced, "user_msdyn_wallsavedqueryusersettings", "owninguser")]
            public const string user_msdyn_wallsavedqueryusersettings = "user_msdyn_wallsavedqueryusersettings";
            [Relationship("msdyn_warehouse", EntityRole.Referenced, "user_msdyn_warehouse", "owninguser")]
            public const string user_msdyn_warehouse = "user_msdyn_warehouse";
            [Relationship("msdyn_wkwconfig", EntityRole.Referenced, "user_msdyn_wkwconfig", "owninguser")]
            public const string user_msdyn_wkwconfig = "user_msdyn_wkwconfig";
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
            [Relationship("msdyn_workorderresolution", EntityRole.Referenced, "user_msdyn_workorderresolution", "owninguser")]
            public const string user_msdyn_workorderresolution = "user_msdyn_workorderresolution";
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
            [Relationship("msdyn_workqueuestate", EntityRole.Referenced, "user_msdyn_workqueuestate", "owninguser")]
            public const string user_msdyn_workqueuestate = "user_msdyn_workqueuestate";
            [Relationship("msdyn_workqueueusersetting", EntityRole.Referenced, "user_msdyn_workqueueusersetting", "owninguser")]
            public const string user_msdyn_workqueueusersetting = "user_msdyn_workqueueusersetting";
            [Relationship("msdynce_botcontent", EntityRole.Referenced, "user_msdynce_botcontent", "owninguser")]
            public const string user_msdynce_botcontent = "user_msdynce_botcontent";
            [Relationship("msfp_alertrule", EntityRole.Referenced, "user_msfp_alertrule", "owninguser")]
            public const string user_msfp_alertrule = "user_msfp_alertrule";
            [Relationship("msfp_emailtemplate", EntityRole.Referenced, "user_msfp_emailtemplate", "owninguser")]
            public const string user_msfp_emailtemplate = "user_msfp_emailtemplate";
            [Relationship("msfp_fileresponse", EntityRole.Referenced, "user_msfp_fileresponse", "owninguser")]
            public const string user_msfp_fileresponse = "user_msfp_fileresponse";
            [Relationship("msfp_localizedemailtemplate", EntityRole.Referenced, "user_msfp_localizedemailtemplate", "owninguser")]
            public const string user_msfp_localizedemailtemplate = "user_msfp_localizedemailtemplate";
            [Relationship("msfp_project", EntityRole.Referenced, "user_msfp_project", "owninguser")]
            public const string user_msfp_project = "user_msfp_project";
            [Relationship("msfp_question", EntityRole.Referenced, "user_msfp_question", "owninguser")]
            public const string user_msfp_question = "user_msfp_question";
            [Relationship("msfp_questionresponse", EntityRole.Referenced, "user_msfp_questionresponse", "owninguser")]
            public const string user_msfp_questionresponse = "user_msfp_questionresponse";
            [Relationship("msfp_satisfactionmetric", EntityRole.Referenced, "user_msfp_satisfactionmetric", "owninguser")]
            public const string user_msfp_satisfactionmetric = "user_msfp_satisfactionmetric";
            [Relationship("msfp_survey", EntityRole.Referenced, "user_msfp_survey", "owninguser")]
            public const string user_msfp_survey = "user_msfp_survey";
            [Relationship("msfp_surveyreminder", EntityRole.Referenced, "user_msfp_surveyreminder", "owninguser")]
            public const string user_msfp_surveyreminder = "user_msfp_surveyreminder";
            [Relationship("msfp_unsubscribedrecipient", EntityRole.Referenced, "user_msfp_unsubscribedrecipient", "owninguser")]
            public const string user_msfp_unsubscribedrecipient = "user_msfp_unsubscribedrecipient";
            [Relationship("new_etudient", EntityRole.Referenced, "user_new_etudient", "owninguser")]
            public const string user_new_etudient = "user_new_etudient";
            [Relationship("new_m_apply", EntityRole.Referenced, "user_new_m_apply", "owninguser")]
            public const string user_new_m_apply = "user_new_m_apply";
            [Relationship("new_m_grade", EntityRole.Referenced, "user_new_m_grade", "owninguser")]
            public const string user_new_m_grade = "user_new_m_grade";
            [Relationship("new_m_lesson", EntityRole.Referenced, "user_new_m_lesson", "owninguser")]
            public const string user_new_m_lesson = "user_new_m_lesson";
            [Relationship("new_m_school", EntityRole.Referenced, "user_new_m_school", "owninguser")]
            public const string user_new_m_school = "user_new_m_school";
            [Relationship("new_m_student", EntityRole.Referenced, "user_new_m_student", "owninguser")]
            public const string user_new_m_student = "user_new_m_student";
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
            [Relationship("pdfsetting", EntityRole.Referenced, "user_pdfsetting", "owninguser")]
            public const string user_pdfsetting = "user_pdfsetting";
            [Relationship("phonecall", EntityRole.Referenced, "user_phonecall", "owninguser")]
            public const string user_phonecall = "user_phonecall";
            [Relationship("pi_course", EntityRole.Referenced, "user_pi_course", "owninguser")]
            public const string user_pi_course = "user_pi_course";
            [Relationship("pi_courseinscription", EntityRole.Referenced, "user_pi_courseinscription", "owninguser")]
            public const string user_pi_courseinscription = "user_pi_courseinscription";
            [Relationship("pi_school", EntityRole.Referenced, "user_pi_school", "owninguser")]
            public const string user_pi_school = "user_pi_school";
            [Relationship("pi_student", EntityRole.Referenced, "user_pi_student", "owninguser")]
            public const string user_pi_student = "user_pi_student";
            [Relationship("pi_studentgroup", EntityRole.Referenced, "user_pi_studentgroup", "owninguser")]
            public const string user_pi_studentgroup = "user_pi_studentgroup";
            [Relationship("processstageparameter", EntityRole.Referenced, "user_processstageparameter", "owninguser")]
            public const string user_processstageparameter = "user_processstageparameter";
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
            [Relationship("revokeinheritedaccessrecordstracker", EntityRole.Referenced, "user_revokeinheritedaccessrecordstracker", "owninguser")]
            public const string user_revokeinheritedaccessrecordstracker = "user_revokeinheritedaccessrecordstracker";
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
            [Relationship("solutioncomponentbatchconfiguration", EntityRole.Referenced, "user_solutioncomponentbatchconfiguration", "owninguser")]
            public const string user_solutioncomponentbatchconfiguration = "user_solutioncomponentbatchconfiguration";
            [Relationship("stagesolutionupload", EntityRole.Referenced, "user_stagesolutionupload", "owninguser")]
            public const string user_stagesolutionupload = "user_stagesolutionupload";
            [Relationship("synapsedatabase", EntityRole.Referenced, "user_synapsedatabase", "owninguser")]
            public const string user_synapsedatabase = "user_synapsedatabase";
            [Relationship("task", EntityRole.Referenced, "user_task", "owninguser")]
            public const string user_task = "user_task";
            [Relationship("untrackedemail", EntityRole.Referenced, "user_untrackedemail", "owninguser")]
            public const string user_untrackedemail = "user_untrackedemail";
            [Relationship("userapplicationmetadata", EntityRole.Referenced, "user_userapplicationmetadata", "owninguser")]
            public const string user_userapplicationmetadata = "user_userapplicationmetadata";
            [Relationship("systemuserauthorizationchangetracker", EntityRole.Referenced, "user_userauthztracker", "systemuserid")]
            public const string user_userauthztracker = "user_userauthztracker";
            [Relationship("userform", EntityRole.Referenced, "user_userform", "owninguser")]
            public const string user_userform = "user_userform";
            [Relationship("userquery", EntityRole.Referenced, "user_userquery", "owninguser")]
            public const string user_userquery = "user_userquery";
            [Relationship("userqueryvisualization", EntityRole.Referenced, "user_userqueryvisualizations", "owninguser")]
            public const string user_userqueryvisualizations = "user_userqueryvisualizations";
            [Relationship("workflowbinary", EntityRole.Referenced, "user_workflowbinary", "owninguser")]
            public const string user_workflowbinary = "user_workflowbinary";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_owning_user", "owninguser")]
            public const string userentityinstancedata_owning_user = "userentityinstancedata_owning_user";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_systemuser", "objectid")]
            public const string userentityinstancedata_systemuser = "userentityinstancedata_systemuser";
            [Relationship("userentityuisettings", EntityRole.Referenced, "userentityuisettings_owning_user", "owninguser")]
            public const string userentityuisettings_owning_user = "userentityuisettings_owning_user";
            [Relationship(WebResourceDefinition.EntityName, EntityRole.Referenced, "webresource_createdby", "createdby")]
            public const string webresource_createdby = "webresource_createdby";
            [Relationship(WebResourceDefinition.EntityName, EntityRole.Referenced, "webresource_modifiedby", "modifiedby")]
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
