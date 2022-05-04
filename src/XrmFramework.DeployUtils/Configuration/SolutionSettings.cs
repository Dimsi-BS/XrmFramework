namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    /// Stores the ConnectionString used for the Crm Client and the Target Solution Name
    /// </summary>
    public class SolutionSettings
    {
        /// <summary>Target Solution Name</summary>
        public string PluginSolutionUniqueName { get; set; }

        /// <summary>Connection String to use to instantiate a Crm Client</summary>
        public string ConnectionString { get; set; }
    }
}
