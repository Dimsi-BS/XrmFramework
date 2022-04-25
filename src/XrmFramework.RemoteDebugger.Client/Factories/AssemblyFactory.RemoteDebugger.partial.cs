using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.RemoteDebugger.Client.Configuration;


namespace XrmFramework.DeployUtils.Utils
{
    partial class AssemblyFactory
    {
        private readonly Guid _debugSessionId;
        private readonly IMapper _mapper;

        public AssemblyFactory(IOptions<DebugSessionSettings> debugSettings,
                               IAssemblyImporter importer,
                               IMapper mapper)
        {
            _debugSessionId = debugSettings.Value.DebugSessionId;
            _importer = importer;
            _mapper = mapper;
        }

        public IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, string debugAssemblyName, out Guid debugPluginId)
        {
            var assembly = service.GetAssemblyByName(debugAssemblyName);

            var debugAssemblyRaw = _importer.CreateAssemblyFromRemote(assembly);

            if (assembly == null)
            {
                debugPluginId = Guid.Empty;
                return debugAssemblyRaw;
            }

            Console.WriteLine("Remote Debug Plugin Exists, Fetching Components...");

            FillRemoteAssemblyContext(service, debugAssemblyRaw);

            var debugAssemblyParsed = _importer.CreateAssemblyFromRemote(assembly);

            var pluginRaw = debugAssemblyRaw.Plugins.First();
            debugPluginId = pluginRaw.Id;

            var pluginsParsed = pluginRaw.Steps
                .Select(s => (s, s.StepConfiguration))
                .Where(su => su.StepConfiguration.DebugSessionId == _debugSessionId)
                .GroupBy(su => su.StepConfiguration.PluginName)
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
                })
                .ToList();

            pluginsParsed.ForEach(debugAssemblyParsed.Plugins.Add);

            debugAssemblyParsed.Workflows.Clear();
            debugAssemblyParsed.CustomApis.Clear();

            return debugAssemblyParsed;
        }

        public IAssemblyContext WrapDebugDiffForDebugDeploy(IAssemblyContext from, Guid debugPluginId, Type TPlugin)
        {
            var debugAssembly = _mapper.Map<IAssemblyContext>(from);

            debugAssembly.Plugins.Clear();
            debugAssembly.Workflows.Clear();
            debugAssembly.CustomApis.Clear();

            var localPlugins = TPlugin.Assembly.GetTypes();

            var debugPlugin = new Plugin("Father");

            foreach (var plugin in from.Plugins)
            {
                var assemblyQualifiedName = localPlugins.FirstOrDefault(p => p.FullName == plugin.FullName)?.AssemblyQualifiedName;

                foreach (var step in plugin.Steps)
                {
                    step.StepConfiguration.PluginName = step.PluginTypeFullName;
                    step.StepConfiguration.AssemblyQualifiedName = assemblyQualifiedName;
                    step.StepConfiguration.DebugSessionId = _debugSessionId;

                    debugPlugin.AddChild(step);
                }
            }

            debugPlugin.ParentId = from.AssemblyInfo.Id;
            debugPlugin.Id = debugPluginId;
            debugAssembly.Plugins.Add(debugPlugin);
            debugAssembly.RegistrationState = RegistrationState.Computed;

            return debugAssembly;
        }

        public IAssemblyContext WrapDiffAssemblyForDebugDiff(IAssemblyContext deployAssemblyDiff)
        {
            var assemblyToDebug = _mapper.Map<IAssemblyContext>(deployAssemblyDiff);
            assemblyToDebug.CleanChildrenWithState(RegistrationState.ToDelete);
            return assemblyToDebug;
        }
    }
}
