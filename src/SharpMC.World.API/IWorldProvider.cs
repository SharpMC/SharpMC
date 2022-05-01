using SharpMC.API.Chunks;
using SharpMC.API.Players;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.World.API
{
    public interface IWorldProvider
    {
        void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos);

        PlayerLocation SpawnPoint { get; }
    }
}