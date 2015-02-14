using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public class Select : ModuleBase
    {
        private double Low_d;
        private double High_d;
        private ModuleBase Low_m;
        private ModuleBase High_m;
        private double Threshold;
        private double Falloff;

        public Select(ModuleBase controlsource, double low, double high, double threshold, double? falloff)
        {
            this.Source = controlsource;
            this.Low_d = low;
            this.High_d = high;
            this.Threshold = threshold;

            if (falloff != null)
                Falloff = (double)falloff;
        }

        public Select(ModuleBase controlsource, ModuleBase low, ModuleBase high, double threshold, double? falloff)
        {
            this.Source = controlsource;
            this.Low_m = low;
            this.High_m = high;
            this.Threshold = threshold;

            if (falloff != null)
                Falloff = (double)falloff;
        }

        public override double Get(double x, double y)
        {
            double control = Source.Get(x, y);

            double low = Low_m == null ? Low_d : Low_m.Get(x, y);
            double high = High_m == null ? High_d : High_m.Get(x, y);

            if (Falloff > 0.0)
            {
                if (control < (Threshold - Falloff))
                    return low;
                else if (control > (Threshold + Falloff))
                    return high;
                else
                {
                    double lower = Threshold - Falloff;
                    double upper = Threshold + Falloff;
                    double blend = Helper.Quintic_Blend((control - lower) / (upper - lower));
                    return Helper.Lerp(blend, low, high);
                }
            }
            else
            {
                if (control < Threshold)
                    return low;
                else
                    return high;
            }
        }
    }
}
