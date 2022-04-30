using SharpMC.API.Worlds;

namespace SharpMC.World.API.Chunks
{
    public interface IChunkRepository
    {
        IChunkColumn LoadChunk(ICoordinates pos);

        void SaveChunk(IChunkColumn chunk, ICoordinates pos);

        bool Exists(ICoordinates pos);
    }
}