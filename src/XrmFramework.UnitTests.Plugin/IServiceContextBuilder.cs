using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests.Plugin;

public interface IServiceContextBuilder
{
    
    IServiceContextBuilder WithUserId(Guid userId);
    
    IServiceContextBuilder WithOrganizationName(string organizationName);
    
    IServiceContextBuilder WithCorrelationId(Guid correlationId);

    IServiceContextBuilder WithBusinessUnitRef(EntityReference businessUnitRef);
    
    IServiceContext Build();
}
