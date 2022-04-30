using SharpMC.API.Worlds;
using SharpMC.World.API.Biomes;

namespace SharpMC.World.API.Chunks
{
    public interface IChunkDecorator
    {
        void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z);
    }
}