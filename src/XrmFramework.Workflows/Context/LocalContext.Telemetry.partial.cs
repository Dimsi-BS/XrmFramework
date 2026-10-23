// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Activities;
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
        private void InitializeTelemetry(CodeActivityContext context)
        {
            _telemetrySink = CreateTelemetrySink(context.GetExtension<SdkLogger>);
        }
    }
}
