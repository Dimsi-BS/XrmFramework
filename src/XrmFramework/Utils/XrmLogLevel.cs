// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

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
