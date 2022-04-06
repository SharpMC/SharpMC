using System;

namespace SharpMC.Util
{
    public class NoiseGeneratorOctaves
    {
        private readonly int _seed;
        private readonly MinecraftNoise[] _generators;

        public NoiseGeneratorOctaves(Random random, int i)
        {
            _seed = i;
           _generators = new MinecraftNoise[i];

            for (var j = 0; j < i; ++j)
            {
                _generators[j] = new MinecraftNoise(random);
            }
        }

        public double[] Noise(double[] adouble, int i, int j, int k, int l, int i1, int j1, double d0, double d1,
            double d2)
        {
			if (adouble == null)
			{
				adouble = new double[l * i1 * j1];
			}
			else
			{
				for (var k1 = 0; k1 < adouble.Length; ++k1)
				{
					adouble[k1] = 0.0D;
				}
			}

			var d3 = 1.0D;

            for (var l1 = 0; l1 < _seed; ++l1)
            {
                var d4 = i * d3 * d0;
                var d5 = j * d3 * d1;
                var d6 = k * d3 * d2;
                var i2 = (long)Math.Round(d4);
                var j2 = (long)Math.Round(d6);

                d4 -= i2;
                d6 -= j2;
                i2 %= 16777216L;
                j2 %= 16777216L;
                d4 += i2;
                d6 += j2;
                _generators[l1].Noise(adouble, d4, d5, d6, l, i1, j1, d0 * d3, d1 * d3, d2 * d3, d3);
                d3 /= 2.0D;
            }

            return adouble;
        }

        public double[] Noise(double[] along, int i, int j, int k, int l, double d0, double d1, double d2)
        {
            return Noise(along, i, 10, j, k, 1, l, d0, 1.0D, d1);
        }
    }
}
