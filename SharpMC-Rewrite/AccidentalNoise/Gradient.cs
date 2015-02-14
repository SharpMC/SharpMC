using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public class Gradient : ModuleBase
    {
        double m_gx1, m_gx2;
        double m_gy1, m_gy2;
        double m_x;
        double m_y;
        double m_vlen;

        public Gradient(double x1, double x2, double y1, double y2)
        {
            SetGradient(x1, x2, y1, y2);
        }

        private void SetGradient(double x1, double x2, double y1, double y2)
        {
            m_gx1 = x1;
            m_gx2 = x2;
            m_gy1 = y1;
            m_gy2 = y2;

            m_x = x2 = x1;
            m_y = y2 - y1;

            m_vlen = (m_x * m_x + m_y * m_y);
        }

        public override double Get(double x, double y)
        {
            double dx = x - m_gx1;
            double dy = y - m_gy1;
            double dp = dx * m_x + dy * m_y;
            dp /= m_vlen;

            return dp;
        }
    }
}
