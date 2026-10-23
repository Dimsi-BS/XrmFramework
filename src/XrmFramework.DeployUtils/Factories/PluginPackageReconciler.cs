// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Factories;

/// <summary>
///     Decides what has to happen around a plugin package push, where the usual
///     "diff then write" order does not hold.
/// </summary>
public static class PluginPackageReconciler
{
	/// <summary>
	///     Steps and images registered against a plugin type the new package no longer carries.
	///     <para>
	///         These have to be unregistered BEFORE the package is pushed: Dataverse rejects an
	///         update that would leave a step pointing at a type the package has dropped, so the
	///         deployment would fail halfway with the old package still in place.
	///     </para>
	///     <para>
	///         Images come along with their step - deleting a step without its images leaves rows
	///         behind that the next diff would have to clean up.
	///     </para>
	/// </summary>
	/// <param name="registeredPlugins">Plugin types currently registered for the assembly.</param>
	/// <param name="localPluginTypeNames">Full type names the new package carries.</param>
	/// <returns>The components to delete, deepest first.</returns>
	public static IReadOnlyList<ICrmComponent> FindStepsOfRemovedPluginTypes(
		IEnumerable<Plugin> registeredPlugins,
		IEnumerable<string> localPluginTypeNames)
	{
		if (registeredPlugins == null) return Array.Empty<ICrmComponent>();

		var keptTypeNames = new HashSet<string>(
			localPluginTypeNames ?? Enumerable.Empty<string>(),
			StringComparer.Ordinal);

		var orphaned = new List<ICrmComponent>();

		foreach (var plugin in registeredPlugins.Where(p => p != null && !keptTypeNames.Contains(p.FullName)))
		{
			foreach (var step in plugin.Steps)
			{
				// Children (the images) before the step itself: DeleteAllComponents orders by
				// descending rank, but being explicit here keeps the intent readable.
				orphaned.AddRange(step.Children);
				orphaned.Add(step);
			}
		}

		return orphaned;
	}
}
