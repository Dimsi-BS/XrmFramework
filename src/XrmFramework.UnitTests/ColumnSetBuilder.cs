using System.ComponentModel.DataAnnotations;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.UnitTests;

internal sealed class ColumnSetBuilder 
    : IColumnSetBuilder
{
    private readonly List<string> _columns = [];
    
    private bool _isAllColumns;
    
    public IColumnSetBuilder WithColumns(params string[] columns)
    {
        _columns.AddRange(columns);

        return this;
    }

    public IColumnSetBuilder WithColumns(ColumnSet columnSet)
    {
        columnSet = columnSet ?? throw new ArgumentNullException(nameof(columnSet));
        if (columnSet.AllColumns)
        {
            _isAllColumns = true;
        }
        else
        {
            _columns.AddRange(columnSet.Columns);
        }
        
        return this;
    }

    public IColumnSetBuilder IsAllColumns(bool isAllColumns = true)
    {
        _isAllColumns = isAllColumns;
        
        return this;
    }

    public ColumnSet Build()
    {
        var columnSet = new ColumnSet(_isAllColumns);
        
        if (!_isAllColumns)
        {
            columnSet.AddColumns(_columns.ToArray());
        }
        
        return columnSet;
    }

    public IEnumerable<ValidationResult> Validate(ColumnSet columnSet)
    {
        if (columnSet.AllColumns && !_isAllColumns
            || !columnSet.AllColumns && _isAllColumns)
        {
            var message = _isAllColumns
                ? "The query should be AllColumns."
                : "The query should not be AllColumns.";
            
            yield return new ValidationResult(
                message,
                [nameof(columnSet)]);
        }
        
        // Create a code that checks if the columns in the columnSet match the expected columns
        if (_columns.Count > 0 && !columnSet.AllColumns)
        {
            var missingColumns = _columns.Except(columnSet.Columns).ToList();

            if (missingColumns.Any())
            {
                yield return new ValidationResult(
                    $"The following columns are missing: {string.Join(", ", missingColumns)}",
                    [nameof(columnSet)]);
            }
        }
    }
}
