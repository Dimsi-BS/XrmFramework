using System;
using Microsoft.Xrm.Sdk;
#if !NET8_0_OR_GREATER
using Microsoft.Xrm.Sdk.Workflow;
#endif

namespace XrmFramework.RemoteDebugger.Common
{
    public class LocalServiceProvider : IServiceProvider
    {
        public delegate RemoteDebuggerMessage RequestHandler(RemoteDebuggerMessage messageString);

        public event RequestHandler RequestSent;

        public ITracingService TracingService { get; } = new LocalTracingService();

        public LocalServiceProvider(RemoteDebugExecutionContext context)
        {
            Context = context;

            OrganizationServiceFactory = new LocalOrganizationServiceFactory(Context, OnRequestSent);

            ServiceEndpointNotificationService = new LocalServiceEndpointNotificationService(Context);
        }

        public LocalServiceEndpointNotificationService ServiceEndpointNotificationService { get; }

        public LocalOrganizationServiceFactory OrganizationServiceFactory { get; }

        protected RemoteDebuggerMessage OnRequestSent(RemoteDebuggerMessage message)
        {
            return RequestSent?.Invoke(message);
        }

        private RemoteDebugExecutionContext Context { get; }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ITracingService))
            {
                return TracingService;
            }

            if (serviceType == typeof(IOrganizationServiceFactory))
            {
                return OrganizationServiceFactory;
            }

            if (typeof(IPluginExecutionContext7).IsAssignableFrom(serviceType) 
#if !NET8_0_OR_GREATER
                || typeof(IWorkflowContext).IsAssignableFrom(serviceType) 
#endif
                )
            {
                return Context;
            }

            if (serviceType == typeof(IServiceEndpointNotificationService))
            {
                return ServiceEndpointNotificationService;
            }

            throw new NotImplementedException();
        }
    }
}
