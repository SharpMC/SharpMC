using SharpMC.Blocks;

namespace SharpMC.Worlds.Standard
{
	internal class CaveGenerator
	{
		private readonly SimplexOctaveGenerator caveNoise;
		private GCRandom gcRandom;
		private int seedi;

		public CaveGenerator(int seed)
		{
			caveNoise = new SimplexOctaveGenerator(seed + 22, 3);
			caveNoise.SetScale(1.0/100);
		}

		public void GenerateCave(ChunkColumn chunk)
		{
			gcRandom = new GCRandom(chunk, seedi);
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + chunk.X*16;
					float oz = z + chunk.Z*16;

					for (var y = 50; y >= 6; y--)
					{
						if (gcRandom.IsInCave(x, y, z))
						{
							chunk.SetBlock(x, y, z, new BlockAir());
						}
					}
				}
			}
		}
	}
}