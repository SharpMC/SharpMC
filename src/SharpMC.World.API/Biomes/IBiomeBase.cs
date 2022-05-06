using SharpMC.API.Blocks;
using SharpMC.API.Worlds;
using SharpMC.World.API.Chunks;
using SharpMC.World.API.Structures;

namespace SharpMC.World.API.Biomes
{
    public interface IBiomeBase
    {
        int Id { get; }
        BiomeIds MinecraftBiomeId { get; }
        IBlock TopBlock { get; }
        IBlock Filling { get; }
        IChunkDecorator[] Decorators { get; }
        IStructure[] TreeStructures { get; }
        int MinTrees { get; }
        int MaxTrees { get; }
        double BaseHeight { get; }
    }
}