using SharpMC.Core.Utils;
using SharpMC.Items;

namespace SharpMC.Blocks
{
    public class BlockTallGrass : Block
    {
        internal BlockTallGrass() : base(31)
        {
            Metadata = 1;
            IsSolid = false;
            Drops = new[] { new ItemWheatSeeds().GetItemStack() };
        }
    }
}