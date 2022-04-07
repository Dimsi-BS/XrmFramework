using Deploy;
using System;
using System.Collections.Generic;
using System.Reflection;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public interface IAssemblyImporter
    {
        PluginAssembly CreateAssemblyFromLocal(Assembly assembly);
        Plugin CreatePluginFromType(Type type);
        Plugin CreateWorkflowFromType(Type type);
        CustomApi CreateCustomApiFromType(Type type);

        Step CreateStepFromRemote(SdkMessageProcessingStep sdkStep, IEnumerable<SdkMessageProcessingStepImage> sdkImages);
        Plugin CreatePluginFromRemote(PluginType pluginType, IEnumerable<Step> steps);
        CustomApi CreateCustomApiFromRemote(CustomApi customApi,
                                            IEnumerable<CustomApiRequestParameter> requestParameters,
                                            IEnumerable<CustomApiResponseProperty> responseProperties);
    }
}