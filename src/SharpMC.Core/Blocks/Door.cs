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

using SharpMC.Core.Entity;
using SharpMC.Core.Enums;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Blocks
{
	public class Door : Block
	{
		internal Door(ushort id) : base(id)
		{
		}

		public override bool PlaceBlock(Level world, Player player, Vector3 blockCoordinates, BlockFace face, Vector3 mouseLocation)
		{
			var direction = player.GetDirection();
			var coordinates = GetNewCoordinatesFromFace(blockCoordinates, face);

			Block block = new Door(Id);
			block.Coordinates = coordinates;
			block.Metadata = direction;

			var x = (int) blockCoordinates.X;
			var y = (int) blockCoordinates.Y;
			var z = (int) blockCoordinates.Z;

			var xd = 0;
			var zd = 0;

			if (direction == 0) zd = 1;
			if (direction == 1) xd = -1;
			if (direction == 2) zd = -1;
			if (direction == 3) xd = 1;

			var i1 = (world.GetBlock(new Vector3(x - xd, y, z - zd)).IsSolid ? 1 : 0) +
			         (world.GetBlock(new Vector3(x - xd, y + 1, z - zd)).IsSolid ? 1 : 0);
			var j1 = (world.GetBlock(new Vector3(x + xd, y, z + zd)).IsSolid ? 1 : 0) +
			         (world.GetBlock(new Vector3(x + xd, y + 1, z + zd)).IsSolid ? 1 : 0);
			var flag = world.GetBlock(new Vector3(x - xd, y, z - zd)).Id == block.Id ||
			           world.GetBlock(new Vector3(x - xd, y + 1, z - zd)).Id == block.Id;
			var flag1 = world.GetBlock(new Vector3(x + xd, y, z + zd)).Id == block.Id ||
			            world.GetBlock(new Vector3(x + xd, y + 1, z + zd)).Id == block.Id;
			var flag2 = false;

			if (flag && !flag1)
			{
				flag2 = true;
			}
			else if (j1 > i1)
			{
				flag2 = true;
			}

			var c2 = coordinates;
			c2.Y++;

			Block blockUpper = new Door(Id);
			blockUpper.Coordinates = c2;
			blockUpper.Metadata = (byte) (0x08 | (flag2 ? 1 : 0));

			world.SetBlock(block);
			world.SetBlock(blockUpper);

			return true;
		}
	}
}