using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using XrmFramework.Remote;

namespace XrmFramework.RemoteDebugger
{
    /// <summary>
    /// Defines the execution for debugging a Plugin
    /// </summary>
    /// <seealso cref="Microsoft.Xrm.Sdk.IPlugin" />
    public class RemoteDebuggerPlugin : IPlugin
    {
        public RemoteDebuggerPlugin(string unsecuredConfig, string securedConfig)
        {
            UnsecuredConfig = unsecuredConfig;
            SecuredConfig = securedConfig;
            StepConfig = string.IsNullOrEmpty(unsecuredConfig)
            ? new()
            : JsonConvert.DeserializeObject<StepConfiguration>(unsecuredConfig);

        }

        public string UnsecuredConfig { get; }
        public string SecuredConfig { get; }
        public StepConfiguration StepConfig { get; }

        internal virtual IDebuggerCommunicationManager GetCommunicationManager(LocalPluginContext localContext)
            => new RemotePluginDebuggerCommunicationManager(localContext, StepConfig.AssemblyQualifiedName,
                SecuredConfig,
                UnsecuredConfig);


        public void Execute(IServiceProvider serviceProvider)
        {
            #region null check and localContext get
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var localContext = new LocalPluginContext(serviceProvider, UnsecuredConfig, SecuredConfig);

            if (localContext.IsDebugContext)
            {
                localContext.Log("This plugin should not be executed in local context");
                return;
            }
            #endregion

            var communicationManager = GetCommunicationManager(localContext);

            localContext.Log("The context is genuine");
            localContext.Log($"\r\nIntended Class {StepConfig.PluginName}");
            localContext.LogStart();

            var debugSession = communicationManager.GetDebugSession();
            if (!ValidateDebugSession(localContext, debugSession))
            {
                LogExit(localContext);
                return;
            }

            localContext.Log($"Debug Session :\n\tDebugeeId : {debugSession.Debugee}\n\tHybridConnectionName  : {debugSession.HybridConnectionName}");

            communicationManager.SendLocalContextToDebugSession(debugSession);

            LogExit(localContext);
        }


        private void LogExit(LocalPluginContext localContext)
        {
            localContext.LogContextExit();
            localContext.Log($"Exiting {StepConfig.PluginName} Remote Debugging");
            localContext.LogExit();
        }


        internal static bool ValidateDebugSession(LocalPluginContext localContext, DebugSession debugSession)
        {
            if (debugSession == null)
            {
                localContext.Log("Corresponding DebugSession Not Found");
                return false;
            }

            if (debugSession.SessionEnd <= DateTime.Today)
            {
                localContext.Log("Debug Session expired, please contact your admin");
                return false;
            }
            return true;
        }
    }
}
