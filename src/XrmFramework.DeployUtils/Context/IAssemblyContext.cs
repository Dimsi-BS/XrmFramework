using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public interface IAssemblyContext
    {
        string Name { get; }
        ICollection<CustomApi> CustomApis { get; set; }
        ICollection<CustomApiRequestParameter> CustomApiRequestParameters { get; set; }
        ICollection<CustomApiResponseProperty> CustomApiResponseProperties { get; set; }

    }
}