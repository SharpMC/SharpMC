using SharpMC.Util;
using SharpMC.World.API;
using SharpMC.World.Standard.API;
using static SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Standard
{
    internal class CaveGenerator : ICaveGenerator
    {
        private readonly int _seed;
        private readonly RandomCreator _creator;

        public CaveGenerator(int seed, RandomCreator creator)
        {
            _creator = creator;
            _seed = seed;
        }

        public void GenerateCave(IChunkColumn chunk, ChunkCoordinates pos)
        {
            var random = _creator(_seed, (pos.X, pos.Z));
            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (var y = 50; y >= 6; y--)
                    {
                        if (random.IsInCave(x, y, z))
                        {
                            chunk.SetBlock(x, y, z, Air);
                        }
                    }
                }
            }
        }
    }
}