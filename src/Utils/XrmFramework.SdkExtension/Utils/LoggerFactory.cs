using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Plugins;

namespace XrmFramework.Utils
{
    public static class LoggerFactory
    {
        public static ILogger GetLogger(LocalContext context, LogMethod logMethod)
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
