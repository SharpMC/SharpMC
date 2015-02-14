using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public class AutoCorrect : ModuleBase
    {
        double m_low, m_high;
        double m_scale2, m_offset2;

        public AutoCorrect(ModuleBase source, double low, double high)
        {
            this.Source = source;
            m_low = low;
            m_high = high;

            Calculate();
        }

        public override double Get(double x, double y)
        {
            double v = Source.Get(x, y);

            return Math.Max(m_low, Math.Min(m_high, v * m_scale2 + m_offset2));
        }

        public void Calculate()
        {
            double mn, mx;
            Random r = new Random();

            // Calculate 2D
            mn = 10000.0;
            mx = -10000.0;
            for (int c = 0; c < 10000; ++c)
            {
                double nx = r.NextDouble() * 4.0 - 2.0;
                double ny = r.NextDouble() * 4.0 - 2.0;

                double v = Source.Get(nx, ny);
                if (v < mn) mn = v;
                if (v > mx) mx = v;
            }

            m_scale2 = (m_high - m_low) / (mx - mn);
            m_offset2 = m_low - mn * m_scale2;
        }
    }
}
