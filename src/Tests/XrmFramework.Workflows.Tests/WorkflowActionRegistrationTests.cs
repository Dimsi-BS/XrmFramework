// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using XrmFramework.Workflow;

// ReSharper disable once CheckNamespace
namespace XrmFramework.Tests.Plugin;

/// <summary>
/// Custom workflow activities validate their action parameters exactly like plugins validate
/// their step parameters (<see cref="CustomWorkflowActivity.SetAction"/>). Nothing referenced
/// XrmFramework.Workflows from a test project until now, so that closed list was never covered.
/// </summary>
[TestFixture]
public class WorkflowActionRegistrationTests
{
    private abstract class TestActivity : CustomWorkflowActivity
    {
        public MethodInfo? RegisteredAction => ActivityAction;

        protected override void Execute(System.Activities.CodeActivityContext context)
        {
            // Never executed: these tests only exercise registration.
        }
    }

    private interface ICustomFrameworkService : IXrmFrameworkService
    {
    }

    private sealed class ContextOnlyActivity : TestActivity
    {
        public ContextOnlyActivity() => SetAction(nameof(Run));

        public void Run(ICustomWorkflowContext _) { }
    }

    private sealed class DateTimeProviderActivity : TestActivity
    {
        public DateTimeProviderActivity() => SetAction(nameof(Run));

        public void Run(ICustomWorkflowContext _, IDateTimeProvider clock) { }
    }

    private sealed class FrameworkServiceActivity : TestActivity
    {
        public FrameworkServiceActivity() => SetAction(nameof(Run));

        public void Run(ICustomWorkflowContext _, ICustomFrameworkService service) { }
    }

    private sealed class UnknownInterfaceActivity : TestActivity
    {
        public UnknownInterfaceActivity() => SetAction(nameof(Run));

        public void Run(ICustomWorkflowContext _, IDisposable notAService) { }
    }

    private sealed class PrivateActionActivity : TestActivity
    {
        public PrivateActionActivity() => SetAction("Run");

        private void Run(ICustomWorkflowContext _) { }
    }

    [Test]
    public void SetAction_ContextOnly_ActionIsRegistered()
    {
        var activity = new ContextOnlyActivity();

        Assert.AreEqual(nameof(ContextOnlyActivity.Run), activity.RegisteredAction?.Name);
    }

    [Test]
    public void SetAction_DateTimeProviderParameter_ActionIsRegistered()
    {
        var activity = new DateTimeProviderActivity();

        Assert.AreEqual(nameof(DateTimeProviderActivity.Run), activity.RegisteredAction?.Name);
    }

    [Test]
    public void SetAction_XrmFrameworkServiceParameter_ActionIsRegistered()
    {
        var activity = new FrameworkServiceActivity();

        Assert.AreEqual(nameof(FrameworkServiceActivity.Run), activity.RegisteredAction?.Name);
    }

    [Test]
    public void SetAction_UnknownInterfaceParameter_Throws()
    {
        Assert.Throws<InvalidPluginExecutionException>(() => new UnknownInterfaceActivity());
    }

    [Test]
    public void SetAction_PrivateAction_Throws()
    {
        Assert.Throws<InvalidPluginExecutionException>(() => new PrivateActionActivity());
    }
}
