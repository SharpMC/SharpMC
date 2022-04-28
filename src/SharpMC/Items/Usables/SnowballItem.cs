using System.Numerics;
using SharpMC.API.Enums;

namespace SharpMC.Items.Usables
{
    internal class SnowballItem : UsableItem
    {
        public SnowballItem()
        {
            IsUsable = true;
        }

        public override void UseItem(IItemLevel world, ILevelPlayer player,
            Vector3 coordinates, BlockFace face)
        {
            // TODO
            // new SnowballEntity(player, world).SpawnEntity();
        }
    }
}