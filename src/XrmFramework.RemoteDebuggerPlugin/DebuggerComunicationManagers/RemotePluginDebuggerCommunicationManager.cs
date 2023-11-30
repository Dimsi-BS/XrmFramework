using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using XrmFramework.Definitions;

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

        protected override void ModifyDebugSessionQuery(QueryExpression query)
        {
            query.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.Equal, _debugSessionId);
        }
    }
}
