using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;

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
        /// Entrypoint for debugging all referenced projects
        /// </summary>
        public void Start()
        {
            Console.WriteLine($"You are about to modify the debug session");

            var assembliesToDebug = Assembly.GetCallingAssembly().GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Where(a => a.GetType("XrmFramework.Plugin") != null
                            || a.GetType("XrmFramework.CustomApi") != null
                            || a.GetType("XrmFramework.Workflow.CustomWorkflowActivity") != null
                )
                .ToList();

            if (!assembliesToDebug.Any())
            {
                throw new ArgumentException(
                    "No project containing components to debug were found, please check that they are referenced");
            }

            var serviceProvider = DebuggerServiceCollectionHelper.ConfigureForRemoteDebug();

            var solutionContext = serviceProvider.GetRequiredService<ISolutionContext>();

            var remoteDebuggerHelper = serviceProvider.GetRequiredService<RegistrationHelper>();

            assembliesToDebug.ForEach(assembly =>
            {
                var targetSolutionName = ServiceCollectionHelper.GetTargetSolutionName(assembly.GetName().Name);
                solutionContext.InitSolutionContext(targetSolutionName);
                remoteDebuggerHelper.UpdateDebugger(assembly);
            });

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
                            // Preferably, use the constructor that takes two strings as parameters, else use the default one
                            var plugin = pluginType.GetConstructors().Any(c =>
                                {
                                    var parameters = c.GetParameters();
                                    return parameters.Length == 2
                                           && parameters[0].ParameterType == typeof(string)
                                           && parameters[1].ParameterType == typeof(string);
                                })
                                ? (IPlugin)Activator.CreateInstance(pluginType, remoteContext.SecureConfig, remoteContext.UnsecureConfig)
                                : (IPlugin)Activator.CreateInstance(pluginType);
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
