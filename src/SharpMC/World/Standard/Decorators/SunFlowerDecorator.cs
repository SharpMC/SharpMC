using SharpMC.World.Standard.API;
using SharpMC.Worlds.API;
using static SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Decorators
{
    public class SunFlowerDecorator : IChunkDecorator
    {
        private readonly IWorldContext _context;

        public SunFlowerDecorator(IWorldContext context)
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
                        chunk.SetBlock(x, y + 1, z, Sunflower);
                        chunk.SetBlock(x, y + 2, z, Lilac);
                    }
                }
            }
        }
    }
}