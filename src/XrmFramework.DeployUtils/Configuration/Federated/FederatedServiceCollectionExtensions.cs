// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Spectre.Console;

namespace XrmFramework.DeployUtils.Configuration.Federated
{
    /// <summary>
    ///     Wires the Dataverse client of a deployment that signs in through workload identity
    ///     federation.
    /// </summary>
    internal static class FederatedServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers an <see cref="IOrganizationService" /> whose tokens come from the running
        ///     job's OIDC identity, and completes <paramref name="deploySettings" /> with what was
        ///     resolved so the deployment can still print what it is about to touch.
        /// </summary>
        /// <remarks>
        ///     The configuration is resolved and validated here, eagerly, rather than inside the
        ///     service factory: a misconfigured pipeline should fail while printing the connection
        ///     summary, not several steps later from inside a lazily built client.
        /// </remarks>
        internal static IServiceCollection AddFederatedOrganizationService(
            this IServiceCollection services,
            DeploySettings deploySettings,
            DeployOptions deployOptions,
            string connectionString)
        {
            if (deployOptions.IsOnPremise)
                throw new FederatedAuthenticationException(
                    "Workload identity federation is not available on-premise. Deploy with a " +
                    "connection string, or target a Dataverse environment.");

            var settings = FederatedConnectionSettings.Create(
                connectionString, deployOptions, Environment.GetEnvironmentVariable);

            settings.Validate();

            deploySettings.Authentication = AuthenticationMode.WorkloadIdentity;
            deploySettings.AuthType = FederatedConnectionSettings.AuthTypeValue;
            deploySettings.Url = settings.Url;
            deploySettings.ClientId = settings.ClientId;

            var oidc = AzureDevOpsOidcProvider.Create(
                deployOptions,
                settings.ServiceConnectionId,
                Environment.GetEnvironmentVariable,
                log: line => AnsiConsole.WriteLine(line));

            var tokenProvider = new FederatedTokenProvider(settings, oidc);

            return services.AddScoped<IOrganizationService>(
                _ => new ServiceClient(new Uri(settings.Url), tokenProvider.TokenProviderFunction));
        }
    }
}

#endif
