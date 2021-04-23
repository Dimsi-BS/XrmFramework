using System;
using Microsoft.Xrm.Sdk;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class LocalContext
    {
#if !REMOTE_DEBUGGER
        public IExecutionContext ExecutionContext { get; set; }

        public IOrganizationService AdminOrganizationService { get; set; }

        public void Log(string message, params object[] args) { }

        public IOrganizationService GetService(Guid userId) => null;

        public Guid GetInitiatingUserId() => Guid.Empty;

        public LogServiceMethod LogServiceMethod { get; set; }
#endif

        public bool IsDebugContext => ExecutionContext.GetType().FullName == typeof(RemoteDebugExecutionContext).FullName;

        public void UpdateContext(RemoteDebugExecutionContext updatedContext)
        {
            ExecutionContext.InputParameters.Clear();
            ExecutionContext.InputParameters.AddRange(updatedContext.InputParameters);

            ExecutionContext.OutputParameters.Clear();
            ExecutionContext.OutputParameters.AddRange(updatedContext.OutputParameters);

            ExecutionContext.SharedVariables.Clear();
            ExecutionContext.SharedVariables.AddRange(updatedContext.SharedVariables);

            Log("Context updated.");
        }
    }
}
