using SharpMC.World.Noises;
using SharpMC.World.Standard.API;
using SharpMC.Worlds.API;
using static SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Decorators
{
    public class OreDecorator : IChunkDecorator
    {
        private readonly IRandomGenerator _generator;

        public OreDecorator(IRandomGenerator generator)
        {
            _generator = generator;
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase biome, int x, int z)
        {
            for (var y = 1; y < 129; y++)
            {
                if (chunk.GetBlock(x, y, z) == Stone)
                {
                    if (y < 128)
                        GenerateCoal(chunk, x, y, z);
                    if (y < 64)
                        GenerateIron(chunk, x, y, z);
                    if (y < 29)
                        GenerateGold(chunk, x, y, z);
                    if (y < 23)
                        GenerateLapis(chunk, x, y, z);
                    if (y < 16)
                    {
                        if (y > 12)
                        {
                            if (_generator.GetRandomNumber(0, 3) == 2)
                                GenerateDiamond(chunk, x, y, z);
                        }
                        else
                        {
                            GenerateDiamond(chunk, x, y, z);
                        }
                    }
                }
            }
        }

        public void GenerateCoal(IChunkColumn chunk, int x, int y, int z)
        {
            if (_generator.GetRandomNumber(0, 35) == 1)
            {
                chunk.SetBlock(x, y, z, CoalOre);
            }
        }

        public void GenerateIron(IChunkColumn chunk, int x, int y, int z)
        {
            if (_generator.GetRandomNumber(0, 65) == 1)
            {
                chunk.SetBlock(x, y, z, IronOre);
            }
        }

        public void GenerateGold(IChunkColumn chunk, int x, int y, int z)
        {
            if (_generator.GetRandomNumber(0, 80) == 1)
            {
                chunk.SetBlock(x, y, z, GoldOre);
            }
        }

        public void GenerateLapis(IChunkColumn chunk, int x, int y, int z)
        {
            if (_generator.GetRandomNumber(0, 80) == 1)
            {
                chunk.SetBlock(x, y, z, LapisOre);
            }
        }

        public void GenerateDiamond(IChunkColumn chunk, int x, int y, int z)
        {
            if (_generator.GetRandomNumber(0, 130) == 1)
            {
                chunk.SetBlock(x, y, z, DiamondOre);
            }
        }
    }
}