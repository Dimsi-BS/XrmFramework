using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

internal sealed class QueryExpressionTester : IFullQueryExpressionTester, IQueryExpressionChecker
{
    private string? _entityName;
    
    private readonly ColumnSetBuilder _columnSetBuilder = new();
    private readonly FilterQueryExpressionTester _filterQueryExpressionTester = new(LogicalOperator.And);
    private readonly List<LinkQueryExpressionTester> _links = new();
    private bool _noLock;

    public IQueryExpressionTester Columns(Action<IColumnSetBuilder> columnsTester)
    {
        columnsTester.Invoke(_columnSetBuilder);
        return this;
    }

    public IQueryExpressionTester Columns(params string[] columns)
    {
        _columnSetBuilder.WithColumns(columns);
        return this;
    }

    public IQueryExpressionTester Criteria(Action<IFilterQueryExpressionTester> filterTester)
    {
        filterTester.Invoke(_filterQueryExpressionTester);
        return this;
    }

    public IQueryExpressionTester Link(Action<ILinkQueryExpressionTester> linkTester)
    {
        var link = new LinkQueryExpressionTester();
        linkTester.Invoke(link);
        
        _links.Add(link);
        return this;
    }
    
    public IEnumerable<ValidationResult> Validate(QueryExpression expression)
    {
        var results = _columnSetBuilder.Validate(expression.ColumnSet)
            .Union(
                _filterQueryExpressionTester.Validate(expression.Criteria)
            ).ToList();
        
        if (_entityName != null && expression.EntityName != _entityName)
        {
            results.Add(new ValidationResult(
                $"The query should be for entity '{_entityName}'.",
                [nameof(expression.EntityName)]));
        }
        
        if (_noLock && !expression.NoLock)
        {
            results.Add(new ValidationResult(
                "The query should be NoLock.",
                [nameof(expression.NoLock)]));
        }
        
        if (_links.Count != expression.LinkEntities.Count)
        {
            results.Add(new ValidationResult(
                $"The query should have {_links.Count} link(s).",
                [nameof(expression.LinkEntities)]));
        }
        else
        {
            foreach (var link in _links)
            {
                var linkEntity = expression.LinkEntities.FirstOrDefault(l => l.LinkToEntityName == link.EntityName);

                results.AddRange(link.Validate(linkEntity));
            }
        }
        
        return results;
    }

    public IFullQueryExpressionTester EntityName(string? entityName)
    {
        _entityName = entityName;
        return this;
    }

    public IFullQueryExpressionTester NoLock()
    {
        _noLock = true;
        return this;
    }
}
