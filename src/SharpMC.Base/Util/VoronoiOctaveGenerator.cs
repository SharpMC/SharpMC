namespace SharpMC.Util
{
	public class VoronoiOctaveGenerator
	{
		private double _xScale, _yScale, _zScale;
		private readonly VoronoiNoiseGenerator[] _octaves;

		public VoronoiOctaveGenerator(int seed, int numOctaves, VoronoiNoiseGenerator.DistanceMethod distanceMethod)
		{
			_octaves = new VoronoiNoiseGenerator[numOctaves];
			for (var i = 0; i < _octaves.Length; i++)
			{
				_octaves[i] = new VoronoiNoiseGenerator(seed, distanceMethod);
			}
		}

		public double GetXScale()
		{
			return _xScale;
		}

		public void SetXScale(double scale)
		{
			_xScale = scale;
		}

		public double GetYScale()
		{
			return _yScale;
		}

		public void SetYScale(double scale)
		{
			_yScale = scale;
		}

		public double GetZScale()
		{
			return _zScale;
		}

		public void SetZScale(double scale)
		{
			_zScale = scale;
		}

		public void SetScale(double scale)
		{
			SetXScale(scale);
			SetYScale(scale);
			SetZScale(scale);
		}

		public void SetSeed(int seed)
		{
			foreach (var gen in _octaves)
			{
				gen.Seed = seed;
			}
		}

		public double Noise(double x, double y, double frequency, double amplitude)
		{
			return Noise(x, y, frequency, amplitude, false);
		}

		public double Noise(double x, double y, double frequency, double amplitude,
				bool normalized)
		{
			double result = 0;
			double amp = 1;
			double freq = 1;
			double max = 0;
			x *= _xScale;
			y *= _yScale;

			foreach (var octave in _octaves)
			{
				result += octave.Noise(x, y, freq) * amp;
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

		public double Noise(double x, double y, double z, double frequency,
				double amplitude)
		{
			return Noise(x, y, z, frequency, amplitude, false);
		}

		/// To generate fractal noise, we calculate the noise of each octave at 
		/// an increasingly larger/smaller frequency and amplitude.
		public double Noise(double x, double y, double z, double frequency,
				double amplitude, bool normalized)
		{
			double result = 0;
			double amp = 1;
			double freq = 1;
			double max = 0;
			x *= _xScale;
			y *= _yScale;
			z *= _zScale;

			foreach (var octave in _octaves)
			{
				result += octave.Noise(x, y, z, freq) * amp;
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
	}
}
