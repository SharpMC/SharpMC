namespace SharpMC.Network.Chunky.Utils
{
    public static class JavaUtil
    {
        public static string ToArrayString<T>(this T[] data)
        {
            return data == null ? string.Empty : $"[{string.Join(", ", data)}]";
        }

        public static bool ArrayEquals<T>(this T[] a, T[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;
            for (var i = 0; i < a.Length; i++)
                if (!a[i].Equals(b[i]))
                    return false;
            return true;
        }
    }
}