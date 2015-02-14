using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public enum CombinerTypes
    {
        ADD,
        MULT,
        MAX,
        MIN,
        AVG
    }

    public class Combiner : ModuleBase
    {
        CombinerTypes Type;
        ModuleBase Source2;

        public Combiner(CombinerTypes type, ModuleBase source1, ModuleBase source2)
        {
            this.Type = type;
            this.Source = source1;
            this.Source2 = source2;
        }

        public override double Get(double x, double y)
        {
            switch (Type)
            {
                case CombinerTypes.ADD: return Add_Get(x, y);
                case CombinerTypes.MULT: return Mult_Get(x, y);
                case CombinerTypes.MAX: return Max_Get(x, y);
                case CombinerTypes.MIN: return Min_Get(x, y);
                case CombinerTypes.AVG: return Avg_Get(x, y);
                default: throw new Exception();
            }
        }

        private double Add_Get(double x, double y)
        {
            return Source.Get(x, y) + Source2.Get(x, y);
        }

        private double Mult_Get(double x, double y)
        {
            double value = 1;
            value *= Source.Get(x, y);
            value *= Source2.Get(x, y);

            return value;
        }

        private double Max_Get(double x, double y)
        {
            double s1 = Source.Get(x, y);
            double s2 = Source2.Get(x, y);

            if (s1 > s2)
                return s1;
            else
                return s2;
        }

        private double Min_Get(double x, double y)
        {
            double s1 = Source.Get(x, y);
            double s2 = Source2.Get(x, y);

            if (s1 < s2)
                return s1;
            else
                return s2;
        }

        private double Avg_Get(double x, double y)
        {
            double val = Source.Get(x, y) + Source2.Get(x, y);
            return val / 2.0f;
        }
    }
}
