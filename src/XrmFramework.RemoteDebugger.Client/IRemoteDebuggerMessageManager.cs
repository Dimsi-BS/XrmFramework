using System;

namespace XrmFramework.RemoteDebugger.Client
{
    public interface IRemoteDebuggerMessageManager: IDisposable
    {
        void RunAndBlock();
    }
}
