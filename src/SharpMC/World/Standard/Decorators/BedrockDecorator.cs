using SharpMC.World.Noises;
using SharpMC.World.Standard.API;
using SharpMC.Worlds.API;
using static SharpMC.Blocks.KnownBlocks;

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