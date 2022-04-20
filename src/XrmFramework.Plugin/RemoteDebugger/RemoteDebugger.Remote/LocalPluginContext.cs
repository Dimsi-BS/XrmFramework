using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class LocalPluginContext
    {
        public RemoteDebugExecutionContext RemoteContext => new(PluginExecutionContext);
    }
}
