using SharpMC.World.Standard.API;
using SharpMC.Worlds.API;

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