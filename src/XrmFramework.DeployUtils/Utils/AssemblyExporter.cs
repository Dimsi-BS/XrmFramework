using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils;

/// <summary>
/// Base implementation of <see cref="IAssemblyExporter"/>
/// </summary>
public partial class AssemblyExporter : IAssemblyExporter
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

    public void CreateAllComponents(IEnumerable<ICrmComponent> componentsToCreate)
    {
        var sortedList = componentsToCreate.ToList();
        sortedList.Sort((x, y) => x.Rank.CompareTo(y.Rank));

        foreach (var component in sortedList)
        {
            CreateComponent(component);
        }
    }

    public IEnumerable<OrganizationRequest> ToUpdateRequestCollection(IEnumerable<ICrmComponent> componentsToUpdate)
    {
        var sortedList = componentsToUpdate.ToList();

        sortedList.Sort((x, y) => x.Rank.CompareTo(y.Rank));

        return sortedList.Select(ToUpdateRequest);
    }

    public void DeleteAllComponents(IEnumerable<ICrmComponent> componentsToDelete)
    {
        var sortedList = componentsToDelete.ToList();
        sortedList.Sort((x, y) => -x.Rank.CompareTo(y.Rank));

        foreach (var component in sortedList)
        {
            DeleteComponent(component);
        }
    }

    public void UpdateComponent(ICrmComponent component)
    {
        var updatedComponent = _converter.ToRegisterComponent(component);
        _registrationService.Update(updatedComponent);
    }

    private UpdateRequest ToUpdateRequest(ICrmComponent component)
    {
        return new UpdateRequest()
        {
            Target = _converter.ToRegisterComponent(component)
        };
    }

    public DeleteRequest ToDeleteRequest(ICrmComponent component)
    {
        return new DeleteRequest()
        {
            Target = new EntityReference(component.EntityTypeName, component.Id)
        };
    }

    public void UpdateAllComponents(IEnumerable<ICrmComponent> componentsToUpdate)
    {
        foreach (var registeringComponent in componentsToUpdate)
            UpdateComponent(registeringComponent);
    }

    /// <summary>
    /// Creates a request to add a component in the form of an <see cref="EntityReference"/>
    /// to the current Solution
    /// </summary>
    /// <param name="objectRef"></param>
    /// <param name="objectTypeCode"></param>
    /// <returns></returns>
    public AddSolutionComponentRequest CreateAddSolutionComponentRequest(EntityReference objectRef,
        int? objectTypeCode = null)
    {
        AddSolutionComponentRequest res = null;
        if (_solutionContext.GetComponentByObjectRef(objectRef) != null) return res;
        res = new AddSolutionComponentRequest
        {
            AddRequiredComponents = false,
            ComponentId = objectRef.Id,
            SolutionUniqueName = _solutionContext.SolutionName
        };

        if (objectTypeCode.HasValue)
            res.ComponentType = objectTypeCode.Value;
        else
            res.ComponentType = objectRef.LogicalName switch
            {
                PluginAssemblyDefinition.EntityName => (int)Deploy.componenttype.PluginAssembly,
                PluginTypeDefinition.EntityName => (int)Deploy.componenttype.PluginType,
                SdkMessageProcessingStepDefinition.EntityName => (int)Deploy.componenttype.SDKMessageProcessingStep,
                SdkMessageProcessingStepImageDefinition.EntityName => (int)Deploy.componenttype
                    .SDKMessageProcessingStepImage,
                _ => res.ComponentType
            };

        return res;
    }
    public void InitExportMetadata(IEnumerable<Step> steps)
    {
        _solutionContext.InitExportMetadata(steps);
    }

    public Deploy.PluginType ToRegisterPluginType(Guid pluginAssemblyId, string pluginFullName)
    {
        var t = new Deploy.PluginType
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
}