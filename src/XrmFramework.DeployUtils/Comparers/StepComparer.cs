using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    /// <summary>
    /// Compares and Determines whether the specified Steps are equal or need updating
    /// </summary>
    public partial class StepComparer : IEqualityComparer<Step>
    {
        public bool Equals(Step x, Step y) =>
            x == null && y == null
            ||
            x?.PluginTypeFullName == y?.PluginTypeFullName
            && x?.PluginTypeName == y?.PluginTypeName
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
