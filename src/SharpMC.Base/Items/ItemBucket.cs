using System.Numerics;
using SharpMC.Blocks;
using SharpMC.Enums;
using SharpMC.World;

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
			blockCoordinates = GetNewCoordinatesFromFace(blockCoordinates, face);
			var bl = world.GetBlock(blockCoordinates);

			var slot = 0;
			var hand0 = player.Inventory.GetItemInHand(0);
			var hand1 = player.Inventory.GetItemInHand(1);
			if (hand0.Id == Id)
			{
				slot = player.Inventory.CurrentSlot + 36;
			}
			else if (hand1.Id == Id)
			{
				slot = 45;
			}

			//player.SendChat("Block: " + bl.Id, ChatColor.Bold);
			if (bl.Id == 65535) return;

			if (bl.Id == 8)
			{
				//Water
				player.Inventory.SetSlot(slot, 326, 0, 1);
				world.SetBlock(new BlockAir {Coordinates = blockCoordinates}, true, true);
			}

			if (bl.Id == 10)
			{
				//lava
				player.Inventory.SetSlot(slot, 327, 0, 1);
				world.SetBlock(new BlockAir { Coordinates = blockCoordinates }, true, true);
			}
		}
	}
}