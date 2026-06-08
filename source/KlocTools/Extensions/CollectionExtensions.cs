/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Klocman.Extensions
{
    public static class CollectionExtensions
    {
        private static readonly Random R = new();
        
        /// <summary>
        /// Wrap this object in an enumerable that returns this object once and finishes.
        /// </summary>
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            return Enumerable.Repeat(obj, 1);
        }

        /// <summary>
        /// Recursively select all subitems based on the selector.
        /// </summary>
        public static IEnumerable<T> SelectManyResursively<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> subitemSelector)
        {
            return enumerable.SelectMany(
                x => Enumerable.Repeat(x, 1)
                .Concat(subitemSelector(x).SelectManyResursively(subitemSelector)));
        }

        /// <summary>
        /// Move item at specified index to a new index
        /// </summary>
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// Move item at specified index to index + 1. Does nothing if item is already last.
        /// </summary>
        public static void MoveUp<T>(this IList<T> list, int oldIndex)
        {
            if (list.Count - 1 > oldIndex)
                list.Move(oldIndex, oldIndex + 1);
        }

        /// <summary>
        /// Move item at specified index to index - 1. Does nothing if item is already first.
        /// </summary>
        public static void MoveDown<T>(this IList<T> list, int oldIndex)
        {
            if (oldIndex > 0)
                list.Move(oldIndex, oldIndex - 1);
        }

        /// <summary>
        /// Returns a read-only wrapper of this dictionary. 
        /// Attempts to modify this collection will throw InvalidOperationException.
        /// </summary>
        public static IDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> obj)
        {
            return new ReadOnlyDictionaryWrapper<TKey, TValue>(obj);
        }

        /// <summary>
        ///     Usually faster than calling Count() and comparing, at worst it's about the same.
        ///     It enumerates at most (count + 1) elements from the collection.
        /// </summary>
        /// <param name="collection">Collection to count against</param>
        /// <param name="count">Count to compare to.</param>
        public static bool CountEquals<T>(this IEnumerable<T> collection, int count)
        {
            if (collection is ICollection<T> col)
                return col.Count == count;

            using (var enumerator = collection.GetEnumerator())
            {
                var i = 0;
                while (enumerator.MoveNext())
                {
                    i++;
                    if (i > count)
                        return false;
                }
                return i == count;
            }
        }

        /// <summary>
        /// Run distinct using the specified equality comparator
        /// </summary>
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source,
            Func<TSource, TSource, bool> equalityComparator)
        {
            var seenItems = new List<TSource>();
            foreach (var item in source)
            {
                if (seenItems.Any(x => equalityComparator(item, x))) continue;

                seenItems.Add(item);
                yield return item;
            }
        }

        /// <summary>
        ///     Run the specified action on all members of the collection as they are enumerated.
        ///     Action will be executed for each enumeration over the element (lazy evaluation).
        /// </summary>
        /// <typeparam name="T">Type that is being iterated over</typeparam>
        /// <param name="collection">Base enumerable</param>
        /// <param name="action">Action to run on all of the enumerated members</param>
        /// <returns>Enumerator</returns>
        public static IEnumerable<T> DoForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        ///     Run the specified action on all members of the collection. The collection is enumerated in process.
        /// </summary>
        /// <typeparam name="T">Type that is being iterated over</typeparam>
        /// <param name="collection">Base enumerable</param>
        /// <param name="action">Action to run on all of the members</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        /// <summary>
        ///     Get ID of the element. If the element is not in the array, returns -1 (indexof is not available)
        /// </summary>
        public static int GetPositionOfElement<T>(this T[] array, T element)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(array[i], element))
                    return i;
            }
            return -1;
        }

        /// <summary>
        ///     Returns random element from this sequence. If it is empty, returns default value of the type.
        /// </summary>
        public static T GetRandomElement<T>(this IEnumerable<T> items)
        {
            var list = items.ToList();
            switch (list.Count)
            {
                case 0:
                    return default(T);
                case 1:
                    return list[0];
                default:
                    return list[R.Next(0, list.Count)];
            }
        }

        /// <summary>
        ///     Remove all items that are contained in the supplied collection.
        /// </summary>
        /// <param name="collection">Collection to remove items from</param>
        /// <param name="items">Collection with items to remove.</param>
        public static void RemoveAll<T>(this IList<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (collection.Contains(item))
                    collection.Remove(item);
            }
        }

        /// <summary>
        ///     Remove all items for which the predicate returns true.
        /// </summary>
        /// <param name="collection">Collection to remove items from</param>
        /// <param name="predicate">Predicate used to choose items to remove.</param>
        public static void RemoveAll<T>(this IList<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection.Where(predicate).ToArray())
                collection.Remove(item);
        }

        /// <summary>
        ///     Create a new sub array starting at specified index and spanning a number of positions.
        /// </summary>
        /// <param name="data">Source array</param>
        /// <param name="index">Starting index.</param>
        /// <param name="length">How many items to take after the index (including the item at index).</param>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Check if any element of the collection equals to the supplied string value.
        /// </summary>
        public static bool Contains(this IEnumerable<string> data, string value, StringComparison options)
        {
            return data.Any(x => x.Equals(value, options));
        }
        
        /// <summary>
        /// Rotate the collection to the left so that the item at startIndex becomes index 0. 
        /// Elements rotated to the left wrap around, so the number of elements stays the same. 
        /// </summary>
        public static IEnumerable<T> Rotate<T>(this ICollection<T> targets, int startIndex)
        {
            return targets.Skip(startIndex).Concat(targets.Take(startIndex));
        }
    }
}