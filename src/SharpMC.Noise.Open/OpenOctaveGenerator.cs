using SharpMC.Noise.API;

namespace SharpMC.Noise.Open
{
    public class OpenOctaveGenerator : INoiseGenerator
    {
        private readonly long _seed;
        private readonly int _octaves;
        private readonly OpenSimplex[] _generators;

        public OpenOctaveGenerator(int seed, int octaves)
        {
            _seed = seed;
            _octaves = octaves;

            _generators = new OpenSimplex[octaves];
            for (var i = 0; i < _generators.Length; i++)
            {
                _generators[i] = new OpenSimplex(seed);
            }
        }

        private double XScale { get; set; }
        private double YScale { get; set; }
        private double ZScale { get; set; }
        private double WScale { get; set; }

        public double Noise(double x, double y, double frequency, double amplitude)
        {
            return Noise(x, y, 0, 0, frequency, amplitude);
        }

        public double Noise(double x, double y, double z, double frequency, double amplitude)
        {
            return Noise(x, y, z, 0, frequency, amplitude);
        }

        private double Noise(double x, double y, double z, double w, double frequency,
            double amplitude, bool normalized = false)
        {
            double result = 0;
            double amp = 1;
            double freq = 1;
            double max = 0;

            x *= XScale;
            y *= YScale;
            z *= ZScale;
            w *= WScale;

            var tmp = (float) (w * freq);
            foreach (var octave in _generators)
            {
                result += octave.Value3D((float) (x * freq), (float) (y * freq), (float) (z * freq)) * amp;
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