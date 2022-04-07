using System.Numerics;
using SharpMC.Blocks;
using SharpMC.Core;
using SharpMC.Core.Entity;
using SharpMC.Enums;
using SharpMC.Util;
using SharpMC.World;

namespace SharpMC.Items
{
    internal class ItemFlintAndSteel : Item
    {
        public ItemFlintAndSteel() : base(259, 0)
        {
            IsUsable = false;
        }

        public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
        {
            blockCoordinates = GetNewCoordinatesFromFace(blockCoordinates, face);
            var block = world.GetBlock(blockCoordinates);
            ConsoleFunctions.WriteInfoLine("Block: " + block.Id);
            if (block.Id != 46)
            {
                var affectedBlock = world.GetBlock(blockCoordinates);
                if (affectedBlock.Id == 0)
                {
                    var fire = new BlockFire
                    {
                        Coordinates = affectedBlock.Coordinates
                    };
                    world.SetBlock(fire);
                }
            }
            else
            {
                new PrimedTNTEntity(world) { KnownPosition = blockCoordinates.ToPlayerLocation() }.SpawnEntity();
            }
        }
    }
}