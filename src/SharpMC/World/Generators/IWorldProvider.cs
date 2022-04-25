using SharpMC.Players;
using SharpMC.Util;

namespace SharpMC.World.Generators
{
    public interface IWorldProvider
    {
        void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos);

        PlayerLocation SpawnPoint { get; }
    }
}