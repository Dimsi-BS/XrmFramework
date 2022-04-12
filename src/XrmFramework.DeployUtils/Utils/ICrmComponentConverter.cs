using Deploy;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public interface ICrmComponentConverter
    {
        PluginType ToRegisterPluginType(Plugin plugin);
        PluginType ToRegisterCustomWorkflowType(Plugin plugin);
        SdkMessageProcessingStep ToRegisterStep(Step step);
        SdkMessageProcessingStepImage ToRegisterImage(StepImage image);
    }
}
