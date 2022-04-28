using System.Numerics;
using SharpMC.API.Entities;
using SharpMC.API.Enums;

namespace SharpMC.Items.Usables
{
    public abstract class UsableItem : Item, IUsableItem
    {
        public bool IsUsable { get; protected set; }

        public abstract void UseItem(IItemLevel world, IPlayer player,
            Vector3 coordinates, BlockFace face);
    }
}