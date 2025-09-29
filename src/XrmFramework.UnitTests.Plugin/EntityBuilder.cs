using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests.Plugin;

internal class EntityBuilder : IEntityBuilder
{
    private readonly Entity _entity = new();
    
    public IEntityBuilder WithLogicalName(string logicalName)
    {
        _entity.LogicalName = logicalName;

        return this;
    }

    public IEntityBuilder WithId(Guid id)
    {
        _entity.Id = id;

        return this;
    }

    public IEntityBuilder WithAttribute(string name, object value)
    {
        _entity[name] = value;

        return this;
    }

    public Entity Build() 
        => _entity;
}