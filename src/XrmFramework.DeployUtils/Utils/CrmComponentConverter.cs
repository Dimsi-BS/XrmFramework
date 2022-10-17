using System;
using System.Linq;
using AutoMapper;
using Deploy;
using Microsoft.Xrm.Sdk;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using CustomApi = XrmFramework.DeployUtils.Model.CustomApi;
using CustomApiRequestParameter = XrmFramework.DeployUtils.Model.CustomApiRequestParameter;
using CustomApiResponseProperty = XrmFramework.DeployUtils.Model.CustomApiResponseProperty;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Utils;

/// <summary>
///     Base implementation of <see cref="ICrmComponentConverter" />
/// </summary>
public class CrmComponentConverter : ICrmComponentConverter
{
	private readonly ISolutionContext _context;
	private readonly IMapper _mapper;

	public CrmComponentConverter(ISolutionContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public Entity ToRegisterComponent(ICrmComponent component)
	{
		switch (component)
		{
			case AssemblyInfo assembly:
				return _mapper.Map<PluginAssembly>(assembly);

			case IAssemblyContext assembly:
				return _mapper.Map<PluginAssembly>(assembly.AssemblyInfo);

			case CustomApi customApi:
				return _mapper.Map<Deploy.CustomApi>(customApi);

			case CustomApiRequestParameter request:
				return _mapper.Map<Deploy.CustomApiRequestParameter>(request);

			case CustomApiResponseProperty response:
				return _mapper.Map<Deploy.CustomApiResponseProperty>(response);

			case Plugin plugin:
				return ToRegisterPluginType(plugin);
			case Step step:
				return ToRegisterStep(step);

			case StepImage image:
				return ToRegisterImage(image);

			case PluginPackage package:
				return _mapper.Map<Deploy.PluginPackage>(package);
			default: throw new ArgumentException("Unknown Crm Component given during Crm export");
		}
	}

	private PluginType ToRegisterPluginType(Plugin plugin)
	{
		var t = new PluginType
		{
			PluginAssemblyId = new EntityReference
			{
				LogicalName = PluginAssemblyDefinition.EntityName,
				Id = plugin.ParentId
			},
			TypeName = plugin.FullName,
			FriendlyName = plugin.FullName
		};

		if (plugin.IsWorkflow)
		{
			t.Name = plugin.DisplayName;
			t.Description = string.Empty;
			t.WorkflowActivityGroupName = "Workflows";
		}
		else
		{
			t.Name = plugin.FullName;
			t.Description = plugin.FullName;
		}

		if (plugin.Id != Guid.Empty) t.Id = plugin.Id;

		return t;
	}

	private SdkMessageProcessingStep ToRegisterStep(Step step)
	{
		// Issue with CRM SDK / Description field max length = 256 characters
		var descriptionAttributeMaxLength = 256;
		var description = step.Description;
		description = description.Length <= descriptionAttributeMaxLength
			? description
			: description.Substring(0, descriptionAttributeMaxLength - 4) + "...)";

		var t = new SdkMessageProcessingStep
		{
			AsyncAutoDelete = step.Mode.Equals(Modes.Asynchronous),
			Description = description,
			EventHandler = new EntityReference(PluginTypeDefinition.EntityName, step.ParentId),
			FilteringAttributes = step.FilteringAttributes.Any() ? string.Join(",", step.FilteringAttributes) : null,
			ImpersonatingUserId = string.IsNullOrEmpty(step.ImpersonationUsername)
				? null
				: new EntityReference(SystemUserDefinition.EntityName,
					_context.GetUserId(step.ImpersonationUsername)),

#pragma warning disable 0612
			InvocationSource = new OptionSetValue((int) sdkmessageprocessingstep_invocationsource.Child),
#pragma warning restore 0612
			IsCustomizable = new BooleanManagedProperty(true),
			IsHidden = new BooleanManagedProperty(false),
			Mode = new OptionSetValue((int) step.Mode),
			Name = description,
#pragma warning disable 0612
			PluginTypeId = new EntityReference(PluginTypeDefinition.EntityName, step.ParentId),
#pragma warning restore 0612
			Rank = step.Order,
			SdkMessageId = _context.GetMessage(step.Message), //GetSdkMessageRef(service, step.Message),
			SdkMessageFilterId =
				_context.GetMessageFilter(step.Message, step.EntityName), //GetSdkMessageFilterRef(service, step),
			//SdkMessageProcessingStepSecureConfigId = GetSdkMessageProcessingStepSecureConfigRef(service, step),
			Stage = new OptionSetValue((int) step.Stage),
			SupportedDeployment = new OptionSetValue((int) sdkmessageprocessingstep_supporteddeployment.ServerOnly),
			Configuration = step.UnsecureConfig
		};

		if (step.Id != Guid.Empty) t.Id = step.Id;

		return t;
	}

	private SdkMessageProcessingStepImage ToRegisterImage(StepImage image)
	{
		var isAllColumns = image.AllAttributes;
		var columns = image.JoinedAttributes;
		var name = image.IsPreImage ? "PreImage" : "PostImage";

		var messagePropertyName = "Target";

		if (image.Message == Messages.Create && !image.IsPreImage)
			messagePropertyName = "Id";
#pragma warning disable 618
		else if (image.Message == Messages.SetState ||
		         image.Message == Messages.SetStateDynamicEntity)
#pragma warning restore 618
			messagePropertyName = "EntityMoniker";

		var t = new SdkMessageProcessingStepImage
		{
			Attributes1 = isAllColumns ? null : columns,
			EntityAlias = name,
			ImageType = new OptionSetValue(image.IsPreImage
				? (int) sdkmessageprocessingstepimage_imagetype.PreImage
				: (int) sdkmessageprocessingstepimage_imagetype.PostImage),
			IsCustomizable = new BooleanManagedProperty(true),
			MessagePropertyName = messagePropertyName,
			Name = name,
			SdkMessageProcessingStepId =
				new EntityReference(SdkMessageProcessingStepDefinition.EntityName, image.ParentId)
		};

		if (image.Id != Guid.Empty) t.Id = image.Id;

		return t;
	}
}