using System;
using System.Collections.Generic;

namespace BumpDetector
{
    public static class Extensions
    {
        public static void AddRespectingCapacity<T>(this List<T> list, T item, int index = 0)
        {
            if(list.Capacity!=0 && list.Count == list.Capacity)
            {
                list.RemoveAt(list.Count - 1);
            }

            list.Insert(index, item);
        }
    }
}

