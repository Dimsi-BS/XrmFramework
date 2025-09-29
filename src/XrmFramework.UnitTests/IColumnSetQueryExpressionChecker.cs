using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

internal interface IColumnSetQueryExpressionChecker
{
    IEnumerable<ValidationResult> Validate(ColumnSet columnSet);
}

internal interface ILinkQueryExpressionChecker
{
    IEnumerable<ValidationResult> Validate(LinkEntity link);
}
