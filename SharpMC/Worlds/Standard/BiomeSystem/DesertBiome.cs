#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	using SharpMC.Blocks;
	using SharpMC.Worlds.Standard.Structures;

	internal class DesertBiome : BiomeBase
	{
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		public override byte MinecraftBiomeId
		{
			get
			{
				return 2;
			}
		}

		public override float Temperature
		{
			get
			{
				return 2.0f;
			}
		}

		public override Block TopBlock
		{
			get
			{
				return new Block(12);
			}
		}

		public override Block Filling
		{
			get
			{
				return new BlockSandStone();
			}
		}

		public override Structure[] TreeStructures
		{
			get
			{
				return new Structure[] { new CactusStructure() };
			}
		}

		public override int MaxTrees
		{
			get
			{
				return 4;
			}
		}
	}
}