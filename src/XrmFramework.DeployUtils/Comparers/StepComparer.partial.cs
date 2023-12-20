using JetBrains.Annotations;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Comparers
{
    public partial class StepComparer
    {
        /// <summary>
        /// Checks if two Steps have different non-defining properties
        /// </summary>
        /// <returns>true if they need updating, false if they are exactly the same</returns>
        ///
        /// This method is in a partial file because it is implemented differently in the RemoteDebugger.Client project

        public bool NeedsUpdate([NotNull] Step x, [NotNull] Step y) =>
            x.DoNotFilterAttributes != y.DoNotFilterAttributes
            || x.FilteringAttributes.Any() != y.FilteringAttributes.Any()
            || string.Join(",", x.FilteringAttributes) != string.Join(",", y.FilteringAttributes)
            || x.Order != y.Order
            || x.ImpersonationUsername != y.ImpersonationUsername;
    }
}
