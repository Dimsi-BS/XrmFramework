namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    /// Stores the ConnectionString used for the Crm Client and the Target Solution Name
    /// </summary>
    public class SolutionSettings
    {
        public string PluginSolutionUniqueName { get; set; }
        public string ConnectionString { get; set; }
    }
}
