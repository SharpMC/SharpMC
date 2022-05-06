using System;

namespace SharpMC.Network.Util
{
    public static class BinaryTool
    {
        public static int[] ToIntArray(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            var first = BitConverter.ToInt32(bytes, 4);
            var second = BitConverter.ToInt32(bytes, 0);
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