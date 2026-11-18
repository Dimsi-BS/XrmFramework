using System;
using Microsoft.Xrm.Sdk;
#if XRMFRAMEWORK_WORKFLOW_ACTIVITIES
using Microsoft.Xrm.Sdk.Workflow;
#endif

namespace XrmFramework.RemoteDebugger.Common
{
    public class LocalServiceProvider : IServiceProvider
    {
        public delegate RemoteDebuggerMessage RequestHandler(RemoteDebuggerMessage messageString);

        public event RequestHandler RequestSent;

        public ITracingService TracingService { get; }

        public LocalServiceProvider(RemoteDebugExecutionContext context, Action<string> traceSink = null)
        {
            TracingService = new LocalTracingService(traceSink);
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

            if (serviceType == typeof(IPluginExecutionContext))
            {
                return Context;
            }

#if XRMFRAMEWORK_WORKFLOW_ACTIVITIES
            // The same context object serves both: RemoteDebugExecutionContext implements
            // IPluginExecutionContext in XrmFramework.Plugin and IWorkflowContext in a partial over
            // in XrmFramework.Workflows, which DeployUtils links on the full-framework leg only.
            // So on .NET the type is not merely unreferenceable here - that facet of the context does
            // not exist, which is the same reason workflow activities cannot run there.
            if (serviceType == typeof(IWorkflowContext))
            {
                return Context;
            }
#endif

            if (serviceType == typeof(IServiceEndpointNotificationService))
            {
                return ServiceEndpointNotificationService;
            }

            throw new NotImplementedException();
        }
    }
}
