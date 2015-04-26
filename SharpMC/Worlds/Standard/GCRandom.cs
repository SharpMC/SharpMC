using LibNoise;
using LibNoise.Primitive;

namespace SharpMC.Worlds.Standard
{
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
			chunk = chunki;
			subtractForLessThanCutoff = amplitude1 - CutOff;
			f1xz = 1.0/sxz;
			f1y = 1.0/sy;

			if (MaxCaveHeight - MinCaveHeight > 128)
			{
				caveBandBuffer = 32;
			}
			else
			{
				caveBandBuffer = 16;
			}

			noiseGen1 = new SimplexPerlin(seed, NoiseQuality.Fast);
			noiseGen2 = new SimplexPerlin((int) noiseGen1.GetValue(chunk.X, chunk.Z), NoiseQuality.Fast);
			noiseGen3 = new SimplexPerlin((int) noiseGen1.GetValue(chunk.X, chunk.Z), NoiseQuality.Fast);
		}

		public bool IsInCave(int x, int y, int z)
		{
			float xx = (chunk.X << 4) | (x & 0xF);
			float yy = y;
			float zz = (chunk.Z << 4) | (z & 0xF);

			double n1 = (noiseGen1.GetValue((float) (xx*f1xz), (float) (yy*f1y), (float) (zz*f1xz))*amplitude1);
			double n2 = (noiseGen2.GetValue((float) (xx*f2xz), (float) (yy*f2y), (float) (zz*f2xz))*amplitude2);
			double n3 = (noiseGen3.GetValue((float) (xx*f3xz), (float) (yy*f3y), (float) (zz*f3xz))*amplitude3);
			var lc = LinearCutoffCoefficient(y);

			var isInCave = n1 + n2 - n3 - lc > 62;
			return isInCave;
		}

		private double LinearCutoffCoefficient(int y)
		{
			// Out of bounds
			if (y < MinCaveHeight || y > MaxCaveHeight)
			{
				return subtractForLessThanCutoff;
				// Bottom layer distortion
			}
			if (y >= MinCaveHeight && y <= MinCaveHeight + caveBandBuffer)
			{
				double yy = y - MinCaveHeight;
				return (-subtractForLessThanCutoff/caveBandBuffer)*yy + subtractForLessThanCutoff;
				// Top layer distortion
			}
			if (y <= MaxCaveHeight && y >= MaxCaveHeight - caveBandBuffer)
			{
				double yy = y - MaxCaveHeight + caveBandBuffer;
				return (subtractForLessThanCutoff/caveBandBuffer)*yy;
				// In bounds, no distortion
			}
			return 0;
		}
	}
}