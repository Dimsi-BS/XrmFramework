using Spectre.Console;
using System.Collections.Generic;
using System.Configuration;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.RemoteDebugger.Client.Services
{
    public class RemoteDebuggerDeploySettingsProvider : IDeploySettingsProvider
    {
        private static DeploySettings DeploySettings = null;

        public DeploySettings GetSelectedDeploySettings()
        {
            if (DeploySettings == null)
            {
                var connectionStrings = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");

                var connectionStringNames = new List<DeploySettings>();
                foreach (ConnectionStringSettings connection in connectionStrings.ConnectionStrings)
                {
                    if (!connection.Name.StartsWith("Xrm"))
                    {
                        continue;
                    }

                    var settings = new DeploySettings { ConnectionString = connection.ConnectionString, Name = connection.Name };

                    connectionStringNames.Add(settings);
                }

                DeploySettings = AnsiConsole
                    .Prompt(new SelectionPrompt<DeploySettings>()
                        .Title("Which connection would you like to use ?")
                        .PageSize(10)
                        .AddChoices(connectionStringNames)
                        .UseConverter(settings => $"{settings.Name} ({settings.Url})")
                    );
            }

            return DeploySettings;
        }
    }
}
