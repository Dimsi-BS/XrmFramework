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
    ///     Introduces the line a deployment ends on once it has run through without an error, so a
    ///     successful run says so instead of just stopping.
    /// </summary>
    public const string CompletedLineHeader = "Deployment completed";

    /// <summary>
    ///     Said in place of the counts when a completed deployment had nothing to write - the diff
    ///     came out empty, and the environment already matched the assembly.
    /// </summary>
    public const string NothingAppliedText = "nothing to change";

    /// <summary>The past tense a registration state reads as once it has been applied.</summary>
    public static string DescribeAppliedState(RegistrationState state) => state switch
    {
        RegistrationState.ToCreate => "added",
        RegistrationState.ToUpdate => "updated",
        RegistrationState.ToDelete => "removed",
        _ => "unchanged"
    };

    /// <summary>
    ///     What a deployment ended up writing, counted per state, e.g. <c>3 added, 1 updated</c>.
    ///     A state nothing happened in is left out rather than printed as a zero.
    /// </summary>
    /// <param name="componentsPool">
    ///     The components the deployment applied - the strategy pool, or the already filtered
    ///     changes: both read the same, since only the writing states are counted.
    /// </param>
    public static string SummarizeApplied(IEnumerable<ICrmComponent> componentsPool)
    {
        var changes = SelectChanges(componentsPool);

        if (changes.Count == 0) return NothingAppliedText;

        var counted = new[] {RegistrationState.ToCreate, RegistrationState.ToUpdate, RegistrationState.ToDelete}
            .Select(state => new {State = state, Count = changes.Count(c => c.RegistrationState == state)})
            .Where(x => x.Count > 0)
            .Select(x => $"{x.Count} {DescribeAppliedState(x.State)}");

        return string.Join(", ", counted);
    }

    /// <summary>
    ///     The lines closing a successful deployment: what it wrote, and - when known - which
    ///     assembly went to which environment, because the console has scrolled past the header by
    ///     then and that pair is what makes the run identifiable afterwards.
    /// </summary>
    /// <param name="componentsPool">The components the deployment applied.</param>
    /// <param name="assemblyName">Name of the deployed assembly. Omitted when <see langword="null" /> or blank.</param>
    /// <param name="targetUrl">Url of the target environment. Omitted when <see langword="null" /> or blank.</param>
    public static IReadOnlyList<string> BuildCompletionLines(
        IEnumerable<ICrmComponent> componentsPool,
        string assemblyName = null,
        string targetUrl = null)
    {
        var lines = new List<string>(2)
        {
            $"[green]{CompletedLineHeader}[/]: {Markup.Escape(SummarizeApplied(componentsPool))}."
        };

        var target = DescribeTarget(assemblyName, targetUrl);

        if (target != null) lines.Add($"  [grey]{Markup.Escape(target)}[/]");

        return lines;
    }

    /// <summary>
    ///     Where the deployment went, from whichever of the two halves is known. Nothing known
    ///     yields <see langword="null" />, so the caller prints no second line at all rather than
    ///     an empty one.
    /// </summary>
    private static string DescribeTarget(string assemblyName, string targetUrl)
    {
        var hasAssembly = !string.IsNullOrWhiteSpace(assemblyName);
        var hasUrl = !string.IsNullOrWhiteSpace(targetUrl);

        if (hasAssembly && hasUrl) return $"{assemblyName} -> {targetUrl}";
        if (hasAssembly) return assemblyName;

        return hasUrl ? targetUrl : null;
    }

    /// <summary>
    ///     The components a pre-push phase is about to delete outright, in the order given.
    /// </summary>
    public static IReadOnlyList<ICrmComponent> SelectRemovals(IEnumerable<ICrmComponent> components)
    {
        if (components == null) return Array.Empty<ICrmComponent>();

        return components.Where(c => c != null).ToList();
    }

    /// <summary>
    ///     Whether the operator has to approve these removals before they are applied.
    /// </summary>
    /// <remarks>
    ///     Two conditions, and both matter. Something has to actually be removed - a deployment
    ///     that only adds and updates must not grow a second prompt. And the run has to be
    ///     interactive: under <c>--noprompt</c>, which exists for CI, there is nobody to answer and
    ///     a prompt would hang the agent instead of deploying.
    /// </remarks>
    /// <param name="components">The components about to be deleted.</param>
    /// <param name="noPrompt">The deployment's silent-mode switch.</param>
    public static bool RequiresRemovalConfirmation(IEnumerable<ICrmComponent> components, bool noPrompt)
        => !noPrompt && SelectRemovals(components).Count > 0;

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
        var removals = SelectRemovals(components);

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
