using SharpMC.Core.Utils;

namespace SharpMC.Blocks
{
    internal class BlockStone : Block
    {
        internal BlockStone() : base(1)
        {
            Drops = new[] { new ItemStack(new BlockCobbleStone(), 1), };
        }
    }
}