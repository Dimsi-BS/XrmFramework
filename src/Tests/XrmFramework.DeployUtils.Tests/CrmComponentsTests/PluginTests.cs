using System;
using System.Linq;
using Moq;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests;

[TestClass]
public class PluginTests
{
    private const string EntityTypeName = "plugintype";
    private readonly Plugin _component;

    public PluginTests()
    {
        _component = new Plugin("thisPlugin");
    }


    [TestMethod]
    public void CrmPropertiesTests()
    {
        Assert.AreEqual(_component.EntityTypeName, EntityTypeName);
        Assert.AreEqual(_component.Rank, 10);
        Assert.AreEqual(_component.DoAddToSolution, false);
        Assert.AreEqual(_component.DoFetchTypeCode, false);
    }

    [TestMethod]
    public void ChildrenTests()
    {
        Assert.IsTrue(_component.Children != null);
        Assert.IsFalse(_component.Children.Any());
    }

    [TestMethod]
    public void AddChildTests()
    {
        var anyComponent = new Mock<ICrmComponent>().Object;
        Assert.ThrowsException<ArgumentException>(() => _component.AddChild(anyComponent));
    }

    [TestMethod]
    public void AddChildTest_Step()
    {
        var step = new Step("thisPlugin", Messages.AddItem, Stages.PostOperation, Modes.Asynchronous, "thisEntity");

        _component.AddChild(step);

        Assert.IsTrue(_component.Children.Count() == 1);
    }

    [TestMethod]
    public void CleanChildTest()
    {
        // Assert
        var step = new Step("thisPlugin", Messages.AddItem, Stages.PostOperation, Modes.Asynchronous, "thisEntity")
        {
            RegistrationState = RegistrationState.ToDelete
        };

        _component.Steps.Add(step);

        // Act
        _component.CleanChildrenWithState(RegistrationState.ToDelete);

        // Assert
        Assert.IsFalse(_component.Children.Any());
    }

    [TestMethod]
    public void CleanChildrenWithStateTest_OneLayer()
    {
        // Arrange
        _component.AddChild(
            new Step("thisPlugin", Messages.Delete, Stages.PreOperation, Modes.Asynchronous, "thisEntity")
            {
                UniqueName = "thisStep"
            });

        Assert.IsTrue(_component.Children.Any());

        // Act
        _component.CleanChildrenWithState(RegistrationState.NotComputed);

        // Assert
        Assert.IsFalse(_component.Children.Any());
    }

    [TestMethod]
    public void CleanChildrenWithStateTest_TwoLayers()
    {
        // Arrange
        var step = new Step("thisPlugin", Messages.Retrieve, Stages.PreOperation, Modes.Asynchronous, "thisEntity")
        {
            UniqueName = "thisStep"
        };

        step.AddChild(new StepImage(step.Message, true, step.Stage)
        {
            AllAttributes = true
        });

        _component.AddChild(step);

        Assert.IsTrue(_component.Children.Any());
        Assert.IsTrue(step.Children.Any());

        // Act
        _component.CleanChildrenWithState(RegistrationState.NotComputed);

        // Assert
        Assert.IsFalse(_component.Children.Any());
        Assert.IsFalse(step.Children.Any());
    }
}