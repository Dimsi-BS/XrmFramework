using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils
{
    public interface IAssemblyFactory
    {
        IAssemblyContext CreateFromLocalAssemblyContext(Type TPlugin);
        IAssemblyContext CreateFromRemoteAssemblyContext(IRegistrationService service, string assemblyName);
        IFlatAssemblyContext CreateFlatAssemblyContextFromAssemblyContext(IAssemblyContext from);
    }
}
