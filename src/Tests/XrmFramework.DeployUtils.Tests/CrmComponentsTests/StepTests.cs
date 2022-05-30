using Moq;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests
{
    [TestClass]
    public class StepTests
    {
        private readonly Step _component;

        private const string EntityTypeName = "sdkmessageprocessingstep";


        public StepTests()
        {
            _component = new("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Synchronous, "thisEntity");
        }


        [TestMethod]
        public void CrmPropertiesTests()
        {
            Assert.AreEqual(_component.EntityTypeName, EntityTypeName);
            Assert.AreEqual(_component.Rank, 2);
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
            ICrmComponent anyComponent = new Mock<ICrmComponent>().Object;
            Assert.ThrowsException<ArgumentException>(() => _component.AddChild(anyComponent));
        }

        [TestMethod]
        public void MergeTest_Same()
        {
            // Arrange
            var thisStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                    "thisEntity")
            { DoNotFilterAttributes = true };

            var otherStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                    "thisEntity")
            { DoNotFilterAttributes = true };

            var otherStepbis = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                    "thisEntity")
            { DoNotFilterAttributes = true, };

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
            { DoNotFilterAttributes = false };
            thisStep.FilteringAttributes.Add("thisAttribute");

            var otherStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                    "thisEntity")
            { DoNotFilterAttributes = true };

            var otherStepbis = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                    "thisEntity")
            { DoNotFilterAttributes = true };

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
            { DoNotFilterAttributes = false };
            thisStep.FilteringAttributes.Add("thisAttribute");

            var otherStep = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                    "thisEntity")
            { DoNotFilterAttributes = false };
            otherStep.FilteringAttributes.Add("otherAttribute");

            var otherStepbis = new Step("thisPlugin", Messages.Create, Stages.PreOperation, Modes.Asynchronous,
                    "thisEntity")
            { DoNotFilterAttributes = false };
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
}