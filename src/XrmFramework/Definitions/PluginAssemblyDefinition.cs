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
	public static class PluginAssemblyDefinition
	{
		public const string EntityName = "pluginassembly";
		public const string EntityCollectionName = "pluginassemblies";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(32)]
			public const string Culture = "culture";

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
			/// Type : Picklist (IsolationMode)
			/// Validity :  Read | Create | Update 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Picklist)]
			[OptionSet(typeof(IsolationMode))]
			public const string IsolationMode = "isolationmode";

			/// <summary>
			/// 
			/// Type : Integer
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Integer)]
			[Range(0, 65534)]
			public const string Major = "major";

			/// <summary>
			/// 
			/// Type : Integer
			/// Validity :  Read 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Integer)]
			[Range(0, 65534)]
			public const string Minor = "minor";

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
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "pluginassemblyid";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(32)]
			public const string PublicKeyToken = "publickeytoken";

			/// <summary>
			/// 
			/// Type : String
			/// Validity :  Read | Create | Update | AdvancedFind 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.String)]
			[StringLength(48)]
			public const string Version = "version";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string createdby_pluginassembly = "createdby_pluginassembly";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_pluginassembly_createdonbehalfby = "lk_pluginassembly_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_pluginassembly_modifiedonbehalfby = "lk_pluginassembly_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string modifiedby_pluginassembly = "modifiedby_pluginassembly";
			[Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
			public const string organization_pluginassembly = "organization_pluginassembly";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship(PluginTypeDefinition.EntityName, EntityRole.Referenced, "pluginassembly_plugintype", PluginTypeDefinition.Columns.PluginAssemblyId)]
			public const string pluginassembly_plugintype = "pluginassembly_plugintype";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_pluginassembly", "objectid")]
			public const string userentityinstancedata_pluginassembly = "userentityinstancedata_pluginassembly";
		}
	}


    [OptionSetDefinition(PluginAssemblyDefinition.EntityName, PluginAssemblyDefinition.Columns.IsolationMode)]
    [DefinitionManagerIgnore]
	public enum IsolationMode
    {
        Null = 0,
        [Description("None")]
        None = 1,
        [Description("Sandbox")]
        Sandbox = 2,
        [Description("External")]
        External = 3,
    }
}
