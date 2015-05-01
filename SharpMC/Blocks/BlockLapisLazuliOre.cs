using SharpMC.Items;

namespace SharpMC.Blocks
{
	public class BlockLapisLazuliOre : Block
	{
		internal BlockLapisLazuliOre() : base(21)
		{
			Drops = new ItemLapisLazuli();
		}
	}
}
