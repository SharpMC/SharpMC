namespace SharpMC.Network.Chunky.Utils
{
    public static class DebugUtil
    {
        public static string ToDebugString<T>(this T[] data)
        {
            return $"[{string.Join(", ", data)}]";
        }
    }
}