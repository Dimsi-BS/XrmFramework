using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    interface IFlatAssemblyContext : IAssemblyContext
    {
        public ICollection<Step> Steps { get; set; }
        public ICollection<StepImage> StepImages { get; set; }

        public ICollection<CustomApiRequestParameter> CustomApiRequestParameters { get; set; }

        public ICollection<CustomApiResponseProperty> CustomApiResponseProperties { get; set; }
    }
}
