using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Definitions
{
	[GeneratedCode("XrmFramework", "1.0")]
	[EntityDefinition]

	[ExcludeFromCodeCoverage]
    [DefinitionManagerIgnore]

    public static class PluginTypeDefinition
	{
		public const string EntityName = "plugintype";
		public const string EntityCollectionName = "plugintypes";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(100)]
			public const string AssemblyName = "assemblyname";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(32)]
			public const string Culture = "culture";

			/// <summary>
			/// 
			/// Type : Memo
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Memo)]
			[StringLength(1048576)]
			public const string CustomWorkflowActivityInfo = "customworkflowactivityinfo";

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
			[StringLength(256)]
			public const string FriendlyName = "friendlyname";

			/// <summary>
			/// 
			/// Type : Boolean
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Boolean)]
			public const string IsWorkflowActivity = "isworkflowactivity";

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
			[CrmLookup(PluginAssemblyDefinition.EntityName, PluginAssemblyDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.pluginassembly_plugintype)]
			public const string PluginAssemblyId = "pluginassemblyid";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "plugintypeid";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			public const string PluginTypeIdUnique = "plugintypeidunique";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(32)]
			public const string PublicKeyToken = "publickeytoken";

			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			public const string SolutionId = "solutionid";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(256)]
			public const string TypeName = "typename";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(48)]
			public const string Version = "version";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string createdby_plugintype = "createdby_plugintype";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_plugintype_createdonbehalfby = "lk_plugintype_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_plugintype_modifiedonbehalfby = "lk_plugintype_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string modifiedby_plugintype = "modifiedby_plugintype";
			[Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
			public const string organization_plugintype = "organization_plugintype";
			[Relationship(PluginAssemblyDefinition.EntityName, EntityRole.Referencing, "pluginassemblyid", PluginTypeDefinition.Columns.PluginAssemblyId)]
			public const string pluginassembly_plugintype = "pluginassembly_plugintype";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship("service", EntityRole.Referenced, "plugin_type_service", "strategyid")]
			public const string plugin_type_service = "plugin_type_service";
			[Relationship("plugintypestatistic", EntityRole.Referenced, "plugintype_plugintypestatistic", "plugintypeid")]
			public const string plugintype_plugintypestatistic = "plugintype_plugintypestatistic";
			[Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "plugintype_sdkmessageprocessingstep", SdkMessageProcessingStepDefinition.Columns.EventHandler)]
			public const string plugintype_sdkmessageprocessingstep = "plugintype_sdkmessageprocessingstep";
			[Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "plugintypeid_sdkmessageprocessingstep", SdkMessageProcessingStepDefinition.Columns.PluginTypeId)]
			public const string plugintypeid_sdkmessageprocessingstep = "plugintypeid_sdkmessageprocessingstep";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_plugintype", "objectid")]
			public const string userentityinstancedata_plugintype = "userentityinstancedata_plugintype";
		}
	}
}
