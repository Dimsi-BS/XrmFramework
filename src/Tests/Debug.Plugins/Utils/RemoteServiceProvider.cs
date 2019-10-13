using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Plugins;
using XrmFramework.Debugger;

namespace Debug.Plugins
{
    public class RemoteServiceProvider : IServiceProvider
    {
        public delegate RemoteDebuggerMessage RequestHandler(RemoteDebuggerMessage message);

        public event RequestHandler RequestSent;

        public ITracingService TracingService { get; } = new LocalTracingService();

        public RemoteServiceProvider(RemoteDebugPluginExecutionContext context)
        {
            Context = context;

            OrganizationServiceFactory = new LocalOrganizationServiceFactory(Context, OnRequestSent);
        }

        public LocalOrganizationServiceFactory OrganizationServiceFactory { get; }

        protected RemoteDebuggerMessage OnRequestSent(RemoteDebuggerMessage message)
        {
            return RequestSent?.Invoke(message);
        }

        private RemoteDebugPluginExecutionContext Context { get; }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ITracingService))
            {
                return TracingService;
            }
            else if (serviceType == typeof(IOrganizationServiceFactory))
            {
                return OrganizationServiceFactory;
            }
            else if (serviceType == typeof(IPluginExecutionContext))
            {
                return Context;
            }

            throw new NotImplementedException();
        }
    }
}
