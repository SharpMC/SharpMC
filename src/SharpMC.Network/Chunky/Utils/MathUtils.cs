using System.Numerics;

namespace SharpMC.Network.Chunky.Utils
{
    public static class MathUtils
    {
        public static long ChunkPositionToLong(int x, int z)
        {
            return ((x & 0xFFFFFFFFL) << 32) | (z & 0xFFFFFFFFL);
        }

        public static int GetGlobalPaletteForSize(int size)
        {
            return 32 - BitOperations.LeadingZeroCount((uint) (size - 1));
        }
    }
}