using System.Collections;
using System.Numerics;
using SharpMC.Blocks;
using SharpMC.Core;
using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Items
{
    public class InfoTool : Item
    {
        internal InfoTool() : base(286, 0)
        {
            IsUsable = true;
        }

        public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
        {
            var blockatpos = world.GetBlock(blockCoordinates);
            if (!(blockatpos is BlockAir))
            {
                var b = new BitArray(new[] { blockatpos.Metadata });
                ConsoleFunctions.WriteLine("\n\n");
                ConsoleFunctions.WriteInfoLine("------------------------------------");
                ConsoleFunctions.WriteInfoLine("Block: " + blockatpos);
                ConsoleFunctions.WriteInfoLine("------------------------------------");
                for (var i = 0; i < b.Count; i++)
                {
                    ConsoleFunctions.WriteInfoLine("Bit " + i + ": " + b[i]);
                }
                ConsoleFunctions.WriteInfoLine("------------------------------------\n\n");
                player.SendChat("Info tool used, Metadata written to chat!", ChatColor.Gold);
            }
        }
    }
}