using System;
using System.Reflection;

namespace XrmFramework
{
    public static class LoggerFactory
    {
        public static ILogger GetLogger(IServiceContext context, LogMethod logMethod)
        {
            var loggerAttribute = typeof(LoggerFactory).Assembly.GetCustomAttribute<LoggerClassAttribute>();

            var loggerType = typeof(DefaultLogHelper);

            if (loggerAttribute != null)
            {
                loggerType = loggerAttribute.LoggerClassType;
            }

            var logger = (ILogger)Activator.CreateInstance(loggerType, new object[] {context.AdminOrganizationService, context, logMethod});

            return logger;
        }
    }
}
