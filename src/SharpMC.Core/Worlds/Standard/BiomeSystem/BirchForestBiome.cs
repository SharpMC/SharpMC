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

using SharpMC.Core.Worlds.Standard.Decorators;
using SharpMC.Core.Worlds.Standard.Structures;

namespace SharpMC.Core.Worlds.Standard.BiomeSystem
{
	internal class BirchForestBiome : BiomeBase
	{
		public override int Id
		{
			get { return 3; }
		}

		public override byte MinecraftBiomeId
		{
			get { return 27; }
		}

		public override ChunkDecorator[] Decorators
		{
			get { return new ChunkDecorator[] {new GrassDecorator(), new ForestDecorator()}; }
		}

		public override Structure[] TreeStructures
		{
			get { return new Structure[] {new BirchTree()}; }
		}

		public override float Temperature
		{
			get { return 0.7f; }
		}
	}
}