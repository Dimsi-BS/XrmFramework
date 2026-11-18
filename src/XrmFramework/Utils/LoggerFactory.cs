using System;
using System.Reflection;

namespace XrmFramework
{
    public static class LoggerFactory
    {
        public static ILogger GetLogger(IServiceContext context, LogMethod logMethod)
            => GetLogger(context, logMethod, NullTelemetrySink.Instance);

        public static ILogger GetLogger(IServiceContext context, LogMethod logMethod, ITelemetrySink telemetrySink)
        {
            var assembly = typeof(LoggerFactory).Assembly;

            var loggerAttribute = assembly.GetCustomAttribute<LoggerClassAttribute>();

            var loggerType = typeof(DefaultLogHelper);

            if (loggerAttribute != null)
            {
                loggerType = loggerAttribute.LoggerClassType;
            }

            var logger = (ILogger)Activator.CreateInstance(loggerType, new object[] {context.AdminOrganizationService, context, logMethod});

            if (logger is DefaultLogHelper defaultLogHelper)
            {
                defaultLogHelper.TelemetrySink = telemetrySink ?? NullTelemetrySink.Instance;

                var telemetryOptions = assembly.GetCustomAttribute<TelemetryOptionsAttribute>();

                if (telemetryOptions != null)
                {
                    defaultLogHelper.MinimumTelemetryLevel = telemetryOptions.MinimumLevel;
                }
            }

            return logger;
        }
    }
}
