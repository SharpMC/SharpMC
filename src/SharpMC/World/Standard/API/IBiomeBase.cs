using SharpMC.Blocks;
using SharpMC.Worlds.API;

namespace SharpMC.World.Standard.API
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