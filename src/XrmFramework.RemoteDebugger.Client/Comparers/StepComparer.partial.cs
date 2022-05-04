using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public partial class StepComparer
    {
        /// <summary>
        /// Checks if two Steps have different non-defining properties
        /// </summary>
        /// <remarks>This method doesn't check for Equality before comparing, but only checks if they are of the same <c>Type</c></remarks>
        /// <returns>true if they need updating, false if they are exactly the same</returns>
        ///
        /// This method is in a partial file because it is implemented differently in the DeployUtils
        public bool NeedsUpdate(Step x, Step y) =>
            x?.DoNotFilterAttributes != y?.DoNotFilterAttributes
            || x.FilteringAttributes.Any() != x.FilteringAttributes.Any()
            || string.Join(",", x.FilteringAttributes) != string.Join(",", y.FilteringAttributes)
            || x.ImpersonationUsername != y.ImpersonationUsername
            || !MethodsNamesEqual(x, y);

        private static bool MethodsNamesEqual(Step x, Step y)
        {
            x.MethodNames.Sort();
            y.MethodNames.Sort();
            return x.MethodNames.SequenceEqual(y.MethodNames);
        }
    }
}