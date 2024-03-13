using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using XrmFramework.RemoteDebugger.Client.Recorder;

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

            if (serviceType == typeof(IPluginExecutionContext) || serviceType == typeof(IWorkflowContext))
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
