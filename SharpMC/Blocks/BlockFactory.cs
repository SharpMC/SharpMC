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
			if (id == 3) return new BlockDirt();
			if (id == 2) return new BlockGrass();
			return new Block(id);
		}

		public static Block GetBlockById(ushort id)
		{
			return GetBlockById(id, 0);
		}
	}
}