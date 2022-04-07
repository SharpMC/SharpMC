using SharpMC.Blocks;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard
{
	internal class CaveGenerator
	{
		private readonly int _seedi;
		private GcRandom _gcRandom;

		public CaveGenerator(int seed)
		{
			_seedi = seed;
		}

		public void GenerateCave(ChunkColumn chunk)
		{
			_gcRandom = new GcRandom(chunk, _seedi);
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					for (var y = 50; y >= 6; y--)
					{
						if (_gcRandom.IsInCave(x, y, z))
						{
							chunk.SetBlock(x, y, z, new BlockAir());
						}
					}
				}
			}
		}
	}
}