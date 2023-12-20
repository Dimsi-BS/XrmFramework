using System;
using System.Linq;
using AutoMapper;
using Microsoft.Xrm.Sdk;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.ConverterTests;

[TestClass]
public class MapperTests
{
	private readonly IMapper _mapper;

	public MapperTests()
	{
		var configuration = new MapperConfiguration(cfg =>
			cfg.AddProfile<AutoMapperLocalToLocalProfile>());
		_mapper = configuration.CreateMapper();
	}

	[TestMethod]
	public void AssemblyInfoToAssemblyContextMapTest()
	{
		// Arrange
		var name = "thisAssembly";
		var culture = "culture";
		var publicKeyToken = "token";
		var version = "version";
		var description = "description";

		var entityTypeName = "pluginassembly";
		var content = "content"u8.ToArray();
		var isolationMode = ModeDIsolation.Sandbox;
		var sourceType = TypeDeSource.Database;

		var assemblyInfo = new AssemblyInfo
		{
			Name = name,
			Version = version,
			SourceType = sourceType,
			IsolationMode = isolationMode,
			Culture = culture,
			PublicKeyToken = publicKeyToken,
			Description = description,
			Content = content
		};

		// Act

		var assemblyCopy = _mapper.Map<IAssemblyContext>(assemblyInfo);

		// Assert

		Assert.IsNotNull(assemblyCopy.AssemblyInfo);
		Assert.AreEqual(name, assemblyCopy.AssemblyInfo.Name);
		Assert.AreEqual(version, assemblyCopy.AssemblyInfo.Version);
		Assert.AreEqual(sourceType, assemblyCopy.AssemblyInfo.SourceType);
		Assert.AreEqual(isolationMode, assemblyCopy.AssemblyInfo.IsolationMode);
		Assert.AreEqual(culture, assemblyCopy.AssemblyInfo.Culture);
		Assert.AreEqual(publicKeyToken, assemblyCopy.AssemblyInfo.PublicKeyToken);
		Assert.AreEqual(description, assemblyCopy.AssemblyInfo.Description);
		Assert.IsTrue(assemblyCopy.AssemblyInfo.Content.SequenceEqual(content));

		Assert.AreEqual(entityTypeName, assemblyCopy.AssemblyInfo.EntityTypeName);
		Assert.AreEqual(name, assemblyCopy.AssemblyInfo.UniqueName);
		Assert.AreEqual(0, assemblyCopy.AssemblyInfo.Rank);
		Assert.IsTrue(assemblyCopy.AssemblyInfo.DoAddToSolution);
		Assert.IsFalse(assemblyCopy.AssemblyInfo.DoFetchTypeCode);
	}

	[TestMethod]
	public void AssemblyInfoMapTest()
	{
		// Arrange
		var name = "thisAssembly";
		var culture = "culture";
		var publicKeyToken = "token";
		var version = "version";
		var description = "description";

		var entityTypeName = "pluginassembly";
		var content = "content"u8.ToArray();
		var isolationMode = ModeDIsolation.Sandbox;
		var sourceType = TypeDeSource.Database;

		var assemblyInfo = new AssemblyInfo
		{
			Name = name,
			Version = version,
			SourceType = sourceType,
			IsolationMode = isolationMode,
			Culture = culture,
			PublicKeyToken = publicKeyToken,
			Description = description,
			Content = content
		};

		// Act

		var assemblyCopy = _mapper.Map<AssemblyInfo>(assemblyInfo);

		// Assert

		Assert.AreEqual(name, assemblyCopy.Name);
		Assert.AreEqual(version, assemblyCopy.Version);
		Assert.AreEqual(sourceType, assemblyCopy.SourceType);
		Assert.AreEqual(isolationMode, assemblyCopy.IsolationMode);
		Assert.AreEqual(culture, assemblyCopy.Culture);
		Assert.AreEqual(publicKeyToken, assemblyCopy.PublicKeyToken);
		Assert.AreEqual(description, assemblyCopy.Description);
		Assert.IsTrue(assemblyCopy.Content.SequenceEqual(content));

		Assert.AreEqual(entityTypeName, assemblyCopy.EntityTypeName);
		Assert.AreEqual(name, assemblyCopy.UniqueName);
		Assert.AreEqual(0, assemblyCopy.Rank);
		Assert.IsTrue(assemblyCopy.DoAddToSolution);
		Assert.IsFalse(assemblyCopy.DoFetchTypeCode);
	}

	[TestMethod]
	public void CustomApiMapTest()
	{
		// Arrange
		var customApi = new CustomApi
		{
			BindingType = new OptionSetValue(2),
			AllowedCustomProcessingStepType = new OptionSetValue(3),
			BoundEntityLogicalName = "thisEntity",
			Description = "thisDescription",
			DisplayName = "thisName",
			ExecutePrivilegeName = "thisPrivilege",
			FullName = "thisFullName",
			IsFunction = true,
			Name = "thisName",
			Prefix = "thisPrefix",
			IsPrivate = false,
			WorkflowSdkStepEnabled = true,
			RegistrationState = RegistrationState.ToCreate
		};

		customApi.AddChild(new CustomApiRequestParameter());
		customApi.AddChild(new CustomApiResponseProperty());

		// Act
		var copy = _mapper.Map<CustomApi>(customApi);

		// Assert
		Assert.AreEqual(customApi.Id, copy.Id);
		Assert.AreEqual(customApi.ParentId, copy.ParentId);
		Assert.AreEqual(customApi.AssemblyId, copy.AssemblyId);

		Assert.AreEqual(customApi.BindingType, copy.BindingType);
		Assert.AreEqual(customApi.AllowedCustomProcessingStepType, copy.AllowedCustomProcessingStepType);
		Assert.AreEqual(customApi.BoundEntityLogicalName, copy.BoundEntityLogicalName);
		Assert.AreEqual(customApi.Description, copy.Description);
		Assert.AreEqual(customApi.DisplayName, copy.DisplayName);
		Assert.AreEqual(customApi.ExecutePrivilegeName, copy.ExecutePrivilegeName);
		Assert.AreEqual(customApi.FullName, copy.FullName);
		Assert.AreEqual(customApi.IsFunction, copy.IsFunction);
		Assert.AreEqual(customApi.Name, copy.Name);
		Assert.AreEqual(customApi.Prefix, copy.Prefix);
		Assert.AreEqual(customApi.IsPrivate, copy.IsPrivate);
		Assert.AreEqual(customApi.UniqueName, copy.UniqueName);
		Assert.AreEqual(customApi.WorkflowSdkStepEnabled, copy.WorkflowSdkStepEnabled);
		Assert.AreEqual(customApi.RegistrationState, copy.RegistrationState);

		Assert.AreEqual(2, copy.Children.Count());
	}

	[TestMethod]
	public void PluginMapTest()
	{
		// Arrange
		var plugin = new Plugin("thisPlugin")
		{
			RegistrationState = RegistrationState.ToUpdate
		};

		plugin.AddChild(new Step("thisPlugin", Messages.Delete, Stages.PreOperation, Modes.Asynchronous, "thisEntity"));


		// Act
		var copy = _mapper.Map<Plugin>(plugin);

		// Assert
		Assert.AreEqual(plugin.Id, copy.Id);
		Assert.AreEqual(plugin.ParentId, copy.ParentId);
		Assert.AreEqual(plugin.RegistrationState, copy.RegistrationState);

		Assert.AreEqual(plugin.DisplayName, copy.DisplayName);
		Assert.AreEqual(plugin.FullName, copy.FullName);
		Assert.AreEqual(plugin.IsWorkflow, copy.IsWorkflow);
		Assert.AreEqual(plugin.UniqueName, copy.UniqueName);

		Assert.AreEqual(1, copy.Children.Count());
	}

	[TestMethod]
	public void CustomRequestMapTest()
	{
		// Arrange
		var customRequest = new CustomApiRequestParameter
		{
			Description = "thisDescription",
			DisplayName = "thisName",
			Name = "thisName",
			RegistrationState = RegistrationState.ToCreate,
			IsOptional = true,
			Type = new OptionSetValue(4),
			UniqueName = "thisUniqueName"
		};

		// Act
		var copy = _mapper.Map<CustomApiRequestParameter>(customRequest);

		// Assert
		Assert.AreEqual(customRequest.Id, copy.Id);
		Assert.AreEqual(customRequest.ParentId, copy.ParentId);
		Assert.AreEqual(customRequest.Description, copy.Description);
		Assert.AreEqual(customRequest.Name, copy.Name);
		Assert.AreEqual(customRequest.Type, copy.Type);
		Assert.AreEqual(customRequest.DisplayName, copy.DisplayName);
		Assert.AreEqual(customRequest.IsOptional, copy.IsOptional);
		Assert.AreEqual(customRequest.UniqueName, copy.UniqueName);
		Assert.AreEqual(customRequest.RegistrationState, copy.RegistrationState);
	}

	[TestMethod]
	public void CustomResponseMapTest()
	{
		// Arrange
		var customResponse = new CustomApiResponseProperty
		{
			Description = "thisDescription",
			DisplayName = "thisName",
			Name = "thisName",
			RegistrationState = RegistrationState.ToCreate,
			IsOptional = true,
			Type = new OptionSetValue(4),
			UniqueName = "thisUniqueName"
		};

		// Act
		var copy = _mapper.Map<CustomApiResponseProperty>(customResponse);

		// Assert
		Assert.AreEqual(customResponse.Id, copy.Id);
		Assert.AreEqual(customResponse.ParentId, copy.ParentId);
		Assert.AreEqual(customResponse.Description, copy.Description);
		Assert.AreEqual(customResponse.Name, copy.Name);
		Assert.AreEqual(customResponse.Type, copy.Type);
		Assert.AreEqual(customResponse.DisplayName, copy.DisplayName);
		Assert.AreEqual(customResponse.IsOptional, copy.IsOptional);
		Assert.AreEqual(customResponse.UniqueName, copy.UniqueName);
		Assert.AreEqual(customResponse.RegistrationState, copy.RegistrationState);
	}

	[TestMethod]
	public void StepMapTest()
	{
		// Arrange
		var step = new Step("thisPlugin", Messages.Update, Stages.PostOperation, Modes.Synchronous, "thisEntity")
		{
			Order = 66,
			ImpersonationUsername = "thisUser",
			RegistrationState = RegistrationState.ToCreate,
			UniqueName = "un",
			PluginTypeFullName = "PluginTypeFullName",
			DoNotFilterAttributes = false,
			PluginTypeName = "PluginTypeName",
			MessageId = Guid.NewGuid(),
			PreImage =
			{
				AllAttributes = true
			},
			PostImage =
			{
				AllAttributes = true
			}
		};

		step.MethodNames.Add("thisMethod");
		step.FilteringAttributes.Add("thisAttribute");

		// Act
		var copy = _mapper.Map<Step>(step);

		// Assert
		Assert.AreEqual(step.Id, copy.Id);
		Assert.AreEqual(step.ParentId, copy.ParentId);
		Assert.AreEqual(step.Description, copy.Description);
		Assert.AreEqual(step.UniqueName, copy.UniqueName);
		Assert.AreEqual(step.RegistrationState, copy.RegistrationState);

		Assert.AreEqual(step.Order, copy.Order);
		Assert.AreEqual(step.Message, copy.Message);
		Assert.AreEqual(step.Mode, copy.Mode);
		Assert.AreEqual(step.Stage, copy.Stage);
		Assert.AreEqual(step.EntityName, copy.EntityName);
		Assert.IsTrue(step.FilteringAttributes.SequenceEqual(copy.FilteringAttributes));
		Assert.AreEqual(step.ImpersonationUsername, copy.ImpersonationUsername);
		Assert.AreEqual(step.MessageId, copy.MessageId);
		Assert.IsTrue(step.MethodNames.SequenceEqual(copy.MethodNames));
		Assert.AreEqual(step.UnsecureConfig, copy.UnsecureConfig);
		Assert.AreEqual(step.PluginTypeName, copy.PluginTypeName);

		Assert.AreEqual(2, step.Children.Count());
	}

	[TestMethod]
	public void StepImageMapTest()
	{
		// Arrange
		var stepImage = new StepImage(Messages.Update, false, Stages.PostOperation)
		{
			UniqueName = "un",
			AllAttributes = true,
			RegistrationState = RegistrationState.ToUpdate,
			FatherStep = new Step("thisPlugin", Messages.Update, Stages.PostOperation, Modes.Synchronous, "thisEntity")
		};

		stepImage.Attributes.Add("attr");

		// Act
		var copy = _mapper.Map<StepImage>(stepImage);

		// Assert
		Assert.AreEqual(stepImage.Id, copy.Id);
		Assert.AreEqual(stepImage.ParentId, copy.ParentId);
		Assert.AreEqual(stepImage.UniqueName, copy.UniqueName);
		Assert.AreEqual(stepImage.RegistrationState, copy.RegistrationState);

		Assert.AreEqual(stepImage.AllAttributes, copy.AllAttributes);
		Assert.AreEqual(stepImage.Message, copy.Message);
		Assert.AreEqual(stepImage.Stage, copy.Stage);
		Assert.AreEqual(stepImage.JoinedAttributes, copy.JoinedAttributes);
		Assert.AreEqual(stepImage.IsPreImage, copy.IsPreImage);
	}
}