using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture_Model.Utilities
{
    public static class GenericUtilities
    {
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
    }
}
