using SharpMC.Blocks;
using SharpMC.World.Standard.API;
using SharpMC.Worlds.API;

namespace SharpMC.World.Standard.Decorators
{
    public class FlowerDecorator : IChunkDecorator
    {
        private readonly IWorldContext _context;

        public FlowerDecorator(IWorldContext context)
        {
            _context = context;
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z)
        {
            var waterLevel = _context.Settings.WaterLevel;
            var generator = _context.Random;

            for (var y = waterLevel; y < 256; y++)
            {
                if (generator.GetRandomNumber(0, 15) == 5)
                {
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock)
                    {
                        var meta = generator.GetRandomNumber(0, 8);
                        var block = KnownBlocks.FloweringAzalea + meta;
                        chunk.SetBlock(x, y + 1, z, block);
                    }
                }
            }
        }
    }
}