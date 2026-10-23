// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

// Part of the framework runtime layer, which is not nullable-annotated. Stated explicitly
// because these sources are also compiled into nullable-enabled projects
// (XrmFramework.Workflows, the test projects).
#nullable disable

namespace XrmFramework
{
    /// <summary>
    /// Severity levels used by the telemetry channel. Values match
    /// <c>Microsoft.Xrm.Sdk.PluginTelemetry.LogLevel</c>.
    /// </summary>
    public enum XrmLogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }
}
