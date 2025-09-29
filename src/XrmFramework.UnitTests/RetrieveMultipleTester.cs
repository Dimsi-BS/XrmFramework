using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

public class RetrieveMultipleTester : IRetrieveMultipleTester, IVerifiable
{
    private readonly QueryExpressionTester _queryExpressionTester = new();
    private readonly List<Entity> _results = new();
    private QueryExpression _queryExpression;
    
    public IList<Entity> Results => _results;
    
    public IRetrieveMultipleTester Query(Action<IFullQueryExpressionTester> queryTester)
    {
        queryTester(_queryExpressionTester);
        
        return this;
    }

    public IRetrieveMultipleTester Returns(IEnumerable<Entity> entities)
    {
        _results.AddRange(entities);

        return this;
    }

    public IEnumerable<ValidationResult> Verify()
        => _queryExpressionTester.Validate(_queryExpression);

    internal void SetQueryExpression(QueryExpression queryExpression)
    {
        _queryExpression = queryExpression;
    }
}
