// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER

using System;

namespace XrmFramework.DeployUtils.Configuration.Federated
{
    /// <summary>
    ///     Raised when a workload identity federation sign-in cannot even be attempted, or when the
    ///     CI/CD platform refused to mint the OIDC token.
    /// </summary>
    /// <remarks>
    ///     Its message is expected to name the missing piece and say how to supply it: every failure
    ///     here happens on a build agent, where the only thing the reader has is this one line.
    /// </remarks>
    public sealed class FederatedAuthenticationException : Exception
    {
        public FederatedAuthenticationException(string message) : base(message)
        {
        }

        public FederatedAuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

#endif
