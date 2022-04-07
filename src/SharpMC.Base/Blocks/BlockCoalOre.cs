using SharpMC.Core.Utils;
using SharpMC.Items;

namespace SharpMC.Blocks
{
    public class BlockCoalOre : Block
    {
        internal BlockCoalOre() : base(16)
        {
            Drops = new[] { new ItemStack(new ItemCoal(), 1) };
            ;
        }
    }
}