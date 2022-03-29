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
        RegistrationState RegistrationState { get; set; }
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string EntityTypeName { get; }
    }
}
