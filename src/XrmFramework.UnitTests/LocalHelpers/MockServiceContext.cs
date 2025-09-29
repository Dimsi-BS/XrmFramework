using Microsoft.Xrm.Sdk;
using Moq;

namespace XrmFramework.UnitTests.LocalHelpers;

public interface IMockServiceContext
{
    
}

internal class MockServiceContext : IMockServiceContext
{
    private readonly Mock<IServiceContext> _serviceContext = new();
    private readonly MockOrganizationService _organizationService = new MockOrganizationService();
    private readonly MockOrganizationService _adminOrganizationService = new MockOrganizationService();

    /*
       void Log(string message, params object[] paramsObject);

       IOrganizationService GetService(Guid userId);

       LogServiceMethod LogServiceMethod { get; }
     */

    public IMockServiceContext UserId(Guid userId)
    {
        _serviceContext.Setup(x => x.UserId).Returns(userId);
        return this;
    }

    public IMockServiceContext InitiatingUserId(Guid initiatingUserId)
    {
        _serviceContext.Setup(x => x.InitiatingUserId).Returns(initiatingUserId);
        return this;
    }

    public IMockServiceContext CorrelationId(Guid correlationId)
    {
        _serviceContext.Setup(x => x.CorrelationId).Returns(correlationId);
        return this;
    }

    public IMockServiceContext OrganizationName(string organizationName)
    {
        _serviceContext.Setup(x => x.OrganizationName).Returns(organizationName);
        return this;
    }

    public IMockServiceContext BusinessUnitRef(EntityReference businessUnitRef)
    {
        _serviceContext.Setup(x => x.BusinessUnitRef).Returns(businessUnitRef);
        return this;
    }

    public IMockOrganizationService OrganizationService()
    {
        return _organizationService;
    }

    public IMockOrganizationService AdminOrganizationService()
    {
        return _adminOrganizationService;
    }
    
    public IServiceContext ServiceContext
    {
        get
        {
            _serviceContext.Setup(x => x.OrganizationService).Returns(_organizationService.Object);
            _serviceContext.Setup(x => x.AdminOrganizationService).Returns(_adminOrganizationService.Object);
            
            return _serviceContext.Object;
        }
    }
}
