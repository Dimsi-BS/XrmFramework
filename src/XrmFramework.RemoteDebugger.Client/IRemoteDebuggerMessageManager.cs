using System;
using System.Threading.Tasks;

namespace XrmFramework.RemoteDebugger.Client
{
    public interface IRemoteDebuggerMessageManager: IDisposable
    {
        Task RunAsync();
    }
}
