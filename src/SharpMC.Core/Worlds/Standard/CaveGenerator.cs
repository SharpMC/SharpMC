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

namespace SharpMC.Core.Worlds.Standard
{
	internal class CaveGenerator
	{
		private readonly int _seedi;
		private GcRandom _gcRandom;

		public CaveGenerator(int seed)
		{
			_seedi = seed;
		}

		public void GenerateCave(ChunkColumn chunk)
		{
			_gcRandom = new GcRandom(chunk, _seedi);
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					for (var y = 50; y >= 6; y--)
					{
						if (_gcRandom.IsInCave(x, y, z))
						{
							chunk.SetBlock(x, y, z, new BlockAir());
						}
					}
				}
			}
		}
	}
}