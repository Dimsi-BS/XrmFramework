
#if !DISABLE_DI

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

        internal bool WebApiUsage { get; private set; }

        internal bool UseWebApiForced { get; private set; }

        public XrmFrameworkOptionBuilder UseConnectionString(string connectionString)
        {
            ConnectionString = connectionString;

            return this;
        }

        public XrmFrameworkOptionBuilder UseWebApi(bool useWebApi)
        {
            WebApiUsage = useWebApi;
            UseWebApiForced = true;

            return this;
        }
    }
}

#endif