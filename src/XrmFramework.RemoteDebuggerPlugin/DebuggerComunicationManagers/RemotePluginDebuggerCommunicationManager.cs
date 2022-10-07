using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.Remote
{
    internal class RemotePluginDebuggerCommunicationManager : PluginDebuggerCommunicationManager
    {
        private readonly Guid _debugSessionId;

        public RemotePluginDebuggerCommunicationManager(LocalPluginContext localContext, string assemblyQualifiedName, string securedConfig, string unsecuredConfig)
        : base(localContext, assemblyQualifiedName, securedConfig, unsecuredConfig)
        {
            var stepConfig = string.IsNullOrEmpty(unsecuredConfig)
                ? new()
                : JsonConvert.DeserializeObject<StepConfiguration>(unsecuredConfig);
            _debugSessionId = stepConfig.DebugSessionId;
            AssemblyQualifiedName = assemblyQualifiedName;
        }

        public override DebugSession GetDebugSession()
        {
            var queryDebugSessions = CreateBaseDebugSessionQuery(Context.GetInitiatingUserId().ToString());

            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.Equal, _debugSessionId);

            var debugSession = Context.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

            return debugSession;
        }
    }
}
