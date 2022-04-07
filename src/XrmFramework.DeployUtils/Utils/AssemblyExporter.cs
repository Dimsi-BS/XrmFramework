using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils;

public class AssemblyExporter : IAssemblyExporter
{
    private readonly IRegistrationService _registrationService;
    private readonly ISolutionContext _solutionContext;

    public AssemblyExporter(ISolutionContext solutionContext, IRegistrationService service)
    {
        _solutionContext = solutionContext;
        _registrationService = service;
    }

    public AddSolutionComponentRequest CreateAddSolutionComponentRequest(EntityReference objectRef,
        int? objectTypeCode = null)
    {
        AddSolutionComponentRequest res = null;
        if (!_solutionContext.Components.Any(c => c.ObjectId.Equals(objectRef.Id)))
        {
            res = new AddSolutionComponentRequest
            {
                AddRequiredComponents = false,
                ComponentId = objectRef.Id,
                SolutionUniqueName = _solutionContext.SolutionName
            };

            if (objectTypeCode.HasValue)
                res.ComponentType = objectTypeCode.Value;
            else
                switch (objectRef.LogicalName)
                {
                    case PluginAssemblyDefinition.EntityName:
                        res.ComponentType = (int)componenttype.PluginAssembly;
                        break;

                    case PluginTypeDefinition.EntityName:
                        res.ComponentType = (int)componenttype.PluginType;
                        break;

                    case SdkMessageProcessingStepDefinition.EntityName:
                        res.ComponentType = (int)componenttype.SDKMessageProcessingStep;
                        break;

                    case SdkMessageProcessingStepImageDefinition.EntityName:
                        res.ComponentType = (int)componenttype.SDKMessageProcessingStepImage;
                        break;
                }
        }

        return res;
    }

    public PluginType ToRegisterPluginType(Guid pluginAssemblyId, string pluginFullName)
    {
        var t = new PluginType
        {
            PluginAssemblyId = new EntityReference
            {
                LogicalName = PluginAssemblyDefinition.EntityName,
                Id = pluginAssemblyId
            },
            TypeName = pluginFullName,
            FriendlyName = pluginFullName,
            Name = pluginFullName,
            Description = pluginFullName
        };

        return t;
    }

    public void CreateAllComponents<T>(IEnumerable<T> components, bool doAddToSolution = false,
        bool doFetchCode = false) where T : ISolutionComponent
    {
        var createComponents = components
            .Where(x => x.RegistrationState == RegistrationState.ToCreate)
            .ToList();

        if (!createComponents.Any()) return;
        var entityTypeCode = doFetchCode
            ? _registrationService?.GetIntEntityTypeCode(createComponents.FirstOrDefault()?.EntityTypeName)
            : null;
        foreach (var component in createComponents)
        {
            var registeringComponent = ToRegisterComponent(component);
            component.Id = _registrationService.Create(registeringComponent);
            registeringComponent.Id = component.Id;
            if (!doAddToSolution) continue;
            var addSolutionComponentRequest =
                CreateAddSolutionComponentRequest(registeringComponent.ToEntityReference(), entityTypeCode);
            if (addSolutionComponentRequest != null) _registrationService.Execute(addSolutionComponentRequest);
        }
    }

    public void DeleteAllComponents<T>(IEnumerable<T> components) where T : ISolutionComponent
    {
        var deleteComponents = components
            .Where(x => x.RegistrationState == RegistrationState.ToDelete)
            .ToList();
        foreach (var component in deleteComponents) _registrationService.Delete(component.EntityTypeName, component.Id);
    }

    public void UpdateAllComponents<T>(IEnumerable<T> components) where T : ISolutionComponent
    {
        var updateComponents = components
            .Where(x => x.RegistrationState == RegistrationState.ToUpdate)
            .ToList();
        foreach (var registeringComponent in updateComponents.Select(component => ToRegisterComponent(component)))
            _registrationService.Update(registeringComponent);
    }

    public void InitExportMetadata(IEnumerable<Step> steps)
    {
        _solutionContext.InitExportMetadata(steps);
    }

    private Entity ToRegisterComponent(ISolutionComponent component)
    {
        switch (component.EntityTypeName)
        {
            case PluginAssemblyDefinition.EntityName:
                return (PluginAssembly)component;

            case CustomApiDefinition.EntityName:
                return (CustomApi)component;

            case CustomApiRequestParameterDefinition.EntityName:
                return (CustomApiRequestParameter)component;

            case CustomApiResponsePropertyDefinition.EntityName:
                return (CustomApiResponseProperty)component;

            case PluginTypeDefinition.EntityName:
                var plugin = (Plugin)component;
                if (plugin.IsWorkflow)
                    return ToRegisterCustomWorkflowType(plugin);
                else
                    return ToRegisterPluginType((Plugin)component);
            case SdkMessageProcessingStepDefinition.EntityName:
                return ToRegisterStep((Step)component);

            case SdkMessageProcessingStepImageDefinition.EntityName:
                return ToRegisterImage((StepImage)component);

            default: throw new ArgumentException("Unknown Solution Component given during Crm export");
        }

        throw new NotImplementedException();
    }

    public PluginAssembly ToPluginAssembly(Assembly assembly, Guid registeredId)
    {
        return new PluginAssembly
        {
            Id = registeredId,
            Content = Convert.ToBase64String(File.ReadAllBytes(assembly.Location))
        };
    }

    private PluginType ToRegisterCustomWorkflowType(Plugin plugin)
    {
        var t = new PluginType
        {
            PluginAssemblyId = new EntityReference
            {
                LogicalName = PluginAssemblyDefinition.EntityName,
                Id = plugin.ParentId
            },
            TypeName = plugin.FullName,
            FriendlyName = plugin.FullName,
            Name = plugin.DisplayName,
            Description = string.Empty,
            WorkflowActivityGroupName = "Workflows"
        };

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
                ? (int)sdkmessageprocessingstepimage_imagetype.PreImage
                : (int)sdkmessageprocessingstepimage_imagetype.PostImage),
            IsCustomizable = new BooleanManagedProperty(true),
            MessagePropertyName = messagePropertyName,
            Name = name,
            SdkMessageProcessingStepId =
                new EntityReference(SdkMessageProcessingStepDefinition.EntityName, image.ParentId)
        };

        if (image.Id != Guid.Empty) t.Id = image.Id;

        return t;
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
            FriendlyName = plugin.FullName,
            Name = plugin.FullName,
            Description = plugin.FullName
        };
        if (plugin.Id != Guid.Empty) t.Id = plugin.Id;

        return t;
    }

    private SdkMessageProcessingStep ToRegisterStep(Step step)
    {
        // Issue with CRM SDK / Description field max length = 256 characters
        var descriptionAttributeMaxLength = 256;
        var description =
            $"{step.PluginTypeName} : {step.Stage} {step.Message} of {step.EntityName} ({step.MethodsDisplayName})";
        description = description.Length <= descriptionAttributeMaxLength
            ? description
            : description.Substring(0, descriptionAttributeMaxLength - 4) + "...)";

        if (!string.IsNullOrEmpty(step.ImpersonationUsername))
        {
            var count = _solutionContext.Users.Count(u => u.Key == step.ImpersonationUsername);

            if (count == 0)
                throw new Exception($"{description} : No user have fullname '{step.ImpersonationUsername}' in CRM.");
            if (count > 1)
                throw new Exception(
                    $"{description} : {count} users have the fullname '{step.ImpersonationUsername}' in CRM.");
        }

        var t = new SdkMessageProcessingStep
        {
            AsyncAutoDelete = step.Mode.Equals(Modes.Asynchronous),
            Description = description,
            EventHandler = new EntityReference(PluginTypeDefinition.EntityName, step.ParentId),
            FilteringAttributes = step.FilteringAttributes.Any() ? string.Join(",", step.FilteringAttributes) : null,
            ImpersonatingUserId = string.IsNullOrEmpty(step.ImpersonationUsername)
                ? null
                : new EntityReference(SystemUserDefinition.EntityName,
                    _solutionContext.Users.First(u => u.Key == step.ImpersonationUsername).Value),

#pragma warning disable 0612
            InvocationSource = new OptionSetValue((int)sdkmessageprocessingstep_invocationsource.Child),
#pragma warning restore 0612
            IsCustomizable = new BooleanManagedProperty(true),
            IsHidden = new BooleanManagedProperty(false),
            Mode = new OptionSetValue((int)step.Mode),
            Name = description,
#pragma warning disable 0612
            PluginTypeId = new EntityReference(PluginTypeDefinition.EntityName, step.ParentId),
#pragma warning restore 0612
            Rank = step.Order,
            SdkMessageId = _solutionContext.Messages[step.Message], //GetSdkMessageRef(service, step.Message),
            SdkMessageFilterId = _solutionContext.Filters.Where(f => f.SdkMessageId.Name == step.Message.ToString()
                                                                     && f.PrimaryObjectTypeCode == step.EntityName)
                .Select(f => f.ToEntityReference()).FirstOrDefault(), //GetSdkMessageFilterRef(service, step),
            //SdkMessageProcessingStepSecureConfigId = GetSdkMessageProcessingStepSecureConfigRef(service, step),
            Stage = new OptionSetValue((int)step.Stage),
            SupportedDeployment = new OptionSetValue((int)sdkmessageprocessingstep_supporteddeployment.ServerOnly),
            Configuration = step.UnsecureConfig
        };

        if (step.Id != Guid.Empty) t.Id = step.Id;

        return t;
    }
}