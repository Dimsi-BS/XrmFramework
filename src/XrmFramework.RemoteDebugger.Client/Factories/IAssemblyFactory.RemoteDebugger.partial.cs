using System;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils
{
    public partial interface IAssemblyFactory
    {
        IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, string debugAssemblyName, out Guid debugPluginId);
        IAssemblyContext CreateDebugAssemblyFromAssembly(IAssemblyContext from, Type TPlugin);

    }
}
