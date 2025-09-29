using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

internal class LinkQueryExpressionTester : ILinkQueryExpressionTester, ILinkQueryExpressionChecker
{
    private readonly Lazy<ColumnSetBuilder> _columnSetQueryExpressionTester;
    private readonly Lazy<FilterQueryExpressionTester> _filterQueryExpressionTester;
    private readonly List<LinkQueryExpressionTester> _links = [];

    private ColumnSetBuilder ColumnSetBuilder => _columnSetQueryExpressionTester.Value;
    private FilterQueryExpressionTester FilterTester => _filterQueryExpressionTester.Value;
    
    private string _toEntityName;
    private string _toColumnName;
    private string _fromColumnName;
    private string _entityAlias;
    
    public string EntityName => _toEntityName;
    public string FromColumnName => _fromColumnName;
    public string ToColumnName => _toColumnName;
    public string EntityAlias => _entityAlias;
    
    public LinkQueryExpressionTester()
    {
        _columnSetQueryExpressionTester =
            new Lazy<ColumnSetBuilder>(
                () => new ColumnSetBuilder());
        
        _filterQueryExpressionTester =
            new Lazy<FilterQueryExpressionTester>(
                () => new FilterQueryExpressionTester(LogicalOperator.And));
    }

    public IQueryExpressionTester Columns(Action<IColumnSetBuilder> columnsSetter)
    {
        columnsSetter.Invoke(ColumnSetBuilder);
        return this;
    }

    public IQueryExpressionTester Columns(params string[] columns)
    {
        ColumnSetBuilder.WithColumns(columns);
        return this;
    }

    public IQueryExpressionTester Criteria(Action<IFilterQueryExpressionTester> filtersSetter)
    {
        filtersSetter.Invoke(FilterTester);
        return this;
    }

    public ILinkQueryExpressionTester ToEntityName(string entityName)
    {
        _toEntityName = entityName;
        return this;
    }

    public ILinkQueryExpressionTester From(string columnName)
    {
        _fromColumnName = columnName;
        return this;
    }

    public ILinkQueryExpressionTester To(string columnName)
    {
        _toColumnName = columnName;
        return this;
    }

    public ILinkQueryExpressionTester Alias(string alias)
    {
        _entityAlias = alias;
        return this;
    }
    
    public IQueryExpressionTester Link(Action<ILinkQueryExpressionTester> linkTester)
    {
        var link = new LinkQueryExpressionTester();
        linkTester.Invoke(link);
        _links.Add(link);
        return link;
    }

    public IEnumerable<ValidationResult> Validate(LinkEntity? link)
    {
        if (link is null)
        {
            yield return new ValidationResult(
                "The link has not been found in the query.\n",
                [nameof(link)]);
            yield break;
        }
        
        if (link.LinkToEntityName != _toEntityName)
        {
            yield return new ValidationResult(
                $"The link to entity name should be {_toEntityName}",
                [nameof(link.LinkFromEntityName)]);
        }

        if (link.LinkFromAttributeName != _fromColumnName)
        {
            yield return new ValidationResult(
                $"The link from column name should be {_fromColumnName}",
                [nameof(link.LinkFromAttributeName)]);
        }

        if (link.LinkToAttributeName != _toColumnName)
        {
            yield return new ValidationResult(
                $"The link to column name should be {_toColumnName}",
                [nameof(link.LinkToAttributeName)]);
        }

        if (link.Columns is not null)
        {
            foreach (var validationResult in ColumnSetBuilder.Validate(link.Columns))
            {
                yield return validationResult;
            }
        }

        foreach (var validationResult in FilterTester.Validate(link.LinkCriteria))
        {
            yield return validationResult;
        }

        foreach (var subLink in _links)
        {
            foreach (var validationResult in subLink.Validate(link))
            {
                yield return validationResult;
            }
        }
    }
}
