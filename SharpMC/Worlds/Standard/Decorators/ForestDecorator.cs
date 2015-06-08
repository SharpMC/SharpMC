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
using SharpMC.Worlds.Standard.BiomeSystem;

namespace SharpMC.Worlds.Standard.Decorators
{
	/// <summary>
	///     Decorater only for the ForestBiome...
	///     Could be used for other forests tho :p
	/// </summary>
	public class ForestDecorator : ChunkDecorator
	{
		private void DecorateTrees(ChunkColumn chunk, BiomeBase biome)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
					{
						if (StandardWorldProvider.GetRandomNumber(0, 13) == 5)
						{
							//The if is so we don't have 'fucked up' trees xD
							if (x < 15 && x > 3 && z < 15 && z > 3)
							{
								if (chunk.GetBlock(x, y + 1, z) == biome.TopBlock.Id)
								{
									GenerateTree(chunk, x, y + 1, z, biome);
								}
							}
						}
					}
				}
			}
		}

		private void GenerateTree(ChunkColumn chunk, int x, int treebase, int z, BiomeBase biome)
		{
			if (biome.TreeStructures.Length > 0)
			{
				var value = StandardWorldProvider.GetRandomNumber(0, biome.TreeStructures.Length);
				biome.TreeStructures[value].Create(chunk, x, treebase, z);
			}
		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			//DecorateTrees(chunk, biome);
		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			if (x < 15 && x > 3 && z < 15 && z > 3)
			{
				for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
				{
					if (StandardWorldProvider.GetRandomNumber(0, 13) == 5)
					{
						if (chunk.GetBlock(x, y + 1, z) == biome.TopBlock.Id)
						{
							GenerateTree(chunk, x, y + 1, z, biome);
						}
					}
				}
			}
		}
	}
}