using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public partial class StepComparer : IEqualityComparer<Step>
    {
        public bool Equals(Step x, Step y) =>
            x == null && y == null
            ||
            x?.PluginTypeFullName == y?.PluginTypeFullName
            && x?.EntityName == y?.EntityName
            && x?.Message == y?.Message
            && x?.Stage == y?.Stage
            && x.Mode == y.Mode;

        public int GetHashCode(Step obj)
            => obj.PluginTypeName.GetHashCode()
               + obj.EntityName.GetHashCode()
               + obj.Message.GetHashCode()
               + obj.Stage.GetHashCode()
               + obj.Mode.GetHashCode();
    }
}
