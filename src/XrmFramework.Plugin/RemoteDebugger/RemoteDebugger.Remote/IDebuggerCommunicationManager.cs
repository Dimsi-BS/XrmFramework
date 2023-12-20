namespace XrmFramework.RemoteDebugger
{
    public interface IDebuggerCommunicationManager
    {
        internal DebugSession? GetDebugSession();
        internal void SendLocalContextToDebugSession(DebugSession debugSession);

    }
}
