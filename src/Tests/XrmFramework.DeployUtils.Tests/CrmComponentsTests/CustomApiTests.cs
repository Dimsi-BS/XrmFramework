using System;
using System.Linq;
using Moq;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests;

[TestClass]
public class CustomApiTests
{
    private const string EntityTypeName = "customapi";
    private readonly CustomApi _component;


    public CustomApiTests()
    {
        _component = new CustomApi();
    }


    [TestMethod]
    public void CrmPropertiesTests()
    {
        Assert.AreEqual(_component.EntityTypeName, EntityTypeName);
        Assert.AreEqual(_component.Rank, 15);
        Assert.AreEqual(_component.DoAddToSolution, true);
        Assert.AreEqual(_component.DoFetchTypeCode, true);
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
        Assert.ThrowsException<ArgumentException>(() => _component.AddChild(anyComponent),
            "CustomApi doesn't take this type of children");
    }

    [TestMethod]
    public void CleanChildrenWithStateTest()
    {
        // Arrange
        _component.AddChild(new CustomApiRequestParameter
        {
            UniqueName = "thisRequest"
        });

        Assert.IsTrue(_component.Children.Any());

        // Act
        _component.CleanChildrenWithState(RegistrationState.NotComputed);

        // Assert
        Assert.IsFalse(_component.Children.Any());
    }
}