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
using SharpMC.Worlds.Standard.Decorators;
using SharpMC.Worlds.Standard.Structures;

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	public class BiomeBase : IBiome
	{
		public virtual double BaseHeight
		{
			get { return 52.0; }
		}

		public virtual int Id
		{
			get { return 0; }
		}

		public virtual byte MinecraftBiomeId
		{
			get { return 0; }
		}

		public virtual int MaxTrees
		{
			get { return 10; }
		}

		public virtual int MinTrees
		{
			get { return 0; }
		}

		public virtual Structure[] TreeStructures
		{
			get { return new Structure[] {new OakTree()}; }
		}

		public virtual ChunkDecorator[] Decorators
		{
			get { return new ChunkDecorator[] {new TreeDecorator()}; }
		}

		public virtual float Temperature
		{
			get { return 0.0f; }
		}

		public virtual Block TopBlock
		{
			get { return new BlockGrass(); }
		}

		public virtual Block Filling
		{
			get { return new Block(3); }
		}
	}
}