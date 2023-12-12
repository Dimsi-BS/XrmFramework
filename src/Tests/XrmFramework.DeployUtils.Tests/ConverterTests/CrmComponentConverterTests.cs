using AutoMapper;
using Deploy;
using Microsoft.Xrm.Sdk;
using Moq;
using Newtonsoft.Json;
using System;
using XrmFramework;
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

            Assert.AreEqual(assemblyId, result.PluginAssemblyId.Id);
            Assert.AreEqual(PluginAssemblyDefinition.EntityName, result.PluginAssemblyId.LogicalName);
            Assert.AreEqual(pluginName, result.TypeName);
            Assert.AreEqual(pluginName, result.FriendlyName);
            Assert.AreEqual(pluginName, result.Name);
            Assert.AreEqual(pluginName, result.Description);
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

            Assert.AreEqual(assemblyId, result.PluginAssemblyId.Id);
            Assert.AreEqual(PluginAssemblyDefinition.EntityName, result.PluginAssemblyId.LogicalName);
            Assert.AreEqual(workflowName, result.TypeName);
            Assert.AreEqual(workflowName, result.FriendlyName);
            Assert.AreEqual(workflowDisplayName, result.Name);
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
            Assert.AreEqual(step.Description, result.Name);
            Assert.AreEqual(stepId, result.Id);
            Assert.AreEqual(filteringAttribute, result.FilteringAttributes);
            Assert.AreEqual(true, result.AsyncAutoDelete);
            Assert.AreEqual(pluginId, result.EventHandler.Id);
            Assert.AreEqual(userId, result.ImpersonatingUserId.Id);
            Assert.AreEqual(true, result.IsCustomizable.Value);
            Assert.AreEqual(false, result.IsHidden.Value);
            Assert.AreEqual((int)mode, result.Mode.Value);
            Assert.AreEqual(order, result.Rank);
            Assert.AreEqual(messageId, result.SdkMessageId.Id);
            Assert.AreEqual(messageFilterId, result.SdkMessageFilterId.Id);
            Assert.AreEqual((int)stage, result.Stage.Value);
            Assert.AreEqual((int)Deploy.sdkmessageprocessingstep_supporteddeployment.ServerOnly, result.SupportedDeployment.Value);
            Assert.AreEqual(JsonConvert.SerializeObject(stepConfig), result.Configuration);

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
            Assert.AreEqual("PreImage", result.Name);
            Assert.AreEqual(stepImageId, result.Id);
            Assert.AreEqual(attribute, result.Attributes1);
            Assert.AreEqual("PreImage", result.EntityAlias);
            Assert.AreEqual((int)Deploy.sdkmessageprocessingstepimage_imagetype.PreImage, result.ImageType.Value);
            Assert.AreEqual(true, result.IsCustomizable.Value);
            Assert.AreEqual(stepId, result.SdkMessageProcessingStepId.Id);
        }
    }
}
