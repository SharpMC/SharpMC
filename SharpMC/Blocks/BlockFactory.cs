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
			if (id == 16) return new BlockCoalOre();
			if (id == 21) return new BlockLapisLazuliOre();
			if (id == 56) return new BlockDiamondOre();
			if (id == 10) return new BlockFlowingLava();
			if (id == 8) return new BlockFlowingWater();
			if (id == 11) return new BlockStationaryLava();
			if (id == 9) return new BlockStationaryWater();
			if (id == 31 && metadata == 1) return new BlockTallGrass();
			return new Block(id);
		}

		public static Block GetBlockById(ushort id)
		{
			return GetBlockById(id, 0);
		}
	}
}