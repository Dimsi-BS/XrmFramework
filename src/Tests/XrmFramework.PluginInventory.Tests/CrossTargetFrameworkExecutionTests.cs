// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     EXECUTES a real net462/net48 plugin on this runtime, in process, the way a .NET-modern remote
///     debugger host would have to.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="CrossTargetFrameworkInventoryTests" /> proves a .NET Framework plugin assembly
///         can be <em>described</em> away from .NET Framework. These tests prove the harder claim the
///         remote debugger rests on: that it can be <em>run</em> - so that debugging a plugin from
///         macOS or Linux needs no retargeting of the plugin project, and the binary under the
///         debugger stays the one that gets deployed.
///     </para>
///     <para>
///         Why these live in the inventory's test project rather than the debugger's: the two things
///         this needs are already laid out here by <c>CopyInventoryHostAndFixtures</c> - real
///         net462/net48 plugin assemblies with their whole output directory, and the published
///         <c>System.Activities</c> stand-in. <c>XrmFramework.RemoteDebugger.Client</c> is, for now,
///         net462;net48 only, so it cannot host a test on this runtime at all.
///     </para>
///     <para>
///         Note what is NOT stubbed: the plugin runs the genuine XrmFramework pipeline - step
///         matching, tracing, the remote-debug session lookup, method dispatch. Only Dataverse itself
///         is replaced, which is exactly what the remote debugger replaces (with the relay in place
///         of these stubs).
///     </para>
/// </remarks>
[TestFixture]
public class CrossTargetFrameworkExecutionTests
{
    private const string AccountPlugin = "XrmFramework.PluginInventory.Fixtures.AccountPlugin";
    private const string WorkflowActivity = "XrmFramework.PluginInventory.Fixtures.ComputeScoreActivity";

    private static string[] TargetFrameworks() => InventoryHost.FixtureTargetFrameworks;

    // ── The load-bearing assertion: one Microsoft.Xrm.Sdk, not two ──────────────────

    /// <remarks>
    ///     Everything else here is a consequence of this. The host's <c>Microsoft.Xrm.Sdk</c> comes
    ///     from Dataverse.Client's net8.0 asset; the plugin was compiled against
    ///     <c>Microsoft.CrmSdk.CoreAssemblies</c>' net462 one. They are interchangeable at runtime
    ///     only because Microsoft ships both under one identity - <c>Version=9.0.0.0</c>,
    ///     <c>PublicKeyToken=31bf3856ad364e35</c>. Were that ever to stop holding, a plugin would
    ///     start rejecting the host's own <c>IPluginExecutionContext</c>, and this is the test that
    ///     would say so instead of leaving a baffling cast failure at debug time.
    /// </remarks>
    [TestCaseSource(nameof(TargetFrameworks))]
    public void Host_And_Plugin_Agree_On_One_Microsoft_Xrm_Sdk(string targetFramework)
    {
        var pluginType = PluginExecutionHost
            .Load(InventoryHost.Fixture(targetFramework))
            .GetType(AccountPlugin, throwOnError: true);

        var pluginsIPlugin = pluginType.GetInterfaces().Single(i => i.Name == nameof(IPlugin));

        Assert.Multiple(() =>
        {
            Assert.That(typeof(IPlugin).IsAssignableFrom(pluginType), Is.True,
                "The net4x plugin does not implement the host's IPlugin: the two runtimes resolved "
                + "different Microsoft.Xrm.Sdk assemblies.");

            // Same identity is not enough; it has to be the very same loaded assembly.
            Assert.That(pluginsIPlugin.Assembly, Is.SameAs(typeof(IPlugin).Assembly));
        });
    }

    // ── Executing the plugin ────────────────────────────────────────────────────────

    /// <remarks>
    ///     The trace is the framework's own, emitted through the stub <c>ITracingService</c>, so
    ///     asserting on it is asserting that XrmFramework's pipeline ran - not merely that a method
    ///     was reachable by reflection.
    /// </remarks>
    [TestCaseSource(nameof(TargetFrameworks))]
    public void Net4x_Plugin_Runs_And_Dispatches_To_The_Matching_Step(string targetFramework)
    {
        var run = PluginExecutionHost.Execute(InventoryHost.Fixture(targetFramework), AccountPlugin);

        Assert.Multiple(() =>
        {
            // The context is PreOperation / Create / Synchronous / account, so of the fixture's five
            // registered steps exactly one matches, and OnCreate is the method it names.
            Assert.That(run.TraceText, Does.Contain("Entity: account"));
            Assert.That(run.TraceText, Does.Contain("Message: Create"));
            Assert.That(run.TraceText, Does.Contain("Stage: PreOperation"));
            Assert.That(run.TraceText, Does.Contain("OnCreate"));

            // A step that should NOT have been selected for this context.
            Assert.That(run.TraceText, Does.Not.Contain("OnUpdate"));
        });
    }

    /// <remarks>
    ///     The remote-debug redirect is compiled into the fixture (no <c>DISABLE_REMOTE_DEBUG</c>), so
    ///     a plugin that really executed must have gone looking for a DebugSession. That query is the
    ///     proof the pipeline was entered rather than short-circuited somewhere benign.
    /// </remarks>
    [TestCaseSource(nameof(TargetFrameworks))]
    public void Running_Plugin_Performs_Its_Debug_Session_Lookup(string targetFramework)
    {
        var run = PluginExecutionHost.Execute(InventoryHost.Fixture(targetFramework), AccountPlugin);

        Assert.That(run.OrganizationServiceCalls,
            Has.Some.Contains("debugsession").IgnoreCase,
            "The plugin made no DebugSession query, so the remote-debug redirect never ran: "
            + $"calls were [{string.Join(", ", run.OrganizationServiceCalls)}].");
    }

    // ── The boundary of what works today ────────────────────────────────────────────

    /// <summary>
    ///     A Custom Workflow Activity LOADS on this runtime, but cannot yet be executed.
    /// </summary>
    /// <remarks>
    ///     This pins the phase-1 limit rather than leaving it to a comment. Loading works because of
    ///     the <c>System.Activities</c> stand-in; running does not, because the stand-in describes
    ///     activities and refuses to run them - every member throws, by design
    ///     (<c>XrmFramework.PluginInventory.SystemActivities</c>). The first thing an activity touches
    ///     is <c>CodeActivityContext.GetExtension&lt;IWorkflowContext&gt;()</c>, from
    ///     <c>LocalContext(CodeActivityContext)</c>, before any <c>try</c>.
    ///     <para>
    ///         When someone gives the stand-in real bodies plus a <c>WorkflowInvoker</c>, this test
    ///         fails - and that is the point: it has to be rewritten into an assertion that the
    ///         activity ran, not quietly left behind.
    ///     </para>
    /// </remarks>
    [TestCaseSource(nameof(TargetFrameworks))]
    public void Workflow_Activity_Loads_But_Cannot_Be_Executed_Yet(string targetFramework)
    {
        var activityType = PluginExecutionHost
            .Load(InventoryHost.Fixture(targetFramework))
            .GetType(WorkflowActivity, throwOnError: true);

        // Constructing it already exercises the stand-in's type layout (InArgument<string> backing
        // fields, [RequiredArgument]) and the activity's own SetAction/SetDisplayName.
        var activity = Activator.CreateInstance(activityType);

        var execute = activityType.GetMethod(
            "Execute", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(execute, Is.Not.Null, "CodeActivity.Execute(CodeActivityContext) was not found.");

        var contextType = execute.GetParameters().Single().ParameterType;
        var context = Activator.CreateInstance(contextType);

        var invocation = Assert.Throws<TargetInvocationException>(
            () => execute.Invoke(activity, new[] { context }));

        Assert.Multiple(() =>
        {
            Assert.That(activity, Is.Not.Null);
            Assert.That(contextType.Assembly.GetName().Name, Is.EqualTo("System.Activities"),
                "The activity did not bind to the stand-in.");
            Assert.That(invocation.InnerException, Is.TypeOf<NotSupportedException>(),
                "Executing an activity failed for some reason other than the stand-in refusing to run.");
        });
    }
}
