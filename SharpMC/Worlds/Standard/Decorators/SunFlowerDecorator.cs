using SharpMC.Blocks;
using SharpMC.Worlds.Standard.BiomeSystem;

namespace SharpMC.Worlds.Standard.Decorators
{
	public class SunFlowerDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
			{
				if (StandardWorldProvider.GetRandomNumber(0, 15) == 5)
				{
					if (chunk.GetBlock(x, y, z) == biome.TopBlock.Id)
					{
						//var meta = ExperimentalV2Generator.GetRandomNumber(0, 8);
						chunk.SetBlock(x, y + 1, z, new Block(175) {Metadata = 0});
						chunk.SetBlock(x, y + 2, z, new Block(175) {Metadata = 1});
					}
				}
			}
		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
		}
	}
}