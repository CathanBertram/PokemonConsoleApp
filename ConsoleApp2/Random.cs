using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public static class Random
    {
        public static System.Random random;
        public static int Next(int min, int max)
        {
            if (random == null)
                random = new System.Random();
            return random.Next(min, max);
        }
        public static double Next(double min, double max)
        {
            if (random == null)
                random = new System.Random();
            return (double)random.Next((int)(min * 1000000), (int)(max * 1000000)) / 1000000;
        }
        public static void Shuffle<T>(T[] array)
        {
            random.Shuffle(array);
        }
        public static void Shuffle<T>(this System.Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
