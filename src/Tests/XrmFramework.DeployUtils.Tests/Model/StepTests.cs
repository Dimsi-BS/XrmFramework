// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.Model;

[TestFixture]
public class StepTests
{
    // ──────────────────────────────────────────────
    //  Constructeur
    // ──────────────────────────────────────────────

    [Test]
    public void Constructor_SetsPluginTypeName()
    {
        var step = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "contact");

        Assert.AreEqual("MyPlugin", step.PluginTypeName);
    }

    [Test]
    public void Constructor_SetsMessage()
    {
        var step = new Step("MyPlugin", Messages.Update, Stages.PreOperation, Modes.Synchronous, "contact");

        Assert.AreEqual(Messages.Update, step.Message);
    }

    [Test]
    public void Constructor_SetsStage()
    {
        var step = new Step("MyPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "contact");

        Assert.AreEqual(Stages.PostOperation, step.Stage);
    }

    [Test]
    public void Constructor_SetsMode()
    {
        var step = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous, "contact");

        Assert.AreEqual(Modes.Asynchronous, step.Mode);
    }

    [Test]
    public void Constructor_SetsEntityName()
    {
        var step = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "account");

        Assert.AreEqual("account", step.EntityName);
    }

    [Test]
    public void Constructor_InitializesPreImage()
    {
        var step = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "contact");

        Assert.IsNotNull(step.PreImage);
        Assert.AreSame(step, step.PreImage.FatherStep);
    }

    [Test]
    public void Constructor_InitializesPostImage()
    {
        var step = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "contact");

        Assert.IsNotNull(step.PostImage);
        Assert.AreSame(step, step.PostImage.FatherStep);
    }

    // ──────────────────────────────────────────────
    //  Associate / Disassociate : EntityName -> RelationshipName
    // ──────────────────────────────────────────────

    [Test]
    public void Constructor_AssociateMessage_MovesEntityNameToRelationship()
    {
        var step = new Step("MyPlugin", Messages.Associate, Stages.PreOperation, Modes.Synchronous, "myrelationship");

        Assert.AreEqual(string.Empty, step.EntityName,
            "EntityName should be cleared for Associate steps.");
        Assert.AreEqual("myrelationship", step.StepConfiguration.RelationshipName,
            "The entity name should move to RelationshipName.");
    }

    [Test]
    public void Constructor_DisassociateMessage_MovesEntityNameToRelationship()
    {
        var step = new Step("MyPlugin", Messages.Disassociate, Stages.PreOperation, Modes.Synchronous, "myrelationship");

        Assert.AreEqual(string.Empty, step.EntityName);
        Assert.AreEqual("myrelationship", step.StepConfiguration.RelationshipName);
    }

    // ──────────────────────────────────────────────
    //  Merge
    // ──────────────────────────────────────────────

    [Test]
    public void Merge_CombinesMethodNames()
    {
        var x = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "contact");
        x.StepConfiguration.RegisteredMethods.Add("OnCreate");

        var y = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "contact");
        y.StepConfiguration.RegisteredMethods.Add("OnCreateAlso");

        x.Merge(y);

        Assert.IsTrue(x.MethodNames.Contains("OnCreate"));
        Assert.IsTrue(x.MethodNames.Contains("OnCreateAlso"));
    }

    [Test]
    public void Merge_DoNotFilterAttributesTrue_ClearsFilteringAttributes()
    {
        var x = new Step("MyPlugin", Messages.Update, Stages.PreOperation, Modes.Synchronous, "contact");
        x.FilteringAttributes.Add("name");

        var y = new Step("MyPlugin", Messages.Update, Stages.PreOperation, Modes.Synchronous, "contact");
        y.DoNotFilterAttributes = true;

        x.Merge(y);

        Assert.IsTrue(x.DoNotFilterAttributes);
        Assert.AreEqual(0, x.FilteringAttributes.Count);
    }

    [Test]
    public void Merge_BothHaveFilteringAttributes_CombinesUniquely()
    {
        var x = new Step("MyPlugin", Messages.Update, Stages.PreOperation, Modes.Synchronous, "contact");
        x.FilteringAttributes.Add("name");

        var y = new Step("MyPlugin", Messages.Update, Stages.PreOperation, Modes.Synchronous, "contact");
        y.FilteringAttributes.Add("email");
        y.FilteringAttributes.Add("name"); // duplicate

        x.Merge(y);

        Assert.AreEqual(2, x.FilteringAttributes.Count);
        Assert.IsTrue(x.FilteringAttributes.Contains("name"));
        Assert.IsTrue(x.FilteringAttributes.Contains("email"));
    }

    // ──────────────────────────────────────────────
    //  Description
    // ──────────────────────────────────────────────

    [Test]
    public void Description_ContainsPluginTypeName()
    {
        var step = new Step("ContactPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "contact");

        Assert.IsTrue(step.Description.Contains("ContactPlugin"));
    }

    // ──────────────────────────────────────────────
    //  UnsecureConfig (JSON serialization)
    // ──────────────────────────────────────────────

    [Test]
    public void UnsecureConfig_IsValidJson()
    {
        var step = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "contact");

        var json = step.UnsecureConfig;

        Assert.IsFalse(string.IsNullOrEmpty(json));
        // Should be deserializable
        var config = Newtonsoft.Json.JsonConvert.DeserializeObject<StepConfiguration>(json);
        Assert.IsNotNull(config);
    }

    // ──────────────────────────────────────────────
    //  UniqueName
    // ──────────────────────────────────────────────

    [Test]
    public void UniqueName_ContainsStageMessageAndEntity()
    {
        var step = new Step("MyPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "contact")
        {
            PluginTypeFullName = "Assembly.MyPlugin"
        };

        var name = step.UniqueName;

        Assert.IsTrue(name.Contains("Assembly.MyPlugin"));
        Assert.IsTrue(name.Contains("PreOperation"));
    }
}
