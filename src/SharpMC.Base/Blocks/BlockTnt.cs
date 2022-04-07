using System.Numerics;
using SharpMC.Core.Entity;
using SharpMC.Enums;
using SharpMC.Util;
using SharpMC.World;

namespace SharpMC.Blocks
{
    internal class BlockTnt : RedstoneDevice
    {
        internal BlockTnt() : base(46)
        {
            IsSolid = true;
            IsReplacible = false;
            IsUsable = true;
        }

        public override void BreakBlock(Level world)
        {
            world.SetBlock(new BlockAir { Coordinates = Coordinates });
        }

        public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
        {
            if (player.Inventory.GetItemInHand(1).Id == 259 || player.Inventory.GetItemInHand(0).Id == 259)
            {
                this.BreakBlock(world);
                new PrimedTNTEntity(world) { KnownPosition = Coordinates.ToPlayerLocation() }.SpawnEntity();
            }
        }

        public override void RedstoneTick(Level world)
        {
            new PrimedTNTEntity(world) { KnownPosition = Coordinates.ToPlayerLocation() }.SpawnEntity();
        }
    }
}