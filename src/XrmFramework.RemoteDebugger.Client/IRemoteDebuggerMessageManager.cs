using System;
using System.Threading.Tasks;
using XrmFramework.RemoteDebugger.Client.Recorder;

namespace XrmFramework.RemoteDebugger.Common
{
    public interface IRemoteDebuggerMessageManager
    {
        event Action<RemoteDebugExecutionContext> ContextReceived;

        Task SendMessage(RemoteDebuggerMessage message);

        Task<RemoteDebuggerMessage> SendMessageWithResponse(RemoteDebuggerMessage message);
        
        void SetSessionRecorder(ISessionRecorder sessionRecorder);

        void RunAndBlock();
    }
}
