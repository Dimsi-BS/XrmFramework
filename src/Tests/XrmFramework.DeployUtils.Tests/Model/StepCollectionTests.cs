// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.Model;

/// <summary>
/// Tests unitaires pour <see cref="StepCollection"/> et son <see cref="StepCollection.StepComparer"/> interne.
/// </summary>
[TestClass]
public class StepCollectionTests
{
    // ──────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────

    /// <summary>
    /// Crée un objet dynamique compatible avec <see cref="Step.FromXrmFrameworkStep"/>.
    /// </summary>
    private static dynamic CreateFakeXrmStep(
        string pluginTypeName = "MyPlugin",
        string message = "Create",
        Stages stage = Stages.PreOperation,
        Modes mode = Modes.Synchronous,
        string entityName = "contact",
        string? unsecureConfig = null,
        string? impersonationUsername = null,
        int order = 1)
    {
        return new FakeXrmStep(pluginTypeName, message, stage, mode, entityName, unsecureConfig, impersonationUsername, order);
    }

    // ──────────────────────────────────────────────
    //  StepCollection.Add — ajout simple
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Add_SingleStep_CollectionContainsOneItem()
    {
        var collection = new StepCollection();
        var step = Step.FromXrmFrameworkStep(CreateFakeXrmStep());

        collection.Add(step);

        Assert.AreEqual(1, collection.Count);
    }

    [TestMethod]
    public void Add_TwoDistinctSteps_CollectionContainsTwoItems()
    {
        var collection = new StepCollection();
        var step1 = Step.FromXrmFrameworkStep(CreateFakeXrmStep(message: "Create"));
        var step2 = Step.FromXrmFrameworkStep(CreateFakeXrmStep(message: "Update"));

        collection.Add(step1);
        collection.Add(step2);

        Assert.AreEqual(2, collection.Count);
    }

    // ──────────────────────────────────────────────
    //  StepCollection.Add — fusion de doublons
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Add_DuplicateStep_MergesIntoSingleEntry()
    {
        var collection = new StepCollection();

        var step1 = Step.FromXrmFrameworkStep(CreateFakeXrmStep("MyPlugin", "Update", Stages.PreOperation, Modes.Synchronous, "account"));
        step1.FilteringAttributes.Add("name");

        var step2 = Step.FromXrmFrameworkStep(CreateFakeXrmStep("MyPlugin", "Update", Stages.PreOperation, Modes.Synchronous, "account"));
        step2.FilteringAttributes.Add("emailaddress1");

        collection.Add(step1);
        collection.Add(step2);

        // Un seul step fusionné doit exister
        Assert.AreEqual(1, collection.Count);
    }

    [TestMethod]
    public void Add_DuplicateStep_MergesFilteringAttributes()
    {
        var collection = new StepCollection();

        var step1 = Step.FromXrmFrameworkStep(CreateFakeXrmStep("MyPlugin", "Update", Stages.PreOperation, Modes.Synchronous, "account"));
        step1.FilteringAttributes.Add("name");

        var step2 = Step.FromXrmFrameworkStep(CreateFakeXrmStep("MyPlugin", "Update", Stages.PreOperation, Modes.Synchronous, "account"));
        step2.FilteringAttributes.Add("emailaddress1");

        collection.Add(step1);
        collection.Add(step2);

        var merged = System.Linq.Enumerable.First(collection);
        CollectionAssert.Contains(merged.FilteringAttributes, "name");
        CollectionAssert.Contains(merged.FilteringAttributes, "emailaddress1");
    }

    // ──────────────────────────────────────────────
    //  StepCollection — étapes distinctes par stage
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Add_SameMessageDifferentStage_AreConsideredDistinct()
    {
        var collection = new StepCollection();
        var preStep = Step.FromXrmFrameworkStep(CreateFakeXrmStep("MyPlugin", "Update", Stages.PreOperation, Modes.Synchronous, "account"));
        var postStep = Step.FromXrmFrameworkStep(CreateFakeXrmStep("MyPlugin", "Update", Stages.PostOperation, Modes.Synchronous, "account"));

        collection.Add(preStep);
        collection.Add(postStep);

        Assert.AreEqual(2, collection.Count);
    }

    [TestMethod]
    public void Add_SameMessageDifferentMode_AreConsideredDistinct()
    {
        var collection = new StepCollection();
        var syncStep = Step.FromXrmFrameworkStep(CreateFakeXrmStep("MyPlugin", "Update", Stages.PostOperation, Modes.Synchronous, "account"));
        var asyncStep = Step.FromXrmFrameworkStep(CreateFakeXrmStep("MyPlugin", "Update", Stages.PostOperation, Modes.Asynchronous, "account"));

        collection.Add(syncStep);
        collection.Add(asyncStep);

        Assert.AreEqual(2, collection.Count);
    }

    // ──────────────────────────────────────────────
    //  StepCollection — opérations ICollection
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Clear_RemovesAllSteps()
    {
        var collection = new StepCollection();
        collection.Add(Step.FromXrmFrameworkStep(CreateFakeXrmStep(message: "Create")));
        collection.Add(Step.FromXrmFrameworkStep(CreateFakeXrmStep(message: "Update")));

        collection.Clear();

        Assert.AreEqual(0, collection.Count);
    }

    // ──────────────────────────────────────────────
    //  Classe helper interne
    // ──────────────────────────────────────────────

    /// <summary>
    /// Objet concret simulant la forme dynamique attendue par <see cref="Step.FromXrmFrameworkStep"/>.
    /// </summary>
    private sealed class FakeXrmStep
    {
        public FakeXrmStep(string pluginTypeName, string message, Stages stage, Modes mode, string entityName,
            string? unsecureConfig, string? impersonationUsername, int order)
        {
            Plugin = new FakePlugin(pluginTypeName);
            Message = Messages.GetMessage(message);
            Stage = (int)stage;
            Mode = (int)mode;
            EntityName = entityName;
            UnsecureConfig = unsecureConfig ?? string.Empty;
            ImpersonationUsername = impersonationUsername ?? string.Empty;
            Order = order;
        }

        public FakePlugin Plugin { get; }
        public Messages Message { get; }
        public int Stage { get; }
        public int Mode { get; }
        public string EntityName { get; }
        public string UnsecureConfig { get; }
        public string ImpersonationUsername { get; }
        public int Order { get; }
        public bool PreImageAllAttributes { get; } = false;
        public bool PostImageAllAttributes { get; } = false;
        public List<string> FilteringAttributes { get; } = new();
        public List<string> PreImageAttributes { get; } = new();
        public List<string> PostImageAttributes { get; } = new();
        public List<string> MethodNames { get; } = new();
    }

    private sealed class FakePlugin
    {
        private readonly string _name;
        public FakePlugin(string name) => _name = name;
        // GetType().Name sera "FakePlugin" — on ne peut pas changer le vrai type au runtime.
        // On expose Name pour que les comparaisons de PluginTypeName soient correctes.
        public string TypeName => _name;
    }
}
