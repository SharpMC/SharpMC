using System.Numerics;
using SharpMC.Core.Entity;
using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Items
{
	public class ItemSnowball : Item
	{
		public ItemSnowball() : base(332, 0)
		{
			IsUsable = true;
		}

		public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			new SnowballEntity(player, world).SpawnEntity();
		}
	}
}