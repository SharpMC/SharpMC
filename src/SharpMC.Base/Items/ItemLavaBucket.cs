using System.Numerics;
using SharpMC.Blocks;
using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Items
{
	public class ItemLavaBucket : Item
	{
		internal ItemLavaBucket() : base(327, 0)
		{
			IsUsable = true;
			MaxStackSize = 1;
		}

		public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			blockCoordinates = GetNewCoordinatesFromFace(blockCoordinates, face);
			//var slot = player.Inventory.CurrentSlot + 36;
			//player.Inventory.SetSlot(slot, 325, 0, 1); //'Empty' the bucket.
			var hand0 = player.Inventory.GetItemInHand(0);
			var hand1 = player.Inventory.GetItemInHand(1);
			if (hand0.Id == Id)
			{
				var slot = player.Inventory.CurrentSlot + 36;
				player.Inventory.SetSlot(slot, 325, 0, 1);
			}
			else if (hand1.Id == Id)
			{
				var slot = 45;
				player.Inventory.SetSlot(slot, 325, 0, 1);
			}
			world.SetBlock(new BlockFlowingLava {Coordinates = blockCoordinates}, true, true); //Place the lava
			//world.GetBlock(blockCoordinates).OnTick(world); //Update the lava
		}
	}
}