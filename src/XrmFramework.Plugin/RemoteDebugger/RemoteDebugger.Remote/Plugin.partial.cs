using Newtonsoft.Json;
using XrmFramework.Remote;
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class Plugin
    {
        private bool IsBeingDebugged(LocalPluginContext localContext)
        {
            if (localContext.IsDebugContext)
            {
                return false;
            }
            
            var debuggerManager = new PluginDebuggerCommunicationManager(localContext, GetType().AssemblyQualifiedName, SecuredConfig, UnSecuredConfig);

            DebugSession? debugSession;
            try
            {
                debugSession = debuggerManager.GetDebugSession();
            }
            catch
            {
                localContext.Log("An error occurred fetching the Debug Session");
                return false;
            }

            if (debugSession == null)
            {
                localContext.Log("No debug session found");
                return false;
            }

            localContext.Log($"A DebugSession exists for this User : \n\tHybridConnectionName : {debugSession.HybridConnectionName}");


            var isInRemoteDebugger = StepIsInRemoteDebuggerPlugin(localContext, debugSession);
            var listenerIsOnline = HybridConnection.TryPingDebugSession(debugSession);

            if (!listenerIsOnline)
            {
                localContext.Log("There are no listeners on this Debug Session");
                return false;
            }
            if (isInRemoteDebugger)
            {
                localContext.Log("This Step is currently being debugged via the RemoteDebugger Plugin, standing down.");
                return true;
            }

            localContext.Log($"The Relay is active, sending context to {debugSession.HybridConnectionName}");


            debuggerManager.SendLocalContextToDebugSession(debugSession);

            localContext.LogContextExit();
            localContext.Log($"Exiting {ChildClassName} Remote Debugging");
            localContext.LogExit();

            return true;
        }


        private bool StepIsInRemoteDebuggerPlugin(LocalPluginContext localContext, DebugSession debugSession)
        {
            var debugAssemblyInfo =
                JsonConvert.DeserializeObject<List<AssemblyContextInfo>>(debugSession.AssembliesDebugInfo);

            var assemblyName = this.GetType().Assembly.GetName().Name;
            var pluginName = this.GetType().FullName;

            var message = localContext.MessageName.ToString();
            var stage = Enum.ToObject(typeof(Stages), localContext.Stage).ToString();
            var mode = localContext.Mode.ToString();
            var entityName = localContext.PrimaryEntityName;

            var assemblyInfo = debugAssemblyInfo.Find(a => a.AssemblyName == assemblyName);

            var pluginInfo = assemblyInfo?.Plugins.Find(p => p.Name == pluginName);
            if (pluginInfo == null) return false;

            return pluginInfo.Steps.Exists(s =>
                s.Message == message
                && s.Stage == stage
                && s.Mode == mode
                && s.EntityName == entityName);
        }
    }
}
