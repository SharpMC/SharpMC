using SharpMC.API.Worlds;
using SharpMC.World.API.Biomes;
using SharpMC.World.API.Chunks;
using SharpMC.World.Standard.API;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Nether
{
    public class NetherLavaDecorator : IChunkDecorator
    {
        private readonly IWorldContext _context;

        public NetherLavaDecorator(IWorldContext context)
        {
            _context = context;
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase biome, int sx, int sz)
        {
            Decorate(chunk, biome);
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase _)
        {
            var waterLevel = _context.Settings.WaterLevel;

            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (var y = 0; y < waterLevel; y++)
                    {
                        if (y < waterLevel)
                        {
                            if (chunk.GetBlock(x, y + 1, z).Equals(Air))
                            {
                                if (y < waterLevel - 3)
                                {
                                    chunk.SetBlock(x, y + 1, z, Lava);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}