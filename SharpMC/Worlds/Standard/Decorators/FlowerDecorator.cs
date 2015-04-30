using SharpMC.Blocks;
using SharpMC.Worlds.Standard.BiomeSystem;

namespace SharpMC.Worlds.Standard.Decorators
{
	public class FlowerDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
			{
				if (StandardWorldProvider.GetRandomNumber(0, 15) == 5)
				{
					if (chunk.GetBlock(x, y, z) == biome.TopBlock.Id)
					{
						var meta = StandardWorldProvider.GetRandomNumber(0, 8);
						chunk.SetBlock(x, y + 1, z, new Block(38) {Metadata = (byte) meta});
					}
				}
			}
		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			/*	var trees = ExperimentalV2Generator.GetRandomNumber(3, 8);
			var treeBasePositions = new int[trees, 2];

			for (var t = 0; t < trees; t++)
			{
				var x = ExperimentalV2Generator.GetRandomNumber(1, 16);
				var z = ExperimentalV2Generator.GetRandomNumber(1, 16);
				treeBasePositions[t, 0] = x;
				treeBasePositions[t, 1] = z;
			}
			for (int y = ExperimentalV2Generator.WaterLevel; y < 256; y++)
			{
				for (var pos = 0; pos < trees; pos++)
				{
					if (chunk.GetBlock(treeBasePositions[pos, 0], y, treeBasePositions[pos, 1]) == biome.TopBlock.Id)
					{
							var meta = ExperimentalV2Generator.GetRandomNumber(0, 8);
							chunk.SetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1],
								new Block(38) {Metadata = (ushort) meta});
					}
				}
			}*/
		}
	}
}