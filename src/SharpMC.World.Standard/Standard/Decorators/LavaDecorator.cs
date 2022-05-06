﻿using SharpMC.API.Worlds;
using SharpMC.World.API.Biomes;
using SharpMC.World.API.Chunks;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Decorators
{
    public class LavaDecorator : IChunkDecorator
    {
        private readonly int _lavaLevel;

        public LavaDecorator(int lavaLevel = 13)
        {
            _lavaLevel = lavaLevel;
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase biome, int sx, int sz)
        {
            Decorate(chunk, biome);
        }

        public void Decorate(IChunkColumn chunk, IBiomeBase _)
        {
            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (var y = 0; y < _lavaLevel; y++)
                    {
                        if (y < _lavaLevel)
                        {
                            if (chunk.GetBlock(x, y + 1, z).Equals(Air))
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