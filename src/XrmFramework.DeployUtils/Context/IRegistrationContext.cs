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
    public interface IRegistrationContext
    {
        string SolutionName { get; }
        Solution Solution { get; set; }
        Publisher Publisher { get; set; }

        List<SolutionComponent> Components { get; set; }
        List<SdkMessageFilter> Filters { get; set; }
        Dictionary<string, EntityReference> Messages { get; }
        List<KeyValuePair<string, Guid>> Users { get; }

        void InitMetadata(IRegistrationService service);
        void InitMetadata(IRegistrationService service, string solutionName);


    }
}
