// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Reporting;

/// <summary>
///     Turns a computed registration strategy into the lines a deployment prints before it writes
///     anything, so the changes it is about to make are never silent.
/// </summary>
/// <remarks>
///     Kept apart from the printing itself, and free of any console dependency, so the wording and
///     the selection of what counts as a change can be asserted in tests rather than eyeballed on a
///     real deployment.
/// </remarks>
public static class DeploymentChangeSummary
{
    /// <summary>
    ///     Shown when the diff comes out empty - a state otherwise indistinguishable, on the
    ///     console, from a deployment that did work.
    /// </summary>
    public const string NoChangesLine = "[grey]No changes detected: nothing to deploy.[/]";

    /// <summary>Introduces the list of changes.</summary>
    public const string HeaderLine = "[bold]Changes to apply:[/]";

    /// <summary>
    ///     Introduces what a plugin package push has to unregister on its way in.
    /// </summary>
    public const string PackageRemovalHeaderLine = "[bold]To unregister before pushing the package:[/]";

    /// <summary>
    ///     The components a deployment is about to write, in the pool's own order - which is the
    ///     order they will be applied in, so the summary reads like the plan it describes.
    /// </summary>
    /// <param name="componentsPool">The full component pool, already flagged with a <see cref="RegistrationState" />.</param>
    public static IReadOnlyList<ICrmComponent> SelectChanges(IEnumerable<ICrmComponent> componentsPool)
    {
        if (componentsPool == null) return Array.Empty<ICrmComponent>();

        return componentsPool
            .Where(c => c != null && IsChange(c.RegistrationState))
            .ToList();
    }

    /// <summary>
    ///     Whether a state means the deployment is going to write something. Everything else
    ///     (<see cref="RegistrationState.Ignore" />, <see cref="RegistrationState.Computed" />,
    ///     <see cref="RegistrationState.NotComputed" />) is deliberately left out: listing it would
    ///     drown the handful of lines that matter.
    /// </summary>
    public static bool IsChange(RegistrationState state)
        => state is RegistrationState.ToCreate or RegistrationState.ToUpdate or RegistrationState.ToDelete;

    /// <summary>The verb and colour a registration state is rendered with.</summary>
    public static (string Verb, string Color) DescribeState(RegistrationState state) => state switch
    {
        RegistrationState.ToCreate => ("Add", "green"),
        RegistrationState.ToUpdate => ("Update", "yellow"),
        RegistrationState.ToDelete => ("Remove", "red"),
        _ => ("?", "grey")
    };

    /// <summary>
    ///     A short description of a component in business terms rather than by type name, e.g.
    ///     <c>plugin 'MySolution.Plugins.AccountPlugin'</c> or
    ///     <c>assembly 'MySolution.Plugins' (version 1.0.0.2)</c>.
    /// </summary>
    /// <remarks>
    ///     Total by construction: an unrecognised or detached component still yields a line, since a
    ///     summary that throws would take the whole deployment down before it wrote anything.
    /// </remarks>
    public static string Describe(ICrmComponent component) => component switch
    {
        null => "(unknown component)",
        AssemblyInfo assembly => $"assembly '{assembly.Name}' (version {assembly.Version})",
        PluginPackage package => $"NuGet package '{package.Name}' (version {package.Version})",
        Plugin {IsWorkflow: true} workflow => $"workflow activity '{workflow.DisplayName}'",
        Plugin plugin => $"plugin '{plugin.FullName}'",
        Step step => $"step '{step.Description}'",
        StepImage image => $"{(image.IsPreImage ? "pre" : "post")}-image of step '{image.FatherStep?.Description}'",
        CustomApi api => $"Custom API '{api.Name}'",
        CustomApiRequestParameter param => $"Custom API parameter '{param.UniqueName}'",
        CustomApiResponseProperty prop => $"Custom API response property '{prop.UniqueName}'",
        _ => $"{component.GetType().Name} '{component.UniqueName}'"
    };

    /// <summary>
    ///     The complete summary, as Spectre markup lines ready to be printed in order.
    /// </summary>
    /// <remarks>
    ///     Component descriptions are escaped: they carry names chosen by whoever wrote the plugin,
    ///     and a single square bracket in a step description would otherwise be read as markup and
    ///     throw in the middle of the deployment.
    /// </remarks>
    public static IReadOnlyList<string> BuildLines(IEnumerable<ICrmComponent> componentsPool)
    {
        var changes = SelectChanges(componentsPool);

        if (changes.Count == 0) return new[] {NoChangesLine};

        var lines = new List<string>(changes.Count + 1) {HeaderLine};

        foreach (var component in changes)
        {
            var (verb, color) = DescribeState(component.RegistrationState);
            lines.Add($"  [{color}]{verb}[/] {Markup.Escape(Describe(component))}");
        }

        return lines;
    }

    /// <summary>
    ///     The lines announcing components deleted outright, ahead of any diff - what a plugin
    ///     package push has to unregister before the platform will accept it.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The state is deliberately ignored. These components come from the remote inventory
    ///         and are handed straight to the exporter rather than through the strategy, so they
    ///         still carry <see cref="RegistrationState.NotComputed" />; every one of them is about
    ///         to be deleted, so every line reads as a removal.
    ///     </para>
    ///     <para>
    ///         Nothing to remove yields no line at all, header included: that phase does not run,
    ///         and announcing it would suggest a write that never happens.
    ///     </para>
    /// </remarks>
    public static IReadOnlyList<string> BuildRemovalLines(IEnumerable<ICrmComponent> components)
    {
        if (components == null) return Array.Empty<string>();

        var removals = components.Where(c => c != null).ToList();

        if (removals.Count == 0) return Array.Empty<string>();

        var (verb, color) = DescribeState(RegistrationState.ToDelete);

        var lines = new List<string>(removals.Count + 1) {PackageRemovalHeaderLine};

        foreach (var component in removals)
        {
            lines.Add($"  [{color}]{verb}[/] {Markup.Escape(Describe(component))}");
        }

        return lines;
    }
}
