using Microsoft.Xrm.Sdk;

namespace XrmFramework;

partial class LocalContext
{
    protected LocalContext(LocalContext context, IPluginExecutionContext parentContext) : this()
    {
        ExecutionContext = parentContext;
        TracingService = context.TracingService;
        Factory = context.Factory;
        OrganizationService = context.OrganizationService;
        _businessUnitRef = context._businessUnitRef;
        Logger = context.Logger;
    }
}