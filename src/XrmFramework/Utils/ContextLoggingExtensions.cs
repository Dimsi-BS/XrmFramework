// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Level aware logging for plugins, workflow activities and services. Every call writes to the
    /// tracing channel exactly like <c>Log</c> does, and additionally to the telemetry channel
    /// (Application Insights) when the level reaches the configured minimum.
    /// Contexts that don't expose a <see cref="ILeveledLogger"/> fall back to <c>Log</c>.
    /// </summary>
    public static class ContextLoggingExtensions
    {
        public static void Log(this IContext context, XrmLogLevel level, string message, params object[] args)
            => Write(context, level, message, args, context.Log);

        public static void LogTrace(this IContext context, string message, params object[] args)
            => Write(context, XrmLogLevel.Trace, message, args, context.Log);

        public static void LogDebug(this IContext context, string message, params object[] args)
            => Write(context, XrmLogLevel.Debug, message, args, context.Log);

        public static void LogInformation(this IContext context, string message, params object[] args)
            => Write(context, XrmLogLevel.Information, message, args, context.Log);

        public static void LogWarning(this IContext context, string message, params object[] args)
            => Write(context, XrmLogLevel.Warning, message, args, context.Log);

        public static void LogError(this IContext context, Exception exception, string message = null, params object[] args)
            => WriteError(context, XrmLogLevel.Error, exception, message, args, context.Log);

        public static void LogCritical(this IContext context, Exception exception, string message = null, params object[] args)
            => WriteError(context, XrmLogLevel.Critical, exception, message, args, context.Log);

        /// <summary>
        /// Opens an Application Insights scope: every entry written until the returned object is
        /// disposed carries the scope in its <c>customDimensions</c>. No effect on the tracing
        /// channel.
        /// </summary>
        public static IDisposable BeginScope(this IContext context, string name) => OpenScope(context, name);

        /// <summary>
        /// Tells whether the telemetry channel writes at that level, to guard expensive
        /// diagnostics. The tracing channel always writes, whatever this returns.
        /// </summary>
        public static bool IsTelemetryEnabled(this IContext context, XrmLogLevel level) => TelemetryEnabled(context, level);

        public static void Log(this IServiceContext context, XrmLogLevel level, string message, params object[] args)
            => Write(context, level, message, args, context.Log);

        public static void LogTrace(this IServiceContext context, string message, params object[] args)
            => Write(context, XrmLogLevel.Trace, message, args, context.Log);

        public static void LogDebug(this IServiceContext context, string message, params object[] args)
            => Write(context, XrmLogLevel.Debug, message, args, context.Log);

        public static void LogInformation(this IServiceContext context, string message, params object[] args)
            => Write(context, XrmLogLevel.Information, message, args, context.Log);

        public static void LogWarning(this IServiceContext context, string message, params object[] args)
            => Write(context, XrmLogLevel.Warning, message, args, context.Log);

        public static void LogError(this IServiceContext context, Exception exception, string message = null, params object[] args)
            => WriteError(context, XrmLogLevel.Error, exception, message, args, context.Log);

        public static void LogCritical(this IServiceContext context, Exception exception, string message = null, params object[] args)
            => WriteError(context, XrmLogLevel.Critical, exception, message, args, context.Log);

        public static IDisposable BeginScope(this IServiceContext context, string name) => OpenScope(context, name);

        public static bool IsTelemetryEnabled(this IServiceContext context, XrmLogLevel level) => TelemetryEnabled(context, level);

        internal static void Write(object context, XrmLogLevel level, string message, object[] args, LogMethod fallback)
        {
            var logger = GetLeveledLogger(context);

            if (logger == null)
            {
                fallback(message, args);
                return;
            }

            logger.Log(level, message, args);
        }

        internal static void WriteError(object context, XrmLogLevel level, Exception exception, string message, object[] args, LogMethod fallback)
        {
            var logger = GetLeveledLogger(context);

            if (logger == null)
            {
                fallback("ERROR : {0}\r\n{1}", new object[] {DefaultLogHelper.SafeFormat(message ?? string.Empty, args), exception});
                return;
            }

            logger.LogError(level, exception, message, args);
        }

        internal static IDisposable OpenScope(object context, string name)
            => GetLeveledLogger(context)?.BeginScope(name) ?? NullTelemetrySink.Instance.BeginScope(name);

        internal static bool TelemetryEnabled(object context, XrmLogLevel level)
            => GetLeveledLogger(context)?.IsTelemetryEnabled(level) == true;

        private static ILeveledLogger GetLeveledLogger(object context)
            => (context as ILoggerAccessor)?.Logger as ILeveledLogger;
    }
}
