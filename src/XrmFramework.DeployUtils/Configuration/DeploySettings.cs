namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    /// Stores the ConnectionString used for the Crm Client and the Target Solution Name
    /// </summary>
    public class DeploySettings
    {

        /// <summary>Target Solution Name</summary>
        public string PluginSolutionUniqueName { get; set; }

        /// <summary>Connection String to use to instantiate a Crm Client</summary>
        public string ConnectionString { get; set; }

        public string AuthType => ConnectionStringParser.GetConnectionStringField(ConnectionString, "AuthType");
        public string Url => ConnectionStringParser.GetConnectionStringField(ConnectionString, "Url");
        public string ClientId => ConnectionStringParser.GetConnectionStringField(ConnectionString, "ClientId");
        public string ClientSecret => ConnectionStringParser.GetConnectionStringField(ConnectionString, "ClientSecret");

    }
}
