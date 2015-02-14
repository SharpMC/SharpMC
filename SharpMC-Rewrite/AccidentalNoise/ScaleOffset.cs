using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public class ScaleOffset : ModuleBase
    {
        double scale, offset;

        public ScaleOffset(double scale, double offset, ModuleBase mbase)
        {
            this.scale = scale;
            this.offset = offset;
            this.Source = mbase;
        }

        public override double Get(double x, double y)
        {
            return Source.Get(x, y) * scale + offset;
        }
    }
}
