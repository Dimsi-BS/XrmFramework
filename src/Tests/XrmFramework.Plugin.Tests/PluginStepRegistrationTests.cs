// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace XrmFramework.Tests.Plugin;

/// <summary>
/// Unit tests for the step registration mechanism (<see cref="Plugin.AddStep"/>)
/// and plugin initialization (<see cref="Plugin"/>).
/// </summary>
[TestFixture]
public class PluginStepRegistrationTests
{
    // ──────────────────────────────────────────────
    //  Minimal concrete plugin for the tests
    // ──────────────────────────────────────────────

    /// <summary>
    /// Minimal plugin with a Create step on the Contact entity.
    /// </summary>
    private sealed class ContactCreatePlugin(string? unsecuredConfig = null, string? securedConfig = null)
        : XrmFramework.Plugin(unsecuredConfig, securedConfig)
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "contact", nameof(OnContactCreate));
        }

        public void OnContactCreate(IPluginContext _) { }
    }

    /// <summary>
    /// Plugin with several steps to test accumulation.
    /// </summary>
    private sealed class MultiStepPlugin() : XrmFramework.Plugin(null, null)
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "account", nameof(OnCreate));
            AddStep(Stages.PostOperation, Messages.Update, Modes.Asynchronous, "account", nameof(OnUpdate));
        }

        public void OnCreate(IPluginContext _) { }
        public void OnUpdate(IPluginContext _) { }
    }

    /// <summary>
    /// Plugin with a step method requesting <see cref="IDateTimeProvider"/> in addition to <see cref="IPluginContext"/>.
    /// </summary>
    private sealed class DateTimeProviderPlugin() : XrmFramework.Plugin(null, null)
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "contact", nameof(OnContactCreate));
        }

        public void OnContactCreate(IPluginContext _, IDateTimeProvider clock) { }
    }

    /// <summary>
    /// A framework service other than <see cref="IDateTimeProvider"/>: what
    /// <see cref="IXrmFrameworkService"/> exists to allow.
    /// </summary>
    private interface ICustomFrameworkService : IXrmFrameworkService
    {
    }

    /// <summary>
    /// Plugin whose step method requests an <see cref="IXrmFrameworkService"/> that is not
    /// <see cref="IDateTimeProvider"/>. The whitelist used to name IDateTimeProvider itself, so
    /// every other implementation of the marker was rejected at registration.
    /// </summary>
    private sealed class FrameworkServicePlugin() : XrmFramework.Plugin(null, null)
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "contact", nameof(OnContactCreate));
        }

        public void OnContactCreate(IPluginContext _, ICustomFrameworkService service) { }
    }

    /// <summary>
    /// Plugin whose step method requests an interface the container knows nothing about
    /// (registration must still fail).
    /// </summary>
    private sealed class UnknownInterfacePlugin() : XrmFramework.Plugin(null, null)
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "contact", nameof(OnContactCreate));
        }

        public void OnContactCreate(IPluginContext _, System.IDisposable notAService) { }
    }

    /// <summary>
    /// Plugin whose step method is private (registration must fail).
    /// </summary>
    private sealed class PrivateMethodPlugin() : XrmFramework.Plugin(null, null, delayStepRegistration: true)
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "contact", nameof(PrivateAction));
        }

#pragma warning disable IDE0051
        private void PrivateAction(IPluginContext _) { }
#pragma warning restore IDE0051
    }

    // ──────────────────────────────────────────────
    //  Plugin initialization
    // ──────────────────────────────────────────────

    [Test]
    public void Plugin_Constructor_StepsInitializedIsTrue()
    {
        var plugin = new ContactCreatePlugin();

        Assert.IsTrue(plugin.StepsInitialized);
    }

    [Test]
    public void Plugin_DelayStepRegistration_StepsInitializedIsFalse()
    {
        var plugin = new PrivateMethodPlugin(); // delayStepRegistration = true

        Assert.IsFalse(plugin.StepsInitialized);
    }

    // ──────────────────────────────────────────────
    //  Step registration
    // ──────────────────────────────────────────────

    [Test]
    public void AddStep_SingleStep_PluginHasOneStep()
    {
        var plugin = new ContactCreatePlugin();

        Assert.AreEqual(1, plugin.Steps.Count);
    }

    [Test]
    public void AddStep_SingleStep_StepHasCorrectMessage()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual(Messages.Create, step.Message);
    }

    [Test]
    public void AddStep_SingleStep_StepHasCorrectStage()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual(Stages.PreOperation, step.Stage);
    }

    [Test]
    public void AddStep_SingleStep_StepHasCorrectMode()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual(Modes.Synchronous, step.Mode);
    }

    [Test]
    public void AddStep_SingleStep_StepHasCorrectEntityName()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual("contact", step.EntityName);
    }

    [Test]
    public void AddStep_SingleStep_MethodNameMatches()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual(nameof(ContactCreatePlugin.OnContactCreate), step.Method?.Name);
    }

    [Test]
    public void AddStep_MultipleSteps_AllStepsRegistered()
    {
        var plugin = new MultiStepPlugin();

        Assert.AreEqual(2, plugin.Steps.Count);
    }

    [Test]
    public void AddStep_DateTimeProviderParameter_StepIsRegistered()
    {
        var plugin = new DateTimeProviderPlugin();

        Assert.AreEqual(1, plugin.Steps.Count);
    }

    [Test]
    public void AddStep_XrmFrameworkServiceParameter_StepIsRegistered()
    {
        var plugin = new FrameworkServicePlugin();

        Assert.AreEqual(1, plugin.Steps.Count);
    }

    [Test]
    public void AddStep_UnknownInterfaceParameter_Throws()
    {
        Assert.Throws<InvalidPluginExecutionException>(() => new UnknownInterfacePlugin());
    }

}
