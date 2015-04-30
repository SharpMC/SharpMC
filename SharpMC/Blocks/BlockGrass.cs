namespace SharpMC.Blocks
{
	public class BlockGrass : Block
	{
		internal BlockGrass() : base(2)
		{
			Drops = new BlockDirt();
		}
	}
}