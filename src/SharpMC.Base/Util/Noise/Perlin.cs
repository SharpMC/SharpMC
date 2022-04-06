namespace SharpMC.Util.Noise
{
    public class Perlin : NoiseGen
    {
        public int Seed { get; set; }
        public int Octaves { get; set; }
        public double Amplitude { get; set; }
        public double Persistance { get; set; }
        public double Frequency { get; set; }
        public double Lacunarity { get; set; }
        public InterpolateType Interpolation { get; set; }

        public Perlin(int seed)
        {
            Seed = seed;
            Octaves = 2;
            Amplitude = 2;
            Persistance = 1;
            Frequency = 1;
            Lacunarity = 2;
            Interpolation = InterpolateType.COSINE;
        }

        /*
         * Psuedo-random number generator methods.
         * For this we use integer noise
         */

        private double Noise2D(double x, double y)
        {
            var n = ((int)x * 1619 + (int)y * 31337 * 1013 * Seed) & 0x7fffffff;
            n = (n << 13) ^ n;
            return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }

        private double Noise3D(double x, double y, double z)
        {
            var n = ((int)x * 1619 + (int)y * 31337 + (int)z * 52591 * 1013 * Seed) & 0x7fffffff;
            n = (n << 13) ^ n;
            return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }

        /*
         * Perlin Noise methods
         */

        public override double Value2D(double x, double y)
        {
            double total = 0.0;
            double frequency = Frequency;
            double amplitude = Amplitude;

            for (int I = 0; I < Octaves; I++)
            {
                total += Interpolated2D(x * frequency, y * frequency) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Persistance;
            }
            return total;
        }

        public override double Value3D(double x, double y, double z)
        {
            double total = 0.0;
            double frequency = Frequency;
            double amplitude = Amplitude;

            for (int I = 0; I < Octaves; I++)
            {
                total += Interpolated3D(x * frequency, y * frequency, z * frequency) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Persistance;
            }
            return total;
        }

        /*
         * Smooth Noise Methods
         */

        private double Smooth2D(double x, double y)
        {
            double x0 = x - 1;
            double x1 = x + 1;
            double y0 = y - 1;
            double y1 = y + 1;

            double corners = (Noise2D(x0, y0) + Noise2D(x1, y0) + Noise2D(x0, y1) + Noise2D(x1, y1)) / 16;
            double sides = (Noise2D(x0, y) + Noise2D(x1, y) + Noise2D(x, y0) + Noise2D(x, y1)) / 8;
            double center = Noise2D(x, y) / 4;

            return corners + sides + center;
        }

        private double Smooth3D(double x, double y, double z)
        {
            double corners = 0;
            corners += Noise3D(x - 1, y - 1, z - 1) + Noise3D(x - 1, y - 1, z + 1) + Noise3D(x - 1, y + 1, z - 1) +
                       Noise3D(x - 1, y + 1, z + 1);
            corners += Noise3D(x + 1, y - 1, z - 1) + Noise3D(x + 1, y - 1, z + 1) + Noise3D(x + 1, y + 1, z - 1) +
                       Noise3D(x + 1, y + 1, z + 1);
            corners /= 32;
            
            corners += Noise3D(x - 1, y, z) + Noise3D(x - 1, y, z) + Noise3D(x, y + 1, z);
            corners += Noise3D(x, y - 1, z) + Noise3D(x, y, z + 1) + Noise3D(x, y, z - 1);
            corners /= 16;
            double center = Noise3D(x, y, z) / 8;
            return corners + center;
        }

        /*
         * Interpolated Noise Methods
         */

        public double Interpolated2D(double x, double y)
        {
            //Grid Cell Coordinates
            int x0 = Floor(x);
            int x1 = x0 + 1;
            int y0 = Floor(y);
            int y1 = y0 + 1;

            //Interpolation weights
            double sx = x - x0;
            double sy = y - y0;

            //Interpolate
            double n0 = Smooth2D(x0, y0);
            double n1 = Smooth2D(x1, y0);
            double n2 = Smooth2D(x0, y1);
            double n3 = Smooth2D(x1, y1);
            double ix0 = Interpolate(n0, n1, sx, Interpolation);
            double ix1 = Interpolate(n2, n3, sx, Interpolation);
            return Interpolate(ix0, ix1, sy, Interpolation);
        }

        public double Interpolated3D(double x, double y, double z)
        {
            //Grid Cell Coordinates
            int x0 = Floor(x);
            int x1 = x0 + 1;
            int y0 = Floor(y);
            int y1 = y0 + 1;
            int z0 = Floor(z);
            int z1 = z0 + 1;

            //Interpolation weights
            double sx = x - x0;
            double sy = y - y0;
            double sz = z - z0;

            //Interpolate
            double n0 = Smooth3D(x0, y0, z0);
            double n1 = Smooth3D(x1, y0, z0);
            double n2 = Smooth3D(x0, y1, z0);
            double n3 = Smooth3D(x1, y1, z0);
            double n4 = Smooth3D(x0, y0, z1);
            double n5 = Smooth3D(x1, y0, z1);
            double n6 = Smooth3D(x0, y1, z1);
            double n7 = Smooth3D(x1, y1, z1);
            double ix0 = Interpolate(n0, n1, sx, Interpolation);
            double ix1 = Interpolate(n2, n3, sx, Interpolation);
            double ix2 = Interpolate(n4, n5, sx, Interpolation);
            double ix3 = Interpolate(n6, n7, sx, Interpolation);
            double iy0 = Interpolate(ix0, ix1, sy, Interpolation);
            double iy1 = Interpolate(ix2, ix3, sy, Interpolation);
            return Interpolate(iy0, iy1, sz, Interpolation);
        }
    }
}
