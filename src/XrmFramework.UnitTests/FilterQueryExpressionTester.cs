using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

internal class FilterQueryExpressionTester(LogicalOperator logicalOperator)
    : IFilterQueryExpressionTester, IFilterQueryExpressionChecker
{
    private List<ConditionExpression> _atLeastOneConditions = new();

    public IFilterQueryExpressionTester WithOperator(LogicalOperator op)
    {
        logicalOperator = op;

        return this;
    }

    public IFilterQueryExpressionTester AtLeastOneCondition(string columnName, ConditionOperator op,
        params object[] value)
    {
        _atLeastOneConditions.Add(new ConditionExpression(columnName, op, value));
        
        return this;
    }

    public IFilterQueryExpressionTester And()
        => new FilterQueryExpressionTester(LogicalOperator.And);

    public IFilterQueryExpressionTester Or()
        => new FilterQueryExpressionTester(LogicalOperator.Or);

    
    public IEnumerable<ValidationResult> Validate(FilterExpression? filter)
    {
        if (filter == null)
        {
            yield return new ValidationResult("FilterExpression cannot be null.");
            yield break;
        }
        
        if (filter.FilterOperator != logicalOperator)
        {
            yield return new ValidationResult(
                $"FilterExpression operator '{filter.FilterOperator}' does not match expected operator '{logicalOperator}'.");
        }
        
        // use _atLeastOneConditions to validate the filter
        if (_atLeastOneConditions.Count > 0)
        {
            var visitor = new FilterExpressionVisitor(_atLeastOneConditions);
            visitor.Visit(filter);

            foreach (var validationResult in visitor.ValidationResults)
            {
                yield return validationResult;
            }
        }

        foreach (var subFilter in filter.Filters)
        {
            foreach (var validationResult in Validate(subFilter))
            {
                yield return validationResult;
            }
        }
    }
}
