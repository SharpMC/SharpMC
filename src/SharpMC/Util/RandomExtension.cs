using System;

namespace SharpMC.Util
{
    public static class RandomExtension
    {
        public static long NextLong(this Random rnd, long min = long.MinValue, long max = long.MaxValue)
        {
            return min + (long) NextULong(rnd, 0, (ulong) Math.Abs(max - min));
        }

        public static ulong NextULong(this Random rnd, ulong min = ulong.MinValue, ulong max = ulong.MaxValue)
        {
            var hight = rnd.Next((int) (min >> 32), (int) (max >> 32));
            var minLow = Math.Min((int) min, (int) max);
            var maxLow = Math.Max((int) min, (int) max);
            var low = (uint) rnd.Next(minLow, maxLow);
            var result = (ulong) hight;
            result <<= 32;
            result |= low;
            return result;
        }
    }
}