using System.Numerics;
using SharpMC.Blocks;
using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Items
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
