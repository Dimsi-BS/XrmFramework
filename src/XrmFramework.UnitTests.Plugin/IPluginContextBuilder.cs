using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests.Plugin;

public interface IPluginContextBuilder
{
    IPluginContextBuilder WithPrimaryEntityName( string primaryEntityName);
    IPluginContextBuilder WithPrimaryEntityId(Guid primaryEntityId);
    //IPluginContextBuilder WithSecondaryEntityName(string secondaryEntityName);

    IPluginContextBuilder WithDepth(int depth);
    
    IPluginContextBuilder WithUserId(Guid userId);
    IPluginContextBuilder WithInitiatingUserId(Guid initiatingUserId);
    IPluginContextBuilder WithStage(Stages stage);
    
    IPluginContextBuilder WithOrganizationId(Guid organizationId);
    IPluginContextBuilder WithCorrelationId(Guid correlationId);
    
    IPluginContextBuilder WithParentContext(Action<IPluginContextBuilder> builder);
    
    IPluginContextBuilder WithBusinessUnitRef(EntityReference businessUnitRef);
    
    IPluginContextBuilder WithInputParameters(Action<IInputParametersBuilder> builder);
    
}