namespace SharpMC.Blocks
{
	public static class BlockFactory
	{
		public static Block GetBlock(short id, byte metadata = 0)
		{
			if (id == 0) return new BlockAir();

			return new Block(id, metadata);
		}
	}
}
