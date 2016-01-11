using System;
using System.Collections.Generic;
using DeviceMotion.Plugin.Abstractions;
using System.Linq;

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

        public static MotionVector CalculateAverage(this List<MotionVector> list)
        {
            return new MotionVector { 
                Value = list.Average(x => x.Value),
                X = list.Average(x => x.X),
                Y = list.Average(x => x.Y),
                Z = list.Average(x => x.Z)
            };
        }
    }
}

