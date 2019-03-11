using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
{
    class DefinitionComparer<T> : IComparer<T>, IEqualityComparer<T> where T : AbstractDefinition, new()
    {
        public string PropertyName { get; set; }

        public bool AscendingOrder { get; set; }

        public DefinitionComparer()
            : this("LogicalName", true)
        {
        }

        public DefinitionComparer(string propertyName, bool ascendingOrder = true)
        {
            PropertyName = propertyName;
            AscendingOrder = ascendingOrder;
        }

        public int Compare(T x, T y)
        {
            if (x == null || y == null)
            {
                return -1;
            }

            var type = typeof(T);
            var property = type.GetProperty(PropertyName);

            var valueX = property.GetValue(x) as string;
            var valueY = property.GetValue(y) as string;

            if (string.IsNullOrEmpty(valueX) && string.IsNullOrEmpty(valueY))
            {
                return 0;
            }
            else if (string.IsNullOrEmpty(valueX))
            {
                return AscendingOrder ? 1 : -1;
            }
            else if (string.IsNullOrEmpty(valueY))
            {
                return AscendingOrder ? -1 : 1;
            }

            return AscendingOrder ? valueX.CompareTo(valueY) : valueY.CompareTo(valueX);
        }

        public bool Equals(T x, T y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(T obj)
        {
            var property = typeof(T).GetProperty(PropertyName);

            return property.GetValue(obj).GetHashCode();
        }
    }
}
