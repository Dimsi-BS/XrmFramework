using CommandLine;

namespace XrmFramework.DeployUtils.CommandOptions
{
    public class WebresourceCommandOptions
    {
        [Option('n', "noprompt", Required = false, HelpText = "Disable connection prompting.")]
        public bool DisablePrompt { get; set; }

        [Option('p', "path", Required = false, HelpText = "Path to the Webresources project folder")]
        public string Path { get; set; }
    }
}
