using System;
using SharpMC.World.Standard.API;
using SharpMC.Worlds.API;

namespace SharpMC.World.Standard.Decorators
{
    public class TreeDecorator : IChunkDecorator
    {
        private readonly IWorldContext _context;

        public TreeDecorator(IWorldContext context)
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
                    if (generator.GetRandomNumber(0, 30) == 5)
                    {
                        if (chunk.GetBlock(x, y + 1, z) == biome.TopBlock)
                        {
                            GenerateTree(chunk, x, y + 1, z, biome);
                        }
                    }
                }
            }
        }

        private void GenerateTree(IChunkColumn chunk, int x, int treebase, int z, IBiomeBase biome)
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

            var trees = generator.GetRandomNumber(biome.MinTrees, biome.MaxTrees);
            var treeBasePositions = new int[trees, 2];
            for (var t = 0; t < trees; t++)
            {
                var x = new Random().Next(1, 16);
                var z = new Random().Next(1, 16);
                treeBasePositions[t, 0] = x;
                treeBasePositions[t, 1] = z;
            }
            for (var y = waterLevel + 2; y < 256; y++)
            {
                for (var pos = 0; pos < trees; pos++)
                {
                    if (treeBasePositions[pos, 0] < 14 && treeBasePositions[pos, 0] > 4 &&
                        treeBasePositions[pos, 1] < 14 &&
                        treeBasePositions[pos, 1] > 4)
                    {
                        if (chunk.GetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1]) ==
                            biome.TopBlock)
                        {
                            GenerateTree(chunk, treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1], biome);
                        }
                    }
                }
            }
        }
    }
}