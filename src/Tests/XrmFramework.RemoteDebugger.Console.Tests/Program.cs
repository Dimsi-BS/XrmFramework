using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Common;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await new RemoteDebuggerStarter<Program, AzureRelayHybridConnectionMessageManager>().RunAsync(args);
    }
}