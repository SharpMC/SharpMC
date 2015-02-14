using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public class ScaleDomain : ModuleBase
    {
        double m_sx = 1;
        double m_sy = 1;

        public ScaleDomain(ModuleBase source, double? x, double? y)
        {
            if (x != null)
                m_sx = (double)x;

            if (y != null)
                m_sy = (double)y;

            this.Source = source;
        }

        public override double Get(double x, double y)
        {
            return Source.Get(x * m_sx, y * m_sy);
        }
    }
}
