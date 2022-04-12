using System;
using System.Linq;

namespace SharpMC.Network.Util
{
    public static class BinaryTool
    {
        public static int[] ToIntArray(long hashedSeed)
        {
            var first = (int) hashedSeed;
            var second = (int) hashedSeed;
            return new[] {first, second};
        }

        public static long ToLong(int[] value)
        {
            var first = (uint) value[0];
            var second = (uint) value[1];
            var unsignedKey = ((ulong) first << 32) | second;
            return (long) unsignedKey;
        }
    }
}