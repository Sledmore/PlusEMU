using System;

namespace Plus.Utilities
{
    public static class Randomizer
    {
        private static Random rand = new Random();

        public static Random GetRandom
        {
            get
            {
                return rand;
            }
        }

        public static int Next()
        {
            return rand.Next();
        }

        public static int Next(int max)
        {
            return rand.Next(max);
        }

        public static int Next(int min, int max)
        {
            return rand.Next(min, max);
        }

        public static double NextDouble()
        {
            return rand.NextDouble();
        }

        public static byte NextByte()
        {
            return (byte)Next(0, 255);
        }

        public static byte NextByte(int max)
        {
            max = Math.Min(max, 255);
            return (byte)Next(0, max);
        }

        public static byte NextByte(int min, int max)
        {
            max = Math.Min(max, 255);
            return (byte)Next(Math.Min(min, max), max);
        }

        public static void NextBytes(byte[] toparse)
        {
            rand.NextBytes(toparse);
        }
    }
}
