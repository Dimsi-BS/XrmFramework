using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Utils;
using AssemblyInfo = XrmFramework.DeployUtils.Model.AssemblyInfo;

namespace XrmFramework.DeployUtils.Tests
{
    [TestClass]
    public class CrmComponentComparerTests
    {
        private readonly CrmComponentComparer _comparer;

        private readonly StepComparer _stepComparer;

        public CrmComponentComparerTests()
        {
            _stepComparer = new();
            _comparer = new();
        }

        [TestMethod]
        public void Equals_SameComponent_AssemblyInfo()
        {
            // Arrange
            var thisComponent = new AssemblyInfo()
            {
                Name = "thisAssembly",
            };

            var otherComponent = new AssemblyInfo()
            {
                Name = "thisAssembly"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void Equals_DifferentComponent_AssemblyInfo()
        {
            // Arrange
            var thisComponent = new AssemblyInfo()
            {
                Name = "thisAssembly",
            };

            var otherComponent = new AssemblyInfo()
            {
                Name = "otherAssembly"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_SameComponent_AssemblyContext()
        {
            // Arrange
            var thisComponent = new AssemblyContext()
            {
                AssemblyInfo = new AssemblyInfo(),
            };

            var otherComponent = new AssemblyContext()
            {
                AssemblyInfo = new AssemblyInfo()
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentComponent_AssemblyContext()
        {
            // Arrange
            var thisComponent = new AssemblyContext()
            {
                AssemblyInfo = new AssemblyInfo()
                { Name = "thisAssembly" }
            };

            var otherComponent = new AssemblyContext()
            {
                AssemblyInfo = new AssemblyInfo()
                { Name = "otherAssembly" }

            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_SameComponent_Plugin()
        {
            // Arrange
            var thisComponent = new Plugin("thisPlugin");

            var otherComponent = new Plugin("thisPlugin");

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentComponent_Plugin()
        {
            // Arrange
            var thisComponent = new Plugin("thisPlugin");

            var otherComponent = new Plugin("otherPlugin");

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_SameComponent_Step()
        {
            // Arrange
            var thisComponent = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");

            var otherComponent = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentComponent_Step()
        {
            // Arrange
            var thisComponent = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");

            var otherComponent = new Step("otherPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity");

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_SameComponent_StepImage()
        {
            // Arrange
            var thisComponent = new StepImage(Messages.RetrieveMultiple, true, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
            };

            var otherComponent = new StepImage(Messages.RetrieveMultiple, true, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentComponent_StepImage()
        {
            // Arrange
            var thisComponent = new StepImage(Messages.RetrieveMultiple, true, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
            };

            var otherComponent = new StepImage(Messages.Default, true, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_SameComponent_CustomApi()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                UniqueName = "thisCustomApi",
            };

            var otherComponent = new CustomApi()
            {
                UniqueName = "thisCustomApi"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentComponent_CustomApi()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                UniqueName = "thisCustomApi",
            };

            var otherComponent = new CustomApi()
            {
                UniqueName = "thisCustomApi"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_SameComponent_CustomApiRequestParameter()
        {
            // Arrange
            var thisComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter",
            };

            var otherComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_SameComponent_CustomApiResponseProperty()
        {
            // Arrange
            var thisComponent = new CustomApiResponseProperty()
            {
                UniqueName = "thisCustomApiResponseProperty",
            };

            var otherComponent = new CustomApiResponseProperty()
            {
                UniqueName = "thisCustomApiResponseProperty"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_SameComponent_Workflow()
        {
            // Arrange
            var thisComponent = new Plugin("thisWorkflow", "workflow");

            var otherComponent = new Plugin("thisWorkflow", "workflow");

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }
    }
}
