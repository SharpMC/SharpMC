// Distributed under the MIT license
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
using SharpMC.Core.Worlds.Standard.Decorators;

namespace SharpMC.Core.Worlds.Nether
{
	public class NetherLavaDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					//Check for temperature.
					for (var y = 0; y < NetherWorldProvider.WaterLevel; y++)
					{
						//Lake generation
						if (y < NetherWorldProvider.WaterLevel)
						{
							if (chunk.GetBlock(x, y + 1, z) == 0)
							{
								if (y < NetherWorldProvider.WaterLevel - 3)
								{
									chunk.SetBlock(x, y + 1, z, new BlockStationaryLava()); //Water
								}
							}
						}
					}
				}
			}
		}
	}
}