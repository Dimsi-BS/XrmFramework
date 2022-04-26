using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                return false;
            }

            var initiatingUserId = localContext.GetInitiatingUserId().ToString();

            localContext.Log("The context is genuine");

            var debugSession = GetDebugSession(localContext.AdminOrganizationService, initiatingUserId);

            if (debugSession == null)
            {
                localContext.Log("Debug session is null");
                return false;
            }

            if (initiatingUserId != debugSession.Debugee)
            {
                localContext.Log("Is currently debugging but not for this user, execute the step normally");
                return false;
            }

            if (StepIsInDebugSession(localContext, debugSession) && ListenerIsOnline(debugSession))
            {
                localContext.Log($"Debug session : {debugSession}");
                localContext.Log("Is currently debugging this step, standing down");
                return true;
            }
            // We have to check whether the remoteDebugger has this step, if not we need to send context to remote debugger ourselves

            //Context will be sent by remote debugger plugin

            return false;
        }

        private static bool ListenerIsOnline(DebugSession debugSession)
        {
            var uri = new Uri($"{debugSession.RelayUrl}/{debugSession.HybridConnectionName}");

            var isOnline = false;

            try
            {
                using var hybridConnection = new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri);

                var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Ping, null, Guid.NewGuid());

                var response = hybridConnection.SendMessage(message).GetAwaiter().GetResult();

                isOnline = response.MessageType == RemoteDebuggerMessageType.Ping;
            }
            catch (HttpRequestException)
            {
                //isOnline is false and will stay that way
            }
            return isOnline;
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
