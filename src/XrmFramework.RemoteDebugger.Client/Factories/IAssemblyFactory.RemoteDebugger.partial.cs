using System;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.RemoteDebugger.Client.Configuration;

namespace XrmFramework.DeployUtils.Utils
{
    public partial interface IAssemblyFactory
    {
        IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, DebugAssemblySettings debugSettings);
        IAssemblyContext WrapDebugDiffForDebugDeploy(IAssemblyContext from, DebugAssemblySettings debugAssemblySettings, Type TPlugin);
        IAssemblyContext WrapDiffAssemblyForDebugDiff(IAssemblyContext deployAssemblyDiff);
    }
}
