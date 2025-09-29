using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests;

public interface IResultsQueryExpressionTester 
{
    IResultsQueryExpressionTester Results(IEnumerable<Entity> entities);
}
