// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Level aware logging for Custom API methods. Same behaviour as
    /// <see cref="ContextLoggingExtensions"/>.
    /// </summary>
    public static class CustomApiContextLoggingExtensions
    {
        public static void Log(this ICustomApiContext context, XrmLogLevel level, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, level, message, args, context.Log);

        public static void LogTrace(this ICustomApiContext context, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, XrmLogLevel.Trace, message, args, context.Log);

        public static void LogDebug(this ICustomApiContext context, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, XrmLogLevel.Debug, message, args, context.Log);

        public static void LogInformation(this ICustomApiContext context, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, XrmLogLevel.Information, message, args, context.Log);

        public static void LogWarning(this ICustomApiContext context, string message, params object[] args)
            => ContextLoggingExtensions.Write(context, XrmLogLevel.Warning, message, args, context.Log);

        public static void LogError(this ICustomApiContext context, Exception exception, string message = null, params object[] args)
            => ContextLoggingExtensions.WriteError(context, XrmLogLevel.Error, exception, message, args, context.Log);

        public static void LogCritical(this ICustomApiContext context, Exception exception, string message = null, params object[] args)
            => ContextLoggingExtensions.WriteError(context, XrmLogLevel.Critical, exception, message, args, context.Log);

        public static IDisposable BeginScope(this ICustomApiContext context, string name)
            => ContextLoggingExtensions.OpenScope(context, name);

        public static bool IsTelemetryEnabled(this ICustomApiContext context, XrmLogLevel level)
            => ContextLoggingExtensions.TelemetryEnabled(context, level);
    }
}
