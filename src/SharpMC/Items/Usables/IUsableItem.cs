using System.Numerics;
using SharpMC.API.Enums;

namespace SharpMC.Items.Usables
{
    public interface IUsableItem : IItem
    {
        void UseItem(IItemLevel world, ILevelPlayer player, Vector3 pos, BlockFace face);
    }
}