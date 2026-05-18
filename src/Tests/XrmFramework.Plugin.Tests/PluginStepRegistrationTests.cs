// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace XrmFramework.Tests.Plugin;

/// <summary>
/// Tests unitaires pour la mécanique d'enregistrement des steps (<see cref="Plugin.AddStep"/>)
/// et l'initialisation du plugin (<see cref="Plugin"/>).
/// </summary>
[TestFixture]
public class PluginStepRegistrationTests
{
    // ──────────────────────────────────────────────
    //  Plugin concret minimal pour les tests
    // ──────────────────────────────────────────────

    /// <summary>
    /// Plugin minimal avec un step Create sur l'entité Contact.
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
    /// Plugin avec plusieurs steps pour tester l'accumulation.
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
    /// Plugin dont la méthode de step est privée (doit échouer à l'enregistrement).
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
    //  Initialisation du plugin
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
    //  Enregistrement des steps
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

}
