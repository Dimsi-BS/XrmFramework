using Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public interface IFlatAssemblyContext : IAssemblyContext
    {
        public ICollection<Step> Steps { get; set; }
        public ICollection<StepImage> StepImages { get; set; }

        public ICollection<CustomApiRequestParameter> CustomApiRequestParameters { get; set; }

        public ICollection<CustomApiResponseProperty> CustomApiResponseProperties { get; set; }
    }
}
