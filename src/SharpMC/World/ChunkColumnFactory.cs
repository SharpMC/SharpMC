using SharpMC.API.Worlds;
using SharpMC.World.API.Chunks;

namespace SharpMC.World
{
    internal sealed class ChunkColumnFactory : IChunkColumnFactory
    {
        public IChunkColumn CreateColumn()
        {
            return new ChunkColumn();
        }
    }
}