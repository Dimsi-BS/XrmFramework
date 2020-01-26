using System;
using System.Threading.Tasks;
using XrmFramework.Debugger;

namespace XrmFramework.RemoteDebugger.Common
{
    public interface IRemoteDebuggerMessageManager
    {
        event Action<RemoteDebugExecutionContext> ContextReceived;

        Task SendMessage(RemoteDebuggerMessage message);

        Task<RemoteDebuggerMessage> SendMessageWithResponse(RemoteDebuggerMessage message);

        void RunAndBlock();
    }
}
