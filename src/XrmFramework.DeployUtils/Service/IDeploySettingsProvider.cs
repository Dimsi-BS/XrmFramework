using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Service;

public interface IDeploySettingsProvider
{
    DeploySettings GetSelectedDeploySettings();
}
