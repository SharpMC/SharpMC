using System;
using System.Collections.Generic;

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	public class BiomeManager
	{
		private static readonly List<BiomeBase> _biomes = new List<BiomeBase>();
		private readonly SimplexOctaveGenerator _octaveGeneratorb;

		public BiomeManager(int seed)
		{
			_octaveGeneratorb = new SimplexOctaveGenerator(seed, 1);
			_octaveGeneratorb.SetScale(1/512.0);
		}

		public BiomeBase GetBiome(int x, int z)
		{
			x = (int) Math.Floor((decimal) (x/12.0));
			z = (int) Math.Floor((decimal) (z/12.0));
			var b = (int) Math.Abs(_octaveGeneratorb.Noise(x, z, 1.0, 2.0)*(_biomes.Count + 1));
			if (b >= _biomes.Count) b = _biomes.Count - 1;

			return _biomes[b];
		}

		public void AddBiomeType(BiomeBase biome)
		{
			_biomes.Add(biome);
		}

		public static BiomeBase GetBiomeById(int id, bool minecraftId = true)
		{
			foreach (var b in _biomes)
			{
				if (minecraftId)
				{
					if (b.MinecraftBiomeId == id)
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