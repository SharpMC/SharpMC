using System;
using System.Collections.Generic;
using SharpMC.World.API;
using SharpMC.World.Noises;
using SharpMC.World.Standard.API;

namespace SharpMC.World.Standard.BiomeSystem
{
    public class BiomeManager : IBiomeManager
    {
        private readonly SimplexOctaveGenerator _octaveGenerator;
        private readonly List<IBiomeBase> _biomes;
        private readonly double _biomeHeight;
        private readonly double _biomeWidth;

        public BiomeManager(int seed, double biomeHeight = 14.5, double biomeWidth = 12.3)
        {
            _biomeHeight = biomeHeight;
            _biomeWidth = biomeWidth;
            _biomes = new List<IBiomeBase>();

            _octaveGenerator = new SimplexOctaveGenerator(seed, 1);
            _octaveGenerator.SetScale(1 / 512.0);
        }

        public void AddBiomeType(IBiomeBase biome)
        {
            _biomes.Add(biome);
        }

        public IBiomeBase GetBiome(int x, int z)
        {
            x = (int) Math.Floor((decimal) (x / _biomeWidth));
            z = (int) Math.Floor((decimal) (z / _biomeHeight));

            var b = (int) Math.Abs(_octaveGenerator.Noise(x, z, 0.5, 0.5) * (_biomes.Count + 1));
            if (b >= _biomes.Count) b = _biomes.Count - 1;
            return _biomes[b];
        }

        public IBiomeBase GetBiomeById(int id, bool minecraftId = true)
        {
            foreach (var b in _biomes)
            {
                if (minecraftId)
                {
                    if (b.MinecraftBiomeId == (BiomeIds) id)
                    {
                        return b;
                    }
                }
                else
                {
                    if (b.Id == id)
                    {
                        return b;
                    }
                }
            }
            return null;
        }
    }
}