using SharpMC.API.Worlds;
using SharpMC.World.API.Biomes;
using SharpMC.World.API.Chunks;
using SharpMC.World.Standard.API;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Decorators
{
    public class GrassDecorator : IChunkDecorator
    {
        private readonly IWorldContext _context;

        public GrassDecorator(IWorldContext context)
        {
            _context = context;
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z)
        {
            var waterLevel = _context.Settings.WaterLevel;
            var generator = _context.Random;

            for (var y = waterLevel; y < 256; y++)
            {
                if (generator.GetRandomNumber(0, 10) == 5)
                {
                    if (chunk.GetBlock(x, y, z).Equals(biome.TopBlock))
                    {
                        chunk.SetBlock(x, y + 1, z, TallGrass);
                    }
                }
            }
        }
    }
}