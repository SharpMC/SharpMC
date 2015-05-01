using SharpMC.Blocks;
using SharpMC.Entity;
using SharpMC.Enums;
using SharpMC.Utils;
using SharpMC.Worlds;

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
			int slot = player.Inventory.CurrentSlot + 36;
			player.Inventory.SetSlot(slot, 325, 0, 1); //'Empty' the bucket.
			world.SetBlock(new BlockFlowingLava() { Coordinates = blockCoordinates }, true, true); //Place the lava
			world.GetBlock(blockCoordinates).OnTick(world); //Update the lava
		}
	}
}
