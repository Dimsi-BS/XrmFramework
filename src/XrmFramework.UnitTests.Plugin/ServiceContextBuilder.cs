using Microsoft.Xrm.Sdk;
using Moq;

namespace XrmFramework.UnitTests.Plugin;

public class ServiceContextBuilder : IServiceContextBuilder
{
    private readonly Mock<IServiceContext> _serviceContextMock = new();
    
    public IServiceContextBuilder WithUserId(Guid userId)
    {
        _serviceContextMock.Setup(x => x.UserId).Returns(userId);
        return this;
    }

    public IServiceContextBuilder WithOrganizationName(string organizationName)
    {
        _serviceContextMock.Setup(x => x.OrganizationName).Returns(organizationName);
        return this;
    }

    public IServiceContextBuilder WithCorrelationId(Guid correlationId)
    {
        _serviceContextMock.Setup(x => x.CorrelationId).Returns(correlationId);
        return this;
    }

    public IServiceContextBuilder WithBusinessUnitRef(EntityReference businessUnitRef)
    {
        _serviceContextMock.Setup(x => x.BusinessUnitRef).Returns(businessUnitRef);
        return this;
    }

    public IServiceContextBuilder WithInitiatingUserId(Guid initiatingUserId)
    {
        _serviceContextMock.Setup(x => x.InitiatingUserId).Returns(initiatingUserId);
        return this;
    }

    public IServiceContext Build()
        => _serviceContextMock.Object;
}