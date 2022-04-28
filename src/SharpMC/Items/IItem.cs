using System.Numerics;
using SharpMC.API.Entities;
using SharpMC.API.Enums;

namespace SharpMC.Items
{
    public interface IItem
    {
        bool IsUsable { get; }

        void UseItem(IItemLevel world, IPlayer player, Vector3 pos, BlockFace face);
    }
}