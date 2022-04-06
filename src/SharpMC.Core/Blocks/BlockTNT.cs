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

using SharpMC.Core.Entity;
using SharpMC.Core.Enums;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Blocks
{
	internal class BlockTnt : RedstoneDevice
	{
		internal BlockTnt() : base(46)
		{
			IsSolid = true;
			IsReplacible = false;
			IsUsable = true;
		}

		public override void BreakBlock(Level world)
		{
			world.SetBlock(new BlockAir {Coordinates = Coordinates});
		}

		public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			if (player.Inventory.GetItemInHand(1).Id == 259 || player.Inventory.GetItemInHand(0).Id == 259)
			{
				this.BreakBlock(world);
				new PrimedTNTEntity(world){KnownPosition = Coordinates.ToPlayerLocation()}.SpawnEntity();
			}
		}

		public override void RedstoneTick(Level world)
		{
			new PrimedTNTEntity(world) { KnownPosition = Coordinates.ToPlayerLocation() }.SpawnEntity();
		}
	}
}