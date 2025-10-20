using System;
using System.Collections.Generic;

namespace XrmFramework.Core.Tests.Comparers;

public class ColumnEqualityComparer : IEqualityComparer<Column>
{
    public bool Equals(Column? x, Column? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.LogicalName == y.LogicalName && x.Name == y.Name && x.Type == y.Type && x.PrimaryType == y.PrimaryType && x.Capabilities == y.Capabilities && Equals(x.Labels, y.Labels) && x.StringLength == y.StringLength && Nullable.Equals(x.MinRange, y.MinRange) && Nullable.Equals(x.MaxRange, y.MaxRange) && x.DateTimeBehavior == y.DateTimeBehavior && x.IsMultiSelect == y.IsMultiSelect && x.EnumName == y.EnumName && x.Selected == y.Selected;
    }

    public int GetHashCode(Column obj)
    {
        unchecked
        {
            var hashCode = (obj.LogicalName != null ? obj.LogicalName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int)obj.Type;
            hashCode = (hashCode * 397) ^ (int)obj.PrimaryType;
            hashCode = (hashCode * 397) ^ (int)obj.Capabilities;
            hashCode = (hashCode * 397) ^ (obj.Labels != null ? obj.Labels.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ obj.StringLength.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.MinRange.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.MaxRange.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.DateTimeBehavior.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.IsMultiSelect.GetHashCode();
            hashCode = (hashCode * 397) ^ (obj.EnumName != null ? obj.EnumName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ obj.Selected.GetHashCode();
            return hashCode;
        }
    }
}
