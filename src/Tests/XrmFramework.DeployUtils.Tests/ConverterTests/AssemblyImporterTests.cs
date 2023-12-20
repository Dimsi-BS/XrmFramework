using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Deploy;
using Microsoft.Xrm.Sdk;
using Moq;
using Newtonsoft.Json;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Importers;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Tests.ConverterTests;

[TestClass]
public class AssemblyImporterTests
{
    private readonly AssemblyImporter _importer;

    public AssemblyImporterTests()
    {
        var mockMapper = new Mock<IMapper>();
        Mock<ISolutionContext> mockContext = new();

        _importer = new AssemblyImporter(mockContext.Object, mockMapper.Object);
    }

    [TestMethod]
    public void CreateStepImageFromRemoteTest()
    {
        // Arrange
        var isPreImage = true;

        var imageType = sdkmessageprocessingstepimage_imagetype.PreImage;
        var stepId = Guid.NewGuid();
        var stepImageId = Guid.NewGuid();
        var attribute = "thisAttribute";

        var sdkStepImage = new SdkMessageProcessingStepImage
        {
            ImageTypeEnum = imageType,
            SdkMessageProcessingStepId = new EntityReference(SdkMessageProcessingStepDefinition.EntityName, stepId),
            Id = stepImageId,
            Attributes1 = attribute
        };

        var sdkStepImageList = new List<SdkMessageProcessingStepImage> {sdkStepImage};

        var step = new Step("thisPlugin", Messages.Merge, Stages.PostOperation, Modes.Asynchronous, "thisEntity")
        {
            Id = stepId
        };

        // Act
        _importer.CreateStepImageFromRemote(step, isPreImage, sdkStepImageList);

        // Arrange
        Assert.AreEqual(stepImageId, step.PreImage.Id);
        Assert.AreEqual(stepId, step.PreImage.ParentId);
        Assert.AreEqual(false, step.PreImage.AllAttributes);
        Assert.IsTrue(step.PreImage.Attributes.Count == 1);
        Assert.IsTrue(step.PreImage.Attributes.Contains(attribute));
    }

    [TestMethod]
    public void CreateStepFromRemoteTest()
    {
        // Arrange
        var pluginTypeFullName = "thisAssembly.thisPlugin";
        var pluginTypeName = "thisPlugin";
        var message = Messages.Update;
        var stage = Stages.PreOperation;
        var mode = Modes.Asynchronous;
        var entityName = "thisEntity";

        var pluginId = Guid.NewGuid();
        var stepId = Guid.NewGuid();

        var stepConfig = new StepConfiguration
        {
            AssemblyName = "thisAssembly",
            AssemblyQualifiedName = "thisAssemblyQualifiedName",
            BannedMethods = new List<string> {"thisMethod"},
            DebugSessionId = Guid.NewGuid(),
            PluginName = pluginTypeName,
            RegisteredMethods = new HashSet<string> {"thisMethod"},
            RelationshipName = "thisRelationshipName"
        };
        var unsecureConfig = JsonConvert.SerializeObject(stepConfig);

        var filteringAttribute = "thisAttribute";

        var order = 1;

        var impersonationUserName = "thisUser";
        var userId = Guid.NewGuid();

        var sdkStep = new SdkMessageProcessingStep
        {
            Id = stepId,
            EventHandler = new EntityReference("someEventHandler", "blabla", null)
            {
                Id = pluginId,
                Name = pluginTypeFullName
            },
            SdkMessageId = new EntityReference("someMessage", message.ToString(), "blabla"),
            StageEnum = sdkmessageprocessingstep_stage.PreOperation,
            ModeEnum = sdkmessageprocessingstep_mode.Asynchronous,
            FilteringAttributes = filteringAttribute,
            ImpersonatingUserId = new EntityReference("blabla", "", null) {Name = impersonationUserName},
            Rank = order,
            Configuration = unsecureConfig
        };

        // Act
        _importer.TryCreateStepFromRemote(sdkStep, new List<SdkMessageProcessingStepImage>(), out var step);

        // Arrange
        Assert.AreEqual(stepId, step.Id);
        Assert.AreEqual(pluginId, step.ParentId);
        Assert.AreEqual(pluginTypeName, step.PluginTypeName);
        Assert.AreEqual(pluginTypeFullName, step.PluginTypeFullName);
        Assert.AreEqual(impersonationUserName, step.ImpersonationUsername);
        Assert.AreEqual(stage, step.Stage);
        Assert.AreEqual(mode, step.Mode);
        Assert.AreEqual(1, step.FilteringAttributes.Count);
        Assert.IsTrue(step.FilteringAttributes.Contains(filteringAttribute));
        Assert.AreEqual(order, step.Order);
        Assert.AreEqual(stepConfig.AssemblyName, step.StepConfiguration.AssemblyName);
        Assert.AreEqual(stepConfig.AssemblyQualifiedName, step.StepConfiguration.AssemblyQualifiedName);
        Assert.IsTrue(step.StepConfiguration.BannedMethods.SequenceEqual(stepConfig.BannedMethods));
        Assert.AreEqual(stepConfig.DebugSessionId, step.StepConfiguration.DebugSessionId);
        Assert.AreEqual(stepConfig.PluginName, step.StepConfiguration.PluginName);
        Assert.AreEqual(stepConfig.RelationshipName, step.StepConfiguration.RelationshipName);

        Assert.IsTrue(step.StepConfiguration.RegisteredMethods.SetEquals(stepConfig.RegisteredMethods));
    }

    [TestMethod]
    public void CreatePluginFromRemoteTest()
    {
        // Arrange
        var pluginName = "thisPlugin";
        var id = Guid.NewGuid();
        var assemblyId = Guid.NewGuid();

        var sdkPlugin = new PluginType
        {
            TypeName = pluginName,
            Name = pluginName,
            Id = id,
            PluginAssemblyId = new EntityReference("", assemblyId)
        };

        // Act
        var result = _importer.CreatePluginFromRemote(sdkPlugin, new List<Step>());

        // Arrange
        Assert.AreEqual(pluginName, result.FullName);
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual(assemblyId, result.ParentId);
        Assert.IsFalse(result.IsWorkflow);
    }

    [TestMethod]
    public void CreateWorkflowFromRemoteTest()
    {
        // Arrange
        var pluginName = "thisPlugin";
        var name = "thisName";
        var id = Guid.NewGuid();
        var assemblyId = Guid.NewGuid();

        var sdkPlugin = new PluginType
        {
            TypeName = pluginName,
            Name = name,
            WorkflowActivityGroupName = "thisGroup",
            Id = id,
            PluginAssemblyId = new EntityReference("", assemblyId)
        };

        // Act
        var result = _importer.CreatePluginFromRemote(sdkPlugin, new List<Step>());

        // Arrange
        Assert.AreEqual(pluginName, result.FullName);
        Assert.AreEqual(name, result.DisplayName);
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual(assemblyId, result.ParentId);
        Assert.IsTrue(result.IsWorkflow);
    }
}