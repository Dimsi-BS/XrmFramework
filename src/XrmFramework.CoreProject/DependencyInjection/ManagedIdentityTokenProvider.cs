#if CORE_PROJECT

using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.AppConfig;

namespace XrmFramework.DependencyInjection
{
    /// <summary>
    /// Provides Dataverse access tokens issued to a managed identity of the hosting Azure resource.
    /// </summary>
    /// <remarks>
    /// Token acquisition, caching and renewal are delegated to MSAL, which only calls the identity endpoint
    /// when no valid token is cached. A single instance must therefore be reused for the whole application lifetime.
    /// </remarks>
    internal sealed class ManagedIdentityTokenProvider
    {
        private readonly IManagedIdentityApplication _application;

        public ManagedIdentityTokenProvider(ManagedIdentityId managedIdentityId)
        {
            _application = ManagedIdentityApplicationBuilder.Create(managedIdentityId).Build();
        }

        public async Task<string> GetAccessTokenAsync(string instanceUrl)
        {
            var authenticationResult = await _application
                .AcquireTokenForManagedIdentity(GetResource(instanceUrl))
                .ExecuteAsync()
                .ConfigureAwait(false);

            return authenticationResult.AccessToken;
        }

        private static string GetResource(string instanceUrl)
            => $"{new Uri(instanceUrl).GetLeftPart(UriPartial.Authority)}/.default";
    }
}

#endif
