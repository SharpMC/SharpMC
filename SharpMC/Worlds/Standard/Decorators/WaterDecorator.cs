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
	public class WaterDecorator : ChunkDecorator
	{
		public override void Decorate(ChunkColumn chunk, BiomeBase biome)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					//var blockbiome = chunk.BiomeId[x*16 + z];
					//if (BiomeManager.GetBiomeById(blockbiome).Temperature < 2.0f) //If the temperature is below 2.0F create lakes.
					//{
					//Check for temperature.
					for (var y = 0; y < StandardWorldProvider.WaterLevel; y++)
					{
						//Lake generation
						if (y < StandardWorldProvider.WaterLevel)
						{
							if (chunk.GetBlock(x, y, z) == 2 || chunk.GetBlock(x, y, z) == 3) //Grass or Dirt?
							{
								if (StandardWorldProvider.GetRandomNumber(1, 40) == 1 && y < StandardWorldProvider.WaterLevel - 4)
									chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(82)); //Clay
								else
								{
									chunk.SetBlock(x, y, z, new BlockSand()); //Sand
									chunk.BiomeId[x*16 + z] = 16; //Beach
								}
							}
							if (chunk.GetBlock(x, y + 1, z) == 0)
							{
								if (y < StandardWorldProvider.WaterLevel - 3)
								{
									chunk.SetBlock(x, y + 1, z, new BlockFlowingWater()); //Water
									chunk.BiomeId[x*16 + z] = 0; //Ocean
								}
							}
						}
					}
					//}
					/*else //Fill up with sand
					{
						for (var y = 0; y < ExperimentalV2Generator.WaterLevel; y++)
						{
							if (y < ExperimentalV2Generator.WaterLevel)
							{
								if (chunk.GetBlock(x, y, z) == 2 || chunk.GetBlock(x, y, z) == 3 || chunk.GetBlock(x,y,z) == 0) //Grass or Dirt?
								{
										chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(12)); //Sand
										//chunk.BiomeId[x * 16 + z] = 16; //Beach
								}
							}
						}
					}*/
				}
			}
		}
	}
}