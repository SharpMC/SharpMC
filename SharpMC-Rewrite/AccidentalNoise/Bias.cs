using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public class Bias : ModuleBase
    {
        private double m_bias;

        public Bias(ModuleBase source, double? bias)
        {
            this.Source = source;

            if (bias != null)
                m_bias = (double)bias;
        }

        public override double Get(double x, double y)
        {
            double va = Source.Get(x, y);
            return Helper.Bias(m_bias, va);
        }
    }
}
