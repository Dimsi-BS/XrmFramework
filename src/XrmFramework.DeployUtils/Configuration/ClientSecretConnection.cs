// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    ///     Builds a Dataverse connection string from an application and its secret handed over
    ///     directly, rather than declared in <c>connectionStrings.config</c>.
    /// </summary>
    /// <remarks>
    ///     This is what a pipeline whose service connection carries a secret needs: the credentials
    ///     already exist, and writing them into a file first only adds a step and a file to clean
    ///     up. Federation remains the better answer where it is available - there is no secret to
    ///     hand over at all - but it is not available everywhere.
    /// </remarks>
    public static class ClientSecretConnection
    {
        /// <summary>
        ///     The better place to put the secret: a command line is visible to every process on
        ///     the agent, an environment variable is not.
        /// </summary>
        public const string SecretVariable = "XRMFRAMEWORK_CLIENT_SECRET";

        /// <summary>Entra application, under the same name the federated path reads.</summary>
        public const string ClientIdVariable = "XRMFRAMEWORK_CLIENT_ID";

        /// <summary>
        ///     <see langword="true" /> when a secret was supplied, so <c>--auth auto</c> can pick
        ///     this mode without being told.
        /// </summary>
        public static bool IsAvailable(DeployOptions options, Func<string, string> environment)
            => Secret(options, environment) != null;

        /// <summary>
        ///     The connection string for <paramref name="options" />, or throws naming what is
        ///     missing.
        /// </summary>
        /// <param name="options">Deployment options carrying the command-line values.</param>
        /// <param name="connectionString">
        ///     Selected connection string, used only to fill in a value the command line left out.
        /// </param>
        /// <param name="environment">Environment variable reader.</param>
        public static string Build(
            DeployOptions options,
            string connectionString,
            Func<string, string> environment)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            var url = First(options.EnvironmentUrl, Field(connectionString, "Url"), Field(connectionString, "ServiceUri"));
            var clientId = First(options.ApplicationId, Field(connectionString, "ClientId"), Field(connectionString, "AppId"), environment(ClientIdVariable));
            var secret = Secret(options, environment);

            var missing = new List<string>();

            if (url == null) missing.Add("the Dataverse URL (--environment, or Url= in the connection string)");
            if (clientId == null) missing.Add($"the application id (--application-id, or ${ClientIdVariable})");
            if (secret == null) missing.Add($"the secret (--secret, or ${SecretVariable})");

            if (missing.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException(
                    "Signing in with an application secret is missing " + string.Join(", ", missing) + ".");

            // Built in the shape the Dataverse client expects, and nothing more: the tenant is
            // discovered from the organization, so asking for it would only be one more value to
            // get wrong.
            return $"AuthType=ClientSecret; Url={url}; ClientId={clientId}; ClientSecret={secret}";
        }

        private static string Secret(DeployOptions options, Func<string, string> environment)
            => First(options?.ClientSecret, environment(SecretVariable));

        private static string First(params string[] candidates)
        {
            foreach (var candidate in candidates)
            {
                if (!string.IsNullOrWhiteSpace(candidate)) return candidate.Trim();
            }

            return null;
        }

        /// <remarks>
        ///     Read by hand rather than through <see cref="ConnectionStringParser" />: that one is
        ///     case-sensitive and splits the whole segment on '=', which mangles a value containing
        ///     one.
        /// </remarks>
        private static string Field(string connectionString, string name)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return null;

            foreach (var segment in connectionString.Split(';'))
            {
                var separator = segment.IndexOf('=');

                if (separator <= 0) continue;

                if (!segment.Substring(0, separator).Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
                    continue;

                var value = segment.Substring(separator + 1).Trim();

                if (!string.IsNullOrEmpty(value)) return value;
            }

            return null;
        }
    }
}
