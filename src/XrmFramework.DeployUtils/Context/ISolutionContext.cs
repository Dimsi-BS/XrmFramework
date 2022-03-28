using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Context
{
    public interface ISolutionContext
    {
        string SolutionName { get; }
        Solution Solution { get; }
        Publisher Publisher { get; }

        List<SolutionComponent> Components { get; }
        List<SdkMessageFilter> Filters { get; }
        Dictionary<string, EntityReference> Messages { get; }
        List<KeyValuePair<string, Guid>> Users { get; }

        void InitMetadata();


    }
}
