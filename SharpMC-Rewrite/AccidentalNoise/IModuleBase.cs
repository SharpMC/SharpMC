using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public abstract class ModuleBase
    {
        public int seed;
        public ModuleBase Source;
        public static readonly int MaxSources = 20;

        public abstract double Get(double x, double y);
    }

    public static class Helper
    {
        public static double Clamp(double v, double l, double h)
        {
            if(v < l) v = l;
            if(v > h) v = h;

            return v;
        }

        public static double Quintic_Blend(double f)
        {
            return f*f*f*(f*(f*6-15) + 10);
        }

        public static double Lerp(double f, double a, double b)
        {
            return a + f * (b - a);
        }

        public static double Bias(double b, double t)
        {
            return Math.Pow(t, Math.Log(b) / Math.Log(0.5));
        }
    }
}
