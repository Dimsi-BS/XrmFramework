using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public interface ICrmComponentComparer : IEqualityComparer<ICrmComponent>
    {
        ICrmComponent CorrespondingComponent(ICrmComponent component, IReadOnlyCollection<ICrmComponent> target);
        bool NeedsUpdate(ICrmComponent x, ICrmComponent y);
    }
}
