// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using NUnit.Framework;
using XrmFramework.RemoteDebugger.Client;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     The remote debugger's one capability gap on .NET: it runs plugins and Custom APIs, and
///     refuses Custom Workflow Activities.
/// </summary>
/// <remarks>
///     <para>
///         Running on net8.0, this fixture only ever sees the .NET leg of
///         <c>WorkflowActivitySupport</c> - which is the half worth testing, because the .NET
///         Framework half is the behaviour that already shipped.
///     </para>
///     <para>
///         What it has to get right is recognition WITHOUT <c>CodeActivity</c>: that type is
///         unreferenceable on this leg, so the check walks base types comparing full names. The
///         activity below therefore derives from a locally declared
///         <c>System.Activities.CodeActivity</c> - the same trick <c>Stubs.cs</c> plays for the
///         XrmFramework types, and all the check can see of the real thing anyway, whether it comes
///         from .NET Framework's System.Activities or from the stand-in a .NET host resolves instead.
///     </para>
///     <para>
///         Driven through reflection because <c>WorkflowActivitySupport</c> is internal to
///         XrmFramework.RemoteDebugger.Client, and it should stay that way - the same reasoning as
///         <see cref="PluginInventoryProcessRunnerTests" />: an <c>InternalsVisibleTo</c> would change
///         a shipped, strong-named assembly to suit a test.
///     </para>
/// </remarks>
[TestFixture]
public class WorkflowActivitySupportTests
{
    // PluginTestRunner is public, so it is a stable anchor onto the client assembly.
    private static readonly Type Support = typeof(PluginTestRunner).Assembly
        .GetType("XrmFramework.RemoteDebugger.Client.WorkflowActivitySupport", throwOnError: true);

    private static bool IsSupported()
        => (bool)Support.GetProperty("IsSupported", BindingFlags.Static | BindingFlags.NonPublic)!
            .GetValue(null)!;

    private static bool IsWorkflowActivity(Type type)
        => (bool)Support.GetMethod("IsWorkflowActivity", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, new object[] { type })!;

    private static Exception RunAndCatch(Type activityType)
    {
        var run = Support.GetMethod("Run", BindingFlags.Static | BindingFlags.NonPublic)!;

        // Null services on purpose: on this leg Run must refuse before it touches anything, so
        // passing real ones would hide a reordering that started doing work first.
        var invocation = Assert.Throws<TargetInvocationException>(
            () => run.Invoke(null, new object[] { activityType, null, null }));

        return invocation.InnerException;
    }

    // ── The capability this runtime reports ─────────────────────────────────────────

    [Test]
    public void Workflow_Activities_Are_Not_Supported_On_This_Runtime()
        => Assert.That(IsSupported(), Is.False,
            "The .NET leg claims it can run workflow activities. Windows Workflow Foundation has no "
            + ".NET build, so either the capability symbol leaked onto this leg or it is now earned.");

    // ── Telling an activity from a plugin, without CodeActivity ─────────────────────

    /// <remarks>
    ///     The load-bearing one. Get it wrong and a workflow activity falls through to the plugin
    ///     branch, where the failure is an <c>InvalidOperationException</c> about a type that is
    ///     "neither an IPlugin nor a CodeActivity" - true, unhelpful, and pointing away from the real
    ///     reason.
    /// </remarks>
    [Test]
    public void A_Workflow_Activity_Is_Recognised_By_Its_Base_Type_Name()
        => Assert.That(IsWorkflowActivity(typeof(SampleActivity)), Is.True);

    /// <remarks>Indirect descent counts: real activities derive from CustomWorkflowActivity.</remarks>
    [Test]
    public void Recognition_Follows_The_Whole_Base_Chain()
        => Assert.That(IsWorkflowActivity(typeof(SampleDerivedActivity)), Is.True);

    [Test]
    public void A_Plugin_Is_Not_Mistaken_For_A_Workflow_Activity()
        => Assert.That(IsWorkflowActivity(typeof(SamplePlugin)), Is.False);

    /// <remarks>
    ///     Name alone is not enough to be a base type: a class merely CALLED CodeActivity, deriving
    ///     from nothing of the sort, is not an activity.
    /// </remarks>
    [Test]
    public void A_Type_That_Only_Shares_The_Name_Is_Not_An_Activity()
        => Assert.That(IsWorkflowActivity(typeof(SameNameDifferentNamespace)), Is.False);

    [Test]
    public void Nothing_Is_Recognised_When_There_Is_No_Type()
        => Assert.That(IsWorkflowActivity(null), Is.False);

    // ── Refusing to run one, readably ───────────────────────────────────────────────

    /// <remarks>
    ///     The message is asserted, not just the exception type: this is the only thing the person
    ///     debugging will see, and the whole point of the refusal is that it names the way out
    ///     (run the debugger on net48) rather than leaving them to guess.
    /// </remarks>
    [Test]
    public void Running_A_Workflow_Activity_Is_Refused_With_The_Way_Out()
    {
        var error = RunAndCatch(typeof(SampleActivity));

        Assert.Multiple(() =>
        {
            Assert.That(error, Is.TypeOf<PlatformNotSupportedException>());
            Assert.That(error.Message, Does.Contain(typeof(SampleActivity).FullName),
                "The refusal does not say which type it is about.");
            Assert.That(error.Message, Does.Contain("net48"),
                "The refusal does not say how to debug the activity after all.");
        });
    }

    // ── Shapes under test ───────────────────────────────────────────────────────────

    private sealed class SampleActivity : global::System.Activities.CodeActivity { }

    private abstract class SampleActivityBase : global::System.Activities.CodeActivity { }

    private sealed class SampleDerivedActivity : SampleActivityBase { }

    private sealed class SamplePlugin { }

    private sealed class SameNameDifferentNamespace : global::NotAnActivity.CodeActivity { }
}
