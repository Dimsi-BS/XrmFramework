// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    ///     Reads the <c>--auth</c> value.
    /// </summary>
    /// <remarks>
    ///     Hand-written rather than left to the enum's own converter, so the spellings a pipeline
    ///     author actually reaches for are accepted - <c>wif</c> above all, which is what the
    ///     feature is called everywhere except in this enum.
    /// </remarks>
    public static class AuthenticationModeParser
    {
        /// <summary>Accepted spellings, for help and error messages.</summary>
        public const string AcceptedValues = "auto, connectionstring or wif";

        /// <summary>
        ///     Parses <paramref name="value" />, treating a missing one as
        ///     <see cref="AuthenticationMode.Auto" />.
        /// </summary>
        /// <returns><see langword="false" /> when the value means nothing.</returns>
        public static bool TryParse(string value, out AuthenticationMode mode)
        {
            mode = AuthenticationMode.Auto;

            if (string.IsNullOrWhiteSpace(value))
                return true;

            switch (value.Trim().ToLowerInvariant())
            {
                case "auto":
                    mode = AuthenticationMode.Auto;
                    return true;

                case "connectionstring":
                case "connection-string":
                    mode = AuthenticationMode.ConnectionString;
                    return true;

                case "wif":
                case "workloadidentity":
                case "workload-identity":
                    mode = AuthenticationMode.WorkloadIdentity;
                    return true;

                default:
                    return false;
            }
        }
    }
}
