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

        /// <summary>
        /// How the assembly reaches Dataverse, from the <c>deployment</c> attribute of the
        /// project in <c>xrmFramework.config</c>.
        /// </summary>
        public DeploymentMode Deployment { get; set; } = DeploymentMode.Assembly;

        /// <summary>Shorthand for <see cref="Deployment" /> being <see cref="DeploymentMode.Package" />.</summary>
        public bool UsePluginPackage => Deployment == DeploymentMode.Package;

        /// <summary>
        /// How the deployment proved who it is, once resolved - never
        /// <see cref="AuthenticationMode.Auto" /> by the time the pipeline reads it.
        /// </summary>
        public AuthenticationMode Authentication { get; set; } = AuthenticationMode.ConnectionString;

        /// <summary>
        /// Target of the deployment, and the identity it used, for the connection summary the
        /// deployment prints before it writes anything.
        /// </summary>
        /// <remarks>
        /// Settable, and falling back to the connection string when nothing was set: under
        /// workload identity federation the URL and the application come from the command line or
        /// from the job's variables, and <see cref="ConnectionString" /> is null.
        /// </remarks>
        public string AuthType
        {
            get => _authType ?? Field("AuthType");
            set => _authType = value;
        }

        /// <inheritdoc cref="AuthType" />
        public string Url
        {
            get => _url ?? Field("Url");
            set => _url = value;
        }

        /// <inheritdoc cref="AuthType" />
        public string ClientId
        {
            get => _clientId ?? Field("ClientId");
            set => _clientId = value;
        }

        public string ClientSecret => Field("ClientSecret");

        private string _authType;
        private string _url;
        private string _clientId;

        private string Field(string name)
            => string.IsNullOrWhiteSpace(ConnectionString)
                ? null
                : ConnectionStringParser.GetConnectionStringField(ConnectionString, name);
    }
}
