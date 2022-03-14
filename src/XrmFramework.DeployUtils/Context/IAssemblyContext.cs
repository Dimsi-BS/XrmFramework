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
        IEnumerable<SdkMessageProcessingStep> Steps { get; set; }
        IEnumerable<CustomApi> CustomApis { get; set; }
        IEnumerable<CustomApiRequestParameter> CustomApiRequestParameters { get; set; }
        IEnumerable<CustomApiResponseProperty> CustomApiResponseProperties { get; set; }

    }
}