using SharpMC.Blocks;
using SharpMC.Worlds.Standard.BiomeSystem;

namespace SharpMC.Worlds.Standard.Decorators
{
	public class GrassDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			/*for (int x = 0; x < 16; x++)
			{
				for (int z = 0; z < 16; z++)
				{
					for (int y = 0; y < 256; y++)
					{
						if (chunk.GetBlock(x, y, z) == chunk.Biome.TopBlock.Id)
						{
							if (ExperimentalV2Generator.GetRandomNumber(0, 5) == 2)
							{
								if (ExperimentalV2Generator.GetRandomNumber(0, 50) == 10)
								{
									chunk.SetBlock(x, y + 1, z, new Block(175) {Metadata = 2});
									chunk.SetBlock(x, y + 2, z, new Block(175) {Metadata = 2});
								}
								else
								{
									chunk.SetBlock(x, y + 1, z, new Block(31) {Metadata = 1});
								}
							}
						}
					}
				}
			var trees = ExperimentalV2Generator.GetRandomNumber(10, 30);
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
						if (ExperimentalV2Generator.GetRandomNumber(0, 5) == 2)
						{
							//if (ExperimentalV2Generator.GetRandomNumber(0, 50) == 10)
							//{
							//	chunk.SetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1], new Block(175) { Metadata = 2 });
							//	chunk.SetBlock(treeBasePositions[pos, 0], y + 2, treeBasePositions[pos, 1], new Block(175) { Metadata = 2 });
							//}
							//else
							//{
								chunk.SetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1], new Block(31) { Metadata = 1 });
							//}
						}
					}
				}
			}
			 */

			//for (int y = ExperimentalV2Generator.WaterLevel; y < 256; y++)
			//{
			//	if (chunk.GetBlock(x, y, z) == biome.TopBlock.Id)
			//	{
			//	}
			//}
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