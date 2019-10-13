using System;
using System.Threading.Tasks;
using Debug.Plugins.Utils;
using Microsoft.Xrm.Sdk;
using Model;
using XrmFramework.Debugger;
using XrmFramework.Model;

namespace Debug.Plugins
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new AzureRelayHybridConnectionMessageManager();

            manager.ContextReceived += remoteContext =>
                {
                    var serviceProvider = new RemoteServiceProvider(remoteContext);

                    serviceProvider.RequestSent += request => manager.SendMessageWithResponse(request).GetAwaiter().GetResult();

                    var pluginExecutionTask = Task.Run(() =>
                    {
                        var service = serviceProvider.OrganizationServiceFactory.CreateOrganizationService(null);

                        var step = service.GetById<SdkMessageProcessingStep>(remoteContext.OwningExtension.Id);

                        var pluginType = Type.GetType(step.PluginType.AssemblyQualifiedName);

                        if (pluginType == null)
                        {
                            manager.SendMessage(new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id));
                            return;
                        }

                        var plugin = (IPlugin) Activator.CreateInstance(pluginType, new object[]{null, null});

                        plugin.Execute(serviceProvider);
                    });

                    pluginExecutionTask.GetAwaiter().GetResult();
                };

            manager.RunAndBlock();
        }
    }
}
