using Microsoft.Xrm.Sdk;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class LocalPluginContext : LocalContext
    {
        #if !REMOTE_DEBUGGER
        public IExecutionContext PluginExecutionContext { get; set; }
        #endif

        public RemoteDebugExecutionContext RemoteContext => new RemoteDebugExecutionContext(PluginExecutionContext);
    }
}
