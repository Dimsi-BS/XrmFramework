using Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public class CrmComponentComparer : ICrmComponentComparer
    {
        private readonly StepComparer _stepComparer;
        private readonly List<Type> _CrmComponentTypeOrder;

        public CrmComponentComparer()
        {
            _stepComparer = new StepComparer();
            _CrmComponentTypeOrder = new List<Type>
            {
                typeof(PluginAssembly),
                typeof(Plugin),
                typeof(Step),
                typeof(StepImage),
                typeof(CustomApi),
                typeof(ICustomApiComponent)
            };
        }
        public ICrmComponent CorrespondingComponent(ICrmComponent from, IReadOnlyCollection<ICrmComponent> target)
        {
            return target.FirstOrDefault(x => Equals(from, x));
        }

        public bool Equals(ICrmComponent x, ICrmComponent y)
        {
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case Step step:
                    return _stepComparer.Equals(step, (Step)y);
                case StepImage image:
                    return _stepComparer.Equals(image.FatherStep, ((StepImage)y).FatherStep)
                         && image.IsPreImage == ((StepImage)y).IsPreImage;
                default:
                    return x.UniqueName == y.UniqueName;
            }
        }

        public bool NeedsUpdate(ICrmComponent x, ICrmComponent y)
        {
            if (x.GetType() != y.GetType())
            {
                return false;
            }
            switch (x)
            {
                case Step step:
                    return _stepComparer.NeedsUpdate(step, (Step)y);
                case StepImage image:
                    return image.JoinedAttributes != ((StepImage)y).JoinedAttributes;
                case ICustomApiComponent component:
                    return !component.Type.Equals(((ICustomApiComponent)y).Type);
                case Plugin:
                    return false;
                case CustomApi api:
                    return NeedsUpdate(api, (CustomApi)y);
                case PluginAssembly:
                    return true;
                default:
                    throw new ArgumentException("SolutionComponent not recognised");
            }
        }

        public int Compare(ICrmComponent x, ICrmComponent y)
        {
            var xIndex = _CrmComponentTypeOrder.IndexOf(x.GetType());
            var yIndex = _CrmComponentTypeOrder.IndexOf(y.GetType());
            return xIndex.CompareTo(yIndex);

        }

        private bool NeedsUpdate(CustomApi x, CustomApi y)
        {
            return !x.BindingType.Equals(y.BindingType)
                || x.IsFunction ^ y.IsFunction
                || x.WorkflowSdkStepEnabled ^ y.WorkflowSdkStepEnabled
                || !x.AllowedCustomProcessingStepType.Equals(y.AllowedCustomProcessingStepType);
        }

        public int GetHashCode(ICrmComponent obj)
        {
            throw new NotImplementedException();
        }
    }
}
