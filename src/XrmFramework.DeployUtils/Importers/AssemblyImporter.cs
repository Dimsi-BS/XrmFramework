using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Deploy;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using CustomApi = XrmFramework.DeployUtils.Model.CustomApi;
using CustomApiRequestParameter = Deploy.CustomApiRequestParameter;
using CustomApiResponseProperty = Deploy.CustomApiResponseProperty;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Importers;

/// <summary>
///     Base implementation of <see cref="IAssemblyImporter" />
/// </summary>
public class AssemblyImporter : IAssemblyImporter
{
    private readonly ICrmMapper _mapper;
    private readonly ISolutionContext _solutionContext;

    public AssemblyImporter(ISolutionContext solutionContext, ICrmMapper mapper)
    {
        _solutionContext = solutionContext;
        _mapper = mapper;
    }

    public AssemblyInfo CreateAssemblyFromLocal(Assembly assembly)
    {
        var fullNameSplit = assembly.FullName.Split(',');

        var name = fullNameSplit[0];
        var version = fullNameSplit[1].Substring(fullNameSplit[1].IndexOf('=') + 1);
        var culture = fullNameSplit[2].Substring(fullNameSplit[2].IndexOf('=') + 1);
        var publicKeyToken = fullNameSplit[3].Substring(fullNameSplit[3].IndexOf('=') + 1);
        var description = $"{name} plugin assembly";

        var t = new AssemblyInfo
        {
            Name = name,
            SourceType = TypeDeSource.Database,
            IsolationMode = ModeDIsolation.Sandbox,
            Culture = culture,
            PublicKeyToken = publicKeyToken,
            Version = version,
            Description = description,
            Content = File.ReadAllBytes(assembly.Location)
        };

        return t;
    }

    public PluginPackage CreatePackageFromRemote(Deploy.PluginPackage package)
    {
        return _mapper.FromRemote(package);
    }

    public IAssemblyContext CreateAssemblyFromRemote(PluginAssembly assembly)
    {
        if (assembly == null) return null;
        var info = _mapper.FromRemote(assembly);
        return new AssemblyContext { AssemblyInfo = info };
    }

    public IAssemblyContext CreateAssemblyFromRemote(AssemblyInfo assemblyInfo)
    {
        return new AssemblyContext { AssemblyInfo = assemblyInfo };
    }

    public bool TryCreateStepFromRemote(SdkMessageProcessingStep sdkStep,
        IEnumerable<SdkMessageProcessingStepImage> sdkImages, out Step step)
    {
        var entityName = sdkStep.EntityName;
        var pluginFullName = sdkStep.EventHandler.Name;
        var pluginFullNameSplit = pluginFullName.Split('.');
        var pluginName = pluginFullNameSplit[pluginFullNameSplit.Length - 1];

        if (sdkStep.StageEnum is not (sdkmessageprocessingstep_stage.Prevalidation
            or sdkmessageprocessingstep_stage.Preoperation or sdkmessageprocessingstep_stage.Postoperation))
        {
            step = null;
            return false;
        }

        step = new Step(pluginName,
            Messages.GetMessage(sdkStep.SdkMessageId.Name),
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            sdkStep.StageEnum switch
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            {
                sdkmessageprocessingstep_stage.Prevalidation => Stages.PreValidation,
                sdkmessageprocessingstep_stage.Preoperation => Stages.PreOperation,
                sdkmessageprocessingstep_stage.Postoperation => Stages.PostOperation
            },
            sdkStep.ModeEnum switch
            {
                sdkmessageprocessingstep_mode.Synchronous => Modes.Synchronous,
                _ => Modes.Asynchronous
            },
            entityName)
        {
            Id = sdkStep.Id,
            PluginTypeFullName = pluginFullName,
            ParentId = sdkStep.EventHandler.Id
        };

        if (!string.IsNullOrWhiteSpace(sdkStep.FilteringAttributes))
        {
            step.FilteringAttributes.Add(sdkStep.FilteringAttributes);
        }

        step.ImpersonationUsername = sdkStep.ImpersonatingUserId?.Name ?? "";
        step.Order = sdkStep.Rank.GetValueOrDefault();
        if (!string.IsNullOrWhiteSpace(sdkStep.Configuration))
            step.StepConfiguration = JsonConvert.DeserializeObject<StepConfiguration>(sdkStep.Configuration);


        var sdkMessageProcessingStepImages = sdkImages.ToList();
        CreateStepImageFromRemote(step, true, sdkMessageProcessingStepImages);
        CreateStepImageFromRemote(step, false, sdkMessageProcessingStepImages);

        return true;
    }

    public Plugin CreatePluginFromRemote(PluginType pluginType, IEnumerable<Step> steps)
    {
        if (pluginType.WorkflowActivityGroupName != null)
            return new Plugin(pluginType.TypeName, pluginType.Name)
            {
                Id = pluginType.Id,
                ParentId = pluginType.PluginAssemblyId.Id
            };

        var plugin = new Plugin(pluginType.TypeName)
        {
            Id = pluginType.Id,
            ParentId = pluginType.PluginAssemblyId.Id
        };

        foreach (var s in steps.Where(s => s.ParentId == plugin.Id)) plugin.Steps.Add(s);
        return plugin;
    }


    public CustomApi CreateCustomApiFromRemote(Deploy.CustomApi customApi,
        IEnumerable<CustomApiRequestParameter> requestParameters,
        IEnumerable<CustomApiResponseProperty> responseProperties)
    {
        var parsedCustomApi = _mapper.FromRemote(customApi);

        requestParameters
            .Where(r => r.CustomApiId.Id == customApi.Id)
            .Select(_mapper.FromRemote)
            .ToList()
            .ForEach(parsedCustomApi.AddChild);

        responseProperties
            .Where(r => r.CustomApiId.Id == customApi.Id)
            .Select(_mapper.FromRemote)
            .ToList()
            .ForEach(parsedCustomApi.AddChild);

        return parsedCustomApi;
    }

    public PluginPackage CreatePackageFromLocal(AssemblyInfo assembly)
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly == null) throw new ArgumentNullException(nameof(assembly));

        var packagesFolderName = entryAssembly
            .GetCustomAttribute<DeployFolderAttribute>()
            .Path;
        var directoryInfos = new DirectoryInfo(packagesFolderName);

        var files = directoryInfos.GetFiles($"{assembly.Name}.*.nupkg");

        var fileInfo = files.FirstOrDefault();

        return fileInfo == null
            ? null
            : new PluginPackage
            {
                Name = $"{_solutionContext.Publisher.CustomizationPrefix}_{assembly.Name}",
                UniqueName = $"{_solutionContext.Publisher.CustomizationPrefix}_{assembly.Name}",
                Version = assembly.Version,
                Content = File.ReadAllBytes(fileInfo.FullName)
            };
    }


    public void CreateStepImageFromRemote(Step step, bool isPreImage,
        IEnumerable<SdkMessageProcessingStepImage> stepImages)
    {
        var imageType = isPreImage
            ? sdkmessageprocessingstepimage_imagetype.PreImage
            : sdkmessageprocessingstepimage_imagetype.PostImage;
        var existingImage = stepImages.FirstOrDefault(i => i.ImageTypeEnum == imageType
                                                           && i.SdkMessageProcessingStepId.Id == step.Id);

        if (existingImage == null)
        {
            return;
        }

        step.PreImage.Id = existingImage.Id;
        step.PreImage.ParentId = step.Id;
        step.PreImage.AllAttributes = existingImage.Attributes1 == null;
        step.PreImage.Attributes.Add(existingImage.Attributes1);
    }

}
