// Distributed under the MIT license
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

namespace SharpMC.Core.Worlds.Standard
{
	internal class GcRandom
	{
		// Density
		private readonly int _amplitude1 = 100;
		private readonly int _amplitude2 = 2;
		private readonly int _amplitude3 = 20;
		// Position
		private readonly int _caveBandBuffer;
		private readonly int _cutOff = 62;
		private readonly double _f1Xz;
		private readonly double _f1Y;
		// Second pass - small noise
		private readonly double _f2Xz = 0.25;
		private readonly double _f2Y = 0.05;
		// Third pass - vertical noise
		private readonly double _f3Xz = 0.025;
		private readonly double _f3Y = 0.005;
		private readonly int _maxCaveHeight = 50;
		private readonly int _minCaveHeight = 6;
		private readonly SimplexPerlin _noiseGen1;
		private readonly SimplexPerlin _noiseGen2;
		private readonly SimplexPerlin _noiseGen3;
		private readonly double _subtractForLessThanCutoff;
		private readonly int _sxz = 200;
		private readonly int _sy = 100;
		public ChunkColumn Chunk;

		internal GcRandom(ChunkColumn chunki, int seed)
		{
			Chunk = chunki;
			_subtractForLessThanCutoff = _amplitude1 - _cutOff;
			_f1Xz = 1.0/_sxz;
			_f1Y = 1.0/_sy;

			if (_maxCaveHeight - _minCaveHeight > 128)
			{
				_caveBandBuffer = 32;
			}
			else
			{
				_caveBandBuffer = 16;
			}

			_noiseGen1 = new SimplexPerlin(seed, NoiseQuality.Fast);
			_noiseGen2 = new SimplexPerlin((int) _noiseGen1.GetValue(Chunk.X, Chunk.Z), NoiseQuality.Fast);
			_noiseGen3 = new SimplexPerlin((int) _noiseGen1.GetValue(Chunk.X, Chunk.Z), NoiseQuality.Fast);
		}

		public bool IsInCave(int x, int y, int z)
		{
			float xx = (Chunk.X << 4) | (x & 0xF);
			float yy = y;
			float zz = (Chunk.Z << 4) | (z & 0xF);

			double n1 = (_noiseGen1.GetValue((float) (xx*_f1Xz), (float) (yy*_f1Y), (float) (zz*_f1Xz))*_amplitude1);
			double n2 = (_noiseGen2.GetValue((float) (xx*_f2Xz), (float) (yy*_f2Y), (float) (zz*_f2Xz))*_amplitude2);
			double n3 = (_noiseGen3.GetValue((float) (xx*_f3Xz), (float) (yy*_f3Y), (float) (zz*_f3Xz))*_amplitude3);
			var lc = LinearCutoffCoefficient(y);

			var isInCave = n1 + n2 - n3 - lc > 62;
			return isInCave;
		}

		private double LinearCutoffCoefficient(int y)
		{
			// Out of bounds
			if (y < _minCaveHeight || y > _maxCaveHeight)
			{
				return _subtractForLessThanCutoff;
				// Bottom layer distortion
			}
			if (y >= _minCaveHeight && y <= _minCaveHeight + _caveBandBuffer)
			{
				double yy = y - _minCaveHeight;
				return (-_subtractForLessThanCutoff/_caveBandBuffer)*yy + _subtractForLessThanCutoff;
				// Top layer distortion
			}
			if (y <= _maxCaveHeight && y >= _maxCaveHeight - _caveBandBuffer)
			{
				double yy = y - _maxCaveHeight + _caveBandBuffer;
				return (_subtractForLessThanCutoff/_caveBandBuffer)*yy;
				// In bounds, no distortion
			}
			return 0;
		}
	}
}