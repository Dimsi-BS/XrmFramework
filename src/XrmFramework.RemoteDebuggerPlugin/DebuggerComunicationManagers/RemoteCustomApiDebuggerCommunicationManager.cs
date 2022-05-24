using System;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Client.Configuration;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;

namespace XrmFramework.Remote
{
    internal class RemoteCustomApiDebuggerCommunicationManager : DebuggerCommunicationManager
    {
        private DebugSession _debugSession;

        public RemoteCustomApiDebuggerCommunicationManager(LocalPluginContext localContext)
            : base(localContext) { }

        public override DebugSession GetDebugSession()
        {
            var queryDebugSessions = CreateBaseDebugSessionQuery(Context.GetInitiatingUserId().ToString());

            var debugSession = Context.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

            _debugSession = debugSession;

            return debugSession;
        }

        protected override RemoteDebugExecutionContext InitRemoteContext()
        {
            if (_debugSession == null)
            {
                _debugSession = GetDebugSession();
            }

            // Parse the Name of the CustomApi as it normally would be on the CRM
            var customApiUniqueName = DebugAssemblySettings.RemoveCustomPrefix(Context.MessageName.ToString());

            // Check if there is assembly that contains the api in the ContextInfo
            var currentAssembly = _debugSession.GetCorrespondingAssemblyInfo(customApiUniqueName);

            if (currentAssembly == null)
            {
                Context.Log("This CustomApi is not registered in the DebugSession");
                //TODO find out what to do in that case
                return null;
            }

            var assemblyQualifiedName = BuildTypeQualifiedName(currentAssembly, customApiUniqueName);

            var remoteContext = Context.RemoteContext;
            remoteContext.Id = Guid.NewGuid();
            remoteContext.TypeAssemblyQualifiedName = assemblyQualifiedName;

            return remoteContext;
        }

        private string BuildTypeQualifiedName(AssemblyContextInfo assembly, string prefixedApiName)
        {
            var customApiName = prefixedApiName.Substring(prefixedApiName.IndexOf('_') + 1);
            return $"{assembly.AssemblyName}.{customApiName},{assembly.AssemblyName},Culture={assembly.Culture}";
        }
    }

}

