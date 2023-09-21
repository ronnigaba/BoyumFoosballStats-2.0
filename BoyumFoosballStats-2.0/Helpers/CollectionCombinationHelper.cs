using System;
using System.Collections.Generic;
using System.Linq;

namespace BoyumFoosballStats.Controller
{
    public static class CollectionCombinationHelper
    {
        public static IEnumerable<IEnumerable<T>>
            GetUniqueCombinations<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetUniqueCombinations(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static IEnumerable<IEnumerable<T>> GetAllCombinations<T>(IEnumerable<T> collection, int size)
        {
            if (size == 0)
            {
                yield return Array.Empty<T>();
            }
            else
            {
                foreach (var item in collection)
                {
                    foreach (var combination in GetAllCombinations(collection, size - 1))
                    {
                        var currentCombination = new List<T> { item };
                        currentCombination.AddRange(combination);
                        yield return currentCombination;
                    }
                }
            }
        }
    }
}