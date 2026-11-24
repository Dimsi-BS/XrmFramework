// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace XrmFramework.DeployUtils.Configuration.Federated
{
    /// <summary>
    ///     Exchanges the CI/CD job's OIDC token for a Dataverse access token, and hands that token
    ///     to <c>ServiceClient</c> on demand.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A Dataverse connection string cannot express a client assertion, so the deployment
    ///         does not build one: it passes <c>ServiceClient</c> the token-provider function this
    ///         class exposes. Nothing secret is configured, stored or logged along the way.
    ///     </para>
    ///     <para>
    ///         MSAL owns the caching. It calls the assertion delegate only when it has no cached
    ///         token for the scope, so a long deployment - pushing a plugin package takes minutes -
    ///         gets a fresh OIDC token when, and only when, it needs one.
    ///     </para>
    /// </remarks>
    public sealed class FederatedTokenProvider
    {
        private readonly IConfidentialClientApplication _application;
        private readonly IOidcTokenProvider _oidc;
        private readonly FederatedConnectionSettings _settings;

        public FederatedTokenProvider(FederatedConnectionSettings settings, IOidcTokenProvider oidc)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _oidc = oidc ?? throw new ArgumentNullException(nameof(oidc));

            settings.Validate();

            _settings = settings;

            _application = ConfidentialClientApplicationBuilder
                .Create(settings.ClientId)
                .WithTenantId(settings.TenantId)
                .WithClientAssertion(options => _oidc.GetAssertionAsync(options.CancellationToken))
                .Build();
        }

        /// <summary>
        ///     The function to hand to <c>ServiceClient</c>: it receives the instance URL and
        ///     answers a bearer token for it.
        /// </summary>
        public Func<string, Task<string>> TokenProviderFunction => GetTokenAsync;

        /// <summary>
        ///     Acquires a Dataverse token for <paramref name="instanceUrl" />.
        /// </summary>
        /// <param name="instanceUrl">
        ///     Instance <c>ServiceClient</c> is about to call. Falls back to the configured
        ///     organization URL, for the calls that pass nothing.
        /// </param>
        public async Task<string> GetTokenAsync(string instanceUrl)
        {
            var scope = _settings.ScopeFor(instanceUrl);

            try
            {
                var result = await _application
                    .AcquireTokenForClient(new[] { scope })
                    .ExecuteAsync(CancellationToken.None)
                    .ConfigureAwait(false);

                return result.AccessToken;
            }
            catch (MsalServiceException ex)
            {
                throw new FederatedAuthenticationException(
                    $"Microsoft Entra refused the federated sign-in for {scope} ({ex.ErrorCode}): {ex.Message} " +
                    Explain(),
                    ex);
            }
            catch (MsalClientException ex)
            {
                throw new FederatedAuthenticationException(
                    $"The federated sign-in for {scope} could not be completed: {ex.Message}",
                    ex);
            }
        }

        /// <summary>
        ///     Points at the one thing Entra rejects a valid OIDC token for: a federated credential
        ///     that does not match this job. The provider knows the exact values to use once it has
        ///     read a token, so they are repeated here rather than left to be guessed.
        /// </summary>
        private string Explain()
        {
            if (_oidc.Issuer == null || _oidc.Subject == null)
                return $"Check that the application is trusted by the {_oidc.PlatformName} job " +
                       "through a federated credential, and that it exists as an application user " +
                       "in Dataverse with a security role.";

            return "Check that the application's federated credential uses issuer " +
                   $"\"{_oidc.Issuer}\" and subject \"{_oidc.Subject}\", and that the " +
                   "application exists as an application user in Dataverse with a security role.";
        }
    }
}

#endif
