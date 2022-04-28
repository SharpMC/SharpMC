using System.Numerics;
using SharpMC.API.Enums;
using SharpMC.Blocks;

namespace SharpMC.Items.Usables
{
    internal class RedstoneItem : UsableItem
    {
        internal RedstoneItem()
        {
            IsUsable = true;
        }

        public override void UseItem(IItemLevel world, ILevelPlayer player,
            Vector3 coordinates, BlockFace face)
        {
            coordinates = GetNewCoordinatesFromFace(coordinates, face);
            var d = KnownBlocks.RedstoneWire;
            world.SetBlock(d, coordinates, true, true);
        }
    }
}