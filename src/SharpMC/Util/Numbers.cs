﻿using System.Linq;

namespace SharpMC.Util
{
    public static class Numbers
    {
        public static long[] GetLongs(int count, long num, params long[] suffix)
            => Enumerable.Range(1, count)
                .Select(_ => num)
                .Concat(suffix)
                .ToArray();
    }
}