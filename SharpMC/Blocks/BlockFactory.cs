namespace SharpMC.Blocks
{
	internal class BlockFactory
	{
		public static Block GetBlockById(ushort id, short metadata)
		{
			if (id == 46) return new BlockTNT();
			if (id == 0) return new BlockAir();
			if (id == 51) return new BlockFire();
			if (id == 7) return new BlockBedrock();
			return new Block(id);
		}

		public static Block GetBlockById(ushort id)
		{
			return GetBlockById(id, 0);
		}
	}
}