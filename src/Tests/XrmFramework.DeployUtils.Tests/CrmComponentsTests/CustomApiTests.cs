using Moq;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests
{
    [TestClass]
    public class CustomApiTests
    {
        private readonly CustomApi _component;

        private const string EntityTypeName = "customapi";


        public CustomApiTests()
        {
            _component = new();
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
            ICrmComponent anyComponent = new Mock<ICrmComponent>().Object;
            Assert.ThrowsException<ArgumentException>(() => _component.AddChild(anyComponent), "CustomApi doesn't take this type of children");
        }
    }
}