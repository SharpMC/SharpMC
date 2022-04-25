using SharpMC.Util;

namespace SharpMC.World.API
{
    public interface ICaveGenerator
    {
        void GenerateCave(IChunkColumn chunk, ChunkCoordinates pos);
    }
}