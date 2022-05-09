using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;
using System.Threading.Tasks;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.RemoteDebugger.Common
{
    public class RemoteDebugger<T> where T : IRemoteDebuggerMessageManager, new()
    {
        public T Manager { get; }

        public RemoteDebugger()
        {
            Manager = new T();
        }

        /// <summary>
        /// Entrypoint for debugging the <typeparamref name="TPlugin"/> Assembly in the solution <paramref name="projectName"/>
        /// </summary>
        /// <typeparam name="TPlugin">Root type of all components to deploy, should be <c>XrmFramework.Plugin</c></typeparam>
        /// <param name="projectName">Name of the local project as named in <c>xrmFramework.config</c></param>
        public void Start<TPlugin>(string projectName)
        {
            Console.WriteLine($"You are about to modify the debug session");

            var serviceProvider = ServiceCollectionHelper.ConfigureForRemoteDebug(projectName);


            var remoteDebuggerHelper = serviceProvider.GetRequiredService<RegistrationHelper>();


            remoteDebuggerHelper.UpdateDebugger<TPlugin>(projectName);

            Manager.ContextReceived += remoteContext =>
                {
                    // Create local service provider from remote context
                    var serviceProvider = new LocalServiceProvider(remoteContext);

                    serviceProvider.RequestSent += request => Manager.SendMessageWithResponse(request).GetAwaiter().GetResult();
                    var pluginExecutionTask = Task.Run(() =>
                    {
                        // Get the assembly qualified name of the plugin to be executed
                        var typeQualifiedName = remoteContext.TypeAssemblyQualifiedName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        // Remove the version part of the list and the public key token
                        typeQualifiedName.RemoveAll(i => i.StartsWith("Version") || i.StartsWith("PublicKeyToken"));
                        //Console.WriteLine(typeQualifiedName);


                        var typeName = string.Join(", ", typeQualifiedName);
                        // Get the pluginType from the newly constructed typeName
                        var pluginType = Type.GetType(typeName);

                        // If no plugin found, return
                        if (pluginType == null)
                        {
                            return;
                        }

                        // The actions to be performed will be different depending on whether the context is a workflow or not
                        if (remoteContext.IsWorkflowContext)
                        {
                            // Create an instance of the workflow
                            var codeActivity = (CodeActivity)Activator.CreateInstance(pluginType);

                            var invoker = new WorkflowInvoker(codeActivity);

                            // The different service available to the service provider to the invoker ?
                            AddExtensionToWorkflowInvoker<IWorkflowContext>(serviceProvider, invoker);
                            AddExtensionToWorkflowInvoker<IOrganizationServiceFactory>(serviceProvider, invoker);
                            AddExtensionToWorkflowInvoker<IServiceEndpointNotificationService>(serviceProvider, invoker);
                            AddExtensionToWorkflowInvoker<ITracingService>(serviceProvider, invoker);

                            // Get arguments from the remote context
                            var inputs = remoteContext.Arguments.ToDictionary(k => k.Key, k => k.Value);
                            // Invoke the corresponding action
                            var outputs = invoker.Invoke(inputs);
                            // Clear arguments now that they have been used
                            remoteContext.Arguments.Clear();

                            // Enter outputs in arguments
                            foreach (var output in outputs)
                            {
                                remoteContext.Arguments[output.Key] = output.Value;
                            }
                        }
                        else
                        {
                            // If a plugin or a custom API, juste create the instance and execute it using the local service provider
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

        private static void AddExtensionToWorkflowInvoker<TService>(IServiceProvider provider, WorkflowInvoker invoker) where TService : class
        {
            var service = provider.GetService(typeof(TService));
            if (service == null)
            {
                return;
            }

            invoker.Extensions.Add<TService>(() => (TService)service);
        }
    }
}
