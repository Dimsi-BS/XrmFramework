using System.Configuration;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Service
{
    public class DeploySettingsProvider : IDeploySettingsProvider
    {
        private readonly ITargetSolutionProvider _targetSolutionProvider;

        public DeploySettingsProvider(ITargetSolutionProvider targetSolutionProvider)
        {
            _targetSolutionProvider = targetSolutionProvider;
        }

        public DeploySettings GetSelectedDeploySettings()
        {
            var xrmFrameworkConfigSection = ConfigHelper.GetSection();
            var connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection]
              .ConnectionString;

            var targetSolutionName = _targetSolutionProvider.GetTargetSolution();

            return new DeploySettings
            { 
                ConnectionString = connectionString,
                PluginSolutionUniqueName = targetSolutionName
            };
        }
    }
}
