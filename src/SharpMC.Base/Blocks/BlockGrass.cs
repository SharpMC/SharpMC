using SharpMC.Core.Utils;

namespace SharpMC.Blocks
{
    public class BlockGrass : Block
    {
        internal BlockGrass() : base(2)
        {
            Drops = new[] { new ItemStack(new BlockDirt(), 1) };
        }
    }
}