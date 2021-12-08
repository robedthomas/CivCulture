using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUtilities
{
    public static class ExtensionMethods
    {
        private static Random commonRandom = new Random();

        public static T[] Flatten2DArray<T>(this T[,] array)
        {
            if (array == null)
            {
                return null;
            }
            int topLength = array.GetLength(0);
            int bottomLength = array.GetLength(1);
            T[] result = new T[topLength * bottomLength];
            for (int i = 0; i < topLength; i++)
            {
                for (int j = 0; j < bottomLength; j++)
                {
                    result[j + (i * bottomLength)] = array[i, j];
                }
            }
            return result;
        }

        public static T PickRandom<T>(this IEnumerable<T> collection, Random random)
        {
            return collection.ElementAt(random.Next(0, collection.Count()));
        }

        public static T PickRandom<T>(this IEnumerable<T> collection)
        {
            return collection.PickRandom(commonRandom);
        }

        public static T PickRandomWithWeight<T>(this IEnumerable<Tuple<T, int>> collectionWithWeights, Random random)
        {
            if (collectionWithWeights.Count() == 0)
            {
                throw new ArgumentException("Received empty collection");
            }
            int totalWeight = 0;
            foreach (Tuple<T, int> itemWithWeight in collectionWithWeights)
            {
                if (itemWithWeight.Item2 < 0)
                {
                    throw new ArgumentOutOfRangeException($"Item weight must be non-negative, got {itemWithWeight.Item2}");
                }
                totalWeight += itemWithWeight.Item2;
            }
            int weightSelected = random.Next(0, totalWeight);
            int currentWeight = 0;
            foreach (Tuple<T, int> itemWithWeight in collectionWithWeights)
            {
                if (weightSelected >= currentWeight && weightSelected < currentWeight + itemWithWeight.Item2)
                {
                    return itemWithWeight.Item1;
                }
                currentWeight += itemWithWeight.Item2;
            }
            throw new InvalidOperationException("Iterated through list without finding item to randomly select. This should not be possible!");
        }

        public static bool TryFindIndex<T>(this T[,] array, T targetItem, out int firstIndex, out int secondIndex)
        {
            firstIndex = -1;
            secondIndex = -1;
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                {
                    if (array[i, j].Equals(targetItem))
                    {
                        firstIndex = i;
                        secondIndex = j;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
