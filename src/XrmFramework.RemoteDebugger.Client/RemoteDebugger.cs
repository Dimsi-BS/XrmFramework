using Microsoft.Xrm.Sdk;
#if !NET8_0_OR_GREATER
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
#endif
using System;
using System.Linq;
using System.Threading.Tasks;

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
                        var typeQualifiedName = remoteContext.TypeAssemblyQualifiedName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        typeQualifiedName.RemoveAll(i => i.StartsWith("Version") || i.StartsWith("PublicKeyToken"));

                        var typeName = string.Join(", ", typeQualifiedName);

                        var pluginType = Type.GetType(typeName);

                        if (pluginType == null)
                        {
                            Manager.SendMessage(new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id));
                            return;
                        }

#if !NET8_0_OR_GREATER
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
#endif
                            IPlugin plugin;
                            if (pluginType.GetConstructor([typeof(string), typeof(string)]) != null) {
                                plugin = (IPlugin)Activator.CreateInstance(pluginType, remoteContext.UnsecureConfig, remoteContext.SecureConfig);
                            } else {
                                plugin = (IPlugin)Activator.CreateInstance(pluginType);
                            }                           

                            plugin.Execute(serviceProvider);
#if !NET8_0_OR_GREATER
                        }
#endif
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

#if !NET8_0_OR_GREATER
        private static void AddExtensionToWorkflowInvoker<TService>(IServiceProvider provider, WorkflowInvoker invoker) where TService : class
        {
            var service = provider.GetService(typeof(TService));
            if (service == null)
            {
                return;
            }

            invoker.Extensions.Add<TService>(() => (TService)service);
        }
#endif
    }
}
