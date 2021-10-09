using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Definitions.Internal
{
	[GeneratedCode("XrmFramework", "2.0")]
	[EntityDefinition]
	[ExcludeFromCodeCoverage]
	public static class SdkMessageProcessingStepDefinition
	{
		public const string EntityName = "sdkmessageprocessingstep";
		public const string EntityCollectionName = "sdkmessageprocessingsteps";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
			/// <summary>
			/// 
			/// Type : Boolean
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Boolean)]
			public const string AsyncAutoDelete = "asyncautodelete";

			/// <summary>
			/// 
			/// Type : Boolean
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Boolean)]
			public const string CanUseReadOnlyConnection = "canusereadonlyconnection";

			/// <summary>
			/// 
			/// Type : Picklist (ComponentState)
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Picklist)]
			[OptionSet(typeof(ComponentState))]
			public const string ComponentState = "componentstate";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(1073741823)]
			public const string Configuration = "configuration";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(256)]
			public const string Description = "description";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(1073741823)]
			public const string EventExpander = "eventexpander";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(PluginTypeDefinition.EntityName, PluginTypeDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.plugintype_sdkmessageprocessingstep)]
			[CrmLookup("serviceendpoint", "serviceendpointid", RelationshipName = "serviceendpoint_sdkmessageprocessingstep")]
			public const string EventHandler = "eventhandler";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100000)]
			public const string FilteringAttributes = "filteringattributes";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(SystemUserDefinition.EntityName, SystemUserDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.impersonatinguserid_sdkmessageprocessingstep)]
			public const string ImpersonatingUserId = "impersonatinguserid";

			/// <summary>
			/// 
			/// Type : ManagedProperty
			/// Validity :  Read | Create | Update 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.ManagedProperty)]
			public const string IsCustomizable = "iscustomizable";

			/// <summary>
			/// 
			/// Type : ManagedProperty
			/// Validity :  Read | Create | Update 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.ManagedProperty)]
			public const string IsHidden = "ishidden";

			/// <summary>
			/// 
			/// Type : Boolean
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Boolean)]
			public const string IsManaged = "ismanaged";

			/// <summary>
			/// 
			/// Type : Picklist (Mode)
			/// Validity :  Read | Create | Update 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Picklist)]
			[OptionSet(typeof(Mode))]
			public const string Mode = "mode";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[PrimaryAttribute(PrimaryAttributeType.Name)]
			[StringLength(256)]
			public const string Name = "name";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(PluginTypeDefinition.EntityName, PluginTypeDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.plugintypeid_sdkmessageprocessingstep)]
			public const string PluginTypeId = "plugintypeid";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup("sdkmessagefilter", "sdkmessagefilterid", RelationshipName = "sdkmessagefilterid_sdkmessageprocessingstep")]
			public const string SdkMessageFilterId = "sdkmessagefilterid";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup("sdkmessage", "sdkmessageid", RelationshipName = "sdkmessageid_sdkmessageprocessingstep")]
			public const string SdkMessageId = "sdkmessageid";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "sdkmessageprocessingstepid";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			public const string SdkMessageProcessingStepIdUnique = "sdkmessageprocessingstepidunique";

			/// <summary>
			/// 
			/// Type : Lookup
			/// Validity :  Read | Create | Update 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Lookup)]
			[CrmLookup(SdkMessageProcessingStepSecureConfigDefinition.EntityName, SdkMessageProcessingStepSecureConfigDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.sdkmessageprocessingstepsecureconfigid_sdkmessageprocessingstep)]
			public const string SdkMessageProcessingStepSecureConfigId = "sdkmessageprocessingstepsecureconfigid";

			/// <summary>
			/// 
			/// Type : State (SdkmessageprocessingstepState)
			/// Validity :  Read | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.State)]
			[OptionSet(typeof(SdkmessageprocessingstepState))]
			public const string StateCode = "statecode";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string createdby_sdkmessageprocessingstep = "createdby_sdkmessageprocessingstep";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "impersonatinguserid", SdkMessageProcessingStepDefinition.Columns.ImpersonatingUserId)]
			public const string impersonatinguserid_sdkmessageprocessingstep = "impersonatinguserid_sdkmessageprocessingstep";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessageprocessingstep_createdonbehalfby = "lk_sdkmessageprocessingstep_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessageprocessingstep_modifiedonbehalfby = "lk_sdkmessageprocessingstep_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string modifiedby_sdkmessageprocessingstep = "modifiedby_sdkmessageprocessingstep";
			[Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
			public const string organization_sdkmessageprocessingstep = "organization_sdkmessageprocessingstep";
			[Relationship(PluginTypeDefinition.EntityName, EntityRole.Referencing, "eventhandler_plugintype", SdkMessageProcessingStepDefinition.Columns.EventHandler)]
			public const string plugintype_sdkmessageprocessingstep = "plugintype_sdkmessageprocessingstep";
			[Relationship(PluginTypeDefinition.EntityName, EntityRole.Referencing, "plugintypeid", SdkMessageProcessingStepDefinition.Columns.PluginTypeId)]
			public const string plugintypeid_sdkmessageprocessingstep = "plugintypeid_sdkmessageprocessingstep";
			[Relationship("sdkmessagefilter", EntityRole.Referencing, "sdkmessagefilterid", SdkMessageProcessingStepDefinition.Columns.SdkMessageFilterId)]
			public const string sdkmessagefilterid_sdkmessageprocessingstep = "sdkmessagefilterid_sdkmessageprocessingstep";
			[Relationship("sdkmessage", EntityRole.Referencing, "sdkmessageid", SdkMessageProcessingStepDefinition.Columns.SdkMessageId)]
			public const string sdkmessageid_sdkmessageprocessingstep = "sdkmessageid_sdkmessageprocessingstep";
			[Relationship(SdkMessageProcessingStepSecureConfigDefinition.EntityName, EntityRole.Referencing, "sdkmessageprocessingstepsecureconfigid", SdkMessageProcessingStepDefinition.Columns.SdkMessageProcessingStepSecureConfigId)]
			public const string sdkmessageprocessingstepsecureconfigid_sdkmessageprocessingstep = "sdkmessageprocessingstepsecureconfigid_sdkmessageprocessingstep";
			[Relationship("serviceendpoint", EntityRole.Referencing, "eventhandler_serviceendpoint", SdkMessageProcessingStepDefinition.Columns.EventHandler)]
			public const string serviceendpoint_sdkmessageprocessingstep = "serviceendpoint_sdkmessageprocessingstep";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("asyncoperation", EntityRole.Referenced, "SdkMessageProcessingStep_AsyncOperations", "owningextensionid")]
			public const string SdkMessageProcessingStep_AsyncOperations = "SdkMessageProcessingStep_AsyncOperations";
			[Relationship(SdkMessageProcessingStepImageDefinition.EntityName, EntityRole.Referenced, "sdkmessageprocessingstepid_sdkmessageprocessingstepimage", "sdkmessageprocessingstepid")]
			public const string sdkmessageprocessingstepid_sdkmessageprocessingstepimage = "sdkmessageprocessingstepid_sdkmessageprocessingstepimage";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_sdkmessageprocessingstep", "objectid")]
			public const string userentityinstancedata_sdkmessageprocessingstep = "userentityinstancedata_sdkmessageprocessingstep";
		}
    }

    [OptionSetDefinition("componentstate")]
    public enum ComponentState
    {
        [Description("Published")]
        Published = 0,
        [Description("Unpublished")]
        Unpublished = 1,
        [Description("Deleted")]
        Deleted = 2,
        [Description("Deleted Unpublished")]
        DeletedUnpublished = 3,
    }

    [OptionSetDefinition(SdkMessageProcessingStepDefinition.EntityName, SdkMessageProcessingStepDefinition.Columns.Mode)]
    public enum Mode
    {
        [Description("Synchronous")]
        Synchronous = 0,
        [Description("Asynchronous")]
        Asynchronous = 1,
    }

    [OptionSetDefinition(SdkMessageProcessingStepDefinition.EntityName, SdkMessageProcessingStepDefinition.Columns.StateCode)]
    public enum SdkmessageprocessingstepState
    {
        [Description("Enabled")]
        Enabled = 0,
        [Description("Disabled")]
        Disabled = 1,
    }
}
