using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;

namespace XrmFramework.UnitTests;

internal class RetrieveAllTester(Mock<IOrganizationService> mock) : IRetrieveAllChecker, IRetrieveAllContext, IVerifiable
{
    private readonly RetrieveMultipleTester _retrieveMultipleTester = new();
    private readonly List<List<Entity>> _returnedEntities = new();
        
    public IEnumerable<ValidationResult> Validate() 
        => _retrieveMultipleTester.Verify();

    public IRetrieveAllContext Query(Action<IFullQueryExpressionTester> builder)
    {
        _retrieveMultipleTester.Query(builder);
            
        return this;
    }

    public IRetrieveAllContext Returns(IEnumerable<Entity> entities)
    {
        _returnedEntities.Add(entities.ToList());

        return this;
    }
        
        
    public void Build()
    {
        if (_returnedEntities.Count == 0)
        {
            _returnedEntities.Add([]);
        }
            
        foreach (var entities in _returnedEntities)
        {
            mock
                .Setup(t => t.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Callback<QueryBase>(query =>
                {
                    if (query is QueryExpression queryExpression)
                    {
                        _retrieveMultipleTester.SetQueryExpression(queryExpression);
                    }
                })
                .Returns(new EntityCollection(entities))
                .Verifiable();
        }
    }

    public IEnumerable<ValidationResult> Verify()
        => _retrieveMultipleTester.Verify();
}
