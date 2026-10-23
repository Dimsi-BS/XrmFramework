// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using SdkLogLevel = Microsoft.Xrm.Sdk.PluginTelemetry.LogLevel;
using SdkLogger = Microsoft.Xrm.Sdk.PluginTelemetry.ILogger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    /// <summary>
    /// Writes to the Application Insights resource configured on the environment, through the
    /// Dataverse <c>Microsoft.Xrm.Sdk.PluginTelemetry.ILogger</c>. Nothing is written when the
    /// environment has no Application Insights integration.
    /// </summary>
    internal sealed class PluginTelemetrySink : ITelemetrySink
    {
        /// <summary>
        /// Messages reaching this sink are already formatted; a fixed structured template keeps a
        /// message holding braces from breaking the call.
        /// </summary>
        private const string MessageTemplate = "{Message}";

        private readonly SdkLogger _logger;

        public PluginTelemetrySink(SdkLogger logger)
        {
            _logger = logger;
        }

        public bool IsEnabled(XrmLogLevel level)
        {
            if (level == XrmLogLevel.None)
            {
                return false;
            }

            try
            {
                return _logger.IsEnabled(Map(level));
            }
            catch (Exception)
            {
                // A failing telemetry channel must never fail the plugin.
                return false;
            }
        }

        public void Write(XrmLogLevel level, string message, Exception exception = null)
        {
            try
            {
                if (exception == null)
                {
                    _logger.Log(Map(level), MessageTemplate, message);
                }
                else
                {
                    _logger.Log(Map(level), exception, MessageTemplate, message);
                }
            }
            catch (Exception)
            {
                // A failing telemetry channel must never fail the plugin.
            }
        }

        public IDisposable BeginScope(string name)
        {
            try
            {
                return _logger.BeginScope(name) ?? NullTelemetrySink.Instance.BeginScope(name);
            }
            catch (Exception)
            {
                return NullTelemetrySink.Instance.BeginScope(name);
            }
        }

        private static SdkLogLevel Map(XrmLogLevel level) => (SdkLogLevel)(int)level;
    }
}
