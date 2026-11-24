// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.Linq;
using Spectre.Console;

namespace XrmFramework.DeployUtils.Reporting
{
    /// <summary>
    ///     Turns what a deployment can throw into the exit code and the one line a build log needs.
    /// </summary>
    /// <remarks>
    ///     Shared by every <c>deploy</c> command so they report the same way: a misconfiguration and
    ///     a failed sign-in are not crashes, and a stack trace around them buries the sentence that
    ///     says what to fix.
    /// </remarks>
    public static class DeployErrors
    {
        /// <summary>Exit code for a deployment that could not run.</summary>
        public const int FailureExitCode = 3;

        /// <summary>
        ///     Runs <paramref name="deployment" /> and answers the process exit code.
        /// </summary>
        /// <returns>
        ///     <c>0</c> on success, and on a declined confirmation - doing exactly as told is not a
        ///     failure, and nothing has been written. <see cref="FailureExitCode" /> otherwise.
        /// </returns>
        public static int Report(Action deployment)
        {
            try
            {
                deployment();

                return 0;
            }
            catch (ConfigurationErrorsException ex)
            {
                // A misconfiguration: the message names the file and the entry to fix.
                return Fail(ex.Message);
            }
            catch (OperationCanceledException ex)
            {
                AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(ex.Message)}[/]");

                return 0;
            }
            catch (Exception ex)
            {
#if !NET462_OR_GREATER
                // A federation failure has to be dug out of the chain: ServiceClient wraps whatever
                // its token provider threw in a connection exception of its own, so it is never the
                // top-level exception.
                var federated = FindFederatedCause(ex);

                if (federated != null) return Fail(federated.Message);
#endif

                AnsiConsole.WriteException(ex);

                return FailureExitCode;
            }
        }

        private static int Fail(string message)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");

            return FailureExitCode;
        }

#if !NET462_OR_GREATER
        /// <summary>
        ///     The federation failure buried in <paramref name="exception" />, or
        ///     <see langword="null" />.
        /// </summary>
        internal static Configuration.Federated.FederatedAuthenticationException FindFederatedCause(
            Exception exception)
        {
            switch (exception)
            {
                case null:
                    return null;

                case Configuration.Federated.FederatedAuthenticationException federated:
                    return federated;

                // Flattened by hand because a token provider runs on a task: what surfaces can be
                // an AggregateException whose interesting branch is not the first one.
                case AggregateException aggregate:
                    return aggregate.InnerExceptions
                        .Select(FindFederatedCause)
                        .FirstOrDefault(found => found != null);

                default:
                    return FindFederatedCause(exception.InnerException);
            }
        }
#endif
    }
}
