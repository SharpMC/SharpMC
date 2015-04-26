using SharpMC.Worlds.Standard.BiomeSystem;

namespace SharpMC.Worlds.Standard.Decorators
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