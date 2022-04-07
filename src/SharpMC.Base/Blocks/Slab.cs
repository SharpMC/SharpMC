using System.Collections;
using System.Numerics;
using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Blocks
{
    public class Slab : Block
    {
        internal Slab(byte metadata) : base(44)
        {
            Metadata = metadata;
        }

        public override bool PlaceBlock(Level world, Player player, Vector3 blockCoordinates, BlockFace face, Vector3 mouseLocation)
        {
            var prevblock = world.GetBlock(Coordinates);
            if (prevblock.Id == Id && prevblock.Metadata == Metadata)
            {
                var ds = new DoubleSlab(Metadata) { Coordinates = Coordinates };
                world.SetBlock(ds);
            }
            else if (prevblock.Id == Id && prevblock.Metadata != Metadata)
            {
                if (player.Gamemode != Gamemode.Creative)
                {
                    player.Inventory.AddItem(Id, Metadata, 1);
                }
                return true;
            }
            else
            {
                var upper = mouseLocation.Y >= 8 && face != BlockFace.PositiveY || face == BlockFace.NegativeY;
                var b = new BitArray(new[] { Metadata })
                {
                    [3] = upper
                };
                Metadata = ConvertToByte(b);
                world.SetBlock(this);
            }
            return true;
        }
    }
}