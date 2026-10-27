// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Spectre.Console;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using XrmFramework.DeployUtils.Reporting;

namespace XrmFramework.DeployUtils.Tests.Reporting;

[TestFixture]
public class DeploymentChangeSummaryTests
{
    private static Step StepOn(string pluginTypeName, string entityName = "account") =>
        new(pluginTypeName, Messages.Update, Stages.PostOperation, Modes.Synchronous, entityName);

    private static T WithState<T>(T component, RegistrationState state) where T : ICrmComponent
    {
        component.RegistrationState = state;
        return component;
    }

    /// <summary>
    ///     A component the diff decided not to touch has no place in a summary of what is about to
    ///     be written: listing the whole pool would bury the handful of lines that matter.
    /// </summary>
    [Test]
    public void SelectChanges_KeepsOnlyTheStatesThatWrite()
    {
        var pool = new ICrmComponent[]
        {
            WithState(new Plugin("P.Created"), RegistrationState.ToCreate),
            WithState(new Plugin("P.Updated"), RegistrationState.ToUpdate),
            WithState(new Plugin("P.Deleted"), RegistrationState.ToDelete),
            WithState(new Plugin("P.Ignored"), RegistrationState.Ignore),
            WithState(new Plugin("P.NotComputed"), RegistrationState.NotComputed),
            WithState(new Plugin("P.Computed"), RegistrationState.Computed)
        };

        var changes = DeploymentChangeSummary.SelectChanges(pool);

        Assert.That(changes.OfType<Plugin>().Select(p => p.FullName),
            Is.EqualTo(new[] {"P.Created", "P.Updated", "P.Deleted"}));
    }

    /// <summary>
    ///     The pool is already ordered the way the deployment will apply it, so the summary has to
    ///     keep that order: it is read as the plan of what is going to happen, in sequence.
    /// </summary>
    [Test]
    public void SelectChanges_PreservesPoolOrder()
    {
        var pool = new ICrmComponent[]
        {
            WithState(new Plugin("P.Third"), RegistrationState.ToDelete),
            WithState(new Plugin("P.Ignored"), RegistrationState.Ignore),
            WithState(new Plugin("P.First"), RegistrationState.ToCreate),
            WithState(new Plugin("P.Second"), RegistrationState.ToUpdate)
        };

        var changes = DeploymentChangeSummary.SelectChanges(pool);

        Assert.That(changes.OfType<Plugin>().Select(p => p.FullName),
            Is.EqualTo(new[] {"P.Third", "P.First", "P.Second"}));
    }

    /// <summary>
    ///     The summary runs before anything is written, so it must not be the thing that breaks the
    ///     deployment: a missing pool or a hole in it yields no line rather than an exception.
    /// </summary>
    [Test]
    public void SelectChanges_ToleratesAbsentAndNullEntries()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DeploymentChangeSummary.SelectChanges(null), Is.Empty);
            Assert.That(DeploymentChangeSummary.SelectChanges(Array.Empty<ICrmComponent>()), Is.Empty);
            Assert.That(DeploymentChangeSummary.SelectChanges(new ICrmComponent[] {null}), Is.Empty);
        });
    }

    /// <summary>
    ///     Only the three writing states count as a change. Pinning them down matters because
    ///     <see cref="RegistrationState.Computed" /> is what an assembly carried by a package is set
    ///     to precisely so no batch writes it - it must not be announced as a change.
    /// </summary>
    [TestCase(RegistrationState.ToCreate, true)]
    [TestCase(RegistrationState.ToUpdate, true)]
    [TestCase(RegistrationState.ToDelete, true)]
    [TestCase(RegistrationState.Ignore, false)]
    [TestCase(RegistrationState.NotComputed, false)]
    [TestCase(RegistrationState.Computed, false)]
    public void IsChange_IsTrueOnlyForWritingStates(RegistrationState state, bool expected)
    {
        Assert.That(DeploymentChangeSummary.IsChange(state), Is.EqualTo(expected));
    }

    /// <summary>
    ///     The verb carries the whole meaning of a line, so each state maps to one that cannot be
    ///     mistaken for another - a wrong verb here reads as the deployment doing the opposite.
    /// </summary>
    [TestCase(RegistrationState.ToCreate, "Add", "green")]
    [TestCase(RegistrationState.ToUpdate, "Update", "yellow")]
    [TestCase(RegistrationState.ToDelete, "Remove", "red")]
    public void DescribeState_MapsEachWritingStateToItsVerb(RegistrationState state, string verb, string color)
    {
        Assert.That(DeploymentChangeSummary.DescribeState(state), Is.EqualTo((verb, color)));
    }

    /// <summary>
    ///     An assembly is announced with its version: that is the one piece of information telling
    ///     whether the deployment is pushing the build that was just made.
    /// </summary>
    [Test]
    public void Describe_AssemblyCarriesItsVersion()
    {
        var assembly = new AssemblyInfo {Name = "MySolution.Plugins", Version = "1.0.0.2"};

        Assert.That(DeploymentChangeSummary.Describe(assembly),
            Is.EqualTo("assembly 'MySolution.Plugins' (version 1.0.0.2)"));
    }

    /// <summary>
    ///     A package is named as such rather than as an assembly: in package mode the two coexist,
    ///     and the line has to say which of them is being written.
    /// </summary>
    [Test]
    public void Describe_PackageIsDistinguishedFromAnAssembly()
    {
        var package = new PluginPackage {Name = "MySolution.Plugins", Version = "1.0.0.2"};

        Assert.That(DeploymentChangeSummary.Describe(package),
            Is.EqualTo("NuGet package 'MySolution.Plugins' (version 1.0.0.2)"));
    }

    /// <summary>
    ///     A workflow activity is a plugin type in the model but a different thing to whoever reads
    ///     the summary, and it is told apart by carrying a display name.
    /// </summary>
    [Test]
    public void Describe_WorkflowActivityIsNotCalledAPlugin()
    {
        var workflow = new Plugin("MySolution.Workflows.ComputeScore", "Compute score");

        Assume.That(workflow.IsWorkflow, Is.True);

        Assert.That(DeploymentChangeSummary.Describe(workflow),
            Is.EqualTo("workflow activity 'Compute score'"));
    }

    /// <summary>
    ///     A plain plugin is identified by its full type name, which is what the environment shows
    ///     and what a developer searches the solution for.
    /// </summary>
    [Test]
    public void Describe_PluginUsesItsFullTypeName()
    {
        var plugin = new Plugin("MySolution.Plugins.AccountPlugin");

        Assume.That(plugin.IsWorkflow, Is.False);

        Assert.That(DeploymentChangeSummary.Describe(plugin),
            Is.EqualTo("plugin 'MySolution.Plugins.AccountPlugin'"));
    }

    /// <summary>
    ///     A step is the component whose change is hardest to guess from a type name, so its line
    ///     reuses the description the model already builds: stage, message and table.
    /// </summary>
    [Test]
    public void Describe_StepReusesItsBusinessDescription()
    {
        var step = StepOn("OnAccountUpdate");

        Assert.That(DeploymentChangeSummary.Describe(step), Is.EqualTo($"step '{step.Description}'"));
        Assert.That(DeploymentChangeSummary.Describe(step),
            Does.Contain("PostOperation").And.Contain("Update").And.Contain("account"));
    }

    /// <summary>
    ///     An image is meaningless on its own: the line names the step it belongs to, and whether it
    ///     is the pre or the post one, since the two are otherwise indistinguishable.
    /// </summary>
    [Test]
    public void Describe_ImageNamesItsSideAndItsParentStep()
    {
        var step = StepOn("OnAccountUpdate");

        Assert.Multiple(() =>
        {
            Assert.That(DeploymentChangeSummary.Describe(step.PreImage),
                Is.EqualTo($"pre-image of step '{step.Description}'"));
            Assert.That(DeploymentChangeSummary.Describe(step.PostImage),
                Is.EqualTo($"post-image of step '{step.Description}'"));
        });
    }

    /// <summary>
    ///     An image detached from its step still has to produce a line. The summary is the last
    ///     thing to run before the environment is touched, and a null dereference here would abort
    ///     a deployment that was otherwise fine.
    /// </summary>
    [Test]
    public void Describe_DetachedImageDoesNotThrow()
    {
        var orphan = new StepImage(Messages.Create, true, Stages.PreOperation) {FatherStep = null};

        Assert.That(() => DeploymentChangeSummary.Describe(orphan), Throws.Nothing);
        Assert.That(DeploymentChangeSummary.Describe(orphan), Does.StartWith("pre-image of step"));
    }

    /// <summary>
    ///     Custom APIs and their parameters are named the way they are addressed over the Web API,
    ///     which is how they are looked up when a caller breaks.
    /// </summary>
    [Test]
    public void Describe_CustomApiAndItsMembers()
    {
        var api = new CustomApi {Name = "dim_ApproveOpportunity"};
        var parameter = new CustomApiRequestParameter {UniqueName = "OpportunityId"};
        var response = new CustomApiResponseProperty {UniqueName = "Approved"};

        Assert.Multiple(() =>
        {
            Assert.That(DeploymentChangeSummary.Describe(api), Is.EqualTo("Custom API 'dim_ApproveOpportunity'"));
            Assert.That(DeploymentChangeSummary.Describe(parameter),
                Is.EqualTo("Custom API parameter 'OpportunityId'"));
            Assert.That(DeploymentChangeSummary.Describe(response),
                Is.EqualTo("Custom API response property 'Approved'"));
        });
    }

    /// <summary>
    ///     A component type the summary does not know about still gets a line, so adding a component
    ///     to the model can never make a deployment silently skip it in the summary.
    /// </summary>
    [Test]
    public void Describe_UnknownComponentFallsBackToItsTypeAndUniqueName()
    {
        var unknown = new UnknownComponent {UniqueName = "something_new"};

        Assert.That(DeploymentChangeSummary.Describe(unknown),
            Is.EqualTo("UnknownComponent 'something_new'"));
    }

    /// <summary>
    ///     Defensive: <see cref="DeploymentChangeSummary.Describe" /> is public and total.
    /// </summary>
    [Test]
    public void Describe_NullYieldsALineRatherThanThrowing()
    {
        Assert.That(DeploymentChangeSummary.Describe(null), Is.EqualTo("(unknown component)"));
    }

    /// <summary>
    ///     An empty diff has to say so. Printing nothing leaves a deployment that changed nothing
    ///     looking exactly like one that worked, which is the ambiguity the summary exists to remove.
    /// </summary>
    [Test]
    public void BuildLines_EmptyPoolSaysNothingToDeploy()
    {
        Assert.That(DeploymentChangeSummary.BuildLines(Array.Empty<ICrmComponent>()),
            Is.EqualTo(new[] {DeploymentChangeSummary.NoChangesLine}));
    }

    /// <summary>
    ///     The realistic no-op: the pool is full, but the diff flagged everything as already in
    ///     place. That is still "nothing to deploy", not an empty list of changes.
    /// </summary>
    [Test]
    public void BuildLines_PoolWithNoWritingStateSaysNothingToDeploy()
    {
        var pool = new ICrmComponent[]
        {
            WithState(new Plugin("P.A"), RegistrationState.Ignore),
            WithState(new Plugin("P.B"), RegistrationState.Computed)
        };

        Assert.That(DeploymentChangeSummary.BuildLines(pool),
            Is.EqualTo(new[] {DeploymentChangeSummary.NoChangesLine}));
    }

    /// <summary>
    ///     The shape of the summary: a header, then exactly one line per change, each carrying the
    ///     verb and the component description, in the order they will be applied.
    /// </summary>
    [Test]
    public void BuildLines_HeaderThenOneLinePerChangeInOrder()
    {
        var pool = new ICrmComponent[]
        {
            WithState(new Plugin("MySolution.Plugins.AccountPlugin"), RegistrationState.ToCreate),
            WithState(new Plugin("MySolution.Plugins.Ignored"), RegistrationState.Ignore),
            WithState(new Plugin("MySolution.Plugins.ContactPlugin"), RegistrationState.ToDelete)
        };

        var lines = DeploymentChangeSummary.BuildLines(pool);

        Assert.Multiple(() =>
        {
            Assert.That(lines, Has.Count.EqualTo(3));
            Assert.That(lines[0], Is.EqualTo(DeploymentChangeSummary.HeaderLine));
            Assert.That(lines[1], Is.EqualTo("  [green]Add[/] plugin 'MySolution.Plugins.AccountPlugin'"));
            Assert.That(lines[2], Is.EqualTo("  [red]Remove[/] plugin 'MySolution.Plugins.ContactPlugin'"));
            Assert.That(lines, Has.None.Contains("Ignored"));
        });
    }

    /// <summary>
    ///     Component names come from whoever wrote the plugin, and a square bracket in one would be
    ///     read as Spectre markup - throwing mid-deployment, right between the diff and the writes.
    ///     The description is escaped, while the verb's own markup stays live.
    /// </summary>
    [Test]
    public void BuildLines_EscapesMarkupComingFromComponentNames()
    {
        var hostile = WithState(new Plugin("MySolution.Plugins.On[Account]Update"), RegistrationState.ToUpdate);

        var line = DeploymentChangeSummary.BuildLines(new ICrmComponent[] {hostile}).Last();

        Assert.Multiple(() =>
        {
            Assert.That(line, Does.Contain("On[[Account]]Update"), "brackets should be escaped");
            Assert.That(line, Does.StartWith("  [yellow]Update[/] "), "the verb's markup stays live");
            // Markup.Remove parses the line the way AnsiConsole.MarkupLine will: it throws on
            // malformed markup, so a clean round-trip is the proof the line is printable.
            Assert.That(Markup.Remove(line), Is.EqualTo("  Update plugin 'MySolution.Plugins.On[Account]Update'"));
        });
    }

    /// <summary>
    ///     Every line the summary produces must survive the call production actually makes,
    ///     <see cref="AnsiConsole.MarkupLine(string)" /> - this is the invariant that keeps the
    ///     summary from being the thing that fails a deployment.
    /// </summary>
    /// <remarks>
    ///     Rendered rather than merely stripped: <see cref="Markup.Remove" /> drops tags without
    ///     resolving them, so it accepts a name like <c>[Account]</c> that the renderer rejects as
    ///     an unknown style. Only writing the line proves it is printable.
    /// </remarks>
    [Test]
    public void BuildLines_EveryLineSurvivesBeingPrinted()
    {
        var step = StepOn("On[Account]Update");
        var pool = new ICrmComponent[]
        {
            WithState(new AssemblyInfo {Name = "A", Version = "1.0.0.0"}, RegistrationState.ToUpdate),
            WithState(new PluginPackage {Name = "A", Version = "1.0.0.0"}, RegistrationState.ToCreate),
            WithState(new Plugin("P.A", "Display [name]"), RegistrationState.ToCreate),
            WithState(step, RegistrationState.ToUpdate),
            WithState(step.PreImage, RegistrationState.ToDelete),
            WithState(new CustomApi {Name = "dim_Api"}, RegistrationState.ToCreate),
            WithState(new UnknownComponent {UniqueName = "x[y]"}, RegistrationState.ToDelete)
        };

        var lines = DeploymentChangeSummary.BuildLines(pool);

        Assert.That(lines, Has.Count.EqualTo(pool.Length + 1));

        var writer = new StringWriter();
        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Out = new AnsiConsoleOutput(writer)
        });

        Assert.That(() =>
        {
            foreach (var line in lines) console.MarkupLine(line);
        }, Throws.Nothing);

        Assert.That(writer.ToString(), Does.Contain("On[Account]Update"),
            "the component's real name should reach the output, brackets included");
    }

    /// <summary>A component type the summary has no special case for.</summary>
    private sealed class UnknownComponent : BaseCrmComponent
    {
        public override string UniqueName { get; set; }
        public override int Rank => 0;
        public override bool DoAddToSolution => false;
        public override bool DoFetchTypeCode => false;
        public override string EntityTypeName => "unknown";
        public override IEnumerable<ICrmComponent> Children => Enumerable.Empty<ICrmComponent>();
        protected override void RemoveChild(ICrmComponent child) { }
    }
}
