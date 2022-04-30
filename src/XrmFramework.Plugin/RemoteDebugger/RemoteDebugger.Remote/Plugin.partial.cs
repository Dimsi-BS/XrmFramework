using Microsoft.Xrm.Sdk;
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
            if (localContext.IsDebugContext) return false;

            var initiatingUserId = localContext.GetInitiatingUserId().ToString();

            DebugSession debugSession = null;
            try
            {
                debugSession = GetDebugSession(localContext.AdminOrganizationService, initiatingUserId);
            }
            catch (Exception e)
            {
                localContext.Log("An error occurred fetching the Debug Session");
                return false;
            }

            if (debugSession == null)
            {
                localContext.Log("Debug session is null");
                return false;
            }

            localContext.Log($"A DebugSession exists for this User : \n\tHybridConnectionName : {debugSession.HybridConnectionName}");

            if (initiatingUserId != debugSession.Debugee)
            {
                localContext.Log("Is currently debugging but not for this user, execute the step normally");
                return false;
            }

            if (!StepIsInDebugSession(localContext, debugSession))
            {
                return false;
            }

            localContext.Log($"This step is in the DebugSession configuration");

            if (!HybridConnection.TryPingDebugSession(debugSession))
            {
                return false;
            }
            // We have to check whether the remoteDebugger has this step, if not we need to send context to remote debugger ourselves

            //Context will be sent by remote debugger plugin
            localContext.Log("The Debugee is online, standing down");

            return true;
        }

        private static DebugSession GetDebugSession(IOrganizationService service, string initiatingUserId)
        {

            var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Debugee, ConditionOperator.Equal,
                initiatingUserId);
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal,
                DebugSessionState.Active.ToInt());

            var debugSession = service.RetrieveAll<DebugSession>(queryDebugSessions)
                .FirstOrDefault();
            return debugSession;
        }

        private bool StepIsInDebugSession(LocalPluginContext localContext, DebugSession debugSession)
        {
            var DebugAssemblyInfo =
                JsonConvert.DeserializeObject<List<AssemblyContextInfo>>(debugSession.AssembliesDebugInfo);

            var assemblyName = this.GetType().Assembly.GetName().Name;
            var pluginName = this.GetType().FullName;

            var assemblyInfo = DebugAssemblyInfo.FirstOrDefault(a => a.AssemblyName == assemblyName);

            var pluginInfo = assemblyInfo?.Plugins.FirstOrDefault(p => p.Name == pluginName);
            if (pluginInfo == null) return false;

            var message = localContext.MessageName.ToString();
            var stage = Enum.ToObject(typeof(Stages), localContext.Stage).ToString();
            var mode = localContext.Mode.ToString();
            var entityName = localContext.PrimaryEntityName;

            return pluginInfo.Steps.Exists(s =>
                s.Message == message
                && s.Stage == stage
                && s.Mode == mode
                && s.EntityName == entityName);
        }
    }
}
