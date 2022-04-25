using SharpMC.Util;

namespace SharpMC.World.Generators
{
    public interface IWorldGenerator : IWorldProvider
    {
        IChunkColumn GenerateChunkColumn(ChunkCoordinates coordinates);
    }
}