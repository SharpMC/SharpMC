using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	public class ChunkDecorator
	{
		public virtual void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
		}

		public virtual void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			Decorate(chunk, biome);
		}
	}
}