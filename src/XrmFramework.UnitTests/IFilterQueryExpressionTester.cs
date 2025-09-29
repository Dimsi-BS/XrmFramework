using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

public interface IFilterQueryExpressionTester
{
    IFilterQueryExpressionTester WithOperator(LogicalOperator logicalOperator);
    
    IFilterQueryExpressionTester AtLeastOneCondition(string columnName, ConditionOperator op, params object[] value);

    IFilterQueryExpressionTester And();
    
    IFilterQueryExpressionTester Or();
}
