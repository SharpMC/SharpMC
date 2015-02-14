using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public static class LCG
    {
        static uint m_state = 0;

        public static void SetSeed(uint s)
        {
            m_state = s;
        }

        public static uint Get()
        {
            m_state = 69069 * m_state + 362437;
            return m_state;
        }

        public static double get01()
        {
            return ((double)Get() / (double)(4294967295UL));
        }
    }
}
