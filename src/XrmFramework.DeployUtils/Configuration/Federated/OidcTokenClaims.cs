// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER

using System;
using System.Text.Json;

namespace XrmFramework.DeployUtils.Configuration.Federated
{
    /// <summary>
    ///     The two claims of an OIDC token worth reporting: they are exactly what the app
    ///     registration's federated credential has to be configured with, and reading them off a
    ///     failed pipeline run is how that gets configured right the first time.
    /// </summary>
    internal static class OidcTokenClaims
    {
        /// <summary>
        ///     Issuer and subject of <paramref name="jwt" />, or <see langword="null" /> when it
        ///     cannot be read.
        /// </summary>
        /// <remarks>
        ///     Decoded by hand rather than with a JWT library: two claims are not worth a
        ///     dependency, and this is diagnostics - a token we cannot read must still be usable,
        ///     so every failure answers null instead of throwing.
        /// </remarks>
        internal static (string Issuer, string Subject)? Read(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt)) return null;

            try
            {
                var parts = jwt.Split('.');

                if (parts.Length < 2) return null;

                using (var document = JsonDocument.Parse(DecodeBase64Url(parts[1])))
                {
                    var root = document.RootElement;

                    if (root.ValueKind != JsonValueKind.Object) return null;

                    return (
                        root.TryGetProperty("iss", out var issuer) ? issuer.GetString() : null,
                        root.TryGetProperty("sub", out var subject) ? subject.GetString() : null);
                }
            }
            catch
            {
                return null;
            }
        }

        private static byte[] DecodeBase64Url(string value)
        {
            var padded = value.Replace('-', '+').Replace('_', '/');

            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }

            return Convert.FromBase64String(padded);
        }
    }
}

#endif
