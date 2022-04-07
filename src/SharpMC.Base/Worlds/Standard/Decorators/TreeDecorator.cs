using System;
using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	public class TreeDecorator : ChunkDecorator
	{
		private void DecorateTrees(ChunkColumn chunk, BiomeBase biome)
		{
			var trees = StandardWorldProvider.GetRandomNumber(biome.MinTrees, biome.MaxTrees);
			var treeBasePositions = new int[trees, 2];

			for (var t = 0; t < trees; t++)
			{
				var x = new Random().Next(1, 16);
				var z = new Random().Next(1, 16);
				treeBasePositions[t, 0] = x;
				treeBasePositions[t, 1] = z;
			}

			for (var y = StandardWorldProvider.WaterLevel + 2; y < 256; y++)
			{
				for (var pos = 0; pos < trees; pos++)
				{
					if (treeBasePositions[pos, 0] < 14 && treeBasePositions[pos, 0] > 4 && treeBasePositions[pos, 1] < 14 &&
					    treeBasePositions[pos, 1] > 4)
					{
						if (chunk.GetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1]) == biome.TopBlock.Id)
						{
							GenerateTree(chunk, treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1], biome);
						}
					}
				}
			}
		}

		private void GenerateTree(ChunkColumn chunk, int x, int treebase, int z, BiomeBase biome)
		{
			if (biome.TreeStructures.Length > 0)
			{
				var value = StandardWorldProvider.GetRandomNumber(0, biome.TreeStructures.Length);
				biome.TreeStructures[value].Create(chunk, x, treebase, z);
			}
		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{

		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			if (x < 15 && x > 3 && z < 15 && z > 3)
			{
				for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
				{
					if (StandardWorldProvider.GetRandomNumber(0, 30) == 5)
					{
						if (chunk.GetBlock(x, y + 1, z) == biome.TopBlock.Id)
						{
							GenerateTree(chunk, x, y + 1, z, biome);
						}
					}
				}
			}
		}
	}
}