using SharpMC.Util;
using SharpMC.World;

namespace SharpMC.Storage.API
{
    public interface IChunkRepository
    {
        IChunkColumn LoadChunk(ChunkCoordinates pos);

        void SaveChunk(IChunkColumn chunk, ChunkCoordinates pos);

        bool Exists(ChunkCoordinates pos);
    }
}