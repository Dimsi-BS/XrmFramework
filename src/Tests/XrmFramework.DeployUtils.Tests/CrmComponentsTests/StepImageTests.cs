using Moq;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests
{
    [TestClass]
    public class StepImageTests
    {
        private readonly StepImage _component;

        private const string EntityTypeName = "sdkmessageprocessingstepimage";
        private const string Name = "ISolutionComponent Implementation";


        public StepImageTests()
        {
            _component = new(Messages.Create, true, Stages.PostOperation);
        }


        [TestMethod]
        public void CrmPropertiesTests()
        {
            Assert.AreEqual(_component.EntityTypeName, EntityTypeName);
            Assert.AreEqual(_component.UniqueName, Name);
            Assert.AreEqual(_component.Rank, 3);
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
            Action<ICrmComponent> addChild = _component.AddChild;
            Assert.ThrowsException<ArgumentException>(() => _component.AddChild(anyComponent), "StepImage doesn't take children");
        }

        [TestMethod]
        public void MergeTest_SameAttributes_AllAttributesIstrue()
        {
            // Arrange
            var thisSI = new StepImage(Messages.Create, true, Stages.PostOperation)
            {
                AllAttributes = true
            };

            var otherSI = new StepImage(Messages.Create, true, Stages.PostOperation)
            {
                AllAttributes = true
            };

            // Act
            thisSI.Merge(otherSI);

            // Assert
            Assert.IsTrue(thisSI.AllAttributes);
            Assert.IsFalse(thisSI.Attributes.Any());
        }

        [TestMethod]
        public void MergeTest_FromAttributes_TargetAllAttributes()
        {
            // Arrange
            var thisSI = new StepImage(Messages.Create, true, Stages.PostOperation);
            thisSI.Attributes.Add("thisAttribute");

            var otherSI = new StepImage(Messages.Create, true, Stages.PostOperation)
            {
                AllAttributes = true
            };

            var otherSIbis = new StepImage(Messages.Create, true, Stages.PostOperation)
            {
                AllAttributes = true
            };

            // Act
            otherSIbis.Merge(otherSI);
            thisSI.Merge(otherSI);

            // Assert
            Assert.IsTrue(thisSI.AllAttributes);
            Assert.IsFalse(thisSI.Attributes.Any());

            Assert.AreEqual(thisSI.AllAttributes, otherSIbis.AllAttributes);
            Assert.IsTrue(thisSI.Attributes.SetEquals(otherSIbis.Attributes));
        }

        [TestMethod]
        public void MergeTest_FromAttributes_TargetAttributes()
        {
            // Arrange
            var thisSI = new StepImage(Messages.Create, true, Stages.PostOperation);
            thisSI.Attributes.Add("thisAttribute");

            var otherSI = new StepImage(Messages.Create, true, Stages.PostOperation);
            otherSI.Attributes.Add("thisAttribute");

            var otherSIbis = new StepImage(Messages.Create, true, Stages.PostOperation);
            otherSIbis.Attributes.Add("thisAttribute");

            // Act
            otherSIbis.Merge(thisSI);
            thisSI.Merge(otherSI);

            // Assert
            Assert.IsFalse(thisSI.AllAttributes);
            Assert.IsTrue(thisSI.Attributes.Count == 1);

            Assert.AreEqual(thisSI.AllAttributes, otherSIbis.AllAttributes);
            Assert.IsTrue(thisSI.Attributes.SetEquals(otherSIbis.Attributes));
        }

        [TestMethod]
        public void MergeTest_FromAttributes_TargetOtherAttributes()
        {
            // Arrange
            var thisSI = new StepImage(Messages.Create, true, Stages.PostOperation);
            thisSI.Attributes.Add("thisAttribute");

            var otherSI = new StepImage(Messages.Create, true, Stages.PostOperation);
            otherSI.Attributes.Add("otherAttribute");

            var otherSIbis = new StepImage(Messages.Create, true, Stages.PostOperation);
            otherSIbis.Attributes.Add("otherAttribute");

            // Act
            otherSIbis.Merge(thisSI);
            thisSI.Merge(otherSI);

            // Assert
            //      GoodMerge
            Assert.IsFalse(thisSI.AllAttributes);
            Assert.IsTrue(thisSI.Attributes.Count == 2);

            //      Symmetry
            Assert.AreEqual(thisSI.AllAttributes, otherSIbis.AllAttributes);
            Assert.IsTrue(thisSI.Attributes.SetEquals(otherSIbis.Attributes));
        }
    }
}
