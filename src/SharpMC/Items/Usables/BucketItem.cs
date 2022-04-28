using System.Numerics;
using SharpMC.API.Enums;
using SharpMC.Blocks;
using static SharpMC.Items.KnownItems;

namespace SharpMC.Items.Usables
{
    internal class BucketItem : UsableItem
    {
        internal BucketItem()
        {
            IsUsable = true;
        }

        public override void UseItem(IItemLevel world, ILevelPlayer player,
            Vector3 coordinates, BlockFace face)
        {
            coordinates = GetNewCoordinatesFromFace(coordinates, face);
            var bl = world.GetBlock(coordinates);

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

            if (bl.Id == 65535)
                return;

            if (bl.Id == WaterBucket.Id)
            {
                // Water
                player.Inventory.SetSlot(slot, 326, 0, 1);
                world.SetBlock(KnownBlocks.Air, coordinates, true, true);
            }

            if (bl.Id == LavaBucket.Id)
            {
                // Lava
                player.Inventory.SetSlot(slot, 327, 0, 1);
                world.SetBlock(KnownBlocks.Air, coordinates, true, true);
            }
        }
    }
}