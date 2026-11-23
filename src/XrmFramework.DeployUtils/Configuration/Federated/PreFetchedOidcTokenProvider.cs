// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XrmFramework.DeployUtils.Configuration.Federated
{
    /// <summary>
    ///     Uses an OIDC token the job has already been handed, instead of requesting one.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is the shortest path, and the one to prefer. An <c>AzureCLI@2</c> step with
    ///         <c>addSpnToEnvironment: true</c> and a workload identity federation service
    ///         connection runs the script with <c>idToken</c>, <c>servicePrincipalId</c> and
    ///         <c>tenantId</c> in its environment - the task has already read the service
    ///         connection through <c>azure-pipelines-task-lib</c>'s
    ///         <c>getEndpointAuthorizationParameter</c> and fetched the federated token. Nothing is
    ///         left for the deployment to request, and nothing to restate in YAML: no tenant, no
    ///         application id, no OIDC endpoint, no <c>SYSTEM_ACCESSTOKEN</c> mapping.
    ///     </para>
    ///     <para>
    ///         We cannot call that library ourselves: it is Node, and it reads
    ///         <c>ENDPOINT_AUTH_*</c> environment variables the agent injects only into a task
    ///         whose inputs declare the service connection. A plain <c>script:</c> step gets none.
    ///         Letting a task that already does it hand us the result is the way to benefit from it
    ///         without shipping an Azure DevOps task of our own.
    ///     </para>
    ///     <para>
    ///         <c>AZURE_FEDERATED_TOKEN_FILE</c> is read for the same reason, and makes the Azure
    ///         Kubernetes and <c>azure/login</c> conventions work at no extra cost.
    ///     </para>
    /// </remarks>
    public sealed class PreFetchedOidcTokenProvider : IOidcTokenProvider
    {
        /// <summary>Our own variable, for a job whose assertion comes from somewhere else entirely.</summary>
        public const string TokenVariable = "XRMFRAMEWORK_OIDC_TOKEN";

        /// <summary>
        ///     Exported by <c>AzureCLI@2</c> with <c>addSpnToEnvironment: true</c> on a workload
        ///     identity federation service connection. Spelled exactly as the task spells it:
        ///     environment variables are case-sensitive on the Linux agents.
        /// </summary>
        public const string AzureCliTokenVariable = "idToken";

        /// <summary>Path to a file holding the assertion - the Azure Kubernetes convention.</summary>
        public const string TokenFileVariable = "AZURE_FEDERATED_TOKEN_FILE";

        private readonly string _token;
        private Action<string> _log;

        private PreFetchedOidcTokenProvider(string token, string platformName)
        {
            _token = token;
            PlatformName = platformName;

            var claims = OidcTokenClaims.Read(token);

            if (claims == null) return;

            Issuer = claims.Value.Issuer;
            Subject = claims.Value.Subject;
        }

        /// <inheritdoc />
        public string PlatformName { get; }

        /// <inheritdoc />
        public string Issuer { get; }

        /// <inheritdoc />
        public string Subject { get; }

        /// <summary>
        ///     <see langword="true" /> when the environment already carries an assertion.
        /// </summary>
        public static bool IsAvailable(Func<string, string> environment, Func<string, string> readFile = null)
            => TryCreate(environment, readFile) != null;

        /// <summary>
        ///     Builds the provider from whichever variable carries the assertion, or answers
        ///     <see langword="null" /> when none does.
        /// </summary>
        /// <param name="environment">Environment variable reader.</param>
        /// <param name="readFile">
        ///     File reader, injected by the tests. Answers <see langword="null" /> for a path it
        ///     cannot read, so an unreadable token file falls through to the next source rather
        ///     than failing the deployment.
        /// </param>
        /// <param name="log">Diagnostic sink, or <see langword="null" /> to stay quiet.</param>
        public static PreFetchedOidcTokenProvider TryCreate(
            Func<string, string> environment,
            Func<string, string> readFile = null,
            Action<string> log = null)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            var provider =
                FromVariable(environment, TokenVariable, $"${TokenVariable}")
                ?? FromVariable(environment, AzureCliTokenVariable, "AzureCLI@2 (addSpnToEnvironment)")
                ?? FromFile(environment, readFile ?? ReadFile);

            if (provider == null) return null;

            provider._log = log;

            return provider;
        }

        /// <inheritdoc />
        /// <remarks>
        ///     Logged on first use rather than when the provider is built, so the diagnostics land
        ///     where the request-based provider's do - after the connection summary, at the point
        ///     the deployment actually signs in - instead of ahead of it.
        /// </remarks>
        public Task<string> GetAssertionAsync(CancellationToken cancellationToken)
        {
            var log = _log;
            _log = null;

            if (log != null)
            {
                log($"OIDC token source: {PlatformName}");

                if (Issuer != null) log($"OIDC token issuer: {Issuer}");
                if (Subject != null) log($"OIDC token subject: {Subject}");
            }

            return Task.FromResult(_token);
        }

        private static PreFetchedOidcTokenProvider FromVariable(
            Func<string, string> environment, string name, string platformName)
        {
            var token = Trimmed(environment(name));

            return token == null ? null : new PreFetchedOidcTokenProvider(token, platformName);
        }

        private static PreFetchedOidcTokenProvider FromFile(
            Func<string, string> environment, Func<string, string> readFile)
        {
            var path = Trimmed(environment(TokenFileVariable));

            if (path == null) return null;

            var token = Trimmed(readFile(path));

            return token == null
                ? null
                : new PreFetchedOidcTokenProvider(token, $"${TokenFileVariable} ({path})");
        }

        private static string ReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return null;
            }
        }

        private static string Trimmed(string value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

#endif
