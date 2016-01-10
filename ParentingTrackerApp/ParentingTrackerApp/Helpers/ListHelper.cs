using System;
using System.Collections.Generic;

namespace ParentingTrackerApp.Helpers
{
    public static class ListHelper
    {
        public delegate int CompareToTarget<T>(T rhs);

        public delegate int Compare<T>(T lhs, T rhs);

        public static void Insert<T>(this IList<T> list, T newItem) where T : IComparable<T>
        {
            var index = list.BinarySearch(newItem);
            if (index < 0)
            {
                index = -index - 1;
            }
            list.Insert(index, newItem);
        }

        public static int BinarySearch<T>(this IList<T> list, T target) where T : IComparable<T>
        {
            return list.BinarySearch(0, list.Count, (rhs) => target.CompareTo(rhs));
        }

        public static int BinarySearch<T>(this IList<T> list, int start, int count,
            T target) where T : IComparable<T>
        {
            return list.BinarySearch(start, count, (rhs) => target.CompareTo(rhs));
        }

        public static int BinarySearch<T>(this IList<T> list, int start, int count,
            CompareToTarget<T> compare)
        {
            int low = start;
            int high = start + count;
            int mid;

            while (low < high)
            {
                mid = (low + high) / 2;
                T v = list[mid];
                // NOTE v as right hand operator
                int comp = compare(v);
                if (comp < 0)
                {
                    high = mid;
                }
                else if (comp > 0)
                {
                    low = mid + 1;
                }
                else
                {
                    return mid;    // found, returning the position  
                }
            }
            return -(low + 1);    // not found, returning minus the position to insert minus one  
        }

        public static void QuickSort<T>(this IList<T> list) where T : IComparable<T>
        {
            list.QuickSort(0, list.Count);
        }

        public static void QuickSort<T>(this IList<T> list, Compare<T> compare)
        {
            list.QuickSort(0, list.Count, compare);
        }

        public static void QuickSort<T>(this IList<T> list, int start, int count) where T : IComparable<T>
        {
            list.QuickSort(start, count, (a, b) => a.CompareTo(b));
        }

        public static void QuickSort<T>(this IList<T> list, int start, int count, Compare<T> compare)
        {
            var low = start;
            var high = start + count - 1;
            list.QuickSortLowHigh(low, high, compare);
        }

        private static void QuickSortLowHigh<T>(this IList<T> list, int low, int high, Compare<T> compare)
        {
            if (low < high)
            {
                var p = list.LomutoPartition(low, high, compare);
                list.QuickSortLowHigh(low, p - 1, compare);
                list.QuickSortLowHigh(p + 1, high, compare);
            }
        }

        /// <summary>
        ///  Lumuto partition for quick sort
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <param name="list">The list</param>
        /// <param name="low">The lower bound</param>
        /// <param name="high">The higher bound</param>
        /// <param name="compare">The comparer</param>
        /// <returns>The splitting index</returns>
        /// <remarks>
        ///  Reference: 
        ///  https://en.wikipedia.org/wiki/Quicksort
        /// </remarks>
        private static int LomutoPartition<T>(this IList<T> list, int low, int high, Compare<T> compare)
        {
            var pivot = list[high];
            var i = low;        // place for swapping
            for (var j = low; j < high; j++)
            {
                var c = compare(list[j], pivot);
                if (c <= 0)
                {
                    var t = list[i];
                    list[i] = list[j];
                    list[j] = t;
                    i++;
                }
            }
            var tt = list[i];
            list[i] = pivot;
            list[high] = tt;
            return i;
        }
    }
}
