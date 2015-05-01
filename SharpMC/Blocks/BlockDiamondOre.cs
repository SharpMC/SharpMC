using SharpMC.Items;

namespace SharpMC.Blocks
{
	public class BlockDiamondOre : Block
	{
		internal BlockDiamondOre() : base(56)
		{
			Drops = new ItemDiamond();
		}
	}
}
