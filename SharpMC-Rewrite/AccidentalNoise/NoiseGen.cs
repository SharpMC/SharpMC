using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public static class NoiseGen
    {
        const uint FNV_32_PRIME = ((uint)0x01000193);
        const uint FNV_32_INIT  =((uint )2166136261);
        const uint FNV_MASK_8 =(((uint)1<<8)-1);

        public enum NoiseFunc
        {
            value_noise_2,
            grad_noise_2
        }

        static double noInterp(double t)
        {
            return 0;
        }

        static double linearInterp(double t)
        {
            return t;
        }

        static double hermiteInterp(double t)
        {
            return (t*t*(3-2*t));
        }

        static double quinticInterp(double t)
        {
            return t*t*t*(t*(t*6-15)+10);
        }

        static double interp(double t, InterpTypes type)
        {
            switch (type)
            {
                case InterpTypes.NONE: return noInterp(t); 
                case InterpTypes.LINEAR: return linearInterp(t); 
                case InterpTypes.QUINTIC: return quinticInterp(t);
                case InterpTypes.CUBIC: return hermiteInterp(t);
            }

            return noInterp(t);
        }

        static private int fast_floor(double t)
        {
            return (t > 0 ? (int)t : (int)t - 1);
        }

        // Edge/Face/Cube/Hypercube interpolation
        static double lerp(double s, double v1, double v2)
        {
            return v1 + s * (v2 - v1);
        }

        private static double noisefunc(double x, double y, int ix, int iy, uint seed, NoiseFunc function)
        {
            switch (function)
            {
                case NoiseFunc.value_noise_2: return value_noise_2(x, y, ix, iy, seed);
                case NoiseFunc.grad_noise_2: return grad_noise_2(x, y, ix, iy, seed);
                default: return value_noise_2(x, y, ix, iy, seed);
            }
        }

        public static double value_noise2D(double x, double y, uint seed, InterpTypes type)
        {
	        int x0=fast_floor(x);
            int y0=fast_floor(y);

            int x1=x0+1;
            int y1=y0+1;

            double xs = interp((x - (double)x0), type);
            double ys = interp((y - (double)y0), type);

	        return interp_XY_2(x,y,xs,ys,x0,x1,y0,y1,seed,NoiseFunc.value_noise_2);
        }

        static double grad_noise_2(double x, double y, int ix, int iy, uint seed)
        {
            uint hash=hash_coords_2(ix,iy,seed);
            double vec1 = gradient2D_lut[hash][0];
            double vec2 = gradient2D_lut[hash][1];

            double dx=x-(double)ix;
            double dy=y-(double)iy;

            return (dx * vec1 + dy * vec2);
        }

        public static double gradient_noise2D(double x, double y, uint seed, InterpTypes type)
        {
	        int x0=fast_floor(x);
            int y0=fast_floor(y);

            int x1=x0+1;
            int y1=y0+1;

            double xs = interp((x - (double)x0), type);
            double ys = interp((y - (double)y0), type);

	        return interp_XY_2(x,y,xs,ys,x0,x1,y0,y1,seed,NoiseFunc.grad_noise_2);
        }

        public static double gradval_noise2D(double x, double y, uint seed, InterpTypes type)
        {
            return value_noise2D(x, y, seed, type) + gradient_noise2D(x, y, seed, type);
        }

        public static double white_noise2D(double x, double y, uint seed, InterpTypes interp)
        {
	        byte hash = (byte) compute_hash_double_2(x,y,seed);
	        return whitenoise_lut[hash];
        }

        const double F2 = 0.36602540378443864676372317075294;
        const double G2 = 0.21132486540518711774542560974902;

        //public static double simplex_noise2D(double x, double y, int seed, InterpTypes interp)
        //{
        //    double s = (x + y) * F2;
        //    int i=fast_floor(x+s);
        //    int j=fast_floor(y+s);

        //    double t = (i+j)*G2;
        //    double X0=i-t;
        //    double Y0=j-t;
        //    double x0=x-X0;
        //    double y0=y-Y0;

        //    int i1,j1;
        //    if(x0>y0)
        //    {
        //        i1=1; j1=0;
        //    }
        //    else
        //    {
        //        i1=0; j1=1;
        //    }

        //    double x1=x0-(double)i1+G2;
        //    double y1=y0-(double)j1+G2;
        //    double x2=x0-1.0+2.0*G2;
        //    double y2=y0-1.0+2.0*G2;

        //    // Hash the triangle coordinates to index the gradient table
        //    uint h0=hash_coords_2(i,j,seed);
        //    uint h1=hash_coords_2(i+i1,j+j1,seed);
        //    uint h2=hash_coords_2(i+1,j+1,seed);

        //    // Now, index the tables
        //    double g0 = gradient2D_lut[h0,0];
        //    double g1 = gradient2D_lut[h1,0];
        //    double g2 = gradient2D_lut[h2,0];

        //    double n0,n1,n2;
        //    // Calculate the contributions from the 3 corners
        //    double t0=0.5-x0*x0-y0*y0;
        //    if(t0<0) n0=0;
        //    else
        //    {
        //        t0 *= t0;
        //        n0 = t0 * t0 * array_dot2(g0,x0,y0);
        //    }

        //    double t1=0.5-x1*x1-y1*y1;
        //    if(t1<0) n1=0;
        //    else
        //    {
        //        t1*=t1;
        //        n1=t1*t1*array_dot2(g1,x1,y1);
        //    }

        //    double t2=0.5-x2*x2-y2*y2;
        //    if(t2<0) n2=0;
        //    else
        //    {
        //        t2*=t2;
        //        n2=t2*t2*array_dot2(g2,x2,y2);
        //    }

        //    // Add contributions together
        //    return (70.0 * (n0+n1+n2)) *1.42188695 + 0.001054489;
        //}

        static double interp_X_2(double x, double y, double xs, int x0, int x1, int iy, uint seed, NoiseFunc function)
        {
            double v1 = noisefunc(x, y, x0, iy, seed, function);
            double v2 = noisefunc(x, y, x1, iy, seed, function);
	        return lerp(xs,v1,v2);
        }

        static double interp_XY_2(double x, double y, double xs, double ys, int x0, int x1, int y0, int y1, uint seed, NoiseFunc noisefunc)
        {
            double v1 = interp_X_2(x, y, xs, x0, x1, y0, seed, noisefunc);
            double v2 = interp_X_2(x, y, xs, x0, x1, y1, seed, noisefunc);
	        return lerp(ys,v1,v2);
        }

        static double value_noise_2(double x, double y, int ix, int iy, uint seed)
        {
            uint n = (hash_coords_2(ix,iy,seed));
            double noise = (double)n / 255.0;
            return noise * 2.0 - 1.0;
        }

        static uint hash_coords_2(int x, int y, uint seed)
        {
            uint[] d = new uint[3]
            {
                (uint)x,
                (uint)y,
                (uint)seed
            };

            return xor_fold_hash(fnv_32_a_buf(d));
        }

        static uint compute_hash_double_2(double x, double y, uint seed)
        {
            double[] d = new double[3]
                {x,y,(double)seed};

            return xor_fold_hash(fnv_32_a_buf(d));
        }

        static uint fnv_32_a_buf(uint[] buf)
        {
            uint hval = FNV_32_INIT;
            byte[] bp = IntsToBytes(buf);
            int i = 0;

            while (i < bp.Length)
            {
                hval ^= (uint)bp[i++];
                hval *= FNV_32_PRIME;
            }

            return hval;
        }

        static uint fnv_32_a_buf(double[] buf)
        {
            uint hval = FNV_32_INIT;
            byte[] bp = DoublesToBytes(buf);
            int i = 0;

            while (i < bp.Length)
            {
                hval ^= (uint)bp[i++];
                hval *= FNV_32_PRIME;
            }

            return hval;
        }

        static byte[] DoublesToBytes(double[] doubles)
        {
            byte[] bytes = new byte[doubles.Length * 8];
            int r = 0;

            for (int i = 0; i < doubles.Length; i++)
            {
                byte[] one = System.BitConverter.GetBytes(doubles[i]);

                for (int j = 0; j < one.Length; j++)
                {
                    bytes[r] = one[j];
                    r++;
                }
            }

            return bytes;
        }

        static byte[] IntsToBytes(uint[] ints)
        {
            byte[] bytes = new byte[ints.Length * 4];
            for (int i = 0, j = 0; i < ints.Length; i++)
            {
                bytes[j++] = (byte)(ints[i] & 0xFF);
                bytes[j++] = (byte)((ints[i] >> 8) & 0xFF);
                bytes[j++] = (byte)((ints[i] >> 16) & 0xFF);
                bytes[j++] = (byte)((ints[i] >> 24) & 0xFF);
            }
            return bytes;
        }

        static byte xor_fold_hash(uint hash)
        {
            // Implement XOR-folding to reduce from 32 to 8-bit hash
            return (byte)((hash>>8) ^ (hash & FNV_MASK_8));
        }

        static double array_dot2(double[] arr, double a, double b)
        {
            return a*arr[0] + b*arr[1];
        }

        #region Lookup Tables
        static double[] whitenoise_lut = new double[256]
        {
          -0.714286,
          0.301587,
          0.333333,
          -1,
          0.396825,
          -0.0793651,
          -0.968254,
          -0.047619,
          0.301587,
          -0.111111,
          0.015873,
          0.968254,
          -0.428571,
          0.428571,
          0.047619,
          0.84127,
          -0.015873,
          -0.746032,
          -0.809524,
          -0.619048,
          -0.301587,
          -0.68254,
          0.777778,
          0.365079,
          -0.460317,
          0.714286,
          0.142857,
          0.047619,
          -0.0793651,
          -0.492063,
          -0.873016,
          -0.269841,
          -0.84127,
          -0.809524,
          -0.396825,
          -0.777778,
          -0.396825,
          -0.746032,
          0.301587,
          -0.52381,
          0.650794,
          0.301587,
          -0.015873,
          0.269841,
          0.492063,
          -0.936508,
          -0.777778,
          0.555556,
          0.68254,
          -0.650794,
          -0.968254,
          0.619048,
          0.777778,
          0.68254,
          0.206349,
          -0.555556,
          0.904762,
          0.587302,
          -0.174603,
          -0.047619,
          -0.206349,
          -0.68254,
          0.111111,
          -0.52381,
          0.174603,
          -0.968254,
          -0.111111,
          -0.238095,
          0.396825,
          -0.777778,
          -0.206349,
          0.142857,
          0.904762,
          -0.111111,
          -0.269841,
          0.777778,
          -0.015873,
          -0.047619,
          -0.333333,
          0.68254,
          -0.238095,
          0.904762,
          0.0793651,
          0.68254,
          -0.301587,
          -0.333333,
          0.206349,
          0.52381,
          0.904762,
          -0.015873,
          -0.555556,
          0.396825,
          0.460317,
          -0.142857,
          0.587302,
          1,
          -0.650794,
          -0.333333,
          -0.365079,
          0.015873,
          -0.873016,
          -1,
          -0.777778,
          0.174603,
          -0.84127,
          -0.428571,
          0.365079,
          -0.587302,
          -0.587302,
          0.650794,
          0.714286,
          0.84127,
          0.936508,
          0.746032,
          0.047619,
          -0.52381,
          -0.714286,
          -0.746032,
          -0.206349,
          -0.301587,
          -0.174603,
          0.460317,
          0.238095,
          0.968254,
          0.555556,
          -0.269841,
          0.206349,
          -0.0793651,
          0.777778,
          0.174603,
          0.111111,
          -0.714286,
          -0.84127,
          -0.68254,
          0.587302,
          0.746032,
          -0.68254,
          0.587302,
          0.365079,
          0.492063,
          -0.809524,
          0.809524,
          -0.873016,
          -0.142857,
          -0.142857,
          -0.619048,
          -0.873016,
          -0.587302,
          0.0793651,
          -0.269841,
          -0.460317,
          -0.904762,
          -0.174603,
          0.619048,
          0.936508,
          0.650794,
          0.238095,
          0.111111,
          0.873016,
          0.0793651,
          0.460317,
          -0.746032,
          -0.460317,
          0.428571,
          -0.714286,
          -0.365079,
          -0.428571,
          0.206349,
          0.746032,
          -0.492063,
          0.269841,
          0.269841,
          -0.365079,
          0.492063,
          0.873016,
          0.142857,
          0.714286,
          -0.936508,
          1,
          -0.142857,
          -0.904762,
          -0.301587,
          -0.968254,
          0.619048,
          0.269841,
          -0.809524,
          0.936508,
          0.714286,
          0.333333,
          0.428571,
          0.0793651,
          -0.650794,
          0.968254,
          0.809524,
          0.492063,
          0.555556,
          -0.396825,
          -1,
          -0.492063,
          -0.936508,
          -0.492063,
          -0.111111,
          0.809524,
          0.333333,
          0.238095,
          0.174603,
          0.333333,
          0.873016,
          0.809524,
          -0.047619,
          -0.619048,
          -0.174603,
          0.84127,
          0.111111,
          0.619048,
          -0.0793651,
          0.52381,
          1,
          0.015873,
          0.52381,
          -0.619048,
          -0.52381,
          1,
          0.650794,
          -0.428571,
          0.84127,
          -0.555556,
          0.015873,
          0.428571,
          0.746032,
          -0.238095,
          -0.238095,
          0.936508,
          -0.206349,
          -0.936508,
          0.873016,
          -0.555556,
          -0.650794,
          -0.904762,
          0.52381,
          0.968254,
          -0.333333,
          -0.904762,
          0.396825,
          0.047619,
          -0.84127,
          -0.365079,
          -0.587302,
          -1,
          -0.396825,
          0.365079,
          0.555556,
          0.460317,
          0.142857,
          -0.460317,
          0.238095,
        };

        static double[][] gradient2D_lut= new double[][]
        {
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
            new double[] {0,1},
            new double[] {0,-1},
            new double[] {1,0},
            new double[] {-1,0},
        new double[] {0,1},
        new double[] {0,-1},
        new double[] {1,0},
        new double[] {-1,0},
        new double[] {0,1},
        new double[] {0,-1},
        new double[] {1,0},
        new double[] {-1,0},
        new double[] {0,1},
        new double[] {0,-1},
        new double[] {1,0},
        new double[] {-1,0},
        new double[] {0,1},
        new double[] {0,-1},
       new double[]  {1,0},
        new double[] {-1,0},
        new double[] {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
       new double[]  {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
        new double[] {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
      new double[]   {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
      new double[]   {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
       new double[]  {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
       new double[]  {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
      new double[]   {1,0},
      new double[]   {-1,0},
      new double[]   {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
       new double[]  {-1,0},
      new double[]   {0,1},
      new double[]   {0,-1},
       new double[]  {1,0},
      new double[]   {-1,0},
       new double[]  {0,1},
      new double[]   {0,-1},
      new double[]   {1,0},
      new double[]  {-1,0},
      new double[]   {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
      new double[]   {-1,0},
       new double[]  {0,1},
       new double[]  {0,-1},
       new double[]  {1,0},
      new double[]   {-1,0}
        };
        #endregion
    }
}
