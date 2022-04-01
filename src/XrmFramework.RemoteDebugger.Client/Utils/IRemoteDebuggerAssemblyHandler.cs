using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.RemoteDebugger.Client.Utils
{
    public interface IRemoteDebuggerAssemblyHandler : IAssemblyFactory
    {
        IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, string debugAssemblyName, out Guid debugPluginId);
        IAssemblyContext CreateDebugAssemblyFromAssembly(IAssemblyContext from, Type TPlugin, Guid debugPluginId);

    }
}
