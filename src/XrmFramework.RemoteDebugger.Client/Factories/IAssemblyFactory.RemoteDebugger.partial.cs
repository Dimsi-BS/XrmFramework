using System;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils
{
    public partial interface IAssemblyFactory
    {
        IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, string debugAssemblyName, out Guid debugPluginId);
        IAssemblyContext WrapDebugDiffForDebugDeploy(IAssemblyContext from, Guid debugPluginId, Type TPlugin);
        IAssemblyContext WrapDiffAssemblyForDebugDiff(IAssemblyContext deployAssemblyDiff);
    }
}
