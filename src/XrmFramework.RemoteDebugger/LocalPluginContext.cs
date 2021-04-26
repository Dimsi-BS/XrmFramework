using Microsoft.Xrm.Sdk;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class LocalPluginContext : LocalContext
    {
                public RemoteDebugExecutionContext RemoteContext => new RemoteDebugExecutionContext(PluginExecutionContext);
    }
}
