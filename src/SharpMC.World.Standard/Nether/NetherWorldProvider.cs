using SharpMC.API.Chunks;
using SharpMC.API.Players;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;
using SharpMC.World.API;
using SharpMC.World.API.Noises;
using SharpMC.World.Noises;
using SharpMC.World.Standard.BiomeSystem;
using SharpMC.World.Standard.Settings;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Nether
{
    internal class NetherWorldProvider : IWorldProvider
    {
        private readonly IRandomGenerator _generator;
        private readonly NetherTweaking _cfg;
        private readonly WorldContext _context;

        public NetherWorldProvider()
        {
            _generator = new RandomGenerator();
            _cfg = new NetherTweaking();
            _context = new WorldContext(_cfg, _generator);
        }

        public void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos)
        {
            var bottom = new SimplexOctaveGenerator(_cfg.Seed.GetHashCode(), 8);
            var top = new SimplexOctaveGenerator(_cfg.Seed.GetHashCode(), 8);
            bottom.SetScale(1 / _cfg.Groundscale);
            top.SetScale(1 / _cfg.Topscale);

            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    float ox = x + pos.X * 16;
                    float oz = z + pos.Z * 16;

                    var bottomHeight = (int) (bottom.Noise(ox, oz, _cfg.BottomsFrequency, _cfg.BottomsAmplitude)
                        * _cfg.BottomsMagnitude + _cfg.BottomOffset);
                    var topHeight = (int) (top.Noise(ox, oz, _cfg.TopFrequency, _cfg.TopAmplitude) *
                        _cfg.TopMagnitude + _cfg.TopOffset);

                    for (var y = 0; y < 256; y++)
                    {
                        if (y == 0 || y == 255)
                        {
                            chunk.SetBlock(x, y, z, Bedrock);
                            continue;
                        }

                        if (y < bottomHeight)
                        {
                            chunk.SetBlock(x, y, z, Netherrack);
                        }

                        if (y < topHeight)
                        {
                            chunk.SetBlock(x, 256 - y, z, Netherrack);
                            if (_generator.GetRandomNumber(1, 50) == 25)
                            {
                                chunk.SetBlock(x, 256 - (y + 1), z, Glowstone);
                            }
                        }
                    }

                    for (var y = bottomHeight; y < 254; y++)
                    {
                        if (chunk.GetBlock(x, y + 1, z) == Air && chunk.GetBlock(x, y, z) == Stone)
                        {
                            chunk.SetBlock(x, y, z, Netherrack);

                            chunk.SetBlock(x, y - 1, z, Netherrack);
                            chunk.SetBlock(x, y - 2, z, Netherrack);
                        }
                    }
                }
            }

            var lava = new NetherLavaDecorator(_context);
            lava.Decorate(chunk, new PlainsBiome(_context));
        }

        public PlayerLocation SpawnPoint => new PlayerLocation(0, 82, 0);
    }
}