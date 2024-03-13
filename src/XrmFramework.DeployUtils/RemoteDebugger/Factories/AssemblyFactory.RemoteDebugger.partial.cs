using System.Reflection;
using AutoMapper;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Importers;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using XrmFramework.DeployUtils.Service;
using XrmFramework.RemoteDebugger.Client.Configuration;

namespace XrmFramework.DeployUtils.Factories;

internal partial class AssemblyFactory
{
    private readonly IDebugSessionProvider _debugSessionProvider;
    private readonly IMapper _mapper;

    public AssemblyFactory(IDebugSessionProvider debugSessionProvider,
      IAssemblyImporter importer,
      IMapper mapper)
    {
        _debugSessionProvider = debugSessionProvider;
        _importer = importer;
        _mapper = mapper;
    }

    public IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, DebugAssemblySettings debugSettings)
    {
        var assembly = service.GetAssemblyByName(DebugAssemblySettings.DebugAssemblyName);

        if (assembly == null) throw new ArgumentException("The DebugAssembly is not deployed on this Solution");

        var debugAssembly = _importer.CreateAssemblyFromRemote(assembly);

        var pluginTypes = service.GetRegisteredPluginTypes(debugAssembly.AssemblyInfo.Id);

        var pluginRaw = pluginTypes.Single(p => p.Name == DebugAssemblySettings.DebugPluginName);
        var customApiRaw = pluginTypes.Single(p => p.Name == DebugAssemblySettings.DebugCustomApiName);

        debugAssembly.AssemblyInfo.Name = debugSettings.TargetAssemblyUniqueName;

        debugSettings.AssemblyId = assembly.Id;
        debugSettings.PluginId = pluginRaw.Id;
        debugSettings.CustomApiId = customApiRaw.Id;

        var stepsRaw = GetParsedSteps(service, debugAssembly.AssemblyInfo.Id);

        var debugSession = _debugSessionProvider.DebugSession;


        var pluginsParsed = stepsRaw
          .Select(s => (s, s.StepConfiguration))
          .Where(su => su.StepConfiguration.DebugSessionId == debugSession.Id
                       && su.StepConfiguration.AssemblyName ==
                       debugSettings.TargetAssemblyUniqueName) // Only look at the current debug session
          .GroupBy(su => su.StepConfiguration.PluginName) // Regroup the steps by plugin
          .Select(stepGroup =>
          {
              var pluginParsed = new Plugin(stepGroup.Key);
              foreach (var (s, stepConfiguration) in stepGroup)
              {
                  var pluginNameSplit = stepConfiguration.PluginName.Split('.');

                  s.PluginTypeFullName = stepConfiguration.PluginName;
                  s.PluginTypeName = pluginNameSplit[pluginNameSplit.Length - 1];
                  pluginParsed.Steps.Add(s);
              }

              pluginParsed.Id = pluginRaw.Id;

              return pluginParsed;
          }) // Recreate the plugin and place its children in it
          .ToList();

        pluginsParsed.ForEach(debugAssembly.AddChild);

        var customApisRaw = GetParsedCustomApis(service, debugAssembly.AssemblyInfo.Id);

        var customApiParsed = customApisRaw
          .Where(c => debugSettings.HasCurrentCustomPrefix(c.Prefix))
          .Select(c =>
          {
              c.Prefix = DebugAssemblySettings.RemoveCustomPrefix(c.Prefix);
              return c;
          })
          .Where(c =>
          {
              var assemblyName = debugSession.GetCorrespondingAssemblyInfo(c.UniqueName)?.AssemblyName;
              return assemblyName != null &&
                 assemblyName.Equals(debugSettings.TargetAssemblyUniqueName);
          })
          .ToList();

        customApiParsed.ForEach(debugAssembly.AddChild);

        return debugAssembly;
    }


    public IAssemblyContext WrapDebugDiffForDebugStrategy(IAssemblyContext from, DebugAssemblySettings debugAssemblySettings,
      Assembly Assembly)
    {
        var debugAssembly = _mapper.Map<IAssemblyContext>(from.AssemblyInfo);

        var localPlugins = Assembly.GetTypes();

        var debugPlugin = new Plugin("Father")
        {
            RegistrationState = RegistrationState.Computed
        };
        var assemblyName = Assembly.GetName().Name;
        foreach (var plugin in from.Plugins)
        {
            var assemblyQualifiedName =
              localPlugins.FirstOrDefault(p => p.FullName == plugin.FullName)?.AssemblyQualifiedName;

            foreach (var step in plugin.Steps)
            {
                step.StepConfiguration.PluginName = step.PluginTypeFullName;
                step.StepConfiguration.AssemblyQualifiedName = assemblyQualifiedName;
                step.StepConfiguration.DebugSessionId = _debugSessionProvider.DebugSession.Id;
                step.StepConfiguration.AssemblyName = assemblyName;

                debugPlugin.AddChild(step);
            }
        }

        debugPlugin.ParentId = debugAssemblySettings.AssemblyId;
        debugPlugin.Id = debugAssemblySettings.PluginId;
        debugAssembly.AddChild(debugPlugin);

        foreach (var customApi in from.CustomApis)
        {
            customApi.ParentId = debugAssemblySettings.CustomApiId;
            //Insert seemingly random value to make the customApi unique
            customApi.Prefix = debugAssemblySettings.AddCustomPrefix(customApi.Prefix);
            debugAssembly.AddChild(customApi);
        }

        return debugAssembly;
    }

    public IAssemblyContext WrapDiffAssemblyForDebugDiff(IAssemblyContext deployAssemblyDiff)
    {
        var assemblyToDebug = _mapper.Map<IAssemblyContext>(deployAssemblyDiff);
        // We can remove now the diff components that are Ignore and whose children are too (recursively)
        // They would only get in the way otherwise
        assemblyToDebug.CleanChildrenWithState(RegistrationState.Ignore);
        assemblyToDebug.CleanChildrenWithState(RegistrationState.ToDelete);

        return assemblyToDebug;
    }
}