using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Model;
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

        public void Start<P>(string solutionName)
        {
            // Get registered steps

            //var plugins = RegistrationHelper.UpdateCrmData<P>("FrameworkTests.Plugins");
            //RegistrationHelper<XrmFramework.RemoteDebuggerPlugin>


            RegistrationHelper.UpdateRemoteDebuggerPlugin<P>(solutionName);
            Console.WriteLine("testey");
            Manager.ContextReceived += remoteContext =>
                {
                    Console.WriteLine("tapadamorda");
                    // Create local service provider from remote context
                    var serviceProvider = new LocalServiceProvider(remoteContext);

                    serviceProvider.RequestSent += request => Manager.SendMessageWithResponse(request).GetAwaiter().GetResult();
                    List<string> pluginsToBeExecuted = new List<string>();
                    var pluginExecutionTask = Task.Run(() =>
                    {
                        /*Console.WriteLine("Stage is {0}, Message is {1}, Mode is {2}, EntityName is {3}",(Stages)remoteContext.Stage,remoteContext.MessageName,(Modes)remoteContext.Mode,remoteContext.PrimaryEntityName);
                        foreach(var p in plugins)
                        {
                            Console.WriteLine(p.FullName);
                            foreach(var step in p.Steps)
                            {
                                if(step.Stage == (Stages)remoteContext.Stage && step.Message == remoteContext.MessageName && step.Mode == (Modes)remoteContext.Mode && step.EntityName == remoteContext.PrimaryEntityName)
                                {
                                    Console.WriteLine("This step, the plugin {0} should be executed", p.FullName);
                                    pluginsToBeExecuted.Add(p.FullName);
                                }
                            }
                        }*/


                        // Get the assembly qualified name of the context which includes the name of the assembly from which this Type object was loaded ????????
                        var typeQualifiedName = remoteContext.TypeAssemblyQualifiedName.Split(new [] {", "}, StringSplitOptions.RemoveEmptyEntries).ToList();
                        // Remove the version part of the list and the public key token
                        typeQualifiedName.RemoveAll(i => i.StartsWith("Version") || i.StartsWith("PublicKeyToken") );
                        //Console.WriteLine(typeQualifiedName);

                        /*foreach(var pluginToBeExecuted in pluginsToBeExecuted)
                        {
                            typeQualifiedName[0] = pluginToBeExecuted;
                            // Reconstruct the string without the deleted elements
                            
                        }*/
                        var typeName = string.Join(", ", typeQualifiedName);
                        //Console.WriteLine(typeName);
                        // Get the pluginType from the newly constructed typeName ?????
                        var pluginType = Type.GetType(typeName);
                        //Console.WriteLine(pluginType.Name);

                        // If no plugin found, return
                        if (pluginType == null)
                        {
                            Manager.SendMessage(new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id));
                            return;
                        }

                        // The actions to be performed will be different depending on wether the context is a workflow or not
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
