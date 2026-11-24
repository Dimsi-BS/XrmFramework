// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using XrmFramework.DeployUtils.Configuration.Federated;

namespace XrmFramework.DeployUtils.Tests.Configuration.Federated;

/// <summary>
///     The shortest federation path: an <c>AzureCLI@2</c> step with <c>addSpnToEnvironment: true</c>
///     has already read the service connection and fetched the federated token, and runs the script
///     with <c>idToken</c>, <c>servicePrincipalId</c> and <c>tenantId</c> in its environment. There
///     is then nothing to request and nothing to restate in YAML.
/// </summary>
[TestFixture]
public class PreFetchedOidcTokenProviderTests
{
    private const string Issuer = "https://vstoken.dev.azure.com/contoso";
    private const string Subject = "sc://contoso/Dataverse/DimsiDev";

    [Test]
    public async Task TryCreate_ReadsTheTokenAzureCliExports()
    {
        var provider = PreFetchedOidcTokenProvider.TryCreate(Reader(new Dictionary<string, string>
        {
            [PreFetchedOidcTokenProvider.AzureCliTokenVariable] = Jwt()
        }));

        Assert.IsNotNull(provider);
        Assert.AreEqual(Jwt(), await provider.GetAssertionAsync(CancellationToken.None));
        Assert.IsTrue(provider.PlatformName.Contains("AzureCLI@2"), provider.PlatformName);
    }

    [Test]
    public async Task TryCreate_ReportsIssuerAndSubjectOnFirstUse()
    {
        var logged = new List<string>();

        var provider = PreFetchedOidcTokenProvider.TryCreate(
            Reader(new Dictionary<string, string>
            {
                [PreFetchedOidcTokenProvider.AzureCliTokenVariable] = Jwt()
            }),
            log: logged.Add);

        Assert.AreEqual(Issuer, provider!.Issuer);
        Assert.AreEqual(Subject, provider.Subject);

        // Nothing is logged while the container is still being built: these lines belong after the
        // connection summary, where the request-based provider's land.
        Assert.IsEmpty(logged);

        await provider.GetAssertionAsync(CancellationToken.None);

        Assert.IsTrue(logged.Any(l => l.Contains(Issuer)), "the issuer is not logged");
        Assert.IsTrue(logged.Any(l => l.Contains(Subject)), "the subject is not logged");

        await provider.GetAssertionAsync(CancellationToken.None);

        Assert.AreEqual(3, logged.Count, "the diagnostics are repeated on every token request");
    }

    [Test]
    public async Task TryCreate_OurOwnVariableWinsOverAzureClis()
    {
        var provider = PreFetchedOidcTokenProvider.TryCreate(Reader(new Dictionary<string, string>
        {
            [PreFetchedOidcTokenProvider.TokenVariable] = "ours",
            [PreFetchedOidcTokenProvider.AzureCliTokenVariable] = "azure-clis"
        }));

        Assert.AreEqual("ours", await provider!.GetAssertionAsync(CancellationToken.None));
    }

    [Test]
    public async Task TryCreate_ReadsTheKubernetesStyleTokenFile()
    {
        var provider = PreFetchedOidcTokenProvider.TryCreate(
            Reader(new Dictionary<string, string>
            {
                [PreFetchedOidcTokenProvider.TokenFileVariable] = "/var/run/secrets/token"
            }),
            readFile: path => path == "/var/run/secrets/token" ? "  from-file  " : null);

        Assert.AreEqual("from-file", await provider!.GetAssertionAsync(CancellationToken.None));
    }

    [Test]
    public void TryCreate_UnreadableTokenFile_FallsThrough()
    {
        // An unreadable file must not fail the deployment on its own: the next source, or the
        // request path, still has a chance.
        var provider = PreFetchedOidcTokenProvider.TryCreate(
            Reader(new Dictionary<string, string>
            {
                [PreFetchedOidcTokenProvider.TokenFileVariable] = "/does/not/exist"
            }),
            readFile: _ => null);

        Assert.IsNull(provider);
    }

    [Test]
    public void TryCreate_NothingInTheEnvironment_AnswersNull()
    {
        Assert.IsNull(PreFetchedOidcTokenProvider.TryCreate(Reader(new Dictionary<string, string>())));
        Assert.IsFalse(PreFetchedOidcTokenProvider.IsAvailable(Reader(new Dictionary<string, string>())));
    }

    [Test]
    public void TryCreate_BlankVariable_IsNotAToken()
    {
        Assert.IsNull(PreFetchedOidcTokenProvider.TryCreate(Reader(new Dictionary<string, string>
        {
            [PreFetchedOidcTokenProvider.AzureCliTokenVariable] = "   "
        })));
    }

    [Test]
    public async Task TryCreate_UnreadableToken_IsStillUsable()
    {
        // Reading the claims is diagnostics; a token we cannot decode must still be handed to MSAL.
        var provider = PreFetchedOidcTokenProvider.TryCreate(Reader(new Dictionary<string, string>
        {
            [PreFetchedOidcTokenProvider.TokenVariable] = "not-a-jwt"
        }));

        Assert.AreEqual("not-a-jwt", await provider!.GetAssertionAsync(CancellationToken.None));
        Assert.IsNull(provider.Issuer);
    }

    [Test]
    public void IsAvailable_SeesAnAssertionTheJobAlreadyHolds()
    {
        Assert.IsTrue(PreFetchedOidcTokenProvider.IsAvailable(Reader(new Dictionary<string, string>
        {
            [PreFetchedOidcTokenProvider.AzureCliTokenVariable] = Jwt()
        })));
    }

    private static Func<string, string> Reader(IDictionary<string, string> environment)
        => name => environment.TryGetValue(name, out var value) ? value : null;

    private static string Jwt()
        => Base64Url("{\"alg\":\"RS256\"}")
           + "." + Base64Url("{\"iss\":\"" + Issuer + "\",\"sub\":\"" + Subject + "\"}")
           + ".signature";

    private static string Base64Url(string json)
        => Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
