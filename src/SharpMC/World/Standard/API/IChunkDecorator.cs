using SharpMC.World;
using SharpMC.World.Standard.API;

namespace SharpMC.Worlds.API
{
    public interface IChunkDecorator
    {
        void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z);
    }
}