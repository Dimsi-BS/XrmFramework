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

    public AssemblyInfo CreateAssemblyFromLocal(string dllPath)
    {
        // Identity read from the metadata (without loading the assembly into the runtime):
        // allows a net8 process to describe a net462 plugin assembly.
        var an = AssemblyName.GetAssemblyName(dllPath);

        var name = an.Name;
        var version = an.Version?.ToString() ?? "0.0.0.0";
        var culture = string.IsNullOrEmpty(an.CultureName) ? "neutral" : an.CultureName;
        var tokenBytes = an.GetPublicKeyToken();
        var publicKeyToken = tokenBytes == null || tokenBytes.Length == 0
            ? "null"
            : string.Concat(tokenBytes.Select(b => b.ToString("x2")));
        var description = $"{name} plugin assembly";

        var t = new AssemblyInfo
        {
            Name = name,
            SourceType = SourceType.Database,
            IsolationMode = IsolationMode.Sandbox,
            Culture = culture,
            PublicKeyToken = publicKeyToken,
            Version = version,
            Description = description,
            Content = File.ReadAllBytes(dllPath)
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
        // Nothing registered yet: no context at all, rather than one wrapping a null AssemblyInfo.
        // The diff reads "no context" as "everything has to be created"; a context whose assembly
        // row is missing looks deployed and blows up as soon as its id is read.
        if (assemblyInfo == null) return null;

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

    public PluginPackage CreatePackageFromLocal(AssemblyInfo assembly, string packagePath)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        if (string.IsNullOrWhiteSpace(packagePath))
            throw new ArgumentException("The plugin package path is required.", nameof(packagePath));

        if (!File.Exists(packagePath))
            throw new FileNotFoundException(
                $"Plugin package '{packagePath}' not found. Build the plugin project with " +
                "<XrmFrameworkPluginPackage>true</XrmFrameworkPluginPackage> to produce it.",
                packagePath);

        // The publisher prefix is required: Dataverse identifies a package by its unique name, and
        // an unprefixed one would collide with any other publisher's package of the same assembly.
        var uniqueName = $"{_solutionContext.Publisher.CustomizationPrefix}_{assembly.Name}";

        return new PluginPackage
        {
            Name = uniqueName,
            UniqueName = uniqueName,
            Version = assembly.Version,
            Content = File.ReadAllBytes(packagePath)
        };
    }

    /// <summary>
    ///     Locates the <c>.nupkg</c> produced for <paramref name="dllPath" />.
    ///     NuGet drops it in <c>bin/&lt;Configuration&gt;/</c>, a sibling of the TFM folder holding the
    ///     assembly, so the assembly folder is searched first and its parent second - that also covers
    ///     a project that redirects <c>PackageOutputPath</c> next to its output.
    /// </summary>
    /// <param name="dllPath">Path of the plugin assembly being deployed.</param>
    /// <returns>The package path, or <see langword="null" /> when none was found.</returns>
    public static string ResolvePackagePath(string dllPath)
    {
        var assemblyFolder = Path.GetDirectoryName(Path.GetFullPath(dllPath));
        if (string.IsNullOrEmpty(assemblyFolder)) return null;

        var assemblyName = Path.GetFileNameWithoutExtension(dllPath);

        // Only the package of THIS assembly: a folder may hold several, one per project.
        var candidateFolders = new[] {assemblyFolder, Path.GetDirectoryName(assemblyFolder)};

        foreach (var folder in candidateFolders)
        {
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) continue;

            var match = new DirectoryInfo(folder)
                .GetFiles($"{assemblyName}.*.nupkg")
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .FirstOrDefault();

            if (match != null) return match.FullName;
        }

        return null;
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
