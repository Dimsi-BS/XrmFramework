using XrmFramework.DeployUtils.Context;

namespace XrmFramework.DeployUtils.Model.Interfaces;

/// <summary>
///     A Component of an Assembly to be deployed on the Crm,
///     pretty much the local machine equivalent of an <see cref="Microsoft.Xrm.Sdk.Entity" />
/// </summary>
public partial interface ICrmComponent : IDeployContext
{
	/// <summary>The unique name of the component</summary>
	string UniqueName { get; }

	/// <summary>The rank of the component, gives an idea of how many layers of other components this one depends on</summary>
	int Rank { get; }

	/// <summary>
	///     Indicates whether or not this components requires an
	///     <see cref="Microsoft.Crm.Sdk.Messages.AddSolutionComponentRequest" />
	///     during export
	/// </summary>
	bool DoAddToSolution { get; }

	/// <summary>Indicates whether or not this components requires an EntityTypeCode during export</summary>
	bool DoFetchTypeCode { get; }

	/// <summary>Stores a custom state used for computing difference between Assemblies and for Deploy</summary>
	RegistrationState RegistrationState { get; set; }

	/// <summary>The Id of the component</summary>
	/// <remarks>When set, also sets its children <see cref="ParentId" /> to the value</remarks>
	Guid Id { get; set; }

	/// <summary>The Id of the component this one depends on</summary>
	Guid ParentId { get; set; }

	/// <summary>The Logical Name this component is known as on the Crm</summary>
	string EntityTypeName { get; }
}

public enum RegistrationState
{
	ToCreate,
	ToUpdate,
	ToDelete,
	Ignore,
	NotComputed,
	Computed
}