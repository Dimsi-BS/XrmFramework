using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Context;

namespace XrmFramework.DeployUtils.Model
{
    public interface ISolutionComponent
    {
        string UniqueName { get; }
        RegistrationState RegistrationState { get; set; }
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string EntityTypeName { get; }
        IEnumerable<ISolutionComponent> Children { get; }
        void AddChild(ISolutionComponent child);
    }
}
