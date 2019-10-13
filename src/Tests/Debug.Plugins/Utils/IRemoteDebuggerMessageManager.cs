using System;
using System.Threading.Tasks;
using XrmFramework.Debugger;

namespace Debug.Plugins.Utils
{
    public interface IRemoteDebuggerMessageManager
    {
        event Action<RemoteDebugPluginExecutionContext> ContextReceived;

        Task SendMessage(RemoteDebuggerMessage message);

        Task<RemoteDebuggerMessage> SendMessageWithResponse(RemoteDebuggerMessage message);

        void RunAndBlock();
    }
}
