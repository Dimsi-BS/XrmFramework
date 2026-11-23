// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration.Federated;

namespace XrmFramework.DeployUtils.Tests.Configuration.Federated;

[TestFixture]
public class AzureDevOpsOidcProviderTests
{
    private const string Endpoint =
        "https://dev.azure.com/contoso/11111111-1111-1111-1111-111111111111/_apis/distributedtask" +
        "/hubs/build/plans/22222222-2222-2222-2222-222222222222/jobs/33333333-3333-3333-3333-333333333333/oidctoken";

    private const string Issuer = "https://vstoken.dev.azure.com/contoso";
    private const string Subject = "sc://contoso/Dataverse/DimsiDev";

    /// <summary>
    ///     Azure DevOps types <c>serviceConnectionId</c> as a uuid, so the id a test hands over to
    ///     be injected has to be one.
    /// </summary>
    private const string Connection = "8d3f6a21-5c44-4f0e-9a1b-2e7d9c0f4b55";

    private const string OtherConnection = "1b2c3d4e-5f60-4712-8345-9a0bcdef1234";

    // ──────────────────────────────────────────────
    //  The request pac makes, header for header
    // ──────────────────────────────────────────────

    [Test]
    public async Task GetAssertionAsync_PostsTheRequestPacWouldHaveMade()
    {
        var handler = Answering(Ok(Jwt()));
        var provider = new AzureDevOpsOidcProvider(Endpoint + "?serviceConnectionId=sc", "job-token", handler.Client);

        await provider.GetAssertionAsync(CancellationToken.None);

        var request = handler.LastRequest;

        Assert.AreEqual(HttpMethod.Post, request.Method);
        Assert.AreEqual(Endpoint + "?serviceConnectionId=sc", request.RequestUri!.ToString());
        Assert.AreEqual("bearer job-token", Header(request, "Authorization"));
        Assert.AreEqual("application/json", Header(request, "Accept"));
        Assert.AreEqual("Suppress", Header(request, "X-TFS-FedAuthRedirect"));
    }

    [Test]
    public async Task GetAssertionAsync_ReturnsTheOidcToken()
    {
        var handler = Answering(Ok(Jwt()));
        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        Assert.AreEqual(Jwt(), await provider.GetAssertionAsync(CancellationToken.None));
    }

    [Test]
    public async Task GetAssertionAsync_ReportsIssuerAndSubject_SoTheFederatedCredentialCanBeConfigured()
    {
        var logged = new List<string>();
        var handler = Answering(Ok(Jwt()));
        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client, logged.Add);

        await provider.GetAssertionAsync(CancellationToken.None);

        Assert.AreEqual(Issuer, provider.Issuer);
        Assert.AreEqual(Subject, provider.Subject);
        Assert.IsTrue(logged.Any(l => l.Contains(Issuer)), "the issuer is not logged");
        Assert.IsTrue(logged.Any(l => l.Contains(Subject)), "the subject is not logged");
    }

    [Test]
    public async Task GetAssertionAsync_UnreadableToken_StillSucceeds()
    {
        // Reading the claims is diagnostics: a token we cannot decode must still be usable.
        var handler = Answering(Ok("not-a-jwt"));
        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        Assert.AreEqual("not-a-jwt", await provider.GetAssertionAsync(CancellationToken.None));
        Assert.IsNull(provider.Issuer);
    }

    // ──────────────────────────────────────────────
    //  Refusals
    // ──────────────────────────────────────────────

    [Test]
    public void GetAssertionAsync_Unauthorized_SaysSo()
    {
        var handler = Answering(new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        });

        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        Assert.IsTrue(error!.Message.Contains("401"), error.Message);
    }

    [Test]
    public void GetAssertionAsync_SignInPage_ExplainsItIsAnAuthenticationProblem()
    {
        // Azure DevOps answers HTML when the request is not authenticated. Without
        // X-TFS-FedAuthRedirect it does so instead of a 401, which is exactly the confusing case.
        var handler = Answering(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<html>Sign in</html>", Encoding.UTF8, "text/html")
        });

        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        Assert.IsTrue(error!.Message.Contains("HTML"), error.Message);
        Assert.IsTrue(error.Message.Contains("System.AccessToken"), error.Message);
    }

    [Test]
    public void GetAssertionAsync_NotFound_SaysSo()
    {
        var handler = Answering(new HttpResponseMessage(HttpStatusCode.NotFound));
        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        Assert.IsTrue(error!.Message.Contains("404"), error.Message);
    }

    [Test]
    public void GetAssertionAsync_EmptyBody_SaysSo()
    {
        var handler = Answering(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
        });

        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        Assert.IsTrue(error!.Message.Contains("oidcToken"), error.Message);
    }

    [Test]
    public void GetAssertionAsync_AnswerWithoutTheField_SaysSo()
    {
        var handler = Answering(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"count\":0}", Encoding.UTF8, "application/json")
        });

        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        Assert.IsTrue(error!.Message.Contains("oidcToken"), error.Message);
    }

    [Test]
    public void GetAssertionAsync_UnreachableEndpoint_NamesIt()
    {
        var handler = Throwing(new HttpRequestException("no route to host"));
        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        // The message, not the inner exception's wording: HttpClient is free to wrap what the
        // handler threw, and the point is that the failure is reported as a federation problem
        // naming the endpoint.
        Assert.IsTrue(error!.Message.Contains("could not be reached"), error.Message);
        Assert.IsTrue(error.Message.Contains("oidctoken"), error.Message);
    }

    // ──────────────────────────────────────────────
    //  Where the endpoint and the token come from
    // ──────────────────────────────────────────────

    [Test]
    public async Task Create_PrefersOurOwnVariableOverPacs()
    {
        var url = await UrlRequestedBy(new DeployOptions(), null, new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.RequestUrlVariable] = Endpoint + "?serviceConnectionId=ours",
            [AzureDevOpsOidcProvider.PacRequestUrlVariable] = Endpoint + "?serviceConnectionId=pacs",
            [AzureDevOpsOidcProvider.RequestTokenVariable] = "job-token"
        });

        Assert.IsTrue(url.Contains("serviceConnectionId=ours"), url);
    }

    [Test]
    public async Task Create_PrefersPacsVariableOverTheSystemOne()
    {
        var url = await UrlRequestedBy(new DeployOptions(), Connection, new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.PacRequestUrlVariable] = Endpoint + "?serviceConnectionId=pacs",
            [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = "https://elsewhere.invalid/oidctoken",
            [AzureDevOpsOidcProvider.SystemRequestTokenVariable] = "job-token"
        });

        Assert.IsTrue(url.Contains("serviceConnectionId=pacs"), url);
    }

    [Test]
    public async Task Create_CommandLineWinsOverEveryVariable()
    {
        var options = new DeployOptions
        {
            OidcRequestUrl = Endpoint + "?serviceConnectionId=cli",
            OidcRequestToken = "cli-token"
        };

        var handler = Answering(Ok(Jwt()));
        var provider = AzureDevOpsOidcProvider.Create(
            options, Connection, Reader(new Dictionary<string, string>
            {
                [AzureDevOpsOidcProvider.RequestUrlVariable] = Endpoint + "?serviceConnectionId=ours"
            }), handler.Client);

        await provider.GetAssertionAsync(CancellationToken.None);

        Assert.IsTrue(handler.LastRequest.RequestUri!.ToString().Contains("serviceConnectionId=cli"));
        Assert.AreEqual("bearer cli-token", Header(handler.LastRequest, "Authorization"));
    }

    [Test]
    public async Task Create_CompletesTheSystemEndpointWithItsQuery()
    {
        var url = await UrlRequestedBy(new DeployOptions(), Connection, new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = Endpoint,
            [AzureDevOpsOidcProvider.SystemRequestTokenVariable] = "job-token"
        });

        Assert.AreEqual(
            Endpoint + "?serviceConnectionId=" + Connection + "&api-version=" + AzureDevOpsOidcProvider.DefaultApiVersion,
            url);
    }

    [Test]
    public async Task Create_LeavesACompleteEndpointAlone()
    {
        var complete = Endpoint + "?serviceConnectionId=already&api-version=7.1";

        var url = await UrlRequestedBy(new DeployOptions(), Connection, new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.PacRequestUrlVariable] = complete,
            [AzureDevOpsOidcProvider.PacRequestTokenVariable] = "job-token"
        });

        Assert.AreEqual(complete, url);
    }

    [Test]
    public async Task Create_AddsTheApiVersionToAnEndpointThatOnlyNamesTheConnection()
    {
        var url = await UrlRequestedBy(new DeployOptions(), null, new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.PacRequestUrlVariable] = Endpoint + "?serviceConnectionId=already",
            [AzureDevOpsOidcProvider.PacRequestTokenVariable] = "job-token"
        });

        Assert.AreEqual(
            Endpoint + "?serviceConnectionId=already&api-version=" + AzureDevOpsOidcProvider.DefaultApiVersion,
            url);
    }

    [Test]
    public async Task Create_HonoursTheApiVersionOverride()
    {
        var url = await UrlRequestedBy(new DeployOptions(), Connection, new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = Endpoint,
            [AzureDevOpsOidcProvider.SystemRequestTokenVariable] = "job-token",
            [AzureDevOpsOidcProvider.ApiVersionVariable] = "7.3-preview.2"
        });

        Assert.IsTrue(url.EndsWith("api-version=7.3-preview.2"), url);
    }

    [Test]
    public void Create_SystemEndpointWithoutAServiceConnection_SaysWhatToPass()
    {
        var error = Assert.Throws<FederatedAuthenticationException>(() => AzureDevOpsOidcProvider.Create(
            new DeployOptions(),
            null,
            Reader(new Dictionary<string, string>
            {
                [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = Endpoint,
                [AzureDevOpsOidcProvider.SystemRequestTokenVariable] = "job-token"
            })));

        Assert.IsTrue(error!.Message.Contains("--service-connection-id"), error.Message);
        Assert.IsTrue(error.Message.Contains(FederatedConnectionSettings.ServiceConnectionIdVariable), error.Message);
    }

    [Test]
    public void Create_NoEndpointAtAll_NamesTheVariables()
    {
        var error = Assert.Throws<FederatedAuthenticationException>(() => AzureDevOpsOidcProvider.Create(
            new DeployOptions(), Connection, Reader(new Dictionary<string, string>())));

        Assert.IsTrue(error!.Message.Contains(AzureDevOpsOidcProvider.SystemRequestUrlVariable), error.Message);
        Assert.IsTrue(error.Message.Contains("System.OidcRequestUri"), error.Message);
    }

    [Test]
    public void Create_NoToken_PointsAtTheYamlMappingThatIsAlwaysForgotten()
    {
        var error = Assert.Throws<FederatedAuthenticationException>(() => AzureDevOpsOidcProvider.Create(
            new DeployOptions(),
            Connection,
            Reader(new Dictionary<string, string>
            {
                [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = Endpoint
            })));

        Assert.IsTrue(error!.Message.Contains("$(System.AccessToken)"), error.Message);
    }

    // ──────────────────────────────────────────────
    //  The service connection id has to be real
    // ──────────────────────────────────────────────

    [Test]
    public async Task Create_UnexpandedVariableInTheEndpoint_IsReplaced()
    {
        // Observed on a real pipeline: PAC_ADO_ID_TOKEN_REQUEST_URL was set to
        // "$(System.OidcRequestUri)?serviceConnectionId=$(ServiceConnectionId)..." with that
        // variable undefined. Azure DevOps left the macro verbatim, ignored it, and answered a
        // pipeline-scoped token - which Entra then rejected for a subject nobody had configured.
        var url = await UrlRequestedBy(
            new DeployOptions(),
            Connection,
            new Dictionary<string, string>
            {
                [AzureDevOpsOidcProvider.PacRequestUrlVariable] =
                    Endpoint + "?serviceConnectionId=$(ServiceConnectionId)&api-version=7.1",
                [AzureDevOpsOidcProvider.PacRequestTokenVariable] = "job-token"
            });

        Assert.IsTrue(url.Contains("serviceConnectionId=" + Connection), url);
        Assert.IsFalse(url.Contains("$("), url);
        Assert.AreEqual(1, Occurrences(url, "serviceConnectionId="), "the parameter is duplicated");
        Assert.IsTrue(url.Contains("api-version=7.1"), "an api-version already there is kept");
    }

    [Test]
    public async Task Create_EmptyServiceConnectionIdInTheEndpoint_IsReplaced()
    {
        var url = await UrlRequestedBy(
            new DeployOptions(),
            Connection,
            new Dictionary<string, string>
            {
                [AzureDevOpsOidcProvider.PacRequestUrlVariable] = Endpoint + "?serviceConnectionId=",
                [AzureDevOpsOidcProvider.PacRequestTokenVariable] = "job-token"
            });

        Assert.IsTrue(url.Contains("serviceConnectionId=" + Connection), url);
        Assert.AreEqual(1, Occurrences(url, "serviceConnectionId="), "the parameter is duplicated");
    }

    [Test]
    public void Create_UnusableServiceConnectionIdAndNoReplacement_SaysWhatToPass()
    {
        var error = Assert.Throws<FederatedAuthenticationException>(() => AzureDevOpsOidcProvider.Create(
            new DeployOptions(),
            null,
            Reader(new Dictionary<string, string>
            {
                [AzureDevOpsOidcProvider.PacRequestUrlVariable] =
                    Endpoint + "?serviceConnectionId=$(ServiceConnectionId)",
                [AzureDevOpsOidcProvider.PacRequestTokenVariable] = "job-token"
            })));

        Assert.IsTrue(error!.Message.Contains("--service-connection-id"), error.Message);
    }

    [Test]
    public async Task GetAssertionAsync_PipelineScopedAnswer_WarnsThatTheConnectionWasIgnored()
    {
        var logged = new List<string>();
        var handler = Answering(Ok(Jwt("p://contoso/Dataverse/MyPipeline")));

        var provider = new AzureDevOpsOidcProvider(
            Endpoint + "?serviceConnectionId=" + OtherConnection, "job-token", handler.Client, logged.Add);

        await provider.GetAssertionAsync(CancellationToken.None);

        var warning = logged.FirstOrDefault(l => l.StartsWith("Warning:"));

        Assert.IsNotNull(warning, "a pipeline-scoped token went unreported");
        Assert.IsTrue(warning!.Contains(OtherConnection), warning);
        Assert.IsTrue(warning.Contains("sc://"), warning);
    }

    [Test]
    public async Task GetAssertionAsync_ServiceConnectionScopedAnswer_DoesNotWarn()
    {
        var logged = new List<string>();
        var handler = Answering(Ok(Jwt()));

        var provider = new AzureDevOpsOidcProvider(
            Endpoint + "?serviceConnectionId=" + OtherConnection, "job-token", handler.Client, logged.Add);

        await provider.GetAssertionAsync(CancellationToken.None);

        Assert.IsFalse(logged.Any(l => l.StartsWith("Warning:")), string.Join(" | ", logged));
    }

    [Test]
    public void Create_ServiceConnectionNameInsteadOfItsId_SaysSo()
    {
        // Azure DevOps types the parameter as a uuid and answers 400 Bad Request for anything else,
        // with nothing in the body naming the mistake. Observed on a real pipeline.
        var error = Assert.Throws<FederatedAuthenticationException>(() => AzureDevOpsOidcProvider.Create(
            new DeployOptions { ServiceConnectionId = "DIMSI-ERP-Dataverse" },
            "DIMSI-ERP-Dataverse",
            Reader(new Dictionary<string, string>
            {
                [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = Endpoint,
                [AzureDevOpsOidcProvider.SystemRequestTokenVariable] = "job-token"
            })));

        Assert.IsTrue(error!.Message.Contains("not a service connection id"), error.Message);
        Assert.IsTrue(error.Message.Contains("resourceId"), error.Message);
    }

    [Test]
    public async Task Create_ServiceConnectionIdKeepingItsQuotes_IsAccepted()
    {
        var url = await UrlRequestedBy(
            new DeployOptions(),
            "\"" + Connection + "\"",
            new Dictionary<string, string>
            {
                [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = Endpoint,
                [AzureDevOpsOidcProvider.SystemRequestTokenVariable] = "job-token"
            });

        Assert.IsTrue(url.Contains("serviceConnectionId=" + Connection), url);
    }

    // ──────────────────────────────────────────────
    //  What a refusal tells the reader
    // ──────────────────────────────────────────────

    [Test]
    public void GetAssertionAsync_BadRequest_QuotesWhatAzureDevOpsSaid()
    {
        // The whole point: the diagnostic is in the envelope's "message", and summarising the
        // body's length instead of reading it threw the answer away.
        var handler = Answering(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                "{\"$id\":\"1\",\"innerException\":null,\"message\":\"The service connection could not be found.\"," +
                "\"typeName\":\"ServiceEndpointNotFoundException\"}",
                Encoding.UTF8,
                "application/json")
        });

        var provider = new AzureDevOpsOidcProvider(
            Endpoint + "?serviceConnectionId=" + Connection, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        Assert.IsTrue(error!.Message.Contains("The service connection could not be found."), error.Message);
    }

    [Test]
    public void GetAssertionAsync_Failure_ShowsTheWholeEndpointQueryIncluded()
    {
        // Nothing secret goes in the URL - the token travels in a header - and the query is what a
        // 400 on this API is about.
        var handler = Answering(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var provider = new AzureDevOpsOidcProvider(
            Endpoint + "?serviceConnectionId=" + Connection, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        Assert.IsTrue(error!.Message.Contains("serviceConnectionId=" + Connection), error.Message);
    }

    [Test]
    public void GetAssertionAsync_NonJsonFailureBody_IsQuotedAsIs()
    {
        var handler = Answering(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("plain words about what went wrong", Encoding.UTF8, "text/plain")
        });

        var provider = new AzureDevOpsOidcProvider(Endpoint, "job-token", handler.Client);

        var error = Assert.ThrowsAsync<FederatedAuthenticationException>(
            () => provider.GetAssertionAsync(CancellationToken.None));

        Assert.IsTrue(error!.Message.Contains("plain words about what went wrong"), error.Message);
    }

    // ──────────────────────────────────────────────
    //  Auto-detection
    // ──────────────────────────────────────────────

    [Test]
    public void IsAvailable_NeedsBothAnEndpointAndAToken()
    {
        Assert.IsTrue(AzureDevOpsOidcProvider.IsAvailable(Reader(new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = Endpoint,
            [AzureDevOpsOidcProvider.SystemRequestTokenVariable] = "job-token"
        })));

        Assert.IsFalse(AzureDevOpsOidcProvider.IsAvailable(Reader(new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.SystemRequestUrlVariable] = Endpoint
        })), "an endpoint without a token is not usable");

        Assert.IsFalse(AzureDevOpsOidcProvider.IsAvailable(Reader(new Dictionary<string, string>
        {
            [AzureDevOpsOidcProvider.SystemRequestTokenVariable] = "job-token"
        })), "a token without an endpoint is not usable");

        Assert.IsFalse(AzureDevOpsOidcProvider.IsAvailable(Reader(new Dictionary<string, string>())),
            "a developer machine is not a pipeline");
    }

    // ──────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────

    private static async Task<string> UrlRequestedBy(
        DeployOptions options,
        string serviceConnectionId,
        IDictionary<string, string> environment)
    {
        var handler = Answering(Ok(Jwt()));

        var provider = AzureDevOpsOidcProvider.Create(
            options, serviceConnectionId, Reader(environment), handler.Client);

        await provider.GetAssertionAsync(CancellationToken.None);

        return handler.LastRequest.RequestUri!.ToString();
    }

    private static Func<string, string> Reader(IDictionary<string, string> environment)
        => name => environment.TryGetValue(name, out var value) ? value : null;

    private static string Header(HttpRequestMessage request, string name)
        => request.Headers.TryGetValues(name, out var values) ? string.Join(",", values) : null;

    private static HttpResponseMessage Ok(string oidcToken)
        => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                "{\"oidcToken\":\"" + oidcToken + "\"}", Encoding.UTF8, "application/json")
        };

    /// <summary>A JWT carrying only the two claims the provider reads.</summary>
    private static string Jwt(string subject = Subject)
        => Base64Url("{\"alg\":\"RS256\"}")
           + "." + Base64Url("{\"iss\":\"" + Issuer + "\",\"sub\":\"" + subject + "\"}")
           + ".signature";

    private static int Occurrences(string text, string value)
    {
        var count = 0;

        for (var i = text.IndexOf(value, StringComparison.Ordinal);
             i >= 0;
             i = text.IndexOf(value, i + value.Length, StringComparison.Ordinal))
        {
            count++;
        }

        return count;
    }

    private static string Base64Url(string json)
        => Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

    private static StubHandler Answering(HttpResponseMessage response) => new StubHandler(response, null);

    private static StubHandler Throwing(Exception exception) => new StubHandler(null, exception);

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        private readonly Exception _exception;

        public StubHandler(HttpResponseMessage response, Exception exception)
        {
            _response = response;
            _exception = exception;
            Client = new HttpClient(this);
        }

        public HttpClient Client { get; }

        public HttpRequestMessage LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            if (_exception != null) throw _exception;

            return Task.FromResult(_response);
        }
    }
}
