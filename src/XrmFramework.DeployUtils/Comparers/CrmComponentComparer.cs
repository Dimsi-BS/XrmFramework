using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public class CrmComponentComparer : ICrmComponentComparer
    {
        private readonly StepComparer _stepComparer;

        public CrmComponentComparer()
        {
            _stepComparer = new StepComparer();
        }
        public ICrmComponent CorrespondingComponent(ICrmComponent from, IReadOnlyCollection<ICrmComponent> target)
        {
            return target.FirstOrDefault(x => Equals(from, x));
        }

        public bool Equals(ICrmComponent x, ICrmComponent y)
        {
            if (x.GetType() != y.GetType()) return false;

            return x switch
            {
                Step step => _stepComparer.Equals(step, (Step)y),
                StepImage image => _stepComparer.Equals(image.FatherStep, ((StepImage)y).FatherStep) &&
                                   image.IsPreImage == ((StepImage)y).IsPreImage,
                AssemblyInfo => true,
                _ => x.UniqueName == y.UniqueName
            };
        }

        public bool NeedsUpdate(ICrmComponent x, ICrmComponent y)
        {
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x switch
            {
                Step step => _stepComparer.NeedsUpdate(step, (Step)y),
                StepImage image => image.JoinedAttributes != ((StepImage)y).JoinedAttributes,
                CustomApiRequestParameter request => NeedsUpdate(request, (CustomApiRequestParameter)y),
                CustomApiResponseProperty response => NeedsUpdate(response, (CustomApiResponseProperty)y),
                Plugin => false,
                CustomApi api => NeedsUpdate(api, (CustomApi)y),
                AssemblyInfo => true,
                _ => throw new ArgumentException("SolutionComponent not recognised")
            };
        }


        private static bool NeedsUpdate(CustomApi x, CustomApi y)
        {
            return !x.BindingType.Equals(y.BindingType)
                || x.IsFunction ^ y.IsFunction
                || x.WorkflowSdkStepEnabled ^ y.WorkflowSdkStepEnabled
                || !x.AllowedCustomProcessingStepType.Equals(y.AllowedCustomProcessingStepType);
        }

        private static bool NeedsUpdate(CustomApiRequestParameter x, CustomApiRequestParameter y)
        {
            return !x.IsOptional == y.IsOptional
                || x.Type.Equals(y.Type);
        }

        private static bool NeedsUpdate(CustomApiResponseProperty x, CustomApiResponseProperty y)
        {
            return x.Type.Equals(y.Type);
        }

        public int GetHashCode(ICrmComponent obj)
        {
            throw new NotImplementedException();
        }
    }
}
