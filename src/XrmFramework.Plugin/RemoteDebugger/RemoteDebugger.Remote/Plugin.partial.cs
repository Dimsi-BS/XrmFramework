using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (localContext.IsDebugContext) return false;


            var debuggerManager = new PluginDebuggerCommunicationManager(GetType().AssemblyQualifiedName, SecuredConfig, UnSecuredConfig);

            DebugSession debugSession;
            try
            {
                debugSession = debuggerManager.GetDebugSession(localContext);
            }
            catch
            {
                localContext.Log("An error occurred fetching the Debug Session");
                return false;
            }

            if (debugSession == null)
            {
                localContext.Log("Debug session is null");
                return false;
            }

            localContext.Log($"A DebugSession exists for this User : \n\tHybridConnectionName : {debugSession.HybridConnectionName}");


            var isInRemoteDebugger = StepIsInRemoteDebugger(localContext, debugSession);
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


            debuggerManager.SendLocalContextToDebugSession(debugSession, localContext);

            localContext.LogContextExit();
            localContext.Log($"Exiting {ChildClassName} Remote Debugging");
            localContext.LogExit();

            return true;
        }


        private bool StepIsInRemoteDebugger(LocalPluginContext localContext, DebugSession debugSession)
        {
            var DebugAssemblyInfo =
                JsonConvert.DeserializeObject<List<AssemblyContextInfo>>(debugSession.AssembliesDebugInfo);

            var assemblyName = this.GetType().Assembly.GetName().Name;
            var pluginName = this.GetType().FullName;

            var assemblyInfo = DebugAssemblyInfo.FirstOrDefault(a => a.AssemblyName == assemblyName);

            var pluginInfo = assemblyInfo?.Plugins.FirstOrDefault(p => p.Name == pluginName);
            if (pluginInfo == null) return false;

            var message = localContext.MessageName.ToString();
            var stage = Enum.ToObject(typeof(Stages), localContext.Stage).ToString();
            var mode = localContext.Mode.ToString();
            var entityName = localContext.PrimaryEntityName;

            return pluginInfo.Steps.Exists(s =>
                s.Message == message
                && s.Stage == stage
                && s.Mode == mode
                && s.EntityName == entityName);
        }
    }
}
