using SharpMC.Players;
using SharpMC.Util;
using SharpMC.World.Generators;
using System;
using SharpMC.Noise.API;
using SharpMC.World.API;
using SharpMC.World.Noises;
using SharpMC.World.Standard.API;
using SharpMC.World.Standard.BiomeSystem;
using SharpMC.World.Standard.Decorators;
using SharpMC.World.Standard.Settings;
using static SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Standard
{
    internal class StandardWorldProvider : IWorldProvider
    {
        private readonly ICaveGenerator _caveGen;
        private readonly IBiomeManager _biomeManager;
        private readonly WorldTweaking _settings;
        private readonly IRandomGenerator _generator;
        private readonly WorldContext _context;
        private readonly NoiseCreator _creator;

        public StandardWorldProvider(NoiseCreator creator, RandomCreator rc)
        {
            _creator = creator;
            _generator = new RandomGenerator();
            _settings = new WorldTweaking();
            _context = new WorldContext(_settings, _generator);
            _caveGen = new CaveGenerator(_settings.Seed.GetHashCode(), rc);
            _biomeManager = new BiomeManager(creator, _settings.Seed.GetHashCode());
            _biomeManager.AddBiomeType(new FlowerForestBiome(_context));
            _biomeManager.AddBiomeType(new ForestBiome(_context));
            _biomeManager.AddBiomeType(new BirchForestBiome(_context));
            _biomeManager.AddBiomeType(new PlainsBiome(_context));
            _biomeManager.AddBiomeType(new DesertBiome(_context));
            _biomeManager.AddBiomeType(new SunFlowerPlainsBiome(_context));
        }

        public void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos)
        {
            var bottom = _creator(_settings.Seed.GetHashCode(), 8);
            var overhang = _creator(_settings.Seed.GetHashCode(), 8);
            overhang.SetScale(1 / _settings.OverhangScale);
            bottom.SetScale(1 / _settings.Groundscale);

            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    float ox = x + pos.X * 16;
                    float oz = z + pos.Z * 16;

                    var cBiome = _biomeManager.GetBiome((int) ox, (int) oz);
                    chunk.SetBiomeId(x * 16 + z, cBiome.MinecraftBiomeId);

                    var bottomHeight = (int) (bottom.Noise(ox, oz, _settings.BottomsFrequency,
                                                  _settings.BottomsAmplitude) * _settings.BottomsMagnitude +
                                              _settings.BottomOffset + cBiome.BaseHeight);

                    var maxHeight = (int) (overhang.Noise(ox, oz, _settings.OverhangFrequency,
                                               _settings.OverhangAmplitude) * _settings.OverhangsMagnitude +
                                           bottomHeight + _settings.OverhangOffset);
                    maxHeight = Math.Max(1, maxHeight);

                    for (var y = 0; y < maxHeight && y < 256; y++)
                    {
                        if (y == 0)
                        {
                            chunk.SetBlock(x, y, z, Bedrock);
                            continue;
                        }

                        if (y > bottomHeight)
                        {
                            if (_settings.EnableOverhang)
                            {
                                var density = overhang.Noise(ox, y, oz,
                                    _settings.OverhangFrequency, _settings.OverhangAmplitude);
                                if (density > _settings.Threshold)
                                    chunk.SetBlock(x, y, z, Stone);
                            }
                        }
                        else
                        {
                            chunk.SetBlock(x, y, z, Stone);
                        }
                    }

                    for (var y = 0; y < 256; y++)
                    {
                        if (chunk.GetBlock(x, y + 1, z) == Air && chunk.GetBlock(x, y, z) == Stone)
                        {
                            chunk.SetBlock(x, y, z, cBiome.TopBlock);

                            chunk.SetBlock(x, y - 1, z, cBiome.Filling);
                            chunk.SetBlock(x, y - 2, z, cBiome.Filling);
                        }
                    }

                    foreach (var decorator in cBiome.Decorators)
                    {
                        decorator.Decorate(chunk, cBiome, x, z);
                    }
                    var ores = new OreDecorator(_generator);
                    ores.Decorate(chunk, cBiome, x, z);
                    var bedrock = new BedrockDecorator(_generator);
                    bedrock.Decorate(chunk, cBiome, x, z);
                }
            }

            var water = new WaterDecorator(_context);
            water.Decorate(chunk, new PlainsBiome(_context));
            _caveGen.GenerateCave(chunk, pos);
            var lava = new LavaDecorator();
            lava.Decorate(chunk, new PlainsBiome(_context));
        }

        public PlayerLocation SpawnPoint => new(0, 82, 0);
    }
}