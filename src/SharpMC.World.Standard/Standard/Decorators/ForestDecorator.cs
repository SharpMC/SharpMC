using SharpMC.API.Worlds;
using SharpMC.World.API.Biomes;
using SharpMC.World.API.Chunks;
using SharpMC.World.Standard.API;

namespace SharpMC.World.Standard.Decorators
{
    public class ForestDecorator : IChunkDecorator
    {
        private readonly IWorldContext _context;

        public ForestDecorator(IWorldContext context)
        {
            _context = context;
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z)
        {
            var waterLevel = _context.Settings.WaterLevel;
            var generator = _context.Random;

            if (x < 15 && x > 3 && z < 15 && z > 3)
            {
                for (var y = waterLevel; y < 256; y++)
                {
                    if (generator.GetRandomNumber(0, 13) == 5)
                    {
                        if (chunk.GetBlock(x, y + 1, z).Equals(biome.TopBlock))
                        {
                            GenerateTree(chunk, x, y + 1, z, biome);
                        }
                    }
                }
            }
        }

        private void GenerateTree(IChunkColumn chunk, int x, int treebase, int z,
            IBiomeBase biome)
        {
            var generator = _context.Random;

            if (biome.TreeStructures.Length > 0)
            {
                var value = generator.GetRandomNumber(0, biome.TreeStructures.Length);
                biome.TreeStructures[value].Create(chunk, x, treebase, z);
            }
        }

        private void DecorateTrees(IChunkColumn chunk, IBiomeBase biome)
        {
            var waterLevel = _context.Settings.WaterLevel;
            var generator = _context.Random;

            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (var y = waterLevel; y < 256; y++)
                    {
                        if (generator.GetRandomNumber(0, 13) == 5)
                        {
                            if (x < 15 && x > 3 && z < 15 && z > 3)
                            {
                                if (chunk.GetBlock(x, y + 1, z).Equals(biome.TopBlock))
                                {
                                    GenerateTree(chunk, x, y + 1, z, biome);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}