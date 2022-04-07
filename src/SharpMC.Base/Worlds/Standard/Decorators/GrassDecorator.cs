using SharpMC.Blocks;
using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	public class GrassDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
			{
				if (StandardWorldProvider.GetRandomNumber(0, 10) == 5)
				{
					if (chunk.GetBlock(x, y, z) == biome.TopBlock.Id)
					{
						chunk.SetBlock(x, y + 1, z, new Block(31) {Metadata = 1});
					}
				}
			}
		}
	}
}