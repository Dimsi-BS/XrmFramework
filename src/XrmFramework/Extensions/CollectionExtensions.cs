using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework
{
    public static class CollectionExtensions
    {
        public static IEnumerable<List<T>> SplitList<T>(this ICollection<T> list, int nSize = 1000)
        {
            var locations = list.ToList();

            for (var i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}
