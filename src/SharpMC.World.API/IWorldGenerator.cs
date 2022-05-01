using SharpMC.API.Chunks;
using SharpMC.API.Worlds;

namespace SharpMC.World.API
{
    public interface IWorldGenerator : IWorldProvider
    {
        IChunkColumn GenerateChunkColumn(ChunkCoordinates coordinates);
    }
}