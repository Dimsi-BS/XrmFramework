using Deploy;
using Microsoft.Xrm.Sdk;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public interface ICrmComponentConverter
    {
        Entity ToRegisterComponent(ICrmComponent component);
        PluginType ToRegisterPluginType(Plugin plugin);
        SdkMessageProcessingStep ToRegisterStep(Step step);
        SdkMessageProcessingStepImage ToRegisterImage(StepImage image);
    }
}
