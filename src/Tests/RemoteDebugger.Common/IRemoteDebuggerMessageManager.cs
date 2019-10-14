using System;
using System.Threading.Tasks;
using XrmFramework.Debugger;

namespace RemoteDebugger.Common
{
    public interface IRemoteDebuggerMessageManager<T> where T : RemoteDebugExecutionContext
    {
        event Action<T> ContextReceived;

        Task SendMessage(RemoteDebuggerMessage message);

        Task<RemoteDebuggerMessage> SendMessageWithResponse(RemoteDebuggerMessage message);

        void RunAndBlock();
    }
}
