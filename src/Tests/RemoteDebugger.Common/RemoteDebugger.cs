using System;
using System.Activities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.RemoteDebugger.Common
{
    public class RemoteDebugger<T> where T : IRemoteDebuggerMessageManager, new()
    {
        public T Manager { get; }

        public RemoteDebugger()
        {
            Manager = new T();
        }

        public void Start()
        {

            Manager.ContextReceived += remoteContext =>
                {
                    var serviceProvider = new LocalServiceProvider(remoteContext);

                    serviceProvider.RequestSent += request => Manager.SendMessageWithResponse(request).GetAwaiter().GetResult();

                    var pluginExecutionTask = Task.Run(() =>
                    {
                        var pluginType = Type.GetType(remoteContext.TypeAssemblyQualifiedName);

                        if (pluginType == null)
                        {
                            Manager.SendMessage(new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id));
                            return;
                        }

                        if (remoteContext.IsWorkflowContext)
                        {
                            var codeActivity = (CodeActivity)Activator.CreateInstance(pluginType);

                            var invoker = new WorkflowInvoker(codeActivity);

                            AddExtensionToWorkflowInvoker<IWorkflowContext>(serviceProvider, invoker);
                            AddExtensionToWorkflowInvoker<IOrganizationServiceFactory>(serviceProvider, invoker);
                            AddExtensionToWorkflowInvoker<IServiceEndpointNotificationService>(serviceProvider, invoker);
                            AddExtensionToWorkflowInvoker<ITracingService>(serviceProvider, invoker);

                            var inputs = remoteContext.Arguments.ToDictionary(k => k.Key, k => k.Value);

                            var outputs = invoker.Invoke(inputs);

                            remoteContext.Arguments.Clear();

                            foreach (var output in outputs)
                            {
                                remoteContext.Arguments[output.Key] = output.Value;
                            }
                        }
                        else
                        {
                            var plugin = (IPlugin)Activator.CreateInstance(pluginType, (string)null, (string)null);

                            plugin.Execute(serviceProvider);
                        }
                    });

                    try
                    {
                        pluginExecutionTask.GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        Manager.SendMessage(new RemoteDebuggerMessage(RemoteDebuggerMessageType.Exception, e, remoteContext.Id));
                    }
                };

            Manager.RunAndBlock();
        }

        private static void AddExtensionToWorkflowInvoker<T>(IServiceProvider provider, WorkflowInvoker invoker) where T : class
        {
            var service = provider.GetService(typeof(T));
            if (service == null)
            {
                return;
            }

            invoker.Extensions.Add<T>(() => (T)service);
        }
    }
}
