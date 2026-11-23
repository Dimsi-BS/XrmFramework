// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER

using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace XrmFramework.DeployUtils.Configuration.Federated
{
    /// <summary>
    ///     Asks Azure DevOps for the OIDC token that proves the identity of the running job, so it
    ///     can be exchanged for a Dataverse token without any secret ever reaching the pipeline.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The exchange is the one <c>pac auth create --azureDevOpsFederated</c> performs, down
    ///         to the headers: a POST, a lower-case <c>bearer</c> scheme, and
    ///         <c>X-TFS-FedAuthRedirect: Suppress</c> - without which Azure DevOps may answer an
    ///         HTML sign-in page instead of a 401. Keeping the two identical is deliberate: a job
    ///         where <c>pac</c> authenticates and this does not is a bug here, not a
    ///         misconfigured pipeline.
    ///     </para>
    ///     <para>
    ///         The endpoint is read from several variable names because the ecosystem has several.
    ///         The Power Platform Build Tools task sets <c>PAC_ADO_ID_TOKEN_REQUEST_*</c>, but with
    ///         <c>process.env</c> inside its own Node process - so they reach the <c>pac</c> that
    ///         task launches and not a later script step. In practice a pipeline sets them itself
    ///         from <c>System.OidcRequestUri</c> and <c>System.AccessToken</c>; they are read here
    ///         anyway, for the contexts that do export them. They are also undocumented, hence our
    ///         own names taking precedence.
    ///     </para>
    /// </remarks>
    public sealed class AzureDevOpsOidcProvider : IOidcTokenProvider
    {
        /// <summary>Our own endpoint variable, the one that cannot be renamed under us.</summary>
        public const string RequestUrlVariable = "XRMFRAMEWORK_ADO_ID_TOKEN_REQUEST_URL";

        /// <summary>Our own request-token variable.</summary>
        public const string RequestTokenVariable = "XRMFRAMEWORK_ADO_ID_TOKEN_REQUEST_TOKEN";

        /// <summary>Endpoint variable the Power Platform Build Tools and pac agree on.</summary>
        public const string PacRequestUrlVariable = "PAC_ADO_ID_TOKEN_REQUEST_URL";

        /// <summary>Request-token variable the Power Platform Build Tools and pac agree on.</summary>
        public const string PacRequestTokenVariable = "PAC_ADO_ID_TOKEN_REQUEST_TOKEN";

        /// <summary>Azure DevOps predefined variable holding the OIDC endpoint, without its query.</summary>
        public const string SystemRequestUrlVariable = "SYSTEM_OIDCREQUESTURI";

        /// <summary>
        ///     Azure DevOps predefined variable holding the job's token. In YAML it has to be
        ///     mapped explicitly - <c>env: SYSTEM_ACCESSTOKEN: $(System.AccessToken)</c>.
        /// </summary>
        public const string SystemRequestTokenVariable = "SYSTEM_ACCESSTOKEN";

        /// <summary>Overrides <see cref="DefaultApiVersion" /> without republishing the CLI.</summary>
        public const string ApiVersionVariable = "XRMFRAMEWORK_ADO_OIDC_API_VERSION";

        /// <summary>
        ///     API version of the Azure DevOps <c>oidctoken</c> endpoint, the one the Power Platform
        ///     Build Tools request. A preview version, which is why
        ///     <see cref="ApiVersionVariable" /> exists.
        /// </summary>
        public const string DefaultApiVersion = "7.2-preview.1";

        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(100);

        private readonly string _requestUrl;
        private readonly string _requestToken;
        private readonly HttpClient _client;
        private readonly Action<string> _log;

        public AzureDevOpsOidcProvider(
            string requestUrl,
            string requestToken,
            HttpClient client = null,
            Action<string> log = null)
        {
            if (string.IsNullOrWhiteSpace(requestUrl)) throw new ArgumentNullException(nameof(requestUrl));
            if (string.IsNullOrWhiteSpace(requestToken)) throw new ArgumentNullException(nameof(requestToken));

            _requestUrl = requestUrl.Trim();
            _requestToken = requestToken.Trim();
            _client = client ?? SharedClient.Value;
            _log = log;
        }

        /// <inheritdoc />
        public string PlatformName => "Azure DevOps";

        /// <inheritdoc />
        public string Issuer { get; private set; }

        /// <inheritdoc />
        public string Subject { get; private set; }

        /// <summary>
        ///     <see langword="true" /> when the environment looks like an Azure DevOps job offering
        ///     federation, so <c>--auth auto</c> can pick it without being told.
        /// </summary>
        public static bool IsAvailable(Func<string, string> environment)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            return ResolveUrlSource(environment) != null && ResolveToken(null, environment) != null;
        }

        /// <summary>
        ///     Builds the provider for the running job, or throws naming what the job is missing.
        /// </summary>
        /// <param name="options">Deployment options carrying the command-line overrides.</param>
        /// <param name="serviceConnectionId">
        ///     Service connection to request the token for, needed only when the endpoint comes
        ///     from <see cref="SystemRequestUrlVariable" /> and therefore has no query yet.
        /// </param>
        /// <param name="environment">Environment variable reader.</param>
        /// <param name="client">Transport, injected by the tests.</param>
        /// <param name="log">Diagnostic sink, or <see langword="null" /> to stay quiet.</param>
        /// <exception cref="FederatedAuthenticationException">
        ///     The job exposes no OIDC endpoint, no token, or not enough to build the endpoint.
        /// </exception>
        public static AzureDevOpsOidcProvider Create(
            DeployOptions options,
            string serviceConnectionId,
            Func<string, string> environment,
            HttpClient client = null,
            Action<string> log = null)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            var url = ResolveUrl(options, serviceConnectionId, environment);
            var token = ResolveToken(options, environment);

            if (token == null)
                throw new FederatedAuthenticationException(
                    "No Azure DevOps token to request an OIDC token with. In a YAML pipeline " +
                    $"System.AccessToken has to be mapped explicitly - add \"env: {SystemRequestTokenVariable}: " +
                    "$(System.AccessToken)\" to the step.");

            return new AzureDevOpsOidcProvider(url, token, client, log);
        }

        /// <inheritdoc />
        public async Task<string> GetAssertionAsync(CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, _requestUrl))
            {
                // Shaped exactly like pac's own request. "bearer" is lower-case there, and
                // X-TFS-FedAuthRedirect keeps Azure DevOps from answering a sign-in page.
                request.Headers.Add("Authorization", "bearer " + _requestToken);
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("X-TFS-FedAuthRedirect", "Suppress");

                HttpResponseMessage response;

                try
                {
                    response = await _client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    throw new FederatedAuthenticationException(
                        $"The Azure DevOps OIDC endpoint could not be reached ({Redact(_requestUrl)}): {ex.Message}",
                        ex);
                }

                using (response)
                {
                    var body = await ReadBodyAsync(response).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                        throw new FederatedAuthenticationException(
                            $"Azure DevOps refused to mint an OIDC token ({(int)response.StatusCode} " +
                            $"{response.ReasonPhrase}) for {Redact(_requestUrl)}. " +
                            DescribeFailure(response, body));

                    var token = ReadOidcToken(body);

                    if (string.IsNullOrWhiteSpace(token))
                        throw new FederatedAuthenticationException(
                            "Azure DevOps answered the OIDC request without an \"oidcToken\" field. " +
                            DescribeFailure(response, body));

                    DescribeToken(token);

                    return token;
                }
            }
        }

        private static string ResolveUrl(
            DeployOptions options,
            string serviceConnectionId,
            Func<string, string> environment)
        {
            var explicitUrl = First(
                options.OidcRequestUrl,
                environment(RequestUrlVariable),
                environment(PacRequestUrlVariable));

            if (explicitUrl != null)
                return WithQuery(explicitUrl, serviceConnectionId, environment);

            var systemUrl = First(environment(SystemRequestUrlVariable));

            if (systemUrl == null)
                throw new FederatedAuthenticationException(
                    "No Azure DevOps OIDC endpoint found. Set " +
                    $"{SystemRequestUrlVariable} (from $(System.OidcRequestUri)) or " +
                    $"{RequestUrlVariable}, or pass --oidc-request-url. Outside a pipeline, use a " +
                    "connection string instead of workload identity federation.");

            return WithQuery(systemUrl, serviceConnectionId, environment);
        }

        /// <summary>
        ///     Completes an endpoint that was given without its query.
        /// </summary>
        /// <remarks>
        ///     Driven by what the URL already carries rather than by where it came from, so a
        ///     complete endpoint is left alone whichever variable held it, and a bare one is
        ///     completed just the same.
        /// </remarks>
        private static string WithQuery(string url, string serviceConnectionId, Func<string, string> environment)
        {
            var result = url;

            if (!HasQueryParameter(result, "serviceConnectionId"))
            {
                if (string.IsNullOrWhiteSpace(serviceConnectionId))
                    throw new FederatedAuthenticationException(
                        $"The Azure DevOps OIDC endpoint ({Redact(url)}) names no service connection. " +
                        "Pass --service-connection-id, set " +
                        $"{FederatedConnectionSettings.ServiceConnectionIdVariable}, or give the whole " +
                        $"endpoint - query included - in {RequestUrlVariable}.");

                result = Append(result, "serviceConnectionId=" + Uri.EscapeDataString(serviceConnectionId.Trim()));
            }

            if (!HasQueryParameter(result, "api-version"))
                result = Append(result, "api-version=" + ApiVersion(environment));

            return result;
        }

        private static string ApiVersion(Func<string, string> environment)
            => First(environment(ApiVersionVariable)) ?? DefaultApiVersion;

        private static string ResolveToken(DeployOptions options, Func<string, string> environment)
            => First(
                options?.OidcRequestToken,
                environment(RequestTokenVariable),
                environment(PacRequestTokenVariable),
                environment(SystemRequestTokenVariable));

        /// <summary>
        ///     Which variable, if any, holds an endpoint - used by the availability probe so that
        ///     <c>--auth auto</c> never has to build a URL just to find out.
        /// </summary>
        private static string ResolveUrlSource(Func<string, string> environment)
            => First(
                environment(RequestUrlVariable),
                environment(PacRequestUrlVariable),
                environment(SystemRequestUrlVariable));

        private static bool HasQueryParameter(string url, string name)
        {
            var query = url.IndexOf('?');

            if (query < 0) return false;

            return url.Substring(query + 1)
                .Split('&')
                .Any(p => p.StartsWith(name + "=", StringComparison.OrdinalIgnoreCase));
        }

        private static string Append(string url, string parameter)
            => url + (url.IndexOf('?') < 0 ? '?' : '&') + parameter;

        private static async Task<string> ReadBodyAsync(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch
            {
                // A body we cannot read must not replace the status code in the message below.
                return null;
            }
        }

        private static string ReadOidcToken(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return null;

            try
            {
                using (var document = JsonDocument.Parse(body))
                {
                    return document.RootElement.ValueKind == JsonValueKind.Object
                           && document.RootElement.TryGetProperty("oidcToken", out var token)
                           && token.ValueKind == JsonValueKind.String
                        ? token.GetString()
                        : null;
                }
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        ///     Says what came back, without echoing it: the body of a failed OIDC request is
        ///     sometimes a sign-in page, and always something we would rather not paste into a
        ///     build log wholesale.
        /// </summary>
        private static string DescribeFailure(HttpResponseMessage response, string body)
        {
            var contentType = response.Content?.Headers?.ContentType?.MediaType;

            if (contentType != null && contentType.IndexOf("html", StringComparison.OrdinalIgnoreCase) >= 0)
                return "The answer was an HTML page, which means the request was not authenticated: " +
                       "check that the token is the job's System.AccessToken and that the pipeline is " +
                       "authorized for this service connection.";

            if (string.IsNullOrWhiteSpace(body))
                return "The answer had no body.";

            return "The answer was " + body.Length + " characters of " + (contentType ?? "an unnamed type") + ".";
        }

        /// <summary>
        ///     Records and logs the issuer and subject of the token.
        /// </summary>
        private void DescribeToken(string jwt)
        {
            var claims = OidcTokenClaims.Read(jwt);

            if (claims == null) return;

            Issuer = claims.Value.Issuer;
            Subject = claims.Value.Subject;

            _log?.Invoke($"OIDC token issuer: {Issuer}");
            _log?.Invoke($"OIDC token subject: {Subject}");
        }

        /// <summary>
        ///     Keeps the endpoint readable in a message without its query, which names the service
        ///     connection and carries nothing a reader needs.
        /// </summary>
        private static string Redact(string url)
        {
            var query = url.IndexOf('?');

            return query < 0 ? url : url.Substring(0, query);
        }

        private static string First(params string[] candidates)
            => candidates.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c))?.Trim();

        private static readonly Lazy<HttpClient> SharedClient = new Lazy<HttpClient>(() =>
            new HttpClient { Timeout = Timeout });
    }
}

#endif
