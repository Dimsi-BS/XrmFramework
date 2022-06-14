using Microsoft.Xrm.Sdk;
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

        public CrmComponentComparerTests()
        {
            _comparer = new();
        }

        #region Equals Tests
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

            var otherComponent = new StepImage(Messages.Default, false, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_SameComponent_CustomApi()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                Name = "thisCustomApi",
            };

            var otherComponent = new CustomApi()
            {
                Name = "thisCustomApi"
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
                Name = "thisCustomApi",
            };

            var otherComponent = new CustomApi()
            {
                Name = "otherCustomApi"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_SameComponent_CustomApiRequestParameter()
        {
            // Arrange
            var thisComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter",
                Type = new OptionSetValue(2)
            };

            var otherComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter",
                Type = new OptionSetValue(2)
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentComponent_CustomApiRequestParameter()
        {
            // Arrange
            var thisComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter",
            };

            var otherComponent = new CustomApiRequestParameter()
            {
                UniqueName = "otherCustomApiRequestParameter"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_SameComponent_CustomApiResponseProperty()
        {
            // Arrange
            var thisComponent = new CustomApiResponseProperty()
            {
                UniqueName = "thisCustomApiResponseProperty",
                Type = new OptionSetValue(2)
            };

            var otherComponent = new CustomApiResponseProperty()
            {
                UniqueName = "thisCustomApiResponseProperty",
                Type = new OptionSetValue(2)
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentComponent_CustomApiResponseProperty()
        {
            // Arrange
            var thisComponent = new CustomApiResponseProperty()
            {
                UniqueName = "thisCustomApiResponseProperty",
            };

            var otherComponent = new CustomApiResponseProperty()
            {
                UniqueName = "otherCustomApiResponseProperty"
            };

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
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

        [TestMethod]
        public void Equals_DifferentComponent_Workflow()
        {
            // Arrange
            var thisComponent = new Plugin("thisWorkflow", "workflow");

            var otherComponent = new Plugin("otherWorkflow", "workflow");

            // Act

            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_DifferentTypes()
        {
            // Arrange
            var thisComponent = new Plugin("thisPlugin");

            var otherComponent = new CustomApi();

            // Act
            var result = _comparer.Equals(thisComponent, otherComponent);

            // Assert
            Assert.IsFalse(result);
        }
        #endregion

        #region NeedsUpdate Tests

        [TestMethod]
        public void NeedsUpdate_SameComponent_AssemblyInfo()
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

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_AssemblyContext()
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

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_Plugin()
        {
            // Arrange
            var thisComponent = new Plugin("thisPlugin");

            var otherComponent = new Plugin("thisPlugin");

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_Step()
        {
            // Arrange
            var thisComponent = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
            {
                PluginTypeFullName = "assembly.thisPlugin",
                FilteringAttributes = { "thisAttribute" },
                ImpersonationUsername = "thisUser",
                Order = 1
            };
            var otherComponent = new Step("otherPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity")
            {
                PluginTypeFullName = "assembly.otherPlugin",
                FilteringAttributes = { "thisAttribute" },
                ImpersonationUsername = "thisUser",
                Order = 1
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_StepImage()
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

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_StepImage_DifferentAllAttributes()
        {
            // Arrange
            var thisComponent = new StepImage(Messages.RetrieveMultiple, true, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity"),
                AllAttributes = true
            };

            var otherComponent = new StepImage(Messages.RetrieveMultiple, true, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity"),
                AllAttributes = false
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_StepImage_DifferentAttributes()
        {
            // Arrange
            var thisComponent = new StepImage(Messages.RetrieveMultiple, true, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity"),
                Attributes = { "thisAttribute" }
            };

            var otherComponent = new StepImage(Messages.RetrieveMultiple, true, Stages.PostOperation)
            {
                FatherStep = new Step("thisPlugin", Messages.Create, Stages.PostOperation, Modes.Synchronous, "entity"),
                Attributes = { "otherAttribute", "otherAttribute2" }
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_CustomApi()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            var otherComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_CustomApi_DifferentBindingType()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            var otherComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(10),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_CustomApi_Different_BoundEntity()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            var otherComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "otherEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_CustomApi_DifferentIsFunction()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            var otherComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = false,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_CustomApi_Different_Workflow()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            var otherComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = false,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_CustomApi_DifferentAllowedCustom()
        {
            // Arrange
            var thisComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(1)
            };

            var otherComponent = new CustomApi()
            {
                Name = "thisCustomApi",
                BindingType = new OptionSetValue(0),
                BoundEntityLogicalName = "thisEntity",
                IsFunction = true,
                WorkflowSdkStepEnabled = true,
                AllowedCustomProcessingStepType = new OptionSetValue(3)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_CustomApiRequestParameter()
        {
            // Arrange
            var thisComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter",
                IsOptional = true,
                Type = new OptionSetValue(42)
            };

            var otherComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter",
                IsOptional = true,
                Type = new OptionSetValue(42)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NeedsUpdate_SameComponent_CustomApiRequestParameter_DifferentIsOptional()
        {
            // Arrange
            var thisComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter",
                IsOptional = true,
                Type = new OptionSetValue(42)
            };

            var otherComponent = new CustomApiRequestParameter()
            {
                UniqueName = "thisCustomApiRequestParameter",
                IsOptional = false,
                Type = new OptionSetValue(42)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NeedsUpdate_CustomApiResponseProperty_Same()
        {
            // Arrange
            var thisComponent = new CustomApiResponseProperty()
            {
                UniqueName = "thisCustomApiRequestParameter",
                Type = new OptionSetValue(42)
            };

            var otherComponent = new CustomApiResponseProperty()
            {
                UniqueName = "otherCustomApiRequestParameter",
                Type = new OptionSetValue(42)
            };

            // Act

            var result = _comparer.NeedsUpdate(thisComponent, otherComponent);

            // Assert

            Assert.IsFalse(result);
        }

        #endregion
    }
}
