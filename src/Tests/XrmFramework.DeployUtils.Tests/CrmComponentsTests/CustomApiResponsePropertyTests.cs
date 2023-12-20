using Microsoft.Xrm.Sdk;
using Moq;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests
{
    [TestClass]
    public class CustomApiResponsePropertyTests
    {
        private readonly CustomApiResponseProperty _component;

        private const string Description = "description";
        private const string DisplayName = "displayName";
        private const bool IsOptional = true;
        private readonly OptionSetValue Type = new OptionSetValue(18);
        private const string Name = "Name";

        private const string EntityTypeName = "customapiresponseproperty";

        public CustomApiResponsePropertyTests()
        {
            _component = new()
            {
                Name = Name,
                Description = Description,
                DisplayName = DisplayName,
                IsOptional = IsOptional,
                Type = Type,
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
            ICrmComponent anyComponent = new Mock<ICrmComponent>().Object;
            Assert.ThrowsException<ArgumentException>(() => _component.AddChild(anyComponent), "CustomApiResponseProperty doesn't take children");
        }

    }
}