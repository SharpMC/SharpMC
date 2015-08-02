using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Core.Entity;
using SharpMC.Core.Enums;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Blocks
{
	public class StairsBlock : Block
	{
		internal StairsBlock(ushort id) : base(id)
		{
			FuelEfficiency = 15;
		}

		public override bool PlaceBlock(Level world, Player player, Vector3 blockCoordinates, BlockFace face, Vector3 mouseLocation)
		{
			byte direction = player.GetDirection();
			byte upper = (byte)((mouseLocation.Y >= 8 && face != BlockFace.PositiveY) || face == BlockFace.NegativeY ? 0x04 : 0x00);
			switch (direction)
			{
				case 0:
					Metadata = (byte)(0 | upper);
					break;
				case 1:
					Metadata = (byte)(2 | upper);
					break;
				case 2:
					Metadata = (byte)(1 | upper);
					break;
				case 3:
					Metadata = (byte)(3 | upper);
					break;
			}
			world.SetBlock(this);
			return true;
		}
	}
}
