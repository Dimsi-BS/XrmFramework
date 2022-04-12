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
    private readonly ICrmComponentConverter _converter;

    public AssemblyExporter(ISolutionContext solutionContext, IRegistrationService service, ICrmComponentConverter converter)
    {
        _solutionContext = solutionContext;
        _registrationService = service;
        _converter = converter;
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

    public void CreateAllComponents(IEnumerable<ICrmComponent> createComponents)
    {
        if (!createComponents.Any())
        {
            return;
        }

        foreach (var component in createComponents)
        {
            int? entityTypeCode = component.DoFetchTypeCode
                ? _registrationService.GetIntEntityTypeCode(component.EntityTypeName)
                : null;

            if (component is CustomApi customApi)
            {
                var customApiPluginType = ToRegisterPluginType(customApi.AssemblyId, customApi.FullName);
                var id = _registrationService.Create(customApiPluginType);
                customApi.PluginTypeId.Id = id;
            }

            var registeringComponent = ToRegisterComponent(component);
            component.Id = _registrationService.Create(registeringComponent);
            registeringComponent.Id = component.Id;

            if (component.DoAddToSolution)
            {
                var addSolutionComponentRequest = CreateAddSolutionComponentRequest(registeringComponent.ToEntityReference(), entityTypeCode);

                if (addSolutionComponentRequest != null)
                {
                    _registrationService.Execute(addSolutionComponentRequest);
                }
            }


        }
    }

    public void DeleteAllComponents(IEnumerable<ICrmComponent> deleteComponents)
    {
        foreach (var component in deleteComponents)
        {
            _registrationService.Delete(component.EntityTypeName, component.Id);

            if (component is CustomApi customApi)
            {
                _registrationService.Delete(PluginTypeDefinition.EntityName, customApi.PluginTypeId.Id);
            }
        }
    }

    public void UpdateAllComponents(IEnumerable<ICrmComponent> updateComponents)
    {
        var updatedComponents = updateComponents.Select(ToRegisterComponent);

        foreach (var registeringComponent in updatedComponents)
            _registrationService.Update(registeringComponent);
    }

    public void InitExportMetadata(IEnumerable<Step> steps)
    {
        _solutionContext.InitExportMetadata(steps);
    }

    private Entity ToRegisterComponent(ICrmComponent component)
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
                return plugin.IsWorkflow
                    ? _converter.ToRegisterCustomWorkflowType(plugin)
                    : _converter.ToRegisterPluginType((Plugin)component);
            case SdkMessageProcessingStepDefinition.EntityName:
                return _converter.ToRegisterStep((Step)component);

            case SdkMessageProcessingStepImageDefinition.EntityName:
                return _converter.ToRegisterImage((StepImage)component);

            default: throw new ArgumentException("Unknown Crm Component given during Crm export");
        }
    }

    public PluginAssembly ToPluginAssembly(Assembly assembly, Guid registeredId)
    {
        return new PluginAssembly
        {
            Id = registeredId,
            Content = Convert.ToBase64String(File.ReadAllBytes(assembly.Location))
        };
    }
}