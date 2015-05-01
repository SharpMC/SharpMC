using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Blocks;
using SharpMC.Entity;
using SharpMC.Enums;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Items
{
	public class ItemWaterBucket : Item
	{
		internal ItemWaterBucket() : base(326, 0)
		{
			IsUsable = true;
			MaxStackSize = 1;
		}

		public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			int slot = player.Inventory.CurrentSlot + 36;
			player.Inventory.SetSlot(slot, 325, 0, 1); //'Empty' the bucket.
			world.SetBlock(new BlockFlowingWater() {Coordinates = blockCoordinates}, true, true); //Place the water
			world.GetBlock(blockCoordinates).OnTick(world); //Update the water
		}
	}
}
