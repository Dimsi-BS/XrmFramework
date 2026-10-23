// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using SdkLogger = Microsoft.Xrm.Sdk.PluginTelemetry.ILogger;

// Part of the framework runtime layer, which is not nullable-annotated. Stated explicitly
// because these sources are also compiled into nullable-enabled projects
// (XrmFramework.Workflows, the test projects).
#nullable disable

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    public partial class LocalContext
    {
        partial void InitializeTelemetry(IServiceProvider serviceProvider)
        {
            _telemetrySink = CreateTelemetrySink(() => (SdkLogger)serviceProvider.GetService(typeof(SdkLogger)));
        }

        /// <summary>
        /// Also used by the workflow flavour of the context, which resolves the logger from its
        /// <c>CodeActivityContext</c> extensions. This project's sources are compiled into
        /// XrmFramework.Workflows too.
        /// </summary>
        private static ITelemetrySink CreateTelemetrySink(Func<SdkLogger> resolveLogger)
        {
            try
            {
                var logger = resolveLogger();

                return logger == null ? (ITelemetrySink)NullTelemetrySink.Instance : new PluginTelemetrySink(logger);
            }
            catch (Exception)
            {
                // Hosts unaware of the telemetry logger (remote debugger replay, test harnesses)
                // throw instead of returning null.
                return NullTelemetrySink.Instance;
            }
        }
    }
}
