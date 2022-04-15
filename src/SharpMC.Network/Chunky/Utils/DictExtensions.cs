using System.Collections.Generic;

namespace SharpMC.Network.Chunky.Utils
{
    public static class DictExtensions
    {
        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> dict, TK key)
        {
            return dict.TryGetValue(key, out var value) ? value : default;
        }

        public static void PutIfAbsent<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV value)
        {
            if (dict.ContainsKey(key))
                return;
            dict[key] = value;
        }
    }
}