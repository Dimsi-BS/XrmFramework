// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER

using System.Threading;
using System.Threading.Tasks;

namespace XrmFramework.DeployUtils.Configuration.Federated
{
    /// <summary>
    ///     Source of the OIDC token a CI/CD platform hands out to prove the identity of the running
    ///     job, used as the client assertion of a workload identity federation sign-in.
    /// </summary>
    /// <remarks>
    ///     An interface for one implementation on purpose: the Azure DevOps and GitHub Actions
    ///     exchanges differ only in where the request endpoint comes from, so the second one should
    ///     be an added file rather than a rewrite of the sign-in path.
    /// </remarks>
    public interface IOidcTokenProvider
    {
        /// <summary>Where the token came from, for error and log messages.</summary>
        string PlatformName { get; }

        /// <summary>
        ///     Issuer claim of the last token obtained, or <see langword="null" />.
        /// </summary>
        /// <remarks>
        ///     Part of the contract because it is half of what has to be pasted into the app
        ///     registration's federated credential, and the sign-in reports both when Entra
        ///     rejects the assertion - whichever provider produced it.
        /// </remarks>
        string Issuer { get; }

        /// <summary>Subject claim of the last token obtained, or <see langword="null" />.</summary>
        string Subject { get; }

        /// <summary>
        ///     Requests a fresh OIDC token.
        /// </summary>
        /// <remarks>
        ///     Called by MSAL only when it has no cached Dataverse token, so there is no caching to
        ///     do here: these tokens are short-lived and the platform mints a new one on demand.
        /// </remarks>
        /// <exception cref="FederatedAuthenticationException">
        ///     The job is not configured for federation, or the platform refused the request.
        /// </exception>
        Task<string> GetAssertionAsync(CancellationToken cancellationToken);
    }
}

#endif
