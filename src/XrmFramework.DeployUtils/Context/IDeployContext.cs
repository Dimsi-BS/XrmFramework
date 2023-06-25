using System;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context;

public interface IDeployContext
{
	/// <summary>Enumeration of the <see cref="ICrmComponent" /> that depend on this component</summary>
	IEnumerable<ICrmComponent> Children { get; }

	/// <summary>Adds a child to this component</summary>
	/// <remarks>
	///     Sets the child ParentId in the process<br />
	///     Throws <see cref="ArgumentException" /> if this component doesn't accept the given <see cref="ICrmComponent" />
	/// </remarks>
	/// <param name="child"></param>
	/// <exception cref="ArgumentException" />
	void AddChild(ICrmComponent child);

	/// <summary>
	///     Removes recursively all children with a given <see cref="RegistrationState" /><br />
	///     A child and all its branch is kept if it or one member of its branch have a different
	///     <see cref="RegistrationState" />
	/// </summary>
	/// <param name="state"></param>
	void CleanChildrenWithState(RegistrationState state);
}