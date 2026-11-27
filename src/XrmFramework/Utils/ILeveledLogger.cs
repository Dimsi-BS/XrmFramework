// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

// Part of the framework runtime layer, which is not nullable-annotated. Stated explicitly
// because these sources are also compiled into nullable-enabled projects
// (XrmFramework.Workflows, the test projects).
#nullable disable

namespace XrmFramework
{
    /// <summary>
    /// Level aware logging, implemented by <see cref="DefaultLogHelper"/>. Kept separate from
    /// <see cref="ILogger"/> so that existing custom <see cref="ILogger"/> implementations keep
    /// compiling.
    /// </summary>
    public interface ILeveledLogger
    {
        /// <summary>
        /// Tells whether the telemetry channel writes at that level. The tracing channel always
        /// writes, whatever this returns.
        /// </summary>
        bool IsTelemetryEnabled(XrmLogLevel level);

        void Log(XrmLogLevel level, string message, params object[] args);

        void LogError(XrmLogLevel level, Exception exception, string message = null, params object[] args);

        IDisposable BeginScope(string name);
    }
}
