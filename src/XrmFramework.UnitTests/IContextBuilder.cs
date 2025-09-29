using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests;

public interface IContextBuilder
{
    IContextBuilder Target(Action<ITargetBuilder> configure);
    
}

public interface ITargetBuilder
{
    IContextBuilder Entity(Action<IEntityBuilder> configure);
    IContextBuilder Entity(Entity entity);
    IContextBuilder EntityReference(EntityReference reference);
    
    
}
