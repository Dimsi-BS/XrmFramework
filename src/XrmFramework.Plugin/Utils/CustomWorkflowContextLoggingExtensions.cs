// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.Workflow
{
    /// <summary>
    /// Level aware logging for custom workflow activities. Same behaviour as
    /// <see cref="ContextLoggingExtensions"/>.
    /// </summary>
    public static class CustomWorkflowContextLoggingExtensions
    {
        public static void Log(this ICustomWorkflowContext context, XrmLogLevel level, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, level, message, args, context.Log);

        public static void LogTrace(this ICustomWorkflowContext context, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, XrmLogLevel.Trace, message, args, context.Log);

        public static void LogDebug(this ICustomWorkflowContext context, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, XrmLogLevel.Debug, message, args, context.Log);

        public static void LogInformation(this ICustomWorkflowContext context, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, XrmLogLevel.Information, message, args, context.Log);

        public static void LogWarning(this ICustomWorkflowContext context, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, XrmLogLevel.Warning, message, args, context.Log);

        public static void LogError(this ICustomWorkflowContext context, Exception exception, string message = null, params object[] args)
            => ContextLoggingExtensions.WriteError(context, XrmLogLevel.Error, exception, message, args, context.Log);

        public static void LogCritical(this ICustomWorkflowContext context, Exception exception, string message = null, params object[] args)
            => ContextLoggingExtensions.WriteError(context, XrmLogLevel.Critical, exception, message, args, context.Log);

        public static IDisposable BeginScope(this ICustomWorkflowContext context, string name)
            => ContextLoggingExtensions.OpenScope(context, name);

        public static bool IsTelemetryEnabled(this ICustomWorkflowContext context, XrmLogLevel level)
            => ContextLoggingExtensions.TelemetryEnabled(context, level);
    }
}
