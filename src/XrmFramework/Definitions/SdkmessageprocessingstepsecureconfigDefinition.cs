using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Definitions
{
	[GeneratedCode("XrmFramework", "1.0")]
	[EntityDefinition]

	[ExcludeFromCodeCoverage]
    [DefinitionManagerIgnore]

    public static class SdkMessageProcessingStepSecureConfigDefinition
	{
		public const string EntityName = "sdkmessageprocessingstepsecureconfig";
		public const string EntityCollectionName = "sdkmessageprocessingstepsecureconfigs";

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class Columns
		{
			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create 
			/// </summary>
			[AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
			[PrimaryAttribute(PrimaryAttributeType.Id)]
			public const string Id = "sdkmessageprocessingstepsecureconfigid";

		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class ManyToOneRelationships
		{
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
			public const string createdby_sdkmessageprocessingstepsecureconfig = "createdby_sdkmessageprocessingstepsecureconfig";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
			public const string lk_sdkmessageprocessingstepsecureconfig_createdonbehalfby = "lk_sdkmessageprocessingstepsecureconfig_createdonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
			public const string lk_sdkmessageprocessingstepsecureconfig_modifiedonbehalfby = "lk_sdkmessageprocessingstepsecureconfig_modifiedonbehalfby";
			[Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
			public const string modifiedby_sdkmessageprocessingstepsecureconfig = "modifiedby_sdkmessageprocessingstepsecureconfig";
			[Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
			public const string organization_sdkmessageprocessingstepsecureconfig = "organization_sdkmessageprocessingstepsecureconfig";
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public static class OneToManyRelationships
		{
			[Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "sdkmessageprocessingstepsecureconfigid_sdkmessageprocessingstep", SdkMessageProcessingStepDefinition.Columns.SdkMessageProcessingStepSecureConfigId)]
			public const string sdkmessageprocessingstepsecureconfigid_sdkmessageprocessingstep = "sdkmessageprocessingstepsecureconfigid_sdkmessageprocessingstep";
			[Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_sdkmessageprocessingstepsecureconfig", "objectid")]
			public const string userentityinstancedata_sdkmessageprocessingstepsecureconfig = "userentityinstancedata_sdkmessageprocessingstepsecureconfig";
		}
	}
}
