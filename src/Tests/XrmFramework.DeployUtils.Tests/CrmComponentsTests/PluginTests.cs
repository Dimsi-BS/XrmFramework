using Moq;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests
{
    [TestClass]
    public class PluginTests
    {
        private readonly Plugin _component;

        private const string EntityTypeName = "plugintype";

        public PluginTests()
        {
            _component = new("thisPlugin");
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
            ICrmComponent anyComponent = new Mock<ICrmComponent>().Object;
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

    }
}