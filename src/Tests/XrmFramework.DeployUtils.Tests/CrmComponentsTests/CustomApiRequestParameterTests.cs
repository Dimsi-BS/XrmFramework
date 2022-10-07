using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Moq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests;

[TestClass]
public class CustomApiRequestParameterTests
{
    private const string Description = "description";
    private const string DisplayName = "displayName";
    private const bool IsOptional = true;
    private const string Name = "Name";

    private const string EntityTypeName = "customapirequestparameter";
    private readonly CustomApiRequestParameter _component;
    private readonly OptionSetValue Type = new(18);

    public CustomApiRequestParameterTests()
    {
        _component = new CustomApiRequestParameter
        {
            Name = Name,
            Description = Description,
            DisplayName = DisplayName,
            IsOptional = IsOptional,
            Type = Type
        };
    }

    [TestMethod]
    public void CrmPropertiesTests()
    {
        Assert.AreEqual(_component.EntityTypeName, EntityTypeName);
        Assert.AreEqual(_component.Rank, 20);
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
            "CustomApiRequestParameter doesn't take children");
    }
}