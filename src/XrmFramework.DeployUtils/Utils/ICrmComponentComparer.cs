using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public interface ICrmComponentComparer : IComparer<ICrmComponent>
    {
        ICrmComponent CorrespondingComponent(ICrmComponent component, IReadOnlyCollection<ICrmComponent> target);
        bool Equals(ICrmComponent x, ICrmComponent y);
        bool NeedsUpdate(ICrmComponent x, ICrmComponent y);
    }
}
