// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015
using System;
using System.Collections.Generic;

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	public class BiomeManager
	{
		private static readonly List<BiomeBase> _biomes = new List<BiomeBase>();
		private readonly SimplexOctaveGenerator _octaveGeneratorb;
		private readonly double BiomeHeigth = 14.5;
		private readonly double BiomeWidth = 12.3;

		public BiomeManager(int seed)
		{
			_octaveGeneratorb = new SimplexOctaveGenerator(seed, 1);
			_octaveGeneratorb.SetScale(1/512.0);
		}

		public BiomeBase GetBiome(int x, int z)
		{
			x = (int) Math.Floor((decimal) (x/BiomeWidth));
			z = (int) Math.Floor((decimal) (z/BiomeHeigth));

			var b = (int) Math.Abs(_octaveGeneratorb.Noise(x, z, 0.5, 0.5)*(_biomes.Count + 1));
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