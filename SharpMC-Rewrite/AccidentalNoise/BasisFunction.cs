using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public enum BasisTypes
    {
        VALUE,
        GRADIENT,
        GRADVAL,
        SIMPLEX,
        WHITE
    };

    public enum InterpTypes
    {
        NONE,
        LINEAR,
        CUBIC,
        QUINTIC
    };

    public class BasisFunction
    {
        private BasisTypes Type;
        private InterpTypes Interp;
        private double[] m_scale = new double[4];
        private double[] m_offset = new double[4];
        private uint m_seed;

        double[,] m_rotmatrix = new double[3, 3];
        double cos2d, sin2d;

        //public BasisFunction()
        //{
        //    this.Type = BasisTypes.GRADIENT;
        //    this.Interp = InterpTypes.QUINTIC;
        //    setMagicNumbers(this.Type);

        //    SetSeed(1000);
        //}

        public BasisFunction (BasisTypes type, InterpTypes interp)
	    {
            this.Type = type;
            this.Interp = interp;
            setMagicNumbers(type);
            SetSeed(1000);
	    }

        private double GetBasis(double x, double y, uint seed)
        {
            //call appr. noise method
            switch (Type)
            {
                case BasisTypes.VALUE: return NoiseGen.value_noise2D(x, y, seed, Interp);
                case BasisTypes.GRADIENT: return NoiseGen.gradient_noise2D(x, y, seed, Interp);
                case BasisTypes.GRADVAL: return NoiseGen.gradval_noise2D(x, y, seed, Interp);
                case BasisTypes.WHITE: return NoiseGen.white_noise2D(x, y, seed, Interp);
                case BasisTypes.SIMPLEX: throw new NotImplementedException();
                default: return NoiseGen.gradient_noise2D(x, y, seed, Interp);
            }
        }

        private void setMagicNumbers(BasisTypes type)
        {
            // This function is a damned hack.
            // The underlying noise functions don't return values in the range [-1,1] cleanly, and the ranges vary depending
            // on basis type and dimensionality. There's probably a better way to correct the ranges, but for now I'm just
            // setting he magic numbers m_scale and m_offset manually to empirically determined magic numbers.
            switch(type)
            {
                case BasisTypes.VALUE:
                {
                    m_scale[0]=1.0; m_offset[0]=0.0;
                    m_scale[1]=1.0; m_offset[1]=0.0;
                    m_scale[2]=1.0; m_offset[2]=0.0;
                    m_scale[3]=1.0; m_offset[3]=0.0;
                } break;

                case BasisTypes.GRADIENT:
                {
                    m_scale[0]=1.86848; m_offset[0]=-0.000118;
                    m_scale[1]=1.85148; m_offset[1]=-0.008272;
                    m_scale[2]=1.64127; m_offset[2]=-0.01527;
                    m_scale[3]=1.92517; m_offset[3]=0.03393;
                } break;

                case BasisTypes.GRADVAL:
                {
                    m_scale[0]=0.6769; m_offset[0]=-0.00151;
                    m_scale[1]=0.6957; m_offset[1]=-0.133;
                    m_scale[2]=0.74622; m_offset[2]=0.01916;
                    m_scale[3]=0.7961; m_offset[3]=-0.0352;
                } break;

                case BasisTypes.WHITE:
                {
                    m_scale[0]=1.0; m_offset[0]=0.0;
                    m_scale[1]=1.0; m_offset[1]=0.0;
                    m_scale[2]=1.0; m_offset[2]=0.0;
                    m_scale[3]=1.0; m_offset[3]=0.0;
                } break;

                default:
                {
                    m_scale[0]=1.0; m_offset[0]=0.0;
                    m_scale[1]=1.0; m_offset[1]=0.0;
                    m_scale[2]=1.0; m_offset[2]=0.0;
                    m_scale[3]=1.0; m_offset[3]=0.0;
                } break;
            };
        }

        private void setRotationAngle(double x, double y, double z, double angle)
        {
            m_rotmatrix[0, 0] = 1 + (1 - Math.Cos(angle)) * (x * x - 1);
            m_rotmatrix[1, 0] = -z * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * y;
            m_rotmatrix[2, 0] = y * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * z;

            m_rotmatrix[0, 1] = z * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * y;
            m_rotmatrix[1, 1] = 1 + (1 - Math.Cos(angle)) * (y * y - 1);
            m_rotmatrix[2, 1] = -x * Math.Sin(angle) + (1 - Math.Cos(angle)) * y * z;

            m_rotmatrix[0, 2] = -y * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * z;
            m_rotmatrix[1, 2] = x * Math.Sin(angle) + (1 - Math.Cos(angle)) * y * z;
            m_rotmatrix[2, 2] = 1 + (1 - Math.Cos(angle)) * (z * z - 1);
        }

        public void SetSeed(uint seed)
        {
            m_seed = seed;
            //Random random = new Random(seed);
            LCG.SetSeed(seed);

            double ax, ay, az;
            double len;

            ax = LCG.get01();
            ay = LCG.get01();
            az = LCG.get01();
            len = Math.Sqrt(ax * ax + ay * ay + az * az);
            ax /= len;
            ay /= len;
            az /= len;
            setRotationAngle(ax, ay, az, LCG.get01() * 3.141592 * 2.0);
            double angle = LCG.get01() * 3.14159265 * 2.0;
            cos2d = Math.Cos(angle);
            sin2d = Math.Sin(angle);
        }

        public double Get(double x, double y)
        {
            double nx, ny;
            nx = x * cos2d - y * sin2d;
            ny = y * cos2d + x * sin2d;
            return GetBasis(nx, ny, m_seed);
        }
    }
}
