using SharpMC.Blocks;
using SharpMC.Worlds.Standard.Decorators;
using SharpMC.Worlds.Standard.Structures;

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	public interface IBiome
	{
		int Id { get; }
		byte MinecraftBiomeId { get; }
		int MaxTrees { get; }
		int MinTrees { get; }
		Structure[] TreeStructures { get; }
		ChunkDecorator[] Decorators { get; }
		float Temperature { get; }
		Block TopBlock { get; }
		Block Filling { get; }
	}
}