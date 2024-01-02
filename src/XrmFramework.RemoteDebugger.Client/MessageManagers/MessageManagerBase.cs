using System;
using System.Activities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using XrmFramework.RemoteDebugger.Client.Recorder;
using XrmFramework.RemoteDebugger.Common;

namespace XrmFramework.RemoteDebugger.Client.MessageManagers;

public abstract class MessageManagerBase : IRemoteDebuggerMessageManager
{
    private readonly ISessionRecorder _sessionRecorder;

    protected MessageManagerBase(ISessionRecorder sessionRecorder)
    {
        _sessionRecorder = sessionRecorder;

        ContextReceived += OnManagerOnContextReceived;
    }

    private event Action<RemoteDebugExecutionContext> ContextReceived;

    public abstract void RunAndBlock();

    protected void RecordMessage(RemoteDebuggerMessage message)
        => _sessionRecorder.AddMessage(message);

    protected abstract Task SendMessage(RemoteDebuggerMessage message);

    protected abstract Task<RemoteDebuggerMessage> SendMessageWithResponse(RemoteDebuggerMessage message);

    protected void OnContextReceived(RemoteDebugExecutionContext obj)
    {
        ContextReceived?.Invoke(obj);
    }

    private void OnManagerOnContextReceived(RemoteDebugExecutionContext remoteContext)
    {
        // Create local service provider from remote context
        var localServiceProvider = new LocalServiceProvider(remoteContext);

        localServiceProvider.RequestSent += request => SendMessageWithResponse(request).GetAwaiter().GetResult();

        var pluginExecutionTask = Task.Run(() =>
        {
            // Get the assembly qualified name of the plugin to be executed
            var typeQualifiedName = remoteContext.TypeAssemblyQualifiedName
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                AddExtensionToWorkflowInvoker<IWorkflowContext>(localServiceProvider, invoker);
                AddExtensionToWorkflowInvoker<IOrganizationServiceFactory>(localServiceProvider, invoker);
                AddExtensionToWorkflowInvoker<IServiceEndpointNotificationService>(localServiceProvider, invoker);
                AddExtensionToWorkflowInvoker<ITracingService>(localServiceProvider, invoker);

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
                var plugin = Array.Exists(pluginType.GetConstructors(), c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 2 && parameters[0].ParameterType == typeof(string) &&
                           parameters[1].ParameterType == typeof(string);
                })
                    ? (IPlugin)Activator.CreateInstance(pluginType, remoteContext.SecureConfig,
                        remoteContext.UnsecureConfig)
                    : (IPlugin)Activator.CreateInstance(pluginType);
                plugin.Execute(localServiceProvider);
            }
        });

        try
        {
            pluginExecutionTask.GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            SendMessage(new RemoteDebuggerMessage(RemoteDebuggerMessageType.Exception, e, remoteContext.Id));
        }
    }

    private static void AddExtensionToWorkflowInvoker<TService>(IServiceProvider provider, WorkflowInvoker invoker)
        where TService : class
    {
        var service = provider.GetService(typeof(TService));
        if (service == null)
        {
            return;
        }

        invoker.Extensions.Add(() => (TService)service);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
    }

    ~MessageManagerBase()
    {
        Dispose(false);
    }
}