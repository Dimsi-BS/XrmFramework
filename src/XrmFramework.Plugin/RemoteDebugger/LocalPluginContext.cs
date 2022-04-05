using System;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class LocalPluginContext
    {
        public RemoteDebugExecutionContext RemoteContext => new(PluginExecutionContext);

        public void LogStart()
        {
            Log($"Entity: {PrimaryEntityName}, Message: {MessageName}, Stage: {Enum.ToObject(typeof(Stages), Stage)}, Mode: {Mode}");

            Log($"\r\nUserId\t\t\t{UserId}\r\nInitiatingUserId\t{InitiatingUserId}");
            Log($"\r\nStart : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
        }

        public void LogExit()
        {
            Log($"End : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}\r\n");
        }

        public void LogContextEntry()
        {
            Log("------------------ Input Variables (before) ------------------");
            DumpInputParameters();
            Log("\r\n------------------ Shared Variables (before) ------------------");
            DumpSharedVariables();
            Log("\r\n---------------------------------------------------------------");
        }
        public void LogContextExit()
        {
            if (IsStage(Stages.PreValidation) || IsStage(Stages.PreOperation))
            {
                Log("\r\n\r\n------------------ Input Variables (after) ------------------");
                DumpInputParameters();
                Log("\r\n------------------ Shared Variables (after) ------------------");
                DumpSharedVariables();
                Log("\r\n---------------------------------------------------------------");
            }
        }
    }
}
