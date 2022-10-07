using AutoMapper;
using Deploy;
using Microsoft.Xrm.Sdk;
using Moq;
using Newtonsoft.Json;
using System;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Tests.ConverterTests
{
    [TestClass]
    public class CrmComponentConverterTests
    {
        private readonly CrmComponentConverter _converter;

        private readonly Mock<ISolutionContext> _mockSolution;
        private readonly Mock<IMapper> _mockMapper;

        public CrmComponentConverterTests()
        {
            _mockMapper = new();
            _mockSolution = new();

            _converter = new(_mockSolution.Object, _mockMapper.Object);
        }

        [TestMethod]
        public void PluginConvertTest()
        {
            // Arrange
            var pluginName = "thisPlugin";
            var id = Guid.NewGuid();
            var assemblyId = Guid.NewGuid();

            var plugin = new Plugin(pluginName)
            {
                ParentId = assemblyId,
                Id = id
            };

            // Act
            var result = (Deploy.PluginType)_converter.ToRegisterComponent(plugin);

            // Assert

            Assert.AreEqual(result.PluginAssemblyId.Id, assemblyId);
            Assert.AreEqual(result.PluginAssemblyId.LogicalName, PluginAssemblyDefinition.EntityName);
            Assert.AreEqual(result.TypeName, pluginName);
            Assert.AreEqual(result.FriendlyName, pluginName);
            Assert.AreEqual(result.Name, pluginName);
            Assert.AreEqual(result.Description, pluginName);
        }

        [TestMethod]
        public void WorkflowConvertTest()
        {
            // Arrange
            var workflowName = "thisWorkflow";
            var workflowDisplayName = "thisWorkflowDisplay";
            var id = Guid.NewGuid();
            var assemblyId = Guid.NewGuid();

            var plugin = new Plugin(workflowName, workflowDisplayName)
            {
                ParentId = assemblyId,
                Id = id
            };

            // Act
            var result = (Deploy.PluginType)_converter.ToRegisterComponent(plugin);

            // Assert

            Assert.AreEqual(result.PluginAssemblyId.Id, assemblyId);
            Assert.AreEqual(result.PluginAssemblyId.LogicalName, PluginAssemblyDefinition.EntityName);
            Assert.AreEqual(result.TypeName, workflowName);
            Assert.AreEqual(result.FriendlyName, workflowName);
            Assert.AreEqual(result.Name, workflowDisplayName);
        }

        [TestMethod]
        public void StepConvertTest()
        {
            // Arrange
            var pluginTypeName = "thisPlugin";
            var message = Messages.AddMembers;
            var stage = Stages.PreOperation;
            var mode = Modes.Asynchronous;
            var entityName = "thisEntity";

            var pluginId = Guid.NewGuid();
            var stepId = Guid.NewGuid();

            var stepConfig = new StepConfiguration()
            {
                AssemblyName = "thisAssembly",
                AssemblyQualifiedName = "thisAssemblyQualifiedName",
                BannedMethods = new() { "thisMethod" },
                DebugSessionId = Guid.NewGuid(),
                PluginName = pluginTypeName,
                RegisteredMethods = new() { "thisMethod" },
                RelationshipName = "thisRelationshipName"
            };

            var filteringAttribute = "thisAttribute";

            var order = 1;

            var impersonationUserName = "thisUser";
            var userId = Guid.NewGuid();



            var step = new Step(pluginTypeName, message, stage, mode, entityName)
            {
                ParentId = pluginId,
                Id = stepId,
                ImpersonationUsername = impersonationUserName,
                Order = order,
                StepConfiguration = stepConfig
            };
            step.FilteringAttributes.Add(filteringAttribute);

            _mockSolution.Setup(m => m.GetUserId(It.IsAny<string>()))
                .Returns(userId);

            var messageId = Guid.NewGuid();
            _mockSolution.Setup(m => m.GetMessage(It.IsAny<Messages>()))
                    .Returns(new EntityReference(SdkMessageDefinition.EntityName, messageId));

            var messageFilterId = Guid.NewGuid();
            _mockSolution.Setup(m => m.GetMessageFilter(It.IsAny<Messages>(), It.IsAny<string>()))
                    .Returns(new EntityReference(SdkMessageFilterDefinition.EntityName, messageFilterId));

            // Act
            var result = (Deploy.SdkMessageProcessingStep)_converter.ToRegisterComponent(step);

            // Assert
            Assert.AreEqual(result.Name, step.Description);
            Assert.AreEqual(result.Id, stepId);
            Assert.AreEqual(result.FilteringAttributes, filteringAttribute);
            Assert.AreEqual(result.AsyncAutoDelete, true);
            Assert.AreEqual(result.EventHandler.Id, pluginId);
            Assert.AreEqual(result.ImpersonatingUserId.Id, userId);
            Assert.AreEqual(result.IsCustomizable.Value, true);
            Assert.AreEqual(result.IsHidden.Value, false);
            Assert.AreEqual(result.Mode.Value, (int)mode);
            Assert.AreEqual(result.Rank, order);
            Assert.AreEqual(result.SdkMessageId.Id, messageId);
            Assert.AreEqual(result.SdkMessageFilterId.Id, messageFilterId);
            Assert.AreEqual(result.Stage.Value, (int)stage);
            Assert.AreEqual(result.SupportedDeployment.Value,
                (int)Deploy.sdkmessageprocessingstep_supporteddeployment.ServerOnly);
            Assert.AreEqual(result.Configuration, JsonConvert.SerializeObject(stepConfig));

        }

        [TestMethod]
        public void StepImageTest()
        {
            // Arrange
            var stepId = Guid.NewGuid();
            var stepImageId = Guid.NewGuid();

            var preImage = true;
            var message = Messages.Update;
            var stage = Stages.PreOperation;
            var attribute = "thisAttribute";
            var allAttributes = false;

            var stepImage = new StepImage(message, preImage, stage)
            {
                Id = stepImageId,
                ParentId = stepId,
                Attributes = { attribute },
                AllAttributes = allAttributes
            };

            // Act
            var result = (SdkMessageProcessingStepImage)_converter.ToRegisterComponent(stepImage);

            // Assert
            Assert.AreEqual(result.Name, "PreImage");
            Assert.AreEqual(result.Id, stepImageId);
            Assert.AreEqual(result.Attributes1, attribute);
            Assert.AreEqual(result.EntityAlias, "PreImage");
            Assert.AreEqual(result.ImageType.Value, (int)Deploy.sdkmessageprocessingstepimage_imagetype.PreImage);
            Assert.AreEqual(result.IsCustomizable.Value, true);
            Assert.AreEqual(result.SdkMessageProcessingStepId.Id, stepId);
        }
    }
}
