using System.Numerics;
using Microsoft.Extensions.Logging;
using SharpMC.API.Enums;
using SharpMC.Blocks;
using SharpMC.Logging;

namespace SharpMC.Items.Usables
{
    internal class FlintAndSteelItem : UsableItem
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(FlintAndSteelItem));

        public FlintAndSteelItem()
        {
            IsUsable = false;
        }

        public override void UseItem(IItemLevel world, ILevelPlayer player,
            Vector3 coordinates, BlockFace face)
        {
            coordinates = GetNewCoordinatesFromFace(coordinates, face);
            var block = world.GetBlock(coordinates);
            Log.LogInformation($"Block: {block.Id}");
            if (block.Id != 46)
            {
                var affectedBlock = world.GetBlock(coordinates);
                if (affectedBlock.Id == 0)
                {
                    var fire = KnownBlocks.Fire;
                    world.SetBlock(fire, coordinates);
                }
            }
            else
            {
                // TODO
                /* new PrimedTNTEntity(world)
                {
                    KnownPosition = blockCoordinates.ToPlayerLocation()
                }.SpawnEntity(); */
            }
        }
    }
}