using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context;

/// <summary>
///     Implementation of <see cref="IAssemblyContext" />
/// </summary>
public class AssemblyContext : IAssemblyContext
{
	public IEnumerable<ICrmComponent> Children
	{
		get
		{
			var children = new List<ICrmComponent>();
			children.AddRange(Plugins);
			children.AddRange(Workflows);
			children.AddRange(CustomApis);
			return children;
		}
	}

	public AssemblyInfo AssemblyInfo { get; set; } = new();

	public IReadOnlyCollection<ICrmComponent> ComponentsOrderedPool
	{
		get
		{
			List<ICrmComponent> pool = new();

			foreach (var crmComponent in Children) CreateSolutionComponentPoolRecursive(pool, crmComponent);

			pool.Sort((x, y) => x.Rank.CompareTo(y.Rank));
			return pool;
		}
	}

	public void SetAssemblyId(Guid id)
	{
		AssemblyInfo.Id = id;
		foreach (var child in Children)
		{
			child.ParentId = id;
		}
	}

	public void AddChild(ICrmComponent child)
	{
		switch (child)
		{
			case Plugin plugin:
				plugin.ParentId = AssemblyInfo.Id;
				if (plugin.IsWorkflow) _workflows.Add(plugin);
				else _plugins.Add(plugin);
				break;
			case CustomApi api:
				api.AssemblyId = AssemblyInfo.Id;
				_customApis.Add(api);
				break;
			default:
				throw new ArgumentException("AssemblyContext doesn't take this type of children");
		}
	}

	public void CleanChildrenWithState(RegistrationState state)
	{
		var childrenWithStateSafe = Children
			.Where(c => c.RegistrationState == state)
			.ToList();
		foreach (var child in childrenWithStateSafe)

		{
			child.CleanChildrenWithState(state);
			if (!child.Children.Any()) RemoveChild(child);
		}
	}

	/// <summary>
	///     Recursively add to the <paramref name="terminalStack" /> the <paramref name="component" /> and its <c>Children</c>
	/// </summary>
	/// <param name="terminalStack">The pool that will eventually contain every <c>Component</c> of the <c>Assembly</c></param>
	/// <param name="component">The current <c>Component</c></param>
	private static void CreateSolutionComponentPoolRecursive(ICollection<ICrmComponent> terminalStack,
		ICrmComponent component)
	{
		terminalStack.Add(component);
		foreach (var child in component.Children) CreateSolutionComponentPoolRecursive(terminalStack, child);
	}

	private void RemoveChild(ICrmComponent child)
	{
		switch (child)
		{
			case Plugin plugin:
				if (plugin.IsWorkflow)
					_workflows.Remove(plugin);
				else _plugins.Remove(plugin);
				break;
			case CustomApi api:
				_customApis.Remove(api);
				break;
			default:
				throw new ArgumentException("AssemblyContext doesn't have this type of children");
		}
	}

	#region Private IAssemblyContext fields implementation

	private readonly List<Plugin> _plugins = new();
	private readonly List<CustomApi> _customApis = new();
	private readonly List<Plugin> _workflows = new();

	#endregion

	#region Public IAssemblyContext Properties get implementation

	public ICollection<Plugin> Plugins => _plugins;
	public ICollection<CustomApi> CustomApis => _customApis;
	public ICollection<Plugin> Workflows => _workflows;

	#endregion
}