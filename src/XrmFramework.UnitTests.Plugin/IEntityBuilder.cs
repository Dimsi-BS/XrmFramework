using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests.Plugin;

public interface IEntityBuilder
{
    IEntityBuilder WithLogicalName(string logicalName);
    IEntityBuilder WithId(Guid id);
    IEntityBuilder WithAttribute(string name, object value);

    Entity Build();
}
