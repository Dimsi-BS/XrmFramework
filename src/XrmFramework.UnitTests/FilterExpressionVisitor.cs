using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

internal sealed class FilterExpressionVisitor(List<ConditionExpression> atLeastOneConditions)
{
    private readonly List<ConditionExpression> _validatedConditions = [];
    
    public void Visit(FilterExpression filterExpression)
    {
        filterExpression.Conditions.Join(atLeastOneConditions,
                condition => condition.AttributeName,
                atLeastOneCondition => atLeastOneCondition.AttributeName,
                (condition, atLeastOneCondition) =>
                {
                    if (condition.Operator == atLeastOneCondition.Operator
                        && condition.Values.SequenceEqual(atLeastOneCondition.Values))
                    {
                        
                        return new {IsValidated = true, condition, atLeastOneCondition};
                    }
                    return new {IsValidated = false, condition, atLeastOneCondition};
                })
            .Where(v => v.IsValidated)
            .ToList()
            .ForEach(v => _validatedConditions.Add(v.atLeastOneCondition));

        foreach (var subFilter in filterExpression.Filters)
        {
            Visit(subFilter);
        }
    }


    public IEnumerable<ValidationResult> ValidationResults
        => atLeastOneConditions
            .Except(_validatedConditions)
            .Select(c => new ValidationResult($"Condition {c.AttributeName} with operator {c.Operator} and values {string.Join(", ", c.Values)} is not validated."));
}
