using SharpMC.API.Chunks;
using SharpMC.API.Worlds;

namespace SharpMC.World.API.Chunks
{
    public interface IChunkRepository
    {
        IChunkColumn LoadChunk(ChunkCoordinates pos);

        void SaveChunk(IChunkColumn chunk, ChunkCoordinates pos);

        bool Exists(ChunkCoordinates pos);
    }
}