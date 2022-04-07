using SharpMC.Blocks;
using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	public class WaterDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					//Check for temperature.
					for (var y = 0; y < StandardWorldProvider.WaterLevel; y++)
					{
						//Lake generation
						if (y < StandardWorldProvider.WaterLevel)
						{
							if (chunk.GetBlock(x, y, z) == 2 || chunk.GetBlock(x, y, z) == 3) //Grass or Dirt?
							{
								if (StandardWorldProvider.GetRandomNumber(1, 40) == 1 && y < StandardWorldProvider.WaterLevel - 4)
									chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(82)); //Clay
								else
								{
									chunk.SetBlock(x, y, z, new BlockSand()); //Sand
									chunk.BiomeId[x*16 + z] = 16; //Beach
								}
							}
							if (chunk.GetBlock(x, y + 1, z) == 0)
							{
								if (y < StandardWorldProvider.WaterLevel - 3)
								{
									chunk.SetBlock(x, y + 1, z, new BlockFlowingWater()); //Water
									chunk.BiomeId[x*16 + z] = 0; //Ocean
								}
							}
						}
					}
				}
			}
		}
	}
}