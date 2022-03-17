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
            var typeQualifiedName = localContext.RemoteContext.TypeAssemblyQualifiedName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine("type qualified name");
            Console.WriteLine(typeQualifiedName[0]);
            Console.WriteLine("type qualified name");

            //typeQualifiedName

            localContext.RemoteContext.TypeAssemblyQualifiedName = pluginToExecute;

            if (!localContext.IsDebugContext)
            {
                
                return;
            }
            // On met le nom du plugin dans remoteContext.TypeAssemblyQualifiedName
            //localContext.RemoteContext.TypeAssemblyQualifiedName = UnsecuredConfig;
            //localContext.RemoteContext.TypeAssemblyQualifiedName =


            // On renvoie le tout au remote debugger

        }

        
    }
    
}
