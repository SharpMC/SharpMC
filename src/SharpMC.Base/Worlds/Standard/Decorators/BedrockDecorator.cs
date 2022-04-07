using SharpMC.Blocks;
using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	public class BedrockDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			for (var y = 1; y < 6; y++)
			{
				if (StandardWorldProvider.GetRandomNumber(0, 5) == 1)
				{
					chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(7));
				}
			}
		}
	}
}