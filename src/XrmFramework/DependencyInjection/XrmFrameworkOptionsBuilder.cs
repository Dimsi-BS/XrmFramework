
#if NETCOREAPP3_1

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.DependencyInjection
{
    public class XrmFrameworkOptionBuilder
    {
        internal string ConnectionString { get; private set; }

        internal Assembly LoggedServiceAssembly { get; private set; }

        public XrmFrameworkOptionBuilder UseConnectionString(string connectionString)
        {
            ConnectionString = connectionString;

            return this;
        }

        public XrmFrameworkOptionBuilder AddLoggedServices<TLoggedService>() where TLoggedService : LoggedServiceBase
        {
            LoggedServiceAssembly = typeof(TLoggedService).Assembly;

            return this;
        }
    }
}

#endif