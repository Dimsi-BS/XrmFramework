using XrmFramework.RemoteDebugger.Common;

namespace XrmFramework.RemoteDebugger
{
    class Program
    {
        static void Main(string[] args)
        {
            var remoteDebugger = new RemoteDebugger<AzureRelayHybridConnectionMessageManager>();

            remoteDebugger.Start();
        }
    }
}