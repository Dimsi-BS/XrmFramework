using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

public interface IQueryExpressionChecker 
{
    IEnumerable<ValidationResult> Validate(QueryExpression expression);
}
