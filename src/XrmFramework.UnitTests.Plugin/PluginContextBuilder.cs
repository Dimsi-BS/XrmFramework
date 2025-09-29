using Microsoft.Xrm.Sdk;
using Moq;

namespace XrmFramework.UnitTests.Plugin;

public class PluginContextBuilder : IPluginContextBuilder
{
    private readonly Mock<IPluginContext> _pluginContextMock = new();

    private Stages _stage;
    
    public IPluginContextBuilder WithPrimaryEntityName(string primaryEntityName)
    {
        _pluginContextMock.Setup(x => x.PrimaryEntityName).Returns(primaryEntityName);
        return this;
    }

    public IPluginContextBuilder WithPrimaryEntityId(Guid primaryEntityId)
    {
        _pluginContextMock.Setup(x => x.PrimaryEntityId).Returns(primaryEntityId);
        return this;
    }

    [Obsolete("Obsolete")]
    public IPluginContextBuilder WithDepth(int depth)
    {
        _pluginContextMock.Setup(x => x.Depth).Returns(depth);
        return this;
    }

    public IPluginContextBuilder WithUserId(Guid userId)
    {
        _pluginContextMock.Setup(x => x.UserId).Returns(userId);
        return this;
    }

    public IPluginContextBuilder WithInitiatingUserId(Guid initiatingUserId)
    {
        _pluginContextMock.Setup(x => x.InitiatingUserId).Returns(initiatingUserId);
        return this;
    }

    public IPluginContextBuilder WithStage(Stages stage)
    {
        _stage = stage;
        _pluginContextMock.Setup(x => x.IsPostOperation()).Returns(_stage == Stages.PostOperation);
        _pluginContextMock.Setup(x => x.IsPreOperation()).Returns(_stage == Stages.PreOperation);
        _pluginContextMock.Setup(x => x.IsPreValidation()).Returns(_stage == Stages.PreValidation);
        _pluginContextMock.Setup(x => x.IsStage(It.IsAny<Stages>())).Returns<Stages>(stageParam => stageParam == _stage);
        return this;
    }

    public IPluginContextBuilder WithOrganizationId(Guid organizationId)
    {
        _pluginContextMock.Setup(x => x.OrganizationId).Returns(organizationId);
        return this;
    }

    public IPluginContextBuilder WithCorrelationId(Guid correlationId)
    {
        _pluginContextMock.Setup(x => x.CorrelationId).Returns(correlationId);
        return this;
    }

    public IPluginContextBuilder WithParentContext(Action<IPluginContextBuilder> builder)
    {
        var parentContextBuilder = new PluginContextBuilder();
        builder.Invoke(parentContextBuilder);
        
        _pluginContextMock.Setup(x => x.ParentContext).Returns(parentContextBuilder.Build());
        return this;
    }

    public IPluginContextBuilder WithBusinessUnitRef(EntityReference businessUnitRef)
    {
        _pluginContextMock.Setup(x => x.BusinessUnitRef).Returns(businessUnitRef);
        return this;
    }

    public IPluginContextBuilder WithInputParameters(Action<IInputParametersBuilder> builder)
    {
        var inputParametersBuilder = new InputParametersBuilder();
        builder.Invoke(inputParametersBuilder);

        var inputParameters = inputParametersBuilder.Build();
        
        _pluginContextMock
            .Setup(x => x.GetInputParameter<object>(It.IsAny<InputParameters>()))
            .Returns<InputParameters>(parameterName => inputParameters[parameterName]);

        return this;
    }

    public IPluginContext Build()
        => _pluginContextMock.Object;
}
