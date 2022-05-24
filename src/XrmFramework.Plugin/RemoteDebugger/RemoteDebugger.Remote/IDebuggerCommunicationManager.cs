namespace XrmFramework.RemoteDebugger
{
    public interface IDebuggerCommunicationManager
    {
        internal DebugSession GetDebugSession(LocalPluginContext localContext);
        internal void SendLocalContextToDebugSession(DebugSession debugSession, LocalPluginContext localContext);

    }
}
