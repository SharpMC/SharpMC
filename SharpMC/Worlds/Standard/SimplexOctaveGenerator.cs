// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015
using LibNoise;
using LibNoise.Primitive;

namespace SharpMC.Worlds.Standard
{
	public class SimplexOctaveGenerator
	{
		private readonly SimplexPerlin[] _generators;
		//private readonly OpenSimplexNoise[] _generators;
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

		public int Octaves { get; private set; }
		public long Seed { get; private set; }
		public double XScale { get; set; }
		public double YScale { get; set; }
		public double ZScale { get; set; }
		public double WScale { get; set; }

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
				result += octave.GetValue((float) (x*freq), (float) (y*freq))*amp;
				max += amp;
				freq *= frequency;
				amp *= amplitude;
			}

			return result;
		}

		public double Noise(double x, double y, double z, double frequency, double amplitude)
		{
			return Noise(x, y, z, 0, frequency, amplitude, false);
		}

		public double Noise(double x, double y, double z, double w, double frequency, double amplitude)
		{
			return Noise(x, y, z, w, frequency, amplitude, false);
		}

		public double Noise(double x, double y, double z, double w, double frequency, double amplitude, bool normalized)
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
				result += octave.GetValue((float) (x*freq), (float) (y*freq), (float) (z*freq), (float) (w*freq))*amp;
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