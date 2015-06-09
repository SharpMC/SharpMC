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
using SharpMC.Core.Worlds.Standard.BiomeSystem;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	public class TreeDecorator : ChunkDecorator
	{
		private void DecorateTrees(ChunkColumn chunk, BiomeBase biome)
		{
			var trees = StandardWorldProvider.GetRandomNumber(biome.MinTrees, biome.MaxTrees);
			var treeBasePositions = new int[trees, 2];

			for (var t = 0; t < trees; t++)
			{
				var x = new Random().Next(1, 16);
				var z = new Random().Next(1, 16);
				treeBasePositions[t, 0] = x;
				treeBasePositions[t, 1] = z;
			}

			for (var y = StandardWorldProvider.WaterLevel + 2; y < 256; y++)
			{
				for (var pos = 0; pos < trees; pos++)
				{
					if (treeBasePositions[pos, 0] < 14 && treeBasePositions[pos, 0] > 4 && treeBasePositions[pos, 1] < 14 &&
					    treeBasePositions[pos, 1] > 4)
					{
						if (chunk.GetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1]) == biome.TopBlock.Id)
						{
							GenerateTree(chunk, treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1], biome);
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

		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			if (x < 15 && x > 3 && z < 15 && z > 3)
			{
				for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
				{
					if (StandardWorldProvider.GetRandomNumber(0, 30) == 5)
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