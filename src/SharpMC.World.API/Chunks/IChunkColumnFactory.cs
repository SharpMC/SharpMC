using SharpMC.API.Worlds;

namespace SharpMC.World.API.Chunks
{
    public interface IChunkColumnFactory
    {
        IChunkColumn CreateColumn();
    }
}