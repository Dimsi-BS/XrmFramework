using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests;

public interface IRetrieveMultipleTester
{
    IRetrieveMultipleTester Query(Action<IFullQueryExpressionTester> queryTester);
    
    IRetrieveMultipleTester Returns(IEnumerable<Entity> entities);
}
