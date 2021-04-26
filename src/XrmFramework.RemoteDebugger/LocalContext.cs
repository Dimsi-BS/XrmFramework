using System;
using Microsoft.Xrm.Sdk;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class LocalContext
    {
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
