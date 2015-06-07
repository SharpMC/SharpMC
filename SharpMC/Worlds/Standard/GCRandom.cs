#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Worlds.Standard
{
	using LibNoise;
	using LibNoise.Primitive;

	internal class GCRandom
	{
		// Density
		private readonly int amplitude1 = 100;

		private readonly int amplitude2 = 2;

		private readonly int amplitude3 = 20;

		// Position
		private readonly int caveBandBuffer;

		private readonly int CutOff = 62;

		private readonly double f1xz;

		private readonly double f1y;

		// Second pass - small noise
		private readonly double f2xz = 0.25;

		private readonly double f2y = 0.05;

		// Third pass - vertical noise
		private readonly double f3xz = 0.025;

		private readonly double f3y = 0.005;

		private readonly int MaxCaveHeight = 50;

		private readonly int MinCaveHeight = 6;

		private readonly SimplexPerlin noiseGen1;

		private readonly SimplexPerlin noiseGen2;

		private readonly SimplexPerlin noiseGen3;

		private readonly double subtractForLessThanCutoff;

		private readonly int sxz = 200;

		private readonly int sy = 100;

		public ChunkColumn chunk;

		internal GCRandom(ChunkColumn chunki, int seed)
		{
			this.chunk = chunki;
			this.subtractForLessThanCutoff = this.amplitude1 - this.CutOff;
			this.f1xz = 1.0 / this.sxz;
			this.f1y = 1.0 / this.sy;

			if (this.MaxCaveHeight - this.MinCaveHeight > 128)
			{
				this.caveBandBuffer = 32;
			}
			else
			{
				this.caveBandBuffer = 16;
			}

			this.noiseGen1 = new SimplexPerlin(seed, NoiseQuality.Fast);
			this.noiseGen2 = new SimplexPerlin((int)this.noiseGen1.GetValue(this.chunk.X, this.chunk.Z), NoiseQuality.Fast);
			this.noiseGen3 = new SimplexPerlin((int)this.noiseGen1.GetValue(this.chunk.X, this.chunk.Z), NoiseQuality.Fast);
		}

		public bool IsInCave(int x, int y, int z)
		{
			float xx = (this.chunk.X << 4) | (x & 0xF);
			float yy = y;
			float zz = (this.chunk.Z << 4) | (z & 0xF);

			double n1 = this.noiseGen1.GetValue((float)(xx * this.f1xz), (float)(yy * this.f1y), (float)(zz * this.f1xz))
			             * this.amplitude1;
			double n2 = this.noiseGen2.GetValue((float)(xx * this.f2xz), (float)(yy * this.f2y), (float)(zz * this.f2xz))
			             * this.amplitude2;
			double n3 = this.noiseGen3.GetValue((float)(xx * this.f3xz), (float)(yy * this.f3y), (float)(zz * this.f3xz))
			             * this.amplitude3;
			var lc = this.LinearCutoffCoefficient(y);

			var isInCave = n1 + n2 - n3 - lc > 62;
			return isInCave;
		}

		private double LinearCutoffCoefficient(int y)
		{
			// Out of bounds
			if (y < this.MinCaveHeight || y > this.MaxCaveHeight)
			{
				return this.subtractForLessThanCutoff;

				// Bottom layer distortion
			}

			if (y >= this.MinCaveHeight && y <= this.MinCaveHeight + this.caveBandBuffer)
			{
				double yy = y - this.MinCaveHeight;
				return (-this.subtractForLessThanCutoff / this.caveBandBuffer) * yy + this.subtractForLessThanCutoff;

				// Top layer distortion
			}

			if (y <= this.MaxCaveHeight && y >= this.MaxCaveHeight - this.caveBandBuffer)
			{
				double yy = y - this.MaxCaveHeight + this.caveBandBuffer;
				return (this.subtractForLessThanCutoff / this.caveBandBuffer) * yy;

				// In bounds, no distortion
			}

			return 0;
		}
	}
}