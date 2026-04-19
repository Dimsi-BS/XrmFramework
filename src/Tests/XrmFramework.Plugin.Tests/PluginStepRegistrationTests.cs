// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.Tests.Plugin;

/// <summary>
/// Tests unitaires pour la mécanique d'enregistrement des steps (<see cref="Plugin.AddStep"/>)
/// et l'initialisation du plugin (<see cref="Plugin"/>).
/// </summary>
[TestClass]
public class PluginStepRegistrationTests
{
    // ──────────────────────────────────────────────
    //  Plugin concret minimal pour les tests
    // ──────────────────────────────────────────────

    /// <summary>
    /// Plugin minimal avec un step Create sur l'entité Contact.
    /// </summary>
    private sealed class ContactCreatePlugin : XrmFramework.Plugin
    {
        public ContactCreatePlugin(string unsecuredConfig = null, string securedConfig = null)
            : base(unsecuredConfig, securedConfig) { }

        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "contact", nameof(OnContactCreate));
        }

        public void OnContactCreate(IPluginContext context) { }
    }

    /// <summary>
    /// Plugin avec plusieurs steps pour tester l'accumulation.
    /// </summary>
    private sealed class MultiStepPlugin : XrmFramework.Plugin
    {
        public MultiStepPlugin()
            : base(null, null) { }

        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "account", nameof(OnCreate));
            AddStep(Stages.PostOperation, Messages.Update, Modes.Asynchronous, "account", nameof(OnUpdate));
        }

        public void OnCreate(IPluginContext context) { }
        public void OnUpdate(IPluginContext context) { }
    }

    /// <summary>
    /// Plugin dont la méthode de step est privée (doit échouer à l'enregistrement).
    /// </summary>
    private sealed class PrivateMethodPlugin : XrmFramework.Plugin
    {
        public PrivateMethodPlugin()
            : base(null, null, delayStepRegistration: true) { }

        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "contact", nameof(PrivateAction));
        }

#pragma warning disable IDE0051
        private void PrivateAction(IPluginContext context) { }
#pragma warning restore IDE0051
    }

    // ──────────────────────────────────────────────
    //  Initialisation du plugin
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Plugin_Constructor_StepsInitializedIsTrue()
    {
        var plugin = new ContactCreatePlugin();

        Assert.IsTrue(plugin.StepsInitialized);
    }

    [TestMethod]
    public void Plugin_DelayStepRegistration_StepsInitializedIsFalse()
    {
        var plugin = new PrivateMethodPlugin(); // delayStepRegistration = true

        Assert.IsFalse(plugin.StepsInitialized);
    }

    // ──────────────────────────────────────────────
    //  Enregistrement des steps
    // ──────────────────────────────────────────────

    [TestMethod]
    public void AddStep_SingleStep_PluginHasOneStep()
    {
        var plugin = new ContactCreatePlugin();

        Assert.AreEqual(1, plugin.Steps.Count);
    }

    [TestMethod]
    public void AddStep_SingleStep_StepHasCorrectMessage()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual(Messages.Create, step.Message);
    }

    [TestMethod]
    public void AddStep_SingleStep_StepHasCorrectStage()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual(Stages.PreOperation, step.Stage);
    }

    [TestMethod]
    public void AddStep_SingleStep_StepHasCorrectMode()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual(Modes.Synchronous, step.Mode);
    }

    [TestMethod]
    public void AddStep_SingleStep_StepHasCorrectEntityName()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual("contact", step.EntityName);
    }

    [TestMethod]
    public void AddStep_SingleStep_MethodNameMatches()
    {
        var plugin = new ContactCreatePlugin();
        var step = plugin.Steps[0];

        Assert.AreEqual(nameof(ContactCreatePlugin.OnContactCreate), step.Method?.Name);
    }

    [TestMethod]
    public void AddStep_MultipleSteps_AllStepsRegistered()
    {
        var plugin = new MultiStepPlugin();

        Assert.AreEqual(2, plugin.Steps.Count);
    }

}
