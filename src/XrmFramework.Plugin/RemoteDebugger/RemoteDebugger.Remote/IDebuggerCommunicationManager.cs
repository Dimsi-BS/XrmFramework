namespace XrmFramework.RemoteDebugger
{
    public interface IDebuggerCommunicationManager
    {
        internal DebugSession GetDebugSession(LocalPluginContext localContext);
        internal void SendRemoteContextToLocal(DebugSession debugSession, LocalPluginContext localContext);

    }
}
