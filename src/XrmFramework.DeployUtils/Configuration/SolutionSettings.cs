using System.Linq;

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

        public string AuthType => GetConnectionStringField("AuthType");
        public string Url => GetConnectionStringField("Url");
        public string ClientId => GetConnectionStringField("ClientId");
        public string ClientSecret => GetConnectionStringField("ClientSecret");


        private string GetConnectionStringField(string field) => ConnectionString
        .Split(';')
        .FirstOrDefault(s => s.TrimStart().StartsWith(field))
        ?.Split('=')[1];
    }
}
