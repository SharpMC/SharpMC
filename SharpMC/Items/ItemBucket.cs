using SharpMC.Entity;
using SharpMC.Enums;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Items
{
	public class ItemBucket : Item
	{
		internal ItemBucket() : base(325, 0)
		{
			IsUsable = true;
		}

		public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			var bl = world.GetBlock(blockCoordinates);

			if (bl.Id == 8)
			{
				//Water
				int slot = player.Inventory.CurrentSlot + 36;
				player.Inventory.SetSlot(slot, 326, 0, 1);
				bl.BreakBlock(world);
			}

			if (bl.Id == 10)
			{
				int slot = player.Inventory.CurrentSlot + 36;
				player.Inventory.SetSlot(slot, 327, 0, 1);
				bl.BreakBlock(world);
			}
		}
	}
}
