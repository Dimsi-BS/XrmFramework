#if PLUGIN || CORE_PROJECT

using Microsoft.Extensions.DependencyInjection;
#if CORE_PROJECT
using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client.AppConfig;
#endif

namespace XrmFramework.DependencyInjection
{
    public interface IXrmFrameworkOptionBuilder
    {
        IXrmFrameworkOptionBuilder UseConnectionString(string connectionString);
        IXrmFrameworkOptionBuilder UseWebApi(bool useWebApi);

#if CORE_PROJECT
        /// <summary>
        /// Connects to the given Dataverse environment using the system assigned managed identity of the hosting Azure resource.
        /// </summary>
        /// <param name="url">Url of the Dataverse environment to connect to, ie. https://myorg.crm4.dynamics.com</param>
        IXrmFrameworkOptionBuilder UseManagedIdentity(string url);

        /// <summary>
        /// Connects to the given Dataverse environment using a user assigned managed identity, identified by its client id.
        /// </summary>
        /// <param name="url">Url of the Dataverse environment to connect to, ie. https://myorg.crm4.dynamics.com</param>
        /// <param name="userAssignedClientId">Client id of the user assigned managed identity.</param>
        IXrmFrameworkOptionBuilder UseManagedIdentity(string url, string userAssignedClientId);

        /// <summary>
        /// Connects to the given Dataverse environment using the given managed identity.
        /// </summary>
        /// <param name="url">Url of the Dataverse environment to connect to, ie. https://myorg.crm4.dynamics.com</param>
        /// <param name="managedIdentityId">
        /// Managed identity to authenticate with, ie. <see cref="ManagedIdentityId.SystemAssigned"/>,
        /// <see cref="ManagedIdentityId.WithUserAssignedClientId"/>, <see cref="ManagedIdentityId.WithUserAssignedResourceId"/>
        /// or <see cref="ManagedIdentityId.WithUserAssignedObjectId"/>.
        /// </param>
        IXrmFrameworkOptionBuilder UseManagedIdentity(string url, ManagedIdentityId managedIdentityId);

        /// <summary>
        /// Connects to the given Dataverse environment using access tokens provided by the caller. Use this to plug in
        /// any authentication flow not covered by the framework, such as an on behalf of flow or a workload identity.
        /// </summary>
        /// <param name="url">Url of the Dataverse environment to connect to, ie. https://myorg.crm4.dynamics.com</param>
        /// <param name="tokenProviderFunction">
        /// Function called whenever an access token is required, it receives the environment url and returns the access token.
        /// It is called repeatedly, caching the token until it expires is the responsibility of the implementation.
        /// </param>
        IXrmFrameworkOptionBuilder UseAccessTokenProvider(string url, Func<string, Task<string>> tokenProviderFunction);
#endif
    }

    public class XrmFrameworkOptionBuilder : IXrmFrameworkOptionBuilder
    {
        public IServiceCollection ServiceCollection { get; }

        public XrmFrameworkOptionBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        internal string ConnectionString { get; private set; }

        internal bool WebApiUsage { get; private set; }

        internal bool UseWebApiForced { get; private set; }

        public IXrmFrameworkOptionBuilder UseConnectionString(string connectionString)
        {
            ConnectionString = connectionString;

            return this;
        }

        public IXrmFrameworkOptionBuilder UseWebApi(bool useWebApi)
        {
            WebApiUsage = useWebApi;
            UseWebApiForced = true;

            return this;
        }

#if CORE_PROJECT
        internal Uri Url { get; private set; }

        internal Func<string, Task<string>> AccessTokenProviderFunction { get; private set; }

        internal bool UsesCallerManagedAuthentication => AccessTokenProviderFunction != null;

        /// <summary>
        /// Whether each <see cref="Microsoft.PowerPlatform.Dataverse.Client.ServiceClient"/> must open its own connection.
        /// A managed identity always issues tokens for the same application, so its connection can safely be shared,
        /// whereas a caller supplied token provider may issue tokens on behalf of the current user.
        /// </summary>
        internal bool RequireNewServiceClientInstance { get; private set; }

        public IXrmFrameworkOptionBuilder UseManagedIdentity(string url)
            => UseManagedIdentity(url, ManagedIdentityId.SystemAssigned);

        public IXrmFrameworkOptionBuilder UseManagedIdentity(string url, string userAssignedClientId)
            => UseManagedIdentity(url, ManagedIdentityId.WithUserAssignedClientId(userAssignedClientId));

        public IXrmFrameworkOptionBuilder UseManagedIdentity(string url, ManagedIdentityId managedIdentityId)
        {
            if (managedIdentityId == null)
            {
                throw new ArgumentNullException(nameof(managedIdentityId));
            }

            var tokenProvider = new ManagedIdentityTokenProvider(managedIdentityId);

            return UseAccessTokenProvider(url, tokenProvider.GetAccessTokenAsync, false);
        }

        public IXrmFrameworkOptionBuilder UseAccessTokenProvider(string url, Func<string, Task<string>> tokenProviderFunction)
            => UseAccessTokenProvider(url, tokenProviderFunction, true);

        private IXrmFrameworkOptionBuilder UseAccessTokenProvider(string url, Func<string, Task<string>> tokenProviderFunction, bool requireNewServiceClientInstance)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var instanceUrl))
            {
                throw new ArgumentException($"'{url}' is not a valid Dataverse environment url.", nameof(url));
            }

            Url = instanceUrl;
            AccessTokenProviderFunction = tokenProviderFunction ?? throw new ArgumentNullException(nameof(tokenProviderFunction));
            RequireNewServiceClientInstance = requireNewServiceClientInstance;

            return this;
        }
#endif
    }
}

#endif
