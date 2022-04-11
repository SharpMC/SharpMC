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
            return value[0] + value[1];
        }
    }
}