using SharpMC.Blocks;
using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	/// <summary>
	///     Lava decorator.
	/// </summary>
	public class LavaDecorator : ChunkDecorator
	{
		/// <summary>
		///     The lava level
		/// </summary>
		public int LavaLevel = 13;

		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					for (var y = 0; y < LavaLevel; y++)
					{
						//Lake generation
						if (y < LavaLevel)
						{
							if (chunk.GetBlock(x, y + 1, z) == 0)
							{
								chunk.SetBlock(x, y + 1, z, BlockFactory.GetBlockById(10)); //Lava
							}
						}
					}
				}
			}
		}
	}
}