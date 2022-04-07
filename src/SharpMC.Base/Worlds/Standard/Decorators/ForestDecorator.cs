using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	/// <summary>
	///     Decorater only for the ForestBiome...
	///     Could be used for other forests tho :p
	/// </summary>
	public class ForestDecorator : ChunkDecorator
	{
		private void DecorateTrees(ChunkColumn chunk, BiomeBase biome)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
					{
						if (StandardWorldProvider.GetRandomNumber(0, 13) == 5)
						{
							//The if is so we don't have 'fucked up' trees xD
							if (x < 15 && x > 3 && z < 15 && z > 3)
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
					if (StandardWorldProvider.GetRandomNumber(0, 13) == 5)
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