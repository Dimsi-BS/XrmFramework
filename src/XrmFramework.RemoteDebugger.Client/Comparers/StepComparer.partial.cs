using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public partial class StepComparer
    {
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