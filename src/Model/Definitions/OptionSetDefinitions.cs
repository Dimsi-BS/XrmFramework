using System.ComponentModel;

namespace Model
{

	[OptionSetDefinition(AccountDefinition.EntityName, AccountDefinition.Columns.Address1_FreightTermsCode)]
	public enum Address1FreightTerms
	{
		Null = 0,
		[Description("FOB")]
		FOB = 1,
		[Description("No Charge")]
		NoCharge = 2,
	}

	[OptionSetDefinition(AccountDefinition.EntityName, AccountDefinition.Columns.CustomerTypeCode)]
	public enum RelationshipType
	{
		Null = 0,
		[Description("Competitor")]
		Competitor = 1,
		[Description("Supplier")]
		Supplier = 10,
		[Description("Vendor")]
		Vendor = 11,
		[Description("Other")]
		Other = 12,
		[Description("Consultant")]
		Consultant = 2,
		[Description("Customer")]
		Customer = 3,
		[Description("Investor")]
		Investor = 4,
		[Description("Partner")]
		Partner = 5,
		[Description("Influencer")]
		Influencer = 6,
		[Description("Press")]
		Press = 7,
		[Description("Prospect")]
		Prospect = 8,
		[Description("Reseller")]
		Reseller = 9,
	}

	[OptionSetDefinition(AccountDefinition.EntityName, AccountDefinition.Columns.StateCode)]
	public enum AccountState
	{
		[Description("Active")]
		Actif = 0,
		[Description("Inactive")]
		Inactif = 1,
	}

	[OptionSetDefinition(ContactDefinition.EntityName, ContactDefinition.Columns.StateCode)]
	public enum ContactState
	{
		[Description("Active")]
		Actif = 0,
		[Description("Inactive")]
		Inactif = 1,
	}
}
