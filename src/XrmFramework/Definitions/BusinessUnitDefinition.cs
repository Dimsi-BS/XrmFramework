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
	}
}
