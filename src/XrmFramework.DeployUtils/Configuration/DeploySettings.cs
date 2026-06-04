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

        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the target CRM is an On-Premises instance.
        /// When <see langword="true" />, the deploy pipeline may use authentication
        /// mechanisms adapted to On-Premises (e.g. AD/NTLM on net462, OAuth on net8+).
        /// </summary>
        public bool IsOnPremise { get; set; }

        public string AuthType => ConnectionStringParser.GetConnectionStringField(ConnectionString, "AuthType");
        public string Url => ConnectionStringParser.GetConnectionStringField(ConnectionString, "Url");
        public string ClientId => ConnectionStringParser.GetConnectionStringField(ConnectionString, "ClientId");
        public string ClientSecret => ConnectionStringParser.GetConnectionStringField(ConnectionString, "ClientSecret");
    }
}
