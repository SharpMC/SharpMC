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

namespace SharpMC.Blocks
{
	internal class BlockFactory
	{
		public static Block GetBlockById(ushort id, short metadata)
		{
			if (id == 46)
			{
				return new BlockTNT();
			}

			if (id == 0)
			{
				return new BlockAir();
			}

			if (id == 51)
			{
				return new BlockFire();
			}

			if (id == 7)
			{
				return new BlockBedrock();
			}

			if (id == 3)
			{
				return new BlockDirt();
			}

			if (id == 2)
			{
				return new BlockGrass();
			}

			if (id == 16)
			{
				return new BlockCoalOre();
			}

			if (id == 21)
			{
				return new BlockLapisLazuliOre();
			}

			if (id == 56)
			{
				return new BlockDiamondOre();
			}

			if (id == 10)
			{
				return new BlockFlowingLava();
			}

			if (id == 8)
			{
				return new BlockFlowingWater();
			}

			if (id == 11)
			{
				return new BlockStationaryLava();
			}

			if (id == 9)
			{
				return new BlockStationaryWater();
			}

			if (id == 31 && metadata == 1)
			{
				return new BlockTallGrass();
			}

			if (id == 5 && metadata == 0)
			{
				return new OakWoodPlank();
			}

			if (id == 64)
			{
				return new BlockOakDoor();
			}

			return new Block(id);
		}

		public static Block GetBlockById(ushort id)
		{
			return GetBlockById(id, 0);
		}
	}
}