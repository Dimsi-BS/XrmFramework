using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class Plugin
    {
        private bool SendToRemoteDebugger(LocalPluginContext localContext)
        {
            if (localContext.IsDebugContext)
            {
                localContext.Log("Went false");
                return false;
            }

            localContext.Log("The context is genuine");

            var initiatingUserId = localContext.GetInitiatingUserId();

            localContext.Log($"Initiating user Id : {initiatingUserId}");

            var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.Equal, initiatingUserId);
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

            var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();


            if (debugSession == null)
            {
                localContext.Log("Debug session is null");
                return false;
            }

            localContext.Log($"Debug session : {debugSession}");

            if (initiatingUserId != debugSession.DebugeeId)
            {
                localContext.Log("Is currently debugging but not for this user, execute the step normally");
                return false;
            }

            // We have to check whether the remoteDebugger has this step, if not we need to send context to remote debugger ourselves

            //Context will be sent by remote debugger plugin

            return true;
        }

        private bool StepIsInDebugSession(DebugSession debugSession, Step step)
        {
            var DebugAssemblyInfo =
                JsonConvert.DeserializeObject<List<AssemblyContextInfo>>(debugSession.AssembliesDebugInfo);
            var stepAssemblyName = step.GetType().Assembly.FullName;

            var stepPluginName = step.
            throw new NotImplementedException();
        }
    }
}
