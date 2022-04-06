using SharpMC.Core.Blocks;
using SharpMC.Core.Entity;
using SharpMC.Core.Enums;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Items
{
	public class ItemRedstone : Item
	{
		internal ItemRedstone() : base(331, 0)
		{
			IsUsable = true;
		}

		public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			blockCoordinates = GetNewCoordinatesFromFace(blockCoordinates, face);
			var d = new BlockRedstoneDust {Coordinates = blockCoordinates};
			//d.SetPowerLevel(new Random().Next(0,15));
			world.SetBlock(d, true, true);
		}
	}
}
