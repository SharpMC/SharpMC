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
using SharpMC.Blocks;
using SharpMC.Worlds.Standard.BiomeSystem;

namespace SharpMC.Worlds.Standard.Decorators
{
	public class FlowerDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			for (var y = StandardWorldProvider.WaterLevel; y < 256; y++)
			{
				if (StandardWorldProvider.GetRandomNumber(0, 15) == 5)
				{
					if (chunk.GetBlock(x, y, z) == biome.TopBlock.Id)
					{
						var meta = StandardWorldProvider.GetRandomNumber(0, 8);
						chunk.SetBlock(x, y + 1, z, new Block(38) {Metadata = (byte) meta});
					}
				}
			}
		}

		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			/*	var trees = ExperimentalV2Generator.GetRandomNumber(3, 8);
			var treeBasePositions = new int[trees, 2];

			for (var t = 0; t < trees; t++)
			{
				var x = ExperimentalV2Generator.GetRandomNumber(1, 16);
				var z = ExperimentalV2Generator.GetRandomNumber(1, 16);
				treeBasePositions[t, 0] = x;
				treeBasePositions[t, 1] = z;
			}
			for (int y = ExperimentalV2Generator.WaterLevel; y < 256; y++)
			{
				for (var pos = 0; pos < trees; pos++)
				{
					if (chunk.GetBlock(treeBasePositions[pos, 0], y, treeBasePositions[pos, 1]) == biome.TopBlock.Id)
					{
							var meta = ExperimentalV2Generator.GetRandomNumber(0, 8);
							chunk.SetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1],
								new Block(38) {Metadata = (ushort) meta});
					}
				}
			}*/
		}
	}
}