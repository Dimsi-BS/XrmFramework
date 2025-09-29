using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests;

public interface IRetrieveAllChecker
{
    IRetrieveAllContext Query(Action<IFullQueryExpressionTester> builder);
    
    IRetrieveAllContext Returns(IEnumerable<Entity> entities);
}
