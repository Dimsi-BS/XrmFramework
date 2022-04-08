using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class Plugin
    {
        private bool SendToRemoteDebugger(LocalPluginContext localContext)
        {
            if (!localContext.IsDebugContext)
            {


                localContext.Log("The context is genuine");

                //if (!string.IsNullOrEmpty(UnSecuredConfig) && UnSecuredConfig.Contains("debugSessions"))
                //{
                //    var debuggerUnsecuredConfig = JsonConvert.DeserializeObject<DebuggerUnsecureConfig>(UnSecuredConfig);

                //    localContext.Log($"Debug session ids : {string.Join(",", debuggerUnsecuredConfig.DebugSessionIds)}");

                var initiatingUserId = localContext.GetInitiatingUserId();

                localContext.Log($"Initiating user Id : {initiatingUserId}");

                var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.Equal, initiatingUserId);
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

                //queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.In, debuggerUnsecuredConfig.DebugSessionIds.Cast<object>().ToArray());

                var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

                localContext.Log($"Debug session : {debugSession}");

                if (debugSession != null)
                {
                    localContext.Log("Debug session is non null");
                    if (initiatingUserId != debugSession.DebugeeId)
                    {
                        localContext.Log("Is currently debugging but not for this user, execute the step normally");
                        return false;
                    }
                    // We have to check wether the remoteDebugger has this step, if not we need to send context to remote debugger ourself
                    else
                    {
                        //Context will be sent by remote debugger plugin

                        return true;
                    }
                }
                else
                {
                    localContext.Log("Debug session is null");
                    return false;
                }

            }
            localContext.Log("Went false");
            return false;
        }
    }
}
