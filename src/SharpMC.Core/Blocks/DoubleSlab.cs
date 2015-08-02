using SharpMC.Core.Utils;

namespace SharpMC.Core.Blocks
{
	public class DoubleSlab : Block
	{
		internal DoubleSlab(byte metadata) : base(43)
		{
			Metadata = metadata;
			Drops = new ItemStack[] { new ItemStack(44, 2, metadata) };
		}
	}
}
