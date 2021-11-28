using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Utilities
{
    public static class GenericUtilities
    {
        private static Random commonRandom = new Random();

        public static T[] Flatten2DArray<T>(T[,] array)
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
