using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests;

public interface IRetrieveAllContext
{
    IRetrieveAllContext Returns(IEnumerable<Entity> entities);
}
