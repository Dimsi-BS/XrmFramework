using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    public partial class LocalContext
    {
        public LocalContext(CodeActivityContext context) : this()
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Obtain the execution context service from the service provider.
            ExecutionContext = context.GetExtension<IWorkflowContext>();

            // Obtain the tracing service from the service provider.
            TracingService = context.GetExtension<ITracingService>();

            // Obtain the Organization Service factory service from the service provider
            Factory = context.GetExtension<IOrganizationServiceFactory>();

            // Use the factory to generate the Organization Service.
            OrganizationService = Factory.CreateOrganizationService(ExecutionContext.UserId);

            _businessUnitRef = new EntityReference("businessunit", ExecutionContext.BusinessUnitId);

            Logger = LoggerFactory.GetLogger(this, TracingService.Trace);
        }
    }
}
