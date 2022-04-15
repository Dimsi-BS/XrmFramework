using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly ICrmComponentComparer _comparer;

        public AssemblyFactory(IOptions<DebugSessionSettings> debugSettings, IAssemblyImporter importer, ICrmComponentComparer comparer)
        {
            _debugSessionId = debugSettings.Value.DebugSessionId;
            _importer = importer;
            _comparer = comparer;
        }

        public IAssemblyContext CreateFromDeployPatch(IDiffPatch patch)
        {
            var result = new AssemblyContext();

            bool RetrievePredicate(DiffComponent x) => x.DiffResult is RegistrationState.ToCreate or RegistrationState.ToUpdate;

            var stepImagesToKeep = patch
                .GetComponentsWhere(d => d.Component is StepImage && RetrievePredicate(d));

            var pluginsToKeep = patch
                .GetComponentsWhere(d => d.Component is Step step
                                    // Retrieve Steps if they have to be updated or if one of their children has to
                                    && (RetrievePredicate(d) ||
                                        step.Children.Any(si => stepImagesToKeep.Contains(si, _comparer))))
                .Select(s => (Step)s)
                .GroupBy(s => s.PluginTypeFullName)
                .Select(stepGroup =>
                {
                    var plugin = new Plugin(stepGroup.Key);
                    foreach (var step in stepGroup)
                    {
                        plugin.AddChild(step);
                    }

                    return plugin;
                })
                .ToList();

            result.Assembly = patch.PluginAssembly;
            pluginsToKeep.ForEach(result.Plugins.Add);

            return result;
        }
        public IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, string debugAssemblyName, out Guid debugPluginId)
        {
            var debugAssemblyRaw = CreateFromRemoteAssemblyContext(service, debugAssemblyName);
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

            pluginsParsed.ForEach(debugAssemblyParsed.Plugins.Add);

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
                    config.PluginName = step.PluginTypeFullName;
                    config.AssemblyQualifiedName = assemblyQualifiedName;
                    config.DebugSessionId = _debugSessionId;

                    step.UnsecureConfig = JsonConvert.SerializeObject(config);
                    debugPlugin.AddChild(step);
                }
            }

            debugPlugin.ParentId = from.Assembly.Id;
            debugPlugin.Id = from.Plugins.First().Id;
            debugAssembly.Plugins.Add(debugPlugin);

            return debugAssembly;
        }
    }
}
