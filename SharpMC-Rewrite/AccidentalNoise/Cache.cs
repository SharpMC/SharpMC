using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public struct SCache
    {
        public double X, Y, Val;
        public bool Valid;
    }

    public class Cache : ModuleBase
    {
        SCache cache;

        public Cache(ModuleBase source)
        {
            this.Source = source;
        }

        public override double Get(double x, double y)
        {
            if (!cache.Valid || cache.X != x || cache.Y != y)
            {
                cache.X = x;
                cache.Y = y;
                cache.Valid = true;
                cache.Val = Source.Get(x, y);
            }

            return cache.Val;
        }
    }
}
