using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly string _debugUser;

        public RemoteDebuggerAssemblyHandler(IAssemblyFactory assemblyFactory, IOptions<DebugSessionSettings> debugSettings)
        {
            _assemblyFactory = assemblyFactory;
            _debugUser = debugSettings.Value.userUniqueUri;
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

            foreach (var stepRaw in pluginRaw.Steps)
            {
                var unsecureConfig = JsonConvert.DeserializeObject<DebugUnsecureConfig>(stepRaw.UnsecureConfig);
                //Skip if this user wasn't the one who registered this step
                if (unsecureConfig.DebugUserUri != _debugUser) continue;
                
                var pluginName = unsecureConfig.PluginName;
                var pluginParsed = new Plugin(pluginName);

                var existingPlugin = AssemblyComparer.CorrespondingComponent(pluginParsed, debugAssemblyParsed.Plugins);
                if (existingPlugin != null)
                {
                    existingPlugin.AddChild(stepRaw);
                }
                else
                {
                    pluginParsed.AddChild(stepRaw);
                    debugAssemblyParsed.Plugins.Add(pluginParsed);
                }
            }

            return debugAssemblyParsed;
        }

        public IAssemblyContext CreateDebugAssemblyFromAssembly(IAssemblyContext from, Type TPlugin, Guid debugPluginId)
        {
            var debugAssembly = new AssemblyContext();

            var localPlugins = TPlugin.Assembly.GetTypes();

            var debugPlugin = new Plugin("Father");

            foreach (var plugin in from.Plugins)
            {
                var unsecureConfig = new DebugUnsecureConfig()
                {
                    AssemblyQualifiedName = localPlugins.FirstOrDefault(p => p.FullName == plugin.FullName)?.AssemblyQualifiedName,
                    DebugUserUri = _debugUser,
                    PluginName = plugin.FullName
                };

                foreach (var step in plugin.Steps)
                {
                    step.UnsecureConfig = JsonConvert.SerializeObject(unsecureConfig);
                    debugPlugin.AddChild(step);
                }
            }
            debugPlugin.Id = debugPluginId;
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
