// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Activities;
using SdkLogger = Microsoft.Xrm.Sdk.PluginTelemetry.ILogger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    public partial class LocalContext
    {
        partial void InitializeTelemetry(IServiceProvider serviceProvider)
        {
            _telemetrySink = CreateTelemetrySink(() => (SdkLogger)serviceProvider.GetService(typeof(SdkLogger)));
        }

        private void InitializeTelemetry(CodeActivityContext context)
        {
            _telemetrySink = CreateTelemetrySink(context.GetExtension<SdkLogger>);
        }

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
