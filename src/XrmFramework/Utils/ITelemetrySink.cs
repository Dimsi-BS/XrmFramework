// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Secondary log channel, written to in addition to <see cref="Microsoft.Xrm.Sdk.ITracingService"/>.
    /// In a plugin or workflow activity it is backed by the Dataverse
    /// <c>Microsoft.Xrm.Sdk.PluginTelemetry.ILogger</c>, which writes to the Application Insights
    /// resource configured on the environment.
    /// </summary>
    public interface ITelemetrySink
    {
        bool IsEnabled(XrmLogLevel level);

        void Write(XrmLogLevel level, string message, Exception exception = null);

        IDisposable BeginScope(string name);
    }

    /// <summary>
    /// Sink used whenever no telemetry channel is available (no Application Insights integration,
    /// console or Azure Function host, remote debugger session).
    /// </summary>
    public sealed class NullTelemetrySink : ITelemetrySink
    {
        public static readonly NullTelemetrySink Instance = new NullTelemetrySink();

        private NullTelemetrySink()
        {
        }

        public bool IsEnabled(XrmLogLevel level) => false;

        public void Write(XrmLogLevel level, string message, Exception exception = null)
        {
        }

        public IDisposable BeginScope(string name) => NullScope.Instance;

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();

            public void Dispose()
            {
            }
        }
    }
}
