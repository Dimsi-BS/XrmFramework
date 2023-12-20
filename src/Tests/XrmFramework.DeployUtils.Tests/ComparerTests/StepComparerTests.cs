using System;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests;

[TestClass]
public class StepComparerTests
{
    private readonly StepComparer _comparer = new();

    [TestMethod]
    public void Equals_SameSteps()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin"
        };
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin"
        };

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_DifferentPlugin()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        { PluginTypeFullName = "assembly.thisPlugin" };
        var otherStep = new Step("otherPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        { PluginTypeFullName = "assembly.otherPlugin" };

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_DifferentMessage()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");
        var otherStep = new Step("thisPlugin", Messages.Default, Stages.PostOperation, Modes.Synchronous, "entity");

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_DifferentStages()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "entity");

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_DifferentModes()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Asynchronous, "entity");

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_DifferentEntities()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "otherEntity");

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_OtherIs_Null()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");
        Step otherStep = null;

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_Both_Null()
    {
        // Arrange
        Step thisStep = null;
        Step otherStep = null;

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_OtherHasNull_Plugin()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        { PluginTypeFullName = "assembly.thisPlugin" };
        var otherStep = new Step(null, Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        { PluginTypeFullName = null };

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_OtherHasNull_Message()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");
        var otherStep = new Step("thisPlugin", null, Stages.PostOperation, Modes.Synchronous, "entity");

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_OtherHasNull_Entity()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, null);

        // Act
        var result = _comparer.Equals(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void NeedsUpdate_SameSteps()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1
        };
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1
        };

        // Act
        var result = _comparer.NeedsUpdate(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void NeedsUpdate_DifferentSteps()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1
        };
        var otherStep = new Step("otherPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.otherPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1
        };

        // Act
        var result = _comparer.NeedsUpdate(thisStep, otherStep);

        // Assert
        Assert.IsFalse(result);
    }


    [TestMethod]
    public void NeedsUpdate_DifferentFilterAttributes()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1
        };
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "otherAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1
        };

        // Act
        var result = _comparer.NeedsUpdate(thisStep, otherStep);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void NeedsUpdate_DifferentImpersonationUser()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1
        };
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "otherUser",
            Order = 1
        };

        // Act
        var result = _comparer.NeedsUpdate(thisStep, otherStep);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void NeedsUpdate_DifferentOrder()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1
        };
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 2
        };

        // Act
        var result = _comparer.NeedsUpdate(thisStep, otherStep);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void NeedsUpdate_DifferentStepConfig()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1,
            StepConfiguration = new StepConfiguration()
            {
                DebugSessionId = Guid.NewGuid()
            }
        };
        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
        {
            PluginTypeFullName = "assembly.thisPlugin",
            FilteringAttributes = { "thisAttribute", "otherAttribute" },
            ImpersonationUsername = "thisUser",
            Order = 1,
            StepConfiguration = new StepConfiguration()
            {
                DebugSessionId = Guid.NewGuid()
            }
        };

        // Act
        var result = _comparer.NeedsUpdate(thisStep, otherStep);

        // Assert
        Assert.IsTrue(result);
    }

}
