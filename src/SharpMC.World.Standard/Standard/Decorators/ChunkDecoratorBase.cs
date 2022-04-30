using SharpMC.API.Worlds;
using SharpMC.World.API.Biomes;
using SharpMC.World.API.Chunks;

namespace SharpMC.World.Standard.Decorators
{
    public abstract class ChunkDecoratorBase : IChunkDecorator
    {
        public abstract void Decorate(IChunkColumn chunk, IBiomeBase biome);

        public virtual void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z)
        {
            Decorate(chunk, biome);
        }
    }
}