namespace XrmFramework.UnitTests;

public interface IQueryExpressionTester
{
    IQueryExpressionTester  Columns(Action<IColumnSetBuilder> columnsTester);

    IQueryExpressionTester Columns(params string[] columns);

    IQueryExpressionTester Criteria(Action<IFilterQueryExpressionTester> criteriaTester);

    IQueryExpressionTester Link(Action<ILinkQueryExpressionTester> linkTester);
}

public interface IFullQueryExpressionTester : IQueryExpressionTester
{
    IFullQueryExpressionTester EntityName(string? entityName);
    
    
    IFullQueryExpressionTester NoLock();
}
