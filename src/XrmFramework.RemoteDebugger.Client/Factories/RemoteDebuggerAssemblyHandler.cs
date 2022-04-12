using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.RemoteDebugger.Client.Configuration;


namespace XrmFramework.RemoteDebugger.Client.Utils
{
    public class RemoteDebuggerAssemblyHandler : IRemoteDebuggerAssemblyHandler
    {
        private readonly IAssemblyFactory _assemblyFactory;
        private readonly Guid _debugSessionId;

        public RemoteDebuggerAssemblyHandler(IAssemblyFactory assemblyFactory, IOptions<DebugSessionSettings> debugSettings)
        {
            _assemblyFactory = assemblyFactory;
            _debugSessionId = debugSettings.Value.DebugSessionId;
        }

        public IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, string debugAssemblyName, out Guid debugPluginId)
        {
            var debugAssemblyRaw = _assemblyFactory.CreateFromRemoteAssemblyContext(service, debugAssemblyName);
            var debugAssemblyParsed = new AssemblyContext();
            debugPluginId = Guid.Empty;
            if (debugAssemblyRaw.Assembly == null) return debugAssemblyParsed;

            debugAssemblyParsed.Assembly = debugAssemblyRaw.Assembly;

            var pluginRaw = debugAssemblyRaw.Plugins.FirstOrDefault();
            debugPluginId = pluginRaw.Id;

            var pluginsParsed = pluginRaw.Steps
                .Select(s => (s, s.StepConfiguration))
                .Where(su => su.Item2.DebugSessionId == _debugSessionId)
                .GroupBy(su => su.Item2.PluginName)
                .Select(stepGroup =>
                {
                    var pluginParsed = new Plugin(stepGroup.Key);
                    foreach (var step in stepGroup)
                    {
                        step.s.PluginTypeFullName = step.StepConfiguration.PluginName;
                        pluginParsed.Steps.Add(step.s);
                    }
                    pluginParsed.Id = pluginRaw.Id;

                    return pluginParsed;
                })
                .ToList();

            pluginsParsed.ForEach(p => debugAssemblyParsed.Plugins.Add(p));

            return debugAssemblyParsed;
        }

        public IAssemblyContext CreateDebugAssemblyFromAssembly(IAssemblyContext from, Type TPlugin)
        {
            var debugAssembly = new AssemblyContext();

            var localPlugins = TPlugin.Assembly.GetTypes();

            var debugPlugin = new Plugin("Father");

            foreach (var plugin in from.Plugins)
            {
                var assemblyQualifiedName = localPlugins.FirstOrDefault(p => p.FullName == plugin.FullName)?.AssemblyQualifiedName;
                var pluginName = plugin.FullName;

                foreach (var step in plugin.Steps)
                {
                    var config = string.IsNullOrWhiteSpace(step.UnsecureConfig) ? new StepConfiguration() : JsonConvert.DeserializeObject<StepConfiguration>(step.UnsecureConfig);
                    config.PluginName = pluginName;
                    config.AssemblyQualifiedName = assemblyQualifiedName;
                    config.DebugSessionId = _debugSessionId;

                    step.UnsecureConfig = JsonConvert.SerializeObject(config);
                    debugPlugin.AddChild(step);
                }
            }
            debugAssembly.Plugins.Add(debugPlugin);

            return debugAssembly;
        }

        public IAssemblyContext CreateFromLocalAssemblyContext(Type TPlugin)
        {
            return _assemblyFactory.CreateFromLocalAssemblyContext(TPlugin);
        }

        public IAssemblyContext CreateFromRemoteAssemblyContext(IRegistrationService service, string assemblyName)
        {
            return _assemblyFactory.CreateFromRemoteAssemblyContext(service, assemblyName);
        }

        public IFlatAssemblyContext CreateFlatAssemblyContextFromAssemblyContext(IAssemblyContext from)
        {
            return _assemblyFactory.CreateFlatAssemblyContextFromAssemblyContext(from);
        }
    }
}
