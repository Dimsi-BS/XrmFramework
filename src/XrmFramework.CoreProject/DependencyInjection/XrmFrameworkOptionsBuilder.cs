
#if PLUGIN || CORE_PROJECT

using Microsoft.Extensions.DependencyInjection;

namespace XrmFramework.DependencyInjection
{
    public interface IXrmFrameworkOptionBuilder
    {
        IXrmFrameworkOptionBuilder UseConnectionString(string connectionString);
        IXrmFrameworkOptionBuilder UseWebApi(bool useWebApi);
    }

    public class XrmFrameworkOptionBuilder : IXrmFrameworkOptionBuilder
    {
        public IServiceCollection ServiceCollection { get; }

        public XrmFrameworkOptionBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        internal string ConnectionString { get; private set; }

        internal bool WebApiUsage { get; private set; }

        internal bool UseWebApiForced { get; private set; }

        public IXrmFrameworkOptionBuilder UseConnectionString(string connectionString)
        {
            ConnectionString = connectionString;

            return this;
        }

        public IXrmFrameworkOptionBuilder UseWebApi(bool useWebApi)
        {
            WebApiUsage = useWebApi;
            UseWebApiForced = true;

            return this;
        }
    }
}

#endif