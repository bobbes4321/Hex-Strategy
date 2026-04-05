using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace _Strategy.Runtime.Util
{
    public static class ListExtensions
    {
        /// <summary>
        /// Gets a specified number of randomly selected items from the list without duplicates.
        /// If the requested count exceeds the list size, returns all items in random order.
        /// </summary>
        /// <typeparam name="T">The type of items in the list</typeparam>
        /// <param name="source">The source list to select from</param>
        /// <param name="count">The number of items to select</param>
        /// <returns>A list containing the randomly selected items</returns>
        public static List<T> GetRandomItems<T>(this List<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (count <= 0)
                return new List<T>();

            // If we're asking for more items than exist, return all items shuffled
            if (count >= source.Count)
                return source.OrderBy(x => Random.value).ToList();

            // Create a copy to avoid modifying the original list
            var availableItems = new List<T>(source);
            var selectedItems = new List<T>();

            // Select random items without replacement
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, availableItems.Count);
                selectedItems.Add(availableItems[randomIndex]);
                availableItems.RemoveAt(randomIndex);
            }

            return selectedItems;
        }

        /// <summary>
        /// Alternative implementation using Fisher-Yates shuffle for better performance with large lists
        /// </summary>
        /// <typeparam name="T">The type of items in the list</typeparam>
        /// <param name="source">The source list to select from</param>
        /// <param name="count">The number of items to select</param>
        /// <returns>A list containing the randomly selected items</returns>
        public static List<T> GetRandomItemsFast<T>(this List<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (count <= 0)
                return new List<T>();

            // If we're asking for more items than exist, return all items shuffled
            if (count >= source.Count)
            {
                var allItems = new List<T>(source);
                Shuffle(allItems);
                return allItems;
            }

            // Create a copy and shuffle only what we need
            var workingList = new List<T>(source);
            var result = new List<T>();

            // Partial Fisher-Yates shuffle - only shuffle the first 'count' elements
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(i, workingList.Count);
                result.Add(workingList[randomIndex]);
                
                // Swap the selected item to the front
                (workingList[i], workingList[randomIndex]) = (workingList[randomIndex], workingList[i]);
            }

            return result;
        }

        /// <summary>
        /// Shuffles a list in place using Fisher-Yates algorithm
        /// </summary>
        /// <typeparam name="T">The type of items in the list</typeparam>
        /// <param name="list">The list to shuffle</param>
        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}