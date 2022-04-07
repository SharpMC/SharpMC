using System;
using System.Collections.Generic;

namespace SharpMC.Core.Worlds.Standard.BiomeSystem
{
	public class BiomeManager
	{
		private static readonly List<BiomeBase> Biomes = new List<BiomeBase>();
		private readonly SimplexOctaveGenerator _octaveGeneratorb;
		private readonly double _biomeHeigth = 14.5;
		private readonly double _biomeWidth = 12.3;

		public BiomeManager(int seed)
		{
			_octaveGeneratorb = new SimplexOctaveGenerator(seed, 1);
			_octaveGeneratorb.SetScale(1/512.0);
		}

		public BiomeBase GetBiome(int x, int z)
		{
			x = (int) Math.Floor((decimal) (x/_biomeWidth));
			z = (int) Math.Floor((decimal) (z/_biomeHeigth));

			var b = (int) Math.Abs(_octaveGeneratorb.Noise(x, z, 0.5, 0.5)*(Biomes.Count + 1));
			if (b >= Biomes.Count) b = Biomes.Count - 1;
			return Biomes[b];
		}

		public void AddBiomeType(BiomeBase biome)
		{
			Biomes.Add(biome);
		}

		public static BiomeBase GetBiomeById(int id, bool minecraftId = true)
		{
			foreach (var b in Biomes)
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