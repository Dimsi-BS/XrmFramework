// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.DeployUtils.Configuration.Federated
{
    /// <summary>
    ///     Identity of a deployment that signs in through workload identity federation:
    ///     <c>AuthType=WorkloadIdentity</c>.
    /// </summary>
    /// <remarks>
    ///     Every field can equally come from the command line, from the connection string, or from
    ///     the environment the CI/CD job exposes - in that order of precedence. None of them is a
    ///     secret, which is the point: such an entry can be committed, unlike the
    ///     <c>ClientSecret</c> ones <c>connectionStrings.config</c> is gitignored for.
    ///     <code>
    ///     &lt;add name="DevOps" connectionString="AuthType=WorkloadIdentity; Url=https://org.crm4.dynamics.com" /&gt;
    ///     </code>
    ///     In a pipeline there need not be a <c>connectionStrings.config</c> at all: the URL comes
    ///     from <c>--environment</c> and the rest from the job's own variables.
    /// </remarks>
    public sealed class FederatedConnectionSettings
    {
        /// <summary>Value of <c>AuthType</c> that routes a connection through federation.</summary>
        public const string AuthTypeValue = "WorkloadIdentity";

        /// <summary>Entra tenant of the app registration, as Azure Pipelines publishes it.</summary>
        public const string TenantIdVariable = "AZURESUBSCRIPTION_TENANT_ID";

        /// <summary>Client id of the app registration, as Azure Pipelines publishes it.</summary>
        public const string ClientIdVariable = "AZURESUBSCRIPTION_CLIENT_ID";

        /// <summary>Service connection the federated credential is bound to.</summary>
        public const string ServiceConnectionIdVariable = "AZURESUBSCRIPTION_SERVICE_CONNECTION_ID";

        /// <summary>
        ///     Client id as <c>AzureCLI@2</c> with <c>addSpnToEnvironment: true</c> exports it.
        ///     Spelled exactly as the task spells it: environment variables are case-sensitive on
        ///     the Linux agents.
        /// </summary>
        public const string AzureCliClientIdVariable = "servicePrincipalId";

        /// <summary>Tenant as <c>AzureCLI@2</c> with <c>addSpnToEnvironment: true</c> exports it.</summary>
        public const string AzureCliTenantIdVariable = "tenantId";

        private FederatedConnectionSettings()
        {
        }

        /// <summary>Dataverse organization URL to deploy to.</summary>
        public string Url { get; private set; }

        /// <summary>Entra tenant to sign in to.</summary>
        public string TenantId { get; private set; }

        /// <summary>Entra application the OIDC token is exchanged for a Dataverse token on behalf of.</summary>
        public string ClientId { get; private set; }

        /// <summary>
        ///     Azure DevOps service connection whose federated credential trusts this job, or
        ///     <see langword="null" /> when the OIDC endpoint is given whole and needs no building.
        /// </summary>
        public string ServiceConnectionId { get; private set; }

        /// <summary>
        ///     <see langword="true" /> when <paramref name="connectionString" /> declares
        ///     <c>AuthType=WorkloadIdentity</c>.
        /// </summary>
        public static bool IsFederatedConnectionString(string connectionString)
            => AuthTypeValue.Equals(AuthTypeOf(connectionString), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     Reads the <c>AuthType</c> of a connection string, tolerating a missing or malformed
        ///     one rather than throwing: deciding how to authenticate happens before anything has
        ///     established that a connection string exists at all.
        /// </summary>
        public static string AuthTypeOf(string connectionString)
            => Field(connectionString, "AuthType") ?? Field(connectionString, "AuthenticationType");

        /// <summary>
        ///     Resolves the settings from the three sources, command line first, environment last.
        /// </summary>
        /// <param name="connectionString">
        ///     Selected connection string, or <see langword="null" /> when the project has none.
        /// </param>
        /// <param name="options">Deployment options carrying the command-line overrides.</param>
        /// <param name="environment">
        ///     Environment variable reader, injected so the precedence can be tested without
        ///     touching the process environment.
        /// </param>
        public static FederatedConnectionSettings Create(
            string connectionString,
            DeployOptions options,
            Func<string, string> environment)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            return new FederatedConnectionSettings
            {
                Url = TrimUrl(First(
                    options.EnvironmentUrl,
                    Field(connectionString, "Url"),
                    Field(connectionString, "ServiceUri"))),

                TenantId = First(
                    options.TenantId,
                    Field(connectionString, "TenantId"),
                    environment(TenantIdVariable),
                    environment(AzureCliTenantIdVariable)),

                ClientId = First(
                    options.ApplicationId,
                    Field(connectionString, "ClientId"),
                    Field(connectionString, "AppId"),
                    environment(ClientIdVariable),
                    environment(AzureCliClientIdVariable)),

                ServiceConnectionId = First(
                    options.ServiceConnectionId,
                    Field(connectionString, "ServiceConnectionId"),
                    environment(ServiceConnectionIdVariable))
            };
        }

        /// <summary>
        ///     Checks that the three values a sign-in cannot be attempted without are present.
        /// </summary>
        /// <remarks>
        ///     Validated up front, and all at once, so a misconfigured pipeline gets the whole list
        ///     in one run instead of one missing value per attempt.
        /// </remarks>
        /// <exception cref="FederatedAuthenticationException">One of them is missing.</exception>
        public void Validate()
        {
            var missing = new List<string>();

            if (string.IsNullOrWhiteSpace(Url))
                missing.Add("the Dataverse URL (--environment, or Url= in the connection string)");

            if (string.IsNullOrWhiteSpace(TenantId))
                missing.Add(
                    $"the Entra tenant (--tenant, TenantId= in the connection string, ${TenantIdVariable}, " +
                    $"or ${AzureCliTenantIdVariable} from AzureCLI@2 with addSpnToEnvironment)");

            if (string.IsNullOrWhiteSpace(ClientId))
                missing.Add(
                    $"the application id (--application-id, ClientId= in the connection string, ${ClientIdVariable}, " +
                    $"or ${AzureCliClientIdVariable} from AzureCLI@2 with addSpnToEnvironment)");

            if (missing.Count == 0) return;

            throw new FederatedAuthenticationException(
                "Workload identity federation is missing " + string.Join(", ", missing) + ".");
        }

        /// <summary>
        ///     The scope a Dataverse token is requested for: the organization's own
        ///     <c>/.default</c>.
        /// </summary>
        /// <param name="instanceUrl">
        ///     Instance the client is about to call, or <see langword="null" /> to use
        ///     <see cref="Url" />. <c>ServiceClient</c> passes the one it is connecting to, which
        ///     need not be the one configured here.
        /// </param>
        /// <remarks>
        ///     Only the authority of that URL is kept. <c>ServiceClient</c> hands its token
        ///     provider the endpoint it is about to call - something like
        ///     <c>.../XRMServices/2011/Organization.svc/web?SDKClientVersion=9.2.49</c> - while the
        ///     audience Dataverse expects is the organization itself. Appending
        ///     <c>/.default</c> to what it passes would ask Entra for a scope that does not exist.
        /// </remarks>
        /// <exception cref="FederatedAuthenticationException">Neither is known.</exception>
        public string ScopeFor(string instanceUrl)
        {
            var url = string.IsNullOrWhiteSpace(instanceUrl) ? Url : instanceUrl.Trim();

            if (string.IsNullOrWhiteSpace(url))
                throw new FederatedAuthenticationException(
                    "No Dataverse URL to request a token for. Pass --environment, or declare Url= " +
                    "in the connection string.");

            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                ? uri.GetLeftPart(UriPartial.Authority) + "/.default"
                : url.TrimEnd('/') + "/.default";
        }

        private static string First(params string[] candidates)
            => candidates.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c))?.Trim();

        private static string Field(string connectionString, string name)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            // Read by hand rather than through ConnectionStringParser.GetConnectionStringField:
            // that one is case-sensitive and splits the whole segment on '=', which mangles any
            // value containing one - a URL with a query string, for instance.
            foreach (var segment in connectionString.Split(';'))
            {
                var separator = segment.IndexOf('=');
                if (separator <= 0) continue;

                if (!segment.Substring(0, separator).Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
                    continue;

                var value = segment.Substring(separator + 1).Trim();

                return string.IsNullOrEmpty(value) ? null : value;
            }

            return null;
        }

        private static string TrimUrl(string url) => url?.TrimEnd('/');
    }
}

#endif
