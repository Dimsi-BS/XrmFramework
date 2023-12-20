using System;
using System.Linq;
using Moq;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests;

[TestClass]
public class StepTests
{
    private const string EntityTypeName = "sdkmessageprocessingstep";
    private readonly Step _component;


    public StepTests()
    {
        _component = new Step("thisPlugin", Messages.Update, Stages.PreOperation, Modes.Synchronous, "thisEntity");
    }


    [TestMethod]
    public void CrmPropertiesTests()
    {
        Assert.AreEqual(_component.EntityTypeName, EntityTypeName);
        Assert.AreEqual(_component.Rank, 20);
        Assert.AreEqual(_component.DoAddToSolution, true);
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
    public void RemoveChildrenWithStateTest()
    {
        // Arrange
        _component.PreImage.AllAttributes = true;
        _component.PreImage.RegistrationState = RegistrationState.Ignore;

        Assert.IsTrue(_component.Children.Any());

        // Act
        _component.CleanChildrenWithState(RegistrationState.Ignore);


        // Assert
        Assert.IsFalse(_component.Children.Any());
    }


    [TestMethod]
    public void MergeTest_Same()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = true};

        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = true};

        var otherStepbis = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = true};

        thisStep.StepConfiguration.RegisteredMethods.Add("thisMethod");
        otherStep.StepConfiguration.RegisteredMethods.Add("thisMethod");
        otherStepbis.StepConfiguration.RegisteredMethods.Add("thisMethod");

        // Act
        otherStep.Merge(thisStep);
        thisStep.Merge(otherStepbis);

        // Assert
        //      GoodMerge
        Assert.IsTrue(thisStep.DoNotFilterAttributes);
        Assert.IsFalse(thisStep.FilteringAttributes.Any());
        Assert.IsTrue(thisStep.MethodNames.Count == 1);

        //      Symmetry
        Assert.AreEqual(thisStep.DoNotFilterAttributes, otherStep.DoNotFilterAttributes);
        Assert.IsTrue(thisStep.FilteringAttributes.SetEquals(otherStep.FilteringAttributes));
        Assert.IsTrue(thisStep.MethodNames.SetEquals(otherStep.MethodNames));
    }

    [TestMethod]
    public void MergeTest_DoNotFilterAttributes()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = false};
        thisStep.FilteringAttributes.Add("thisAttribute");

        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = true};

        var otherStepbis = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = true};

        // Act
        otherStep.Merge(thisStep);
        thisStep.Merge(otherStepbis);

        // Assert
        //      GoodMerge
        Assert.IsTrue(thisStep.DoNotFilterAttributes);
        Assert.IsFalse(thisStep.FilteringAttributes.Any());

        //      Symmetry
        Assert.AreEqual(thisStep.DoNotFilterAttributes, otherStep.DoNotFilterAttributes);
        Assert.IsTrue(thisStep.FilteringAttributes.SetEquals(otherStep.FilteringAttributes));
    }

    [TestMethod]
    public void MergeTest_FilteringAttributes()
    {
        // Arrange
        var thisStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = false};
        thisStep.FilteringAttributes.Add("thisAttribute");

        var otherStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = false};
        otherStep.FilteringAttributes.Add("otherAttribute");

        var otherStepbis = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                "thisEntity")
            {DoNotFilterAttributes = false};
        otherStepbis.FilteringAttributes.Add("otherAttribute");

        // Act
        otherStep.Merge(thisStep);
        thisStep.Merge(otherStepbis);

        // Assert
        //      GoodMerge
        Assert.IsFalse(thisStep.DoNotFilterAttributes);
        Assert.IsTrue(thisStep.FilteringAttributes.Count == 2);

        //      Symmetry
        Assert.AreEqual(thisStep.DoNotFilterAttributes, otherStep.DoNotFilterAttributes);
        Assert.IsTrue(thisStep.FilteringAttributes.SetEquals(otherStep.FilteringAttributes));
    }
}