using SharpMC.API.Chunks;
using SharpMC.API.Worlds;

namespace SharpMC.World.API
{
    public interface ICaveGenerator
    {
        void GenerateCave(IChunkColumn chunk, ChunkCoordinates pos);
    }
}