using Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Model;
using static XrmFramework.DeployUtils.Model.StepCollection;

namespace XrmFramework.DeployUtils.Utils
{
    public class AssemblyComparer
    {
        public static readonly StepComparer _stepComparer = new();
        public static ISolutionComponent CorrespondingComponent<T>(T from, IEnumerable<T> target) where T : ISolutionComponent
        {
            return target.FirstOrDefault(x => ComponentEqual(from, x));
        }

        private static bool ComponentEqual(ISolutionComponent x, ISolutionComponent y)
        {
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case Step:
                    return _stepComparer.Equals((Step)x, (Step)y);
                case StepImage:
                    return ((StepImage)x).IsPreImage == ((StepImage)y).IsPreImage;
                default:
                    return x.UniqueName == y.UniqueName;
            }
        }

        public static bool NeedsUpdate(ISolutionComponent x, ISolutionComponent y)
        {
            if (x.GetType() != y.GetType()) return false;
            switch (x)
            {
                case Plugin:
                    return false;
                case Step step:
                    return _stepComparer.NeedsUpdate(step, (Step)y);
                case StepImage image:
                    return image.JoinedAttributes != ((StepImage)y).JoinedAttributes;
                case CustomApi api:
                    return NeedsUpdate(api, (CustomApi)y);
                case ICustomApiComponent component:
                    return NeedsUpdate(component, (ICustomApiComponent)y);
                default:
                    throw new ArgumentException("SolutionComponent not recognised");
            }
        }

        public static bool NeedsUpdate(ICustomApiComponent x, ICustomApiComponent y)
        {
            return !x.Type.Equals(y.Type);
        }

        public static bool NeedsUpdate(CustomApi x, CustomApi y)
        {
            return !x.BindingType.Equals(y.BindingType)
                || x.IsFunction ^ y.IsFunction
                || x.WorkflowSdkStepEnabled ^ y.WorkflowSdkStepEnabled
                || !x.AllowedCustomProcessingStepType.Equals(y.AllowedCustomProcessingStepType);
        }
    }
}
