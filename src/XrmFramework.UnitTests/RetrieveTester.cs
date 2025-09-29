using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Moq;

namespace XrmFramework.UnitTests;

internal class RetrieveTester(Mock<IOrganizationService> mock) : IRetrieveBuilder, IVerifiable
{
    private bool _isRetrieveRequest;
    private EntityReference _target = new();
    private ColumnSet _columnSet = new();
    
    private bool _wasCalled;
    private bool _wasRequestCall;
    private EntityReference? _capturedTarget;
    private ColumnSet? _capturedColumnSet;

    
    public IRetrieveBuilder EntityName(string entityName)
    {
        _target.Name = entityName;
        return this;
    }

    public IRetrieveBuilder Id(Guid id)
    {
        _target.Id = id;
        return this;
    }

    public IRetrieveBuilder AsRetrieveRequest()
    {
        _isRetrieveRequest = true;
        return this;
    }

    public IRetrieveBuilder Columns(Action<IColumnSetBuilder> builder)
    {
        var columnSetBuilder = new ColumnSetBuilder();
        builder(columnSetBuilder);

        _columnSet = columnSetBuilder.Build();

        return this;
    }

    public IRetrieveBuilder EntityReference(EntityReference entityReference)
    {        
        _target = entityReference;
        return this;
    }

    public void Build()
    {
        if (_isRetrieveRequest)
        {
            mock
                .Setup(s => s.Execute(It.IsAny<RetrieveRequest>()))
                .Callback<OrganizationRequest>(request =>
                {
                    if (request is RetrieveRequest retrieveRequest)
                    {
                        _wasCalled = true;
                        _wasRequestCall = true;
                        _capturedTarget = retrieveRequest.Target;
                        _capturedColumnSet = retrieveRequest.ColumnSet;
                    }
                })
                .Returns<OrganizationRequest>(request =>
                {
                    if (request is RetrieveRequest rr)
                    {
                        return new RetrieveResponse
                        {
                            Results =
                            {
                                ["Entity"] = new Entity(rr.Target.LogicalName)
                                {
                                    Id = rr.Target.Id
                                }
                            }
                        };
                    }

                    return new OrganizationResponse();
                })
                .Verifiable();
        }
        else
        {
            mock
                .Setup(s => s.Retrieve(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ColumnSet>()))
                .Callback<string, Guid, ColumnSet>((entityName, id, columns) =>
                {
                    _wasCalled = true;
                    _wasRequestCall = false;
                    _capturedTarget = new EntityReference(entityName, id);
                    _capturedColumnSet = columns;
                })
                .Returns<string, Guid, ColumnSet>((entityName, id, _) =>
                    new Entity(entityName) { Id = id })
                .Verifiable();
        }
    }
    
    public IEnumerable<ValidationResult> Verify()
    {
        var results = new List<ValidationResult>();

        if (!_wasCalled)
        {
            results.Add(new ValidationResult("No Retrieve call was captured.", [nameof(Build)]));
            return results;
        }

        if (_isRetrieveRequest && !_wasRequestCall)
        {
            results.Add(new ValidationResult("Expected a RetrieveRequest via Execute, but a direct Retrieve call was made.", [nameof(AsRetrieveRequest)]));
        }
        else if (!_isRetrieveRequest && _wasRequestCall)
        {
            results.Add(new ValidationResult("Expected a direct Retrieve call, but a RetrieveRequest via Execute was made.", [nameof(AsRetrieveRequest)]));
        }

        // Validate target
        if (!string.Equals(_target.LogicalName, _capturedTarget?.LogicalName, StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ValidationResult($"EntityName mismatch. Expected '{_target.LogicalName}', got '{_capturedTarget?.LogicalName}'.", [nameof(EntityName)]));
        }

        if (_target.Id != Guid.Empty && _capturedTarget != null && _target.Id != _capturedTarget.Id)
        {
            results.Add(new ValidationResult($"Id mismatch. Expected '{_target.Id}', got '{_capturedTarget.Id}'.", [nameof(Id)]));
        }

        // Validate ColumnSet
        if (_capturedColumnSet == null)
        {
            results.Add(new ValidationResult("No ColumnSet was provided in the call.", [nameof(Columns)]));
            return results;
        }

        if (_columnSet.AllColumns != _capturedColumnSet.AllColumns)
        {
            var message = _columnSet.AllColumns
                ? "The query should be AllColumns."
                : "The query should not be AllColumns.";
            results.Add(new ValidationResult(message, [nameof(Columns)]));
        }

        if (!_columnSet.AllColumns)
        {
            var expected = _columnSet.Columns?.ToArray() ?? Array.Empty<string>();
            var actual = _capturedColumnSet.Columns?.ToArray() ?? Array.Empty<string>();

            // ensure all expected columns are present
            var missing = expected.Except(actual, StringComparer.OrdinalIgnoreCase).ToList();
            if (missing.Count > 0)
            {
                results.Add(new ValidationResult($"The following columns are missing: {string.Join(", ", missing)}", [nameof(Columns)]));
            }
        }

        return results;
    }

}
