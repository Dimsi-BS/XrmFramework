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
	public AssemblyInfo AssemblyInfo { get; set; } = new();

	public IReadOnlyCollection<ICrmComponent> ComponentsOrderedPool
	{
		get
		{
			List<ICrmComponent> pool = new();

			CreateSolutionComponentPoolRecursive(pool, this);

			pool.Sort((x, y) => x.Rank.CompareTo(y.Rank));
			return pool;
		}
	}

	public bool NeedsRegister()
	{
		return AssemblyInfo.RegistrationState is RegistrationState.ToCreate
		       || AssemblyInfo.Package?.RegistrationState is RegistrationState.ToCreate;
	}

	public Guid Id
	{
		get => AssemblyInfo.Id;
		set
		{
			foreach (var child in Plugins) child.ParentId = value;
			foreach (var child in Workflows) child.ParentId = value;
			foreach (var child in CustomApis) child.AssemblyId = value;

			AssemblyInfo.Id = value;
		}
	}

	public IEnumerable<ICrmComponent> Children
	{
		get
		{
			var children = new List<ICrmComponent>();
			children.AddRange(AssemblyInfo.Children);
			children.AddRange(Plugins);
			children.AddRange(Workflows);
			children.AddRange(CustomApis);
			return children;
		}
	}

	public void AddChild(ICrmComponent child)
	{
		switch (child)
		{
			case Plugin plugin:
				plugin.ParentId = Id;
				if (plugin.IsWorkflow) _workflows.Add(plugin);
				else _plugins.Add(plugin);
				break;
			case CustomApi api:
				api.AssemblyId = Id;
				_customApis.Add(api);
				break;
			case PluginPackage package:
				package.ParentId = Id;
				AssemblyInfo.AddChild(package);
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
			case PluginPackage package:
				AssemblyInfo.Package = null;
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

	#region AssemblyInfo ProxyProperties

	public string UniqueName => AssemblyInfo.UniqueName;
	public int Rank => AssemblyInfo.Rank;
	public bool DoAddToSolution => AssemblyInfo.DoAddToSolution;
	public bool DoFetchTypeCode => AssemblyInfo.DoFetchTypeCode;

	public RegistrationState RegistrationState
	{
		get => AssemblyInfo.RegistrationState;
		set => AssemblyInfo.RegistrationState = value;
	}

	public Guid ParentId { get; set; }
	public string EntityTypeName => AssemblyInfo.EntityTypeName;

	#endregion
}