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

using SharpMC.Core.Blocks;
using SharpMC.Core.Worlds.Standard.BiomeSystem;

namespace SharpMC.Core.Worlds.Standard.Decorators
{
	public class OreDecorator : ChunkDecorator
	{
		private ChunkColumn _chunke;

		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			_chunke = chunk;
			for (var y = 1 /*No need to check first layer :p*/; y < 128 /*Nothing above this*/; y++)
			{
				if (chunk.GetBlock(x, y, z) == 1)
				{
					if (y < 128)
					{
						GenerateCoal(x, y, z);
					}

					if (y < 64)
					{
						GenerateIron(x, y, z);
					}

					if (y < 29)
					{
						GenerateGold(x, y, z);
					}

					if (y < 23)
					{
						GenerateLapis(x, y, z);
					}

					if (y < 16)
					{
						if (y > 12)
						{
							if (StandardWorldProvider.GetRandomNumber(0, 3) == 2)
							{
								GenerateDiamond(x, y, z);
							}
						}
						else
						{
							GenerateDiamond(x, y, z);
						}
					}
				}
			}
		}

		public void GenerateCoal(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 35) == 1)
			{
				_chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(16));
			}
		}

		public void GenerateIron(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 65) == 1)
			{
				_chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(15));
			}
		}

		public void GenerateGold(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 80) == 1)
			{
				_chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(14));
			}
		}

		public void GenerateDiamond(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 130) == 1)
			{
				_chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(56));
			}
		}

		public void GenerateLapis(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 80) == 1)
			{
				_chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(21));
			}
		}
	}
}