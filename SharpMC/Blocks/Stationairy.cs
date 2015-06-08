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
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Blocks
{
	public class Stationairy : Block
	{
		internal Stationairy(ushort id) : base(id)
		{
			IsSolid = false;
			IsBuildable = false;
			IsReplacible = true;
		}

		public override void DoPhysics(Level level)
		{
			CheckForHarden(level, (int) Coordinates.X, (int) Coordinates.Y, (int) Coordinates.Z);

			if (level.GetBlock(Coordinates).Id == Id)
			{
				SetToFlowing(level);
			}
		}

		private void SetToFlowing(Level world)
		{
			var flowingBlock = BlockFactory.GetBlockById((byte) (Id - 1));
			flowingBlock.Metadata = Metadata;
			flowingBlock.Coordinates = Coordinates;
			world.SetBlock(flowingBlock, applyPhysics: false);
			world.ScheduleBlockTick(flowingBlock, 5);
		}

		private void CheckForHarden(Level world, int x, int y, int z)
		{
			var block = world.GetBlock(new Vector3(x, y, z));
			{
				var harden = false;
				if (block is BlockFlowingLava || block is BlockStationaryLava)
				{
					if (IsWater(world, x, y, z))
					{
						harden = true;
					}

					if (harden || IsWater(world, x, y, z + 1))
					{
						harden = true;
					}

					if (harden || IsWater(world, x - 1, y, z))
					{
						harden = true;
					}

					if (harden || IsWater(world, x + 1, y, z))
					{
						harden = true;
					}

					if (harden || IsWater(world, x, y + 1, z))
					{
						harden = true;
					}

					if (harden)
					{
						int meta = block.Metadata;

						if (meta == 0)
						{
							world.SetBlock(new BlockObsidian {Coordinates = new Vector3(x, y, z)});
						}
						else if (meta <= 4)
						{
							world.SetBlock(new BlockCobbleStone {Coordinates = new Vector3(x, y, z)});
						}
					}
				}
			}
		}

		private bool IsWater(Level world, int x, int y, int z)
		{
			var block = world.GetBlock(new Vector3(x, y, z - 1));
			return block is BlockFlowingWater || block is BlockStationaryWater;
		}
	}
}