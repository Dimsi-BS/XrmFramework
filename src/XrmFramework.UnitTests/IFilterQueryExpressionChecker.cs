using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

internal interface IFilterQueryExpressionChecker
{
    IEnumerable<ValidationResult> Validate(FilterExpression filter);
}
