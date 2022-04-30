using SharpMC.API.Worlds;
using SharpMC.World.API;
using SharpMC.World.Noises;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Standard
{
    internal class CaveGenerator : ICaveGenerator
    {
        private readonly int _seed;

        public CaveGenerator(int seed)
        {
            _seed = seed;
        }

        public void GenerateCave(IChunkColumn chunk, ICoordinates pos)
        {
            var random = new GcRandom(chunk, _seed, pos);
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