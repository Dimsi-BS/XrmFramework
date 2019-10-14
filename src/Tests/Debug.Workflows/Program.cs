using System;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Model;
using RemoteDebugger.Common;
using XrmFramework.Debugger;
using XrmFramework.Model;

namespace Debug.Plugins
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new AzureRelayHybridConnectionMessageManager<RemoteDebugWorkflowExecutionContext>();

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

                        var plugin = (IPlugin) Activator.CreateInstance(pluginType, (string)null, (string) null);

                        plugin.Execute(serviceProvider);
                    });

                    try
                    {

                        pluginExecutionTask.GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        manager.SendMessage(new RemoteDebuggerMessage(RemoteDebuggerMessageType.Exception, e, remoteContext.Id));
                    }
                };

            manager.RunAndBlock();
        }
    }
}
