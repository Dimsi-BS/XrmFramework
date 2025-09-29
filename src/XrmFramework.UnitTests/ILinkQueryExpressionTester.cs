namespace XrmFramework.UnitTests;

public interface ILinkQueryExpressionTester : IQueryExpressionTester
{
    ILinkQueryExpressionTester ToEntityName(string entityName);
    
    ILinkQueryExpressionTester From(string columnName);
    
    ILinkQueryExpressionTester To(string columnName);
    
    ILinkQueryExpressionTester Alias(string alias);
}
