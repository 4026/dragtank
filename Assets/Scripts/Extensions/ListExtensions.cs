using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragTank.Extensions
{
    static class ListExtensions
    {
        private static Random rng = new Random();

        /// <summary>
        /// Randomize the order of a List.
        /// Fisher-Yates shuffle implementation stolen from http://stackoverflow.com/a/1262619.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
