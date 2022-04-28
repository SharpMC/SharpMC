using System.Numerics;
using SharpMC.API.Enums;

namespace SharpMC.Items.Usables
{
    public abstract class UsableItem : Item, IUsableItem
    {
        public bool IsUsable { get; protected set; }

        public abstract void UseItem(IItemLevel world, ILevelPlayer player,
            Vector3 coordinates, BlockFace face);

        protected Vector3 GetNewCoordinatesFromFace(Vector3 target, BlockFace face)
        {
            switch (face)
            {
                case BlockFace.NegativeY:
                    target.Y--;
                    break;
                case BlockFace.PositiveY:
                    target.Y++;
                    break;
                case BlockFace.NegativeZ:
                    target.Z--;
                    break;
                case BlockFace.PositiveZ:
                    target.Z++;
                    break;
                case BlockFace.NegativeX:
                    target.X--;
                    break;
                case BlockFace.PositiveX:
                    target.X++;
                    break;
            }
            return target;
        }
    }
}