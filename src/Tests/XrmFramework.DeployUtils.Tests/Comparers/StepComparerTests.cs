// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.Comparers;

[TestFixture]
public class StepComparerTests
{
    private StepComparer _comparer = null!;

    [SetUp]
    public void SetUp()
    {
        _comparer = new StepComparer();
    }

    // Helper to create a canonical Step
    private static Step MakeStep(
        string pluginTypeName = "MyPlugin",
        string pluginTypeFullName = "Assembly.MyPlugin",
        string entityName = "contact",
        Messages message = null!,
        Stages stage = Stages.PreOperation,
        Modes mode = Modes.Synchronous)
    {
        message ??= Messages.Create;
        return new Step(pluginTypeName, message, stage, mode, entityName)
        {
            PluginTypeFullName = pluginTypeFullName
        };
    }

    // ──────────────────────────────────────────────
    //  Equals — cas positifs
    // ──────────────────────────────────────────────

    [Test]
    public void Equals_IdenticalSteps_ReturnsTrue()
    {
        var x = MakeStep();
        var y = MakeStep();

        Assert.IsTrue(_comparer.Equals(x, y));
    }

    [Test]
    public void Equals_BothNull_ReturnsTrue()
    {
        Assert.IsTrue(_comparer.Equals(null!, null!));
    }

    // ──────────────────────────────────────────────
    //  Equals — cas négatifs
    // ──────────────────────────────────────────────

    [Test]
    public void Equals_XNull_ReturnsFalse()
    {
        Assert.IsFalse(_comparer.Equals(null!, MakeStep()));
    }

    [Test]
    public void Equals_YNull_ReturnsFalse()
    {
        Assert.IsFalse(_comparer.Equals(MakeStep(), null!));
    }

    [Test]
    public void Equals_DifferentMessage_ReturnsFalse()
    {
        var x = MakeStep(message: Messages.Create);
        var y = MakeStep(message: Messages.Update);

        Assert.IsFalse(_comparer.Equals(x, y));
    }

    [Test]
    public void Equals_DifferentStage_ReturnsFalse()
    {
        var x = MakeStep(stage: Stages.PreOperation);
        var y = MakeStep(stage: Stages.PostOperation);

        Assert.IsFalse(_comparer.Equals(x, y));
    }

    [Test]
    public void Equals_DifferentMode_ReturnsFalse()
    {
        var x = MakeStep(mode: Modes.Synchronous);
        var y = MakeStep(mode: Modes.Asynchronous);

        Assert.IsFalse(_comparer.Equals(x, y));
    }

    [Test]
    public void Equals_DifferentPluginTypeName_ReturnsFalse()
    {
        var x = MakeStep(pluginTypeName: "PluginA");
        var y = MakeStep(pluginTypeName: "PluginB");

        Assert.IsFalse(_comparer.Equals(x, y));
    }

    [Test]
    public void Equals_DifferentEntityName_ReturnsFalse()
    {
        var x = MakeStep(entityName: "contact");
        var y = MakeStep(entityName: "account");

        Assert.IsFalse(_comparer.Equals(x, y));
    }

    // ──────────────────────────────────────────────
    //  GetHashCode
    // ──────────────────────────────────────────────

    [Test]
    public void GetHashCode_EqualSteps_ReturnSameHash()
    {
        var x = MakeStep();
        var y = MakeStep();

        Assert.AreEqual(_comparer.GetHashCode(x), _comparer.GetHashCode(y));
    }

    [Test]
    public void GetHashCode_DifferentSteps_ReturnDifferentHash()
    {
        var x = MakeStep(entityName: "contact");
        var y = MakeStep(entityName: "account");

        // Hash collision is theoretically possible but should not happen for these simple inputs
        Assert.AreNotEqual(_comparer.GetHashCode(x), _comparer.GetHashCode(y));
    }

    // ──────────────────────────────────────────────
    //  NeedsUpdate
    // ──────────────────────────────────────────────

    [Test]
    public void NeedsUpdate_IdenticalSteps_ReturnsFalse()
    {
        var x = MakeStep();
        var y = MakeStep();

        Assert.IsFalse(_comparer.NeedsUpdate(x, y));
    }

    [Test]
    public void NeedsUpdate_DifferentDoNotFilterAttributes_ReturnsTrue()
    {
        var x = MakeStep();
        x.DoNotFilterAttributes = true;

        var y = MakeStep();
        y.DoNotFilterAttributes = false;

        Assert.IsTrue(_comparer.NeedsUpdate(x, y));
    }

    [Test]
    public void NeedsUpdate_DifferentOrder_ReturnsTrue()
    {
        var x = MakeStep();
        x.Order = 1;

        var y = MakeStep();
        y.Order = 2;

        Assert.IsTrue(_comparer.NeedsUpdate(x, y));
    }

    [Test]
    public void NeedsUpdate_DifferentImpersonationUsername_ReturnsTrue()
    {
        var x = MakeStep();
        x.ImpersonationUsername = "admin@org.com";

        var y = MakeStep();
        y.ImpersonationUsername = null;

        Assert.IsTrue(_comparer.NeedsUpdate(x, y));
    }

    [Test]
    public void NeedsUpdate_DifferentFilteringAttributes_ReturnsTrue()
    {
        var x = MakeStep();
        x.FilteringAttributes.Add("name");

        var y = MakeStep();
        // y has no filtering attributes

        Assert.IsTrue(_comparer.NeedsUpdate(x, y));
    }

    [Test]
    public void NeedsUpdate_SameFilteringAttributes_ReturnsFalse()
    {
        var x = MakeStep();
        x.FilteringAttributes.Add("name");
        x.FilteringAttributes.Add("firstname");

        var y = MakeStep();
        y.FilteringAttributes.Add("name");
        y.FilteringAttributes.Add("firstname");

        Assert.IsFalse(_comparer.NeedsUpdate(x, y));
    }
}
