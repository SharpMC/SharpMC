using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public enum FractalType
    {
        FBM,
        RIDGEDMULTI,
        BILLOW,
        MULTI,
        HYBRIDMULTI
    }

    public class Fractal : ModuleBase
    {
        public FractalType Type;
        public int m_numoctaves;
        private double m_H;
        private double m_gain;
        private double m_offset;
        private double m_lacunarity;
        public double m_frequency;
        private BasisFunction[] m_basis = new BasisFunction[MaxSources];
       // private ModuleBase[] m_source = new ModuleBase[MaxSources];
        private double[] m_exparray = new double[MaxSources];
        private double[,] m_correct = new double[MaxSources, 2];

        public Fractal(FractalType type, BasisTypes basisType, InterpTypes interpType, int? octaves, double? frequency, uint? seed)
        {
            if (octaves == null)
                octaves = 8;

            if (frequency == null)
                frequency = 1.0;

            if (seed == null)
                seed = 10000;

            SetOctaves((int)octaves);
            m_frequency = (double)frequency;
            m_lacunarity = 2;
            SetType(type);
            SetAllSourceTypes(basisType, interpType);

            SetSeed((uint)seed);
        }

        private void SetOctaves(int octaves)
        {
            if (octaves >= MaxSources)
                octaves = MaxSources - 1;

            m_numoctaves = octaves;
        }

        private void SetType(FractalType type)
	    {
		    this.Type = type;
		    
            switch(type)
		    {
                case FractalType.FBM: m_H = 1.0; m_gain = 0; m_offset = 0; fBm_calcWeights(); break;
                case FractalType.RIDGEDMULTI: m_H = 0.9; m_gain = 2; m_offset = 1; RidgedMulti_calcWeights(); break;
                case FractalType.BILLOW: m_H = 1; m_gain = 0; m_offset = 0; Billow_calcWeights(); break;
                case FractalType.MULTI: m_H = 1; m_offset = 0; m_gain = 0; Multi_calcWeights(); break;
                case FractalType.HYBRIDMULTI: m_H = 0.25; m_gain = 1; m_offset = 0.7; HybridMulti_calcWeights(); break;
		        default: m_H=1.0; m_gain=0;m_offset=0; fBm_calcWeights(); break;
		    };
	    }

        private void SetAllSourceTypes(BasisTypes basis_type, InterpTypes interp)
        {
            for(int i=0; i < MaxSources; i++)
                m_basis[i] = new BasisFunction(basis_type, interp);
        }

        void SetSeed(uint seed)
	    {
            for (uint c = 0; c < MaxSources; ++c) m_basis[c].SetSeed(seed + c * 300);
	    }

        #region Weight Calculations
        private void fBm_calcWeights()
        {
	        //std::cout << "Weights: ";
	        for(int i=0; i<MaxSources; ++i)
            {
                m_exparray[i]= Math.Pow(m_lacunarity, -i*m_H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.0, maxvalue = 0.0;

            for(int i=0; i<MaxSources; ++i)
            {
                minvalue += -1.0 * m_exparray[i];
                maxvalue += 1.0 * m_exparray[i];

                double A = -1.0, B = 1.0;
                double scale = (B - A) / (maxvalue - minvalue);
                double bias = A - minvalue * scale;
                m_correct[i,0]=scale;
                m_correct[i,1]=bias;
            }
        }

        private void RidgedMulti_calcWeights()
        {
            for (int i = 0; i < MaxSources; ++i)
            {
                m_exparray[i] = Math.Pow(m_lacunarity, -i * m_H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.0, maxvalue = 0.0;
            for (int i = 0; i < MaxSources; ++i)
            {
                minvalue += (m_offset - 1.0) * (m_offset - 1.0) * m_exparray[i];
                maxvalue += (m_offset) * (m_offset) * m_exparray[i];

                double A = -1.0, B = 1.0;
                double scale = (B - A) / (maxvalue - minvalue);
                double bias = A - minvalue * scale;
                m_correct[i,0] = scale;
                m_correct[i,1] = bias;
            }
        }

        private void Billow_calcWeights()
        {
            for (int i = 0; i < MaxSources; ++i)
            {
                m_exparray[i] = Math.Pow(m_lacunarity, -i * m_H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.0, maxvalue = 0.0;
            for (int i = 0; i < MaxSources; ++i)
            {
                minvalue += -1.0 * m_exparray[i];
                maxvalue += 1.0 * m_exparray[i];

                double A = -1.0, B = 1.0;
                double scale = (B - A) / (maxvalue - minvalue);
                double bias = A - minvalue * scale;
                m_correct[i, 0] = scale;
                m_correct[i, 1] = bias;
            }
        }

        private void Multi_calcWeights()
        {
            for (int i = 0; i < MaxSources; ++i)
                m_exparray[i] = Math.Pow(m_lacunarity, -i * m_H);

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 1.0, maxvalue = 1.0;
            for (int i = 0; i < MaxSources; ++i)
            {
                minvalue *= -1.0 * m_exparray[i] + 1.0;
                maxvalue *= 1.0 * m_exparray[i] + 1.0;

                double A = -1.0, B = 1.0;
                double scale = (B - A) / (maxvalue - minvalue);
                double bias = A - minvalue * scale;
                m_correct[i, 0] = scale;
                m_correct[i, 1] = bias;
            }

        }

        private void HybridMulti_calcWeights()
        {
            for (int i = 0; i < MaxSources; ++i)
            {
                m_exparray[i] = Math.Pow(m_lacunarity, -i * m_H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 1.0, maxvalue = 1.0;
            double weightmin, weightmax;
            double A = -1.0, B = 1.0, scale, bias;

            minvalue = m_offset - 1.0;
            maxvalue = m_offset + 1.0;
            weightmin = m_gain * minvalue;
            weightmax = m_gain * maxvalue;

            scale = (B - A) / (maxvalue - minvalue);
            bias = A - minvalue * scale;
            m_correct[0,0] = scale;
            m_correct[0,1] = bias;

            for (int i = 1; i < MaxSources; ++i)
            {
                if (weightmin > 1.0) weightmin = 1.0;
                if (weightmax > 1.0) weightmax = 1.0;

                double signal = (m_offset - 1.0) * m_exparray[i];
                minvalue += signal * weightmin;
                weightmin *= m_gain * signal;

                signal = (m_offset + 1.0) * m_exparray[i];
                maxvalue += signal * weightmax;
                weightmax *= m_gain * signal;

                scale = (B - A) / (maxvalue - minvalue);
                bias = A - minvalue * scale;
                m_correct[i,0] = scale;
                m_correct[i,1] = bias;
            }
        }
        #endregion

        public override double Get(double x, double y)
        {
            double v = 0;

            switch (Type)
            {
                case FractalType.FBM: v = fBm_get(x, y); break;
                case FractalType.RIDGEDMULTI: v = RidgedMulti_get(x, y); break;
                case FractalType.BILLOW: v = Billow_get(x, y); break;
                case FractalType.MULTI: v = Multi_get(x, y); break;
                case FractalType.HYBRIDMULTI: v = HybridMulti_get(x, y); break;
                default: v = fBm_get(x, y); break;
            }

            return Helper.Clamp(v, -1.0, 1.0);
        }

        #region Gets

        private double fBm_get(double x, double y)
        {
            double value = 0.0;
            double signal = 0;

            x *= m_frequency;
            y *= m_frequency;

            for (int i = 0; i < m_numoctaves; i++)
            {
                signal = m_basis[i].Get(x, y) * m_exparray[i];
                value += signal;
                x *= m_lacunarity;
                y *= m_lacunarity;
            }

            return value;
        }

        private double Multi_get(double x, double y)
        {
            double value = 1.0;
            x*=m_frequency;
            y*=m_frequency;

            for(int i=0; i < m_numoctaves; i++)
            {
                value *= m_basis[i].Get(x, y) * m_exparray[i] + 1.0;
                x*=m_lacunarity;
                y*=m_lacunarity;

            }

            return value * m_correct[m_numoctaves - 1, 0] + m_correct[m_numoctaves - 1, 1];
        }

        private double Billow_get(double x, double y)
        {
	        double value=0.0, signal;
            x*=m_frequency;
            y*=m_frequency;

            for(uint i=0; i < m_numoctaves; ++i)
            {
                signal = m_basis[i].Get(x,y);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * m_exparray[i];

                x*=m_lacunarity;
                y*=m_lacunarity;
            }

            value += 0.5;
            return value*m_correct[m_numoctaves-1,0] + m_correct[m_numoctaves-1,1];
        }

        private double RidgedMulti_get(double x, double y)
        {
	        double result=0.0, signal;
            x*=m_frequency;
            y*=m_frequency;

            for(uint i=0; i<m_numoctaves; ++i)
            {
                signal=m_basis[i].Get(x,y);
                signal=m_offset-Math.Abs(signal);
                signal *= signal;
                result +=signal*m_exparray[i];

                x*=m_lacunarity;
                y*=m_lacunarity;

            }

            return result * m_correct[m_numoctaves-1,0] + m_correct[m_numoctaves-1,1];
        }

        private double HybridMulti_get(double x, double y)
        {
	        double value, signal, weight;
            x*=m_frequency;
            y*=m_frequency;


            value = m_basis[0].Get(x,y) + m_offset;
            weight = m_gain * value;
            x*=m_lacunarity;
            y*=m_lacunarity;

            for(uint i=1; i < m_numoctaves; ++i)
            {
                if(weight>1.0) weight=1.0;
                signal = (m_basis[i].Get(x,y)+m_offset)*m_exparray[i];
                value += weight*signal;
                weight *=m_gain * signal;
                x*=m_lacunarity;
                y*=m_lacunarity;

            }

            return value*m_correct[m_numoctaves-1,0] + m_correct[m_numoctaves-1,1];
        }

        #endregion
    }
}
