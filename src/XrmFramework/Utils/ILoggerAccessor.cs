// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

// Part of the framework runtime layer, which is not nullable-annotated. Stated explicitly
// because these sources are also compiled into nullable-enabled projects
// (XrmFramework.Workflows, the test projects).
#nullable disable

namespace XrmFramework
{
    /// <summary>
    /// Implemented by contexts willing to expose their logger, so that the level aware extension
    /// methods of <see cref="ContextLoggingExtensions"/> can reach it without adding members to
    /// the context interfaces.
    /// </summary>
    public interface ILoggerAccessor
    {
        ILogger Logger { get; }
    }
}
