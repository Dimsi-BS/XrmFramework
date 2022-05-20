using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    /// <summary>
    /// Base implementation of <see cref="ICrmComponentComparer"/>
    /// </summary>
    public class CrmComponentComparer : ICrmComponentComparer
    {
        private readonly StepComparer _stepComparer = new();

        public ICrmComponent CorrespondingComponent(ICrmComponent from, IReadOnlyCollection<ICrmComponent> target)
        {
            return target.FirstOrDefault(x => Equals(from, x));
        }

        public bool Equals(ICrmComponent x, ICrmComponent y)
        {
            if (x == null || y == null || x.GetType() != y.GetType()) return false;

            return x switch
            {
                Step step => _stepComparer.Equals(step, (Step)y),
                StepImage image => _stepComparer.Equals(image.FatherStep, ((StepImage)y).FatherStep) &&
                                   image.IsPreImage == ((StepImage)y).IsPreImage,
                _ => x.UniqueName == y.UniqueName
            };
        }

        public bool NeedsUpdate(ICrmComponent x, ICrmComponent y)
        {
            if (!Equals(x, y)) return false;

            return x switch
            {
                Step step => _stepComparer.NeedsUpdate(step, (Step)y),
                StepImage image => image.JoinedAttributes != ((StepImage)y).JoinedAttributes,
                CustomApiRequestParameter request => NeedsUpdate(request, (CustomApiRequestParameter)y),
                CustomApiResponseProperty response => NeedsUpdate(response, (CustomApiResponseProperty)y),
                Plugin => false,
                CustomApi api => NeedsUpdate(api, (CustomApi)y),
                AssemblyInfo => true,
                IAssemblyContext => true,
                _ => throw new ArgumentException("SolutionComponent not recognized")
            };
        }

        /// <summary>
        /// NeedsUpdate implementation for two <see cref="CustomApi"/>
        /// </summary>
        /// <returns>true if they need updating, false if they are exactly the same</returns>
        private static bool NeedsUpdate(CustomApi x, CustomApi y)
        {
            return !x.BindingType.Equals(y.BindingType)
                   || !x.BoundEntityLogicalName.Equals(y.BoundEntityLogicalName)
                   || x.IsFunction ^ y.IsFunction
                   || x.WorkflowSdkStepEnabled ^ y.WorkflowSdkStepEnabled
                   || !x.AllowedCustomProcessingStepType.Equals(y.AllowedCustomProcessingStepType);
        }

        /// <summary>
        /// NeedsUpdate implementation for two <see cref="CustomApiRequestParameter"/>
        /// </summary>
        /// <returns>true if they need updating, false if they are exactly the same</returns>
        private static bool NeedsUpdate(CustomApiRequestParameter x, CustomApiRequestParameter y)
        {
            return !x.IsOptional == y.IsOptional
                || !x.Type.Equals(y.Type);
        }

        /// <summary>
        /// NeedsUpdate implementation for two <see cref="CustomApiResponseProperty"/>
        /// </summary>
        /// <returns>true if they need updating, false if they are exactly the same</returns>
        private static bool NeedsUpdate(CustomApiResponseProperty x, CustomApiResponseProperty y)
        {
            return !x.Type.Equals(y.Type);
        }

        public int GetHashCode(ICrmComponent obj)
        {
            throw new NotImplementedException();
        }
    }
}
