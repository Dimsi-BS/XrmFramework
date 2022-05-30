using XrmFramework.Remote;

namespace XrmFramework.RemoteDebugger
{
    public class RemoteDebuggerCustomApi : RemoteDebuggerPlugin
    {
        public RemoteDebuggerCustomApi(string methodName) : base(null, null)
        {
        }

        internal override IDebuggerCommunicationManager GetCommunicationManager(LocalPluginContext localContext)
         => new RemoteCustomApiDebuggerCommunicationManager(localContext);
    }
}
