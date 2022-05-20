using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Client.Configuration;


namespace XrmFramework.DeployUtils.Utils
{
    internal partial class AssemblyFactory
    {
        private readonly DebugSession _debugSession;
        private readonly IMapper _mapper;

        public AssemblyFactory(IOptions<DebugSession> debugSession,
                               IAssemblyImporter importer,
                               IMapper mapper)
        {
            _debugSession = debugSession.Value;
            _importer = importer;
            _mapper = mapper;
        }

        public IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, DebugAssemblySettings debugSettings)
        {
            var assembly = service.GetAssemblyByName(DebugAssemblySettings.DebugAssemblyName);

            if (assembly == null)
            {
                throw new ArgumentException("The DebugAssembly is not deployed on this Solution");
            }

            assembly.Name = debugSettings.TargetAssemblyUniqueName;
            var debugAssemblyRaw = _importer.CreateAssemblyFromRemote(assembly);

            Console.WriteLine("Remote Debug Plugin Exists, Fetching Components...");

            FillRemoteAssemblyContext(service, debugAssemblyRaw);

            var debugAssemblyParsed = _mapper.Map<IAssemblyContext>(debugAssemblyRaw.AssemblyInfo);

            var pluginRaw = debugAssemblyRaw.Plugins.Single(p => p.FullName == DebugAssemblySettings.DebugPluginName);

            debugSettings.AssemblyId = assembly.Id;
            debugSettings.PluginId = pluginRaw.Id;

            /*
             * Little trick here :
             * If there is already a CustomApi registered on the RemoteDebugger, then its PluginType will be filtered out
             * Then we can get the Id of the PluginType from any registered CustomApi's ParentId
             * Else the actual PluginType wasn't filtered out and we can get its Id directly
             */
            debugSettings.CustomApiId = debugAssemblyRaw.CustomApis.Any()
                ? debugAssemblyRaw.CustomApis.First().ParentId
                : debugAssemblyRaw.Plugins.Single(p => p.FullName == DebugAssemblySettings.DebugCustomApiName).Id;

            var pluginsParsed = pluginRaw.Steps
                .Select(s => (s, s.StepConfiguration))
                .Where(su => su.StepConfiguration.DebugSessionId == _debugSession.Id
                                            && su.StepConfiguration.AssemblyName == assembly.Name) // Only look at the current debug session
                .GroupBy(su => su.StepConfiguration.PluginName) // Regroup the steps by plugin
                .Select(stepGroup =>
                {
                    var pluginParsed = new Plugin(stepGroup.Key);
                    foreach (var (s, stepConfiguration) in stepGroup)
                    {
                        s.PluginTypeFullName = stepConfiguration.PluginName;
                        pluginParsed.Steps.Add(s);
                    }
                    pluginParsed.Id = pluginRaw.Id;

                    return pluginParsed;
                }) // Recreate the plugin and place its children in it
                .ToList();

            pluginsParsed.ForEach(debugAssemblyParsed.AddChild);

            var customApiParsed = debugAssemblyRaw.CustomApis
                .Where(c => debugSettings.HasCurrentCustomPrefix(c.UniqueName))
                .Select(c =>
                {
                    c.UniqueName = DebugAssemblySettings.RemoveCustomPrefix(c.UniqueName);
                    return c;
                })
                .Where(c => _debugSession.GetCorrespondingAssemblyInfo(c.UniqueName) != null)
                .ToList();

            customApiParsed.ForEach(debugAssemblyParsed.AddChild);

            return debugAssemblyParsed;
        }


        public IAssemblyContext WrapDebugDiffForDebugStrategy(IAssemblyContext from, DebugAssemblySettings debugSettings, Assembly Assembly)
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
                var assemblyQualifiedName = localPlugins.FirstOrDefault(p => p.FullName == plugin.FullName)?.AssemblyQualifiedName;

                foreach (var step in plugin.Steps)
                {
                    step.StepConfiguration.PluginName = step.PluginTypeFullName;
                    step.StepConfiguration.AssemblyQualifiedName = assemblyQualifiedName;
                    step.StepConfiguration.DebugSessionId = _debugSession.Id;
                    step.StepConfiguration.AssemblyName = assemblyName;

                    debugPlugin.AddChild(step);
                }
            }

            debugPlugin.ParentId = debugSettings.AssemblyId;
            debugPlugin.Id = debugSettings.PluginId;
            debugAssembly.AddChild(debugPlugin);
            debugAssembly.RegistrationState = RegistrationState.Computed;

            foreach (var customApi in from.CustomApis)
            {
                customApi.ParentId = debugSettings.CustomApiId;
                //Insert seemingly random value to make the customApi unique
                customApi.UniqueName = debugSettings.AddCustomPrefix(customApi.UniqueName);
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
}
