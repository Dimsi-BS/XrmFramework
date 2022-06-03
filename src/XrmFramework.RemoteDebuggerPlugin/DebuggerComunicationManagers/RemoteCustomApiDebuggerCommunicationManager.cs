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

            var debugApiName = Context.MessageName.ToString().Split('_');

            // Parse the Name of the CustomApi as it normally would be on the CRM
            var apiPrefix = DebugAssemblySettings.RemoveCustomPrefix(debugApiName[0]);
            var apiName = debugApiName[1];

            // Check if there is assembly that contains the api in the ContextInfo
            var currentAssembly = _debugSession.GetCorrespondingAssemblyInfo($"{apiPrefix}_{apiName}");

            if (currentAssembly == null)
            {
                throw new ArgumentException("This CustomApi is not registered in the DebugSession");
            }

            var assemblyQualifiedName = BuildTypeQualifiedName(currentAssembly, apiName);

            var remoteContext = Context.RemoteContext;
            remoteContext.Id = Guid.NewGuid();
            remoteContext.TypeAssemblyQualifiedName = assemblyQualifiedName;

            return remoteContext;
        }

        private string BuildTypeQualifiedName(AssemblyContextInfo assembly, string apiName)
        {
            return $"{assembly.AssemblyName}.{apiName},{assembly.AssemblyName},Culture={assembly.Culture}";
        }
    }

}

