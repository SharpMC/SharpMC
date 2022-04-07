using SharpMC.Blocks;
using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.Core.Worlds.Standard.Decorators;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Nether
{
	public class NetherLavaDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					//Check for temperature.
					for (var y = 0; y < NetherWorldProvider.WaterLevel; y++)
					{
						//Lake generation
						if (y < NetherWorldProvider.WaterLevel)
						{
							if (chunk.GetBlock(x, y + 1, z) == 0)
							{
								if (y < NetherWorldProvider.WaterLevel - 3)
								{
									chunk.SetBlock(x, y + 1, z, new BlockStationaryLava()); //Water
								}
							}
						}
					}
				}
			}
		}
	}
}