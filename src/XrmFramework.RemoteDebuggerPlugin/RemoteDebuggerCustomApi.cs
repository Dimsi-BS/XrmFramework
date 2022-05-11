using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.RemoteDebugger.Client.Configuration;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;

namespace XrmFramework.RemoteDebugger
{
    public class RemoteDebuggerCustomApi : RemoteDebuggerPlugin
    {
        public RemoteDebuggerCustomApi(string methodName) : base(null, null)
        {

        }

        internal override RemoteDebugExecutionContext InitRemoteContext(LocalPluginContext localContext, DebugSession debugSession)
        {
            var debugAssemblies =
                JsonConvert.DeserializeObject<List<AssemblyContextInfo>>(debugSession.AssembliesDebugInfo);

            var customApiUniqueName = DebugAssemblySettings.RemoveCustomPrefix(localContext.MessageName.ToString());

            var currentAssembly = debugAssemblies
                .FirstOrDefault(a => a.CustomApis.Exists(c => c.UniqueName == customApiUniqueName));

            if (currentAssembly == null)
            {
                localContext.Log("This CustomApi is not registered in the DebugSession");
                return base.InitRemoteContext(localContext, debugSession);
            }

            var assemblyQualifiedName = BuildTypeQualifiedName(currentAssembly, customApiUniqueName);
            StepConfig.AssemblyQualifiedName = assemblyQualifiedName;
            return base.InitRemoteContext(localContext, debugSession);
        }

        private string BuildTypeQualifiedName(AssemblyContextInfo assembly, string prefixedApiName)
        {
            var customApiName = prefixedApiName.Substring(prefixedApiName.IndexOf('_') + 1);
            return $"{assembly.AssemblyName}.{customApiName},{assembly.AssemblyName},Culture={assembly.Culture}";
        }

        internal override bool GetDebugSession(LocalPluginContext localContext, out DebugSession debugSession)
        {
            var queryDebugSessions = CreateBaseDebugSessionQuery(localContext);

            debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

            return ValidateDebugSession(localContext, debugSession);
        }
    }
}
