using System;

namespace Plus.Utilities
{
    public static class Randomizer
    {
        public static Random GetRandom { get; } = new Random();

        public static int Next()
        {
            return GetRandom.Next();
        }

        public static int Next(int max)
        {
            return GetRandom.Next(max);
        }

        public static int Next(int min, int max)
        {
            return GetRandom.Next(min, max);
        }

        public static double NextDouble()
        {
            return GetRandom.NextDouble();
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
            GetRandom.NextBytes(toparse);
        }
    }
}
