using System;
using System.Collections.Generic;
using System.Reflection;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    interface IAssemblyImporter
    {
        IAssemblyContext CreateAssemblyFromLocal(Assembly assembly);
        IAssemblyContext CreateAssemblyFromRemote(Deploy.PluginAssembly assembly);

        Plugin CreatePluginFromType(Type type);
        Plugin CreateWorkflowFromType(Type type);
        CustomApi CreateCustomApiFromType(Type type);

        Step CreateStepFromRemote(Deploy.SdkMessageProcessingStep sdkStep, IEnumerable<Deploy.SdkMessageProcessingStepImage> sdkImages);
        Plugin CreatePluginFromRemote(Deploy.PluginType pluginType, IEnumerable<Step> steps);
        CustomApi CreateCustomApiFromRemote(Deploy.CustomApi customApi,
                                            IEnumerable<Deploy.CustomApiRequestParameter> requestParameters,
                                            IEnumerable<Deploy.CustomApiResponseProperty> responseProperties);
    }
}