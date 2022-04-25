using LibNoise;
using LibNoise.Primitive;

namespace SharpMC.World.Noises
{
    public class SimplexOctaveGenerator : INoiseGenerator
    {
        private readonly SimplexPerlin[] _generators;

        public int Octaves { get; }
        public long Seed { get; }

        public SimplexOctaveGenerator(int seed, int octaves)
        {
            Seed = seed;
            Octaves = octaves;

            _generators = new SimplexPerlin[octaves];
            for (var i = 0; i < _generators.Length; i++)
            {
                _generators[i] = new SimplexPerlin(seed, NoiseQuality.Fast);
            }
        }

        public double XScale { get; set; }
        public double YScale { get; set; }
        public double ZScale { get; set; }
        public double WScale { get; set; }

        public double Noise(double x, double y, double z, double frequency, double amplitude)
        {
            return Noise(x, y, z, 0, frequency, amplitude, false);
        }

        private double Noise(double x, double y, double z, double w, double frequency, double amplitude)
        {
            return Noise(x, y, z, w, frequency, amplitude, false);
        }

        public double Noise(double x, double y, double frequency, double amplitude)
        {
            double result = 0;
            double amp = 1;
            double freq = 1;
            double max = 0;

            x *= XScale;
            y *= YScale;

            foreach (var octave in _generators)
            {
                result += octave.GetValue((float) (x * freq), (float) (y * freq)) * amp;
                max += amp;
                freq *= frequency;
                amp *= amplitude;
            }

            return result;
        }

        private double Noise(double x, double y, double z, double w, double frequency, double amplitude, bool normalized)
        {
            double result = 0;
            double amp = 1;
            double freq = 1;
            double max = 0;

            x *= XScale;
            y *= YScale;
            z *= ZScale;
            w *= WScale;

            foreach (var octave in _generators)
            {
                result += octave.GetValue((float) (x * freq), (float) (y * freq), (float) (z * freq),
                    (float) (w * freq)) * amp;
                max += amp;
                freq *= frequency;
                amp *= amplitude;
            }

            if (normalized)
            {
                result /= max;
            }

            return result;
        }

        public void SetScale(double scale)
        {
            XScale = scale;
            YScale = scale;
            ZScale = scale;
            WScale = scale;
        }
    }
}