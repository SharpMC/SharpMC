using System.Numerics;
using SharpMC.API.Enums;
using SharpMC.Blocks;

namespace SharpMC.Items.Usables
{
    internal class LavaBucketItem : UsableItem
    {
        internal LavaBucketItem()
        {
            IsUsable = true;
        }

        public override void UseItem(IItemLevel world, ILevelPlayer player,
            Vector3 coordinates, BlockFace face)
        {
            coordinates = GetNewCoordinatesFromFace(coordinates, face);
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
            // Place the lava
            world.SetBlock(KnownBlocks.Lava, coordinates, true, true);
        }
    }
}