// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    ///     How the deployment proves who it is to Dataverse, chosen by the <c>--auth</c>
    ///     command-line option.
    /// </summary>
    public enum AuthenticationMode
    {
        /// <summary>
        ///     Use workload identity federation when the job looks like it offers it, and the
        ///     connection string otherwise. The default: a pipeline needs no new switch, and a
        ///     developer machine keeps behaving exactly as before.
        /// </summary>
        Auto,

        /// <summary>
        ///     Always read the connection string selected in <c>xrmFramework.config</c>.
        /// </summary>
        ConnectionString,

        /// <summary>
        ///     Always sign in through workload identity federation, failing with an explicit
        ///     message when the job does not provide what that needs.
        /// </summary>
        WorkloadIdentity
    }
}
