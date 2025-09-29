using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk;
using Moq;

namespace XrmFramework.UnitTests;

public class MockOrganizationService : IMockOrganizationService
{
    private readonly Mock<IOrganizationService> _mock = new();
    
    private readonly List<IVerifiable> _verifiables = new();
    
    public IMockOrganizationService RetrieveAll(Action<IRetrieveAllChecker> builder)
    {
        var checker = new RetrieveAllTester(_mock);
        
        _verifiables.Add(checker);
        
        builder.Invoke(checker);
        
        checker.Build();

        return this;
    }

    public IMockOrganizationService RetrieveMultiple(Action<IRetrieveAllChecker> builder)
    {
        var checker = new RetrieveAllTester(_mock);
        
        _verifiables.Add(checker);
        
        builder.Invoke(checker);
        
        checker.Build();

        return this;
    }
    
    public IMockOrganizationService Retrieve(Action<IRetrieveBuilder> builder)
    {
        var checker = new RetrieveTester(_mock);
        
        _verifiables.Add(checker);
        
        builder.Invoke(checker);
        
        checker.Build();

        return this;
    }

    public IOrganizationService Object => _mock.Object;

    public void Verify()
    {
        _mock.Verify();
        
        foreach (var verifiable in _verifiables)
        {
            var results = verifiable.Verify();

            var errorMessages = results.Select(r => r.ErrorMessage).ToList();
            if (errorMessages.Any())
            {
                throw new ValidationException($"Verification failed: {string.Join(", ", errorMessages)}");
            }
        }
    }
}

internal interface IVerifiable
{
    IEnumerable<ValidationResult> Verify();
}
