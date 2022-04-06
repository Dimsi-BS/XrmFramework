using System;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.RemoteDebugger.Client.Utils
{
    public interface IRemoteDebuggerAssemblyHandler : IAssemblyFactory
    {
        IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, string debugAssemblyName, out Guid debugPluginId);
        IAssemblyContext CreateDebugAssemblyFromAssembly(IAssemblyContext from, Type TPlugin);

    }
}
