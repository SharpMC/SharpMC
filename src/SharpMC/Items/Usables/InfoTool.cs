using System.Collections;
using System.Numerics;
using Microsoft.Extensions.Logging;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.Blocks;
using SharpMC.Logging;

namespace SharpMC.Items.Usables
{
    public class InfoTool : UsableItem
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(InfoTool));

        internal InfoTool()
        {
            IsUsable = true;
        }

        public override void UseItem(IItemLevel world, IPlayer player,
            Vector3 coordinates, BlockFace face)
        {
            var block = world.GetBlock(coordinates);
            if (block != KnownBlocks.Air)
            {
                var b = new BitArray(new[] {(byte) block.DefaultState});
                Log.LogInformation("------------------------------------");
                Log.LogInformation($"Block: {block}");
                Log.LogInformation("------------------------------------");
                for (var i = 0; i < b.Count; i++)
                {
                    Log.LogInformation($"Bit {i}: {b[i]}");
                }
                Log.LogInformation("------------------------------------\n\n");
                player.SendChat("Info tool used, Metadata written to chat!", ChatColor.Gold);
            }
        }
    }
}