using SharpMC.World.Standard.API;
using SharpMC.Worlds.API;
using static SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Decorators
{
    public class WaterDecorator : IChunkDecorator
    {
        private readonly IWorldContext _context;

        public WaterDecorator(IWorldContext context)
        {
            _context = context;
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase _)
        {
            var waterLevel = _context.Settings.WaterLevel;
            var generator = _context.Random;

            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (var y = 0; y < waterLevel; y++)
                    {
                        if (y < waterLevel)
                        {
                            if (chunk.GetBlock(x, y, z) == Grass
                                || chunk.GetBlock(x, y, z) == Dirt)
                            {
                                if (generator.GetRandomNumber(1, 40) == 1
                                    && y < waterLevel - 4)
                                    chunk.SetBlock(x, y, z, Clay);
                                else
                                {
                                    chunk.SetBlock(x, y, z, Sand);
                                    chunk.SetBiomeId(x * 16 + z, BiomeIds.Beach);
                                }
                            }
                            if (chunk.GetBlock(x, y + 1, z) == Air)
                            {
                                if (y < waterLevel - 3)
                                {
                                    chunk.SetBlock(x, y + 1, z, Water);
                                    chunk.SetBiomeId(x * 16 + z, BiomeIds.Ocean);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z)
        {
            Decorate(chunk, biome);
        }
    }
}