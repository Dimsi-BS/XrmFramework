using Microsoft.Xrm.Sdk;
using System;


namespace XrmFramework.RemoteDebuggerPlugin
{
    public class RemoteDebuggerPlugin : IPlugin
    {
        public RemoteDebuggerPlugin(string unsecuredConfig, string securedConfig)
        {
            UnsecuredConfig = unsecuredConfig;
            SecuredConfig = securedConfig;
        }

        public string UnsecuredConfig { get; }
        public string SecuredConfig { get; }

        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var localContext = new LocalPluginContext(serviceProvider);




            // On modifie le contexte en utilisant unsecuredConfig pour obtenir le nom du plugin qu'on est censé utiliser
            var pluginToExecute = UnsecuredConfig;
            localContext.RemoteContext.TypeAssemblyQualifiedName = pluginToExecute;

            if (SendToRemoteDebugger(localContext))
            {
                return;
            }
            // On met le nom du plugin dans remoteContext.TypeAssemblyQualifiedName

            // On renvoie le tout au remote debugger

        }

        private bool SendToRemoteDebugger(LocalPluginContext context)
        {
            return true;
        }
    }
}
