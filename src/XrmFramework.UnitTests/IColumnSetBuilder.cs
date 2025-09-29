using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

public interface IColumnSetBuilder
{
    /// <summary>
    /// Specifies the columns to be retrieved in the QueryExpression.
    /// </summary>
    /// <param name="columns">The columns to retrieve.</param>
    /// <returns>An instance of IColumnSetQueryExpressionTester for further configuration.</returns>
    IColumnSetBuilder WithColumns(params string[] columns);
    
    /// <summary>
    /// Specifies the columns to be retrieved in the QueryExpression.
    /// </summary>
    /// <param name="columnSet">The ColumnSet to retrieve.</param>
    /// <returns>An instance of IColumnSetQueryExpressionTester for further configuration.</returns>
    IColumnSetBuilder WithColumns(ColumnSet columnSet);
    
    /// <summary>
    /// Specifies whether all columns will be retrieved in the QueryExpression.
    /// </summary>
    /// <param name="isAllColumns"></param>
    /// <returns>An instance of IColumnSetQueryExpressionTester for further configuration.</returns>
    IColumnSetBuilder IsAllColumns(bool isAllColumns = true);
}
