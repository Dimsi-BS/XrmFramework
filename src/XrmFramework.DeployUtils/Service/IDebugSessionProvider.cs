using XrmFramework.RemoteDebugger;

namespace XrmFramework.DeployUtils.Service;

public interface IDebugSessionProvider
{
    DebugSession DebugSession { get; }
}
