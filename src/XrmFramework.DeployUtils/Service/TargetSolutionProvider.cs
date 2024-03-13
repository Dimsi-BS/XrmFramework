using Spectre.Console;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Service
{
    public class TargetSolutionProvider : ITargetSolutionProvider
    {
        private readonly string _projectName;

        public TargetSolutionProvider(string projectName)
        {
            _projectName = projectName;
        }

        public string GetTargetSolution()
        {
            var xrmFrameworkConfigSection = ConfigHelper.GetSection();

            var projectConfig = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>()
              .FirstOrDefault(p => p.Name == _projectName);

            if (projectConfig == null)
            {
                var defaultColor = Console.ForegroundColor;

                AnsiConsole.MarkupLine(
                  $@"[red]No reference to the project {_projectName} has been found in the xrmFramework.config file.[/]");
                Console.ForegroundColor = defaultColor;
                Environment.Exit(1);
            }

            return projectConfig.TargetSolution;
        }
    }
}
