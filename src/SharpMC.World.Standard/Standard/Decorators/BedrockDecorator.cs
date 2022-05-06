using SharpMC.API.Worlds;
using SharpMC.World.API.Biomes;
using SharpMC.World.API.Chunks;
using SharpMC.World.API.Noises;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Decorators
{
    public class BedrockDecorator : IChunkDecorator
    {
        private readonly IRandomGenerator _generator;

        public BedrockDecorator(IRandomGenerator generator)
        {
            _generator = generator;
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z)
        {
            for (var y = 1; y < 6; y++)
            {
                if (_generator.GetRandomNumber(0, 5) == 1)
                {
                    chunk.SetBlock(x, y, z, Bedrock);
                }
            }
        }
    }
}