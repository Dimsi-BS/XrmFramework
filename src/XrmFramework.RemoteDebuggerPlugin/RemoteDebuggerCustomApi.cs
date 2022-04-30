using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;
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
            var initiatingUserId = localContext.GetInitiatingUserId().ToString();

            var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Debugee, ConditionOperator.Equal, initiatingUserId);

            debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

            #region checkers, if wrong returns false
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

            //if (!HybridConnection.TryPingDebugSession(debugSession))
            //{
            //    localContext.Log("Debug Session exists but is not listening");
            //    return false;
            //}
            #endregion
            return true;
        }

    }
}
